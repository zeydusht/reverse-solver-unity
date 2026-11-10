# pt1 geri bildirimi + veri → ürün kararları (10 Kasım 2026)

Kaynak: Supabase `v_attempts` / `v_funnel` (pt1, 51 oyuncu) + Zeyd'in arkadaşlarından doğrudan duyduğu yorumlar.

## Veri
- 51 oyuncu; 7 kişi 40 bölümü bitirdi (%14); medyan oyuncu 9. bölümde.
- 10. bölümden önce bırakan 30 kişiden **21'i son bölümünü KAZANIP** çıktı; kaybedip çıkan yalnızca 6. → asıl sorun "bir tane daha" çekiminin olmaması. İlk oturumlar 1–2 dk.
- Anlamadan oynama: ortalama takılma (jam) 3. bölümde 10, 4. bölümde 17; bırakanlarda 12–25.
- 8–9. bölümde duvar: ilk denemede kazanma %63 / %38 (bomba girişi).
- Fırat (Zeyd'in babası): 24. bölümde, 57 dk; ileri yaş grubunda zorluk çok daha yüksek.

## Oyuncu yorumları
1. **Kenar parçalar:** ekranın sağ/sol kenarına çok yakın; telefon kılıfına çarpıyor, tarayıcıda daha da zor. **BÜYÜK SORUN.**
2. **Sürükleme mesafesi:** parçayı çıkarmak için fazla kaydırmak gerekiyor. Küçük bir fiske (swipe) yetsin, parça o yöne aksın.
3. **Boosterlar anlaşılmıyor** (özellikle sihirli değnek). Her biri ne yaptığını görsel olarak göstermeli.
4. **Haz/efekt eksik:** "bir şey eksik", "durağan". Patlama, çatlama, ödül hissi, "aferin" anı yok.
5. **Mekanik geç anlaşılıyor:** "3–4 bölüm çözdüm ama mekaniği anlamadım"; bazıları bölümün nasıl bittiğini 5. bölümde anladı.
6. **Bomba sayısı anlaşılmadı** ("33'ün anlamını çözemedim").
7. **Çivi/kilit anlaşılmadı:** 11. bölümde biri kilitli parçayı defalarca kaydırmaya çalıştı ("bende mi sorun var" videosu). Yeni asma kilit görseli yardımcı olabilir, ama açıklama da gerekli.
8. **Tıkanma var:** 11. bölüm ve ileri yaşta 24. bölüm. Oyun "çok kolay" değil; kitleye göre değişiyor.
9. **Olumlu:** arayüz ve çizimler beğenildi; "Reels kaydırır gibi", "metroda oynanacak oyun"; seviye ilerledikçe zorluğun artması hissediliyor.

## Kararlar (Zeyd + PM)
- **A. Onboarding / tutorial yeniden:**
  - 1. bölüm 2×2, 4 parça. Parça 1: doğru parça + sürükleme yolu gösterilir. Parça 2: takılma gösterilir, sonra doğru yöne (aşağı) çıkarttırılır. Parça 3: bilerek hata yaptırılır (sağ üstteki sola, içeri itilir, tıkanır), sonra sağa, dışarı çıkarttırılır.
  - 2. bölüm kolay, yalnız kaydırma.
  - Bölümün nasıl bittiği açıkça söylenir ("Bütün parçaları çıkar!").
  - Her adım için telemetri.
- **B. "Bir tane daha" çekimi:** kazanınca büyük "Sonraki" düğmesi ve hızlı geçiş; kurtarılan Jiggle'lar Jigo'nun sürüsüne katılır; ilk 10 bölümde küçük hikâye anları ve merak kancası; 3, 5, 10. bölümde küçük ödüller.
- **C. Engel tanıtımlarını yay:** 3–6–9 gibi sık aralık yerine normal oyunlardaki gibi seyrek (ör. 3 → 10–15 → 18–20 …). Zeyd Royal Match'in tanıtım aralıklarına bakacak; PM de araştıracak. İlk 10 bölümde ilk denemede kazanma ≥%85; bir engelin tanıtıldığı bölümde kaybetmek yok.
- **D. Kenar boşluğu:** tahta yanlardan içeri alınır (telefon kılıfı + tarayıcı kenar hareketleri için güvenli kenar payı). Kenar parçaları rahat tutulur. Gerekirse hücre küçülür.
- **E. Fiske ile çıkarma:** kısa bir kaydırma yeterli (hız + yön algılama); parça o yönde akar ve engel yoksa çıkar. Yanlışlıkla tetiklenmemesi için eşik.
- **F. Booster'lar kendini göstersin:**
  - **Makas:** ekrana gelir, kesilebilecek yerler parlar, "şunu kes"; makas gidip gerçekten keser ve orası açılır.
  - **Sihirli değnek:** ekran hafif kararır, onay ("kullanmak istiyor musun?"). Tahta 4 bölgeye ayrılır (sağ üst, sol üst, sol alt, sağ alt); oyuncu seçer, değnek o bölgenin ortasına dokunur, yalnız orası değişir. (Bütün tahtayı değiştirmek yerine. Kural değişikliği, dengesi kontrol edilmeli.)
  - **Çekiç:** parça seçilir; çekiç yukarıdan "donk" diye vurur, parça seker, aşağı düşer ve yok olur.
  - **Saat:** basınca Jigo (üst penceredeki) kolunu uzatıp saati tutar, süre durur; üstte kaç saniye tutacağını gösteren bir bar boşalır; bar bitince Jigo saati bırakır.
- **G. Haz ve efektler (juice):** parça çıkınca patlama/çatlama parçacıkları, ekran sarsıntısı, ses, kombo hissi; kazanınca büyük kutlama; "aferin" anları. Her etkileşime geri bildirim.
- **H. Bomba ve kilit açıklaması:** tanıtım kartında net cümle ("Bombayı 5 hamle içinde çıkar, yoksa patlar"; "Kilit N hamle sonra açılır, o zamana kadar başka parçaları oyna"). Kilitli parça kaydırılmaya çalışılınca kilit sallanır ve kalan hamle sayısı vurgulanır.
- **I. Ölçüm:** yeni onboarding'i ayrı kohortla (`pt2`) aynı arkadaş grubuna ve yeni kişilere oynat; ilk 10 bölüm tutunma, takılma sayısı ve tutorial adımları karşılaştırılır.

## Sıra (öneri)
1. D + E (kenar ve fiske): en büyük sürtünme, hızlı kazanç.
2. A + H (tutorial ve açıklamalar).
3. C (bölüm sırası ve tanıtımları yaymak, SET40 yeniden dizilim).
4. B + G (çekim ve efektler).
5. F (booster gösterimleri; Codex görselleri gerekir).
6. pt2 testi.

## Sonraya: sıralama (leaderboard), Zeyd onayı 10 Kasım
Bir arkadaş "kaçıncı oldum, en hızlı kim?" diye sordu. LinkedIn oyunlarındaki gibi.
1. Kazanma ekranında yüzdelik dilim: "18 sn, oyuncuların %75'inden hızlısın, rekor 11 sn". Mevcut veriyle yapılır. Onboarding'den hemen sonra.
2. Günün bulmacası + günlük sıralama (geri dönüş kancası). TestFlight'tan sonra.
3. Arkadaş sıralaması: davet linki ya da arkadaş kodu. En son.
Kural: booster ve devam kullanılan oyunlar sıralamaya girmez; hakaret içeren isimler gösterilmez.
