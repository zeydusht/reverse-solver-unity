exec(open('./load.py').read())
import json, itertools
J=json.load(open('./irt.json')); d=pd.Series({int(k):v for k,v in J['d'].items()})
X=FF.set_index('slot').sort_index().copy()
X['d']=d
X['hasB']=(X.bombs>0).astype(float)
X['tight']=np.where(X.bombs>0,np.exp(-X.slack_min.clip(lower=0)/2.0),0)
X['ffrac']=np.where(X.bombs>0,X.fuse_min/X.pieces,1.0)
X['early']=np.where(X.bombs>0,1-X.fuse_min/X.pieces,0)  # bomb must go early
X['nb']=X.bombs
X['lspm']=np.log(X.spm)
X['press']=np.clip(2.2-X.spm,0,None)  # time pressure below 2.2 s/move
X['lpieces']=np.log(X.pieces)
X['lk']=X.locks; X['ch']=X.chains; X['wl']=X.walls_sides
cands=['hasB','tight','early','nb','press','lpieces','lk','ch','wl','lspm']
def fit(cols,lam=1.0,idx=None):
    Z=X if idx is None else X.loc[idx]
    M_=np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])
    R=np.eye(M_.shape[1])*lam; R[0,0]=0
    b=np.linalg.solve(M_.T@M_+R,M_.T@Z.d.values); return b
def pred(b,cols,Z): return np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])@b
def loo(cols,lam=1.0):
    e=[]
    for s in X.index:
        idx=[i for i in X.index if i!=s]; b=fit(cols,lam,idx)
        e.append(pred(b,cols,X.loc[[s]])[0]-X.loc[s,'d'])
    return np.array(e)
best=[]
for k in range(1,6):
    for cols in itertools.combinations(cands,k):
        e=loo(list(cols)); best.append((np.sqrt(np.mean(e**2)),cols))
best.sort()
base=np.sqrt(np.mean((X.d-X.d.mean())**2))
print('baseline rmse',round(base,2))
for r,c in best[:12]: print(round(r,3),c)
cols=['tight','early','lspm']
b=fit(cols); print('coef',dict(zip(['c']+cols,np.round(b,2))))
X['dhat']=pred(b,cols,X); X['res_']=X.d-X.dhat
e=loo(cols); X['loo']=e
sg=lambda z:1/(1+np.exp(z))
X['p_hat']=sg(X.dhat); X['p_irt']=sg(X.d)
print(X[['level_id','bombs','slack_min','fuse_min','pieces','spm','p_irt','p_hat','res_']].round(2).to_string())
X[['level_id','d','dhat','res_','p_irt','p_hat','tight','early','lspm']].to_csv('./reg_out.csv')
