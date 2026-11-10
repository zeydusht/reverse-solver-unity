# CODEX_TASK32 — HUD final rötuşu (TASK31 onaylandı)

Zeyd: **"Aferin, bu menü oldu."** TASK31'in yerleşimi, katmanları, booster/rozet stili, Jigo'nun tedirgin, korku, üzgün ve mutlu hâlleri, kilit B ve zincir **onaylandı**. Bunları bozma, yalnız aşağıdakileri değiştir. Aynı kalite kuralı geçerli: referansla yan yana kırp, renkleri ölç, `docs/art/task32/denetim.md`.

## 1. Şerit rengi: M1 → referans maviye kayan gradyan
Old money (M1) seçildi, ama dikey geçiş daha belirgin olsun: **üstte koyu old money, altta RM mavisine yaklaşan.**
- Üst şerit: üst `#112C55` → orta `#173F78` → alt `#1D5499`.
- Alt şerit: aynı ailede (üst kenarı biraz açık, aşağı doğru koyu ya da tersi; hangisi tuşları daha iyi öne çıkarıyorsa, iki örnek göster).
- Panel çerçeveleri ve kurdeleler bu yeni şeritle uyumlu tona çekilsin.

## 2. Panel başlıkları: eğimli kurdele kalkıyor
- "Parça" ve "Süre" başlıklarının arkasındaki **eğimli/kemerli kurdele RM'ye birebir olmak zorunda değil**, bizde iyi durmadı. Yerine **düz, yatay bir başlık şeridi** tasarla (ince altın üst/alt çizgi olabilir, renkler serbest). Yaratıcı ol, ama sade ve okunur olsun.
- **Başlık yazısı büyüsün ve okunsun:** şu an çok küçük. Kalın, yuvarlak font (Baloo 2 ExtraBold ya da Fredoka Bold), krem dolgu, koyu kontur. Başlık şeridinin ~%60'ı yüksekliğinde.
- Aynı düz şerit **"Süre doldu" panelinin başlığında** da kullanılsın.

## 3. Panel rakamları küçülsün
`24` ve `0:42` hâlâ büyük. Özellikle **dikeyde** fazla. Rakam yüksekliği gövdenin **~%34**'ü olsun (şimdi daha fazla). Yatay ve dikeyde tam ortalı. Sol paneldeki parça ikonu rakamla aynı boyda.

## 4. Jigo "normal" hâli
Kaşlar şimdi iyi. Ama ağız fazla açık, **sırıtıyor**. Normalde ağız kapalı ya da çok az açık, **tatlı ve yumuşak bir gülümseme**. Göz kırpma kalsın. Diğer 4 hâle dokunma.

## 5. Ayarlar tuşu → Durdur tuşu
Dişli yerine **durdur (pause, iki dikey çubuk)** ikonu. Krem, kalın koyu kontur. Mavi gövde ve halkalar aynı. Oyuncu "durdur nerede?" demeden bulsun. (Menü içinde seviye, ses, çık yine var.)

## 6. Booster ikonlarını RM renkleriyle yeniden boya
**Şekiller aynı kalır, yalnız renk ve katman.** RM'nin yeşil tuş üstünde iyi çalışan renkleri (`ref_alt.png`'den ölçüldü, kendin de ölç):
- **Kırmızı:** `#D02030` / `#C02030` / gölge `#B01020` (palyaço şapkasının sol yanı)
- **Mor:** ışık `#A030F0`, orta `#8010C0`, gölge `#7010B0` / çekiç başı `#7040F0`–`#5030D0`
- **Sarı:** `#F0C010` / `#F0B000` / gölge `#E09000`, parlak `#F0E030`
- **Koyu:** top gövdesi `#202040` / `#303060`

Uygulama:
- **Makas:** kulpları RM kırmızısı, ağız gümüş, vida sarı.
- **Sihirli değnek:** çubuk RM moru, yıldız RM sarısı (net okunacak).
- **Çekiç:** tek renk mavi yerine **katmanlı**. Öneri: baş RM moru, iki ucunda kırmızı kapak, ortasında sarı bant, sap krem/ahşap. Mavi kalacaksa en azından sarı bant ve krem uçlarla katmanla. İki seçenek göster.
- **Saat:** gövde/kasa koyu (`#202040`–`#303060`, topun gövdesi gibi), kadran krem, ziller ve ayaklar RM sarısı.
- Her ikonda RM gibi: üstten ışık, alttan gölge, kalın koyu kontur, küçük parlama lekesi.

## 7. Kilit ve zincir
- Kilit B onaylı. Oyundaki boyutu hücrenin ~%55'i olsun (önizlemede büyük görünüyordu); 64 pt hücrede örnek göster.
- Zincir onaylı (p20 zinciri). Ek çizim gerekmez.

## 8. Süre doldu paneli
- Başlık düz şerit (§2). "+30 sn" ve satın al tuşu (içinde altın + 900) aynı kalsın, beğenildi.
- Sağ üstteki **X kalsın.** Ek olarak satın al tuşunun altına küçük, sade bir "Hayır, teşekkürler" metin tuşu **seçenek olarak** çiz (iki versiyon göster: yalnız X / X + metin tuşu). Karar sonra.

## Teslim
- `docs/art/task32/onizleme.png`: final oyun ekranı (390×844) + RM yan yana; booster ikonları önce/sonra; çekiç 2 seçenek; Jigo normal önce/sonra; Süre doldu paneli 2 versiyon.
- Dosyalar `Assets/Art/_yeni/HUD32/` (yalnız değişenler; değişmeyenler HUD31'den kullanılacak, listesini yaz).
- 3 satır özet, **dur**.
