# PM_MESAJ_99: Bölüm müziği E; daha yumuşak çıkış sesi; lastik çarpma sesi duyulsun; son parça sesi kalksın; kazanma ekranı: her vuruş bir öğe, 7 / 5 vuruş A/B

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses ve kazanma ekranı zamanlaması.

Zeyd (23 Kasım 10:34): "E'yi seçtim, baya iyi oldu. Müzikler iyi, ses efektleri kaldı."

## 1. Bölüm müziği
`&bolummuzik=E` (Cat in Space) varsayılan.

**Not:** döngü 32 sn, kısa. Tekrar fark edilirse diye parçanın kendi içinden ikinci bir varyasyon çıkarılabiliyorsa (farklı başlangıç noktası, bir bölüm atlama) A/B döngüsü yap: 32 + 32 sn sırayla, dikiş tıksız. Olmuyorsa olduğu gibi kalsın ve raporda yaz.

## 2. Çıkış sesi: daha yumuşak sürükleme
- **Sorun:** Parça çıkarken "boing" gibi ses geliyor, çok duyunca sıkıyor.
- **İstenen:** yumuşak, kısa, kulağı yormayan **sürükleme / kayma** sesi: hafif "şşuh" (whoosh), tahta ya da kartonun yumuşak kayması, kağıt sürtünmesi. Boing, zıplama, yaylanma tınısı yok.
- En az 5 CC0 aday (kayıtlı, sentetik değil). Her biri 3–4 varyasyon + ±%4 perde.
- Kısa: ≤ 0,25 sn, yumuşak başlangıç (sert transient yok).
- Ses düzeyi tıkın biraz altında; art arda 10 çıkışta yorucu olmasın. Ölçüt: aynı sesin 10 kez art arda kaydını dinleme sayfasına koy ("10 çıkış art arda" düğmesi).
- Dinleme sayfasında "Çıkış (yumuşak)" bölümü, `&sescikis=` harfleri devam eder. Varsayılan: en yumuşak ve en kısa olan; gerekçeyi yaz.

## 3. Lastik çarpma sesi duyulmuyor
- Şimdiki ses (a) kalsın, **sesi yükselsin**: tık ile aynı seviyede ya da 1–2 dB üstünde (yaklaşık −14 LUFS kısa ses).
- Lastik 2'nin büzülme anına tam otursun (temas karesi).
- Boşluksuz takılmada da duyulsun, biraz daha kısık.

## 4. Son parça sesi kalksın
Son parçada ayrı ses çalmasın. Son parça da normal çıkış sesiyle çıksın, ardından kazanma fanfarı gelir.

## 5. Kazanma ekranı: her vuruş bir ekran öğesi; fazladan Jiggle'lar kalksın
Zeyd: "Ekstra Jiggle gelmesine gerek yok. Her vurgu bir butona otursun: ilk vuruşta Harika, sonrakinde Jiggle, sonrakinde Sonraki Bölüm, sonrakinde Ana Menü… 7 vuruş uzun gelmiş olabilir, 5'e kısaltılabilir. A/B yapalım."
- Her vuruşta bir Jiggle'ın çıkması **kaldırılsın**. Ekranda yalnız asıl öğeler.

**A: 7 vuruş** (fanfar b olduğu gibi):

| Vuruş | Olay |
|---|---|
| 1 | ışık huzmesi |
| 2 | "Harika!" |
| 3 | Jigo zıplar (mutlu) |
| 4 | +50 altın |
| 5 | "Sonraki Seviye" |
| 6 | "Menü" |
| 7 | (son akor) konfeti |

**B: 5 vuruş** (fanfar b'nin 5 vuruşa kısaltılmış hâli):
- Ortadaki iki vuruşu çıkar ya da birleştir, son akor aynı kalsın. Kesimler tıksız, müzikal olarak doğal dursun.

| Vuruş | Olay |
|---|---|
| 1 | ışık + "Harika!" |
| 2 | Jigo zıplar |
| 3 | +50 altın |
| 4 | "Sonraki Seviye" |
| 5 | (son akor) "Menü" + konfeti |

**Ortak kurallar:**
- Her öğe kendi vuruşunda "tak" diye gelir: 1,15×'ten 1×'e, 0,12 sn. Vuruştan 10–20 ms önde başlar.
- Dokununca her şey hemen tamamlanır; düğmeler fanfar bitmeden de basılabilir hâle gelir.
- Sandık varsa fanfardan sonra.
- **Bayrak:** `&kazanmavurus=7` / `&kazanmavurus=5`. Varsayılan **5**; Zeyd ikisini deneyip seçecek.
- Vuruş haritası aracı (`make_beatmap96.py`) iki sürüm için de harita üretsin.
- **Video:** iki sürüm yan yana, sesli, `docs/art/pm99/kazanma_7_5.mp4`.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (yumuşak çıkış adayları, 10 art arda)
  - `p23/?c=p23&test=1&lang=tr&lv=3&kazanmavurus=5`
  - `p23/?c=p23&test=1&lang=tr&lv=3&kazanmavurus=7`
