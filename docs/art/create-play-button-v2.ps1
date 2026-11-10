$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$code=@'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
public static class RestoredPlayButton {
 public static void Export(string input,string output,bool pressed){
  using(Bitmap src=new Bitmap(input)) using(Bitmap result=new Bitmap(684,336,PixelFormat.Format32bppArgb)){
   int l=src.Width,t=src.Height,r=0,b=0;
   for(int y=0;y<src.Height;y++)for(int x=0;x<src.Width;x++)if(src.GetPixel(x,y).A>220){l=Math.Min(l,x);r=Math.Max(r,x);t=Math.Min(t,y);b=Math.Max(b,y);}
   float sx=660f/(r-l+1),sy=300f/(b-t+1);int pad=20;
   int cl=Math.Max(0,l-pad),ct=Math.Max(0,t-pad),cr=Math.Min(src.Width,r+pad+1),cb=Math.Min(src.Height,b+pad+1);
   using(Graphics g=Graphics.FromImage(result)){
    g.Clear(Color.Transparent);g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.CompositingQuality=CompositingQuality.HighQuality;
    g.DrawImage(src,new RectangleF(12-(l-cl)*sx,12+(pressed?4:0)-(t-ct)*sy,(cr-cl)*sx,(cb-ct)*sy),new RectangleF(cl,ct,cr-cl,cb-ct),GraphicsUnit.Pixel);
   }
   result.Save(output,ImageFormat.Png);
  }
 }
 static void Label(Bitmap image,string font){
  using(PrivateFontCollection fc=new PrivateFontCollection()){
   fc.AddFontFile(font);
   using(Graphics g=Graphics.FromImage(image))using(GraphicsPath text=new GraphicsPath()){
    g.SmoothingMode=SmoothingMode.AntiAlias;text.AddString("Level 7",fc.Families[0],0,130,new PointF(),StringFormat.GenericTypographic);
    RectangleF box=text.GetBounds();float scale=84f/box.Height;
    using(Matrix m=new Matrix()){m.Translate(-box.X,-box.Y);text.Transform(m);}
    using(Matrix m=new Matrix()){m.Scale(scale,scale);text.Transform(m);}
    box=text.GetBounds();using(Matrix m=new Matrix()){m.Translate((684-box.Width)/2,12+(300-box.Height)/2+6);text.Transform(m);}
    using(GraphicsPath shadow=(GraphicsPath)text.Clone())using(Matrix m=new Matrix())using(Pen p=new Pen(Color.FromArgb(145,24,55,3),8f))using(SolidBrush sb=new SolidBrush(Color.FromArgb(145,24,55,3))){m.Translate(0,5);shadow.Transform(m);g.DrawPath(p,shadow);g.FillPath(sb,shadow);}
    using(Pen outline=new Pen(Color.FromArgb(255,37,67,5),7f)){outline.LineJoin=LineJoin.Round;g.DrawPath(outline,text);}
    using(SolidBrush cream=new SolidBrush(Color.FromArgb(255,249,241,186)))g.FillPath(cream,text);
   }
  }
 }
 public static void Preview(string reference,string normal,string font,string output){
  using(Bitmap canvas=new Bitmap(1560,590,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(canvas))using(Bitmap refImg=new Bitmap(reference))using(Bitmap button=new Bitmap(normal)){
   g.Clear(Color.FromArgb(255,22,43,42));g.InterpolationMode=InterpolationMode.HighQualityBicubic;
   using(Font f=new Font("Segoe UI",20,FontStyle.Bold))using(SolidBrush c=new SolidBrush(Color.FromArgb(255,249,241,186))){g.DrawString("REFERANS",f,c,24,18);g.DrawString("YEN\u0130DEN \u00C7\u0130Z\u0130M",f,c,852,18);}
   g.DrawImage(refImg,24,60,760,(int)Math.Round(760f*refImg.Height/refImg.Width));Label(button,font);g.DrawImage(button,840,201,684,336);
   canvas.Save(output,ImageFormat.Png);
  }
 }
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies 'System.Drawing'
$dir='Assets\Art\UI\PlayButton'
[RestoredPlayButton]::Export((Join-Path $dir 'Source\normal-v2.png'),(Join-Path $dir 'menu-play-button-v2.png'),$false)
[RestoredPlayButton]::Export((Join-Path $dir 'Source\pressed-v2.png'),(Join-Path $dir 'menu-play-button-v2-pressed.png'),$true)
[RestoredPlayButton]::Preview('Assets\Art\Reference\level-button-reference.png',(Join-Path $dir 'menu-play-button-v2.png'),(Join-Path $dir 'LilitaOne-Regular.ttf'),(Join-Path $dir 'preview-v2.png'))
Write-Output 'Restored PlayButton v2 exported: 684x336, visible silhouette 660x300.'
