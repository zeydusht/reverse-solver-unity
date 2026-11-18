# PM_86 §5: Müzik adayları ve lisansları

Beşinin lisansı da **CC0 1.0** (kamu malı): ticari kullanım serbest, atıf gerekmiyor, değiştirilebilir. Lisans metni: https://creativecommons.org/publicdomain/zero/1.0/

| Bayrak | Parça | Yazar | Sayfa (lisans orada) | Tempo | Döngü | Çalgılar (sayfadaki etiketler) |
|---|---|---|---|---|---|---|
| `&muzik=a` | Cozy Puzzle In-Game 3 | Juhani Junkala's "Cozy Puzzle" paketi | https://opengameart.org/content/cozy-puzzle-in-game-3 | 108 BPM | 124,4 sn (224 vuruş) | mallet, nefesli, akustik gitar, bas; "loopable" |
| `&muzik=b` | Cozy Puzzle Title | aynı paket | https://opengameart.org/content/cozy-puzzle-title | 95 BPM | 60,6 sn (96 vuruş) | bakır nefesli, flüt, sakin; "loopable" |
| `&muzik=c` | Cozy Puzzle In-Game 2 | aynı paket | https://opengameart.org/content/cozy-puzzle-in-game-2 | 90 BPM | 128,0 sn (192 vuruş) | bakır nefesli, synth, piyano; "loopable" |
| `&muzik=d` | Cozy Puzzle In-Game 1 | aynı paket | https://opengameart.org/content/cozy-puzzle-in-game-1 | 118 BPM | 130,2 sn (256 vuruş) | saksafon, flüt, mallet, bossa nova; "loopable" |
| `&muzik=e` | Feel Good Island | OpenGameArt | https://opengameart.org/content/feel-good-island | ~126 BPM | 106,6 sn | marimba, piyano, davul, bas; komik, mutlu, bulmaca |

**Notlar**
- Yazar adları sayfadan doğrulanmalı; lisans her sayfada CC0 olarak yazılı (OpenGameArt'ın CC0 filtresiyle bulundu).
- "Feel Good Island" sayfasında CC0'ın yanında OGA-BY 3.0 da listeleniyor (çift lisans). Biz CC0'ı seçiyoruz; yine de mağaza öncesi PM bir kez sayfadan kontrol etsin.
- Pixabay Music lisansı da uygundu (atıfsız ticari kullanım). Ancak site otomatik indirmeyi engelliyor (403), o yüzden bu turda kullanılamadı.
- Mixkit elendi: lisansı video oyunlarında kullanımı yasaklıyor.
- Atıf gerektiren (CC-BY, OGA-BY) ya da yalnız kişisel kullanıma izin verenler alınmadı.

**Döngü ve ses**
- Cozy Puzzle parçaları baştan döngü için yapılmış: süreleri tam vuruş sayısına denk. Olduğu gibi kullanıldı, kesilmedi.
- Feel Good Island'ın 3 sn'lik yankı kuyruğu döngünün başına katlandı; döngü tam ölçüye kesildi.
- Dikiş yerinde tık yok: örnek sıçraması parçanın kendi adımlarının %99,9'undan küçük (`Tools/make_music86.py` çıktısı).
- Ses seviyesi eski müzikle aynı yükseklikte (RMS 0,085), oyundaki müzik sesi efektlerin altında (0,35).
- a, c, d 120 sn'den biraz uzun (124–130 sn). Kısaltmak döngüyü bozacağı için tam bırakıldı.
- d (118) ve e (~126) istenen 90–110 BPM aralığının biraz üstünde.

**Doğrulanmayan:** parçaların kulağa nasıl geldiği (ben dinleyemiyorum). Seçimi Zeyd kulakla yapacak. Hiçbiri beğenilmezse PM'in dediği gibi Suno benzeri araç konuşulur.
