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
 (10, 4, 'R', ['C16-ac10bfbb', 'C10-afebfb9d', 'C17-73ba08a9', 'C16-076e1e91'], 'bombasız zirve: serbest görünen ama çıkamayan parçalar'),
 (11, 2, 'R', 'nefes', 'nefes'),
 (12, 2, 'R', ['C12-125a3f3b', 'C12-782c6447', 'C12-be16cb13'], 'tanıtım: zincirli çift'),
 (13, 3, 'R', 'zor', 'zincirin iki ucu farklı yöne ister'),
 (14, 3, 'R', 'zor', 'zincirin iki ucu farklı yöne ister'),
 (15, 2, 'R', ['C15-6d50aae9', 'C15-928804c9', 'C15-1662c4ef'], 'tanıtım: saat'),
 (16, 2, 'R', 'nefes', 'nefes'),
 (17, 3, 'R', 'zor', 'mühür kolay yönü kapatıyor, uzun yoldan dolanmak'),
 (18, 4, 'R', 'zor', 'mühür kolay yönü kapatıyor, uzun yoldan dolanmak'),
 (19, 5, 'R', ['C14-648ee119', 'C17-73ba08a9'], 'bombasız zirve: kalabalıkta tek ilerleten parça'),
 (20, 2, 'R', ['C20-43bbb687', 'C20-45673591', 'C20-facc0b3b'], 'tanıtım: bombalı parça'),
 (21, 3, 'Z', ['Z19', 'Z21'], 'tek bomba, pay +2: bombaya erken yönel [3]'),
 (22, 4, 'Z', ['Z15', 'Z22'], 'tek bomba, çiviler yolu daraltıyor [3]'),
 (23, 3, 'R', ['C23-010df019', 'C23-7cd79db1', 'C23-8910a601'], 'tanıtım: çekiç'),
 (24, 6, 'Z', ['Z16'], 'tek bomba, pay +1: bir boş hamle hakkı [6]'),
 (25, 5, 'Z', ['Z17', 'Z04'], 'bomba zincirin arkasında [4]'),
 (26, 3, 'R', 'nefes', 'nefes'),
 (27, 5, 'R', ['C30-37066ecb'], 'iki bomba, iki ayrı köşe'),
 (28, 7, 'Z', ['Z08'], 'dört bomba, çivi kazma yolunda [7]'),
 (29, 9, 'Z', ['Z36'], 'ZİRVE: bomba önce, sonra art arda anahtar parça anları [9]'),
 (30, 3, 'R', 'nefes', 'nefes'),
 (31, 6, 'Z', ['Z26', 'Z29'], 'bombalar seyrek, süre bol: ritim [6]'),
 (32, 8, 'Z', ['Z07'], 'dört bomba, ilk hamle tuzaklı [8]'),
 (33, 4, 'Z', ['Z10', 'Z02'], 'tek bomba, geniş tahta [2]'),
 (34, 7, 'Z', ['Z29', 'Z26'], 'tuzak dolu yol, her hamle sayılır [6]'),
 (35, 9, 'Z', ['Z13'], 'ZİRVE (cezalandıran): bomba ortada, iki yönden kazılıyor [9]'),
 (36, 4, 'R', 'nefes', 'nefes'),
 (37, 9, 'Z', ['Z37'], 'ZİRVE: kayar ama çıkamaz satırı [9]'),
 (38, 6, 'Z', ['Z25', 'Z23'], 'tek yol, az seçenek, dar fitil [4]'),
 (39, 4, 'R', 'nefes', 'nefes'),
 (40, 9, 'Z', ['Z18'], 'ZİRVE (final): 51 hamlelik kazı, dört çivi [9]'),
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

for slot, target, src, cand, idea in PLAN:
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
