from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,hashlib,shutil,random,html,sys
from datetime import datetime
ROOT=Path(__file__).resolve().parents[2];W=ROOT/'docs/art/task25';O=ROOT/'Assets/Art/Paintings_v2';OLD=ROOT/'Assets/Art/Paintings';R=Image.Resampling.LANCZOS;FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(p,x):p.write_text(json.dumps(x,ensure_ascii=False,indent=2),encoding='utf-8')
def label(d,xy,s,pt=20):d.text(xy,s,font=ImageFont.truetype(str(FONT),pt),fill='#fff6d8')
def tiles(im,n,cols,rows,where):
 where.mkdir(parents=True,exist_ok=True);cw=200;ch=210;sheet=Image.new('RGB',(cols*cw,rows*ch),'#123f46');d=ImageDraw.Draw(sheet);parts=[]
 for row in range(rows):
  for col in range(cols):
   p=im.crop((round(col*im.width/cols),round(row*im.height/rows),round((col+1)*im.width/cols),round((row+1)*im.height/rows)));p.save(where/f'r{row+1}c{col+1}.png');parts.append((row+1,col+1,p));a=p.copy();a.thumbnail((194,174),R);sheet.paste(a,(col*cw+(cw-a.width)//2,row*ch+32));label(d,(col*cw+5,row*ch+4),f'{n:02} r{row+1}c{col+1}',17)
 sheet.save(where.parent/f'butun-{cols}x{rows}.jpg',quality=94)
 return parts
def export(g,records):
 n=g['level'];tag=g['phase'];key=f'{n:02}';raw=Path(g['generatedPath']);a=Image.open(raw).convert('RGB');assert a.width==a.height;prev=O/'Denemeler'/f'T25-{tag}-oncesi';prev.mkdir(exist_ok=True)
 for p in [O/f'painting-{key}.jpg',O/'Source'/f'painting-{key}.png']:
  if p.exists():
   target=prev/('Source-'+p.name if p.suffix=='.png' else p.name)
   if not target.exists():shutil.copyfile(p,target)
 shutil.copyfile(raw,O/'Denemeler'/f'painting-{key}-T25-{tag}-generated.png');im=a.resize((1536,1536),R);src=O/'Source'/f'painting-{key}.png';im.save(src);small=im.resize((1024,1024),R);game=O/f'painting-{key}.jpg'
 for q in range(90,64,-2):
  small.save(game,quality=q,optimize=True,progressive=True,subsampling=0)
  if game.stat().st_size<=300000:break
 assert game.stat().st_size<=300000
 qa=W/'qa'/tag/key;parts=tiles(small,n,6,7,qa/'parcalar-6x7');sample=random.Random(2500+n).sample(range(42),6);sheet=Image.new('RGB',(1200,220),'#123f46');d=ImageDraw.Draw(sheet)
 for i,k in enumerate(sample):
  row,col,p=parts[k];p=p.copy();p.thumbnail((192,176),R);sheet.paste(p,(i*200+(200-p.width)//2,34));label(d,(i*200+5,3),f'{n:02} r{row}c{col}',17)
 sheet.save(qa/'alti-parca.jpg',quality=94)
 if n<=3:
  for c in [3,4]:tiles(small,n,c,c,qa/f'parcalar-{c}x{c}')
 t=next((x for x in json.loads((W/'topics.json').read_text(encoding='utf-8')) if x[0]==n),None);r=dict(records.get(n,{}));r.update(level=n,theme=t[1] if t else r['theme'],titleTR=t[1] if t else r['titleTR'],titleEN=t[2] if t else r['titleEN'],scene=t[3] if t else r['scene'],source=f'Source/painting-{key}.png',sourceSize=[1536,1536],sourceBytes=src.stat().st_size,generatedSize=list(a.size),normalization='LANCZOS1536 square; no crop; generated original retained in Denemeler',game=game.name,gameSize=[1024,1024],jpegQuality=q,gameBytes=game.stat().st_size,sourceSha256=sha(src),gameSha256=sha(game),task25Phase=tag,qaDirectory=str(qa.relative_to(ROOT)).replace('\\','/'),qa={'grid':[6,7],'allPieces':42,'sampleSeed':2500+n,'sampleCount':6,'visualReview':'pending'})
 if tag=='A':r['prompt']=g['prompt']
 else:r.setdefault('revisionHistory',[]).append({'task':'CODEX_TASK25','phase':tag,'prompt':g['prompt'],'previousArchive':str(prev.relative_to(O)).replace('\\','/')})
 records[n]=r
def review(m):
 body=['<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Tablolar1–40 · Task25</title><style>body{max-width:1200px;margin:auto;padding:20px;background:#123f46;color:#fff6d8;font:17px system-ui}a{color:#ffdb79}.pair{display:grid;grid-template-columns:1fr 1fr;gap:16px}img{width:100%;height:auto}article{padding:22px 0;border-top:1px solid #6b9e9c}p{line-height:1.5}@media(max-width:600px){.pair{grid-template-columns:1fr}}</style><h1>Tablolar1–40 · Task25</h1><p>Eski solda, yeni sağda. Mevcut Paintings korunur; oyuna bağlanmadı.1–10 yeni stilde,21/23/24/30/40 parça ayrımı ve üç gece tonu düzeltmesi.</p>']
 for r in m['paintings']:
  n=r['level'];qa=r.get('qaDirectory','docs/art/task24');sample=qa+'/alti-parca.jpg' if 'qaDirectory' in r else qa+f'/alti-parca-{n:02}.jpg';full=qa+'/butun-6x7.jpg' if 'qaDirectory' in r else qa+f'/izgara-{n:02}.jpg'
  body.append(f'<article id="tablo-{n}"><h2>{n:02} · {html.escape(r["titleTR"])} / {html.escape(r["titleEN"])}</h2><p>{html.escape(r["scene"])}</p><div class="pair"><div>Önce<img loading="lazy" src="../Paintings/painting-{n:02}.jpg"></div><div>Yeni<img loading="lazy" src="painting-{n:02}.jpg"></div></div><p><a href="Source/painting-{n:02}.png">1536px kaynak</a> · <a href="../../../{sample}">6 örnek parça</a> · <a href="../../../{full}">6×7 bütün parça / ızgara</a></p></article>')
 (O/'inceleme.html').write_text(''.join(body)+'</html>',encoding='utf-8')
 for start in [1,11,21,31]:
  ns=[n for n in range(start,start+10) if any(x['level']==n for x in m['paintings'])]
  if not ns:continue
  im=Image.new('RGB',(len(ns)*320,730),'#123f46');d=ImageDraw.Draw(im)
  for i,n in enumerate(ns):
   for row,p in enumerate([OLD/f'painting-{n:02}.jpg',O/f'painting-{n:02}.jpg']):im.paste(Image.open(p).resize((310,310),R),(i*320+5,row*365+45));label(d,(i*320+8,row*365+8),f'{n:02} '+('ÖNCE' if row==0 else 'YENİ'))
  im.save(O/f'once-sonra-{start:02}-{start+9:02}.png')
def run(phase):
 mpath=O/'manifest-v2.json';m=json.loads(mpath.read_text(encoding='utf-8'));records={r['level']:r for r in m['paintings']};gens=json.loads((W/'generations.json').read_text(encoding='utf-8'))
 for g in gens:
  if g['phase']==phase:export(g,records)
 m['paintings']=[records[n] for n in sorted(records)];m.update(task='CODEX_TASK25',paintingCount=len(records),complete=len(records)==40,visualReviewComplete=False);m['batches']=[{'batch':i+1,'room':room,'firstLevel':a,'lastLevel':a+9,'preview':f'once-sonra-{a:02}-{a+9:02}.png'} for i,(a,room) in enumerate([(1,'Akdeniz ve kıyı köyü'),(11,'Orman ve kır salonu'),(21,'Kış ve dağ salonu'),(31,'Gece ve büyü salonu')])];write(mpath,m);review(m)
 baseline=json.loads((W/'paintings-baseline.json').read_text(encoding='utf-8'));assert all((ROOT/p).exists() and sha(ROOT/p)==v for p,v in baseline.items());print(phase,'exported;',len(records),'paintings in manifest; original Paintings unchanged')
if __name__=='__main__':run(sys.argv[1])
