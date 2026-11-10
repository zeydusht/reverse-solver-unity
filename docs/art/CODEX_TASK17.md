# Codex Task17: Zincir v3 (resimli tahtada görünür zincir)

Zeyd 40 bölümü resimli tahtada oynarken zincirin neredeyse görünmediğini söyledi. Halkalar çok ince ve küçük, gri rengi de detaylı resmin içinde kayboluyor. Kilit (uç parçası) görünüyor, ama iki kilidi bağlayan zincir seçilmiyor.

Kurallar:
- Yalnız `Assets/Art/Gameplay/Task17/` ve `docs/art/` klasörlerine yaz. C#, Unity ya da dotnet çalıştırma.
- Renkler `PALETTE.md`'den gelsin.

## İstenen

1. **Zincir halkası v3 (`chain-link-v3.png`)**
   - Tekrarlanan halka, 128×64. Eskisinden belirgin şekilde **kalın ve iri** olsun; tahtada hücrenin yaklaşık %18–20'si kalınlığında görünecek.
   - Renk: koyu demir / lacivert gövde (#0B2A36 civarı), üstte altın ışık vurgusu (#FBBA0E). Gri olmasın.
   - **Dış hale:** halkanın çevresinde 3–4 px krem (#F8EBD0) kontur. Böylece hem koyu hem açık resim bölgelerinde zincir ayrışır.
   - Halkalar uç uca eklenince kesintisiz bir zincir oluşmalı; dikiş görünmesin.
2. **Uç kilidi v3 (`chain-lock-v3.png`)**, 160×160: mevcut altın kilit stilinde, zincirle aynı kalınlıkta bağlantı halkası olsun.
3. **Önizleme:** `preview-chain.png`. Zincir, resimli bir tahtanın üstünde çapraz uzanıyor olsun; aynı görselde eski zincir yan yana dursun. Kullanılacak resimler:
   - `Assets/Art/Paintings/painting-40.jpg` (detaylı ve renkli)
   - `painting-24.jpg` (karlı, açık renk)

   İki resimde de zincir tek bakışta görünmeli.
4. HANDOFF'a satır ekle: dosya / oyunda nereye / boyut / pivot.

Bitince "Task17 bitti" yaz.
