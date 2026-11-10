$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$code=@'
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
public static class Task14Pack {
 public static string Dir;
 static int[][] P={new[]{11,42,54},new[]{15,79,90},new[]{28,127,140},new[]{206,93,1},new[]{243,138,3},new[]{251,186,14},new[]{255,241,194},new[]{248,235,208},new[]{232,87,74}};
 static int[][] Edges={new[]{0,1},new[]{1,2},new[]{2,7},new[]{0,3},new[]{3,4},new[]{4,5},new[]{5,6},new[]{6,7},new[]{0,8},new[]{8,7},new[]{0,7}};
 static Dictionary<int,int> Cache=new Dictionary<int,int>();
 public static string PathOf(string name){return Path.Combine(Dir,name+".png");}
 public static Bitmap Load(string name){using(Bitmap s=new Bitmap(PathOf(name))){Bitmap b=Canvas(s.Width,s.Height);using(Graphics g=Graphics.FromImage(b)){g.CompositingMode=CompositingMode.SourceCopy;g.DrawImageUnscaled(s,0,0);}return b;}}
 static Bitmap Canvas(int w,int h){return new Bitmap(w,h,PixelFormat.Format32bppArgb);}
 static Graphics G(Bitmap b){Graphics g=Graphics.FromImage(b);g.SmoothingMode=SmoothingMode.AntiAlias;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.CompositingQuality=CompositingQuality.HighQuality;return g;}
 static Rectangle Bounds(Bitmap b){int l=b.Width,t=b.Height,r=-1,bt=-1;for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++)if(b.GetPixel(x,y).A>200){l=Math.Min(l,x);r=Math.Max(r,x);t=Math.Min(t,y);bt=Math.Max(bt,y);}if(r<0)throw new Exception("Empty source");return Rectangle.FromLTRB(Math.Max(0,l-2),Math.Max(0,t-2),Math.Min(b.Width,r+3),Math.Min(b.Height,bt+3));}
 struct V {public double x,y,z;public V(double a,double b,double c){x=a;y=b;z=c;}public static V operator +(V a,V b){return new V(a.x+b.x,a.y+b.y,a.z+b.z);}public static V operator -(V a,V b){return new V(a.x-b.x,a.y-b.y,a.z-b.z);}public static V operator *(V a,double n){return new V(a.x*n,a.y*n,a.z*n);}public double Dot(V b){return x*b.x+y*b.y+z*b.z;}public V Cross(V b){return new V(y*b.z-z*b.y,z*b.x-x*b.z,x*b.y-y*b.x);}}
 struct Face {public V a,b,c,n;}
 static List<Face> Hull;
 static void MakeHull(){Hull=new List<Face>();for(int i=0;i<P.Length;i++)for(int j=i+1;j<P.Length;j++)for(int k=j+1;k<P.Length;k++){V a=new V(P[i][0],P[i][1],P[i][2]),b=new V(P[j][0],P[j][1],P[j][2]),c=new V(P[k][0],P[k][1],P[k][2]),n=(b-a).Cross(c-a);if(n.Dot(n)<.01)continue;bool pos=false,neg=false;for(int v=0;v<P.Length;v++){double d=n.Dot(new V(P[v][0],P[v][1],P[v][2])-a);if(d>.01)pos=true;if(d<-.01)neg=true;}if(pos&&neg)continue;if(pos)n=n*-1;Hull.Add(new Face{a=a,b=b,c=c,n=n});}}
 static V Segment(V p,V a,V b){V d=b-a;double t=Math.Max(0,Math.Min(1,(p-a).Dot(d)/d.Dot(d)));return a+d*t;}
 static V Triangle(V p,Face f){V ab=f.b-f.a,ac=f.c-f.a;V q=p-f.n*(f.n.Dot(p-f.a)/f.n.Dot(f.n)),aq=q-f.a;double aa=ab.Dot(ab),bb=ab.Dot(ac),cc=ac.Dot(ac),d=aq.Dot(ab),e=aq.Dot(ac),den=aa*cc-bb*bb,u=(d*cc-e*bb)/den,v=(e*aa-d*bb)/den;if(u>=0&&v>=0&&u+v<=1)return q;V best=Segment(p,f.a,f.b);double dist=(p-best).Dot(p-best);foreach(V next in new[]{Segment(p,f.b,f.c),Segment(p,f.c,f.a)}){double dd=(p-next).Dot(p-next);if(dd<dist){best=next;dist=dd;}}return best;}
 static int Map(int rgb){int result;if(Cache.TryGetValue(rgb,out result))return result;if(Hull==null)MakeHull();V p=new V((rgb>>16)&255,(rgb>>8)&255,rgb&255);bool inside=true;foreach(Face f in Hull)if(f.n.Dot(p-f.a)>.01){inside=false;break;}if(inside){Cache[rgb]=rgb;return rgb;}double distance=1e30;V closest=p;foreach(Face f in Hull){V q=Triangle(p,f);double d=(p-q).Dot(p-q);if(d<distance){closest=q;distance=d;}}result=((int)Math.Round(closest.x)<<16)|((int)Math.Round(closest.y)<<8)|(int)Math.Round(closest.z);Cache[rgb]=result;return result;}
 static void Finish(Bitmap b){Rectangle r=new Rectangle(0,0,b.Width,b.Height);BitmapData bd=b.LockBits(r,ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);byte[] data=new byte[bd.Stride*b.Height];Marshal.Copy(bd.Scan0,data,0,data.Length);for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++){int i=y*bd.Stride+x*4;if(data[i+3]<12){data[i]=data[i+1]=data[i+2]=data[i+3]=0;continue;}int v=Map((data[i+2]<<16)|(data[i+1]<<8)|data[i]);data[i]=(byte)(v&255);data[i+1]=(byte)((v>>8)&255);data[i+2]=(byte)((v>>16)&255);}Marshal.Copy(data,0,bd.Scan0,data.Length);b.UnlockBits(bd);}
 static void Save(Bitmap b,string name){Finish(b);using(MemoryStream m=new MemoryStream()){b.Save(m,ImageFormat.Png);File.WriteAllBytes(PathOf(name),m.ToArray());}}
 public static void Fit(string src,string name,int w,int h,int pad,bool contain){using(Bitmap s=new Bitmap(src))using(Bitmap b=Canvas(w,h))using(Graphics g=G(b)){Rectangle r=Bounds(s);float dw=w-2*pad,dh=h-2*pad;if(contain){float scale=Math.Min(dw/r.Width,dh/r.Height);dw=r.Width*scale;dh=r.Height*scale;}g.DrawImage(s,new RectangleF((w-dw)/2,(h-dh)/2,dw,dh),r,GraphicsUnit.Pixel);Save(b,name);}}
 public static void State(string source,string name,string state){using(Bitmap s=Load(source))using(Bitmap b=Canvas(s.Width,s.Height))using(Graphics g=G(b)){float factor=state=="pressed"?.94f:.62f;ColorMatrix cm=new ColorMatrix(new float[][]{new[]{factor,0f,0f,0f,0f},new[]{0f,factor,0f,0f,0f},new[]{0f,0f,factor,0f,0f},new[]{0f,0f,0f,1f,0f},new[]{(1-factor)*11/255f,(1-factor)*42/255f,(1-factor)*54/255f,0f,1f}});using(ImageAttributes ia=new ImageAttributes()){ia.SetColorMatrix(cm);int dy=state=="pressed"?6:0;g.DrawImage(s,new Rectangle(0,dy,s.Width,s.Height-dy-((state=="pressed")?3:0)),0,0,s.Width,s.Height,GraphicsUnit.Pixel,ia);}Save(b,name);}}
 public static void Urgent(string source,string name){using(Bitmap b=Load(source)){for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++){Color c=b.GetPixel(x,y);if(x>b.Width*.05&&x<b.Width*.95&&y>b.Height*.18&&y<b.Height*.82&&((x-b.Width*.16)*(x-b.Width*.16)+(y-b.Height*.5)*(y-b.Height*.5)>b.Height*b.Height*.1156)&&c.A>0&&c.B>c.R*.30&&c.G>c.R*.65&&c.R>150){float t=Math.Max(0,Math.Min(.22f,(c.R-220)/180f));b.SetPixel(x,y,Color.FromArgb(c.A,(int)(232+16*t),(int)(87+148*t),(int)(74+134*t)));}}Save(b,name);}}
 public static void Prune(string name){using(Bitmap b=Load(name)){int w=b.Width,h=b.Height;int[] labels=new int[w*h];int id=0,best=0,max=0;Queue<int> q=new Queue<int>();for(int y=0;y<h;y++)for(int x=0;x<w;x++){int at=y*w+x;if(labels[at]!=0||b.GetPixel(x,y).A<32)continue;id++;int count=0;labels[at]=id;q.Enqueue(at);while(q.Count>0){int a=q.Dequeue();count++;int ax=a%w,ay=a/w;foreach(int v in new[]{a-1,a+1,a-w,a+w}){if(v<0||v>=w*h||Math.Abs(v%w-ax)>1||labels[v]!=0||b.GetPixel(v%w,v/w).A<32)continue;labels[v]=id;q.Enqueue(v);}}if(count>max){max=count;best=id;}}for(int y=0;y<h;y++)for(int x=0;x<w;x++)if(labels[y*w+x]!=best)b.SetPixel(x,y,Color.Transparent);Save(b,name);}}
 public static void Bomb(string source){using(Bitmap s=new Bitmap(source))using(Bitmap b=Canvas(384,384))using(Graphics g=G(b)){int l=s.Width,t=s.Height,r=0,bt=0;for(int y=0;y<s.Height;y++)for(int x=0;x<s.Width;x++){Color c=s.GetPixel(x,y);if(c.A>230&&c.G>c.R*1.22&&c.B>c.R*1.3&&c.G>30){l=Math.Min(l,x);r=Math.Max(r,x);t=Math.Min(t,y);bt=Math.Max(bt,y);}}float sx=292f/(r-l+1),sy=292f/(bt-t+1),cx=(l+r)/2f,cy=(t+bt)/2f;g.DrawImage(s,192-cx*sx,192-cy*sy,s.Width*sx,s.Height*sy);Save(b,"bomb-384");File.WriteAllText(Path.Combine(Dir,"Source/Export/bomb-alignment.json"),"{\"canvasCenter\":[192,192],\"bodyDiameter\":[292,292],\"sourceBodyBounds\":["+l+","+t+","+r+","+bt+"],\"numberAnchor\":[0.5,0.5],\"numbersBaked\":false}");}}
 public static void Seal(string name){using(Bitmap b=Load(name)){int band=Math.Min(6,b.Width/8);for(int y=0;y<b.Height;y++){Color a=b.GetPixel(0,y),z=b.GetPixel(b.Width-1,y);Color mid=Color.FromArgb((a.A+z.A)/2,(a.R+z.R)/2,(a.G+z.G)/2,(a.B+z.B)/2);for(int k=0;k<band;k++){float v=(band-k)/(float)band;foreach(int xx in new[]{k,b.Width-1-k}){Color c=b.GetPixel(xx,y);b.SetPixel(xx,y,Color.FromArgb((int)(c.A*(1-v)+mid.A*v),(int)(c.R*(1-v)+mid.R*v),(int)(c.G*(1-v)+mid.G*v),(int)(c.B*(1-v)+mid.B*v)));}}}Save(b,name);}}
 public static void InsetVertical(string name){using(Bitmap s=Load(name))using(Bitmap b=Canvas(s.Width,s.Height))using(Graphics g=G(b)){g.DrawImage(s,new Rectangle(0,2,s.Width,s.Height-4),new Rectangle(0,0,s.Width,s.Height),GraphicsUnit.Pixel);Save(b,name);}}
 public static void Corner(string source){using(Bitmap s=new Bitmap(source))using(Bitmap b=Canvas(96,96))using(Graphics g=G(b)){Rectangle r=Bounds(s);int cutX=r.X+(int)(r.Width*.35),cutY=r.Y+(int)(r.Height*.65);int[] xs={r.X,cutX,r.Right},ys={r.Y,cutY,r.Bottom},dx={0,48,96},dy={0,48,96};for(int y=0;y<2;y++)for(int x=0;x<2;x++)g.DrawImage(s,new Rectangle(dx[x],dy[y],dx[x+1]-dx[x],dy[y+1]-dy[y]),new Rectangle(xs[x],ys[y],xs[x+1]-xs[x],ys[y+1]-ys[y]),GraphicsUnit.Pixel);Finish(b);using(Bitmap tile=new Bitmap(PathOf("wall-repeat-96x48"))){for(int j=0;j<48;j++){b.SetPixel(95,j+48,tile.GetPixel(0,j));b.SetPixel(j,0,tile.GetPixel(0,47-j));}}b.Save(PathOf("wall-corner-96x96"),ImageFormat.Png);}}
 public static void Overlay(string background,string icon,string name,int ix,int iy,int iw,int ih){using(Bitmap b=Load(background))using(Bitmap s=Load(icon))using(Graphics g=G(b)){g.DrawImage(s,ix,iy,iw,ih);Save(b,name);}}
 public static void Booster(string icon,string state){string name="booster-"+icon+"-"+state;if(state=="pressed"){State("booster-"+icon+"-normal",name,"pressed");return;}using(Bitmap b=Load("booster-button"))using(Graphics g=G(b))using(Bitmap s=Load("booster-"+icon)){g.DrawImage(s,12,12,240,240);if(state=="locked"||state=="zero"){using(SolidBrush shade=new SolidBrush(Color.FromArgb(state=="locked"?110:75,11,42,54)))g.FillEllipse(shade,11,11,242,242);}if(state=="locked"){using(Bitmap l=Load("booster-lock"))g.DrawImage(l,84,84,96,96);}else{using(Bitmap badge=Load("booster-badge"))g.DrawImage(badge,180,180,78,78);}Save(b,name);}}
 static Bitmap Slice(Bitmap src,int w,int h,int l,int r,int t,int b){Bitmap dst=Canvas(w,h);using(Graphics g=G(dst)){int[] xx={0,l,src.Width-r,src.Width},yy={0,t,src.Height-b,src.Height},dx={0,l,w-r,w},dy={0,t,h-b,h};for(int iy=0;iy<3;iy++)for(int ix=0;ix<3;ix++)g.DrawImage(src,new Rectangle(dx[ix],dy[iy],dx[ix+1]-dx[ix],dy[iy+1]-dy[iy]),new Rectangle(xx[ix],yy[iy],xx[ix+1]-xx[ix],yy[iy+1]-yy[iy]),GraphicsUnit.Pixel);}return dst;}
 public static void Card(string kind,int height){using(Bitmap src=Load("card-panel"))using(Bitmap b=Slice(src,969,height,108,108,300,108))using(Graphics g=G(b)){if(kind=="win"){using(Bitmap star=Load("star-full")){g.DrawImage(star,220,325,180,180);g.DrawImage(star,394,292,180,180);g.DrawImage(star,568,325,180,180);}}else if(kind=="pause"){using(Bitmap icon=Load("hud-pause-normal"))g.DrawImage(icon,400,300,168,168);}else if(kind=="start"){using(Bitmap icon=Load("hud-level-badge"))g.DrawImage(icon,376,290,216,216);}Save(b,"card-"+kind);}}
 public static void Reason(string source,string name){using(Bitmap b=Canvas(240,240))using(Bitmap s=Load(source))using(Graphics g=G(b)){g.DrawImage(s,0,0,240,240);Save(b,name);}}
 public static void EmptyStar(){using(Bitmap b=Load("star-full")){for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++){Color c=b.GetPixel(x,y);if(c.A>0){float t=(c.R+c.G+c.B)/765f;b.SetPixel(x,y,Color.FromArgb(c.A,(int)(11+17*t),(int)(42+85*t),(int)(54+86*t)));}}Save(b,"star-empty");}}
 public static void Preview(string[] names,string output,int cols){int cellW=300,cellH=258;using(Bitmap b=Canvas(cols*cellW,((names.Length+cols-1)/cols)*cellH))using(Graphics g=G(b))using(Font f=new Font("Segoe UI",12,FontStyle.Bold))using(SolidBrush label=new SolidBrush(Color.FromArgb(248,235,208))){g.Clear(Color.FromArgb(15,79,90));for(int i=0;i<names.Length;i++){int x=(i%cols)*cellW,y=(i/cols)*cellH;using(Bitmap s=Load(names[i])){float scale=Math.Min(270f/s.Width,210f/s.Height);float w=s.Width*scale,h=s.Height*scale;g.DrawImage(s,x+(cellW-w)/2,y+12+(210-h)/2,w,h);}g.DrawString(names[i],f,label,x+12,y+229);}b.Save(PathOf(output),ImageFormat.Png);}}
 public static void SeamProof(){using(Bitmap b=Canvas(768,240))using(Graphics g=G(b))using(Bitmap wall=Load("wall-repeat-96x48"))using(Bitmap chain=Load("chain-link-repeat-96x48"))using(Bitmap corner=Load("wall-corner-96x96")){g.Clear(Color.FromArgb(15,79,90));for(int i=0;i<6;i++){g.DrawImage(wall,24+i*96,24,96,48);g.DrawImage(chain,24+i*96,130,96,48);}g.DrawImage(corner,636,24,96,96);b.Save(PathOf("preview-seams"),ImageFormat.Png);}}
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies System.Drawing
$dir=Join-Path (Get-Location) 'Assets/Art/Gameplay/Task14'
[Task14Pack]::Dir=$dir
$records=[Collections.Generic.List[object]]::new()
function Record($name,$w,$h,$use,$slice=@(0,0,0,0),$pivot=@(.5,.5)){$records.Add([pscustomobject]@{file=($name+'.png');width=$w;height=$h;use=$use;sliceLTRB=$slice;pivot=$pivot;scale=3})}
function Fit($source,$name,$w,$h,$pad,$contain,$use,$slice=@(0,0,0,0)){[Task14Pack]::Fit((Join-Path $dir $source),$name,$w,$h,$pad,$contain);Record $name $w $h $use $slice}
function Pressed($name,$w,$h,$use,$slice=@(0,0,0,0)){[Task14Pack]::State($name,($name+'-pressed'),'pressed');Record ($name+'-pressed') $w $h $use $slice}
Fit 'Source/Export/hud-level-crop.png' 'hud-level-badge' 216 216 6 $true 'HUD level rozeti; LV ve numara ayrı'
[Task14Pack]::Prune('hud-level-badge')
Pressed 'hud-level-badge' 216 216 'HUD level rozeti basılı'
Fit 'Source/Export/hud-moves-crop.png' 'hud-moves-pill' 360 168 6 $false 'HUD hamle kutusu; parça ikonu içerir, sayı boş' @(120,72,72,72)
Pressed 'hud-moves-pill' 360 168 'HUD hamle kutusu basılı' @(120,72,72,72)
Fit 'Source/Export/hud-time-crop.png' 'hud-time-pill' 408 168 6 $false 'HUD süre kutusu; saat ikonu içerir, sayı boş' @(126,72,72,72)
Pressed 'hud-time-pill' 408 168 'HUD süre kutusu basılı' @(126,72,72,72)
[Task14Pack]::Urgent('hud-time-pill','hud-time-pill-urgent');Record 'hud-time-pill-urgent' 408 168 'HUD azalan süre; mercan zemin, sayı boş' @(126,72,72,72)
Pressed 'hud-time-pill-urgent' 408 168 'HUD acil süre basılı' @(126,72,72,72)
Fit 'Source/Export/hud-pause-crop.png' 'hud-pause-normal' 168 168 6 $true 'HUD durdur tuşu'
[Task14Pack]::State('hud-pause-normal','hud-pause-pressed','pressed');Record 'hud-pause-pressed' 168 168 'HUD durdur tuşu basılı'
Fit 'Source/Export/booster-button-source.png' 'booster-button' 264 264 6 $true 'Booster boş yuvarlak tuş zemini'
Pressed 'booster-button' 264 264 'Booster boş zemin basılı'
[Task14Pack]::State('booster-button','booster-button-zero','zero');Record 'booster-button-zero' 264 264 'Booster 0-adet zemin durumu; rakam ayrı'
Fit 'Source/Revision2/chain-endpoint-source.png' 'booster-lock' 96 96 5 $true 'Booster kilitli durum kaplama simgesi'
[Task14Pack]::Overlay('booster-button-zero','booster-lock','booster-button-locked',84,84,96,96);Record 'booster-button-locked' 264 264 'Booster kilitli zemin; kilit içerir'
foreach($icon in @('scissors','wand','hammer','clock')){Fit ('Source/Export/booster-'+$icon+'-source.png') ('booster-'+$icon) 240 240 45 $true ('Booster bağımsız '+$icon+' ikonu; görünür ölçü 150 px')}
[Task14Pack]::Fit((Join-Path (Get-Location) 'Assets/Art/Gameplay/Task12/booster-badge.png'),'booster-badge',96,96,3,$true);Record 'booster-badge' 96 96 'Booster adet rozeti; tüm rakamlar boş'
foreach($icon in @('scissors','wand','hammer','clock')){foreach($state in @('normal','pressed','locked','zero')){[Task14Pack]::Booster($icon,$state);Record ('booster-'+$icon+'-'+$state) 264 264 ('Birleşik '+$icon+' booster / '+$state+'; sayı ayrı')}}
Fit 'Source/Export/booster-rail-source.png' 'booster-bar' 1170 360 9 $false 'Booster çubuğu boş paneli; tuşlar ayrı' @(108,108,108,72)
Fit 'Source/Export/board-frame-source.png' 'board-frame-gold' 1125 1125 12 $false 'Tahta altın çerçevesi; ortası şeffaf, bariyer ayrı' @(300,300,300,300)
[Task14Pack]::Bomb((Join-Path $dir 'Source/Revision2/bomb-centered-source.png'));Record 'bomb-384' 384 384 'Hücre bomba engeli; gövde merkezi (192,192), boş krem sayaç'
Fit 'Source/Revision2/nail-source.png' 'nail-head-384' 384 384 24 $true 'Hücre çivi engeli; üstten altın baş, boş geniş sayı yüzü'
Fit 'Source/Revision2/chain-repeat-source.png' 'chain-link-repeat-96x48' 96 48 0 $true 'İki hücre arası zincir; X tekrar'
[Task14Pack]::Seal('chain-link-repeat-96x48')
Fit 'Source/Revision2/chain-endpoint-source.png' 'chain-endpoint-144' 144 144 6 $true 'Zincirin hücrelerdeki belirgin uç kilidi'
Fit 'Source/Export/wall-repeat-source.png' 'wall-repeat-96x48' 96 48 0 $false 'Kapalı kenar bariyeri; X tekrar, dikey kullanımda 90 derece döndür'
[Task14Pack]::InsetVertical('wall-repeat-96x48')
[Task14Pack]::Seal('wall-repeat-96x48')
[Task14Pack]::Corner((Join-Path $dir 'Source/Export/wall-corner-source.png'));Record 'wall-corner-96x96' 96 96 'Bariyer L köşe; yukarı ve sağ açık bağlantı' @(0,0,0,0) @(.25,.25)
Fit 'Source/Export/card-panel-source.png' 'card-panel' 969 1200 12 $false 'Ortak kart paneli; başlık ve gövde metni boş' @(192,192,300,132)
foreach($button in @('primary','secondary','danger')){Fit ('Source/Export/btn-'+$button+'-source.png') ('btn-'+$button) 600 216 6 $false ('Kart '+$button+' tuşu; yazısız') @(84,84,84,84);Pressed ('btn-'+$button) 600 216 ('Kart '+$button+' tuşu basılı; yazısız') @(84,84,84,84)}
Fit 'Source/Export/star-full-source.png' 'star-full' 180 180 9 $true 'Kazanma dolu yıldız'
[Task14Pack]::EmptyStar();Record 'star-empty' 180 180 'Kazanma boş yıldız durumu'
foreach($kind in @('pause','win','lose','start')){$h=if($kind -eq 'pause'){1320}else{1200};[Task14Pack]::Card($kind,$h);Record ('card-'+$kind) 969 $h ('Kart '+$kind+'; tüm metinler ve eylem tuşları ayrı') @(192,192,552,132)}
[Task14Pack]::Reason('bomb-384','loss-reason-bomb');Record 'loss-reason-bomb' 240 240 'Kaybetme kartı bomba sebebi; boş etiket'
[Task14Pack]::Reason('booster-clock','loss-reason-time');Record 'loss-reason-time' 240 240 'Kaybetme kartı süre sebebi'
[Task14Pack]::Overlay('card-lose','loss-reason-bomb','card-lose-bomb',364,290,240,240);Record 'card-lose-bomb' 969 1200 'Kaybetme kartı bomba sebebiyle; metin ve tuş ayrı' @(192,192,552,132)
[Task14Pack]::Overlay('card-lose','loss-reason-time','card-lose-time',364,290,240,240);Record 'card-lose-time' 969 1200 'Kaybetme kartı süre sebebiyle; metin ve tuş ayrı' @(192,192,552,132)
[Task14Pack]::Preview(@('hud-level-badge','hud-level-badge-pressed','hud-moves-pill','hud-moves-pill-pressed','hud-time-pill','hud-time-pill-urgent','hud-pause-normal','hud-pause-pressed','booster-bar','board-frame-gold','bomb-384','nail-head-384','chain-link-repeat-96x48','chain-endpoint-144','wall-repeat-96x48','wall-corner-96x96'),'preview-export',4)
[Task14Pack]::Preview(@('booster-scissors-normal','booster-wand-normal','booster-hammer-normal','booster-clock-normal','booster-scissors-pressed','booster-wand-pressed','booster-hammer-pressed','booster-clock-pressed','booster-scissors-locked','booster-wand-locked','booster-hammer-locked','booster-clock-locked','booster-scissors-zero','booster-wand-zero','booster-hammer-zero','booster-clock-zero'),'preview-booster-states',4)
[Task14Pack]::Preview(@('card-pause','card-win','card-lose-bomb','card-start','btn-primary','btn-secondary','btn-danger','star-full','btn-primary-pressed','btn-secondary-pressed','btn-danger-pressed','star-empty'),'preview-cards',4)
[Task14Pack]::SeamProof()
$records | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $dir 'manifest.json') -Encoding UTF8
Write-Output ('Task14 sprite exports: '+$records.Count)
