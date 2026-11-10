// Approved floor revision, unchanged camera. Only a binary painting aperture is replaced.
(async()=>{try{
const img=new Image();img.src='approved-scene.png';await img.decode();
const canvas=document.querySelector('canvas'),ctx=canvas.getContext('2d',{alpha:false,willReadFrequently:true});
const P=496,cropWidth=971*390/844,start=P+(313-64)-cropWidth/2;
const aperture=[[144.5,167.5],[361.5,191.5],[356.5,507.5],[137.5,498]];
const smooth=t=>t*t*t*(t*(t*6-15)+10);
let lastTrack;
function area(poly){let sum=0;for(let i=0;i<poly.length;i++){const a=poly[i],b=poly[(i+1)%poly.length];sum+=a[0]*b[1]-a[1]*b[0];}return Math.abs(sum)/2;}
function clip(poly,W,H){for(const [axis,bound,greater]of [[0,0,true],[0,W,false],[1,0,true],[1,H,false]]){const result=[];for(let i=0;i<poly.length;i++){const a=poly[i],b=poly[(i+1)%poly.length],ina=greater?a[axis]>=bound:a[axis]<=bound,inb=greater?b[axis]>=bound:b[axis]<=bound;if(ina)result.push(a);if(ina!==inb){const t=(bound-a[axis])/(b[axis]-a[axis]);result.push([a[0]+(b[0]-a[0])*t,a[1]+(b[1]-a[1])*t]);}}poly=result;if(!poly.length)break;}return poly;}
function instance(id,camera,W,H){const sx=W/cropWidth,sy=H/971;
const quad=aperture.map(([x,y])=>[(x+id*P-camera)*sx,y*sy]);
const polygon=clip(quad,W,H),fraction=polygon.length?area(polygon)/area(quad):0;
return{id:'bay-'+id,quad,visible:fraction>0,partiallyVisible:fraction>0&&fraction<.999999,visibility:fraction<=0?'hidden':fraction<.999999?'partial':'full',visibleFraction:fraction,clippedPolygon:polygon};}
function inside(x,y,quad){let positive=false,negative=false;for(let j=0;j<4;j++){const a=quad[j],b=quad[(j+1)%4],cross=(b[0]-a[0])*(y-a[1])-(b[1]-a[1])*(x-a[0]);positive ||=cross>0;negative ||=cross<0;}return!(positive&&negative);}
function render(p,W=780,H=1688){canvas.width=W;canvas.height=H;const travel=P*smooth(p),phase=travel-Math.floor(travel/P)*P;
ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';
ctx.drawImage(img,start+phase,0,cropWidth,971,0,0,W,H);
const before=ctx.getImageData(0,0,W,H),after=ctx.createImageData(W,H);after.data.set(before.data);
const instances=[instance(1,start+travel,W,H),instance(2,start+travel,W,H)];let count=0;
for(const frame of instances){if(!frame.visible)continue;const q=frame.quad;
const x0=Math.max(0,Math.floor(Math.min(...q.map(v=>v[0])))),x1=Math.min(W-1,Math.ceil(Math.max(...q.map(v=>v[0]))));
const y0=Math.max(0,Math.floor(Math.min(...q.map(v=>v[1])))),y1=Math.min(H-1,Math.ceil(Math.max(...q.map(v=>v[1]))));
for(let y=y0;y<=y1;y++)for(let x=x0;x<=x1;x++)if(inside(x+.5,y+.5,q)){const n=(y*W+x)*4;after.data[n]=0;after.data[n+1]=255;after.data[n+2]=0;after.data[n+3]=255;count++;}}
ctx.putImageData(after,0,0);
const primary=instances.reduce((a,b)=>a.visibleFraction>=b.visibleFraction?a:b);
lastTrack={quad:primary.quad,visible:primary.visible,partiallyVisible:primary.partiallyVisible,frameId:primary.id,instances,greenPixelCount:count};
return lastTrack;
}
render(0);
window.task18={ready:true,aperture,render,png(){return canvas.toDataURL('image/png');},track(){return lastTrack;},backgroundPNG(p,W=780,H=1688){canvas.width=W;canvas.height=H;const d=P*smooth(p),phase=d-Math.floor(d/P)*P;ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';ctx.drawImage(img,start+phase,0,cropWidth,971,0,0,W,H);return canvas.toDataURL('image/png');}};
}catch(e){window.task18Error=e.stack;}})();
