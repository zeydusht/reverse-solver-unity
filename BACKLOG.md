# Reverse Solver — BACKLOG

Son güncelleme: 24 Ekim 2026, 17:45 (Claude Code)

Kurallar:
- PM yalnız "PM" bölümünü, Claude Code yalnız "Mühendislik" bölümünü günceller.
- Her güncellemede en üstteki "Son güncelleme" satırı da değişir.
- Durum etiketleri: ⏳ sürüyor · 🔜 sırada · ✅ bitti · ⛔ beklemede

---

## ŞİMDİ

| İş | Kim | Durum | Tahmini süre |
|---|---|---|---|
| SET40 v3 (öğretici eğri + tutorial + Build 5 hazırlığı) — PM_MESAJ_6 | Claude Code | 🔜 gece | ~1 gece |

## ZEYD — senin işlerin

| İş | Ne zaman | Süre |
|---|---|---|
| 40'lık setten 5 zirve + 5 rastgele bölümü oyna; her bölüm için 1–10 puan + "aynı bölüm gibi mi?" | Set hazır olunca | ~25 dk |
| 40'lık seti onay kuyruğunda onayla | Yukarıdaki maddeden sonra | ~1 sa |
| CSV Excel kontrolü (36 sütun) | Boş vaktinde | 5 dk |
| iPhone telefon testi (docs/TELEFON_TESTI.md) | Build 5'ten önce | ~25 dk |
| Seviye editörü kabul turu + Zorluk paneli turu | Boş vaktinde | ~30 dk |
| GitHub Actions'taki eski re-run'ı iptal et (0552040) | Boş vaktinde | 2 dk |
| Desktop'taki saatlik gece görevini durdur | Boş vaktinde | 1 dk |
| Playtest daveti: her davetli 1–2 kişiye iletsin mi, ek bir grup var mı? (cevap bekliyor) | Playtest'ten önce | 1 dk |
| 4G kontrolü | Playtest'ten önce | 10 dk |

## PM

| İş | Durum |
|---|---|
| Oturum 5 kararı: yöntem kabul, 40'lık set onaylandı (M16 çeşitlilik eklendi) | ✅ |
| SET40 v1 → v2: zirveler yenilendi; v2 ONAYLANDI (zirveler 8–10, "en keyifli tur"); SET40 v3 öğretici eğri (Zeyd'in slot planı) + tutorial/uyarı ekranları + Build 5 hazırlığı (PM_MESAJ_6) | ✅ 21 Ekim 23:00 |
| Build 5 yayını: Zeyd'in "Build 5'i yayınla" komutunu bekliyor; sonra playtest (~20 kişi) | 🔜 |
| Görsel/ses üretimi Art Director'e (ChatGPT/Codex) devredildi: ART_DIRECTOR_BRIEF.md | ✅ 23 Ekim |
| App Store / iOS | ⛔ ertelendi |

## Mühendislik (Claude Code doldurur)

| İş | Durum | Tahmini süre |
|---|---|---|
| Codex yorumları kontrol + commit (e640763; kod aynı, yalnızca yorum + yeni testler) | ✅ | |
| SET40 v3: 12 yeni bölüm + v2 havuzu yeni eğriye dizildi (574a9ce) | ✅ | |
| Tutorial eli (1–2), tanıtım kartında N, zor bölüm uyarısı, metinler Texts.cs | ✅ kod, build bekliyor | |
| İkon + menü koridor arka planı | ✅ kod, görünüm doğrulanacak | |
| Build 5 (v3 varsayılan, ?set=classic, 👍/👎) — yerel build ✅, verify 6/7 (card ölçüm hatası); yayın Zeyd komutuyla | ✅ | |
| ASSET_IHTIYAC doğrulandı; CODEX_GOREV.md (3 görev) | ✅ | |
| HardestLevelStillUnderTwoSeconds: 3,4 sn (sınır 2) — makine yükü, kodla ilgisiz; izlenecek | ⛔ | |
| ZeydBot v2 | ⛔ setten sonra | |

## Art (ChatGPT / Codex — Art Director doldurur)

| İş | Durum | Tahmini süre |
|---|---|---|
| App ikonu: onaylı mavi sürüm, PWA/favicon/Apple touch dışa aktarımları (1024/512/256/192/180/32) | ✅ | |
| Ana menü koridoru v04: kaynak korunuyor; iki güvenli LEVEL X yerleşimi hazır (`Menu/safe-area-options-v01.html`) | ✅ | |
| LEVEL X düğmesi: turkuaz/turuncu iki @3x 9-slice adayı (`Menu/Buttons/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Tutorial eli: dokun/kaydır için eldivenli ve çıplak A/B setleri (`TutorialHand/Options/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Engel tanıtımları: çivi, bomba, zincir, duvar için A/B kadrajları (`ObstacleIntros/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Zor bölüm uyarısı: Baron/Jiggle odaklı A/B (`HardWarning/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| P0 #7–9: modal çerçevesi, kazanma ve kaybetme görselleri | 🔜 sırada | |
| P1 #10–14: oyun içi engel/booster exportları, tablolar, tahta altlığı ve ses | 🔜 sırada | |

## Bitenler (son 3 gün)

- ✅ ZeydBot v1: 30 bölümle kuruldu, oturum 3 sınavını geçti (sıra 0,99)
- ✅ Tasarımcı ajan yöntemi: Z31–Z40, 10 bölümün 8'i 8+ puan
- ✅ Yeni metrikler: M13 anahtar parça anı, M14 bomba çeşitliliği, M15 süre gerilimi
- ✅ Supabase: moves tablosu, 007 (touch/pause/puan); kayıptan sonra puan sorusu düzeltildi
- ✅ Telefondan Claude Code'a mesaj (Remote Control) çalışıyor
