from pathlib import Path
import sys
root=Path(__file__).resolve().parent
sys.path.insert(0,str(root.parents[1]/'.task16-python'))
from PIL import Image,ImageDraw,ImageFont
im=Image.open(root/'approved-bay.png').convert('RGB')
for name,box in [('top',(122,155,384,213)),('bottom',(114,485,386,532)),('left',(119,165,152,523)),('right',(352,181,380,523))]:
    crop=im.crop(box);scale=5
    out=Image.new('RGB',(crop.width*scale,crop.height*scale+35),'#102b33');out.paste(crop.resize((crop.width*scale,crop.height*scale),Image.Resampling.NEAREST),(0,35));d=ImageDraw.Draw(out);f=ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf',16)
    if crop.width>crop.height:
        for x in range(box[0],box[2],10):d.text(((x-box[0])*scale,4),str(x),font=f,fill='white');d.line(((x-box[0])*scale,35,(x-box[0])*scale,out.height),fill='#ffffff44')
    else:
        for y in range(box[1],box[3],10):d.text((4,35+(y-box[1])*scale),str(y),font=f,fill='white')
    out.save(root/f'aperture-{name}-guide.png')
corners=[('TL',(136,160,153,180)),('TR',(354,184,371,204)),('BL',(130,489,147,509)),('BR',(347,493,364,513))]
board=Image.new('RGB',(520,600),'#102b33');d=ImageDraw.Draw(board);f=ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf',14)
for j,(label,box) in enumerate(corners):
    x0=25+(j%2)*260;y0=55+(j//2)*300;sc=12
    board.paste(im.crop(box).resize(((box[2]-box[0])*sc,(box[3]-box[1])*sc),Image.Resampling.NEAREST),(x0,y0))
    d.text((x0,y0-50),label+' '+str(box),font=f,fill='white')
    for x in range(box[0],box[2],2):d.text((x0+(x-box[0])*sc,y0-22),str(x),font=f,fill='white')
    for y in range(box[1],box[3],2):d.text((x0,y0+(y-box[1])*sc),str(y),font=f,fill='white')
board.save(root/'aperture-corners-guide.png')
