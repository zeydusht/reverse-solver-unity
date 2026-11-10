$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$type=@'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
public static class PaletteTask12 {
 static double Clamp(double x){return Math.Max(0,Math.Min(1,x));}
 static Color Mix(Color a, Color b, double t, byte alpha){t=Clamp(t);return Color.FromArgb(alpha,(int)Math.Round(a.R+(b.R-a.R)*t),(int)Math.Round(a.G+(b.G-a.G)*t),(int)Math.Round(a.B+(b.B-a.B)*t));}
 static Color Gold(double v, byte alpha){
  Color deep=Color.FromArgb(255,206,93,1), mid=Color.FromArgb(255,243,138,3), bright=Color.FromArgb(255,251,186,14), light=Color.FromArgb(255,255,241,194);
  if(v<0.27)return deep;
  if(v<0.48)return Mix(deep,mid,(v-0.27)/0.21,alpha);
  if(v<0.68)return Mix(deep,mid,(v-0.48)/0.20,alpha);
  if(v<0.84)return Mix(mid,bright,(v-0.68)/0.16,alpha);
  return Mix(bright,light,(v-0.84)/0.16,alpha);
 }
 static void Hsv(Color c,out double h,out double s,out double v){double r=c.R/255.0,g=c.G/255.0,b=c.B/255.0,max=Math.Max(r,Math.Max(g,b)),min=Math.Min(r,Math.Min(g,b)),d=max-min;v=max;s=max==0?0:d/max;if(d==0)h=0;else if(max==r)h=60*((g-b)/d%6);else if(max==g)h=60*((b-r)/d+2);else h=60*((r-g)/d+4);if(h<0)h+=360;}
 static Color Map(Color c,bool icons){double h,s,v;Hsv(c,out h,out s,out v);byte a=c.A;if(a==0)return c;
  if(icons){
   if(s<0.16 && v>0.62){Color cream=Color.FromArgb(255,248,235,208);return Mix(Color.FromArgb(255,191,166,125),cream,(v-0.45)/0.55,a);}
   if(h>=14&&h<=72&&s>0.12)return Gold(v,a);
   if(v<0.24){Color navy=Color.FromArgb(255,11,42,54);return Mix(Color.FromArgb(255,3,12,18),navy,v/0.24,a);}
   if(v>0.82&&s<0.35){Color hi=Color.FromArgb(255,255,241,194);return Mix(Color.FromArgb(255,248,235,208),hi,(v-0.65)/0.35,a);}
   return Gold(v,a);
  }
  if(h>=14&&h<=68&&s>0.18)return Gold(v,a);
  return c;
 }
 static Bitmap Copy32(string path){using(Bitmap src=new Bitmap(path)){Bitmap b=new Bitmap(src.Width,src.Height,PixelFormat.Format32bppArgb);using(Graphics g=Graphics.FromImage(b)){g.CompositingMode=CompositingMode.SourceCopy;g.DrawImage(src,new Rectangle(0,0,src.Width,src.Height));}return b;}}
 static void Transform(Bitmap b,bool icons){Rectangle r=new Rectangle(0,0,b.Width,b.Height);BitmapData d=b.LockBits(r,ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);int stride=d.Stride;byte[] data=new byte[stride*b.Height];Marshal.Copy(d.Scan0,data,0,data.Length);for(int y=0;y<b.Height;y++){int row=y*stride;for(int x=0;x<b.Width;x++){int i=row+x*4;byte bb=data[i],gg=data[i+1],rr=data[i+2],aa=data[i+3];if(aa==0)continue;Color o=Map(Color.FromArgb(aa,rr,gg,bb),icons);data[i]=o.B;data[i+1]=o.G;data[i+2]=o.R;data[i+3]=o.A;}}Marshal.Copy(data,0,d.Scan0,data.Length);b.UnlockBits(d);}
 public static void Remap(string src,string dst,bool icons){using(Bitmap b=Copy32(src)){Transform(b,icons);b.Save(dst,ImageFormat.Png);}}
 public static void FitIcon(string src,string dst,int target){using(Bitmap s=Copy32(src)){BitmapData d=s.LockBits(new Rectangle(0,0,s.Width,s.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);int stride=d.Stride;byte[] data=new byte[stride*s.Height];Marshal.Copy(d.Scan0,data,0,data.Length);s.UnlockBits(d);int l=s.Width,t=s.Height,r=-1,bt=-1;for(int y=0;y<s.Height;y++){int row=y*stride;for(int x=0;x<s.Width;x++){if(data[row+x*4+3]>60){if(x<l)l=x;if(x>r)r=x;if(y<t)t=y;if(y>bt)bt=y;}}}if(r<l)throw new Exception("Empty icon: "+src);Rectangle crop=new Rectangle(l,t,r-l+1,bt-t+1);int inner=144;double scale=Math.Min(inner/(double)crop.Width,inner/(double)crop.Height);int w=(int)Math.Round(crop.Width*scale),h=(int)Math.Round(crop.Height*scale);using(Bitmap o=new Bitmap(target,target,PixelFormat.Format32bppArgb)){using(Graphics g=Graphics.FromImage(o)){g.Clear(Color.Transparent);g.SmoothingMode=SmoothingMode.HighQuality;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.DrawImage(s,new Rectangle((target-w)/2,(target-h)/2,w,h),crop,GraphicsUnit.Pixel);}Transform(o,true);o.Save(dst,ImageFormat.Png);}}}
}
'@
Add-Type -TypeDefinition $type -ReferencedAssemblies 'System.Drawing'
$dir='Assets\Art\Gameplay\Task12';New-Item -ItemType Directory -Force $dir|Out-Null
$gen='C:\Users\zeydt\.codex\generated_images\01a1695b-7c0a-7de3-a650-ac9e7c19776e'

# Recolored Task10 toy icons: shared gold/cream/navy palette; subject fills 72% of the inner button.
[PaletteTask12]::FitIcon((Join-Path $gen 'exec-d7196cc9-5514-43a9-a853-567cc494b321.png'),(Join-Path $dir 'booster-scissors.png'),240)
[PaletteTask12]::FitIcon((Join-Path $gen 'exec-0fd6a498-f1c0-4c48-a16a-a93e09784d1e.png'),(Join-Path $dir 'booster-wand.png'),240)
[PaletteTask12]::FitIcon((Join-Path $gen 'exec-1276341a-9c24-41a1-8fd6-764327c8c8c8.png'),(Join-Path $dir 'booster-hammer.png'),240)
[PaletteTask12]::FitIcon((Join-Path $gen 'exec-743ae0bb-08a5-4636-8e1c-b86b71122222.png'),(Join-Path $dir 'booster-clock.png'),240)

# Palette-exact booster ring and coral count badge.
$fmt=[System.Drawing.Imaging.PixelFormat]::Format32bppArgb;$navy=[System.Drawing.Color]::FromArgb(255,11,42,54)
$b=[System.Drawing.Bitmap]::new(264,264,$fmt);$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent)
$shadow=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(100,4,25,34));$g.FillEllipse($shadow,10,15,245,245);$shadow.Dispose()
$rim=[System.Drawing.Drawing2D.GraphicsPath]::new();$rim.AddEllipse(6,4,252,252);$rg=[System.Drawing.Drawing2D.PathGradientBrush]::new($rim);$rg.CenterPoint=[System.Drawing.PointF]::new(102,62);$rg.CenterColor=[System.Drawing.Color]::FromArgb(255,251,186,14);$rg.SurroundColors=@([System.Drawing.Color]::FromArgb(255,206,93,1));$g.FillPath($rg,$rim);$rg.Dispose();$rim.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,255,241,194),4);$g.DrawEllipse($p,9,7,246,246);$p.Dispose()
$inner=[System.Drawing.Drawing2D.GraphicsPath]::new();$inner.AddEllipse(29,27,206,206);$ig=[System.Drawing.Drawing2D.PathGradientBrush]::new($inner);$ig.CenterPoint=[System.Drawing.PointF]::new(111,86);$ig.CenterColor=[System.Drawing.Color]::FromArgb(255,28,127,140);$ig.SurroundColors=@([System.Drawing.Color]::FromArgb(255,15,79,90));$g.FillPath($ig,$inner);$ig.Dispose();$inner.Dispose()
$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,11,42,54),7);$g.DrawEllipse($p,27,25,210,210);$p.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(130,255,241,194),5);$g.DrawArc($p,42,37,180,180,205,122);$p.Dispose();$g.Dispose();$b.Save((Join-Path $dir 'booster-button.png'),[System.Drawing.Imaging.ImageFormat]::Png);$b.Dispose()
$b=[System.Drawing.Bitmap]::new(96,96,$fmt);$g=[System.Drawing.Graphics]::FromImage($b);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.Clear([System.Drawing.Color]::Transparent);$s=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(100,4,25,34));$g.FillEllipse($s,5,8,87,85);$s.Dispose();$f=[System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Rectangle]::new(3,2,90,90),[System.Drawing.Color]::FromArgb(255,232,87,74),[System.Drawing.Color]::FromArgb(255,196,57,49),90);$g.FillEllipse($f,3,2,90,90);$f.Dispose();$p=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255,248,235,208),4);$g.DrawEllipse($p,5,4,86,86);$p.Dispose();$hi=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(150,255,241,194));$g.FillEllipse($hi,21,13,39,11);$hi.Dispose();$g.Dispose();$b.Save((Join-Path $dir 'booster-badge.png'),[System.Drawing.Imaging.ImageFormat]::Png);$b.Dispose()

# Task11 UI copies with only its gold scale remapped, keeping original volumes and layout.
$ui=@('hud-level-badge.png','hud-pill.png','hud-pill-urgent.png','hud-icon-piece.png','hud-icon-clock.png','menu-play-button.png','menu-play-button-pressed.png','btn-primary.png','btn-primary-pressed.png','btn-secondary.png','btn-secondary-pressed.png','btn-danger.png','btn-danger-pressed.png','card-panel.png','star-full.png','star-empty.png')
foreach($n in $ui){[PaletteTask12]::Remap((Join-Path 'Assets\Art\Gameplay\Task11' $n),(Join-Path $dir $n),$false)}
[PaletteTask12]::Remap('Assets\Art\Gameplay\Task7\board-frame-gold-v1.png',(Join-Path $dir 'board-frame-gold-v2.png'),$false)

# Palette comparison board: one frame sample, four booster buttons, then the current HUD.
$preview=[System.Drawing.Bitmap]::new(1800,580,$fmt);$g=[System.Drawing.Graphics]::FromImage($preview);$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality;$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.Clear([System.Drawing.Color]::FromArgb(255,11,42,54));$font=[System.Drawing.Font]::new('Segoe UI',18,[System.Drawing.FontStyle]::Bold);$cream=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255,248,235,208));$g.DrawString('BOARD FRAME',$font,$cream,28,24);$g.DrawString('BOOSTERS',$font,$cream,430,24);$g.DrawString('HUD',$font,$cream,1020,24)
$frame=[System.Drawing.Bitmap]::new((Join-Path $dir 'board-frame-gold-v2.png'));$g.DrawImage($frame,24,72,350,350);$frame.Dispose()
$names=@('booster-scissors.png','booster-wand.png','booster-hammer.png','booster-clock.png');$xs=@(420,565,710,855);for($i=0;$i -lt 4;$i++){$button=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-button.png'));$icon=[System.Drawing.Bitmap]::new((Join-Path $dir $names[$i]));$badge=[System.Drawing.Bitmap]::new((Join-Path $dir 'booster-badge.png'));$g.DrawImage($button,$xs[$i],95,124,124);$g.DrawImage($icon,$xs[$i]+4,99,112,112);$g.DrawImage($badge,$xs[$i]+85,183,42,42);$button.Dispose();$icon.Dispose();$badge.Dispose()}
$level=[System.Drawing.Bitmap]::new((Join-Path $dir 'hud-level-badge.png'));$g.DrawImage($level,1025,87,112,112);$level.Dispose();$pill=[System.Drawing.Bitmap]::new((Join-Path $dir 'hud-pill.png'));$g.DrawImage($pill,1145,105,610,171);$pill.Dispose();$piece=[System.Drawing.Bitmap]::new((Join-Path $dir 'hud-icon-piece.png'));$clock=[System.Drawing.Bitmap]::new((Join-Path $dir 'hud-icon-clock.png'));$g.DrawImage($piece,1160,129,56,56);$g.DrawImage($clock,1160,197,56,56);$piece.Dispose();$clock.Dispose();$g.DrawImage([System.Drawing.Bitmap]::new((Join-Path $dir 'hud-pill-urgent.png')),1145,300,610,171)
$font.Dispose();$cream.Dispose();$g.Dispose();$preview.Save((Join-Path $dir 'preview.png'),[System.Drawing.Imaging.ImageFormat]::Png);$preview.Dispose();Write-Output 'Gameplay Task12 palette set exported.'
