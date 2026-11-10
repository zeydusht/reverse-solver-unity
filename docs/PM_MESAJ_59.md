# PM_MESAJ_59 — Yeni onboarding: 2×2 tutorial + açıklamalar (p21)

Kaynak: `docs/PT1_GERIBILDIRIM.md` (A ve H). pt1'de oyuncular mekaniği anlamadan deneme-yanılma yaptı (3–4. bölümde ortalama 10–17 takılma), bölümün nasıl bittiğini geç anladı, bomba sayısını ve kilidi anlamadı. Yalnız p21. Ağır işleri `codex exec` ile Codex CLI'ya ver.
Görseller: `Assets/Art/_yeni/T41/Tutorial/` (el, ok, takılma ve doğru işaretleri, Jigo konuşma balonu, 2×2 çayır resmi) ve kilit uyarısı.

## 0. PM_58'den kalan kararlar
- `FunSimV5Tests.HardestLevelStillUnderTwoSeconds`: makine hızına bağlı. `[Category("Perf")]` yap, yayın kapısından çıkar, ayrı koşulsun. Sınır aynı.
- Fiske/sürükleme telemetrisi: PM `moves` tablosuna boş bırakılabilir `input text` sütununu ekleyecek (Zeyd onayından sonra). Sütun hazır olana kadar istemci göndermesin; bayrakla açılsın (`INPUT_COL`), PM haber verince açılır.

## 1. Bölüm 1 = 2×2 rehberli tutorial (4 parça, kaybetmek YOK)
Parçalar: A (sol üst), B (sağ üst), C (sol alt), D (sağ alt). Süre sayacı durur ya da görünmez; kayıp yok. Diğer parçalara dokunulursa nazikçe titrer, hamle sayılmaz.
1. **Doğru hamle:** C parlar, el + kesik ok aşağıyı gösterir. Balon (Jigo): "Parçayı aşağı kaydır!" → C çıkar, yeşil tik + "Harika!".
2. **Takılma + düzeltme:** B parlar, ok **sola** (içeri). Balon: "Şimdi bunu sola it." → B, A'ya çarpıp takılır (takılma işareti + titreme). Balon: "Önü kapalı! Parçalar başka parçanın içinden geçemez. Boş tarafa, dışarı doğru it." → ok sağa, B sağdan çıkar.
3. **Kendin dene:** ok yok, yalnız A hafif parlar. Balon: "Hangi yöne itersen çıkar? Dene!" Yanlış yöne iterse takılma + kısa ipucu, doğruysa tik.
4. **Son parça:** D. Balon: "Bütün parçaları çıkarınca resim kurtulur!" → D çıkar → kutlama.
- Gerçek kural motoruyla doğrula (takılma anlamı, çıkış yönleri); senaryo kurala uymuyorsa en yakın geçerli dizilimi kur ve raporla.
- Her adım için telemetri olayı: `tut_step` (adım no, sonuç, süre, yanlış deneme sayısı). Tablo yoksa `docs/sql/017_tutorial_events.sql` yaz (row_id'ye sütun SELECT dahil), **uygulama**; PM uygular.

## 2. Bölüm 2: kolay, yalnız kaydırma
3×3 civarı, engelsiz, cömert süre, tek doğru sıra yok (çok yol). İlk denemede kazanma hedefi ≥%95. Bölüm başında küçük balon: "Önce önü açık parçaları çıkar."

## 3. Bölümün nasıl bittiği
İlk 3 bölümde başlangıçta 1,5 sn'lik üst bant: "**Bütün parçaları tahtadan çıkar!**". Parça sayacı her çıkışta küçük "pop".

## 4. Engel tanıtım kartları: net cümle
- **Bomba:** "Bombalı parçayı sayı **bitmeden** çıkar. Her hamlede sayı 1 azalır; 0 olursa patlar!" Kartta küçük animasyon: sayı 3→2→1.
- **Kilit:** "Kilitli parça **N hamle** boyunca hareket etmez. Başka parçaları oyna; sayı 0 olunca kilit açılır."
- **Zincir:** "Zincirli iki parça **birlikte** hareket eder."
- Kart metinleri TR/EN, kısa, büyük punto.

## 5. Kilitli parçaya hamle denemesi
Kilitli parça kaydırılmaya çalışılırsa: kilit sallanır (T41 kareleri), sayaç etrafında mercan halka parlar, hamle sayılmaz, küçük balon (ilk 2 seferde): "Kilit N hamle sonra açılacak."

## Teslim
p21 güncel; yeni oyuncu olarak baştan oynanabilsin (`&reset=1` yoksa ekle: ilerlemeyi ve tutorial'ı sıfırlar). EditMode yeşil (Perf hariç), `verify_build` 4/4. Kısa GIF: tutorial 4 adım, kilit uyarısı, bomba kartı. Rapor `docs/art/task29/ENTEGRASYON_RAPOR.md`'ye "PM_59" eki, PROJE_DURUMU'na tek satır, commit + push.
