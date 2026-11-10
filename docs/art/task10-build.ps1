$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$dir='Assets\Art\Gameplay\Task10';New-Item -ItemType Directory -Force $dir|Out-Null
$gen='C:\Users\zeydt\.codex\generated_images\01a1695b-7c0a-7de3-a650-ac9e7c19776e'
$fmt=[System.Drawing.Imaging.PixelFormat]::Format32bppArgb
function Fit-Image([string]$srcPath,[string]$dstPath,[int]$size){
 $src=[System.Drawing.Bitmap]::new($srcPath);$left=$src.Width;$top=$src.Height;$right=-1;$bottom=-1
 for($y=0;$y -lt $src.Height;$y++){for($x=0;$x -lt $src.Width;$x++){$c=$src.GetPixel($x,$y);if($c.A -ge 32){if($x -lt $left){$left=$x};if($x -gt $right){$right=$x};if($y -lt $top){$top=$y};if($y -gt $bottom){$bottom=$y}}}}
 if($right -lt $left){throw "No visible subject: $srcPath"};$crop=[System.Drawing.Rectangle]::new($left,$top,$right-$left+1,$bottom-$top+1);$sc=[Math]::Min(($size-8)/[double]$crop.Width,($size-8)/[double]$crop.Height);$w=[int][Math]::Round($crop.Width*$sc);$h=[int][Math]::Round($crop.Height*$sc)
 $out=[System.Drawing.Bitmap]::new($size,$size,$fmt);$g=[System.Drawing.Graphics]::FromImage($out);$g.Clear([System.Drawing.Color]::Transparent);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality;$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality;$g.DrawImage($src,[System.Drawing.Rectangle]::new([int](($size-$w)/2),[int](($size-$h)/2),$w,$h),$crop,[System.Drawing.GraphicsUnit]::Pixel);$g.Dispose()
 for($y=0;$y -lt $size;$y++){for($x=0;$x -lt $size;$x++){$c=$out.GetPixel($x,$y);if($c.A -lt 16){$out.SetPixel($x,$y,[System.Drawing.Color]::Transparent)}}};$out.Save($dstPath,[System.Drawing.Imaging.ImageFormat]::Png);$out.Dispose();$src.Dispose()
}
Fit-Image (Join-Path $gen 'exec-d9bf7580-0606-4717-ba50-353d9276cb35.png') (Join-Path $dir 'booster-scissors.png') 240
Fit-Image (Join-Path $gen 'exec-595eed0c-193a-42b8-bbe2-5a29d1de282f.png') (Join-Path $dir 'booster-wand.png') 240
Fit-Image (Join-Path $gen 'exec-61edcdd6-5f0a-44c0-be07-4704cf11a349.png') (Join-Path $dir 'booster-hammer.png') 240
Fit-Image (Join-Path $gen 'exec-9c35062b-99be-404a-8392-69521c40a73c.png') (Join-Path $dir 'booster-clock.png') 240

# Reference-like circular button: warm two-tone rim, deep blue separator, teal radial face.
$button=[System.Drawing.Bitmap]::new(264,264,$fmt);$g=[System.Drawing.Graphics]::FromImage($button);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent)
$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(105,8,25,45));$g.FillEllipse($shadow,9,14,246,246);$shadow.Dispose()
$outer=[System.Drawing.Drawing2D.GraphicsPath]::new();$outer.AddEllipse(7,4,250,250);$pg=[System.Drawing.Drawing2D.PathGradientBrush]::new($outer);$pg.CenterColor=[System.Drawing.Color]::FromArgb(255,255,224,104);$pg.SurroundColors=@([System.Drawing.Color]::FromArgb(255,230,112,25));$g.FillPath($pg,$outer);$pg.Dispose();$outer.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,226,127),4);$g.DrawEllipse($p,10,7,244,244);$p.Dispose()
$inner=[System.Drawing.Drawing2D.GraphicsPath]::new();$inner.AddEllipse(29,27,206,206);$ipg=[System.Drawing.Drawing2D.PathGradientBrush]::new($inner);$ipg.CenterPoint=[System.Drawing.PointF]::new(116,93);$ipg.CenterColor=[System.Drawing.Color]::FromArgb(255,74,218,204);$ipg.SurroundColors=@([System.Drawing.Color]::FromArgb(255,18,115,137));$g.FillPath($ipg,$inner);$ipg.Dispose();$inner.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,19,51,79),7);$g.DrawEllipse($p,27,25,210,210);$p.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(125,255,255,240),5);$g.DrawArc($p,42,37,180,180,205,122);$p.Dispose()
$button.Save((Join-Path $dir 'booster-button.png'),[System.Drawing.Imaging.ImageFormat]::Png);$g.Dispose();$button.Dispose()

# Empty red count badge with thin pale rim.
$badge=[System.Drawing.Bitmap]::new(96,96,$fmt);$g=[System.Drawing.Graphics]::FromImage($badge);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent);$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(110,12,25,49));$g.FillEllipse($shadow,5,8,87,85);$shadow.Dispose()
$path=[System.Drawing.Drawing2D.GraphicsPath]::new();$path.AddEllipse(3,2,90,90);$pg=[System.Drawing.Drawing2D.PathGradientBrush]::new($path);$pg.CenterColor=[System.Drawing.Color]::FromArgb(255,255,98,83);$pg.SurroundColors=@([System.Drawing.Color]::FromArgb(255,204,27,49));$g.FillPath($pg,$path);$pg.Dispose();$path.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,246,228),4);$g.DrawEllipse($p,5,4,86,86);$p.Dispose();$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(150,255,255,245));$g.FillEllipse($hi,23,14,42,12);$hi.Dispose();$badge.Save((Join-Path $dir 'booster-badge.png'),[System.Drawing.Imaging.ImageFormat]::Png);$g.Dispose();$badge.Dispose()

# Composite four finished buttons with count badges for visual review.
$preview=[System.Drawing.Bitmap]::new(1240,340,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[System.Drawing.Graphics]::FromImage($preview);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality;$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.Clear([System.Drawing.Color]::FromArgb(255,22,57,82));$names=@('Scissors','Wand','Hammer','Clock');$icons=@('booster-scissors.png','booster-wand.png','booster-hammer.png','booster-clock.png');$font=[System.Drawing.Font]::new('Segoe UI',16,[System.Drawing.FontStyle]::Bold);$text=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,255,246,223))
for($i=0;$i -lt 4;$i++){$x=24+$i*304;$base=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-button.png'));$icon=[System.Drawing.Bitmap]::new((Join-Path $dir $icons[$i]));$badge=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-badge.png'));$g.DrawImage($base,$x,22,264,264);$g.DrawImage($icon,$x+12,25,240,240);$g.DrawImage($badge,$x+194,204,96,96);$g.DrawString($names[$i],$font,$text,$x+83,304);$base.Dispose();$icon.Dispose();$badge.Dispose()}
$g.Dispose();$font.Dispose();$text.Dispose();$preview.Save((Join-Path $dir 'preview.png'),[System.Drawing.Imaging.ImageFormat]::Png);$preview.Dispose();Write-Output 'Task10 exported.'
