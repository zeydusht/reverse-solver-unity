"""PM_107 §C5: measures one scenario recorded by Tools/pm107_miks.js (<dir>: efektler.webm, muzik.webm, log.json).

Every sound the game played is a [mix] line in the log: "[mix] <bus> <clip> <volume> <t>" (Mix107.Log; a continuous
sound, the clock's tick-tock or the chain, logs its level as it changes). Each one's level in the mix is known: the
clip's own loudness (the loudest 400 ms of the file, LUFS, measured here from Resources) plus 20 log10(volume).
  duyulmayan ses  a sound more than 6 dB under the loudest other thing playing with it (another sound's expected
                  level over the same 400 ms, or the music as recorded): drowned. Each listed with what drowns it.
  tepe aşımı      a 10 ms stretch of the effects + music, as they reach the speaker, peaking over -1 dBFS.
Writes <dir>/olcum.json and <dir>/seviye.png (the short-term loudness of the effects, the music and the sum, each
sound's expected level as a dot). Prints the counts.
    python -I Tools/pm107_miks_olc.py <dir> [--clips <clip table json>]
"""
import json, os, re, subprocess, sys, glob
import numpy as np, soundfile as sf, pyloudnorm as pyln, imageio_ffmpeg
from scipy.signal import butter, sosfilt

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
AUDIO = os.path.join(ROOT, 'Assets', '_Game', 'Presentation', 'Resources', 'Art', 'audio')
SR = 44100
MASK_DB = 6.0
PEAK_DB = -1.0
d = sys.argv[1]
TABLE = sys.argv[sys.argv.index('--clips') + 1] if '--clips' in sys.argv else os.path.join(ROOT, 'docs', 'art', 'pm107', 'miks', 'klipler.json')


def clip_table():
    """name -> {lufs: loudest 400 ms, peak dBFS, seconds} for every wav under Resources/Art/audio (cached)."""
    files = sorted(glob.glob(os.path.join(AUDIO, '**', '*.wav'), recursive=True))
    stamp = max(os.path.getmtime(f) for f in files)
    if os.path.exists(TABLE) and os.path.getmtime(TABLE) >= stamp:
        return json.load(open(TABLE, encoding='utf-8'))
    out = {}
    for f in files:
        y, sr = sf.read(f, always_2d=True)
        y = y.mean(axis=1)
        out[os.path.splitext(os.path.basename(f))[0]] = dict(lufs=round(short_lufs(y, sr), 2), peak=round(20 * np.log10(np.max(np.abs(y)) + 1e-9), 2),
                                                             seconds=round(len(y) / sr, 3), folder=os.path.basename(os.path.dirname(f)))
    os.makedirs(os.path.dirname(TABLE), exist_ok=True)
    json.dump(out, open(TABLE, 'w', encoding='utf-8'), indent=1, ensure_ascii=False)
    return out


def short_lufs(y, sr):
    """The loudest 400 ms (LUFS, K-weighted); a sound shorter than that padded with silence."""
    meter = pyln.Meter(sr, block_size=.4)
    n = int(.4 * sr)
    if len(y) < n: y = np.concatenate([y, np.zeros(n - len(y))])
    best = -70.0
    hop = int(.1 * sr)
    for a in range(0, len(y) - n + 1, hop):
        try:
            v = meter.integrated_loudness(y[a:a + n])
        except ValueError:
            continue
        if np.isfinite(v): best = max(best, v)
    return best


raw = {}


def decode(name):
    src = os.path.join(d, name + '.webm')
    if not os.path.exists(src) or os.path.getsize(src) < 100: return None
    out = os.path.join(d, name + '.wav')
    subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), '-y', '-loglevel', 'error', '-i', src, '-af', 'aresample=async=1:first_pts=0', '-ac', '1', '-ar', str(SR), out], check=True)
    y, _ = sf.read(out)
    raw[name] = y.astype(float)   # the peaks: as recorded (the hum is 50 dB down; a high-pass would ring over them)
    return sosfilt(butter(4, 70, 'hp', fs=SR, output='sos'), y.astype(float))   # the loudness: the recorder's 25 Hz hum out


def curve(y, win=.4, hop=.05):
    """Short-term loudness (LUFS) every `hop` s over `win` s windows."""
    meter = pyln.Meter(SR, block_size=win)
    n, h = int(win * SR), int(hop * SR)
    t, v = [], []
    for a in range(0, max(1, len(y) - n), h):
        seg = y[a:a + n]
        try:
            l = meter.integrated_loudness(seg) if np.max(np.abs(seg)) > 1e-5 else -70
        except ValueError:
            l = -70
        t.append((a + n / 2) / SR); v.append(max(-70, l if np.isfinite(l) else -70))
    return np.array(t), np.array(v)


clips = clip_table()
fx = decode('efektler'); mu = decode('muzik')
# the music's recorder starts with its context (the first touch): its silence before that put back
meta = json.load(open(os.path.join(d, 'meta.json'))) if os.path.exists(os.path.join(d, 'meta.json')) else {}
lead = int(float(meta.get('musicStart') or 0) * SR)
if mu is not None and lead > 0:
    mu = np.concatenate([np.zeros(lead), mu])
    if 'muzik' in raw: raw['muzik'] = np.concatenate([np.zeros(lead), raw['muzik']])
if fx is None: sys.exit('efektler.webm yok')
n = max(len(fx), len(mu) if mu is not None else 0)
pad = lambda y: np.concatenate([y, np.zeros(n - len(y))]) if y is not None else np.zeros(n)
fx, mu = pad(fx), pad(mu)
mix = fx + mu
logs = json.load(open(os.path.join(d, 'log.json'), encoding='utf-8'))

# the sounds: [mix] <bus> <clip> <volume> (one-shots); a continuous one logs again as its level moves ("~" before the bus)
events = []
for x in logs:
    m = re.match(r'\[mix\] (~?)(\w+) (\S+) ([\d.,]+)', x['line'])
    if not m: continue
    cont, bus, name, vol = m.group(1) == '~', m.group(2), m.group(3), float(m.group(4).replace(',', '.'))
    c = clips.get(name)
    if c is None: continue
    level = c['lufs'] + 20 * np.log10(max(vol, 1e-4))
    events.append(dict(t=x['t'], bus=bus, clip=name, vol=vol, level=round(level, 1), len=c['seconds'], cont=cont))
# a continuous sound lasts until its next line (its level held), or .4 s
for i, e in enumerate(events):
    if e['cont']:
        nxt = [f['t'] for f in events[i + 1:] if f['cont'] and f['clip'].split('-')[0] == e['clip'].split('-')[0]]
        e['len'] = (nxt[0] - e['t']) if nxt else .4

tm, lm = curve(mu) if np.max(np.abs(mu)) > 0 else (np.array([0.0]), np.array([-70.0]))
music_at = lambda t: float(np.interp(t, tm, lm))
drowned = []
for e in events:
    if e['cont'] and e['vol'] < .15: continue   # a continuous sound fading in or out (the chain slowing): not an event
    a, b = e['t'], e['t'] + min(.4, e['len'])
    rivals = [(f['level'], f['clip']) for f in events if f is not e and f['t'] < b and f['t'] + min(.4, f['len']) > a and not (f['cont'] and f['vol'] < .02)]
    rivals.append((music_at(a + .2), 'müzik'))
    top = max(rivals)
    if e['level'] < top[0] - MASK_DB:
        drowned.append(dict(t=round(e['t'], 2), clip=e['clip'], level=e['level'], by=top[1], by_level=round(top[0], 1)))

# the peaks of what reaches the speaker (effects + music, unfiltered)
rf, rm = raw.get('efektler', np.zeros(0)), raw.get('muzik', np.zeros(0))
k = max(len(rf), len(rm))
mix = np.concatenate([rf, np.zeros(k - len(rf))]) + np.concatenate([rm, np.zeros(k - len(rm))])
w = int(.01 * SR)
blocks = np.abs(mix[:len(mix) // w * w]).reshape(-1, w).max(axis=1)
over = int(np.sum(20 * np.log10(blocks + 1e-12) > PEAK_DB))
peak = float(20 * np.log10(np.max(np.abs(mix)) + 1e-12))

tf, lf = curve(fx); ts, ls = curve(mix)
res = dict(dir=os.path.basename(os.path.normpath(d)), sounds=len(events), drowned=len(drowned), drowned_list=drowned,
           peaks_over=over, peak_dbfs=round(peak, 2), effects_lufs_max=round(float(lf.max()), 1), music_lufs_median=round(float(np.median(lm[lm > -60])) if np.any(lm > -60) else -70, 1),
           mix_lufs_max=round(float(ls.max()), 1),
           by_clip={k: dict(count=sum(1 for e in events if e['clip'] == k), level=max(e['level'] for e in events if e['clip'] == k)) for k in sorted({e['clip'] for e in events})})
json.dump(res, open(os.path.join(d, 'olcum.json'), 'w', encoding='utf-8'), indent=1, ensure_ascii=False)

import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
fig, ax = plt.subplots(figsize=(12, 4.2), dpi=110)
ax.plot(tm, lm, color='#3b6fb6', lw=1.2, label='müzik')
ax.plot(tf, lf, color='#e3a33b', lw=1.2, label='efektler')
ax.plot(ts, ls, color='#222', lw=.8, alpha=.6, label='toplam')
buses = sorted({e['bus'] for e in events})
cols = dict(zip(buses, ['#c0392b', '#27ae60', '#8e44ad', '#16a085', '#d35400', '#7f8c8d']))
for bname in buses:
    es = [e for e in events if e['bus'] == bname and not (e['cont'] and e['vol'] < .02)]
    ax.scatter([e['t'] for e in es], [e['level'] for e in es], s=14, color=cols[bname], label=bname, zorder=3)
for x in drowned: ax.annotate('duyulmuyor', (x['t'], x['level']), fontsize=7, color='#c0392b')
ax.set_ylim(-60, 0); ax.set_xlabel('s'); ax.set_ylabel('LUFS (400 ms)')
ax.set_title(f"{res['dir']}: {len(events)} ses · duyulmayan {len(drowned)} · tepe aşımı {over} (tepe {peak:.1f} dBFS)")
ax.grid(alpha=.25); ax.legend(loc='lower right', fontsize=7, ncol=4)
fig.tight_layout(); fig.savefig(os.path.join(d, 'seviye.png'))
print(f"[miks] {res['dir']}: ses {len(events)}, duyulmayan {len(drowned)}, tepe aşımı {over} (tepe {peak:.1f} dBFS), efekt en yüksek {res['effects_lufs_max']} LUFS, müzik {res['music_lufs_median']} LUFS")
for x in drowned[:12]: print(f"   duyulmuyor {x['t']:7.2f} s  {x['clip']} {x['level']} <- {x['by']} {x['by_level']}")
