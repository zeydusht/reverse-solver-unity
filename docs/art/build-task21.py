"""Task21 art-only export and measured three-screen assembly."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter,ImageChops
import ast,json,hashlib,shutil,math
import numpy as np
ROOT=Path(__file__).resolve().parents[2];ART=ROOT/'Assets/Art';OLD=ART/'Menu/Task20';OUT=ART/'Menu/Task21';WORK=ROOT/'docs/art/task21';SRC=WORK/'sources';PREV=OUT/'Preview'
for p in (OUT,SRC,PREV):p.mkdir(parents=True,exist_ok=True)
R=Image.Resampling.LANCZOS;INK='#0B2A36';CREAM='#FFF6D8';SIZE=(1125,2436)
RECORDS=[];CACHE={};MARKS={}
# Reuse only export primitives, never execute Task20's build.
tree=ast.parse((ROOT/'docs/art/export-menu-task20.py').read_text(encoding='utf8'))
exec(compile(ast.Module(body=[x for x in tree.body if isinstance(x,ast.FunctionDef) and x.name in ('clean','fit','pressed','save','s9','fit9')],type_ignores=[]),'<export-primitives>','exec'))
def clean(im,component=True):
 im=im.copy();im.putalpha(im.getchannel('A').point(lambda v:v if v>=32 else 0));return im
def s9(a,sz,b):
 if not any(b):return a.resize(sz,R)
 l,t,r,bot=b;w,h=a.size;tw,th=sz
 assert tw>=l+r and th>=t+bot,(sz,b)
 xs=[0,l,w-r,w];ys=[0,t,h-bot,h];dx=[0,l,tw-r,tw];dy=[0,t,th-bot,th];o=Image.new('RGBA',sz)
 for j in range(3):
  for i in range(3):
   if xs[i+1]<=xs[i] or ys[j+1]<=ys[j] or dx[i+1]<=dx[i] or dy[j+1]<=dy[j]:continue
   p=a.crop((xs[i],ys[j],xs[i+1],ys[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),R);o.alpha_composite(p,(dx[i],dy[j]))
 return o
mapping=json.loads((WORK/'sources.json').read_text(encoding='utf8'))
for k,p in mapping.items():shutil.copy2(p,SRC/(k+'.png'))
def source(n):return Image.open(SRC/(n+'.png')).convert('RGBA')
def old(n):return Image.open(OLD/(n+'.png')).convert('RGBA')
def region(a,b):return a.crop(tuple(round(v*(a.width if i%2==0 else a.height)) for i,v in enumerate(b)))
def put(n,a,use,b=(0,0,0,0),**kw):return save(a,n,use,b,**kw)
for i,n in enumerate(('home','collection','jiggles','trophy')):
 a=source('icon'+str(i))
 for state,h in [('normal',147),('selected-large',184)]:
  tight=clean(a);tight=tight.crop(tight.getbbox());w=round(h*tight.width/tight.height)
  put('tab-icon-'+n+'-'+state,fit(tight,(w,h),0),n+' sekme ikonu; '+state,state=state)
bg=Image.new('RGBA',(1125,258),'#10707F');d=ImageDraw.Draw(bg)
for y in range(10):
 t=y/9;c=tuple(round(a*(1-t)+b*t) for a,b in zip((243,138,3),(251,186,14)))
 d.line((0,y,1124,y),fill=(*c,255))
put('tabbar-bg',bg,'Tam ekran alt çubuk, alt güvenli alan dahil',(1,10,1,1),category='flat-ui')
put('tab-slot',Image.new('RGBA',(225,248),'#10707F'),'Normal yuva; 75pt',(1,1,1,1),category='flat-ui')
put('tab-slot-selected',Image.new('RGBA',(450,248),'#1A9AAE'),'Seçili yuva; 150pt',(1,1,1,1),category='flat-ui')
put('tab-divider',Image.new('RGBA',(3,248),'#0B4A55'),'Yuva ayırıcı',(0,1,0,1),category='flat-ui')
put('tab-badge',old('notification-badge'),'Boş kırmızı bildirim rozeti')
a=source('profile')
fr=fit(region(a,(0,0,1,.51)),(270,270),0,stretch=True)
put('profile-square-frame',fr,'90pt kare profil çerçevesi',innerRectPx=[48,48,174,174])
put('profile-name-plate',fit9(region(a,(0,.52,1,.73)),(360,126),(45,40,45,40)),'Ayrı koyu isim plaketi',(45,40,45,40))
put('counter-pill-v2',fit9(region(a,(0,.75,1,1)),(420,120),(54,39,54,39)),'Krem sayaç; soldaki ikon ayrı',(54,39,54,39))
a=source('rings')
for i,n in enumerate(('avatar-ring','avatar-ring-selected','check-badge')):
 sz=(48,48) if i==2 else (180,180)
 b=fit(region(a,[(0,0,.36,1),(.40,0,.76,1),(.78,.1,1,.9)][i]),sz,0,stretch=True)
 if i<2:
  al=b.getchannel('A');ImageDraw.Draw(al).ellipse((24,24,155,155),fill=0);b.putalpha(al)
 put(n,b,'Avatar seçimi '+n,innerDiameterPx=132 if i<2 else None)
mask=Image.new('RGBA',(180,180));ImageDraw.Draw(mask).ellipse((24,24,155,155),fill='white');put('avatar-ring-mask',mask,'132px iç çap daire alfa maskesi')
for c in ('blue','pink','red','green','purple'):
 a=clean(old('avatar-'+c));a=a.crop(a.getbbox());a=ImageOps.contain(a,(103,103),R)
 b=Image.new('RGBA',(180,180));b.alpha_composite(a,((180-a.width)//2,(180-a.height)//2));b.putalpha(ImageChops.multiply(b.getchannel('A'),mask.getchannel('A')))
 put('avatar-'+c+'-round',b,'İç çapın %78 boyunda maskeli avatar',contentDiameterPx=103,ringInnerDiameterPx=132)
for n in ('frame','badge','portrait'):
 a=fit(old('milestone-'+n),(156,156),0)
 put('milestone-medal-'+n,a,'Bağımsız ulaşılmış madalyon',state='normal')
 ar=np.asarray(a).copy();gray=(ar[:,:,:3]*[.299,.587,.114]).sum(2)
 ar[:,:,:3]=ar[:,:,:3]*.22+gray[:,:,None]*.55+np.array([243,235,210])*.23
 b=Image.fromarray(ar);lock=fit(old('icon-lock'),(51,60),0);b.alpha_composite(lock,(103,94))
 put('milestone-medal-'+n+'-locked',b,'Soluk ve kilitli bağımsız madalyon',state='locked')
dots=Image.new('RGBA',(210,18));d=ImageDraw.Draw(dots)
for x in range(6,208,30):d.ellipse((x,5,x+8,13),fill='#FFF6D8')
put('milestone-dots',dots,'Kısa noktalı bağlantı; yatay ray yok')
a=source('settings')
spec=[('settings-panel',(35/1024,40/1536,480/1024,630/1536),(987,1500),(45,45,45,45)),('settings-row',(500/1024,220/1536,1010/1024,420/1536),(897,195),(38,38,38,38)),('settings-icon-btn',(80/1024,690/1536,450/1024,1070/1536),(162,162),(35,35,35,35)),('icon-privacy',(600/1024,700/1536,940/1024,1075/1536),(102,114),(0,0,0,0)),('icon-mail',(50/1024,1150/1536,490/1024,1480/1536),(114,96),(0,0,0,0)),('settings-icon-disc',(580/1024,1110/1536,980/1024,1500/1536),(132,132),(0,0,0,0))]
for n,b,sz,sl in spec:
 raw=region(a,b);im=fit9(raw,sz,sl) if any(sl) else fit(raw,sz,0)
 put(n,im,'Ayarlar '+n,sl,interactive=n=='settings-icon-btn')
# New standalone cream pictograms; no rectangle or previous button rim.
for i,n in enumerate(('sound','music','vibration')):
 im=region(source('symbols'),(i/3,0,(i+1)/3,1))
 put('settings-symbol-'+n,fit(im,(72,72),1,component=False),'Koyu daire üstünde bağımsız '+n+' sembolü')
for row in RECORDS:row['isRuntime']=True
NEW={r['file'][:-4]:r for r in RECORDS};META={r['file'][:-4]:r for r in json.loads((OLD/'manifest.json').read_text(encoding='utf8')) if r.get('isRuntime')};ALL={};DEPEND={}
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'
def txt(t,x,y,size=18,color=INK,outline=0,shadow=False,anchor='mm',maxw=None,stroke=INK):
 f=ImageFont.truetype(str(FONT),round(size*3));dd=ImageDraw.Draw(TEXT)
 if maxw:
  while f.getlength(t)>maxw*3:size-=.25;f=ImageFont.truetype(str(FONT),round(size*3))
 sw=round(outline*3)
 if shadow:dd.text((x*3,y*3+round(size*3*.08)),t,font=f,anchor=anchor,fill=stroke,stroke_width=sw,stroke_fill=stroke)
 dd.text((x*3,y*3),t,font=f,anchor=anchor,fill=color,stroke_width=sw,stroke_fill=stroke)
 texts.append(dict(text=t,x=x,y=y,sizePt=size,outlinePt=outline,shadow=shadow,anchor=anchor))
def pic(n):return CACHE[n] if n in CACHE else old(n)
def add(n,b):
 x,y,w,h=b;meta=NEW.get(n,META.get(n));a=s9(pic(n),(round(w*3),round(h*3)),meta['sliceLTRB']);IM.alpha_composite(a,(round(x*3),round(y*3)))
 layers.append(dict(sprite=('Task21/' if n in NEW else 'Task20/')+n+'.png',rectPt=list(b)))
 if n not in NEW:DEPEND[n+'.png']=meta['sha256']
def start(n,bg):
 global IM,TEXT,texts,layers,NAME
 NAME=n;IM=Image.new('RGBA',SIZE);TEXT=Image.new('RGBA',SIZE);texts=[];layers=[];add(bg,(0,0,375,812))
def nav(active,base=726):
 add('tabbar-bg',(0,base,375,86));x=0
 for i,(n,label) in enumerate(zip(('home','collection','jiggles','trophy'),('Ev','Koleksiyon','Jiggles','Başarılar'))):
  sel=i==active;w=150 if sel else 75;cx=x+w/2
  add('tab-slot-selected' if sel else 'tab-slot',(x,base+10/3,w,86-10/3))
  if x:add('tab-divider',(x-.5,base+10/3,1,86-10/3))
  key='tab-icon-'+n+('-selected-large' if sel else '-normal');h=184/3 if sel else 49;iw=h*pic(key).width/pic(key).height
  add(key,(cx-iw/2,base-33 if sel else base+3,iw,h))
  if sel:txt(label,cx,base+39,17,CREAM,outline=.95,shadow=True,maxw=w-10)
  x+=w
def done():
 im=Image.alpha_composite(IM,TEXT);im.convert('RGB').save(PREV/(NAME+'.png'));ALL[NAME]=dict(layers=layers,text=texts);return im
start('01-ev','scene-home')
# Real game painting, never reference pixels.
a=Image.open(ART/'Gameplay/Task15/painting-test-01.png').convert('RGBA');a=ImageOps.fit(a,(705,1134),R);a.putalpha(old('hero-picture-mask').getchannel('A'));IM.alpha_composite(a,(186,435))
layers.append(dict(gameArt='Assets/Art/Gameplay/Task15/painting-test-01.png',rectPt=[62,145,235,378],mask='Task20/hero-picture-mask.png'));DEPEND['hero-picture-mask.png']=META['hero-picture-mask']['sha256']
add('hero-frame',(62,145,235,378));add('avatar-disc',(26.5,62.5,53,53));add('avatar-blue',(29,65,48,48));add('profile-square-frame',(8,44,90,90))
add('profile-name-plate',(101,68,96,42));txt('Zeynep Yılmaz',149,89,12,CREAM,maxw=80)
add('counter-pill-v2',(210,74,102,30));add('jiggle-counter-icon',(199,66,46,46));txt('0',275,89,23)
add('button-settings',(327,66,46,46));add('button-primary',(84.375,613,206.25,90));txt('Level 1',187.5,658,43,CREAM,outline=2.4,shadow=True,stroke='#1E4D05');add('button-levels',(19,636,44,44));nav(0);done()
start('03-jiggles','scene-jiggles');txt('Jiggles',187.5,75,32,CREAM,outline=1.8,shadow=True)
add('counter-pill-v2',(96,108,210,44));add('jiggle-counter-icon',(68,95,58,58));txt('1.248',207,130,30);txt('Jiggle kurtarıldı',187.5,164,18,CREAM,outline=.8)
add('jiggles-shelf',(20,178,340,300))
for c,cx,feet in [('green',135,306),('purple',246,306),('blue',92,446),('pink',188,446),('red',285,446)]:add('jiggle-'+c+'-standing',(cx-44,feet-132,88,132))
for cx,y,count in [(135,318,'211'),(246,318,'190'),(92,461,'328'),(188,461,'277'),(285,461,'242')]:add('color-count-pill',(cx-24,y-10,48,20));txt(count,cx,y,16)
txt('Avatarım',187.5,497,19,CREAM,outline=1,shadow=True)
for i,c in enumerate(('blue','pink','red','green','purple')):
 x=13.5+i*72;add('avatar-disc',(x+8,531,44,44));add('avatar-'+c+'-round',(x,523,60,60));add('avatar-ring-selected' if i==0 else 'avatar-ring',(x,523,60,60))
 if i==0:add('check-badge',(x+43,566,16,16))
for x in (93,213):add('milestone-dots',(x,612,70,6))
for n,cx,count,label,locked in [('frame',67,'500','Çerçeve',False),('badge',187,'1.000','Rozet',False),('portrait',307,'2.000','Portre',True)]:
 add('milestone-medal-'+n+('-locked' if locked else ''),(cx-24,591,48,48));txt(count,cx,653,18,CREAM,outline=.8);txt(label,cx,675,14,CREAM,outline=.6)
nav(2);done()
start('06-ayarlar','scene-achievements');txt('Ayarlar',187.5,155,34,CREAM,outline=1.9,shadow=True);add('settings-panel',(23,187,329,500));add('button-close',(315,173,34,34))
for n,y,on,label in [('sound',253,True,'Ses'),('music',333,True,'Müzik'),('vibration',410,False,'Titreşim')]:
 add('settings-row',(38,y-32.5,299,65));add('settings-icon-disc',(43,y-22,44,44));add('settings-symbol-'+n,(53,y-12,24,24));txt(label,101,y,20,anchor='lm')
 add('toggle-on-track' if on else 'toggle-off-track',(266,y-14,60,28));add('toggle-knob',(299 if on else 269,y-12,24,24))
txt('Dil',73,497,20);add('language-left-selected',(123,475,198,44));txt('Türkçe',174,497,16,CREAM,outline=.65);txt('English',271,497,16,CREAM,outline=.65)
for n,x,label in [('privacy',86,'Gizlilik'),('mail',235,'Bize yaz')]:add('settings-icon-btn',(x,546,54,54));add('icon-'+n,(x+11,557,32,32));txt(label,x+27,617,15)
txt('Sürüm 1.0.0',187.5,655,12,'#60767A');nav(0);done()
# All four width-shifting states, no inactive labels.
start('04-sekme-durumlari','scene-home');IM=Image.new('RGBA',SIZE,'#294C55');layers=[]
for i in range(4):nav(i,165+i*155)
done()
board=Image.new('RGB',(1125,812),INK)
for i,n in enumerate(('01-ev','03-jiggles','06-ayarlar')):board.paste(Image.open(PREV/(n+'.png')).resize((375,812),R),(i*375,0))
board.save(OUT/'onizleme.png')
(WORK/'preview-layout.json').write_text(json.dumps(ALL,ensure_ascii=False,indent=2),encoding='utf8');(WORK/'dependencies.json').write_text(json.dumps(DEPEND,indent=2),encoding='utf8')
for p in [OUT/'onizleme.png',*PREV.glob('*.png')]:
 a=Image.open(p);RECORDS.append(dict(file=p.relative_to(OUT).as_posix(),use='İnceleme önizlemesi; runtime değil',width=a.width,height=a.height,sliceLTRB=[0,0,0,0],pivot=[.5,.5],scale=1,transparent=False,isRuntime=False,category='preview',state='reference',sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(OUT/'manifest.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(dict(runtime=sum(r['isRuntime'] for r in RECORDS),preview=sum(not r['isRuntime'] for r in RECORDS))))

