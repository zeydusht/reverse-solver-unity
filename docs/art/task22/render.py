"""Task22 art-only native UI redesign. No Unity, import, or integration operations.
Approved painted bitmap materials are retained; new UI geometry/states are local.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter,ImageChops,ImageEnhance
import json,math,hashlib,html,datetime,colorsys
R=Path(__file__).resolve().parents[3];W=R/'docs/art/task22';O=R/'Assets/Art/Redesign_T22'
J=json.loads((W/'jobs.json').read_text(encoding='utf8'))
INK='#0B2A36';TEAL='#1C7F8C';DARK='#0F4F5A';CREAM='#F8EBD0';WHITE='#FFF9F1'
FONT=R/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'
RES=Image.Resampling.LANCZOS;CACHE={};RECORDS=[];TEXTS=[];PLACEMENTS=[]
def px(v):return round(v*3)
def canvas(w,h):return Image.new('RGBA',(px(w),px(h)))
def font(s):return ImageFont.truetype(str(FONT),px(s))
def source(p):
 if p not in CACHE:
  with Image.open(R/p) as a:CACHE[p]=a.convert('RGBA')
 return CACHE[p].copy()
def fit(a,w,h):return ImageOps.contain(a,(px(w),px(h)),RES)
def put(im,a,x,y,w=None,h=None):
 if w is not None:a=ImageOps.contain(a,(px(w),px(h)),RES)
 im.alpha_composite(a,(px(x),px(y)))
def center(im,a,x,y,w,h):
 a=fit(a,w,h);im.alpha_composite(a,(px(x)+(px(w)-a.width)//2,px(y)+(px(h)-a.height)//2))
def s9(a,w,h,b=(90,90,90,90)):
 tw,th=px(w),px(h);l,t,r,bottom=b
 if tw<l+r or th<t+bottom:return a.resize((tw,th),RES)
 xs=[0,l,a.width-r,a.width];ys=[0,t,a.height-bottom,a.height]
 dx=[0,l,tw-r,tw];dy=[0,t,th-bottom,th];im=Image.new('RGBA',(tw,th))
 for yy in range(3):
  for xx in range(3):
   part=a.crop((xs[xx],ys[yy],xs[xx+1],ys[yy+1])).resize((dx[xx+1]-dx[xx],dy[yy+1]-dy[yy]),RES)
   im.alpha_composite(part,(dx[xx],dy[yy]))
 return im
def grad_round(im,box,top,bottom,radius=12,outline=None,width=1):
 b=tuple(px(v) for v in box);ww=b[2]-b[0];hh=b[3]-b[1]
 if ww<=0 or hh<=0:return
 mask=Image.new('L',(ww,hh));ImageDraw.Draw(mask).rounded_rectangle((0,0,ww-1,hh-1),radius=px(radius),fill=255)
 g=ImageOps.colorize(Image.linear_gradient('L').resize((ww,hh)),top,bottom).convert('RGBA');g.putalpha(mask);im.alpha_composite(g,(b[0],b[1]))
 if outline:ImageDraw.Draw(im).rounded_rectangle(b,radius=px(radius),outline=outline,width=px(width))
def txt(im,t,x,y,size=16,color=INK,outline=False,width=None,anchor='mm',record=True,stroke=INK):
 f=font(size)
 if width:
  while f.getlength(t)>px(width) and size>13:size-=.25;f=font(size)
  assert f.getlength(t)<=px(width)+1,(t,size,width)
 d=ImageDraw.Draw(im);sw=max(1,px(size*.045)) if outline else 0
 if outline:d.text((px(x),px(y+size*.075)),t,font=f,anchor=anchor,fill=stroke,stroke_width=sw,stroke_fill=stroke)
 d.text((px(x),px(y)),t,font=f,anchor=anchor,fill=color,stroke_width=sw,stroke_fill=stroke)
 if record:TEXTS.append(dict(text=t,xPt=x,yPt=y,sizePt=size,color=color,outline=outline,anchor=anchor))
def lines(im,t,x,y,width=270,size=16,spacing=23,color=INK):
 words=t.split();rows=[];row=''
 for word in words:
  test=(row+' '+word).strip()
  if font(size).getlength(test)>px(width) and row:rows.append(row);row=word
  else:row=test
 if row:rows.append(row)
 for k,row in enumerate(rows):txt(im,row,x,y+k*spacing,size,color)
 return len(rows)*spacing
def save_asset(folder,name,im,use,b=(0,0,0,0),state='normal',minhit=None,origin=None):
 assert im.mode=='RGBA'
 file=folder/(name+'.png');im.save(file)
 rec=dict(file=file.relative_to(O).as_posix(),use=use,width=im.width,height=im.height,scale=3,
   sizePt=[im.width/3,im.height/3],sliceLTRB=list(b),pivot=[.5,.5],state=state,
   transparent=im.getchannel('A').getextrema()[0]<255,sha256=hashlib.sha256(file.read_bytes()).hexdigest(),runtimeCandidate=True)
 if minhit:rec['minHitPt']=minhit
 if origin:rec['origin']=origin
 RECORDS.append(rec);return im
def pressed(a):
 b=ImageEnhance.Brightness(a).enhance(.92);b.putalpha(a.getchannel('A'));o=Image.new('RGBA',a.size)
 o.alpha_composite(b.resize((a.width,a.height-9),RES),(0,9));return o
def glyph(im,t,x,y,s=18,color=WHITE):txt(im,t,x,y,s,color,False,record=False)
def chip(w=48,h=44,state='normal'):
 im=canvas(w,h);sel=state=='selected'
 grad_round(im,(1,2,w-1,h-1),INK,DARK,10)
 grad_round(im,(2,1,w-2,h-4),TEAL,DARK,9)
 grad_round(im,(4,2,w-4,h-7),'#FBBA0E' if sel else WHITE,'#F38A03' if sel else CREAM,7)
 if sel:grad_round(im,(6,4,w-6,h-9),TEAL,DARK,6)
 ImageDraw.Draw(im).line((px(11),px(4),px(w-11),px(4)),fill=WHITE,width=px(.7))
 return pressed(im) if state=='pressed' else im
def ui_button(primary=True,w=190,h=94):
 # Approved raster paint is retained; original ratio, no flattening of its bevel.
 p='Assets/Art/Gameplay/Task15/btn-primary-v2.png' if primary else 'Assets/Art/Gameplay/Task15/btn-secondary-v2.png'
 a=source(p)
 if primary or w<175:
  # Preserve the approved tall 684:336 silhouette. Only the transparent gutter varies.
  out=canvas(w,h);center(out,a,0,0,w,h);return out
 return s9(source('Assets/Art/Menu/Task20/button-secondary.png'),w,h,(75,60,75,60))
def button(im,label,x,y,w=248,h=70,primary=True):
 put(im,ui_button(primary,w,h),x,y);txt(im,label,x+w/2,y+h/2-2,19 if w>170 else 15,WHITE,True,width=w-20,stroke='#1E6305' if primary else INK)
 PLACEMENTS.append(dict(role='primary' if primary else 'secondary',rectPt=[x,y,w,h],minHitPt=[w,max(44,h*.8)]))
def shell(w,h):
 return s9(source('Assets/Art/Menu/Task20/modal-panel.png'),w,h,(120,150,120,150))
def header(im,title,w):
 # A restrained teal headband replaces gold scrolls; cream type is a separate layer in production.
 put(im,s9(source('Assets/Art/Menu/Task21/profile-name-plate.png'),w-32,60,(60,45,60,45)),16,14)
 txt(im,title,w/2,43,24,WHITE,True,width=w-47)
def icon(n):
 paths={
  'nail':'Assets/Art/Gameplay/Task14/nail-head-384.png','bomb':'Assets/Art/Gameplay/Task14/bomb-384.png',
  'scissors':'Assets/Art/Gameplay/Task14/booster-scissors.png','hammer':'Assets/Art/Gameplay/Task14/booster-hammer.png',
  'clock':'Assets/Art/Gameplay/Task14/booster-clock.png','wand':'Assets/Art/Gameplay/Task14/booster-wand.png',
  'jiggle':'Assets/Art/Gameplay/Task15/avatar-blue.png','lock':'Assets/Art/Menu/Task20/icon-lock.png',
  'time':'Assets/Art/Gameplay/Task14/loss-reason-time.png','lossbomb':'Assets/Art/Gameplay/Task14/loss-reason-bomb.png'}
 return source(paths[n])
def hero_plate(n,w=200,h=140):
 im=canvas(w,h)
 grad_round(im,(8,8,w-8,h-8),'#FFF9F1',CREAM,26)
 if n=='chain':
  a=source('Assets/Art/Gameplay/Task14/chain-link-repeat-96x48.png');end=source('Assets/Art/Gameplay/Task14/chain-endpoint-144.png')
  for x in range(55,145,18):center(im,a,x,58,28,22)
  center(im,end,17,43,64,64);center(im,end,119,43,64,64)
 elif n=='wall':
  a=source('Assets/Art/Gameplay/Task15/wall-repeat-v3.png')
  a=a.resize((px(164),px(25)),RES);put(im,a,18,58)
 elif n=='hard':
  center(im,icon('jiggle'),58,18,84,100)
  b=chip(42,44,'selected');put(im,b,133,12);glyph(im,'!',154,32,25)
 else:center(im,icon(n),39,12,122,116)
 return im
def card_components(folder,w,h,n):
 blank=shell(w,h);save_asset(folder,'panel',blank,'Krem/turkuaz ortak panel; başlık/metin ayrı',(120,150,120,150))
 band=canvas(w,76);header(band,'',w);save_asset(folder,'baslik-zemini',band,'Boş turkuaz başlık',(70,38,70,38))
 for key,pri in [('ana-tus',True),('ikincil-tus',False)]:
  a=ui_button(pri,190,94) if pri else ui_button(False,248,60)
  sl=(0,0,0,0) if pri else (75,60,75,60)
  save_asset(folder,key,a,'Onaylı boyalı tuş; ana eylemde özgün en/boy korunur',sl,minhit=[190,75] if pri else [248,48],origin='Assets/Art/Gameplay/Task15/btn-primary-v2.png' if pri else 'Assets/Art/Menu/Task20/button-secondary.png')
  save_asset(folder,key+'-basili',pressed(a),'Basılı durum',sl,state='pressed',minhit=[190,75] if pri else [248,48])
 if n:
  a=hero_plate(n);save_asset(folder,'nesne-alani',a,'Onaylı nesne + krem yumuşak sahne; metin yok')
def compose_card(j):
 n=j['id'];w=322
 if n==1:h=560;title='Tahta boşaldı!';hero=None
 elif n in (3,4):h=580 if n==3 else 480;title='Bomba patladı' if n==3 else 'Süre doldu';hero='bomb' if n==3 else 'time'
 elif n==5:h=410;title='Durduruldu';hero=None
 elif 6<=n<=14:
  h=470;title=['Çivili parça','Bombalı parça','Zincirli parçalar','Kapalı kenar','Makas','Çekiç','Saat','Değnek','Zor bölüm'][n-6]
  hero=['nail','bomb','chain','wall','scissors','hammer','clock','wand','hard'][n-6]
 elif n==16:h=380;title='Hoş geldin!';hero='jiggle'
 elif n==17:h=390;title='Tek bir soru';hero=None
 card_components(CURRENT,w,h,hero);im=shell(w,h);header(im,title,w)
 if n==1:
  for k in range(3):center(im,source('Assets/Art/Gameplay/Task14/star-full.png'),103+k*39,86,32,32)
  txt(im,'109 saniye kaldı',w/2,139,17);txt(im,'0 kez takıldın',w/2,161,14,DARK)
  button(im,'Sonraki bölüm',66,177,190,94)
  txt(im,'Ne kadar zordu?',w/2,293,16)
  for k in range(10):
   x=27+(k%5)*55;y=315+(k//5)*50;put(im,chip(48,44),x,y);txt(im,str(k+1),x+24,y+20,16)
  button(im,'Tekrar oyna',25,432,132,65,False);button(im,'Menü',165,432,132,65,False)
  txt(im,'Geç',w/2,520,15,DARK)
 elif n in (3,4):
  center(im,hero_plate(hero),81,77,160,99)
  if n==3:txt(im,'0',w/2,132,23)
  body='Fitil bitmeden bombanın yolunu aç.' if n==3 else 'Süre dolmadan tüm parçaları dışarı çıkar.'
  lines(im,body,w/2,191,260,16,23)
  button(im,'Tekrar dene',66,230,190,94)
  if n==3:
   txt(im,'Ne kadar zordu?',w/2,344,16)
   for k in range(10):
    x=27+(k%5)*55;y=365+(k//5)*50;put(im,chip(48,44),x,y);txt(im,str(k+1),x+24,y+20,16)
   button(im,'Menü',66,463,190,57,False)
   txt(im,'Geç',w/2,545,14,DARK)
  else:button(im,'Menü',37,365,248,60,False)
 elif n==5:
  txt(im,'Hazır olduğunda devam et.',w/2,105,16,DARK)
  button(im,'Devam',66,132,190,94)
  button(im,'Yeniden başla',37,242,248,60,False)
  button(im,'Ana menü',37,315,248,60,False)
 elif 6<=n<=14:
  center(im,hero_plate(hero),61,84,200,140)
  if n in (6,7):txt(im,'5' if n==6 else '23',161,156,27)
  bodies={6:'Diğer parçaları çıkar. Çivinin sayacı 0 olunca bu parça serbest kalır.',7:'Fitil bitmeden bombanın yolunu aç ve bombalı parçayı dışarı çıkar.',8:'Zincirli iki parça birlikte çıkar. İkisinin de yolunu aynı yöne aç.',9:'Parçalar kırmızı-beyaz kapalı kenardan geçemez. Açık bir yol bul.',10:'Makası seç. Kesmek istediğin birleşim noktasına dokun.',11:'Çekici seç. Kaldırmak istediğin parçaya dokun.',12:'Saat, geri sayıma 20 saniye ekler.',13:'Değnek çıkıntıları yeniden dizer. Resim değişmez.',14:'Bu bölüm daha zor. Önce açık yolları bul, sonra hamleni yap.'}
  lines(im,bodies[n],w/2,249,264,16,23)
  tips={6:'Her hamlede sayaç azalır.',7:'Önce bombanın yolunu aç.',8:'Zinciri izleyerek eşini bul.',9:'Duvarı aşmak yerine yolunu değiştir.',10:'Kesilen birleşim artık takılmaz.',11:'Bomba ve çivili parçayı da kaldırır.',12:'+20 saniye',13:'Yeni çıkış yolları ara.',14:'Süre ilk hamlede başlar.'}
  lines(im,tips[n],w/2,335,270,14,20,DARK)
  button(im,'Başla' if n==14 else 'Anladım',66,360,190,94)
 elif n==16:
  center(im,icon('jiggle'),128,80,66,70)
  txt(im,'Sana nasıl seslenelim?',w/2,172,17)
  field=canvas(270,56);grad_round(field,(1,3,269,55),DARK,TEAL,12);grad_round(field,(3,5,267,52),WHITE,CREAM,10)
  save_asset(CURRENT,'isim-alani',field,'Boş giriş alanı',(30,24,30,24),minhit=[270,56]);put(im,field,26,196);txt(im,'Adın',52,225,16,DARK,anchor='lm')
  button(im,'Başla',66,265,190,94)
 elif n==17:
  lines(im,'Bu tür bulmaca oyunlarını oynar mısın?',w/2,101,270,17,24)
  for k,t in enumerate(['Hiç','Ara sıra','Sık']):button(im,t,37,157+k*67,248,59,False)
 return im
def build_chip(j):
 im=canvas(286,130)
 for st in ('normal','selected','pressed'):
  save_asset(CURRENT,'kutu-'+st,chip(50,44,st),'Sayı ayrı; zorluk hedefi 50×44 pt',(21,21,21,21),st,[50,44])
 for i in range(10):
  x=6+(i%5)*55;y=34+(i//5)*50;st='selected' if i==6 else 'normal'
  put(im,chip(50,44,st),x,y);txt(im,str(i+1),x+25,y+20,17,WHITE if st=='selected' else INK)
 txt(im,'1 çok kolay · 10 çok zor',143,12,14)
 return im
def switch(on=True):
 im=s9(source('Assets/Art/Menu/Task20/toggle-'+('on' if on else 'off')+'-track.png'),76,44,(45,36,45,36))
 # Distinct marks supplement both hue and thumb position.
 x=39 if on else 3
 center(im,source('Assets/Art/Menu/Task20/toggle-knob.png'),x,4,34,34)
 d=ImageDraw.Draw(im)
 if on:d.line([(px(14),px(21)),(px(19),px(26)),(px(27),px(15))],fill=WHITE,width=px(2.1),joint='curve')
 else:
  d.line((px(51),px(16),px(60),px(25)),fill=WHITE,width=px(2));d.line((px(51),px(25),px(60),px(16)),fill=WHITE,width=px(2))
 return im
def language(selected='tr'):
 im=s9(source('Assets/Art/Menu/Task20/language-'+('left' if selected=='tr' else 'right')+'-selected.png'),214,50,(55,45,55,45))
 d=ImageDraw.Draw(im);cx=16 if selected=='tr' else 198
 d.line([(px(cx-4),px(24)),(px(cx-1),px(28)),(px(cx+5),px(20))],fill=WHITE,width=px(1.6),joint='curve')
 return im
def node(state):
 im=canvas(66,66);d=ImageDraw.Draw(im)
 for b,fill in [((2,4,64,65),INK),((2,2,64,62),DARK),((4,2,62,59),TEAL),((7,5,59,56),WHITE),((9,7,57,54),DARK if state=='locked' else '#1E6305' if state=='current' else TEAL)]:
  d.ellipse(tuple(px(v) for v in b),fill=fill)
 d.arc((px(11),px(8),px(55),px(49)),200,300,fill=WHITE,width=px(1.3))
 if state=='current':d.ellipse((px(4),px(2),px(62),px(60)),outline='#FBBA0E',width=px(1.2))
 if state=='locked':center(im,icon('lock'),23,18,20,26)
 return im
def play_bubble():
 im=canvas(90,50);a=ui_button(True,90,44);put(im,a,0,0)
 d=ImageDraw.Draw(im);d.polygon([(px(37),px(40)),(px(53),px(40)),(px(45),px(49))],fill='#1E6305')
 return im
def collection_locked():
 im=canvas(81,108);grad_round(im,(1,3,80,107),INK,DARK,8)
 grad_round(im,(2,1,79,104),TEAL,DARK,7)
 grad_round(im,(5,4,76,99),CREAM,WHITE,5)
 grad_round(im,(8,7,73,96),DARK,INK,4)
 d=ImageDraw.Draw(im)
 for y in range(18,90,14):d.line((px(12),px(y),px(68),px(y)),fill='#0F4F5A',width=2)
 center(im,icon('lock'),26,36,29,36)
 return im
def track():
 im=canvas(254,28);grad_round(im,(1,3,253,27),INK,DARK,14);grad_round(im,(3,2,251,24),WHITE,CREAM,12);grad_round(im,(5,5,249,21),DARK,INK,9);return im
def fillbar():
 im=canvas(244,16);grad_round(im,(0,1,243,15),TEAL,DARK,8);ImageDraw.Draw(im).line((px(8),px(3),px(235),px(3)),fill=WHITE,width=px(.7));return im
def build_other(j):
 n=j['id']
 if n==15:
  im=canvas(335,430)
  emblem=canvas(220,180)
  center(emblem,source('Assets/Art/Gameplay/Task15/avatar-pink.png'),16,60,82,96)
  center(emblem,source('Assets/Art/Gameplay/Task15/avatar-green.png'),125,60,82,96)
  center(emblem,icon('jiggle'),65,10,98,142)
  save_asset(CURRENT,'yukleme-jiggles',emblem,'Onaylı üç özgün Jiggle ile yükleme amblemi')
  center(im,emblem,57,32,220,180)
  txt(im,'Reverse Solver',167.5,246,29,WHITE,True,width=310)
  txt(im,'Yükleniyor…',167.5,297,17,CREAM)
  for k in range(3):
   a=canvas(12,12);ImageDraw.Draw(a).ellipse((3,3,32,32),fill=WHITE if k==1 else TEAL);save_asset(CURRENT,'bekleme-noktasi-'+str(k),a,'Belirsiz yükleme animasyonu noktası; gerçek yüzde değildir');put(im,a,143+k*21,336)
  return im
 if n==18:
  im=canvas(270,150)
  for st,on in [('acik',True),('kapali',False)]:
   a=switch(on);save_asset(CURRENT,'anahtar-'+st,a,'44pt yüksek; renk + konum + işaret birlikte',state=st,minhit=[76,44]);save_asset(CURRENT,'anahtar-'+st+'-basili',pressed(a),'Basılı anahtar',state='pressed',minhit=[76,44])
  for k,(t,on) in enumerate([('Ses',True),('Müzik',False)]):
   row=s9(source('Assets/Art/Menu/Task21/settings-row.png'),270,65,(38,38,38,38));put(im,row,0,k*76);txt(im,t,23,32+k*76,18,anchor='lm');put(im,switch(on),181,10+k*76)
  return im
 if n==19:
  im=canvas(286,140)
  for st in ['tr','en']:
   a=language(st);save_asset(CURRENT,'dil-'+st,a,'Boş segmentli dil seçimi; metin ayrı',(32,24,32,24),st,[107,50]);save_asset(CURRENT,'dil-'+st+'-basili',pressed(a),'Basılı dil',state='pressed',minhit=[107,50])
  txt(im,'Dil',143,17,18)
  for y,st in [(40,'tr')]:
   put(im,language(st),36,y);txt(im,'Türkçe',89.5,y+23,16,WHITE);txt(im,'English',196.5,y+23,16,INK)
  txt(im,'Seçili dil turkuaz yüzeyde',143,117,13,DARK)
  return im
 if n==20:
  im=canvas(260,275)
  for st in ['complete','current','locked']:
   a=node(st);save_asset(CURRENT,'dugum-'+st,a,'Sayı ayrı; tek düğüm ailesi',state=st,minhit=[66,66] if st!='locked' else None)
   if st!='locked':save_asset(CURRENT,'dugum-'+st+'-basili',pressed(a),'Basılı düğüm',state='pressed',minhit=[66,66])
  a=play_bubble();save_asset(CURRENT,'oyna-balonu',a,'Oyna metni ayrı; üçgen kuyruk; hedef 90×50',minhit=[90,50]);save_asset(CURRENT,'oyna-balonu-basili',pressed(a),'Basılı Oyna',state='pressed',minhit=[90,50])
  center(im,node('complete'),179.5,189,54,54);txt(im,'12',206.5,214,19,WHITE,True)
  put(im,node('current'),45.5,107);txt(im,'13',78.5,136,20,WHITE,True)
  center(im,node('locked'),179.5,37,54,54)
  put(im,play_bubble(),33.5,19.5);txt(im,'Oyna',78.5,39.5,17,WHITE,True)
  return im
 if n==21:
  im=canvas(227,145);locked=collection_locked();save_asset(CURRENT,'kilitli-hucre',locked,'81×108 pt; 3:4 dış çerçeve; kilit yerleşik')
  ribbon=canvas(89,26);grad_round(ribbon,(1,2,88,25),DARK,INK,7);grad_round(ribbon,(2,1,87,23),TEAL,DARK,6)
  save_asset(CURRENT,'siradaki-seridi',ribbon,'26pt yüksek; etiket ayrı 14pt',(18,18,18,18))
  a=source('Assets/Art/Paintings/painting-05.jpg');a=ImageOps.fit(a,(px(69),px(92)),RES,centering=(.5,.5))
  put(im,locked,16,24);put(im,a,22,31);put(im,ribbon,12,1);txt(im,'Sıradaki',56.5,13,14,WHITE,True)
  put(im,locked,130,24);txt(im,'Bölüm 5',56.5,143,13);txt(im,'Bölüm 6',170.5,143,13)
  return im
 if n==22:
  im=canvas(330,82);back=s9(source('Assets/Art/Menu/Task21/settings-row.png'),330,82,(38,38,38,38));put(im,back,0,0)
  save_asset(CURRENT,'ilerleme-kapsulu',back,'Kısa krem zemin; sayılar ayrı',(38,38,38,38))
  a=track();save_asset(CURRENT,'ilerleme-yatagi',a,'254×28pt boş turkuaz yatak',(40,30,40,30))
  b=fillbar();save_asset(CURRENT,'ilerleme-dolgusu',b,'244×16pt; genişlik esnetilmez, oranla maskelenir',(24,12,24,12))
  txt(im,'315 / 2.000',165,18,18);put(im,a,38,33)
  part=b.crop((0,0,round(b.width*315/2000),b.height));put(im,part,43,38)
  txt(im,'500',104,69,13);txt(im,'1.000',165,69,13);txt(im,'2.000',277,69,13)
  return im
 raise ValueError(n)

def prepare_screen(j,layer):
 # Keep exact screenshot dimensions/registration. Only proposed UI rect is replaced.
 raw=source(j['source']);sz=raw.size;screenH=sz[1]*375/sz[0]
 screen=raw.resize((1125,px(screenH)),RES)
 n=j['id'];w=layer.width/3;h=layer.height/3
 if n in [1,3,4,5,6,7,8,9,10,11,12,13,14,16,17]:
  x,y=(375-w)/2,(screenH-h)/2
  # The legacy popup obscures its backdrop. Reconstruction is limited to its old bounds,
  # using the real unoccluded game/menu capture; all other source pixels stay in place.
  if n in [1,3]:old=(26,80,350,590)
  elif n==4:old=(25,150,350,600)
  elif n==5:old=(25,150,350,550)
  elif n==6:old=(35,70,340,610)
  elif n==7:old=(35,110,340,630)
  elif n==10:old=(35,200,340,600)
  elif n==16:old=(20,215,355,480)
  elif n==17:old=(30,150,345,570)
  else:old=None
  if old:
   bgsrc='docs/qa/2026-11-02-ftue/61_menu_ev.png' if n in [16,17] else 'docs/qa/2026-11-02-ftue/08_b1_serbest.png'
   bg=source(bgsrc).resize(screen.size,RES);veil=Image.new('RGBA',screen.size,(5,25,30,225));bg=Image.alpha_composite(bg,veil)
   old=tuple(v*(screenH/667 if i%2 else 1) for i,v in enumerate(old))
   # Detect the complete bright legacy gold rim; manual bounds were too tight for some headers.
   if n not in (16,):
    small=raw.resize((375,round(screenH)),RES).convert('RGB');mask=Image.new('L',small.size)
    pix=list(small.getdata());mask.putdata([255 if (r>150 and g>65 and b<150 and r-g>15 and g-b>20 and 60<=i//375<screenH-40) else 0 for i,(r,g,b) in enumerate(pix)])
    bb=mask.getbbox()
    if bb:old=(max(12,bb[0]-8),max(60,bb[1]-8),min(363,bb[2]+8),min(screenH-30,bb[3]+8))
   box=tuple(px(v) for v in old);screen.paste(bg.crop(box),box)
  if n in [8,9,11,12,13,14]:screen=Image.alpha_composite(screen,Image.new('RGBA',screen.size,(5,25,30,155)))
  put(screen,layer,x,y)
  crop=(0,max(0,min(y-18,old[1]-10 if old else y-18)),375,min(screenH,max(y+h+18,old[3]+10 if old else y+h+18)))
 elif n==2:
  # In the latest rating card, cover the old two-row chips only and preserve surrounding card.
  x,y=44.5,310
  grad_round(screen,(44,310,331,440),CREAM,CREAM,8);put(screen,layer,x,y);crop=(35,300,340,443)
 elif n==15:
  x,y=20,(screenH-h)/2;put(screen,layer,x,y);crop=(0,0,375,screenH)
 elif n==18:
  x,y=256,158
  # On-screen rows remain exactly where they are. New 76x44 switches replace old 65x36 faces.
  for sy,on in [(158,True),(237,True),(315,True)]:
   box=(255,sy-1,333,sy+45);grad_round(screen,box,CREAM,CREAM,3);put(screen,switch(on),256,sy)
  crop=(235,143,339,370)
 elif n==19:
  x,y=107,400
  grad_round(screen,(106,398,323,451),CREAM,CREAM,8)
  put(screen,language('en'),107,400);txt(screen,'Türkçe',160.5,423,16,INK);txt(screen,'English',267.5,423,16,WHITE);crop=(40,382,340,465)
 elif n==20:
  x,y=45,275
  # Exact existing node centers. Approved glow, path, avatar, and number placements stay visible.
  put(screen,layer,x,y);crop=(45,275,305,550)
 elif n==21:
  x,y=143,259
  ribbon=canvas(89,26);grad_round(ribbon,(1,2,88,25),DARK,INK,7);grad_round(ribbon,(2,1,87,23),TEAL,DARK,6)
  put(screen,ribbon,x,y);txt(screen,'Sıradaki',x+44.5,y+12,14,WHITE,True)
  put(screen,collection_locked(),260,269);crop=(135,250,350,395)
 elif n==22:
  x,y=22.5,483
  # A cream-backed proposal explicitly replaces the old horizontal rail region.
  put(screen,layer,x,y);crop=(10,475,365,571)
 after=screen.resize(sz,RES)
 crop=tuple(round(v*sz[0]/375) for v in crop)
 return raw,after,crop,dict(xPt=x,yPt=y,widthPt=w,heightPt=h)

def review(j,layer):
 raw,after,crop,place=prepare_screen(j,layer)
 before=raw.crop(crop);aft=after.crop(crop)
 # Single shared pixel crop, no independent zooming or repositioning.
 pad=30;head=65;foot=42;cw=before.width;ch=before.height
 pair=Image.new('RGB',(cw*2+pad*3,ch+head+foot),'#102F39')
 pair.paste(before.convert('RGB'),(pad,head));pair.paste(aft.convert('RGB'),(cw+pad*2,head))
 d=ImageDraw.Draw(pair);f=ImageFont.truetype(str(FONT),max(9,min(33,round(cw/19))))
 d.text((pad,20),'ÖNCE · '+Path(j['source']).parent.name,font=f,fill=CREAM)
 d.text((cw+pad*2,20),'SONRA · T22 TASLAK',font=f,fill=CREAM)
 d.text((pad,head+ch+8),f'{j["id"]:02} · Aynı ekran kırpımı / aynı ölçek',font=ImageFont.truetype(str(FONT),min(27,max(9,round(pair.width/30)))),fill=CREAM)
 pair.save(CURRENT/'once-sonra.png');after.save(CURRENT/'ekran-sonra.png')
 # Phone previews are exact 375x667 /390x844 viewports. Full popup layouts are recentered,
 # other UI overlays preserve proportional anchors. This is visual QA, not a Unity device test.
 for sw,sh in [(375,667),(390,844)]:
  if j['id'] in [1,3,4,5,6,7,8,9,10,11,12,13,14,16,17]:
   bgpath='docs/qa/2026-11-02-ftue/61_menu_ev.png' if j['id'] in [16,17] else 'docs/qa/2026-11-02-ftue/08_b1_serbest.png'
   a=ImageOps.fit(source(bgpath),(sw,sh),RES);a=Image.alpha_composite(a,Image.new('RGBA',a.size,(5,25,30,225)))
   ui=layer.resize((round(layer.width/3),round(layer.height/3)),RES)
   a.alpha_composite(ui,((sw-ui.width)//2,(sh-ui.height)//2))
  else:a=ImageOps.pad(after,(sw,sh),RES,color=INK,centering=(.5,.5))
  a.save(CURRENT/f'telefon-{sw}x{sh}.png')
 return dict(source=j['source'],sourceSize=list(raw.size),cropPx=list(crop),placementPt=place)

MAN=[]
for j in J:
 CURRENT=O/j['folder'];CURRENT.mkdir(exist_ok=True);TEXTS=[];PLACEMENTS=[]
 start=len(RECORDS)
 if j['id'] in [1,3,4,5,6,7,8,9,10,11,12,13,14,16,17]:layer=compose_card(j)
 elif j['id']==2:layer=build_chip(j)
 else:layer=build_other(j)
 layer.save(CURRENT/'taslak-katmani.png')
 evidence=review(j,layer)
 (CURRENT/'metin-yerlesimi.json').write_text(json.dumps({'font':'Nunito Black','scale':3,'text':TEXTS,'buttons':PLACEMENTS},ensure_ascii=False,indent=2),encoding='utf8')
 assets=RECORDS[start:]
 limitation=''
 if j['id'] in [8,9,11,12,13]:limitation='Önce görüntüsü modal kart değildir; en yeni ilgili öğretim/booster bağlamıdır. Sonra bu bağlam üstüne önerilen karttır. Mevcut modal gibi gösterilmez.'
 elif j['id'] in [4,5,14,18,19]:limitation='Gece gelen 3 Kasım tam tur kaydı kullanıldı. Kaynak İngilizce; sonra kartlar Türkçe inceleme taslağıdır. Dil seçicide mevcut English seçimi korunur. S14 kaynağı zor bölüm başlangıç kartıdır, bağımsız uyarı modalı değildir.'
 elif j['id'] in [1,2,3]:limitation='En yeni rating kaydı İngilizce; sonra Türkçe inceleme taslağıdır. İki sıra mevcut kodda zaten vardır. Soru/oyun verisi örnektir.'
 note=f'''# {j['id']:02} — {j['title']}

- Bulgu: S{j['id']:02}. Öncelik: {j['priority']}.
- Sorun: {j['issue']}
- Değişiklik: Onaylı Task20/21 krem-turkuaz malzeme ve Task15 tuş boyasıyla yeniden kurulan UI. Başlık Nunito Black; krem dolgu, lacivert kontur ve aşağı gölge. Nesne/karakter kimlikleri korunur. Bu bir özgün karakter yeniden çizimi değildir.
- Neden: Daha sakin yüzey, tek ana eylem, okunur metin ve dokunulur boyut. Yalnız renk farkına dayanmayan seçim durumları kullanılır.
- Yerine geçecek kaynak/üretim noktası: `{j['target']}`. Kodla çizilen öğelerde birebir eski PNG yoktur; sprite entegrasyonu PM/Claude işidir.
- Kaynak ekran: `{j['source']}`; gerçek boyut {evidence['sourceSize']}. Önce/sonra aynı `cropPx={evidence['cropPx']}` ile alınır. Konumlar `metin-yerlesimi.json` ve ana manifestte.
- {limitation or 'Kaynak gerçek ekran kaydıdır; etkileşim testi yapılmadı.'}
- Eski modalın kapattığı arka plan yalnız eski modal sınırında gerçek oyun/menü fotoğrafı + koyu perde ile yeniden kuruldu. Haritada düğümler aynı mevcut merkezlere yerleşir; yol, avatar ve onaylı parlama korunur. Sonra görseli gerçek oyun screenshotı diye sunulmaz.
- `taslak-katmani.png` yalnız inceleme içindir: örnek metinleri içerir. `panel`, `baslik-zemini`, tuş ve diğer parça PNG'leri metinsiz üretim adaylarıdır. Sayılar/metinler kodda basılmalıdır.
- Ölçek: her sprite 3 px = 1 pt; `manifest.json` gerçek boyutları, normal/basılı durumları, SHA-256 ve `sliceLTRB` değerlerini içerir. 9-slice sırası **sol, üst, sağ, alt**; değerler fiziksel pikseldir. İkonlar esnetilmez. Hiçbir dosya oyuna bağlanmadı; Unity .meta üretilmedi.
- QA: son yerleşim denemesi 3 (azami sınır). Son görsel inceleme sonucu ortak `docs/art/task22/QA.md` dosyasında. Telefon PNG'lerinde kartlar özgün en/boy ile yeniden ortalanır; diğer kaynak ekranlar en/boy korunarak sığdırılır. Gerçek responsive Unity testini temsil etmez.

## Parçalar

| Dosya | @3x px | pt | 9-slice L/T/R/B | Durum |
|---|---:|---:|---|---|
'''
 for a in assets:note+=f"| {Path(a['file']).name} | {a['width']}×{a['height']} | {a['sizePt'][0]:g}×{a['sizePt'][1]:g} | {a['sliceLTRB']} | {a['state']} |\n"
 (CURRENT/'NOT.md').write_text(note,encoding='utf8')
 MAN.append({**j,**evidence,'assets':[a['file'] for a in assets],'attempt':3,'review':'pending'})
 print(f"{j['id']:02}: {j['title']} ({len(assets)} PNG)",flush=True)
 (O/'manifest.json').write_text(json.dumps({'scale':3,'font':'Nunito Black','integrated':False,'items':MAN,'assets':RECORDS},ensure_ascii=False,indent=2),encoding='utf8')

# Self-contained, local, offline review. No external fonts, scripts, or network dependencies.
page='''<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task22 — Önce / Sonra</title><style>
:root{color-scheme:dark}*{box-sizing:border-box}body{margin:0;background:#08232d;color:#fff9f1;font:16px/1.55 system-ui,sans-serif}header,main{max-width:1200px;margin:auto;padding:24px}header{padding-top:48px}h1{font-size:clamp(28px,5vw,48px);margin:0}p{max-width:900px;color:#cce1df}a{color:#87e4e3}nav{display:flex;flex-wrap:wrap;gap:8px}nav a,button{padding:10px 16px;border-radius:10px;background:#1c7f8c;color:#fff9f1;border:1px solid #448c95;text-decoration:none;min-height:44px}article{padding:24px;margin:24px 0;border:1px solid #34616a;border-radius:18px;background:#103540;scroll-margin-top:15px}h2{margin:14px 0 4px}.pair{display:block;max-width:100%;height:auto;margin:auto;border-radius:8px}label{display:flex;gap:12px;align-items:center;font-weight:bold;margin:16px 0}input{width:24px;height:24px;accent-color:#79c800}small{display:block;color:#c4d7d7}details{margin-top:12px}pre{white-space:pre-wrap;background:#08232d;border-radius:8px;padding:16px}.sticky{position:sticky;bottom:0;padding:12px 24px;background:#061d26ed;border-top:1px solid #448c95}button{cursor:pointer}#selection{font-weight:bold;margin-right:18px}footer{padding:32px;text-align:center}</style>
<header><small>REVERSE SOLVER · 03 KASIM 2026 · ENTEGRASYON YOK</small><h1>22 öneri. Önce / Sonra.</h1><p>Solda kaynak ekranın kırpımı, sağda aynı kırpıma yerleştirilmiş T22 taslağı. Onaylı nesneler ve tuş malzemeleri korunarak panel, seçim ve yazı hiyerarşisi yenilendi. Seçim kutuları yalnız bu tarayıcıdaki inceleme yardımcısıdır; oyunu değiştirmez.</p><p>Kaynağı eski veya modal olmayan işler her kartta açıkça yazılıdır. Telefon önizlemeleri kompozisyon ölçeğidir; Unity cihaz testi değildir. Taslaklar Türkçe; en yeni 01–03 rating kaynakları İngilizcedir.</p><nav>'''
page+=''.join(f'<a href="#i{j["id"]}">{j["id"]:02}</a>' for j in J)+'</nav></header><main>'
for j in J:
 folder=html.escape(j['folder']);title=html.escape(j['title']);lim=''
 if j['id'] in [8,9,11,12,13]:lim='Önce: ilgili öğretim/booster bağlamı; modal kart kaydı değil.'
 if j['id'] in [4,5,14,18,19]:lim='Önce: 3 Kasım tam tur (İngilizce). S14: zor bölüm başlangıç bağlamı.'
 page+=f'''<article id="i{j['id']}"><a href="{folder}/once-sonra.png"><img class="pair" loading="lazy" src="{folder}/once-sonra.png" alt="{j['id']:02} {title}: solda önce, sağda sonra"></a><h2>{j['id']:02} — {title}</h2><p>{html.escape(j['issue'])}</p><small>{html.escape(lim)} Kaynak: {html.escape(j['source'])}</small><label><input type="checkbox" value="{j['id']}" aria-label="{j['id']} numarayı seç">Bu öneriyi seç</label><details><summary>Parçalar ve teknik not</summary><p><a href="{folder}/NOT.md">NOT.md</a> · <a href="{folder}/ekran-sonra.png">Tam ekran taslak</a> · <a href="{folder}/telefon-375x667.png">375×667</a> · <a href="{folder}/telefon-390x844.png">390×844</a> · <a href="{folder}/metin-yerlesimi.json">Metin/konum</a></p></details></article>'''
page+='''</main><div class="sticky"><span id="selection" role="status" aria-live="polite">Seçim yok</span><button id="copy">Numaraları kopyala</button></div><footer>22 / 22 çizim işi · Yalnız art adayları · <a href="manifest.json">Manifest</a></footer><script>
const boxes=[...document.querySelectorAll('input')],status=document.getElementById('selection');function selection(){return boxes.filter(b=>b.checked).map(b=>b.value).join(', ')}function update(){status.textContent=selection()?selection()+' olsun':'Seçim yok';try{localStorage.setItem('t22-selection',selection())}catch{}}
try{const saved=(localStorage.getItem('t22-selection')||'').split(', ');boxes.forEach(b=>b.checked=saved.includes(b.value))}catch{}boxes.forEach(b=>b.addEventListener('change',update));update();document.getElementById('copy').onclick=async()=>{const text=selection();if(!text)return;try{await navigator.clipboard.writeText(text+' olsun');status.textContent=text+' · kopyalandı'}catch{status.textContent=text+' olsun · seçip kopyalayabilirsin'}};</script></html>'''
(O/'inceleme.html').write_text(page,encoding='utf8')
print(f'DONE {len(J)} jobs / {len(RECORDS)} runtime candidate PNGs',flush=True)
