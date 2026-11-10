'use strict';
(async()=>{
const canvas=document.querySelector('canvas'),status=document.querySelector('#status'),slider=document.querySelector('#time');
const gl=canvas.getContext('webgl',{alpha:false,antialias:false,preserveDrawingBuffer:true});
if(!gl)throw Error('WebGL unavailable');
const shared=await(await fetch('../../../Assets/_Game/Presentation/Resources/CorridorVictory/CorridorProjection.cginc')).text();
const vertex='attribute vec2 position;varying vec2 uv;void main(){uv=position*.5+.5;gl_Position=vec4(position,0.,1.);}';
const fragment=`precision highp float;
#define float2 vec2
#define float3 vec3
#define float4 vec4
#define tex2D texture2D
#define lerp mix
#define frac fract
uniform sampler2D _MainTex,_Wall,_Floor,_Frame,_Columns,_Picture,_Arches;
uniform float _Progress,_HasPicture;uniform vec3 _CameraOffset;uniform vec2 _PictureSize,cover;varying vec2 uv;
${shared}
void main(){gl_FragColor=Corridor((uv-.5)*cover+.5);}`;
function shader(type,source){const s=gl.createShader(type);gl.shaderSource(s,source);gl.compileShader(s);if(!gl.getShaderParameter(s,gl.COMPILE_STATUS))throw Error(gl.getShaderInfoLog(s));return s;}
const program=gl.createProgram();gl.attachShader(program,shader(gl.VERTEX_SHADER,vertex));gl.attachShader(program,shader(gl.FRAGMENT_SHADER,fragment));gl.linkProgram(program);if(!gl.getProgramParameter(program,gl.LINK_STATUS))throw Error(gl.getProgramInfoLog(program));gl.useProgram(program);
const quad=gl.createBuffer();gl.bindBuffer(gl.ARRAY_BUFFER,quad);gl.bufferData(gl.ARRAY_BUFFER,new Float32Array([-1,-1,1,-1,-1,1,-1,1,1,-1,1,1]),gl.STATIC_DRAW);const pos=gl.getAttribLocation(program,'position');gl.enableVertexAttribArray(pos);gl.vertexAttribPointer(pos,2,gl.FLOAT,false,0,0);
const loc=name=>gl.getUniformLocation(program,name);
const paths=['../../../Assets/Art/Menu/CorridorVictory/reference.png','../../../Assets/Art/Menu/CorridorVictory/wall.png','../../../Assets/Art/Menu/CorridorVictory/floor.png','../../../Assets/Art/Menu/CorridorVictory/frame.png','../../../Assets/Art/Menu/CorridorVictory/columns.png','../../../Assets/Art/Paintings/painting-01.jpg'];
paths.push('../../../Assets/Art/Menu/CorridorVictory/arches.png');
const names=['_MainTex','_Wall','_Floor','_Frame','_Columns','_Picture','_Arches'];
await Promise.all(paths.map((src,i)=>new Promise((resolve,reject)=>{const im=new Image();im.onload=()=>{gl.activeTexture(gl.TEXTURE0+i);const t=gl.createTexture();gl.bindTexture(gl.TEXTURE_2D,t);gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL,true);gl.texImage2D(gl.TEXTURE_2D,0,gl.RGBA,gl.RGBA,gl.UNSIGNED_BYTE,im);gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_MIN_FILTER,gl.LINEAR);gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_MAG_FILTER,gl.LINEAR);gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_WRAP_S,gl.CLAMP_TO_EDGE);gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_WRAP_T,gl.CLAMP_TO_EDGE);gl.uniform1i(loc(names[i]),i);resolve();};im.onerror=()=>reject(Error(src));im.src=src;})));
let running=false,start=0,p=0,frames=0,completed=0,raf=0,previous=0,gaps=[];
function ease(t){return t*t*t*(t*(t*6-15)+10);}
function render(progress){p=Math.max(0,Math.min(1,progress));const t=ease(p),pull=Math.sin(Math.PI*t);gl.uniform1f(loc('_Progress'),p);gl.uniform3f(loc('_CameraOffset'),-3.42*t,0,-2.0007*t-.32*pull*pull);gl.uniform1f(loc('_HasPicture'),document.querySelector('#picture').checked?1:0);gl.uniform2f(loc('_PictureSize'),1024,1024);gl.drawArrays(gl.TRIANGLES,0,6);frames++;slider.value=p;}
function resize(){const r=canvas.getBoundingClientRect(),d=Math.min(2,devicePixelRatio||1);canvas.width=Math.round(r.width*d);canvas.height=Math.round(r.height*d);gl.viewport(0,0,canvas.width,canvas.height);const ratio=r.width/r.height/(1024/1536);gl.uniform2f(loc('cover'),Math.min(1,ratio),Math.min(1,1/ratio));render(p);}
function tick(now){if(!running)return;if(previous)gaps.push(now-previous);previous=now;render((now-start)/3000);if(p>=1){running=false;completed++;status.textContent='Next identical bay. Static until another victory.';return;}raf=requestAnimationFrame(tick);}
function play(){if(running)return false;running=true;start=performance.now();previous=0;gaps=[];render(0);status.textContent='Victory · travelling to the next bay…';raf=requestAnimationFrame(tick);return true;}
function idle(){running=false;cancelAnimationFrame(raf);render(0);status.textContent='Ordinary menu return · static. No victory, no movement.';}
document.querySelector('#win').onclick=play;document.querySelector('#idle').onclick=idle;slider.oninput=()=>{running=false;cancelAnimationFrame(raf);render(Number(slider.value));status.textContent=`Inspecting ${(p*3).toFixed(2)} / 3.00 seconds`;};document.querySelector('#picture').onchange=()=>render(p);window.addEventListener('resize',resize);
window.corridorDemo={ready:true,play,idle,setProgress(v){running=false;cancelAnimationFrame(raf);render(v);},get state(){return{running,p,frames,completed,gaps};},png(){return canvas.toDataURL('image/png');}};
resize();status.textContent='Menu is static. Simulate a victory to begin.';
})().catch(e=>{document.querySelector('#status').textContent=e.message;window.corridorError=e.stack;console.error(e);});
