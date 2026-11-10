from t23_common import *
T='docs/qa/2026-11-03-tam/';F='docs/qa/2026-11-02-ftue/';Q='docs/qa/2026-11-02-rating/'
def generated(key):return art('Assets/Art/Redesign_T23/KAYNAKLAR/'+key+'.png')
def shared(t):
 for kind in ['primary','secondary']:
  for state in ['', '-pressed']:
   t.use('button-'+kind+state,'Assets/Art/Gameplay/Task15/btn-'+kind+'-v2'+state+'.png',(228,112),role='Korunan onaylı tuş;684:336 en-boy oranı',hit=[128,50])
def extent(source,h):
 a=art(source);return (20,(a.height/a.width*375-h)/2,335,h)
def rating(t,im,y,selected=7):
 v=t.variant;w=48 if v=='A' else 50;gap=8 if v=='A' else 7;left=(335-(5*w+4*gap))/2
 tile=t.use('rating-normal','Assets/Art/Redesign_T22/113-ai-puan/rating-normal-@3x.png',(w,44),role='101 ailesinde rakamsız puan kutusu',hit=[w,44]);check=art('Assets/Art/Menu/Task21/check-badge.png');sel=tile.copy();paste(sel,check,(w-14,0,14,14));t.save('rating-selected',sel,'Seçili durum; renk dışı tik',source='T22 rating + Task21 check',hit=[w,44])
 for i in range(10):
  x=left+(i%5)*(w+gap);yy=y+(i//5)*50;paste(im,sel if i+1==selected else tile,(x,yy,w,44),True);t.text.append(txt(im,str(i+1),x+w/2,yy+22,18 if v=='B' else 17,maxw=w-8))
 return y+94
def win(v):
 t=Trial('02-kazanma',v,'Tahta boşaldı!',Q+'a_kart.png','Screens.Result / Card','Task14/card-panel.png → popup-card');im=t.panel(600);shared(t)
 for i in range(3):paste(im,art('Assets/Art/Gameplay/Task14/star-full.png'),(108+i*42,79,35,35))
 t.text.append(txt(im,'108 saniye kaldı · 0 takılma',167.5,137,14 if v=='A' else 15,maxw=292));t.button(im,'Sonraki bölüm',167.5,165 if v=='A' else 160,198 if v=='A' else 204)
 t.text.append(txt(im,'Ne kadar zordu?',167.5,294,16,maxw=285));t.text.append(txt(im,'1 çok kolay · 10 çok zor',167.5,317,13,maxw=285));rating(t,im,337)
 t.button(im,'Tekrar oyna',91,453,128,False);t.button(im,'Menü',244,453,128,False);t.text.append(txt(im,'Puanlamayı geç',167.5,558,14,maxw=285))
 t.notes+=['Üç yıldız, ana yeşil Sonraki bölüm, isteğe bağlı1–10 ve Puanlamayı geç korunur.','A daha küçük gövde yazısı/198pt ana tuş; B15pt istatistik/204pt ana tuş ve50pt puan kutuları.'];return t.finish(im,extent(t.source,600))
def loss(v,key):
 bomb=key=='bomb';folder='03-kayip-bomba' if bomb else '04-kayip-sure';title='Bomba patladı' if bomb else 'Süre doldu';src=Q+'c_kayip_kart.png' if bomb else T+'375x667_06_kaybetme.png';t=Trial(folder,v,title,src,'Screens.Result / Card','popup-card + loss-reason-'+key);im=t.panel(620);shared(t)
 reason=t.use('loss-reason','Assets/Art/Gameplay/Task14/loss-reason-'+key+'.png',(66,66),role='Onaylı kayıp nedeni korunur');paste(im,reason,(134.5,82,66,66))
 if bomb:t.text.append(txt(im,'0',167.5,118,22,maxw=40))
 copy=['Önce bombanın yolunu aç.','Çekiç onu tek hamlede söker.'] if bomb else ['Tahtada parçalar kaldı.','Saat sana20 saniye kazandırır.']
 if not bomb:copy[1]='Saat sana 20 saniye kazandırır.'
 t.text+=lines(im,copy,167.5,165,14 if v=='A' else 15,gap=22)
 t.button(im,'Tekrar dene',167.5,210,184 if v=='A' else 190);t.text.append(txt(im,'Ne kadar zordu?',167.5,330,15,maxw=285));t.text.append(txt(im,'1 çok kolay · 10 çok zor',167.5,353,13,maxw=285));rating(t,im,373)
 t.button(im,'Menü',167.5,483,150,False);t.text.append(txt(im,'Puanlamayı geç',167.5,588,14,maxw=285));t.notes+=['Soru açık durumda bile yeşil Tekrar dene korunur. Soru akışı kaldırılmaz.','Soru kapalı durumda9-slice panel kısalır; metin ve eylem sıralaması korunur.'];return t.finish(im,extent(src,620))
def pause(v):
 t=Trial('05-durdurma',v,'Durduruldu',T+'375x667_04_pause.png','Screens.Pause / Card','popup-card + btn2-primary / btn2-secondary');im=t.panel(430);shared(t)
 positions=[(90,198),(200,198),(310,198)] if v=='A' else [(86,210),(206,190),(316,190)]
 for (s,primary),(y,w) in zip([('Devam',True),('Yeniden başla',False),('Ana menü',False)],positions):t.button(im,s,167.5,y,w,primary)
 t.notes+=['A eşit198pt tuşlar; B ana Devam210pt, ikinciller190pt. Yeni metin veya ekran eklenmedi.'];return t.finish(im,extent(t.source,430))
def rating_family(v):
 t=Trial('06-puan-kutulari',v,'Puan kutuları',Q+'a_kart.png','Card.ChipRow','ShapeView chip yüzeyi → rating-normal/selected');im=Image.new('RGBA',sizept((335,118)));rating(t,im,10);t.notes+=['A48×44, B50×44pt. İki sıra5 sütun; normal ve seçili yüzler rakamsız.','Rakam18pt (B), seçili durum renge bağlı kalmaz; tik14pt.'];return t.finish(im,(20,337,335,118))
INTROS=[
('07-tanitim-civi','Çivili parça','nail',['Diğer parçaları çıkar.','Sayaç sıfır olunca çivi açılır.'],F+'14_b3_kart.png'),
('08-tanitim-bomba','Bombalı parça','bomb',['Fitil bitmeden bombayı çıkar.','Önce onun yolunu aç.'],F+'37_b6_kart.png'),
('09-tanitim-zincir','Zincirli parçalar','chain',['İki parça birlikte çıkar.','İkisinin yolunu da aç.'],'docs/qa/2026-11-02-spot/375x667_b10_1.png'),
('10-tanitim-duvar','Duvar','wall',['Parça duvardan geçemez.','Açık kenardan yol bul.'],'docs/qa/2026-11-02-spot/375x667_b14_1.png'),
('11-booster-makas','Makas','scissors',['Önce makası seç.','Sonra kesilecek ekleme dokun.'],F+'34_b5_kart.png'),
('12-booster-degnek','Sihirli değnek','wand',['Çıkıntıları yeniden dizer.','Resim aynı kalır, kilitler değişir.'],'docs/qa/2026-11-02-booster/b9_1_tus.png'),
('13-booster-cekic','Çekiç','hammer',['Önce çekici seç.','Çıkaracağın parçaya dokun.'],'docs/qa/2026-11-02-booster/b23_1_tus.png'),
('14-booster-saat','Saat','clock',['Geri sayıma20 saniye ekler.','Kullanmadan önce onay ver.'],'docs/qa/2026-11-02-booster/b15_4_onay.png'),
('15-zor-bolum','Zor bölüm','hard',['Bu bölüm biraz daha zorlu.','İlk hamleden önce yolları incele.'],T+'375x667_02_baslangic.png')]
def round_scene(a,pt):
 a=ImageOps.fit(a,sizept(pt),R);mask=Image.new('L',a.size,0);ImageDraw.Draw(mask).rounded_rectangle((0,0,a.width-1,a.height-1),radius=24,fill=255);a.putalpha(mask);return a
def intro(v,row):
 folder,title,key,copy,source=row;copy=[s.replace('sayıma20','sayıma 20') for s in copy];t=Trial(folder,v,title,source,'Screens.Intro / Screens.StartCard','intro-'+key+' / popup-card; ortak sahne alanı');im=t.panel(520);shared(t)
 if key in ['nail','bomb','chain','wall']:
  source_art=f'Assets/Art/ObstacleIntros/Option{v}/{key}-888x592.png';a=art(source_art)
 else:source_art='Assets/Art/Redesign_T23/KAYNAKLAR/scene_'+key+'_'+v+'.png';a=art(source_art)
 scene_pt=(288,192) if v=='A' else (296,198);scene=round_scene(a,scene_pt);t.save('character-scene',scene,'Tek karakterli öğretim sahnesi',source=source_art);paste(im,scene,((335-scene_pt[0])/2,79,scene_pt[0],scene_pt[1]),True)
 t.text+=lines(im,copy,167.5,306 if v=='A' else 312,14 if v=='A' else 15,gap=22)
 t.button(im,'Başla' if key=='hard' else 'Anladım',167.5,366 if v=='A' else 370,212 if v=='A' else 218)
 t.notes+=['Üstte tek karakterli sahne, altında yalnız iki kısa satır, altta tek yeşil tuş. Turuncu ikinci mesaj ve küçük ikon kutusu yok.','Baron engellerde, Jiggle booster sahnesinde; mevcut iyi Baron resimleri korunmuştur.','A288×192/14pt, B296×198/15pt ve daha geniş ana eylem karşılaştırması.']
 if key in ['chain','wall','wand','hammer','clock']:t.notes+=['Önce görüntüsü ayrı tanıtım modalı değildir; en yeni ilgili öğretim/onay bağlamıdır. Yeni sahne önerisi bu sınırla incelenmelidir.']
 if key=='hard':t.notes+=['PM36:19/23/25/30/35/36/38 tek Baron uyarısı.23te ardından çekiç eğitimi ayrı el adımı olabilir; ikinci Baron kartı eklenmez.']
 return t.finish(im,extent(source,520),{'characterCount':1,'messageLines':copy,'sceneKey':key})
def name(v):
 t=Trial('17-isim',v,'Hoş geldin!',F+'02_isim.png','WebGLTemplates/ReverseSolver #rs-name','name-card/input/rs-name-go;107 kabul yönü');im=t.panel(390);shared(t);t.text+=lines(im,['Adını yaz, sonuçların','bu isimle kaydedilsin.'],167.5,110,15 if v=='A' else 16)
 field=t.use('name-input','Assets/Art/Menu/Task20/counter-pill.png',(277,58),b=(54,30,54,30),role='Düzenlenebilir gerçek input için boş krem yüzey');paste(im,field,(29,168,277,58),True);t.text.append(txt(im,'Adın',167.5,197,17 if v=='A' else 18,maxw=240));t.button(im,'Başla',167.5,260,212 if v=='A' else 218);t.notes+=['Playtest sürümü yok. Başla yeşil; input gerçek HTML alanı olarak kalmalı, PNG metni olmaz.','Klavye açık görünürlük canlı cihaz testi gerektirir.'];return t.finish(im,extent(t.source,390))
def profile(v):
 t=Trial('18-profil-sorusu',v,'Seni tanıyalım',F+'03_profil_sorusu.png','Profile survey / Card','popup-card / profil anketi');im=t.panel(490);shared(t);t.text+=lines(im,['Bu tür bulmaca oyunlarını','ne sıklıkla oynarsın?'],167.5,110,15)
 for s,y in zip(['Hiç','Ara sıra','Sık'],[168,270,372]):t.button(im,s,167.5,y,190 if v=='A' else 198,False)
 t.notes+=['107 isim ekranıyla aynı yumuşak turkuaz/krem aile. Yanıtlar eşit turkuaz seçim; hiçbiri ana yeşil eylem değildir.'];return t.finish(im,extent(t.source,490))
def complete(v):
 source=T+'tr_390x844_won40_1_ev.png';t=Trial('26-koleksiyon-tamam',v,'Koleksiyon tamamlandı',source,'Screens.Result isLast / CollectionScreen','40.bölüm bitiş kartı; yeni özel terminal tasarımı');im=t.panel(590);shared(t)
 path=ROOT/('Assets/Art/Redesign_T23/KAYNAKLAR/scene_complete_'+v+'.png');assert path.exists(),'Complete celebration generation not yet ready'
 hero=contain(Image.open(path).convert('RGBA'),sizept((280,230)));canvas=Image.new('RGBA',sizept((280,230)));canvas.alpha_composite(hero,((canvas.width-hero.width)//2,(canvas.height-hero.height)//2));t.save('collection-celebration',canvas,'Tek Jiggle ve tamamlanan tablo; yazısız kutlama',source=str(path.relative_to(ROOT)));paste(im,canvas,(27.5,82,280,230),True)
 counter=t.use('completion-counter','Assets/Art/Menu/Task21/counter-pill-v2.png',(166,45),b=(54,33,54,33),role='40/40 dinamik sayı alanı');paste(im,counter,(84.5,327,166,45),True);t.text.append(txt(im,'40 / 40',167.5,350,25,maxw=142));t.text+=lines(im,['Bütün tablolar galerinde.','Yeni bir şaheser kurtardın!'],167.5,402,15);t.button(im,'Koleksiyon',167.5,461,208)
 t.notes+=['Önce kaynağı won40 menü durumudur; gerçek terminal kartı karesi bulunmadı. Mevcut olmayan ekran görülmüş gibi gösterilmez.','Ana eylem mevcut Koleksiyon sekmesine götürür; yeni sahne/ödül/sayaç sistemi önerilmez. Kutlama tek sefer koşulu kod listesinde.','A sarılan Jiggle, B tabloyu sunan Jiggle; ikisi de tek karakterli yeni imagegen denemesi.'];return t.finish(im,extent(source,590),{'characterCount':1,'evidenceLimitation':'Before is won40 home state, not actual terminal card'})
def run(include_complete=True):
 records=[]
 for v in ['A','B']:
  for func in [win,lambda v:loss(v,'bomb'),lambda v:loss(v,'time'),pause,rating_family,name,profile]:records.append(func(v))
  for row in INTROS:records.append(intro(v,row))
  if include_complete:records.append(complete(v))
 (WORK/'card-trials.json').write_text(json.dumps(records,ensure_ascii=False,indent=2),encoding='utf-8')
 paths=[OUT/'DENEMELER'/v/r['folder']/'onizleme.png' for r in records if r['variant']=='A' for v in ['A','B']]
 for page in range((len(paths)+7)//8):contact(paths[page*8:(page+1)*8],WORK/f'kartlar-A-B-{page+1}.jpg',cols=4,cw=290,ch=580)
 print(len(records),'card trials built')
if __name__=='__main__':
 import sys
 run('--without-complete' not in sys.argv)
