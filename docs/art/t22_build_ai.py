"""Non-destructive AI bitmap export + screenshot compositing. No runtime hookup."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageOps, ImageChops, ImageStat
import json, hashlib, shutil, html, math
ROOT=Path(__file__).resolve().parents[2];WORK=ROOT/'docs/art/task22';OUT=ROOT/'Assets/Art/Redesign_T22';SRC=OUT/'ai-kaynaklar';SRC.mkdir(exist_ok=True)
R=Image.Resampling.LANCZOS; FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf';INK='#0B2A36';CREAM='#FFF6D8'
mapping=json.loads((WORK/'ai-sources.json').read_text(encoding='utf-8'));RAW={};RECORDS=[];JOBS=[];TEXT=[]
for k,p in mapping.items():
 q=SRC/(k+'.png')
 if not q.exists():shutil.copyfile(p,q)
 a=Image.open(q).convert('RGBA');bb=a.getchannel('A').point(lambda x:255 if x>=24 else 0).getbbox();RAW[k]=a.crop(bb)
def nine(a,size,b):
 l,t,r,bot=b;w,h=a.size;tw,th=size
 assert tw>l+r and th>t+bot
 xx=[0,l,w-r,w];yy=[0,t,h-bot,h];dx=[0,l,tw-r,tw];dy=[0,t,th-bot,th];im=Image.new('RGBA',size)
 for j in range(3):
  for i in range(3):im.alpha_composite(a.crop((xx[i],yy[j],xx[i+1],yy[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),R),(dx[i],dy[j]))
 return im
PANEL=RAW['panel'].resize((990,1290),R)
def export(folder,name,im,role,b=(0,0,0,0),source=None):
 p=OUT/folder;p.mkdir(exist_ok=True);q=p/(name+'.png');im.save(q)
 row=dict(file=q.relative_to(OUT).as_posix(),width=im.width,height=im.height,logicalPt=[im.width/3,im.height/3],scale=3,sliceLTRB=list(b),role=role,source=source or 'imagegen; ai-kaynaklar',sha256=hashlib.sha256(q.read_bytes()).hexdigest(),isRuntimeCandidate=True,approved=False)
 RECORDS.append(row);return im
def texture(folder,name,key,pt,b=(0,0,0,0)):
 a=RAW[key].resize(tuple(round(x*3) for x in pt),R);return export(folder,name,a,'Yeni boş bitmap; metin ayrı',b)
def art(path):return Image.open(ROOT/path).convert('RGBA')
def fit(a,size):return ImageOps.contain(a,tuple(round(x) for x in size),R)
def paste(im,a,box,contain=True):
 x,y,w,h=box;sz=(round(w*3),round(h*3));a=fit(a,sz) if contain else a.resize(sz,R);im.alpha_composite(a,(round(x*3+(sz[0]-a.width)/2),round(y*3+(sz[1]-a.height)/2)))
def text(im,t,x,y,pt=16,color=INK,outline=False,maxw=None):
 f=ImageFont.truetype(str(FONT),round(pt*3));d=ImageDraw.Draw(im);stroke=round(pt*.05*3) if outline else 0
 assert not maxw or f.getlength(t)+stroke*2<=maxw*3,(t,pt,maxw,f.getlength(t)/3)
 if outline:d.text((round(x*3),round(y*3+pt*.08*3)),t,font=f,anchor='mm',fill=INK,stroke_width=stroke,stroke_fill=INK)
 d.text((round(x*3),round(y*3)),t,font=f,anchor='mm',fill=color,stroke_width=stroke,stroke_fill=INK)
 TEXT.append(dict(text=t,x=x,y=y,sizePt=pt,maxWidthPt=maxw))
def body(im,lines,x,y,pt=14,gap=21):
 for i,line in enumerate(lines):text(im,line,x,y+i*gap,pt,maxw=285)
def button(im,label,x,y,w=220,h=52,primary=True):
 a=art('Assets/Art/Menu/Task20/button-'+('primary' if primary else 'secondary')+'.png');paste(im,a,(x-w/2,y,w,h),False);text(im,label,x,y+h/2-2,17,CREAM,True,w-20)
def panel(folder,pt,title):
 a=nine(PANEL,tuple(round(v*3) for v in pt),(72,210,72,72));export(folder,'panel-@3x',a,'Ortak boş turkuaz/krem panel',(72,210,72,72));o=a.copy();text(o,title,pt[0]/2,35,23,CREAM,True,pt[0]-35);return o
def screenshot(path,width=375):
 a=art(path);return a.resize((width*3,round(a.height/a.width*width*3)),R)
def compare(folder,title,source,rect,proposal,note,target,variants=None):
 # Both halves retain the same source screenshot; only the proposal rectangle is replaced.
 old=screenshot(source);new=old.copy();x,y,w,h=rect
 if proposal is not None:
  if folder[:3] in {'101','102','103','104','105','107','108'}:
   # Reconstruct only the modal's underlying scene from our own approved art.
   bg=art('Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png') if folder[:3] not in {'107','108'} else art('Assets/Art/Menu/Task20/scene-home.png')
   bg=bg.resize(old.size,R);bg.alpha_composite(Image.new('RGBA',bg.size,(8,32,34,220)))
   box=(round(x*3),round(y*3),round((x+w)*3),round((y+h)*3));new.paste(bg.crop(box),box[:2])
   note+=' Eski modalın köşe izlerini kaldırmak için yalnız panel altındaki sahne aynı oyunun onaylı arka planıyla yeniden kuruldu; karşılaştırma piksel eşitliği testi değildir.'
  paste(new,proposal,rect,False)
 p=OUT/folder;new.save(p/'ekran-sonra-375.png')
 crop=(max(0,round((x-7)*3)),max(0,round((y-7)*3)),min(old.width,round((x+w+7)*3)),min(old.height,round((y+h+7)*3)))
 left=old.crop(crop);right=new.crop(crop);pair=Image.new('RGB',(left.width*2+30,left.height+90),'#173D49');pair.paste(left,(0,75));pair.paste(right,(left.width+30,75));d=ImageDraw.Draw(pair);f=ImageFont.truetype(str(FONT),30);d.text((20,12),'ÖNCE · kaynak ekran',font=f,fill=CREAM);d.text((left.width+50,12),'SONRA · onaysız taslak',font=f,fill=CREAM);pair.save(p/'once-sonra.png')
 (p/'NOT.md').write_text(f'# {folder} — {title}\n\nOnaysız aday; oyuna bağlanmadı.\n\nKaynak: `{source}`. @2x veya farklı çözünürlükteki görüntü375 pt genişliğe normalize edildi. Önce/sonra aynı bağlam ve aynı dikdörtgende: `{rect}` pt.\n\n{note}\n\nYerine geçme önerisi: `{target}`. Doğrudan değiştirilmedi; sprite anahtarı ve yerleşim uyarlaması ayrı PM/kod işi.\n\nTüm runtime adayları RGBA, @3x; metinler yalnız önizlemede Nunito Black ile çizilir, boş sprite içine gömülmez. Manifestteki L,T,R,B kenarlar kaynak pikselidir; Unity L,B,R,T sırasına çevrilmelidir. İkon/ana eylem tuşu onaylı Task14/15/20/21 bitmaplerinden yalnız önizlemeye yerleştirildi; bu parçalar yeniden çizildi diye sayılmaz.\n\nGörsel kabul:1. taslak. Bu sürüm telefon önizlemesi ve alfa/kenar/boyut kontrolünden sonra değerlendirilecek; nihai QA `ai-validation.json` içinde.\n'+ ('\nVaryantlar: '+', '.join(variants)+'\n' if variants else ''),encoding='utf-8')
 JOBS.append(dict(folder=folder,title=title,source=source,rectPt=rect,note=note,target=target,variants=variants or []))
F='docs/qa/2026-11-02-ftue/';S='docs/qa/2026-11-02-son/';M='docs/qa/2026-11-01-p20b/';T='docs/qa/2026-11-03-tam/'
# 101: current two-row rating rather than obsolete one-row layout.
folder='101-ai-kazanma';im=panel(folder,(335,530),'Tahta boşaldı!')
for i in range(3):paste(im,art('Assets/Art/Gameplay/Task14/star-full.png'),(108+i*42,74,35,35))
text(im,'108 saniye kaldı · 0 takılma',167.5,123,14,maxw=292);button(im,'Sonraki bölüm',167.5,148,248,56)
text(im,'Ne kadar zordu?',167.5,236,17,maxw=285);text(im,'1 çok kolay · 10 çok zor',167.5,259,13,maxw=285)
chip=RAW['chip'].resize((144,132),R)
for i in range(10):
 x=31+(i%5)*56;y=281+(i//5)*50;paste(im,chip,(x,y,48,44),False);text(im,str(i+1),x+24,y+22,17)
button(im,'Tekrar oyna',91,404,142,48,False);button(im,'Menü',244,404,142,48,False);text(im,'Puanlamayı geç',167.5,481,14,maxw=285)
compare(folder,'Kazanma · ortak ince çerçeve','docs/qa/2026-11-02-rating/a_kart.png',(20,70,335,530),im,'Güncel ana eylem önce, iki sıralı puan ve altta geç bağlantısı korunur. Yenilik turkuaz panel ve dokulu kutulardır. Rakamlar48×44 pt; yapısal kod önerisi ayrı.','Assets/Art/Gameplay/Task14/card-panel.png → Resources/Art/popup-card.png')
# 102 losses: each separately previewed, shared art shell.
folder='102-ai-kayip'
for key,label,lines,source in [('bomb','Bomba patladı',['Fitil bitmeden yolunu aç.','Çekiç bombayı tek hamlede söker.'],'docs/qa/2026-11-02-rating/c_kayip_kart.png'),('time','Süre doldu',['Tahtada parçalar kaldı.','Saat sana 20 saniye kazandırır.'],T+'375x667_06_kaybetme.png')]:
 im=panel(folder,(335,580),label);paste(im,art('Assets/Art/Gameplay/Task14/loss-reason-'+key+'.png'),(124,96,87,87));body(im,lines,167.5,227);button(im,'Tekrar dene',167.5,310,248,58);button(im,'Menü',167.5,403,220,52,False);text(im,'Her denemede yeni bir yol var.',167.5,521,13,maxw=295);im.save(OUT/folder/('onizleme-'+key+'.png'))
 if key=='bomb':compare(folder,'Bomba ve süre kaybı',source,(20,43,335,580),im,'Bomba kaynağı2 Kasım rating. Bu puan sorusu kapalı görünüm önerisidir; soru akışını kaldırma izni vermez. Süre için3 Kasım tam tur375/390 karesi sonradan geldi ve ayrı önce/sonra eklendi.','Assets/Art/Gameplay/Task14/card-lose-bomb.png / card-lose-time.png; ortak popup-card', ['bomba','süre'])
 else:
  old=screenshot(source);new=old.copy();bg=art('Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png').resize(old.size,R);bg.alpha_composite(Image.new('RGBA',bg.size,(8,32,34,220)));box=(60,120,1065,1860);new.paste(bg.crop(box),box[:2]);paste(new,im,(20,40,335,580),False);new.save(OUT/folder/'ekran-sonra-sure-375.png')
  pair=Image.new('RGB',(old.width*2+30,old.height+75),'#173D49');pair.paste(old,(0,75));pair.paste(new,(old.width+30,75));dd=ImageDraw.Draw(pair);ff=ImageFont.truetype(str(FONT),30);dd.text((20,10),'ÖNCE ·3 Kasım süre kaybı',font=ff,fill=CREAM);dd.text((old.width+50,10),'SONRA · onaysız taslak',font=ff,fill=CREAM);pair.save(OUT/folder/'once-sonra-sure.png')
# 103 pause.
folder='103-ai-durdurma';im=panel(folder,(335,440),'Durduruldu');text(im,'Hazır olduğunda devam et.',167.5,111,15,maxw=290)
for label,y,primary in [('Devam',162,True),('Yeniden başla',245,False),('Ana menü',328,False)]:button(im,label,167.5,y,248,52,primary)
compare(folder,'Durdurma',T+'375x667_04_pause.png',(20,115,335,440),im,'3 Kasım tam tur kullanıldı. Kaynak İngilizce, aday Türkçe; metinler önizleme yerelleştirmesidir. Devam eylemi tek yeşil, kalan iki eylem turkuaz.','Assets/Art/Gameplay/Task14/card-pause.png; ortak popup-card')
# 104 eight introductions; exact own-game objects retained.
folder='104-ai-tanitim';intro=[('nail','Çivili parça',['Diğer parçaları çıkar.','Sayaç sıfır olunca çivi açılır.'],F+'14_b3_kart.png','Assets/Art/Gameplay/ObstacleIcons/nail-128.png'),('bomb','Bombalı parça',['Fitil bitmeden bombayı çıkar.','Önce onun yolunu aç.'],F+'37_b6_kart.png','Assets/Art/Gameplay/Task14/bomb-384.png'),('chain','Zincirli parçalar',['İki parça birlikte çıkar.','İkisinin yolunu da aç.'],'docs/qa/2026-11-02-spot/375x667_b10_1.png','Assets/Art/Gameplay/Task17/chain-gold-endpoint-144.png'),('wall','Duvar',['Parça duvardan geçemez.','Açık kenardan yol bul.'],'docs/qa/2026-11-02-spot/375x667_b14_1.png','Assets/Art/Gameplay/Task15/wall-repeat-v3.png')]
for key,title,lines,src,obj in intro:
 if not (ROOT/obj).exists():obj='Assets/Art/Gameplay/Task14/'+('chain-endpoint-144.png' if key=='chain' else 'wall-repeat-96x48.png')
 im=panel(folder,(335,590),title);paste(im,art(obj),(115,125,105,105))
 if key=='bomb':text(im,'23',167.5,181,23)
 body(im,lines,167.5,309,15);button(im,'Anladım',167.5,466,248,55);im.save(OUT/folder/('onizleme-'+key+'.png'))
 if key=='nail':compare(folder,'8 tanıtım · nesneye odaklı ortak kabuk',src,(20,110,335,590),im,'Çivi/bomba için gerçek modal önce karesi var. Zincir/duvar ve makas dışı booster modal karesi yok; ilgili en yeni öğretim ekranı yalnız bağlamdır. Önizlemedeki nesne ikonları onaylı kendi oyun kaynaklarıdır. Eğitim eli değiştirilmedi.','Screens.Intro + popup-card + mevcut intro-* / booster ikonları',[x[0] for x in intro]+['scissors','wand','hammer','clock'])
for key,title,lines,src in [('scissors','Makas',['Önce makası seç.','Sonra kesilecek ekleme dokun.'],F+'34_b5_kart.png'),('wand','Sihirli değnek',['Çıkıntıları yeniden dizer.','Resim aynı kalır, kilitler değişir.'],'docs/qa/2026-11-02-booster/b9_1_tus.png'),('hammer','Çekiç',['Önce çekici seç.','Çıkarmak istediğin parçaya dokun.'],'docs/qa/2026-11-02-booster/b23_1_tus.png'),('clock','Saat',['Süreye 20 saniye ekler.','Kullanmadan önce onay ver.'],'docs/qa/2026-11-02-booster/b15_4_onay.png')]:
 im=panel(folder,(335,590),title);paste(im,art('Assets/Art/Gameplay/Task14/booster-'+key+'.png'),(120,130,95,95));body(im,lines,167.5,309,14);button(im,'Anladım',167.5,466,248,55);im.save(OUT/folder/('onizleme-'+key+'.png'))
# 105 hard warning based on code; no invented screenshot evidence.
folder='105-ai-zor';im=panel(folder,(335,400),'Zor bölüm');paste(im,art('Assets/Art/Menu/Task20/baron-head.png'),(124,85,87,100));body(im,['Bu bölüm biraz daha zorlu.','İlk hamleden önce yolları incele.'],167.5,222,14);button(im,'Başla',167.5,300,248,55)
compare(folder,'Zor bölüm · güncel giriş',T+'375x667_02_baslangic.png',(20,135,335,400),im,'3 Kasım tam tur Level19 Tough level başlangıç karesi kullanıldı. Yeni Baron odaklı görünüm önerisidir; mevcut kural/engel verileri oyunda korunmalı. İngilizce kaynak/Türkçe önizleme.','Screens.HardWarning / StartCard + hard.png')
# 106 loading overlay - transparent presentation component, background retained.
folder='106-ai-yukleme';im=Image.new('RGBA',(990,1200));paste(im,art('Assets/Art/Menu/Task20/avatar-blue.png'),(107,12,116,116));text(im,'Reverse Solver',165,173,29,CREAM,True,320);text(im,'Galerin hazırlanıyor',165,223,17,CREAM,True,310)
paste(im,art('Assets/Art/Menu/Task20/progress-track.png'),(37,267,256,22),False);paste(im,art('Assets/Art/Menu/Task20/progress-fill.png'),(41,272,128,12),False);text(im,'Yükleniyor…',165,333,14,CREAM,True,290)
skin=Image.new('RGBA',(990,1200));paste(skin,art('Assets/Art/Menu/Task20/avatar-blue.png'),(107,12,116,116));paste(skin,art('Assets/Art/Menu/Task20/progress-track.png'),(37,267,256,22),False);paste(skin,art('Assets/Art/Menu/Task20/progress-fill.png'),(41,272,128,12),False)
export(folder,'loading-content-@3x',skin,'Metinsiz yerleşim kabuğu; yüzde/fill ve metin entegrasyonda dinamik olmalı',source='Yeni dizilim; mevcut Task20 avatar ve progress sprite yeniden kullanıldı')
compare(folder,'Yükleme · kimlik ve görünür durum',F+'01_yukleme.png',(22,155,330,400),im,'Kaynak boş koyu yükleme karesi; tek kareyle donma sonucu çıkarılmaz. Bu bir sunum önerisidir. Ürün adı ve durum önizlemede; gerçek yüzde veriyle sürülmeli.','Assets/WebGLTemplates/ReverseSolver/index.html #loader / TemplateData v3-progress-*')
# 107 name and 108 profile question.
folder='107-ai-isim';im=panel(folder,(335,350),'Hoş geldin!');body(im,['Adını yaz, sonuçların','bu isimle kaydedilsin.'],167.5,110,15);paste(im,RAW['chip'],(28,175,279,58),False);text(im,'Adın',167.5,204,17);button(im,'Başla',167.5,259,248,57)
compare(folder,'İsim girişi · oyunla aynı aile',F+'02_isim.png',(20,265,335,350),im,'Web formu yeni ortak kabukla önerildi; düzenlenebilir alan önizlemedir, gerçek input ve klavye kodu değiştirilmedi.','Assets/WebGLTemplates/ReverseSolver/index.html #rs-name')
folder='108-ai-profil-sorusu';im=panel(folder,(335,420),'Seni tanıyalım');body(im,['Bu tür bulmaca oyunlarını','ne sıklıkla oynarsın?'],167.5,105,15)
for label,y in [('Hiç',164),('Ara sıra',230),('Sık',296)]:button(im,label,167.5,y,248,50,False)
compare(folder,'Profil sorusu · eşit seçenekler',F+'03_profil_sorusu.png',(20,190,335,420),im,'Üç yanıt eşdeğer turkuaz eylem. Atlanabilirlik yalnız ürün/kod önerisidir; bu aday oyunun davranışını değiştirmez.','Profil anketi Card / popup-card')
# 109 settings strips, rendered over exact source screen region.
folder='109-ai-ayarlar';on=texture(folder,'toggle-on-@3x','switch',(78,44));off=texture(folder,'toggle-off-@3x','off',(78,44));lang=texture(folder,'language-tr-@3x','language',(210,50));en=texture(folder,'language-en-@3x','languageright',(210,50))
base=screenshot(T+'375x667_08_ayarlar.png');overlay=base.copy()
for y,a in [(159,on),(237,on),(315,off)]:paste(overlay,a,(248,y,78,44),False)
paste(overlay,en,(116,400,210,50),False);text(overlay,'Türkçe',168.5,425,16,INK,False,93);text(overlay,'English',273.5,425,16,CREAM,True,93)
# exact patch includes existing settings context; accepted as screen composition, runtime files separate.
crop=(50,143,300,339);x,y,w,h=crop;patch=overlay.crop((x*3,y*3,(x+w)*3,(y+h)*3))
compare(folder,'Ayarlar · renk dışı durum işareti',T+'375x667_08_ayarlar.png',crop,patch,'3 Kasım kaynağı. ON tik/sağ düğme, OFF çarpı/sol düğme. Titreşim OFF yalnız durum örneği; mevcut tercihleri değiştirmez. Mevcut kod hedefleri en az44 pt zaten. Dil etiketleri ayrı metin, kaynakta olduğu gibi English seçili.','Task20/toggle-* ve language-*-selected; MenuSettings render uyarlaması gerekir')
# 110 current level: only current node/bubble changed; locked/completed approved nodes retained.
folder='110-ai-harita';node=texture(folder,'node-current-@3x','node-v2' if 'node-v2' in RAW else 'node',(66,66));bubble=texture(folder,'play-bubble-@3x','bubble',(88,48));a=screenshot(S+'375x667_harita.png');patch=a.crop((30*3,295*3,180*3,478*3))
# Determine the exact path repeat phase from unoccluded center pixels, not by guessing.
tile=art('Assets/Art/Menu/Task20/levels-path-repeat.png').resize((256,512),R);small=a.resize((375,667),R)
samples=[(x,y) for x in [177,190,203] for y in range(125,540,17)]
best=(float('inf'),0)
for offset in range(512):
 score=0;count=0
 for x,y in samples:
  c=tile.getpixel((round(x-59.5),(y+offset)%512));o=small.getpixel((x,y))
  if c[3]>240:score+=sum((c[k]-o[k])**2 for k in range(3));count+=1
 if count and score/count<best[0]:best=(score/count,offset)
back=art('Assets/Art/Menu/Task20/scene-levels.png').resize(a.size,R);tile3=tile.resize((768,1536),R)
for y in range(-512,667+512,512):back.alpha_composite(tile3,(round(59.5*3),round((y-best[1])*3)))
for box in [(77,307,173,352),(73,377,174,462)]:
 x0,y0,x1,y1=box;patch.paste(back.crop((x0*3,y0*3,x1*3,y1*3)),((x0-30)*3,(y0-295)*3))
paste(patch,bubble,(49,12,88,48),False);text(patch,'Oyna',93,34,17,CREAM,True,78);paste(patch,node,(60,87,66,66),False);text(patch,'13',93,120,22,CREAM,True,54)
compare(folder,'Harita · tek yeşil eylem',S+'375x667_harita.png',(30,295,150,183),patch,'Yalnız güncel düğüm ve Oyna balonu. Tamam/kilit/zor durumlarının onaylı Task20 sprite ve numaraları korunmalı. Eski düğümü gizleyen yeni konumlar görsel kontrolle düzeltilir.','Task20/node-current.png + LevelsMap.cs PlayBubble (koda aktarılmadı)')
# 111 collection status, new ribbon and cream framed locked tile using approved border.
folder='111-ai-koleksiyon';ribbon=texture(folder,'next-label-@3x','next',(90,27));locked=Image.new('RGBA',(243,324));paste(locked,RAW['chip'],(0,0,81,108),False);paste(locked,art('Assets/Art/Menu/Task20/portrait-frame.png'),(0,0,81,108),False);paste(locked,art('Assets/Art/Menu/Task20/icon-lock.png'),(26,39,29,32));export(folder,'locked-cell-@3x',locked,'Yeni krem yüzey ve mevcut çerçeve/ikonun birleşimi',source='imagegen chip + Task20 portrait-frame, icon-lock')
base=screenshot(S+'375x667_koleksiyon_siradaki.png');patch=base.crop((127*3,125*3,357*3,390*3));paste(patch,ribbon,(15,140,90,27),False);text(patch,'Sıradaki',60,152,14,CREAM,True,83);paste(patch,locked,(133,145,81,108),False)
compare(folder,'Koleksiyon · okunur Sıradaki',S+'375x667_koleksiyon_siradaki.png',(127,125,230,265),patch,'Etiket14 pt; kilitli hücreyi yeni krem yüzey ve onaylı çerçeveyle birleştirir. Resim ve hücre etkileşimi değiştirilmedi; etiket durum bilgisidir.','CollectionScreen next-ribbon + Task20 portrait-placeholder (sahne bağlantısı yok)')
# 112 remove long horizontal rail in screenshot mockup; cover local strip with clean scene art.
folder='112-ai-jiggles';medal=texture(folder,'reward-medal-@3x','milestone',(56,66));lockedmedal=medal.copy();paste(lockedmedal,art('Assets/Art/Menu/Task20/icon-lock.png'),(40,46,16,20));export(folder,'reward-medal-locked-@3x',lockedmedal,'Eşik altı: kilit ile belirtilen madalyon',source='imagegen milestone + Task20 icon-lock');base=screenshot(S+'375x667_jiggles.png');patch=Image.new('RGBA',(1029,324));bg=art('Assets/Art/Menu/Task20/scene-jiggles.png').resize((1125,2001),R);patch.alpha_composite(bg.crop((48,1416,1077,1740)))
for i,(label,key) in enumerate([('500','milestone-medal-frame-locked'),('1.000','milestone-medal-badge-locked'),('2.000','milestone-medal-portrait-locked')]):
 a=lockedmedal if i==0 else art('Assets/Art/Menu/Task21/'+key+'.png');paste(patch,a,(26+i*111,3,58,60));text(patch,label,55+i*111,84,15,CREAM,True,100)
patch.paste(base.crop((48,1698,1077,1740)),(0,282))
compare(folder,'Jiggles · üç bağımsız madalyon',S+'375x667_jiggles.png',(16,472,343,108),patch,'Uzun çubuğun yerine Task21 yönündeki üç bağımsız ödül.315 sayısı500 altı olduğundan üçü de kilitli. İlk madalyon yeni; diğer ikisi onaylı Task21. Raf ve avatar yeniden çizilmedi. PM33 uzun barıyla farklı tercih; onay gerektirir.','JigglesScreen m13-progress-track/fill yerleşimi; mevcut Task21 milestone-medal-* ile birlikte')
# 113 rating standalone assets, number remains dynamic.
folder='113-ai-puan';normal=texture(folder,'rating-normal-@3x','chip',(48,44));selected=normal.copy();paste(selected,art('Assets/Art/Menu/Task21/check-badge.png'),(34,0,14,14));export(folder,'rating-selected-@3x',selected,'Seçili kutu: mevcut tikle renk dışı durum',source='imagegen chip + Task21/check-badge')
im=Image.new('RGBA',(1005,354));
for i in range(10):
 x=31+(i%5)*56;y=9+(i//5)*50;paste(im,selected if i==6 else normal,(x,y,48,44),False);text(im,str(i+1),x+24,y+22,17)
compare(folder,'Puan kutuları · iki sıra / seçili tik','docs/qa/2026-11-02-rating/a_kart.png',(20,337,335,118),im,'Güncel iki sıralı düzen referans alındı.48×44 pt görsel hedefi örnek; kod hedefi küçültülmemeli.7 seçili durum örneği; normal ve seçili PNG rakamsızdır.','Card.ChipRow ShapeView arka planı; yeni sprite anahtarı ayrı entegrasyon')
# Review HTML: all before/after pairs, exact pixel link, and all intro variants.
head='''<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task22 · AI malzeme adayları</title><style>body{margin:0;background:#0b2a36;color:#fff6d8;font:16px system-ui;line-height:1.6}main{max-width:1050px;margin:auto;padding:30px 20px}h1{font-size:30px}article{background:#143d48;border:1px solid #46717b;padding:22px;margin:28px 0;border-radius:20px}img{max-width:100%;height:auto;display:block;margin:auto}a{color:#b7ea43}.variants{display:grid;grid-template-columns:repeat(auto-fit,minmax(140px,1fr));gap:14px}.variants img{max-height:330px}summary{cursor:pointer}small{color:#c8dddd}nav{position:sticky;top:0;background:#0b2a36;padding:8px}label{display:inline-block;padding:8px}button{font:inherit;background:#79c800;color:#0b2a36;padding:10px 18px;border:0;border-radius:9px}textarea{width:100%;min-height:65px;font:inherit}</style><main><h1>Task22 ·101–113 AI malzeme adayları</h1><p>Onaysız taslaklar. Solda kaynak ekran; sağda aynı bağlamda öneri. Hiçbir aday oyuna bağlanmadı.1–22 numaralı ana çalışmayla birlikte veya ayrı seçilebilir.</p><p><a href="inceleme.html">Ana incelemeye dön</a> · <a href="../../../docs/art/DENETIM_T22_AI_EK.md">Bağımsız dört tur</a></p><nav><button onclick="document.querySelector('#secimler').value=Array.from(document.querySelectorAll('input:checked')).map(x=>x.value).join(', ')">Seçimleri listele</button></nav><textarea id="secimler" aria-label="Seçilen aday numaraları" readonly></textarea>'''
parts=[head]
for j in JOBS:
 f=j['folder'];n=f.split('-')[0];parts.append(f'<article id="aday-{n}"><a href="{f}/once-sonra.png"><img loading="lazy" src="{f}/once-sonra.png" alt="{html.escape(j["title"])} önce ve sonra"></a><p><label><input type="checkbox" value="{n}"> <b>{n} · {html.escape(j["title"])}</b></label></p><p>{html.escape(j["note"])}</p><small>Kaynak: {html.escape(j["source"])}</small><p><a href="{f}/NOT.md">Dosya / entegrasyon notu</a> · <a href="{f}/ekran-sonra-375.png">Tam bağlam</a></p>')
 if (OUT/f/'once-sonra-sure.png').exists():parts.append(f'<p><a href="{f}/once-sonra-sure.png">Süre kaybı ·3 Kasım önce/sonra</a></p>')
 variants=list((OUT/f).glob('onizleme-*.png'))
 if variants:parts.append('<details><summary>Varyantları aç</summary><div class="variants">'+''.join(f'<figure><img loading="lazy" src="{f}/{p.name}" alt="{p.stem}"><figcaption>{p.stem}</figcaption></figure>' for p in variants)+'</div></details>')
 parts.append('</article>')
parts.append('<p>Metinler önizleme içindir. Ayrı boş sprite ve manifestte@3x/slice bilgileri teslim edildi. Kaynak dosyalar ve onaylı PNGler değiştirilmedi.</p></main></html>')
(OUT/'inceleme-ai.html').write_text(''.join(parts),encoding='utf-8')
RECORDS=list({r['file']:r for r in RECORDS}.values())
(OUT/'ai-manifest.json').write_text(json.dumps(RECORDS,ensure_ascii=False,indent=2),encoding='utf-8')
(WORK/'ai-jobs.json').write_text(json.dumps(JOBS,ensure_ascii=False,indent=2),encoding='utf-8')
(WORK/'ai-text-layout.json').write_text(json.dumps(TEXT,ensure_ascii=False,indent=2),encoding='utf-8')
print('Families',len(JOBS),'candidate PNG',len(RECORDS))
