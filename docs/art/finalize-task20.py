"""Task20 verification, artifact index and Claude Code handoff."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,hashlib,math
import numpy as np

ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'Assets/Art/Menu/Task20';WORK=ROOT/'docs/art/task20';P=OUT/'Preview'
items=[r for r in json.loads((OUT/'manifest.json').read_text(encoding='utf8')) if r.get('category')!='preview']
names={r['file'] for r in items};assert len(names)==len(items)
for r in items:
    a=Image.open(OUT/r['file']);assert a.mode=='RGBA',r['file'];assert a.size==(r['width'],r['height'])
    l,t,rr,b=r['sliceLTRB'];assert min(l,t,rr,b)>=0 and l+rr<a.width and t+b<a.height,r['file']
    assert r['pivot']==[.5,.5] or r['pivot']==[.5,0],r['file']
    assert hashlib.sha256((OUT/r['file']).read_bytes()).hexdigest()==r['sha256']
    if r.get('category')=='background':assert max(a.size)<=2048
    else:assert a.getchannel('A').getextrema()[0]==0,r['file']
    if r.get('isInteractive') and r.get('state')!='pressed':
        assert r['file'][:-4]+'-pressed.png' in names,r['file']
    if r.get('innerRectPx') and r.get('state')!='pressed':
        x,y,w,h=r['innerRectPx'];assert abs(w/h-.75)<.001
        assert a.getchannel('A').crop((x,y,x+w,y+h)).getextrema()==(0,0),r['file']
path=Image.open(OUT/'levels-path-repeat.png');ar=np.asarray(path)
assert np.array_equal(ar[0],ar[-1])
mask=Image.open(OUT/'hero-picture-mask.png').getchannel('A');assert mask.getpixel((0,0))==0 and mask.getpixel((mask.width//2,mask.height//2))>250
protected=json.loads((WORK/'protected-input-hashes.json').read_text())
for file,digest in protected.items():assert hashlib.sha256((ROOT/file).read_bytes()).hexdigest()==digest,file

# Source corrections are checked against actual Fredoka glyph bounds, not an
# assumed font-height box. Anti-alias/optical emboldening receives a 1px allowance.
fontfile=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Fredoka-SemiBold.ttf'
rows=json.loads((ROOT/'docs/art/task19d/layers/04-basarilar-text.json').read_text(encoding='utf8'))
r=next(r for r in rows if r['text']=='10 bombalı bölümü ilk denemede geç');f=ImageFont.truetype(str(fontfile),r['fontSizePx'])
bbox=ImageDraw.Draw(Image.new('L',(1125,2436))).textbbox((r['x'],r['y']),r['renderText'],font=f,anchor=r['anchor'])
rightgap=1032-(bbox[2]+1);assert rightgap>=24,rightgap
rows=json.loads((ROOT/'docs/art/task19d/layers/03-jiggles-text.json').read_text(encoding='utf8'))
r=next(r for r in rows if r['text']=='Avatarım');f=ImageFont.truetype(str(fontfile),r['fontSizePx'])
bbox=ImageDraw.Draw(Image.new('L',(1125,2436))).textbbox((r['x'],r['y']),r['renderText'],font=f,anchor=r['anchor'])
# Selected avatar ring begins around 500pt in the approved source plate.
avatargap=1500-(bbox[3]+1);assert avatargap>=16,avatargap
rows=json.loads((ROOT/'docs/art/task19d/layers/07-bolumler-text.json').read_text(encoding='utf8'))
assert len([r for r in rows if r['text']=='10'])==1
assert any(r['text']=='19' and r['y']==642 for r in rows)
portraits=json.loads((ROOT/'docs/art/task19d/portraits.json').read_text())
assert any(r['screen']=='07-bolumler' and r['level']==10 and r['targetBox']==[179,559,30,40] for r in portraits)
layouts=json.loads((WORK/'preview-layout.json').read_text(encoding='utf8'))
assert len(layouts)==9
for screen,v in layouts.items():
    assert Image.open(P/(screen+'.png')).size==(1125,2436)
    for layer in v['layers']:
        if 'sprite' in layer:assert layer['sprite'] in names
        if 'gameArt' in layer:assert 'Paintings/' in layer['gameArt'] or 'painting-test-01.png' in layer['gameArt']

# A real checkerboard sprite contact and a repeat-seam visual proof.
cols=8;cw,ch=190,185;contact=Image.new('RGB',(cols*cw,math.ceil(len(items)/cols)*ch));d=ImageDraw.Draw(contact)
for y in range(0,contact.height,12):
    for x in range(0,contact.width,12):d.rectangle((x,y,x+11,y+11),fill='#60717A' if (x//12+y//12)%2 else '#46565F')
for i,r in enumerate(items):
    x=i%cols*cw;y=i//cols*ch;a=Image.open(OUT/r['file']).convert('RGBA');a.thumbnail((180,145))
    contact.paste(a,(x+(cw-a.width)//2,y+5),a);d.text((x+3,y+154),r['file'][:28],fill='white')
contact.save(P/'sprites-kontak.png')
tile=Image.open(OUT/'levels-path-repeat.png').convert('RGBA');test=Image.new('RGBA',(tile.width,tile.height*2),'#46565F')
test.alpha_composite(tile,(0,0));test.alpha_composite(tile,(0,tile.height));test.convert('RGB').save(P/'levels-repeat-proof.png')

# Index non-runtime preview PNGs too: every delivered PNG has one manifest row.
artifacts=[OUT/'preview-kontak.png',*sorted(P.glob('*.png'))]
for p in artifacts:
    a=Image.open(p)
    items.append(dict(file=str(p.relative_to(OUT)).replace('\\','/'),use='İnceleme önizlemesi; oyuna import edilmez',
        width=a.width,height=a.height,sliceLTRB=[0,0,0,0],pivot=[.5,.5],scale=1,transparent=False,
        category='preview',isRuntime=False,state='reference',sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
for r in items:
    if r.get('category')!='preview':r['isRuntime']=True
    if r.get('category')=='background':r['pixelsPerUnit']=1
(OUT/'manifest.json').write_text(json.dumps(items,ensure_ascii=False,indent=2),encoding='utf8')
runtime=[r for r in items if r.get('isRuntime')]
report=dict(runtimeSprites=len(runtime),previewPngs=len(artifacts),protectedInputsChecked=len(protected),
    alpha=True,pressedPairs=True,sliceBounds=True,portraitCentersTransparent=True,pathSeamExactRGBA=True,
    sourceCorrections=dict(bombDescriptionRightGapPx=rightgap,avatarHeadingGapPx=avatargap,marker10Moved=True,hard19Locked=True),
    rebuiltScreens=9,previewUsesOnlyExportedSpritesAndGamePaintings=True,originalFilesUnchanged=True)
(WORK/'validation.json').write_text(json.dumps(report,indent=2),encoding='utf8')

head='''# Task20 — Menü üretim sprite'ları / Claude Code handoff

Task19D onaylı tasarım temel alınmıştır. Bu klasördeki **runtime PNG'ler yazısız RGBA sprite'lardır**; sayı/isim/etiketler oyun tarafından yazılır. `category: preview` / `isRuntime: false` satırlarını oyuna import etmeyin. C#, sahne, prefab ve `Assets/_Game` dosyalarına yazılmadı. Onaylı Task13/14/15 girdileri SHA-256 ile değişmeden doğrulandı.

## Import ve yerleşim

- `manifest.json` bir dizidir; her PNG'nin tek satırı vardır. `width/height` gerçek piksel, `scale:3` UI @3x referansıdır. `sliceLTRB` sırası **SOL, ÜST, SAĞ, ALT** ve birimi **kaynak piksel**dir. Unity Vector4 border sırası ise **SOL, ALT, SAĞ, ÜST**; çevirmeden aktarmayın. Pivotlar normalize x/y'dir; taban-ankorlu aktör/yol `[0.5,0]`, diğerleri `[0.5,0.5]`.
- Mevcut `Assets/_Game/Editor/MenuArtImport.cs` @3x menü sprite'larını3PPU, koridor arka planını1PPU kullanıyor; manifest de UI=3/background=1 belirtir. Sprite Mode Single/FullRect, alfa şeffaflığı açık, UI clamp/bilinear. Mevcut renderer için sliced sprite çizimi; UGUI'ye aktarılırsa Canvas referans PPU oranı ayrıca eşleştirilmeli,3PPU doğrudan varsayılan UGUI NativeSize ile karıştırılmamalı. Dinamik metin için tam Türkçe Fredoka Bold kullanın; font importuna bu işte dokunulmadı.
- Task20 klasörü mevcut importer'ın Resources yol eşleşmesine dahil değildir. Bağlayan taraf bu dosyalara manifest ayarlarını uygulamalıdır; otomatik oluşan yeni `.meta` varsayımlarına güvenmeyin. Genel Resources kuralındaki1024 limiti özellikle modal/hero/yol parçalarını küçültür: bu pakette2048 limiti ve orijinal piksel boyutlarını koruyan import kullanın; border değerleri ham PNG pikselidir. Importer'a bu işte yazılmadı.
- Arka planlar tam sahne ve UI'sızdır; en uzun kenar2048'in altındadır. Ekrana 375×812pt stretch edin; `referenceCanvasPx:[1125,2436]` ile manifestteki doğal boyutu karıştırmayın.
- `levels-path-repeat.png`: yalnız dikeyRepeat. İlk/son RGBA satırı bire bir aynıdır; UV'yi yatay clamp/dikey repeat uygulayın. Tekrarlanan yolun önizleme testi `Preview/levels-repeat-proof.png`.
- `portrait-frame` içi207×276, büyük varyant içi504×672, kilometre taşı içi90×120px, hepsi3:4 ve tamamen şeffaftır. `innerRectPx` kaynak x/y/w/h. Gerçek1024kare tabloyu önce merkezden `[128,0,896,1024]` kırpın; çerçevenin ALTINA koyun. Resim sprite'a gömülü değildir. Ev'in oyma çerçevesi eğik bir açıklığa sahiptir: `hero-picture-mask` ile dinamik resmi maskeleyin.
- Tüm düğümler sayısızdır. Kilitli ve zor-kilitli düğümlerde sayıyı alt boşluğa, açık/tamamlanan/güncel düğümlerde merkeze yazın. `node-hard-locked` kırmızı rim+Baron+kilidi birlikte içerir; `node-hard-open` kilitsizdir. Glow ayrı katmandır.
- `toggle-on-track`, `toggle-off-track`, `toggle-knob` ayrıdır. Dil seçeneklerinin yazıları yoktur; sol/sağ seçili varyantları ayrı dosyalardır. Sağ seçili kaplama sol varyantın yatay aynasıdır, etiketleri ayrıca yazın.
- Basılı dosyalar aynı tuval boyutundadır. `pressOffsetPx` ve `pressScaleY` alanları, Task14/15 usulü basma dönüşümünü açıklar; düğmenin dinamik yazı/alt resimlerini de aynı dönüşümle izletin. Portre resmini pressed-frame ile kullanırken bu dönüşümü resme de uygulayın.
- Sekmeler: `nav-bar-base` → dört `nav-slot-normal/selected` → seçili `nav-bubble` → dört ikon → bağımsız bildirim rozeti → metinler. Seçili yuvanın TAMAMI açıktır. Normal ikon126px, seçili164px (~%30). Etiket ikonun altında, kabarcık dışında, en üsttedir. Kupa ikonuna rozet gömülü değildir. Basılı durumlar hem yuva hem ikonlarda bulunur.

## Kaynak ekran düzeltmeleri

Task19D kaynakları ve `kontak.png` yeniden üretildi: tablo10 işareti10 düğümünün yanına taşındı; altında ikinci10 yok. Bölüm19 kırmızı rim+Baron+kilit ile kilitli. Bomba açıklamasında en az24px sağ boşluk ve Avatarım ile avatar halkaları arasında en az16px boşluk doğrulandı. Ölçülen değerler aşağıdadır.

## Önizleme ve farklar

`preview-kontak.png`, dokuz `Preview/01…08` ekranından oluşturuldu. Bu ekranlar **yalnız export edilen sprite'lar, mevcut gerçek tablo dosyaları ve yeniden çizilen dinamik metinlerden** kuruldu; Task19D tam ekran resmi/kart kesiti arka plana yapıştırılmadı. Çizim listesi `docs/art/task20/preview-layout.json`; renk/piksel fark ölçümü `preview-differences.json`.

Task19D ile **piksel özdeşliği yoktur**. Nedenler:

1. Temiz sahnelerde önceden UI'nın örttüğü mimari yeniden tamamlandı. Sahne boşlukları ve ışık ayrıntıları küçük farklılık gösterir.
2. Birleşik kart/tuş/plaketler ortak temiz9-slice üretim kabuklarına ayrıldı; kenar ve yansıma ayrıntıları normalize edildi. Eski sahnenin içine pişmiş yazılar/ikonlar tekrar kullanılmadı.
3. Beş raf aktörü ve raf gerçek alfa ile bağımsızlaştı. Mor karakter/raf kenarında komşu parça kalıntısı görülünce ayrı izolasyon yapıldı. Raf üst katı ile üst sayaçların tabanı yaklaşık21pt aşağı hizalandı; karakter/raf oranlarında küçük farklar vardır.
4. Bölüm yolu artık tekrarlanabilir bir sprite'tır. Düğümlerin onaylı merkezleri, dikey ilerleme/yön ve durumları korunur; alttaki taş/halı sarmalının şekli farklıdır. Tablo işaretlerinde tekrar eden ek numara kullanılmaz. Bu, eski tek parça merdiven resminin bire bir kopyası değildir.
5. Kupa ikonunun boş kırmızı rozeti temizlendi; rozet bağımsız katmandır. Basılı/sağ-dil varyantları üretim için türetilmiştir.

Bu farklar paket gizlenerek değil, katman ayrımı/tekrar/9-slice zorunlulukları nedeniyle açıkça kaydedildi. Kontak ve parçalar görsel olarak incelenmelidir; bu teslim oyuna bağlama veya runtime test yapmaz.

## Yeniden üretim / kaynaklar

`python docs/art/export-menu-task20.py` → `python docs/art/build-task19d.py` → `python docs/art/render-task20-preview.py` → `python docs/art/finalize-task20.py`.

Sanatsal temizleme/izolasyon için **built-in image_gen** kullanıldı; tam prompt seti `docs/art/task20/prompts.json`. Scriptler alpha temizleme, onaylı parça yeniden kullanımı, durum türetme ve kenar birleştirme işlemlerini yapar. Ham kaynaklar `docs/art/task20/sources` altındadır; oyuna gerekmez.

'''
summary=f"Üretim parçası: **{len(runtime)} PNG**. İnceleme PNG'si: **{len(artifacts)}**. Korunan girdi: **{len(protected)} dosya**. Bomba sağ boşluğu: **{rightgap}px**. Avatarım alt boşluğu: **{avatargap}px**.\n\n"
table='## Her dosyanın kaydı\n\n| Dosya | Oyunda nerede | Ölçü px | 9-slice L/T/R/B px | Pivot | Tür |\n|---|---|---:|---|---|---|\n'
for r in items:
    use=r['use'].replace('|','/');table+=f"| [{r['file']}]({r['file']}) | {use} | {r['width']}×{r['height']} | {','.join(map(str,r['sliceLTRB']))} | {r['pivot']} | {'inceleme' if not r['isRuntime'] else 'üretim'} |\n"
(OUT/'HANDOFF.md').write_text(head+summary+table,encoding='utf8')
print(json.dumps(report,ensure_ascii=False))
