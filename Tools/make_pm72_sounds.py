"""PM_72: short exit feedback sounds (44.1 kHz mono 16-bit) into Resources/Art/audio.
pm72-tick   ~70 ms  soft pop on every exit
pm72-defuse ~240 ms pop + a short hiss (a bomb piece out, the fuse going out)
pm72-unlock ~150 ms metallic crack (a lock breaking)"""
import math, random, struct, wave
SR = 44100
OUT = 'Assets/_Game/Presentation/Resources/Art/audio/'
random.seed(72)

def write(name, samples):
    peak = max(1e-9, max(abs(s) for s in samples))
    with wave.open(OUT + name + '.wav', 'wb') as w:
        w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR)
        w.writeframes(b''.join(struct.pack('<h', int(s / peak * .8 * 32767)) for s in samples))

def pop(dur, f0, f1, decay):
    out, ph = [], 0.0
    for i in range(int(SR * dur)):
        t = i / SR
        f = f1 + (f0 - f1) * math.exp(-t * 60)
        ph += 2 * math.pi * f / SR
        env = (1 - math.exp(-t * 2500)) * math.exp(-t * decay)
        out.append(math.sin(ph) * env)
    return out

tick = pop(.07, 1250, 520, 70)
write('pm72-tick', tick)

hiss, lp = [], 0.0
for i in range(int(SR * .24)):
    t = i / SR
    lp += .25 * (random.uniform(-1, 1) - lp)
    hiss.append(lp * .55 * math.exp(-t * 14) * min(1, t / .02))
base = pop(.24, 900, 330, 30)
write('pm72-defuse', [a + b for a, b in zip(base, hiss)])

crack = []
for i in range(int(SR * .15)):
    t = i / SR
    ring = sum(math.sin(2 * math.pi * f * t) * a for f, a in ((2350, .5), (3720, .35), (5150, .2))) * math.exp(-t * 38)
    click = random.uniform(-1, 1) * math.exp(-t * 400)
    crack.append(ring + click * .9)
write('pm72-unlock', crack)
print('ok')
