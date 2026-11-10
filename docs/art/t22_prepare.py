from pathlib import Path
from datetime import datetime
import json, hashlib, subprocess
from PIL import Image, ImageDraw, ImageFont
ROOT=Path(__file__).resolve().parents[2]
WORK=ROOT/'docs/art/task22'; WORK.mkdir(exist_ok=True)
OUT=ROOT/'Assets/Art/Redesign_T22'; OUT.mkdir(exist_ok=True)
def log(s):
 with (ROOT/'docs/art/GECE_LOG.md').open('a',encoding='utf-8') as f:f.write('\n'+datetime.now().astimezone().isoformat(timespec='seconds')+' — '+s+'\n')
baseline=WORK/'baslangic.json'
if not baseline.exists():
 paths=[]; errors=[]
 for base in ['Assets','ProjectSettings','Tools']:
  for p in (ROOT/base).rglob('*'):
   if not p.is_file() or 'Redesign_T22' in p.parts:continue
   try:paths.append({'path':p.relative_to(ROOT).as_posix(),'size':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
   except OSError as e:errors.append({'path':str(p),'error':str(e)})
 baseline.write_text(json.dumps({'timestamp':datetime.now().astimezone().isoformat(),'files':paths,'errors':errors},ensure_ascii=False,indent=2),encoding='utf-8')
 (WORK/'git-baslangic.txt').write_bytes(subprocess.run(['git','status','--porcelain=v1','-z'],cwd=ROOT,stdout=subprocess.PIPE,stderr=subprocess.PIPE).stdout)
 log('Başlangıç: Task22 ve stil/palet rehberi okundu; korunan ağaçlar için SHA-256 ve mevcut Git durumu kaydedildi. karar: yalnız izinli iki ağaçta adaylar üretilecek; önceden mevcut silinmiş/değişmiş dosyalar geri alınmayacak.')
 log('karar: 2026-11-03-tam henüz yok; 2 Kasım ekranları esas, eksik durumlar tarih belirtilerek eski QA ve salt okunur C# ile tamamlanacak. Canlı URL web aracıyla açılamadı; ekran görüntüleriyle ilerleniyor. Python Windows takma adı yerine kurulu Python 3.14 ve sistem Pillow kullanılıyor; kurulum yapılmadı.')
font=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),14)
groups={
 '01-ftue':list((ROOT/'docs/qa/2026-11-02-ftue').glob('*.png')),
 '02-son-spot':list((ROOT/'docs/qa/2026-11-02-son').glob('375*.png'))+list((ROOT/'docs/qa/2026-11-02-spot').glob('375*.png')),
 '03-menus':list((ROOT/'docs/qa/2026-11-01-p20b').glob('tr_375x667_won12*.png')),
 '04-booster':list((ROOT/'docs/qa/2026-11-02-booster').glob('*.png')),
 '05-referans':list((ROOT/'Assets/Art/References/1.11 ref').glob('*.PNG')),
 '06-onayli':[ROOT/'Assets/Art/Menu/Task21/onizleme.png',ROOT/'Assets/Art/Menu/Task20/onizleme.png',ROOT/'Assets/Art/Menu/Task19D/onizleme.png'],
 '07-ek':[ROOT/'docs/qa/2026-10-26-p19/375x667_04_pause.png',ROOT/'docs/qa/2026-11-01-corridor/375x667_lv1_c_son.png',ROOT/'docs/qa/2026-11-01-corridor/375x667_lv40_kart.png',ROOT/'docs/qa/2026-11-02-rating/c_kayip_kart.png']}
manifest={}
for group,paths in groups.items():
 paths=sorted(p for p in paths if p.exists());manifest[group]=[p.relative_to(ROOT).as_posix() for p in paths]
 cols=6; cw=210;ch=405
 for page in range((len(paths)+17)//18):
  subset=paths[page*18:(page+1)*18];sheet=Image.new('RGB',(cols*cw,((len(subset)+cols-1)//cols)*ch),'#273F49');d=ImageDraw.Draw(sheet)
  for i,p in enumerate(subset):
   a=Image.open(p).convert('RGB');a.thumbnail((200,365));x=i%cols*cw;y=i//cols*ch;sheet.paste(a,(x+(cw-a.width)//2,y));d.text((x+3,y+369),p.name[:28],font=font,fill='white')
  sheet.save(WORK/(group+f'-{page+1}.jpg'),quality=90)
(WORK/'kaynak-ekranlar.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
print('Kaynaklar', {k:len(v) for k,v in manifest.items()})
