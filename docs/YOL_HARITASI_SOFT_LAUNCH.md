# Reverse Solver — Soft Launch Yol Haritası (7 gün)

Hazırlayan: PM · 17 Kasım 2026 akşamı · Hedef: **24 Kasım Salı App Store'da canlı (yalnız Türkiye)**

## Alınan kararlar
- Kanal: iOS App Store, ülke yalnız **Türkiye**, ücretsiz, iPhone (iPad yok).
- Hedef: **100–150 indirme**, kişisel reklam bütçesi 300–500 TL.
- Para: **AppLovin MAX** reklam (ödüllü + geçiş). Uygulama içi satın alma (IAP) v1'de **yok** (Apple banka/vergi sözleşmesi zaman alır).
- İçerik: **100 bölüm** (1–40 mevcut, 41–100 yeni) ve 100 tablo.
- Animasyonlar mükemmel olmak zorunda değil. **Özellik dondurma: 19 Kasım akşamı.** Sonrasında yalnız bölüm, hata ve mağaza işi.

## Ölçülecekler (soft launch hedefleri, kaba)
| Ölçü | Hedef | Not |
|---|---|---|
| D1 tutma | ≥ %35 | Bulmaca türü için iyi sayılan seviye |
| D7 tutma | ≥ %10 | |
| Günlük oyun süresi | ≥ 15 dk | |
| 10. bölüme ulaşan | ≥ %60 | pt1'de birçok kişi ilk 10'da bıraktı |
| Çökme olmayan oturum | ≥ %99 | |
| Ödüllü reklam izleme oranı | %15–25 (DAU başına) | |
| Kurulum maliyeti (CPI) | 1. günden ölç | |

100–150 indirmede oranlar kabaca ±%8–10 oynar. Kesin sonuç değil, yön gösterir.

## Gün gün

### Gün 0 · Salı 17 Kasım (bu akşam)
- **Claude Code:** `docs/PM_MESAJ_81.md` (mağaza sürümü altyapısı: `live/` yolu, gömülü bölüm yedeği, AppLovin MAX, ATT izni, `sl1` telemetrisi, uzaktan ayar, 100 bölüm desteği, iPhone 1.0.0).
- **Zeyd:**
  1. Dalış için A / B / C seçimini yaz (PM_80). Seçim gelmezse varsayılan A kalır, ayrıca uğraşmıyoruz.
  2. AppLovin hesabı aç (aşağıdaki "Zeyd'in adımları").

### Gün 1 · Çarşamba 18 Kasım
- **Claude Code:** PM_81 biter. Ardından **PM_82**: 100 bölüm planı (1–100 yeniden dizilim, model v2 + tasarımcı ajan), harita kapalı, rehberli 1. bölüm, isteğe bağlı reklam, kazanma düğmeleri.
- **Codex:** **TASK51 = 60 yeni tablo** (41–100), üç parti: 41–60, 61–80, 81–100. Bölümler ve tablolar Pages'ten indiği için 61–100 tablolar yayından sonra da eklenebilir; yeni build gerekmez.
- **Zeyd:** App Store Connect'te mağaza bilgileri (adım adım liste PM'den gelecek): uygulama gizliliği (reklam verisi), yaş sınırı, yalnız Türkiye, ücretsiz.

### Gün 2 · Perşembe 19 Kasım — ÖZELLİK DONDURMA
- **Claude Code:** PM_83 = Zeyd'in görsel düzeltme listesi + bilinen hataların kapanışı + 1–100 otomatik tur (her bölüm çözülebilir, kenar boşluğu 0, 60 fps).
- **Zeyd:** telefonda 1–40 kesintisiz tur + 41–60'tan seçme bölümler; not alıp PM'e at.
- **PM:** 41–100 zorluk eğrisini kontrol eder (zor/nefes ritmi), gerekirse ayar ister.

### Gün 3 · Cuma 20 Kasım
- **Claude Code:** PM_84 = mağaza paketi: ekran görüntüleri (TR + EN, App Store Connect'in istediği iPhone boyutu), `app-ads.txt` (`zeydusht.github.io` kök sitesi), gizlilik sayfası güncel, **iOS 1.0.0 (8) zip** + `rs-yukle.sh` + `MAC_ADIMLAR_1.0.0.txt`.
- **PM:** mağaza açıklaması (TR/EN), anahtar kelimeler, alt başlık.
- **Zeyd:** 2–3 reklam videosu (oyun kaydından 10–15 sn; TikTok/Higgsfield).

### Gün 4 · Cumartesi 21 Kasım
- **Zeyd:** Mac'te yükleme (`cd Downloads` → `bash rs-yukle.sh`).
- **Risk:** Mac'li arkadaş ~21 Kasım dönüyor. Dönmezse **B planı:** saatlik kiralık Mac (MacinCloud vb.) ya da Unity Build Automation (`docs/CLOUD_BUILD.md`). Karar Cuma akşamı.
- İşlenince TestFlight (iç grup) ile iPhone'da duman testi: reklamlar (test modu), ATT izni, Supabase'e `sl1` olayları düşüyor mu.

### Gün 5 · Pazar 22 Kasım
- Duman testinde çıkanlar düzeltilir. Gerekirse 1.0.0 (9).
- **İncelemeye gönder:** "Elle yayınla" seçili, yalnız Türkiye.

### Gün 6 · Pazartesi 23 Kasım
- Apple incelemesi (çoğunlukla 1–2 gün).
- **PM:** Supabase görünümleri hazır: `v_sl_tutma` (D1/D3/D7), `v_sl_huni`, `v_sl_reklam`. Apple kampanya linkleri (kanal başına: `meta`, `tiktok`, `pt1`).
- **Zeyd:** Meta reklam hesabı, kampanya taslağı (aşağıda).

### Gün 7 · Salı 24 Kasım — YAYIN
- Onay gelince **Yayınla**.
- Reklam başlar: 300–500 TL'yi 4–5 güne yay. pt1 grubuna ve TikTok'a mağaza linki.
- **1. gün kontrolü:** CPI. Bütçe 100–150 indirmeye yetmeyecek gibiyse ya bütçe artar ya hedef 60–80'e iner. Organik kanallar (pt1, TikTok) eksiği kapatır.

### Sonrası (Gün 8–14)
- Her akşam PM raporu: indirme, D1/D3, bırakma bölümleri, reklam geliri.
- 1 Aralık civarı **D7 kararı:** devam / düzelt / dur. Bölüm ayarları build'siz `live/` üzerinden yapılır.

## Reklam (UA) planı
- AppLovin MAX **para kazanma** için. AppLovin'in reklam verme tarafı bu küçük bütçeye uygun değil, bu yüzden **indirmeleri Meta getirir** (Instagram Reels ağırlıklı).
- v1'de Meta SDK yok. Kampanya App Store linkine gider, linkte Apple kampanya kodu olur. İndirmeleri App Store Connect → App Analytics → Kaynaklar'da kanal kanal görürüz. Kesin oyuncu sayıları Supabase'ten gelir.
- Hedefleme: Türkiye, iOS, 18–44, ilgi alanı bulmaca/mobil oyun. 2–3 video yarışır, ilk 2 günden sonra kazanan kalır.

## Reklam yerleşimi (oyun içi)
- **Ödüllü 1:** Süre doldu → "Reklam izle, +30 sn" (altınla devam seçeneği de kalır).
- **Ödüllü 2:** Kazanma ekranı → "Reklam izle, altını 2× al".
- **Geçiş reklamı:** 12. bölümden sonra, yalnız kazanmadan sonra, en az 3 bölümde ve 120 sn'de bir. Kayıptan sonra yok, ilk oturumun ilk 10 dakikasında yok.
- Banner **yok** (HUD tasarımı bozulmasın).
- Sıklık `live/config.json`'dan değişir, build gerekmez.

## Zeyd'in adımları (AppLovin, ~15 dk)
1. `dash.applovin.com` → hesap aç (bireysel, "Zeyd Tekin").
2. Account → Keys → **SDK Key**'i kopyala.
3. MAX → Manage → Ad Units → Create Ad Unit: platform **iOS**, paket `com.zeydtekin.reversesolver`. İki tane aç: **Rewarded** ve **Interstitial**.
4. SDK Key + iki Ad Unit ID'yi PM'e gönder. Claude Code bunları `Assets/Config/ads.json`'a yazar.
5. Ödeme ve vergi bilgisi şimdilik gerekmez, ilk ödemeden önce doldurulur.

## Riskler
| Risk | Etki | Önlem |
|---|---|---|
| Mac zamanında yok | Yayın kayar | B planı: kiralık Mac / Unity Build Automation; Cuma karar |
| Apple reddi (reklam/gizlilik) | +1–3 gün | ATT metni, gizlilik etiketleri, gizlilik sayfası PM_81'de eksiksiz |
| 60 tablo yetişmez | Görsel tekrar | 41–60 şart; 61–100 geçici tekrar tablo, sonra build'siz değişir |
| Bütçe indirmeye yetmez | Az veri | 1. gün CPI kontrolü, organik destek, hedefi indir |
| Yayın sürümü p23 geliştirmesinden etkilenir | Canlı oyun bozulur | Mağaza sürümü yalnız `live/` okur; `live/`'a yalnız PM onayıyla yayın |
