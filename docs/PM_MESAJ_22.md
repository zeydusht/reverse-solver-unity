# PM → Claude Code (31 Ekim 2026) — Zincirli parça kilidi ortalama + iOS 0.5.1 (4)

## Sorun
Zincirli (renkli çift) parçalarda kilit simgesi hücrenin ortasında değil, aşağı kayık görünüyor (özellikle 40. seviye). Zincir uçlarındaki kilit gövdesi, ait olduğu hücrenin tam merkezine oturmalı.

## Yap
1. Kilit sprite'ının **gövde merkezi** = hücre merkezi olsun. Sprite pivotu/ofseti yüzünden (halka üstte, gövde altta) kayma varsa ofseti düzelt. Hem yatay hem dikey ortala.
2. Kilit boyutu hücrenin yaklaşık **%45**'i olsun (taşmasın, zincir halkaları okunabilir kalsın).
3. Zincir çizgisi kilitlerin merkezinden merkezine gitsin (çizgi kilidin ortasına girsin, kenarına değil).
4. Şu seviyelerde kontrol et ve ekran görüntüsü al: **40, 24, SHOW1**. 4 renk çiftinin hepsi (chain v3) doğru görünmeli. Yatay ve dikey zincirlere bak.
5. Bozulan yer olursa geri al, `OVERNIGHT.md`'ye yaz.

## Çıktı
- Commit mesajı: "fix: chain lock centred on cell".
- Önizlemeyi yayınla (`p19/?set=redesign` ya da yeni klasör), linki `OVERNIGHT.md` başına yaz. Kök adresteki Build 5'e DOKUNMA.
- Sonra iOS Unity derlemesi: `Builds/ReverseSolver-iOS-0.5.1.zip` yeniden üret, sürüm **0.5.1 (4)** (build numarası 3 → 4). Dosya tarihini kontrol et, bitince bana tarihi yaz.
- Masaüstü 9:16 değişikliği (PM_MESAJ_21) önce yayınlanmış olsun. Henüz yayınlamadıysan onu da bu turda yap.
