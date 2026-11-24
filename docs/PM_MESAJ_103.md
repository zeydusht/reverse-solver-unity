# PM_MESAJ_103: Ses seçimleri kesinleşti; varsayılan yap ve iOS build'e göm

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses varsayılanları ve paketleme.

## 1. Zeyd'in son seçimleri (24 Kasım 06:51): varsayılan

| Ses | Seçim |
|---|---|
| Lobi | n |
| Bölüm | E |
| Kazanma | g |
| Kazanma tınısı | **marimba** (değişti) |
| Çıkış | f |
| Çarpma | a |
| Tık | b |
| Makas | **c** (değişti) |
| Değnek | a |
| Çekiç | a |
| Saat | a |
| Koridor whoosh | a |
| Dalış | a |
| Çizgi | a |
| Geri çekilme | a |
| Sekme | a |

## 2. Kilitle ve temizle
- Bu seçimler `Assets/Config/ses.json` (ya da mevcut ayar yeri) içinde tek yerde dursun.
- Bayraklar test için kalsın.
- **iOS build:**
  - seçilen bütün ses ve müzik dosyaları build'in içine gömülsün; çevrimdışı da çalışsın;
  - müzikler internetten indirilmeyi beklemesin;
  - seçilmeyen adaylar iOS build'e girmesin (boyut).
  - Build boyutundaki değişimi raporla.
- **Web:** seçilen dosyalar önceden yüklenen listede, adaylar yalnız dinleme sayfasında.
- Lisans listesi (`docs/art/pm95`, `pm96`, `pm98`, `pm102` LISANS dosyaları) → tek dosya `docs/SES_LISANSLARI.md`: yalnız oyunda kullanılan sesler, kaynak, lisans (CC0), link. Mağaza ve gizlilik için.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır: "Sesler kesinleşti". Commit + push.
- Son mesaj kısa, en altta, "Oyna" linki: `p23/?c=p23&test=1&lang=tr&lv=3`.
