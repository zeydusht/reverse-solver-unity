# CODEX_TASK26 — Açılış hikâye animasyonu (storyboard → onay → final)

Önce oku: `docs/ART_DIRECTOR_BRIEF.md`, `docs/HIKAYE.md`, `docs/art/STYLE_CARD.md`, `docs/art/PALETTE.md`, `docs/art/DECISIONS.md` (Baron ve Jiggles için onaylı referanslar). Kurallar aynı: yalnız `docs/art/` ve `Assets/Art/`, oyun kodu yok. Orijinal IP: Jiggles minyona, Baron Gru'ya benzemeyecek.

## Ne
Oyun ilk kez açıldığında, 1. bölümden önce oynayan **15–20 sn'lik (dokunma hariç), atlanabilir** bir hikâye animasyonu. Oyuncu hikâyeyi tek bakışta anlamalı: "Baron Jiggles'ı dondurup tablolara hapsetti. Ben uyanan Jiggle'ım, tabloları sırayla açıp herkesi kurtaracağım." Konuşma yok, en fazla 2 kısa yazı (İngilizce + Türkçe sürüm).

## Storyboard (Zeyd onayladı, 8 Kasım; detayları sen önerebilirsin)
| # | Süre | Sahne |
|---|---|---|
| 1 | 2 sn | Güneşli çayır, Jiggles sürüsü oynuyor, zıplıyor. Sevimli, renkli. |
| 2 | 3 sn | Gölge düşer: Baron gelir. Jiggles'ı toplar, masada yapboz gibi birleştirir. Yerine oturan her Jiggle **donar**, gözleri üzgün. |
| 3 | 2,5 sn | **Kısa görüntü:** Baron bitmiş yapbozu çerçeveletir ve **duvara asar**. Kibirli, komik bir "Masterpiece!" anı. |
| 4 | 2 sn | Şatonun uzun koridoru: duvarda onlarca tablo, içlerinde donmuş Jiggles. Menü koridoruyla aynı mekân. Baron uzaklaşır, ışıklar kısılır. |
| 5 | 3 sn + dokunma | Bir tablodaki **tek bir Jiggle hafifçe kıpırdar**. Animasyon burada durur, o Jiggle parlar ve "Uyandır! / Tap to wake!" yazısı çıkar. **Oyuncu dokunur**, Jiggle uyanır, tablodan "weee!" diye fırlar. **Bu Jiggle oyuncunun kahramanı olur.** (Dokunma 5 sn içinde gelmezse kendiliğinden uyanır.) |
| 6 | 2,5 sn | Kahraman Jiggle koridordaki tablo sırasına bakar, kararlı bir yüz yapar ve ilk tabloya koşar. Yazı: **"Free them all!" / "Hepsini kurtar!"** Kamera ilk tabloya yaklaşır, mevcut koridor zoom geçişine bağlanır, 1. bölüm başlar. |

**Kahraman Jiggle (yeni karakter, Zeyd 8 Kasım):** sıradan bir Jiggle, ama **kendi yaptığı minik savaşçı kıyafetiyle**: şatoda bulduğu eşyalardan derme çatma, eski usul bir şövalye/savaşçı. Elinde **tatlı bir mızrak** var. Öneri: mızrak = **ressam fırçası** (ucunda renk, Baron'un tablolarına karşı sanatla savaşır), miğfer = **yüksük**, kalkan = **bir yapboz parçası** ya da düğme. Ton sevimli ve komik olmalı, korkutucu ya da şiddetli değil. **Telefon boyutunda (64–96 px) bile siluet okunmalı**, detay az olmalı. Başka bir oyun karakterine benzememeli: dikiş iğnesi kılıç ve pelerin birleşimi Hollow Knight'ı (Hornet) çağırır, kaçın. Minyon ve Gru yasağı aynen geçerli. 2–3 seçenek çiz (ör. fırça mızrak + yüksük, kürdan mızrak + şişe kapağı miğfer…). İleride koridor haritasında sıradaki tabloya yürüyen karakter de bu olacak. 3 isim öner.

## Adım 1 — Önce storyboard (ucuz, limit dostu)
- **Kahraman Jiggle referans kartı** (2–3 seçenek, sıradan Jiggle'ın yanında).
- 6 sahnenin her biri için 1 kaba kare (sabit görsel) + her sahnede neyin hareket ettiğini anlatan 1 satır.
- Tek bir temas sayfası: `docs/art/task26/storyboard.png` + `docs/art/task26/STORYBOARD.md`.
- **Burada dur, Zeyd onaylasın.** Final üretime onaysız geçme.

## Adım 2 — Final (onaydan sonra)
- Katmanlı sprite'lar (arka plan, karakter, ön plan ayrı PNG), `Assets/Art/Intro/`. Toplam en fazla **2 MB** (WebGL, iPhone). Video yok: sprite + tween.
- `docs/art/task26/intro_anim.json`: sahne sırası, süreler, her katmanın anahtar kareleri (konum, ölçek, saydamlık, easing), yazılar ve sesin girdiği anlar ("weee" PM_45 ses haritasıyla uyumlu).
- Zeyd'in izlemesi için önizleme: `docs/art/task26/onizleme.mp4` (yalnız inceleme amaçlı, oyuna girmeyecek).
- `docs/art/HANDOFF.md`'ye entegrasyon isteği: yalnız ilk açılışta oynar, "Atla" düğmesi var, ayarlardan tekrar izlenebilir. Bağlantıyı Claude Code yapacak.
- `DECISIONS.md` ve `BACKLOG.md` Art bölümünü güncelle.

## Teslim (Adım 1)
`storyboard.png` + `STORYBOARD.md` + Zeyd'e 3 satır özet: ne değişti, ne soruyorsun.
