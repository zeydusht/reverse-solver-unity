"""Join independently produced art proposals without replacing the primary work."""
from pathlib import Path
from datetime import datetime
import json,re
ROOT=Path(__file__).resolve().parents[2];O=ROOT/'Assets/Art/Redesign_T22';W=ROOT/'docs/art/task22';p=O/'inceleme.html';s=p.read_text(encoding='utf-8')
start='<!-- T22_AI_EK_START -->';end='<!-- T22_AI_EK_END -->'
if start in s:s=s[:s.index(start)]+s[s.index(end)+len(end):]
ai=(O/'inceleme-ai.html').read_text(encoding='utf-8');fragment=ai[ai.index('<article id='):ai.rindex('</main>')]
fragment=re.sub(r'id="aday-(\d+)"',r'id="i\1"',fragment);fragment=fragment.replace('class="variants"','class="ai-variants"').replace('<img loading="lazy" src=','<img class="pair" loading="lazy" src=')
# The time loss has its own true before/after, shown inline in the same review page.
fragment=fragment.replace('<a href="102-ai-kayip/once-sonra-sure.png">Süre kaybı ·3 Kasım önce/sonra</a>','<a href="102-ai-kayip/once-sonra-sure.png"><img class="pair" loading="lazy" src="102-ai-kayip/once-sonra-sure.png" alt="Süre kaybı3 Kasım önce ve sonra">Süre kaybı ·3 Kasım önce/sonra</a>')
heading='''<section id="ai-ek"><style>.ai-variants{display:grid;grid-template-columns:repeat(auto-fit,minmax(140px,1fr));gap:16px}.ai-variants img{max-height:400px;max-width:100%;height:auto}.ai-variants figure{margin:0}</style><h2>101–113 ·13 ek AI malzeme alternatifi</h2><p>Bunlar ana22 öneriye alternatiflerdir; aynı sorunu çözdükleri için bulgu sayısına tekrar eklenmez. Seçim listesi bütün numaraları birlikte toplar.112 madalyon seçeneği,22 kısa ilerleme barına alternatif ürün kararıdır.102 puan sorusu kapalı durumdur; puan akışını kaldırma izni değildir. Yeni kaynaklar/teknik doğrulama <a href="ai-manifest.json">ayrı manifestte</a>.</p>'''
block=start+heading+fragment+'</section>'+end
s=s.replace('</main>',block+'</main>',1);s=s.replace('<h1>22 öneri. Önce / Sonra.</h1>','<h1>22 ana öneri +13 alternatif. Önce / Sonra.</h1>');s=s.replace('22 / 22 çizim işi · Yalnız art adayları','22 / 22 ana çizim işi +13 alternatif aile · Yalnız art adayları')
p.write_text(s,encoding='utf-8')
mf=O/'manifest.json';m=json.loads(mf.read_text(encoding='utf-8'));m['supplementalManifests']=['ai-manifest.json'];m['reviewFamilies']=35;m['reviewBeforeAfterImages']=36;mf.write_text(json.dumps(m,ensure_ascii=False,indent=2),encoding='utf-8')
report=ROOT/'docs/art/DENETIM_2026-11-03.md';body=report.read_text(encoding='utf-8');marker='## Bağımsız AI malzeme çapraz kontrolü'
if marker not in body:
 body+='\n'+marker+'\n\n[Bağımsız dört tur / kanıt eki](DENETIM_T22_AI_EK.md) ve [AI teknik QA](task22/ai-validation.json) eklendi.101–113 numaralı13 alternatif aile ana inceleme sayfasında aynı anda görülebilir. Bu ekin41 satırı çoğunlukla aynı bulguları çapraz doğrular ve61 ana bulguya toplanmaz.3 Kasım tam turla eski süre/pause/zor/Ayarlar kaynakları güncellendi.\n\nÜrün kararı:112 üç madalyon alternatifi, ana22 kısa ilerleme kapsülüne karşı değerlendirilmelidir. PM33 işlevi/veri eşikleri değişmez; oyuna hiçbir alternatif bağlanmadı. Harita düğümü2. imagegen sürümünde altın kenarı inceltildi; ilk kaynak korunmuştur.\n'
 report.write_text(body,encoding='utf-8')
note=W/'KOORDINASYON.md'
with note.open('a',encoding='utf-8') as f:f.write('\n'+datetime.now().astimezone().isoformat(timespec='seconds')+' —101–113 hazır. Ana inceleme35 aile/36 önce-sonra ile birleştirildi. Yeniden render edilirse t22_merge_review.py yalnız bağlantı/ek bloğunu tekrar korur. AI PNGleri deneme artığı değildir; ai-manifest.json içinde20 runtime adayına karşılık gelir.\n')
print('merged review:35 families,36 before/after images')
