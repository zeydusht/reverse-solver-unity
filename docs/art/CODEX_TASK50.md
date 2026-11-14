# CODEX TASK 50: Kazanma ekranı + harita bölüm düğmeleri (yeniden)

Reverse Solver, dikey mobil (390×844 ölçüt, @3x çiz). Her öğe için **3 farklı seçenek** (A, B, C). Çıktılar şeffaf PNG parçaları + oyun içi birleşik önizleme.

## Ortak stil (her tarifin başına ekle)
> Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.
>
> Palette:
> - deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
> - warm cream #F6EBD3 panels,
> - antique gold #D9A441 trims with soft drop shadow,
> - green #3DBA4E primary buttons with layered 3D bevel.
>
> Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.
>
> Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.
>
> Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

## Öğe 1: Kazanma ekranı (3 seçenek)
Zeyd'in yönü: **sade, büyük, sevimli, tek bakışta "kazandın"**. Kalabalık yok.

Ekrandaki öğeler (yukarıdan aşağı):
1. 3B başlık "Harika!" (EN: "Great!") ayrı katman. Kurdele yok.
2. Büyük mutlu Jigo, yüksüğüyle selam veriyor (ayrı katman, 2 poz: zıplama ve selam).
3. Ödül: tek satır "+50" altın sikke ikonu. Sikke ikonu ayrı katman.
4. Sandık ilerleme çubuğu: küçük, 5 bölmeli.
5. Yeşil 3B "Sonraki Seviye" butonu (büyük) + altında küçük mavi "Menü".

Seçenekler:
- **A:** krem kart, mavi çerçeve, ekranın ortasında; arka plan bulanık tahta.
- **B:** kartsız, öğeler doğrudan mavi ışıklı arka plan üstünde; arkada altın konfeti ve ışık huzmesi.
- **C:** üstte tablo çerçevesi içinde çözülmüş resmin küçük hâli ("tablo kurtarıldı" hissi), altında Jigo ve buton.

Parçalar: `win_title_tr.png`, `win_title_en.png`, `win_jigo_jump.png`, `win_jigo_salute.png`, `win_panel.png` (A için), `win_bg_glow.png` (B için), `win_frame_small.png` (C için), `coin.png`, `chest_bar_bg.png`, `chest_bar_fill.png`, `btn_green_big.png`, `btn_blue_small.png`.

## Öğe 2: Harita (bölüm merdiveni) bölüm düğmeleri (3 seçenek)
Yuvarlak madalyon düğmeler, ortada büyük sayı (sayı oyunda yazılacak; boş madalyon çiz, ayrıca sayılı örnek önizleme).

Durumlar, her seçenekte hepsi:
- **Tamamlandı:** mavi gövde, altın çerçeve, küçük altın tik rozeti.
- **Şu anki:** daha büyük (1,25×), parlak altın çerçeve, hafif ışık halesi. Üstünde küçük Jigo başı işareti (ayrı katman).
- **Kilitli:** soluk gri-mavi gövde, gümüş çerçeve, küçük asma kilit rozeti.
- **Zor bölüm:** kırmızı-bordo gövde, koyu altın çerçeve, Kont mühürü rozeti (mühür, kafatası değil).
- **Engel dersi:** normal düğme + küçük engel rozeti (kilit / zincir / duvar / bomba ikonları, onaylı ikonlarla uyumlu).
- Düğmeler arası yol: kalın altın noktalı çizgi (karo).

Seçenekler:
- **A:** düz madalyon, kalın 3B kenar.
- **B:** müze rozeti gibi oymalı altın çerçeve.
- **C:** yapboz parçası biçimli düğme (oyunun konusu).

Parçalar: her durum için `node_<durum>.png`, `badge_check.png`, `badge_lock.png`, `badge_kont.png`, `badge_<engel>.png`, `jigo_marker.png`, `path_dot.png`.

## Teslim
- `Assets/Art/_yeni/T50/<öğe>/<A|B|C>/` parçalar.
- `docs/art/task50/secenekler/` önizlemeler (oyun içi birleşik, 390×844).
- `docs/art/task50/LISTE.md`.
