# PM → Claude Code (3 Kasım 23:25): profil sorusu bölüm 2'den sonra (p20, davetten önce son iş)

**İlk iş:** commit et. PM dosyalarını asla geri alma.

## Değişiklik
- **Zamanlama:** Profil sorusu artık isim girişinden hemen sonra **sorulmayacak**. Bölüm 2'nin kazanma kartında oyuncu "Sonraki bölüm"e (ya da "Menü"ye) basınca bir kez sorulsun.
  - Bölüm 3'ün çivi tanıtım kartı, soru cevaplandıktan **sonra** açılsın. PM_40'taki "tanıtımı yutma" düzeltmesi korunmalı.
  - Oyuncu bölüm 2'yi kaybedip menüye dönerse soru gelmesin; bölüm 2'yi kazanınca gelsin.
  - Doğrudan bölüm linkiyle (`?lv=`) 2'den ileri bir bölüme giren oyuncu ilk kazandığı bölümün sonunda görsün.
- **Metin:**
  - TR: "Telefonda bulmaca oyunları oynar mısın? (Candy Crush, Royal Match gibi)"
  - EN: "Do you play puzzle games on your phone? (like Candy Crush or Royal Match)"
  - Başlık "Tek bir soru" / "One question" aynı kalsın. Cevaplar aynı: Hiç / Ara sıra / Sık.
- `player_profiles` satırı ve anahtarı aynı. Veri biçimine dokunma.

## Kontrol
- `?c=ic` ile yeni profil aç: isim → bölüm 1 → bölüm 2 kazan → soru → cevap → bölüm 3 çivi kartı.
- Supabase `player_profiles`'ta satırı gör.
- Kart kilidi (PM_38) bu akışta da çalışsın: `jams` 0, `tut`/jam satırı yok.
- p20'ye yayınla. Sürüm etiketi `web-p20-2026-11-03` kalsın.
- OVERNIGHT başına 2 satır. Zeyd'e "bitti" de.
- **5 dakika kuralı:** Bu küçük bir iş. Build ve yayın dahil 25 dakikayı geçmesin. Takılırsan dur ve durumu yaz.
