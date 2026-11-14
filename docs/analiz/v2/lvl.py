exec(open('./load.py').read())
import json
def killer(lf):
    if not isinstance(lf,str) or not lf: return None
    d={int(a):int(b) for a,b in (x.split(':') for x in lf.split(';') if ':' in x)}
    k=min(d,key=d.get); return (k,d[k],len(d))
out=[]
for _,f in FF.sort_values('slot').iterrows():
    a=A[A.slot==f.slot]; a1=a[a.attempt==1]
    n=a1.pid.nunique()
    win1=(a1.res=='w').mean()
    pw=a.groupby('pid').apply(lambda g: g.sort_values('attempt'))
    players=a.pid.unique()
    att_to_win=[];neverwin=0
    for p in players:
        g=a[a.pid==p].sort_values('attempt')
        w=g[g.res=='w']
        if len(w): att_to_win.append(int(w.attempt.min()))
        else: neverwin+=1
    # learning curve: win rate at attempt k among those who reached k
    lc={k:round((a[a.attempt==k].res=='w').mean(),2) for k in [1,2,3] if (a.attempt==k).sum()>=4}
    lc['4+']=round((a[a.attempt>=4].res=='w').mean(),2) if (a.attempt>=4).sum()>=4 else None
    b=a[a.res=='b']
    kil=[killer(x) for x in b.lf]; kil=[k for k in kil if k]
    from collections import Counter
    kc=Counter(k[0] for k in kil)
    kex=Counter(int(e) for e in b.ex.dropna())
    top_ex=kex.most_common(2)
    t=a[a.res=='t']
    w_=a[a.res=='w']
    out.append(dict(slot=int(f.slot),id=f.level_id,pieces=int(f.pieces),bombs=int(f.bombs),slack=int(f.slack_min),fuse=int(f.fuse_min),locks=int(f.locks),chains=int(f.chains),walls=int(f.walls_sides),timer=int(f.timer),spm=float(f.spm),
      n=int(n),win1=round(win1,2),
      f1_bomb=round((a1.res=='b').mean(),2),f1_time=round((a1.res=='t').mean(),2),f1_quit=round((a1.res=='q').mean(),2),
      neverwin=neverwin,att_mean=round(np.mean(att_to_win),1) if att_to_win else None,
      lc=lc,
      bomb_deaths=len(b),killers=dict(kc.most_common(3)),death_ex=top_ex,
      time_left_pieces=round(float((f.pieces - t.ex).median()),0) if len(t) else None,
      jam_per_exit_w=round(float((w_.jm/w_.ex.clip(lower=1)).median()),2) if len(w_) else None,
      jam_per_exit_l=round(float((a[a.res!='w'].jm/a[a.res!='w'].ex.clip(lower=1)).median()),2) if (a.res!='w').sum() else None,
      ft_w=int(w_[w_.attempt==1].ft.median()) if (w_.attempt==1).sum() else None,
      ft_l=int(a1[a1.res!='w'].ft.median()) if (a1.res!='w').sum() else None,
      left_med_w=int(w_.left.median()) if len(w_) else None,
      boost_w=round(float(w_.boost.mean()),2) if len(w_) else None))
json.dump(out,open('./lvl.json','w'),indent=0,default=str)
for o in out:
    print(o['slot'],o['id'],f"n{o['n']} w1 {o['win1']} b{o['f1_bomb']} t{o['f1_time']} | slack{o['slack']} fuse{o['fuse']} B{o['bombs']} | att{o['att_mean']} lc{o['lc']} | kill{o['killers']} ex{o['death_ex']} | tLeftPcs{o['time_left_pieces']} jam w{o['jam_per_exit_w']}/l{o['jam_per_exit_l']} ft {o['ft_w']}/{o['ft_l']} left{o['left_med_w']} never{o['neverwin']}")
