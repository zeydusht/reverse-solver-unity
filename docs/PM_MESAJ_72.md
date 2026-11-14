# PM_MESAJ_72: Görsel toparlama (p22). Zeyd'in 15 Kasım gece testi

Kodu kendin yaz. Yalnız p22. Önce `docs/` commit. Zeyd'in sözü: "arayüz çok bozulmuş, ortalık çok karışmış, önce görseli düzenleyelim, levelları sonra oynarım". PM_71 bekliyorsa bundan **sonra** yap.

## 0. Önce durumu gör
Zeyd'in kullandığı iki linkle bütün akışı 390×844 ekran görüntüsüyle gez:
- `p22/?c=p22&reset=1&lang=tr`
- `p22/?c=p22&test=1&lang=tr`

Gezilecek ekranlar: açılış, ana menü, harita, bölüm 1–3, bir bombalı bölüm, kazanma, sandık, kaybetme, Durdur.

Görüntüleri `docs/art/task29/pm72/once/` klasörüne koy. Her sorunun nereden geldiğini bul: hangi PM, hangi dal, hangi bayrak. PM_65'teki "onaylı tarz" listesi ölçüt.

## 1. Tutorial geri gelsin, eski el ve kararma hiç olmasın (HATA, tekrar etti)
- Zeyd 1. bölümde ekranın karardığını ve eski eli gördü. PM_60 kararı: ipucu yalnız alttaki Jigo'lu krem-mavi sohbet kutusu + parçada altın parlama. Kararma yok, el yok, büyük ok yok.
- Nedenini bul. PM_65'te düzeltilmişti, PM_66–70 arasında bir yerde geri gelmiş: `P21Onboarding`, `&tut=2x2` varsayılana mı sızdı, test modu farklı yoldan mı gidiyor? Hangi kohort, link ya da bayrak olursa olsun yalnız PM_60 akışı çalışsın. `&tut=2x2` dahil eski el kodunu p22'den tamamen sil.
- **Kalıcı test:** `c=p22`, `c=p22&reset=1`, `c=p22&test=1` ve kohortsuz linkte 1. bölüm açılınca sahnede el/ok/kararma nesnesi yok; sohbet kutusu ve altın parlama var.

## 2. Ana menüdeki üst mavi şerit kalksın
Ana menüde oyun içi HUD'a benzeyen üst mavi şerit "alakasız" bulundu.
- Menüde üst şerit olmasın: altın ve ayarlar arka planın üstünde küçük, yarı saydam krem hap olarak dursun.
- Alt sekme şeridi (Bölümler, Koleksiyon, Jiggles, Başarılar) kalsın.
- Önce/sonra görüntüsü al.

## 3. Komboyu kaldır, yerine her çıkışta küçük "tatmin" anı koy
Zeyd: "Kombo sarmadı. Art arda yapınca iyi ama hep art arda olmuyor, ekrandaki görüntü de çok büyük. Bir şeyi çıkarınca iyi bir dopamin salgılansın; minimal, telefonu titreten cinsten."
- **Kaldır:** kombo yazıları ("Güzel!", "Harika!"…), rozetleri, çerçeve ışık dalgası, şok halkası, kombo sayacı. `T47` görselleri p22'de kullanılmasın.
- **Her başarılı çıkışta:**
  - kısa hafif titreşim (iOS'ta light impact);
  - parçanın tahtadan ayrıldığı yerde küçük, opak altın kıvılcım (0,25 sn, küçük, tahtayı kapatmasın);
  - kısa "tık/pop" sesi.
- **Bombalı parça çıkınca:** orta titreşim + biraz daha büyük altın patlama + bomba ikonunun söndüğü kısa an.
- **Kilit açılınca:** hafif titreşim + kilit kırılma sesi.
- **Son parça:** başarı titreşimi (notification success), sonra kazanma ekranı.
- **Takılma:** çok kısa, zayıf "red" titreşimi (Zeyd'e ayrı ayar: açık/kapalı; varsayılan açık).
- **Titreşim altyapısı:**
  - iOS uygulamasında gerçek haptic kullan: UIImpactFeedbackGenerator / UINotificationFeedbackGenerator, küçük bir native eklentiyle.
  - Web'de Android'de `navigator.vibrate` (10–20 ms).
  - iPhone Safari'de web titreşimi yok. Orada yalnız görsel ve ses kalsın; raporda belirt.
  - Ayarlar'daki "Titreşim" anahtarı hepsini kapatsın.

## 4. Test linki: her şey açık
`&test=1` ile:
- bütün bölümler (1–40) haritada açık ve seçilebilir;
- her booster 999;
- altın 999 999;
- devam ücretsiz.

Normal linki etkilemesin. Test modundaki ilerleme normal kayda yazılmasın. Telemetride `test=1` işaretli gitsin ya da hiç gitmesin (pt/p22 verisi kirlenmesin).

## 5. Kazanma ekranı ve harita bölüm düğmeleri: yeniden çizim (Codex)
İkisi de "bizim stilden kopmuş, çok kötü" bulundu.
- Tarifler `docs/art/CODEX_TASK50.md`'de. **Codex'i `codex exec` ile sen çalıştır:**
  - komut `codex exec --skip-git-repo-check … < /dev/null`, `timeout 1800`;
  - her öğe ayrı çağrı;
  - çıktılar `Assets/Art/_yeni/T50/` ve `docs/art/task50/secenekler/`.
- Her öğe için **3 seçenek** üret, oyun içine yerleştirilmiş hâliyle (390×844) ekran görüntüsü al. **Oyuna bağlama.** Zeyd seçecek.
- Codex takılırsa ya da çıktı tarzdan uzaksa en çok 2 kez tarifle yeniden dene, sonra dur ve raporla.

## Teslim
- 1–4 p22'de canlı.
- EditMode yeşil, `verify_build` p22 4/4.
- Önce/sonra görüntüleri `docs/art/task29/pm72/`.
- T50 seçenekleri tek sayfada: `docs/art/task50/SECENEKLER.md` (her öğe A/B/C).
- `ENTEGRASYON_RAPOR.md`'ye "PM_72" eki, `PROJE_DURUMU`'na tek satır, commit + push. Sonra **dur**.
