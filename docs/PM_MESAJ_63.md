# PM_MESAJ_63 — Takılma (jam) davranışı: zincirli parça yerinden çıkmasın (p21)

Kaynak: `docs/art/reff/ScreenRecording_11-11-2026 11-25-24_1.MP4` (Zeyd, 18 sn, test modunda bölüm). PM kare kare inceledi.

## Sorun
- Videonun başındaki (0–3 sn) normal parçaların yanlış hamlesi **iyi**: parça yerinde kısa bir tık/sarsıntı yapıyor. Zeyd: "Yanlış hamle çok kıpırdamasın, ilk yaptığım gibi kalsın."
- Ama **zincirli parça** (turkuaz zincirli, ~9,6 sn ve ~10,9 sn): hareket edemediği hâlde **yuvasından çıkıp** yarım hücre kadar gidiyor (arkasında boş, karanlık delik açılıyor) ve sonra **geri geliyor**. Zeyd: "Hareket edemiyorsa orada takılsın."

## Yapılacak
1. **Zincirli parçanın takılması normal parçayla aynı olsun:** hamle geçersizse (kendisi ya da zinciriyle bağlı eşi engelleniyorsa) parça **yuvasından ayrılmaz**; yalnız normal parçalardaki kısa takılma sarsıntısı (aynı genlik, aynı süre) + zincirde küçük bir fizik dürtüsü (impuls). Boş delik görünmemeli.
   - Hamle **geçerliyse** ikisi birlikte kayar (değişmez).
   - Sebebi bul: geçersiz hamle önce iki parçayı da hareket ettirip sonra mı geri alıyor? Geçerlilik **hareketten önce** kontrol edilsin.
2. **Takılma sarsıntısı genliği:** genel olarak biraz azalsın (şu anki değerin ~%70'i); 0,15–0,2 sn, en çok 2 salınım. Fiskede de aynı.
3. Zincir fiziği: takılmada yalnız hafif bir sallanma; parça yerinde.
4. Test: EditMode'a "zincirli geçersiz hamle → parça pozisyonu değişmez" testi. Vitrin + bölüm 12'de GIF: zincirli parça takılması (önce/sonra) → `docs/art/task29/pm63/`.

## Teslim
p21 güncel, EditMode yeşil (Perf hariç), `verify_build` 4/4, rapor `ENTEGRASYON_RAPOR.md`'ye "PM_63" eki, PROJE_DURUMU'na tek satır, commit + push (önce `docs/` commit). `codex exec` girdisi kapalı (`< /dev/null`).
