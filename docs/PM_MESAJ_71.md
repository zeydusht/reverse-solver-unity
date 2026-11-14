# PM_MESAJ_71: Tahmini gerçek veriyle düzelt + p22'de 8 bölüme ince ayar

Kodu kendin yaz. Yalnız p22. Önce `docs/` commit. Supabase'e yazma yok.

## Neden
PM_70 iyi; kurallar ve eğri tuttu. Ama bu 40 bölümün hepsini pt1'de insanlar **gerçekten oynadı**. Model v2'nin hatası ±11 puan, bazı bölümlerde ±25–30. Elimizde bölümün kendi gerçek sonucu varken yalnız modele güvenmek gereksiz.

PM hesabı (`docs/analiz/v2/irt_level_d.csv` + PM_70 tablosu):

`veriyle = gerçek pt1 zorluğu + (yeni sürümün model zorluğu − pt1 sürümünün model zorluğu)`

`birleşik = logit karışımı: (1−w)·model + w·veriyle`, `w = n/(n+10)`, n = pt1'de bölümü oynayan kişi.

Birleşik tahmine göre hedef dışı kalan bölümler:

| slot | bölüm | rol | model | veriyle | birleşik | hedef | durum |
|---|---|---|---|---|---|---|---|
| 11 | S21 | normal | ,67 | ,45 | ,52 | ,55–,75 | biraz zor |
| 26 | S17 | normal | ,66 | ,97 | ,87 | ,50–,70 | çok kolay |
| 27 | S10 | normal | ,60 | ,97 | ,85 | ,50–,70 | çok kolay |
| 32 | S39 | normal | ,56 | ,77 | ,66 | ,45–,65 | biraz kolay |
| 33 | S32 | **zor** | ,40 | ,58 | ,48 | ,30–,45 | zor değil |
| 36 | Z25 | normal | ,55 | ,81 | ,67 | ,45–,65 | kolay |
| 37 | S38 | normal | ,56 | ,19 | **,39** | ,45–,65 | gizli zor: 31–40'ta 4. zor olur |
| 30 | S08 | normal | ,65 | ,35 | ,52 | ,45–,65 | sınırda (S47/S08 tipi: açık tahta, dikkat dağıtıcı) |

## 1. Veriyle düzeltmeyi koda ekle
`DifficultyModelV2`'ye `PWin1Blended(level)` ekle:
- `irt_level_d.csv`'yi `Resources`'a ya da `docs/analiz/v2`'den okunan bir tabloya koy.
- Aynı `level_id` için pt1 sürümünün model zorluğunu hesapla, yukarıdaki formülü uygula.
- Verisi olmayan bölümde `PWin1` ile aynı sonucu versin.
- Üretici, eğri kontrolü ve raporlar bundan sonra `PWin1Blended`'ı kullansın. Raporda `model` ve `veriyle` yan sütun olarak kalsın.
- Test: S38 v2 için birleşik ≈ ,39 (±0,02); verisi olmayan bir bölümde `PWin1Blended == PWin1`.
- Sonraki test grupları için genel olsun: ileride p22 verisi geldiğinde aynı tablo p22 sonuçlarıyla güncellenecek. Kohort adı parametre.

## 2. İnce ayar (birleşik tahmine göre)
PM_70'teki sıra geçerli: fitil → süre → bombalı parçayı taşı → engel. Tahtaya dokunma, sürüm +1.
- **11 S21:** ,60 civarına çıkar (fitil +2 ya da süre).
- **26 S17, 27 S10:** ,60–,65'e indir. Fitili kıs ya da kilit ekle. Bombayı daraltılmış bölgeye taşıyabilirsin. Kurallar bozulmasın.
- **32 S39:** ,58 civarına indir.
- **33 S32 (zor):** ,40'a indir.
- **36 Z25:** ,58 civarına indir.
- **37 S38:** ,55'e çıkar. pt1'de en çok ölüm 8, 29 ve 30 numaralı bombalardaydı; v2'de 30:8 değişmedi. Önce onu gevşet.
- **30 S08:** ,58 civarına çıkar. Bu açık tahta tipinde model iyimser: fitili +3 aç ya da bombayı tahtanın görünür kenarına yakın bir parçaya taşı.
- Diğer bölümler kalsın. Değiştirdiğin bölümün komşularında eğri ve kurallar bozulmasın: art arda zor yok, 31–40'ta en çok 3 zor.

## Teslim
- p22 güncel, EditMode yeşil, `verify_build` p22 4/4.
- Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_71": slot · bölüm · rol · model · veriyle · birleşik (önce → sonra) · değişiklik.
- `PROJE_DURUMU`'na tek satır, commit + push. Sonra **dur**.
