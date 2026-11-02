# Codex Task22 — GECE İŞİ: Tüm oyunun stil ve UX denetimi + yeniden çizimler + temizlik önerisi

Zeyd: "Oyunu baştan sona defalarca inceleyin. Bizim stile uymayan her şeyi daha iyi çiz, önce/sonra hazırla, onayladıklarımı değiştiririz. Kullanılmayan dosyaları bul, sormadan silme."

Bu bütün gece sürecek bir iş. **Soru sorma.** Emin olmadığın yerde en mantıklı seçimi yap ve `docs/art/GECE_LOG.md`'ye "karar: …" diye yaz. Her saat başı aynı dosyaya 2–3 satır ilerleme notu düş: ne bitti, sırada ne var.

## Kurallar (kesin)
- **Yalnız** `Assets/Art/Redesign_T22/` ve `docs/art/` altına yaz.
- C#, `Assets/_Game`, sahne, prefab ve mevcut onaylı PNG'lere **dokunma**. Oyuna hiçbir şey bağlanmayacak.
- **Hiçbir dosyayı silme, taşıma ya da yeniden adlandırma.** Temizlik yalnız öneri listesi.
- **Eğitim eline dokunma.** Claude Code onu PM_MESAJ_34 ile ayrıca yaptırıyor.
- Stil kaynakları:
  - `docs/art/STIL_REHBERI_RM.md` (önce oku)
  - `docs/art/PALETTE.md`
  - `Assets/Art/References/1.11 ref/`
  - Onaylı Task19D/20/21 menüleri
- Royal Match'ten kopya yok: tarz evet, piksel, ikon ve karakter hayır.

## Kaynak ekranlar
En yeni ekran görüntülerini kullan:
- `docs/qa/2026-11-02-son/`
- `docs/qa/2026-11-02-spot/`
- `docs/qa/2026-11-02-booster/`
- `docs/qa/2026-11-02-ftue/`
- `docs/qa/2026-11-01-p20b/`
- `docs/qa/2026-11-03-tam/` (varsa; Claude Code gece tam tur çekecek)

Tarayıcın internete çıkabiliyorsa canlı oyunu da aç: `https://zeydusht.github.io/reverse-solver-unity/p20/?c=ic&unlock=1&lang=tr`. Açamıyorsan ekran görüntüleri yeterli.

## A. Denetim: 4 ayrı tur, her biri farklı gözle

Her turu ayrı yap, hepsini aynı dosyaya yaz: `docs/art/DENETIM_2026-11-03.md`.

1. **Stil turu:** Her ekranda her öğeyi Royal Match rehberiyle kıyasla.
   - Konturlar ve gölgeler aynı ailede mi?
   - Yazı stili (Nunito Black, krem dolgu, koyu kontur, alt gölge) tutarlı mı?
   - Altın oranı ≤ %10 mu?
   - Düz ve vektör gibi kalan, soluk ya da "eski" görünen parça var mı?
2. **UX turu:** Telefon boyutunda (375×667 ve 390×844):
   - 13 pt altı yazı
   - 44 pt altı dokunma alanı
   - Düşük kontrast
   - Neye basılacağı belli olmayan tuş
   - Kalabalık kart
   - Gereksiz ekran
3. **Tutarlılık turu:** Aynı işi yapan tuşlar her ekranda aynı mı? (yeşil ana, turkuaz ikincil, kırmızı X)
   - Başlık pankartları aynı mı?
   - Kart çerçeveleri aynı mı?
   - Türkçe/İngilizce metin karışıklığı var mı?
4. **İlk kez oynayan gözü:** Yükleme → isim → eğitim → ilk 5 bölüm → kayıp → menü sekmeleri.
   - "Burada ne yapacağım?" dedirten her an.
   - Çirkin, kaba, "cart curt" görünen her şey.

Her bulgu bir tablo satırı:

| # | Ekran | Öğe | Sorun | Önem (yüksek/orta/düşük) | Öneri | İş türü (çizim / kod / metin) |
|---|---|---|---|---|---|---|

Kod ya da metin işi olanları da yaz. Onları PM, Claude Code'a verecek.

**En az şu ekranlar:**
- yükleme
- isim girişi ve profil sorusu
- Ev
- 4 sekme ve alt çubuk
- Koleksiyon ve tablo detayı
- Jiggles ve ilerleme çubuğu
- Başarılar
- Profil kartı
- Ayarlar
- Bölümler haritası
- oyun ekranı: HUD ve booster çubuğu
- tanıtım kartları (çivi, bomba, zincir, duvar, 4 booster)
- zor bölüm uyarısı
- kazanma kartı ve 1–10 kutuları
- kayıp kartları (bomba, süre)
- durdurma kartı
- koridor geçişi
- 40. bölüm bitişi

## B. Yeniden çizim: önce/sonra

Denetimde **yüksek** ve **orta** işaretlenen her **çizim** işi için yeni sürüm çiz:
- **Klasör:** `Assets/Art/Redesign_T22/<sıra>-<kısa-ad>/`
  - Yeni PNG'ler: oyunun kullanacağı boyutta, şeffaf, @3x. Gerekiyorsa 9-slice notu.
  - `once-sonra.png`: solda mevcut ekranın ilgili kırpıntısı, sağda aynı yerde yeni çizim (ekranın üstüne yerleştirilmiş taslak).
  - `NOT.md`: ne değişti, neden, oyunda hangi dosyanın yerine geçecek.
- **Öncelik sırası** (denetim başka şey bulursa onu da ekle):
  1. Kazanma ve kayıp kartları (1–10 kutuları dahil), durdurma kartı
  2. Tanıtım kartları ve zor bölüm uyarısı kartı
  3. Yükleme ekranı ve isim giriş ekranı
  4. Ayarlar anahtarları ve dil seçici
  5. Bölümler haritası düğümleri ve "Oyna" balonu
  6. Koleksiyon "Sıradaki" şeridi ve kilitli hücre
  7. Jiggles ilerleme çubuğu
  8. Koridor geçişindeki yazı ve tuş (varsa)
- **Tek bir inceleme sayfası:** `Assets/Art/Redesign_T22/inceleme.html`
  - Bütün önce/sonra'lar alt alta, her birinin altında numarası ve tek satır açıklama.
  - Zeyd buradan "3, 5, 8 olsun" diye seçecek.

**Kalite:** Her yeni çizimi bitirince kendin tekrar bak. Önceki sürümden net daha iyi değilse yeniden çiz. Bir öğede en fazla 3 deneme yap. Gece boyunca listeyi bitirene kadar devam et.

## C. Temizlik önerisi (SİLME YOK)

`docs/art/TEMIZLIK_ONERI.md`:
- **Neyi tara:** `Assets/Art/` ve `docs/art/` altındaki dosyalar ve klasörler.
  - Her biri için: boyut ve oyunda kullanılıp kullanılmadığı.
  - Kullanım kontrolü: kod, prefab, sahne, manifest ve `Resources`'ta adı ya da GUID'i geçiyor mu? `.meta` GUID'ini `Assets/` içinde ara.
  - Kullanılmayan, iptal edilmiş taslaklar (ör. Task19, Task19B, 19C ara sürümleri, eski stil adayları, `drafts/` içindekiler).
  - `docs/art` içindeki `.task15-python`, `.task16-python`, `.task15-pip-cache`, `.chrome-task6` gibi araç ve önbellek klasörleri.
- **Üç grup:**
  1. **Güvenle silinebilir:** hiçbir yerde kullanılmıyor, onaylı değil, yeniden üretilebilir.
  2. **Arşive taşınabilir:** onaylı geçmiş, referans değeri var.
  3. **Kalmalı.**
- Her satır: yol, boyut (MB), neden.
- En sonda toplam kazanılacak MB.
- **Hiçbir şeyi silme.** PM listeyi kontrol edecek, Zeyd onaylayınca ayrı bir görevle silinecek.

## Sabah teslimi
`docs/art/GECE_LOG.md` sonuna özet:
- kaç bulgu (yüksek / orta / düşük)
- kaç yeniden çizim
- inceleme sayfasının yolu
- temizlikte toplam MB
- Zeyd'in karar vermesi gereken en önemli 5 madde
