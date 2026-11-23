# PM_100 RAPOR

**Önemli:** ben ses dinleyemiyorum. Seçimleri ölçümle yaptım; kulakla karar Zeyd'in.

Ses yüksekliği her yerde aynı ölçümle verildi: sesin en yüksek 400 ms'si (anlık LUFS). Büyük sayı daha yüksek demek. "Telefon" sütunu 350 Hz–8 kHz bant süzgeçle yapılan kabaca bir küçük hoparlör yaklaşımı.

## 1. Kazanma fanfarı: aynı ritim, farklı tını (`&kazanmaton=`)

- **Notalar fanfar b'nin kendisi.** Kayıttan çıkarıldı: C7 arpeji (Do Mi Sol), Mi♭ (Mi♭ Sol Si♭), Fa (Fa La Do), Si♭ (Si♭ Re Fa Si♭).
  - Her nota b'deki kendi anında başlıyor; vuruşlar tam b'nin vuruşlarında.
  - Süre aynı: b 2,54 sn, b5 1,92 sn.
  - 5 vuruşlu sürüm aynı notalardan kuruldu: kesimdeki notalar çıktı, sonrakiler kesim kadar öne alındı. Kayıt kesilmedi; sesler doğal sönüyor.
- **Vuruş haritası değişmedi.** Oyun b ve b5'in haritasını kullanıyor, tını yalnız çalan dosyayı değiştiriyor.

| Bayrak | Çalgılar | Ses (LUFS) |
|---|---|---|
| `piyano` (varsayılan) | bugünkü | −12,9 |
| `marimba` | marimba + glockenspiel (sıcak, oyuncak) | −12,9 |
| `pizz` | keman pizzicato + çello pizzicato + el çanı (zarif, saray) | −12,9 |
| `ksilofon` | ksilofon + marimba bas + el çırpma (enerjik, arcade) | −12,9 |
| `kutu` | glockenspiel + kalimba, altında yumuşak synth pad (masalsı) | −12,9 |
| `bakir` | trompet staccato + tuba + timpani (ek, istenen bakır) | −12,9 |

- **Ses yüksekliği:** hepsi b'nin ses yüksekliğine eşitlendi (bütün dosya, LUFS), tepe −1 dBFS.
- **Vuruş anında atak (ms, + geç):** 5 ms'lik zarfın en dik yükselişi, vuruşun ±30 ms'sinde. Yaylı ve trompetin atağı yavaş; notalar "duyulan başlangıçları" (zarfın yarı tepeye çıktığı an) vuruşa gelecek şekilde yerleştirildi.

  | | 7 vuruş (b) | 5 vuruş (b5) |
  |---|---|---|
  | bugünkü piyano | 0, +11, +25, +15, +24, +23, +25 | 0, +11, +24, +23, +25 |
  | marimba | 0, 0, 0, +13, −1, +8, +4 | 0, 0, +10, +8, +4 |
  | pizz | 0, −6, +2, +16, +1, −4, +1 | 0, −6, +15, −4, +1 |
  | ksilofon | 0, 0, −4, −4, −4, +3, +1 | 0, 0, −4, +9, +1 |
  | kutu | 0, 0, +24, −1, +1, −1, +1 | 0, 0, +1, −2, +5 |
  | bakır | 0, +27, +5, +23, +20, +6, +16 | 0, −12, +15, −3, +17 |

  Yeni tınılar vuruşa bugünkü fanfar kadar, çoğu daha yakın oturuyor. Bakır, trompetin yavaş atağından dolayı en geniş olanı.
- **Müzik kutusu notu:** gerçek bir müzik kutusu ya da celesta kaydı bu CC0 kütüphanelerde (VCSL, VSCO 2 CE) yok. Tını glockenspiel ile kalimbadan kuruldu; ikisi de çelik dil ya da çubuk sesi.
- **Dinleme sayfası:** "Kazanma tınısı" bölümü; her tını için "Kazanma ekranında dene" ve "7 vuruşla" bağlantısı var.

## 2. Takılma ve kayma ayrıldı

- **Takılan hamle:**
  - Parça bırakılıp kaymaya başlayınca yalnız 45 ms'lik kısık bir kalkış "şş"u çalar: çıkış sesinin ilk 45 ms'si, 900 Hz altı kesik, çıkıştan 9 dB kısık.
  - Temas anında takılma sesi gelir.
  - İkisi üst üste binmez. Kalkış, kayma temasa en az 55 ms sürüyorsa çalar; daha kısa kaymada yalnız takılma sesi duyulur.
  - Boşluksuz takılmada (yer yok) yalnız takılma sesi.
- **Başarılı çıkış:** yalnız kayma sesi (değişmedi).
- **Tını ayrımı:**

  | | önce | sonra |
  |---|---|---|
  | kayma (çıkış f), spektral merkez | 2175 Hz | **3535 Hz**: 450 Hz altı kesildi; hafif, havalı |
  | takılma (çarpma a), spektral merkez | 735 Hz | **275 Hz**: tepesi yumuşatıldı, 400–1800 Hz ortası güçlendirildi; tok, kauçuk "bonk" |

  Telefon hoparlörü yaklaşımında takılma tık kadar: −22,0'a karşı tık −22,4.
- **Test** (dosyalardan, oyunun çaldığı gibi): takılma (kalkış + 70 ms sonra çarpma) ile çıkış.
  - Merkezler: takılma **592 Hz**, çıkış **3437 Hz**. Fark ~2,8 kHz, yaklaşık 2,5 oktav.
  - Kalkış 45 ms, temas 70 ms'de: üst üste binme 0 ms.
  - Sayfada "Takılma, sonra çıkış" kaydı var.
- **Oyun içi kayıt** (aynı parçayla, `Tools/pm100_record.js` + `Tools/pm100_measure.py`): sonuç aşağıda, "Oyun içi ölçüm" bölümünde.

## 3. Ses düzeyleri: yeni karışım

| Ses | Önce | Sonra | Tepe (dBFS) | Telefon önce → sonra |
|---|---|---|---|---|
| tık (UI, referans) | −16,1 | −16,1 | −1,0 | −22,4 → −22,4 |
| çıkış / kayma (f) | −19,2 | **−15,0** | −3,0 | −31,3 → **−19,7** |
| kalkış (takılmada, yeni) | – | −23,6 | −3,0 | – |
| takılma (çarpma a) | −14,7 | −14,9 | −3,0 | −23,8 → **−22,0** |
| bomba (defuse) | −16,9 | **−14,6** | −3,0 | −17,3 → −15,3 |
| kilit (açılma) | −20,5 | **−14,6** | −3,0 | −20,8 → −14,7 |
| bölüm müziği E | −24,5 | **−27,5** | −13,5 | −28,3 → −31,3 |
| lobi müziği n | −24,1 | **−27,1** | −19,5 | −28,6 → −31,6 |
| kazanma fanfarı b5 | −10,5 | −10,5 | −0,4 | (değişmedi) |

- **Oyun efektleri** (kayma, takılma, bomba, kilit) tık düzeyinin ~1,5 dB üstünde. Tepeleri −3 dBFS'de, böylece `&sfx=1.5`'e yer var.
- **Müzik** 3 dB aşağıda (0,35 → 0,248).
- **Aradaki fark:** müzik çalarken efektler artık müziğin en yüksek anından ~12 dB yukarıda; önce kayma yalnız ~5 dB üstündeydi.
- **Bayraklar:**
  - `&sfx=` efekt çarpanı (1.5 = +3,5 dB). 1'in üstünde oyun aynı sesi aynı anda ikinci bir kaynaktan da çalıyor; bir kaynağın sesi 1'de duruyor, ikisi tam toplanıyor.
  - `&muzikses=` müzik çarpanı (0.7 = −3 dB).
- **Kazanma tınıları ve fanfar değişmedi.**

## 4. Geçiş sesleri (`&gecisses=0` kapatır)

- Ayrı kanalda (kendi kaynakları), ses anahtarıyla kapanıyor.
- Her biri için 3 kayıtlı CC0 aday var; varsayılan a. Seçim bayrakları:
  - `&geciskoridor=`
  - `&gecisdalis=`: tingi de seçer.
  - `&geciscizgi=`
  - `&gecisgeri=`: HUD tıkını da seçer.
  - `&gecissekme=`

  Değerler a, b ya da c; 0 kapatır.

| Geçiş | Uyum | Düzey |
|---|---|---|
| Koridor kayışı | dokusu kayışla başlıyor. Ses her karede kayışın hızını izliyor: kalkışta artıyor, ortada en yüksek, oturuşta sönüyor. Perde de biraz değişiyor (0,9 → 1,1). | −30 (en hızlı anda) |
| Dalış | yükselen whoosh dokunuştan 0,6 sn önce başlayıp resme değdiği an bitiyor; orada ting. Dalış hızlanırsa (dokunma) başlangıç ona göre ayarlanıyor. | −30 / ting −29 |
| Çizgiler | çizgiler başlarken çok hafif kâğıt / fırça. | −36 |
| Geri çekilme | geri çekilme başlarken alçalan whoosh; HUD yerine oturunca minik tık. | −31 / tık −29 |
| Sekme kayışı | alt menüde sayfalar kayınca kısa swish (geri yaylanan kısa kaydırmada yok). | −29 |
| Panel | PM_98'deki vuş, aynı. | |

- **Düzeyler:** hepsi müziğin (−27,5) altında ya da yakınında, efektlerden 14–20 dB aşağıda.
- **Kaynaklar:** `LISANS.md`.

## Oyun içi ölçüm (aynı parça, önce takılma sonra çıkış)

- **Nasıl:** `Tools/pm100_record.js` oyunun kendi ses çıkışını sayfa içinde kaydetti (Unity'nin WebAudio'su); `Tools/pm100_measure.py` ölçtü.
  - 2. bölüm çözümüyle oynandı.
  - Önünde iki boş hücre ve ötesinde parça olan ilk parça (0) sağa iki kez fiske ile atıldı, takıldı. Sonra kendi çözüm hamlesiyle yukarıdan çıktı.

| Hamle | Ses 1 | Ses 2 | Ara |
|---|---|---|---|
| 0 → takılma (hızlı fiske, kayma 98 ms) | kalkış, 3883 Hz | takılma, +102 ms, 525 Hz | kalkış bittikten 77 ms sonra takılma |
| 0 → takılma (yumuşak fiske) | kalkış, 3610 Hz | takılma, 358 Hz | 76 ms |
| 0 ↑ çıkış (aynı parça) | kayma, **2815 Hz** | – | – |
| öteki 10 çıkış | kayma, 2718–3041 Hz | – | – |

- **Sonuç:** aynı parçada takılma 358–525 Hz, çıkış 2815 Hz. Merkezler arasında ~2,3–2,5 kHz fark var, yaklaşık 2,5 oktav. Kalkış ile takılma hiç üst üste binmedi.
- **Kısa kayma:** tek hücrelik boşluğa hızlı fiske kaymayı 30 ms'ye indiriyor. O zaman kalkış çalmıyor, yalnız takılma sesi geliyor; aksi hâlde üst üste binerdi. Kalkışın iki uzunluğu var:
  - 45 ms, kayma ≥ 55 ms;
  - 25 ms, kayma 35–55 ms.
- **Gözlem (değiştirmedim, kapsam dışı):** parmak parçayı temasa kendisi götürüp bırakınca takılma sesi ~50 ms arayla iki kez çalıyor.
  - Bir kez temas anında, bir kez bırakıştaki yaylanmada (PM_75'ten beri böyle).
  - Ses artık yüksek olduğu için daha çok fark edilebilir. Karar PM'in.
- **Kazanma tınısı oyunda:** `&kazanmaton=marimba` ile kazanma kaydedildi.
  - Kayıttaki fanfar marimba dosyasıyla 0,996 benzer; piyano dosyasıyla 0,14.
  - Ekran olayları vuruştan 16–31 ms önce tetikleniyor, piyanoyla aynı (PM_96 düzeni).
  - Müzik sesi günlükte 0,248.

## Build

- Veri +0,55 MB (Brotli toplam 39,92 → 40,45 MB): 60 yeni kısa ses (efektler ve geçişler).
- Kazanma tınıları build'e girmiyor; GitHub Pages'ten iniyor, yalnız seçilince.

## Doğrulanmayan

- Seslerin kulağa nasıl geldiği.
- Gerçek iPhone hoparlöründe denge. Telefon sütunu bir süzgeç yaklaşımı.
- iPhone uygulama sürümünde (iOS) kazanma tınıları yok; tınılar yalnız web sürümünde çalıyor, oradaki fanfar piyano.
