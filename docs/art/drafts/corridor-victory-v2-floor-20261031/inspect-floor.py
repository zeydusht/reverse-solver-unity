from pathlib import Path
import sys
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageFont
im=Image.open(root/'original-panorama.png').convert('RGB')
im.crop((0,720,1000,900)).resize((2000,360)).save(root/'floor-source-detail.png')
im.crop((0,750,800,835)).resize((2400,255)).save(root/'border-source-detail.png')
im.crop((150,730,650,790)).resize((2000,240)).save(root/'marble-source-detail.png')
rot=im.crop((0,748,900,850)).rotate(-2.6,resample=Image.Resampling.BICUBIC,expand=True)
rot.save(root/'border-rotated.png')
guide=Image.new('RGB',(rot.width*2,rot.height*2+40),'#102b33');guide.paste(rot.resize((rot.width*2,rot.height*2)),(0,40));d=ImageDraw.Draw(guide);f=ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf',18)
for x in range(0,rot.width,50):d.text((x*2,4),str(x),font=f,fill='white');d.line((x*2,40,x*2,guide.height),fill='#ffffff44')
guide.save(root/'border-rotated-guide.png')
im.crop((506,715,615,745)).resize((872,240),Image.Resampling.NEAREST).save(root/'base-contact-source.png')
