# PM_98: Yeni bölüm müzikleri ve UI seslerinin lisansları

Hepsi **CC0 1.0** (kamu malı): ticari kullanım serbest, atıf gerekmiyor. https://creativecommons.org/publicdomain/zero/1.0/

OpenGameArt sayfalarında üç alan tek tek kontrol edildi: lisans, açıklama ve "Attribution Notice" (atıf notu). Atıf notu dolu olan sayfalar alınmadı.

Referans (Block Out!, `docs/art/pm96/ref/`) yalnız ölçüm ve yerel karşılaştırma için. Repoya ve yayına girmez.

## Bölüm müziği A–D: kendi bestelerimiz (`Tools/make_level98.py`)

- **Melodiler, akorlar ve davul kalıpları bizim.** Hiçbir parçadan kopya yok; nota planı `BESTE.md`'de.
- **Çalgılar:** Versilian Community Sample Library (VCSL) kayıtları. https://github.com/sgossner/VCSL, `LICENSE`: CC0 1.0 Universal.
  - **Davul ve perküsyon:** Bass Drum 1, Cajon, Snare Drum Modern 1, Claps, Hi-Hat Cymbal, Shaker (Small), Tambourine 1.
  - **Ezgi:** Marimba, Xylophone, Glockenspiel, Concert Harp, Kalimba (Kenya), Grand Piano (Kawai).
- **Bas:** bizim sentezimiz (sinüs ve harmonikleri); dış kaynak yok.

## Bölüm müziği E–G: CC0 araması (`Tools/make_music98.py`)

| Bayrak | Parça | Yükleyen | Sayfa | Vurmalı oranı | Döngü |
|---|---|---|---|---|---|
| `E` | Cat in Space | mutantleg | https://opengameart.org/content/cat-in-space | 0,63 | parçanın kendi tekrarı, 32 sn |
| `F` | Danza D' Bots II | lushogames | https://opengameart.org/content/danza-d-bots-ii | 0,57 (eşik altı) | parçanın kendi tekrarı, 87 sn |
| `G` | Upbeat Beat | centurionofwar | https://opengameart.org/content/upbeat-beat | 0,55 (eşik altı) | parçanın kendi tekrarı, 40 sn |

**Arama.** OpenGameArt'ta yalnız CC0 filtresiyle şu anahtar kelimelerle 532 sayfa tarandı:

- arcade, edm, dance, drums, beat, electronic, funk, house, breakbeat, groove;
- hip hop, casual, hyper casual, mobile game, techno, synthpop, drum loop, percussion, rhythm.

PM_96 havuzuyla birlikte yaklaşık 330 parçanın vurmalı oranı ölçüldü (referansla aynı ölçü).

**Sonuç:**

- 0,6 eşiğini geçen yalnız 2 parça var:
  - Cat in Space (0,63, 120 BPM);
  - Next to You (0,70, ama 92 BPM'lik duygusal bir parça; elendi).
- Gerçek CC0 oyun müziklerinde bu oran neredeyse hiç 0,5'i geçmiyor. Referanstaki 0,92 bu yüzden ancak bizim bestemizle (A–D) yakalandı.
- F ve G sayfada "eşik altı, karşılaştırma için" diye işaretli.

## UI sesleri (`Tools/make_ui98.py`, `Resources/Art/audio/pm98/`)

| Ses | Kaynak | Sayfa |
|---|---|---|
| tık | PM_95'in tık adayları (`docs/art/pm95/LISANS.md`; varsayılan b: OwlishMedia "87 Clickety Clips") | https://opengameart.org/content/87-clickety-clips |
| anahtar | Kenney **UI Audio** (switch16, switch18) | https://kenney.nl/assets/ui-audio |
| panel açılış / kapanış vuşu | artisticdude **Swishes Sound Pack** (swish-4, swish-5; kapanış ters çalınır) | https://opengameart.org/content/swishes-sound-pack |
| pasif tok | Kenney **Impact Sounds** (impactWood_light 000, 002) | https://kenney.nl/assets/impact-sounds |
