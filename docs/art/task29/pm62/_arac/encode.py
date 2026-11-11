from pathlib import Path
from PIL import Image
import json

ROOT = Path(__file__).resolve().parent
names = ['saat','makas','kazanma','jigo_gecis','harita_hap']
for name in names:
    folder = ROOT/name
    paths = sorted(folder.glob('*.png'))
    if not paths:
        continue
    times = json.loads((folder/'times.json').read_text())
    frames = [Image.open(p).convert('RGB') for p in paths]
    durations = [max(20,round((times[i+1]-times[i])/10)*10) for i in range(len(times)-1)]+[800]
    dest = ROOT.parent/(name+'.gif')
    sample = Image.new('RGB',(390,844))
    for i in range(16):
        sample.paste(frames[min(len(frames)-1,i*len(frames)//16)].resize((97,211)),((i%4)*97,(i//4)*211))
    for colors in [128,96,64,48,32]:
        palette = sample.quantize(colors=colors,method=Image.Quantize.FASTOCTREE)
        converted = [f.quantize(palette=palette,dither=Image.Dither.NONE) for f in frames]
        converted[0].save(dest,save_all=True,append_images=converted[1:],duration=durations,loop=0,optimize=True,disposal=1)
        if dest.stat().st_size <= 2500000:
            break
    with Image.open(dest) as gif:
        assert gif.size==(390,844),gif.size
        assert gif.n_frames>1
        assert dest.stat().st_size<=2500000
        print(f'{name}.gif: {dest.stat().st_size} bytes, {gif.size}, {gif.n_frames} frames, {colors} colors',flush=True)
