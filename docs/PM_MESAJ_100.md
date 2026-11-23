# PM_MESAJ_100: Kazanma fanfarına farklı tınılar (aynı ritim); takılma ve kayma sesleri ayrışsın ve duyulsun; geçişlere "whoosh" sesleri

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama. Kapsam: yalnız ses.

Zeyd (23 Kasım 12:56): "Kazanma ekranında piyano iyi, müzik ve ekran tam oturmuş."

## 1. Kazanma fanfarı: aynı ritim, farklı tını (A/B)
- Bugünkü fanfarın (b, 5 vuruş) **ritmi, vuruş zamanları ve uzunluğu birebir aynı** kalsın. Vuruş haritası değişmesin, ekran senkronu bozulmasın.
- Aynı notaları ve ritmi farklı çalgılarla çal (CC0 gerçek çalgı kayıtları, PM_96'daki gibi). En az 4 tını:
  - **marimba + glockenspiel** (sıcak, oyuncak);
  - **pizzicato yaylılar + çan** (zarif, "old money" saray);
  - **ksilofon + el çırpma** (enerjik, arcade);
  - **müzik kutusu / celesta + yumuşak synth pad** (masalsı).

  Bulursan bakır nefesli (kısa trompet vurguları) da ekle.
- Piyano (bugünkü) de seçenek olarak kalır.
- Ses yüksekliği hepsinde eşit.
- **Bayrak:** `&kazanmaton=piyano|marimba|pizz|ksilofon|kutu`. Varsayılan: piyano.
- Dinleme sayfasında "Kazanma tınısı" bölümü.

## 2. Takılma ve kayma sesleri karışmasın
**Kural:**
- **Takılan hamlede** kayma sesi tam çalmaz. Yalnız çok kısa ve hafif bir kalkış "şş"u (≤ 60 ms, kısık), ardından temas anında takılma sesi. İkisi üst üste binmez.
- **Başarılı çıkışta** yalnız kayma sesi.
- **Tını ayrımı:** kayma sesi hafif ve havalı (yüksek frekans ağırlıklı whoosh). Takılma sesi tok ve kauçuk (alçak/orta frekans "bonk"). Gerekirse filtreyle ayır (kaymada düşük frekansları kes, takılmada yüksekleri yumuşat).
- **Test:** aynı parçayla önce takılma, sonra başarılı çıkış kaydı; iki ses kulakla değilse ölçümle ayrışıyor mu (spektral merkezler arası fark) rapora yaz.

## 3. Ses düzeyleri: efektler hâlâ arkada
Zeyd: "Takılma ve sürükleme sesleri çok arkada kalıyor, duyulmuyor."
- **Karışımı yeniden dengele:**
  - müzik (lobi ve bölüm) ~3 dB aşağı;
  - oyun efektleri (kayma, takılma, bomba, kilit) tık düzeyine ya da 1–2 dB üstüne.
- **Hedef:** telefon hoparlöründe orta seste, müzik çalarken her hamle net duyulsun.
- **Ayar bayrakları** (Zeyd hızlı denesin): `&sfx=` (efekt çarpanı, varsayılan 1, ör. 1.5) ve `&muzikses=` (müzik çarpanı, ör. 0.7). Zeyd uygun değeri söyleyince varsayılan yaparız.
- Ölçüm tablosu: her sesin oyundaki son seviyesi (LUFS / tepe), önce/sonra.

## 4. Geçişlere ses efektleri (whoosh)
Hafif, müziğin altında kalan, yorucu olmayan geçiş sesleri. CC0 kayıtlı, her biri için 3 aday.

| Geçiş | Ses | Uyum |
|---|---|---|
| Kazanma → koridor kayışı (duvar sağa kayarken) | yumuşak "whoosh" | sesin yüksekliği kayışın hız eğrisini izlesin: kalkışta artar, ortada en yüksek, oturuşta söner |
| Dalış: tabloya yaklaşma | içeri doğru yükselen hafif "whoosh" | resme değdiği an yumuşak bir parıltı / "ting" |
| Çizgiler oluşurken | çok hafif "kâğıt / fırça" dokusu | isteğe bağlı, abartma |
| Geri çekilme + HUD gelişi | yumuşak ters whoosh | HUD otururken minik "tık" |
| Alt menü sekme kayışı | kısa, çok hafif swish | |
| Panel açılış/kapanış | PM_98'deki vuş (aynı kalsın) | |

- Geçiş sesleri ayrı bir kanalda; ses anahtarıyla kapanır.
- Bayrak `&gecisses=0` ile kapatılabilsin (karşılaştırma için).
- Dinleme sayfasında "Geçiş sesleri" bölümü + oyunda canlı.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/onizleme/sesler.html` (kazanma tınıları, geçiş sesleri)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (yeni karışım + geçiş sesleri)
  - `p23/?c=p23&test=1&lang=tr&lv=3&sfx=1.5&muzikses=0.7` (daha yüksek efekt denemesi)
  - `p23/?c=p23&test=1&lang=tr&lv=3&kazanmaton=marimba`
