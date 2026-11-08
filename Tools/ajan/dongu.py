"""Kart → fabrika tarifi → ölçüm → kural eleştirisi (en fazla üç tur).

Unity'yi kullanıcı adına başlatan komutlar --aday / --uret'tir. Önceden alınmış
ölçümle çalışmak için --no-unity [--olcum dosya] kullanılır. Kaynak set yazılmaz.
"""
import argparse
import copy
import csv
import json
import math
import itertools
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'Tools'))
import hikaye_egrisi
from level_similarity import curve_similarity, mask_similarity

HERE = Path(__file__).resolve().parent
LOG = ROOT / 'Logs/ajan'
CARDS = ROOT / 'docs/bolum_kartlari'


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def write(path, value):
    Path(path).parent.mkdir(parents=True, exist_ok=True)
    Path(path).write_text(json.dumps(value, ensure_ascii=False, indent=2, allow_nan=False) + '\n', encoding='utf-8')


def validate(value, spec, definitions, at='kart'):
    """Validate the draft-07 subset used by kart_sema, without dependencies."""
    if '$ref' in spec:
        return validate(value, definitions[spec['$ref'].split('/')[-1]], definitions, at)
    if 'oneOf' in spec:
        hits = 0
        for option in spec['oneOf']:
            try:
                validate(value, option, definitions, at)
                hits += 1
            except ValueError:
                pass
        if hits != 1:
            raise ValueError(f'{at}: tek bir şema dalıyla eşleşmeli')
        return
    kinds = {'object': lambda v: isinstance(v, dict), 'array': lambda v: isinstance(v, list),
             'integer': lambda v: type(v) is int, 'number': lambda v: type(v) in (int, float) and math.isfinite(v),
             'string': lambda v: isinstance(v, str), 'boolean': lambda v: type(v) is bool, 'null': lambda v: v is None}
    types = spec.get('type', list(kinds))
    if not isinstance(types, list):
        types = [types]
    if not any(kinds[t](value) for t in types):
        raise ValueError(f'{at}: tür {types} olmalı')
    if 'enum' in spec and value not in spec['enum']:
        raise ValueError(f'{at}: geçersiz değer {value!r}')
    if type(value) in (int, float):
        if value < spec.get('minimum', -math.inf) or value > spec.get('maximum', math.inf):
            raise ValueError(f'{at}: sınır dışında')
    if isinstance(value, str):
        import re
        if len(value) < spec.get('minLength', 0) or ('pattern' in spec and not re.search(spec['pattern'], value)):
            raise ValueError(f'{at}: geçersiz metin')
    if isinstance(value, dict):
        properties = spec.get('properties', {})
        if set(spec.get('required', [])) - value.keys():
            raise ValueError(f'{at}: zorunlu alan eksik')
        if spec.get('additionalProperties') is False and value.keys() - properties.keys():
            raise ValueError(f'{at}: bilinmeyen alan {value.keys() - properties.keys()}')
        for key in value.keys() & properties.keys():
            validate(value[key], properties[key], definitions, at + '.' + key)
    if isinstance(value, list):
        if not spec.get('minItems', 0) <= len(value) <= spec.get('maxItems', math.inf):
            raise ValueError(f'{at}: eleman sayısı yanlış')
        for i, item in enumerate(value):
            validate(item, spec.get('items', {}), definitions, f'{at}[{i}]')


def load_card(path):
    card = read(path)
    defaults = read(HERE / 'tip_varsayilan.json')['tipler']
    card = {**defaults.get(card.get('tip'), {}), **card}
    schema = read(HERE / 'kart_sema.json')
    validate(card, schema, schema['definitions'])
    recipe = card['tarif']
    if recipe['id'] != card['id'] or recipe['timer'] != card['sure_sn']:
        raise ValueError('Kart/tarif id ve süre uyuşmalı')
    if recipe['genre'] != card['tip']:
        raise ValueError('Kart/tarif tipi uyuşmalı')
    if not recipe.get('shapes') and not all(k in recipe for k in ('w', 'h')):
        raise ValueError('tarif: shapes veya w,h gerekli')
    for key in ('seals', 'chains', 'nails'):
        v = recipe[key]
        if isinstance(v, list) and v[0] > v[1]:
            raise ValueError(f'tarif.{key}: ters aralık')
    if recipe['seed_from'] > recipe['seed_to'] or card['kalan_sure_orani'][0] > card['kalan_sure_orani'][1]:
        raise ValueError('Ters tohum/süre aralığı')
    if card['en_uzun_bogaz_min'] > card['en_uzun_bogaz_max']:
        raise ValueError('Ters boğaz aralığı')
    for bomb in recipe['bombs']:
        if bomb['earliest'][0] > bomb['earliest'][1]:
            raise ValueError('Ters bomba çıkış aralığı')
    return card


def batch(method, *args):
    result = subprocess.run(['bash', 'Tools/unity_batch.sh', 'method',
                             'ReverseSolver.LevelEditor.' + method, *map(str, args)],
                            cwd=ROOT, capture_output=True, text=True, encoding='utf-8', errors='replace')
    if result.returncode:
        raise RuntimeError(f'{method} başarısız (çıkış {result.returncode}):\n' + (result.stdout + result.stderr)[-3000:])


def distribution(values):
    values = sorted(values)
    def q(p):
        if not values:
            return None
        x = (len(values) - 1) * p
        i = int(x)
        return values[i] + (values[min(i + 1, len(values) - 1)] - values[i]) * (x - i)
    return dict(n=len(values), median=q(.5), p25=q(.25), p75=q(.75))


def player_model(level, measured):
    """v1 accepts arbitrary ids; v2 assumes Znn and encodes play order as skill.

    Fit v1's existing trap/thinking functions on its frozen training partition.
    Its optional sample sink adds quantiles without changing model draws/results.
    """
    try:
        import zeydbot_model as model
        path = LOG / f"paths_{level['id']}.csv"
        with path.open(encoding='utf-8-sig', newline='') as stream:
            rows = list(csv.DictReader(stream))
        paths = {}
        for row in rows:
            if row['level_id'] != level['id']:
                raise ValueError('Yol kimliği uyuşmuyor')
            paths.setdefault(row['run'], []).append(row)
        # A truncated or unverified path is never interpreted as a win.
        if not paths or any(len(p) != len(level['pieces']) or int(p[-1]['kalan_parca']) != 1
                            or any(int(s['dogru']) < 1 for s in p) for p in paths.values()):
            raise ValueError('Eksik/doğrulanmamış ZeydBot yolu')
        first, _, moves, _, _ = model.load()
        train = set(first)
        trap = model.fit_logistic(model.trap_samples(moves, train))
        think = model.fit_think(moves, train)
        samples = []
        win, _, _ = model.play(level['id'], {level['id']: {'sure_limiti': level['timer']}},
                                {level['id']: paths}, trap, think, samples=samples)
        return dict(source='ZeydBot v1', calibrated=False, win_rate=win,
                    remaining=distribution([s['time_left'] / level['timer'] for s in samples if s['won']]),
                    loss_piece_share=distribution([s['loss_piece_share'] for s in samples if not s['won']]),
                    tail_time_share=distribution([s['tail_time_share'] for s in samples if s['won']]))
    except (OSError, ValueError, KeyError, ZeroDivisionError, OverflowError, ImportError) as error:
        fun = measured['bots']['funsim']
        remaining = dict(fun['time_left'])
        for key in ('median', 'p25', 'p75'):
            if remaining.get(key) is not None:
                remaining[key] /= level['timer']
        return dict(source='FunSim', fallback_reason=str(error), win_rate=fun['win_rate'],
                    remaining=remaining, loss_piece_share=fun['loss_piece_share'], tail_time_share=None)


def comparisons(level, signature, candidates):
    reference = read(ROOT / 'Assets/_Game/Levels/redesign.json')['levels']
    with (ROOT / 'docs/analiz/hikaye_egrisi_set40.csv').open(encoding='utf-8-sig', newline='') as stream:
        curves = {row['id']: row['egri_imzasi'] for row in csv.DictReader(stream)}
    all_rows = []
    for i, other in enumerate(reference):
        all_rows.append(dict(id=other['id'], set='SET40', bolum=i + 1,
                             curve=curve_similarity(signature, curves[other['id']]), mask=mask_similarity(level, other)))
    for other, sig in candidates:
        if other['id'] != level['id']:
            all_rows.append(dict(id=other['id'], set='aday', bolum=None,
                                 curve=curve_similarity(signature, sig), mask=mask_similarity(level, other)))
    intro_rows = [r for r in all_rows if r['set'] == 'SET40' and r['bolum'] <= 9]
    return dict(candidate_max=max((r['mask'] for r in all_rows if r['set'] == 'aday'), default=0),
                curve=sorted(all_rows, key=lambda r: (-r['curve'], r['id']))[:3],
                mask=sorted(all_rows, key=lambda r: (-r['mask'], r['id']))[:3],
                intro_max=max(r['mask'] for r in intro_rows),
                intro_nearest=max(intro_rows, key=lambda r: r['mask'])['id'],
                maximum=max((r['mask'] + r['curve']) / 2 for r in all_rows))


def critique(card, level, measured, metrics, similar, player):
    """Rules R01..R22 are documented in ELESTIRMEN.md."""
    recipe = card['tarif']
    checks = []
    def check(rule, ok, value, target, field, new, warning=False):
        checks.append(dict(rule=rule, status='UYARI' if warning else ('GEÇTİ' if ok else 'KALDI'), measured=value, target=target,
                           correction=None if ok or warning else dict(field=field, value=new)))
    def new_shape():
        return next([w, h] for w, h in ((5, 5), (5, 6), (6, 6), (6, 5))
                    if [w, h] != recipe.get('shapes', [[recipe.get('w'), recipe.get('h')]])[0])
    def tighten_bomb(earlier=False):
        if not recipe['bombs']:
            return 'bombs', [dict(earliest=[3, 5], margin=2, region='center')]
        bomb = recipe['bombs'][0]
        shift = -1 if earlier else 1
        return 'bombs[0].earliest', [max(1, v + shift) for v in bomb['earliest']]
    steps = measured['steps']
    check('R01 çözüm', measured['solved'], measured['solved'], True, 'seed_from', recipe['seed_from'] + 400)
    check('R02 kesin ölçüm', not any(s['unk'] for s in steps), sum(len(s['unk']) for s in steps), 0,
          'seed_from', recipe['seed_from'] + 400, warning=any(s['unk'] for s in steps))
    # Type defaults form a floor: a card cannot silently weaken the book.
    defaults = read(HERE / 'tip_varsayilan.json')['tipler'][card['tip']]
    opening = max(defaults['ilk3_min_secenek'], card['ilk3_min_secenek'])
    check('R03 açılış', metrics['ilk3_min_secenek'] >= opening, metrics['ilk3_min_secenek'], f'>={opening}',
          'min_open_first', max(recipe['min_open_first'], opening + 1))
    lo, hi = max(defaults['en_uzun_bogaz_min'], card['en_uzun_bogaz_min']), min(defaults['en_uzun_bogaz_max'], card['en_uzun_bogaz_max'])
    check('R04 boğaz uzunluğu', lo <= metrics['en_uzun_bogaz'] <= hi, metrics['en_uzun_bogaz'], [lo, hi],
          'w,h', new_shape())
    minimum = max(defaults['bogaz_sayisi_min'], card['bogaz_sayisi_min'])
    check('R05 boğaz sayısı', metrics['bogaz_sayisi'] >= minimum, metrics['bogaz_sayisi'], f'>={minimum}',
          'seed_from', recipe['seed_from'] + 400)
    avalanche = card['cig_orani_min']
    if card['tip'] not in ('zor', 'super-zor'):
        avalanche = max(defaults['cig_orani_min'], avalanche or 0)
    if avalanche is not None:
        check('R06 göreli çığ', metrics['cig_goreli'] >= avalanche, metrics['cig_goreli'], f'>={avalanche}',
              'seed_from', recipe['seed_from'] + 400)
    trap_limit = 0 if card['tip'] == 'tanitim' else .30
    check('R07 ilk tuzak', float(metrics['ilk_hamle_tuzak_orani']) <= trap_limit,
          float(metrics['ilk_hamle_tuzak_orani']), f'<={trap_limit}', 'bombs[0].region',
          'bottom' if recipe['bombs'] and recipe['bombs'][0].get('region') != 'bottom' else 'top',
          warning=card['tip'] == 'tanitim')
    check('R08 tuzaklı adım', metrics['tuzak_adim'] >= card['tuzak_adim_min'], metrics['tuzak_adim'],
          f">={card['tuzak_adim_min']}", *tighten_bomb())
    exits = {b['p']: b for b in measured['bots']['bot_bilincli']['bomb_exits']}
    for i, (piece, fuse) in enumerate((level.get('bombs') or {}).items()):
        bomb = exits.get(int(piece), {})
        k = bomb.get('earliest', -1)
        check(f'R09 fitil/adım p{piece}', fuse < metrics['adim_sayisi'], fuse, f"<{metrics['adim_sayisi']}",
              f'bombs[{i}].margin', max(0, metrics['adim_sayisi'] - k - 1))
        intro = card['tip'] == 'tanitim'
        margin = 4 if intro else card['bomba_fitil_max']
        if intro:
            check(f'R09 tanıtım fitil payı p{piece}', k >= 1 and fuse >= k + margin,
                  dict(fuse=fuse, k=k), 'F>=k+4; üst sınır yok', f'bombs[{i}].margin', margin,
                  warning=k < 1)
        check(f'R10 fitil/erken çıkış p{piece}', k >= 1 and (fuse >= k + margin if intro else fuse <= k + margin),
              dict(fuse=fuse, k=k), 'F>=k+4' if intro else f'F<=k+{margin}', f'bombs[{i}].margin', margin,
              warning=intro or k < 1)
    if recipe['bombs']:
        threat = metrics['bomba_tehdit_adimi']
        if threat == '' and level.get('bombs'):
            # PM_48B: an aware player never sees pay <= 2 when F == k+2, so the threat is the step at which a
            # player who ignores the bomb reaches pay 2: min fuse - 2 (D recipe: "tehdit ilk N hamle + 2 içinde").
            threat = max(1, min(int(f) for f in level['bombs'].values()) - 2)
        check('R11 tehdit adımı', threat != '' and threat <= card['bomba_tehdit_adimi_max'], threat,
              f"<={card['bomba_tehdit_adimi_max']}", *tighten_bomb(earlier=True), warning=card['tip'] == 'tanitim')
    if card['bomba_unutan_kaybetmeli'] or card['tip'] == 'tanitim':
        for key in ('bot_rastgele_bombasiz', 'bot_bomba_unutan'):
            rate = measured['bots'][key]['win_rate']
            check('R12 ' + key, bool(level.get('bombs')) and (.15 <= rate <= .50 if card['tip'] == 'tanitim' and key == 'bot_rastgele_bombasiz' else rate <= .10), rate,
                  '[0.15, 0.50]' if card['tip'] == 'tanitim' and key == 'bot_rastgele_bombasiz' else '<=0.10',
                  *tighten_bomb(), warning=card['tip'] == 'tanitim' or
                  (key == 'bot_rastgele_bombasiz' and measured['bots'][key].get('runs', 0) < 20))
    check('R13 bilinçli bot', measured['bots']['bot_bilincli']['won'], measured['bots']['bot_bilincli']['won'], True,
          'seed_from', recipe['seed_from'] + 400)
    selected, source = remaining_source(level, measured, player)
    if player.get('source', '').startswith('ZeydBot') and source == 'FunSim':
        check('R14 ZeydBot örneği', False, player['remaining'], 'n>=20 ve kalibre model',
              'timer', recipe['timer'], warning=True)
    remaining = selected['median']
    lo, hi = card['kalan_sure_orani']
    mid = (lo + hi) / 2
    timer = max(5, min(600, round(level['timer'] * (1 - remaining) / (1 - mid)))) if remaining is not None else recipe['timer']
    check('R14 kalan süre', remaining is not None and lo <= remaining <= hi,
          dict(source=source, **selected), [lo, hi], 'timer', timer,
          warning=selected.get('n', 0) < 20 or remaining is None or
                  (source == 'FunSim' and measured['bots'].get('funsim', {}).get('calibrated') is False))
    if card['tip'] == 'tanitim':
        check('R15 ilk9 siluet', similar['intro_max'] < .9, similar['intro_max'], '<0.90', 'w,h', new_shape(), warning=True)
    calibration = read(HERE / 'kalibrasyon.json') if (HERE / 'kalibrasyon.json').exists() else {}
    threshold = calibration.get('by_type', {}).get(card['tip'], {}).get('median')
    threshold = threshold if threshold is not None else .20
    check('R16 final temposu', False, dict(tail=player.get('tail_time_share'), kalibrasyon=threshold),
          f'kalibrasyon medyanı <= {threshold}; danışma kontrolü', 'seed_from', recipe['seed_from'], warning=True)
    if card['tip'] in ('zor', 'super-zor'):
        loss = player['loss_piece_share']['median']
        check('R17 az kala kayıp', loss is not None and loss <= .20, loss, '<=0.20',
              'timer', min(600, recipe['timer'] + 5), warning=player['loss_piece_share']['n'] < 20 or not player.get('calibrated', False))
    # Check physical intent too; missing bombs must never vacuously pass R09/R10.
    count = len(level.get('bombs') or {})
    check('R18 tarifteki bomba sayısı', count == len(recipe['bombs']), count, len(recipe['bombs']), 'seed_from', recipe['seed_from'] + 400)
    if card.get('funsim_ilk_min', defaults.get('funsim_ilk_min')) is not None:
        rate = measured['bots'].get('funsim', {}).get('win_rate')
        fun = measured['bots'].get('funsim', {})
        runs = fun.get('runs', fun.get('time_left', {}).get('n', 0) + fun.get('loss_piece_share', {}).get('n', 0))
        minimum = card.get('funsim_ilk_min', defaults.get('funsim_ilk_min'))
        if card['tip'] == 'tanitim':
            minimum = max(defaults['funsim_ilk_min'], minimum)
        check('R19 FunSim ilk deneme', rate is not None and rate >= minimum, rate, f'>={minimum}',
              'bombs[0].region', 'center', warning=rate is None or runs < 20 or fun.get('calibrated') is False)
    fun = measured['bots'].get('funsim', {})
    time_loss = fun.get('first_time_loss_rate')
    limit = .05 if card['tip'] in ('tanitim', 'nefes') else .15
    check('R20 süreden kayıp', time_loss is not None and time_loss <= limit, time_loss, f'<={limit}',
          'timer', min(600, math.ceil(recipe['timer'] * 1.3 / 5) * 5),
          warning=card['bolum_no'] > 10 or time_loss is None or fun.get('runs', 0) < 20)
    shapes = recipe.get('shapes', [[recipe.get('w'), recipe.get('h')]])
    square_shapes = [[min(w, h), min(w, h)] for w, h in shapes]
    check('R21 tahta oranı', abs(level['w'] - level['h']) <= 1 and all(abs(w - h) <= 1 for w, h in shapes),
          dict(board=[level['w'], level['h']], shapes=shapes), '|w-h|<=1', 'shapes', square_shapes,
          warning=card['bolum_no'] > 10)
    check('R22 çeşitlilik', similar.get('candidate_max', 0) < .90, similar.get('candidate_max', 0),
          '<0.90', 'shapes', alternative_shapes(recipe, card['tip']), warning=True)
    for c in checks:
        if c['status'] == 'KALDI' and c['rule'].split()[0] in {'R09', 'R10'} and recipe['bombs']:
            index = int(c['correction']['field'].split('[')[1].split(']')[0])
            if index < len(recipe['bombs']):
                current = recipe['bombs'][index].get('region', 'any')
                c['placement'] = dict(field=f'bombs[{index}].region', value='center' if current != 'center' else 'bottom')
    if any(s['unk'] for s in steps):
        for c in checks:
            if c['rule'].split()[0] in {'R01', 'R03', 'R04', 'R05', 'R06', 'R07', 'R08', 'R09', 'R11'}:
                c.update(status='UYARI', correction=None)
    return checks


def decision(checks):
    return 'RED' if any(c['status'] == 'KALDI' for c in checks) else 'GEÇTİ'


def remaining_source(level, measured, player):
    remaining = player['remaining']
    if remaining.get('n', 0) >= 20 and player.get('calibrated', False):
        return remaining, player['source']
    remaining = dict(measured['bots'].get('funsim', {}).get('time_left', distribution([])))
    for key in ('median', 'p25', 'p75'):
        if remaining.get(key) is not None:
            remaining[key] /= level['timer']
    return remaining, 'FunSim'


def alternative_shapes(recipe, kind):
    pool = [(5, 5), (6, 6), (5, 6), (6, 5), (7, 7), (6, 7)]
    current = recipe.get('shapes', [[recipe.get('w'), recipe.get('h')]])
    unused = [list(shape) for shape in pool if list(shape) not in current]
    return (unused + [list(shape) for shape in pool if list(shape) in current])[:3]


def placement_note(fix, recipe):
    field, value = fix['field'], fix['value']
    if field.endswith('.earliest'):
        old = recipe_value(recipe, field)
        return f'- Yerleşim: bombayı {old[0]}. sıradan {value[0]}. sıraya al; earliest {value}.'
    if field.endswith('.region'):
        names = dict(top='üst', bottom='alt', left='sol', right='sağ', center='orta', any='herhangi bir')
        return f'- Yerleşim: bombayı açılıştan uzak {names[value]} bölgeye al; region={value}.'
    if field == 'shapes':
        return f'- Yerleşim: adayları farklı tahtalarda üret: {value}.'
    return '- Öneriyi yeni adaylarda yeniden ölç.'


def select_finalists(card, pairs):
    midpoint = sum(card['kalan_sure_orani']) / 2
    def rank(pair):
        value = pair[1].get('selection_remaining')
        if 'selection_remaining' not in pair[1]:
            value = pair[1]['player']['remaining']['median']
        return (abs(value - midpoint) if value is not None else math.inf,
                pair[1]['similarity']['maximum'], pair[0]['id'])
    ordered = sorted(pairs, key=rank)
    compatible = [(a, b) for a, b in itertools.combinations(ordered, 2)
                  if mask_similarity(a[0], b[0]) < .90]
    if compatible:
        return list(min(compatible, key=lambda pair: (rank(pair[0]), rank(pair[1]))))
    return ordered[:1]


def conflicts(rows):
    # Collapse per-piece/subcheck names into rule IDs: a rule passes only if
    # every subcheck is GEÇTİ. UYARI cannot prove a conflict.
    states = []
    for row in rows:
        grouped = {}
        for check in row['checks']:
            grouped.setdefault(check['rule'].split()[0], []).append(check['status'])
        states.append({key for key, values in grouped.items() if all(v == 'GEÇTİ' for v in values)})
    rules = sorted(set().union(*states)) if states else []
    result = [f'{a} ve {b} aynı anda sağlanamadı: tek tek geçen adaylar var; '
              'hiçbir ölçülmüş adayda ikisi birlikte GEÇTİ değil (bu aday örneklemi için).'
              for a, b in itertools.combinations(rules, 2)
              if not any({a, b} <= state for state in states)]
    return result or ['Bu aday örnekleminde tek tek geçen fakat birlikte geçmeyen kural çifti bulunamadı.']


def calibrate(measurement_path):
    levels = read(ROOT / 'Assets/_Game/Levels/redesign.json')['levels']
    measurements = read(measurement_path)['levels']
    byid = {m['id']: m for m in measurements}
    if len(levels) != 40 or len(measurements) != 40 or set(byid) != {l['id'] for l in levels}:
        raise ValueError('Kalibrasyon SET40 için tam 40 eşleşen ölçüm gerektirir')
    rows, groups, fallback = [], {}, []
    for index, level in enumerate(levels, 1):
        model = player_model(level, byid[level['id']])
        tail = model.get('tail_time_share') or distribution([])
        kind = level.get('genre') or (level.get('zeydbot') or {}).get('kind')
        if not kind:
            kind = 'tanitim' if index <= 10 else 'normal'
            fallback.append(level['id'])
        kind = {'kolay': 'tanitim', 'easy': 'tanitim', 'intro': 'tanitim',
                'orta': 'normal', 'cokzor': 'super-zor'}.get(kind, kind)
        row = dict(id=level['id'], tip=kind, source=model['source'],
                   calibrated=model.get('calibrated', False), tail_time_share=tail)
        rows.append(row)
        if tail['median'] is not None:
            groups.setdefault(kind, []).append(tail['median'])
    result = dict(source=str(measurement_path), metric='ZeydBot v1 kazanan son %30 süre payı; bölüm medyanlarının medyanı',
                  fallback_note='genre/zeydbot.kind yoksa bölüm 1–10 tanıtım/kolay, kalanı normal',
                  fallback_ids=fallback, levels=rows,
                  all=distribution([r['tail_time_share']['median'] for r in rows if r['tail_time_share']['median'] is not None]),
                  by_type={k: distribution(v) for k, v in groups.items()})
    result['status'] = 'GEÇTİ' if result['all']['n'] == 40 and all(
        r['tail_time_share']['n'] >= 20 and r['calibrated'] for r in rows) else 'UYARI'
    write(HERE / 'kalibrasyon.json', result)
    print(f"Kalibrasyon: {result['all']['n']}/40 bölüm; {result['status']}")


def spark(values):
    chars = '▁▂▃▄▅▆▇█'
    peak = max(values, default=1) or 1
    return ''.join(chars[min(7, int(v * 7 / peak))] for v in values)


def report(card, row):
    if row['id'].startswith(('S', 'Z')):
        raise ValueError('S*/Z* raporları başka işe ait; üzerine yazılamaz')
    m, player = row['metrics'], row['player']
    lines = [f"# {row['id']} · {row['decision']}", '',
             f"{card['tip']} · {card['sure_sn']} sn · {card['fikir']}",
             f"Anahtar: {card['anahtar']}", '',
             f"Eğri: {spark(row['choices'])} · " + ','.join(map(str, row['choices'])),
             f"{m['adim_sayisi']} adım; {m['bogaz_sayisi']} boğaz, en uzun {m['en_uzun_bogaz']}; göreli çığ {m['cig_goreli']:.3f}.", '',
             '| Kontrol | Sonuç | Ölçüm | Hedef |', '|---|---|---|---|']
    for c in row['checks']:
        lines.append(f"| {c['rule']} | {c['status']} | {json.dumps(c['measured'], ensure_ascii=False)} | {c['target']} |")
    random_bot, forget, aware, fun = (row['bots'][k] for k in
        ('bot_rastgele_bombasiz', 'bot_bomba_unutan', 'bot_bilincli', 'funsim'))
    lines += ['', f"Rastgele (1000): kazanma {random_bot['win_rate']:.1%}, bomba {random_bot['bomb_rate']:.1%}, donma {random_bot['frozen_rate']:.1%}.",
              f"Unutan: {forget['outcome']}, {forget['moves']} hamle. Bilinçli: {aware['outcome']}, {aware['moves']} hamle; bombalar {aware['bomb_exits']}.",
              f"FunSim: ilk deneme kazanma {fun['win_rate']:.1%}, near-miss {fun['near_miss']:.1%}.",
              f"Oyuncu modeli: {player['source']} · kazanma {player['win_rate']:.1%}; kalan/timer {player['remaining']}."]
    if player.get('fallback_reason'):
        lines.append('FunSim geri dönüş nedeni: ' + player['fallback_reason'])
    if player.get('tail_time_share') is None:
        lines.append('K5 süre ölçümü yok: final temposu UYARI; kalibrasyon eşiği tabloda gösterilir.')
    for kind in ('curve', 'mask'):
        lines += [f"En yakın 3 ({kind}): " + '; '.join(f"{r['id']} ({r['set']}): {r[kind]:.3f}" for r in row['similarity'][kind])]
    if card['tip'] == 'tanitim':
        lines.append(f"İlk 1–9 en yakın siluet: {row['similarity']['intro_nearest']} ({row['similarity']['intro_max']:.3f}).")
    lines += ['', 'siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.',
              f"FunSim ilk deneme süreden kayıp: {fun.get('first_time_loss_rate', 'ölçüm yok')}",
              '', f"Eleştirmen: **{row['decision']}**"]
    for c in sorted(row['checks'], key=lambda c: not c['rule'].startswith('R20 ')):
        if c['status'] == 'KALDI':
            fix = c['correction']
            old = recipe_value(row['recipe'], fix['field'])
            lines.append(placement_note(fix, row['recipe']))
            if c.get('placement'):
                lines.append(placement_note(c['placement'], row['recipe']))
            lines.append(f"- {c['rule']}: ölçülen {c['measured']}, hedef {c['target']}; tarif `{fix['field']}` {old} → {fix['value']}.")
    (CARDS / f"{row['id']}_rapor.md").write_text('\n'.join(lines) + '\n', encoding='utf-8')


def recipe_value(recipe, field):
    if field == 'w,h':
        return recipe.get('shapes', [[recipe.get('w'), recipe.get('h')]])[0]
    if field.startswith('bombs['):
        i = int(field.split('[')[1].split(']')[0])
        return recipe['bombs'][i].get(field.split('.')[-1]) if i < len(recipe['bombs']) else None
    return recipe.get(field)


def evaluate(card, candidate_path, no_unity=False, measurement_path=None):
    levels = read(candidate_path)['levels']
    ids = [l['id'] for l in levels]
    if len(set(ids)) != len(ids):
        raise ValueError('Tekrarlanan aday id')
    measurement_path = measurement_path or LOG / f"olcum_{card['id']}.json"
    if not levels:
        return []
    if not no_unity:
        # Remove stale measurement before starting the batch.
        Path(measurement_path).unlink(missing_ok=True)
        batch('AjanOlcum.Run', '-rsFile', str(Path(candidate_path).resolve()), '-rsOut', str(Path(measurement_path).resolve()))
    measurements = read(measurement_path)['levels']
    if {m['id'] for m in measurements} != set(ids) or len(measurements) != len(levels):
        raise ValueError('Ölçüm ile aday kimlikleri uyuşmuyor')
    byid = {m['id']: m for m in measurements}
    measured = {}
    for level in levels:
        m = byid[level['id']]
        metrics, _, choices, traps = hikaye_egrisi.measure(m)
        n = len(m['steps'])
        tail_start = n - math.ceil(n * .30)
        # Actual pieces remaining before each move; no absolute-choice proxy.
        metrics['cig_goreli'] = sum(len(s['ok']) / max(1, m['pieces'] - i)
                                   for i, s in enumerate(m['steps']) if i >= tail_start) / max(1, n - tail_start)
        metrics['tuzak_adim'] = sum(t > 0 for t in traps)
        measured[level['id']] = metrics, choices
    candidates = [(l, measured[l['id']][0]['egri_imzasi']) for l in levels]
    output = []
    for level in levels:
        m = byid[level['id']]
        metrics, choices = measured[level['id']]
        similar = comparisons(level, metrics['egri_imzasi'], candidates)
        player = player_model(level, m)
        checks = critique(card, level, m, metrics, similar, player)
        row = dict(id=level['id'], decision=decision(checks),
                   metrics=metrics, choices=choices, checks=checks, bots=m['bots'], player=player,
                   similarity=similar, recipe=copy.deepcopy(card['tarif']),
                   selection_remaining=remaining_source(level, m, player)[0]['median'])
        report(card, row)
        output.append(row)
    return output


def revise(recipe, rows):
    recipe = copy.deepcopy(recipe)
    changed = set()
    seed_recommendation = recipe['seed_to'] + 1
    # R20 overrides every other timer suggestion across all candidates.
    time_fixes = [c['correction']['value'] for row in rows for c in row['checks']
                  if c.get('rule', '').startswith('R20 ') and c.get('correction')]
    if time_fixes:
        recipe['timer'] = max(time_fixes)
        changed.add('timer')
    for row in rows:
        fixes = [fix for check in row['checks'] for fix in (check.get('correction'), check.get('placement')) if fix]
        for fix in fixes:
            if not fix or fix['field'] in changed:
                continue
            field, value = fix['field'], fix['value']
            changed.add(field)
            if field == 'seed_from':
                seed_recommendation = max(seed_recommendation, value)
            if field == 'w,h':
                recipe['w'], recipe['h'] = value
                if recipe.get('shapes'):
                    recipe['shapes'] = alternative_shapes(recipe, recipe['genre'])
            elif field.startswith('bombs['):
                i = int(field.split('[')[1].split(']')[0])
                if i < len(recipe['bombs']):
                    recipe['bombs'][i][field.split('.')[-1]] = value
            elif field != 'seed_from':
                recipe[field] = value
    span = recipe['seed_to'] - recipe['seed_from'] + 1
    recipe['seed_from'] = seed_recommendation
    recipe['seed_to'] = seed_recommendation + span - 1
    return recipe


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--kart', type=Path)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument('--kalibre', type=Path)
    mode.add_argument('--aday', type=Path)
    mode.add_argument('--uret', action='store_true')
    parser.add_argument('--no-unity', action='store_true', help='Yalnız önceden üretilmiş/ölçülmüş çıktıları oku')
    parser.add_argument('--olcum', type=Path, help='--aday --no-unity için ölçüm JSON dosyası')
    args = parser.parse_args(argv)
    if args.kalibre:
        calibrate(args.kalibre)
        return
    if not args.kart:
        parser.error('--kart gerekli')
    card = load_card(args.kart)
    LOG.mkdir(parents=True, exist_ok=True)
    CARDS.mkdir(parents=True, exist_ok=True)
    if args.aday:
        rows = evaluate(card, args.aday, args.no_unity, args.olcum)
        write(LOG / f"{card['id']}_ozet.json", dict(id=card['id'], levels=rows))
    else:
        if args.olcum:
            raise ValueError('--olcum yalnız --aday ile kullanılır')
        recipe = copy.deepcopy(card['tarif'])
        all_rows, passed, all_levels = [], {}, {}
        turns = CARDS / f"{card['id']}_turlar.md"
        # A fresh invocation starts a new reproducible run history.
        turns.write_text('| Tur | Aday | Karar | KALDI | UYARI |\n|---|---|---|---|---|\n', encoding='utf-8')
        for turn in range(1, 4):
            recipe['id'] = card['id'] if turn == 1 else f"{card['id']}_t{turn}"
            work = copy.deepcopy(card)
            work['tarif'] = copy.deepcopy(recipe)
            work['sure_sn'] = recipe['timer']
            recipe_path = LOG / f"{card['id']}_tarif_tur{turn}.json"
            candidate_path = LOG / f"{card['id']}_tur{turn}.json"
            measurement_path = LOG / f"olcum_{card['id']}_tur{turn}.json"
            write(recipe_path, recipe)
            if not args.no_unity:
                candidate_path.unlink(missing_ok=True)
                batch('AjanUret.Run', '-rsRecipe', str(recipe_path), '-rsOut', str(candidate_path))
            rows = evaluate(work, candidate_path, args.no_unity, measurement_path)
            levels = {l['id']: l for l in read(candidate_path)['levels']}
            all_levels.update(levels)
            with turns.open('a', encoding='utf-8') as stream:
                if not rows:
                    stream.write(f'| {turn} | — | UYARI | — | Üretim süzgecinden geçen aday yok; seed_from +400 |\n')
                for row in rows:
                    reasons = '; '.join(c['rule'] for c in row['checks'] if c['status'] == 'KALDI') or '—'
                    warnings = '; '.join(c['rule'] for c in row['checks'] if c['status'] == 'UYARI') or '—'
                    stream.write(f"| {turn} | {row['id']} | {row['decision']} | {reasons} | {warnings} |\n")
                    if row['decision'] == 'GEÇTİ':
                        passed[row['id']] = levels[row['id']], row
            all_rows.extend(rows)
            if len(select_finalists(card, passed.values())) >= 2:
                break
            # Revisions use rejected candidates only; accepted boards remain intact.
            recipe = revise(recipe, [r for r in rows if r['decision'] == 'RED'])
        finalists = select_finalists(card, passed.values())
        if not passed and all_rows:
            closest = min(all_rows, key=lambda r: (sum(c['status'] == 'KALDI' for c in r['checks']), r['id']))
            level = copy.deepcopy(all_levels[closest['id']])
            level['etiket'] = 'aday'
            finalists = [(level, closest)]
            with turns.open('a', encoding='utf-8') as stream:
                stream.write('\nGEÇTİ yok; en az KALDI içeren aday finalist olarak etiketlendi.\n')
                for conflict in conflicts(all_rows):
                    stream.write(conflict + '\n')
        write(CARDS / f"{card['id']}_finalist.json", dict(format=1, levels=[p[0] for p in finalists]))
        rows = all_rows
        write(LOG / f"{card['id']}_ozet.json", dict(id=card['id'], levels=rows, finalists=[p[0]['id'] for p in finalists]))
    print(f"{card['id']}: {len(rows)} aday, {sum(r['decision'] == 'GEÇTİ' for r in rows)} GEÇTİ, {sum(r['decision'] == 'RED' for r in rows)} RED")


if __name__ == '__main__':
    if hasattr(sys.stdout, 'reconfigure'):
        sys.stdout.reconfigure(encoding='utf-8')
        sys.stderr.reconfigure(encoding='utf-8')
    try:
        main()
    except (OSError, ValueError, KeyError, RuntimeError) as error:
        sys.exit(str(error))
