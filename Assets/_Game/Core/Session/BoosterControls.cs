using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public enum BoosterButton { Ready, Locked, Out }

    /* The web game's booster bar logic (index.html BOOSTERS, bar click,
       askConfirm, showJoints, useWand/useHammer/useClock), without the DOM.
       Presentation draws what this says and forwards taps; every change to
       the game goes through GameSession commands.

         scissors, hammer   tap to arm (tap again to disarm), then tap a joint
                            or a piece; while armed, pieces cannot be dragged
         wand, clock        tap opens a confirmation; Confirm() uses it
         locked             tap shows "N. bölümde açılıyor"
         out of stock       tap does nothing */
    public sealed class BoosterControls
    {
        public const string ScissorsHint = "Kesmek istediğin birleşim noktasına dokun";
        public const string HammerHint = "Kırmak istediğin parçaya dokun";
        public const string WandFailed = "Değnek yeni bir dizilim bulamadı, hakkın harcanmadı";
        public const string ClockDone = "+20 saniye";

        readonly GameSession _s;

        public Booster? Armed { get; private set; }
        public Booster? Asking { get; private set; }
        /* The amber line above the bar; empty when there is nothing to say. */
        public string Prompt { get; private set; } = "";

        public BoosterControls(GameSession session) { _s = session; }

        public bool BlocksDrag => Armed.HasValue;

        public BoosterButton StateOf(Booster b) =>
            !_s.IsUnlocked(b) ? BoosterButton.Locked : _s.Stock(b) == 0 ? BoosterButton.Out : BoosterButton.Ready;

        public int UnlockLevel(Booster b) =>
            _s.Level.Unlock != null && _s.Level.Unlock.TryGetValue(b.Id(), out int n) ? n : 0;

        /* Web .bst.fresh: the booster this level introduces, until it is used. */
        public bool IsFresh(Booster b) =>
            _s.Level.BoosterIntro != null && _s.Level.BoosterIntro.Id == b.Id() && _s.Used(b) == 0;

        public static bool NeedsConfirm(Booster b) => b == Booster.Wand || b == Booster.Clock;

        /* (title, body) of the confirmation card, as in the web game. */
        public (string title, string body) Question(Booster b) => b == Booster.Wand
            ? ("Değnek kullanılsın mı?",
               "Tahtadaki tüm çıkıntılar yeniden dizilir. Resim değişmez ama hangi parçaların birbirine kilitli olduğu tamamen değişir." +
               $" Elinde {_s.Stock(b)} tane kaldı.")
            : ("Saat kullanılsın mı?", $"Geri sayıma 20 saniye eklenir. Elinde {_s.Stock(b)} tane kaldı.");

        public void Tap(Booster b)
        {
            if (_s.IsOver) return;
            switch (StateOf(b))
            {
                case BoosterButton.Locked:
                    Prompt = $"{UnlockLevel(b)}. bölümde açılıyor";
                    return;
                case BoosterButton.Out:
                    return;
            }
            if (NeedsConfirm(b))
            {
                Asking = b;
                return;
            }
            Armed = Armed == b ? (Booster?)null : b;
            Prompt = Armed == Booster.Scissors ? ScissorsHint : Armed == Booster.Hammer ? HammerHint : "";
        }

        public CommandResult Confirm()
        {
            if (!Asking.HasValue) return CommandResult.Ok;
            var b = Asking.Value;
            Asking = null;
            var r = b == Booster.Wand ? _s.UseWand() : _s.UseClock();
            if (r == CommandResult.Ok && b == Booster.Wand) Disarm();      // web spend(): armed = null
            if (r == CommandResult.Ok && b == Booster.Clock) Prompt = ClockDone;   // useClock keeps armed
            if (r == CommandResult.NoArrangement) Prompt = WandFailed;
            return r;
        }

        public void CancelAsk() => Asking = null;

        /* With the hammer armed, tapping a piece breaks it. */
        public CommandResult TapPiece(int piece)
        {
            if (Armed != Booster.Hammer) return CommandResult.Ok;
            var r = _s.UseHammer(piece);
            if (r == CommandResult.Ok) Disarm();
            return r;
        }

        /* Joints the scissors can cut now (web showJoints): vertical joints at
           (x, y) left of a cell, horizontal ones above it. */
        public List<(int x, int y, bool vertical)> Joints()
        {
            var list = new List<(int, int, bool)>();
            if (Armed != Booster.Scissors) return list;
            var l = _s.Level;
            for (int y = 0; y < l.Height; y++)
                for (int x = 0; x < l.Width; x++)
                {
                    if (x > 0 && _s.IsCuttable(x, y, true)) list.Add((x, y, true));
                    if (y > 0 && _s.IsCuttable(x, y, false)) list.Add((x, y, false));
                }
            return list;
        }

        public CommandResult TapJoint(int x, int y, bool vertical)
        {
            if (Armed != Booster.Scissors) return CommandResult.Ok;
            var r = _s.UseScissors(x, y, vertical);
            if (r == CommandResult.Ok) Disarm();
            return r;
        }

        public void Disarm()
        {
            Armed = null;
            Prompt = "";
        }

        public void ClearPrompt() => Prompt = "";
    }
}
