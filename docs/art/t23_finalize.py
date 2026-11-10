from t23_select import *
import html

CHOICES={
'01-bolumler':('B','20pt numara ve sağ alt 20×22pt kilit küçük ekranda daha açık. Güncel altın, biten turkuaz, kapalı koyu korunuyor.'),
'02-kazanma':('B','15pt istatistik, 50pt kutular ve daha geniş ana eylem A’dan daha okunur; üç yıldız ve ikincil eylemler dengeli.'),
'03-kayip-bomba':('B','15pt iki satır açıklama ve 190pt tekrar tuşu A’dan rahat okunuyor; puanlama alanı taşmıyor.'),
'04-kayip-sure':('B','15pt neden açıklaması ve daha geniş tekrar tuşu tercih edildi. İki satır ve tüm eylemler kart içinde.'),
'05-durdurma':('A','Eşit 198pt tuşların hizası ve dikey aralığı daha sakin. Yeşil Devam zaten yeterli öncelik sağlıyor.'),
'06-puan-kutulari':('B','50×44pt yüz ve 18pt rakamlar A’nın 48pt/17pt setinden okunur. Seçili tik korunuyor.'),
'07-tanitim-civi':('A','A çivinin yolu kilitlediğini tahta bütünüyle gösteriyor. B yakın planı daha sıcak olsa da mekanizma bağlamını daraltıyor.'),
'08-tanitim-bomba':('A','A’da bomba karakterin elinde tek odak; fitil ve gövde ayrı okunuyor. B’de iki el ve tahta ön planı kalabalıklaştırıyor.'),
'09-tanitim-zincir':('B','B’de iki parça arasındaki zincir ve bağlantılar yakın planda daha net. A’da karakterin çekiş hareketi zincirin önüne geçiyor.'),
'10-tanitim-duvar':('A','A sakin geniş görünüşte duvar ve iki yanındaki yolları birlikte anlatıyor. B’de öfkeli yüz sahnenin ana odağını alıyor.'),
'11-booster-makas':('B','B iki mor parçanın dar bağını kesmeyi açık anlatıyor. A’nın yatay parçalarında kesilecek eklem daha belirsiz.'),
'12-booster-degnek':('B','B’de aynı resim ve belirgin değişen parça bağlantıları daha açık; A’nın ışık halkası mekanizmayı örtüyor.'),
'13-booster-cekic':('B','B tek parçanın yerinden çıkmasını gösteriyor; A’nın tokmak üzerindeki gereksiz yapboz amblemi yok.'),
'14-booster-saat':('B','B’de Jiggle saati tutuyor; A’daki büyük havada saat yerine daha doğal ve sıcak ilişki var.'),
'15-zor-bolum':('A','A’daki kıvrımlı dar yol ve farklı engeller zorluğu anlatıyor; B daha sıradan dört parçalı tahta gibi okunuyor.'),
'17-isim':('B','16pt açıklama ve daha geniş yeşil Başla, küçük telefonda A’dan rahat. İsim alanı hâlâ gerçek input için boş yüzey.'),
'18-profil-sorusu':('A','Eşit 190pt cevaplar yeterli hedef büyüklüğünde ve kartta daha fazla nefes alanı bırakıyor. B’nin büyümesi okunurluk kazandırmıyor.')}

def run():
 records=[];checks=[]
 for folder,(v,why) in sorted(CHOICES.items()):
  decision=select(folder,v,why);p=OUT/'NIHAI'/folder;j=json.loads((p/'deneme.json').read_text(encoding='utf-8'));records.append(j)
  hashes=[]
  for trial in ['A','B']:
   q=OUT/'DENEMELER'/trial/folder;hashes.append(filehash(q/'onizleme.png'))
  assert hashes[0]!=hashes[1],folder
  for a in j['assets']:
   im=Image.open(p/a['file']);assert im.mode=='RGBA';assert im.size==(a['width'],a['height']);assert filehash(p/a['file'])==a['sha256'];assert im.width%3==0 and im.height%3==0,(folder,a['file'])
  for w,h in [(375,667),(390,844)]:
   card=Image.open(p/'onizleme.png').convert('RGBA');screen=Image.new('RGBA',(w*3,h*3),'#123F46');screen.alpha_composite(card,((screen.width-card.width)//2,(screen.height-card.height)//2));screen.save(p/f'olcu-{w}x{h}.png')
  checks.append({'family':folder,'chosen':v,'twoDistinctTrials':True,'rgbaAssets':len(j['assets']),'phoneSizes':['375x667','390x844']})
 title='Task23 — 17 nihai aile'
 body=['<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>'+title+'</title><style>body{margin:auto;max-width:1080px;padding:20px;background:#123f46;color:#fff6d8;font:17px system-ui}a{color:#ffdb79}article{padding:20px 0;border-top:1px solid #72afa6}.pair{display:flex;gap:20px;align-items:flex-start}.pair img{max-width:100%;max-height:680px;object-fit:contain}.pair>div{width:50%;min-width:0}h2{font-size:23px}p{line-height:1.5}@media(max-width:650px){.pair{display:block}.pair>div{width:100%}}</style><h1>'+title+'</h1><p>Her ailede iki deneme karşılaştırıldı; aşağıda yalnız seçilen sürüm var. Oyuna bağlanmadı. “Hepsi” veya aile numaralarıyla inceleyebilirsiniz. Son talimatın kapsamı DENEMELER’deki mevcut 17 ailedir; Task23’ün diğer 9 ailesi bu teslimde tamamlanmış sayılmıyor.</p>']
 md=['# Task23 — Kod bağlantı listesi (17 aile)','','Durum: çizim teslimi. Oyun kodu, prefab, sahne, Paintings ve mevcut onaylı PNG dosyaları değiştirilmedi. Son kullanıcı talimatı mevcut DENEMELER’deki 17 aileyi nihai seçip Task24’e geçmektir. Eksik kalan 9 aile aşağıda açıkça listelenir.','','## Bağlama kuralları','','- Her `NIHAI/<aile>/deneme.json` dosyasındaki `assets` kesin dosya listesidir; `logicalPt`, @3x ölçü, `sliceLTRB`, kaynak ve SHA-256 içerir. Unity border sırası L,B,R,T olarak çevrilir. PPU/Canvas ölçeğini mevcut sistemden koru; @3x PNG pikselini mantıksal UI ölçüsü sanma.','- `onizleme`, `ekran-sonra`, `once-sonra`, `olcu-*` runtime sprite değildir. Yazılar Nunito Black ayrı katman; başlık22/23, gövde14–16, puan17/18pt. Resme metin gömme.','- Panel 335pt genişlikte; telefon güvenli alanında ortala. Yeşil tuş684:336 oranını koru; sıkıştırılmış düz şerit yapma. Basılı sprite da aynı ölçekte. Dokunma hedefleri en az44pt.','- 375×667 ve390×844 PNG ölçü kontrolleri grafik kanıtıdır; gerçek Unity akışı, klavye ve cihaz testi tamamlanmış sayılmaz.','','| Aile | Sınıf / yer | Değişecek kaynak / anahtar | Nihai |','|---|---|---|---|']
 for j in records:
  f=j['folder'];v,why=CHOICES[f];url='NIHAI/'+f+'/';esc=html.escape
  body.append(f'<article id="{f}"><h2>{esc(f)} · {esc(j["title"])}</h2><p>{esc(why)}</p><div class="pair"><div><h3>Nihai</h3><img loading="lazy" src="{url}onizleme.png"></div><div><h3>Önce / sonra</h3><a href="{url}once-sonra.png"><img loading="lazy" src="{url}once-sonra.png"></a></div></div><p><a href="{url}NOT.md">Değişim notu</a> · <a href="{url}deneme.json">Sprite / ölçü listesi</a> · <a href="{url}olcu-375x667.png">375</a> · <a href="{url}olcu-390x844.png">390</a></p></article>')
  md.append(f'| {f} | `{j["ui"]}` | {j["target"]} | `{v}`; [NOT](../../Assets/Art/Redesign_T23/NIHAI/{f}/NOT.md) |')
 md+=['','## Davranış ve metin yerleşimi','','- **01 Bölümler öncelik:** merkezde bölüm numarası20pt. Kilit20×22pt ayrı sağ alt rozet, bitenlerde yeşil tik. Güncel düğüm altın66pt, biten turkuaz54pt, kapalı koyu54pt; zor çerçeve ayrı. Oyna84×44pt. Düğümler basamak yüzeyine oturmalı; `anchors` örneğini merdiven fazı ve scroll koordinatıyla eşleştir. Ortada büyük kilit eski taslakları kullanma. PM36 kararı.','- **02–04 sonuç:** üç yıldız/istatistik, ana yeşil eylem, isteğe bağlı1–10 puanlama ve Puanlamayı geç korunur. Kayıpta nedeni ve iki satır yardım göster; puanlama kapalıysa paneli9-slice kısalt. UI kodunda var olan sonuç/tekrar/menü olaylarını kullan.','- **05:** aynı boyda üç tuş; Devam yeşil, Yeniden başla/Ana menü turkuaz. Aynı mevcut pause olayları.','- **06:** iki sıra, beş sütun.50×44pt kutular,7pt yatay aralık,50pt satır adımı. Sayılar18pt ayrı metin. Seçili kutuda tik var.','- **07–15:** üstte tek karakter sahnesi, altında en fazla iki kısa satır, altta tek yeşil tuş. `text` dizisindeki koordinatlar335pt kart yereline göredir. Küçük ikon kutusunu ve turuncu ikinci mesajı kaldır. Engellerde ve zor bölümde Baron; booster sahnesinde Jiggle. Sahne PNG’sini9-slice yapma.','- **15 tek uyarı:**19/23/25/30/35/36/38’de yalnız bir Baron kartı.23’ün sonraki çekiç el eğitimi ayrı adım; ikinci Baron kartı oluşturma. Mevcut eğitim eli dosyaları değişmez.','- **17:** WebGL isim alanı gerçek HTML input; Adın placeholder, Başla eylemi, iki kısa açıklama. Playtest sürümü metni yok. Klavye açıkken safe-area ve scroll canlı test edilmeli.','- **18:** isim kartıyla aynı panel; Hiç / Ara sıra / Sık eşit turkuaz cevaplar. Var olan anket kaydı ve ilerleme olayları korunur.','','## Bu teslimde tamamlanmayan aileler','','16 yükleme;19 ayar anahtarları;20 dil;21 koleksiyon;22 Jiggles;23 HUD;24 booster çubuğu;25 koridor;26 koleksiyon bitişi;27 ev üst barı. Bu listede aslında10 numara vardır; mevcut17 aile ile toplam27 kapsamı oluşur. Ek kaynak çizimleri var olsa da bunlar nihai teslim değildir. Son kullanıcı kapsamı gereği Task24’e geçildi.','','Not: Önce görüntülerinde bazı tanıtımların tam eski modalı bulunmadığından ilgili öğretim/onay ekranı kullanılmıştır; aile NOT dosyalarında sınırlamalar yazılıdır.']
 # 27 total minus17 current =10; keep scope wording arithmetically correct everywhere.
 body=[s.replace('diğer 9 ailesi','diğer 10 ailesi') for s in body];md=[s.replace('Eksik kalan 9 aile','Eksik kalan 10 aile') for s in md]
 (OUT/'inceleme.html').write_text(''.join(body)+'</html>',encoding='utf-8');(ROOT/'docs/art/T23_KOD_LISTESI.md').write_text('\n'.join(md)+'\n',encoding='utf-8')
 (WORK/'nihai-dogrulama.json').write_text(json.dumps(checks,ensure_ascii=False,indent=2),encoding='utf-8')
 with (ROOT/'docs/art/GECE_LOG.md').open('a',encoding='utf-8') as g:g.write('\n\n## '+datetime.now().astimezone().isoformat(timespec='minutes')+' — Task23 mevcut17 aile teslimi\n\n17 ailenin iki farklı denemesi görsel olarak karşılaştırıldı, tek seçim NIHAI altına alındı; inceleme.html ve T23_KOD_LISTESI.md hazır. Merdiven önceliği tamam: altın güncel düğüm, numara merkezde, kilit sağ altta. RGBA/@3x/ölçü ve kaynak hash kontrolleri geçti. Gerçek Unity testi yapılmadı.\n\nSon kullanıcı talimatı mevcut17 aileyi bitirip Task24’e geçmek olduğundan diğer10 aile tamamlanmış sayılmadı; kod listesinde açıkça belirtildi. A/B ve tüm kaynaklar korundu, silme yok.\n')
 print('17 finals, 34 distinct trials, asset QA passed; review and code list written')
if __name__=='__main__':run()
