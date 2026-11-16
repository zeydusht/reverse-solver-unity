"""PM_75: the knock of a stuck piece meeting a tab ("tok"): a short, dry wooden knock, 110 ms, 44.1 kHz mono.
Two damped modes of a small wooden block plus a 3 ms click, gently low-passed. Writes
Assets/_Game/Presentation/Resources/Art/audio/pm75-tok.wav.
    python Tools/make_pm75_tok.py"""
import math, os, struct, wave

RATE = 44100
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "_Game", "Presentation", "Resources", "Art", "audio", "pm75-tok.wav")


def knock():
    n = int(RATE * .11)
    seed = 75
    out = []
    lp = 0.0
    for i in range(n):
        t = i / RATE
        body = .62 * math.sin(2 * math.pi * 410 * t) * math.exp(-t / .030)
        body += .34 * math.sin(2 * math.pi * 1180 * t + .6) * math.exp(-t / .012)
        body += .12 * math.sin(2 * math.pi * 2650 * t + 1.1) * math.exp(-t / .005)
        seed = (seed * 1103515245 + 12345) & 0x7FFFFFFF
        noise = (seed / 0x7FFFFFFF * 2 - 1) * .45 * math.exp(-t / .0016)
        x = body + noise
        lp += (x - lp) * .55          # soften the click
        attack = min(1.0, t / .0012)
        out.append(lp * attack)
    peak = max(abs(v) for v in out)
    return [v / peak * .82 for v in out]


def main():
    samples = knock()
    with wave.open(OUT, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(RATE)
        w.writeframes(b"".join(struct.pack("<h", int(max(-1, min(1, v)) * 32767)) for v in samples))
    print("wrote", os.path.normpath(OUT), len(samples), "samples")


if __name__ == "__main__":
    main()
