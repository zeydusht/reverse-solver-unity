$ErrorActionPreference='Stop'
$repo=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$out=Join-Path $repo 'Assets/Audio/MusicCandidates'
[IO.Directory]::CreateDirectory($out) | Out-Null
$synth=@'
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

public class MusicNote {
 public string instrument;
 public double beat, length, velocity, pan;
 public int midi;
}
public class MusicTrack {
 public string id, key, description;
 public int bpm, beatsPerBar, bars;
 public List<MusicNote> notes=new List<MusicNote>();
 public MusicTrack(string name,int tempo,int meter,int count,string tonality,string desc) {
  id=name;bpm=tempo;beatsPerBar=meter;bars=count;key=tonality;description=desc;
 }
 public void Add(string instrument,double beat,int midi,double length,double velocity,double pan) {
  notes.Add(new MusicNote{instrument=instrument,beat=beat,midi=midi,length=length,velocity=velocity,pan=pan});
 }
}
public static class PuzzleMusic {
 const int SR=44100;
 const double TAU=6.283185307179586;
 static string Num(double v) {return v.ToString("0.########",CultureInfo.InvariantCulture);}
 static double Smooth(double t,double gate,double attack,double release) {
  double up=1-Math.Exp(-t/attack);
  double down=t<gate?1:Math.Exp(-(t-gate)/release);
  return up*down;
 }
 static double Voice(string kind,double t,double f,double gate,Random rng,ref double noise) {
  double phase=TAU*f*t;
  double v=0;
  if(kind=="marimba") {
   v=(Math.Sin(phase)*Math.Exp(-t/.34)+.24*Math.Sin(phase*4.01)*Math.Exp(-t/.052)+.035*Math.Sin(phase*9.97)*Math.Exp(-t/.027));
   return v*(1-Math.Exp(-t/.003))*Math.Exp(-Math.Max(0,t-gate)/.15);
  }
  if(kind=="piano") {
   for(int h=1;h<=8;h++) {
    double detune=1+.00023*h*h;
    double a=Math.Pow(h,-1.6)*Math.Exp(-t/(1.30/Math.Pow(h,.6)));
    v+=a*(Math.Sin(phase*h*detune)+.24*Math.Sin(phase*h*detune*1.00075));
   }
   return .58*v*(1-Math.Exp(-t/.005))*Math.Exp(-Math.Max(0,t-gate)/.24);
  }
  if(kind=="pluck"||kind=="harp") {
   double decay=kind=="harp"?.80:.48;
   for(int h=1;h<=7;h++)
    v+=Math.Pow(h,-1.65)*Math.Sin(phase*h+.11*Math.Sin(h))*Math.Exp(-t/(decay/Math.Sqrt(h)));
   return .70*v*(1-Math.Exp(-t/.004))*Math.Exp(-Math.Max(0,t-gate)/.24);
  }
  if(kind=="celeste") {
   v=Math.Sin(phase)*Math.Exp(-t/1.1)+.25*Math.Sin(phase*2.002)*Math.Exp(-t/.45)+.085*Math.Sin(phase*3.98)*Math.Exp(-t/.22)+.025*Math.Sin(phase*5.41)*Math.Exp(-t/.08);
   return .76*v*(1-Math.Exp(-t/.008))*Math.Exp(-Math.Max(0,t-gate)/.36);
  }
  if(kind=="bass") {
   v=.79*Math.Sin(phase)+.19*Math.Sin(phase*2)+.045*Math.Sin(phase*3);
   return v*Smooth(t,gate,.018,.10)*Math.Exp(-t/.95);
  }
  if(kind=="pad") {
   for(int h=1;h<=5;h++) {
    double vibrato=.0020*Math.Sin(TAU*.34*t+h*.6);
    v+=Math.Pow(h,-2.15)*(Math.Sin(phase*h*(1.0014+vibrato))+.60*Math.Sin(phase*h*.9989+.7));
   }
   return .42*v*Smooth(t,gate,.30,.60);
  }
  if(kind=="flute") {
   double ph=phase+.09*Math.Sin(TAU*4.8*t)*(1-Math.Exp(-t/.3));
   noise=.86*noise+.14*(rng.NextDouble()*2-1);
   return (.91*Math.Sin(ph)+.10*Math.Sin(ph*2)+.022*Math.Sin(ph*3)+.035*noise)*Smooth(t,gate,.065,.12);
  }
  if(kind=="brush") {
   double n=rng.NextDouble()*2-1;
   noise=.76*noise+.24*n;
   return (n-noise)*Math.Exp(-t/.023)*(1-Math.Exp(-t/.0015))*.35;
  }
  if(kind=="wood") {
   return .5*(Math.Sin(phase)+.3*Math.Sin(phase*2.73))*Math.Exp(-t/.019)*(1-Math.Exp(-t/.001));
  }
  if(kind=="thump") {
   return .6*Math.Sin(TAU*(52*t+2.1*(1-Math.Exp(-t/.025))))*Math.Exp(-t/.075)*(1-Math.Exp(-t/.003));
  }
  return 0;
 }
 static void Filter(float[] a) {
  double hp=Math.Exp(-TAU*24/SR),lp=1-Math.Exp(-TAU*5500/SR);
  double prevX=a[a.Length-1],prevY=0,low=0;
  // Warm the filter on a complete cycle, then retain its periodic steady state.
  for(int pass=0;pass<2;pass++) for(int i=0;i<a.Length;i++) {
   double x=a[i]; double y=x-prevX+hp*prevY;prevX=x;prevY=y;low+=lp*(y-low);
   if(pass==1)a[i]=(float)low;
  }
 }
 public static void Render(MusicTrack track,string folder,int seed) {
  double spb=60.0/track.bpm,totalBeats=track.bars*track.beatsPerBar;
  int n=(int)Math.Round(totalBeats*spb*SR);
  float[] left=new float[n],right=new float[n];Random rng=new Random(seed);
  foreach(MusicNote note in track.notes) {
   double onset=note.beat*spb;
   if(note.instrument!="pad") onset+=(rng.NextDouble()-.5)*.009;
   int start=(int)Math.Round(onset*SR);start=((start%n)+n)%n;
   double gate=note.length*spb;
   double release=note.instrument=="pad"?3.8:note.instrument=="celeste"?2.5:note.instrument=="piano"?1.9:.9;
   if(note.instrument=="brush"||note.instrument=="wood"||note.instrument=="thump")release=.22;
   int count=(int)((gate+release)*SR);
   double f=440*Math.Pow(2,(note.midi-69)/12.0)*Math.Pow(2,(rng.NextDouble()-.5)*1.8/1200);
   double gain=note.velocity*(.94+.12*rng.NextDouble());
   double pan=Math.Max(-.85,Math.Min(.85,note.pan));
   double gl=Math.Cos((pan+1)*Math.PI/4)*gain,gr=Math.Sin((pan+1)*Math.PI/4)*gain;
   double noise=0;
   for(int j=0;j<count;j++) {
    double t=(double)j/SR,v=Voice(note.instrument,t,f,gate,rng,ref noise);
    double tail=1.0-(double)j/count;
    if(tail<.015)v*=tail/.015;
    int k=start+j;if(k>=n)k%=n;
    left[k]+=(float)(v*gl);right[k]+=(float)(v*gr);
   }
  }
  // Short stereo room taps; circular reads preserve all tails over the loop join.
  float[] dryL=(float[])left.Clone(),dryR=(float[])right.Clone();
  double[] delays={.029,.043,.071,.113,.173,.241,.337,.463,.621,.833,1.087};
  double[] gains={.12,.10,.09,.078,.063,.050,.041,.031,.024,.017,.012};
  for(int d=0;d<delays.Length;d++) {
   int delay=(int)(delays[d]*SR);double g=gains[d];
   for(int i=0;i<n;i++) {
    int k=i-delay;if(k<0)k+=n;
    left[i]+=(float)((d%2==0?dryR[k]:dryL[k])*g);
    right[i]+=(float)((d%2==0?dryL[k]:dryR[k])*g);
   }
  }
  Filter(left);Filter(right);
  double sum=0,peak=0;
  for(int i=0;i<n;i++){left[i]=(float)Math.Tanh(left[i]*.85);right[i]=(float)Math.Tanh(right[i]*.85);sum+=(double)left[i]*left[i]+(double)right[i]*right[i];peak=Math.Max(peak,Math.Max(Math.Abs(left[i]),Math.Abs(right[i])));}
  double rms=Math.Sqrt(sum/(n*2)),normal=.085/Math.Max(1e-12,rms);if(peak*normal>.70)normal=.70/peak;
  string path=Path.Combine(folder,"music-"+track.id+".wav");
  using(BinaryWriter w=new BinaryWriter(File.Create(path))) {
   w.Write(Encoding.ASCII.GetBytes("RIFF"));w.Write(36+n*4);w.Write(Encoding.ASCII.GetBytes("WAVEfmt "));w.Write(16);w.Write((short)1);w.Write((short)2);w.Write(SR);w.Write(SR*4);w.Write((short)4);w.Write((short)16);w.Write(Encoding.ASCII.GetBytes("data"));w.Write(n*4);
   for(int i=0;i<n;i++){w.Write((short)Math.Round(left[i]*normal*32767));w.Write((short)Math.Round(right[i]*normal*32767));}
  }
  double seam=Math.Max(Math.Abs(left[0]-left[n-1]),Math.Abs(right[0]-right[n-1]))*normal;
  StringBuilder json=new StringBuilder();json.Append("{\"id\":\"").Append(track.id).Append("\",\"bpm\":").Append(track.bpm).Append(",\"beatsPerBar\":").Append(track.beatsPerBar).Append(",\"bars\":").Append(track.bars).Append(",\"key\":\"").Append(track.key).Append("\",\"description\":\"").Append(track.description).Append("\",\"seconds\":").Append(Num((double)n/SR)).Append(",\"frames\":").Append(n).Append(",\"rmsDbfs\":").Append(Num(20*Math.Log10(rms*normal))).Append(",\"peakDbfs\":").Append(Num(20*Math.Log10(peak*normal))).Append(",\"seamJump\":").Append(Num(seam)).Append(",\"notes\":[");
  for(int i=0;i<track.notes.Count;i++){MusicNote e=track.notes[i];if(i>0)json.Append(',');json.Append("{\"instrument\":\"").Append(e.instrument).Append("\",\"beat\":").Append(Num(e.beat)).Append(",\"midi\":").Append(e.midi).Append(",\"length\":").Append(Num(e.length)).Append(",\"velocity\":").Append(Num(e.velocity)).Append(",\"pan\":").Append(Num(e.pan)).Append('}');}
  json.Append("]}");File.WriteAllText(Path.Combine(folder,"score-"+track.id+".json"),json.ToString(),new UTF8Encoding(false));
  Console.WriteLine(track.id+": "+Num((double)n/SR)+" sec; "+track.notes.Count+" notes; RMS "+Num(20*Math.Log10(rms*normal))+" dBFS; seam "+Num(seam));
 }
 static void Bed(MusicTrack t,int[][] chords,int[] roots,int style) {
  for(int bar=0;bar<t.bars;bar++) {
   double at=bar*t.beatsPerBar;
   int[] chord=chords[bar%8];int root=roots[bar%8];
   for(int j=0;j<chord.Length;j++)t.Add("pad",at+.025*j,chord[j],t.beatsPerBar-.2,style==2?.014:style==3?.025:.020,(j-1.5)*.22);
   t.Add("bass",at,root,style==3?2.6:1.35,style==3?.072:.095,0);
   if(style!=3)t.Add("bass",at+(t.beatsPerBar==3?2:2.5),root+((bar%4==3)?7:0),.75,.069,0);
   if(style==0) {
    for(int j=0;j<4;j++)t.Add("pluck",at+j+.5,chord[(j+bar)%chord.Length],.56,.030,(j%2==0?-.42:.42));
    for(int j=0;j<4;j++)t.Add("brush",at+j+.5,62,.06,.026,-.24);
    if(bar%2==1)t.Add("wood",at+2,76,.06,.028,.28);
   }
   if(style==1) {
    for(int beat=1;beat<3;beat++) for(int j=0;j<3;j++)t.Add("piano",at+beat+j*.025,chord[(j+bar/8)%chord.Length],.65,.044-j*.005,(j-1)*.18);
    if(bar%2==0)t.Add("harp",at+2.45,chord[3]+12,.55,.021,.45);
   }
   if(style==2) {
    for(int beat=0;beat<2;beat++)for(int j=0;j<3;j++)t.Add("pluck",at+.5+beat*2+j*.045,chord[(j+bar)%chord.Length],.55,.045-j*.005,(j-1)*.26);
    t.Add("thump",at,36,.08,.035,0);t.Add("thump",at+2,36,.07,.027,0);
    for(int j=0;j<8;j++)t.Add("brush",at+j*.5+.25,70,.04,j%2==0?.023:.015,.38);
    t.Add("wood",at+1,76,.04,.037,-.35);t.Add("wood",at+3,72,.04,.029,-.30);
   }
   if(style==3) {
    for(int j=0;j<4;j++)t.Add("harp",at+j*.85+.15,chord[(j+bar)%chord.Length],1.15,.030,Math.Sin((bar+j)*1.1)*.48);
   }
  }
 }
 public static MusicTrack[] Compose() {
  MusicTrack a=new MusicTrack("a",92,4,24,"D major","Warm playful marimba, pizzicato-style plucks, gentle strings and a flute middle phrase");
  int[][] ac={new[]{54,57,61,64},new[]{54,57,62,66},new[]{55,59,62,66},new[]{57,59,61,64},new[]{55,59,62,66},new[]{57,61,64,66},new[]{55,59,62,66},new[]{55,61,64,69}};
  Bed(a,ac,new[]{38,35,43,45,40,42,43,45},0);
  int[][] am={new[]{74,69,66,64,66},new[]{71,74,73,69,66},new[]{67,71,69,66,64},new[]{76,73,71,69,66},new[]{64,67,71,69,66},new[]{66,69,73,71,69},new[]{74,71,69,67,66},new[]{64,69,73,71,69}};
  double[] ar={.25,1,1.75,2.5,3.25};
  for(int bar=0;bar<24;bar++) {
   int section=bar/8;
   for(int j=0;j<5;j++) {
    if(section==1&&j==1)continue;
    int index=section==1?(j+2)%5:j;
    a.Add(section==1?"flute":"marimba",bar*4+ar[j]+(section==2&&j%2==0?.125:0),am[bar%8][index],section==1?.63:.52,section==1?.090:.118,section==1?.12:-.13);
   }
   if(section==2&&bar%4==2)a.Add("celeste",bar*4+3.6,ac[bar%8][2]+12,.8,.026,.48);
  }
  MusicTrack b=new MusicTrack("b",78,3,24,"F major","Gentle piano waltz with harp answers and soft sustained harmony");
  int[][] bc={new[]{57,60,64,67},new[]{57,60,62,65},new[]{58,62,65,69},new[]{58,62,64,67},new[]{58,62,65,69},new[]{57,60,64,67},new[]{58,62,65,69},new[]{58,60,64,67}};
  Bed(b,bc,new[]{41,38,46,36,43,41,46,36},1);
  int[][] bm={new[]{69,72,67,65},new[]{74,72,69,65},new[]{70,69,65,67},new[]{67,64,62,64},new[]{67,70,74,72},new[]{69,67,65,64},new[]{65,69,70,74},new[]{72,67,64,67}};
  double[] br={.15,1,1.7,2.35};
  for(int bar=0;bar<24;bar++) {
   for(int j=0;j<4;j++) {
    if(bar/8==1&&j==2)continue;
    int pitch=bm[bar%8][(j+(bar/8==1?1:0))%4];
    b.Add("piano",bar*3+br[j],pitch,j==0?.85:.52,.115-(j%2)*.012,.05);
   }
   if(bar/8==2&&bar%2==1)b.Add("harp",bar*3+2.7,bc[bar%8][1]+12,.7,.028,-.45);
  }
  MusicTrack c=new MusicTrack("c",100,4,24,"C major","Light plucked rhythm, rounded wooden mallets, soft percussion and breezy flute responses");
  int[][] cc={new[]{52,55,60,62},new[]{53,57,60,64},new[]{55,60,64,67},new[]{53,57,60,64},new[]{55,59,62,64},new[]{55,59,62,67},new[]{53,57,60,62},new[]{53,59,62,67}};
  Bed(c,cc,new[]{36,38,45,41,43,40,41,43},2);
  int[][] cm={new[]{72,76,74,67,69},new[]{74,77,76,72,69},new[]{76,79,76,74,72},new[]{69,72,74,72,69},new[]{71,74,76,74,67},new[]{67,71,74,72,71},new[]{69,72,77,76,74},new[]{74,71,69,67,71}};
  double[] cr={.25,1,1.75,2.75,3.5};
  for(int bar=0;bar<24;bar++) {
   for(int j=0;j<5;j++) {
    if((bar/8==1&&j%2==1)||(bar%4==3&&j==3))continue;
    c.Add(bar/8==1?"flute":"marimba",bar*4+cr[j],cm[bar%8][j],bar/8==1?.9:.38,bar/8==1?.083:.100,-.10);
   }
   if(bar%4==2)c.Add("pluck",bar*4+3.85,cc[bar%8][1]+12,.6,.030,.52);
  }
  MusicTrack d=new MusicTrack("d",76,4,24,"A dorian","Dreamy celeste, rippling harp and airy sustained harmony with no drums");
  int[][] dc={new[]{57,60,64,71},new[]{54,57,59,62},new[]{55,59,62,66},new[]{55,59,62,66},new[]{57,60,64,71},new[]{55,59,62,64},new[]{54,57,62,64},new[]{55,59,62,64}};
  Bed(d,dc,new[]{45,38,43,40,45,36,38,40},3);
  int[][] dm={new[]{69,72,71},new[]{69,66,64},new[]{71,74,69},new[]{67,71,76},new[]{72,76,71},new[]{71,67,64},new[]{69,74,76},new[]{71,67,64}};
  double[] dr={.35,1.8,3};
  for(int bar=0;bar<24;bar++) {
   for(int j=0;j<3;j++) {
    if(bar/8==1&&j==1)continue;
    int pitch=dm[bar%8][(j+(bar/8==1?1:0))%3];
    d.Add("celeste",bar*4+dr[j],pitch,1.2,j==0?.090:.072,(j-1)*.27);
   }
   if(bar/8==2&&bar%4==1)d.Add("flute",bar*4+1.4,dc[bar%8][2],1.4,.033,.24);
  }
  return new[]{a,b,c,d};
 }
}
'@
Add-Type -TypeDefinition $synth -Language CSharp
$tracks=[PuzzleMusic]::Compose()
for($i=0;$i -lt $tracks.Count;$i++) {
    [PuzzleMusic]::Render($tracks[$i],$out,(20261028+$i*101))
}
