# Task14 — yazı tipi önerisi

Öneri: **Lilita One Regular**. Beğenilen Level v2 tuşunun önizlemesinde kullanılan aynı font; Task14 revizyon 2'de HUD, adet ve engel sayaçlarına da uygulanarak gösterildi. Tombul, kısa metin ve rakam odaklı bu yüzü görsel uyum için seçtim. Royal Match'in kendi fontu olduğu iddia edilmiyor.

- Teslim fontu: `Assets/Art/Gameplay/Task14/LilitaOne-Regular.ttf`
- Birlikte aktarılacak lisans: `Assets/Art/Gameplay/Task14/LilitaOne-OFL.txt`
- Resmî kaynak/lisans: [Google Fonts deposu](https://github.com/google/fonts/tree/main/ofl/lilitaone), [OFL 1.1](https://github.com/google/fonts/blob/main/ofl/lilitaone/OFL.txt). Lisans fontun oyunla birlikte gömülüp dağıtılmasına izin verir; telif/lisans dosyası korunur.
- Sprite PNG'lerine yazı gömülmez; tüm sayaçlar ve etiketler ayrı TMP metinleridir. Assets/_Game içinde değişiklik yapılmadı.
- Krem zeminli HUD ve engel sayaçları: `#0B2A36`, kontursuz. Örnek telefonda HUD 25–29 px; bomba 20 px, çivi 23 px. Rakam alanı sabit tutulur; iki hane için otomatik küçültme/üst sınır kullanılabilir.
- Mercan adet rozetleri: `#F8EBD0`, çok ince `#0B2A36` kenar/gölge; 390 px telefon taslağında 18 px.
- Task14 kart/tuş metni: krem `#F8EBD0`, ince lacivert `#0B2A36` kontur/gölge. Krem yüzeylerde lacivert metin. Task14'te yalnız PALETTE.md kullanılır; başka klasördeki onaylı yeşil Level tuşu bu dışa aktarımda değiştirilmedi.
- Zeyd Task14'ü onayladı. Font dosyası değiştirilmeden, lisansıyla birlikte kopyalandı. Oyun içi TMP font atlası üretimi ve entegrasyonu Claude Code'a aittir; C# dosyasına dokunulmadı.

## TTF'den doğrulanan Türkçe kapsamı

Fontun Unicode cmap tablosu `docs/art/check-task14-font.ps1` ile doğrudan okundu. Sistem fontu fallback'i kullanılmadan glyph ID kontrol edildi. Sonuç: `docs/art/reports/task14-font-coverage.json`.

| Harf | Unicode | Fontta var mı? |
|---|---|---|
| ğ | U+011F | Hayır |
| ş | U+015F | Hayır |
| İ | U+0130 | Hayır |
| ı | U+0131 | Evet |
| ç | U+00E7 | Evet |
| ö | U+00F6 | Evet |
| ü | U+00FC | Evet |

0–9 ve süre için `:` karakteri de doğrulandı. İngilizce kısa UI ve sayaçlarda kullanılabilir. Türkçe metinlerde eksik ğ/ş/İ için Fredoka gibi Türkçe kapsamı olan bir TMP fallback fontu gerekir; eksik harfler Lilita dosyasına eklenmiş değildir.
