"""PM_102 §2 and §5: measures Tools/pm102_record.js's recordings.
  jams   each jam's knock (the loudest sound after it): its peak (dBFS); the spread over all of them (<= 1 dB wanted)
  gecis  each transition: its motion's moment ([gecis] olay ...), its sound's log line ([gecis] ses ...) and where that
         sound starts in the recording; the corridor's whoosh: its loudest moment against the slide's middle, and its
         loudness against the music's over the slide
Writes <dir>/olcum.json and prints a table.
    python -I Tools/pm102_measure.py <dir>          (imageio-ffmpeg's ffmpeg; numpy, soundfile, scipy, pyloudnorm)"""
import json, os, re, subprocess, sys
import numpy as np, soundfile as sf, pyloudnorm as pyln, imageio_ffmpeg
from scipy.signal import butter, sosfilt

d = sys.argv[1]
SR = 44100


def decode(name):
    src = os.path.join(d, name + '.webm')
    if not os.path.exists(src) or os.path.getsize(src) < 100: return None
    out = os.path.join(d, name + '.wav')
    subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), '-y', '-loglevel', 'error', '-i', src, '-af', 'aresample=async=1:first_pts=0', '-ac', '1', '-ar', str(SR), out], check=True)
    y, _ = sf.read(out)
    return sosfilt(butter(4, 70, 'hp', fs=SR, output='sos'), y.astype(float))   # the recorder's 25 Hz hum out


y = decode('efektler'); music = decode('muzik')
env = np.sqrt(np.convolve(y ** 2, np.ones(220) / 220, 'same'))
floor = np.percentile(env, 30) + 1e-6
info = json.load(open(os.path.join(d, 'moves.json')))
logs = json.load(open(os.path.join(d, 'log.json')))
db = lambda v: 20 * np.log10(v + 1e-12)
res = {}


def onset(t, within=.35):
    """Where a sound starts at or after t - 30 ms: the 5 ms envelope rising over 6x the floor (s), or None."""
    a, b = max(0, int((t - .03) * SR)), min(len(env), int((t + within) * SR))
    idx = np.nonzero(env[a:b] > floor * 6)[0]
    return (a + idx[0]) / SR if len(idx) else None


def lufs(x):
    if x is None or len(x) < int(.4 * SR) or np.abs(x).max() < 1e-5: return None
    return round(pyln.Meter(SR, block_size=.4).integrated_loudness(np.stack([x, x], 1)), 1)


if info['mode'] == 'jams':
    rows = []
    for m in info['moves']:
        a, b = int(m['at'] * SR), min(len(y), int((m['release'] + .9) * SR))
        seg = y[a:b]
        if len(seg) == 0 or np.abs(seg).max() < floor * 8: rows.append(dict(m, peak=None)); continue
        rows.append(dict(m, peak=round(float(db(np.abs(seg).max())), 2)))
    for kind in ('jam', 'nudge'):
        ps = [r['peak'] for r in rows if r['what'] == kind and r['peak'] is not None]
        if ps: print(f'{kind:5s} {len(ps)} takılma: tepe {min(ps):.2f} .. {max(ps):.2f} dBFS, fark {max(ps) - min(ps):.2f} dB')
    allp = [r['peak'] for r in rows if r['what'] in ('jam', 'nudge') and r['peak'] is not None]
    res = dict(rows=rows, count=len(allp), spread_db=round(max(allp) - min(allp), 2) if allp else None,
               peaks=allp)
    print(f'hepsi: {len(allp)} takılma, tepeler arası fark {res["spread_db"]} dB')
else:
    table = []
    for i, x in enumerate(logs):
        g = re.match(r'\[gecis\] ses (\w+)', x['line'])
        if not g: continue
        kind, t = g.group(1), x['t']
        ev = next((e for e in reversed(logs[:i + 1]) if e['line'].startswith('[gecis] olay') and t - .25 <= e['t'] <= t + .01), None)
        s = onset(t)
        table.append(dict(sound=kind, event=ev['line'][len('[gecis] olay '):] if ev else '-', event_t=round(ev['t'], 3) if ev else None,
                          log_t=round(t, 3), sound_t=round(s, 3) if s else None, delta_ms=round(1000 * (s - (ev['t'] if ev else t))) if s else None))
    for x in logs:
        if x['line'].startswith('[gecis] olay') and not any(r['event_t'] == round(x['t'], 3) for r in table):
            table.append(dict(sound='-', event=x['line'][len('[gecis] olay '):], event_t=round(x['t'], 3), log_t=None, sound_t=None, delta_ms=None))
    table.sort(key=lambda r: r['event_t'] if r['event_t'] is not None else r['log_t'])
    for r in table: print(f"{r['event']:34s} olay {r['event_t']}  ses {r['sound']:8s} başlangıç {r['sound_t']}  fark {r['delta_ms']} ms")
    k = next((r for r in table if r['sound'] == 'Koridor'), None)
    if k and k['sound_t']:
        a = int(k['sound_t'] * SR); b = a + int(1.5 * SR)
        w = int(.15 * SR); e = np.sqrt(np.convolve(y[a:b] ** 2, np.ones(w) / w, 'same')); top = (a + int(np.argmax(e))) / SR   # a 150 ms envelope
        res['corridor'] = dict(start=k['sound_t'], loudest_at=round(top - k['sound_t'], 3), slide_middle=.65,
                               whoosh_lufs=lufs(y[a + int(.3 * SR):a + int(1.0 * SR)]),
                               music_lufs=lufs(music[a + int(.3 * SR):a + int(1.0 * SR)]) if music is not None and len(music) > b else None)
        print('koridor:', res['corridor'])
    res['table'] = table
json.dump(res, open(os.path.join(d, 'olcum.json'), 'w', encoding='utf8'), ensure_ascii=False, indent=1)
