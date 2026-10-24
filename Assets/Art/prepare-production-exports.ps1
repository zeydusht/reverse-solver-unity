Add-Type -AssemblyName System.Drawing
$art = Split-Path -Parent $MyInvocation.MyCommand.Path
function Get-ContentBounds([System.Drawing.Bitmap]$bmp) {
  $minX=$bmp.Width; $minY=$bmp.Height; $maxX=-1; $maxY=-1
  for($y=0;$y -lt $bmp.Height;$y++){for($x=0;$x -lt $bmp.Width;$x++){
    if($bmp.GetPixel($x,$y).A -gt 8){if($x -lt $minX){$minX=$x};if($x -gt $maxX){$maxX=$x};if($y -lt $minY){$minY=$y};if($y -gt $maxY){$maxY=$y}}
  }}
  if($maxX -lt 0){throw 'No visible pixels found.'}
  return [System.Drawing.Rectangle]::new($minX,$minY,$maxX-$minX+1,$maxY-$minY+1)
}
function Export-Fit([string]$source,[string]$destination,[int]$size=1024,[int]$inner=800) {
  $src=[System.Drawing.Bitmap]::FromFile($source); $box=Get-ContentBounds $src
  $out=[System.Drawing.Bitmap]::new($size,$size,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g=[System.Drawing.Graphics]::FromImage($out);$g.Clear([System.Drawing.Color]::Transparent);$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $scale=[math]::Min($inner/$box.Width,$inner/$box.Height);$w=[int]($box.Width*$scale);$h=[int]($box.Height*$scale);$dest=[System.Drawing.Rectangle]::new([int](($size-$w)/2),[int](($size-$h)/2),$w,$h)
  $g.DrawImage($src,$dest,$box,[System.Drawing.GraphicsUnit]::Pixel);$g.Dispose();$out.Save($destination,[System.Drawing.Imaging.ImageFormat]::Png);$out.Dispose();$src.Dispose()
}
foreach($family in @('Boosters','Obstacles')) {
  $folder=Join-Path $art $family; $exports=Join-Path $folder 'Exports';New-Item -ItemType Directory -Force -Path $exports|Out-Null
  Get-ChildItem $folder -File -Filter '*.png' | Where-Object Name -notlike 'reference-*' | ForEach-Object { Export-Fit $_.FullName (Join-Path $exports $_.Name) }
}
$iconSource=Join-Path $art 'Review\Ikon\2-market-mavi-v01.png';$iconDir=Join-Path $art 'Icon';$iconExports=Join-Path $iconDir 'Exports';New-Item -ItemType Directory -Force -Path $iconExports|Out-Null
Copy-Item -LiteralPath $iconSource -Destination (Join-Path $iconDir '2-market-mavi-approved-source.png') -Force
$icon=[System.Drawing.Bitmap]::FromFile($iconSource);foreach($size in @(1024,512,256,192,180,32)){$out=[System.Drawing.Bitmap]::new($size,$size,[System.Drawing.Imaging.PixelFormat]::Format24bppRgb);$g=[System.Drawing.Graphics]::FromImage($out);$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.DrawImage($icon,0,0,$size,$size);$g.Dispose();$out.Save((Join-Path $iconExports ("2-market-mavi-{0}.png" -f $size)),[System.Drawing.Imaging.ImageFormat]::Png);$out.Dispose()};$icon.Dispose()
$buttonDir=Join-Path $art 'Menu';$buttonExports=Join-Path $buttonDir 'Exports';New-Item -ItemType Directory -Force -Path $buttonExports|Out-Null
foreach($name in @('teal','orange')){$src=[System.Drawing.Bitmap]::FromFile((Join-Path $buttonDir ("level-button-{0}-v01.png" -f $name)));$box=Get-ContentBounds $src;$out=[System.Drawing.Bitmap]::new(1024,320,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[System.Drawing.Graphics]::FromImage($out);$g.Clear([System.Drawing.Color]::Transparent);$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$scale=[math]::Min(950/$box.Width,270/$box.Height);$w=[int]($box.Width*$scale);$h=[int]($box.Height*$scale);$g.DrawImage($src,[System.Drawing.Rectangle]::new([int]((1024-$w)/2),[int]((320-$h)/2),$w,$h),$box,[System.Drawing.GraphicsUnit]::Pixel);$g.Dispose();$out.Save((Join-Path $buttonExports ("level-button-{0}-1024x320.png" -f $name)),[System.Drawing.Imaging.ImageFormat]::Png);$src.Dispose();$out.Dispose()}
$anim=Join-Path $art 'AnimationDemo';$sprites=Join-Path $anim 'sprites';New-Item -ItemType Directory -Force -Path $sprites|Out-Null
function Save-Crop([string]$file,[int]$x,[int]$y,[int]$w,[int]$h,[string]$out) {
  $im=[System.Drawing.Bitmap]::FromFile($file);$crop=$im.Clone([System.Drawing.Rectangle]::new($x,$y,$w,$h),[System.Drawing.Imaging.PixelFormat]::Format32bppArgb);$crop.Save($out,[System.Drawing.Imaging.ImageFormat]::Png);$crop.Dispose();$im.Dispose()
}
$j=Join-Path $anim 'sources\jiggle-run-alpha-sheet-v01.png';$b=Join-Path $anim 'sources\baron-run-alpha-sheet-v01.png'
Save-Crop $j 332 14 560 741 (Join-Path $sprites 'jiggle-run-01.png');Save-Crop $j 1239 14 562 741 (Join-Path $sprites 'jiggle-run-02.png')
Save-Crop $b 292 8 548 795 (Join-Path $sprites 'baron-run-01.png');Save-Crop $b 1046 10 501 797 (Join-Path $sprites 'baron-run-02.png')
$menu=Join-Path $art 'Menu';$bg=[System.Drawing.Bitmap]::FromFile((Join-Path $menu 'corridor-v04-source.png'))
$canvas=[System.Drawing.Bitmap]::new(1024,1536,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[System.Drawing.Graphics]::FromImage($canvas);$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.DrawImage($bg,0,0,1024,1536)
$baronSprite=[System.Drawing.Bitmap]::FromFile((Join-Path $sprites 'baron-run-01.png'));$jiggleSprite=[System.Drawing.Bitmap]::FromFile((Join-Path $sprites 'jiggle-run-01.png'));$button=[System.Drawing.Bitmap]::FromFile((Join-Path $menu 'level-button-teal-v01.png'))
$g.DrawImage($baronSprite,[System.Drawing.Rectangle]::new(155,790,320,465));$g.DrawImage($jiggleSprite,[System.Drawing.Rectangle]::new(415,790,395,523));$g.DrawImage($button,[System.Drawing.Rectangle]::new(245,1327,534,181))
$font=[System.Drawing.Font]::new('Arial',42,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel);$brush=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::White);$sf=[System.Drawing.StringFormat]::new();$sf.Alignment=[System.Drawing.StringAlignment]::Center;$sf.LineAlignment=[System.Drawing.StringAlignment]::Center
$g.DrawString('LEVEL 12',$font,$brush,[System.Drawing.RectangleF]::new(270,1353,484,105),$sf)
$canvas.Save((Join-Path $menu 'opening-layout-v01.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$font.Dispose();$brush.Dispose();$sf.Dispose();$baronSprite.Dispose();$jiggleSprite.Dispose();$button.Dispose();$bg.Dispose();$g.Dispose();$canvas.Dispose()
Write-Output 'Created trimmed 1024px exports, four run sprites, and menu composition preview.'
