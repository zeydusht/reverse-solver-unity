# PM → Claude Code (31 Ekim 2026) — Koridor kazanma animasyonu entegrasyonu (PM_MESAJ_23'ten SONRA başla)

Önkoşul: PM_MESAJ_23 (SET40 v4) bitti. **PM_MESAJ_25 (veri sistemi Faz 1) önce gelir**; bu mesaj ondan sonra.

**ÖNEMLİ (1 Kasım): Bu geçiş oyun testine (pt1) girecek.** Davetler henüz gitmedi, Zeyd bu iş bitince gönderecek.
- `p19/`'a yayınla (test linki: `p19/?set=redesign&c=pt1`).
- **Bölümlere, `redesign.json`'a ve oynanışa dokunma.** Tahminler SET40 v4'e göre kilitlendi. Yalnız kazanma sonrası ekran değişiyor.
- **Veri kaydı bozulmamalı (PM_25 Faz 1).** Yayından sonra `?c=ic` ile 2 bölüm oyna (biri kazanma). `plays`/`moves` satırlarında cohort, device_id ve platform hâlâ dolu olsun. Kazanınca 1–10 zorluk sorusu ve 👍/👎 hâlâ gelsin. Geçiş animasyonu bu soruların önüne ya da arkasına binmesin.
  - Sıra: kazanma → (1–10 + 👍/👎) → koridor geçişi → sıradaki bölüm. Daha doğal bir sıra varsa yaz.
- Kök adrese ve iOS zip'e dokunma.

## PM kontrolü (1 Kasım): Codex Task 18 teslim edildi ve doğrulandı
- `corridor-empty.mp4` (780×1688, 60 FPS, 181 kare, 1,80 MB) ve `corridor-empty-web.mp4` (540×1168, 30 FPS, 91 kare, 0,70 MB) + eşleşen `frame-track.json` / `frame-track-web.json`. README'yi oku.
- **Kamera bir çerçeveden komşu çerçeveye kayıyor.** Her karede `instances` dizisi var: `bay-1` = başlangıçtaki çerçeve (çıkar), `bay-2` = komşu çerçeve (girer); ortada ikisi de kısmen görünür. Eşleme: **bay-1 = az önce kazanılan bölümün tablosu (N), bay-2 = sıradaki bölümün tablosu (N+1)**. Zeyd'in istediği "15'ten 16'ya geçiş" tam olarak bu.
- Hareket saf yatay kayma (y ve şekil sabit): quad'lar karelere göre yalnız x'te değişiyor. Perspektif dönüşümü (homografi) ya da sabit dörtgen + x kaydırma ikisi de olur.
- PM testi: iki test görselini her karede `instances` dörtgenlerine homografiyle yerleştirdim, dörtgeni merkezden **%1,2 büyütünce** kalan yeşil piksel 60 FPS'te en fazla 7, web sürümünde 1 (görünmez). Chroma-key kullanma, dörtgene yerleştir + %1,2 büyüt (çerçeve tablonun kenarını örter).
- Web için `corridor-empty-web.mp4` + `frame-track-web.json` kullan (2 MB altı). iOS'ta 60 FPS sürüm.
- 40. bölümden sonra N+1 yok: bay-2'ye yer tutucu (ör. "Koleksiyon tamam" ya da 1. tablo) koy veya animasyonu bay-1'de bitir, hangisi daha temizse; gerekçeyi yaz.

## Girdi (Codex hazırlıyor)
`docs/art/drafts/corridor-victory-v3-layered-20261031/` (yukarıdaki kontrol). Taslak (tablo pişmiş): `docs/art/drafts/corridor-victory-v2-floor-20261031/floor-revision-step.mp4`.

## Amaç
Bölüm kazanılınca koridor animasyonu oynar: önce kazanılan tablo (N) duvarda görünür, kamera koridorda kayar ve sıradaki bölümün tablosu (N+1) gelir; oyuncu sonraki bölüme "geçmiş" olur. Tablo videoya pişmez, katman olarak üstüne konur.

## Yap
1. **Mevcut koridor kazanma çalışmanı bul.** OVERNIGHT.md'ye göre commitlenmemiş bir koridor kazanma animasyonu işi vardı (kenara alınmıştı). Neredeyse ona bak, yeni videoya uyarlamak yerine ikisinden basit olanı seç, gerekçeyi OVERNIGHT.md'ye yaz.
2. **Tablo katmanı:** `frame-track.json`'a göre her karede tabloyu 4 köşeye oturt (perspektif veya sadece yatay kayma + ölçek yetiyorsa ona göre). Tablo çerçevenin içinden taşmasın, yeşil kenar görünmesin (hafif iç kırpma).
3. **Önden hazırlama:** Oyuncu N. seviyeyi oynarken N+1'in tablosunu arka planda yükle (Pages'ten). Kazanınca animasyon hemen başlasın, beklemesin. N+1 henüz inmediyse bekletme: yer tutucu tablo (kısa fade) göster, inince değiştir. Son seviyede (40) sonraki tablo yok, bitiş ekranı akışı değişmesin.
4. **Web ve boyut:** Web'de video oynatma ve yükleme süresini ölç. 2 MB'ı geçiyorsa web sürümünü (30 FPS / küçük) kullan. WebGL'de video oynatmak sorun çıkarırsa kare dizisi/sprite yolunu dene, hangisini seçtiğini yaz.
5. **Atla düğmesi:** Oyuncu animasyonu dokunarak atlayabilsin, atlayınca sonraki ekran hemen gelsin.
6. Ses/titreşim akışı bozulmasın (kazanma sesi, yıldızlar, devam düğmesi aynı sırada).

## Doğrula
- Önizlemede seviye 1, 15, 40, SHOW1: animasyon çalışıyor, tablo doğru, yeşil kenar yok.
- Seviye 14 → 15: 15'in tablosu önceden inmiş (ağı yavaşlatıp bir kez dene).
- Masaüstü 9:16 alanında ve dar pencerede bozulmuyor.
- Bozulan yer olursa eski kazanma ekranına geri dön, OVERNIGHT.md'ye yaz.

## Yayın / yapma
- Yalnız önizleme (`p19/?set=redesign` veya yeni klasör), linki OVERNIGHT.md başına yaz.
- Kök Build 5'e dokunma. iOS zip üretme (TestFlight sorunu sürüyor).
