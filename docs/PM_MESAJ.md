# PM → Claude Code (20 Ekim 2026, 08:55)

Oturum 3 bitti (Zeyd, 20 Ekim 07:49–07:58, oyuncu "ddd"). Z21–Z30'un hepsi ilk denemede KAZANÇ.
Puanlar: Z21 3 · Z22 2 · Z23 5 · Z24 3 · Z25 4 · Z26 6 · Z27 1 · Z28 1 · Z29 6 · Z30 1.

PM'in hesabı (dondurulmuş `fc4ef7c` ile):

| Ölçüt | ZeydBot | Rastgele oyuncu | Bomba payı |
|---|---|---|---|
| Puan sırası (Spearman, hedef ≥ 0,7) | 0,99 ✓ | 0,74 | 0,96 |
| Kazandı/kaybetti | 9/10 | 10/10 | 10/10 |
| Kalan sn ±%25 | 8/10 | 9/10 | 9/10 |

Z29'da bot %0 kazanma / 8,7 puan dedi; Zeyd 64 sn kala kazandı, puan 6.

**Zeyd'in sözlü notu (AYNEN):** "Bugünküler o kadar zor gelmedi, 12 gibisi gelmedi bile."
→ Oturum 3'teki zor bölümler Z12 / Z13 / Z18 seviyesine ulaşmadı. Hem öğrenme etkisi hem setin kendisi olabilir; ayır.

## İşler

0. Yukarıdaki sayıları Supabase'den doğrula. RAPOR.md'ye "Oturum 3 sonucu" bölümü olarak ekle (her sayının kaynağı sorgu), commit.

`zeydbot-v2` dalında:

1. **Kalibrasyon:** En zor bölümlerde aşırı karamsarlık var (Z29; LOO'da Z07/Z13/Z18 %0). Bombaya yönelme ve tuzak olasılığını her LOO katında ayrı hesapla.
2. **Öğrenme etkisi:** Oturum (1→3) ya da toplam oynanan bölüm sayısını beceri değişkeni olarak ekle. Zeyd dün zorların yarısını ilk denemede kaybetti, bugün hiçbirini.
3. Z01–Z30 (Z01/Z07 işaretli) ile yeniden kur. 30 bölümde LOO; tabanlarla aynı tablo.
4. **"Düşündüren zor" / "cezalandıran zor":** Z12 ile Z13 arasındaki farkı (tuzak var/yok, tek doğru hamle zinciri, düşünme süresi) bölüm özelliği olarak ölç ve puan modeline ekle. Z12'yi Z21–Z30'un zorlarıyla (Z23, Z25, Z26, Z29) kıyasla: bunlar neden Z12 gibi hissettirmedi?
5. **Oyun:** 1–10 sorusu kayıptan sonra da sorulsun (yalnız `?set=zeydbot`). Bir sonraki build'e koy; canlı 40'a ve bayraklara dokunma. Şema değişikliği gerekirse SQL taslağı yaz, PM çalıştırır.
6. **Z12 tipi adaylar:** v2 + 4. maddedeki bulgularla 5 "Z12 tipi" aday üret (hedef: Zeyd'in puanı 8–9, tuzak az, dar fitil, düşündüren). `?set=zeydbot` altında Z31–Z35 olarak yayınla. Tahminleri Zeyd oynamadan dondur ve commit et.

## Kurallar

- Eğri önerisini fabrikaya bağlama; önce PM onayı.
- Canlı 40, kök adres, bayraklar: dokunma.
- Bitince OVERNIGHT.md başına Zeyd için ≤ 10 satır sade Türkçe özet ve Z31–Z35 linkleri.
- Tahmin dosyalarında bölüm türü yazma (Zeyd kör oynasın), ya da türü dosyanın en altına, katlanmış bölüme koy.
