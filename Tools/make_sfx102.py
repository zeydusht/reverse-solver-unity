"""PM_102 §2-5: the knock steady and clear, the boosters' own recorded sounds, the corridor's whoosh heard.
§2 the knock (a): its four takes the same length (.15 s, a 40 ms fade), the same loudness and the same peak (each
   levelled until it reaches the common ceiling: no take louder than another), 2.5 dB over the slide as played (the
   exit is played 1 dB down: GameRoot). Out: pm102/carpma-a1..4 (read before pm100).
§4 the boosters (BoosterSounds102), three recorded CC0 candidates each (docs/art/pm102/LISANS.md), toy-like (softened
   on top, short), at the effects' level:
     makas   the cut: one real "snip", on the frame the blades close (its attack at the clip's start)
     degnek  a rising shimmer whose soft "poof" falls on the turn (DegnekLead s into the clip)
     cekic   the blow with a break (ceramic / wood crack) on the strike; a soft clatter as the piece falls
     saat    a quiet tick-tock loop through the freeze, an ice crackle as it freezes, the ice breaking as the frozen
             bar breaks
§5 the corridor's texture 5 dB louder (-25 at the slide's fastest; it was under the lobby music coming back).
    python -I Tools/make_sfx102.py <Kenney + OGA packs dir> <PM_100 whoosh dir> <VCSL dir, flattened> <PM_102 sources dir>
Out: Resources/Art/audio/pm102/, docs/art/pm102/onizleme/sesler/ + sesler102.json."""
import glob, json, os, sys
import numpy as np, soundfile as sf, librosa
from scipy.signal import butter, sosfilt

ARGS = sys.argv[1:]
_h = open('Tools/make_sfx100.py', encoding='utf8').read().split('os.makedirs(OUT, exist_ok=True)\nfor f in os.listdir(OUT)')[0]
_ns = {'__name__': 'sfx100', '__file__': 'Tools/make_sfx100.py'}
sys.argv = ['make_sfx100'] + ARGS[:3]
exec(compile(_h, 'make_sfx100', 'exec'), _ns)
globals().update({k: _ns[k] for k in ('SR', 'read', 'loud', 'peak', 'centroid', 'limit', 'level', 'hp', 'lp', 'fades', 'onset_trim',
                                       'stretch', 'loop', 'flatten', 'write', 'phone', 'RES', 'META')})
PACKS, WHOOSH, VCSL, SRC = ARGS
OUT = os.path.join(RES, 'pm102')
PAGE = 'docs/art/pm102/onizleme'
DEGNEK_LEAD = .40   # the wand clip's poof, s from its start (BoosterSounds102.WandLead)
os.makedirs(OUT, exist_ok=True); os.makedirs(os.path.join(PAGE, 'sesler'), exist_ok=True)
for f in os.listdir(OUT):
    if f.endswith('.wav') or f.endswith('.wav.meta'): os.remove(os.path.join(OUT, f))
src = lambda f: os.path.join(SRC, f)
k = lambda pack, f: os.path.join(PACKS, pack, 'Audio', f)
vc = lambda pat: sorted(glob.glob(os.path.join(VCSL, pat)))[0]
report = {}


def exactly(xs, target, ceil_db):
    """Takes at the same loudness and the same peak: each levelled to the target under the ceiling, then raised (and
    limited) until its peak is the ceiling, then the loudness evened by a last common trim."""
    c = 10 ** (ceil_db / 20)
    out = []
    for x in xs:
        y = level(x, target, ceil_db)
        y = limit(y / np.abs(y).max() * c, c)
        out.append(y)
    return out


# ---- §2 the knock ----------------------------------------------------------------------------------------------------
tick = [read(os.path.join(RES, 'pm98', f'ui-tick-b{n}.wav')) for n in range(1, 5)] if os.path.exists(os.path.join(RES, 'pm98', 'ui-tick-b1.wav')) else None   # PM_105: gone
if tick is None: sys.exit('PM_105: the button tick (pm98/ui-tick-b, its level reference) is gone; this script made its files once and is not run again')
exits = [read(os.path.join(RES, 'pm100', f'cikis-f{n}.wav')) for n in range(1, 5)]
EXIT_PLAYED = float(np.mean([loud(x, 10 ** (-1 / 20)) for x in exits]))   # the exit, 1 dB down (GameRoot.ExitLevel)
KNOCK = EXIT_PLAYED + 2.5
olds = [read(os.path.join(RES, 'pm100', f'carpma-a{n}.wav')) for n in range(1, 5)]
shaped = [np.pad(y, (0, int(.15 * SR) - len(y))) for y in (fades(onset_trim(x)[:int(.15 * SR)], .002, .04) for x in olds)]   # one length
knocks = exactly(shaped, KNOCK, -1.5)
for n, y in enumerate(knocks, 1): write(OUT, f'carpma-a{n}.wav', y)
report['carpma'] = dict(before=[round(loud(x), 2) for x in olds], before_peak=[round(peak(x), 2) for x in olds],
                        before_len=[round(len(x) / SR, 3) for x in olds],
                        after=[round(loud(x), 2) for x in knocks], after_peak=[round(peak(x), 2) for x in knocks],
                        after_len=[round(len(x) / SR, 3) for x in knocks], exit_played=round(EXIT_PLAYED, 2), target=round(KNOCK, 2),
                        tick=round(float(np.mean([loud(x) for x in tick])), 2))
print('çarpma önce', report['carpma']['before'], 'tepe', report['carpma']['before_peak'], 'uzunluk', report['carpma']['before_len'])
print('çarpma sonra', report['carpma']['after'], 'tepe', report['carpma']['after_peak'], f'(kayma oyunda {EXIT_PLAYED:.2f}, hedef {KNOCK:.2f})')

# ---- §5 the corridor -------------------------------------------------------------------------------------------------
for l in 'abc':
    x = read(os.path.join(RES, 'pm100', f'gecis-koridor-{l}.wav'))
    write(OUT, f'gecis-koridor-{l}.wav', level(x, -25, -6))
    write(os.path.join(PAGE, 'sesler'), f'gecis-koridor-{l}.wav', level(x, -25, -6), False)
print('koridor: -30 -> -25 LUFS (en hızlı anda)')

# ---- §4 the boosters -------------------------------------------------------------------------------------------------
FX = float(np.mean([loud(x) for x in tick])) + 1.5   # the game's effects (PM_100): the tick + 1.5 dB


def toy(x, top=7000): return lp(x, top, 2)


def first_hit(x, length):
    """From the first sound, `length` s, a soft fade out."""
    return fades(onset_trim(x, .002)[:int(length * SR)], .002, min(.08, length / 3))


def hits(x, thresh=.3):
    """The attacks of a recording (s): its 10 ms envelope crossing `thresh` of its peak."""
    e = np.convolve(np.abs(x), np.ones(441) / 441, 'same'); on = e > e.max() * thresh; out = []
    i = 0
    while i < len(on):
        if on[i]: out.append(i / SR); i += int(.12 * SR)
        else: i += 1
    return out


def poof():
    """The wand's poof: a soft swish, low-passed (artisticdude's Swishes, CC0)."""
    x = read(os.path.join(PACKS, 'oga', 'swishes-sound-pack', 'swishes', 'swish-7.wav'))
    return fades(lp(onset_trim(x), 3000), .004, .06)


def wand(shimmer):
    """A rising shimmer into the poof at DEGNEK_LEAD s; the shimmer rings on under it a little."""
    n = int(DEGNEK_LEAD * SR); s = shimmer[:n + int(.35 * SR)].copy()
    env = np.concatenate([np.linspace(.15, 1, n) ** 1.5, np.linspace(1, 0, len(s) - n) ** 2]) if len(s) > n else np.linspace(.15, 1, len(s))
    s = s * env[:len(s)]
    out = np.zeros(n + int(.5 * SR)); out[:len(s)] += s
    p = poof(); out[n:n + len(p)] += .9 * p[:len(out) - n] / (np.abs(p).max() + 1e-9) * np.abs(s).max()
    return fades(toy(out, 9000), .01, .1)


def mix(*parts):
    """(x, at s, gain) summed."""
    n = max(int(at * SR) + len(x) for x, at, _ in parts)
    out = np.zeros(n)
    for x, at, g in parts: a = int(at * SR); out[a:a + len(x)] += x * g
    return out


def tick_loop(tick_a, tock_b, gain_b=.8):
    """A 1 s loop: a tick at 0, a tock at .5 s."""
    out = np.zeros(SR)
    for x, at, g in ((tick_a, 0, 1), (tock_b, .5, gain_b)):
        x = first_hit(x, .2); a = int(at * SR); out[a:a + len(x)] += x[:len(out) - a] * g
    return out


scis = read(src('scissors.ogg')); hair = read(src('hair_scissors_01.mp3')); shears = read(src('garden_shears_01.mp3'))
bf = lambda f: read(src(os.path.join('sfx_breaking_and_falling', f)))
ice = lambda f: read(src(os.path.join('IceShatters_0', 'IceShatters', f)))
clock = read(src('ticking_clock.wav')); th = hits(clock, .4)
clock_tick = clock[int(th[2] * SR):] if len(th) > 3 else clock
CAND = {
    'makas': [
        ('a', 'Makas (kâğıt)', 'OpenGameArt "Scissors"', 'https://opengameart.org/content/scissors', [(toy(first_hit(scis, .22)), FX)]),
        ('b', 'Saç makası', 'OpenGameArt "Various Scissors"', 'https://opengameart.org/content/various-scissors', [(toy(first_hit(hair, .25)), FX)]),
        ('c', 'Bahçe makası, hafif', 'OpenGameArt "Various Scissors"', 'https://opengameart.org/content/various-scissors', [(toy(first_hit(shears, .25), 6000), FX)]),
    ],
    'degnek': [
        ('a', 'Rüzgâr çanı + puf', 'VCSL Mark Trees + Swishes Sound Pack', 'https://github.com/sgossner/VCSL', [(wand(read(vc('*Mark Trees*fastAsc1*'))), FX - 1)]),
        ('b', 'Glockenspiel glisandosu + puf', 'VSCO 2 CE + Swishes Sound Pack', 'https://github.com/sgossner/VSCO-2-CE', [(wand(onset_trim(read(src('glock_fx_up_chromatic_fast_01.wav')))), FX - 1)]),
        ('c', 'Çan ağacı + puf', 'VCSL Bell Tree + Swishes Sound Pack', 'https://github.com/sgossner/VCSL', [(wand(onset_trim(read(vc('*Bell Tree*Hit_C5*')))[:int(DEGNEK_LEAD * SR)][::-1]), FX - 1)]),
    ],
    'cekic': [
        ('a', 'Tahta vuruş + çatlama', 'OpenGameArt "75 CC0 breaking / falling / hit sfx"', 'https://opengameart.org/content/75-cc0-breaking-falling-hit-sfx',
         [(toy(mix((first_hit(bf('bfh1_wood_hit_02.ogg'), .2), 0, 1), (first_hit(bf('bfh1_wood_breaking_03.ogg'), .3), .015, .8))), FX), (toy(first_hit(bf('bfh1_wood_falling_02.ogg'), .45)), FX - 8)]),
        ('b', 'Seramik kırılma', 'OpenGameArt "75 CC0 breaking / falling / hit sfx"', 'https://opengameart.org/content/75-cc0-breaking-falling-hit-sfx',
         [(toy(mix((first_hit(bf('bfh1_hit_07.ogg'), .2), 0, 1), (first_hit(bf('bfh1_breaking_03.ogg'), .35), .01, .8)), 6500), FX), (toy(first_hit(bf('bfh1_falling_06.ogg'), .4)), FX - 8)]),
        ('c', 'Kalın tahta, yumuşak', 'Kenney Impact Sounds + OpenGameArt 75 CC0', 'https://kenney.nl/assets/impact-sounds',
         [(toy(mix((first_hit(read(k('impact-sounds', 'impactPlank_medium_000.ogg')), .25), 0, 1), (first_hit(bf('bfh1_wood_breaking_04.ogg'), .3), .02, .7)), 6000), FX),
          (toy(first_hit(bf('bfh1_rock_falling_07.ogg'), .35)), FX - 8)]),
    ],
    'saat': [
        ('a', 'Duvar saati', 'OpenGameArt "Ticking Clock" + "Ice Breaking/Shattering"', 'https://opengameart.org/content/ticking-clock',
         [(loop(np.concatenate([lp(clock_tick, 6000)] * 2), 1.0, .05), FX - 12), (toy(first_hit(ice('LedasLuzta2.ogg'), .35), 6000), FX - 4), (toy(first_hit(ice('LedasLuzta.ogg'), .5), 7000), FX - 2)]),
        ('b', 'Tik-tak', 'OpenGameArt "Tick and Tock" + "Ice Breaking/Shattering"', 'https://opengameart.org/content/tick-and-tock',
         [(tick_loop(read(src('tick.wav')), read(src('tick2.wav'))), FX - 12), (toy(first_hit(bf('bfh1_glass_hit_01.ogg'), .3), 6000), FX - 4), (toy(first_hit(ice('LedasLuzta33.ogg'), .6), 7000), FX - 2)]),
        ('c', 'Küçük saat', 'OpenGameArt "Clock tick" + "75 CC0 breaking"', 'https://opengameart.org/content/clock-tick-0',
         [(tick_loop(read(src('Timetick.ogg')), stretch(read(src('Timetick.ogg')), 1.12), .7), FX - 12), (toy(first_hit(read(k('impact-sounds', 'impactGlass_light_001.ogg')), .3), 6000), FX - 4),
          (toy(first_hit(bf('bfh1_glass_breaking_02.ogg'), .45), 6500), FX - 2)]),
    ],
}
PARTS = {'makas': ['kesim'], 'degnek': ['donusum'], 'cekic': ['vurus', 'kirinti'], 'saat': ['tiktak', 'donma', 'kirilma']}
page = {}
for b, cands in CAND.items():
    page[b] = []
    for letter, name, credit, link, parts in cands:
        files = []
        for part, (x, target) in zip(PARTS[b], parts):
            y = level(x, target, -3)
            f = f'booster-{b}-{part}-{letter}.wav'
            write(OUT, f, y); write(os.path.join(PAGE, 'sesler'), f, y, False)
            files.append(dict(part=part, file=f, seconds=round(len(y) / SR, 2), lufs=round(loud(y), 1), peak=round(peak(y), 1)))
            print(f'{b:6s} {letter} {part:8s} {name:28s} {len(y) / SR:4.2f} s {loud(y):6.1f} LUFS tepe {peak(y):5.1f}')
        page[b].append(dict(letter=letter, name=name, credit=credit, link=link, files=files, default=letter == 'a'))
report['booster_fx'] = round(FX, 2)
json.dump(dict(booster=page, carpma=report['carpma']), open(os.path.join(PAGE, 'sesler102.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
json.dump(report, open('docs/art/pm102/olcum102.json', 'w', encoding='utf8'), ensure_ascii=False, indent=1)
