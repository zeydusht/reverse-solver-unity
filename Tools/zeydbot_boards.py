# Designer loop: readable boards for the designer agent (piece numbers, bombs with fuse, nails, chains,
# sealed lanes, joints, solution order) -> Logs/zdesign/tahtalar.md
import json, sys
L = {l['id']: l for l in json.load(open('Assets/_Game/Levels/zeydbot.json', encoding='utf-8'))['levels']}
out = ['# Taban tahtalar (zeydbot.json)', '',
       'Hücre: parça no; B = bomba (fitil), N = çivi (sayaç), ~ = iki hücreli zincir parça. Koordinat (x, y), sol üst (0, 0).',
       'vEdge[y][x]: (x, y) hücresinin SOL kenarı (x = 1..w-1); hEdge[y][x]: ÜST kenarı (y = 1..h-1). 0 düz, ±1 diş (kilitli dikiş).', '']
for lid in sys.argv[1:]:
    l = L[lid]; w, h = l['w'], l['h']
    pieces = l['pieces'] if isinstance(l['pieces'], list) else json.loads(l['pieces'])
    at = {}
    for i, cells in enumerate(pieces):
        for c in cells: at[(c[0], c[1])] = i
    out += [f"## {lid} ({w}×{h}, süre {l['timer']}, bomba {l['bombs']}, çivi {l['nails']}, mühür {l.get('sealed')})", '', '```']
    out.append('     ' + ''.join(f'{x:>6}' for x in range(w)))
    for y in range(h):
        row = []
        for x in range(w):
            i = at.get((x, y))
            if i is None: row.append('     .'); continue
            t = str(i)
            if str(i) in l['bombs']: t += 'B' + str(l['bombs'][str(i)])
            if str(i) in l['nails']: t += 'N' + str(l['nails'][str(i)])
            if len(pieces[i]) == 2: t += '~'
            row.append(f'{t:>6}')
        out.append(f'{y:>4} ' + ''.join(row))
    out.append('```')
    out.append('vEdge: ' + json.dumps(l['vEdge']))
    out.append('hEdge: ' + json.dumps(l['hEdge']))
    out.append('Çözüm sırası (parça yön): ' + ' '.join(f"{m['piece']}{m['dir']}" for m in l['solution']))
    out.append('')
open('Logs/zdesign/tahtalar.md', 'w', encoding='utf-8').write('\n'.join(out))
print('ok', len(out))
