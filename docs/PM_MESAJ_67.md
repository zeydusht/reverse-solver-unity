# PM_MESAJ_67 — TASK48 bağla + değnek (b) + bomba dersi 7 (p22)

Zeyd kararları (14 Kasım 11:05): değnek **b**, bomba dersi **7**. Codex TASK48 PM ve Zeyd tarafından onaylı. Kodu kendin yaz. Yalnız p22. Önce `docs/` commit.

## 1. TASK48 görsellerini bağla (`Assets/Art/_yeni/T48/`, `docs/art/task48/LISTE.md`)
- Ana menü: üst/alt şerit old money mavi, sekme başlıkları kurdelesiz 3B yazı (Bölümler, Koleksiyon, Jiggles, Başarılar).
- Ayarlar ve Profil: krem gövde + mavi çerçeve, yeni anahtarlar ve düğmeler, kırmızı X. Sürüm etiketi doğru (`p22-…`).
- **Kazanma ekranı:** yeni krem kart, 3B "Harika!", büyük mutlu Jigo, bölmeli sandık ilerlemesi, "N Jiggle kurtarıldı!", katmanlı yeşil **Sonraki Seviye** + mavi **Menü**.
- Eski turkuaz parçaların hiçbiri p22'de kalmasın (TR + EN ekran görüntüsüyle kontrol).

## 2. Değnek: seçenek b
Önce çeyrek seçimi (mevcut). **Hiçbir çeyrek ≥1 parça açamıyorsa** seçim modunda tek seçenek: "**Bütün tahtayı değiştir!**" (onaylı yeşil onay düğmesi). Bütün tahtada en az 1 parçayı serbest bırakan, çözülebilir düzeni seçer. Bu da mümkün değilse mevcut uyarı, booster harcanmaz. Ölç ve raporla: kullanılabilirlik %24'ten kaça çıktı, ortalama açılan parça.

## 3. Bomba dersi 7. bölüme
- `P21Onboarding.BombLesson` = 7 (süresiz, tek bomba, cömert sayaç, kayıp yok).
- 8 ve 9: **yumuşak giriş**: en çok 1 bomba, sayaç cömert (en az parça sayısı + 4). Hedef ilk denemede kazanma ≥%85 (pt1'de 8: %63, 9: %38 duvarı vardı).
- 10'dan sonra kaldırılan bombalar **geri gelir** (PM_66 listesindeki 11, 12, 13, 16, 17, 18, 19). 20'deki eski bomba dersi normal bölüm olur (S24 özgün hâli).
- Diğer dersler aynı: kilit 3, zincir 10, duvar 14.
- FunSim ile 1–20 ilk deneme kazanma tablosu, ders ve yumuşak bölümler ≥%85.

## Teslim
p22 güncel, EditMode yeşil, `verify_build` p22 4/4. Ekran görüntüleri: menü, Ayarlar, Profil, kazanma (TR/EN), değnek "Bütün tahtayı değiştir", 7. bölüm bomba kartı. Rapor "PM_67", PROJE_DURUMU'na tek satır, commit + push.
