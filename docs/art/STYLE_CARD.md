# Reverse Solver — Stil Kartı v0.2

Durum: 23 Ekim 2026, aday 2 çalışma stili; Baron aday 3 Zeyd tarafından seçildi. Yeni referans kartları ve ikon adayları inceleme için hazırdır.

Görsel dayanaklar: Assets/Art/Ornekler/StilAdaylari/Jiggles/2.png ve Assets/Art/Ornekler/StilAdaylari/Koridor/2.png. Önceki örnekler korunur. Bu seçim karakter referans kartlarının veya her mimari ayrıntının onayı anlamına gelmez.

- Görünüm: üst düzey 3D animasyon filmi hissi; organik, hafif asimetrik formlar, yumuşak oyuncak yüzeyler ve kontrollü parlaklık. Geniş renk alanları ve sade siluetler. İlk örnekteki yoğun plastik parlaklığı yerine daha yumuşak yüzey geçişleri.
- Palet: koyu petrol #123F46, turkuaz #48BDB2, sıcak turuncu #F39449, altın #E3B65A, krem #F4E8CF, mürekkep #19252D. Bunlar başlangıç renkleridir; telefon testiyle doğrulanacak.
- Işık: geniş sıcak ana ışık, serin dolgu ve yumuşak gölgeler; sinematik derinlik, sınırlı parlama. Menüde atmosfer kullanılabilir; oyun alanında kenarları örten bloom yok.
- Kenarlar: yuvarlatılmış hacimler; parça dış sınırları ve bağlantı çıkıntıları kesintisiz, belirgin koyu konturla okunur. Kesin kontur kalınlığı gerçek telefon boyutunda belirlenecek.
- Jiggles geometrisi (Zeyd, 23 Ekim): düzenli kare taban ve eşit köşe yarıçapları. Her kenar düz kalabilir veya tam orta noktasında tek çıkıntı/oyuk taşıyabilir; kaymış bağlantı olmaz. Bağlantı boyutları tutarlı tutulur. Organik asimetri yüz, uzuv ve pozlarda kullanılabilir; yapboz gövdesi eğriltilmez. Önden geometri denemesi: Assets/Art/Ornekler/StilAdaylari/Jiggles/2-ortali-v01.png (henüz nihai onaylı referans değil).
- Uzuv ayrımı (Zeyd, 23 Ekim): yapboz çıkıntısı sabit gövde geometrisidir; kol/el değildir. Her Jiggle iki ayrı kol/el ve iki ayrı bacak/ayak taşır. Kollar bağlantılardan aşağıdaki gövde noktalarından çıkar; çıkıntıdan filizlenmez. Pozda el, kol ve yapboz bağlantısı üst üste binerek tek uzuv gibi görünmez; aralarında görünür boşluk ve ayrı siluet korunur. İki saç telli demodaki mercan karakterin bağlantıya karışan kolu kabul edilecek örnek değildir.
- Güncel Jiggles yönü: Zeyd ekstra tatlı Minyon referanslı ifade ve tulum denemelerini iptal etti. Meraklı/büzülmüş ağızlı 4-merakli-v01.png ve kıyafetli 5-tulum-v01.png ana referans olarak kullanılmaz. Aday 2'nin önceki yüz ailesi ve onaylanan merkezli geometri / ayrı uzuv kuralı korunur. Saç demosundan nihai seçim yapılmadı.
- Tablolar: orta kontrast, büyük biçimler, az detay. Tuzaklar şekil ve ton farkıyla da ayırt edilir; yalnız renge güvenilmez.
- Jiggle: kalın, yumuşatılmış yapboz gövdesi; orta boy, kırık beyaz göz aklı gözler, ifadeli göz kapakları ve belirgin kaşlar, hafif asimetrik gülüş; kısa esnek uzuvlar ve fasulye biçimli eller. Neşeli ve biraz sakar. Aşırı büyük bebek gözleri, sarı kapsül, gözlük ve tulum kullanılmaz. Kesin oranlar ana referans kartında sabitlenecek.
- Ortam: koleksiyoncunun sıcak, teatral şatosu; kıvrımlı duvarlar, organik kemerler, iri oyma ahşap ve sıcak pirinç/altın ayrıntılar. Petrol duvarlar ve mercan vurgular. Yoğun menü ayrıntısı oyun tahtasına taşınmaz.
- Güncel menü seçimi: Zeyd Review/Koridor/2.png'yi seçti ve son düzen revizyonu olarak `2-ortali-yan-blok-v04.png` sürümünü açıkça seçti. Ana duvar bloğunda tek büyük tablo bulunur ve iki sütun arasındaki duvar düzlemine göre ortalanır; sağdaki komşu blokta küçük tablo kalır. “Tek çerçeve” tüm sahne değil, ana blok için geçerlidir. Hedef akışta kamera geri çekilirken mevcut tablo sağa kayıp yan blokta küçük görünür, yeni level ana blokta sunulur. `LEVEL_GECISI.md` içindeki 24 Ekim notu kısa konsept animatiğini, gerçek katmanlı tablo geçişinin ise henüz yapılmadığını açıklar.
- Baron: ONAYLI tasarım dayanağı Ornekler/Baron/3.png. Yana dalgalı koyu mürdüm saç, belirgin favoriler, kısa geniş burun, petrol yelek, bordo astarlı altın kenarlı uzun siyah palto, uzun ince siluet. Ana öfke ifadesi özellikle aday 3'ün sağ alt yüzüdür: derin V yapan güçlü çatık kaşlar ve komik öfke. Kel kafa / uzun burun / çizgili atkı birleşimi kullanılmaz. İsim henüz kesin değil.
- Asla: fotogerçekçilik, ince görsel gürültü, sert korku estetiği, başka IP'leri taklit eden karakter/ses, oyun tahtasında yoğun efekt.
- Teknik: arka plan uzun kenarı en fazla 2048 px; UI atlasları; hafif sprite/tween animasyonları. Sıkıştırma ve atlas boyutları mühendislikle doğrulanacak.

Her karakter için onaylı tek ana referans kullanılacak. Ön/yan/üç çeyrek görünüş, oranlar, ifadeler ve renkler referansta sabitlenecek. Sonraki üretimlerde bu referans ve stil kartı birlikte verilecek.

23 Ekim — Son karar: tulum ve ekstra tatlı ifade yönleri iptal. Geçmiş dosyalar arşiv olarak korunur; önceki tulum deneme izni artık geçerli üretim yönü değildir.

## Tek çalışma referansı / karakter

Onaylı ikon görseli: Assets/Art/Review/Ikon/2-market-mavi-v01.png. Son kırmızı/sarı/yeşil ikon denemesi seçilmedi; turkuaz/mercan/lavanta kimlik renkleri sürer. Bu seçim mevcut karakter kartlarını değiştirmez.

Aktif dosya/koruma durumları için REFERANS_DURUMU.md esastır. Açıkça iptal edilen meraklı ifade ve tulum ile hatalı Jiggles v01 yalnız doğrulanmış yedekte tutulur; artık aktif klasörlerde yoktur. Yeni Review çıktıları onay alınmadan bu kartların yerine geçmez.

- Jiggles: Assets/Art/Characters/Jiggles-reference-v02.png. Saçsız ve kıyafetsiz başlangıç standardı; yeni saç kararı verilmedi. v01 anatomik hata nedeniyle kullanılmaz. Normal yüz ailesi ve ayrı uzuvlar geçerli. Kartın yeni sunumu Zeyd değerlendirmesine açık.
- Baron: Assets/Art/Characters/Baron-reference-v01.png. Aday 3'ten türetildi; dört görünüş ve dört ifade. Kartın yeni sunumu Zeyd değerlendirmesine açık; karakter seçimi onaylı.
- Sonraki üretimlerde bu kartlardan ilgili olanı girdi olarak ver; yalnız metinle benzer karakter üretme. Eski adaylar arşivdir. Kartlar 3D model/rig veya teknik sprite değildir.
