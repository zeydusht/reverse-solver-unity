# Reverse Solver — BACKLOG

Son güncelleme: 21 Ekim 2026, 17:00 (Claude Code)

Kurallar:
- PM yalnız "PM" bölümünü, Claude Code yalnız "Mühendislik" bölümünü günceller.
- Her güncellemede en üstteki "Son güncelleme" satırı da değişir.
- Durum etiketleri: ⏳ sürüyor · 🔜 sırada · ✅ bitti · ⛔ beklemede

---

## ŞİMDİ

| İş | Kim | Durum | Tahmini süre |
|---|---|---|---|
| SET40 v1 yayında (Build 5z6), Zeyd'in onay oturumu bekleniyor | Zeyd | 🔜 | ~20 dk |

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
| Set gelince: 10 bölümlük oynama listesi + Zeyd'in puanlarıyla karar | 🔜 |
| Build 5 + playtest planı (kök adres yeni sete geçer, 👍/👎 açılır, ~20 kişi) | ⛔ set onayından sonra |
| App Store / iOS | ⛔ ertelendi |

## Mühendislik (Claude Code doldurur)

| İş | Durum | Tahmini süre |
|---|---|---|
| Oturum 5 sonucu RAPOR.md'ye + Z36–Z40 benzerlik kontrolü | ✅ 4c910f6 | |
| M16 çeşitlilik ölçümü (`Tools/level_similarity.py`, eşik önerisi %65) | ✅ | |
| Havuz ölçümü: redesign'da 7×8 dışı zor malzeme yok | ✅ | |
| SET40 v1: 40 slot seçimi (M16 < %65, en yüksek %64) + SET40.md | ✅ | |
| SET40 v1: redesign.json + build + doğrulama + yayın (f9aaa8a, gh-pages 1cada35) | ✅ | |
| Farklı boyutta zor üretimi (SET40 v2 için) | 🔜 | ≤ 1 gece |
| ZeydBot v2 (oturum 4–5 verisiyle) | ⛔ setten sonra | |

## Bitenler (son 3 gün)

- ✅ ZeydBot v1: 30 bölümle kuruldu, oturum 3 sınavını geçti (sıra 0,99)
- ✅ Tasarımcı ajan yöntemi: Z31–Z40, 10 bölümün 8'i 8+ puan
- ✅ Yeni metrikler: M13 anahtar parça anı, M14 bomba çeşitliliği, M15 süre gerilimi
- ✅ Supabase: moves tablosu, 007 (touch/pause/puan); kayıptan sonra puan sorusu düzeltildi
- ✅ Telefondan Claude Code'a mesaj (Remote Control) çalışıyor
