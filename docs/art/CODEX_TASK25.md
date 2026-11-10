# Codex Task25 — Tablolar 1–10 yeni stilde + 5 düzeltme + 2 kart düzeltmesi

Zeyd Task24'ün yeni stilini (sade, tek odaklı, animasyon filmi konsepti) **beğendi**. Bütün koleksiyon bu stilde olacak.

**Kurallar:**
- Yalnız `Assets/Art/Paintings_v2/`, `Assets/Art/Redesign_T23/NIHAI/` ve `docs/art/` altına yaz.
- Mevcut `Assets/Art/Paintings/` dosyalarına dokunma.
- Silme yok, oyuna bağlama yok, soru yok.
- **Haftalık limitin az.** Önce A'yı bitir, sonra B'yi, sonra C'yi. Limit biterse yarım kalan işi `GECE_LOG.md`'ye yaz.

## A. Tablolar 1–10, yeni stilde (Task24 ile aynı kurallar)
- Salon teması **1–10 Akdeniz ve kıyı köyü**: mevcut tabloların konusu ama Task24 stilinde.
- 10 farklı konu, her birinde tek bir odak noktası.
- 1–2 küçük Jiggle olabilir.
- Konu listesini `docs/art/T24_KONULAR.md`'ye ekle.
- Yapılacak dosyalar:
  - `painting-01..10.jpg` (1024×1024, ≤ 300 KB)
  - `Source/painting-01..10.png`
- `manifest-v2.json`'a 1–10'u ekle; manifest 40 tablonun hepsini içersin.
- `once-sonra-01-10.png` hazırla. `inceleme.html`'e 1–10'u ekle.
- **Bölüm 1–3 tabloları için ek kural:** Bu bölümler küçük tahta (3×3, 4×4). Her parça ayırt edilsin, büyük düz alan olmasın.

## B. 5 tablonun düzeltmesi (PM bulgusu)
- **21, 23, 24, 30, 40:** Gökyüzü ya da kar alanı çok geniş ve düz. 6×7'ye bölününce birçok parça aynı düz renk oluyor.
- Aynı konu ve kompozisyon kalsın. Yalnız şunları ekle:
  - boş alanlara doku ve renk geçişi
  - ikincil biçimler (bulut kümeleri, kar tümsekleri, uzak ağaç siluetleri, ayak izleri…)
- Parçalanma testini yap: 6×7 ızgarayla kes, her parçada ayırt edilebilir bir şekil ya da renk olsun.
- **Renk çeşitliliği:**
  - 31–40 "gece ve büyü" salonu. Pembe-mor gün batımı yerine gerçek gece mavisi, ay ışığı ve sıcak fener ışığı tonları olsun.
  - Hepsini baştan çizme. En çok pembe kalan 3 tanesini seçip tonunu geceye çek.
- Eski sürümleri `Denemeler/` altında koru.

## C. Kart düzeltmeleri (`Redesign_T23/NIHAI`)
1. `17-isim`: "Adın" yer tutucu yazısı soluk gri olsun (koyu metin gibi görünüyor). Yazılmış isim koyu kalsın. İki halin önizlemesini hazırla.
2. `15-zor-bolum`: Baron sahnesi `07-tanitim-civi`'deki sahneye çok benziyor. Zor bölüme özel bir sahne çiz: Baron kollarını kavuşturmuş, kendinden emin, arkada karışık bir tahta. Aynı Baron, tek karakter.

## Teslim
- `GECE_LOG.md` sonuna özet yaz: ne bitti, kalan limit.
