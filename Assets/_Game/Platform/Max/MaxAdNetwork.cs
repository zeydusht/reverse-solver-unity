using System;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Platform
{
    /* PM_81 §3: AppLovin MAX, the AppLovin network only (no mediation adapters), rewarded and interstitial, no banner.
       The keys come from Assets/Config/ads.json (IosBuild copies it into Resources for the iOS build); with the
       placeholders still in it nothing starts and no ad button ever shows. MAX's terms and privacy flow (set up by
       Editor/AdsSetup) shows the privacy link and the ATT question when the SDK starts, so the game starts it only
       after level 2 is won (PM: not on the first open). Ads load ahead and reload after each show or failure. */
    public sealed class MaxAdNetwork : IAdNetwork
    {
        readonly string _rewardedId, _interstitialId;
        bool _started;
        bool _earned;
        Action<AdResult, string> _rewardedDone, _interstitialDone;
        int _rewardedRetry, _interstitialRetry;

        public string Name => "AppLovin";
        public bool Initialized { get; private set; }
        public event Action<string, string, double> Revenue;

#if UNITY_IOS && !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            var keys = Keys();
            if (keys == null) { Debug.Log("[ads] MAX: no keys (placeholders), no ads"); return; }
            Ads.Network = new MaxAdNetwork(keys.Value.rewarded, keys.Value.interstitial);
        }
#endif

        public MaxAdNetwork(string rewardedId, string interstitialId)
        {
            _rewardedId = rewardedId; _interstitialId = interstitialId;
        }

        /* The ad unit ids from Resources/ads.json, or null while they are placeholders. */
        public static (string rewarded, string interstitial)? Keys()
        {
            var t = Resources.Load<TextAsset>("ads");
            if (t == null) return null;
            try
            {
                var o = JsonReader.Parse(t.text);
                string R(string k) => o.TryGet(k, out var n) && n.Kind == JsonKind.String ? n.AsString : "";
                string sdk = R("sdk_key"), rew = R("ios_rewarded"), inter = R("ios_interstitial");
                if (Placeholder(sdk) || Placeholder(rew) || Placeholder(inter)) return null;
                return (rew, inter);
            }
            catch (JsonException) { return null; }
        }

        public static bool Placeholder(string s) => string.IsNullOrEmpty(s) || s.StartsWith("BURAYA");

        public void Init(Action<string> attStatus)
        {
            if (_started) return;
            _started = true;
            MaxSdkCallbacks.OnSdkInitializedEvent += cfg =>
            {
                Initialized = true;
                attStatus?.Invoke(cfg.AppTrackingStatus.ToString().ToLowerInvariant());
                HookRewarded();
                HookInterstitial();
                MaxSdk.LoadRewardedAd(_rewardedId);
                MaxSdk.LoadInterstitial(_interstitialId);
            };
            MaxSdk.InitializeSdk();
        }

        public bool IsReady(bool rewarded) =>
            Initialized && (rewarded ? MaxSdk.IsRewardedAdReady(_rewardedId) : MaxSdk.IsInterstitialReady(_interstitialId));

        public void Show(bool rewarded, string placement, Action<AdResult, string> done)
        {
            if (!IsReady(rewarded)) { done(AdResult.NotReady, null); return; }
            if (rewarded)
            {
                _earned = false;
                _rewardedDone = done;
                MaxSdk.ShowRewardedAd(_rewardedId, placement);
            }
            else
            {
                _interstitialDone = done;
                MaxSdk.ShowInterstitial(_interstitialId, placement);
            }
        }

        public void ShowDebugger()
        {
            if (Initialized) MaxSdk.ShowMediationDebugger();
        }

        void HookRewarded()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (id, info) => _rewardedRetry = 0;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (id, error) => Retry(true);
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (id, reward, info) => _earned = true;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (id, info) => Revenue?.Invoke(info.Placement, info.NetworkName, info.Revenue);
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (id, error, info) =>
            {
                Finish(ref _rewardedDone, AdResult.Failed, error?.Message);
                MaxSdk.LoadRewardedAd(_rewardedId);
            };
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (id, info) =>
            {
                // the reward only from MAX's "reward received" (never from a mere close)
                Finish(ref _rewardedDone, _earned ? AdResult.Rewarded : AdResult.Closed, null);
                MaxSdk.LoadRewardedAd(_rewardedId);
            };
        }

        void HookInterstitial()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (id, info) => _interstitialRetry = 0;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (id, error) => Retry(false);
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (id, info) => Revenue?.Invoke(info.Placement, info.NetworkName, info.Revenue);
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (id, error, info) =>
            {
                Finish(ref _interstitialDone, AdResult.Failed, error?.Message);
                MaxSdk.LoadInterstitial(_interstitialId);
            };
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (id, info) =>
            {
                Finish(ref _interstitialDone, AdResult.Closed, null);
                MaxSdk.LoadInterstitial(_interstitialId);
            };
        }

        static void Finish(ref Action<AdResult, string> slot, AdResult r, string error)
        {
            var d = slot; slot = null;
            d?.Invoke(r, error);
        }

        /* Exponential back-off (2, 4, 8 … 64 s), as AppLovin recommends. */
        void Retry(bool rewarded)
        {
            int n = rewarded ? ++_rewardedRetry : ++_interstitialRetry;
            float wait = Mathf.Pow(2, Mathf.Min(6, n));
            Delay.Run(wait, () => { if (rewarded) MaxSdk.LoadRewardedAd(_rewardedId); else MaxSdk.LoadInterstitial(_interstitialId); });
        }

        sealed class Delay : MonoBehaviour
        {
            static Delay _i;
            public static void Run(float seconds, Action a)
            {
                if (_i == null) { var go = new GameObject("RSAdsDelay"); DontDestroyOnLoad(go); _i = go.AddComponent<Delay>(); }
                _i.StartCoroutine(After(seconds, a));
            }
            static System.Collections.IEnumerator After(float s, Action a) { yield return new WaitForSecondsRealtime(s); a(); }
        }
    }
}
