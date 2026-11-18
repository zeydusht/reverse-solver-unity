# PM_MESAJ_83: Zeyd'in sl100 turu: rehber, sandık ekonomisi, altın, Oyna düğmesi, tablolar 41–100

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (18 Kasım 08:37): "Oyun güzel, kazanma ekranı iyi." Bozuk olanlar aşağıda.

## 1. Rehber (1. bölüm): yanlış el geldi
PM_82'de istenen p20 eldivenli eli **yanlış eldi** (PM hatası). Zeyd'in onayladığı başka bir el var: ekranı karartıyordu, yalnız oynanacak parça açık kalıyordu ve aşağı doğru kayma oku/izi çıkıyordu.
- **Git geçmişinde bütün rehber eli sürümlerini bul.** En az şunlar:
  - Codex TASK41 tutorial eli (10 Kasım, Zeyd onayladı);
  - PM_59 / PM_60 rehberi;
  - `codex/01-onboarding` (altın noktalar);
  - p20 eldivenli el.

  Her birini bayrakla aç: `&el=a`, `&el=b`, `&el=c`, `&el=d`. Hangi harfin hangisi olduğunu ve tarihini raporla. Her birinin 390×844 kısa GIF'ini `docs/art/pm83/el/` klasörüne koy. Zeyd seçecek. Varsayılan: karartma + ok/iz olan sürüm; bulamazsan TASK41 eli.
- **Karartma (spot ışık):** rehberli adımlarda (2–5) ekran ~%60 kararır. Yalnız oynanacak parça ve gideceği yol açık kalır. Yol üstünde gidiş yönüne doğru kayan ok/iz (aşağı). Serbest oyuna geçince (adım 6) karartma tamamen kalkar. Diğer bölümlerde karartma asla yok.
- **Jigo'nun konuşma kutusu üste taşınsın:** üst şeridin hemen altında. Zeyd alttan kaydırırken kutu elini ve parmağını kapatıyordu. Jigo portresi kutunun solunda kalır. Kutu tahtayla üst üste binmesin; gerekirse tahta biraz aşağı insin, kenar payı kuralı korunur.
- Metinler ve 7 adım PM_82'deki gibi kalır.

## 2. Sandık: doğru ekonomi
Şu anki sorunlar:
- açılmamış booster veriyor (3. bölümde değnek gibi);
- "Topla" oyuncuyu sonraki bölüme atıyor.

**Yeni kural (PM kararı):**

| Sandık | Ne zaman | İçerik |
|---|---|---|
| Normal sandık | her 5 bölümde (5, 10, 15 …); kazanma ekranındaki 5 bölmeli çubuk dolunca | 1–20: 100 altın · 21–50: 150 · 51–100: 200. 10. bölümden itibaren + 1 booster |
| Kont sandığı | çok zor bölüm kazanılınca (19, 29 … 99); koyu kırmızı, Kont mühürlü | normal sandığın 2 katı altın + 2 booster |
| Zor bölüm | 14, 24 … 94 kazanılınca | sandık yok, kazanma ekranında +100 ek altın ("Zor bölüm bonusu") |

- **Booster yalnız açılmış olanlardan** (makas 6, çekiç 13, saat 18, değnek 25). En az elde olan booster'a ağırlık ver.
- 5. bölüm sandığı yalnız altın.
- Bölüm başı kazanç +50 altın aynı. Başlangıç altını 5000 ve devam fiyatları aynı.
- Sayılar `live/config.json`'da olsun, build'siz değişsin.

**Akış:**
1. Sandık **kazanma ekranının içinde** açılır.
2. Sandığa dokun (3 dokunuşlu açılış kalır). Ödüller **kendiliğinden** sayaçlarına uçar: altın üst sayaca, booster küçük ikonla.
3. "Topla" düğmesi **kalkar**.
4. Bitince kazanma ekranı aynen durur ("Sonraki Seviye" / "Menü"). Oyuncu kendisi geçer.
5. Ekonomi testi: 1→100 sanal oyuncuyla toplam altın ve booster akışı. Rapora tablo: bölüm 10, 25, 50, 100'de ortalama altın ve booster.

## 3. Altın görünümü: gerçek oyun parası
Şimdiki altın ikonu eski ve sönük. Bilinen oyunlardaki gibi parlak, kalın kenarlı, yuvarlak altın para:
- ortada kabartma (yıldız ya da Jigo yüzü);
- üstte parlama;
- sayaçta yanında "+" düğmesi yok.

Codex 3 seçenek çizer, puanla, en iyisini bağla. Her yerde aynı ikon: üst sayaç, kazanma ekranı, sandık, devam paneli, hediye, mağaza. Ödül uçuşunda 5–8 para tek tek uçsun, sayaç tıklayarak artsın.

## 4. "Oyna · Bölüm N" düğmesi
PM_82'de yatayda büyütülmüş. **Harita kalkmadan önceki boyutuna** dön, ekranın ortasında dursun. Zor ve çok zor renkleri ve Kont mührü kalsın.

## 5. Tablolar 41–100: hikâye ve Jiggle eksik
1–40'ta her tablonun adı var ("Son şatonun ışığı" gibi) ve hikâyeye bağlı. 41–100'de ad yok ve tablolarda Jiggle yok. Bu bilerek yapılmadı; TASK51 tarifinde eksik kalmış.
- **Adlar:** 41–100 için TR + EN tablo adları. Kont'un galerisinin hikâyesine bağlı (`docs/HIKAYE.md`). Her 10 tablo bir oda/tema olsun (ör. 41–50 kış salonu, 51–60 deniz kanadı …). 1–40 ile aynı ses ve uzunlukta. Listeyi `docs/art/pm83/TABLO_ADLARI.md`'ye yaz.
- **Jiggle'lar:** her tabloda 1–40'taki gibi farklı renkte Jiggle'lar olsun. Önce **mevcut tabloya Jiggle ekleme (düzenleme)** dene; tutmazsa yeniden çiz. Stil 1–40 ile aynı.
- Önce 41–60, sonra 61–100. 91–100 eksikleri de aynı kuralla.
- Kalite: 1–40'tan 3 tabloyla yan yana karşılaştırma görseli `docs/art/pm83/tablo_karsilastirma.png`.

## Achievements (Zeyd önerisi)
PM kararı: **yayından sonra (v1.1).** Özellik dondurma 19 Kasım. Şimdilik Profil'deki kurtarılan Jiggle sayısı yeter. Kod yazma.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&reset=1&lang=tr` (rehber, varsayılan el)
  - `p23/?c=p23&reset=1&lang=tr&el=a`, `&el=b`, `&el=c`, `&el=d`
  - `p23/?c=p23&test=1&lang=tr&lv=19` (Kont sandığı)
  - `p23/?c=p23&test=1&lang=tr&lv=10` (normal sandık + booster)
