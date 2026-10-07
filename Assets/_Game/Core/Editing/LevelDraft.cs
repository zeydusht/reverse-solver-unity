using System;
using System.Collections.Generic;
using System.Text;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Editing
{
    /* How one side of one cell looks to the player. */
    public enum Side { Socket = -1, Flat = 0, Tab = 1 }

    /* Two cells that move as one piece. They need not touch: the web levels
       chain cells across the board (the cable is drawn between them). */
    public readonly struct Chain : IEquatable<Chain>
    {
        public readonly Cell A, B;
        public Chain(Cell a, Cell b) { A = a; B = b; }
        public bool Has(Cell c) => A.Equals(c) || B.Equals(c);
        public bool Equals(Chain o) => A.Equals(o.A) && B.Equals(o.B);
        public override bool Equals(object obj) => obj is Chain c && Equals(c);
        public override int GetHashCode() => A.GetHashCode() * 31 + B.GetHashCode();
        public override string ToString() => $"{A}-{B}";
    }

    public readonly struct EditResult
    {
        public readonly bool Ok;
        public readonly string Reason;     // Turkish, shown in the editor when refused
        EditResult(bool ok, string reason) { Ok = ok; Reason = reason; }
        public static readonly EditResult Done = new EditResult(true, null);
        public static EditResult No(string reason) => new EditResult(false, reason);
        public override string ToString() => Ok ? "ok" : Reason;
    }

    /* A piece shape: the profile of each of the four sides. Dropping one on a
       cell sets the joints it shares with its neighbours, so the neighbours
       take the matching shape by construction. */
    public sealed class PieceTemplate
    {
        public readonly string Name;
        public readonly Side U, R, D, L;

        public PieceTemplate(string name, Side u, Side r, Side d, Side l) { Name = name; U = u; R = r; D = d; L = l; }

        public Side Get(Dir d) => d == Dir.U ? U : d == Dir.R ? R : d == Dir.D ? D : L;

        /* A small palette that covers the shapes the web levels use most:
           a free square, one/two/four tabs, sockets, and mixes. */
        public static readonly PieceTemplate[] All =
        {
            new PieceTemplate("Düz kare",        Side.Flat,   Side.Flat,   Side.Flat,   Side.Flat),
            new PieceTemplate("Sağa çıkıntı",    Side.Flat,   Side.Tab,    Side.Flat,   Side.Flat),
            new PieceTemplate("Aşağı çıkıntı",   Side.Flat,   Side.Flat,   Side.Tab,    Side.Flat),
            new PieceTemplate("Yatay çıkıntı",   Side.Flat,   Side.Tab,    Side.Flat,   Side.Tab),
            new PieceTemplate("Dikey çıkıntı",   Side.Tab,    Side.Flat,   Side.Tab,    Side.Flat),
            new PieceTemplate("Dört çıkıntı",    Side.Tab,    Side.Tab,    Side.Tab,    Side.Tab),
            new PieceTemplate("Dört girinti",    Side.Socket, Side.Socket, Side.Socket, Side.Socket),
            new PieceTemplate("Çıkıntı-girinti", Side.Socket, Side.Tab,    Side.Socket, Side.Tab),
        };
    }

    /* An editable level. Every edit keeps it valid: joints are shared between
       neighbours (so two facing sides always fit), a chain is two distinct
       cells each in at most one chain, nails and bombs sit on pieces and never
       together, nails never on a chain (no web level has one; their badges
       would collide on the drawing).

       Pieces are not stored: they are derived in the web levels' order, chains
       first in chain order, then single cells row by row. Every web level is in
       that order, so loading one and writing it back gives the same file.
       Nails and bombs are kept on the piece's first cell (its anchor). */
    public sealed class LevelDraft
    {
        public const int MinSize = 2, MaxSize = 12, MaxCount = 99, MinTimer = 5, MaxTimer = 600;
        public static readonly string[] DefaultPalette = { "#ef5342", "#4a93c4" };
        public static readonly string[] BoosterIds = { "scissors", "wand", "hammer", "clock" };

        public const string UnsavedId = "YENI";
        public string Id;                  // null until a design is first saved
        public int Version = 1;
        public int Number;                 // `lv`; decides booster unlocks
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Timer { get; private set; } = 60;

        int[,] _v, _h;                     // same layout as LevelData.V / H
        readonly List<Chain> _chains = new List<Chain>();
        readonly Dictionary<Cell, int> _nails = new Dictionary<Cell, int>();
        readonly Dictionary<Cell, int> _bombs = new Dictionary<Cell, int>();
        readonly Dictionary<Dir, List<int>> _sealed = new Dictionary<Dir, List<int>>();
        int[] _colors;

        public List<string> Palette = new List<string>(DefaultPalette);
        public string Art = "Tasarım";
        public LevelIntro Intro, BoosterIntro;
        public Dictionary<string, int> Boosters = new Dictionary<string, int>();
        public Dictionary<string, int> Unlock = new Dictionary<string, int>();
        public int Load;
        /* Written from the solver on save, never typed in. It belongs to the
           gameplay it was found for: after any gameplay edit it is dropped
           (piece numbers may no longer even exist) until the next save. */
        public List<Move> Solution
        {
            get => _solutionKey == GameplayKey() ? _solution : new List<Move>();
            set { _solution = value ?? new List<Move>(); _solutionKey = GameplayKey(); }
        }
        List<Move> _solution = new List<Move>();
        string _solutionKey;

        LevelDraft() { }

        public static LevelDraft New(int width, int height)
        {
            var d = new LevelDraft();
            d.Width = Clamp(width, MinSize, MaxSize);
            d.Height = Clamp(height, MinSize, MaxSize);
            d._v = new int[d.Height, d.Width + 1];
            d._h = new int[d.Height + 1, d.Width];
            d._colors = new int[d.Width * d.Height];
            int[] unlockAt = { 3, 6, 9, 12 };                 // the web levels' unlock table
            for (int i = 0; i < BoosterIds.Length; i++)
            {
                d.Boosters[BoosterIds[i]] = 0;
                d.Unlock[BoosterIds[i]] = unlockAt[i];
            }
            return d;
        }

        /* Loads any level. Pieces of more than two cells exist in the format but
           in no level; the editor does not support them. */
        public static LevelDraft FromLevel(LevelData l)
        {
            var d = new LevelDraft
            {
                Id = l.Id, Version = l.Version, Number = l.Number,
                Width = l.Width, Height = l.Height, Timer = l.TimerSeconds,
                _v = (int[,])l.V.Clone(), _h = (int[,])l.H.Clone(),
                Palette = new List<string>(l.Palette.Length > 0 ? l.Palette : DefaultPalette),
                Art = l.Art, Intro = Copy(l.Intro), BoosterIntro = Copy(l.BoosterIntro),
                Boosters = new Dictionary<string, int>(), Unlock = new Dictionary<string, int>(),
                Load = l.Load
            };
            foreach (var kv in l.Boosters) d.Boosters[kv.Key] = kv.Value;
            foreach (var kv in l.Unlock) d.Unlock[kv.Key] = kv.Value;
            d._colors = l.Colors.Length == l.CellCount ? (int[])l.Colors.Clone() : new int[l.CellCount];

            foreach (var cells in l.Pieces)
            {
                if (cells.Length > 2) throw new LevelFormatException($"{l.Id}: pieces of {cells.Length} cells are not supported by the editor");
                if (cells.Length == 2) d._chains.Add(new Chain(cells[0], cells[1]));
            }
            foreach (var kv in l.Nails) d._nails[l.Pieces[kv.Key][0]] = kv.Value;
            foreach (var kv in l.Bombs) d._bombs[l.Pieces[kv.Key][0]] = kv.Value;
            foreach (var kv in l.Sealed)
                if (kv.Value.Length > 0)
                {
                    var lanes = new List<int>(kv.Value);
                    lanes.Sort();
                    d._sealed[kv.Key] = lanes;
                }
            d.Solution = new List<Move>(l.Solution);
            return d;
        }

        public LevelDraft Clone()
        {
            var d = new LevelDraft
            {
                Id = Id, Version = Version, Number = Number, Width = Width, Height = Height, Timer = Timer,
                _v = (int[,])_v.Clone(), _h = (int[,])_h.Clone(), _colors = (int[])_colors.Clone(),
                Palette = new List<string>(Palette), Art = Art,
                Intro = Copy(Intro), BoosterIntro = Copy(BoosterIntro),
                Boosters = new Dictionary<string, int>(Boosters), Unlock = new Dictionary<string, int>(Unlock),
                Load = Load,
                _solution = new List<Move>(_solution), _solutionKey = _solutionKey
            };
            d._chains.AddRange(_chains);
            foreach (var kv in _nails) d._nails[kv.Key] = kv.Value;
            foreach (var kv in _bombs) d._bombs[kv.Key] = kv.Value;
            foreach (var kv in _sealed) d._sealed[kv.Key] = new List<int>(kv.Value);
            return d;
        }

        static LevelIntro Copy(LevelIntro i) => i == null ? null
            : new LevelIntro { Id = i.Id, Icon = i.Icon, Title = i.Title, Body = i.Body, Tip = i.Tip };

        static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;

        public bool OnBoard(Cell c) => c.X >= 0 && c.X < Width && c.Y >= 0 && c.Y < Height;

        // ---- joints and sides -------------------------------------------------------

        /* VJoint(x, y): boundary left of (x, y), inner for x in 1..W-1.
           HJoint(x, y): boundary above (x, y), inner for y in 1..H-1. */
        public int VJoint(int x, int y) => _v[y, x];
        public int HJoint(int x, int y) => _h[y, x];

        public bool IsInnerV(int x, int y) => x >= 1 && x < Width && y >= 0 && y < Height;
        public bool IsInnerH(int x, int y) => y >= 1 && y < Height && x >= 0 && x < Width;

        public EditResult SetJoint(int x, int y, bool vertical, int value)
        {
            if (value < -1 || value > 1) return EditResult.No("Kenar değeri -1, 0 ya da 1 olmalı.");
            if (vertical ? !IsInnerV(x, y) : !IsInnerH(x, y)) return EditResult.No("Tahtanın dış kenarı her zaman düzdür.");
            if (vertical) _v[y, x] = value; else _h[y, x] = value;
            return EditResult.Done;
        }

        /* Flat -> tab one way -> tab the other way -> flat. */
        public EditResult CycleJoint(int x, int y, bool vertical)
        {
            if (vertical ? !IsInnerV(x, y) : !IsInnerH(x, y)) return EditResult.No("Tahtanın dış kenarı her zaman düzdür.");
            int j = vertical ? _v[y, x] : _h[y, x];
            return SetJoint(x, y, vertical, j == 0 ? 1 : j == 1 ? -1 : 0);
        }

        /* Same mapping as TravelRule.EdgeOf: on the left/top side a joint of -1
           is this cell's tab, on the right/bottom side +1 is. */
        public Side SideOf(Cell c, Dir side)
        {
            if (!JointFor(c, side, out int x, out int y, out bool vertical)) return Side.Flat;
            int v = vertical ? _v[y, x] : _h[y, x];
            if (v == 0) return Side.Flat;
            bool low = side == Dir.L || side == Dir.U;
            return (low ? v == -1 : v == 1) ? Side.Tab : Side.Socket;
        }

        public EditResult SetSide(Cell c, Dir side, Side s)
        {
            if (!OnBoard(c)) return EditResult.No("Hücre tahtanın dışında.");
            if (!JointFor(c, side, out int x, out int y, out bool vertical)) return EditResult.No("Tahtanın dış kenarı her zaman düzdür.");
            bool low = side == Dir.L || side == Dir.U;
            int v = s == Side.Flat ? 0 : (s == Side.Tab) == low ? -1 : 1;
            return SetJoint(x, y, vertical, v);
        }

        /* Sets the cell's inner sides to the template; outer sides stay flat. */
        public int ApplyTemplate(Cell c, PieceTemplate t)
        {
            if (!OnBoard(c)) return 0;
            int n = 0;
            foreach (var d in DirExt.All)
                if (SetSide(c, d, t.Get(d)).Ok) n++;
            return n;
        }

        bool JointFor(Cell c, Dir side, out int x, out int y, out bool vertical)
        {
            x = c.X; y = c.Y; vertical = side == Dir.L || side == Dir.R;
            if (side == Dir.R) x++;
            if (side == Dir.D) y++;
            return vertical ? IsInnerV(x, y) : IsInnerH(x, y);
        }

        // ---- chains -------------------------------------------------------------------

        public IReadOnlyList<Chain> Chains => _chains;

        public int ChainAt(Cell c)
        {
            for (int i = 0; i < _chains.Count; i++) if (_chains[i].Has(c)) return i;
            return -1;
        }

        public Cell AnchorOf(Cell c)
        {
            int i = ChainAt(c);
            return i < 0 ? c : _chains[i].A;
        }

        public EditResult AddChain(Cell a, Cell b)
        {
            if (!OnBoard(a) || !OnBoard(b)) return EditResult.No("Hücre tahtanın dışında.");
            if (a.Equals(b)) return EditResult.No("Zincir iki farklı hücreyi bağlar.");
            if (ChainAt(a) >= 0 || ChainAt(b) >= 0) return EditResult.No("Hücre zaten bir zincirde; önce o zinciri ayır.");
            if (_nails.ContainsKey(a) || _nails.ContainsKey(b)) return EditResult.No("Çivili parça zincirlenemez; önce çiviyi kaldır.");
            if (_bombs.ContainsKey(a) && _bombs.ContainsKey(b)) return EditResult.No("İki bombalı parça zincirlenemez; birinin bombasını kaldır.");
            if (_bombs.TryGetValue(b, out int fuse))                // the pair's bomb sits on its anchor
            {
                _bombs.Remove(b);
                _bombs[a] = fuse;
            }
            _chains.Add(new Chain(a, b));
            return EditResult.Done;
        }

        /* Splits the chain through c; a bomb stays on the first cell. */
        public EditResult RemoveChain(Cell c)
        {
            int i = ChainAt(c);
            if (i < 0) return EditResult.No("Bu hücrede zincir yok.");
            _chains.RemoveAt(i);
            return EditResult.Done;
        }

        // ---- nails and bombs ------------------------------------------------------------

        public int NailAt(Cell c) => _nails.TryGetValue(AnchorOf(c), out int n) ? n : 0;
        public int BombAt(Cell c) => _bombs.TryGetValue(AnchorOf(c), out int n) ? n : 0;

        /* count <= 0 removes the nail. */
        public EditResult SetNail(Cell c, int count)
        {
            if (!OnBoard(c)) return EditResult.No("Hücre tahtanın dışında.");
            var a = AnchorOf(c);
            if (count <= 0) { _nails.Remove(a); return EditResult.Done; }
            if (ChainAt(c) >= 0) return EditResult.No("Zincirli parçaya çivi konmaz.");
            if (_bombs.ContainsKey(a)) return EditResult.No("Bir parçada hem çivi hem bomba olamaz.");
            _nails[a] = Clamp(count, 1, MaxCount);
            return EditResult.Done;
        }

        /* fuse <= 0 removes the bomb. */
        public EditResult SetBomb(Cell c, int fuse)
        {
            if (!OnBoard(c)) return EditResult.No("Hücre tahtanın dışında.");
            var a = AnchorOf(c);
            if (fuse <= 0) { _bombs.Remove(a); return EditResult.Done; }
            if (_nails.ContainsKey(a)) return EditResult.No("Bir parçada hem çivi hem bomba olamaz.");
            _bombs[a] = Clamp(fuse, 1, MaxCount);
            return EditResult.Done;
        }

        /* Eraser: the cell's nail, bomb and chain. */
        public bool ClearCell(Cell c)
        {
            if (!OnBoard(c)) return false;
            var a = AnchorOf(c);
            bool any = _nails.Remove(a) | _bombs.Remove(a);
            if (ChainAt(c) >= 0) { RemoveChain(c); any = true; }
            return any;
        }

        // ---- walls --------------------------------------------------------------------

        /* A wall segment on a board side; lanes are columns for U/D, rows for L/R. */
        public int LaneCount(Dir side) => side.IsVertical() ? Width : Height;

        public bool IsSealed(Dir side, int lane) => _sealed.TryGetValue(side, out var l) && l.Contains(lane);

        public EditResult SetSealed(Dir side, int lane, bool on)
        {
            if (lane < 0 || lane >= LaneCount(side)) return EditResult.No("Bu kenarda böyle bir şerit yok.");
            if (!_sealed.TryGetValue(side, out var lanes)) _sealed[side] = lanes = new List<int>();
            if (on && !lanes.Contains(lane)) { lanes.Add(lane); lanes.Sort(); }
            if (!on) lanes.Remove(lane);
            if (lanes.Count == 0) _sealed.Remove(side);
            return EditResult.Done;
        }

        public EditResult ToggleSealed(Dir side, int lane) => SetSealed(side, lane, !IsSealed(side, lane));

        public IReadOnlyList<int> SealedLanes(Dir side) =>
            _sealed.TryGetValue(side, out var l) ? (IReadOnlyList<int>)l : Array.Empty<int>();

        // ---- colour, size, timer ----------------------------------------------------

        public int ColorAt(Cell c) => _colors[c.Y * Width + c.X];

        public EditResult SetColor(Cell c, int paletteIndex)
        {
            if (!OnBoard(c)) return EditResult.No("Hücre tahtanın dışında.");
            if (paletteIndex < 0 || paletteIndex >= Palette.Count) return EditResult.No("Palette böyle bir renk yok.");
            _colors[c.Y * Width + c.X] = paletteIndex;
            return EditResult.Done;
        }

        public EditResult SetTimer(int seconds)
        {
            Timer = Clamp(seconds, MinTimer, MaxTimer);
            return EditResult.Done;
        }

        public EditResult SetBooster(string id, int stock)
        {
            if (Array.IndexOf(BoosterIds, id) < 0) return EditResult.No("Böyle bir güçlendirici yok.");
            Boosters[id] = Clamp(stock, 0, MaxCount);
            return EditResult.Done;
        }

        /* Keeps what still fits: joints, colours, walls, and chains and obstacles
           whose cells are all on the new board. */
        public EditResult Resize(int width, int height)
        {
            width = Clamp(width, MinSize, MaxSize);
            height = Clamp(height, MinSize, MaxSize);
            var v = new int[height, width + 1];
            var h = new int[height + 1, width];
            var colors = new int[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    bool had = x < Width && y < Height;     // old inner joints only; old outer ones were 0
                    if (had) colors[y * width + x] = _colors[y * Width + x];
                    if (had && x >= 1) v[y, x] = _v[y, x];
                    if (had && y >= 1) h[y, x] = _h[y, x];
                }
            int oldW = Width, oldH = Height;
            Width = width; Height = height;
            _v = v; _h = h; _colors = colors;

            _chains.RemoveAll(c => !OnBoard(c.A) || !OnBoard(c.B));
            foreach (var k in new List<Cell>(_nails.Keys)) if (!OnBoard(k)) _nails.Remove(k);
            foreach (var k in new List<Cell>(_bombs.Keys)) if (!OnBoard(k)) _bombs.Remove(k);
            foreach (var side in new List<Dir>(_sealed.Keys))
            {
                _sealed[side].RemoveAll(lane => lane >= LaneCount(side));
                if (_sealed[side].Count == 0) _sealed.Remove(side);
            }
            return oldW == width && oldH == height ? EditResult.No("Boyut aynı.") : EditResult.Done;
        }

        // ---- derived ------------------------------------------------------------------

        public List<Cell[]> BuildPieces()
        {
            var pieces = new List<Cell[]>(Width * Height);
            var inChain = new HashSet<Cell>();
            foreach (var c in _chains)
            {
                pieces.Add(new[] { c.A, c.B });
                inChain.Add(c.A);
                inChain.Add(c.B);
            }
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var c = new Cell(x, y);
                    if (!inChain.Contains(c)) pieces.Add(new[] { c });
                }
            return pieces;
        }

        /* Everything that changes how the level plays. Two drafts with the same
           key play the same; the version only goes up when it changes. Colours,
           palette, art, intro cards and the stored solution are not in it. */
        public string GameplayKey()
        {
            var sb = new StringBuilder();
            sb.Append(Width).Append('x').Append(Height).Append(" t").Append(Timer).Append(" v");
            for (int y = 0; y < Height; y++) for (int x = 1; x < Width; x++) sb.Append(_v[y, x] + 1);
            sb.Append(" h");
            for (int y = 1; y < Height; y++) for (int x = 0; x < Width; x++) sb.Append(_h[y, x] + 1);
            var pieces = BuildPieces();
            for (int p = 0; p < pieces.Count; p++)
            {
                var a = pieces[p][0];
                if (pieces[p].Length > 1) sb.Append(" c").Append(a).Append(pieces[p][1]);
                if (_nails.TryGetValue(a, out int n)) sb.Append(" n").Append(a).Append(n);
                if (_bombs.TryGetValue(a, out int f)) sb.Append(" b").Append(a).Append(f);
            }
            foreach (var d in DirExt.All)
                if (_sealed.TryGetValue(d, out var lanes)) sb.Append(" s").Append(d).Append(string.Join(",", lanes));
            foreach (var id in BoosterIds)
            {
                sb.Append(' ').Append(id).Append(Boosters.TryGetValue(id, out int s) ? s : 0);
                sb.Append('/').Append(Unlock.TryGetValue(id, out int u) ? u : 0);
            }
            return sb.ToString();
        }

        public LevelData ToLevelData()
        {
            var pieces = BuildPieces();
            var l = new LevelData
            {
                Id = Id ?? UnsavedId, Version = Version, Number = Number,
                Width = Width, Height = Height, TimerSeconds = Timer,
                Pieces = pieces,
                V = (int[,])_v.Clone(), H = (int[,])_h.Clone(),
                Colors = (int[])_colors.Clone(), Palette = Palette.ToArray(),
                Art = Art, Intro = Copy(Intro), BoosterIntro = Copy(BoosterIntro),
                Boosters = new Dictionary<string, int>(Boosters), Unlock = new Dictionary<string, int>(Unlock),
                Load = Load, Solution = new List<Move>(Solution)
            };
            var nails = new Dictionary<int, int>();
            var bombs = new Dictionary<int, int>();
            for (int p = 0; p < pieces.Count; p++)
            {
                if (_nails.TryGetValue(pieces[p][0], out int n)) nails[p] = n;
                if (_bombs.TryGetValue(pieces[p][0], out int f)) bombs[p] = f;
            }
            l.Nails = nails;
            l.Bombs = bombs;
            var sealedLanes = new Dictionary<Dir, int[]>();
            foreach (var kv in _sealed) sealedLanes[kv.Key] = kv.Value.ToArray();
            l.Sealed = sealedLanes;
            l.Open = LevelStats.Of(l).OpenPercent;
            return l;
        }

        /* One level object in levels.json's format (outer joints as null). */
        public void WriteJson(JsonWriter w)
        {
            var l = ToLevelData();
            w.BeginObject()
             .Field("id", Id ?? UnsavedId).Field("version", Version).Field("lv", Number)
             .Field("w", Width).Field("h", Height).Field("timer", Timer);
            WriteNamed(w, "boosters", Boosters);
            WriteNamed(w, "unlock", Unlock);
            WriteIntro(w, "bintro", BoosterIntro);
            w.Key("pieces").BeginArray();
            foreach (var p in l.Pieces)
            {
                w.BeginArray();
                foreach (var c in p) w.BeginArray().Value(c.X).Value(c.Y).EndArray();
                w.EndArray();
            }
            w.EndArray();
            w.Key("vEdge").BeginArray();
            for (int y = 0; y < Height; y++)
            {
                w.BeginArray();
                for (int x = 0; x <= Width; x++) { if (IsInnerV(x, y)) w.Value(_v[y, x]); else w.Null(); }
                w.EndArray();
            }
            w.EndArray();
            w.Key("hEdge").BeginArray();
            for (int y = 0; y <= Height; y++)
            {
                w.BeginArray();
                for (int x = 0; x < Width; x++) { if (IsInnerH(x, y)) w.Value(_h[y, x]); else w.Null(); }
                w.EndArray();
            }
            w.EndArray();
            w.Key("sealed").BeginObject();
            foreach (var d in DirExt.All)
                if (_sealed.TryGetValue(d, out var lanes))
                {
                    w.Key(d.ToString()).BeginArray();
                    foreach (var lane in lanes) w.Value(lane);
                    w.EndArray();
                }
            w.EndObject();
            WriteCounters(w, "nails", l.Nails);
            WriteCounters(w, "bombs", l.Bombs);
            w.Key("colors").BeginArray();
            foreach (var c in _colors) w.Value(c);
            w.EndArray();
            w.Key("palette").BeginArray();
            foreach (var c in Palette) w.Value(c);
            w.EndArray();
            w.Key("solution").BeginArray();
            foreach (var m in Solution) w.BeginObject().Field("piece", m.Piece).Field("dir", m.Dir.ToString()).EndObject();
            w.EndArray();
            w.Field("load", Load).Field("open", l.Open).Field("art", Art);
            WriteIntro(w, "intro", Intro);
            w.EndObject();
        }

        static void WriteNamed(JsonWriter w, string key, Dictionary<string, int> map)
        {
            w.Key(key).BeginObject();
            foreach (var kv in map) w.Field(kv.Key, kv.Value);
            w.EndObject();
        }

        static void WriteCounters(JsonWriter w, string key, IReadOnlyDictionary<int, int> map)
        {
            var keys = new List<int>(map.Keys);
            keys.Sort();
            w.Key(key).BeginObject();
            foreach (var k in keys) w.Field(k.ToString(System.Globalization.CultureInfo.InvariantCulture), map[k]);
            w.EndObject();
        }

        static void WriteIntro(JsonWriter w, string key, LevelIntro i)
        {
            w.Key(key);
            if (i == null) { w.Null(); return; }
            w.BeginObject();
            if (i.Id != null) w.Field("id", i.Id);
            if (i.Icon != null) w.Field("icon", i.Icon);
            if (i.Title != null) w.Field("title", i.Title);
            if (i.Body != null) w.Field("body", i.Body);
            if (i.Tip != null) w.Field("tip", i.Tip);
            w.EndObject();
        }

        /* Rule violations; empty for every draft the edits above can produce.
           The random-edit test checks this after each step. */
        public List<string> Problems()
        {
            var p = new List<string>();
            if (Width < MinSize || Width > MaxSize || Height < MinSize || Height > MaxSize) p.Add($"size {Width}x{Height}");
            if (_v.GetLength(0) != Height || _v.GetLength(1) != Width + 1 || _h.GetLength(0) != Height + 1 || _h.GetLength(1) != Width)
                p.Add("joint grid size");
            else
            {
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x <= Width; x++)
                    {
                        int j = _v[y, x];
                        if (j < -1 || j > 1) p.Add($"v{x},{y}={j}");
                        if (!IsInnerV(x, y) && j != 0) p.Add($"outer v{x},{y}");
                    }
                for (int y = 0; y <= Height; y++)
                    for (int x = 0; x < Width; x++)
                    {
                        int j = _h[y, x];
                        if (j < -1 || j > 1) p.Add($"h{x},{y}={j}");
                        if (!IsInnerH(x, y) && j != 0) p.Add($"outer h{x},{y}");
                    }
            }
            var used = new HashSet<Cell>();
            foreach (var c in _chains)
            {
                if (!OnBoard(c.A) || !OnBoard(c.B)) p.Add($"chain {c} off board");
                if (c.A.Equals(c.B)) p.Add($"chain {c} one cell");
                if (!used.Add(c.A) || !used.Add(c.B)) p.Add($"chain {c} overlaps");
            }
            foreach (var kv in _nails)
            {
                if (!OnBoard(kv.Key) || !AnchorOf(kv.Key).Equals(kv.Key)) p.Add($"nail {kv.Key} not on an anchor");
                if (ChainAt(kv.Key) >= 0) p.Add($"nail {kv.Key} on a chain");
                if (_bombs.ContainsKey(kv.Key)) p.Add($"nail and bomb {kv.Key}");
                if (kv.Value < 1 || kv.Value > MaxCount) p.Add($"nail {kv.Key}={kv.Value}");
            }
            foreach (var kv in _bombs)
            {
                if (!OnBoard(kv.Key) || !AnchorOf(kv.Key).Equals(kv.Key)) p.Add($"bomb {kv.Key} not on an anchor");
                if (kv.Value < 1 || kv.Value > MaxCount) p.Add($"bomb {kv.Key}={kv.Value}");
            }
            foreach (var kv in _sealed)
                foreach (var lane in kv.Value)
                    if (lane < 0 || lane >= LaneCount(kv.Key)) p.Add($"sealed {kv.Key}{lane}");
            if (_colors.Length != Width * Height) p.Add("colors length");
            foreach (var c in _colors) if (c < 0 || c >= Palette.Count) { p.Add($"colour {c}"); break; }
            if (Timer < MinTimer || Timer > MaxTimer) p.Add($"timer {Timer}");
            return p;
        }
    }
}
