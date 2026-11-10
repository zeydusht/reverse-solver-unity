param([switch]$Final)
$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$code=@'
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
public static class Gameplay14Revision {
 static string Root;static FontFamily Digits,Labels;static float Bx=35,By=235,Side=320;
 static Color Navy=Color.FromArgb(255,11,42,54),Cream=Color.FromArgb(255,248,235,208);
 static void Sprite(Graphics g,string path,float x,float y,float w,float h){using(Bitmap b=new Bitmap(Path.Combine(Root,path)))g.DrawImage(b,x,y,w,h);}
 static void Setup(Graphics g){g.SmoothingMode=SmoothingMode.AntiAlias;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.CompositingQuality=CompositingQuality.HighQuality;}
 static void Text(Graphics g,string s,float cx,float cy,float size,Color c,bool number,float stroke,Color edge){using(GraphicsPath p=new GraphicsPath()){p.AddString(s,number?Digits:Labels,0,size,new PointF(),StringFormat.GenericTypographic);RectangleF bounds=p.GetBounds();using(Matrix m=new Matrix()){m.Translate(cx-bounds.X-bounds.Width/2,cy-bounds.Y-bounds.Height/2);p.Transform(m);}if(stroke>0)using(Pen pen=new Pen(edge,stroke)){pen.LineJoin=LineJoin.Round;g.DrawPath(pen,p);}using(SolidBrush b=new SolidBrush(c))g.FillPath(b,p);}}
 static void Calibrate(Bitmap ui){
  int cx=ui.Width/2,cy=(int)(ui.Height*.47);if(ui.GetPixel(cx,cy).A>100)throw new Exception("Board opening is not transparent");int l=cx,r=cx,t=cy,b=cy;
  while(l>0&&ui.GetPixel(l,cy).A<160)l--;while(r<ui.Width-1&&ui.GetPixel(r,cy).A<160)r++;while(t>0&&ui.GetPixel(cx,t).A<160)t--;while(b<ui.Height-1&&ui.GetPixel(cx,b).A<160)b++;
  float sx=390f/ui.Width,sy=844f/ui.Height;Side=Math.Min((r-l)*sx,(b-t)*sy)-6;Bx=(l+r)*sx/2-Side/2;By=(t+b)*sy/2-Side/2;
  File.WriteAllText(Path.Combine(Root,"docs/art/reports/task14-board-placement.json"),"{\"x\":"+Bx.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"y\":"+By.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"side\":"+Side.ToString(System.Globalization.CultureInfo.InvariantCulture)+"}");
 }
 static void Board(Graphics g,string[] names){
  using(SolidBrush b=new SolidBrush(Color.FromArgb(255,15,79,90)))g.FillRectangle(b,Bx-3,By-3,Side+6,Side+6);
  GraphicsState s=g.Save();g.SetClip(new RectangleF(Bx-1,By-1,Side+2,Side+2));float cell=Side/5,scale=cell/384;
  for(int y=0;y<5;y++)for(int x=0;x<5;x++){int index=y*5+x;if(index==0||index==24)continue;Sprite(g,"Assets/Art/Pieces/Kit81/flat/"+names[index]+".png",Bx+x*cell-128*scale,By+y*cell-128*scale,640*scale,640*scale);}
  g.Restore(s);
 }
 static void Obstacles(Graphics g){
  float c=Side/5,cx=Bx+2.5f*c,ny=By+1.5f*c,by=By+3.5f*c,cy=By+2.5f*c,left=Bx+.5f*c,right=Bx+1.5f*c;
  Sprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/chain-repeat-source.png",left,cy-c*.16f,c,c*.32f);
  Sprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/chain-endpoint-source.png",left-c*.30f,cy-c*.30f,c*.60f,c*.60f);
  Sprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/chain-endpoint-source.png",right-c*.30f,cy-c*.30f,c*.60f,c*.60f);
  Sprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/nail-source.png",cx-c*.45f,ny-c*.45f,c*.90f,c*.90f);Text(g,"12",cx,ny,23,Navy,true,0,Cream);
  Bomb(g,cx,by,c*.98f);Text(g,"24",cx,by-.35f,20,Navy,true,0,Cream);
 }
 static void Bomb(Graphics g,float cx,float cy,float size){
  float scale=size*.76f/982f;
  Sprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/bomb-centered-source.png",cx-626.5f*scale,cy-698f*scale,1254*scale,1254*scale);
 }
 static void TightSprite(Graphics g,string path,float x,float y,float w,float h){
  using(Bitmap b=new Bitmap(Path.Combine(Root,path))){int l=b.Width,t=b.Height,r=0,bt=0;for(int yy=0;yy<b.Height;yy++)for(int xx=0;xx<b.Width;xx++)if(b.GetPixel(xx,yy).A>220){l=Math.Min(l,xx);t=Math.Min(t,yy);r=Math.Max(r,xx);bt=Math.Max(bt,yy);}g.DrawImage(b,new RectangleF(x,y,w,h),new RectangleF(l,t,r-l+1,bt-t+1),GraphicsUnit.Pixel);}
 }
 static void RevisedUi(Graphics g){
  string[] icons={"scissors","wand","hammer","clock"};
  for(int i=0;i<4;i++){TightSprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/booster-"+icons[i]+"-source.png",24+i*88,705,78,78);Sprite(g,"Assets/Art/Gameplay/Task12/booster-badge.png",77+i*88,760,25,25);}
  TightSprite(g,"Assets/Art/Gameplay/Task14/Source/Revision2/wall-source.png",344,339,28,142);
 }
 static void GuideUi(Graphics g){
  Sprite(g,"Assets/Art/Gameplay/Task12/hud-level-badge.png",11,65,58,58);
  Sprite(g,"Assets/Art/Menu/Task13/counter-pill.png",79,73,108,48);
  Sprite(g,"Assets/Art/Menu/Task13/counter-pill.png",194,73,119,48);
  Sprite(g,"Assets/Art/Gameplay/Task12/hud-icon-piece.png",84,83,29,29);
  Sprite(g,"Assets/Art/Gameplay/Task12/hud-icon-clock.png",198,83,29,29);
  Sprite(g,"Assets/Art/Gameplay/Task7/hud-pause-normal.png",326,73,48,48);
  Sprite(g,"Assets/Art/Gameplay/Task7/board-frame-gold-v1.png",15,215,360,360);
  Sprite(g,"Assets/Art/Menu/Task13/bottom-nav.png",12,681,366,123);
  string[] names={"scissors","wand","hammer","clock"};for(int i=0;i<4;i++){float x=27+i*88;Sprite(g,"Assets/Art/Gameplay/Task12/booster-button.png",x,707,72,72);Sprite(g,"Assets/Art/Gameplay/Task12/booster-"+names[i]+".png",x+1,708,70,70);if(i<3)Sprite(g,"Assets/Art/Gameplay/Task12/booster-badge.png",x+50,760,24,24);}
 }
 static void Numbers(Graphics g){
  Text(g,"LV",41,91,12,Navy,false,0,Navy);Text(g,"7",41,113,29,Navy,true,0,Navy);
  Text(g,"23",147,106,29,Navy,true,0,Navy);Text(g,"0:48",275,106,25,Navy,true,0,Navy);
  Text(g,"3",89,772,18,Cream,true,.5f,Navy);Text(g,"2",177,772,18,Cream,true,.5f,Navy);Text(g,"0",265,772,18,Cream,true,.5f,Navy);Text(g,"1",353,772,18,Cream,true,.5f,Navy);
 }
 static void DetailPreview(){
  using(Bitmap b=new Bitmap(720,304,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(b)){
   Setup(g);g.Clear(Color.FromArgb(255,15,79,90));
   Text(g,"64 px / TWO DIGITS",360,29,23,Cream,false,0,Navy);
   string p="Assets/Art/Gameplay/Task14/Source/Revision2/";
   Bomb(g,100,112,64);Sprite(g,p+"nail-source.png",236,80,64,64);Sprite(g,p+"chain-repeat-source.png",408,96,64,32);Sprite(g,p+"chain-endpoint-source.png",564,80,64,64);
   Text(g,"BOMB",100,167,17,Cream,false,0,Navy);Text(g,"NAIL",268,167,17,Cream,false,0,Navy);Text(g,"CHAIN",440,167,17,Cream,false,0,Navy);Text(g,"LOCK",596,167,17,Cream,false,0,Navy);
   Bomb(g,100,233,64);Text(g,"24",100,232.6f,23,Navy,true,0,Cream);Sprite(g,p+"nail-source.png",236,201,64,64);Text(g,"12",268,233,27,Navy,true,0,Cream);
   string[] names={"scissors","wand","hammer","clock"};for(int i=0;i<4;i++)TightSprite(g,p+"booster-"+names[i]+"-source.png",389+i*75,207,64,64);
   b.Save(Path.Combine(Root,"Assets/Art/Gameplay/Task14/preview-details.png"),ImageFormat.Png);
  }
 }
 public static void Render(string root,string[] names,bool final){
  Root=root;using(PrivateFontCollection digits=new PrivateFontCollection())using(PrivateFontCollection labels=new PrivateFontCollection()){
   digits.AddFontFile(Path.Combine(root,"Assets/Art/UI/PlayButton/LilitaOne-Regular.ttf"));Digits=digits.Families[0];labels.AddFontFile(Path.Combine(root,"Assets/Art/UI/PlayButton/LilitaOne-Regular.ttf"));Labels=labels.Families[0];
   using(Bitmap full=new Bitmap(1170,2532,PixelFormat.Format32bppArgb))using(Graphics g=Graphics.FromImage(full)){
    if(final){using(Bitmap ui=new Bitmap(Path.Combine(root,"Assets/Art/Gameplay/Task14/Source/ui-overlay.png")))Calibrate(ui);}
    Setup(g);g.ScaleTransform(3,3);Sprite(g,"Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png",0,0,390,844);Board(g,names);
    if(final){using(Bitmap overlay=new Bitmap(Path.Combine(root,"Assets/Art/Gameplay/Task14/Source/ui-overlay.png"))){GraphicsState save=g.Save();g.ExcludeClip(Rectangle.Round(new RectangleF(Bx-1,By-1,Side+2,Side+2)));g.DrawImage(overlay,0,0,390,844);g.Restore(save);}}
    else GuideUi(g);
    RevisedUi(g);Obstacles(g);if(final)Numbers(g);DetailPreview();
    string source=Path.Combine(root,final?"Assets/Art/Gameplay/Task14/Source/preview-ingame@3x.png":"Assets/Art/Gameplay/Task14/Source/layout-guide.png");full.Save(source,ImageFormat.Png);
    if(final){using(Bitmap small=new Bitmap(390,844,PixelFormat.Format32bppArgb))using(Graphics sg=Graphics.FromImage(small)){Setup(sg);sg.DrawImage(full,0,0,390,844);small.Save(Path.Combine(root,"Assets/Art/Gameplay/Task14/preview-ingame.png"),ImageFormat.Png);}}
   }
  }
 }
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies 'System.Drawing'
$level=Get-Content 'docs/art/task14-preview-board.json' -Encoding utf8 -Raw|ConvertFrom-Json
function Mark([object]$v,[bool]$inverted){if($null -eq $v -or $v -eq 0){return 'F'};if(($inverted -and $v -eq -1) -or (-not $inverted -and $v -eq 1)){return 'O'};return 'I'}
$names=[System.Collections.Generic.List[string]]::new()
for($y=0;$y -lt 5;$y++){for($x=0;$x -lt 5;$x++){$u=if($y -eq 0){'F'}else{Mark $level.hEdge[$y][$x] $true};$r=if($x -eq 4){'F'}else{Mark $level.vEdge[$y][$x+1] $false};$d=if($y -eq 4){'F'}else{Mark $level.hEdge[$y+1][$x] $false};$l=if($x -eq 0){'F'}else{Mark $level.vEdge[$y][$x] $true};$names.Add(('piece_'+$u+'-'+$r+'-'+$d+'-'+$l))}}
[Gameplay14Revision]::Render((Get-Location).Path,$names.ToArray(),$Final.IsPresent)
Write-Output $(if($Final){'Task14 composed preview exported:390x844.'}else{'Task14 full-screen layout guide exported.'})
