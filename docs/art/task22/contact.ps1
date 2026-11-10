$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$base=(Get-Location).Path
$out=Join-Path $base 'docs/art/task22'
$groups=@{
 'ftue'=@(Get-ChildItem docs/qa/2026-11-02-ftue -Filter *.png | Sort-Object Name)
 'menu375'=@(Get-ChildItem docs/qa/2026-11-01-p20b -Filter 'tr_375*.png' | Sort-Object Name)+@(Get-ChildItem docs/qa/2026-11-02-son -Filter '375*.png')
 'menu390'=@(Get-ChildItem docs/qa/2026-11-01-p20b -Filter 'tr_390x844_won12*.png' | Sort-Object Name)+@(Get-ChildItem docs/qa/2026-11-02-son -Filter '390*.png')
 'spot'=@(Get-ChildItem docs/qa/2026-11-02-spot -Filter '375*.png' | Sort-Object Name)+@(Get-ChildItem docs/qa/2026-11-02-spot -Filter '390*.png' | Sort-Object Name)
 'extra'=@(Get-Item docs/qa/2026-10-26-p18/375x667_04_pause.png,docs/qa/2026-11-01-corridor/375x667_lv40_kart.png,docs/qa/2026-11-01-corridor/375x667_lv1_menu_b_orta.png,docs/qa/2026-10-28-paint40-live/375x667_win_2_kart.png)+@(Get-ChildItem docs/qa/2026-11-02-booster -Filter '*0a.png')
 'approved'=@(Get-ChildItem Assets/Art/Menu/Task21/Preview -Filter *.png)+@(Get-ChildItem Assets/Art/Menu/Task19D -Filter '0*.png')+@(Get-ChildItem 'Assets/Art/References/1.11 ref' -File | Where-Object {$_.Name -match 'IMG_24(37|46|50|51)\.'})
}
$font=New-Object System.Drawing.Font('Segoe UI',10)
foreach($key in $groups.Keys){
 $files=@($groups[$key]);$page=0
 for($start=0;$start -lt $files.Count;$start+=12){
  $items=@($files | Select-Object -Skip $start -First 12);$rows=[Math]::Ceiling($items.Count/4)
  $im=New-Object System.Drawing.Bitmap(800,([int]$rows*440));$g=[System.Drawing.Graphics]::FromImage($im);$g.Clear([System.Drawing.Color]::FromArgb(226,230,230));$g.InterpolationMode='HighQualityBicubic'
  for($i=0;$i -lt $items.Count;$i++){
   try {$src=[System.Drawing.Image]::FromFile($items[$i].FullName)} catch {Write-Output ("OKUNAMADI: "+$items[$i].FullName);continue};$x=($i%4)*200;$y=[Math]::Floor($i/4)*440
   $scale=[Math]::Min(196/$src.Width,406/$src.Height);$w=[int]($src.Width*$scale);$h=[int]($src.Height*$scale)
   $g.DrawImage($src,[int]($x+(200-$w)/2),[int]$y,$w,$h);$g.DrawString($items[$i].Name,$font,[System.Drawing.Brushes]::Black,[System.Drawing.RectangleF]::new($x+3,$y+407,195,32));$src.Dispose()
  }
  $im.Save((Join-Path $out ("contact-$key-$page.png")),[System.Drawing.Imaging.ImageFormat]::Png);$g.Dispose();$im.Dispose();$page++
 }
}
$font.Dispose()
