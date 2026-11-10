# CODEX_TASK33 — Booster ikonları: yeniden boyama İPTAL, eskiye dön + kontur

Zeyd TASK32 ikonlarını beğenmedi: makasta renkler yeşil tuşla birbirine karışmış, değneğin yıldızı değişmiş, çekiç (mor+kırmızı) kötü, saatin yarısı siyah yarısı sarı. **TASK32'nin booster ikonu kısmı iptal.** HUD'un geri kalanına dokunma.

## Sorunun sebebi
RM ikonları yeşil tuşta iyi okunuyor, çünkü her ikonun etrafında **kalın koyu kontur** ve onun dışında ince açık bir parlama var. Bu ikonu yeşilden ayırıyor. Bizimkilerde kontur yok, o yüzden renkler yeşile karışıyor. Çözüm renk değiştirmek değil, **kontur ve ayrım**.

## Yapılacak
Başlangıç noktası **TASK31'deki ("önce") ikonlar**. Şekil, çizim ve yıldız **aynen**; yeni çizim yok.
1. **Her ikona kalın koyu kontur:** 64 pt tuşta ~2,5–3 pt, koyu lacivert/mor-kahve (`#2C2652` gibi). Dışına 1 pt açık parlama çizgisi.
2. **Doygunluk** hafif artsın (+%10–15), ama ton değişmesin.
3. İkonun altına ince, yumuşak bir gölge (yeşil gövde üstünde havada dursun).
4. İkon başına:
   - **Makas:** eski kırmızı kulp ve gümüş ağız aynen kalsın, yalnız kontur ve gölge.
   - **Sihirli değnek:** eski yıldız **birebir** kalsın, çubuk eski mor. Yalnız kontur.
   - **Çekiç:** eski **mavi çekiç** (`task28/_arsiv/Z2-v3-mavi-cekic`). Mor/kırmızı yok. Yalnız kontur.
   - **Saat:** yeşil saat yeşil tuşta kayboluyor, siyah da olmadı. Gövde **altın/turuncu** (RM topunun sarısı gibi, `#F0C010` → `#E09000`), kadran krem, akrep yelkovan koyu, ziller altın. Siyah yok.
5. Her ikon tuşun iç diskinin ~%70'ini kaplar, halkaya değmez.

## Teslim
`docs/art/task33/onizleme.png`: 4 ikon × (TASK31 önce / yeni), **gerçek yeşil tuşun üstünde**, 64 pt ve oyun ekranında alt şerit hâlinde. RM alt şeridi yanında. Dosyalar `Assets/Art/_yeni/HUD33/Boosters/`. 3 satır özet, dur.
