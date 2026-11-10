$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$dir='Assets\Art\Gameplay\Task9';New-Item -ItemType Directory -Force $dir|Out-Null
$fmt=[System.Drawing.Imaging.PixelFormat]::Format32bppArgb
function New-Bmp([int]$s){$b=[System.Drawing.Bitmap]::new($s,$s,$fmt);$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality;return @($b,$g)}
function Grad([int]$x,[int]$y,[int]$w,[int]$h,[string]$top,[string]$bottom){$r=[System.Drawing.Rectangle]::new($x,$y,$w,$h);return [System.Drawing.Drawing2D.LinearGradientBrush]::new($r,[System.Drawing.ColorTranslator]::FromHtml($top),[System.Drawing.ColorTranslator]::FromHtml($bottom),90)}
function Path-Round([float]$x,[float]$y,[float]$w,[float]$h,[float]$r){$p=[System.Drawing.Drawing2D.GraphicsPath]::new();$d=2*$r;$p.AddArc($x,$y,$d,$d,180,90);$p.AddArc($x+$w-$d,$y,$d,$d,270,90);$p.AddArc($x+$w-$d,$y+$h-$d,$d,$d,0,90);$p.AddArc($x,$y+$h-$d,$d,$d,90,90);$p.CloseFigure();return $p}
function Save-Bmp($b,[string]$name){$b.Save((Join-Path $dir $name),[System.Drawing.Imaging.ImageFormat]::Png)}
$navy=[System.Drawing.Color]::FromArgb(255,24,39,59)

# Plain raised button: broad toy-like orange rim and turquoise inset.
$b,$g=New-Bmp 264
$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(90,12,28,47));$g.FillEllipse($shadow,13,18,238,236);$shadow.Dispose()
$outer=Grad 8 5 248 248 '#FFD368' '#E78332';$g.FillEllipse($outer,8,5,248,248);$outer.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,222,142),5);$g.DrawEllipse($p,12,8,240,240);$p.Dispose()
$inner=Grad 27 24 210 210 '#65D9CF' '#168D9A';$g.FillEllipse($inner,27,24,210,210);$inner.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,14,80,95),6);$g.DrawEllipse($p,27,24,210,210);$p.Dispose()
$glint=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(90,255,255,245));$g.FillEllipse($glint,53,38,96,24);$glint.Dispose()
$g.Dispose();Save-Bmp $b 'booster-button.png';$b.Dispose()

# Child-safe scissors: chunky coral loops and broad soft silver blades.
$b,$g=New-Bmp 240
$blade1=[System.Drawing.Drawing2D.GraphicsPath]::new();$blade1.AddPolygon([System.Drawing.Point[]]@([System.Drawing.Point]::new(105,119),[System.Drawing.Point]::new(26,27),[System.Drawing.Point]::new(15,15),[System.Drawing.Point]::new(23,44),[System.Drawing.Point]::new(111,135)));$g.FillPath((Grad 15 15 100 125 '#F7F1DD' '#AFC7D0'),$blade1);$g.DrawPath([System.Drawing.Pen]::new($navy,12),$blade1);$blade1.Dispose()
$blade2=[System.Drawing.Drawing2D.GraphicsPath]::new();$blade2.AddPolygon([System.Drawing.Point[]]@([System.Drawing.Point]::new(116,119),[System.Drawing.Point]::new(204,25),[System.Drawing.Point]::new(223,14),[System.Drawing.Point]::new(213,44),[System.Drawing.Point]::new(109,137)));$g.FillPath((Grad 106 14 118 125 '#FFF5D8' '#B7C8CC'),$blade2);$g.DrawPath([System.Drawing.Pen]::new($navy,12),$blade2);$blade2.Dispose()
$coral=Grad 42 111 150 111 '#FF9B83' '#E64F5B'
$g.FillEllipse($coral,42,109,91,112);$g.FillEllipse($coral,107,109,91,112);$coral.Dispose()
$p=[System.Drawing.Pen]::new($navy,12);$g.DrawEllipse($p,42,109,91,112);$g.DrawEllipse($p,107,109,91,112);$p.Dispose()
$hole=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,24,39,59));$g.FillEllipse($hole,66,132,43,63);$g.FillEllipse($hole,131,132,43,63);$hole.Dispose()
$hub=Grad 90 91 60 60 '#FFE38A' '#F2A33F';$g.FillEllipse($hub,90,91,60,60);$p=[System.Drawing.Pen]::new($navy,10);$g.DrawEllipse($p,90,91,60,60);$p.Dispose();$hub.Dispose()
$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(190,255,255,240));$g.FillEllipse($hi,102,99,19,8);$hi.Dispose();$g.Dispose();Save-Bmp $b 'booster-scissors.png';$b.Dispose()

# Thick wand with oversized five-point star.
$b,$g=New-Bmp 240
$g.TranslateTransform(120,128);$g.RotateTransform(-38)
$handle=Path-Round -24 9 48 151 23;$hf=Grad -24 9 48 151 '#55D1C6' '#13828F';$g.FillPath($hf,$handle);$g.DrawPath([System.Drawing.Pen]::new($navy,12),$handle);$hf.Dispose();$handle.Dispose()
$gold=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,255,190,71));$g.FillEllipse($gold,-23,15,46,15);$g.FillEllipse($gold,-23,135,46,15);$gold.Dispose()
$g.ResetTransform()
$star=[System.Drawing.Drawing2D.GraphicsPath]::new();$pts=[System.Collections.Generic.List[System.Drawing.PointF]]::new();for($i=0;$i -lt 10;$i++){$a=(-90+$i*36)*[Math]::PI/180;$r=if($i%2 -eq 0){83}else{40};$pts.Add([System.Drawing.PointF]::new([float](125+[Math]::Cos($a)*$r),[float](78+[Math]::Sin($a)*$r)))};$star.AddPolygon($pts.ToArray());$sf=Grad 41 6 168 152 '#FFF27A' '#F4A42E';$g.FillPath($sf,$star);$g.DrawPath([System.Drawing.Pen]::new($navy,12),$star);$sf.Dispose();$star.Dispose()
$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(190,255,255,238));$g.FillEllipse($hi,101,35,39,15);$hi.Dispose();$g.Dispose();Save-Bmp $b 'booster-wand.png';$b.Dispose()

# Oversized toy mallet: big horizontal soft head and short rounded handle.
$b,$g=New-Bmp 240
$handle=Path-Round 100 115 45 112 22;$hf=Grad 100 115 45 112 '#F6C86C' '#E47A37';$g.FillPath($hf,$handle);$g.DrawPath([System.Drawing.Pen]::new($navy,13),$handle);$hf.Dispose();$handle.Dispose()
$grip=Grad 105 172 35 47 '#67D7CB' '#168895';$g.FillRectangle($grip,[System.Drawing.Rectangle]::new(105,172,35,47));$grip.Dispose()
$head=Path-Round 25 40 190 105 32;$headFill=Grad 26 40 188 105 '#FFD76D' '#E78733';$g.FillPath($headFill,$head);$g.DrawPath([System.Drawing.Pen]::new($navy,14),$head);$headFill.Dispose();$head.Dispose()
$face=Grad 29 55 43 75 '#FFE38A' '#F0A43D';$g.FillEllipse($face,29,55,43,75);$p=[System.Drawing.Pen]::new($navy,8);$p.Dispose();$face.Dispose()
$side=Grad 168 55 42 75 '#F4AB47' '#D96A31';$g.FillEllipse($side,168,55,42,75);$p=[System.Drawing.Pen]::new($navy,8);$p.Dispose();$side.Dispose()
$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(175,255,255,242));$g.FillEllipse($hi,74,48,83,17);$hi.Dispose();$g.Dispose();Save-Bmp $b 'booster-hammer.png';$b.Dispose()

# Round toy alarm clock with paired bells and a broad quiet dial.
$b,$g=New-Bmp 240
$feet=Grad 72 188 39 37 '#F8C75D' '#DE8034';$g.FillEllipse($feet,72,185,39,37);$g.FillEllipse($feet,132,185,39,37);$feet.Dispose();$p=[System.Drawing.Pen]::new($navy,11);$g.DrawEllipse($p,72,185,39,37);$g.DrawEllipse($p,132,185,39,37);$p.Dispose()
$bells=Grad 56 27 130 65 '#FFE27A' '#ED8B38';$g.FillEllipse($bells,54,28,65,54);$g.FillEllipse($bells,121,28,65,54);$bells.Dispose();$p=[System.Drawing.Pen]::new($navy,12);$g.DrawEllipse($p,54,28,65,54);$g.DrawEllipse($p,121,28,65,54);$p.Dispose()
$case=Grad 36 52 168 168 '#55D4C9' '#138491';$g.FillEllipse($case,36,49,168,168);$p=[System.Drawing.Pen]::new($navy,14);$g.DrawEllipse($p,36,49,168,168);$p.Dispose();$case.Dispose()
$rim=Grad 56 69 128 128 '#FFD871' '#E98636';$g.FillEllipse($rim,56,69,128,128);$rim.Dispose();$p=[System.Drawing.Pen]::new($navy,8);$g.DrawEllipse($p,56,69,128,128);$p.Dispose()
$dial=Grad 66 79 108 108 '#FFF4D9' '#E8D8B7';$g.FillEllipse($dial,66,79,108,108);$dial.Dispose()
$hand=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,24,39,59),10);$hand.StartCap='Round';$hand.EndCap='Round';$g.DrawLine($hand,120,132,120,94);$g.DrawLine($hand,120,132,147,143);$hand.Dispose()
$hub=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,239,125,52));$g.FillEllipse($hub,111,123,18,18);$hub.Dispose();$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(140,255,255,245));$g.FillEllipse($hi,78,86,52,11);$hi.Dispose();$g.Dispose();Save-Bmp $b 'booster-clock.png';$b.Dispose()

# Blank coral counter disk with one pale outline.
$b,$g=New-Bmp 96;$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(95,9,25,43));$g.FillEllipse($shadow,5,8,86,85);$shadow.Dispose();$fill=Grad 4 3 88 88 '#FF7769' '#D93649';$g.FillEllipse($fill,4,3,88,88);$fill.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,244,220),5);$g.DrawEllipse($p,6,5,84,84);$p.Dispose();$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(130,255,255,243));$g.FillEllipse($hi,23,14,36,10);$hi.Dispose();$g.Dispose();Save-Bmp $b 'booster-badge.png';$b.Dispose()

# Four rendered button compositions for quick review.
$preview=[System.Drawing.Bitmap]::new(1240,360,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[System.Drawing.Graphics]::FromImage($preview);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.Clear([System.Drawing.Color]::FromArgb(255,24,39,59))
$names=@('Scissors','Wand','Hammer','Clock');$icons=@('booster-scissors.png','booster-wand.png','booster-hammer.png','booster-clock.png');$font=[System.Drawing.Font]::new('Segoe UI',17,[System.Drawing.FontStyle]::Bold);$text=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,255,239,208))
for($i=0;$i -lt 4;$i++){$x=28+$i*306;$button=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-button.png'));$icon=[System.Drawing.Bitmap]::new((Join-Path $dir $icons[$i]));$badge=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-badge.png'));$g.DrawImage($button,$x,57,250,250);$g.DrawImage($icon,$x+14,62,228,228);$g.DrawImage($badge,$x+184,238,70,70);$g.DrawString($names[$i],$font,$text,$x+86,316);$button.Dispose();$icon.Dispose();$badge.Dispose()}
$g.Dispose();$font.Dispose();$text.Dispose();$preview.Save((Join-Path $dir 'preview.png'),[System.Drawing.Imaging.ImageFormat]::Png);$preview.Dispose()
Write-Output 'Task9 toy booster assets created.'


