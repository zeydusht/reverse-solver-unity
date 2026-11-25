# PM_MESAJ_108: Menü → bölüm dalışı: Zeyd'in hız grafiği (ÖNİZLEME, yayın yok)

Kodu kendin yaz. **PM_107 bitmediyse önce onu bitir.** Ayrı yerel dal `pm108-onizleme`. **Push yok, yayın yok.** Zeyd videoyla onaylayınca yayınlanır. Bitiş mesajı kısa, en altta.

**Kapsam:** yalnız menü → bölüm dalışının (yay + yaklaşma) zaman eğrisi. Yol, çerçeve, resimde kalış, geri çekilme, kazanma → koridor dalışı, sesler: **değişmez**. Dalış whoosh'u yeni hıza uysun (PM_107 C6).

## Zeyd'in çizimi (25 Kasım 07:59)
Hız–zaman grafiği iki çizim:
- **Şimdiki** (üst): hız baştan sona neredeyse düz ve düşük. Zeyd: "Başta yavaş gidiyor gibi geldi."
- **İstenen** (alt):
  1. Hız sıfırdan değil, **orta bir hızla hemen başlar**.
  2. Hızlanarak **yüksek bir tepeye** çıkar; tepe kısa bir plato yapar (yolun ilk yarısı, yay kısmı).
  3. Sonra yumuşakça iner ve **daha düşük, sabit bir hızda** tabloya yaklaşır. Bu iniş ~2. saniye işaretinden önce biter.
  4. Bu sabit hızla devam eder (son yaklaşma).

Özü: **daha hızlı**, başta bekletmeyen, ortada atılan, sona doğru sakin ve sabit.

## Uygulama
- PM_92'deki "ekran hızına göre" kurulan eğriyi kullan (yol aynı, zaman dağılımı yeni). Ölçülen ekran hızı bu şekle uysun:
  - **t = 0:** hız tepenin ~%55'i (ilk karede belirgin hareket);
  - **0 → %25 süre:** tepeye yumuşak yükseliş (ivme sürekli);
  - **%25 → %45:** tepe platosu;
  - **%45 → %60:** tepenin ~%60'ına yumuşak iniş;
  - **%60 → son:** sabit hız (±%5); en son 1–2 karede çok kısa yumuşak iniş (PM_92'deki değme kalır).
- **Toplam süre:** yay + yaklaşma şimdiki 1,5 sn'den **~1,2 sn**'ye (Zeyd "daha hızlı" diyor).
- **Bayraklar** (Zeyd telefonda denesin):
  - `&dalis108=0` (eski);
  - `&dalishiz=` (toplam süre çarpanı);
  - `&dalistepe=` (tepe/son hız oranı, varsayılan 1,65).
- Kenar boşluğu 0, 20 ms'yi aşan kare raporu.

## Önizleme
- `docs/art/pm108/hiz.png`: tasarım ve ölçülen ekran hızı; Zeyd'in istediği şekil üstüne çizili.
- `docs/art/pm108/dalis_yan_yana.mp4` (şimdiki | yeni) ve `dalis_yeni.mp4`: 60 fps, 390×844, sesli (whoosh dahil), ≤ 10 MB.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu (eski → yeni), video yolları.
