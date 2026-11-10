from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps,ImageFilter
import json,hashlib,math,shutil
from datetime import datetime
ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'Assets/Art/Redesign_T23';WORK=ROOT/'docs/art/task23';R=Image.Resampling.LANCZOS
FONT=ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf';INK='#0B2A36';CREAM='#FFF6D8';TEAL='#1C7F8C'
def art(path):return Image.open(ROOT/path).convert('RGBA')
def sizept(pt):return tuple(round(v*3) for v in pt)
def contain(a,sz):return ImageOps.contain(a,sz,R)
def paste(im,a,rect,stretch=False):
 x,y,w,h=rect;sz=sizept((w,h));a=a.resize(sz,R) if stretch else contain(a,sz);im.alpha_composite(a,(round(x*3+(sz[0]-a.width)/2),round(y*3+(sz[1]-a.height)/2)))
def nine(a,sz,b):
 l,t,r,bot=b;w,h=a.size;tw,th=sz;assert tw>l+r and th>t+bot
 xs=[0,l,w-r,w];ys=[0,t,h-bot,h];dx=[0,l,tw-r,tw];dy=[0,t,th-bot,th];o=Image.new('RGBA',sz)
 for j in range(3):
  for i in range(3):
   if xs[i+1]<=xs[i] or ys[j+1]<=ys[j]:continue
   o.alpha_composite(a.crop((xs[i],ys[j],xs[i+1],ys[j+1])).resize((dx[i+1]-dx[i],dy[j+1]-dy[j]),R),(dx[i],dy[j]))
 return o
def txt(im,s,x,y,pt=15,color=INK,banner=False,maxw=None,green=False,anchor='mm'):
 f=ImageFont.truetype(str(FONT),round(pt*3));d=ImageDraw.Draw(im);sw=round(pt*.05*3) if banner else 0;stroke='#1E4D05' if green else INK
 assert pt>=13,(s,pt)
 if maxw:assert f.getlength(s)+sw*2<=maxw*3,(s,pt,maxw,f.getlength(s)/3)
 if banner:d.text((round(x*3),round((y+pt*.08)*3)),s,font=f,anchor=anchor,fill=stroke,stroke_width=sw,stroke_fill=stroke)
 d.text((round(x*3),round(y*3)),s,font=f,anchor=anchor,fill=color,stroke_width=sw,stroke_fill=stroke)
 return dict(text=s,xPt=x,yPt=y,sizePt=pt,color=color,banner=banner,maxWidthPt=maxw)
def lines(im,ls,x,y,pt=15,gap=22,maxw=285):
 assert len(ls)<=2
 return [txt(im,s,x,y+i*gap,pt,maxw=maxw) for i,s in enumerate(ls)]
def pill_text(im,s,x,y,w,pt=14):return txt(im,s,x,y,pt,maxw=w)
def filehash(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
class Trial:
 def __init__(self,folder,variant,title,source,ui,target):
  self.folder=folder;self.variant=variant;self.path=OUT/'DENEMELER'/variant/folder;self.path.mkdir(parents=True,exist_ok=True);self.rows=[];self.text=[];self.title=title;self.source=source;self.ui=ui;self.target=target;self.notes=[]
 def save(self,name,im,role,b=(0,0,0,0),source=None,hit=None):
  im=im.convert('RGBA');p=self.path/(name+'.png');im.save(p);l,t,r,bb=b;assert l+r<im.width and t+bb<im.height
  row=dict(file=p.name,width=im.width,height=im.height,logicalPt=[im.width/3,im.height/3],scale=3,sliceLTRB=list(b),role=role,source=source or 'Task23 layout/export',sha256=filehash(p),approved=False)
  if hit:row['minHitPt']=hit
  self.rows.append(row);return im
 def use(self,name,path,pt,b=(0,0,0,0),role='Onaylı parçanın korunmuş exportu',hit=None):return self.save(name,art(path).resize(sizept(pt),R),role,b,path,hit)
 def panel(self,h,title=None):
  base=art('Assets/Art/Redesign_T22/101-ai-kazanma/panel-@3x.png');im=nine(base,sizept((335,h)),(72,210,72,72));self.save('panel',im,'PM kabul101 yumuşak turkuaz panel',(72,210,72,72),'T22/101-ai-kazanma/panel-@3x.png');preview=im.copy();self.text.append(txt(preview,title or self.title,167.5,35,22 if self.variant=='A' else 23,CREAM,True,300));return preview
 def button(self,im,s,x,y,w=218,primary=True):
  # Approved Task15 aspect is684×336. Never squash that button into a thin strip.
  h=w*336/684;key='btn-primary-v2' if primary else 'btn-secondary-v2';src='Assets/Art/Gameplay/Task15/'+key+'.png';a=art(src);paste(im,a,(x-w/2,y,w,h),True)
  self.text.append(txt(im,s,x,y+h/2-3,20 if primary else 17,CREAM,True,w-18,primary))
  return dict(label=s,rectPt=[x-w/2,y,w,h],hitPt=[w,h*.8])
 def finish(self,preview,rect=None,extra=None):
  preview.save(self.path/'onizleme.png')
  record=dict(folder=self.folder,variant=self.variant,title=self.title,source=self.source,ui=self.ui,target=self.target,assets=list({r['file']:r for r in self.rows}.values()),text=self.text,notes=self.notes)
  if extra:record.update(extra)
  if rect:
   before=art(self.source);scale=375/before.width;before=before.resize((1125,round(before.height*scale*3)),R);after=before.copy();x,y,w,h=rect;assert preview.size==sizept((w,h)),(self.folder,preview.size,rect)
   if self.rows and self.rows[0]['file']=='panel.png':
    background='Assets/Art/Menu/Task20/scene-home.png' if any(k in self.folder for k in ['isim','profil','tamam']) else 'Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png'
    bg=art(background).resize(before.size,R);bg.alpha_composite(Image.new('RGBA',bg.size,(8,32,34,220)));box=(0,210,before.width,before.height-210);after.paste(bg.crop(box),box[:2]);self.notes.append('Eski modalın altındaki sahne yalnız karşılaştırma için onaylı oyun arka planından yeniden kuruldu; eski kartın kenarları yeni adayın arkasında bırakılmadı.')
   paste(after,preview,rect,True);after.save(self.path/'ekran-sonra.png');pad=6;box=(max(0,round((x-pad)*3)),max(0,round((y-pad)*3)),min(before.width,round((x+w+pad)*3)),min(before.height,round((y+h+pad)*3)));a=before.crop(box);b=after.crop(box)
   pair=Image.new('RGB',(a.width+b.width+24,max(a.height,b.height)+72),INK);pair.paste(a,(0,72));pair.paste(b,(a.width+24,72));d=ImageDraw.Draw(pair);f=ImageFont.truetype(str(FONT),26);d.text((12,10),'ÖNCE · kaynak',font=f,fill=CREAM);d.text((a.width+36,10),'SONRA · Task23',font=f,fill=CREAM);pair.save(self.path/'once-sonra.png');record['rectPt']=rect
  (self.path/'deneme.json').write_text(json.dumps(record,ensure_ascii=False,indent=2),encoding='utf-8')
  return record
def contact(paths,path,cols=3,cw=360,ch=540):
 sheet=Image.new('RGB',(cols*cw,math.ceil(len(paths)/cols)*ch),'#274854');d=ImageDraw.Draw(sheet);f=ImageFont.truetype(str(FONT),17)
 for i,p in enumerate(paths):
  a=Image.open(p).convert('RGBA');a.thumbnail((cw-15,ch-65),R);x=i%cols*cw;y=i//cols*ch;sheet.paste(a,(x+(cw-a.width)//2,y),a);d.text((x+6,y+ch-55),p.parent.parent.name+'/'+p.parent.name,font=f,fill=CREAM)
 sheet.save(path,quality=94)
