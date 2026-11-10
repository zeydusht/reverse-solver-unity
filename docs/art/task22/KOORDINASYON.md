# Task22 eşzamanlı çalışma notu

2026-11-03: Bu oturum t22_prepare.py, t22_cleanup.py, task22/baslangic.json ve temizlik-* kanıtlarını oluşturdu. task22/prepare.py, jobs.json, screens.json ve original-art-baseline.json başka eşzamanlı yazardan geldi; bunlara dokunulmayacak.

Bu oturum imagegen ile yeni panel/kutu/anahtar/etiket/madalyon bitmapleri üretiyor. Çakışmayı önlemek için AI kaynak ve teslimleri Assets/Art/Redesign_T22/ai-kaynaklar ve 101+ numaralı aday klasörlerinde tutulacak. İnceleme girişi inceleme-ai.html olacak; ana inceleme.html varsa yalnız bağlantı eklenerek korunacak. Ana denetim raporu mevcutsa bu oturumun bağımsız dört turu ek rapora yazılıp bağlantı eklenecek.

Önemli doğrulama: Card.ChipRow mevcut kodda iki sıra, 375 pt ekranda 51x44 pt ve 390 pt ekranda 54x44 pt. Eski tek sıra ekran güncel hata diye yazılmamalı. MenuSettings.Add hedefleri en az44 pt; 36 pt toggle görünümü 36 pt hit anlamına gelmiyor.

Hiçbir dosya silinmeyecek, taşınmayacak, yeniden adlandırılmayacak. Öğretim eline ve oyun koduna dokunulmayacak.

2026-11-03T00:53:38+03:00 —101–113 hazır. Ana inceleme35 aile/36 önce-sonra ile birleştirildi. Yeniden render edilirse t22_merge_review.py yalnız bağlantı/ek bloğunu tekrar korur. AI PNGleri deneme artığı değildir; ai-manifest.json içinde20 runtime adayına karşılık gelir.
