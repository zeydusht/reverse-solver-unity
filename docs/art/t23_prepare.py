from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
from datetime import datetime
import hashlib,json,os
R=Path(__file__).resolve().parents[2];W=R/'docs/art/task23';O=R/'Assets/Art/Redesign_T23'
for p in [W,O,O/'DENEMELER',O/'NIHAI',O/'KAYNAKLAR']:p.mkdir(exist_ok=True)
def log(t):
 with (R/'docs/art/GECE_LOG.md').open('a',encoding='utf-8') as f:f.write('\n## '+datetime.now().astimezone().isoformat(timespec='minutes')+' — Task23\n'+t+'\n')
if not (W/'koruma-baslangic.json').exists():
 records=[];errors=[]
 for base in ['Assets/Art','Assets/_Game','Assets/WebGLTemplates','ProjectSettings']:
  for dp,dn,fn in os.walk(R/base,onerror=lambda e:errors.append(str(e))):
   dn[:]=[n for n in dn if n not in {'Redesign_T23','.deps'}]
   for n in fn:
    p=Path(dp)/n
    try:records.append({'path':p.relative_to(R).as_posix(),'size':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
    except OSError as e:errors.append(str(e))
 (W/'koruma-baslangic.json').write_text(json.dumps({'time':datetime.now().astimezone().isoformat(),'files':records,'errors':errors},ensure_ascii=False,indent=2),encoding='utf-8')
 log('- Task23 ve stil/palet rehberi okundu; çalışma yalnız Redesign_T23 ve docs/art içinde. Hiçbir dosya silinmeyecek/taşınmayacak, eğitim eli ve kod değişmeyecek.\n- karar: En az iki anlamlı deneme her aile için DENEMELER/A ve B içinde saklanacak; NIHAI ve ana inceleme sayfası yalnız seçilen sürümü gösterecek.\n- karar: Kullanıcının01:20 merdiven eki öncelikli. İlk okumalarda ek başlığı dosyada henüz görünmedi; düğüm çalışması önce başlatıldı, dosya tekrar okunacak. Güncel düğüm altın, biten turkuaz, kilitli koyu kalacak; kilit büyütülecek.')
font=ImageFont.truetype(str(R/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),19)
groups={
'baron-sahneleri':[p for d in ['OptionA','OptionB'] for p in (R/'Assets/Art/ObstacleIntros'/d).glob('*.png')],
'onayli-govdeler':[R/'Assets/Art/Menu/Task20/jiggle-blue-standing.png',R/'Assets/Art/Menu/Task20/baron-head.png',R/'Assets/Art/HardWarning/option-a-baron-warning-888x592.png'],
'cilalama':[R/'Assets/Art/Gameplay/Task15/hud-moves-pill-v2.png',R/'Assets/Art/Gameplay/Task15/hud-time-pill-v2.png',R/'Assets/Art/Gameplay/Task14/hud-level-badge.png',R/'Assets/Art/Gameplay/Task14/hud-pause-normal.png']+[R/f'Assets/Art/Gameplay/Task14/booster-{k}-normal.png' for k in ['scissors','wand','hammer','clock']],
't22-kabul':[R/'Assets/Art/Redesign_T22/101-ai-kazanma/once-sonra.png',R/'Assets/Art/Redesign_T22/15-yukleme/ekran-sonra.png',R/'Assets/Art/Redesign_T22/22-jiggles/once-sonra.png']}
for key,paths in groups.items():
 paths=[p for p in paths if p.exists()];cols=4;cw=330;ch=300;sheet=Image.new('RGB',(cols*cw,((len(paths)+cols-1)//cols)*ch),'#31505D');d=ImageDraw.Draw(sheet)
 for i,p in enumerate(paths):
  a=Image.open(p).convert('RGBA');a.thumbnail((310,250));x=i%cols*cw;y=i//cols*ch;sheet.paste(a,(x+(cw-a.width)//2,y),a);d.text((x+5,y+255),p.parent.name+'/'+p.name[:20],font=font,fill='white')
 sheet.save(W/(key+'.jpg'),quality=93)
print('baseline and reference contacts ready')
