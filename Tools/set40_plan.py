# SET40 v1 (PM_MESAJ_4, PM decision 2026-10-21 (2) = A): fills the 40 slots from the verified material.
# Sources: the 103-draft redesign.json as it was before SET40 (git show 7a40c05:Assets/_Game/Levels/redesign.json > Logs/zdesign/redesign_eski.json) and zeydbot.json.
# Obstacle / booster introductions stay on their slots (nail 3, scissors 5, seal 7, wand 9, chain 12, clock 15,
# bomb 20, hammer 23: the redesign set's order); no bomb before 20. Every pair of chosen levels must stay under the
# M16 threshold (Tools/level_similarity.py, rotation / mirror free) and one idea is used at most twice.
#   python Tools/set40_plan.py  -> Logs/zdesign/set40_plan.json + a markdown table on stdout
import json, os, sys
sys.path.insert(0, os.path.dirname(__file__))
from level_similarity import similarity

M16 = 0.65
M16_PEAK = 0.50          # PM_MESAJ_5: peaks against each other and against every Z level
# peaks: the designer's choices (Logs/zdesign/zirve_secilen.json, ids in order P1..P5) with their ideas
PK = json.load(open('Logs/zdesign/zirve_secilen.json', encoding='utf-8'))['levels']
PIDEA = json.load(open('Logs/zdesign/zirve_fikir.json', encoding='utf-8'))
R = {l['id']: l for l in json.load(open('Logs/zdesign/redesign_eski.json', encoding='utf-8'))['levels']}
Z = {l['id']: l for l in json.load(open('Assets/_Game/Levels/zeydbot.json', encoding='utf-8'))['levels']}
for l in Z.values(): l.setdefault('fabrika', {})

# (slot, target, source, candidates in preference order, idea). Z levels: Zeyd's own 1-10 in brackets in the idea.
PLAN = [
 (1, 1, 'R', ['C01-2086a88b', 'C01-5e45697d'], 'ilk dokunuş: parçalar tek tek çıkar'),
 (2, 1, 'R', 'kolay', 'açık tahta, ısınma'),
 (3, 2, 'R', ['C03-40f02903', 'C03-5be4a237', 'C03-70357341'], 'tanıtım: çivili parça'),
 (4, 2, 'R', 'kolay', 'açık tahta, ısınma'),
 (5, 2, 'R', ['C05-8fcbc1dd', 'C05-9a6db959', 'C05-762a97b1'], 'tanıtım: makas'),
 (6, 3, 'R', 'orta', 'kilitli dikişleri okumak'),
 (7, 2, 'R', ['C07-19057303', 'C07-4ef0899b', 'C07-fe23ac0d'], 'tanıtım: mühürlü kenar'),
 (8, 3, 'R', 'orta', 'kilitli dikişleri okumak'),
 (9, 2, 'R', ['C09-111692a5', 'C09-7cebcdcf', 'C09-c78731ab'], 'tanıtım: değnek'),
 (10, 3, 'R', ['C16-ac10bfbb', 'C10-afebfb9d', 'C17-73ba08a9', 'C16-076e1e91'], 'bombasız zirve: serbest görünen ama çıkamayan parçalar'),
 (11, 2, 'R', 'nefes', 'nefes'),
 (12, 2, 'R', ['C12-125a3f3b', 'C12-782c6447', 'C12-be16cb13'], 'tanıtım: zincirli çift'),
 (13, 3, 'R', 'zor', 'zincirin iki ucu farklı yöne ister'),
 (14, 3, 'R', 'zor', 'zincirin iki ucu farklı yöne ister'),
 (15, 2, 'R', ['C15-6d50aae9', 'C15-928804c9', 'C15-1662c4ef'], 'tanıtım: saat'),
 (16, 2, 'R', 'nefes', 'nefes'),
 (17, 3, 'R', 'zor', 'mühür kolay yönü kapatıyor, uzun yoldan dolanmak'),
 (18, 3, 'R', 'zor', 'mühür kolay yönü kapatıyor, uzun yoldan dolanmak'),
 (19, 3, 'R', ['C14-648ee119', 'C17-73ba08a9'], 'bombasız zirve: kalabalıkta tek ilerleten parça'),
 (20, 2, 'R', ['C20-43bbb687', 'C20-45673591', 'C20-facc0b3b'], 'tanıtım: bombalı parça'),
 (21, 3, 'Z', ['Z19', 'Z21'], 'tek bomba, pay +2: bombaya erken yönel [3]'),
 (22, 4, 'Z', ['Z15', 'Z22'], 'tek bomba, çiviler yolu daraltıyor [3]'),
 (23, 3, 'R', ['C23-010df019', 'C23-7cd79db1', 'C23-8910a601'], 'tanıtım: çekiç'),
 (24, 6, 'Z', ['Z16'], 'tek bomba, pay +1: bir boş hamle hakkı [6]'),
 (25, 5, 'Z', ['Z17', 'Z04'], 'bomba zincirin arkasında [4]'),
 (26, 3, 'R', 'nefes', 'nefes'),
 (27, 5, 'R', ['C30-37066ecb'], 'iki bomba, iki ayrı köşe'),
 (28, 7, 'Z', ['Z08'], 'dört bomba, çivi kazma yolunda [7]'),
 (29, 7, 'P', 'P1', 'ZİRVE'),
 (30, 4, 'Z', ['Z10', 'Z02', 'Z24'], 'nefes: tek bomba, geniş tahta [2]'),
 (31, 6, 'Z', ['Z26'], 'bombalar seyrek, süre bol: ritim [6]'),
 (32, 8, 'Z', ['Z07'], 'dört bomba, ilk hamle tuzaklı [8]'),
 (33, 7, 'P', 'P2', 'ZİRVE'),
 (34, 4, 'Z', ['Z22', 'Z24', 'Z21'], 'nefes: tek bomba, çiviler [2]'),
 (35, 8, 'Z', ['Z13'], 'cezalandıran zor: tuzak dolu [9]'),
 (36, 7, 'P', 'P3', 'ZİRVE'),
 (37, 8, 'Z', ['Z18'], 'uzun kazı, dört çivi [9]'),
 (38, 8, 'P', 'P4', 'ZİRVE'),
 (39, 8, 'Z', ['Z29'], 'gerilim: tuzak dolu yol, her hamle sayılır [6]'),
 (40, 9, 'P', 'P5', 'ZİRVE (final)'),
]

chosen, used_ideas, out = [], {}, []
def intro_ok(l, slot):
    # obstacles not before their introduction (SlotSpec.IntroducedAt: nail 3, seal 7, chain 12, bomb 20)
    pcs = l['pieces'] if isinstance(l['pieces'], list) else json.loads(l['pieces'])
    if l.get('nails') and slot < 3: return False
    if any((l.get('sealed') or {}).get(k) for k in 'UDLR') and slot < 7: return False
    if any(len(c) == 2 for c in pcs) and slot < 12: return False
    if l.get('bombs') and slot < 20: return False
    return True

def ok(l):
    return all(similarity(l, c)[0] < M16 and similarity(c, l)[0] < M16 for c in chosen)

PEAKS = []
for slot, target, src, cand, idea in PLAN:
    if src == 'P':
        l = PK[int(cand[1:]) - 1]
        # M16 for peaks (PM_MESAJ_5): under 50 % against every Z level and every other peak, under 65 % against the rest
        zmax = max(similarity(l, z)[0] for z in Z.values())
        pmax = max([similarity(l, q)[0] for q in PEAKS] or [0])
        if zmax >= M16_PEAK or pmax >= M16_PEAK or not ok(l): print(f'slot {slot}: zirve {l["id"]} eşik üstü (Z %{zmax*100:.0f}, zirve %{pmax*100:.0f})'); continue
        PEAKS.append(l); chosen.append(l)
        out.append({'slot': slot, 'target': target, 'src': 'P', 'id': l['id'], 'genre': 'cokzor', 'idea': 'ZİRVE: ' + PIDEA[l['id']], 'size': f"{l['w']}x{l['h']}",
                    'pieces': len(l['pieces']), 'bombs': len(l.get('bombs') or {}), 'z_max': round(zmax, 3), 'peak_max': round(pmax, 3)})
        continue
    pool = R if src == 'R' else Z
    if isinstance(cand, str):
        cand = sorted([k for k, l in R.items() if l['genre'] == cand], key=lambda k: (R[k]['fabrika'].get('slot', 0), k))
    pick = next((k for k in cand if k not in [o['id'] for o in out] and intro_ok(pool[k], slot) and ok(pool[k])), None)
    if pick is None: print(f'slot {slot}: M16 altında aday yok ({cand[:3]})'); continue
    if used_ideas.get(idea, 0) >= 2 and idea != 'nefes': print(f'slot {slot}: fikir 2 kez kullanıldı: {idea}')
    used_ideas[idea] = used_ideas.get(idea, 0) + 1
    l = pool[pick]; chosen.append(l)
    out.append({'slot': slot, 'target': target, 'src': src, 'id': pick, 'genre': l.get('genre') or 'zor', 'idea': idea, 'size': f"{l['w']}x{l['h']}",
                'pieces': len(l['pieces'] if isinstance(l['pieces'], list) else json.loads(l['pieces'])), 'bombs': len(l.get('bombs') or {})})
json.dump(out, open('Logs/zdesign/set40_plan.json', 'w', encoding='utf-8'), ensure_ascii=False, indent=0)
# the highest pairwise similarity in the set
mx = max(((similarity(a, b)[0], i, j) for i, a in enumerate(chosen) for j, b in enumerate(chosen) if i < j), default=(0, 0, 0))
print(f'{len(out)} slot dolu; en yüksek çift benzerliği %{mx[0] * 100:.0f} ({out[mx[1]]["id"]} – {out[mx[2]]["id"]})')
