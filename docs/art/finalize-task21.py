from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,hashlib,math
import numpy as np
ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'Assets/Art/Menu/Task21';OLD=ROOT/'Assets/Art/Menu/Task20';WORK=ROOT/'docs/art/task21'
items=json.loads((OUT/'manifest.json').read_text(encoding='utf8'));runtime=[r for r in items if r['isRuntime']];layouts=json.loads((WORK/'preview-layout.json').read_text(encoding='utf8'))
contact=Image.new('RGB',(7*180,math.ceil(len(runtime)/7)*170),'#4D626C');d=ImageDraw.Draw(contact)
for i,r in enumerate(runtime):
 a=Image.open(OUT/r['file']).convert('RGBA');a.thumbnail((168,127));x=i%7*180;y=i//7*170;contact.paste(a,(x+(180-a.width)//2,y),a);d.text((x+3,y+132),r['file'][:26],fill='white')
p=OUT/'Preview/sprites-kontak.png';contact.save(p);items=[r for r in items if r['file']!='Preview/sprites-kontak.png'];items.append(dict(file=p.relative_to(OUT).as_posix(),use='Sprite inceleme paftası',width=contact.width,height=contact.height,sliceLTRB=[0,0,0,0],pivot=[.5,.5],scale=1,transparent=False,isRuntime=False,category='preview',state='reference',sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
for r in items:
 p=OUT/r['file'];a=Image.open(p);assert a.size==(r['width'],r['height']);assert hashlib.sha256(p.read_bytes()).hexdigest()==r['sha256']
 if r['isRuntime']:
  assert a.mode=='RGBA';l,t,rr,b=r['sliceLTRB'];assert l+rr<a.width and t+b<a.height
assert len(items)==len(list(OUT.rglob('*.png')))
for n,c in [('tab-slot',(16,112,127,255)),('tab-slot-selected',(26,154,174,255)),('tab-divider',(11,74,85,255))]:
 a=np.asarray(Image.open(OUT/(n+'.png')));assert np.all(a==c)
assert np.all(np.asarray(Image.open(OUT/'tabbar-bg.png'))[10:]==(16,112,127,255))
for n in ('avatar-ring','avatar-ring-selected'):
 a=Image.open(OUT/(n+'.png'));assert a.getchannel('A').getpixel((90,90))==0;assert a.getchannel('A').getpixel((90,6))>64
mask=np.asarray(Image.open(OUT/'avatar-ring-mask.png').getchannel('A'))
for c in ('blue','pink','red','green','purple'):
 a=Image.open(OUT/('avatar-'+c+'-round.png')).getchannel('A');b=a.getbbox();assert max(b[2]-b[0],b[3]-b[1])<=103;assert np.all(np.asarray(a)[mask==0]==0)
for name,screen in layouts.items():
 if name=='04-sekme-durumlari':continue
 slots=[l for l in screen['layers'] if l.get('sprite') in ('Task21/tab-slot.png','Task21/tab-slot-selected.png')];assert sorted(l['rectPt'][2] for l in slots)==[75,75,75,150]
 assert all(l['rectPt'][1]+l['rectPt'][3]==812 for l in slots)
 labels=[t for t in screen['text'] if t['y']>=726];assert len(labels)==1
 assert labels[0]['text']==('Jiggles' if name=='03-jiggles' else 'Ev')
 a=np.asarray(Image.open(OUT/'Preview'/(name+'.png')))
 allowed={(16,112,127),(26,154,174),(11,74,85)}
 assert set(map(tuple,a[2334:,:,:3].reshape(-1,3)))<=allowed
 label=labels[0];f=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),round(label['sizePt']*3))
 bottom=f.getbbox(label['text'],anchor='mm')[3]+label['y']*3+round(label['outlinePt']*3)+round(label['sizePt']*3*.08)
 assert bottom<2334,bottom
assert next(l for l in layouts['01-ev']['layers'] if l.get('sprite')=='Task21/profile-square-frame.png')['rectPt']==[8,44,90,90]
oldrows=json.loads((OLD/'manifest.json').read_text(encoding='utf8'))
for r in oldrows:assert hashlib.sha256((OLD/r['file']).read_bytes()).hexdigest()==r['sha256'],r['file']
# Font coverage uses actual glyph differences, including every Turkish character.
font=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),50)
missing=bytes(font.getmask(chr(0x10ffff)))
assert all(bytes(font.getmask(c))!=missing for c in 'ğşİıĞŞçÇöÖüÜ')
report=dict(runtimeSprites=len(runtime),previewPngs=len(items)-len(runtime),manifestDimensionsAndHashes=True,allPngsIndexed=True,exactFlatColors=True,slotRatio='2:1',barBottomPt=812,barTopPt=726,homeIndicatorClear=True,profileFramePt=90,profileTopPt=44,avatarContentRatio=103/132,avatarsClipped=True,selectedLabelsOnly=True,oldTask20HashesChecked=len(oldrows),turkishGlyphCoverage=True)
(WORK/'validation.json').write_text(json.dumps(report,indent=2),encoding='utf8');(OUT/'manifest.json').write_text(json.dumps(items,ensure_ascii=False,indent=2),encoding='utf8')
head='''# Task21 — Claude Code art handoff

42 yeni yazısız RGBA runtime PNG ve 6 inceleme PNG'si. Yalnız art üretildi; C#, sahne, prefab, importer ve font dosyaları değiştirilmedi. Task20'nin bütün manifest hash'leri yeniden doğrulandı. Royal Match görüntüleri yalnız yapı/ölçü referansıdır; onlardan piksel, ikon, karakter veya logo alınmadı. Yeni ikonlar ve çerçeveler built-in image_gen ile üretildi; düz renkli çubuk, daire maskeleri ve noktalı yol ölçülü raster geometrisidir. Prompt seti: `docs/art/task21/prompts.json`; kaynaklar aynı dizinin `sources/` altındadır.

## Import

Task20 biçimindeki `manifest.json` bir dizidir: gerçek piksel width/height, `sliceLTRB` SOL/ÜST/SAĞ/ALT kaynak piksel, normalize pivot, UI scale=3 ve pixelsPerUnit=3. Unity border sırası SOL/ALT/SAĞ/ÜST'tür; dönüştürün. Single, FullRect, alpha transparency, clamp/bilinear, sıkıştırmasız veya yüksek kaliteli RGBA, maxSize 2048; orijinal ölçüleri koruyun. Task21 mevcut Resources importer yoluna dahil değildir; manifest ayarlarını bağlayan taraf uygulamalıdır. isRuntime:false PNG'ler oyuna import edilmez. UGUI kullanılırsa Canvas PPU oranı ayrıca eşleştirilir.

## Öncelik ve yerleşim

`STIL_REHBERI_RM.md` Task21 metnindeki çelişkilerden üstündür: profil 64 değil 90pt; normal sekme ikonları çubuk yüksekliğinin yaklaşık %60'ı. Referans ekran 375×812pt, @3x 1125×2436. Kesin sprite/metin kutuları `docs/art/task21/preview-layout.json` içindedir; bütün önizlemeler bu katmanlarla yeniden kuruldu.

- Çubuk üstü y726, yüksekliği86pt, altı812: cihazın alt güvenli alanı DAHİL. Farklı cihazda ekran altına bağlayın, alt alanı 9-slice ile uzatın; çubuğu safe-area içine taşımayın. İlk10px tek altın şerit, geri kalanı düz #10707F. Yuvalar şeridin altından en alta uzar; seçili #1A9AAE, ayırıcı #0B4A55 3px. Kabarcık, dekoratif çerçeve veya kabartma yok.
- Dört sekme için ağırlıklar seçili2, diğerleri1: genişlikler150/75/75/75pt; seçime göre yerleri kayar. RM'nin beş sekmeli390/195px örneğini dört sekmeye aynen uygulamayın. Dört durum `Preview/04-sekme-durumlari.png` içinde. Geçişte yuva genişliklerini ve ikon merkezlerini birlikte enterpole edin.
- Normal ikon147px=49pt, seçili184px≈61.33pt (%25.17 büyüme); selected-large yeni çizim değil aynı silüetin yüksek çözünürlüklü varyantıdır. Normal üst729, seçili üst693; seçili etiket merkezi765pt. Yalnız seçili etiket yazılır. 778–812pt ev göstergesi alanında ikon/metin yoktur. tab-badge boş kırmızı rozettir, gerekirse sayı ayrı yazılır.
- Profil dış kutu [8,44,90,90]pt; y44 durum çubuğunun hemen altıdır. Avatar, ayrı koyu isim plaketi, sayaç ve ayar tuşu ortak merkez y89. Portre48pt ve daire53pt, çerçeve altındadır; portre çerçeveye değmez. İsim plaketin içinde, kare içinde yazmaz. Sayaç ikon46pt, krem hap30pt; ikon soldan taşar, sayı sağdaki boş alana yazılır. Profil çerçevesinin merkezi şeffaftır, portrait üzerine çizilir.
- Ana yeşil CTA genişlik206.25pt (%55), kutusu [84.375,613,206.25,90]; çubuğa23pt (69px) boşluk. Yazı krem #FFF6D8, koyu yeşil #1E4D05 kontur ve aşağı gölge. Sekme başlıklarında pankart yoktur.
- Avatar seçimi: bütün katmanlar180px ortak tuval. Halka dış180, iç132px; avatar içerik sınırı103px (%78.03). avatar-ring-mask beyaz alfa dairedir. Disk→maskeli avatar-round→halka→seçiliyse ayrı check-badge sırasıyla çizilir. Tik48px, halkanın sağ altında; avatar sprite'ını yeniden180px içerik boyuna büyütmeyin. Normal turkuaz ve seçili altın halka merkezleri aynıdır.
- Jiggles500/1.000/2.000: ayrı madalyon merkezleri67/187/307pt, y615; arada70pt kısa noktalar, uzun ray yok. 1.248 örneğinde ilk ikisi normal, 2.000 soluk+kilitli. Eşik ve ödül isimleri oyun metnidir. Kilitli varyantta kilit resme dahildir, tekrar kilit eklemeyin.
- Ayarlar paneli krem ağırlıklı; her satır ayrı krem9-slice karo. Soldaki dark-turquoise disc ve merkez sembol ayrı katmanlar. Gizlilik/Bize yaz iki54pt kare tuştur, altlarında bağımsız küçük etiketler; yatay uzun tuş yok. settings-icon-btn-pressed aynı tuvalde aşağı offset + Y daralmasıdır; ikon da aynı basma dönüşümünü izlemelidir. Toggle ve dil seçiciler Task20'den gelir. Kırmızı X panelin sağ üst köşesinden taşar.

## Bağımlılıklar ve metin

Değişmeyen sahneler, oyuncu portreleri/aktörler, raf, tablo çerçevesi, yeşil CTA, ayar dişlisi, toggle, dil seçici ve X Task20'dendir; kopya dosya üretilmedi. Kullanılan adlar ve SHA-256 `docs/art/task21/dependencies.json` içindedir. Gerçek Ev tablosu `Assets/Art/Gameplay/Task15/painting-test-01.png`; Task20 hero-picture-mask ile kırpılır. Eski nav-bar/slot/bubble, milestone-rail, yatay settings tuşları ve avatar ringleri yerine Task21 parçalarını kullanın.

Önizlemede mevcut OFL Nunito Black kullanıldı, Türkçe ğşİıĞŞçÇöÖüÜ glifleri doğrulandı; font dosyasına yazılmadı. Bütün dinamik yazılar/etiketler oyunda çizilir; runtime PNG'lere gömülü değildir. Sayaç sayıları lacivert, kontur/gölgesiz; başlık ve seçili etiketler krem, lacivert kontur ve aşağı gölge. Projedeki nihai font seçimi PM27 akışında bağlanır.

## Dosyalar

'''
table='| PNG | px | slice L/T/R/B | kullanım |\n|---|---:|---|---|\n'
for r in runtime:table+=f"| {r['file']} | {r['width']}×{r['height']} | {r['sliceLTRB']} | {r['use']} |\n"
(OUT/'HANDOFF.md').write_text(head+table+'\nDoğrulama: `docs/art/task21/validation.json`. Önizleme: `onizleme.png`; tam boy üç ekran ve dört sekme durumu `Preview/` içinde.\n',encoding='utf8')
print(json.dumps(report))
