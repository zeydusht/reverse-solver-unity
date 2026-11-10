# PM → Claude Code (3 Kasım 15:50): yeni tablolar + yeni kartlar oyuna (p20, davetten önce)

**İlk iş:** commit et. PM dosyalarını asla geri alma. PM_38 bitmediyse önce onu bitir.

Zeyd yeni stili onayladı. Davetler bu iş bitince gidecek. Codex'in çizim klasörlerinde (`Assets/Art/Paintings_v2/`, `Assets/Art/Redesign_T23/`) dosya **değiştirme**, oradan **kopyala**.

**Claude'un haftalık limiti az (%67 kullanıldı).** Sırayla yap, her adım sonunda commit et. Limit biterse kaldığın adımı OVERNIGHT'a yaz.

## 1. 40 tablo (öncelik 1)
- Kaynak: `Assets/Art/Paintings_v2/painting-01..40.jpg` + `manifest-v2.json` (başlıklar TR/EN).
- Oyunun tablo yükleme yolunu bu dosyalara çevir:
  - koleksiyon
  - Ev'deki büyük tablo
  - bölüm tahtası
  - koridor geçişi
  - tablo detayı
- **Bölüm geometrisi, parça sayısı ve `level_version` DEĞİŞMEYECEK.** Yalnız görsel değişiyor. Tahmin sistemi buna bağlı.
- Eski `Paintings/` silinmeyecek. Build'e girmesin yeter (boyut büyümesin).
- İndirme boyutunu ölç, önceki 13,83 MB ile kıyasla. 1 MB'tan fazla artarsa sıkıştırmayı ayarla.

## 2. Kartlar (öncelik 2, sırayla)
- Kaynak: `Assets/Art/Redesign_T23/NIHAI/<aile>/` (her ailede `NOT.md`). Yapılacaklar listesi: `docs/art/T23_KOD_LISTESI.md`.
- Sıra:
  1. Kazanma (02) + puan kutuları (06)
  2. Kayıp: bomba (03), süre (04); durdurma (05)
  3. Tanıtım kartları: 07–10 (çivi, bomba, zincir, duvar)
  4. Booster kartları: 11–14
  5. Zor bölüm (15)
  6. İsim (17): yer tutucu soluk gri, yazılan isim koyu
  7. Profil sorusu (18)
- **Kurallar:**
  - Metinler PNG'de değil, oyunun kendi Nunito Black metni olacak.
  - Mevcut TR/EN metin anahtarlarını kullan.
  - Zor bölüm kartının metni: "Bu bölüm biraz daha zorlu. Acele etme, önce bak." / EN "This one is a bit harder. Take your time, look first."
- **PM_38'deki tıklama kilidi bozulmayacak.** Kart değişse de kilit aynı kalsın.
- 1–10 kutuları, 👍/👎 ve "Puanlamayı geç" akışı ile `set_rating` aynı kalacak. Veri akışına dokunma.

## Kontrol
- `?c=ic&unlock=1` ile kontrol et, 390×844 ve 375×667:
  - Bölüm 1 eğitimi
  - Bölüm 3 (çivi tanıtımı)
  - Bölüm 5 (makas)
  - Bölüm 19 (zor)
  - Bölüm 2: kazanma kartı + 7 puanı
  - Bölüm 28: bomba kaybı
  - Koleksiyon ve koridor
- Supabase'den doğrula:
  - `plays` satırı geliyor
  - `zorluk_puani` 7
  - fazladan `moves` satırı yok
- Ekran görüntüleri: `docs/qa/2026-11-03-yeni-gorunum/`
- p20'ye yayınla. Kök ve p19'a dokunma.
- Sürüm etiketini `web-p20-2026-11-03` yap. Davet verisi bu sürümle ayrılacak.
- OVERNIGHT başına 2 satır.
