# CODEX_TASK26 — storyboard v03 düzeltmeleri (PM, 8 Kasım)

Genel: stil, ışık, koridor ve Jiggles ailesi **onaylı stile uyuyor**. 9 karelik akış (kostüm ilk kurtarıştan sonra, 1. bölüm = aynı tablo) iyi. Zeyd'in son kararı gelene kadar bu akış korunur. Aşağıdakileri düzelt, sonra yine dur.

1. **Karakter doğru (Zeyd onayladı).** Kare 02/05'teki Kont, seçtiğimiz karakter (`Baron-reference-v01.png`), aynen kalsın.
2. **İsim: Kont (Zeyd, 8 Kasım).** Kötü adamın adı **Kont**. Storyboard'daki "Kont" doğru, aynen kalsın. Eski belgelerdeki "Baron" çalışma adıydı.
3. **Yeşil Jiggle'ın kolu (kare 01, Zeyd):** kol çıkıntıdan çıkıyor gibi görünüyor. Kural: kollar gövdeden ayrı çıkar, çıkıntıyla birleşmez (STYLE_CARD, uzuv ayrımı). Mor Jiggle tamam.
3b. **Kıyafet sahnesi (Zeyd):** tablo inince kıyafet parçaları (fırça, yüksük, kalkan) **çerçeveden yere düşer**, kahraman onları yerde bulup giyer. Kare 08–09 buna göre.
4. **Kahraman kartı:** "Fırça şövalyesi" yönü onaylanmaya aday. 64 px ve 96 px okunurluğu iyi. 2. bir seçenek ekle: aynı fırça ve yüksük, ama kalkan yapboz parçası yerine düğme. Zeyd karşılaştırsın.
5. **Son yazı:** "Save the Jiggles" ve "Free them all!" seçeneklerinin ikisini de göster, Zeyd seçsin. Türkçesi: "Jiggles'ı kurtar!" / "Hepsini kurtar!"

## 6. Adım 2 değişti: animasyon = AI video (Royal Match ara sahneleri gibi)
Zeyd sprite/tween değil, **Royal Match'teki gibi sinematik video** istiyor. Üretim: sen **anahtar kareleri ve çekim listesini** hazırlarsın, Zeyd videoları **Higgsfield**'da (görselden videoya) üretir.
- Her çekim için 9:16, 1080×1920 **başlangıç karesi** (gerekirse bitiş karesi de). Karakterler referans kartlarından girdi alınarak üretilir, tutarlılık şart.
- `docs/art/task26/CEKIM_LISTESI.md`: çekim no, süre (2–4 sn), başlangıç/bitiş karesi dosyası, **Higgsfield hareket istemi** (İngilizce, kamera hareketi + karakter hareketi + "no text, no morphing of puzzle shape"), ses notu ("weee").
- "Uyandır" anı ikiye bölünür: video A uyanmadan önce donar, oyunda dokunma gelir, sonra video B oynar.

Teslim: `docs/art/task26/storyboard-v04.png` + `CEKIM_LISTESI.md` + 9 başlangıç karesi. 3 satır özet.
