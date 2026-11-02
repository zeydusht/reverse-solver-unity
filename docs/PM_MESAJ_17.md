# PM → Claude Code (27 Ekim 2026, 20:40) — Vitrin bölümü (SHOW1)

Bu yeni bir oturum olabilir. Önce bağlamı al:
- `CLAUDE.md`
- `OVERNIGHT.md` başı: son durum
- son PM mesajı: `docs/PM_MESAJ_16.md`

Kurallar:
- Soru sorma.
- Token tasarrufu yap.
- Her adımda commit at.

## Amaç

Zeyd oyunu bir yatırımcıya (Görkem Bey) gönderecek. Bu kişi linke bir kez tıklayıp 2–3 dakika oynayacak. O sürede oyunun en iyi halini görmeli: resimli tahta, bütün engeller ve bir "aha" anı.

## Yap

1. **Yeni level: SHOW1.**
   - Resimli moddaki Akdeniz resmini kullan: `Assets/Art/Gameplay/Task15/painting-test-01.png`. Mevcut `painting=1` modunda S46'da kullanılanla aynı.
   - 40'lık sete (SET40 v3) EKLEME; ayrı kimlikle dursun.
   - Tahta 6×6 ya da 7×7.
2. **Engeller.** Dördü de olsun:
   - en az 1 bomba
   - 1–2 çivi
   - 1 zincir (iki uçlu, kilitli)
   - en az bir kenarda duvar
3. **Zorluk orta.** İlk kez oynayan biri 2–3 dakikada, en fazla 1–2 denemede çözebilmeli.
   - Süre ve hamle sınırı cömert olsun.
   - Bomba gerilim yaratsın ama kaybettirmesin.
4. **"Anahtar parça" anı.** Bombayı kurtarmak için önce zincirli parçaların doğru sırayla açılması gereksin.
5. **Doğrulama.**
   - Mevcut level fabrikası düzenini kullan: tasarımcı agent ile üret, ZeydBot Monte Carlo ile doğrula.
   - İlk denemede kazanma oranı yüksek olsun.
   - Metrik puanlarını (M1–M17) ve bot sonucunu OVERNIGHT.md başına yaz.
6. **Doğrudan açılış.**
   - `lv=SHOW1` linki menüyü ve isim girişini atlayıp doğrudan bu bölümü açsın. İsim gerekiyorsa otomatik "SHOW" olsun.
   - Bölüm başında tek cümlelik bir ipucu kartı olsun: "Parçaları kaydırarak çıkar, bombayı patlamadan kurtar."
   - Kazanınca kart "Tebrikler! Bu bir demo bölüm." desin; tek tuş "Tekrar oyna" olsun.
7. **Telemetri.** Bu linkten gelen oyunlar kayıtlarda "SHOW" etiketiyle ayrılsın. Supabase şemasına dokunma; mevcut alanları kullan.
8. **Yayın.** Linki yayınla: `https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=SHOW1`
   - 3 telefon boyutunda kontrol et: 375×667, 390×844, 430×932.
   - Linki OVERNIGHT.md'nin en başına yaz.

Dokunma:
- kök adresteki Build 5
- 40'lık set
- iOS ayarları
