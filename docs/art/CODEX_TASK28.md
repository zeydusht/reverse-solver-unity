# CODEX_TASK28 — Oyun arayüzünü açılış videosu kalitesine taşı (Zeyd, 9 Kasım)

Zeyd Task26'daki görselleri (hazırlık sayfaları + çekim kareleri) çok beğendi. Yeni hedef: **oyunun bütün arayüz ve oyun içi görselleri o kalitede ve o dünyada olsun.** Ölçüt artık `docs/art/task26/` (mekan-*, aksesuar, kont-pozlar, kahraman-final, cekim-kareleri-v06). Kurallar aynı: yalnız `docs/art/` ve `Assets/Art/`, oyun kodu yok, `PALETTE.md` korunur (değişiklik gerekiyorsa önce öner).

## Önce izle: referans oyunlar
`Level Design/kareler/` içinde Royal Match (`rm/`, 18 temas sayfası) ve Block Out (`blockout/`, 21 sayfa) var, `kucuk.mp4` videoları da. Bunları **aç ve incele**: HUD düzeni, booster çubuğu, hedef/hamle göstergesi, duraklat, kazan/kaybet, "Out of Time +30 sn" teklif ekranı, bölüm başı kartı, zor bölüm uyarısı, harita/ev ekranı. Bizim karakterimizi değil, **işçiliği ve düzeni** referans al (malzeme hissi, kalın kontur, okunurluk, dokunma alanı). Royal Match ve Block Out'un karakter, logo, amblem ve ikonlarını kopyalama.

## Adım 1 — Denetim (yalnız bu, sonra dur)
`docs/art/task28/DENETIM.md` + `docs/art/task28/denetim.png` (temas sayfası):
1. `Assets/Art/` altındaki **kullanımda olan** her arayüz/oyun görselini listele (boosterlar, engeller: bomba/çivi/zincir/duvar, HUD rozetleri, duraklat, ayarlar, kazanma/kaybetme, süre bitti, bölüm kartı, zor bölüm (Kont kartı), harita düğümleri, profil, tutorial eli, ikon, yükleme ekranı…). Hangi ekranlarda kullanıldıklarını yaz (bilmiyorsan dosya adından tahmin et ve işaretle).
2. Her birine not ver: **Uyar / Az uyar / Uymaz** (Task26 stiliyle), tek cümle sebep. Eksik ekranları da yaz (RM/BO'da olup bizde olmayan: ör. süre bitti teklif ekranı, booster açılış kartı).
3. Öncelik sırası öner: oyuncunun en çok gördüğü önce (HUD + boosterlar + engeller → duraklat/kazan/kaybet → menü/harita → kalanlar).
4. Her "Uymaz/Az uyar" grubu için **2 stil seçeneği** küçük örnek (ör. booster çubuğu A/B), Zeyd seçsin.

## Adım 2 — Üretim (Zeyd seçince, grup grup)
- Seçilen stille grubu yeniden çiz. Şeffaf PNG, mevcut dosya adları ve boyutlarıyla birebir (Claude Code entegrasyonu kolay olsun), eski sürüm `Assets/Art/_eski/` altına.
- Engeller (bomba, çivi, zincir, duvar) 64 px'te de okunmalı, renge değil şekle dayanmalı. Bomba sayacı ve çivi sayısı ayrı katman.
- Her grup sonunda `docs/art/HANDOFF.md`'ye entegrasyon isteği, `DECISIONS.md` güncelle, dur.

Teslim (Adım 1): `DENETIM.md` + `denetim.png` + seçenek örnekleri. 3 satır özet.
