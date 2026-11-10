$ErrorActionPreference = 'Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll') | Out-Null
$root = (Get-Location).Path
$out = Join-Path $root 'Assets\Art\Gameplay\Task8'
New-Item -ItemType Directory -Force $out | Out-Null
$gen = 'C:\Users\zeydt\.codex\generated_images\01a1695b-7c0a-7de3-a650-ac9e7c19776e'

function Fit-Image([string]$source,[string]$target,[int]$width,[int]$height,[int]$cropX=0,[int]$cropY=0,[int]$cropW=0,[int]$cropH=0) {
  $src = [System.Drawing.Bitmap]::new($source)
  if ($cropW -le 0) { $cropW = $src.Width }
  if ($cropH -le 0) { $cropH = $src.Height }
  $bounds = [System.Drawing.Rectangle]::new($cropX,$cropY,$cropW,$cropH)
  $left=$cropW; $top=$cropH; $right=-1; $bottom=-1
  for($y=$cropY; $y -lt ($cropY+$cropH); $y++) { for($x=$cropX; $x -lt ($cropX+$cropW); $x++) {
    $c=$src.GetPixel($x,$y)
    if($c.A -ge 32) { if($x-$cropX -lt $left){$left=$x-$cropX};if($y-$cropY -lt $top){$top=$y-$cropY};if($x-$cropX -gt $right){$right=$x-$cropX};if($y-$cropY -gt $bottom){$bottom=$y-$cropY} }
  }}
  if($right -lt $left -or $bottom -lt $top){throw "No visible artwork in $source"}
  $trim = [System.Drawing.Rectangle]::new($cropX+$left,$cropY+$top,$right-$left+1,$bottom-$top+1)
  $scale=[Math]::Min(($width-12)/[double]$trim.Width,($height-12)/[double]$trim.Height)
  $dw=[int][Math]::Round($trim.Width*$scale);$dh=[int][Math]::Round($trim.Height*$scale)
  $bmp=[System.Drawing.Bitmap]::new($width,$height,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g=[System.Drawing.Graphics]::FromImage($bmp);$g.Clear([System.Drawing.Color]::Transparent)
  $g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality;$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $dest=[System.Drawing.Rectangle]::new([int](($width-$dw)/2),[int](($height-$dh)/2),$dw,$dh)
  $g.DrawImage($src,$dest,$trim,[System.Drawing.GraphicsUnit]::Pixel);$g.Dispose()
  for($y=0;$y -lt $height;$y++){for($x=0;$x -lt $width;$x++){$c=$bmp.GetPixel($x,$y);if($c.A -lt 24){$bmp.SetPixel($x,$y,[System.Drawing.Color]::Transparent)}}}
  $bmp.Save($target,[System.Drawing.Imaging.ImageFormat]::Png);$bmp.Dispose();$src.Dispose()
}

$files = @(
  @('exec-7f5997f5-77e0-4947-88a5-f6a397276173.png','booster-button.png',264,264),
  @('exec-35630d13-c5f6-4378-bf90-f56991b0b768.png','booster-scissors.png',240,240),
  @('exec-c5c32502-1d8a-4b3d-896b-633400bdd889.png','booster-wand.png',240,240),
  @('exec-d8d0ab6a-c398-498a-b2aa-81c3ca35e371.png','booster-hammer.png',240,240),
  @('exec-f31df4ae-2e2b-4b87-889c-5a2490fe6896.png','booster-clock.png',240,240),
  @('exec-693387c3-ad3d-4232-b398-eaa68414b087.png','booster-badge.png',96,96,1000,0,774,887),
  @('exec-1026ab7b-fb56-485b-a04f-1b3c2c22a267.png','hud-level-badge-v3.png',216,216),
  @('exec-318f1537-63e3-41e6-afaf-b4696d9d7723.png','hud-pill-v3.png',600,168)
)
foreach($f in $files) {
  $source=Join-Path $gen $f[0];$target=Join-Path $out $f[1]
  if($f.Count -ge 7){Fit-Image $source $target $f[2] $f[3] $f[4] $f[5] $f[6] $f[7]}
  else{Fit-Image $source $target $f[2] $f[3]}
}

# Contact sheet for review; this is a documentation preview, not an in-game asset.
$preview=[System.Drawing.Bitmap]::new(1260,920,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g=[System.Drawing.Graphics]::FromImage($preview);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::FromArgb(255,24,48,54))
$font=[System.Drawing.Font]::new('Segoe UI',14,[System.Drawing.FontStyle]::Bold)
$labelBrush=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,246,231,200))
$items=@(
 @('booster-button.png','Buton',264,264),@('booster-scissors.png','Makas',240,240),@('booster-wand.png','Asa',240,240),@('booster-hammer.png','Ã‡ekiÃ§',240,240),
 @('booster-clock.png','Saat',240,240),@('booster-badge.png','SayÄ± rozeti',96,96),@('hud-level-badge-v3.png','Level badge',216,216),@('hud-pill-v3.png','Ãœst bar hapÄ±',600,168)
)
for($i=0;$i -lt $items.Count;$i++){$col=$i%4;$row=[int][Math]::Floor($i/4);$x=15+$col*312;$y=15+$row*445;$g.DrawString($items[$i][1],$font,$labelBrush,$x,$y);$im=[System.Drawing.Bitmap]::new((Join-Path $out $items[$i][0]));$maxW=280;$maxH=365;$sc=[Math]::Min($maxW/[double]$im.Width,$maxH/[double]$im.Height);$w=[int]($im.Width*$sc);$h=[int]($im.Height*$sc);$g.DrawImage($im,$x+[int](($maxW-$w)/2),$y+32+$maxH-$h,$w,$h);$im.Dispose()}
$g.Dispose();$font.Dispose();$labelBrush.Dispose();$preview.Save((Join-Path $root 'docs\art\Task8-review.png'),[System.Drawing.Imaging.ImageFormat]::Png);$preview.Dispose()
Write-Output 'Task8 exports and review sheet created.'


