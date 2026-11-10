from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps
import json,shutil,hashlib
ROOT=Path(__file__).resolve().parents[2];O=ROOT/'Assets/Art/Redesign_T22';W=ROOT/'docs/art/task22';R=Image.Resampling.LANCZOS
p=Path(json.loads((W/'node-v2-source.json').read_text(encoding='utf-8'))['source']);q=O/'ai-kaynaklar/node-v2.png'
if not q.exists():shutil.copyfile(p,q)
a=Image.open(q).convert('RGBA');box=a.getchannel('A').point(lambda v:255 if v>=24 else 0).getbbox();a=a.crop(box).resize((198,198),R);a.save(O/'110-ai-harita/node-current-@3x.png')
mapping=json.loads((W/'ai-sources.json').read_text(encoding='utf-8'));mapping['node-v2']=str(p);(W/'ai-sources.json').write_text(json.dumps(mapping,ensure_ascii=False,indent=2),encoding='utf-8')
shot=Image.open(O/'110-ai-harita/ekran-sonra-375.png').convert('RGBA');shot.alpha_composite(a,(270,1146));d=ImageDraw.Draw(shot);f=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),66);d.text((369,1249), '13',font=f,anchor='mm',fill='#0B2A36',stroke_width=3,stroke_fill='#0B2A36');d.text((369,1242),'13',font=f,anchor='mm',fill='#FFF6D8',stroke_width=3,stroke_fill='#0B2A36');shot.save(O/'110-ai-harita/ekran-sonra-375.png')
old=Image.open(ROOT/'docs/qa/2026-11-02-son/375x667_harita.png').convert('RGBA').resize((1125,2001),R);b=(69,864,561,1455);left=old.crop(b);right=shot.crop(b);pair=Image.new('RGB',(left.width*2+30,left.height+90),'#173D49');pair.paste(left,(0,75));pair.paste(right,(left.width+30,75));d=ImageDraw.Draw(pair);f=ImageFont.truetype(str(ROOT/'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),30);d.text((20,12),'ÖNCE · kaynak ekran',font=f,fill='#FFF6D8');d.text((left.width+50,12),'SONRA · onaysız taslak',font=f,fill='#FFF6D8');pair.save(O/'110-ai-harita/once-sonra.png')
mf=O/'ai-manifest.json';rs=json.loads(mf.read_text(encoding='utf-8'));row=next(r for r in rs if r['file']=='110-ai-harita/node-current-@3x.png');row['source']='imagegen node-v2:2.deneme, ince altın kenar';row['sha256']=hashlib.sha256((O/row['file']).read_bytes()).hexdigest();mf.write_text(json.dumps(rs,ensure_ascii=False,indent=2),encoding='utf-8')
print('node-v2 exported; old node.png retained')
