# PM_MESAJ_64 — Codex devrini birleştir → p22 önizleme + PM_63 + kombo merdiveni

**Kural (14 Kasım):** Zeyd Max plana geçti. **Kodu kendin yaz, `codex exec` kullanma.** Codex yalnız görsel üretir (gerekirse `docs/art/CODEX_TASKnn.md` yaz, PM'e bildir). Dal değiştirmeden önce `docs/`'u commit et.

## 0. Durum
- Canlı p21 = PM_62 (gh-pages `738806cb`). Zeyd'in telefonunda TestFlight 0.6.0 (7).
- Codex 12 Kasım'da `codex/01`…`codex/14` dallarında 14 adımlık bir devir yaptı (özet `docs/PROJE_DURUMU.md` → "Codex devir çalışması"). **Hiçbiri yayında değil.** HEAD `codex/14-hud-booster-polish`.
- PM_63 (zincirli parçanın takılması) **yapılmadı**. T47 kombo görselleri hazır (`Assets/Art/_yeni/T47/`, `docs/art/task47/LISTE.md`), oyuna girmedi.

## 1. Birleştir ve gözden geçir
- `codex/01`…`codex/14`'ü `art` (PM_62 hâli) üzerine sırayla birleştir. Çakışmaları çöz. Her adım için 1 satır not: ne getiriyor, PM_56–62 kararlarıyla çelişiyor mu.
- **Bilinen çelişkiler, Zeyd'e seçenek olarak bırak (bayrakla):**
  - **2×2 rehberli tutorial** (codex/01): Zeyd 10 Kasım'da 2×2'yi reddetti, PM_60'ta eski 1. bölüm + sohbet kutusuna dönüldü. p22'de **varsayılan PM_60 akışı**; Codex'in 2×2'si `&tut=2x2` ile açılsın.
  - **Engel sırası 3/10/14/20** (codex/03): Zeyd'in "tanıtımları yay" isteğine uygun → p22'de **varsayılan açık**.
  - **Saat** (codex/14: düğme kalkanı Süre'yi kapatır) ile PM_62 (fırça dokunur, panel donar): ikisini de tut; varsayılan **PM_62 fırçası**, Codex'inki `&saat=kalkan`.
  - **Jigo yeni kıyafet / Kont / yeni storyboard** (codex/07–09): yalnız sanat dosyası, **oyuna bağlama**. Mevcut onaylı Jigo ve açılış videosu kalır.
  - Makas temposu: PM_62 (1,2 sn) mi codex/14 (~2,1 sn) mi? Varsayılan PM_62; `&makas=yavas` ile Codex'inki.
- Test modu (`&test=1`) korunsun.

## 2. PM_63: zincirli parça takılması (`docs/PM_MESAJ_63.md`)
Geçersiz hamlede zincirli parça **yuvasından çıkmaz**; normal parçalardaki kısa sarsıntı + zincirde küçük dürtü. Geçerlilik **hareketten önce** kontrol edilsin. Takılma sarsıntısı genliği ~%70'e insin (0,15–0,2 sn, en çok 2 salınım). EditMode testi ekle.

## 3. Kombo merdiveni (T47)
`docs/art/task47/LISTE.md` ve `DERS.md`'ye göre: x2 "Güzel!", x3 "Harika!", x4 "Süper!", x5+ "Muhteşem!" (EN: Nice/Great/Super/Amazing). Kademe arttıkça rozet büyür, renk ısınır, patlama güçlenir; x4+ çerçeve ışık dalgası + şok halkası. Ritim: önce temas ışığı → kırılma → ~300 ms sonra rozet (50–100 ms'de 1,3×, 100–150 ms'de 1×'e oturur, ~650 ms görünür). Kombo penceresi 1,2 sn, **takılma komboyu sıfırlar** (PM_62). Skor/puan sayısı yok (oyunumuzda puan yok); "+20" gibi yazıları kullanma.

## 4. Yayın
- **Yeni klasör `p22/`** (p20, p21, kök, pt1'e dokunma). Seviyeler p21 ile aynı kaynaktan.
- Linkler: `p22/?c=p22&lang=tr`, test: `p22/?c=p22&test=1&lang=tr`.
- EditMode (Perf hariç) yeşil, `verify_build` p22 için 4/4, PC FPS. GIF: zincirli takılma önce/sonra, kombo x2→x5, saat (iki seçenek). `docs/art/task29/pm64/`.
- Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_64" eki: 14 dalın birleştirme notları + bayrak listesi. PROJE_DURUMU'na tek satır. Commit + push.
