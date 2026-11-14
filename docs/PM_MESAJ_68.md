# PM_MESAJ_68 — 11–20 bomba duvarını kır + sandık ekranı yeni dile (p22)

Zeyd onayı (14 Kasım 12:16). Kodu kendin yaz. Yalnız p22. Önce `docs/` commit.

## Veri (pt1 gerçek, ilk deneme kazanma; yalnız 11'e ulaşan iyi oyuncular)
11 S21 %29 · 12 S51 %71 · 13 S34 %60 · 14 Z21 %94 · 15 S13 %94 · 16 Z04 %43 · **17 S18 %14** · 18 S22 %69 · **19 S28 %0 (0/13)** · 20 S24 %33.
PM_67 FunSim (Kolay profil) aynı yönde ama daha kötümser (11: %31, 17: %23, 19–20: %0). Sorgu: `v_attempts` cohort pt1, attempt=1.

## 1. Simülasyonu gerçeğe kalibre et
FunSim'i (ya da bir "orta oyuncu" profili) pt1 ilk deneme oranlarına kalibre et (1–20, aynı kimlikler). Kalibrasyon hatasını raporla (bölüm başına fark, ortalama mutlak hata).

## 2. Hedefler (kalibre edilmiş simülasyonla)
- 11–20 normal bölüm: ilk deneme **%55–75**.
- "Zor bölüm" (kartlı): **%35–45**, 11–20 içinde en çok 2 tane, **art arda iki zor yok**.
- Dersler (10 zincir, 14 duvar) ve 8–9 yumuşak bölümler mevcut hâliyle (≥%85).

## 3. Ayarlar
- **11 S21, 17 S18, 20 S24:** önce bomba sayaçlarını cömertleştir (+2…+4), gerekirse süreyi artır. Tahta düzenine dokunmadan hedefe ulaşmaya çalış.
- **19 S28 (4 bomba, %0):** 30'lu bölümlerin sonuna taşı (ör. 38–40 arası, "zor bölüm" olarak). Yerine 1–40 setinden hedef aralığa uyan, bombası en çok 1 olan bir bölüm getir (kimlik değişirse kayıt/sayım uyumluluğunu koru, PM_59/64'teki sürüm kuralı).
- 16 Z04 (%43): "zor bölüm" kartı varsa kalsın, yoksa hedefe çek.
- Değişen her bölüm için yeni sürüm numarası (telemetri karışmasın).
- Sonuç tablosu: 1–20, önce/sonra simülasyon oranı, pt1 gerçek, değişiklik.

## 4. Sandık ekranı
Eski lacivert sandık ekranını kazanma ekranıyla aynı dile getir: krem kart + mavi çerçeve (`devam_v2/panel`), 3B "Sandık!" başlık, krem üstüne ödüller, katmanlı yeşil "Topla". Yeni çizim gerekiyorsa Codex için `docs/art/CODEX_TASK49.md` yaz (çalıştırma).

## Teslim
p22 güncel, EditMode yeşil (yeni hedef testi: 11–20 aralıkları), `verify_build` p22 4/4. Ekran: sandık ekranı TR/EN. Rapor "PM_68" + sonuç tablosu, PROJE_DURUMU'na tek satır, commit + push.
