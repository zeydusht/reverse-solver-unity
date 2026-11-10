from pathlib import Path
from PIL import Image,ImageDraw
p=Path('C:/Users/zeydt/.codex/generated_images/01a19450-e379-7620-829d-edee0958153c')
files=sorted(p.glob('*.png'),key=lambda f:f.stat().st_mtime,reverse=True)[:7]
im=Image.new('RGB',(1400,800),'#61727A');d=ImageDraw.Draw(im)
for i,f in enumerate(files):
 a=Image.open(f).convert('RGBA');a.thumbnail((340,350));x=i%4*350;y=i//4*400;im.paste(a,(x,y),a);d.text((x+2,y+360),f.name[:16],fill='white')
im.save('docs/art/task21/sources-contact.png')
