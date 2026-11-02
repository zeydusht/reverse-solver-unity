# PM → Claude Code (1 Kasım 2026) — Koridor geçişi "Ana menü" yolunda da oynasın (p19)

Zeyd p19'u denedi:
- Kazanma kartında **"Sonraki bölüm"** → koridor geçişi oynuyor. Doğru.
- **"Ana menü"** → geçiş yok, menü direkt açılıyor. Zeyd burada da geçiş istiyor.

## Yap
1. **Kazanma sonrası "Ana menü":** kazanma kartı → aynı koridor geçişi (N'nin tablosu → N+1'in tablosu) → **menü açılır**.
   - Menüdeki büyük tablo zaten N+1'in tablosu. Geçişin son karesi menüye yumuşak bağlansın.
   - Kısa bir fade (0,2–0,3 sn) yeter, menü UI'si (profil, sayaç, Level tuşu) bu sırada belirsin.
   - Ekranda zıplama olmasın.
2. **Tıklama kontrolü:**
   - "Sonraki bölüm" ya da "Ana menü" tuşuna basan tıklama geçişi **atlatmasın**.
   - Atlama yalnız geçiş başladıktan sonraki yeni bir dokunuşla olsun (ör. 0,3 sn sonra gelen dokunuş).
3. **Kaybetme kartında** ve 40. bölüm bitişinde geçiş **yok**. Mevcut davranış korunsun.
4. **Değişmeyecekler:**
   - Bölümler, `redesign.json`, kök adres ve iOS.
   - Veri kaydı: cohort, device_id ve 1–10 sorusu aynen kalır. 1–10 ve 👍/👎 sorusu geçişten **önce** gelir.

## Kontrol
- `verify_build.js --only corridor`'a "Ana menü" yolunu ekle (1→menü, 15→menü). Menü N+1 tablosuyla açılıyor mu, yeşil piksel 0 mı?
- Canlıda `?c=ic` ile tek kazanma → Ana menü dene.
- OVERNIGHT.md başına 1 satır.

Yayın: `p19/`. Davetler henüz gitmedi.
