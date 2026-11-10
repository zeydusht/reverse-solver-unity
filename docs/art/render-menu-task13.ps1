$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$code=@'
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
public static class Menu13Review {
 static string Dir;static FontFamily Lilita;
 static Color Cream=Color.FromArgb(255,249,241,186),Ink=Color.FromArgb(255,11,42,54);
 static void Sprite(Graphics g,string name,float x,float y,float w,float h){string p=Path.Combine(Dir,name+".png");if(!File.Exists(p))return;using(Bitmap b=new Bitmap(p))g.DrawImage(b,x,y,w,h);}
 static void Text(Graphics g,string text,float cx,float cy,float size,Color color,float stroke,Color edge){
  using(GraphicsPath p=new GraphicsPath()){
   p.AddString(text,Lilita,0,size,new PointF(),StringFormat.GenericTypographic);RectangleF b=p.GetBounds();using(Matrix m=new Matrix()){m.Translate(cx-b.Width/2-b.X,cy-b.Height/2-b.Y);p.Transform(m);}
   if(stroke>0){using(GraphicsPath s=(GraphicsPath)p.Clone())using(Matrix m=new Matrix())using(Pen sh=new Pen(Color.FromArgb(160,edge),stroke)){m.Translate(0,1.6f);s.Transform(m);g.DrawPath(sh,s);}using(Pen pen=new Pen(edge,stroke)){pen.LineJoin=LineJoin.Round;g.DrawPath(pen,p);}}
   using(SolidBrush brush=new SolidBrush(color))g.FillPath(brush,p);
  }
 }
 static void Heading(Graphics g,string text,float x,float y){using(Font f=new Font("Segoe UI",20,FontStyle.Bold))using(SolidBrush c=new SolidBrush(Cream))g.DrawString(text,f,c,x,y);}
 static void Setup(Graphics g){g.SmoothingMode=SmoothingMode.AntiAlias;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.CompositingQuality=CompositingQuality.HighQuality;}
 static void Phone(Graphics g){
  using(Bitmap bg=new Bitmap(Path.Combine(Dir,"..","corridor-v04-source.png"))){float scale=Math.Max(390f/bg.Width,844f/bg.Height);g.DrawImage(bg,(390-bg.Width*scale)/2,0,bg.Width*scale,bg.Height*scale);}
  using(LinearGradientBrush shade=new LinearGradientBrush(new RectangleF(0,0,390,140),Color.FromArgb(100,4,28,33),Color.FromArgb(0,4,28,33),LinearGradientMode.Vertical))g.FillRectangle(shade,0,0,390,140);
  Sprite(g,"profile-box",13,55,168,55);Sprite(g,"profile-button",14,55,54,54);
  Text(g,"Zeyd",113,73,20,Ink,0,Ink);Text(g,"Jiggles: 128",119,93,11,Ink,0,Ink);
  Sprite(g,"counter-pill",223,68,99,30);Sprite(g,"token-star",204,58,44,44);Text(g,"12",279,83,23,Ink,0,Ink);
  Sprite(g,"settings-button",332,59,44,44);
  Text(g,"Jiggles rescued",195,534,19,Cream,2.5f,Ink);
  Sprite(g,"progress-track",43,548,307,36);if(File.Exists(Path.Combine(Dir,"progress-fill.png"))){GraphicsState s=g.Save();g.SetClip(new RectangleF(53,558,75,15));Sprite(g,"progress-fill",53,558,286,15);g.Restore(s);}
  Sprite(g,"token-jiggle",23,541,48,48);Text(g,"128 / 500",205,566,20,Cream,2.2f,Ink);
  Sprite(g,"menu-play-button",76,625,238,117);Text(g,"Level 7",195,683,50,Cream,2.4f,Color.FromArgb(255,37,67,5));
  Sprite(g,"levels-button",325,628,46,46);Text(g,"Levels",348,684,13,Cream,1.7f,Ink);
  Sprite(g,"bottom-nav",0,762,390,82);Sprite(g,"nav-active",17,754,84,74);
  string[] names={"nav-home","nav-collection","nav-jiggles","nav-trophy"};string[] labels={"Home","Gallery","Jiggles","Awards"};float[] centers={58,150,242,334};
  for(int i=0;i<4;i++){Sprite(g,names[i],centers[i]-27,i==0?761:766,54,54);Text(g,labels[i],centers[i],831,17,Cream,1.5f,Ink);}
 }
 static void Board(string file,int width,int height,Action<Graphics> content){using(Bitmap b=new Bitmap(width,height,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(b)){Setup(g);g.Clear(Color.FromArgb(255,17,43,46));content(g);b.Save(Path.Combine(Dir,file),ImageFormat.Png);}}
 public static void Render(string dir,string font){
  Dir=dir;using(PrivateFontCollection fc=new PrivateFontCollection()){fc.AddFontFile(font);Lilita=fc.Families[0];
   Board("preview-menu@3x.png",1170,2532,g=>{g.ScaleTransform(3,3);Phone(g);});
   using(Bitmap full=new Bitmap(Path.Combine(Dir,"preview-menu@3x.png")))using(Bitmap small=new Bitmap(390,844,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(small)){Setup(g);g.DrawImage(full,0,0,390,844);small.Save(Path.Combine(Dir,"preview-menu.png"),ImageFormat.Png);}
   Board("preview-header.png",1500,490,g=>{
    Heading(g,"PROFILE / COUNTERS / SETTINGS",32,24);Sprite(g,"profile-box",30,95,480,156);Sprite(g,"profile-button",31,96,156,156);Text(g,"Zeyd",335,145,48,Ink,0,Ink);Text(g,"Jiggles: 128",336,204,27,Ink,0,Ink);
    Sprite(g,"counter-pill",610,112,360,108);Sprite(g,"token-star",557,99,120,120);Text(g,"12",809,165,55,Ink,0,Ink);Sprite(g,"settings-button",1060,82,168,168);
    string[] tokens={"token-coin","token-heart","token-star","token-jiggle"};for(int i=0;i<4;i++){float x=50+i*350;Sprite(g,"counter-pill",x+48,330,275,82);Sprite(g,tokens[i],x,316,108,108);Text(g,i==0?"1,250":i==1?"5 / 5":i==2?"12":"128",x+213,373,40,Ink,0,Ink);}
   });
   Board("preview-nav.png",1500,530,g=>{
    Heading(g,"BOTTOM NAVIGATION",32,22);Sprite(g,"bottom-nav",40,250,1420,250);Sprite(g,"nav-active",70,204,290,258);
    string[] names={"nav-home","nav-collection","nav-jiggles","nav-trophy"};string[] labels={"Home","Gallery","Jiggles","Awards"};for(int i=0;i<4;i++){float x=220+i*350;Sprite(g,names[i],x-95,212,190,190);Text(g,labels[i],x,446,43,Cream,3f,Ink);Sprite(g,names[i],x-64,70,128,128);}
   });
   Board("preview-controls.png",1500,820,g=>{
    Heading(g,"APPROVED PLAY / STATES / SMALL CONTROLS",32,20);Sprite(g,"menu-play-button",28,85,570,280);Text(g,"Level 7",313,225,120,Cream,6f,Color.FromArgb(255,37,67,5));
    Sprite(g,"settings-button",675,100,160,160);Sprite(g,"settings-button-pressed",850,100,160,160);Sprite(g,"levels-button",1095,90,190,190);
    Sprite(g,"profile-button",40,430,200,200);Sprite(g,"profile-button-pressed",260,430,200,200);Sprite(g,"plus-button",590,440,100,100);Sprite(g,"plus-button-pressed",730,440,100,100);Sprite(g,"notification-badge",910,444,96,96);Text(g,"3",958,491,52,Cream,2f,Ink);
    Sprite(g,"progress-track",72,680,1300,115);if(File.Exists(Path.Combine(Dir,"progress-fill.png"))){GraphicsState s=g.Save();g.SetClip(new Rectangle(94,711,720,44));Sprite(g,"progress-fill",94,711,1252,44);g.Restore(s);}Text(g,"128 / 500",736,735,52,Cream,3f,Ink);
   });
   Board("preview.png",1800,1240,g=>{
    Heading(g,"REVERSE SOLVER  /  MENU ART",36,22);using(Bitmap phone=new Bitmap(Path.Combine(Dir,"preview-menu@3x.png")))g.DrawImage(phone,38,87,505,1093);
    Sprite(g,"profile-button",620,110,200,200);Sprite(g,"settings-button",860,120,180,180);Sprite(g,"levels-button",1090,112,190,190);
    Sprite(g,"counter-pill",1330,152,390,117);Sprite(g,"token-star",1293,138,132,132);Text(g,"12",1550,210,58,Ink,0,Ink);
    Sprite(g,"profile-box",620,365,660,214);Sprite(g,"profile-button",623,367,208,208);Text(g,"Zeyd",1050,435,60,Ink,0,Ink);Text(g,"Jiggles: 128",1058,514,34,Ink,0,Ink);
    Sprite(g,"token-coin",1350,381,150,150);Sprite(g,"token-heart",1540,381,150,150);
    Sprite(g,"bottom-nav",610,760,1125,236);Sprite(g,"nav-active",617,713,252,222);string[] icons={"nav-home","nav-collection","nav-jiggles","nav-trophy"};for(int i=0;i<4;i++)Sprite(g,icons[i],662+i*274,744,166,166);
    Sprite(g,"progress-track",646,1090,1030,116);if(File.Exists(Path.Combine(Dir,"progress-fill.png"))){GraphicsState s=g.Save();g.SetClip(new Rectangle(666,1123,510,40));Sprite(g,"progress-fill",666,1123,987,40);g.Restore(s);}Text(g,"128 / 500",1160,1146,48,Cream,3f,Ink);
   });
  }
 }
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies 'System.Drawing'
[Menu13Review]::Render('Assets\Art\Menu\Task13','Assets\Art\UI\PlayButton\LilitaOne-Regular.ttf')
Write-Output 'Menu Task13 previews rendered.'
