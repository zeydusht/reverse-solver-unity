# PM_MESAJ_46 — Kazanma/bomba titreşimi yutuluyor (0.5.2 yüklenmeden önce)

PM_45 kontrol edildi, 60 FPS ve titreşim haritası doğru. Tek hata:

## Sorun
`GameRoot.OnEvent`: son parça çıkınca aynı karede önce `PieceRemoved` → `Haptics.Play(Light)`, hemen ardından `Finished/Win` → `Haptics.Play(Success)` geliyor. `Haptics.Play` içindeki 50 ms kuralı ikinciyi atıyor → **kazanma titreşimi hiç çalışmıyor**. Bomba (Heavy) bir hamleyle aynı karede patlarsa o da yutulur.

## Yapılacak
1. `Haptics.Play`: `Success` ve `Heavy` 50 ms kuralına takılmasın (her zaman çalsın). 50 ms kuralı yalnız Light/Select/Medium için kalsın.
2. Kazanmada Light + Success üst üste gelmesin diye: son parça çıkıyorsa (`_session.Board.PresentCount == 0`) Light çalma, yalnız Success çalsın.
3. Sürüm **0.5.2 (6)** (build numarasını 1 artır), zip'i yeniden üret: `Builds/ReverseSolver-iOS-0.5.2.zip` (eskisinin üzerine).
4. `PROJE_DURUMU.md` iOS bölümüne bir satır ekle.

## Kabul
- Editörde/mantıkta: son parça → yalnız Success; bomba → Heavy her durumda çalıyor.
