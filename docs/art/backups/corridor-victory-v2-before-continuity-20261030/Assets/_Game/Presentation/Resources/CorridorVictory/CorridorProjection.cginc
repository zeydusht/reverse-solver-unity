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
float4 Corridor(float2 uv) {
    if(_Progress<=0. || _Progress>=1.)return WithPicture(tex2D(_MainTex,uv),uv);
    float3 ray=float3((uv.x-.5)*.6217364906,(uv.y-.5442708333)*.932604736,1.);
    float3 wall=WallPoint(ray,0.);
    float3 local=RepeatBay(wall);
    float2 v=float2((local.x+1.4)/3.42,(local.y+1.5)/5.);
    float4 result=tex2D(_Wall,clamp(v,float2(.0001,.0001),float2(.9999,.9999)));
    float2 wallPhoto=ProjectPoint(local);
    float wallKnown=tex2D(_Arches,clamp(v,float2(0.,0.),float2(1.,1.))).r;
    result.rgb=lerp(result.rgb,tex2D(_MainTex,clamp(wallPhoto,float2(0.,0.),float2(1.,1.))).rgb,wallKnown*InPhoto(wallPhoto));
    result.a=1.;
    if(ray.y<-.0001) {
        float t=-1.5/ray.y;
        if(t>0. && t<(wall.z-_CameraOffset.z)) {
            float3 fp=_CameraOffset+t*ray;
            float2 fuv=float2(frac((fp.x+1.4)/3.42),(fp.z-.585*fp.x)/5.32);
            float4 extension=tex2D(_Floor,clamp(fuv,float2(.001,.001),float2(.999,.999)));
            float2 floorPhoto=ProjectPoint(RepeatBay(fp));
            float floorKnown=tex2D(_Arches,clamp(fuv,float2(0.,0.),float2(1.,1.))).g;
            result=float4(lerp(extension.rgb,tex2D(_MainTex,clamp(floorPhoto,float2(0.,0.),float2(1.,1.))).rgb,floorKnown*InPhoto(floorPhoto)),1.);
        }
    }
    float3 framePoint=RepeatBay(WallPoint(ray,.08));
    float2 frameUV=ProjectPoint(framePoint);
    float4 frame=SafeSample(_Frame,float2((framePoint.x+1.4)/3.42,(framePoint.y+1.5)/5.));
    frame.rgb=tex2D(_MainTex,clamp(frameUV,float2(0.,0.),float2(1.,1.))).rgb;
    if(frame.a>.5)frame=WithPicture(frame,frameUV);
    result=CompositeOver(result,frame);
    float3 columnPoint=RepeatBay(WallPoint(ray,.65));
    float2 columnUV=float2((columnPoint.x+1.4)/3.42,(columnPoint.y+1.5)/5.);
    columnUV.y=min(columnUV.y,.9999);
    float4 column=SafeSample(_Columns,columnUV);
    float2 columnPhoto=ProjectPoint(columnPoint);
    column.rgb=lerp(column.rgb,tex2D(_MainTex,clamp(columnPhoto,float2(0.,0.),float2(1.,1.))).rgb,InPhoto(columnPhoto)*step(columnPoint.x,1.6));
    result=CompositeOver(result,column);
    return result;
}
