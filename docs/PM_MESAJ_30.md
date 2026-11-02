# PM → Claude Code (2 Kasım 2026, 18:00) — Booster eğitimi + çoklu spotlight (p20)

PM_29'a sabah eklenen bölüm dosyaya kaydedilmemişti. Bu mesaj onun tamamı ve **kararları önceden veriyor**. Yine belirsiz kalan bir nokta olursa en basit seçeneği uygula, OVERNIGHT'a "PM kararı bekliyor" diye not düş, durma.

Yayın `p20/`. p19, kök, `redesign.json`, bölümler ve iOS dokunulmaz. Veri kuralları (PM_25/29) geçerli.

## 1. Booster'ların yönlendirilmiş ilk kullanımı

**Neden:** Tanıtım kartı "Makası kullan" diyor, ama el yok, kimse booster'ı denemiyor.

**Nerede:** Booster'ın açıldığı ilk bölüm (5 makas, 9 değnek, 15 saat, 23 çekiç). Tanıtım kartı kapanınca, **süre başlamadan ve ilk hamleden önce**.

Akış:
1. **Tuş adımı:** Ekran ~%60 kararır, yalnız alttaki ilgili booster tuşu aydınlık kalır. El tuşa dokunur (döngü). Altta A3 stilinde yazı, TR/EN:

| Booster | TR | EN |
|---|---|---|
| makas | "Makası seç" | "Tap the scissors" |
| değnek | "Değneği seç" | "Tap the wand" |
| saat | "Saate dokun" | "Tap the clock" |
| çekiç | "Çekici seç" | "Tap the hammer" |

   Yalnız o tuş tepki verir.

2. **Hedef adımı** (makas, değnek, çekiç): Ekran kararır, yalnız **bir hedef parça** aydınlık kalır. El ona dokunur.
   - Yazı booster'ın **koddaki gerçek etkisine** göre olsun. Örneğin makas zinciri kesiyorsa: TR "Zinciri kesmek için parçaya dokun" / EN "Tap a piece to cut its chain". Metni koda göre sen yaz.
   - **Hedef seçimi:**
     - Booster'ın etkisinin gerçekten işe yaradığı parça olsun: makas için zincirli parça, çekiç için çivili ya da kilitli parça vb.
     - Bölümde birden çok aday varsa, kayıtlı çözümde en erken kullanılan adayı seç.
     - Uygun aday yoksa SmartPlayer'ın en iyi gördüğü parçayı seç.
     - Hiç aday yoksa hedef adımını atla, yalnız tuş adımı yapılır.
   - Yalnız o parça tepki verir.

3. **Saat:** Tuşa dokunmak yeterli. Süre artar, sayaç üzerinde kısa bir "+X sn" efekti çıkar.

4. **Etki gerçek uygulanır.** Tahta değişir, sonra örtü kalkar ve normal oyun başlar. Süre ilk normal hamlede başlar (mevcut kural).

5. **Ücretsiz:** Bu eğitim kullanımı oyuncunun hakkından düşmez.

6. **Veri:**
   - Eğitim kullanımı `moves`'a normal booster satırı olarak gider.
   - `plays`'teki `b_scissors / b_wand / b_hammer / b_clock` sayaçlarına **eklenmez**. Böylece "booster'sız ilk deneme" ölçümü bozulmaz.
   - Bu dört bölümün (5, 9, 15, 23) herkeste aynı yardımı aldığını OVERNIGHT'a not düş. PM, veri raporunda dikkate alacak.

7. Aynı oyuncu bölümü tekrar oynarsa (2. deneme, Koleksiyon'dan tekrar) eğitim **tekrar gösterilmez**.

## 2. Booster hakları: `BoosterStart = 3`

- **Model: kalıcı envanter**, Royal Match gibi. Her booster için oyuncunun bir stoku var.
  - Booster açıldığında (eğitim kullanımından sonra) stok **3** olur.
  - Her kullanımda 1 azalır, bölümler arası taşınır, kendiliğinden dolmaz.
  - Tuşun üstündeki kırmızı rozet stoğu gösterir. Stok 0 ise tuş gri olur ve rozet "0" gösterir.
- Şu anki "her bölümde 1" kuralının yerine geçer. `redesign.json`'daki bölüm başına `boosters` alanı **değiştirilmez**, envanter kodu onun yerine geçer (çalışma zamanı).
- Sayı tek sabitte dursun (`BoosterStart = 3`). PM ileride 1 ya da 2 yapabilmeli.
- Envanter yerel kayıtta tutulur, profil ve ilerleme gibi. Supabase'e yazmak gerekmez, kullanım zaten `moves`'ta görünüyor.
- Ekonomi (satın alma, ödül) **yok**.

## 3. Çoklu spotlight

Bölüm 10 zincir spotlight'ı şu an tek parçayı aydınlatıyor. Bunu düzelt:
- **İki zincirli parça + aralarındaki zincir** birlikte aydınlık olsun, ikisinin çıkış yolu da görünsün.
- Diğer spotlight'lar da birden çok bölge alabilmeli:
  - **Çivi:** çivili parça + sayacı.
  - **Duvar:** duvar şeridinin tamamı.
  - **Bomba:** bomba + fitil sayacı.
- Teknik: örtü maskesi birden çok dikdörtgen ve dairenin birleşimi olsun.

## Kontrol
- `?c=ic`, temiz kayıt, bölüm 5'e kadar oyna (`?won=4` debug kısayolu varsa onu kullan).
  - Makas eğitimi: tuş adımı → hedef → etki.
  - Stok 3 görünüyor mu? Bir kullanım sonra 2 mi?
- Bölüm 9, 15, 23 için aynısı (debug kısayolu ile).
- Bölüm 10 zincir spotlight'ı: iki parça + zincir aydınlık mı?
- Supabase: eğitim kullanımı `moves`'ta görünüyor, `plays.b_*` 0 mı?
- Ekran görüntüleri: `docs/qa/2026-11-02-booster/`.
- OVERNIGHT.md başına: link + sonuç + belirsiz kalan noktalar.
