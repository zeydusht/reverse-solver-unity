# Designer loop (PM_MESAJ_2): scores drafts with the FROZEN ZeydBot v1, exactly as committed in fc4ef7c: the
# model code and its training CSVs (denemeler, hamleler, bolum_ozellik, yollar) are read from that commit, the
# drafts' paths and features (ZeydDesign.Check) are added beside them. Nothing fitted on later data.
# Also the thinking profile: the bot's expected seconds per move, against Zeyd's real Z12 first attempt.
#
#   python Tools/zeydbot_score_drafts.py [Logs/zdesign]
import csv, io, os, subprocess, sys, tempfile, types

FROZEN = 'fc4ef7c'
ROOT = os.path.join(os.path.dirname(__file__), '..')


def git_show(path):
    return subprocess.run(['git', 'show', f'{FROZEN}:{path}'], capture_output=True, cwd=ROOT).stdout.decode('utf-8')


def main(dz):
    tmp = tempfile.mkdtemp(prefix='zv1-')
    for f in ('denemeler.csv', 'hamleler.csv', 'bolum_ozellik.csv', 'yollar.csv'):
        open(os.path.join(tmp, f), 'w', encoding='utf-8', newline='').write(git_show('docs/analiz/zeydbot/' + f))
    zm = types.ModuleType('zeydbot_model_v1'); zm.__file__ = os.path.join(ROOT, 'Tools', 'zeydbot_model.py')
    exec(compile(git_show('Tools/zeydbot_model.py'), 'zeydbot_model@' + FROZEN, 'exec'), zm.__dict__)
    zm.D = tmp
    first, score, moves, feat, paths = zm.load()
    drafts = {r['level_id']: r for r in csv.DictReader(open(os.path.join(dz, 'taslak_ozellik.csv'), encoding='utf-8'))}
    for r in csv.DictReader(open(os.path.join(dz, 'taslak_yollar.csv'), encoding='utf-8')):
        paths.setdefault(r['level_id'], {}).setdefault(r['run'], []).append(r)
    for l, r in drafts.items(): feat[l] = {**r, 'tur': 'taslak'}
    levels = sorted(first)
    trap, think, sc, _ = zm.fit_all(levels, first, score, moves, feat, paths)
    c = think[0]

    def think_profile(l):
        # expected thinking per move along the bot's paths (lognormal mean), first move free
        import math
        tot = n = 0
        for steps in paths[l].values():
            for j, s in enumerate(steps):
                if j == 0 or int(s['secenek']) == 0: continue
                mu = sum(ci * xi for ci, xi in zip(c, zm.think_x(int(s['secenek']), int(s['dogru']), int(s['kalan_parca']))))
                tot += math.exp(mu + think[2] ** 2 / 2) / 1000; n += 1
        return tot / max(n, 1)

    print('| Taslak | v1 puan | ilk deneme kazanma | kalan s | bot düşünme s/hamle | rastgele | tuzaklı | zorunlu | yanıltıcı/adım | süre/yol | hedef |')
    print('|---|---|---|---|---|---|---|---|---|---|---|')
    for l in ['Z12', 'Z13'] + sorted(drafts):
        w, left, used = zm.play(l, feat, paths, trap, think)
        s = zm.clamp_score(sc[0] + sc[1] * (1 - w) + sc[2] * used)
        d = drafts.get(l)
        ok = 8 <= s <= 9.99 and .30 <= w <= .60
        extra = (f"%{float(d['rastgele_kazanma']) * 100:.0f} | {d['yol_tuzakli_hamle']} | {d['yol_zorunlu']} | {float(d['yaniltici_adim']):.2f} | {float(d['sure_limiti']) / float(d['en_kisa_cozum']):.2f}"
                 if d else '— | — | — | — | —')
        print(f"| {l}{' (referans)' if not d else ''} | {s:.1f} | %{w * 100:.0f} | {left:.0f} | {think_profile(l):.2f} | {extra} | {'✓' if ok else '✗'} |")
    print('\nZeyd Z12 ilk deneme: ortalama düşünme 3,35 s/hamle (denemeler.csv think_ort_ms 3350), süre bitti.')


if __name__ == '__main__':
    main(sys.argv[1] if len(sys.argv) > 1 else os.path.join(ROOT, 'Logs', 'zdesign'))
