// Shared by the Unity material and the WebGL prototype (type aliases only).
float2 ProjectPoint(float3 p) {
    return float2(.5 + 1.6083984375*p.x/p.z, .5442708333 + 1.072265625*p.y/p.z);
}
float3 RepeatBay(float3 p) {
    float section=floor((p.x+1.4)/3.42);
    return p-float3(3.42,0.,2.0007)*section;
}
float3 WallPoint(float3 ray,float offset) {
    float distance=(5.32-offset+.585*_CameraOffset.x-_CameraOffset.z)/(ray.z-.585*ray.x);
    return _CameraOffset+distance*ray;
}
float4 SafeSample(sampler2D image,float2 uv) {
    if(uv.x<0. || uv.x>1. || uv.y<0. || uv.y>1.)return float4(0.,0.,0.,0.);
    return tex2D(image,uv);
}
float Cross2(float2 a,float2 b) {return a.x*b.y-a.y*b.x;}
float4 WithPicture(float4 background,float2 uv) {
    if(_HasPicture<.5)return background;
    float2 p=float2(uv.x*1024.,(1.-uv.y)*1536.);
    if(Cross2(float2(313.,63.),p-float2(349.,296.))<0. ||
       Cross2(float2(1.,473.),p-float2(662.,359.))<0. ||
       Cross2(float2(-315.,-17.),p-float2(663.,832.))<0. ||
       Cross2(float2(1.,-519.),p-float2(348.,815.))<0.)return background;
    float2 v=float2((p.x-348.)/315.,1.-(p.y-296.)/536.);
    float ratio=_PictureSize.x/_PictureSize.y, target=315./536.;
    float2 crop=float2(min(1.,target/ratio),min(1.,ratio/target));
    return tex2D(_Picture,(v-.5)*crop+.5);
}
float4 CompositeOver(float4 back,float4 front) {return float4(lerp(back.rgb,front.rgb,front.a),1.);}
float InPhoto(float2 uv) {return step(0.,uv.x)*step(uv.x,1.)*step(0.,uv.y)*step(uv.y,1.);}
// Intersect real side faces of the two relief columns in a repeated wall bay.
float PillarBound(float pixel) {
    float r=(pixel-512.)/1647.;
    return 5.12*r/(1.-.585*r);
}
float SideDistance(float3 ray,float x) {
    if(abs(ray.x)<.00001)return 100000.;
    float t=(x-_CameraOffset.x)/ray.x;
    float3 p=_CameraOffset+t*ray;
    float back=5.32+.585*x;
    if(t<=0. || p.y< -1.5 || p.y>1.45 || p.z<back-.2 || p.z>back)return 100000.;
    return t;
}
float3 MarbleSide(float3 p,float shade) {
    float back=5.32+.585*p.x;
    float u=.772+clamp((back-p.z)/.2,0.,1.)*.012;
    float sourceY=700.-1647.*p.y/5.18;
    if(sourceY>940.)sourceY=1880.-sourceY;
    sourceY=clamp(sourceY,280.,940.);
    float v=1.-sourceY/1536.;
    return tex2D(_Source,clamp(float2(u,v),float2(0.,0.),float2(1.,1.))).rgb*shade;
}
float4 Corridor(float2 uv) {
    if(_Progress<=0. || _Progress>=1.)return WithPicture(tex2D(_MainTex,uv),uv);
    float3 ray=float3((uv.x-.5)*.6217364906,(uv.y-.5442708333)*.932604736,1.);
    float3 wall=WallPoint(ray,0.);
    float wallDepth=wall.z-_CameraOffset.z;
    float nearest=wallDepth;
    float3 local=RepeatBay(wall);
    float2 v=float2((local.x+1.4)/3.42,(local.y+1.5)/5.);
    float4 result=tex2D(_Wall,clamp(v,float2(.0001,.0001),float2(.9999,.9999)));
    float2 photo=ProjectPoint(local);
    float known=tex2D(_Arches,clamp(v,float2(0.,0.),float2(1.,1.))).r;
    // The skirting is a level physical band around the entire continuous wall.
    known*=smoothstep(-.70,-.50,local.y);
    result.rgb=lerp(result.rgb,tex2D(_Source,clamp(photo,float2(0.,0.),float2(1.,1.))).rgb,known*InPhoto(photo));
    result.a=1.;
    if(ray.y<-.0001) {
        float t=-1.5/ray.y;
        if(t>0. && t<nearest) {
            float3 p=_CameraOffset+t*ray;
            // A single orthonormal floor basis, shared by carpet, borders and grout.
            float norm=1.1585443453;
            float s=(p.x+.585*p.z)/norm;
            float n=(p.z-.585*p.x)/norm;
            float2 fuv=float2(frac(s/(3.42*norm)),(n-1.5/norm)/(4./norm));
            result=float4(tex2D(_Floor,float2(fuv.x,clamp(fuv.y,.0001,.9999))).rgb,1.);
            float dx=(frac((p.x+1.22)/3.42+.5)-.5)*3.42;
            float dn=p.z-.585*p.x-4.87;
            result.rgb*=1.-.17*exp(-(dx*dx/.0484+dn*dn/.01));
            nearest=t;
        }
    }
    float3 frameWorld=WallPoint(ray,.08);
    float3 framePoint=RepeatBay(frameWorld);
    float2 frameUV=ProjectPoint(framePoint);
    float4 frame=SafeSample(_Frame,float2((framePoint.x+1.4)/3.42,(framePoint.y+1.5)/5.));
    frame.rgb=tex2D(_Source,clamp(frameUV,float2(0.,0.),float2(1.,1.))).rgb;
    if(frame.a>.5)frame=WithPicture(frame,frameUV);
    if(frameWorld.z-_CameraOffset.z<nearest){result=CompositeOver(result,frame);if(frame.a>.99)nearest=frameWorld.z-_CameraOffset.z;}
    float section=floor((wall.x+1.4)/3.42);
    for(int bay=-1;bay<=1;bay++) {
        float shift=3.42*(section+float(bay));
        for(int pillar=0;pillar<2;pillar++) {
            float left=PillarBound(pillar==0?-48.:775.)+shift;
            float right=PillarBound(pillar==0?140.:855.)+shift;
            float a=SideDistance(ray,left),b=SideDistance(ray,right);
            float t=min(a,b);
            if(t<nearest){float3 p=_CameraOffset+t*ray;result=float4(MarbleSide(p,.93),1.);nearest=t;}
        }
    }
    float3 columnWorld=WallPoint(ray,.2);
    float columnDepth=columnWorld.z-_CameraOffset.z;
    float3 columnPoint=RepeatBay(columnWorld);
    if(columnPoint.y>=-1.5 && columnPoint.y<=1.45 && columnDepth<=nearest+.0001) {
        float widen=columnPoint.y< -1.30?.04:0.;
        float x=columnPoint.x;
        bool body=(x>=PillarBound(-48.)-widen && x<=PillarBound(140.)+widen)||(x>=PillarBound(775.)-widen && x<=PillarBound(855.)+widen)||(x-3.42>=PillarBound(-48.)-widen && x-3.42<=PillarBound(140.)+widen);
        if(body){result=float4(MarbleSide(columnWorld,1.),1.);nearest=columnDepth;}
    }
    if(columnPoint.y>=1.40 && columnPoint.y<=1.65 && columnDepth<=nearest+.0001) {
        float x=columnPoint.x;
        float flare=.025*sin(3.14159265*clamp((columnPoint.y-1.40)/.25,0.,1.));
        bool capital=(x>=PillarBound(-48.)-flare && x<=PillarBound(0.))||(x-3.42>=PillarBound(-48.)-flare && x-3.42<=PillarBound(0.));
        float capY=260.-clamp((columnPoint.y-1.40)/.25,0.,1.)*65.;
        float capX=x>1.6?x-3.42:x;
        float capU=790.+20.*clamp((capX-PillarBound(-48.))/(PillarBound(0.)-PillarBound(-48.)),0.,1.);
        if(capital){result=float4(tex2D(_Source,float2(capU/1024.,1.-capY/1536.)).rgb,1.);nearest=columnDepth;}
    }
    float2 columnUV=float2((columnPoint.x+1.4)/3.42,(columnPoint.y+1.5)/5.);
    columnUV.y=min(columnUV.y,.9999);
    float4 column=SafeSample(_Columns,float2(columnUV.x*.5,columnUV.y));
    column.a*=step(0.,columnUV.x)*step(columnUV.x,1.);
    float2 columnPhoto=ProjectPoint(columnPoint);
    column.rgb=lerp(column.rgb,tex2D(_Source,clamp(columnPhoto,float2(0.,0.),float2(1.,1.))).rgb,InPhoto(columnPhoto)*step(columnPoint.x,1.6));
    if(columnPoint.y>=-1.5 && columnDepth<=nearest+.0001)result=CompositeOver(result,column);
    if(column.a>.99 && columnDepth<nearest)nearest=columnDepth;
    for(int plantBay=-1;plantBay<=1;plantBay++) {
        float cx=-1.22+3.42*(section+float(plantBay));
        float z=4.87+.585*cx;
        float t=z-_CameraOffset.z;
        float3 p=_CameraOffset+t*ray;
        float2 puv=float2((p.x-cx)/.825+.5,(p.y+1.5)/1.65);
        if(t>0. && t<nearest && puv.x>=0. && puv.x<=1. && puv.y>=0. && puv.y<=1.) {
            float4 plant=tex2D(_Columns,float2(.5+puv.x*.5,puv.y));
            result=CompositeOver(result,plant);
            if(plant.a>.99)nearest=t;
        }
    }
    return result;
}

