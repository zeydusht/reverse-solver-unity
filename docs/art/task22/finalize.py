from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageChops
import json,csv,collections,re,datetime,hashlib
R=Path(__file__).resolve().parents[3];W=R/'docs/art/task22';O=R/'Assets/Art/Redesign_T22'
F=json.loads((W/'findings.json').read_text(encoding='utf8'));A=R/'docs/art/DENETIM_2026-11-03.md';text=A.read_text(encoding='utf8')
new=[dict(id='U19',round=2,screen='40/40 harita',element='Oyna balonu üst sınır',problem='Yeni won40 harita karesinde Oyna balonunun üst yarısı içerik maskesinin arkasında kesiliyor; düğüm40 görünür, balon etiketi eksik.',severity='orta',suggestion='Kaydırma üst sınırına balon+avatar yüksekliği kadar başlangıç boşluğu ekle; mevcut 44pt hedef maskenin dışında kalmamalı.',kind='kod',evidence='docs/qa/2026-11-03-tam/tr_390x844_won40_8_bolumler.png'),
 dict(id='İ10',round=4,screen='40/40 sonrası Ev',element='Tamamlandı / yeniden oyna durumu',problem='Yeni won40 Ev karesinde ana eylem yalnız Bölüm40; koleksiyon40/40 olsa da setin tamamlandığı veya bunun tekrar oynama olduğu açıklanmıyor.',severity='orta',suggestion='Mevcut yeşil tuşta Tekrar oyna / Bölüm40 ve kısa 40 bölüm tamamlandı metniyle durumu açıkla. Yeni bitmap gerekmiyor.',kind='metin',evidence='docs/qa/2026-11-03-tam/tr_390x844_won40_1_ev.png')]
for f in new:
 if f['id'] in {x['id'] for x in F}:continue
 F.append(f)
 cells=[f['id'],f['screen'],f['element'],f['problem']+' Kanıt: `'+f['evidence']+'`',f['severity'],f['suggestion'],f['kind']]
 row='| '+' | '.join(cells)+' |\n\n'
 marker='## Tur 3' if f['round']==2 else '## Çizim kapanış eşlemesi'
 text=text.replace(marker,row+marker,1)
old='docs/qa/2026-11-02-son/375x667_koleksiyon_siradaki.png';latest='docs/qa/2026-11-03-tam/375x667_koleksiyon_siradaki.png'
assert ImageChops.difference(Image.open(R/old).convert('RGB'),Image.open(R/latest).convert('RGB')).getbbox() is None
M=json.loads((O/'manifest.json').read_text(encoding='utf8'))
for j in M['items']:
 if j['id']!=21:continue
 j['equivalentOlderSource']=old;j['source']=latest
 p=O/j['folder'];note=p/'NOT.md';note.write_text(note.read_text(encoding='utf8').replace(old,latest)+'\n3 Kasım tam turdaki Sıradaki durumunun pikselleri önceki kaynakla aynıdır; en yeni eşdeğer kaynağa güncellendi.\n',encoding='utf8')
 pair=Image.open(p/'once-sonra.png').convert('RGB');d=ImageDraw.Draw(pair);d.rectangle((0,0,pair.width,64),fill='#102F39');cw=(pair.width-90)//2
 f=ImageFont.truetype(str(R/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),max(9,min(33,round(cw/19))))
 d.text((30,20),'ÖNCE · 2026-11-03-tam',font=f,fill='#F8EBD0');d.text((cw+60,20),'SONRA · T22 TASLAK',font=f,fill='#F8EBD0');pair.save(p/'once-sonra.png')
for f in F:
 if f.get('drawing')==21:f['evidence']=latest
 if f['id']=='İ08':f['evidence']='Assets/_Game/Presentation/Ui/Screens.cs::Result; docs/qa/2026-11-03-tam/tr_390x844_won40_2_koleksiyon.png'
counts=collections.Counter(f['severity'] for f in F)
text=text.replace(old,latest)
text=re.sub(r'Toplam \d+ kayıt: \d+ yüksek / \d+ orta / \d+ düşük\.',f'Toplam {len(F)} kayıt: {counts["yüksek"]} yüksek / {counts["orta"]} orta / {counts["düşük"]} düşük.',text)
text+='''
## Son 40/40 ve kaynak kesimi

3 Kasım won40 Ev, Koleksiyon ve harita kareleri ayrıca açıldı. Koleksiyon40/40 görünür; Evde Bölüm40 tekrar eylemi ve haritada üstten kırpılan Oyna balonu U19/İ10 olarak kaydedildi. Gerçek terminal kazanma modalı bu kayıtların içinde hâlâ yok. Yeni tam turdaki Sıradaki durum karesi önceki2 Kasım kaynağıyla piksel eşit olduğu doğrulanarak S21'in kaynak yolu da yenilendi. Ana tablo sayımı bu son iki kayıt dahil63'tür; ek raporun tekrarları ayrıca toplanmaz.
'''
A.write_text(text,encoding='utf8');(W/'findings.json').write_text(json.dumps(F,ensure_ascii=False,indent=2),encoding='utf8')
(O/'manifest.json').write_text(json.dumps(M,ensure_ascii=False,indent=2),encoding='utf8')
page=(O/'inceleme.html').read_text(encoding='utf8').replace(old,latest);(O/'inceleme.html').write_text(page,encoding='utf8')
jobs=json.loads((W/'jobs.json').read_text(encoding='utf8'))
for j in jobs:
 if j['id']==21:j['source']=latest
(W/'jobs.json').write_text(json.dumps(jobs,ensure_ascii=False,indent=2),encoding='utf8')
# Freeze the final evidence tables under distinct names; concurrent work may keep its own draft tables.
for oldname,newname in [('temizlik-dosyalar.csv','temizlik-final-dosyalar.csv'),('temizlik-klasorler.csv','temizlik-final-klasorler.csv'),('temizlik-erisim-hatalari.json','temizlik-final-erisim-hatalari.json'),('temizlik-ozet.json','temizlik-final-ozet.json')]:
 (W/newname).write_bytes((W/oldname).read_bytes())
with (W/'temizlik-final-dosyalar.csv').open(encoding='utf-8-sig',newline='') as f:rows=list(csv.DictReader(f))
C=R/'docs/art/TEMIZLIK_ONERI.md';ct=C.read_text(encoding='utf8')
ct=ct.replace('temizlik-dosyalar.csv','temizlik-final-dosyalar.csv').replace('temizlik-klasorler.csv','temizlik-final-klasorler.csv').replace('temizlik-erisim-hatalari.json','temizlik-final-erisim-hatalari.json')
table='''## Öncelikli eski klasörler — ayrıntılı karar özeti

Bu tablo yukarıdaki grupların alt kırılımıdır; toplam kazanca bir daha eklenmez. Aynı klasörde aktif kaynak varsa yalnız arşiv grubuna giren dosyalar arşiv adayıdır; tam dosya CSV'si belirleyicidir.

| Yol | Okunabilen MB | Güvenli cache MB | Arşiv adayı MB | Kalmalı MB | Neden |
|---|---:|---:|---:|---:|---|
'''
for prefix in ['Assets/Art/Menu/Task19','Assets/Art/Menu/Task19B','Assets/Art/Menu/Task19C','Assets/Art/Menu/Task19D','docs/art/drafts','docs/art/backups','docs/art/.task15-python','docs/art/.task16-python','docs/art/.task15-pip-cache','docs/art/.chrome-task6']:
 rr=[r for r in rows if r['path'].startswith(prefix+'/')];amount=collections.Counter()
 for r in rr:amount[r['group']]+=int(r['bytes'])
 why='Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur.'
 if 'python' in prefix:why='Erişim reddi veren alt dizinler var; bilinen boyut alt sınırdır, kaldırma önerilmedi.'
 if 'cache' in prefix or '.chrome' in prefix:why='Yeniden üretilebilir araç önbelleği; oyunda kullanım kanıtı yok.'
 table+=f'| `{prefix}` | {sum(amount.values())/1e6:.3f} | {amount["Güvenle silinebilir"]/1e6:.3f} | {amount["Arşive taşınabilir"]/1e6:.3f} | {amount["Kalmalı"]/1e6:.3f} | {why} |\n'
ct=ct.replace('## Kazanç hesabı',table+'\n## Kazanç hesabı')
ct+='\nSon kanıt kesimi: `task22/temizlik-final-ozet.json`. Sonradan üretilen teslim metadata/kopya tabloları bu tarama anının dışında kalır; hepsi kalmalı teslim dosyalarıdır, kazanç hesabına dahil değildir.\n'
C.write_text(ct,encoding='utf8');(R/'docs/art/TEMIZLIK_T22_KANITLI.md').write_text(ct,encoding='utf8')
c=json.loads((W/'temizlik-final-ozet.json').read_text(encoding='utf8'))
delivery=dict(findings=len(F),severity=dict(counts),mainDrawings=22,alternativeFamilies=13,PNG=150,beforeAfter=36,cleanupSafeMB=c['safeMB'],cleanupArchiveMB=c['archiveMB'],cleanupPotentialMB=c['potentialMB'],actualRecoveredMB=0,review='Assets/Art/Redesign_T22/inceleme.html',missingEvidence=['Gerçek 40. bölüm terminal kazanma modalı','Zincir/duvar ve bazı booster ayrı modal kayıtları','Klavye açık isim formu'])
(W/'teslim-ozet.json').write_text(json.dumps(delivery,ensure_ascii=False,indent=2),encoding='utf8')
with (R/'docs/art/GECE_LOG.md').open('a',encoding='utf8') as log:
 log.write(f'''\n## {datetime.datetime.now().astimezone().isoformat(timespec='minutes')} — son kontrol ve kararlar
- karar: İlk “piksel≈pt” varsayımı düzeltildi: kaynaklar @2x/@3x olabilir; UX hesapları mantıksal375/390 pt ve kod ölçülerinden yapıldı. PNG fiziksel pikseli doğrudan pt sayılmadı.
- Son tam turdaki TR375/TR390 menüler ve won40 Ev/Koleksiyon/harita da incelendi. Kapanmış Level→Bölüm farkı düşük kayıt; balon kırpılması ve tamamlanma dili iki yeni kod/metin bulgusu.
- karar: 101–113 bağımsız ek adaylar değiştirilmeden tek inceleme sayfasına eklendi. Ek rapordaki39 tekrar ana toplamı şişirmedi; üç benzersiz doğrulanmış kod konusu U18/T09/T10'a alındı.
- Teknik kontrol:150 aday PNG,35 aile ve36 önce/sonra. Ana130 PNG alfa/@3x/hash/slice; ek20 PNG alfa/@3x/hash geçti. Ana kaynak havuzundaki3.082 dosya değişmedi/eksilmedi. HTML yerel dosya hedefleri ve JS sözdizimi kontrol edildi; browser/Unity testi iddia edilmiyor.
- karar: Son temizlik kanıtları eşzamanlı taslak raporlardan ayrı `temizlik-final-*` dosyalarında donduruldu. Erişilemeyen10 dizin belirsiz/kalmalı; bilinmeyen baytlar kazanıma eklenmedi. Klasör tabloları recursive olduğundan toplanmaz.

## Sabah teslim özeti — A / B / C
- **A:** Dört ayrı tur tamamlandı: **{len(F)} kayıt; {counts['yüksek']} yüksek / {counts['orta']} orta / {counts['düşük']} düşük.** Düşük kayıtların bir kısmı uygunluk/kapanmış sorun notudur; yeni hata sayısı değildir. Rapor: `docs/art/DENETIM_2026-11-03.md`.
- **B:** Yüksek/orta22 çizim işinin tamamı01–22 klasörlerinde. Ek101–113 ailesiyle birlikte35 seçilebilir aile,150 şeffaf @3x PNG adayı,36 önce/sonra görüntüsü. Her ana işte NOT.md ve telefon önizlemeleri var. İlk iki deneme korunur; yeni tasarım yönü en çok3 deneme.
- **Tek inceleme sayfası:** `Assets/Art/Redesign_T22/inceleme.html`. Numaralar seçilebilir ve kopyalanabilir; oyuna yazmaz. Ayrı AI sayfası tarihçe olarak korunur ama bütün önce/sonralar ana sayfada da vardır.
- **C:** {c['files']} okunabilir dosya, erişilebilir/eksik işaretli{c['folders']} klasör kaydı; Assets içinde{c['AssetsTextFiles']} metin ve{c['runtimeHashFiles']} runtime içerik kopyası kontrol edildi. **{c['safeMB']:.3f} MB güvenle kaldırılabilir cache + {c['archiveMB']:.3f} MB dış arşivleme adayı = {c['potentialMB']:.3f} MB koşullu potansiyel. Gerçekte kazanılan0 MB.** Rapor: `docs/art/TEMIZLIK_ONERI.md`; sabit kanıt kopyası `docs/art/TEMIZLIK_T22_KANITLI.md`.
- **Kanıt sınırı:** Yeni tam turda gerçek40 kazanma modalı yok; won40 menü/40/40 koleksiyon durumu incelendi. Ayrı intro modalı olmayan öğelerde en yeni ilgili öğretim bağlamı açıkça etiketlendi. Eğitim eli değişmedi.
- **Kurallar:** Yalnız Assets/Art/Redesign_T22 ve docs/art yazıldı. Silme, taşıma, yeniden adlandırma, git, Unity, build yok. C#, Assets/_Game, sahne, prefab ve eski sanat PNG'lerine yazılmadı. Korunan kaynakların hash kanıtı `task22/original-art-check.json`.

### Zeyd'in karar vermesi gereken en önemli5 madde
1. Sonuç ailesi:01/03/04/05 ve02 kutuları mı;101/102/103 ve113 boyalı alternatifleri mi? Ana yeşil eylem ve mevcut soru akışı korunmalı.
2. Tanıtım/uyarı:06–14 nesne odaklı sade set mi;104 ortak boyalı kabuk ve105 Baron alternatifi mi?
3. İlk giriş:15/16/17 ile106/107/108 arasında yükleme/isim/soru yönü; anketin zamanlaması ayrı ürün kararı.
4. Ayar/harita/koleksiyon/Jiggles:18–22 ile109–112 alternatifleri. Özellikle22 mevcut PM33 ilerleme işlevini korur;112 madalyon yönü işlev değişikliği kararı ister.
5. Temizlik: Önce{c['safeMB']:.3f} MB yeniden üretilebilir cache; ardından{c['archiveMB']:.3f} MB tarihçe dış arşivleme listesini PM ile seç. Aynı diske taşıma alan kazandırmaz; bu görev hiçbirini uygulamadı.
''')
print(json.dumps(delivery,ensure_ascii=False),flush=True)
