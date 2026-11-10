$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$code=@'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
public static class Menu13Art {
 public static void Export(string source,string target,int width,int height,bool stretch){
  using(Bitmap original=new Bitmap(source))using(Bitmap src=new Bitmap(original.Width,original.Height,PixelFormat.Format32bppArgb)){
   using(Graphics sg=Graphics.FromImage(src)){sg.CompositingMode=CompositingMode.SourceCopy;sg.DrawImageUnscaled(original,0,0);}
   BitmapData data=src.LockBits(new Rectangle(0,0,src.Width,src.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);byte[] pixels=new byte[data.Stride*src.Height];Marshal.Copy(data.Scan0,pixels,0,pixels.Length);int stride=data.Stride;src.UnlockBits(data);
   int left=src.Width,top=src.Height,right=-1,bottom=-1;
   for(int y=0;y<src.Height;y++)for(int x=0;x<src.Width;x++){if(pixels[y*stride+x*4+3]>=96){left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);}}
   if(right<left)throw new Exception("Empty sprite: "+source);
   int bw=right-left+1,bh=bottom-top+1;float margin=width<=84?4:6;
   if(target.EndsWith("profile-avatar.png")||target.EndsWith("bottom-nav.png"))margin=0;
   float dw=width-margin*2,dh=height-margin*2;
   if(!stretch){float factor=Math.Min(dw/bw,dh/bh);dw=bw*factor;dh=bh*factor;}
   float sx=dw/bw,sy=dh/bh;int pad=12;int cl=Math.Max(0,left-pad),ct=Math.Max(0,top-pad),cr=Math.Min(src.Width,right+1+pad),cb=Math.Min(src.Height,bottom+1+pad);
   using(Bitmap result=new Bitmap(width,height,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(result)){
    g.Clear(Color.Transparent);g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.CompositingQuality=CompositingQuality.HighQuality;
    g.DrawImage(src,new RectangleF((width-dw)/2-(left-cl)*sx,(height-dh)/2-(top-ct)*sy,(cr-cl)*sx,(cb-ct)*sy),new RectangleF(cl,ct,cr-cl,cb-ct),GraphicsUnit.Pixel);
    result.Save(target,ImageFormat.Png);
   }
  }
 }
 public static void Profile(string framePath,string avatarPath,string destination,bool pressed){
  using(Bitmap frame=new Bitmap(framePath))using(Bitmap avatar=new Bitmap(avatarPath))using(Bitmap result=new Bitmap(216,216,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(result)){
   g.Clear(Color.Transparent);g.SmoothingMode=SmoothingMode.AntiAlias;g.InterpolationMode=InterpolationMode.HighQualityBicubic;
   using(SolidBrush bg=new SolidBrush(Color.FromArgb(255,15,79,90)))g.FillRectangle(bg,30,30,156,156);
   g.DrawImage(avatar,33,31+(pressed?3:0),150,150);g.DrawImage(frame,0,0,216,216);result.Save(destination,ImageFormat.Png);
  }
 }
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies 'System.Drawing'
$dir='Assets\Art\Menu\Task13'
$specs=Get-Content -LiteralPath 'docs/art/menu-task13-spec.json' -Encoding utf8 -Raw|ConvertFrom-Json
foreach($item in $specs){$src=Join-Path $dir ('Source\'+$item.name+'.png');if(Test-Path -LiteralPath $src){[Menu13Art]::Export($src,(Join-Path $dir ($item.name+'.png')),$item.width,$item.height,$item.stretch);Write-Output ('Exported '+$item.name)}}
if((Test-Path (Join-Path $dir 'profile-frame.png')) -and (Test-Path (Join-Path $dir 'profile-avatar.png'))){[Menu13Art]::Profile((Join-Path $dir 'profile-frame.png'),(Join-Path $dir 'profile-avatar.png'),(Join-Path $dir 'profile-button.png'),$false)}
if(Test-Path (Join-Path $dir 'profile-frame-pressed.png')){[Menu13Art]::Profile((Join-Path $dir 'profile-frame-pressed.png'),(Join-Path $dir 'profile-avatar.png'),(Join-Path $dir 'profile-button-pressed.png'),$true)}
