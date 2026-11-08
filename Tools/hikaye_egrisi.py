"""SET40 hikâye eğrisi; yalnız standart kütüphane, kaynak seviyeler değişmez.

K2 = bogaz_sayisi >= 1 ve cig_orani >= 3.
K3 = en_uzun_bogaz (sayısal; tipe göre yorumlanır, ortak geçme eşiği yok).
K4 = ilk3_min_secenek >= 2 ve ilk_hamle_tuzak_orani <= 0.30.
K5_vekil = son ceil(adim_sayisi * .30) adımın ortalama seçeneği >= 3.
    ZeydBot süresi yok; bu, süre temelli K5'in yalnız vekilidir.
bomba_D = en az bir bomba için 0 <= ilk_fitil - çıkış_adımı <= 2.
    Çıkış adımı 1 tabanlıdır; tehdit adımıyla kıyaslanmaz. Çıkmayan bomba
    geçmez; bombasız bölüm UYGULANMAZ. Çoklu bombada her bomba ayrı sınanır.
Boğaz = seçenek=1 olan en az 2 ardışık adımlık blok; en uzun değer tek
    adımlık blokları da içerir. Eşit en büyük sıçramada ilk i seçilir;
    anahtar_adim = floor(100*i/n), i 0 tabanlı (sıçrama öncesi adım).
Bomba payı = kayıtlı kalan fitilin minimumu (geçen adım tekrar çıkarılmaz).
İlk fitil = ilk durumdaki minimum fitil. Tehdit = min(fitil)<=2 ilk adım.
İmza = iki uç dahil 10 eşit konumda lineer örnekleme; boş eğri sıfırdır.
"""
import argparse
import csv
import html
import json
import math
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
FIELDS = ('bolum id adim_sayisi ilk3_min_secenek ilk_hamle_tuzak_orani '
          'bogaz_sayisi en_uzun_bogaz anahtar_adim sicrama_kat cig_orani '
          'bomba_min_pay bomba_tehdit_adimi bomba_ilk_fitil egri_imzasi').split()


def signature(values):
    if not values:
        return [0.0] * 10
    out = []
    for j in range(10):
        x = j * (len(values) - 1) / 9
        i = int(x)
        out.append(values[i] + (values[min(i + 1, len(values) - 1)] - values[i]) * (x - i))
    return out


def measure(level):
    steps = level['steps']
    choices = [len(s['ok']) for s in steps]
    traps = [len(s['removable']) - len(s['ok']) for s in steps]
    n = len(steps)
    runs, run = [], 0
    for choice in choices + [0]:
        if choice == 1:
            run += 1
        elif run:
            runs.append(run)
            run = 0
    key = max(range(n - 1), key=lambda i: choices[i + 1] - choices[i]) if n > 1 else None
    tail = choices[-max(1, math.ceil(n * .30)):]
    avalanche = sum(tail) / len(tail) if tail else 0
    ratio = traps[0] / len(steps[0]['removable']) if n and steps[0]['removable'] else 0
    fuses = [b['f'] for s in steps for b in s['bombs']]
    initial = steps[0]['bombs'] if n else []
    exits = {s['move']: i + 1 for i, s in enumerate(steps)}
    bomb_pass = any(b['p'] in exits and 0 <= b['f'] - exits[b['p']] <= 2 for b in initial)
    row = dict(zip(FIELDS, [
        level['index'], level['id'], n, min(choices[:3], default=0), f'{ratio:.4f}',
        sum(r >= 2 for r in runs), max(runs, default=0),
        100 * key // n if key is not None else '',
        f'{choices[key + 1] / max(1, choices[key]):.2f}' if key is not None else '',
        f'{avalanche:.2f}', min(fuses, default=''),
        next((i + 1 for i, s in enumerate(steps) if any(b['f'] <= 2 for b in s['bombs'])), ''),
        min((b['f'] for b in initial), default=''),
        ';'.join(f'{x:.1f}' for x in signature(choices))]))
    verdict = lambda condition: 'GEÇTİ' if condition else 'KALDI'
    checks = {'bolum': level['index'],
              'K2': verdict(row['bogaz_sayisi'] >= 1 and avalanche >= 3),
              'K3': row['en_uzun_bogaz'],
              'K4': verdict(row['ilk3_min_secenek'] >= 2 and ratio <= .30),
              'K5_vekil': verdict(avalanche >= 3),
              'bomba_D': verdict(bomb_pass) if initial else 'UYGULANMAZ'}
    return row, checks, choices, traps


def chart(level, choices, traps):
    peak = max(choices + traps + [1])
    def xy(i, value):
        return 24 + 252 * i / max(1, len(choices) - 1), 145 - 115 * value / peak
    points = ' '.join(f'{x:.2f},{y:.2f}' for x, y in (xy(i, v) for i, v in enumerate(choices)))
    dots = ''.join(f'<circle cx="{xy(i, v)[0]:.2f}" cy="{xy(i, v)[1]:.2f}" r="3" fill="#d32f2f"/>'
                   for i, v in enumerate(traps) if v > 0)
    title = html.escape(f"{level['index']} · {level['id']} · {len(choices)} adım")
    return (f'<figure><figcaption>{title}</figcaption><svg viewBox="0 0 300 165" role="img" '
            f'aria-label="{title}"><path d="M24 25V145H280" fill="none" stroke="#ccd2da"/>'
            f'<text x="2" y="30">{peak}</text><text x="7" y="149">0</text>'
            f'<polyline points="{points}" fill="none" stroke="#244c80" stroke-width="3"/>'
            f'{dots}</svg></figure>')


def write_csv(path, fields, rows):
    with path.open('w', encoding='utf-8', newline='') as stream:
        writer = csv.DictWriter(stream, fieldnames=fields)
        writer.writeheader()
        writer.writerows(rows)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--no-unity', action='store_true')
    args = parser.parse_args()
    if not args.no_unity:
        result = subprocess.run(['bash', 'Tools/unity_batch.sh', 'method',
                                 'ReverseSolver.LevelEditor.HikayeEgrisi.Run'],
                                cwd=ROOT, capture_output=True, text=True, errors='replace')
        if result.returncode:
            raise RuntimeError('Unity batch başarısız:\n' + result.stdout + result.stderr)
    levels = json.loads((ROOT / 'Logs/hikaye/steps.json').read_text(encoding='utf-8-sig'))['levels']
    levels = sorted(levels, key=lambda level: level['index'])
    if len(levels) != 40 or [l['index'] for l in levels] != list(range(1, 41)):
        raise ValueError('SET40 için 1..40 sıra numaralı tam 40 bölüm gerekir.')
    if any(not level['solved'] for level in levels):
        raise ValueError('Çözülemeyen bölüm var; eksik yol ile rapor üretilmedi.')
    measured = [measure(level) for level in levels]
    out = ROOT / 'docs/analiz'
    out.mkdir(parents=True, exist_ok=True)
    write_csv(out / 'hikaye_egrisi_set40.csv', FIELDS, [m[0] for m in measured])
    checks = [m[1] for m in measured]
    write_csv(out / 'hikaye_kontrol.csv', list(checks[0]), checks)
    cards = ''.join(chart(level, m[2], m[3]) for level, m in zip(levels, measured))
    page = ('<!doctype html><html lang="tr"><meta charset="utf-8">'
            '<meta name="viewport" content="width=device-width,initial-scale=1">'
            '<title>SET40 hikâye eğrisi</title><style>'
            'body{background:#f7f8fc;color:#202c3d;font:14px system-ui;margin:24px}'
            'main{display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:12px}'
            'figure{margin:0;background:white;border:1px solid #dde3ec;padding:10px;border-radius:8px}'
            'figcaption{font-weight:600}svg{width:100%;display:block}text{font-size:11px}'
            '</style><h1>SET40 hikâye eğrisi</h1>'
            '<p>Kalın çizgi: seçenek · Kırmızı noktalar: tuzak sayısı (y). '
            'Yatay eksen: çözüm adımları. Dikey ölçek her kartta ayrıdır. '
            'Bütçesi tükenen adaylar seçenek sayısına dahildir (steps.json: unk). '
            'K5 yalnız vekildir; ZeydBot süresi yok.</p><main>' + cards + '</main></html>')
    (out / 'hikaye_egrisi_set40.html').write_text(page, encoding='utf-8')
    for rule in ('K2', 'K4', 'K5_vekil', 'bomba_D'):
        values = [c[rule] for c in checks]
        print(f"{rule}: {values.count('GEÇTİ')} geçti, {values.count('KALDI')} kaldı"
              + (f", {values.count('UYGULANMAZ')} uygulanmaz" if rule == 'bomba_D' else ''))
    print(f'K3: {len(checks)} sayısal ölçüm; tipe göre yorumlanır.')


if __name__ == '__main__':
    try:
        main()
    except (OSError, ValueError, RuntimeError) as error:
        sys.exit(str(error))
