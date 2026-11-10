// One fixed, periodically addressed illustration. No perspective warping or animated blends.
(async()=>{try{
const img=new Image();img.src='module-source.png';await img.decode();
const overlap=32,period=img.width-overlap,h=img.height;
const read=document.createElement('canvas');read.width=img.width;read.height=h;
const rc=read.getContext('2d',{willReadFrequently:true});rc.drawImage(img,0,0);
const raw=rc.getImageData(0,0,img.width,h).data;
const tile=document.createElement('canvas');tile.width=period;tile.height=h;
const tc=tile.getContext('2d');const data=tc.createImageData(period,h);
// Fixed spatial seam in the unoccupied 32px edge strip: no frames, plant or column.
// The same periodic material applies to wall, dado, floor and carpet together.
for(let y=0;y<h;y++)for(let x=0;x<period;x++){
const a=(y*img.width+x)*4,b=(y*img.width+period+x)*4,o=(y*period+x)*4;
const u=x/overlap,k=x<overlap?u*u*(3-2*u):1;
for(let c=0;c<3;c++)data.data[o+c]=x<overlap?Math.round(raw[b+c]*(1-k)+raw[a+c]*k):raw[a+c];
data.data[o+3]=255;
}tc.putImageData(data,0,0);
// Materialize consecutive bays, including guard bays: interpolation crosses identical pixels.
const strip=document.createElement('canvas');strip.width=period*4;strip.height=h;
const sc=strip.getContext('2d');for(let i=0;i<4;i++)sc.drawImage(tile,i*period,0);
const canvas=document.querySelector('#view'),ctx=canvas.getContext('2d',{alpha:false,willReadFrequently:true});
const cropWidth=h*390/844,paintingCenter=594,start=period+paintingCenter-cropWidth/2;
let running=true,epoch=performance.now(),current=0;
const smooth=t=>t*t*t*(t*(t*6-15)+10);
function render(p){current=p;ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';
// Canonical periodic addressing avoids Canvas subpixel rounding depending on bay index.
// One period is the identical, already aligned scene material, not a different crop reset.
const distance=period*smooth(p),phase=distance-Math.floor(distance/period)*period;
ctx.drawImage(strip,start+phase,0,cropWidth,h,0,0,canvas.width,canvas.height);
document.querySelector('#progress').value=p;document.querySelector('#position').textContent=(p*3).toFixed(2)+' sn · '+Math.round(p*100)+'%';}
const pause=()=>{running=false;document.querySelector('#play').textContent='Döngüyü oynat';};
document.querySelector('#play').onclick=()=>{if(running)pause();else{running=true;epoch=performance.now()-current*3000;document.querySelector('#play').textContent='Döngüyü durdur';}};
document.querySelector('#progress').oninput=e=>{pause();render(+e.target.value);};
function tick(now){if(running)render(((now-epoch)%3000)/3000);requestAnimationFrame(tick);}render(0);requestAnimationFrame(tick);
window.bayDemo={ready:true,period,height:h,cropWidth,start,paintingCenter,overlap,
setProgress(p){pause();render(p);},png(){return canvas.toDataURL('image/png');},tilePNG(){return tile.toDataURL('image/png');},
panoramaPNG(){const c=document.createElement('canvas');c.width=Math.ceil(cropWidth+period);c.height=h;c.getContext('2d').drawImage(strip,start,0,c.width,h,0,0,c.width,h);return c.toDataURL('image/png');},
metrics(){const get=p=>{render(p);return ctx.getImageData(0,0,canvas.width,canvas.height).data;};const a=get(0),b=get(1);let max=0,sum=0;for(let i=0;i<a.length;i++){const e=Math.abs(a[i]-b[i]);max=Math.max(max,e);sum+=e;}return{endpointMaxChannelError:max,endpointMeanChannelError:sum/a.length,period,cropWidth,travelBays:1,travelViewportWidths:period/cropWidth,viewport:[canvas.width,canvas.height],intermediateSource:'one fixed periodic illustration; no per-frame generation',duration:3};}};
}catch(e){window.bayError=e.stack;document.querySelector('#error').textContent=e.message;}})();
