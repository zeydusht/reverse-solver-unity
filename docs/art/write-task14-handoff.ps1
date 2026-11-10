$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$dir='Assets/Art/Gameplay/Task14'
$items=Get-Content "$dir/manifest.json" -Raw -Encoding UTF8 | ConvertFrom-Json
$lines=[Collections.Generic.List[string]]::new()
$lines.Add('<!-- TASK14_FINAL_EXPORT_BEGIN -->')
$lines.Add('## Task14 — final export tamamlandı (Zeyd onaylı)')
$lines.Add('')
$lines.Add('61 üretim PNG: @3x, gerçek alpha, yazısız/rakamsız. Border sırası sol/sağ/üst/alt; birim PNG pikseli. Pivot normalize Unity sol-alt başlangıçlıdır. Kontrol geçti; oyun entegrasyonu yapılmadı. Ayrıntı: `docs/art/TASK14_EXPORT.md`.')
$lines.Add('')
$lines.Add('| Dosya | Oyunda nereye | Boyut | 9-slice L/R/T/B | Pivot x/y |')
$lines.Add('|---|---|---|---|---|')
foreach($item in $items){
 $border=if(($item.sliceLTRB | Measure-Object -Sum).Sum -eq 0){'Yok'}else{$item.sliceLTRB -join '/'}
 $pv=($item.pivot | ForEach-Object {$_.ToString('0.00',[Globalization.CultureInfo]::InvariantCulture)}) -join '/'
 $lines.Add(('| `{0}/{1}` | {2} | {3}×{4} px | {5} | {6} |' -f $dir,$item.file,$item.use,$item.width,$item.height,$border,$pv))
}
foreach($name in @('preview-export','preview-booster-states','preview-cards','preview-seams','preview-ingame','preview-details')){
 $b=[Drawing.Bitmap]::new((Join-Path (Get-Location) "$dir/$name.png"))
 $lines.Add(('| `{0}/{1}.png` | İnceleme; runtime sprite değildir | {2}×{3} px | Uygulanmaz | Uygulanmaz |' -f $dir,$name,$b.Width,$b.Height))
 $b.Dispose()
}
$lines.Add('| `Assets/Art/Gameplay/Task14/LilitaOne-Regular.ttf` | TMP kaynak font; ı/ç/ö/ü var, ğ/ş/İ yok | TTF | Uygulanmaz | Uygulanmaz |')
$lines.Add('| `Assets/Art/Gameplay/Task14/LilitaOne-OFL.txt` | Font ile birlikte korunacak OFL 1.1 lisansı | Metin | Uygulanmaz | Uygulanmaz |')
$lines.Add('| `Assets/Art/Gameplay/Task14/manifest.json` | Dosya başına kullanım, ölçü, slice, pivot, @3x ve SHA-256 | 61 sprite kaydı | Metadata | Metadata |')
$lines.Add('| `Assets/Art/Gameplay/Task14/README.md` | Paket özeti ve entegrasyon bağlantıları | Markdown | Uygulanmaz | Uygulanmaz |')
$lines.Add('')
$lines.Add('Bomba gövde merkezi (192,192); etiket ve çivi yüzü iki hane için boştur. `zero` durumları 0 rakamı içermez. Birleşik booster ile ayrı zemin+ikon yolu alternatiflerdir; iki yol üst üste kullanılmaz. Kart tuşları ayrı `btn-*` dosyalarıdır. Kazanma yıldızlarını dinamik göstermek için ortak panel ve ayrı yıldızlar kullanılır.')
$lines.Add('')
$lines.Add('Zincir/duvar X tekrar kenarlarında ve duvar köşe bağlantılarında RGBA farkı: 0. Frame merkezi alpha=0. 252 korunan kaynakta hash farkı: 0. Font TTF kopyası birebir. `Source/` ham/önceki çalışma arşividir, final atlas teslimine dahil değildir. C# dosyası ve Assets/_Game değiştirilmedi. Font ayrıntısı: `docs/art/TASK14_FONT.md`; rapor: `docs/art/reports/task14-export-validation.json`.')
$lines.Add('<!-- TASK14_FINAL_EXPORT_END -->')
$path=Join-Path (Get-Location) 'docs/art/HANDOFF.md'
$text=[IO.File]::ReadAllText($path)
$block=$lines -join "`r`n"
if($text.Contains('<!-- TASK14_FINAL_EXPORT_BEGIN -->')){$text=[regex]::Replace($text,'(?s)<!-- TASK14_FINAL_EXPORT_BEGIN -->.*?<!-- TASK14_FINAL_EXPORT_END -->',$block)}else{$text=$text.TrimEnd()+"`r`n`r`n"+$block+"`r`n"}
[IO.File]::WriteAllText($path,$text,[Text.UTF8Encoding]::new($false))
Write-Output ('Task14 HANDOFF rows: '+($items.Count+10))
