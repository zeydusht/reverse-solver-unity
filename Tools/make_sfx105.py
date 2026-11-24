"""PM_105 §2: the obstacles' sounds, three candidates each (a-c), recorded CC0 (docs/art/pm105/LISANS.md):
  zincir      chain rattle grains (0.06-0.14 s), played while a chain moves: their rate and level follow its speed
  zincirger   the chain pulled taut at a chained pair's knock: a short metal ting, a rattle 25 ms on
  kilitdokun  a press on a locked piece: a short locked rattle (<= 0.25 s)
  kilitac     a lock opening: a key turning, then the padlock dropping
  patlama     a bomb going off: a toy's pop and puff (no deep boom: high-passed at 150 Hz, the top softened), <= 0.7 s
  duvar       a knock on the red wall: a firmer bonk than the knock on a piece, in the range a phone plays (no sub)
Levels against what the game plays: the knock (pm102/carpma-a*), the slide (pm100/cikis-f*) and the button tok
(pm104/ui-tok-a1), the loudest 400 ms in LUFS (as Tools/make_sfx104.py).
  zincir -9 dB under the slide at full speed (the game plays it 25-100 % with the speed); zincirger at the knock
  (the game plays it 3 dB under); kilitdokun the knock -2; kilitac the slide; patlama the knock +1; duvar the knock +1.5.
Out: Resources/Art/audio/pm105/<key>-<letter><n>.wav and docs/art/pm105/onizleme/sesler/ + sesler105.json.
    python -I Tools/make_sfx105.py <Kenney packs dir (impact-sounds, rpg-audio, interface-sounds)> <OpenGameArt dir (p95/dl/oga)> <pm105 downloads dir>"""
import json, os, sys, uuid, glob
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln
from scipy.signal import butter, sosfilt

SR = 44100
KEN, OGA, DL = sys.argv[1], sys.argv[2], sys.argv[3]
RES = 'Assets/_Game/Presentation/Resources/Art/audio'
OUT = os.path.join(RES, 'pm105')
PAGE = 'docs/art/pm105/onizleme'
META = open(os.path.join(RES, 'pm104', 'ui-tok-a1.wav.meta'), encoding='utf8').read()


def read(p): return librosa.load(p, sr=SR, mono=True)[0].astype(np.float64)


def loud(x):
    m = pyln.Meter(SR, block_size=.4); w = int(.4 * SR)
    y = np.concatenate([np.asarray(x), np.zeros(w)]); best = -99.0
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


def level(x, target, ceil_db=-2.0):
    c = 10 ** (ceil_db / 20)
    for _ in range(12): x = limit(x * 10 ** ((target - loud(x)) / 20), c)
    return x


def hp(x, f, order=2): return sosfilt(butter(order, f, 'hp', fs=SR, output='sos'), x)
def lp(x, f, order=2): return sosfilt(butter(order, f, 'lp', fs=SR, output='sos'), x)


def fades(x, fin=.003, fout=.03):
    x = x.copy(); a, b = int(fin * SR), min(len(x) // 2, int(fout * SR))
    if a: x[:a] *= np.linspace(0, 1, a) ** 2
    if b: x[-b:] *= np.linspace(1, 0, b) ** 2
    return x


def onset_trim(x, lead=.002, thr=.05):
    e = np.convolve(np.abs(x), np.ones(88) / 88, 'same'); on = int(np.argmax(e > e.max() * thr))
    return x[max(0, on - int(lead * SR)):]


def tail_trim(x, thr=.02):
    e = np.convolve(np.abs(x), np.ones(88) / 88, 'same'); idx = np.where(e > e.max() * thr)[0]
    return x[:idx[-1] + 1] if len(idx) else x


def shift(x, semis):
    r = 2 ** (semis / 12); t = np.arange(int(len(x) / r)) * r
    return np.interp(t, np.arange(len(x)), x)


def cut(x, seconds, fout=.03): return fades(tail_trim(onset_trim(x))[:int(seconds * SR)], .002, fout)


def mix(a, b, at, gain):
    """a, then b from `at` seconds on at `gain` dB."""
    i = int(at * SR); n = max(len(a), i + len(b)); y = np.zeros(n)
    y[:len(a)] += a; y[i:i + len(b)] += b * 10 ** (gain / 20)
    return y


def grains(p, n, length=.11, gap=.05):
    """The n loudest separate hits of a recording, each `length` long from just before its start."""
    x = read(p); w = int(.008 * SR)
    e = np.sqrt(np.convolve(x ** 2, np.ones(w) / w, 'same'))
    out, used = [], np.zeros(len(x), bool)
    for _ in range(n):
        e2 = np.where(used, 0, e); i = int(np.argmax(e2))
        if e2[i] < e.max() * .12: break
        a = max(0, i - int(.006 * SR)); b = min(len(x), a + int(length * SR))
        used[max(0, a - int(gap * SR)):b] = True
        out.append(fades(x[a:b], .002, .035))
    return out


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


# the game's own levels
KNOCK = float(np.mean([loud(read(os.path.join(RES, 'pm102', f'carpma-a{n}.wav'))) for n in range(1, 5)]))
SLIDE = float(np.mean([loud(read(os.path.join(RES, 'pm100', f'cikis-f{n}.wav'))) for n in range(1, 5)]))
TOK = loud(read(os.path.join(RES, 'pm104', 'ui-tok-a1.wav')))
print(f'çarpma a {KNOCK:.1f} LUFS, kayma f {SLIDE:.1f}, tok a {TOK:.1f}')

rpg = lambda f: os.path.join(KEN, 'rpg-audio', 'Audio', f)
imp = lambda f: os.path.join(KEN, 'impact-sounds', 'Audio', f)
r80 = lambda f: os.path.join(DL, '80-CC0-RPG-SFX_0', f)
wm = lambda f: os.path.join(DL, '100-CC0-wood-metal-SFX', f)
c100 = lambda f: os.path.join(DL, '100-CC0-SFX_0', f)
bang = lambda f: os.path.join(DL, '25-CC0-bang-sfx', f)
door = lambda f: os.path.join(DL, 'DoorLockSounds', 'DoorLockSounds', f.replace('.wav', '.ogg'))
pops = lambda f: os.path.join(OGA, 'pop-sounds', f)
RD = lambda pack, slug: (f'rubberduck, "{pack}" (OpenGameArt, CC0)', f'https://opengameart.org/content/{slug}')
R80, RWM, R100, RBANG = RD('80 CC0 RPG SFX', '80-cc0-rpg-sfx'), RD('100 CC0 metal and wood SFX', '100-cc0-metal-and-wood-sfx'), RD('100 CC0 SFX', '100-cc0-sfx'), RD('25 CC0 bang / firework SFX', '25-cc0-bang-firework-sfx')
DOOR = ('cough-e, "Door Lock Sounds" (OpenGameArt, CC0)', 'https://opengameart.org/content/door-lock-sounds')
POPS = ('cogitollc, "Pop sounds" (OpenGameArt, CC0)', 'https://opengameart.org/content/pop-sounds')
KRPG, KIMP = ('Kenney, RPG Audio (CC0)', 'https://kenney.nl/assets/rpg-audio'), ('Kenney, Impact Sounds (CC0)', 'https://kenney.nl/assets/impact-sounds')


def nth(p, n):
    g = grains(p, 3)
    return g[n % len(g)]


def chain_grain(x): return fades(lp(hp(x, 500, 2), 9000, 2), .002, .035)


def rattle(files, n=4, length=.11):
    out = []
    for p in files: out += grains(p, n, length)
    return [chain_grain(g) for g in out]


def ting_rattle(ting, grain, gain=-6):
    t = fades(hp(tail_trim(onset_trim(ting))[:int(.22 * SR)], 400, 2), .002, .08)
    return mix(t, chain_grain(grain), .025, gain)


def key_drop(key, drop, at, keylen=.42, gain=-4):
    k = fades(hp(tail_trim(onset_trim(key))[:int(keylen * SR)], 250, 2), .002, .05)
    d = fades(hp(tail_trim(onset_trim(drop))[:int(.25 * SR)], 250, 2), .001, .08)
    return mix(k, d, at, gain)


def toy_bang(x, semis=0, seconds=.6, puff=None):
    y = tail_trim(onset_trim(x))[:int(seconds * SR)]
    if semis: y = shift(y, semis)
    y = lp(hp(y, 150, 3), 6500, 2)
    if puff is not None: y = mix(fades(hp(onset_trim(puff), 300, 2)[:int(.12 * SR)], .001, .04), y, .012, 0)
    return fades(y, .002, .2)


def bonk(x, semis=0, seconds=.2):
    y = tail_trim(onset_trim(x, lead=.001))[:int(seconds * SR)]
    if semis: y = shift(y, semis)
    return fades(lp(hp(y, 180, 3), 6000, 2), .001, .07)


SETS = {
    'zincir': ('Zincir şıkırtısı (sürüklerken / sallanırken)', SLIDE - 9, [
        ('a', 'Zincir halkaları', R80, lambda: rattle([r80('chain_01.ogg'), r80('chain_02.ogg'), r80('chain_03.ogg')], 2)),
        ('b', 'Anahtar destesi', RWM, lambda: rattle([wm('keys_05.ogg'), wm('keys_06.ogg'), wm('keys_07.ogg')], 2)),
        ('c', 'Toka ve metal ufaklık', KRPG, lambda: rattle([rpg('beltHandle1.ogg'), rpg('beltHandle2.ogg'), rpg('handleCoins2.ogg')], 2)),
    ]),
    'zincirger': ('Zincir gerilip takılınca (çın + şıkırtı)', KNOCK, [
        ('a', 'İnce metal çın + zincir', RWM, lambda: [ting_rattle(read(wm(f'metal_hit_0{n}.ogg')), nth(r80('chain_01.ogg'), n)) for n in (1, 4, 5)]),
        ('b', 'Hafif metal vuruş + toka', KIMP, lambda: [ting_rattle(read(imp(f'impactMetal_light_00{n}.ogg')), nth(rpg('beltHandle2.ogg'), n)) for n in (0, 2, 4)]),
        ('c', 'Metal tını + anahtar', R80, lambda: [ting_rattle(read(r80(f'metal_0{n}.ogg')), nth(wm('keys_05.ogg'), n)) for n in (1, 3)]),
    ]),
    'kilitdokun': ('Kilitli parçaya dokununca', KNOCK - 2, [
        ('a', 'Kilitli kapı kolu', DOOR, lambda: [fades(hp(g, 250, 2), .002, .05) for g in grains(door('LockedDoorHandleJiggle.wav'), 3, .2, .15)]),
        ('b', 'Kilit tıkırtısı', R80, lambda: [cut(hp(read(r80(f'lock_0{n}.ogg')), 250, 2), .22, .06) for n in (1, 3)]),
        ('c', 'Metal mandal', KRPG, lambda: [cut(hp(read(rpg('metalLatch.ogg')), 250, 2), .18, .05), cut(hp(read(rpg('metalClick.ogg')), 250, 2), .2, .06)]),
    ]),
    'kilitac': ('Kilit açılınca (anahtar dönüşü + kilidin düşüşü)', SLIDE, [
        ('a', 'Anahtar + metal düşüş', R100, lambda: [key_drop(read(c100('key_open_01.ogg')), read(imp('impactMetal_light_001.ogg')), .26),
                                                      key_drop(read(c100('key_open_02.ogg')), read(imp('impactMetal_light_004.ogg')), .2)]),
        ('b', 'Kapı kilidi açılır + düşüş', DOOR, lambda: [key_drop(read(door('UnlockDoor.wav')), read(imp('impactMetal_light_002.ogg')), .34, .45)]),
        ('c', 'Asma kilit açılır', RWM, lambda: [key_drop(read(wm('lock_open_01.ogg')), read(imp('impactMetal_light_000.ogg')), .42, .5, -6)]),
    ]),
    'patlama': ('Bomba patlayınca (oyuncak tadında)', KNOCK + 1, [
        ('a', 'Yumuşak patlama + pof', R100, lambda: [toy_bang(read(c100('explosion.ogg')), 2, .5, read(pops('pop3.ogg')))]),
        ('b', 'Kısa bang, oyuncak', RBANG, lambda: [toy_bang(read(bang('bang_05.ogg')), 4, .45), toy_bang(read(bang('bang_08.ogg')), 4, .45)]),
        ('c', 'Konfeti patlaması', RBANG, lambda: [toy_bang(read(bang('fw_05.ogg')), 3, .6, read(pops('pop1.ogg')))]),
    ]),
    'duvar': ('Kırmızı duvara çarpınca (bonk)', KNOCK + 1.5, [
        ('a', 'Ahşap bonk (çarpmadan tok)', KIMP, lambda: [bonk(read(imp(f'impactWood_heavy_00{n}.ogg')), 7) for n in (0, 2, 3)]),
        ('b', 'Ahşap vuruş', RWM, lambda: [bonk(read(wm(f'wood_hit_0{n}.ogg'))) for n in (1, 3, 5)]),
        ('c', 'Yumuşak ağır vuruş', KIMP, lambda: [bonk(read(imp(f'impactSoft_heavy_00{n}.ogg')), 9, .22) for n in (0, 1, 4)]),
    ]),
}

for old in glob.glob(os.path.join(OUT, '*.wav')) + glob.glob(os.path.join(PAGE, 'sesler', '*.wav')):
    os.remove(old)
page = {}
for key, (title, target, cands) in SETS.items():
    page[key] = dict(title=title, lufs=round(target, 1), candidates=[])
    for letter, name, (credit, link), make in cands:
        takes = make()[:4]
        entry = dict(letter=letter, name=name, credit=credit, link=link, takes=[])
        for n, x in enumerate(takes, 1):
            y = level(x, target)
            f = f'{key}-{letter}{n}.wav'
            write(OUT, f, y); write(os.path.join(PAGE, 'sesler'), f, y)
            entry['takes'].append(dict(file=f, seconds=round(len(y) / SR, 3), lufs=round(loud(y), 1), peak=round(peak(y), 1), centroid=int(centroid(y))))
        page[key]['candidates'].append(entry)
        print(f'{key} {letter} {name}: ' + ', '.join(f"{t['seconds']:.2f} sn {t['lufs']} LUFS {t['centroid']} Hz" for t in entry['takes']))
json.dump(page, open(os.path.join(PAGE, 'sesler105.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
print('yazıldı:', OUT, PAGE)
