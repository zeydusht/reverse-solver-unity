$ErrorActionPreference = 'Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll') | Out-Null
$outDir='Assets\Art\Gameplay\Task7'
New-Item -ItemType Directory -Force $outDir | Out-Null
$fmt=[System.Drawing.Imaging.PixelFormat]::Format32bppArgb
$hq=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

function New-Bitmap([int]$w,[int]$h){ return [System.Drawing.Bitmap]::new($w,$h,$fmt) }
function Rounded([float]$x,[float]$y,[float]$w,[float]$h,[float]$r){
  $p=[System.Drawing.Drawing2D.GraphicsPath]::new(); $d=$r*2
  $p.AddArc($x,$y,$d,$d,180,90); $p.AddArc(($x+$w-$d),$y,$d,$d,270,90); $p.AddArc(($x+$w-$d),($y+$h-$d),$d,$d,0,90); $p.AddArc($x,($y+$h-$d),$d,$d,90,90); $p.CloseFigure(); return $p
}
function Save-Png($bmp,[string]$name){ $bmp.Save((Join-Path $outDir $name),[System.Drawing.Imaging.ImageFormat]::Png) }
function Gradient([System.Drawing.Rectangle]$r,[System.Drawing.Color]$a,[System.Drawing.Color]$b,[float]$angle=90){ return [System.Drawing.Drawing2D.LinearGradientBrush]::new($r,$a,$b,$angle) }

$sourceBomb=[System.Drawing.Bitmap]::new('Assets\Art\Gameplay\Task6\bomb-counter-384.png')
$bomb=New-Bitmap 384 384; $g=[System.Drawing.Graphics]::FromImage($bomb); $g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; $g.InterpolationMode=$hq
$g.DrawImage($sourceBomb,[System.Drawing.Rectangle]::new(0,-40,384,384),[System.Drawing.Rectangle]::new(0,0,384,384),[System.Drawing.GraphicsUnit]::Pixel)
$upper=New-Bitmap 384 150
for($y=0;$y -lt 150;$y++){for($x=0;$x -lt 384;$x++){$c=$sourceBomb.GetPixel($x,$y);if($c.A -eq 0){continue};$warm=(($c.R -gt ($c.G+8) -and $c.G -gt ($c.B+2)) -or ($c.R -gt 205 -and $c.G -gt 175 -and $c.B -gt 135));if($warm){$upper.SetPixel($x,$y,$c)}}}
$g.DrawImageUnscaled($upper,0,0)
$label=[System.Drawing.Rectangle]::new(102,102,180,180); $labelBrush=Gradient $label ([System.Drawing.Color]::FromArgb(255,255,247,223)) ([System.Drawing.Color]::FromArgb(255,242,229,199)) 55; $g.FillEllipse($labelBrush,$label); $labelBrush.Dispose()
$g.Dispose(); Save-Png $bomb 'bomb-v2-384.png'; $sourceBomb.Dispose();$upper.Dispose();$bomb.Dispose()

$sourceNail=[System.Drawing.Bitmap]::new('Assets\Art\Gameplay\Task6\nail-head-384.png'); $nail=New-Bitmap 384 384; $g=[System.Drawing.Graphics]::FromImage($nail);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.InterpolationMode=$hq;$g.DrawImageUnscaled($sourceNail,0,0)
$nailDisk=[System.Drawing.Rectangle]::new(82,82,220,220);$nailFill=Gradient $nailDisk ([System.Drawing.Color]::FromArgb(255,255,242,195)) ([System.Drawing.Color]::FromArgb(255,238,215,151)) 50;$g.FillEllipse($nailFill,$nailDisk);$nailFill.Dispose();$g.Dispose();Save-Png $nail 'nail-head-v2-384.png';$sourceNail.Dispose();$nail.Dispose()

$pattern=New-Bitmap 48 48
for($y=0;$y -lt 48;$y++){for($x=0;$x -lt 48;$x++){$q=($x+$y)%48;if($q -lt 24){$pattern.SetPixel($x,$y,[System.Drawing.Color]::FromArgb(255,244,238,218))}else{$pattern.SetPixel($x,$y,[System.Drawing.Color]::FromArgb(255,205,48,42))}}}
$wall=New-Bitmap 96 96;$g=[System.Drawing.Graphics]::FromImage($wall);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent)
$band=[System.Drawing.Rectangle]::new(0,12,96,72);$g.FillRectangle([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,92,27,29)),0,10,96,76);$tb=[System.Drawing.TextureBrush]::new($pattern);$g.FillRectangle($tb,$band);$g.DrawLine([System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,112,32,31),3),0,12,96,12);$g.DrawLine([System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,112,32,31),3),0,84,96,84);$g.Dispose();Save-Png $wall 'wall-repeat-v2-96x96.png';$wall.Dispose()
$corner=New-Bitmap 96 96;$g=[System.Drawing.Graphics]::FromImage($corner);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent);$lp=[System.Drawing.Drawing2D.GraphicsPath]::new();$lp.AddRectangle([System.Drawing.Rectangle]::new(0,0,96,32));$lp.AddRectangle([System.Drawing.Rectangle]::new(0,0,32,96));$g.FillPath([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,92,27,29)),$lp);$tb=[System.Drawing.TextureBrush]::new($pattern);$g.FillPath($tb,$lp);$outline=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,112,32,31),3);$g.DrawPath($outline,$lp);$g.Dispose();Save-Png $corner 'wall-corner-v2-96x96.png';$corner.Dispose();$lp.Dispose();$outline.Dispose()

$frameSource=[System.Drawing.Bitmap]::new('C:\Users\zeydt\.codex\generated_images\01a1695b-7c0a-7de3-a650-ac9e7c19776e\exec-f4b2fc05-cfca-4d62-b4aa-8a482e59c64b.png')
$frame=New-Bitmap 1125 1125;$g=[System.Drawing.Graphics]::FromImage($frame);$g.InterpolationMode=$hq;$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality;$g.DrawImage($frameSource,[System.Drawing.Rectangle]::new(0,0,1125,1125),[System.Drawing.Rectangle]::new(0,0,$frameSource.Width,$frameSource.Height),[System.Drawing.GraphicsUnit]::Pixel)
$g.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy;$g.FillRectangle([System.Drawing.SolidBrush]::new([System.Drawing.Color]::Transparent),205,220,715,690);$g.Dispose();Save-Png $frame 'board-frame-gold-v1.png';$frame.Dispose();$frameSource.Dispose()

function Draw-CircleButton([int]$size,[string]$name,[bool]$pressed){
  $b=New-Bitmap $size $size;$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $shift=0;if($pressed){$shift=3}
  $shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(95,8,27,35));$g.FillEllipse($shadow,6,9+$shift,$size-12,$size-14);$shadow.Dispose()
  $outer=Gradient ([System.Drawing.Rectangle]::new(4,2+$shift,$size-8,$size-8)) ([System.Drawing.Color]::FromArgb(255,255,222,128)) ([System.Drawing.Color]::FromArgb(255,172,105,35)) 45;$g.FillEllipse($outer,4,2+$shift,$size-8,$size-8);$outer.Dispose()
  $rim=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,240,179),2);$g.DrawEllipse($rim,7,5+$shift,$size-14,$size-14);$rim.Dispose()
  $bodyRect=[System.Drawing.Rectangle]::new(14,12+$shift,$size-28,$size-28);if($pressed){$c1=[System.Drawing.Color]::FromArgb(255,18,133,143);$c2=[System.Drawing.Color]::FromArgb(255,9,76,87)}else{$c1=[System.Drawing.Color]::FromArgb(255,55,203,193);$c2=[System.Drawing.Color]::FromArgb(255,15,111,123)};$body=Gradient $bodyRect $c1 $c2 90;$g.FillEllipse($body,$bodyRect);$body.Dispose()
  $stroke=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,104,224,213),3);$g.DrawEllipse($stroke,18,16+$shift,$size-36,$size-36);$stroke.Dispose()
  $barRect1=[System.Drawing.Rectangle]::new([int]($size*.34),[int]($size*.32)+$shift,[int]($size*.12),[int]($size*.37));$barRect2=[System.Drawing.Rectangle]::new([int]($size*.54),[int]($size*.32)+$shift,[int]($size*.12),[int]($size*.37))
  foreach($r in @($barRect1,$barRect2)){$p=Rounded $r.X $r.Y $r.Width $r.Height 5;$sp=$p.Clone();$sm=[System.Drawing.Drawing2D.Matrix]::new();$sm.Translate(2,2);$sp.Transform($sm);$g.FillPath([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(130,1,34,41)),$sp);$g.FillPath([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,255,245,218)),$p);$sm.Dispose();$sp.Dispose();$p.Dispose()}
  $g.Dispose();Save-Png $b $name;$b.Dispose()
}
Draw-CircleButton 168 'hud-pause-normal.png' $false
Draw-CircleButton 168 'hud-pause-pressed.png' $true

$badge=New-Bitmap 216 216;$g=[System.Drawing.Graphics]::FromImage($badge);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(95,7,28,34));$g.FillEllipse($shadow,9,12,198,198);$shadow.Dispose();$rim=Gradient ([System.Drawing.Rectangle]::new(5,3,206,206)) ([System.Drawing.Color]::FromArgb(255,255,222,124)) ([System.Drawing.Color]::FromArgb(255,168,93,29)) 45;$g.FillEllipse($rim,5,3,206,206);$rim.Dispose()
$badgeFace=Gradient ([System.Drawing.Rectangle]::new(14,12,188,188)) ([System.Drawing.Color]::FromArgb(255,60,207,194)) ([System.Drawing.Color]::FromArgb(255,11,91,105)) 90;$g.FillEllipse($badgeFace,14,12,188,188);$badgeFace.Dispose();$ring=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,122,229,215),3);$g.DrawEllipse($ring,20,18,176,176);$ring.Dispose()
$lvSlot=Rounded 65 34 86 33 15;$slot=Gradient ([System.Drawing.Rectangle]::new(65,34,86,33)) ([System.Drawing.Color]::FromArgb(255,255,224,143)) ([System.Drawing.Color]::FromArgb(255,191,126,43)) 90;$g.FillPath($slot,$lvSlot);$slot.Dispose();$lvSlot.Dispose()
$num=[System.Drawing.Rectangle]::new(55,77,106,91);$numFill=Gradient $num ([System.Drawing.Color]::FromArgb(255,255,247,221)) ([System.Drawing.Color]::FromArgb(255,234,218,178)) 90;$g.FillEllipse($numFill,$num);$numFill.Dispose();$g.DrawEllipse([System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,223,174,80),3),$num)
$g.Dispose();Save-Png $badge 'hud-level-badge.png';$badge.Dispose()

function Draw-Pill([string]$name,[bool]$urgent){
 $b=New-Bitmap 600 168;$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
 $p=Rounded 3 3 594 158 47;$shadow=$p.Clone();$m=[System.Drawing.Drawing2D.Matrix]::new();$m.Translate(0,6);$shadow.Transform($m);$g.FillPath([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(105,5,24,31)),$shadow);$m.Dispose();$shadow.Dispose()
 $gold=Gradient ([System.Drawing.Rectangle]::new(3,3,594,158)) ([System.Drawing.Color]::FromArgb(255,255,226,139)) ([System.Drawing.Color]::FromArgb(255,174,100,34)) 90;$g.FillPath($gold,$p);$gold.Dispose()
 $inner=Rounded 10 10 580 143 42
 if($urgent){$c1=[System.Drawing.Color]::FromArgb(255,244,131,111);$c2=[System.Drawing.Color]::FromArgb(255,142,37,53)}else{$c1=[System.Drawing.Color]::FromArgb(255,68,205,190);$c2=[System.Drawing.Color]::FromArgb(255,12,87,103)}
 $body=Gradient ([System.Drawing.Rectangle]::new(10,10,580,143)) $c1 $c2 90;$g.FillPath($body,$inner);$body.Dispose();$line=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(210,185,248,231),2);$g.DrawPath($line,$inner);$line.Dispose()
 $well=[System.Drawing.Rectangle]::new(24,22,124,120);$wellFill=Gradient $well ([System.Drawing.Color]::FromArgb(255,30,154,159)) ([System.Drawing.Color]::FromArgb(255,8,57,71)) 90;if($urgent){$wellFill.Dispose();$wellFill=Gradient $well ([System.Drawing.Color]::FromArgb(255,182,65,66)) ([System.Drawing.Color]::FromArgb(255,89,22,39)) 90};$g.FillEllipse($wellFill,$well);$wellFill.Dispose();$rim=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,211,115),6);$g.DrawEllipse($rim,25,22,122,122);$rim.Dispose();$wellHi=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(180,189,255,235),2);$g.DrawEllipse($wellHi,32,29,108,108);$wellHi.Dispose()
 $g.Dispose();$p.Dispose();$inner.Dispose();Save-Png $b $name;$b.Dispose()
}
Draw-Pill 'hud-pill.png' $false
Draw-Pill 'hud-pill-urgent.png' $true

$icon=New-Bitmap 96 96;$g=[System.Drawing.Graphics]::FromImage($icon);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$piece=[System.Drawing.Drawing2D.GraphicsPath]::new();$piece.StartFigure();$piece.AddLine(14,18,35,18);$piece.AddBezier([System.Drawing.PointF]::new(35,18),[System.Drawing.PointF]::new(35,8),[System.Drawing.PointF]::new(42,7),[System.Drawing.PointF]::new(48,7));$piece.AddBezier([System.Drawing.PointF]::new(48,7),[System.Drawing.PointF]::new(54,7),[System.Drawing.PointF]::new(61,8),[System.Drawing.PointF]::new(61,18));$piece.AddLine(61,18,82,18);$piece.AddLine(82,18,82,35);$piece.AddBezier([System.Drawing.PointF]::new(82,35),[System.Drawing.PointF]::new(72,35),[System.Drawing.PointF]::new(71,42),[System.Drawing.PointF]::new(71,48));$piece.AddBezier([System.Drawing.PointF]::new(71,48),[System.Drawing.PointF]::new(71,54),[System.Drawing.PointF]::new(72,61),[System.Drawing.PointF]::new(82,61));$piece.AddLine(82,61,82,82);$piece.AddLine(82,82,14,82);$piece.CloseFigure()
$pShadow=$piece.Clone();$m=[System.Drawing.Drawing2D.Matrix]::new();$m.Translate(0,3);$pShadow.Transform($m);$g.FillPath([System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(120,4,28,37)),$pShadow);$m.Dispose();$pShadow.Dispose();$pieceFill=Gradient ([System.Drawing.Rectangle]::new(14,7,68,75)) ([System.Drawing.Color]::FromArgb(255,90,224,204)) ([System.Drawing.Color]::FromArgb(255,15,93,107)) 90;$g.FillPath($pieceFill,$piece);$pieceFill.Dispose();$outer=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,13,46,54),7);$g.DrawPath($outer,$piece);$outer.Dispose();$goldEdge=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,245,193,86),3);$g.DrawPath($goldEdge,$piece);$goldEdge.Dispose();$g.Dispose();Save-Png $icon 'hud-icon-piece.png';$piece.Dispose();$icon.Dispose()

$clock=New-Bitmap 96 96;$g=[System.Drawing.Graphics]::FromImage($clock);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(100,5,29,36));$g.FillEllipse($shadow,11,13,76,76);$shadow.Dispose();$gold=Gradient ([System.Drawing.Rectangle]::new(8,6,80,80)) ([System.Drawing.Color]::FromArgb(255,255,225,129)) ([System.Drawing.Color]::FromArgb(255,170,98,33)) 45;$g.FillEllipse($gold,8,5,80,80);$gold.Dispose();$face=Gradient ([System.Drawing.Rectangle]::new(15,12,66,66)) ([System.Drawing.Color]::FromArgb(255,252,247,220)) ([System.Drawing.Color]::FromArgb(255,224,207,161)) 90;$g.FillEllipse($face,15,12,66,66);$face.Dispose();$rim=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,255,235),2);$g.DrawEllipse($rim,19,16,58,58);$rim.Dispose();$hand=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,15,85,95),6);$hand.StartCap=[System.Drawing.Drawing2D.LineCap]::Round;$hand.EndCap=[System.Drawing.Drawing2D.LineCap]::Round;$g.DrawLine($hand,48,45,48,27);$g.DrawLine($hand,48,45,62,52);$hand.Dispose();$hub=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,223,155,53));$g.FillEllipse($hub,42,39,12,12);$hub.Dispose();$g.Dispose();Save-Png $clock 'hud-icon-clock.png';$clock.Dispose()

$frameSource.Dispose()
Write-Output 'Task7 art exported.'


