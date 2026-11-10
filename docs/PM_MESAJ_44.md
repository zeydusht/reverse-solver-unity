# PM → Claude Code (3 Kasım 23:58): WhatsApp link önizlemesi (p20, yalnız HTML, 10 dk)

**İlk iş:** commit et. PM dosyalarını asla geri alma. **Unity build YOK, oyun dosyalarına dokunma.** Bu iş yalnız gh-pages'teki `p20/index.html` ve bir görsel.

## Ne yapılacak
WhatsApp'ta link önizlemesinde eski mavi yapboz ikonu çıkıyor, açıklama yok.

1. `docs/web/og-image.jpg` dosyasını gh-pages'te `p20/og-image.jpg` olarak koy. 1200×630, PM hazırladı.
2. `p20/index.html` `<head>` içine ekle (varsa eskisini değiştir):
```html
<meta property="og:title" content="Reverse Solver">
<meta property="og:description" content="Parçaları çıkar, tabloyu kurtar! Telefondan tarayıcıda oyna, indirme yok.">
<meta property="og:image" content="https://zeydusht.github.io/reverse-solver-unity/p20/og-image.jpg">
<meta property="og:image:width" content="1200">
<meta property="og:image:height" content="630">
<meta property="og:type" content="website">
<meta property="og:url" content="https://zeydusht.github.io/reverse-solver-unity/p20/">
<meta name="twitter:card" content="summary_large_image">
<meta name="description" content="Parçaları çıkar, tabloyu kurtar! Telefondan tarayıcıda oyna, indirme yok.">
```
3. Push et. `curl -s https://zeydusht.github.io/reverse-solver-unity/p20/ | grep og:` ile canlıda gör.
4. OVERNIGHT başına 1 satır. Zeyd'e "bitti" de. 10 dakikayı geçmesin.
