# Codex görevleri (Claude Code yazar, Codex yapar)

Kurallar: her görevde **push/deploy yok**, Unity çalıştırmak gerekmez. Yalnızca görevde adı geçen dosyalara ve `docs/art/`, `Assets/Art/` altına dokun. Bitince görevin başlığına **Bitti** yaz ve 1–2 satır not bırak; Claude Code git diff ile kontrol edip birleştirir.

## 1. İngilizce metin taslağı — Bitti
- **Amaç:** Oyuncuya görünen yeni metinlerin İngilizce karşılığı.
- **Dosyalar:** okunur: `Assets/_Game/Presentation/Ui/Texts.cs`, `Assets/_Game/Levels/redesign.json` (`intro`, `bintro` alanları). Yazılır: yalnızca yeni `docs/i18n/en.md`.
- **Kabul:** Texts.cs'deki her sabit ve redesign.json'daki her intro/bintro için `Türkçe | English` satırı içeren tablo; karakter adları `docs/HIKAYE.md`'deki gibi (Jiggles, Baron); kod dosyaları değişmez.

Not: 5 UI sabiti ve 8 bölümdeki intro/bintro başlık, açıklama ve ipuçları `docs/i18n/en.md` tablosuna eklendi. Yalnızca dokümantasyon değişti; Görev 2–3 onay bekliyor.

## 2. Tutorial eli PNG export'u (P0 #4 onaylanınca)
- **Amaç:** Onaylı el çiziminden oyun export'u.
- **Dosyalar:** `Assets/Art/Tutorial/Exports/hand-tap.png`, `hand-swipe.png`.
- **Kabul:** 288×288 px, şeffaf arka plan, parmak ucu görselin sol üst çeyreğinde ve `docs/art/HANDOFF.md`'de pivot noktası (px) yazılı; her dosya ≤ 60 KB.

## 3. Engel tanıtım kart görselleri export'u (P0 #5 onaylanınca)
- **Dosyalar:** `Assets/Art/Intro/Exports/{nail,bomb,chain,wall,hard}.png`.
- **Kabul:** 888×592 px, PNG, her biri ≤ 250 KB; HANDOFF.md'ye satır eklenmiş.
