# PM_MESAJ_66 — Zeyd'in p22 kararları (14 Kasım 10:05) (p22)

Kodu kendin yaz, `codex exec` kullanma. Yalnız p22 (p20, p21, kök, pt1'e dokunma). Önce `docs/` commit.

## 1. Takılma: BÜTÜN parçalar için (PM_63'ün genişletilmesi)
Zeyd: "Zincirli parça için yaptığını diğer parçalar için de yap; gitmiyorsa çok hareket etmesin." Hiç ilerleyemeyen **her** parça (normal, zincirli, kilitli, bombalı; sert ya da yumuşak engel fark etmez) yalnız küçük takılma payı alır (0,049 hücre, 0,18 sn, en çok 2 salınım). Geçerlilik `Probe` ile hareketten önce. Yumuşak engeldeki %30 sürükleme payı kalksın. Test: 40 bölümde bütün takılan hamlelerde parça yuvasında kalıyor.

## 2. Saat: varsayılan Codex'in **sol el + düğme kalkanı** versiyonu
Zeyd: "Sağ kolunu uzatmıyordu; Codex sol eliyle uzanarak çizmişti." Varsayılan = codex/14 kalkan akışı (`&saat=kalkan` olan). Ama **portre canlı kalsın** (nefes, geçiş kareleri; `&portre=sabit` yalnız bayrakla). PM_62 fırçası `&saat=firca` ile açılsın.

## 3. Değnek: gerçekten işe yarıyor mu? Görünür yap
Zeyd ne yaptığını anlamadı. Önce ölç ve raporla: değnek bir bölgede ne değiştiriyor (çıkıntı yönleri), kaç kez **yeni serbest parça** doğuruyor (40 bölümde rastgele durumlarda oran). Sonra:
- Değnek, seçilen bölgede **en az 1 parçayı serbest bırakacak** şekilde değişiklik seçsin (mümkün değilse o bölge seçilemez/gri).
- Görsel geri bildirim: bölgedeki parçalar kısa havalanıp **dönerek** yeni çıkıntılarıyla oturur; artık çıkabilen parçalar **altın parlar** ve küçük yazı: "2 parça serbest!".
- Değişiklik yoksa booster harcanmaz.

## 4. Bomba sırası: değiştirme, bilgi ver
Zeyd'e açıklandı: bomba dersi 20'de olduğu için daha önceki bölümlerdeki bombalar kaldırıldı (Codex devri). Şimdilik **varsayılan kalır**; raporda 20'den önce bombası kaldırılan bölümleri listele (bölüm no, kaç bomba). Zeyd belki bomba dersini öne alacak (ör. 7).

## 5. Küçük düzeltme
Makas seçim modunda alt şeridin üstünde soluk ikinci ipucu satırı ("Kesmek istediğin birleşim noktasına dokun") yineleniyor → kaldır.

## 6. Codex görselleri
`docs/art/CODEX_TASK48.md` güncellendi (menü, Ayarlar/Profil, menü şeritleri, **kazanma ekranı baştan**). Zeyd Codex'e verecek. Gelince bağla: Durdur'a **T45 paneli** (yuvarlak Ses/Müzik/Titreşim, kırmızı Çık) — bu T45'te zaten hazır, **şimdi bağla**.

## Teslim
p22 güncel, EditMode yeşil, `verify_build` p22 4/4. GIF: normal parça takılması, değnek (serbest kalan parçalar), saat (kalkan, canlı portre), Durdur T45. Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_66", PROJE_DURUMU'na tek satır, commit + push.
