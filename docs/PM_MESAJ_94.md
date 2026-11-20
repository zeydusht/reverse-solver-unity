# PM_MESAJ_94: Lastik takılma 2. tur: gidiş anında hızlı, daha fazla esneme seçenekleri

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. **Kapsam: yalnız takılma animasyonu (PM_93 §1).** Başka hiçbir şeye dokunma.

Zeyd (20 Kasım 15:35): "Lastiği bayağı beğendim."

## 1. Boşluklu takılmada gidiş: yavaş kalkış olmasın
- **Sorun:** Parçanın önünde 1–2 boş hücre, sonra engel varken sağa itince parça önce yuvasından **yavaş** çıkıyor, sonra hızlanıyor. "Struggle" hissi.
- **İstenen:** itildiği an **tam hızla fırlasın** ("zınk"), engele çarpsın, lastik gibi büzülüp geri seksin.
- **Gidiş eğrisi:** başta yavaşlatma yok.
  - İlk karede yüksek hızla başlar: fiskenin / sürüklemenin hızını devralır, en az belirli bir alt hız.
  - Çarpmaya kadar sabit ya da çok hafif artan hız.
  - Çarpmadan önce yavaşlama yok.
  - Gidiş süresi mesafeye göre kısa: 1 hücre ≈ 0,07 sn, 2 hücre ≈ 0,11 sn civarı.
- **Gidişteki uzama** (hareket yönünde) ilk karelerde hemen gelsin, çarpma anında büzülmeye dönsün.
- Geri sekme ve oturma bugünkü gibi (beğenildi).

## 2. Daha lastik: Zeyd karşılaştırıp seçecek
Zeyd: "Biraz daha gömülüp çıksın, daha lastiğimsi."

**PM görüşü:** biraz artırmak iyi olur. Çok artarsa resim ve tırnaklar fazla bozulup parça okunmaz hâle gelebilir ve "jöle" gibi durur. Bu yüzden 3 seviye hazırla, Zeyd telefonda seçsin:

| Bayrak | Çarpmada basılma | Yanlara genişleme | Not |
|---|---|---|---|
| `&lastik=1` | bugünkü | bugünkü | PM_93 |
| `&lastik=1.5` | ~%28–30 | ~%14–16 | daha derin gömülme, yuvaya dönüşte aşma biraz daha (%6) |
| `&lastik=2` | ~%36–40 | ~%18–20 | en abartılı; salınım 2 tur |

- **Gömülme hissi:** çarpan kenar engelin kenarında sabit kalsın, parçanın gövdesi o kenara doğru ezilsin. Engelin içine geçme görünümü olmasın.
- **Süre:** her seviyede toplam ≤ 0,5 sn. Art arda itişte kesilip yeniden başlar.
- **Varsayılan:** `1.5`. Zeyd seçince kalıcı yaparız.
- Boşluksuz takılmada (hemen yanında engel) aynı seviye çarpanı uygulanır; hareket yine kısa.
- Resim, tırnak, kilit ve bomba parçayla birlikte esner, kenar çizgisi kopmaz.
- GIF yarım hız, üç seviye yan yana: `docs/art/pm94/lastik_1_15_2.gif` (boşluklu + boşluksuz).

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr&lv=2&lastik=1`
  - `p23/?c=p23&test=1&lang=tr&lv=2&lastik=1.5`
  - `p23/?c=p23&test=1&lang=tr&lv=2&lastik=2`
