$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$dir='Assets\Art\Gameplay\Task11';New-Item -ItemType Directory -Force $dir|Out-Null
$fmt=[System.Drawing.Imaging.PixelFormat]::Format32bppArgb
$navy=[System.Drawing.Color]::FromArgb(255,24,49,70)
function New-Canvas([int]$w,[int]$h){$b=[System.Drawing.Bitmap]::new($w,$h,$fmt);$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality;return @($b,$g)}
function Hex([string]$s){return [System.Drawing.ColorTranslator]::FromHtml($s)}
function Grad([int]$x,[int]$y,[int]$w,[int]$h,[string]$a,[string]$b,[float]$angle=90){return [System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Rectangle]::new($x,$y,$w,$h),(Hex $a),(Hex $b),$angle)}
function RoundPath([float]$x,[float]$y,[float]$w,[float]$h,[float]$r){$p=[System.Drawing.Drawing2D.GraphicsPath]::new();$d=2*$r;$p.AddArc($x,$y,$d,$d,180,90);$p.AddArc($x+$w-$d,$y,$d,$d,270,90);$p.AddArc($x+$w-$d,$y+$h-$d,$d,$d,0,90);$p.AddArc($x,$y+$h-$d,$d,$d,90,90);$p.CloseFigure();return $p}
function Save-Png($b,[string]$n){$b.Save((Join-Path $dir $n),[System.Drawing.Imaging.ImageFormat]::Png)}
function Solid([int]$a,[string]$c){$co=Hex $c;return [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb($a,$co.R,$co.G,$co.B))}
function Soft-Spot($g,[int]$x,[int]$y,[int]$w,[int]$h,[int]$alpha){$p=[System.Drawing.Drawing2D.GraphicsPath]::new();$p.AddEllipse($x,$y,$w,$h);$f=[System.Drawing.Drawing2D.PathGradientBrush]::new($p);$f.CenterColor=[System.Drawing.Color]::FromArgb($alpha,255,255,255);$f.SurroundColors=@([System.Drawing.Color]::FromArgb(0,255,255,255));$g.FillPath($f,$p);$f.Dispose();$p.Dispose()}

function Draw-Pill([string]$name,[bool]$urgent){
 $b,$g=New-Canvas 600 168;$g.Clear([System.Drawing.Color]::Transparent)
 $sh=Solid 100 '#163146';$p=RoundPath 8 12 584 148 58;$g.FillPath($sh,$p);$sh.Dispose();$p.Dispose()
 $outer=RoundPath 5 3 590 151 62;$gold=Grad 5 3 590 151 '#FFE07B' '#E88832';$g.FillPath($gold,$outer);$gold.Dispose()
 $inner=RoundPath 17 14 566 128 52;if($urgent){$a='#FA8E78';$z='#BC3C53'}else{$a='#70E2D2';$z='#087E93'};$face=Grad 17 14 566 128 $a $z 90;$g.FillPath($face,$inner);$face.Dispose()
 $outline=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(170,20,62,82),4);$g.DrawPath($outline,$inner);$outline.Dispose()
 $g.SetClip($inner);$shade=Solid 34 '#073A5A';$g.FillEllipse($shade,25,112,550,42);$shade.Dispose();Soft-Spot $g 54 16 450 72 70;$g.ResetClip()
 $p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(180,255,246,208),3);$g.DrawArc($p,25,10,550,142,205,135);$p.Dispose()
 $g.Dispose();$outer.Dispose();$inner.Dispose();Save-Png $b $name;$b.Dispose()
}
Draw-Pill 'hud-pill.png' $false;Draw-Pill 'hud-pill-urgent.png' $true

# Level badge with two blank writing areas.
$b,$g=New-Canvas 216 216;$g.Clear([System.Drawing.Color]::Transparent);$shadow=Solid 90 '#172B45';$g.FillEllipse($shadow,10,14,198,198);$shadow.Dispose()
$rim=Grad 5 3 206 206 '#FFE47D' '#DC762E';$g.FillEllipse($rim,5,3,206,206);$rim.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,238,164),5);$g.DrawEllipse($p,9,7,198,198);$p.Dispose()
$face=Grad 21 19 174 174 '#62D8CA' '#168B9A';$g.FillEllipse($face,21,19,174,174);$face.Dispose()
$lv=RoundPath 63 31 90 39 18;$lvf=Grad 63 31 90 39 '#FFF1BE' '#F0C45E';$g.FillPath($lvf,$lv);$g.DrawPath([System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(210,206,124,44),3),$lv);$lvf.Dispose();$lv.Dispose()
$num=[System.Drawing.Rectangle]::new(47,77,122,99);$nf=Grad 47 77 122 99 '#FFF8E7' '#EBDDBF';$g.FillEllipse($nf,$num);$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,221,133),5);$g.DrawEllipse($p,$num);$p.Dispose();$nf.Dispose()
$hi=Solid 90 '#FFFFFF';$g.FillEllipse($hi,42,24,71,17);$hi.Dispose();$g.Dispose();Save-Png $b 'hud-level-badge.png';$b.Dispose()

# HUD piece glyph: rounded puzzle tile with a single raised knob and soft gold edge.
$b,$g=New-Canvas 120 120;$g.Clear([System.Drawing.Color]::Transparent);$path=[System.Drawing.Drawing2D.GraphicsPath]::new();$path.StartFigure();$path.AddLine(23,24,45,24);$path.AddBezier(45,24,44,8,53,6,60,6);$path.AddBezier(60,6,69,6,77,10,75,24);$path.AddLine(75,24,97,24);$path.AddLine(97,24,97,47);$path.AddBezier(97,47,111,45,115,53,115,60);$path.AddBezier(115,60,115,68,111,75,97,73);$path.AddLine(97,73,97,97);$path.AddLine(97,97,23,97);$path.CloseFigure();$sh=Solid 90 '#142B45';$g.FillPath($sh,$path);$sh.Dispose();$m=[System.Drawing.Drawing2D.Matrix]::new();$m.Translate(0,4);$path.Transform($m);$m.Dispose();$f=Grad 20 7 97 96 '#76E5D2' '#11879A';$g.FillPath($f,$path);$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,231,177,63),5);$g.DrawPath($p,$path);$p.Dispose();$f.Dispose();$path.Dispose();$hi=Solid 110 '#FFFFFF';$g.FillEllipse($hi,36,31,35,9);$hi.Dispose();$g.Dispose();Save-Png $b 'hud-icon-piece.png';$b.Dispose()

# HUD clock glyph.
$b,$g=New-Canvas 120 120;$g.Clear([System.Drawing.Color]::Transparent);$sh=Solid 95 '#142B45';$g.FillEllipse($sh,17,20,89,88);$sh.Dispose();$gold=Grad 14 10 92 92 '#FFE37A' '#E78632';$g.FillEllipse($gold,14,8,92,92);$gold.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,24,49,70),5);$g.DrawEllipse($p,14,8,92,92);$p.Dispose();$dial=Grad 26 20 68 68 '#FFFCED' '#E8D9B7';$g.FillEllipse($dial,26,20,68,68);$dial.Dispose();$hand=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,24,69,86),7);$hand.StartCap='Round';$hand.EndCap='Round';$g.DrawLine($hand,60,55,60,34);$g.DrawLine($hand,60,55,76,63);$hand.Dispose();$hub=Solid 255 '#ED8E39';$g.FillEllipse($hub,55,50,10,10);$hub.Dispose();$hi=Solid 120 '#FFFFFF';$g.FillEllipse($hi,30,18,34,8);$hi.Dispose();$g.Dispose();Save-Png $b 'hud-icon-clock.png';$b.Dispose()

function Draw-Button([string]$name,[string]$top,[string]$bottom,[string]$pressedTop,[string]$pressedBottom,[bool]$pressed,[int]$height=168,[int]$width=600){
 $b,$g=New-Canvas $width $height;$g.Clear([System.Drawing.Color]::Transparent);$shift=0;if($pressed){$shift=7}
 $shadow=Solid 105 '#172D48';$sp=RoundPath 12 (12+$shift) ($width-24) ($height-23) 52;$g.FillPath($shadow,$sp);$shadow.Dispose();$sp.Dispose()
 $y=3+$shift;$h=$height-19-$shift;$outer=RoundPath 5 $y ($width-10) $h 52;$of=Grad 5 $y ($width-10) $h '#FFE17C' '#E77E2E';$g.FillPath($of,$outer);$of.Dispose()
 $iy=12+$shift;$ih=$height-39-$shift;$inner=RoundPath 15 $iy ($width-30) $ih 43;$a=$top;$z=$bottom;if($pressed){$a=$pressedTop;$z=$pressedBottom};$f=Grad 15 $iy ($width-30) $ih $a $z 90;$g.FillPath($f,$inner);$f.Dispose()
 $p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(180,22,62,79),3);$g.DrawPath($p,$inner);$p.Dispose()
 $g.SetClip($inner);$shade=Solid 28 '#163B5B';$g.FillEllipse($shade,28,($iy+$ih-40),($width-56),42);$shade.Dispose();Soft-Spot $g 36 ($iy+3) ($width-100) 62 82;$g.ResetClip()
 $p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(170,255,244,212),3);$g.DrawArc($p,18,($iy+2),($width-36),($ih-4),205,130);$p.Dispose();$g.Dispose();$outer.Dispose();$inner.Dispose();Save-Png $b $name;$b.Dispose()
}
Draw-Button 'menu-play-button.png' '#FFC35C' '#E77A2E' '#EAA34D' '#C85E2C' $false 216 600
Draw-Button 'menu-play-button-pressed.png' '#FFC35C' '#E77A2E' '#EAA34D' '#C85E2C' $true 216 600
Draw-Button 'btn-primary.png' '#FFC35C' '#E77A2E' '#EAA34D' '#C85E2C' $false
Draw-Button 'btn-primary-pressed.png' '#FFC35C' '#E77A2E' '#EAA34D' '#C85E2C' $true
Draw-Button 'btn-secondary.png' '#69DFD0' '#108698' '#45BCAF' '#0B6579' $false
Draw-Button 'btn-secondary-pressed.png' '#69DFD0' '#108698' '#45BCAF' '#0B6579' $true
Draw-Button 'btn-danger.png' '#FF9B82' '#D44351' '#E77669' '#AD3145' $false
Draw-Button 'btn-danger-pressed.png' '#FF9B82' '#D44351' '#E77669' '#AD3145' $true

# Cream card with broad toy-gold frame and clean, stretch-safe edges.
$b,$g=New-Canvas 969 1200;$g.Clear([System.Drawing.Color]::Transparent);$shadow=Solid 120 '#162E48';$sh=RoundPath 27 34 914 1137 86;$g.FillPath($shadow,$sh);$shadow.Dispose();$sh.Dispose();$outer=RoundPath 22 15 925 1140 88;$of=Grad 22 15 925 1140 '#FFF09A' '#D9762D';$g.FillPath($of,$outer);$of.Dispose();$inner=RoundPath 48 41 873 1088 66;$cf=Grad 48 41 873 1088 '#FFFAEB' '#E8D8B7';$g.FillPath($cf,$inner);$cf.Dispose();$g.SetClip($inner);Soft-Spot $g 90 54 660 250 48;$g.ResetClip();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(195,255,255,238),6);$g.DrawPath($p,$inner);$p.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(70,158,99,45),3);$g.DrawPath($p,$outer);$p.Dispose();$g.Dispose();$outer.Dispose();$inner.Dispose();Save-Png $b 'card-panel.png';$b.Dispose()

function Star-Path([float]$cx,[float]$cy,[float]$outer,[float]$inner){$p=[System.Drawing.Drawing2D.GraphicsPath]::new();$points=[System.Collections.Generic.List[System.Drawing.PointF]]::new();for($i=0;$i -lt 10;$i++){$a=(-90+$i*36)*[Math]::PI/180;$r=$outer;if($i%2 -eq 1){$r=$inner};$points.Add([System.Drawing.PointF]::new([float]($cx+[Math]::Cos($a)*$r),[float]($cy+[Math]::Sin($a)*$r)))};$p.AddPolygon($points.ToArray());return $p}
$b,$g=New-Canvas 180 180;$g.Clear([System.Drawing.Color]::Transparent);$s=Star-Path 90 90 78 36;$shadow=Solid 90 '#17314A';$m=[System.Drawing.Drawing2D.Matrix]::new();$m.Translate(0,7);$ss=$s.Clone();$ss.Transform($m);$g.FillPath($shadow,$ss);$m.Dispose();$ss.Dispose();$shadow.Dispose();$sf=Grad 18 12 144 150 '#FFF47A' '#F4A12F';$g.FillPath($sf,$s);$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(225,76,92,92),4);$p.LineJoin=[System.Drawing.Drawing2D.LineJoin]::Round;$g.DrawPath($p,$s);$p.Dispose();$g.SetClip($s);Soft-Spot $g 52 30 68 44 120;$g.ResetClip();$sf.Dispose();$s.Dispose();$g.Dispose();Save-Png $b 'star-full.png';$b.Dispose()
$b,$g=New-Canvas 180 180;$g.Clear([System.Drawing.Color]::Transparent);$s=Star-Path 90 90 78 36;$ef=Grad 18 12 144 150 '#FFFDF1' '#D8D7C8';$g.FillPath($ef,$s);$round=[System.Drawing.Pen]::new((Hex '#E6E2D1'),9);$round.LineJoin=[System.Drawing.Drawing2D.LineJoin]::Round;$g.DrawPath($round,$s);$round.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,116,151,154),7);$p.LineJoin=[System.Drawing.Drawing2D.LineJoin]::Round;$g.DrawPath($p,$s);$p.Dispose();$ef.Dispose();$s.Dispose();$g.Dispose();Save-Png $b 'star-empty.png';$b.Dispose()

# Five isolated preview sheets for review by asset group.
function Preview-Group([string]$name,[string[]]$files,[string[]]$labels,[int]$w,[int]$h,[int]$columns){$b,$g=New-Canvas $w $h;$g.Clear([System.Drawing.Color]::FromArgb(255,24,49,70));$font=[System.Drawing.Font]::new('Segoe UI',15,[System.Drawing.FontStyle]::Bold);$brush=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,255,244,217));$cell=[int]($w/$columns);for($i=0;$i -lt $files.Count;$i++){$im=[System.Drawing.Bitmap]::new((Join-Path $dir $files[$i]));$col=$i%$columns;$row=[int][Math]::Floor($i/$columns);$x=$col*$cell+16;$y=$row*([int]($h/[Math]::Ceiling($files.Count/[double]$columns)))+12;$g.DrawString($labels[$i],$font,$brush,$x,$y);$maxW=$cell-32;$maxH=[int]($h/[Math]::Ceiling($files.Count/[double]$columns))-48;$scale=[Math]::Min($maxW/[double]$im.Width,$maxH/[double]$im.Height);$dw=[int]($im.Width*$scale);$dh=[int]($im.Height*$scale);$g.DrawImage($im,$x+[int](($maxW-$dw)/2),$y+26+$maxH-$dh,$dw,$dh);$im.Dispose()};$g.Dispose();$font.Dispose();$brush.Dispose();Save-Png $b $name;$b.Dispose()}
Preview-Group 'preview-hud.png' @('hud-level-badge.png','hud-pill.png','hud-pill-urgent.png','hud-icon-piece.png','hud-icon-clock.png') @('Level badge','HUD pill','Urgent pill','Piece','Clock') 1420 390 3
Preview-Group 'preview-menu.png' @('menu-play-button.png','menu-play-button-pressed.png') @('Play button','Pressed') 1240 290 2
Preview-Group 'preview-card-buttons.png' @('btn-primary.png','btn-primary-pressed.png','btn-secondary.png','btn-secondary-pressed.png','btn-danger.png','btn-danger-pressed.png') @('Primary','Pressed','Secondary','Pressed','Danger','Pressed') 1500 570 3
Preview-Group 'preview-card-panel.png' @('card-panel.png') @('Card panel') 600 690 1
Preview-Group 'preview-stars.png' @('star-full.png','star-empty.png') @('Full star','Empty star') 600 270 2
Write-Output 'Task11 UI set and grouped previews created.'
