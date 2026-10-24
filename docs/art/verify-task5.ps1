$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$manifest=Get-Content -Raw -Encoding utf8 (Join-Path $root 'Assets/Art/UI/Task5/manifest.json') | ConvertFrom-Json
$sheet=[Drawing.Bitmap]::new(1200,([int][Math]::Ceiling($manifest.assets.Count/6))*150)
$g=[Drawing.Graphics]::FromImage($sheet)
$g.Clear([Drawing.ColorTranslator]::FromHtml('#28474c'))
$g.InterpolationMode=[Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$font=[Drawing.Font]::new('Arial',9)
$i=0;$issues=@();$bytes=0
foreach($a in $manifest.assets){
  $file=Join-Path $root $a.path
  $im=[Drawing.Bitmap]::FromFile($file)
  if($im.Width -ne $a.width -or $im.Height -ne $a.height){$issues+='Dimension: '+$a.name}
  $bytes+=(Get-Item -LiteralPath $file).Length
  $hasAlpha=$false
  for($x=0;$x -lt $im.Width;$x++){if($im.GetPixel($x,0).A -lt 255 -or $im.GetPixel($x,$im.Height-1).A -lt 255){$hasAlpha=$true;break}}
  if(-not $hasAlpha -and $a.id -ne 26){$issues+='Alpha: '+$a.name}
  if($a.slice){if(($a.slice[0]+$a.slice[2]) -ge $im.Width -or ($a.slice[1]+$a.slice[3]) -ge $im.Height){$issues+='Slice center: '+$a.name}}
  if($a.tile -eq 'x'){for($y=0;$y -lt $im.Height;$y++){if($im.GetPixel(0,$y).ToArgb() -ne $im.GetPixel($im.Width-1,$y).ToArgb()){$issues+='Tile seam: '+$a.name;break}}}
  $x0=($i%6)*200;$y0=[Math]::Floor($i/6)*150
  $scale=[Math]::Min(180/$im.Width,110/$im.Height)
  $g.DrawImage($im,[Drawing.RectangleF]::new([single]($x0+10),[single]($y0+5),[single]($im.Width*$scale),[single]($im.Height*$scale)))
  $g.DrawString($a.name,$font,[Drawing.Brushes]::White,[single]($x0+6),[single]($y0+118))
  $im.Dispose();$i++
}
$g.Dispose();$font.Dispose()
$sheet.Save((Join-Path $root 'docs/art/reports/task5-contact-sheet.png'),[Drawing.Imaging.ImageFormat]::Png);$sheet.Dispose()
$report=[ordered]@{pngCount=$i;totalBytes=$bytes;checks=@('PNG decode','inventory dimensions','transparent border (except full-screen background)','positive 9-slice center','pixel-identical horizontal tile edges');issues=$issues}
$report | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 (Join-Path $root 'docs/art/reports/task5-validation.json')
$report | ConvertTo-Json -Depth 5
if($issues.Count){exit 1}
