# PM → Claude Code (3 Kasım 2026) — PM_34'ten sonra gece işi: tam ekran turu (Codex denetimi için)

**İlk iş:** commit et. PM dosyalarını geri alma.

PM_34 bitince bunu yap. Zeyd'in kendi Codex'i bu gece `docs/art/CODEX_TASK22.md` ile tüm oyunu denetleyip yeniden çizimler hazırlıyor. O yalnız `Assets/Art/Redesign_T22/` ve `docs/art/` altına yazıyor. **Sen o klasörlere ve `Assets/Art` altındaki dosyalara dokunma.** Çakışma olmasın.

## Tam ekran turu → `docs/qa/2026-11-03-tam/`
p20, `?c=ic&unlock=1`, 390×844 ve 375×667, TR (390×844'te EN de).

Her ekranın ekran görüntüsünü al, dosya adı ekranın adı olsun:
- yükleme (yüzde göstergesi görünürken)
- isim girişi, profil sorusu
- Ev
- 4 sekme
- koleksiyon ve tablo detayı
- Jiggles
- Başarılar
- profil kartı
- Ayarlar
- Bölümler haritası (baş, orta ve son)
- Bölüm 1 eğitimi: 3 adım
- oyun ekranı: küçük tahta (bölüm 2) ve büyük tahta (bölüm 30)
- tanıtım kartları: 3, 6, 10, 14 ve booster bölümleri 5, 9, 15, 23
- zor bölüm uyarısı (19)
- kazanma kartı, bomba kaybı, süre kaybı
- durdurma kartı
- koridor geçişi (orta kare)
- 40. bölüm bitişi

Ekran görüntülerini bitirince klasöre bir `LISTE.md` koy: dosya adı → ekran.

## Kod tarafı mini denetim → `docs/UX_KOD_DENETIM.md`
Görsel değil, davranış:
- Çift tıklama ile iki kez açılan kart var mı?
- Geri tuşu ya da X her kartta çalışıyor mu?
- Kart açıkken arkadaki tahtaya tık geçiyor mu?
- Sekme geçişinde takılma var mı?
- Uzun isim ya da İngilizce metinlerde taşma var mı?
- Sesler: her tuşta tık sesi var mı, kazanma ve kayıp sesleri doğru mu?

Bulgu tablosu: ekran / sorun / önem / öneri. **Düzeltme yapma**, PM karar verecek. Tek istisna: çok küçük metin hataları.

5 dk kuralı geçerli, Unity build hariç. Bitince OVERNIGHT.md başına 2 satır.
