# PM_69 kontrol tablosu: level_features_v2.csv'den (set redesign-v4) seçili bölümler.  python -I pm69_kontrol.py
import csv
R = {(r['level_id']): r for r in csv.DictReader(open('level_features_v2.csv', encoding='utf-8')) if r['set_name'] == 'redesign-v4'}
old = {}
for line in open('feat.txt', encoding='utf-8').read().strip().split('\n'):
    v = line.split(','); old[(v[1], v[2])] = v[11] if v[11] != '-1' else ''           # fuse_slack_min (level_features)
cols = [('slot', 'slot'), ('pieces', 'parça'), ('fuse', 'fitil'), ('old', 'eski pay'), ('slack_true', 'pay_true'), ('choke_n', 'boğaz'),
        ('p_survive_rand', 'p_rand'), ('p_survive_greedy', 'p_greedy'), ('p_survive_aware', 'p_aware'),
        ('nat_slack_med_rand', 'nat_pay_rand'), ('nat_slack_med_aware', 'nat_pay_aware'), ('first_choke_ex_rand', 'ilk_patlama'),
        ('distract_max', 'çeldirici'), ('distract_vs_slack_max', 'çeld/(pay+1)'), ('open_mean', 'open_ort'), ('narrow_n', 'dar'),
        ('narrow_hidden_n', 'dar_gizli'), ('narrow_by_lock', 'dar_kilit'), ('narrow_by_chain', 'dar_zincir'), ('narrow_by_wall', 'dar_duvar'),
        ('jam_bait_start', 'tuzak_baş'), ('dir_ambiguity', 'yön_belirsiz')]
def table(ids):
    print('| bölüm | ' + ' | '.join(c[1] for c in cols) + ' |'); print('|' + '---|' * (len(cols) + 1))
    for i in ids:
        r = dict(R[i]); r['old'] = old.get((i, r['level_version']), '')
        print(f'| {i} | ' + ' | '.join(r[c[0]].replace(';', '/') for c in cols) + ' |')
table(['S31', 'S17', 'S32', 'S18', 'S47', 'S08', 'S50'])
print(); table(['Z25', 'S29', 'Z23', 'S06', 'S36', 'Z24'])
