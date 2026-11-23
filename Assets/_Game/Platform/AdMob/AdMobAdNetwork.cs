using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using ReverseSolver.Presentation;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Platform
{
    /* PM_101: Google AdMob (the Google Mobile Ads Unity plugin), AdMob alone (no mediation yet), rewarded and
       interstitial, no banner; AppLovin MAX before it (PM_81). The ids: Resources/ads.json (IosBuild writes it for the
       build, AdKeys picks the set: the store build's real ids, Google's test ids in a test build and on a test device).
       Started after level 2 (GameRoot.AskAds81), in Google's order:
         1. UMP (User Messaging Platform): the consent form where the law asks for one (the EU, the UK); elsewhere none
         2. the ATT question (Plugins/iOS/RSAtt.mm), if not asked yet; its answer goes to the telemetry (att_status)
         3. the SDK, then the two ads load; each reloads after a show or a failure (2, 4 ... 64 s back-off)
       Ads are only requested when UMP allows it (CanRequestAds). The reward comes only from the rewarded ad's own
       callback; a close alone gives nothing. Revenue: OnAdPaid (micros / 1e6, in the account's currency, USD). */
    public sealed class AdMobAdNetwork : IAdNetwork
    {
        readonly AdKeys.Set _keys;
        bool _started, _earned;
        RewardedAd _rewarded;
        InterstitialAd _interstitial;
        Action<AdResult, string> _rewardedDone, _interstitialDone;
        string _rewardedPlacement, _interstitialPlacement;
        int _rewardedRetry, _interstitialRetry;
        string _att = "not_asked", _consent = "-";

        public string Name => "AdMob";
        public bool Initialized { get; private set; }
        public event Action<string, string, double> Revenue;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] static extern int RS_AttStatus();
        [DllImport("__Internal")] static extern void RS_AttRequest();
        [DllImport("__Internal")] static extern int RS_AttAnswered();
        [DllImport("__Internal")] static extern string RS_Idfa();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            var t = Resources.Load<TextAsset>("ads");
            var keys = AdKeys.Choose(t != null ? t.text : null, Debug.isDebugBuild, AdKeys.TestDevice);
            if (keys == null) { Debug.Log("[ads] AdMob: Resources/ads.json yok ya da okunamadı, reklam yok"); return; }
            Ads.Network = new AdMobAdNetwork(keys);
            Debug.Log($"[ads] AdMob: {(keys.Test ? "Google test kimlikleri" : "gerçek kimlikler")} (build {keys.Build}, test cihazı {AdKeys.TestDevice})");
        }
#else
        static int RS_AttStatus() => 3;
        static void RS_AttRequest() { }
        static int RS_AttAnswered() => 1;
        static string RS_Idfa() => "";
#endif

        public AdMobAdNetwork(AdKeys.Set keys) { _keys = keys; Main.Ensure(); }

        static string AttName(int s) => s == 0 ? "not_determined" : s == 1 ? "restricted" : s == 2 ? "denied" : "authorized";

        public void Init(Action<string> attStatus)
        {
            if (_started) return;
            _started = true;
            var request = new ConsentRequestParameters { TagForUnderAgeOfConsent = false };
            if (_keys.TestDevices.Length > 0) request.ConsentDebugSettings = new ConsentDebugSettings { TestDeviceHashedIds = new List<string>(_keys.TestDevices) };
            ConsentInformation.Update(request, updateError =>
                Main.Run(() =>
                {
                    if (updateError != null) Debug.Log($"[ads] UMP güncellenemedi: {updateError.Message}");
                    ConsentForm.LoadAndShowConsentFormIfRequired(formError => Main.Run(() =>
                    {
                        if (formError != null) Debug.Log($"[ads] UMP formu: {formError.Message}");
                        _consent = ConsentInformation.ConsentStatus.ToString();
                        AskAtt(status =>
                        {
                            _att = status;
                            attStatus?.Invoke(status);
                            StartSdk();
                        });
                    }));
                }));
        }

        /* After UMP, Google's order: the ATT question if it was not asked yet (UMP's own IDFA explainer may already
           have asked it). */
        static void AskAtt(Action<string> done)
        {
            int s = RS_AttStatus();
            if (s != 0) { done(AttName(s)); return; }
            RS_AttRequest();
            Main.Poll(() => RS_AttAnswered() != 0, () => done(AttName(RS_AttStatus())));
        }

        void StartSdk()
        {
            if (!ConsentInformation.CanRequestAds())
            {
                Debug.Log("[ads] UMP: reklam istenemez (onay yok), reklam yok");
                return;
            }
            var config = new RequestConfiguration { TestDeviceIds = new List<string>(_keys.TestDevices) };
            MobileAds.SetRequestConfiguration(config);
            MobileAds.Initialize(status => Main.Run(() =>
            {
                Initialized = true;
                Debug.Log($"[ads] AdMob hazır; ATT {_att}, onay {_consent}");
                LoadRewarded();
                LoadInterstitial();
            }));
        }

        public bool IsReady(bool rewarded) =>
            Initialized && (rewarded ? _rewarded != null && _rewarded.CanShowAd() : _interstitial != null && _interstitial.CanShowAd());

        public void Show(bool rewarded, string placement, Action<AdResult, string> done)
        {
            if (!IsReady(rewarded)) { done(AdResult.NotReady, null); return; }
            if (rewarded)
            {
                _earned = false; _rewardedDone = done; _rewardedPlacement = placement;
                _rewarded.Show(reward => Main.Run(() => _earned = true));
            }
            else
            {
                _interstitialDone = done; _interstitialPlacement = placement;
                _interstitial.Show();
            }
        }

        void LoadRewarded()
        {
            _rewarded?.Destroy(); _rewarded = null;
            RewardedAd.Load(_keys.Rewarded, new AdRequest(), (ad, error) => Main.Run(() =>
            {
                if (error != null || ad == null) { Debug.Log($"[ads] ödüllü yüklenemedi: {error?.GetMessage()}"); Retry(true); return; }
                _rewardedRetry = 0; _rewarded = ad;
                ad.OnAdPaid += v => Main.Run(() => Paid(_rewardedPlacement, ad.GetResponseInfo(), v));
                ad.OnAdFullScreenContentClosed += () => Main.Run(() =>
                {
                    // the reward only from the ad's own reward callback (never from a mere close)
                    Finish(ref _rewardedDone, _earned ? AdResult.Rewarded : AdResult.Closed, null);
                    LoadRewarded();
                });
                ad.OnAdFullScreenContentFailed += e => Main.Run(() => { Finish(ref _rewardedDone, AdResult.Failed, e?.GetMessage()); LoadRewarded(); });
            }));
        }

        void LoadInterstitial()
        {
            _interstitial?.Destroy(); _interstitial = null;
            InterstitialAd.Load(_keys.Interstitial, new AdRequest(), (ad, error) => Main.Run(() =>
            {
                if (error != null || ad == null) { Debug.Log($"[ads] geçiş yüklenemedi: {error?.GetMessage()}"); Retry(false); return; }
                _interstitialRetry = 0; _interstitial = ad;
                ad.OnAdPaid += v => Main.Run(() => Paid(_interstitialPlacement, ad.GetResponseInfo(), v));
                ad.OnAdFullScreenContentClosed += () => Main.Run(() => { Finish(ref _interstitialDone, AdResult.Closed, null); LoadInterstitial(); });
                ad.OnAdFullScreenContentFailed += e => Main.Run(() => { Finish(ref _interstitialDone, AdResult.Failed, e?.GetMessage()); LoadInterstitial(); });
            }));
        }

        /* ad_shown's revenue: AdMob's value in micros of the account's currency, in whole units. */
        void Paid(string placement, ResponseInfo info, AdValue v)
        {
            string source = null;
            try { source = info?.GetLoadedAdapterResponseInfo()?.AdSourceName; } catch (Exception) { }
            Revenue?.Invoke(placement, string.IsNullOrEmpty(source) ? "AdMob" : source, Micros(v.Value));
            if (v.CurrencyCode != "USD") Debug.Log($"[ads] gelir {v.CurrencyCode} cinsinden (USD değil): {Micros(v.Value)}");
        }

        public static double Micros(long micros) => micros / 1e6;

        static void Finish(ref Action<AdResult, string> slot, AdResult r, string error)
        {
            var d = slot; slot = null;
            d?.Invoke(r, error);
        }

        void Retry(bool rewarded)
        {
            int n = rewarded ? ++_rewardedRetry : ++_interstitialRetry;
            Main.After(Mathf.Pow(2, Mathf.Min(6, n)), () => { if (rewarded) LoadRewarded(); else LoadInterstitial(); });
        }

        public void ShowDebugger() => MobileAds.OpenAdInspector(e => Main.Run(() => { if (e != null) Debug.Log($"[ads] Ad Inspector: {e.GetMessage()}"); }));

        public void ShowPrivacyOptions() => ConsentForm.ShowPrivacyOptionsForm(e => Main.Run(() => { if (e != null) Debug.Log($"[ads] gizlilik seçenekleri: {e.Message}"); }));

        public string Describe()
        {
            string idfa = RS_Idfa();
            if (string.IsNullOrEmpty(idfa) || idfa.StartsWith("00000000")) idfa = "yok (izin verilmedi)";
            return $"AdMob · {(_keys.Test ? "test kimlikleri" : "gerçek kimlikler")} · ATT {_att}\nIDFA {idfa}";
        }

        /* The SDK's callbacks may come off Unity's thread: everything here runs on it. */
        sealed class Main : MonoBehaviour
        {
            static Main _i;
            static readonly List<Action> _queue = new List<Action>();
            static Main I { get { Ensure(); return _i; } }
            /* On Unity's thread (the network's constructor): the runner exists before any callback can come. */
            public static void Ensure() { if (_i == null) { var go = new GameObject("RSAdMob"); DontDestroyOnLoad(go); _i = go.AddComponent<Main>(); } }
            public static void Run(Action a) { lock (_queue) _queue.Add(a); }
            public static void After(float seconds, Action a) => I.StartCoroutine(Wait(seconds, a));
            public static void Poll(Func<bool> until, Action then) => I.StartCoroutine(PollRun(until, then));
            static System.Collections.IEnumerator Wait(float s, Action a) { yield return new WaitForSecondsRealtime(s); a(); }
            static System.Collections.IEnumerator PollRun(Func<bool> until, Action then)
            {
                while (!until()) yield return new WaitForSecondsRealtime(.2f);
                then();
            }
            void Update()
            {
                Action[] run;
                lock (_queue) { if (_queue.Count == 0) return; run = _queue.ToArray(); _queue.Clear(); }
                foreach (var a in run) a();
            }
        }
    }
}
