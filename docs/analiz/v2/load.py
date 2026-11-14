import pandas as pd, numpy as np, re
fc="slot,level_id,ver,w,h,cells,pieces,chains,bombs,fuse_min,fuse_mean,slack_min,slack_mean,locks,lock_max,walls_lanes,walls_sides,timer,sol_len,spm,open_edge,genre,hard,boosters".split(',')
F=pd.DataFrame([r.split(',') for r in open('./feat.txt').read().strip().split('\n')],columns=fc)
for c in fc:
    if c not in('level_id','genre','hard'): F[c]=pd.to_numeric(F[c])
F['hard']=F.hard=='t'
ac="pid,slot,ver,attempt,res,dur,left,moves,jams,boost".split(',')
A=pd.DataFrame([r.split(',') for r in open('./att.txt').read().strip().split(';')],columns=ac)
for c in ac:
    if c!='res': A[c]=pd.to_numeric(A[c])
A['order']=range(len(A))
s=open('./mov.txt').read()
pat=re.compile(r'(\d+),([SZ]\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),([\d:;]*?)(?=;\d+,[SZ]\d+,|$)')
rows=[m.groups() for m in pat.finditer(s)]
M=pd.DataFrame(rows,columns="pid,level_id,attempt,ex,jm,tc,bo,ft,tmax,lf".split(','))
for c in "pid,attempt,ex,jm,tc,bo,ft,tmax".split(','): M[c]=pd.to_numeric(M[c])
# map slot->level (pt1 versions played)
lv=A.groupby(['slot','ver']).size().reset_index()[['slot','ver']]
FF=F.merge(lv,on=['slot','ver'])
A=A.merge(FF[['slot','level_id']],on='slot',how='left')
A=A.merge(M,on=['pid','level_id','attempt'],how='left')
