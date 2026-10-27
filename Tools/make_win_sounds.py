"""Three candidate win sounds, synthesised from scratch (no samples: public domain / CC0 by construction).
A bell arpeggio C-E-G-C + shimmer, B marimba tune + applause-like sparkle, C pop + rising magic sparkle.
Levels matched to the game's sounds (RMS ~0.10, peak <= 0.5). Writes Assets/Audio/WinCandidates/win-{a,b,c}.wav
and docs/sounds/index.html (test page). Run: python Tools/make_win_sounds.py"""
import math, random, struct, wave, os, base64

SR = 44100
random.seed(20261027)

def note(f):  # MIDI -> Hz
    return 440 * 2 ** ((f - 69) / 12)

def env(t, a, d):  # attack, exponential decay
    return (t / a if t < a else math.exp(-(t - a) / d))

def mix(buf, start, samples):
    i0 = int(start * SR)
    for i, v in enumerate(samples):
        if i0 + i < len(buf): buf[i0 + i] += v

def bell(freq, dur, amp=1.0):
    parts = [(1, 1, 1.0), (2.0, .6, .45), (2.76, .4, .25), (5.4, .25, .12), (8.93, .15, .06)]   # inharmonic bell
    out = []
    for i in range(int(dur * SR)):
        t = i / SR
        out.append(amp * sum(g * env(t, .003, dur * k) * math.sin(2 * math.pi * freq * r * t) for r, k, g in parts))
    return out

def marimba(freq, dur, amp=1.0):
    out = []
    for i in range(int(dur * SR)):
        t = i / SR
        v = math.sin(2 * math.pi * freq * t) * env(t, .002, .18) + .35 * math.sin(2 * math.pi * freq * 4 * t) * env(t, .001, .04)
        out.append(amp * v)
    return out

def ping(freq, dur, amp):
    return [amp * env(i / SR, .002, dur * .35) * math.sin(2 * math.pi * freq * i / SR) for i in range(int(dur * SR))]

def sparkle(buf, start, length, count, lo, hi, amp, rising=False):
    for k in range(count):
        t = start + (k / count if rising else random.random()) * length
        f = (lo + (hi - lo) * (k / count)) if rising else random.uniform(lo, hi)
        mix(buf, t, ping(f * random.uniform(.97, 1.03), .18, amp * random.uniform(.5, 1)))

def clap_noise(buf, start, length, amp):  # soft applause-like crackle: short filtered noise bursts
    for k in range(int(length * 60)):
        t = start + random.random() * length
        n = int(.012 * SR); prev = 0
        burst = []
        for i in range(n):
            x = random.uniform(-1, 1); prev = .6 * prev + .4 * x       # gentle low-pass
            burst.append(amp * prev * math.exp(-i / (n / 4)))
        mix(buf, t, burst)

def pop(buf, start, amp):
    n = int(.09 * SR)
    mix(buf, start, [amp * math.exp(-i / (n / 5)) * math.sin(2 * math.pi * (900 - 6000 * i / SR) * i / SR) for i in range(n)])

def finish(buf, target_rms=.10, peak_max=.5):
    # gentle fade at the end, then match the game's loudness
    n = len(buf); fade = int(.12 * SR)
    for i in range(fade): buf[n - fade + i] *= 1 - i / fade
    rms = math.sqrt(sum(x * x for x in buf) / n) or 1
    k = target_rms / rms
    peak = max(abs(x) for x in buf) * k
    if peak > peak_max: k *= peak_max / peak
    return [x * k for x in buf]

def write(path, buf):
    with wave.open(path, 'w') as w:
        w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR)
        w.writeframes(b''.join(struct.pack('<h', max(-32767, min(32767, int(x * 32767)))) for x in buf))

os.makedirs('Assets/Audio/WinCandidates', exist_ok=True)

# A) bright bell arpeggio C5 E5 G5 C6 + shimmer
a = [0.0] * int(1.6 * SR)
for k, m in enumerate((72, 76, 79, 84)):
    mix(a, k * .11, bell(note(m), 1.1 if k == 3 else .7, 1.0 if k == 3 else .8))
sparkle(a, .35, .9, 14, 3000, 6500, .12)
# B) marimba tune G4 C5 E5 G5 E5 C6 + applause-like sparkle
b = [0.0] * int(1.7 * SR)
for k, m in enumerate((67, 72, 76, 79, 76, 84)):
    mix(b, k * .1 + (.08 if k == 5 else 0), marimba(note(m), .45 if k < 5 else .8, .9))
clap_noise(b, .55, .9, .18)
sparkle(b, .6, .8, 10, 2500, 5000, .08)
# C) pop + rising magic sparkle
c = [0.0] * int(1.4 * SR)
pop(c, 0, 1.0)
sparkle(c, .06, .9, 24, 1200, 5200, .35, rising=True)
mix(c, .9, bell(note(96), .5, .35))

out = {}
for name, buf in (('a', a), ('b', b), ('c', c)):
    path = f'Assets/Audio/WinCandidates/win-{name}.wav'
    write(path, finish(buf)); out[name] = path
    print(path, round(len(buf) / SR, 2), 's')

# test page: the three sounds embedded, one button each
html = ['<!doctype html><html lang="tr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">',
        '<title>Kazanma sesi adayları</title><style>body{margin:0;background:#F8EBD0;color:#0B2A36;font:17px/1.5 -apple-system,Segoe UI,sans-serif}',
        'main{max-width:480px;margin:0 auto;padding:28px 16px}button{display:block;width:100%;margin:14px 0;padding:18px;border:0;border-radius:16px;',
        'background:#1C7F8C;color:#F8EBD0;font-size:19px;font-weight:700}p{color:#0F4F5A}</style></head><body><main>',
        '<h1>Kazanma sesi adayları</h1><p>Her tuş bir adayı çalar. Beğendiğini Claude\'a harfiyle söyle (A, B ya da C).</p>']
labels = {'a': 'A — Çan arpeji + parıltı', 'b': 'B — Marimba melodisi + pırıltı', 'c': 'C — Pop + yükselen sihirli ışıltı'}
for k, p in out.items():
    data = base64.b64encode(open(p, 'rb').read()).decode()
    html.append(f'<button onclick="document.getElementById(\'s{k}\').currentTime=0;document.getElementById(\'s{k}\').play()">{labels[k]}</button>'
                f'<audio id="s{k}" src="data:audio/wav;base64,{data}"></audio>')
html.append('<p>Sesler kodla üretildi (örnek dosya kullanılmadı), telif yok. Ses seviyesi oyundaki diğer seslerle eşit.</p></main></body></html>')
os.makedirs('docs/sounds', exist_ok=True)
open('docs/sounds/index.html', 'w', encoding='utf-8').write(''.join(html))
print('docs/sounds/index.html')
