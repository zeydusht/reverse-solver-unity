"""PM_95 §2: the sound candidates (exit, knock, last piece, win jingle, button tick), recorded sounds, all CC0
(Kenney packs and OpenGameArt; docs/art/pm95/LISANS.md). Each candidate is 1-4 takes (the game picks one at random,
never the same twice in a row, and shifts the pitch a little: no machine gun on a run of exits).
Each take: mono 44.1 kHz, trimmed from 3 ms before its onset (a slide from its body, not its faint start), faded out, layered where the candidate is two sounds
(a card slide and a soft pop: the pop where the slide ends), and set to the loudness of the sound it stands in for as
the game plays it (Unity normalises the old clips' peak; the new ones are imported unnormalised) (the loudest 50 ms, K-weighted as in BS.1770), so the game's volumes stay as they are; a click too sharp to reach
it under the peak stays lower, and the takes of one candidate are set to one level.
Out: Assets/_Game/Presentation/Resources/Art/audio/pm95/<kind>-<letter><n>.wav (in the build: &ses...= flags) and
docs/art/pm95/onizleme/sesler.json (the listening page's list).
    python -I Tools/make_sfx95.py <downloads dir>      (numpy, scipy, soundfile)"""
import json, os, sys, uuid
import numpy as np, soundfile as sf
from scipy.signal import lfilter, resample_poly

DL = sys.argv[1]
OUT = 'Assets/_Game/Presentation/Resources/Art/audio/pm95'
OLD = 'Assets/_Game/Presentation/Resources/Art/audio'
SR = 44100
K = 'Kenney (kenney.nl), CC0'
KEN = {'casino': ('Casino Audio', 'https://kenney.nl/assets/casino-audio'), 'impact': ('Impact Sounds', 'https://kenney.nl/assets/impact-sounds'),
       'rpg': ('RPG Audio', 'https://kenney.nl/assets/rpg-audio'), 'ui': ('UI Audio', 'https://kenney.nl/assets/ui-audio'),
       'iface': ('Interface Sounds', 'https://kenney.nl/assets/interface-sounds'), 'jingles': ('Music Jingles', 'https://kenney.nl/assets/music-jingles')}
PATH = {'casino': 'casino-audio/Audio', 'impact': 'impact-sounds/Audio', 'rpg': 'rpg-audio/Audio', 'ui': 'ui-audio/Audio',
        'iface': 'interface-sounds/Audio', 'jingles': 'music-jingles/Audio'}


def src(pack, name):
    return os.path.join(DL, PATH[pack], name) if pack in PATH else os.path.join(DL, 'oga', pack, name)


def load(path):
    x, sr = sf.read(path, dtype='float64', always_2d=True)
    m = x.mean(axis=1)
    if sr != SR: m = resample_poly(m, SR, sr)
    return m


def trim(m, longest, fade=.02, onset=.02):
    a = np.abs(m); pk = a.max()
    on = max(0, int(np.argmax(a > pk * onset)) - int(.003 * SR))
    off = len(a) - int(np.argmax(a[::-1] > pk * .004))
    y = m[on:min(off, on + int(longest * SR))].copy()
    f = min(len(y) // 2, int(fade * SR))
    y[-f:] *= np.cos(np.linspace(0, np.pi / 2, f)) ** 2
    g = int(.002 * SR); y[:g] *= np.linspace(0, 1, g)
    return y


def kweight(x):
    """BS.1770 K-weighting at 44.1 kHz (the shelf and the high-pass)."""
    b1, a1 = [1.53090959, -2.65116903, 1.16916686], [1, -1.66375011, .71265753]
    b2, a2 = [1, -2, 1], [1, -1.98998479, .99000468]
    return lfilter(b2, a2, lfilter(b1, a1, x))


def loud(x):
    """The loudest 50 ms (K-weighted RMS, dB)."""
    k = kweight(np.concatenate([x, np.zeros(SR // 20)]))
    w = SR // 20
    c = np.convolve(k ** 2, np.ones(w) / w, 'valid')
    return 10 * np.log10(c.max() + 1e-12)


def layer(base, top, at, gain_db):
    """`top` laid over `base` from `at` s (at the base's own loudness + gain_db)."""
    top = top * 10 ** ((loud(base) + gain_db - loud(top)) / 20)
    s = int(at * SR)
    y = np.zeros(max(len(base), s + len(top)))
    y[:len(base)] += base; y[s:s + len(top)] += top
    return y


def end_of(m):
    a = np.abs(m); return (len(a) - int(np.argmax(a[::-1] > a.max() * .05))) / SR


# kind: (stands in for, extra dB, [(letter, name, credit, link, [take builders])])
def plain(pack, name, longest, fade=.02, onset=.02):
    return lambda: trim(load(src(pack, name)), longest, fade, onset)


def slide_pop(pack, name, pop, gain=-3):
    def make():
        s = trim(load(src(pack, name)), .25, .02, .12)   # from the slide's body (its faint start would come late)
        p = trim(load(src('pop-sounds', pop)), .12, .015)
        return layer(s, p, max(0, end_of(s) - .03), gain)
    return make


def thud_boing(thud, boing):
    def make():
        t = trim(load(src('impact', thud)), .25)
        b = trim(load(src('boings', boing)), .4, .08)
        return layer(t, b, .012, -9)
    return make


POPS = ['pop2.ogg', 'pop4.ogg', 'pop8.ogg', 'pop10.ogg']
OGA = 'https://opengameart.org/content/'
KINDS = {
    'cikis': ('pm72-tick.wav', -2, [
        ('a', 'Karton kayması', 'Kenney Casino Audio (card-slide), CC0', KEN['casino'][1],
         [plain('casino', f'card-slide-{i}.ogg', .25, .02, .12) for i in (1, 2, 4, 6)]),
        ('b', 'Karton kayması + yumuşak pop', 'Kenney Casino Audio (card-slide) + cogitollc "Pop sounds", CC0', KEN['casino'][1] + ' , ' + OGA + 'pop-sounds',
         [slide_pop('casino', f'card-slide-{i}.ogg', p) for i, p in zip((1, 2, 4, 6), POPS)]),
        ('c', 'Hafif whoosh + pop', 'artisticdude "Swishes Sound Pack" + cogitollc "Pop sounds", CC0', OGA + 'swishes-sound-pack , ' + OGA + 'pop-sounds',
         [(lambda w=w, p=p: layer(trim(load(src('swishes-sound-pack', f'swishes/swish-{w}.wav')), .2) * .5,
                                  trim(load(src('pop-sounds', p)), .12, .015), .07, 0)) for w, p in zip((10, 11, 12, 13), POPS)]),
        ('d', 'Kumaş hışırtısı', 'Kenney RPG Audio (cloth), CC0', KEN['rpg'][1],
         [plain('rpg', f'cloth{i}.ogg', .3, .06, .08) for i in (1, 2, 3)]),
        ('e', 'Yumuşak pop', 'cogitollc "Pop sounds" (iPhone kaydı), CC0', OGA + 'pop-sounds',
         [plain('pop-sounds', p, .12, .015) for p in POPS]),
    ]),
    'carpma': ('pm75-tok.wav', 0, [
        ('a', 'Yumuşak tok', 'Kenney Impact Sounds (impactSoft_medium), CC0', KEN['impact'][1],
         [plain('impact', f'impactSoft_medium_00{i}.ogg', .2, .04) for i in range(4)]),
        ('b', 'Kauçuk bonk', 'Kenney Impact Sounds (impactPunch_medium), CC0', KEN['impact'][1],
         [plain('impact', f'impactPunch_medium_00{i}.ogg', .28, .06) for i in (0, 1, 3, 4)]),
        ('c', 'Boing', 'ezduzziteh "Boings", CC0', OGA + 'boings',
         [plain('boings', n, .4, .08) for n in ('bung_01.mp3', 'bing_01_0.mp3', 'bing_02.mp3')]),
        ('d', 'Tok + hafif boing', 'Kenney Impact Sounds + ezduzziteh "Boings", CC0', KEN['impact'][1] + ' , ' + OGA + 'boings',
         [thud_boing(f'impactSoft_medium_00{i}.ogg', b) for i, b in ((0, 'bung_01.mp3'), (1, 'bing_01_0.mp3'), (2, 'bung_01.mp3'))]),
    ]),
    'bitis': ('popfull.wav', 0, [
        ('a', 'Pizzicato ta-da (yükselen)', 'Kenney Music Jingles (jingles_PIZZI10), CC0', KEN['jingles'][1],
         [plain('jingles', 'Pizzicato jingles/jingles_PIZZI10.ogg', 1.2, .1)]),
        ('b', 'Çelik davul ta-da', 'Kenney Music Jingles (jingles_STEEL00), CC0', KEN['jingles'][1],
         [plain('jingles', 'Steel jingles/jingles_STEEL00.ogg', 1.2, .1)]),
        ('c', 'Zil parıltısı', 'pwl "Bell dings/chimes", CC0', OGA + 'bell-dingschimes',
         [plain('bell-dingschimes', 'bell_ding3.wav', 1.4, .25)]),
    ]),
    'kazan': ('win.wav', 0, [
        ('a', 'Apple Cider zafer (f müziğiyle aynı takım)', 'Zane Little "Apple Cider Juiced!" (victory), CC0', OGA + 'apple-cider-juiced',
         [plain('apple-cider-juiced', 'victory_3.wav', 3.3, .35)]),
        ('b', 'Pizzicato zafer', 'Fupi "Win Jingle" (WinPizzicato), CC0', OGA + 'win-jingle',
         [plain('win-jingle', 'WinPizzicato.wav', 2.8, .3)]),
        ('c', 'Çelik davul zafer', 'Fupi "Win Jingle" (WinSteelDrums), CC0', OGA + 'win-jingle',
         [plain('win-jingle', 'WinSteelDrums.wav', 2.9, .3)]),
    ]),
    'tik': ('pm72-tick.wav', 0, [
        ('a', 'Fare tıkı', 'Kenney UI Audio (click), CC0', KEN['ui'][1],
         [plain('ui', f'click{i}.ogg', .08, .01) for i in (2, 4, 5)]),
        ('b', 'Gerçek düğme tıkı', 'owlishmedia "87 Clickety Clips", CC0', OGA + '87-clickety-clips',
         [plain('87-clickety-clips', f'click{i}.wav', .1, .015) for i in (72, 71, 12, 29)]),
        ('c', 'Yumuşak tok tık', 'Kenney Interface Sounds (drop), CC0', KEN['iface'][1],
         [plain('iface', f'drop_00{i}.ogg', .12, .03) for i in (2, 3, 4)]),
    ]),
}
DEFAULT = {'cikis': 'b', 'carpma': 'a', 'bitis': 'a', 'kazan': 'a', 'tik': 'b'}


META = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
    preloadAudioData: 1
  platformSettingOverrides: {{}}
  forceToMono: 1
  normalize: 0
  loadInBackground: 0
  ambisonic: 0
  3D: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta(path):
    """Unity's import settings: not normalised (the loudness above is the loudness heard), loaded with the clip."""
    m = path + '.meta'
    guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    open(m, 'w', newline='\n').write(META.format(guid=guid or uuid.uuid4().hex))


def main():
    os.makedirs(OUT, exist_ok=True)
    for f in os.listdir(OUT):
        if f.endswith('.wav'): os.remove(os.path.join(OUT, f))
    page = {}
    for kind, (old, extra, cands) in KINDS.items():
        o = load(os.path.join(OLD, old))
        target = loud(o / np.abs(o).max()) + extra   # as the game plays it: Unity normalises the old clips' peak
        page[kind] = []
        for letter, name, credit, link, takes in cands:
            files = []
            ys = []
            for make in takes:
                y = make()
                y *= 10 ** ((target - loud(y)) / 20)
                ys.append(y * min(1, .95 / np.abs(y).max()))     # a sharp click can't reach it under the peak
            level = min(loud(y) for y in ys)                       # the takes of one candidate: one level
            for n, y in enumerate(ys, 1):
                y = y * 10 ** ((level - loud(y)) / 20)
                path = f'{OUT}/{kind}-{letter}{n}.wav'
                sf.write(path, y.astype(np.float32), SR, subtype='PCM_16')
                meta(path)
                files.append({'file': f'{kind}-{letter}{n}.wav', 'seconds': round(len(y) / SR, 3), 'dB': round(loud(y) - target, 1)})
            page[kind].append({'letter': letter, 'name': name, 'credit': credit, 'link': link, 'default': DEFAULT[kind] == letter,
                               'takes': files})
            print(f'{kind}-{letter}: {name}: {len(files)} takes, ' + ', '.join(f"{t['seconds']:.2f} s ({t['dB']:+.1f} dB)" for t in files))
    os.makedirs('docs/art/pm95/onizleme', exist_ok=True)
    json.dump(page, open('docs/art/pm95/onizleme/sesler.json', 'w', encoding='utf8'), ensure_ascii=False, indent=1)


main()
