# Codex PM_83 — Tablolar 41–100'e Jiggle ekle

Bu görev tek seferde bir parti içindir. Komut hangi partiyi istiyorsa (ör. "41–50") yalnız onu yap.

## Sorun
1–40'taki tabloların çoğunda küçük, sevimli bir Jiggle var: tablonun ana nesnesiyle bir şey yapıyor (fener yakıyor, elmayı sepete yuvarlıyor, kızakla kayıyor). 41–100'de Jiggle yok (TASK51 tarifinde "karakter yok" yazıyordu, bu hataydı).

## Ne yapılacak
Her tabloya **bir** Jiggle ekle (tablo 100'de birden çok). Önce **düzenleme** dene: mevcut `Assets/Art/Paintings_v2/Source/painting-NN.png` (yoksa `painting-NN.jpg`) girdi, yalnız Jiggle eklenir, geri kalan her şey aynı kalır (kompozisyon, renk, ışık, nesneler). Düzenleme tutmazsa (Jiggle yapışık, yanlış ölçek, sahne bozuldu) aynı konuyu yeniden çiz ve bunu loga yaz.

Görsel üretim aracına girdi olarak ver:
1. düzenlenecek tablo;
2. `Assets/Art/Characters/Jiggles-reference-v02.png` (yalnız karakter kimliği: yumuşak kare yapboz gövde, ortada çıkıntılar, gövdenin altından çıkan iki ayrı kol, iki ayrı bacak, tüysüz, giysisiz, büyük kahverengi gözler, gülen ağız);
3. ölçek ve stil için `Assets/Art/Paintings_v2/painting-17.jpg` ve `painting-26.jpg` (Jiggle'ın tablo içindeki boyu ve boyama dili).

## Jiggle kuralları
- Boyu tablonun yüksekliğinin **%13–17'si** (1–40'taki gibi; 41–60'ta %9–11 kaldı, biraz küçüktü). Ön ya da orta planda, kolayca görülen yerde; kenara yapışık değil, kesilmemiş.
- Tablonun ana nesnesiyle **bir iş yapsın** (aşağıdaki listedeki eylem). Mutlu, meraklı yüz.
- Renk aşağıdaki listedeki gibi; düz, doygun, matte (referanstaki turkuaz, mercan kırmızısı, mor; ek olarak sarı ve pembe).
- Aynı boyama dili: matte guaj, yumuşak ışık, sahnenin ışık yönüne uyan gölge. Çizgi, yazı, çerçeve yok.
- Puzzle kuralı korunur: 6×6 ve 8×8 kesimde düz renk parça olmasın.

## Liste (no · konu · Jiggle'ın eylemi · renk)
41 Vaha kuyusu · kuyudan kovayı ipiyle çekiyor · turkuaz
42 Kervan çadırı · minderde oturup çay döküyor · mercan
43 Kum tepesi feneri · fener kapısının önünde el sallıyor · mor
44 Baharat tezgâhı · kepçeyle baharat tepesinden alıyor · sarı
45 Kaktüs bahçesi · küçük sulama kabıyla kaktüsü suluyor · pembe
46 Mozaik çeşme · çeşmenin kenarında oturmuş, eli suda · mercan
47 Uçan halı dükkânı · süzülen halının üstünde oturuyor · turkuaz
48 Kızıl kaya kemeri · sırt çantasıyla kemerin altından yürüyor · mor
49 Hurma hasadı · hurma sepetini taşıyor · turkuaz
50 Çöl gözlemevi · gözlemevi kapısında dürbünle bakıyor · sarı
51 Mercan bahçesi · mercanlar arasında yüzüyor, küçük kabarcıklar · sarı
52 Batık gemi pruvası · pruvanın üstünde oturmuş · mercan
53 Işıklı balık mağarası · küçük ışıklı balıkların yanında · turkuaz
54 İstiridye incisi · inciyi iki eliyle tutuyor · mor
55 Kaplumbağa yolu · kaplumbağanın sırtında · mercan
56 Yosun ormanı · bir yosun şeridine tutunmuş · sarı
57 Denizaltı · lombozdan el sallıyor · turkuaz
58 Deniz altındaki kum kalesi · kulenin tepesine bayrak dikiyor · mor
59 Denizanası balosu · bir denizanasıyla dans ediyor · pembe
60 Hazine sandığı · sandıktan bir altın para çıkarıyor · turkuaz
61 Balkabağı tarlası · büyük kabağı itiyor · mor
62 Elma şırası değirmeni · değirmenin kolunu çeviriyor · turkuaz
63 Yaprak yağmuru ve bank · bankta oturup düşen yaprağı yakalıyor · turkuaz
64 Korkuluk ve kargalar · korkuluğun şapkasında oturuyor · mor
65 Ambar kapısı · saman balyası taşıyor · turkuaz
66 Mantar sepeti · sepete mantar koyuyor · sarı
67 Sonbahar kayıkhanesi · kayıkta kürek çekiyor · turkuaz
68 Ceviz ağacı salıncağı · salıncakta sallanıyor · mor
69 Hasat sofrası · masaya turta koyuyor · mercan
70 Sisli bağ evi · üzüm salkımı taşıyor · sarı
71 Uçan gemi iskelesi · gemiyi iskeleye iple bağlıyor · mercan
72 Bulut feneri · fenerin balkonundan el sallıyor · mor
73 Balon pazarı · bir balonun ipini tutuyor, ayakları yerden kesilmiş · turkuaz
74 Rüzgâr gülleri kulesi · elinde rüzgâr gülü, kulenin önünde · sarı
75 Gökkuşağı köprüsü · köprüde yürüyor · mor
76 Posta güvercinliği · güvercine mektup veriyor · turkuaz
77 Bulutta çay bahçesi · çay döküyor · mercan
78 Pervaneli kütüphane · merdivende kitap okuyor · mor
79 Fırtına gözcüsü kulesi · dürbünle bulutlara bakıyor · sarı
80 Gün doğumu güvertesi · dümende · turkuaz
81 Sarmaşıklı taş kapı · sarmaşığı aralayıp kapıdan bakıyor · mercan
82 Şelaleli tapınak · basamakta oturmuş şelaleye bakıyor · sarı
83 Kök köprü · kökün üstünden yürüyor · turkuaz
84 Ateşböceği vadisi · ateşböceğini avucunda tutuyor · mor
85 Taş kaplumbağa heykeli · heykelin başına çıkmış · mercan
86 Gizli bahçe havuzu · nilüfer yaprağının üstünde · turkuaz
87 Ağaç ev köyü · ip merdivenden tırmanıyor · sarı
88 Kristal mağara · parlayan kristale dokunuyor · mercan
89 Antik gözlem kulesi · kulenin tepesinde · turkuaz
90 Orkide merdiveni · merdivenden iniyor, elinde orkide · mor
91 Şato kapısı ve köprü · köprüde şatoya doğru yürüyor · turkuaz
92 Mum ışıklı koridor · elinde mumla koridorda · mercan
93 Saat kulesi içi · bir dişlinin üstünde · sarı
94 Kont'un kütüphanesi · kocaman bir kitabı itiyor · mor
95 Kubbeli ziyafet salonu · masadan bir üzüm alıyor · turkuaz
96 Ayna salonu · aynaya bakıyor, aynada da görünüyor · mercan
97 Kule gül bahçesi · bir gülü kokluyor · pembe
98 Simya laboratuvarı · renkli bir şişeyi tutuyor · turkuaz
99 Taht salonu · boş tahtın basamağında, tahta bakıyor · mor
100 Şatonun üstünde gün doğumu · tabloda Jiggle yoksa 4–6 farklı renkte Jiggle sevinçle uçuşuyor / zıplıyor; varsa renklerini çeşitlendir

## Dosyalar (yalnız bunlara yaz)
- `Assets/Art/Paintings_v2/jiggle/painting-NN.jpg`: 1024×1024 RGB JPG, en çok 300.000 bayt.
- `Assets/Art/Paintings_v2/jiggle/Source/painting-NN.png`: kaynak.
- `docs/art/pm83/jiggle/once-sonra-NN-MM.png`: partinin 10 tablosu, üstte eski, altta yeni (her biri 200 px).
- `docs/art/pm83/JIGGLE_LOG.md`: partinin sonuna 3–5 satır (hangileri düzenleme, hangileri yeniden çizim, sorun).
- Mevcut `painting-NN.jpg` ve manifest'e dokunma, oyuna bağlama yok, soru yok.

## Kontrol (her tablo)
- Jiggle var, tam görünüyor, boyu %13–17, rengi listedeki.
- Sahnenin geri kalanı eskisiyle aynı (düzenlemede).
- 6×6 ve 8×8 kesimde düz renk parça yok; 300.000 bayt; 1024×1024 RGB.
