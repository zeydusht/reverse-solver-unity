from t23_common import *
from PIL import ImageOps
W=ROOT/'docs/art/task25';N=OUT/'NIHAI'
def savej(p,x):p.write_text(json.dumps(x,ensure_ascii=False,indent=2),encoding='utf-8')
def archive(p):
 q=p/'T25-oncesi';q.mkdir(exist_ok=True)
 for f in list(p.iterdir()):
  if f.is_file() and not (q/f.name).exists():shutil.copyfile(f,q/f.name)
def texts(im,rows):
 for r in rows:txt(im,r['text'],r['xPt'],r['yPt'],r['sizePt'],r['color'],r.get('banner',False),r.get('maxWidthPt'),green=r['text']=='Başla')
def card_update(p,j,im):
 im.save(p/'onizleme.png');before=Image.open(p/'T25-oncesi/onizleme.png').convert('RGBA');pair=Image.new('RGB',(im.width*2+24,max(im.height,before.height)+72),INK);pair.paste(before,(0,72),before);pair.paste(im,(im.width+24,72),im);d=ImageDraw.Draw(pair);f=ImageFont.truetype(str(FONT),26);d.text((10,10),'ÖNCE · Task23 nihai',font=f,fill=CREAM);d.text((im.width+36,10),'SONRA · Task25',font=f,fill=CREAM);pair.save(p/'once-sonra.png')
 if (p/'T25-oncesi/ekran-sonra.png').exists():
  screen=Image.open(p/'T25-oncesi/ekran-sonra.png').convert('RGBA');paste(screen,im,j['rectPt'],True);screen.save(p/'ekran-sonra.png')
 for w,h in [(375,667),(390,844)]:
  screen=Image.new('RGBA',(w*3,h*3),'#123F46');screen.alpha_composite(im,((w*3-im.width)//2,(h*3-im.height)//2));screen.save(p/f'olcu-{w}x{h}.png')
 for row in j['assets']:
  a=Image.open(p/row['file']);assert a.mode=='RGBA';assert a.size==(row['width'],row['height']);row['sha256']=filehash(p/row['file'])
 savej(p/'deneme.json',j);s=json.loads((p/'secim.json').read_text(encoding='utf-8'));s['task25Revision']=j['task25Revision'];s['items']=j['assets'];savej(p/'secim.json',s)
def name():
 p=N/'17-isim';archive(p);j=json.loads((p/'deneme.json').read_text(encoding='utf-8'));im=Image.open(p/'panel.png').convert('RGBA');paste(im,Image.open(p/'name-input.png').convert('RGBA'),(29,168,277,58),True);paste(im,art('Assets/Art/Gameplay/Task15/btn-primary-v2.png'),(58.5,260,218,218*336/684),True)
 for r in j['text']:
  if r['text']=='Adın':r['color']='#92999E'
 texts(im,j['text']);im.save(p/'isim-bos.png');typed=Image.open(p/'panel.png').convert('RGBA');paste(typed,Image.open(p/'name-input.png').convert('RGBA'),(29,168,277,58),True);paste(typed,art('Assets/Art/Gameplay/Task15/btn-primary-v2.png'),(58.5,260,218,218*336/684),True);rows=json.loads(json.dumps(j['text']))
 for r in rows:
  if r['text']=='Adın':r.update(text='Zeynep',color=INK)
 texts(typed,rows);typed.save(p/'isim-yazildi.png');j['task25Revision']={'placeholder':'Adın','placeholderColor':'#92999E','valueColor':INK,'typedExample':'Zeynep','inputType':'real HTML input; runtime text remains separate','previewStates':['isim-bos.png','isim-yazildi.png']};card_update(p,j,im)
 with (p/'NOT.md').open('a',encoding='utf-8') as f:f.write('\n\n## Task25 düzeltmesi\n\nAdın yer tutucusu soluk gri #92999E; yazılmış isim #0B2A36 koyu. isim-bos.png ve isim-yazildi.png iki önizleme. Gerçek input `::placeholder { color:#92999E; opacity:1; }`, input metni #0B2A36; erişilebilir etiket Adın ayrı kalmalı. PNG input yüzeyi değişmedi. once-sonra artık Task23 nihai ↔ Task25 karşılaştırmasıdır; önceki tüm dosyalar T25-oncesi altında korunur.\n')
def hard():
 p=N/'15-zor-bolum';archive(p);j=json.loads((p/'deneme.json').read_text(encoding='utf-8'));g=json.loads((W/'baron-generation.json').read_text(encoding='utf-8'));shutil.copyfile(g['generatedPath'],p/'T25-source.png');a=ImageOps.fit(Image.open(g['generatedPath']).convert('RGBA'),(864,576),R);mask=Image.new('L',a.size,0);ImageDraw.Draw(mask).rounded_rectangle((0,0,863,575),radius=24,fill=255);a.putalpha(mask);a.save(p/'character-scene.png')
 im=Image.open(p/'panel.png').convert('RGBA');paste(im,a,(23.5,79,288,192),True);paste(im,art('Assets/Art/Gameplay/Task15/btn-primary-v2.png'),(61.5,366,212,212*336/684),True);texts(im,j['text']);j['task25Revision']={'source':'T25-source.png','prompt':g['prompt'],'characterCount':1,'pose':'Arms crossed, confident; complex board behind','replaces':'Pointing Baron scene too similar to nail intro'}
 for row in j['assets']:
  if row['file']=='character-scene.png':row['source']='T25-source.png';row['role']='Zor bölüme özel kolları kavuşturulmuş Baron; arkada karmaşık tahta'
 card_update(p,j,im)
 with (p/'NOT.md').open('a',encoding='utf-8') as f:f.write('\n\n## Task25 düzeltmesi\n\nZor bölüme özel tek Baron: kolları kavuşturulmuş ve kendinden emin; karmaşık tahta arkada. Çiviye işaret eden sahne artık kullanılmaz. Yeni character-scene.png 864×576 RGBA (@3x288×192pt),9-slice yok; T25-source.png işlenmemiş üretim kaynağıdır. Önceki bütün nihai dosyalar T25-oncesi altında korunur. once-sonra Task23 nihai ↔ Task25 karşılaştırmasıdır. Tek uyarı kuralı değişmez.\n')
def run():
 name();hard();(N/'T25-inceleme.html').write_text('<!doctype html><html lang="tr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Task25 kartlar</title><style>body{background:#123f46;color:#fff6d8;font:18px system-ui;max-width:1100px;margin:auto;padding:20px}img{max-width:100%;max-height:700px}section{display:flex;gap:20px;flex-wrap:wrap}a{color:#ffdb79}</style><h1>Task25 · İki kart düzeltmesi</h1><h2>17 · İsim</h2><section><div><p>Boş: soluk gri yer tutucu</p><img src="17-isim/isim-bos.png"></div><div><p>Yazılmış: koyu isim</p><img src="17-isim/isim-yazildi.png"></div></section><h2>15 · Zor bölüm</h2><img src="15-zor-bolum/onizleme.png"><p><a href="15-zor-bolum/once-sonra.png">Önce / sonra</a> · <a href="15-zor-bolum/NOT.md">Bağlama notu</a></p><p>Oyuna bağlanmadı. Eski sürümler ailelerin T25-oncesi klasöründe korunur.</p></html>',encoding='utf-8')
 with (ROOT/'docs/art/T23_KOD_LISTESI.md').open('a',encoding='utf-8') as f:f.write('\n\n## Task25 güncellemesi\n\n-17-isim: gerçek HTML input placeholder rengi #92999E ve opacity1; girilmiş isim #0B2A36 kalır. İki durum NIHAI/17-isim/isim-bos.png ve isim-yazildi.png.\n-15-zor-bolum: yeni character-scene.png288×192pt (@3x864×576),9-slice yok. Aynı Baron kolları kavuşturulmuş; arkadaki karışık tahta zor bölüme özel. Panel/metin/tek uyarı davranışı korunur.\n-İki kartın Task23 sürümleri kendi T25-oncesi altında; güncel inceleme NIHAI/T25-inceleme.html.\n')
 print('C: placeholder/typed states and unique Baron card exported; previous files retained')
if __name__=='__main__':run()
