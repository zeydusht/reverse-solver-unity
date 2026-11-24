"""PM_106 §2-3: the exit in Block Out's manner (docs/art/pm106/BLOCKOUT_SES_ANALIZ.md) and a real chain.

  cikis q-u   the exit as the piece crosses the edge: one take each, no random pitch. Block Out's release is a 12-16 ms
              wooden/plastic "tok" (energy at ~395 Hz) and its exit a ~0.6 s plastic crumble; silence while dragging.
                q  tok + plastic crumble (Block Out's two sounds in one)
                r  the tok alone (Block Out's release)
                s  a plastic clack + crumble
                t  a pop + a short sparkle (PM's hypothesis 2)
                u  the tok + Block Out's long crumble
  zincir d-f  a chain rattling while it moves: three seamless loops of ~1.3 s each from one real chain recording
              (the game crossfades from one to another, never the same twice; the level follows the chain's speed)
  zincirger d-f  the chain pulled taut: a short metal ting and a burst of that chain

Why PM_105's chain did not sound like one (measured): its grains were 0.11 s single hits of a 0.5 s shake whose energy
sat at 8.8 kHz (no link body), high-passed at 500 Hz, at -24 LUFS and played at 25-100 % of that with the speed, one
grain per half cell: scattered ticks and hiss, not links rubbing.

Levels against the game's: the knock (pm102/carpma-a*), the loudest 400 ms in LUFS. The exits at the knock +1 (the
game plays the exit 1 dB down: GameRoot.ExitLevel); the chain's loops 5 dB under the knock at full speed (the game
plays them 25-100 % with the speed); zincirger at the knock (the game plays it 3 dB under).
Mono files (half the web build's raw samples).
Out: Resources/Art/audio/pm106/ and docs/art/pm106/onizleme/sesler/ + sesler106.json.
    python -I Tools/make_sfx106.py <Kenney packs dir (impact-sounds, casino-audio)> <OpenGameArt dir (p95/dl/oga)> <pm105 downloads dir> <Freesound previews dir (<id>.mp3)>"""
import json, os, sys, uuid, glob
import numpy as np, soundfile as sf, librosa, pyloudnorm as pyln
from scipy.signal import butter, sosfilt

SR = 44100
KEN, OGA, DL, FS = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
RES = 'Assets/_Game/Presentation/Resources/Art/audio'
OUT = os.path.join(RES, 'pm106')
PAGE = 'docs/art/pm106/onizleme'
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


def mix(a, b, at, gain):
    i = int(at * SR); n = max(len(a), i + len(b)); y = np.zeros(n)
    y[:len(a)] += a; y[i:i + len(b)] += b * 10 ** (gain / 20)
    return y


def write(folder, name, x):
    os.makedirs(folder, exist_ok=True); path = os.path.join(folder, name)
    sf.write(path, x, SR, subtype='PCM_16')
    if folder != OUT: return
    m = path + '.meta'; guid = None
    if os.path.exists(m):
        for line in open(m):
            if line.startswith('guid:'): guid = line.split()[1]
    lines = [f'guid: {guid or uuid.uuid4().hex}' if l.startswith('guid:') else l for l in META.splitlines()]
    open(m, 'w', newline='\n').write('\n'.join(lines) + '\n')


KNOCK = float(np.mean([loud(read(os.path.join(RES, 'pm102', f'carpma-a{n}.wav'))) for n in range(1, 5)]))
print(f'çarpma a {KNOCK:.1f} LUFS')

imp = lambda f: os.path.join(KEN, 'impact-sounds', 'Audio', f)
cas = lambda f: os.path.join(KEN, 'casino-audio', 'Audio', f)
pops = lambda f: os.path.join(OGA, 'pop-sounds', f)
wm = lambda f: os.path.join(DL, '100-CC0-wood-metal-SFX', f)
r80 = lambda f: os.path.join(DL, '80-CC0-RPG-SFX_0', f)
fsd = lambda i: os.path.join(FS, f'{i}.mp3')
KIMP, KCAS = ('Kenney, Impact Sounds (CC0)', 'https://kenney.nl/assets/impact-sounds'), ('Kenney, Casino Audio (CC0)', 'https://kenney.nl/assets/casino-audio')
POPS = ('cogitollc, "Pop sounds" (OpenGameArt, CC0)', 'https://opengameart.org/content/pop-sounds')
RWM = ('rubberduck, "100 CC0 metal and wood SFX" (OpenGameArt, CC0)', 'https://opengameart.org/content/100-cc0-metal-and-wood-sfx')
R80 = ('rubberduck, "80 CC0 RPG SFX" (OpenGameArt, CC0)', 'https://opengameart.org/content/80-cc0-rpg-sfx')
FS1 = ('alienistcog, "chain-rattle-small.aif" (Freesound, CC0)', 'https://freesound.org/s/124399/')
FS2 = ('freemaster2, "Chains 2" (Freesound, CC0)', 'https://freesound.org/s/348974/')
FS3 = ('Kneeling, "pickingchain.ogg" (Freesound, CC0)', 'https://freesound.org/s/643863/')


# ---- the exit: Block Out's tok and crumble ----
def tok(x, semis=2, body=.011, ring=.07, ring_db=-24, seconds=.2):
    """A wooden knock made Block Out's: the body down 20 dB in ~13 ms, a faint ring behind it."""
    y = onset_trim(x, lead=.001)
    if semis: y = shift(y, semis)
    y = y[:int(seconds * SR)]
    t = np.arange(len(y)) / SR
    env = np.maximum(np.exp(-np.maximum(0, t - .003) / body), 10 ** (ring_db / 20) * np.exp(-t / ring))
    return fades(lp(hp(y * env, 110, 2), 8000, 2), .0008, .03)


def crumble(x, seconds, hpf=350, semis=-3, top=4500):
    """Plastic bits pouring: a casino chips recording from its start, a little lower and darker (Block Out's crumble
    sits at 1-3 kHz; the chips at 5-6), high-passed, fading out."""
    y = tail_trim(onset_trim(x))
    if semis: y = shift(y, semis)
    y = y[:int(seconds * SR)]
    return fades(lp(hp(y, hpf, 2), top, 2), .002, seconds * .45)


def sparkle(x, seconds=.22):
    y = tail_trim(onset_trim(x))[:int(seconds * SR)]
    return fades(hp(y, 3000, 2), .001, seconds * .6)


def pop(x, semis=0, seconds=.09):
    y = tail_trim(onset_trim(x, lead=.001))[:int(seconds * SR)]
    if semis: y = shift(y, semis)
    return fades(hp(y, 150, 2), .001, .03)


# ---- the chain: seamless loops from a real recording ----
def loops(path, n=3, seconds=1.3, hpf=250, skip=0.0):
    """The n steadiest busy stretches of a chain recording (a 50 ms rms high and even), each made a seamless loop
    (its last 90 ms crossfaded into its head, equal power)."""
    x = read(path)
    x = lp(hp(x, hpf, 2), 9000, 2)   # PM_105's chain was all hiss (8.8 kHz): keep the links' clink, not the air
    w = int(.05 * SR); r = np.sqrt(np.convolve(x ** 2, np.ones(w) / w, 'same'))[::w]
    L = int(seconds / .05); best = []
    used = np.zeros(len(r), bool)
    lo = int(skip / .05)
    for _ in range(n):
        score = np.full(len(r), -1e9)
        for i in range(lo, len(r) - L):
            if used[i:i + L].any(): continue
            seg = r[i:i + L]
            if seg.max() < 1e-6: continue
            score[i] = np.log(seg.mean() + 1e-9) + 1.5 * np.log(np.percentile(seg, 20) / (seg.max() + 1e-9) + 1e-3)
        i = int(np.argmax(score))
        if score[i] < -1e8: break
        used[max(0, i - L // 2):i + L + L // 2] = True
        fade = int(.09 * SR)
        a, b = i * w, i * w + int(seconds * SR) + fade
        seg = x[a:b].copy()
        head, tail = seg[:fade], seg[-fade:]
        k = np.linspace(0, np.pi / 2, fade)
        body = seg[:-fade].copy()
        body[:fade] = head * np.sin(k) + tail * np.cos(k)
        # even it out inside the loop (a soft 150 ms compressor), so one loud clink does not stand out each turn
        e = np.sqrt(np.convolve(body ** 2, np.ones(int(.15 * SR)) / int(.15 * SR), 'same'))
        g = (np.median(e) / np.maximum(e, 1e-9)) ** .75
        best.append(body * np.clip(g, .35, 3))
    return best


def burst(path, seconds=.28, hpf=250):
    """The loudest clatter of a chain recording, from just before it."""
    x = lp(hp(read(path), hpf, 2), 12000, 2)
    w = int(.02 * SR); e = np.sqrt(np.convolve(x ** 2, np.ones(w) / w, 'same'))
    i = int(np.argmax(e)); a = max(0, i - int(.01 * SR))
    return fades(x[a:a + int(seconds * SR)], .002, .12)


def ting_burst(ting, b, gain=-5):
    t = fades(hp(tail_trim(onset_trim(ting))[:int(.22 * SR)], 400, 2), .002, .08)
    return mix(t, b, .02, gain)


SETS = {
    'cikis': ('Sürükleme / çıkış (Block Out tarzı)', KNOCK + 1, [
        ('q', 'Tok + plastik kırıntı', KIMP, lambda: [mix(tok(read(imp('impactWood_light_000.ogg'))), crumble(read(cas('chips-handle-4.ogg')), .28), .012, -9)]),
        ('r', 'Yalın tok (Block Out bırakma)', KIMP, lambda: [tok(read(imp('impactWood_light_002.ogg')), seconds=.18)]),
        ('s', 'Plastik şak + kırıntı', KCAS, lambda: [mix(mix(fades(hp(onset_trim(read(cas('chips-collide-1.ogg')), lead=.001)[:int(.1 * SR)], 200, 2), .0008, .04),
                                                            tok(read(imp('impactWood_light_001.ogg')), seconds=.12), 0, -6), crumble(read(cas('chips-stack-3.ogg')), .22), .015, -11)]),
        ('t', 'Pop + parıltı', POPS, lambda: [mix(pop(read(pops('pop2.ogg')), -1), sparkle(read(imp('impactGlass_light_000.ogg'))), .025, -13)]),
        ('u', 'Tok + uzun ufalanma (Block Out çıkışı)', KCAS, lambda: [mix(tok(read(imp('impactWood_light_000.ogg'))), crumble(read(cas('chips-handle-5.ogg')), .55, 300), .01, -5)]),
    ]),
    'zincir': ('Zincir şıkırtısı (gerçek zincir, sürüklerken / sallanırken)', KNOCK - 5, [
        ('d', 'Küçük zincir', FS1, lambda: loops(fsd(124399), 3, skip=1.0)),
        ('e', 'Zincir (orta)', FS2, lambda: loops(fsd(348974), 3)),
        ('f', 'Zincir elde', FS3, lambda: loops(fsd(643863), 3, seconds=1.1)),
    ]),
    'zincirger': ('Zincir gerilip takılınca (çın + gerçek zincir)', KNOCK, [
        ('d', 'Metal çın + küçük zincir', FS1, lambda: [ting_burst(read(wm('metal_hit_01.ogg')), burst(fsd(124399))), ting_burst(read(wm('metal_hit_04.ogg')), burst(fsd(124399), .24))]),
        ('e', 'Metal vuruş + zincir', FS2, lambda: [ting_burst(read(imp('impactMetal_light_000.ogg')), burst(fsd(348974))), ting_burst(read(imp('impactMetal_light_002.ogg')), burst(fsd(348974), .24))]),
        ('f', 'Metal tını + zincir elde', FS3, lambda: [ting_burst(read(r80('metal_01.ogg')), burst(fsd(643863))), ting_burst(read(r80('metal_03.ogg')), burst(fsd(643863), .24))]),
    ]),
}
LOOPS = {'zincir'}

for old in glob.glob(os.path.join(OUT, '*.wav')) + glob.glob(os.path.join(PAGE, 'sesler', '*.wav')):
    os.remove(old)
page = {}
for key, (title, target, cands) in SETS.items():
    page[key] = dict(title=title, lufs=round(target, 1), candidates=[])
    for letter, name, (credit, link), make in cands:
        takes = make()[:4]
        entry = dict(letter=letter, name=name, credit=credit, link=link, takes=[])
        for n, x in enumerate(takes, 1):
            # a loop keeps its seam: no fades (the limiter's gain moves slowly, the seam stays smooth)
            y = level(x, target)
            f = f'{key}-{letter}{n}.wav'
            write(OUT, f, y); write(os.path.join(PAGE, 'sesler'), f, y)
            entry['takes'].append(dict(file=f, seconds=round(len(y) / SR, 3), lufs=round(loud(y), 1), peak=round(peak(y), 1), centroid=int(centroid(y))))
        page[key]['candidates'].append(entry)
        print(f'{key} {letter} {name}: ' + ', '.join(f"{t['seconds']:.2f} sn {t['lufs']} LUFS {t['peak']} dB {t['centroid']} Hz" for t in entry['takes']))
json.dump(page, open(os.path.join(PAGE, 'sesler106.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
print('yazıldı:', OUT, PAGE)
