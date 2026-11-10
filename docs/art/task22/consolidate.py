from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageChops
import json,html,hashlib,datetime,re
R=Path(__file__).resolve().parents[3];O=R/'Assets/Art/Redesign_T22';W=R/'docs/art/task22'
J=json.loads((W/'jobs.json').read_text(encoding='utf8'));M=json.loads((O/'manifest.json').read_text(encoding='utf8'))
F=json.loads((W/'findings.json').read_text(encoding='utf8'));report=(R/'docs/art/DENETIM_2026-11-03.md').read_text(encoding='utf8')
for n,old,new in [(20,'docs/qa/2026-11-02-son/375x667_harita.png','docs/qa/2026-11-03-tam/tr_375x667_won12_8_bolumler.png'),(22,'docs/qa/2026-11-02-son/375x667_jiggles.png','docs/qa/2026-11-03-tam/tr_375x667_won12_4_jiggles.png')]:
 assert ImageChops.difference(Image.open(R/old).convert('RGB'),Image.open(R/new).convert('RGB')).getbbox() is None
 for j in M['items']:
  if j['id']!=n:continue
  j['source']=new;j['equivalentOlderSource']=old
  p=O/j['folder'];note=p/'NOT.md';s=note.read_text(encoding='utf8').replace(old,new)
  if 'Piksel eşitliği' not in s:s+='\nPiksel eşitliği: 3 Kasım yeni kaynak ile önceki 2 Kasım son kaynağının RGBA piksel farkı 0. En yeni eşdeğer kaynak yoluna güncellendi; çizim yeniden üretilmedi.\n'
  note.write_text(s,encoding='utf8')
  # Only the source caption changes; before and after crops are already pixel-identical.
  pair=Image.open(p/'once-sonra.png').convert('RGB');d=ImageDraw.Draw(pair);d.rectangle((0,0,pair.width,64),fill='#102F39')
  cw=(pair.width-90)//2;f=ImageFont.truetype(str(R/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),max(9,min(33,round(cw/19))))
  d.text((30,20),'ÖNCE · 2026-11-03-tam',font=f,fill='#F8EBD0');d.text((cw+60,20),'SONRA · T22 TASLAK',font=f,fill='#F8EBD0');pair.save(p/'once-sonra.png')
 for row in F:
  if row.get('drawing')==n:row['evidence']=new
 report=report.replace(old,new)
(W/'findings.json').write_text(json.dumps(F,ensure_ascii=False,indent=2),encoding='utf8')
report+='''
## Son tam tur kontrolü ve ek adaylar

3 Kasım TR375 ve TR390 menü dizileri yeniden karşılaştırıldı. Evdeki eski Level metni yeni kayıtta Bölüm olarak düzelmiş; T01 düşük/kapanmış kayıt olarak güncellendi. Harita ve Jiggles yeni TR375 kareleri önceki “son” kareleriyle piksel piksele aynı; S20/S22 en yeni eşdeğer kaynağa geçirildi. Koleksiyonun Sıradaki hücresinin tamamını gösteren ayrı durum için 2 Kasım son/koleksiyon_siradaki karesi korunur; yeni genel 375 koleksiyon karesinde bu hücre ekran dışında kalır.

Çalışma klasöründe eşzamanlı hazırlanmış **101–113 AI malzeme alternatifleri** de bulundu. [Bağımsız ek denetim](DENETIM_T22_AI_EK.md) ayrı tarihli bir kaynak olarak korunur; ana raporla tekrar eden 39 satır bulgu toplamına eklenmedi. Yeni ve kodla doğrulanan üç konu ana tur tablolarına U18/T09/T10 olarak işlendi. Ek rapordaki eski kaynak/eksik güncel pause/süre bilgileri, yeni tam tur sonrası ana rapor tarafından güncellenmiştir.

Tek `inceleme.html` içinde **01–22 ana işler + 101–113 alternatif aileler** alt alta yer alır. Ana 22 çizim işinin kapsamı tamamdır. Alternatifleri seçmek zorunlu değildir. 112 madalyon yönü, PM_MESAJ_33 ilerleme işlevini değiştirebileceğinden ürün kararı ister; 22 numaralı ana öneri mevcut ilerleme işlevini korur. 110 alternatif düğümde tek sprite altın maskesi %13,875; bu bir ekran alanı yüzdesi değildir, toplam ekran altın oranı cihaz yerleşiminde ayrıca ölçülmelidir.
'''
(R/'docs/art/DENETIM_2026-11-03.md').write_text(report,encoding='utf8')
ai=json.loads((O/'ai-manifest.json').read_text(encoding='utf8'));aij=json.loads((W/'ai-jobs.json').read_text(encoding='utf8'))
titles={101:'Kazanma · boyalı kabuk',102:'Kayıp · ortak kabuk',103:'Durdurma · boyalı kabuk',104:'Sekiz tanıtım · ortak kabuk',105:'Zor bölüm · Baron alternatifi',106:'Yükleme · ilerleme alanı',107:'İsim girişi · boyalı kabuk',108:'Profil sorusu · boyalı kabuk',109:'Ayarlar · yeni boyalı durumlar',110:'Harita · yeni düğüm ve balon',111:'Koleksiyon · krem kilitli hücre',112:'Jiggles · madalyon alternatifi',113:'Puan · dokulu seçim kutuları'}
alternates=[]
for p in sorted(O.glob('1??-ai-*')):
 if not p.is_dir():continue
 n=int(p.name.split('-')[0]);pairs=sorted(p.glob('once-sonra*.png'))
 if not pairs:continue
 alternates.append(dict(id=n,folder=p.name,title=titles.get(n,p.name),pairs=[x.relative_to(O).as_posix() for x in pairs],manifest='ai-manifest.json'))
M['coexistingAlternatives']=alternates;M['unusedAttemptFiles']=['01-kazanma/nesne-alani.png']
(O/'manifest.json').write_text(json.dumps(M,ensure_ascii=False,indent=2),encoding='utf8')
page=(O/'inceleme.html').read_text(encoding='utf8')
# Idempotent consolidation: never append the same alternatives twice.
page=re.sub(r'<!-- T22_ALTERNATIVES_START -->[\s\S]*?<!-- T22_ALTERNATIVES_END -->','',page)
block='<!-- T22_ALTERNATIVES_START --><section><h2>101–113 · Boyalı malzeme alternatifleri</h2><p>Aynı çizim bulguları için çalışma klasöründe eşzamanlı hazırlanmış ayrı aday ailesi. Numaralar birbirinden bağımsızdır; örneğin 3 veya 102 seçilebilir. Ana bulgu sayısına tekrar eklenmez.</p>'
for a in alternates:
 n=a['id'];folder=a['folder'];title=html.escape(a['title']);desc='Aynı iş için farklı boyalı panel/malzeme adayı; ayrıntı NOT.md içinde.'
 if n==112:desc='Üç ayrı madalyon yönü. Mevcut PM33 ilerleme çubuğu işlevinden farklı; ürün kararı gerekir. İşlevi koruyan ana öneri: 22.'
 if n==110:desc='Tok yeşil düğüm ve Oyna balonu; tek düğümde altın maskesi %13,875. Ekranın toplam altın oranı ayrıca değerlendirilmelidir.'
 block+=f'<article id="i{n}">'
 for pair in a['pairs']:block+=f'<a href="{pair}"><img class="pair" loading="lazy" src="{pair}" alt="{n} {title}: önce ve sonra"></a>'
 block+=f'<h2>{n} — {title}</h2><p>{desc}</p><label><input type="checkbox" value="{n}" aria-label="{n} numarayı seç">Bu alternatifi seç</label><details><summary>Alternatif notları ve varyantlar</summary><p><a href="{folder}/NOT.md">NOT.md</a> · <a href="ai-manifest.json">AI manifesti</a> · <a href="inceleme-ai.html#i{n}">Ayrı alternatif sayfası</a></p>'
 if n==104:
  block+='<p>Sekiz tanıtım varyantı: '
  for q in sorted((O/folder).glob('onizleme-*.png')):block+=f'<a href="{q.relative_to(O).as_posix()}">{html.escape(q.stem.removeprefix("onizleme-"))}</a> · '
  block+='</p>'
 block+='</details></article>'
block+='</section><!-- T22_ALTERNATIVES_END -->'
page=page.replace('</main>',block+'</main>')
page=page.replace('22 öneri. Önce / Sonra.','22 öneri + 13 alternatif. Önce / Sonra.')
page=page.replace('22 / 22 çizim işi · Yalnız art adayları','22 / 22 ana çizim işi + 13 alternatif aile · Yalnız art adayları')
page=page.replace('<nav>','<nav>'+''.join(f'<a href="#i{a["id"]}">{a["id"]}</a>' for a in alternates)) if 'href="#i101"' not in page else page
for n,old,new in [(20,'docs/qa/2026-11-02-son/375x667_harita.png','docs/qa/2026-11-03-tam/tr_375x667_won12_8_bolumler.png'),(22,'docs/qa/2026-11-02-son/375x667_jiggles.png','docs/qa/2026-11-03-tam/tr_375x667_won12_4_jiggles.png')]:page=page.replace(old,new)
(O/'inceleme.html').write_text(page,encoding='utf8')
# Independent mechanical check of every coexisting AI candidate; do not mutate its PNGs or manifest.
aierrors=[]
for a in ai:
 p=O/a['file']
 with Image.open(p) as im:
  if im.mode!='RGBA' or im.size!=(a['width'],a['height']) or im.getchannel('A').getextrema()[0]!=0 or im.width%3 or im.height%3:aierrors.append(a['file']+' size/alpha')
 if hashlib.sha256(p.read_bytes()).hexdigest()!=a['sha256']:aierrors.append(a['file']+' hash')
(W/'consolidation.json').write_text(json.dumps({'mainJobs':22,'mainPNG':len(M['assets']),'alternativeFamilies':len(alternates),'alternativePNG':len(ai),'totalCandidatePNG':len(M['assets'])+len(ai),'alternativeErrors':aierrors,'mainFindings':len(F),'duplicatesNotAdded':39,'beforeAfterImages':22+sum(len(a['pairs']) for a in alternates)},ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps({'families':len(alternates),'totalPNG':len(M['assets'])+len(ai),'errors':aierrors},ensure_ascii=False))
