// Original image, one constant-size source rectangle, horizontal translation only.
(async()=>{try{
const img=new Image();img.src='original-panorama.png';await img.decode();
const canvas=document.querySelector('canvas'),ctx=canvas.getContext('2d',{alpha:false,willReadFrequently:true});
const cropWidth=img.height*390/844,centers=[313,809],distance=centers[1]-centers[0],start=centers[0]-cropWidth/2;
let running=false,epoch=0;
const smooth=t=>t*t*t*(t*(t*6-15)+10);
function render(p){ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';
ctx.drawImage(img,start+distance*smooth(p),0,cropWidth,img.height,0,0,canvas.width,canvas.height);
document.querySelector('#progress').value=p;document.querySelector('#position').textContent=(p*3).toFixed(2)+' sn';}
document.querySelector('#play').onclick=()=>{running=true;epoch=performance.now();render(0);};
document.querySelector('#progress').oninput=e=>{running=false;render(+e.target.value);};
function tick(now){if(running){const p=Math.min(1,(now-epoch)/3000);render(p);if(p===1)running=false;}requestAnimationFrame(tick);}render(0);requestAnimationFrame(tick);
window.bayDemo={ready:true,setProgress(p){running=false;render(p);},png(){return canvas.toDataURL('image/png');},
metrics(){return{source:'docs/art/drafts/corridor-victory-v2-panorama-20261030/panorama-clean.png',sourceSize:[img.width,img.height],cropWidth,cropHeight:img.height,cropY:0,startCropX:start,middleCropX:start+distance/2,endCropX:start+distance,paintingCenters:centers,travelSourcePixels:distance,travelAdjacentPaintings:1,viewport:[canvas.width,canvas.height],duration:3,zoom:false,perspectiveTransform:false,redraw:false,loop:false,sourceAltered:false};}};
}catch(e){window.bayError=e.stack;document.querySelector('#error').textContent=e.message;}})();
