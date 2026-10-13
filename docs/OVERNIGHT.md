# Faz 4 gecesi — 13 → 14 Ekim 2026 (seviye fabrikası)

Talimat (Zeyd): sabaha / ertesi güne kadar kendi başına; önce `docs/FAZ4_SEVIYE_FABRIKASI.md`, sonra Zeyd'e bağlı olmayan açık işler; Zeyd'e bağlı her şey aşağıdaki listede; bitince en üste "GECE BİTTİ".

## Kaldığım yer
- **Bitenler (main):** G1 `b4df3d6` · plan `f8d6ecd`, `2369b2c` · G2 `1cddd9b` · G3 `884e130` · G4 `2217bc3` · G5 ajanlar · G4-ek `23067ef` · G6 `bd1c5ff` · G7 onay sekmesi `95eeb35` · kontrol noktası 4 `b234794` · 4-b `544d51f` · T/N/K ilk koşu `74869e6` · P1 playtest taslağı `0e8216a` · P2 Excel `97f2ea5` · P3 case study `6b26340`. G8 `faz4-g8` dalında `55ecbe4` (bayraklar kapalı; Unity koşusu bekliyor).
- **Şu an:** T/N/K yeniden (`kolay2-20261014`) ve O/Z/ÇZ 20 slot "Derin" (`ozc2-20261014`, 8 paralel parça) koşuları sürüyor (20:51'de başladı, en fazla 8 sa). Bitince: birleştirme → ajanlar (tasarımcı + psikolog) → PM kontrol noktası 5 → bağımsız inceleme → RAPOR → onay talimatı.
- **Commit'lenmemiş:** yok.

## Onay kuyruğu
Henüz yok (40 slot koşusu sürüyor).

## Zeyd'in yapacakları (hiçbiri bu geceyi bekletmez)
1. Sabah ilk iş bu bölümü oku; "GECE BİTTİ" varsa önce onu.
2. Build 2 telefon oturumu (`docs/TELEFON_TESTI.md`, ~25 dk). Build 3'ün tek engeli bu.
3. Onay kuyruğu hazır olunca: editörde oyna, Onayla / Reddet + not (talimat burada yazılacak).
4. Sabah: Zorluk paneli turu, seviye editörü kabulü, CSV'yi Excel'de açma, MCP düğmesi, gerçek görseller, izin listesi kararı, saatlik görevi kapatma.
5. Gelecek: `docs/sql/004_keyif.sql` kararı ve "yeni seti canlıya al" komutu yalnızca sende; Actions'taki eski re-run'ı iptal et; playtest öncesi 4G teyidi.

---

# Sabah ek çalışması — 13 Ekim 2026, 07:00–08:45 (Kanal A)

Zeyd "2–3 saat daha çalışın" dedi. İşleri PM sıraladı (S1–S6, T1–T3, U1–U2, V1–V3); hepsi bitti ve main'e push edildi. **Yayın değişmedi:** gh-pages hâlâ Build 2 (`47328ff`). Bu işlerden oyuna girenler (S1, S5) Build 3'te yayına çıkacak.

| İş | Commit | Sonuç |
|---|---|---|
| S1 açılış testi | `4e64340` | Boş test, gerçek bir testle değiştirildi. Test, isim kapısı açıkken 20 sn geçtiğinde oturum ve deneme başlamadığını, isim girilince L02'nin tam süreyle açıldığını sınıyor. Kapıyı devre dışı bırakan sahte bir hata eklenince 2 test kırıldı, yani testler hatayı yakalıyor. **Unity Test Runner'da henüz koşmadı** (Unity açık); Build 3'ten önce zorunlu. |
| S2 web verisi | `7be3682` | Web oyununun eski verisi az çıktı: 10'dan fazla oyun oynanmış yalnızca 1 seviye var (L01), PM'in eşiği 10 seviye. Sonuçlar yalnızca yön gösteriyor; modeldeki sabitler değişmedi. 6 Eylül'e kadarki 50 satır eski, daha uzun sürelerle oynanmış; analizden kalıcı olarak çıkarıldı. Modelin parça başına süresi ortalamada tutuyor (0,98×). Yapı ölçüleri hangi seviyenin zor olduğunu doğru yönde gösteriyor (kazanma oranıyla ρ −0,67). `docs/analiz/web_verisi_2026-10-13.md` |
| S3 40 seviye raporu | `7be3682`, `9978688` | `docs/analiz/40_seviye_raporu.md`. 19 seviyede önerilen süre mevcuttan en az %20 farklı. İlk 10 seviyenin süresi formülün yaklaşık 2 katı. Canlı 40 seviyeye dokunulmadı; bu bulgu Faz 4'e alındı. |
| S4 kart görüntüsü | `8b4653e` | Canlı Build 2'de 375×667'de "Resmi tamamladın" kartı: kart ekrana sığıyor, düğmeler 46 pt. Resmin çerçevesi başlığa değiyordu. |
| S5 kart boşluğu | `a80868f` | Resmin üstüne 8 pt boşluk eklendi; Build 3'te yayına çıkacak. |
| S6 editör | `e8c384c` | Kırpma kaydırıcıları Geri al'dan sonra doğru değere dönüyor; klasörden görsel atama geri alınabiliyor. Bunun için otomatik test yok, senin editör turunda kontrol edilecek. |
| T1 model | `9978688` | Tanıtım ve Nefes türlerinde artık bombaya izin var (bomba payı 8 ve üstü). L21'in hedef dışı ölçü sayısı 6'dan 5'e indi. Unity'siz testler 454/454. |
| T2 K1 ve FPS (PC) | `71f57ba` | Canlı Build 2'de, bilgisayarda gerçek GPU ile, 375×667, dokunma olaylarıyla: **12 kenar çekişinin 12'si tek seferde çıktı.** L01'de 5, L40'ta 7 çekiş; üçü tahtanın dışından tutularak, üçü fiskeyle. 10 sn sürekli sürüklemede L40 ve D02'de saniyede ortalama 59,9 kare, en düşük 58,5. Bu ölçüm iPhone ölçütünün yerini tutmaz. `docs/screens/k1/K1_pulls_pc_375x667.png` |
| U1 telefon listesi | `390f107` | `TELEFON_TESTI.md`'nin K1 bölümüne bir not eklendi: bilgisayarda 12/12 çıktı, asıl ölçüt iPhone. Adımlar değişmedi. |
| U2 doğrulama betiği | `29e3ff0` | `node Tools/verify_build.js <adres>` tek komutla açılış kontrolünü (40/40), K1'deki 12 kenar çekişini, PC FPS'ini ve D01 kartının yerleşimini sınıyor. Bayraksız Supabase'e hiçbir şey yazmaz. A6b ve M5 kontrolleri yalnızca `--send --player TEST_…` ile çalışır; bu oturumda canlı veriye yazılmadı. **Canlı Build 2'de 3/4 geçti:** açılış, K1 12/12 ve FPS. Kart kontrolü bilerek kaldı: başlık ile resim çerçevesi arası 3,5 pt, eşik 8 pt. Bu S5 ile Build 3'te düzelecek. |
| V1 analiz filtresi | `1f8bdb6` | `docs/sql/003`'e ortak veri kalitesi filtresi eklendi: F1–F5, her biri karar tarihiyle. F1 `TEST_` adları, F2 6 Eylül 14:00 UTC öncesi eski süreli web satırları, F3 saat güçlendiricili süreler, F4 Build 2 öncesi Unity satırları, F5 web'in deneme ve bırakma alanları. Aynı filtre anon REST'le denendi: 133 satır, 5 oyuncu. SQL Supabase'de çalıştırılmadı. |
| V2 yerel doğrulama | `f11b215` | `verify_build.js --local Builds/publish/br`: build klasörünün kopyasını GitHub Pages'teki gibi sunuyor (`Content-Encoding` göndermeden), koşu bitince kopyayı ve Chrome profilini siliyor. Build 2 kopyasında sonuç canlıdaki gibi 3/4. Yayın sırası `CLAUDE.md`'ye yazıldı. |
| V3 case study notları | bu commit | `docs/case_study/notlar.md`: veri kalitesi, girdi düzeltmesi, zorluk modeli, 40 seviye ve yöntem başlıklarında bulgular, her biri kaynağıyla. Yeni analiz yok. |

**Yöntem notu:** Normal ayarlı başsız Chrome yazılımla çizim yaptığı için saniyede yaklaşık 3 kare veriyordu; o hızla bölüm kazanılamıyor, FPS de ölçülemiyor. Chrome'u ayrı bir portta gerçek GPU ile açınca saniyede 60 kare oluyor. Bu yöntem `CLAUDE.md`'ye yazıldı.

## Zeyd'in sırası
1. **Telefon oturumu:** `docs/TELEFON_TESTI.md` (Build 2). Bilgisayarda K1 12/12 ve 60 FPS çıktı; asıl ölçüt iPhone'daki sonuç.
2. **Unity:** Unity'yi sen açtın, kapatmadım. İşin bitince ikisinden birini yap:
   - Kaydet, Unity'yi kapat ve bana "Unity kapalı" yaz; testleri ben koşarım.
   - Ya da Window → General → Test Runner → EditMode → Run All, sonra sonucu getir.

   Bu adım Build 3'ün ön koşulu.
3. **Editör turu:** `LEVEL_EDITOR.md`. Yeni maddeler:
   - Kırpma kaydırıcısını oynat, Ctrl+Z'ye bas; kaydırıcı eski değere dönmeli.
   - "Klasörden görselleri ata"dan sonra Ctrl+Z; atama geri alınmalı.
   - Zorluk paneli: Tanıtım türünde bombalı bir tasarım artık uyarı vermemeli.
4. Bundan sonrası aşağıdaki sabah raporunun "Zeyd'in yapacakları" listesi.

---

# Sabah raporu — 13 Ekim 2026 (Kanal A)

**Kısaca:** Build 2 yayında. Telefonda denenmeyi bekliyor. Gece planındaki A0–A11'in hepsi yapıldı. İçlerinde **Zorluk paneli** de var; yalnızca editörde çalışıyor, build'e girmiyor. Bağımsız inceleme build'e giren kodda veri kalitesi hatası bulmadı. Kanal B gece boyunca hiç görünmedi.

## Yayında olan
- **Oyun:** https://zeydusht.github.io/reverse-solver-unity/ adresinde, gh-pages `47328ff` (Build 2c, 02:06).
  - Brotli 7.178.477 B, gzip 9.076.885 B. Build 1'e göre fark yok denecek kadar az (+24 KB, resim çizimi).
- **Kod:** main `c65406b`.
- **Build 2'de:**
  - L01 öğretici eli
  - K1 kenar parçaları
  - G1 görselli tasarımlar (yalnızca D01 ve D02; canlı 40 bölümde görsel yok)
  - "Resmi tamamladın" kartı
  - M6 efektleri
  - Bağlantıyla açılan bölüm isim kapısının arkasında bekliyor
  - Canlı veri gönderimi açık

## Gece yapılanlar
| Adım | Sonuç |
|---|---|
| A1 derleme hatası | `1d7ed02`. Görseller `UnityWebRequest` + `LoadImage` ile yükleniyor, manifest'e modül eklenmedi. Hemen push edildi. |
| A2 testler | Unity'de tamamı geçti. Gecenin son koşusu **455/455**. |
| A3 resim çizimi | `939f8a0`. Shader PM onayıyla commit'lendi. Okunabilirlik puanları: D01 0,567, D02 0,379; uyarı eşiği 0,30. |
| A4–A5 | K1 ve G1 ekran görüntüleri `docs/screens/k1`, `docs/screens/g1` altında. Efektler ve kademe 2 ekranda görüldü. |
| A6 Build 1 | Yayınlandı ve doğrulandı. m5-verify geçti: TEST_GECE satırları doğru, yenilemede isim ve ilerleme kalıyor, debug oturumu 0 satır gönderiyor. |
| A6b | Bağlantıyla açılan bölüm (`?lv=2`) isim yazılmadan başlamıyor. Canlı ölçüm: kapıda 22 sn beklendi, L02 tam süreyle başladı; satırdaki süre 43 sn, gerçek süre 44 sn. |
| A7 G1 editör | Kırpma (yakınlaştırma ve kaydırma), dikiş okunabilirliği uyarısı, klasörden toplu atama, "Resmi tamamladın" kartı, kılavuz ve telif notu. |
| A8 Zorluk paneli | `8577211`, `c65406b`. Ayrıntısı aşağıda. |
| A9 Build 2 | `47328ff`. m5-verify yeniden geçti. |
| A10 bağımsız inceleme | Build'e giren kodda hata yok. Editörde 2 hata bulundu ve düzeltildi, 1 test boşluğu kapatıldı. Kalan maddeler aşağıda. |

### Zorluk paneli (A8)
Seviye editöründe **Zorluk** bölümü: tür seçilir, **Hesapla**'ya basılır. Ne yaptığı:
- Ölçüleri türün hedef aralığıyla karşılaştırır: yeşil aralıkta, turuncu aralık dışında.
- Süre, fitil ve çivi önerir. Öneriyi uygulamak için **Uygula** düğmesi var; **Geri al** ile geri alınır ve kaydedince sürüm 1 artar.
- Kılavuz: `docs/LEVEL_EDITOR.md` → "Zorluk paneli".

Ölçülenler ve teyitler:
- L40 için yapı ölçümü + 200 rastgele oyun + model **120 ms** sürdü (hedef ≤ 1 sn). Çok zor türünde önerilen süre 65 sn, s = 1,35.
- **Teyit (a):** Tam arama bütçeyi aşınca çözücünün kazma yolu kullanılıyor. Bu durumda fitil önerisi "(yaklaşık)", bomba payı "≥" diye gösteriliyor. L26, L30 ve L34 böyle; L30 için ayrıca test var.
- **Teyit (b):** Panel s'nin kaynağını yazıyor (yapıdan, bombalı ya da oyunun ilk bölümü).
- Tür bir etikettir, sürümü değiştirmez; `designs.json`'a `"genre"` olarak yazılır.
- **CSV artık 36 sütun.** Yeni sütunlar: `tur`, `onerilen_sure`, `onerilen_fitil`, `onerilen_civi`, `medyan_yildiz`.
  - `darbogaz_adim` kaldırıldı. Yerine tek bir `darbogaz` sütunu geldi: PRODUCT tanımıyla, 10+ parça kalmışken yalnızca bir parçanın çıkabildiği hamleler. 40 bölümde 0 ile 6 arasında, medyanı 0.
- **Yazılmayanlar:** "Zirve anı" ve "dikkatsiz oyuncunun patlama oranı". Planda tanımları yok; PM backlog'a aldı.

### Bağımsız inceleme (A10)
- **Düzeltildi:**
  - Kırpmadan vazgeçince, başka tasarıma geçip dönünce ya da Unity'yi yeniden açınca diskteki görsel artık kaydedilen kırpmayla yeniden kesiliyor.
  - "Klasörden görselleri ata" kaydedilmemiş düzenlemeleri artık kaydedilmiş gibi göstermiyor.
  - "Süre bitti" kartının testi gerçekten görselli bir bölümle yapılıyor.
- **Sabaha kaldı:**
  - LaunchPlanTests'teki bir test boş, çünkü düzeltme olmasa da geçiyor. Gerçek teste çevrilecek; açılış koduna dokunan bir sonraki build'den önce zorunlu.
  - "Resmi tamamladın" kartında resmin çerçevesi başlığa değiyor. Yaklaşık 8 pt boşluk eklenecek, Build 3'e girecek. PM, telefon listesinin hedefi değişmesin diye bu gece build alınmamasına karar verdi.
- **Backlog:** kırpma kaydırıcıları Geri al'dan sonra eski değerde kalıyor; resim geç gelirse kart döndürmede başlık değiştiriyor; debug satırında bellek ölçümü yok.
- **Doğrulandı:**
  - `levels.json` ve Golden değişmedi.
  - D03 bayt bayt aynı.
  - Repoda gizli anahtar yok, yalnızca publishable anahtar var.
  - Core, Presentation ve Platform editör koduna bağlanmıyor.
  - Debug ve tasarım oturumları veri göndermiyor.

### Yapılamayanlar ve nedeni
- **Editör ekran görüntüleri:** Unity MCP bütün gece bağlanmadı; eklenti sunucuyu yalnızca arayüzdeki düğmeyle başlatıyor. Bu yüzden Zorluk paneli, kırpma paneli ve "Resmi tamamladın" kartının 375×667 görüntüsü yok. Unity gece 01:06'da kurala uyarak kapatıldı (30 dk MCP yanıtı yoktu; kaydedilmemiş iş ve açık diyalog olmadığı kontrol edildi), işler batchmode ile yapıldı. Unity şu an kapalı.
- **Kanal B:** `gece-b` dalı origin'de hiç oluşmadı. PM kararı:
  - Tek DifficultyModel olacak, o da A8'inki.
  - B ileride çalışırsa yalnızca B1–B2 (web verisi ve sabitlerin ayarı) alınacak.

## Zeyd'in yapacakları (sırayla)
1. Bu raporu oku.
2. **Telefon oturumu, yaklaşık 25 dk:** `docs/TELEFON_TESTI.md`'deki 7 bölüm. Kök adresi kullan, ad `TEST_ZEYD`.
   - Ek olarak bak:
     - `?debug=1` ile görselli en büyük tahtada sürüklerken FPS.
     - D01 kazanılınca "Resmi tamamladın" kartı geliyor mu.
     - D02'de girinti ve çıkıntılar seçiliyor mu.
   - Getir:
     - her madde için geçti/kaldı,
     - FPS sayısı,
     - K1 için 10 çekişten kaçının tek seferde çıktığı,
     - kalan maddelerin ekran görüntüleri.
3. **MCP, 1 dk:** Unity'yi aç, MCP sunucusunu arayüzdeki düğmeyle başlat, Claude'a "MCP açık" yaz.
4. **Zorluk paneli turu:** `LEVEL_EDITOR.md` → "Zorluk paneli". Bir tasarıma tür seç, Hesapla'ya bas, bir öneriyi Uygula ile uygula, sonra geri al.
   - Getir: anlaşılmayan terimler, yanlış görünen öneriler, eksik bulduğun şeyler.
5. **Seviye editörü kabulü:** "İlk seviyeni kur" turunu gerçek fareyle yap; Ctrl+Z, Ctrl+Y ve Ctrl+S'yi de dene. Sonra tasarımı iPhone'da `…/br/?debug=1&set=designs&lv=D0X` adresinden oyna.
   - Getir: süre, takıldığın yer, eksikler, palet→tahta sürükle-bırak isteyip istemediğin.
6. **Gerçek görseller:** kendi fotoğraflarını ya da kullanım hakkın olan görselleri `D01.jpg` gibi adlarla bir klasöre koy, toplu atamayla ekle.
   - Getir: okunabilirlik uyarısı çıktı mı, telefonda dikişler seçiliyor mu.
7. **CSV:** `Builds/seviyeler.csv`'yi Türkçe Excel'de aç.
   - Bak: 36 sütun ayrı ayrı mı, Türkçe karakterler doğru mu, L01'de `acik_kenar_orani` 0,42 mi.
   - Getir: geçti/kaldı.
8. **Karar:** Unity'yi yeniden başlatma ve batchmode izni yalnızca bu gece için miydi, kalıcı mı? Getir: "gece" ya da "kalıcı".
9. **Kanal B:** B oturumunu başlattın mı? Getir: "açılmadı" ya da çıktısının yeri.
10. **İzin listesi:** `.claude/settings.local.json`'a ekleme yapmak senin kararın.
11. Desktop'taki saatlik gece görevini duraklat.
12. Actions'ta kuyrukta bekleyen eski re-run'ı (commit `0552040`) iptal et.
13. **Playtest öncesi (şimdi değil):** son build ile 4G (LTE) teyidi, 3 açılış.

## Açık işler
- **Sabah, mühendislik (0c):**
  - LaunchPlanTests'teki boş testi gerçek teste çevir ve mutasyon kontrolüyle doğrula.
  - MCP açılınca: "Resmi tamamladın" kartının 375×667 görüntüsü ve editör görüntüleri (G1 editör ve Zorluk paneli) `LEVEL_EDITOR.md`'ye.
  - Kart çerçevesine 8 pt boşluk ekle. Build 3'e girecek.
- **Build 3:** Zeyd'in telefon sonuçlarıyla birlikte alınacak. Veri kalitesini etkileyen bir hata çıkarsa her şeyin önüne geçer.
- **Kanal B:** B1–B2, B4, B5–B6. Ne zaman yapılacaklarına PM, Zeyd'in cevabından sonra karar verecek.
- **Faz 1'in kalanı:** M6 ölçüt 7, K1 ölçüt 5 (iPhone'da 10 çekişte 9), 4G teyidi.
- **Sonra:** M6 ses, Faz 2 test modu.

---

# Gece — 2026-10-12 → 13 (Kanal A, oturum 1, main)

Plan: `docs/GECE_2026-10-13.md` (+ `docs/GOREV_KENAR_GORSEL.md`). Kanal B `gece-b` dalında, raporu `docs/analiz/GECE_B.md`. Kalp atışı: `Logs/overnight-heartbeat.txt`.

## Kaldığım yer
- **Son tamamlanan adım:** A9 — Build 2 canlıda (gh-pages 47328ff, 02:06): G1-b "Resmi tamamladın" kartı, A6b (bağlantılı bölüm isim kapısının arkasında bekler), K1 debug raporu düzeltmesi. Brotli 7.178.477 B, gzip 9.076.885 B. Unity batch 445/445. Canlı: A6b (kapıda 22 sn → L02 tam süre, satır dur 43 = gerçek 44 sn), m5-verify yeniden geçti, D01 kartı ve L01 kartı (değişmemiş) yerelde Build 2'de görüldü. TELEFON_TESTI Build 2'ye göre güncel.
- **Önceki:** A0–A6 (Build 1 69241ed), A7 editör kısmı (6d9a724: kırpma, uyarı 0,30, toplu atama), G1-b (4bf4cc2), A6b (d9a08e8), rapor düzeltmesi (d813ba6).
- **Unity:** kapalı; batchmode ile çalışılıyor. MCP bu oturumda geri gelmedi (eklenti sunucuyu yalnızca arayüzden başlatıyor) → editör ekran görüntüleri sabah listesinde.
- **Yarım kalan iş:** yok.
- **Sıradaki somut adım:** gece bitti (A8 `8577211`, CSV `c65406b`, A10 işlendi, A11 sabah raporu yukarıda). Sabah: 0c işleri.
- **Commit'lenmemiş değişiklik:** yok.

## Sabah listesi (Zeyd)
- docs/TELEFON_TESTI.md ile telefon oturumu (Build 2 canlıda).
- Editör ekran görüntüleri (Görsel ekle paneli, kırpma, uyarı, adım adım oynatma, Parçaları ayır, Zorluk paneli): bu gece MCP yoktu; Unity'yi açıp kılavuzdaki turu yaparken bakılacak.

---

# Gece — 2026-10-07 → 2026-10-08

Görev: `docs/GECE_PLANI.md`. Kalp atışı: `Logs/overnight-heartbeat.txt` (git dışı).

## Kaldığım yer (2026-10-08 22:35, görev: docs/GOREV_KENAR_GORSEL.md)
- **Engel:** Unity süreci çalışıyor (Responding) ve MCP sunucusuna (127.0.0.1:8080) bağlı, ama komut kuyruğu 21:44'teki play modu çıkışından beri takılı: ping yanıtsız. Çözüm: Zeyd Unity penceresine tıklar (odak → refresh → derleme → domain reload). Programla öne getirmek işe yaramadı. Unity programla kapatılmaz (PM).
- **Unity'siz doğrulama yolu:** scratchpad/headless/compile.sh (tüm assembly'ler Unity DLL'lerine karşı Roslyn ile) + run.sh (EditMode testleri Mono'da; TestContext → Console). Ara kanıt; bağlayıcı olan Unity Test Runner (PM).
- **Commit'lenenler (main):** 1647699 L01 el · 79bb93d K1 · 517f269 G1 kısım 1 · 7c60a33/ca62772 kademe 2 · bba9b60 M6 efektleri · 26e59cf G1 kontrol hazırlığı · 592dd8c M5 bayrak AÇIK (Zeyd onayı) · 89667e9 supap metni "söküldü" · 197479b docs/TELEFON_TESTI.md · 2bb3208 Monte Carlo · 2be3951 analiz SQL taslakları. Son: 423 test geçti.
- **Commit'lenmemiş:** Piece.shader (G1 resim çizimi) — Unity'de görülüp PM onayından sonra.
- **PM kuralı:** build'e kadar build'e giren yeni oyun kodu yok (Core/Presentation/Platform); yalnızca editör-only, test, docs.
- **Unity dönünce:** Unity'de tüm testler → "Reverse Solver → Test görsellerini D01–D03'e koy" → shader görüntüleri (editör + 390×844, RenderShot "D01") PM'e → onayla shader commit (45 dk sınır; tutmazsa build eski shader'la) → K1 görüntüleri (L01 + L40, 375/390/430) → efektler ve kademe 2 ekranda → Brotli+gzip build → gh-pages → başsız Chrome + scratchpad/m5-verify.js (başarısızsa bayrak kapat, yeniden yayınla) → PM → Zeyd'e docs/TELEFON_TESTI.md.

## Sabah listesi (Zeyd)
- **Gönderimi canlıya açma senin onayını bekliyor.** SQL doğrulandı (salt-okuma: sütunlar var, `level_id` dolu). PM onayladı ama Claude Code otomatik modu bayrağı açmayı engelledi. Yapılacak: Claude'a "gönderimi aç" de (ya da `Assets/_Game/Platform/Web/TelemetrySettings.cs`'te `SendEnabled = true`), build + yayın + TEST_GECE/TEST_ZEYD doğrulaması o zaman yapılır.
- `.claude/settings.local.json`'a gece izinlerini ekleme isteği Claude Code'un otomatik modunda "kendi izinlerini değiştirme" diye reddedildi; dosya değişmedi. Oturum otomatik modda sürdüğü için gece akışı buna takılmadı. İstersen izin listesini kendin ekle (plan 0.1'deki liste).

---

# Gece raporu — 2026-10-06

**Sabah ilk iş: web build'i alınamıyor.** Windows'un Akıllı Uygulama Denetimi (Smart App Control) Unity'nin kendi dosyasını engelliyor:
`WebGLPlayerBuildProgram.Data.dll ... Uygulama Denetimi ilkesi bu dosyayı engelledi (0x800711C7)`.
17:35'teki M2 build'i sorunsuzdu; engel sonradan başladı. Kod ya da proje sorunu değil. Bu yüzden bu gece hiçbir build yayınlanamadı; M3 kodu editörde tamam ama canlıda değil.

## Senin yapacakların (sırayla; biri tutunca dur)
1. **Build engeli** (PM'in sırası):
   1. Bilgisayarı yeniden başlat, Unity'de *Reverse Solver → Build WebGL*'i bir kez dene.
   2. Tutmazsa Unity Hub → Installs → 6000.6.4f1 → Uninstall; aynı sürümü **WebGL Build Support** ile yeniden kur, build'i dene.
   3. Tutmazsa bulut build (GitHub Actions; hazırlık durumu aşağıda, secret'ları senin eklemen gerekiyor).
   4. Başka bir bilgisayar varsa orada dene.
   5. **Smart App Control'ü kapatmak yalnızca son çare: geri dönüşsüz** — kapatınca Windows sıfırlanmadan tekrar açılamaz. Önce PM'e sor.
      - *Düzeltme (2026-10-07):* Bu not eskidi. Nisan 2026 güncellemesinden (KB5083769) beri SAC kapatıldıktan sonra tekrar açılabiliyor. Zeyd SAC'ı kapattı; build artık engellenmiyor.
   - Getireceğin: hangi adımı denedin, build geçti mi, geçmediyse hata metni.
2. Build yayınlanınca iPhone'da: `?debug=1` ile en büyük tahtada (ör. `?lv=40&debug=1`) sürüklerken FPS; L01–L05, L11, L21, L31'i oyna (sürükleme parmağı takip ediyor mu, sayfa kayıyor/yakınlaşıyor mu, engeller okunuyor mu).
3. Actions'ta kuyrukta bekleyen eski re-run'ı (0552040) iptal et.
4. Playtest öncesi 4G (LTE) teyidi: kök adres, 3 açılış, panel değerleri + fast.com.

## Neler bitti
### M3 — kod tamam, build/yayın bekliyor (PM kararı)
- Tahta, parçalar, zincirler, çivi/bomba rozetleri, duvarlar, üst şerit ve bitiş kartı web'in CSS/SVG'sinden birebir taşındı (4 shader). Web görüntüsüyle yan yana: `docs/screens/m3/L40_375x667.png` ve `docs/screens/m3/web/L40_375x667_web.png`. Diğer 23 görüntü (6 seviye × 3 boyut, bitiş kartları) `Builds/screens/m3/` altında (git dışı).
- Hücre boyutu web'in formülüyle aynı: 375×667'de en büyük tahtada **56 pt** (dokunma alanı ≥ 44 pt şartı sağlanıyor); 390×844'te 58, 430×932'de 65.
- **Türkçe karakterler:** TMP'nin hazır fontunda Ğ ğ İ ı Ş ş yoktu (yedek font 350 KB'lık TTF'yi build'e sokacaktı). Yerine 115 karakterlik kendi statik atlasımız (512×512) yapıldı; ğ ü ş ı ö ç İ Ğ Ü Ş I Ö Ç doğru görünüyor: `docs/screens/m3/charset_375x667.png`. TTF build'e girmiyor. **Fontun build boyutuna etkisi ölçülemedi** (build alınamıyor); ilk build'de size-log'dan raporlanacak.
- Renk uzayı Gamma'ya alındı: yarı saydam katmanlar artık tarayıcıdaki CSS gibi karışıyor (Linear'da sönük yıldızlar ve perdeler açık çıkıyordu).
- Sürükleme: gerçek Input System yolundan sanal fareyle test edildi — 15 karede takip hatası **0,00 pt** (yumuşatma yok), kısa sürükleme yerine dönüyor, tam sürükleme parçayı çıkarıyor.
- `?lv=N` ile seviye seçimi; `?debug=1` ile süre paneli, açılış kontrolü ve sürüklerken medyan FPS satırı. iOS'ta iki parmakla ve çift dokunarak yakınlaştırma engellendi.
- Testler: 334 EditMode testi geçiyor.

### M4 — kod tamam, build/yayın bekliyor
- **Ana menü** (Oyna · Bölüm N, Bölümler), **bölüm listesi** (40 bölüm; önceki bölüm kazanılınca sonraki açılır, `?debug=1` hepsini açar), **booster çubuğu** web'deki gibi (sayı rozeti, seçili amber, kilitli gri + "N. BÖLÜM", yeni booster nabız), **makas hedefleme** (eklemlerde amber noktalar), **çekiç**, **değnek ve saat onay kartı**, **tanıtım kartları** (engel / yeni booster; açıkken süre durur), bitiş kartına **Menü** düğmesi.
- Booster mantığı Core'da (Unity'siz) ve test edildi: her booster için dokunma → oyun komutu → olay; yeniden başlatma ve menüye dönüş "bıraktı" kaydı yazıyor.
- Görüntüler: `docs/screens/m4/` (booster'lı tahta, makas, menü, değnek sorusu); üç boyutun hepsi `Builds/screens/m4/`.
- Dokunma alanları: booster 60 pt, kart düğmeleri 46 pt, bölüm hücreleri ≈49 pt, makas noktaları 44 pt. Alt şerit bölgesinde arayüz yok.
- Yok (öneri): L01'deki öğretici el → M6; oyuncu adı ekranı → M5.

### Bulut build (yedek yol) hazır
- `.github/workflows/webgl-build.yml`: yalnızca elle başlar (Actions → *WebGL build* → *Run workflow*), Brotli + gzip build'lerini artifact olarak verir, yayınlamaz. Unity 6000.6.4f1 WebGL imajı game-ci'de mevcut.
- Kurulum: `docs/CLOUD_BUILD.md`. Secret'ları (UNITY_EMAIL, UNITY_PASSWORD, UNITY_LICENSE) senin eklemen gerekiyor. **Belirsizlik:** bu bilgisayarda klasik lisans dosyası yok (Unity'nin yeni lisans sistemi); game-ci'nin Personal lisansı hangi yolla kabul ettiğini onların sayfasından doğrulaman lazım. Çalışacağı garanti değil.
- Doğrulanmadı: workflow'u başlatamadım (GitHub kimliği gerekiyor).

## PM kararları (ayrıntı `docs/PRODUCT.md`)
- M3 kapanmadı: "kod tamam, build/yayın bekliyor". İlk build'de M3 + M4 birlikte yayınlanacak, iPhone testleri tek seferde.
- Bu gece: M4 kodu ve M5'in Supabase'e dokunmayan ön işleri (PlayerPrefs, kayıt kuyruğu, `level_id` SQL taslağı). Canlı gönderim yok.
- Bulut build (GitHub Actions) bu gece ~1 saatlik zaman kutusuyla hazırlanacak; yalnızca elle tetiklenir, secret repoya girmez.

## Testlerin durumu
346/346 EditMode (editörde) + sanal fare sürükleme testi (0,00 pt takip hatası). Web build'i yok, iPhone testi yok.

## Önceki gece — 2026-10-05

Kısa özet: boyut çalışması kapandı (11,8 → 8,7 MB gzip, 6,8 MB Brotli), M1 bitti (seviye verisi + kural motoru, 227 test yeşil), ikisi de PM onaylı. **Sabah ilk iş GitHub Pages'i açman lazım; açılmadan hiçbir adres çalışmıyor.**

### Neler bitti

### Boyut çalışması
Toplam indirme (indirilen 4 dosya, sıkıştırılmış). Ham değerler `Builds/size-log.csv`'de.

| Adım | wasm | data | Toplam |
|---|---|---|---|
| A başlangıç | 9,02 MB | 2,66 MB | 11,81 MB |
| B kullanılmayan paketler | 8,50 | 2,52 | 11,15 |
| D URP: post-processing, HDR, ışık, lens flare kapalı; Unlit | 8,50 | 2,41 | 11,04 |
| E Unity splash kapalı | 8,50 | 2,35 | 10,98 |
| C High stripping + IL2CPP Optimize Size | 7,43 | 1,86 | 9,42 |
| F wasm Code Optimization: DiskSizeLTO (Unity varsayılanı BuildTimes'tı) | 6,65 | 1,86 | 8,64 |
| G Brotli (aynı kod) | 5,16 | 1,47 | 6,81 |
| **M1 gzip** (şu an yayında) | 6,66 | 1,87 | **8,67** |
| **M1 Brotli** (şu an yayında) | 5,16 | 1,49 | **6,84** |

- Brotli 1,8 MB daha küçük ama JS ile açması bilgisayarda 1,55 sn, gzip 0,27 sn. Başa baş bağlantı hızı ≈ 11 Mbit/s. Kararı senin 4G ölçümün verecek.
- İndirmenin neredeyse tamamı kod (wasm + IL2CPP metadata). UI Toolkit (~1,5 MB IL) çıkarılamıyor: Input System paketi ona bağımlı.
- Ana ekran alt şeridi: sayfa arka planı build sırasında kamera rengine (#314D79) eşitleniyor. iPhone'da doğrulanmadı.

### M1: seviye verisi ve kural motoru
- `Assets/_Game/Levels/levels.json`: web'deki 40 seviye birebir, her birinde kalıcı `id` (L01–L40) ve `version: 1`.
- Core (saf C#, Unity'siz): JSON okuyucu, `LevelParser`, `LevelStats`, `TravelRule` (web kuralının birebir portu), `GreedySolver`.
- Web oyununun kendi kodu Node'da çalıştırılarak 10.612 altın değer üretildi; C# portu hepsinde aynı.
- High stripping'li gerçek web build'i bu bilgisayarda (başsız Chrome) "40 seviye okundu, 40 çözülebilir (51 ms)" yazdı.

### PM kararları (ayrıntı ve gerekçeler `docs/PRODUCT.md`'de)
- Boyut adımları B–F kabul, splash kapalı kalıyor. 6 MB ara hedefi engelleyici değil.
- Sevk edilen sıkıştırma şimdilik **gzip**. Brotli ancak 4G'de medyanda ≥ 0,5 sn hızlıysa seçilecek. İkisi de 10 sn'yi aşarsa boyut işi yeniden açılacak.
- UI Toolkit çıkarılmayacak, URP/uGUI fork'lanmayacak. Input System'den vazgeçilmeyecek.
- Seviye `id`'si kalıcı, `version` oynanış değişince artar.
- M1 kapandı (iPhone kontrolü sende). Sıradaki iş **M2: GameSession** (çivi, bomba, supap, booster'lar, süre, deneme kaydı). İnsan metriklerinin tanımı sabitlendi.
- PhysX modülünü çıkarma fikri backlog'a gitti.

### Testler
- 227 EditMode testi, hepsi geçiyor (`ReverseSolver.Core.Tests`).
- Kurala bilerek bir hata eklendiğinde 160'ı kırıldı. Testler kuralı gerçekten denetliyor; hata geri alındı.
- iPhone Safari'de hiçbir şey test edilmedi.

### Senin yapacakların
1. **GitHub Pages'i aç:** github.com/zeydusht/reverse-solver-unity → Settings → Pages → Source: "Deploy from a branch" → `gh-pages`, `/ (root)` → Save. 1–2 dk sonra https://zeydusht.github.io/reverse-solver-unity/ açılmalı. 404 alırsan Pages ayar sayfasının ekran görüntüsünü getir.
2. **M1 kontrolü:** iPhone Safari'de `/gz/` adresini aç. Süre panelinde "40 seviye okundu, 40 çözülebilir" yazmalı. Ekran görüntüsünü getir.
3. **4G ölçümü:**
   - Wi-Fi ve 5G kapalı olmalı. Önce fast.com'da hız testi yap.
   - `/gz/` ve `/br/` adreslerinin her birini 3 kez aç. Her açılıştan önce Ayarlar → Safari → Geçmişi ve Web Sitesi Verilerini Temizle.
   - Getir: her açılışın panel değerleri (toplam, indirme, JS açma, wasm, motor) ve hız testi sonucu.
4. **Ana ekran:** Paylaş → Ana Ekrana Ekle ile ekle ve oradan aç. Altta farklı renkte bir şerit kalmadığını kontrol et, ekran görüntüsü getir.
5. **Güvenlik duvarı:** Windows Güvenlik Duvarı'nda Python'un "Ortak" ağ iznini kaldır. Yerel sunucular kapalı.

### Açık işler
- Mühendislik: M2 (9 kabul ölçütü, `PRODUCT.md` → "M2 tanımı"). 4G sonucuna göre sıkıştırma kararını uygulamak. M3'te geçici açılış kontrolünü ve süre panelini kaldırmak.
- Backlog: PhysX modülünü çıkarmak, Input Manager'a geçiş (yalnızca 4G 10 sn'yi aşarsa), Unity içinde seviye editörü.

Commit'ler: main `bee34ed`, `806238d`, `e1cf67f` ve bu raporun commit'i. gh-pages `0552040`. Hepsi push'landı.