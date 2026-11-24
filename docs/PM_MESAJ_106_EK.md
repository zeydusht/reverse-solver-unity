# PM_MESAJ_106_EK: Testler Zeyd'in Chrome'unda sürekli gizlilik sayfası ve mailto açıyor

**Sorun:** Zeyd'in bilgisayarında Chrome'da sürekli yeni "Reverse Solver — Gizlilik Politikası" sekmeleri açılıyor; daha önce de "mailto" uygulama seçme penceresi çıktı. Bunlar hafızayı dolduruyor (VS Code daha önce hafıza yetmediği için kapandı).

**Büyük ihtimalle sebep:** PlayMode / editör testlerindeki "bütün düğmelere bas" taraması (PM_98, 1088 dokunuş; PM_105) Ayarlar'daki **Gizlilik** ve **veri silme e-postası** düğmelerine de basıyor. Unity Editor'de `Application.OpenURL` sistemin varsayılan tarayıcısında gerçekten sekme / mail uygulaması açıyor. Başka bir sebep varsa bul ve raporla.

**Düzeltme:**
- Dış bağlantı açan bütün çağrılar tek bir yerden geçsin (`LinkAcici.Ac(url)`).
- Editörde, testlerde ve `&test=1`'de gerçekten açmasın, yalnız günlüğe yazsın ("açılacaktı: …"). Testler bu kaydı doğrulasın.
- Gerçek oyunda (iOS ve web, test bayrağı yokken) aynen açılsın.
- Ayrıca test sırasında kendi açtığın tarayıcı sekmelerini (Playwright vb.) iş bitince kapat.

**Teslim:** EditMode + PlayMode yeşil; testler çalışırken Zeyd'in Chrome'unda yeni sekme açılmadığını doğrula. Commit + push. PM_106'ya devam.
