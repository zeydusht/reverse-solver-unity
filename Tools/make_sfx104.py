"""PM_104 §2-3: the slide (a piece leaving) as a soft swoosh, and a "tok" for the buttons.
§2 the slide, candidates k-p (&sescikis=), recorded CC0 (docs/art/pm104/LISANS.md), 4 takes each: from its first sound,
   0.12-0.25 s, a soft 20-30 ms rise and a 60 ms fade, high-passed (air, no thud: well apart from the knock's bonk at
   358-525 Hz), at the level of today's f (pm100/cikis-f*: the effects' level). Each with its own lift-offs for a jam
   that slides (kalkis: the first 45 ms, kalkisk: 25 ms, high-passed at 900 Hz, 9 dB under it), as PM_100 made them.
     k  bamboo swoosh, bright     qubodup "Swish - bamboo stick weapon swhoshes" swosh-15, -16, -17, -18
     l  bamboo swoosh, short      qubodup swosh-03, -08, -12, -20
     m  bamboo swoosh, wide       qubodup swosh-10, -11, -14, -22
     n  air whoosh                "Air whoosh" (OpenGameArt) whoosh2: four swells cut from the loop
     o  airy sweep                Kenney Interface Sounds maximize_001, _002, _003 + minimize_003 (rise, then fade)
     p  bamboo swoosh, lifted     k's takes a little higher (+3 semitones): shorter and brighter
§3 the button "tok" (UiSounds.Kind.Tok: Durdur, Devam, Menü, the bottom tabs, Ayarlar), candidates a-g (&sesdugme=),
   short (<= 0.12 s), body in the low mids, the top softened (no shrill click) and nothing under 200 Hz (a phone's
   speaker plays none of it), at the UI tick's level:
     a  soft wood            Kenney Impact Sounds impactWood_light_000, _002, _004
     b  wood, fuller         Kenney Impact Sounds impactWood_medium_001, _003, _004
     c  soft muffled         Kenney Impact Sounds impactSoft_medium_000, _002, _003
     d  plastic tok          Kenney Interface Sounds drop_002, _003, _004
     e  plastic select       Kenney Interface Sounds select_005, _006
     f  soft menu tok        Lokif "GUI Sound Effects" misc_menu, misc_menu_3
     g  little plank         Kenney Impact Sounds impactPlank_medium_001, _003
Out: Resources/Art/audio/pm104/ (read before pm100: cikis-, kalkis-, kalkisk-, ui-tok-) and docs/art/pm104/onizleme/
sesler/ + sesler104.json. The listening page's own copies of what the game plays for the slide f and the knock a
(pm100/cikis-f*, pm102/carpma-a*) go there too: the page played older versions (PM_104 §1).
    python -I Tools/make_sfx104.py <Kenney packs dir (impact-sounds, interface-sounds)> <pm104 downloads dir>"""
import json, os, sys, uuid, glob, shutil
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln
from scipy.signal import butter, sosfilt

SR = 44100
KEN, DL = sys.argv[1], sys.argv[2]
RES = 'Assets/_Game/Presentation/Resources/Art/audio'
OUT = os.path.join(RES, 'pm104')
PAGE = 'docs/art/pm104/onizleme'
META = open(os.path.join(RES, 'pm104', 'ui-tok-a1.wav.meta'), encoding='utf8').read()  # PM_105: the tick's files are gone (the same import settings)


def read(p): return librosa.load(p, sr=SR, mono=True)[0].astype(np.float64)


def loud(x, vol=1.0):
    """The loudest 400 ms (momentary loudness, LUFS), as played (vol)."""
    x = np.asarray(x) * vol; m = pyln.Meter(SR, block_size=.4); w = int(.4 * SR)
    y = np.concatenate([x, np.zeros(w)]); best = -99.0
    for i in range(0, max(1, len(y) - w), int(.01 * SR)):
        seg = y[i:i + w]
        if np.abs(seg).max() < 1e-6: continue
        best = max(best, m.integrated_loudness(np.stack([seg, seg], 1)))
    return best


def peak(x): return float(20 * np.log10(np.abs(x).max() + 1e-12))
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


def onset_trim(x, lead=.003, thr=.05):
    e = np.convolve(np.abs(x), np.ones(88) / 88, 'same'); on = int(np.argmax(e > e.max() * thr))
    return x[max(0, on - int(lead * SR)):]


def tail_trim(x, thr=.02):
    e = np.convolve(np.abs(x), np.ones(88) / 88, 'same'); idx = np.where(e > e.max() * thr)[0]
    return x[:idx[-1] + 1] if len(idx) else x


def stretch(x, r):
    t = np.arange(int(len(x) / r)) * r
    return np.interp(t, np.arange(len(x)), x)


def write(folder, name, x):
    os.makedirs(folder, exist_ok=True); path = os.path.join(folder, name)
    sf.write(path, np.stack([x, x], 1), SR, subtype='PCM_16')
    if folder != OUT: return
    m = path + '.meta'; guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    lines = [f'guid: {guid or uuid.uuid4().hex}' if l.startswith('guid:') else l for l in META.splitlines()]
    open(m, 'w', newline='\n').write('\n'.join(lines) + '\n')


imp = lambda f: os.path.join(KEN, 'impact-sounds', 'Audio', f)
ui = lambda f: os.path.join(KEN, 'interface-sounds', 'Audio', f)
bam = lambda n: os.path.join(DL, 'swoshes', f'swosh-{n:02d}.flac')
lok = lambda f: os.path.join(DL, 'GUI_Sound_Effects_by_Lokif', 'GUI_Sound_Effects_by_Lokif', f)
QUBO = ('qubodup (Iwan Gabovitch), "Swish - bamboo stick weapon swhoshes"', 'https://opengameart.org/content/swish-bamboo-stick-weapon-swhoshes')
KEN_UI = ('Kenney, Interface Sounds', 'https://kenney.nl/assets/interface-sounds')
KEN_IMP = ('Kenney, Impact Sounds', 'https://kenney.nl/assets/impact-sounds')

# the level of today's slide f and of the UI tick (what the game plays)
F_NOW = [read(os.path.join(RES, 'pm100', f'cikis-f{n}.wav')) for n in range(1, 5)]
FX = float(np.mean([loud(x) for x in F_NOW]))
TICK = [read(os.path.join(RES, 'pm98', f'ui-tick-b{n}.wav')) for n in range(1, 5)] if os.path.exists(os.path.join(RES, 'pm98', 'ui-tick-b1.wav')) else None   # PM_105: gone
UI = float(np.mean([loud(x) for x in TICK])) if TICK else -16.1   # PM_105: the tick's measured level (its files are gone)
print(f'kayma f: {FX:.1f} LUFS, tepe {max(peak(x) for x in F_NOW):.1f}; UI tık: {UI:.1f} LUFS')


def swoosh(x, rise=.025, length=.22, hpf=700, lift=1.0):
    x = onset_trim(x, lead=.0, thr=.03)
    if lift != 1: x = stretch(x, lift)
    x = tail_trim(x)[:int(length * SR)]
    x = hp(x, hpf, 3)
    return fades(x, rise, min(.06, len(x) / SR * .45))


def air_swells(p, n=4, length=.2):
    """Four swells of a long whoosh: its loudest short spans, each from a little before its peak."""
    x = read(p); w = int(.02 * SR)
    e = np.sqrt(np.convolve(x ** 2, np.ones(w) / w, 'same'))
    out, used = [], np.zeros(len(x), bool)
    for _ in range(n):
        e2 = np.where(used, 0, e); i = int(np.argmax(e2))
        a = max(0, i - int(.09 * SR)); b = min(len(x), a + int(length * SR))
        used[max(0, a - int(.3 * SR)):b + int(.3 * SR)] = True
        out.append(fades(hp(x[a:b], 700, 3), .06, .08))
    return out


SLIDES = [
    ('k', 'Bambu swoosh, parlak', QUBO, [bam(n) for n in (15, 16, 17, 18)], dict()),
    ('l', 'Bambu swoosh, kısa', QUBO, [bam(n) for n in (3, 8, 12, 20)], dict(length=.16)),
    ('m', 'Bambu swoosh, geniş', QUBO, [bam(n) for n in (10, 11, 14, 22)], dict(length=.25, rise=.03)),
    ('n', 'Hava whoosh', ('"Air whoosh" (OpenGameArt, CC0)', 'https://opengameart.org/content/air-whoosh'), None, None),
    ('o', 'İnce hava taraması', KEN_UI, [ui('maximize_001.ogg'), ui('maximize_002.ogg'), ui('maximize_003.ogg'), ui('minimize_003.ogg')], dict(length=.2, rise=.03, hpf=900)),
    ('p', 'Bambu swoosh, yükseltilmiş', QUBO, [bam(n) for n in (15, 16, 17, 18)], dict(lift=2 ** (3 / 12), length=.18)),
]
TOKS = [
    ('a', 'Yumuşak ahşap', KEN_IMP, [imp(f'impactWood_light_00{n}.ogg') for n in (0, 2, 4)]),
    ('b', 'Ahşap, dolgun', KEN_IMP, [imp(f'impactWood_medium_00{n}.ogg') for n in (1, 3, 4)]),
    ('c', 'Yumuşak, boğuk', KEN_IMP, [imp(f'impactSoft_medium_00{n}.ogg') for n in (0, 2, 3)]),
    ('d', 'Plastik tok', KEN_UI, [ui(f'drop_00{n}.ogg') for n in (2, 3, 4)]),
    ('e', 'Plastik seçim', KEN_UI, [ui('select_005.ogg'), ui('select_006.ogg')]),
    ('f', 'Yumuşak menü toku', ('Lokif, "GUI Sound Effects" (OpenGameArt, CC0)', 'https://opengameart.org/content/gui-sound-effects'), [lok('misc_menu.wav'), lok('misc_menu_3.wav')]),
    ('g', 'Küçük tahta', KEN_IMP, [imp(f'impactPlank_medium_00{n}.ogg') for n in (1, 3)]),
]

for old in glob.glob(os.path.join(OUT, '*.wav')) + glob.glob(os.path.join(PAGE, 'sesler', '*.wav')):
    os.remove(old)
page = dict(cikis=[], dugme=[], esles={})
for letter, name, (credit, link), files, opt in SLIDES:
    takes = air_swells(os.path.join(DL, 'whoosh2_0.wav')) if files is None else [swoosh(read(p), **opt) for p in files]
    entry = dict(letter=letter, name=name, credit=credit, link=link, takes=[])
    for n, x in enumerate(takes, 1):
        y = level(x, FX); write(OUT, f'cikis-{letter}{n}.wav', y); write(os.path.join(PAGE, 'sesler'), f'cikis-{letter}{n}.wav', y)
        lift = fades(onset_trim(x)[:int(.045 * SR)], .01, .02)
        write(OUT, f'kalkis-{letter}{n}.wav', level(hp(lift, 900), FX - 9))
        short = fades(onset_trim(x)[:int(.025 * SR)], .006, .012)
        write(OUT, f'kalkisk-{letter}{n}.wav', level(hp(short, 900), FX - 9))
        entry['takes'].append(dict(file=f'cikis-{letter}{n}.wav', seconds=round(len(y) / SR, 3), lufs=round(loud(y), 1), peak=round(peak(y), 1), centroid=int(centroid(y))))
    page['cikis'].append(entry)
    print(f'kayma {letter} {name}: ' + ', '.join(f"{t['seconds']:.2f} sn {t['centroid']} Hz" for t in entry['takes']))
for letter, name, (credit, link), files in TOKS:
    entry = dict(letter=letter, name=name, credit=credit, link=link, takes=[])
    for n, p in enumerate(files, 1):
        x = tail_trim(onset_trim(read(p), lead=.001))[:int(.12 * SR)]
        x = fades(hp(lp(x, 5000, 2), 200, 2), .002, min(.04, len(x) / SR * .5))   # no shrill top, no sub a phone cannot play
        y = level(x, UI)
        write(OUT, f'ui-tok-{letter}{n}.wav', y); write(os.path.join(PAGE, 'sesler'), f'ui-tok-{letter}{n}.wav', y)
        entry['takes'].append(dict(file=f'ui-tok-{letter}{n}.wav', seconds=round(len(y) / SR, 3), lufs=round(loud(y), 1), peak=round(peak(y), 1), centroid=int(centroid(y))))
    page['dugme'].append(entry)
    print(f'düğme {letter} {name}: ' + ', '.join(f"{t['seconds']:.2f} sn {t['centroid']} Hz" for t in entry['takes']))
# §1: the page's copies of what the game plays for f and the knock a (their own names: the page reads sesler/<file>)
for src, files in (('pm100', [f'cikis-f{n}.wav' for n in range(1, 5)]), ('pm102', [f'carpma-a{n}.wav' for n in range(1, 5)])):
    for f in files:
        shutil.copyfile(os.path.join(RES, src, f), os.path.join(PAGE, 'sesler', f))
        page['esles'][f] = src
json.dump(page, open(os.path.join(PAGE, 'sesler104.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
print('yazıldı:', OUT, PAGE)
