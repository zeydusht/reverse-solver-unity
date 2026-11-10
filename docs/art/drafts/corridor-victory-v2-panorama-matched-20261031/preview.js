// One static scene with identical neighboring bays. No time-varying image blend.
(async()=>{try{
const img=new Image();img.src='matched-scene.png';await img.decode();
const canvas=document.querySelector('canvas'),ctx=canvas.getContext('2d',{alpha:false,willReadFrequently:true});
const period=496,cropWidth=971*390/844,start=period+(313-64)-cropWidth/2;
let running=false,epoch=0,cycles=1;
const smooth=t=>t*t*t*(t*(t*6-15)+10);
function render(p){const travel=period*smooth(p),phase=travel-Math.floor(travel/period)*period;
ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';
ctx.drawImage(img,start+phase,0,cropWidth,971,0,0,canvas.width,canvas.height);
document.querySelector('#progress').value=p;document.querySelector('#position').textContent=(p*3).toFixed(2)+' sn';}
function play(n){cycles=n;running=true;epoch=performance.now();render(0);}
document.querySelector('#play').onclick=()=>play(1);document.querySelector('#three').onclick=()=>play(3);
document.querySelector('#progress').oninput=e=>{running=false;render(+e.target.value);};
function tick(now){if(running){const t=(now-epoch)/3000;if(t>=cycles){render(1);running=false;}else render(t-Math.floor(t));}requestAnimationFrame(tick);}render(0);requestAnimationFrame(tick);
window.bayDemo={ready:true,setProgress(p){running=false;render(p);},png(){return canvas.toDataURL('image/png');},
metrics(){const get=p=>{render(p);return ctx.getImageData(0,0,canvas.width,canvas.height).data;};const a=get(0),b=get(1);let max=0,sum=0;for(let i=0;i<a.length;i++){const v=Math.abs(a[i]-b[i]);max=Math.max(max,v);sum+=v;}return{source:'original first bay x64..560; complete native pillar over wall join; locally repaired static floor',sourceSize:[1488,971],period,cropWidth,cropHeight:971,cropY:0,startCropX:start,endCropX:start+period,travelSourcePixels:period,travelAdjacentPaintings:1,viewport:[780,1688],duration:3,easing:'same quintic smoothstep as approved panorama-single-step',zoom:false,perspectiveTransform:false,temporalBlending:false,endpointMaxChannelError:max,endpointMeanChannelError:sum/a.length,loopOnlyForReview:true};}};
}catch(e){window.bayError=e.stack;document.querySelector('#error').textContent=e.message;}})();
