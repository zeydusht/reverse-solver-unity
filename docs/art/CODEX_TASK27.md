# CODEX_TASK27 — Açılış videosunu yerelde üretmeyi dene (yalnız 01. çekim)

Zeyd soruyor: "Higgsfield yerine bunu yapabilen bir kütüphane yok mu, Codex bir denesin." Amaç: **01. çekimi** (Vadi ve sürü, 3 sn) Zeyd'in bilgisayarında üretmek ve Higgsfield'la karşılaştırmak. Girdi: `docs/art/task26/cekim-kareleri-v06/01-baslangic.png` + `01-bitis.png`, istem `CEKIM_LISTESI.md` 01. Çıktılar `docs/art/task26/video/` altına (git dışı tut, büyük dosya).

## Adım 1 — Donanımı ölç (2 dk, önce bu)
`nvidia-smi` (yoksa `wmic path win32_VideoController get name,AdapterRAM`), RAM, boş disk. Sonucu `docs/art/task26/video/DONANIM.md`'ye yaz.

## Adım 2 — Yola karar ver
- **A) Uygun NVIDIA GPU varsa** (yaklaşık 12 GB VRAM ve üstü): açık kaynak bir **görselden videoya** modeli dene. Wan 2.x I2V, LTX-Video veya benzeri, en kolay kurulan (diffusers ya da ComfyUI). Başlangıç ve bitiş karesi destekleyen tercih edilir. 9:16, 3–5 sn, düşük adım sayısıyla önce hızlı bir deneme. İndirme boyutu 20 GB'ı geçecekse **önce Zeyd'e sor**.
- **B) GPU yok ya da yetersizse:** model kurma. Bunun yerine **2.5D animasyon** dene: kareyi katmanlara ayır (gökyüzü, şato/tepe, çayır, Jiggles grupları), derinliğe göre paralaks kamera itişi, Jiggles'a küçük zıplama/döngü hareketleri (Python + Pillow/moviepy ya da ffmpeg). Kalite RM seviyesi olmaz, amaç ucuz bir karşılaştırma.

## Adım 3 — Teslim ve dur
- `01-yerel.mp4` (9:16, 3–5 sn) + `video/RAPOR.md`: hangi yol, ne kadar sürdü, nelerin iyi/kötü olduğu (yapboz şekli bozuldu mu, şato yerinde mi), diğer 10 çekim için tahmini süre.
- Herhangi bir ücretli API (Runway, Luma, Kling, Veo, Sora vb.) **kullanma, anahtar isteme**. Sadece raporda seçenek olarak yaz.
- Burada dur. Zeyd Higgsfield sonucuyla karşılaştırıp karar verecek.
