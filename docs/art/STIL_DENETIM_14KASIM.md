# Stil denetimi — 14 Kasım gece (PM_65, p22)

Yöntem: p22 build'i başsız Chrome'da 390×844, TR ve EN, `Tools/gif_capture.js` senaryolarıyla gezildi (`shot` adımı). Gezilen ekranlar: isim kapısı, menü, harita (kaydırma), Koleksiyon, Jiggles, Başarılar, Ayarlar, Profil, açılış videosu, bölüm 1 (p22, `c=ic`, cohort'suz), 2, 3 (kilit dersi), 5 (makas dersi), 9 (değnek), 10 (zincir), 12, 14 (duvar), 15 (saat), 20 (bomba), 23 (çekiç, zor bölüm kartı), makas/değnek/çekiç seçim modu, saat animasyonu, kombo x2–x5+, kazanma, sandık, Süre doldu, Durdur. Görüntüler `docs/art/stil_denetim/` (`*_once.jpg` düzeltme öncesi, `*_sonra.jpg` sonrası).

Ölçüt: `docs/PM_MESAJ_65.md` "Onaylı tarz".

| Ekran | Sorun | Görüntü | Durum |
|---|---|---|---|
| Bölüm 1, `c=ic` (test linki) ya da cohort'suz link | **Eski eldivenli el + büyük sarı ok + karartma** ("Parçayı sola sürükle"). Yeni 1. bölüm akışı yalnız `c=p21/p22`'de açılıyordu, diğer girişler eski "Tutorial 1"e düşüyordu. Zeyd'in gördüğü bu. | `01_bolum1_el_ic_once` · `02_bolum1_el_cohortsuz_once` → `01_bolum1_el_ic_sonra` | **Düzeltildi:** `ic` ve cohort'suz giriş de p21/p22 oyunu (`TelemetryMeta.P21Line`, `P21Onboarding.Enabled`); eski el (`TutorialHand`) bu oyunda hiç açılmaz |
| Booster dersleri (5, 9, 15, 23), engel gösterimleri | Spotlight eldiveni ve sarı ok | `04_kart_makas_once` → `17_makas_dersi_sonra` | **Düzeltildi:** p21/p22'de `TutorialSpotlight` eldiven ve büyük ok çizmez; aydınlık şerit ve yazı kalır |
| Spotlight talimat yazısı | Nunito Black afiş (onaylı yazı tipi değil); kilit metni "Sıfır / olunca" diye kötü bölünüyor | `13_ders_kilit_yazi_once` → `13_ders_kilit_yazi_sonra` | **Düzeltildi:** Baloo 2 ExtraBold başlık stili; satır sonu olan metin ikinci kez bölünmez |
| Engel, booster ve "Zor bölüm" kartları | Turkuaz **kurdele** başlıklı eski T23 panel | `03_kart_kilit_once`, `05_kart_bomba_once` → `03_kart_kilit_sonra`, `05_kart_bomba_sonra`, `18_kart_kilit_en_sonra` | **Düzeltildi:** p21/p22'de onaylı krem gövde + mavi çerçeve paneli (`devam_v2/panel`, 9-slice), başlık kurdelesiz 3B yazı (krem dolgu, mavi kontur, altın gölge) |
| Durdur | Kurdeleli turkuaz panel | `06_durdur_once` → `06_durdur_sonra` | **Kısmen:** panel ve başlık yeni dilde; **Ses/Çık düğmeleri hâlâ turkuaz** → T45 Durdur paneli (Codex hazır, onay bekliyor) · **Zeyd'e sor** |
| Sandık | "Sandığa dokun!" yanında **beyaz eldivenli el** (T44 dokunma eli) | `07_sandik_el_once` → `07_sandik_el_sonra` | **Düzeltildi:** el gizlendi; sallanan sandık + yazı kalır |
| Kazanma (EN) | "Chest in 1 levels!" | `12_kazanma_en_once` → `12_kazanma_en_sonra` | **Düzeltildi:** "Chest in 1 level!" |
| Ayarlar | Sürüm etiketi "p20-2026-11-03" (telemetride de `app_version`) | `08_ayarlar_once` | **Düzeltildi:** `p22-2026-11-14` |
| Ayarlar, Profil | Eski turkuaz çerçeve, turkuaz düğmeler; Profil başlığı kurdeleli | `08_ayarlar_once`, `09_profil_once` → `09_profil_sonra` | **Codex'e** (`CODEX_TASK48.md` madde 2) · **Zeyd'e sor** (menü ayrıca onaylanmıştı) |
| Menü sekmeleri (Bölümler, Koleksiyon, Jiggles, Başarılar) | Başlıklar turkuaz **kurdele** üstünde | `10_harita_once` | **Codex'e** (madde 1) · **Zeyd'e sor:** menü TASK19/20'de ayrıca onaylandı; ölçüt "kurdele yok" diyor, hangisi geçerli? |
| Menü üst/alt şerit | Turkuaz (oyun HUD'u old money mavi) | `15_menu` | **Zeyd'e sor** → evet derse Codex madde 5 |
| Booster seçim modu | Başlık plakası ("Sihirli Makas!") turkuaz | `11_makas_secim` | **Codex'e** (madde 3) · **Zeyd'e sor** (T42 PM onaylıydı) |
| Kazanma | "Sonraki Seviye" düz koyu yeşil, diğer yeşil düğmelerden farklı | `16_kazanma_tr` | **Codex'e** (madde 4) |
| Süre doldu | Düz mavi başlık şeridi (kurdele uçlu) | `14_sure_doldu` | Değiştirilmedi: TASK31/32'de onaylı ("düz başlık şeridi") |
| Kilit | Turuncu-kırmızı mühürlü asma kilit | `13_ders_kilit_yazi_sonra` | Uygun: PM_54'te seçilen kilit B (`padlock-b`), beyaz sayı |
| HUD, Jigo, bomba, zincir, duvar, kombo, saat | — | GIF'ler `task29/pm64/` | Uygun |
| Metin taraması | "Baron", "çivi/nail" | — | Ekranda yok (yalnız iç tasarım notlarında) |
| Kombo rakamı | Yazı tiplerinde "×" glifi yok | `task29/pm64/kombo_x2_x5.gif` | "x2" ile yazılıyor; glif için font atlası yeniden üretilebilir (kod işi, küçük) |

## Zeyd'e sor (özet)
1. Menü kurdeleli başlıkları ve turkuaz menü şeritleri kalsın mı, oyun içi dile mi geçsin?
2. Ayarlar/Profil yeni dile geçsin mi (Codex madde 2)?
3. T45 Durdur paneli (yuvarlak Ses/Müzik/Titreşim, kırmızı Çık) bağlansın mı?
4. Booster seçim başlık plakası turkuazdan maviye dönsün mü?
