# PM_MESAJ_55 — Vitrin geri bildirimi düzeltmeleri (PM_53 sonrası, p21)

Zeyd vitrini (`p21/?lv=SHOW_ART`) telefonda oynadı. Genel olarak iyi, şunlar düzelecek. Yalnız p21.

**Çalışma şekli:** ağır işleri (kod, betik) `codex exec` ile Codex CLI'ya ver. Sen böl, kısa kontrol et, test et, yayınla. Uzun log okuma.

## 1. Bomba nabzı her sayıda çalışsın
- Şu an 7 nabız atıyor, 3 atmıyor (acil hâlde yalnız hamle anında "pump" var). Acil hâlde de **sürekli** nabız olsun: ~%8 ölçek, ~0,6 sn döngü + hamle anında ekstra pump + titreme. ≥4'te mevcut %3 / 1,2 sn nefes kalsın.
- Fitil ateşi şimdilik kod tarafında **1,8 kat** büyüsün ve arkasına sıcak ışık halesi (additive, sarı→turuncu) eklensin. Codex yeni kıvılcım karelerini (TASK30 §6) verince onlara geçilir.

## 2. Zincir sarkması: kısa zincirler daha bol
- Uzunlar iyi. Kısalar (komşu parça, 1–2 hücre) fazla gergin, mesafe kısa görünüyor.
- Yeni kural: sarkma = **max(0,22 hücre, boyun %10'u)**, üst sınır **0,45 hücre**. Kısalarda halka sayısı da artsın (zincir biraz bol görünsün). Başka parçanın üstüne binme kuralı aynı.
- Yaylanma aynı kalsın (Zeyd beğendi).

## 3. Zincirli parçalar diğerlerinden yüksek duruyor
Zeyd: "zincirli parçalar diğer parçalardan daha yukarıda gibi". Bul: zincirli parça bir gölge/ölçek/sorting farkıyla mı öne çıkıyor? Parça diğerleriyle **aynı düzlemde** olmalı; yalnız zincir ve bağlantı noktaları üstte. Önce/sonra ekran görüntüsü al.

## 4. Zincir görünümü: eskiye dön
Yeni ince zincir seçilmiyor. Codex TASK30 §5'te eski zincirden halka sprite'ı çıkaracak. O gelene kadar: halka sprite'ını **1,6 kat** büyüt (kalınlık), boşlukları buna göre ayarla.

## 5. Kilit
Görünümü Codex TASK30 §4'te yeniden çiziyor, Zeyd seçecek. Şimdilik dokunma.

## Teslim
p21 güncel, EditMode yeşil, `verify_build` 4/4. Vitrin linki + kısa rapor (`docs/art/task28/ENTEGRASYON_RAPOR.md`'ye "PM_55" eki). PROJE_DURUMU'na tek satır.
