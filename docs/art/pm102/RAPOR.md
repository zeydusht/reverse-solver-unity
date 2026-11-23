# PM_102 RAPOR

**Önemli:** ben ses dinleyemiyorum; bütün seçimler ölçümle. Ses yüksekliği: sesin en yüksek 400 ms'si (anlık LUFS).

## 1. Kazanma fanfarı: temel g ("Piyano ta-da")

- **Varsayılan artık g.** g'nin kendi vuruş haritası zaten 5 vuruş: 0 / 0,30 / 0,60 / 0,90 / 1,45 sn. Kazanma ekranı bunlara oturuyor:
  1. ışık + "Harika!"
  2. Jigo
  3. +50 altın
  4. Sonraki
  5. Menü + konfeti
- **Tınılar** (`&kazanmaton=`): g'nin kendi partisyonu (`Tools/make_fanfare96.py`) başka çalgılarla çalındı (`Tools/make_fanfare102.py`). Melodi, ritim ve süre (2,45 sn) aynı.
  - `marimba`: marimba + glockenspiel
  - `pizz`: pizzicato yaylılar + çan
  - `ksilofon`: ksilofon + el çırpma
  - `kutu`: müzik kutusu (glockenspiel + kalimba) + yumuşak pad

  Hepsi g'nin ses yüksekliğinde (−12,9 LUFS, tepe −1 dBFS).
- **Vuruşlardaki atak** (5 ms zarfın en dik yükselişi, ms; + geç):

  | | 1 | 2 | 3 | 4 | 5 |
  |---|---|---|---|---|---|
  | g (piyano) | 0 | +16 | +20 | +22 | +15 |
  | marimba | 0 | +17 | +13 | −2 | +25 |
  | pizz | 0 | 0 | +12 | +14 | +4 |
  | ksilofon | 0 | +20 | +11 | +17 | −4 |
  | kutu | 0 | +27 | +20 | +18 | +19 |

  Tınılar vuruşa g'nin kendisi kadar oturuyor.
- **b ve b'den türetilenler** sayfada "eski" bölümünde. Oyunda `&kazanma=b&kazanmaton=…` ile hâlâ açılıyor.

## 2. Lastik çarpma sesi: sabit ve net

**Neden bazen kısık, bazen yüksekti** (bulunan sebepler):

1. Boşluksuz takılma sesi **3 dB kısık** çalıyordu (PM_99'dan kalma 0,7 çarpanı). En büyük fark buydu.
2. Perde rastgeleliği **±%5**'ti.
3. Dört kaydın uzunluğu farklıydı: 0,12–0,18 sn. Kısa olan kulağa daha kısık gelir.
4. Parmak parçayı temasa kendisi götürüp bırakınca ses **~50 ms arayla iki kez** çalıyordu: temasta bir, bırakıştaki yaylanmada bir.

Kayıtların kendi ses yükseklikleri arasında ise yalnız 0,7 dB fark vardı; tepeleri eşitti.

**Yapılan:**

- **Kayıtlar:** dördü de aynı uzunlukta (0,15 sn) ve aynı tepede (−1,5 dBFS). Ses yükseklikleri −13,2…−13,8 LUFS. Önce −14,6…−15,3'tü.
- **Seviye:** kaymanın oyunda çalan düzeyinden (−16,0) **2,5 dB üstte**.
- **Boşluksuz takılma:** aynı seviye.
- **Rastgelelik:** yalnız perde (±%3); ses seviyesi rastgele değil.
- **Çift ses:** 0,15 sn içinde ikinci takılma sesi çalmıyor.

**Ölçüm** (oyunun kendi ses çıkışının kaydı, `Tools/pm102_record.js --mode jams`):

| | Takılma | Tepeler |
|---|---|---|
| boşluklu | 10 | +0,63 … +1,38 |
| boşluksuz | 10 | +0,71 … +1,50 |
| **20'si birden** | 20 | **fark 0,87 dB** (hedef ≤ 1) |

Kayıt zinciri (tarayıcının Opus kaydı) keskin seslerin tepesini 1–3 dB yüksek gösteriyor. Mutlak değerler bu yüzden cihazdaki tepe değil; anlamlı olan aralarındaki fark.

## 3. Kayma sesi 1 dB kısıldı

Oyunda 0,891 çarpanla çalıyor: −15,0'dan −16,0 LUFS'a.

## 4. Booster sesleri

CC0 gerçek kayıtlar, oyuncak tadında (tepesi yumuşatılmış, kısa), oyun efekti seviyesinde (−14,6 LUFS). Her biri için 3 aday var; `&sesmakas=` `&sesdegnek=` `&sescekic=` `&sessaat=` (a/b/c, varsayılan a). Kaynaklar `LISANS.md`'de.

| Booster | Ne zaman | Ses |
|---|---|---|
| **Makas** | bıçaklar kapanıp parça koptuğu karede | gerçek makas "şık" (eski tık ve kilit sesi kalktı) |
| **Değnek** | parıltı dönüşümden 0,4 sn önce başlar, "puf" dönüşüm karesinde | rüzgâr çanı / glockenspiel glisandosu / çan ağacı + yumuşak puf |
| **Çekiç** | darbe karesinde vuruş + kırılma (tahta / seramik); parça kırılıp düşerken hafif tıkırtı (8 dB kısık) | |
| **Saat** | saat zamanlayıcıya değince buz çatırtısı; donma boyunca kısık tik-tak döngüsü (−26,6); buzlu bar kırılırken buz kırılması | |

PlayMode testi saatin üç sesinin çaldığını ve tik-takın çözülmeyle durduğunu doğruluyor. Makas, değnek ve çekiç sesleri animasyonun kendi karesine bağlı ama oyun içinde kayıtla ölçülmedi.

## 5. Geçiş sesleri: koridor whoosh

- **Sebep:** koridor sesi çalıyordu ama çok kısıktı: en hızlı anında −30 LUFS, hıza göre de kısılıyordu, geri gelen lobi müziğinin altında kalıyordu.
- **Yapılan:**
  - 5 dB yükseltildi (−25).
  - Ses hızın 0,75 üssüyle izliyor: kalkışta daha erken duyulur, ortada en yüksek, oturuşta söner.
  - Kayışın ilk hareket karesinde başlıyor.
- **Dalış whoosh'u:** koridordan sonra yaklaşma 0,5 sn, whoosh 0,6 sn. Whoosh artık kendi ortasından başlıyor, sonu tam değme anına denk geliyor.
- **Ölçüm** (`Tools/pm102_record.js --mode gecis`):
  - koridor whoosh'u kayış başladıktan 27 ms sonra duyuluyor;
  - en yüksek anı kayışın 0,61. sn'si (ortası 0,65);
  - kayış süresince whoosh −26,0 LUFS, aynı anda müzik −32,2: 6 dB üstte.

**Bütün geçişler:** olay zamanı ve sesin kayıtta başladığı an (fark, ms):

| Geçiş | Olay | Ses | Fark |
|---|---|---|---|
| Kazanma → koridor | kayışın ilk hareket karesi | koridor whoosh | +27 |
| Koridor → bölüm (dalış) | değmeye 0,50 sn kala | dalış whoosh | +18 |
| | değme | ting + kâğıt | +24 |
| | geri çekilme | ters whoosh | −29 |
| | HUD oturdu | tık | +12 |
| Bölüm → menü | duraklat → Ana menü | (tanımlı geçiş sesi yok; panel vuşu PM_98'deki gibi) | – |
| Sekme kayışı | sayfa kayışı (2 kez) | swish | +9 / +16 |
| Menü → bölüm (dalış) | değmeye 0,59 sn kala | dalış whoosh | +97 * |
| | değme | ting + kâğıt | +16 |
| | geri çekilme | ters whoosh | −29 |
| | HUD oturdu | tık | +12 |

- **\*** Dalış whoosh'u çok kısık başlayıp yükseliyor. Ölçüm eşiği onu ancak 97 ms sonra yakalıyor; sesin kendisi olay karesinde başlıyor.
- **±30 ms:** oyunun günlük satırlarının tarayıcıya ulaşma gecikmesi.
- **PlayMode testleri** (yeni iki test):
  - sekme, menüden dalış ve kazanma → koridor → dalış sırasında her sesin sırasıyla tetiklendiğini doğruluyor;
  - koridor sesinin en hızlı anda tam, oturuşta sönmüş olduğunu doğruluyor.

## Doğrulanmayan

- Seslerin kulağa nasıl geldiği.
- Gerçek iPhone hoparlöründe denge.
- Makas, değnek ve çekiç seslerinin oyun içi kaydı (koddan, animasyonun karesinde).
