from pathlib import Path
from PIL import Image, ImageDraw
import json

root=Path(__file__).resolve().parent
sheet=Image.new('RGB',(780,1080),'white')
draw=ImageDraw.Draw(sheet)
for row,name in enumerate(['saat','makas','jigo_gecis','kazanma','harita_hap']):
    p=root.parent/(name+'.gif')
    gif=Image.open(p)
    dur=0
    for n in range(gif.n_frames):
        gif.seek(n);dur+=gif.info['duration']
    for col,fraction in enumerate([0,.2,.4,.65,.85,1]):
        gif.seek(round((gif.n_frames-1)*fraction))
        frame=gif.convert('RGB')
        if name in ['saat','jigo_gecis']:
            frame=frame.crop((0,0,390,185))
        thumb=frame.copy();thumb.thumbnail((130,180))
        sheet.paste(thumb,(col*130,row*216+25))
    draw.text((5,row*216+5),f'{name}: {p.stat().st_size} bytes, {dur/1000:.2f}s',fill='black')
sheet.save(root/'verification.png')
