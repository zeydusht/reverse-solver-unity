using System;
using System.Collections;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public enum AdResult { Rewarded, Closed, Failed, NotReady }

    /* An ad network as the game sees it. iOS: AppLovin MAX (ReverseSolver.Platform.Max). Web: none (no ads, no
       buttons); &reklam=sahte: a fake one that is always ready, to try the placements in a browser. */
    public interface IAdNetwork
    {
        string Name { get; }
        bool Initialized { get; }
        /* Starts the SDK; MAX's terms and privacy flow asks the ATT question here (status: "authorized", "denied", …). */
        void Init(Action<string> attStatus);
        bool IsReady(bool rewarded);
        /* error: why it failed (Failed), else null. */
        void Show(bool rewarded, string placement, Action<AdResult, string> done);
        /* placement, network, revenue in USD (MAX OnAdRevenuePaidEvent). */
        event Action<string, string, double> Revenue;
        void ShowDebugger();
    }

    /* PM_81 §3: the game's one way to an ad. Every rule is in Core.AdRules and live/config.json; this keeps the device's
       counters, stops the game's clock and sound while an ad is up, and reports every outcome to the soft launch's
       events (SlTelemetry). A reward is only given from the network's "reward earned" callback; any failure leaves
       the game exactly as it was. */
    public static class Ads
    {
        public static IAdNetwork Network;             // null: no ads here
        public static bool Showing { get; private set; }
        public static Action<string, string, string, double?, string> Event;   // event, placement, network, revenue, value
        const string StateKey = "rs_ads";
        static AdState _state;
        static bool _revenueHooked;

        public static AdState State
        {
            get
            {
                if (_state == null) _state = AdState.Parse(Host.Current?.Store.Get(StateKey));
                return _state;
            }
        }

        public static void Save()
        {
            if (Host.Current == null) return;
            Host.Current.Store.Set(StateKey, State.ToJson());
            Host.Current.Store.Save();
        }

        public static string Today => DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        public static double Now => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

        public static bool Available => Network != null && Network.Initialized && Live.Config.AdsOn;

        /* Whether a rewarded button shows now (never a dead one). With report, a button the rules allow but whose ad
           is not loaded yet is reported once (ad_skipped_not_ready). */
        public static bool CanOffer(AdPlacement p, bool report = false)
        {
            if (Network == null || !Network.Initialized) return false;
            bool allowed = AdRules.ShowRewardButton(Live.Config, State, p, Today, true);
            if (!allowed) return false;
            bool ready = Network.IsReady(true);
            if (!ready && report) Event?.Invoke("ad_skipped_not_ready", AdPlacements.Name(p), Network.Name, null, null);
            return ready;
        }

        public static void ShowReward(AdPlacement p, Action<bool> rewarded)
        {
            if (Showing) return;   // PM_86 §6: a quick second tap does not start a second ad
            if (!CanOffer(p)) { Event?.Invoke("ad_skipped_not_ready", AdPlacements.Name(p), Network?.Name, null, null); rewarded(false); return; }
            Show(true, p, r =>
            {
                if (r == AdResult.Rewarded)
                {
                    AdRules.Rewarded(State, p, Today);
                    Save();
                    Event?.Invoke("ad_reward", AdPlacements.Name(p), Network.Name, null, null);
                    Haptics.Play(Haptics.Kind.Success);   // PM_86 §4: the reward
                }
                rewarded(r == AdResult.Rewarded);
            });
        }

        /* Every win counts towards the next interstitial (kept on the device). */
        public static void CountWin()
        {
            AdRules.Won(State);
            Save();
        }

        /* I1: after a win, on Next, before the corridor; `then` always runs (shown, skipped or failed). */
        public static void AfterWin(int wonLevel, bool firstSession, double sessionStart, Action then)
        {
            bool due = Network != null && Network.Initialized &&
                       AdRules.InterstitialDue(Live.Config, State, wonLevel, false, Now, firstSession, sessionStart);
            if (!due) { then(); return; }
            if (!Network.IsReady(false))
            {
                Event?.Invoke("ad_skipped_not_ready", AdPlacements.Name(AdPlacement.I1Interstitial), Network.Name, null, null);
                then();
                return;
            }
            Show(false, AdPlacement.I1Interstitial, r =>
            {
                if (r != AdResult.Failed) { AdRules.InterstitialShown(State, Now); Save(); }
                then();
            });
        }

        static void Show(bool rewarded, AdPlacement p, Action<AdResult> done)
        {
            HookRevenue();
            Showing = true;
            AudioListener.pause = true;
            try
            {
                Network.Show(rewarded, AdPlacements.Name(p), (r, error) =>
                {
                    Showing = false;
                    AudioListener.pause = false;
                    if (r == AdResult.Failed) Event?.Invoke("ad_failed", AdPlacements.Name(p), Network.Name, null, error);
                    done(r);
                });
            }
            catch (Exception e)
            {
                Showing = false;
                AudioListener.pause = false;
                Event?.Invoke("ad_failed", AdPlacements.Name(p), Network?.Name, null, e.Message);
                done(AdResult.Failed);
            }
        }

        static void HookRevenue()
        {
            if (_revenueHooked || Network == null) return;
            _revenueHooked = true;
            Network.Revenue += (placement, network, revenue) => Event?.Invoke("ad_shown", placement, network, revenue, null);
        }

        /* Settings: the version line tapped 5 times (the hidden test menu). */
        public static void Debugger() => Network?.ShowDebugger();
    }

    /* &reklam=sahte (web, testing): always ready; an ad is a dark screen for 1.5 s, then the reward. */
    public sealed class FakeAdNetwork : IAdNetwork
    {
        readonly MonoBehaviour _host;
        readonly Vector2 _screen;
        public FakeAdNetwork(MonoBehaviour host, Vector2 screen) { _host = host; _screen = screen; }
        public string Name => "fake";
        public bool Initialized { get; private set; }
        public event Action<string, string, double> Revenue;
        public void Init(Action<string> attStatus) { Initialized = true; attStatus?.Invoke("not_applicable"); }
        public bool IsReady(bool rewarded) => true;
        public void ShowDebugger() => Debug.Log("[ads] fake network: no debugger");

        public void Show(bool rewarded, string placement, Action<AdResult, string> done) => _host.StartCoroutine(Run(rewarded, placement, done));

        IEnumerator Run(bool rewarded, string placement, Action<AdResult, string> done)
        {
            var root = new GameObject("fake ad").transform;
            var cam = Camera.main;
            if (cam != null)
            {
                root.SetParent(cam.transform, false);
                float k = cam.orthographicSize / (_screen.y / 2);
                root.localScale = new Vector3(k, k, 1);
                root.localPosition = new Vector3(-_screen.x / 2 * k, _screen.y / 2 * k, 5);
            }
            ShapeView.Create(root, "shade", _screen * 1.2f, 0, 9000).Radius(0).Fill(Draw.Rgba(10, 10, 14, .96f)).transform.localPosition = Draw.W(_screen / 2);
            var t = TextView.Create(root, "label", (rewarded ? "Ödüllü reklam" : "Geçiş reklamı") + "\n(test)\n" + placement, 26, Vector4.one, true, order: 9001);
            t.transform.localPosition = Draw.W(_screen / 2);
            yield return new WaitForSecondsRealtime(1.5f);
            UnityEngine.Object.Destroy(root.gameObject);
            Revenue?.Invoke(placement, "fake", 0);
            done(rewarded ? AdResult.Rewarded : AdResult.Closed, null);
        }
    }
}
