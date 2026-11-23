"""PM_102 §1: the win fanfare g ("Piano ta-da", PM_96, Zeyd's pick) in other instruments: its own melody, rhythm and
length (Tools/make_fanfare96.py's score: three rising chords G, Am, Bm a beat apart, the C chord, a pickup and the big
C with bells), so g's beat map (its 5 beats) is theirs too and the win screen stays on them. PM_100's tones were b's
(a misunderstanding); they stay on the page as the old ones.
  marimba   marimba + glockenspiel                                          VCSL
  pizz      violin section pizzicato + cello pizzicato + hand chimes        VSCO 2 CE, VCSL
  ksilofon  xylophone + marimba (the low notes) + hand claps                VCSL
  kutu      music box: glockenspiel (soft) + kalimba, a soft synth pad (ours) under it   VCSL
The percussion of g (snare on the chords, kick, cymbal and timpani on the big C) stays, lighter for the music box.
Every note's attack on g's own times (Tools/make_fanfare100.py's placement); each levelled to g's loudness.
    python -I Tools/make_fanfare102.py <samples dir: VCSL and VSCO 2 CE files flattened>
Out: Assets/Audio/Fanfare96/kazanma-g-<tone>.wav (the web plays them from GitHub Pages, music/pm96/; the editor loads
them from here), docs/art/pm102/onizleme/sesler/kazanma-g-<tone>.wav, docs/art/pm102/fanfar102.json."""
import json, os, sys
import numpy as np, soundfile as sf, pyloudnorm as pyln

sys.argv = [sys.argv[0], sys.argv[1]]
sys.path.insert(0, 'Tools')
import make_fanfare100 as F   # its instruments, placement, pad, levelling and onset finder

SR = F.SR
WEB, PAGE = 'Assets/Audio/Fanfare96', 'docs/art/pm102/onizleme/sesler'
LENGTH = 2.449                                      # g's length (the beat map)
BEATS = [0.0, .302, .603, .902, 1.453]              # g's beats (the map)
m = lambda name: F.m(name) + 12                     # make_fanfare96 names its notes as VCSL does: an octave low

# g's score (make_fanfare96.py): (time, notes, velocity, part)
S = .3
CHORDS = [['G3', 'B3', 'D4', 'G4'], ['A3', 'C4', 'E4', 'A4'], ['B3', 'D4', 'F#4', 'B4'], ['C4', 'E4', 'G4', 'C5']]
FIN = 3 * S + .55
SCORE = []
for k, ch in enumerate(CHORDS):
    SCORE += [(k * S + i * .006, m(n), .62 + .04 * k, 'chord') for i, n in enumerate(ch)]
    SCORE += [(k * S, None, .28 + .05 * k, 'snare')]
SCORE += [(3 * S, None, .45, 'kick'), (FIN - .12, m('G4'), .5, 'pickup')]
SCORE += [(FIN + i * .004, m(n), .62, 'final') for i, n in enumerate(['C3', 'G3', 'C4', 'E4', 'G4', 'C5', 'E5'])]
SCORE += [(FIN, m('C5'), .5, 'bell'), (FIN, m('G5'), .5, 'bell'), (FIN, None, .55, 'crash'), (FIN, None, .7, 'timp'), (FIN, m('C7'), .4, 'sparkle')]
PADS = [(0, .3, CHORDS[0]), (.3, .6, CHORDS[1]), (.6, .9, CHORDS[2]), (.9, FIN - .05, CHORDS[3]), (FIN, LENGTH, ['C3', 'G3', 'C4', 'E4'])]

SNARE = F.Hits('*Snare Drum, Modern 1*_HitNS_v5*')
KICK = F.Hits('*Bass Drum 1*')
CRASH = F.Hits('*Clash Cymbals 1*ff2*')
TIMP = F.Hits('*Timpani 1__Hit__Timpani1_Hit_v4*')
TRI = F.Hits('*Triangles*hit_mf*')
BELLTREE = F.Hits('*Bell Tree*Hit_C5*')


def perc(buf, t, v, part, soft=1.0):
    src = {'snare': SNARE, 'kick': KICK, 'crash': CRASH, 'timp': TIMP}[part]
    x = src.hit()[:int(1.2 * SR)]
    F.place(buf, x, t, v * soft)


def render(tone):
    buf = np.zeros((int(LENGTH * SR) + SR, 2))
    for t, n, v, part in SCORE:
        g = v ** 1.4
        if part in ('snare', 'kick', 'crash', 'timp'):
            if tone == 'ksilofon' and part == 'snare': F.place(buf, F.CLAPS.hit(), t, .45 * g)
            elif tone == 'kutu':
                if part in ('crash', 'timp'): F.place(buf, TRI.hit()[:int(1.2 * SR)], t, .35 * g)
            else: perc(buf, t, v, part, .8)
            continue
        if part == 'sparkle':
            F.place(buf, F.GLOCK.note(n), t, .5 * g)
            if tone == 'kutu': F.place(buf, BELLTREE.hit(), t + .03, .3 * g)
            continue
        if tone == 'marimba':
            if part == 'bell': F.place(buf, F.GLOCK.note(n + 12), t, .45 * g)
            else: F.place(buf, F.MARIMBA.note(n), t, g); F.place(buf, F.GLOCK.note(n + 12), t, .18 * g) if part in ('pickup', 'final') and n >= m('C5') else None
        elif tone == 'pizz':
            if part == 'bell': F.place(buf, F.CHIMES.note(n, 1.4), t, .35 * g)
            elif n < m('G3') + 1: F.place(buf, F.CELLOS.note(n), t, .9 * g)
            else: F.place(buf, F.VIOLINS.note(n), t, g)
        elif tone == 'ksilofon':
            if part == 'bell': F.place(buf, F.GLOCK.note(n + 12), t, .4 * g)
            elif n >= m('G4'): F.place(buf, F.XYLO.note(n), t, .85 * g)
            else: F.place(buf, F.MARIMBA.note(n), t, .6 * g)
        elif tone == 'kutu':
            if part == 'bell': F.place(buf, F.GLOCK.note(n + 12), t, .35 * g)
            elif n >= m('C4'): F.place(buf, F.GLOCK.note(n + 12), t, .5 * g); F.place(buf, F.KALIMBA.note(n), t, .55 * g)
            else: F.place(buf, F.KALIMBA.note(n), t, .45 * g)
    if tone == 'kutu':
        p = F.pad([(a, b, [m(x) - 12 for x in ns]) for a, b, ns in PADS], len(buf))
        buf += p * .14 * np.max(np.abs(buf))
    buf = buf[:int(LENGTH * SR)]
    n = int(.3 * SR); buf[-n:] *= np.linspace(1, 0, n)[:, None] ** 2
    return buf


TONES = ['marimba', 'pizz', 'ksilofon', 'kutu']
if __name__ == '__main__':
    os.makedirs(PAGE, exist_ok=True)
    g = F.read_b(os.path.join(WEB, 'kazanma-g.wav'))
    meter = pyln.Meter(SR, block_size=.2)
    target = meter.integrated_loudness(g)
    report = {'target_lufs': round(target, 2), 'beats': BEATS, 'tones': {}}
    for tone in TONES:
        x = F.level_to(render(tone), target)[:int(round(LENGTH * SR))]
        sf.write(os.path.join(WEB, f'kazanma-g-{tone}.wav'), x, SR, subtype='PCM_16')
        sf.write(os.path.join(PAGE, f'kazanma-g-{tone}.wav'), x, SR, subtype='PCM_16')
        on = F.onsets(x)
        dev = [round(1000 * float(on[np.argmin(np.abs(on - r))] - r), 1) for r in BEATS]
        report['tones'][tone] = dict(length=round(len(x) / SR, 3), lufs=round(meter.integrated_loudness(x), 2),
                                     peak=round(20 * np.log10(np.max(np.abs(x))), 2), beat_attack_ms=dev)
        print(f'{tone:9s} {len(x) / SR:.3f} s {report["tones"][tone]["lufs"]:6.2f} LUFS peak {report["tones"][tone]["peak"]:5.1f}  beat attacks (ms) {dev}')
    on = F.onsets(g[:int(LENGTH * SR)])
    report['g_itself_ms'] = [round(1000 * float(on[np.argmin(np.abs(on - r))] - r), 1) for r in BEATS]
    print('g itself', report['g_itself_ms'])
    json.dump(report, open('docs/art/pm102/fanfar102.json', 'w', encoding='utf8'), indent=1)
