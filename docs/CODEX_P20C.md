# Codex görevi P20-C — Task21 parçaları (PM_MESAJ_27 bölüm B)

Kaynak: `docs/PM_MESAJ_27.md` bölüm B, `Assets/Art/Menu/Task21/HANDOFF.md` + `manifest.json` (ölçüler: px ÷ 3 = pt; referans ekran 375×812). Önizleme: `Assets/Art/Menu/Task21/onizleme.png`, `Preview/`. Kesin kutular: `docs/art/task21/preview-layout.json`.

## Dokunma
`Assets/_Game/Levels/*`, `Core/`, telemetri, kazanma akışı, `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`. Commit atma. `Library/ Logs/ Builds/` okuma. A bölümünde yapılanları bozma: `TextView.Banner` / `MenuUi.Banner` (Nunito Black, `Fonts/Title`) stili, `MenuScroll` ataleti, Jiggles'ta renk sayaçlarının OLMAMASI (önizlemede var ama PM kaldırdı; sayı koyma), sekme çubuğu zemininin ekranın altına kadar inmesi.

## 0. Import
`Tools/art_choice.py`'ye Task20'deki gibi Task21'i ekle: `isRuntime != false` her PNG `t21-<dosya>` adıyla `Resources/Art/`'a; `sliceLTRB` [L,T,R,B] → borders [L,B,R,T]. `Assets/_Game/Editor/MenuArtImport.cs`'te t21 için t20 olmayan-sahne kuralının aynısı (PPU 3, 4'ün katına dolgu + crunch, sıkıştırmasız yapma). Sonra `python Tools/art_choice.py`.

## 1. Alt çubuk v3 (`Presentation/Ui/MenuTabs.cs`, `MenuTabs` sınıfı)
- Zemin `t21-tabbar-bg` (9-slice) çubuk üstünden ekranın altına; çubuk yüksekliği 86 pt **alt güvenli alan dahil**, güvenli alan 34'ten büyükse zemin aşağı uzar. Yani `Top = screen.y - Mathf.Max(34, safe.w) - 52` (ikon/etiket güvenli alanın üstünde), zemin `Top`'tan `screen.y`'ye.
- Yuvalar: seçili 2 ağırlık, diğerleri 1 (375'te 150/75/75/75 pt; genişlikler `screen.x`'e oranla). `t21-tab-slot` / `t21-tab-slot-selected` şeridin (ilk 10 px = 3.33 pt) altından ekran altına; aralarına `t21-tab-divider`.
- İkonlar yazısız: normal `t21-tab-icon-<key>-normal` 49 pt yükseklik, seçili `t21-tab-icon-<key>-selected-large` 61.33 pt yükseklik ve yukarı taşar (375×812'de normal üst y729, seçili üst y693; çubuk üstü 726). Genişlik görselin oranından.
- Yalnız seçili sekmenin etiketi, `MenuUi.Banner` ile, seçili ikonun altında (375×812'de merkez y765).
- Seçim değişince 0,15 sn tween: yuva genişlikleri, ikon merkezleri ve seçili ikon boyu birlikte enterpole (`Tweens.Run`). Mevcut `Press(index, done)` akışına bağla (tween bitince `done`).
- Rozet `t21-tab-badge` (boş), sayı ayrı `MenuUi.Label` ile; kupa ikonunun sağ üstü.
- `Hit(pt)`: yeni yuva sınırlarına göre.
- Eski `nav-bar-base`, `nav-slot-*`, `nav-bubble`, `nav-icon-*` kullanımı kalkar.

## 2. Sol üst (Ev ekranı, `Presentation/Ui/Screens.cs` → `Task13`, yalnız `Art.On`)
- Kare avatar: `t21-profile-square-frame` 90 pt (dış kutu sol 8 pt; merkez y = `top + 45` olsun, `top` zaten güvenli alan üstü + 4 pt); içinde mevcut `AvatarTile` portresi 48 pt + daire 53 pt (çerçevenin altında, çerçeve üstte çizilir; çerçevenin ortası saydam).
- İsim plaketi `t21-profile-name-plate` (9-slice) karenin sağında, aynı merkez y; genişlik isme göre (en az 70, en çok kalan alan); isim krem `MenuUi.Label`, uzunsa küçülür (mevcut `FitWidth` mantığı, "Zeynep Yılmazoğlu" sığmalı).
- Sayaç: `t21-counter-pill-v2` krem hap 30 pt yükseklik, Jiggle ikonu (mevcut `jiggle-counter`) 46 pt soldan taşar; sayı lacivert, konturSUZ (HANDOFF: sayaç sayıları lacivert) — A'da `Banner` yapılmıştı, krem hapta lacivert düz yazıya çevir.
- Dişli mevcut `m13-settings-button` aynı merkez y.
- `Profile`, `Settings` dokunma kutularını yeni yerlere güncelle. Plakete ya da kareye dokunmak Profil kartını açar (bugünkü gibi).

## 3. Avatarım (`Presentation/Ui/JigglesScreen.cs`, `Avatar(...)`)
- Katmanlar aynı tuvalde (180 px = size): `t20-avatar-disc` → `t21-avatar-<renk>-round` (zaten maskeli, %78; tam `size` ile çiz, ayrıca büyütme/maske yapma — A'daki daire kesme kodunu kaldır) → `t21-avatar-ring` ya da seçiliyse `t21-avatar-ring-selected` → seçiliyse `t21-check-badge` (size×48/180, halkanın sağ altı).

## 4. Kilometre taşları (aynı dosya)
- `milestone-rail` kalkar. 3 madalyon `t21-milestone-medal-{frame,badge,portrait}` (ulaşılmadıysa `-locked`; kilit resimde var, ayrıca kilit/check koyma), 52 pt; merkezler 375'te x 67/187/307 (ekran genişliğine oranla), aralarda `t21-milestone-dots` (70 pt). Altında eşik (`Texts.Count`) ve ödül adı (`Texts.Milestone`) mevcut gibi.

## 5. Ayarlar (`Presentation/Ui/MenuSettings.cs`)
- Panel `t21-settings-panel` (9-slice), X `t20-button-close` sağ üst köşeden taşar.
- Ses / Müzik / Titreşim: her satır `t21-settings-row` krem karo; solda `t21-settings-icon-disc` + `t21-settings-symbol-{sound,music,vibration}`; etiket lacivert Title font (`titleFont: true`); sağda mevcut `t20-toggle-*`.
- Dil satırı mevcut gibi.
- Gizlilik ve Bize yaz: yan yana iki 54 pt kare `t21-settings-icon-btn` (basılınca `-pressed`, ikon da 2 pt aşağı), içinde `t21-icon-privacy` / `t21-icon-mail`, altlarında küçük lacivert etiket. İşlevler aynı (OpenURL / mailto).
- Sürüm yazısı altta.
- Panel başlığı "Ayarlar" panelin üstünde `MenuUi.Banner`.

## Bitince
Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6). KISA rapor: değişen dosyalar, varsayımlar, eksikler.
