# PM_99 RAPOR

**Önemli:** ben ses dinleyemiyorum. Seçimleri ölçümle yaptım; kulakla karar Zeyd'in.

## 1. Bölüm müziği E (Cat in Space)

- `&bolummuzik=E` varsayılan.
- **A/B döngüsü yapıldı.** Parça aynı 32 sn'lik cümleyi iki kez çalıyor, ama iki yarı birebir aynı değil: dalga benzerliği 0,95, küçük farklar var.
  - Döngü artık 64 sn: önce 1. yarı (A), sonra 2. yarı (B).
  - B'nin son 1 sn'si parçanın sönüşüydü. Yerine A'nın kendi son saniyesi kondu: 31,0 sn'de, vuruş üstünde 40 ms çapraz geçiş. Böylece B, A gibi bitip A'nın başına akıyor.
  - Dikiş adımı 0,009; parçanın olağan adımı 0,105. Tık yok.
- Parçanın içinde üçüncü bir varyasyon yok; ölçü benzerlikleri 0,75–0,98.

## 2. Yumuşak çıkış sesi (`&sescikis=f…j`)

Hepsi kayıtlı ve CC0 (`LISANS.md`). Ortak işlem:

- 3–4 kayıt, ±%4 perde;
- en çok 0,25 sn;
- 15 ms yumuşak başlangıç (sert vuruş yok), 40 ms sönüş;
- −17 LUFS (tık −14: 3 dB altında).

| | Ses | Uzunluk | Yumuşaklık (düşük = yumuşak) |
|---|---|---|---|
| **f (varsayılan)** | Yumuşak swish | 0,17 sn | **7,8** |
| g | Kumaş sürtünmesi | 0,25 sn | 10,6 |
| j | Kağıt kayması | 0,25 sn | 14,0 |
| h | Kısa kart kayması | 0,25 sn | 15,0 |
| i | Kart itme | 0,25 sn | 15,9 |

- **Yumuşaklık puanı** dört şeyin birleşimi: tepe/ortalama oranı (sertlik), parlaklık (kHz), uzunluk ve başlangıcın hızı.
- **f neden varsayılan:**
  - En yumuşak ve en kısa.
  - Tepe/ortalama oranı en düşük, yani sivri bir vuruşu yok.
  - Parlaklığı en düşük (1,4–1,7 kHz). Diğerleri kart ve kağıt sesi olduğu için 5–7 kHz'de, tiz.
  - Boing ya da yaylanma tınısı yok.
- **Şimdiki c'nin sorunu:** "hafif whoosh + pop" idi; zıplama hissini pop kısmı veriyordu.
- **Dinleme sayfası:** "Çıkış (yumuşak)" bölümü. Her adayda:
  - ▶: her basış başka kayıt;
  - **"10 çıkış art arda":** 0,42 sn arayla on çıkışın kaydı;
  - Seç ve Oyunda dene.

  Karşılaştırma için şimdiki c'nin "10 art arda" kaydı da sayfada.

## 3. Lastik çarpma sesi (a)

- **Sesi aynı, düzeyi yükseldi:** −21,8'den −13,7…−14,4 LUFS'a, tıkla aynı seviyeye. Tepe −1 dBFS'de sınırlandı.
- **Neden hiç duyulmuyordu:** ses ~130 Hz'lik alçak bir "tok". Telefon hoparlörü ~300 Hz altını neredeyse çalmıyor; yalnız düzeyi artırmak yetmezdi.
  - Telefon hoparlörüne benzeyen süzgeçle ölçünce tık −38,6, eski çarpma −43,2 çıkıyordu (4,6 dB altında).
- **Yapılan:** sesin kendi üst harmonikleri hafif doygunlukla eklendi (350 Hz üstü). Tını hâlâ alçak bir tok, ağırlık merkezi ~800 Hz.
  - Aynı süzgeçle artık −39,4: tıkla neredeyse eşit.
- **Zamanlama:**
  - Boşluklu takılmada ses lastik 2'nin temas karesinde çalıyor; büzülme o karede başlıyor (`BoardView.RubberKnock`, `t ≥ slide`).
  - Boşluksuz takılmada ses 3 dB daha kısık (0,7). Parça zaten temas hâlinde olduğu için temas, bırakma anı.

## 4. Son parça sesi kalktı

- Son parça normal çıkış sesiyle çıkıyor.
- Ardından kazanma fanfarı geliyor (Success titreşimi aynen).
- `&sesbitis=` artık kullanılmıyor.

## 5. Kazanma ekranı: her vuruş bir öğe (`&kazanmavurus=5` varsayılan, `=7`)

- Fazladan Jiggle'lar kaldırıldı.
- Her öğe kendi vuruşunda "tak" diye geliyor: 1,15×'ten 1×'e, 0,12 sn.
- Sonraki ve Menü ayrı vuruşlarda.

| | 7 vuruş (fanfar b) | 5 vuruş (fanfar b5) |
|---|---|---|
| 1 | ışık | ışık + "Harika!" |
| 2 | "Harika!" | Jigo zıplar |
| 3 | Jigo zıplar | +50 altın (sandık çubuğuyla) |
| 4 | +50 altın | Sonraki |
| 5 | Sonraki | (son akor) Menü + konfeti |
| 6 | Menü | |
| 7 | (son akor) konfeti | |

**b5 nasıl kesildi** (`Tools/make_beatmap96.py`):

- Fanfar b'nin 3. ve 4. vuruşu çıkarıldı: 3. vuruştan 12 ms önceden 5. vuruştan 12 ms önceye kadar, 0,625 sn.
- Bu tam 4 sekizlik nota uzunluğu; ritim kaymadan sürüyor. Kesimde 10 ms çapraz geçiş var.
- Son akor aynı. Süre 2,54 → 1,92 sn.
- Araç iki harita da üretiyor: b (7 vuruş), b5 (5 vuruş; b'nin vuruşlarından kaydırılarak).

**Dokunma ve sandık:**

- Dokununca her şey hemen tamamlanıyor.
- Ekrana gelmiş bir düğme, fanfar bitmeden de basılabiliyor.
- Sandık varsa fanfardan sonra geliyor; düğmeler sandıktan sonra.

**Ölçüm** (kayıt, oyunun saatiyle): öğeler vuruştan 15–31 ms önce tetikleniyor; ekrana bir tazeleme sonra çıkıyorlar (PM_96'daki düzen).

- 7 vuruş: ışık −21, Harika −30, Jigo −20, altın −15, Sonraki −29, Menü −25, konfeti −31 ms.
- 5 vuruş: ışık + Harika −20, Jigo −29, altın −20, Sonraki −16, Menü + konfeti −21 ms.

**Video** `kazanma_7_5.mp4`:

- 10 sn, 60 fps, sesli, 1,1 MB.
- Önce soldaki 7 vuruşlu sürüm oynar, sonra sağdaki 5 vuruşlu; seslerinin üst üste binmemesi için sırayla.

## Doğrulanmayan

- Seslerin kulağa nasıl geldiği.
- Çarpma sesinin gerçek iPhone hoparlöründe duyulması. Ölçüm süzgeçle yapılan bir yaklaşık.
