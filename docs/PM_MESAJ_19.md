# PM → Claude Code (28 Ekim 2026, 17:15) — Toplu düzeltme ve iOS 0.5.1

PM_MESAJ_18 (40 tablo) bitmediyse önce onu bitir, sonra bunu yap.

Hedef: bu turun sonunda **iOS 0.5.1 Xcode zip'i** hazır olsun. Zeyd zip'i arkadaşının Mac'inden TestFlight'a yükleyecek.

Kurallar:
- Soru sorma.
- Token tasarrufu yap.
- Her adımda commit at.

Ekran görüntüsü: `docs/qa/feedback-2026-10-28/win-card.png`.

## 1. Sesler

- Parça her çıktığında çalan ses **kaldırılsın**.
- **Takılma sesi** kalsın: parça başka bir parçaya ya da duvara çarpınca.
- **Son parça** çıkınca C sesinin uzun hali çalsın, ardından kazanma fanfarı.
- Bomba, kilit ve booster sesleri aynen kalsın.

## 2. Ayarlar

Ayarlar ekranına bu seçenekleri ekle; hepsi PlayerPrefs'te saklansın:
- **Ses:** açık / kapalı (bütün efektler)
- **Müzik:** açık / kapalı (müzik varsa)
- **Titreşim:** açık / kapalı
- **Dil:** English / Türkçe

## 3. Dil: varsayılan İngilizce

- Oyunun tüm metinleri bir dil tablosundan gelsin. Varsayılan **English**; Türkçe ayarlardan seçilsin.
- Örnekler:
  - Hedef: "Save the Jiggles!" / "Jiggle'ları kurtar!". Şu anki "Tüm Jiggles'ı kurtar" yanlış.
  - Kazanma kartı başlığı: "Board cleared!" / "Tahta boşaldı!"
  - Tuşlar: Next level, Play again, Menu, Continue, Restart, Main menu, Start.
  - Geri bildirim satırı: "Did you like this level?" ve "How hard was it? 1 very easy · 10 very hard".
- Türkçe metinlerde ğ, ş, İ ve ı harflerinin doğru çıktığını kontrol et.
- Fontlar: İngilizcede LilitaOne başlıklarda kullanılabilir; Türkçe Fredoka ile kalsın.
- Supabase'e yazılan kayıtlarda değişiklik yok. Yalnız oyuncunun dil ayarını mevcut bir alanda (meta/notes) not edebilirsen et; şemaya dokunma.

## 4. "Çok sarı" sorunu (kartlar ve isim girişi)

Zeyd başlangıçta ve kazanma kartında ekranın sarıya boğulduğunu söylüyor. Bu, cihaza bağlı değil; tasarım böyle, herkes aynı şekilde görüyor. Düzelt:

- **Kart tuşları:** Bütün kartlarda yalnız Task15 tuşlarını kullan:
  - `btn-primary-v2`: yeşil, ana eylem (Next level, Continue, Start)
  - `btn-secondary-v2`: turkuaz (Play again, Menu, Restart, Main menu)

  Turuncu ve sarı hap tuşlar hiçbir kartta kalmasın. Kazanma kartında hâlâ eski turuncu "Sonraki bölüm" ve gerilmiş turkuaz tuşlar var.
- **Puan satırı (1–10):** Altın halkalı 10 büyük daire yerine tek sırada 10 küçük krem çip olsun: lacivert rakam ve ince turkuaz kontur. Seçilen çip turkuaz dolsun.
- **Beğendim / Beğenmedim:** İki küçük ikonlu tuş olsun, 👍 ve 👎 benzeri; kendimiz çiziyoruz, emoji fontu kullanma. Turkuaz kontur, krem zemin.
- **Kazanma kartının boyu kısalsın.** Sıralama yukarıdan aşağı: yıldızlar, tek satır sonuç, beğeni satırı, puan satırı, yeşil "Next level", altta yan yana iki küçük turkuaz tuş (Play again · Menu).
- **İsim girişi:**
  - Yazı alanı krem zemin, lacivert (#0B2A36) yazı. Beyaz yazı okunmuyordu.
  - En fazla 12 karakter.
  - Profil kutusundaki isim otomatik küçülsün (taşmasın); yine sığmazsa "…" ile kısalsın.
  - Ekranda büyük sarı alan olmasın: kart iç zemini krem, başlık turkuaz.

## 5. Duvar ile altın çerçeve karışıyor

Kırmızı-beyaz duvar altın çerçeve çizgisinin üstüne denk geliyor ve iki renk birbirine karışıyor.

- Duvar olan kenarda **o kenarın altın çerçeve çizgisini gizle**; duvar şeridi çerçevenin yerine geçsin. Kenar "kapalı" diye tek bakışta okunmalı.
- Duvar şeridini hücrenin yaklaşık **%25'i** kalınlığa çıkar. Altına hafif koyu gölge koy ki resmin ve mavi parçaların üstünde ayrışsın. Köşeler düzgün birleşmeli.
- Duvarsız kenarlarda altın çerçeve aynen kalsın.
- Hem düz mavi hem resimli tahtada kontrol et: bölüm 19, 30, 40 ve SHOW1.

## 6. Titreşim

PM_MESAJ_18'deki titreşim maddesini yap: iOS'ta UIImpactFeedbackGenerator.
- takılmada light impact
- bomba patlayınca heavy impact
- kazanınca success notification
- Ayarlar'dan kapatılabilsin

Web'de sessizce atla.

## 7. Teslim

- Önizleme 19'u `?set=redesign` adresine yayınla. 3 telefon boyutunda ekran görüntüsü al: menü, isim girişi, oyun, kazanma kartı, ayarlar. Hem English hem Türkçe.
- **iOS 0.5.1 (build 2)** üret: `Builds/ReverseSolver-iOS-0.5.1.zip`. Bu sürüm şunları içersin:
  - 40 tablo
  - bu turdaki her şey
  - titreşim

  MAC_YUKLEME.md'ye "sonraki yüklemeler" notunu güncelle.
- Kök adresteki Build 5'e dokunma.
- OVERNIGHT.md başına en fazla 6 satır özet yaz: önizleme linki, zip yolu, kalan sorunlar.
