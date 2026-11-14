import pandas as pd, numpy as np, json
P=pd.read_pickle('P.pkl')
n=pd.Series({o['slot']:o['n'] for o in json.load(open('/home/claude/an/lvl.json'))}); P['n']=n
sg=lambda z:1/(1+np.exp(z)); P['p']=sg(P.d)
lg=lambda p: np.log((p+0.01)/(1.01-p))
W_=0.85
def prep(D):
    D=D.copy(); hb=D.bombs>0
    D['Bw']=np.where(hb,-lg(W_*D.p_survive_aware.fillna(1)+(1-W_)*D.p_survive_rand.fillna(1)),-lg(1.0))
    D['lspm']=np.log(D.sec_per_move); D['nc']=D.narrow_by_chain/D.pieces
    return D
P=prep(P); cols=['Bw','lspm','nc']
M=np.column_stack([np.ones(len(P))]+[P[c] for c in cols]); Wt=np.diag(P.n*P.p*(1-P.p)+1)
R=np.eye(4)*0.3; R[0,0]=0; b=np.linalg.solve(M.T@Wt@M+R,M.T@Wt@P.d.values)
print('coef',dict(zip(['c']+cols,np.round(b,2))))
P['pm']=sg(M@b)
# LOO
e=[]
for s in P.index:
    Z=P.drop(s); Mz=np.column_stack([np.ones(len(Z))]+[Z[c] for c in cols]); Wz=np.diag(Z.n*Z.p*(1-Z.p)+1)
    bz=np.linalg.solve(Mz.T@Wz@Mz+R,Mz.T@Wz@Z.d.values); e.append(sg(np.r_[1,[P.loc[s,c] for c in cols]]@bz))
P['ploo']=e
print('LOO MAE',round((P.ploo-P.p).abs().mean(),3))
print(P[['level_id','p','ploo','p_survive_aware','p_survive_rand','sec_per_move','nc']].round(2).to_string())
json.dump({'w':W_,'cols':cols,'b':list(b)},open('model_v2.json','w'))
F=prep(pd.read_csv('level_features_v2.csv'))
Mf=np.column_stack([np.ones(len(F))]+[F[c] for c in cols]); F['p_pred']=sg(Mf@b)
F.to_csv('pool_scored.csv',index=False)
print(F[F.set_name=='p22'].sort_values('slot')[['slot','level_id','level_version','bombs','slack_true_min','p_survive_aware','p_survive_rand','sec_per_move','p_pred']].round(2).to_string(index=False))
