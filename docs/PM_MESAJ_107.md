# PM_MESAJ_107: GECE İŞİ: ses miksajını finalize et + görsel uyum denetimi (Block Out kalitesinde menü, altın, koleksiyon)

Zeyd uyuyor, sabaha kadar çalış. Kodu kendin yaz. p23 üzerinde. **PM_106 / EK / EK2 bitmediyse önce onları bitir.** Önce `docs/` commit. Codex: en güçlü model, `xhigh`, `< /dev/null`, `timeout 1800`; kalite döngüsü PM_74 ile aynı (en az 3 fikir, puanla, en iyisini iyileştir, ölçütü geçmeyeni bağlama). Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

## Güvenlik (PM_74 kuralları)
- Her aşamada commit + p23'e yayın + `verify_build` p23.
- Bir değişiklik bir şeyi bozarsa geri al, raporla, devam et.
- Force push yok. Supabase'e yazma yok.
- **Zeyd'in onayladığı şeylere dokunma:** dalış, koridor, çerçeve, lastik, kazanma ekranı yerleşimi, HUD.
- Tarayıcı sekmeleri: işin bitince kapat. `OpenURL` testte açmasın (PM_106_EK).

## A. Ses seçimleri: varsayılan (Zeyd, 24 Kasım 23:59)

| Ses | Seçim |
|---|---|
| Lobi | n |
| Bölüm | E |
| Kazanma | g |
| Kazanma tınısı | **pizz** |
| Çıkış | **u** |
| Çarpma | a |
| Makas | c |
| Değnek | a |
| Çekiç | **c** |
| Saat | **b** |
| Koridor | a |
| Dalış | a |
| Çizgi | **c** |
| Geri | a |
| Sekme | a |
| Düğme | **c** |
| Zincir | **f** |
| Zincir gerilme | **f** |
| Kilit dokunma | a |
| Kilit açılma | a |
| Patlama | a |
| Duvar | a |

`ses.json`'a yaz. iOS'a göm, seçilmeyenler build dışında. `SES_LISANSLARI.md` güncel.

## B. Zincirli parça serbestçe kayınca da zincir sesi
- **Sorun:** Zincirli parça takılınca zincir sesi geliyor. Ama boşluğa itilip başarılı kaydığında (zincir izin verdiği kadar gidip çıkınca ya da zincirle sürüklenince) zincir sesi yok.
- **İstenen:** zincirli parça her hareket ettiğinde (sürükleme, kayma, çıkış, zincirin sallanması) zincir şıkırtısı. Hız ve zincir hareketiyle orantılı, durunca söner. Takılmada gerilme sesi (f) ayrıca.

## C. Ses mühendisliği: tam miksaj (en önemli iş)
Zeyd: "Ses ayar dengesini çok iyi yapman lazım. Bir şey kulağımı tırmalamasın, ne çok olsun ne az; net ve temiz duyulsun. Saati durdurdum, parçaları kaydırırken tik takı hiç duymadım; kayma sesi tik taktan fazlaydı."

1. **Kanallar (bus):** müzik, oyun efektleri, UI, sürekli sesler (saat tik-tak, zincir şıkırtısı), fanfar / jingle. Her birinin seviyesi tek yerden.
2. **Hedef düzeyler:** her ses için kısa ses yüksekliği (LUFS) ve tepe tablosu. Telefon hoparlöründe orta seste:
   - müzik arkada;
   - efektler net;
   - hiçbir ses ötekini tamamen örtmüyor.
3. **Öncelik ve kısma (ducking):**
   - saat booster'ı aktifken tik-tak duyulsun: kayma / çıkış sesleri o sürede ~4–6 dB kısılsın ya da tik-tak öne alınsın; müzik de biraz insin;
   - fanfar çalarken diğer efektler kısılsın;
   - patlama ve kayıp anında müzik kısılsın.
4. **Tırmalamayı önle:**
   - kayma ve çıkış seslerinde sert tizleri yumuşat (yüksek raf filtresi, 6–8 kHz civarı);
   - saldırıyı birkaç ms yumuşat;
   - art arda çok hızlı aynı ses (fiske serisi) üst üste binip patlamasın: aynı sesten aynı anda en çok 2, ses sınırlayıcı (limiter) ana çıkışta.
5. **Senaryo kayıtları:** her senaryoda sesli kayıt + seviye grafiği `docs/art/pm107/miks/`:
   - bölüm oynanışı (10 çıkış, 3 takılma);
   - saat booster'ı aktifken kayma;
   - zincirli bölüm;
   - bomba + kayıp;
   - kazanma fanfarı + sandık;
   - menü gezintisi (bütün düğmeler).

   Her kayıtta "duyulmayan ses" ve "tepe aşımı" sayısı 0.
6. **Dalış sesi:** menüden Oyna'ya basınca tabloya yaklaşırken neredeyse ses yok, yalnız son anda "fışt". Whoosh yaklaşmanın başında başlasın, kamera hızıyla birlikte yükselsin, varışta sönsün. Kazanma → koridor → dalış da aynı denetimden geçsin.
7. Rapor `docs/art/pm107/MIKS_RAPORU.md`: önce/sonra tablo, ne neden değişti.

## D. Görsel uyum denetimi (bütün ekranlar)
Zeyd: "Koleksiyon detayında eski mavi kalmış. Yeni renge uymayan ne varsa düzeltin. Block Out gibi kaliteli olsun; karmaşık değil, bizim stile uyan."

**Referans:** `docs/art/pm107/ref/` (Block Out ekranları: ana menü, alt çubuk, altın sayacı, profil, koleksiyon, mağaza, takım). Yalnız referans: kalite, okunurluk ve katman dili için. Kopyalama yok, renk/karakter/yazı bizim (old money mavi + krem + altın, kalın 3B yazı).

1. **Tam tarama:** bütün ekranları ve ara panelleri 390×844 ekran görüntüsüyle gez. Eski renk paletinde kalanları (eski mavi, eski tonlar, eski font, eski çerçeve) listele: `docs/art/pm107/DENETIM.md` (ekran · sorun · önem 1–3).

   Ekranlar:
   - menü, 4 sekme;
   - **koleksiyon detayı** (Zeyd: eski mavi);
   - Jiggles, Başarılar, Profil;
   - Ayarlar, Durdur;
   - kayıp / süre doldu;
   - kazanma, sandık;
   - booster ders kartları;
   - engel kartları;
   - "Yeni tablolar yolda!";
   - yükleme ekranı;
   - reklam panelleri;
   - hediye.

   Hepsini yeni palete getir.
2. **Koleksiyon:**
   - kartlarda "Bölüm 1" yazısı okunmuyor: kontrast, boyut, kalın 3B;
   - arka plan resmi kayıyor / sıçrıyor gibi: bul ve düzelt (PM_87/88 sekme kayışıyla ilgili olabilir).
3. **Alt menü çubuğu (yeniden):** Block Out kalitesinde. Seçili sekme yükselen açık panel, büyük ikon + etiket; diğerleri ikon; ince ayrım çizgileri; derinlik ve gölge; ikonlar daha zengin ve oyuncak gibi ama sade. Codex 3 seçenek, PM_74 döngüsü.
4. **Altın (coin) görseli (yeniden):** Block Out'taki gibi parlak, kalın kenarlı, okunur; sayaç kutusu ve "+" yok. Her yerde aynı: menü, HUD, kazanma, sandık, devam paneli. Codex 3 seçenek.
5. **Profil / oyuncu rozeti** (menü sol üstü, Profil ekranı): referanstaki gibi temiz çerçeveli avatar + isim + bölüm. Bizim Jiggle avatarlarıyla.
6. **Kendi bulduklarını da düzelt** (PM görevi: sen de görev bul): yazı okunurluğu, hizalama, taşma, dokunma alanları, tutarsız düğmeler, kalitesiz kalmış görseller.
   - Kural: "daha iyi" = daha karmaşık değil. Daha temiz, okunur, bizim stilde.
   - Her değişiklik için önce/sonra görüntüsü `docs/art/pm107/once_sonra/`.

## Sıra
A → B → C (en önemli) → D1–D2 → D3–D5 → D6 (zaman kaldıkça). Erken bitirme; liste bitince D6'dan devam et.

## Teslim
- EditMode + PlayMode + WebKit ses testi yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na kısa özet, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&lang=tr` (menü, alt çubuk, altın, koleksiyon)
  - `p23/?c=p23&test=1&lang=tr&lv=18&booster=saat` (saat + kayma miksajı)
  - `p23/?c=p23&test=1&lang=tr&lv=31` (zincir sesi)
