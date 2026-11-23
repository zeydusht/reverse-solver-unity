"""PM_100 §2-4: the game's effects louder and apart, and the transitions' sounds.
§2 the slide (exit) and the jam must not blur: the exit's takes (every soft candidate f-j) high-passed at 450 Hz (an airy
   whoosh, no body); the knock (a) its 400-1800 Hz middle up, gently low-passed (3.5 kHz, first order) (a rubbery thud, its top softened); a jam that slides first gets
   only a short soft lift-off (kalkis: the exit's first 45 ms, high-passed at 900 Hz, 9 dB under it; kalkisk its first
   25 ms for a short slide) before the knock at the contact (GameRoot: the one that ends before the contact, or none).
§3 the mix: the game's effects (exit, knock, bomb, lock) at the tick's level + 1.5 dB, their peaks at most -3 dBFS
   (&sfx=1.5 has room); the music 3 dB down (Music.cs). Every loudness here is the loudest 400 ms (momentary, LUFS).
§4 the transitions (Gecis100.cs): three candidates each, all recorded CC0 (docs/art/pm100/LISANS.md), soft, under the
   music: the corridor's slide (a steady texture the game shapes by the slide's speed), the dive's way in (a rising
   whoosh) and its touch (a ting), the cuts drawing (paper / brush, very quiet), the pull back (a falling whoosh) and the
   HUD settling (a tiny tick), the tab bar's move (a short swish).
    python -I Tools/make_sfx100.py <Kenney + OGA packs dir (casino-audio, rpg-audio, ui-audio, interface-sounds,
                                    impact-sounds, oga/)> <whoosh dir (air-whoosh.wav, wind-woosh-loop.ogg, bamboo.mp3,
                                    OceanDrum_Sus_*.wav)> <VCSL samples dir, flattened>
Out: Resources/Art/audio/pm100/ (read before pm99 and pm95), docs/art/pm100/onizleme/sesler/ + sesler100.json,
docs/art/pm100/karisim.json (every sound before and after)."""
import glob, json, os, sys, uuid
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln
from scipy.signal import butter, sosfilt

SR = 44100
PACKS, WHOOSH, VCSL = sys.argv[1], sys.argv[2], sys.argv[3]
RES = 'Assets/_Game/Presentation/Resources/Art/audio'
OUT = os.path.join(RES, 'pm100')
PAGE = 'docs/art/pm100/onizleme'
META = open(os.path.join(RES, 'pm95', 'tik-b1.wav.meta'), encoding='utf8').read()
rng = np.random.default_rng(100)


def read(p): return librosa.load(p, sr=SR, mono=True)[0].astype(np.float64)


phone = butter(4, [350, 8000], 'band', fs=SR, output='sos')   # roughly what a phone's small speaker plays


def loud(x, vol=1.0):
    """The loudest 400 ms (momentary loudness, LUFS), the sound in both ears, as played (vol)."""
    x = np.asarray(x) * vol; m = pyln.Meter(SR, block_size=.4); w = int(.4 * SR)
    y = np.concatenate([x, np.zeros(w)]); best = -99.0
    for i in range(0, max(1, len(y) - w), int(.01 * SR)):
        seg = y[i:i + w]
        if np.abs(seg).max() < 1e-6: continue
        best = max(best, m.integrated_loudness(np.stack([seg, seg], 1)))
    return best


def peak(x, vol=1.0): return float(20 * np.log10(np.abs(np.asarray(x, np.float64) * vol).max() + 1e-12))


def centroid(x): return float(librosa.feature.spectral_centroid(y=np.asarray(x, float) + 1e-9, sr=SR, n_fft=1024, hop_length=256).mean())


def limit(x, ceil):
    w = int(.002 * SR); a = np.abs(x)
    e = np.array([a[max(0, i - w):i + w].max() for i in range(0, len(a), w)])
    g = np.minimum(1, ceil / np.maximum(e, 1e-9))
    for i in range(1, len(g)): g[i] = min(g[i], g[i - 1] * .85 + .15)
    return x * np.interp(np.arange(len(a)), np.arange(len(g)) * w, g)


def level(x, target, ceil_db=-3.0):
    c = 10 ** (ceil_db / 20)
    for _ in range(12): x = limit(x * 10 ** ((target - loud(x)) / 20), c)
    return x


def hp(x, f, order=2): return sosfilt(butter(order, f, 'hp', fs=SR, output='sos'), x)
def lp(x, f, order=2): return sosfilt(butter(order, f, 'lp', fs=SR, output='sos'), x)


def fades(x, fin=.01, fout=.03):
    x = x.copy(); a, b = int(fin * SR), min(len(x) // 2, int(fout * SR))
    if a: x[:a] *= np.linspace(0, 1, a) ** 2
    if b: x[-b:] *= np.linspace(1, 0, b) ** 2
    return x


def onset_trim(x, lead=.003):
    e = np.convolve(np.abs(x), np.ones(88) / 88, 'same'); on = int(np.argmax(e > e.max() * .05))
    return x[max(0, on - int(lead * SR)):]


def stretch(x, r):
    """Resampled by r (r < 1: slower and lower)."""
    t = np.arange(int(len(x) / r)) * r
    return np.interp(t, np.arange(len(x)), x)


def sweep(x, r0, r1):
    """Played at a rate gliding from r0 to r1 (a rising or falling pitch)."""
    n = len(x); pos, out, i = 0.0, [], 0
    while pos < n - 1:
        r = r0 + (r1 - r0) * pos / n
        out.append(np.interp(pos, np.arange(n), x)); pos += r
    return np.array(out)


def flatten(x, win=.25):
    """A steady texture: divided by its slow RMS envelope (the pulses of a recording evened out)."""
    w = int(win * SR); e = np.sqrt(np.convolve(x ** 2, np.ones(w) / w, 'same')) + 1e-6
    return x / e * np.median(e)


def loop(x, seconds, xf=.25):
    """A seamless loop of `seconds`: the tail crossfaded into the head (equal power)."""
    n, f = int(seconds * SR), int(xf * SR)
    x = x[:n + f].copy(); t = np.linspace(0, np.pi / 2, f)
    x[:f] = x[:f] * np.sin(t) + x[n:n + f] * np.cos(t)
    return x[:n]


def write(folder, name, x, meta=True, stereo=True):
    os.makedirs(folder, exist_ok=True); path = os.path.join(folder, name)
    sf.write(path, np.stack([x, x], 1) if stereo else x, SR, subtype='PCM_16')
    if not meta: return
    m = path + '.meta'; guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    lines = [f'guid: {guid or uuid.uuid4().hex}' if l.startswith('guid:') else l for l in META.splitlines()]
    open(m, 'w', newline='\n').write('\n'.join(lines) + '\n')


k = lambda pack, f: os.path.join(PACKS, pack, 'Audio', f)
oga = lambda d, f: os.path.join(PACKS, 'oga', d, f)
wh = lambda f: os.path.join(WHOOSH, f)
vc = lambda pat: sorted(glob.glob(os.path.join(VCSL, pat)))[0]

os.makedirs(OUT, exist_ok=True)
for f in os.listdir(OUT):
    if f.endswith('.wav') or f.endswith('.wav.meta'): os.remove(os.path.join(OUT, f))
mix = {}

# ---- §3: the reference, the tick (UI, Zeyd's b) ----------------------------------------------------------------------
tick = [read(os.path.join(RES, 'pm98', f'ui-tick-b{n}.wav')) for n in range(1, 5)]
REF = float(np.mean([loud(x) for x in tick])); FX = REF + 1.5
PH = float(np.mean([loud(sosfilt(phone, x)) for x in tick]))
mix['tik (UI, referans)'] = dict(before=round(REF, 1), after=round(REF, 1), peak=round(max(peak(x) for x in tick), 1), phone_before=round(PH, 1), phone_after=round(PH, 1))
print(f'tık: {REF:.1f} LUFS; oyun efektleri hedefi {FX:.1f}')

# ---- §2 + §3: the exits (high-passed), their lift-offs, the knock (low-passed), the bomb, the lock --------------------
cent = {}
for letter in 'fghij':
    olds = [read(p) for p in sorted(glob.glob(os.path.join(RES, 'pm99', f'cikis-{letter}[0-9].wav')))]
    for n, x in enumerate(olds, 1):
        y = level(hp(x, 450), FX); write(OUT, f'cikis-{letter}{n}.wav', y)
        lift = fades(onset_trim(x)[:int(.045 * SR)], .01, .02)
        write(OUT, f'kalkis-{letter}{n}.wav', level(hp(lift, 900), FX - 9))
        short = fades(onset_trim(x)[:int(.025 * SR)], .006, .012)   # for a short slide (35-55 ms to the contact)
        write(OUT, f'kalkisk-{letter}{n}.wav', level(hp(short, 900), FX - 9))
    if letter == 'f':
        news = [read(os.path.join(OUT, f'cikis-f{n}.wav')) for n in range(1, len(olds) + 1)]
        mix['çıkış (kayma, f)'] = dict(before=round(np.mean([loud(x) for x in olds]), 1), after=round(np.mean([loud(x) for x in news]), 1),
                                      peak=round(max(peak(x) for x in news), 1), centroid_before=round(np.mean([centroid(x) for x in olds])),
                                      centroid_after=round(np.mean([centroid(x) for x in news])),
                                      phone_before=round(np.mean([loud(sosfilt(phone, x)) for x in olds]), 1), phone_after=round(np.mean([loud(sosfilt(phone, x)) for x in news]), 1))
        cent['cikis'] = mix['çıkış (kayma, f)']['centroid_after']
        lifts = [read(os.path.join(OUT, f'kalkis-f{n}.wav')) for n in range(1, len(olds) + 1)]
        mix['kalkış (takılmada, yeni)'] = dict(before=None, after=round(np.mean([loud(x) for x in lifts]), 1), peak=round(max(peak(x) for x in lifts), 1),
                                              length_ms=round(1000 * max(len(x) for x in lifts) / SR))
olds = [read(os.path.join(RES, 'pm99', f'carpma-a{n}.wav')) for n in range(1, 5)]
news = []
for n, x in enumerate(olds, 1):
    bp = sosfilt(butter(2, [400, 1800], 'band', fs=SR, output='sos'), x)   # its rubbery middle up (a phone hears it)
    y = level(lp(x + 1.2 * bp, 3500, 1), FX); write(OUT, f'carpma-a{n}.wav', y); news.append(y)
mix['takılma (çarpma a)'] = dict(before=round(np.mean([loud(x) for x in olds]), 1), after=round(np.mean([loud(x) for x in news]), 1),
                                 peak=round(max(peak(x) for x in news), 1), centroid_before=round(np.mean([centroid(x) for x in olds])),
                                 centroid_after=round(np.mean([centroid(x) for x in news])),
                                 phone_before=round(np.mean([loud(sosfilt(phone, x)) for x in olds]), 1), phone_after=round(np.mean([loud(sosfilt(phone, x)) for x in news]), 1))
cent['carpma'] = mix['takılma (çarpma a)']['centroid_after']
for key, src, vol, name in (('bomba', 'pm72-defuse.wav', .85, 'bomba (defuse)'), ('kilit', 'pm72-unlock.wav', 1.0, 'kilit (açılma)')):
    x = read(os.path.join(RES, src)); y = level(x, FX); write(OUT, f'{key}-a1.wav', y)
    mix[name] = dict(before=round(loud(x, vol), 1), after=round(loud(y), 1), peak=round(peak(y), 1), phone_before=round(loud(sosfilt(phone, x), vol), 1), phone_after=round(loud(sosfilt(phone, y)), 1))
# the music: 3 dB down (Music.cs Volume .35 -> .248)
for name, p in (('bölüm müziği E', 'Assets/Audio/Music98/music-E.mp3'), ('lobi müziği n', 'Assets/Audio/Music96/music-n.mp3')):
    x = librosa.load(p, sr=SR, mono=True, duration=40)[0]
    mix[name] = dict(before=round(loud(x, .35), 1), after=round(loud(x, .35 * 10 ** (-3 / 20)), 1), peak=round(peak(x, .35 * 10 ** (-3 / 20)), 1),
                     phone_before=round(loud(sosfilt(phone, x), .35), 1), phone_after=round(loud(sosfilt(phone, x), .35 * 10 ** (-3 / 20)), 1))
fan = read('Assets/Audio/Fanfare96/kazanma-b5.wav')
mix['kazanma fanfarı b5'] = dict(before=round(loud(fan), 1), after=round(loud(fan), 1), peak=round(peak(fan), 1))

# §2's test from the files as the game plays them: a jam (the lift-off, the knock at the contact .07 s later) and an exit
jam = np.zeros(int(.6 * SR)); l1 = read(os.path.join(OUT, 'kalkis-f1.wav')); k1 = read(os.path.join(OUT, 'carpma-a1.wav'))
jam[:len(l1)] += l1; jam[int(.07 * SR):int(.07 * SR) + len(k1)] += k1[:len(jam) - int(.07 * SR)]
ex = read(os.path.join(OUT, 'cikis-f1.wav'))
write(os.path.join(PAGE, 'sesler'), 'takilma-sonra-cikis.wav', np.concatenate([jam, np.zeros(int(.4 * SR)), ex, np.zeros(int(.3 * SR))]), False)
test = dict(jam_centroid=round(centroid(jam[:int(.5 * SR)])), exit_centroid=round(centroid(ex)),
            lift_ms=round(1000 * len(l1) / SR), contact_ms=70, overlap_ms=max(0, round(1000 * len(l1) / SR) - 70))
print('takılma/çıkış:', test)

# ---- §4: the transitions ---------------------------------------------------------------------------------------------
air, wind, bamboo = read(wh('air-whoosh.wav')), read(wh('wind-woosh-loop.ogg')), read(wh('bamboo.mp3'))
ocean = read(sorted(glob.glob(wh('OceanDrum_Sus_2*')))[0])


def env_db(x, w=.05):
    n = int(w * SR); return np.sqrt(np.convolve(x ** 2, np.ones(n) / n, 'same'))


def air_pass(i):
    """The air whoosh's i-th pass: (its rise to the peak, its fall from it)."""
    from scipy.signal import find_peaks
    e = env_db(air)[::441]   # 10 ms steps
    peaks, _ = find_peaks(e, height=e.max() * .4, distance=60)
    p = int(peaks[min(i, len(peaks) - 1)]) * 441
    return air[max(0, p - int(.6 * SR)):p], air[p:p + int(.75 * SR)]


def bamboo_swish():
    e = env_db(bamboo, .02); p = int(np.argmax(e)); return bamboo[max(0, p - int(.05 * SR)):p + int(.3 * SR)]


rise_air, fall_air = air_pass(1)
rise_wind = sweep(lp(wind[:int(.62 * SR)], 5000), .85, 1.2)   # a rising pitch, ~.6 s
T = lambda n: np.linspace(0, 1, n)
GEC = {}   # kind -> [(letter, name, credit, link, clip, target)]
AIR = ('Air Whoosh (OpenGameArt)', 'https://opengameart.org/content/air-whoosh')
WIND = ('Wind Whoosh Loop (OpenGameArt)', 'https://opengameart.org/content/wind-whoosh-loop')
BAMBOO = ('Swish - bamboo stick weapon swhoshes (OpenGameArt)', 'https://opengameart.org/content/swish-bamboo-stick-weapon-swhoshes')
VCSL_L = ('VCSL', 'https://github.com/sgossner/VCSL')
KEN = lambda pack: (f'Kenney, {pack}', 'https://kenney.nl/assets/' + pack.lower().replace(' ', '-'))
SWISH = ('artisticdude, Swishes Sound Pack', 'https://opengameart.org/content/swishes-sound-pack')
BELL = ('Bell Dings/Chimes (OpenGameArt)', 'https://opengameart.org/content/bell-dingschimes')
# the corridor: a steady 2 s loop (the game sets its loudness and pitch by the slide's speed each frame)
GEC['koridor'] = [
    ('a', 'Rüzgâr (yumuşak hava)', *WIND, loop(lp(flatten(hp(wind[int(.5 * SR):], 120)), 5000), 2.0), -30),
    ('b', 'Okyanus davulu (boncuk hışırtısı)', *VCSL_L, loop(lp(flatten(hp(ocean[int(2 * SR):], 200)), 6000), 2.0), -30),
    ('c', 'Hava (parlak whoosh)', *AIR, loop(lp(flatten(hp(air, 300), .4), 4500), 2.0), -30),
]
# the dive's way in: a rising whoosh, .6 s, ending at the touch (the ting is its own clip, on the touch's frame)
GEC['dalis'] = [
    ('a', 'Hava, içeri', *AIR, fades(rise_air * T(len(rise_air)) ** 2, .05, .015), -30),
    ('b', 'Rüzgâr, yükselen', *WIND, fades(rise_wind * T(len(rise_wind)) ** 2, .05, .015), -30),
    ('c', 'Ters swish', *BAMBOO, fades(stretch(bamboo_swish()[::-1], .6), .05, .015), -30),
]
GEC['ting'] = [
    ('a', 'Glockenspiel parıltısı', *VCSL_L, fades(onset_trim(read(vc('*glock_soft_C7*')))[:int(.9 * SR)], 0, .4), -29),
    ('b', 'Küçük çan', *BELL, fades(onset_trim(read(oga('bell-dingschimes', 'bell_ding1.wav'))), 0, .3), -29),
    ('c', 'Üçgen', *VCSL_L, fades(onset_trim(read(vc('*triangle3_hit_mf*')))[:int(.9 * SR)], 0, .5), -29),
]
# the cuts drawing: paper / brush, very quiet, ~.7 s
shuf = read(k('casino-audio', 'card-shuffle.ogg')); e = env_db(shuf, .1); p = int(np.argmax(e))
sh = [onset_trim(read(f)) for f in sorted(glob.glob(os.path.join(VCSL, '*Shaker, Small*Up*')))][:2] + [onset_trim(read(f)) for f in sorted(glob.glob(os.path.join(VCSL, '*Shaker, Small*Down*')))][:1]
brush = np.zeros(int(.75 * SR))
for i, x in enumerate(sh): a = int(i * .22 * SR); brush[a:a + len(x)] += x[:len(brush) - a]
GEC['cizgi'] = [
    ('a', 'Kâğıt hışırtısı', *KEN('Casino Audio'), fades(lp(shuf[max(0, p - int(.35 * SR)):p + int(.35 * SR)], 5000), .15, .2), -36),
    ('b', 'Sayfa', *KEN('RPG Audio'), fades(lp(stretch(onset_trim(read(k('rpg-audio', 'bookFlip1.ogg'))), .8), 5000), .05, .15), -36),
    ('c', 'Fırça (küçük çalkalayıcı)', *VCSL_L, fades(lp(brush, 6000), .03, .15), -36),
]
# the pull back: a falling whoosh, ~.7 s; the HUD settling: a tiny tick
w = sweep(lp(wind[int(1.5 * SR):int(2.2 * SR)], 5000), 1.15, .85)
GEC['geri'] = [
    ('a', 'Hava, dışarı', *AIR, fades(fall_air * (1 - T(len(fall_air))) ** 1.5, .01, .05), -31),
    ('b', 'Rüzgâr, alçalan', *WIND, fades(w * (1 - T(len(w))) ** 1.5, .03, .05), -31),
    ('c', 'Swish, yavaş', *BAMBOO, fades(stretch(bamboo_swish(), .6), .005, .1), -31),
]
GEC['hud'] = [
    ('a', 'Yumuşak tık', *KEN('UI Audio'), fades(onset_trim(read(k('ui-audio', 'click2.ogg')))[:int(.15 * SR)], 0, .03), -29),
    ('b', 'Küçük tık', *KEN('Interface Sounds'), fades(onset_trim(read(k('interface-sounds', 'click_002.ogg')))[:int(.15 * SR)], 0, .03), -29),
    ('c', 'Cam tık', *KEN('Impact Sounds'), fades(onset_trim(read(k('impact-sounds', 'impactGlass_light_000.ogg')))[:int(.2 * SR)], 0, .05), -29),
]
GEC['sekme'] = [
    ('a', 'Kısa swish', *SWISH, fades(onset_trim(read(oga('swishes-sound-pack', 'swishes/swish-12.wav'))), .005, .03), -29),
    ('b', 'Kısa swish 2', *SWISH, fades(onset_trim(read(oga('swishes-sound-pack', 'swishes/swish-2.wav'))), .005, .03), -29),
    ('c', 'Kart kayması', *KEN('Casino Audio'), fades(onset_trim(read(k('casino-audio', 'card-slide-1.ogg'))), .005, .03), -29),
]
page = {'gecis': {}, 'mix': mix, 'test': test}
for kind, cands in GEC.items():
    page['gecis'][kind] = []
    for letter, name, credit, link, x, target in cands:
        y = level(x, target, -6)
        write(OUT, f'gecis-{kind}-{letter}.wav', y)
        write(os.path.join(PAGE, 'sesler'), f'gecis-{kind}-{letter}.wav', y, False)
        page['gecis'][kind].append(dict(letter=letter, name=name, credit=credit, link=link, file=f'gecis-{kind}-{letter}.wav',
                                        seconds=round(len(y) / SR, 2), lufs=round(loud(y), 1), default=letter == 'a'))
        print(f'geçiş {kind:7s} {letter} {name:28s} {len(y) / SR:4.2f} s {loud(y):6.1f} LUFS tepe {peak(y):5.1f}')
for name, r in mix.items(): print(f'{name:26s} önce {r["before"]}  sonra {r["after"]}  tepe {r["peak"]}  telefon {r.get("phone_before")} -> {r.get("phone_after")}' + (f'  merkez {r.get("centroid_before")} -> {r.get("centroid_after")} Hz' if 'centroid_after' in r else ''))
json.dump(page, open(os.path.join(PAGE, 'sesler100.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
json.dump(dict(mix=mix, test=test), open('docs/art/pm100/karisim.json', 'w', encoding='utf8'), ensure_ascii=False, indent=1)
