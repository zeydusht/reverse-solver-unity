"""PM_107 §C7: docs/art/pm107/MIKS_RAPORU.md from the measurements (docs/art/pm107/miks/<scenario>/olcum.json, after;
miks/once/<scenario>/olcum.json, before: &miks=0&limiter=0) and the mastering table (miks/hedefler.json).
    python -I Tools/pm107_miks_rapor.py
"""
import json, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
M = os.path.join(ROOT, 'docs', 'art', 'pm107', 'miks')
OUT = os.path.join(ROOT, 'docs', 'art', 'pm107', 'MIKS_RAPORU.md')
SC = [('bolum', 'Bölüm oynanışı (10 çıkış, 3 takılma)'), ('saat', 'Saat booster\'ı açıkken kayma'), ('zincir', 'Zincirli bölüm'),
      ('bomba', 'Bomba + kayıp'), ('kazanma', 'Kazanma fanfarı (4. bölüm baştan sona)'), ('menu', 'Menü gezintisi (düğmeler)'), ('dalis', 'Menüden Oyna: dalış')]


def load(p):
    try:
        return json.load(open(p, encoding='utf-8'))
    except Exception:
        return None


rows = []
for key, name in SC:
    a, b = load(os.path.join(M, key, 'olcum.json')), load(os.path.join(M, 'once', key, 'olcum.json'))
    f = lambda r, k, u='': '–' if r is None else f"{r[k]}{u}"
    rows.append(f"| {name} | {f(b, 'sounds')} / {f(a, 'sounds')} | {f(b, 'drowned')} → **{f(a, 'drowned')}** | {f(b, 'peaks_over')} → **{f(a, 'peaks_over')}** | "
                f"{f(b, 'peak_dbfs')} → {f(a, 'peak_dbfs')} | {f(b, 'effects_lufs_max')} → {f(a, 'effects_lufs_max')} | {f(b, 'music_lufs_median')} → {f(a, 'music_lufs_median')} | "
                f"[grafik](miks/{key}/seviye.png) |")

h = load(os.path.join(M, 'hedefler.json')) or {}
files = []
for n, r in sorted(h.items()):
    files.append(f"| {n} | {r['before']['lufs']} | **{r['after']['lufs']}** | {r['before']['peak']} → {r['after']['peak']} | {'evet' if r['smooth'] else ''} |")

md = f"""# PM_107 §C: ses miksajı raporu

Zeyd'in istediği: "Bir şey kulağımı tırmalamasın, ne çok olsun ne az; net ve temiz duyulsun. Saati durdurdum, parçaları kaydırırken tik takı hiç duymadım."

## Ne neden değişti

| Sorun (önce) | Sebep (ölçülen) | Ne yaptım |
|---|---|---|
| Saat açıkken tik-tak duyulmuyordu | Tik-tak dosyası −29,6 LUFS, kayma −15,5 LUFS. Tik-tak kaymanın 14 dB altındaydı, tamamen örtülüyordu. | Tik-tak −21,5 LUFS'e çıktı. Saat çalışırken kayma, çıkış ve öteki efektler 5 dB, müzik 3 dB kısılıyor (Mix107: Saat kanalı hiç kısılmaz). Kayma artık tik-takın 0,5 dB altında. |
| Dalışta neredeyse ses yok, sonda yalnız "fışt" | Whoosh −33 LUFS ve yalnız son 0,6 sn'de çalıyordu (yaklaşma 2–3 sn). | Yaklaşmanın başından kamera hızını izleyen bir hava sesi var: koridorun dokusu, hız eğrisi 30u²(1−u)², varışta sıfır. Sondaki whoosh −24 LUFS'e çıktı ve %55 düzeyde, havanın ucu gibi çalıyor. Kazanma → koridor → dalış da aynı yoldan geçiyor. |
| Kayma ve çıkış tizde tırmalıyordu | Seçilen çıkış (u) ile zincirin tizinde sert enerji vardı; saldırı ani. | Kayma, çıkış, zincir ve whoosh'larda 7 kHz'den yukarısı −4 dB (yüksek raf süzgeci), 4 ms yumuşak saldırı. Bütün dosyalarda 1 ms giriş ve 5 ms çıkış sönümü (tık yok). |
| Hızlı fiske serisinde sesler üst üste biniyordu | Aynı ses sınırsız sayıda üst üste çalabiliyordu. | Aynı ses aynı anda en çok 2 kez (Mix107.Admit). Web'de her ses bağlamının çıkışında sınırlayıcı: eşik −4 dBFS, oran 20, saldırı 2 ms. |
| Fanfar sırasında efektler | Fanfar ve efektler aynı düzeydeydi. | Fanfar çalarken efektler ve zincir 6 dB, düğmeler 3 dB kısık. Kazanma ekranından çıkınca hemen geri gelir. |
| Patlama ve kayıp | Müzik yalnız kayıpta kısılıyordu. | Bomba patlayınca müzik 1,6 sn boyunca 4 dB daha aşağı iner (kaybın kendi kısmasının üstüne). |
| Seviyeler dağınıktı | Dosyalar −15…−33 LUFS arasındaydı; birbirine göre ayarlanmamıştı. | Her seçili ses hedef düzeyine getirildi (aşağıdaki tablo). Tepeler −1,5 dBFS'in altında, sert kırpma yok. |

## Kanallar (Mix107, tek yer)

| Kanal | İçinde | Saat açık | Fanfar | Patlama |
|---|---|---|---|---|
| Müzik | lobi ve bölüm döngüsü | −3 dB | (bölüm müziği zaten kesik) | −4 dB, 1,6 sn |
| Efekt | kayma/çıkış, çarpma, engeller, booster sesleri | −5 dB | −6 dB | |
| Arayüz | düğme toku, panel vuşu, geçişler, dalış havası | | −3 dB | |
| Zincir | zincirin kesintisiz şıkırtısı | | −6 dB | |
| Saat | tik-tak | kısılmaz | | |
| Fanfar | kazanma fanfarı | | | |

Kısmalar yumuşak geçer: inişte saniyede 80 dB, çıkışta saniyede 20 dB; pompalama olmaz.

`&miks=0` hepsini kapatır: eski dosyalar, kısma yok, ses sınırı yok, dalış havası yok. `&limiter=0` sınırlayıcıyı kapatır.

## Senaryo kayıtları (önce → sonra)

Her kayıt oyunun kendi çıkışından (Chrome, gerçek GPU) alındı: efektler ve müzik ayrı; `Tools/pm107_miks.js`.

- **Önce:** `&miks=0&limiter=0`. Seçimler aynı, miksaj eski.
- **Ölçüm:** `Tools/pm107_miks_olc.py`.
- **Duyulmayan ses:** Aynı 400 ms'de çalan en yüksek şeyin (başka bir ses ya da müzik) 6 dB'den fazla altında kalan ses.
- **Tepe aşımı:** Efekt ile müzik toplamının −1 dBFS'i aştığı 10 ms'lik dilimler.

| Senaryo | ses sayısı (önce / sonra) | duyulmayan | tepe aşımı | tepe dBFS | efekt en yüksek LUFS | müzik LUFS | seviye |
|---|---|---|---|---|---|---|---|
{chr(10).join(rows)}

Ses kayıtları `miks/<senaryo>/efektler.webm` ve `muzik.webm`. Önce kayıtları `miks/once/<senaryo>/` altında.

## Dosya düzeyleri (Tools/make_miks107.py, Resources/Art/audio/pm107)

En yüksek 400 ms'lik dilim, LUFS. Hedefler:

- kayma / çıkış −17;
- çarpma −17,5;
- düğme −20;
- panel vuşu −23;
- zincir −22,5;
- zincir gerilme −17;
- patlama −16;
- tik-tak −21,5;
- booster'lar −18;
- koridor −25;
- dalış −24.

Hafif geçiş işaretleri (çizgi, geri, HUD, sekme) olduğu gibi, müziğin altında kaldı.

| Dosya | önce LUFS | sonra LUFS | tepe dBFS | yumuşatma |
|---|---|---|---|---|
{chr(10).join(files)}

## Doğrulanmayan

- **Telefon:** Ölçümler bilgisayarda Chrome'da yapıldı; iPhone hoparlöründe dinleme Zeyd'de. Bilgisayarda doğru duyulan dengenin telefonda da korunması bekleniyor, çünkü hedefler göreli. Telefon hoparlörü 200 Hz altını keser; çarpma ve kaymanın gövdesi 300 Hz üstünde.
- **Sandık ekranı:** Ayrıca kaydedilmedi. `&sandik=1` kazanma ekranını ilk dokunuştan önce açıyor; webde ses ilk dokunuşla açıldığı için o anda ses yok. Gerçek sandık bölümü 3 ve 5 ise ders ya da kayıt aracının oynayamadığı bölümler. Sandığın sesleri efekt ve fanfar kanallarında, aynı kısma kurallarıyla.
- **iOS uygulaması:** Sınırlayıcı yalnız web'de (WebAudio). iOS'ta kanallar ve kısmalar aynı; sınırlayıcının yerini dosyaların −1,5 dBFS tepe sınırı ve 2 ses sınırı tutuyor.
"""
open(OUT, 'w', encoding='utf-8', newline='\n').write(md)
print('[rapor]', OUT, len(rows), 'senaryo', len(files), 'dosya')
