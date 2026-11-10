# PM → Claude Code (3 Kasım 15:00): yayın + davet öncesi son düzeltme (p20)

**İlk iş:** commit et. PM dosyalarını asla geri alma. Codex şu an `docs/art/CODEX_TASK25.md` üzerinde çalışıyor; `Assets/Art/Paintings_v2/` ve `Redesign_T23/` klasörlerine dokunma.

## 0. Bekleyen yayın
PM_37'nin gh-pages commit'i (848fa47) push edilemedi. Önce github.com'a erişimi dene. Erişim varsa önce PM_37'yi yayınla, sonra 1'e geç. Hâlâ erişilemiyorsa OVERNIGHT'a yaz ve 1'le devam et.

## 1. Kart tıklamaları tahtaya ya da başka karta geçiyor (UX denetimi #3, #4, #6 — yüksek)
Davetten önce şart. Bu hatada oyuncunun istemediği bir hamle hem oyunu hem veriyi bozar.

Tek ve basit bir kilit yeterli:
- Kart açıkken ve kart kapandıktan sonra, parmak bırakılana kadar ve **en az 0,25 sn** boyunca yeni basma hiçbir yere gitmesin: tahtaya, menüye ya da yeni karta.
- Bekleyen kart eylemi (0,12 sn gecikme) varken gelen ikinci basmalar tamamen yutulsun. Eylem bir kez çalışsın.
- `CloseCard`, `Card.Destroy` ve `Build` çağrıları bekleyen kart tween'ini iptal etsin. Geri çağrı, kart değiştiyse hiçbir şey yapmasın.

**Test** (otomatik oynayıcıyla, `?c=ic&unlock=1`):
- (a) kazanma kartında "Sonraki bölüm"e hızlı çift tık
- (b) tanıtım kartında "Anladım"a hızlı çift tık
- (c) kart kapanır kapanmaz tahtaya tık

Üçünde de beklenmeyen hamle olmamalı: `moves`'ta fazladan satır yok, `jams` 0. Supabase'den doğrula.

## 2. UX #24 (kart yüksekliği)
Yalnız oyuncunun gerçekten görebildiği bir kart 375×667'de taşıyorsa düzelt: yüksekliği güvenli alana sınırla, gövdeyi küçült ya da kaydır. Eski LevelList kartı artık açılmıyorsa dokunma, OVERNIGHT'a "ulaşılamaz" yaz.

## 3. Küçük
Haritada şu anki düğüm zor bölümse Jiggle avatarı Baron rozetine biniyor (19). Avatarı biraz sola kaydır ya da Baron rozetini o durumda gizle.

## Bitiş
- p20'ye yayınla. Kök ve p19'a dokunma.
- `docs/qa/2026-11-03-davet/` klasörüne koy:
  - 1'in test sonuçları
  - bölüm 1, 19 ve harita ekranları
- OVERNIGHT başına 2 satır.
- 5 dk kuralı geçerli (Unity build hariç).
