# Codex görevi P20-D — Yeni eğitim (PM_MESAJ_28 bölüm A)

Kaynak: `docs/PM_MESAJ_28.md` bölüm A (oku). Set: `Assets/_Game/Levels/redesign.json` (SET40 v4; slot 1 = S41, `tutorial: 1`).

## Dokunma
`Assets/_Game/Levels/*` (redesign.json dahil), `Core/Rules`, `TravelRule`, `GreedySolver`, telemetri satır şeması (yeni sütun YOK), `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`. Commit atma. `Library/ Logs/ Builds/` okuma. Geçici dosyaları repo köküne değil `%TEMP%`'e yaz.

## Bugünkü durum
- `Core/Session/Tutorial.cs` (`Step`, `StepsOf`), `Presentation/Board/TutorialHand.cs` (el + ok, eldiven hep yukarı bakıyor: "PM_MESAJ_8 upright" — Zeyd'in "el yukarıyı gösteriyor" şikâyeti büyük olasılıkla bu), `GameRoot.ShowTutorialStep()` (+ çağrıldığı yerler ~569, ~707–714), `_bar.Notice(Texts.TutorialFirst/Next)`.
- Tanıtım kartları: `Screens.Intro`, `_introOpen` (kalır).
- Saat: `GameRoot.Update` içinde `_session.Tick` (`!_introOpen && _card == null && _list == null`).

## 1. Veri (A4)
`Assets/_Game/Presentation/Resources/tutorial.json`:
```json
{ "steps": [
  { "slot": 1, "adim": 1, "tur": "surukle", "parca": 3, "metinAnahtari": "drag" },
  { "slot": 1, "adim": 2, "tur": "surukle", "parca": <S41 çözümünde 2. sıradaki parça>, "metinAnahtari": "drag" },
  { "slot": 1, "adim": 3, "tur": "kural",   "metinAnahtari": "rule" },
  { "slot": 3,  "adim": 1, "tur": "goster", "engel": "nail",  "metinAnahtari": "nail" },
  { "slot": 6,  "adim": 1, "tur": "goster", "engel": "bomb",  "metinAnahtari": "bomb" },
  { "slot": 10, "adim": 1, "tur": "goster", "engel": "chain", "metinAnahtari": "chain" },
  { "slot": 14, "adim": 1, "tur": "goster", "engel": "wall",  "metinAnahtari": "wall" } ] }
```
- `slot` = setteki sıra (1 tabanlı). `parca` = parça indeksi. **Yön JSON'da yok**: `Level.Solution`'da o parçanın `Dir`'i (A1). Adım 2'nin parçası, adım 1'den sonra o yönde gerçekten çıkabiliyor olmalı (`session.Probe(piece, dir).Exit`); değilse çözümde çıkabilen ilk parçayı seç ve bunu rapora yaz.
- `goster` adımında `engel`, o bölümde engelin bulunduğu ilk hücre: nail → `nails`, bomb → `bombs`, chain → zincir verisi seviyede nasıl tutuluyorsa, wall → `sealed` kenar. Seviyede yoksa adımı atla ve raporla.
- Okuma: `Presentation` içinde küçük `TutorialScript` sınıfı (Unity `JsonUtility` yeter).

## 2. Metinler
`Texts.cs`'e (`Lang.T(en, tr)`): drag yönü (`Drag the piece to the left/right/up/down` | `Parçayı sola/sağa/yukarı/aşağı sürükle`), rule (`Pull every piece out without hitting other pieces!` | `Tüm parçaları diğer parçalara çarpmadan dışarı çek!`), nail/bomb/chain/wall (PM_MESAJ_28 A3 tablosu, birebir). Aynı metinleri `docs/i18n/en.md`'ye yeni bir "Tutorial" tablosu olarak ekle (kaynak orası; kod onunla aynı).
- **Kural doğruluğu:** oyun koduna (Core: GameSession / nail / bomb / chain / sealed) bakarak her cümle doğru mu kontrol et (çivi gerçekten hamleyle mi açılıyor, zincirli parçalar gerçekten birlikte mi çıkıyor, duvar geçişi engelliyor mu). Yanlışsa metni kurala uyacak şekilde düzelt ve raporda yaz.

## 3. Sürükleme adımı (A2, `tur: surukle`)
Yeni `Presentation/Board/TutorialSpotlight.cs` (ya da TutorialHand'i yeniden yaz):
- **Örtü:** ekran ~%60 koyu (`#000` α .6), hedef parçanın hücreleri + çıkış yönünde tahta kenarına kadar şerit (dikdörtgen) aydınlık: deliği 4 dikdörtgenle çiz (`ShapeView`). Sorting: tahtanın üstü, HUD ve kartların altı.
- **El:** `hand-swipe` görseli (Art.Place). Döngü (~1,6 sn): parçaya iner (basma: hafif küçülme), çıkış yönünde tahta kenarına kadar kayar, solar, başa döner. Eldiven yönü: parmak ucu parçada; yatay yönlerde de hareket yönü net olsun (gerekirse görseli döndür; "hep yukarı" kalmasın).
- **Ok:** elin arkasında, parçadan kenara uzanan yarı saydam kalın ok (ShapeView şerit + üçgen uç; renk krem/sarı α .55).
- **Yazı:** ekranın altında (tahtanın altı, booster çubuğunun üstü ya da yerine) `TextView ... .Banner(TextView.OnTeal)`, ~24 pt, `FitWidth(screen.x - 32, 14)`.
- **Girdi:** adım açıkken yalnız hedef parça sürüklenebilir. Başka yere dokunuş → el animasyonu baştan, hata sesi yok, **session'a komut gitmez** (jam sayılmaz). Hedef parça yanlış yöne çekilirse: session'a göndermeden parça küçük yatay/dikey sallanır (0,25 sn) + el baştan. Doğru yön → normal hamle (session'a gider, normal kaydedilir).
  - Sürükleme girdisinin session'a gittiği yeri (`_drag`, `DragController` ya da BoardView) bul; reddetme oraya, mümkün olan en erken noktaya.
- **Başarı:** parça çıkınca kısa efekt (ör. hedefte 0,3 sn parlama/ölçek), örtü 0,2 sn'de kalkar, sonraki adım.
- **Saat:** eğitim adımları (kural kartı dahil) bitene kadar `_session.Tick` çalışmaz.
- Retry'da (aynı bölüm yeniden) eğitim yeniden gösterilir; `?debug=1&lv=1` ile de açılır.

## 4. Kural kartı (`tur: kural`)
Hafif örtü (α .35) + ortada kart (`t20-modal-panel` ya da mevcut kart stili), metin `Banner` değil, krem kart içinde lacivert Title font (`MenuUi.Label(..., dark: true, titleFont: true)`), ~20 pt, iki satıra sığsın (gerekirse `\n`). Altında küçük çizim: iki mini parça (ShapeView kare), birinin yolu açık + yeşil tik, diğerinin yolu kapalı (önünde başka parça) + kırmızı çarpı; mevcut `t20-icon-green-check` varsa onu kullan. Dokununca kapanır, serbest oyun.

## 5. Engel adımı (A3, `tur: goster`)
Tanıtım kartı kapandıktan sonra: örtü %60, engelin hücresi aydınlık (yuvarlatılmış kare delik, hücre + 6 pt), altta metin (Banner), dokununca geçer. Hamle zorlaması yok. Süre bu adım bitene kadar başlamaz. Bölümün ilk açılışında bir kez (tanıtım kartıyla aynı kural: `_introSeen`).

## 6. Eski kodun temizliği
Eski el + `_bar.Notice(TutorialFirst/Next)` slot 1 için kalkar (yeni sistem yerine). `Tutorial.Hint` / web portu testleri bozulmasın (Core'a dokunma; yalnız Presentation'dan çağırmayı bırak). `tutorial: 2` gibi başka bölümlerde eski el varsa, yeni sistemde karşılığı yoksa eski davranış kalsın.

## 7. Test
`Assets/_Game/Tests/EditMode/TutorialScriptTests.cs` (Core.Tests asmdef Presentation'ı görmüyorsa JSON'u test içinde `System.IO.File` + basit ayrıştırma ile oku): her `surukle` adımı için redesign.json'dan bölümü yükle, önceki adımları uygula, beklenen yön = çözümdeki yön ve `Probe(...).Exit == true`. `goster` adımlarında engel hücresi var.

## Bitince
Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6). KISA rapor: dosyalar, adım 2'nin parçası ve yönü, kural doğruluğu sonucu, eksikler.
