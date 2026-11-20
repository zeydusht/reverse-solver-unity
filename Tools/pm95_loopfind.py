import sys, numpy as np, soundfile as sf
def feats(m, sr, hop):
    n=2048; frames=[]
    win=np.hanning(n); edges=np.geomspace(60,8000,37)
    fr=np.fft.rfftfreq(n,1/sr); idx=[(fr>=edges[i])&(fr<edges[i+1]) for i in range(36)]
    for s in range(0,len(m)-n,hop):
        sp=np.abs(np.fft.rfft(m[s:s+n]*win)); frames.append([np.log(sp[ix].sum()+1e-6) for ix in idx])
    F=np.array(frames); F-=F.mean(1,keepdims=True); F/=np.linalg.norm(F,axis=1,keepdims=True)+1e-9; return F
for f in sys.argv[1:]:
    x,sr=sf.read(f,always_2d=True); m=x.mean(1); a=np.abs(m)
    env=[np.sqrt((m[i:i+sr//4]**2).mean()) for i in range(0,len(m),sr//4)]
    print('==',f.split('/')[-1], f'{len(m)/sr:.1f}s', 'first 3s env', ' '.join(f'{e:.3f}' for e in env[:12]), '| last 3s', ' '.join(f'{e:.3f}' for e in env[-12:]))
    hop=sr//10; F=feats(m,sr,hop); T=len(F); W=80  # 8 s window
    best=[]
    for lag in range(int(55*10), min(int(125*10), T-W)):
        sim=(F[:T-lag]*F[lag:]).sum(1)
        c=np.convolve(sim,np.ones(W)/W,'valid')
        j=int(np.argmax(c)); best.append((c[j],lag/10,j/10))
    best.sort(reverse=True)
    print('   top loops (sim, length s, start s):', [(round(b[0],3),b[1],b[2]) for b in best[:6]])
