from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageStat,ImageFilter
import json,hashlib,shutil,random,html,datetime
ROOT=Path(__file__).resolve().parents[2];WORK=ROOT/'docs/art/task24';OUT=ROOT/'Assets/Art/Paintings_v2';OLD=ROOT/'Assets/Art/Paintings';R=Image.Resampling.LANCZOS
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def label(d,xy,s,pt=22):d.text(xy,s,font=ImageFont.truetype(str(FONT),pt),fill='#fff6d8')
def run():
 OUT.mkdir(exist_ok=True);(OUT/'Source').mkdir(exist_ok=True);(OUT/'Denemeler').mkdir(exist_ok=True)
 gens=json.loads((WORK/'generations.json').read_text(encoding='utf-8'));topics={r[0]:r for r in json.loads((WORK/'topics.json').read_text(encoding='utf-8'))};records=[];qa=[]
 for g in gens:
  n=g['level'];raw=Path(g['generatedPath']);assert raw.exists();original=Image.open(raw).convert('RGB');assert original.width==original.height
  for prior in g.get('history',[]):shutil.copyfile(prior['generatedPath'],OUT/'Denemeler'/f'painting-{n:02}-{prior["variant"]}.png')
  shutil.copyfile(raw,OUT/'Denemeler'/f'painting-{n:02}-generated.png');im=original.resize((1536,1536),R);src=OUT/'Source'/f'painting-{n:02}.png';im.save(src)
  game=OUT/f'painting-{n:02}.jpg';small=im.resize((1024,1024),R)
  for quality in range(90,64,-2):
   small.save(game,quality=quality,optimize=True,progressive=True,subsampling=0)
   if game.stat().st_size<=300000:break
  assert game.stat().st_size<=300000
  pieces=[];pd=WORK/'parcalar'/f'{n:02}';pd.mkdir(parents=True,exist_ok=True)
  for row in range(7):
   for col in range(6):
    box=(round(col*1024/6),round(row*1024/7),round((col+1)*1024/6),round((row+1)*1024/7));p=small.crop(box);p.save(pd/f'r{row+1}c{col+1}.png');pieces.append((row+1,col+1,p))
  sample=random.Random(2400+n).sample(range(42),6);sheet=Image.new('RGB',(1200,240),'#123f46');d=ImageDraw.Draw(sheet)
  for i,idx in enumerate(sample):
   row,col,p=pieces[idx];p.thumbnail((190,174),R);sheet.paste(p,(i*200+(200-p.width)//2,35));label(d,(i*200+8,5),f'{n:02} r{row} c{col}',17)
  sheet.save(WORK/f'alti-parca-{n:02}.jpg',quality=94)
  grid=small.copy();draw=ImageDraw.Draw(grid)
  for c in range(1,6):draw.line((round(c*1024/6),0,round(c*1024/6),1024),fill='#fff6d8',width=2)
  for r in range(1,7):draw.line((0,round(r*1024/7),1024,round(r*1024/7)),fill='#fff6d8',width=2)
  grid.save(WORK/f'izgara-{n:02}.jpg',quality=90)
  t=topics[n];records.append(dict(level=n,theme=t[1],titleTR=t[1],titleEN=t[2],scene=t[3],source=f'Source/painting-{n:02}.png',sourceSize=[1536,1536],sourceBytes=src.stat().st_size,generatedSize=list(original.size),normalization='LANCZOS resize to1536 square; no crop; unmodified generated original retained in Denemeler',game=game.name,gameSize=[1024,1024],jpegQuality=quality,gameBytes=game.stat().st_size,sourceSha256=sha(src),gameSha256=sha(game),prompt=g['prompt'],revisionPrompt=g.get('revisionPrompt')))
  qa.append(dict(level=n,grid=[6,7],seed=2400+n,sample=[dict(row=pieces[i][0],col=pieces[i][1]) for i in sample],all42Pieces=str(pd.relative_to(ROOT)),sampleSheet=f'docs/art/task24/alti-parca-{n:02}.jpg',visualReview='pending'))
 (OUT/'manifest-v2.json').write_text(json.dumps(dict(task='CODEX_TASK24',complete=len(records)==30,paintingCount=len(records),generator='built-in image_gen',styleReference='docs/art/STYLE_CARD.md',characterReference='Assets/Art/Characters/Jiggles-reference-v02.png',assetRoot='Assets/Art/Paintings_v2/',sourceSize=[1536,1536],gameSize=[1024,1024],jpegMaxBytes=300000,paintings=records),ensure_ascii=False,indent=2),encoding='utf-8')
 (WORK/'parcalanma-testleri.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2),encoding='utf-8')
 available={g['level'] for g in gens}
 groups=[([11,21,31],'once-sonra-pilot.png')]+[(list(range(a,a+10)),f'once-sonra-{a}-{a+9}.png') for a in [11,21,31]]
 for ns,name in groups:
  ns=[n for n in ns if n in available]
  if not ns:continue
  cw=320;ch=365;im=Image.new('RGB',(cw*len(ns),ch*2),'#123f46');d=ImageDraw.Draw(im)
  for i,n in enumerate(ns):
   for row,path in enumerate([OLD/f'painting-{n:02}.jpg',OUT/f'painting-{n:02}.jpg']):
    im.paste(Image.open(path).resize((310,310),R),(i*cw+5,row*ch+45));label(d,(i*cw+9,row*ch+9),f'{n:02} '+('ÖNCE' if row==0 else 'YENİ'),22)
  im.save(OUT/name)
 body=['<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task24 — Tablolar11–40</title><style>body{max-width:1200px;margin:auto;padding:20px;background:#123f46;color:#fff6d8;font:17px system-ui}a{color:#ffdb79}.pair{display:grid;grid-template-columns:1fr 1fr;gap:16px}img{width:100%;height:auto}article{padding:22px 0;border-top:1px solid #6b9e9c}h2{font-size:24px}p{line-height:1.5}@media(max-width:600px){.pair{grid-template-columns:1fr}}</style><h1>Task24 — Tablolar11–40</h1><p>Eski solda, yeni sağda. Mevcut Paintings değişmedi; oyuna bağlantı yapılmadı. Yeni çizimler büyük biçimler, sakin alanlar ve tek odakla hazırlandı.</p>']
 for r in records:
  n=r['level'];body.append(f'<article id="tablo-{n}"><h2>{n} · {html.escape(r["titleTR"])} / {html.escape(r["titleEN"])}</h2><p>{html.escape(r["scene"])}</p><div class="pair"><div>Önce<img loading="lazy" src="../Paintings/painting-{n:02}.jpg"></div><div>Yeni<img loading="lazy" src="painting-{n:02}.jpg"></div></div><p><a href="Source/painting-{n:02}.png">1536px kaynak</a> · <a href="../../../docs/art/task24/izgara-{n:02}.jpg">6×7 ızgara</a> · <a href="../../../docs/art/task24/alti-parca-{n:02}.jpg">Rastgele6 parça</a></p></article>')
 (OUT/'inceleme.html').write_text(''.join(body)+'</html>',encoding='utf-8')
 baseline=json.loads((WORK/'paintings-baseline.json').read_text(encoding='utf-8'));changes=[p for p,h in baseline.items() if not (ROOT/p).exists() or sha(ROOT/p)!=h];assert not changes,changes
 print(f'{len(records)} exported; Source1536 / JPG1024 <=300000 bytes; {len(records)*42} tiles; old Paintings hash unchanged')
if __name__=='__main__':run()
