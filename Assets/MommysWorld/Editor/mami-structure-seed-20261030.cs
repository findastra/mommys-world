// Designed by Astra. Codex (GPT-6) structural seed, 2026-10-30.
// Editor-only builder; keeps the October 22 draft and all existing world assets intact.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class MamiStructureSeed20261030
{
    const string Root = "Assets/MommysWorld/ColiseumSeed";
    const string ScenePath = Root + "/mami-coliseum-seed-20261030.unity";
    const string Review = "Review/coliseum-seed";
    const float Arrival = 8f, Pitch = 5.6f, Atrium = 12f, MainRadius = 52f;
    const float LowTide = -1f, HighTide = 2.4f, DockDeck = 5.2f;
    static int Floors = 20;
    const int RoofLevel = 4;
    static Material ivory, noir, pink, metal, glass, rock, sand, water, bronze, dockStone;
    static Transform world;
    static Dictionary<string, Mesh> imported = new Dictionary<string, Mesh>();
    static Vector3 dockOrigin = new Vector3(-145, 0, -150);

    [Serializable] public class Bundle { public MeshRecord[] meshes; }
    [Serializable] public class MeshRecord { public string name; public float[] vertices, normals, uv; public int[] triangles; }

    sealed class Buffer
    {
        public readonly List<Vector3> v = new List<Vector3>();
        public readonly List<Vector3> n = new List<Vector3>();
        public readonly List<Vector2> uv = new List<Vector2>();
        public readonly List<int> t = new List<int>();
        public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
        {
            var cross=Vector3.Cross(b-a,c-a);if(cross.sqrMagnitude<1e-12f)cross=Vector3.Cross(c-a,d-a);
            if (Vector3.Dot(cross,normal)<0) { var q=b;b=d;d=q; }
            int k=v.Count; v.AddRange(new[]{a,b,c,d});
            for(int j=0;j<4;j++) n.Add(normal.normalized);
            uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});
            if(Vector3.Cross(b-a,c-a).sqrMagnitude>1e-12f)t.AddRange(new[]{k,k+1,k+2});
            if(Vector3.Cross(c-a,d-a).sqrMagnitude>1e-12f)t.AddRange(new[]{k,k+2,k+3});
        }
        public void Append(Mesh mesh, Matrix4x4 matrix)
        {
            int k=v.Count; var vv=mesh.vertices;var nn=mesh.normals;var uu=mesh.uv;
            var nm=matrix.inverse.transpose;
            for(int i=0;i<vv.Length;i++){v.Add(matrix.MultiplyPoint3x4(vv[i]));n.Add(nm.MultiplyVector(nn[i]).normalized);uv.Add(uu.Length==vv.Length?uu[i]:Vector2.zero);}
            foreach(int i in mesh.triangles)t.Add(k+i);
        }
        public void Box(Vector3 center, Vector3 size, Quaternion rot)
        {
            var s=size*.5f;Vector3[] p=new Vector3[8];
            for(int i=0;i<8;i++)p[i]=center+rot*new Vector3((i&1)==0?-s.x:s.x,(i&2)==0?-s.y:s.y,(i&4)==0?-s.z:s.z);
            Quad(p[1],p[3],p[7],p[5],rot*Vector3.right);Quad(p[0],p[4],p[6],p[2],rot*Vector3.left);
            Quad(p[2],p[6],p[7],p[3],rot*Vector3.up);Quad(p[0],p[1],p[5],p[4],rot*Vector3.down);
            Quad(p[4],p[5],p[7],p[6],rot*Vector3.forward);Quad(p[0],p[2],p[3],p[1],rot*Vector3.back);
        }
        public void Ring(float inner,float outer,float top,float thickness,float start=0,float end=360,int segments=192)
        {
            for(int i=0;i<segments;i++){
                float a=Mathf.Lerp(start,end,i/(float)segments),b=Mathf.Lerp(start,end,(i+1f)/segments);
                var ia=P(inner,a,top);var ib=P(inner,b,top);var oa=P(outer,a,top);var ob=P(outer,b,top);var down=Vector3.down*thickness;
                Quad(ia,ib,ob,oa,Vector3.up);Quad(ia+down,oa+down,ob+down,ib+down,Vector3.down);
                Quad(oa,ob,ob+down,oa+down,P(1,(a+b)*.5f,0));Quad(ib,ia,ia+down,ib+down,-P(1,(a+b)*.5f,0));
                if(i==0 && end-start<359.99f)Quad(ia+down,ia,oa,oa+down,P(1,a-90,0));
                if(i==segments-1 && end-start<359.99f)Quad(ib,ib+down,ob+down,ob,P(1,b+90,0));
            }
        }
        public void Beam(Vector3 a,Vector3 b,float width,float depth=-1)
        { var d=b-a;Box((a+b)*.5f,new Vector3(width,depth<0?width:depth,d.magnitude),Quaternion.LookRotation(d,Vector3.up)); }
        public Mesh Make(string name)
        {var m=new Mesh{name=name,indexFormat=IndexFormat.UInt32};m.SetVertices(v);m.SetNormals(n);m.SetUVs(0,uv);m.SetTriangles(t,0);m.RecalculateBounds();m.RecalculateTangents();return m;}
    }
    static Vector3 P(float r,float degrees,float y) { float a=degrees*Mathf.Deg2Rad;return new Vector3(Mathf.Cos(a)*r,y,Mathf.Sin(a)*r); }
    static bool IsBody(int level) { return level>=0 && level<=RoofLevel; }
    static float Radius(int level) { return IsBody(level)?MainRadius:Mathf.Abs(level)==Floors?16f:10f; }
    static Transform Group(string name,Transform parent=null){var g=new GameObject(name);g.transform.SetParent(parent==null?world:parent,false);return g.transform;}
    static string Slug(string s){return s.ToLowerInvariant().Replace(' ','-').Replace('_','-');}
    static Mesh SaveMesh(Mesh m,string name)
    {
        string path=Root+"/meshes/"+Slug(name)+"-20261030.asset";
        var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if(old!=null){EditorUtility.CopySerialized(m,old);UnityEngine.Object.DestroyImmediate(m);EditorUtility.SetDirty(old);return old;}
        AssetDatabase.CreateAsset(m,path);return m;
    }
    static GameObject MeshObject(string name,Mesh mesh,Material mat,Transform parent,bool collision=false)
    {
        var g=new GameObject(name);g.transform.SetParent(parent,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;
        var r=g.AddComponent<MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=ShadowCastingMode.On;
        if(collision)g.AddComponent<MeshCollider>().sharedMesh=mesh;
        return g;
    }
    static GameObject Emit(string name,Buffer b,Material m,Transform parent,bool collision=false)
    {return MeshObject(name,SaveMesh(b.Make(name),name),m,parent,collision);}
    static GameObject Box(string name,Vector3 p,Vector3 size,Material mat,Transform parent,bool collision=true)
    {var b=new Buffer();b.Box(p,size,Quaternion.identity);return Emit(name,b,mat,parent,collision);}
    static Material Material(string name,string shader,Color color,float smooth=.6f,float metallic=.03f)
    {
        string path=Root+"/materials/"+name+"-20261030.mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        var s=Shader.Find(shader);if(s==null)throw new Exception("Missing shader "+shader);
        if(m==null){m=new Material(s);AssetDatabase.CreateAsset(m,path);}else m.shader=s;
        m.color=color;m.SetFloat("_Glossiness",smooth);m.SetFloat("_Metallic",metallic);m.enableInstancing=true;EditorUtility.SetDirty(m);return m;
    }
    static void Materials()
    {
        ivory=Material("carved-ivory-marble","Mami/Carved Marble",new Color(.83f,.85f,.87f),.62f);
        ivory.SetFloat("_Scale",.7f);
        ivory.SetColor("_VeinColor",new Color(.45f,.47f,.49f));ivory.SetFloat("_RootStrength",.10f);ivory.SetColor("_RootColor",new Color(.62f,.025f,.19f));
        dockStone=Material("airstrip-pale-limestone","Standard",new Color(.72f,.73f,.69f),.35f);
        noir=Material("carved-black-marble","Mami/Carved Marble",new Color(.035f,.043f,.052f),.68f);
        noir.SetColor("_VeinColor",new Color(.55f,.61f,.65f));noir.SetFloat("_RootStrength",.3f);
        pink=Material("rose-mineral-inlay","Standard",new Color(.66f,.04f,.22f),.62f,.5f);pink.EnableKeyword("_EMISSION");pink.SetColor("_EmissionColor",new Color(.35f,.008f,.08f));
        metal=Material("brushed-platinum","Standard",new Color(.5f,.57f,.62f),.8f,.9f);
        glass=Material("frosted-stair-stone","Standard",new Color(.43f,.65f,.67f),.8f,.35f);
        glass.color=new Color(.65f,.83f,.85f,.28f);glass.SetFloat("_Mode",3);glass.SetInt("_SrcBlend",(int)BlendMode.One);glass.SetInt("_DstBlend",(int)BlendMode.OneMinusSrcAlpha);glass.SetInt("_ZWrite",0);glass.EnableKeyword("_ALPHAPREMULTIPLY_ON");glass.SetOverrideTag("RenderType","Transparent");glass.renderQueue=3000;
        rock=Material("weathered-cliff-marble","Standard",new Color(.28f,.31f,.33f),.2f);
        sand=Material("pale-sandstone","Standard",new Color(.64f,.57f,.43f),.16f);
        bronze=Material("aged-bronze","Standard",new Color(.25f,.19f,.10f),.6f,.75f);
        water=Material("tidal-ocean","Mami/Tidal Ocean",new Color(.035f,.24f,.29f),.9f);
        water.SetFloat("_DryRadius",18.7f);water.SetFloat("_DryBottom",Arrival-Floors*Pitch-3);
    }
    static void ImportMeshes()
    {
        imported.Clear();var bundle=JsonUtility.FromJson<Bundle>(File.ReadAllText("Tools/Blender/out/mami-structure-meshes-20261030.json"));
        foreach(var rec in bundle.meshes){
            var m=new Mesh{name=rec.name,indexFormat=IndexFormat.UInt32};int count=rec.vertices.Length/3;
            var v=new Vector3[count];var n=new Vector3[count];var u=new Vector2[count];
            for(int i=0;i<count;i++){v[i]=new Vector3(rec.vertices[i*3],rec.vertices[i*3+1],rec.vertices[i*3+2]);n[i]=new Vector3(rec.normals[i*3],rec.normals[i*3+1],rec.normals[i*3+2]);u[i]=new Vector2(rec.uv[i*2],rec.uv[i*2+1]);}
            m.vertices=v;m.normals=n;m.uv=u;m.triangles=rec.triangles;m.RecalculateBounds();m.RecalculateTangents();imported[rec.name]=SaveMesh(m,rec.name);
        }
    }
    [MenuItem("MommysWorld/Structure/Build Open Coliseum")]
    public static void BuildColiseum(){Build();}
    static void Build()
    {
        if(EditorApplication.isPlaying)throw new Exception("Leave Play mode before building.");
        foreach(string d in new[]{Root,Root+"/meshes",Root+"/materials",Review})Directory.CreateDirectory(d);
        if(File.Exists(ScenePath)){Directory.CreateDirectory(Review+"/backups");File.Copy(ScenePath,Review+"/backups/mami-seed-before-rebuild-20261030-"+DateTime.Now.ToString("HHmmss")+".unity",true);}
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);world=null;
        world=Group("Mami's World | structural seed");Materials();ImportMeshes();
        BuildBuilding();BuildRetainingShell();BuildDock();BuildLandscape();BuildOcean();Lighting();Descriptor();
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);
        FinishPreview();RefreshBuffers();world=GameObject.Find("Mami's World | structural seed").transform;SetTide(0);Validate();Photos();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
        Selection.activeGameObject=world.gameObject;
        if(SceneView.lastActiveSceneView!=null)SceneView.lastActiveSceneView.LookAt(new Vector3(0,45,0),Quaternion.Euler(24,32,0),180);
    }
    static void BuildBuilding()
    {
        var building=Group("01 | five-level open coliseum and extended core");
        for(int level=-Floors;level<=Floors;level++){
            bool body=IsBody(level);float y=Arrival+level*Pitch,r=Radius(level),inner=body?Atrium:6.1f;string id=(level+Floors).ToString("00");
            var floor=Group(body?"Club level "+(level+1)+(level==RoofLevel?" | open Rave House roof":" | main gallery"):"Core landing "+level.ToString("+00;-00")+(level==Floors?" | Heaven terminus":level==-Floors?" | Hell terminus":""),building);
            bool portalGap=level==1;float arcStart=portalGap?282:0,arcEnd=portalGap?618:360;
            var slab=new Buffer();slab.Ring(inner,r,y,body?.48f:.28f,arcStart,arcEnd);Emit((body?"club-slab-":"core-landing-")+id,slab,level%2==0?ivory:noir,floor,true);
            var trims=new Buffer();trims.Ring(r-.45f,r+.15f,y+.04f,.16f,arcStart,arcEnd);trims.Ring(inner,inner+.28f,y+.04f,.14f,arcStart,arcEnd);Emit("carved-floor-cornices-"+id,trims,ivory,floor);
            var inlay=new Buffer();inlay.Ring(inner+.7f,inner+.745f,y+.012f,.014f,arcStart,arcEnd);if(body)inlay.Ring(r-2,r-1.95f,y+.012f,.014f,arcStart,arcEnd);Emit("mineral-floor-inlay-"+id,inlay,pink,floor);
            var bridge=new Buffer();bridge.Box(new Vector3((inner+5.7f)*.5f,y-.19f,0),new Vector3(inner-5.7f+.2f,.38f,2.4f),Quaternion.identity);Emit("stair-landing-bridge-"+id,bridge,ivory,floor,true);
            var rails=new Buffer();var guards=new Buffer();
            if(portalGap){RailArc(rails,inner+.12f,y+1.1f,7,258,108);RailArc(rails,inner+.12f,y+1.1f,282,353,32);guards.Ring(inner+.05f,inner+.18f,y+1.05f,1.05f,7,258,108);guards.Ring(inner+.05f,inner+.18f,y+1.05f,1.05f,282,353,32);}
            else{RailArc(rails,inner+.12f,y+1.1f,body?7:12,body?353:348,body?144:72);guards.Ring(inner+.05f,inner+.18f,y+1.05f,1.05f,body?7:12,body?353:348,body?144:72);}
            if(level==0){RailArc(rails,r-.35f,y+1.1f,0,262,112);RailArc(rails,r-.35f,y+1.1f,278,360,36);guards.Ring(r-.44f,r-.31f,y+1.05f,1.05f,0,262,112);guards.Ring(r-.44f,r-.31f,y+1.05f,1.05f,278,360,36);}
            else{RailArc(rails,r-.35f,y+1.1f,arcStart,arcEnd,body?160:64);guards.Ring(r-.44f,r-.31f,y+1.05f,1.05f,arcStart,arcEnd,body?160:64);}
            foreach(float z in new[]{-1.2f,1.2f}){rails.Beam(new Vector3(5.75f,y+1.1f,z),new Vector3(inner,y+1.1f,z),.06f);guards.Box(new Vector3((inner+5.7f)*.5f,y+.5f,z),new Vector3(inner-5.7f,1,.10f),Quaternion.identity);}
            if(portalGap)foreach(float a in new[]{258f,282f}){rails.Beam(P(inner,a,y+1.1f),P(r,a,y+1.1f),.07f);guards.Beam(P(inner,a,y+.5f),P(r,a,y+.5f),.12f,1);}
            Emit("gallery-balustrade-"+id,rails,metal,floor,true);Emit("gallery-solid-guards-"+id,guards,glass,floor,true);
            if(body && level<RoofLevel){
                var columns=new Buffer();var arches=new Buffer();int bays=40;float pierRadius=MainRadius-1.3f;
                for(int k=0;k<bays;k++){
                    float a=k*360f/bays,next=(k+1)*360f/bays,mid=(a+next)*.5f;
                    if(!(level<2 && a>254 && a<286))columns.Append(imported["eroded-column-lod1"],Matrix4x4.TRS(P(pierRadius,a,y),Quaternion.Euler(0,-a,0),new Vector3(1.3f,Pitch/5.4f,1.3f)));
                    if(!(level<2 && mid>252 && mid<288))Arch(arches,P(pierRadius,mid,y),Quaternion.Euler(0,-mid-90,0),2*pierRadius*Mathf.Sin(Mathf.PI/bays)-1.1f,Pitch-.5f,.38f,.65f);
                }
                foreach(float row in new[]{Atrium+2.8f,31f,43f})for(int k=0;k<16;k++){
                    float a=k*22.5f+11.25f;if(portalGap && a>258 && a<282)continue;
                    columns.Append(imported["eroded-column-lod1"],Matrix4x4.TRS(P(row,a,y),Quaternion.Euler(0,-a,0),new Vector3(1.1f,Pitch/5.4f,1.1f)));
                    if(row<42)arches.Beam(P(row,a,y+Pitch-.8f),P(row+12,a,y+Pitch-.8f),.55f,.65f);
                }
                Emit("fluted-coliseum-piers-"+id,columns,ivory,floor,true);Emit("carved-coliseum-arches-"+id,arches,ivory,floor,true);
            }
            if(level<Floors)BuildStair(floor,level,y,id);
        }
        var portal=new Buffer();Arch(portal,new Vector3(0,Arrival,-MainRadius+1.3f),Quaternion.identity,20,Pitch*2-.5f,.8f,1.2f);
        foreach(float x in new[]{-10.4f,10.4f})portal.Box(new Vector3(x,Arrival+2.6f,-MainRadius+1.3f),new Vector3(1.2f,5.2f,1.5f),Quaternion.identity);
        Emit("double-height-arrival-portal",portal,ivory,building,true);
        var roof=Group("Open rooftop | bar and light-pillar structure",building);float roofY=Arrival+RoofLevel*Pitch;
        var bar=new Buffer();bar.Ring(24,26.2f,roofY+1.05f,1.05f,30,150,64);bar.Ring(23.8f,26.4f,roofY+1.15f,.12f,30,150,64);Emit("rooftop-curved-marble-bar",bar,noir,roof,true);
        var roofPiers=new Buffer();for(int k=0;k<8;k++)roofPiers.Append(imported["eroded-column-lod1"],Matrix4x4.TRS(P(46,k*45,roofY),Quaternion.Euler(0,-k*45,0),new Vector3(1.1f,1.2f,1.1f)));Emit("rooftop-fairy-light-pillars",roofPiers,ivory,roof,true);
        var termini=new Buffer();termini.Ring(0,5.8f,Arrival-Floors*Pitch,.55f);termini.Ring(0,2.3f,Arrival+Floors*Pitch,.55f);termini.Ring(2.3f,5.8f,Arrival+Floors*Pitch,.2f,348,372,8);Emit("finite-stair-termini",termini,noir,building,true);
        var pole=GameObject.CreatePrimitive(PrimitiveType.Cylinder);pole.name="Continuous platinum pole | finite 224 m";pole.transform.SetParent(building);pole.transform.position=new Vector3(0,Arrival,0);pole.transform.localScale=new Vector3(.28f,Floors*Pitch+.5f,.28f);pole.GetComponent<Renderer>().sharedMaterial=metal;
        for(int k=0;k<4;k++){var p=GameObject.CreatePrimitive(PrimitiveType.Cylinder);p.name="Core structural light-pole "+k;p.transform.SetParent(building);p.transform.position=P(8.5f,k*90+45,Arrival);p.transform.localScale=new Vector3(.22f,Floors*Pitch,.22f);p.GetComponent<Renderer>().sharedMaterial=pink;}
        BuildFoundation(building);
    }
    static void BuildFoundation(Transform building)
    {
        if(GameObject.Find("Coliseum tidal foundation")!=null)return;
        var foundation=Group("Coliseum tidal foundation",building);var piers=new Buffer();var beams=new Buffer();
        var column=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/meshes/eroded-column-lod1-20261030.asset");
        foreach(float row in new[]{30f,MainRadius-1.3f}){
            int count=row<40?16:40;
            for(int k=0;k<count;k++){
                float a=k*360f/count;
                if(a>258 && a<282)continue; // Leave the shore approach clear.
                piers.Append(column,Matrix4x4.TRS(P(row,a,1.1f),Quaternion.Euler(0,-a,0),new Vector3(1.8f,(Arrival-1.1f)/5.4f,1.8f)));
            }
            beams.Ring(row-1,row+1,Arrival-.48f,.65f);
        }
        Emit("tidal-foundation-carved-piers",piers,ivory,foundation,true);Emit("tidal-foundation-ring-beams",beams,noir,foundation,true);
    }
    [MenuItem("MommysWorld/Structure/Finish Coliseum Foundation")]
    public static void FinishFoundation()
    {
        world=GameObject.Find("Mami's World | structural seed").transform;Materials();
        BuildFoundation(GameObject.Find("01 | five-level open coliseum and extended core").transform);
        FinishPreview();AssetDatabase.SaveAssets();EditorSceneManager.SaveOpenScenes();Validate();Photos();EditorSceneManager.SaveOpenScenes();
    }
    static void RailArc(Buffer b,float r,float y,float a,float z,int segments)
    {for(int i=0;i<segments;i++){float u=Mathf.Lerp(a,z,i/(float)segments),v=Mathf.Lerp(a,z,(i+1f)/segments);b.Beam(P(r,u,y),P(r,v,y),.055f);if(i%4==0)b.Beam(P(r,u,y-1.1f),P(r,u,y),.045f);}}
    static void BuildStair(Transform parent,int level,float baseY,string id)
    {
        const int steps=32;const float start=12f,sweep=336f;
        var stairs=new Buffer();var stringers=new Buffer();var rail=new Buffer();
        for(int i=0;i<steps;i++){
            float a=start+i*sweep/steps,b=start+(i+1)*sweep/steps,top=baseY+(i+1)*Pitch/steps;
            stairs.Ring(2.4f,5.8f,top,.20f,a,b,3);
            var aa=P(5.7f,a,top+1.1f);var bb=P(5.7f,b,top+1.1f);rail.Beam(aa,bb,.065f);rail.Beam(P(2.5f,a,top+1.1f),P(2.5f,b,top+1.1f),.065f);
            if(i%2==0){rail.Beam(aa-Vector3.up*1.1f,aa,.045f);rail.Beam(P(2.5f,a,top),P(2.5f,a,top+1.1f),.045f);}
            stringers.Beam(P(2.55f,a,top-.24f),P(2.55f,b,top-.24f),.16f,.24f);stringers.Beam(P(5.65f,a,top-.24f),P(5.65f,b,top-.24f),.16f,.24f);
        }
        stairs.Ring(2.4f,5.8f,baseY+Pitch,.2f,348,372,4);
        RailArc(rail,2.5f,baseY+Pitch+1.1f,348,372,8);
        Emit("spiral-stair-treads-"+id,stairs,glass,parent,true);Emit("stair-carved-stringers-"+id,stringers,ivory,parent);Emit("stair-handrails-"+id,rail,metal,parent,true);
    }
    static void Arch(Buffer buf,Vector3 p,Quaternion rot,float width,float height,float thick,float depth)
    {
        float rad=width*.5f,center=height-rad;
        // Elliptical arch crown preserves usable headroom in narrower upper bays.
        float rise=Mathf.Min(rad,height*.5f);center=height-rise;
        for(int i=0;i<16;i++){
            float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;
            Vector3[] q=new Vector3[8];
            for(int side=0;side<2;side++){
                float z=(side==0?-1:1)*depth*.5f;
                q[side*4]=p+rot*new Vector3(Mathf.Cos(a)*rad,center+Mathf.Sin(a)*rise,z);
                q[side*4+1]=p+rot*new Vector3(Mathf.Cos(b)*rad,center+Mathf.Sin(b)*rise,z);
                q[side*4+2]=p+rot*new Vector3(Mathf.Cos(b)*(rad+thick),center+Mathf.Sin(b)*(rise+thick),z);
                q[side*4+3]=p+rot*new Vector3(Mathf.Cos(a)*(rad+thick),center+Mathf.Sin(a)*(rise+thick),z);
            }
            buf.Quad(q[0],q[1],q[2],q[3],rot*Vector3.back);buf.Quad(q[4],q[5],q[6],q[7],rot*Vector3.forward);
            buf.Quad(q[0],q[4],q[5],q[1],rot*new Vector3(-Mathf.Cos((a+b)/2),-Mathf.Sin((a+b)/2),0));
            buf.Quad(q[3],q[2],q[6],q[7],rot*new Vector3(Mathf.Cos((a+b)/2),Mathf.Sin((a+b)/2),0));
            buf.Quad(q[0],q[3],q[7],q[4],rot*new Vector3(Mathf.Sin(a),-Mathf.Cos(a),0));buf.Quad(q[1],q[5],q[6],q[2],rot*new Vector3(-Mathf.Sin(b),Mathf.Cos(b),0));
        }
    }
    static void BuildRetainingShell()
    {
        var shell=Group("02 | narrow dry lower shaft and foundation");var wall=new Buffer();
        wall.Ring(18.7f,19.4f,Arrival-.5f,Floors*Pitch+1,0,360,192);Emit("submerged-core-retaining-shell",wall,noir,shell,true);
        var foundation=new Buffer();foundation.Ring(0,19.4f,Arrival-Floors*Pitch-.5f,2);Emit("lower-foundation-cap",foundation,noir,shell,true);
    }
    static void BuildDock()
    {
        var dock=Group("03 | detached Sunshine airstrip and tidal boarding");dock.position=dockOrigin;
        Box("airstrip-long-runway",new Vector3(0,DockDeck-.65f,0),new Vector3(24,1.3f,112),dockStone,dock);
        Box("airstrip-perpendicular-arm",new Vector3(40,DockDeck-.65f,45),new Vector3(56,1.3f,14),dockStone,dock);
        var details=new Buffer();var supports=new Buffer();
        for(int z=-52;z<=52;z+=8){supports.Box(new Vector3(-10,-2,z),new Vector3(1.5f,13,1.5f),Quaternion.identity);supports.Box(new Vector3(10,-2,z),new Vector3(1.5f,13,1.5f),Quaternion.identity);}
        for(int x=20;x<=64;x+=8){supports.Box(new Vector3(x,-2,40),new Vector3(1.5f,13,1.5f),Quaternion.identity);supports.Box(new Vector3(x,-2,50),new Vector3(1.5f,13,1.5f),Quaternion.identity);}
        Emit("airstrip-raised-piers",supports,dockStone,dock,true);
        var joints=new Buffer();for(int z=-52;z<=52;z+=4)joints.Box(new Vector3(0,DockDeck+.006f,z),new Vector3(23.9f,.012f,.025f),Quaternion.identity);for(int x=16;x<=64;x+=4)joints.Box(new Vector3(x,DockDeck+.006f,45),new Vector3(.025f,.012f,13.9f),Quaternion.identity);Emit("airstrip-stone-course-joints",joints,bronze,dock);
        for(int z=-50;z<=50;z+=4){details.Box(new Vector3(0,DockDeck+.015f,z),new Vector3(.12f,.025f,1.6f),Quaternion.identity);}
        details.Box(new Vector3(-11.6f,DockDeck+.02f,0),new Vector3(.18f,.03f,110),Quaternion.identity);details.Box(new Vector3(11.6f,DockDeck+.02f,0),new Vector3(.18f,.03f,110),Quaternion.identity);
        Emit("airstrip-joints-and-runway-inlay",details,bronze,dock);
        // A bespoke crowned M occupies the original runway emblem position.
        var mark=new Buffer();float y=DockDeck+.045f,zmark=15;
        mark.Beam(new Vector3(-4,y,zmark-4),new Vector3(-4,y,zmark+4),.65f,.035f);mark.Beam(new Vector3(-4,y,zmark+4),new Vector3(0,y,zmark-1),.65f,.035f);
        mark.Beam(new Vector3(0,y,zmark-1),new Vector3(4,y,zmark+4),.65f,.035f);mark.Beam(new Vector3(4,y,zmark+4),new Vector3(4,y,zmark-4),.65f,.035f);
        for(int i=0;i<48;i++){float a=i*360f/48,b=(i+1)*360f/48;mark.Beam(new Vector3(0,y,zmark)+P(7,a,0),new Vector3(0,y,zmark)+P(7,b,0),.16f,.035f);}
        mark.Beam(new Vector3(-3,y,zmark+8),new Vector3(3,y,zmark+8),.35f,.035f);
        mark.Beam(new Vector3(-3,y,zmark+8),new Vector3(-4,y,zmark+11),.35f,.035f);mark.Beam(new Vector3(-4,y,zmark+11),new Vector3(-1.5f,y,zmark+9.3f),.35f,.035f);
        mark.Beam(new Vector3(-1.5f,y,zmark+9.3f),new Vector3(0,y,zmark+12),.35f,.035f);mark.Beam(new Vector3(0,y,zmark+12),new Vector3(1.5f,y,zmark+9.3f),.35f,.035f);
        mark.Beam(new Vector3(1.5f,y,zmark+9.3f),new Vector3(4,y,zmark+11),.35f,.035f);mark.Beam(new Vector3(4,y,zmark+11),new Vector3(3,y,zmark+8),.35f,.035f);Emit("mami-crowned-m-runway-emblem",mark,pink,dock);
        // The reference's tower, upper gallery, cistern and perpendicular narrow arm establish the recognizable plan.
        var tower=new Buffer();var towerCenter=new Vector3(15,DockDeck,44);
        for(int k=0;k<4;k++){var q=Quaternion.Euler(0,k*90,0);tower.Box(towerCenter+q*new Vector3(0,4,3.7f),new Vector3(8,8,.6f),q);tower.Box(towerCenter+q*new Vector3(-2.5f,10,3.7f),new Vector3(3,4,.6f),q);tower.Box(towerCenter+q*new Vector3(2.5f,10,3.7f),new Vector3(3,4,.6f),q);tower.Box(towerCenter+q*new Vector3(0,14,3.7f),new Vector3(8,4,.6f),q);Arch(tower,towerCenter+q*new Vector3(0,8,3.7f),q,2,4,.25f,.6f);}
        Emit("airstrip-control-tower",tower,dockStone,dock,true);
        Box("airstrip-tower-base-plinth",new Vector3(15,DockDeck+.6f,44),new Vector3(9.5f,1.2f,9.5f),noir,dock);
        Box("airstrip-tower-cornice",new Vector3(15,DockDeck+16,44),new Vector3(10,.7f,10),ivory,dock);
        var roof=new Buffer();for(int k=0;k<8;k++){
            float a=k*45;var c=new Vector3(15,DockDeck+17.6f,44);var p=c+P(6,a,-1.0f);var l=c+P(2.1f,a-27,-.6f);var r=c+P(2.1f,a+27,-.6f);
            roof.Quad(c,l,p,r,Vector3.up);roof.Quad(c-Vector3.up*.15f,r-Vector3.up*.15f,p-Vector3.up*.15f,l-Vector3.up*.15f,Vector3.down);
        }Emit("airstrip-palm-form-canopy",roof,bronze,dock);
        var cistern=GameObject.CreatePrimitive(PrimitiveType.Cylinder);cistern.name="Airstrip water cistern";cistern.transform.SetParent(dock,false);cistern.transform.localPosition=new Vector3(-5,DockDeck+3.5f,45);cistern.transform.localScale=new Vector3(5,3.5f,5);cistern.GetComponent<Renderer>().sharedMaterial=metal;
        var basin=new Buffer();basin.Ring(3.4f,4.2f,DockDeck+1.1f,1.1f,0,360,64);var basinObj=Emit("airstrip-cistern-basin",basin,ivory,dock,true);basinObj.transform.localPosition=new Vector3(-5,0,45);
        var pipe=new Buffer();pipe.Beam(new Vector3(-7,DockDeck+6,45),new Vector3(-9,DockDeck+6,45),.22f);pipe.Beam(new Vector3(-9,DockDeck+6,45),new Vector3(-9,DockDeck+.5f,45),.22f);Emit("airstrip-cistern-pipe",pipe,bronze,dock);
        // Low deck follows tide; long hinged gangway remains connected to the fixed deck.
        var pontoon=Group("Tide-following pontoon",dock);pontoon.localPosition=new Vector3(24,.65f,-34);
        Box("jet-ski-boarding-pontoon",Vector3.zero,new Vector3(9,.5f,24),noir,pontoon);
        var fingers=new Buffer();for(int k=0;k<4;k++)fingers.Box(new Vector3(7,0,-9+k*6),new Vector3(7,.5f,1.15f),Quaternion.identity);Emit("four-jet-ski-berth-fingers",fingers,ivory,pontoon,true);
        var ramp=GameObject.CreatePrimitive(PrimitiveType.Cube);ramp.name="Tidal gangway";ramp.transform.SetParent(dock,false);ramp.GetComponent<Renderer>().sharedMaterial=metal;
        for(int k=0;k<4;k++){var b=Group("Reserved jet ski berth "+(k+1)+" | 3.4 x 1.2 m",pontoon);b.localPosition=new Vector3(7,0,-6+k*6);}
        var flag=new Buffer();flag.Beam(new Vector3(65,DockDeck,45),new Vector3(65,DockDeck+13,45),.18f);Emit("airstrip-flagpole",flag,metal,dock);
    }
    static void BuildLandscape()
    {
        var landscape=Group("04 | near-building cliffs and open caves");
        PlaceRock("East cliff with through cave","mountain-cave",new Vector3(79,-.5f,20),25,Vector3.one,landscape);
        PlaceRock("West cliff with through cave","mountain-cave",new Vector3(-83,-.5f,30),-30,new Vector3(.95f,1f,.95f),landscape);
        PlaceRock("Close sea arch","rock-arch",new Vector3(77,-.5f,-43),-30,Vector3.one,landscape);
        PlaceRock("Northern ridge","mountain-cave",new Vector3(15,-.5f,91),90,new Vector3(1.8f,1.5f,1.3f),landscape);
        // Three sand islets under the airstrip, preserving its water-separated silhouette.
        for(int i=0;i<3;i++){
            var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name="Airstrip support islet "+(i+1);g.transform.SetParent(landscape);
            g.transform.position=dockOrigin+new Vector3(i==2?66:0,-3.2f,i==0?-48:45);g.transform.localScale=new Vector3(i==2?28:45,8,i==2?28:38);g.GetComponent<Renderer>().sharedMaterial=sand;
        }
        // Main island is a geological shelf around the caisson, not a solid sphere that fills the lower floors.
        var shelf=new Buffer();shelf.Ring(19.4f,MainRadius+13,1.1f,8,0,360,192);Emit("main-island-foundation-shelf",shelf,rock,landscape,true);
        var entry=new Buffer();entry.Box(new Vector3(0,4,-58),new Vector3(18,1.2f,16),Quaternion.identity);Emit("high-water-arrival-terrace",entry,ivory,landscape,true);
        // Individual broad risers connect shore terrace to the raised arrival gallery.
        var approach=new Buffer();for(int k=0;k<20;k++){float yy=4.6f+(k+1)*3.4f/20;approach.Box(new Vector3(0,yy-.1f,-63.7f+k*.6f),new Vector3(12,.2f,.6f),Quaternion.identity);}
        approach.Box(new Vector3(0,7.8f,-50.8f),new Vector3(12,.4f,2.4f),Quaternion.identity);Emit("arrival-terrace-stairs",approach,ivory,landscape,true);
        var fragments=Group("Eroded carved fragments",landscape);
        for(int k=0;k<7;k++){var g=MeshObject("broken-column-fragment-"+k,imported["broken-column"],ivory,fragments,true);g.transform.localPosition=new Vector3(57+k*2,1.2f,-14-k*3);g.transform.localRotation=Quaternion.Euler(0,k*38,k%2==0?70:0);}
    }
    static void PlaceRock(string name,string mesh,Vector3 pos,float yaw,Vector3 scale,Transform parent)
    {var g=MeshObject(name,imported[mesh],rock,parent,true);g.transform.localPosition=pos;g.transform.localRotation=Quaternion.Euler(0,yaw,0);g.transform.localScale=scale;}
    static void BuildOcean()
    {
        var group=Group("05 | ocean and tide study");var b=new Buffer();const int count=192;const float size=10000;
        // Quad normals and tangents explicitly support the world-normal water shader.
        for(int x=0;x<count;x++)for(int z=0;z<count;z++){
            float a=-size/2+size*x/count,c=-size/2+size*z/count,d=size/count;
            b.Quad(new Vector3(a,0,c),new Vector3(a,0,c+d),new Vector3(a+d,0,c+d),new Vector3(a+d,0,c),Vector3.up);
        }
        var m=b.Make("ocean-surface");var bounds=m.bounds;bounds.Expand(new Vector3(0,.7f,0));m.bounds=bounds;
        MeshObject("Ocean surface",SaveMesh(m,"ocean-surface"),water,group);
    }
    static void Lighting()
    {
        var group=Group("06 | study lighting and cameras");var sun=new GameObject("Late afternoon sun").AddComponent<Light>();sun.transform.SetParent(group);sun.type=LightType.Directional;sun.intensity=1.2f;sun.color=new Color(1,.91f,.82f);sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(35,-38,0);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.58f,.62f,.68f);
        var sky=new Material(Shader.Find("Skybox/Procedural"));sky.SetFloat("_SunSize",.035f);sky.SetColor("_SkyTint",new Color(.5f,.5f,.5f));sky.SetColor("_GroundColor",new Color(.45f,.47f,.5f));
        string skypath=Root+"/materials/afternoon-sky-20261030.mat";var old=AssetDatabase.LoadAssetAtPath<Material>(skypath);if(old!=null){EditorUtility.CopySerialized(sky,old);UnityEngine.Object.DestroyImmediate(sky);sky=old;}else AssetDatabase.CreateAsset(sky,skypath);
        RenderSettings.skybox=sky;RenderSettings.sun=sun;RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.57f,.68f,.77f);RenderSettings.fogDensity=.0008f;
        QualitySettings.shadowDistance=220;QualitySettings.shadows=ShadowQuality.All;
        var camera=new GameObject("Structural overview camera").AddComponent<Camera>();camera.transform.SetParent(group);camera.transform.position=new Vector3(-330,195,-390);camera.transform.LookAt(new Vector3(-20,38,-20));camera.fieldOfView=52;camera.nearClipPlane=.1f;camera.farClipPlane=12000;camera.clearFlags=CameraClearFlags.Skybox;camera.allowHDR=true;
        // Subterranean galleries use sparse architectural lights; no gameplay effects in this seed.
        for(int i=-Floors;i<=0;i+=4){var light=new GameObject("Lower gallery worklight "+i).AddComponent<Light>();light.transform.SetParent(group);light.type=LightType.Point;light.transform.position=new Vector3(0,Arrival+i*Pitch+3,0);light.range=55;light.intensity=2;light.color=new Color(.85f,.60f,.72f);light.shadows=LightShadows.None;}
    }
    static void Descriptor()
    {
        var type=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("VRC.SDK3.Components.VRCSceneDescriptor")).FirstOrDefault(t=>t!=null);
        if(type==null)throw new Exception("Installed VRChat scene descriptor type was not found.");
        var g=Group("07 | world spawn and SDK descriptor");var component=g.gameObject.AddComponent(type);
        var spawn=Group("Arrival gallery spawn",g);spawn.position=dockOrigin+new Vector3(0,DockDeck+.15f,15);spawn.rotation=Quaternion.Euler(0,45,0);
        var serialized=new SerializedObject(component);var spawns=serialized.FindProperty("spawns");if(spawns==null)throw new Exception("SDK spawns property unavailable.");spawns.arraySize=1;spawns.GetArrayElementAtIndex(0).objectReferenceValue=spawn;
        var respawn=serialized.FindProperty("RespawnHeightY");if(respawn==null)throw new Exception("SDK respawn height property unavailable.");respawn.floatValue=Arrival-Floors*Pitch-10;serialized.ApplyModifiedPropertiesWithoutUndo();
        EnsurePipeline();
    }
    static void EnsurePipeline()
    {
        var type=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("VRC.Core.PipelineManager")).FirstOrDefault(t=>t!=null);
        if(type==null)throw new Exception("Installed SDK PipelineManager type was not found.");
        var descriptor=GameObject.Find("07 | world spawn and SDK descriptor");if(descriptor.GetComponent(type)==null)descriptor.AddComponent(type);
        var dt=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("VRC.SDK3.Components.VRCSceneDescriptor")).First(t=>t!=null);
        var so=new SerializedObject(descriptor.GetComponent(dt));var height=so.FindProperty("RespawnHeightY");if(height==null)throw new Exception("SDK respawn height unavailable.");height.floatValue=Arrival-Floors*Pitch-10;
        var reference=so.FindProperty("ReferenceCamera");if(reference!=null)reference.objectReferenceValue=GameObject.Find("Structural overview camera");so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(descriptor.GetComponent(dt));EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    [MenuItem("MommysWorld/Structure/Finish Scene Preview")]
    public static void FinishPreview()
    {
        Materials();EnsurePipeline();
        foreach(var t in UnityEngine.Object.FindObjectsOfType<TextMesh>())if(t.name.StartsWith("level-wayfinding"))UnityEngine.Object.DestroyImmediate(t.gameObject);
        foreach(var c in UnityEngine.Object.FindObjectsOfType<Camera>())c.farClipPlane=12000;
        var existing=GameObject.Find("Ocean architectural reflection");var probe=existing==null?new GameObject("Ocean architectural reflection").AddComponent<ReflectionProbe>():existing.GetComponent<ReflectionProbe>();
        probe.transform.position=new Vector3(-40,80,-120);probe.size=new Vector3(1000,450,1000);probe.mode=ReflectionProbeMode.Realtime;probe.refreshMode=ReflectionProbeRefreshMode.ViaScripting;probe.timeSlicingMode=ReflectionProbeTimeSlicingMode.NoTimeSlicing;probe.resolution=128;probe.farClipPlane=10000;probe.clearFlags=ReflectionProbeClearFlags.Skybox;probe.RenderProbe();
        AssetDatabase.SaveAssets();EditorSceneManager.SaveOpenScenes();
    }
    [MenuItem("MommysWorld/Structure/Refresh Saved Mesh Buffers")]
    public static void RefreshBuffers()
    {
        AssetDatabase.SaveAssets();EditorSceneManager.SaveOpenScenes();
        foreach(string guid in AssetDatabase.FindAssets("t:Mesh",new[]{Root+"/meshes"}))AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid),ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
        EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
    }
    static void Text(string name,string value,Vector3 position,Quaternion rotation,float size,Color color,Transform parent)
    {var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localRotation=rotation;var t=go.AddComponent<TextMesh>();t.text=value;t.fontSize=64;t.characterSize=size;t.color=color;t.anchor=TextAnchor.MiddleLeft;}
    [MenuItem("MommysWorld/Structure/Preview Low Tide")]
    public static void Low(){SetTide(LowTide);}
    [MenuItem("MommysWorld/Structure/Preview High Tide")]
    public static void High(){SetTide(HighTide);}
    static void SetTide(float height)
    {
        var ocean=GameObject.Find("Ocean surface");if(ocean==null)throw new Exception("Open the structure scene first.");ocean.transform.position=new Vector3(0,height,0);
        var pontoon=GameObject.Find("Tide-following pontoon");pontoon.transform.localPosition=new Vector3(24,height+.65f,-34);
        var ramp=GameObject.Find("Tidal gangway");Vector3 a=dockOrigin+new Vector3(12f,DockDeck,-34),b=dockOrigin+new Vector3(20,height+.90f,-34);
        ramp.transform.position=(a+b)/2-Vector3.up*.10f;ramp.transform.rotation=Quaternion.LookRotation(b-a,Vector3.up);ramp.transform.localScale=new Vector3(2.3f,.20f,Vector3.Distance(a,b)+.20f);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());SceneView.RepaintAll();
    }
    [MenuItem("MommysWorld/Structure/Validate Seed")]
    public static void Validate()
    {
        Physics.SyncTransforms();var report=new List<string>{"Mami structural seed — editor geometry validation","UTC "+DateTime.UtcNow.ToString("o"),"Five main club levels; narrow core landings from -20 to +20. Provisional dimensions."};
        int errors=0,samples=0;Action<bool,string> check=(ok,msg)=>{report.Add((ok?"PASS ":"FAIL ")+msg);if(!ok)errors++;};
        for(int level=-Floors;level<=Floors;level++){
            float y=Arrival+level*Pitch;
            foreach(float radius in new[]{IsBody(level)?14f:7f,Radius(level)-1.5f}){
                var p=P(radius,45,y+1);RaycastHit hit;bool found=Physics.Raycast(p,Vector3.down,out hit,2f);samples++;check(found&&Mathf.Abs(hit.point.y-y)<.12f,"floor "+level+" support r="+radius.ToString("F1"));
            }
            if(level<Floors)for(int i=0;i<32;i++){
                float a=12+(i+.5f)*336/32,yStep=y+(i+1)*Pitch/32;var p=P(4,a,yStep+.3f);RaycastHit hit;bool found=Physics.Raycast(p,Vector3.down,out hit,.6f);samples++;
                check(found&&Mathf.Abs(hit.point.y-yStep)<.04f,"stair "+level+"/"+i+" has matching visible support");
                check(!Physics.Raycast(P(4,a,yStep+.25f),Vector3.up,1.9f),"stair "+level+"/"+i+" headroom");
            }
        }
        check(DockDeck-1.3f>HighTide+.3f,"airstrip underside above high tide plus wave crest by "+(DockDeck-1.3f-HighTide-.3f).ToString("F2")+" m");
        check(Arrival-.48f>HighTide+.3f,"arrival gallery above high tide");
        check(Vector2.Distance(new Vector2(dockOrigin.x,dockOrigin.z),Vector2.zero)-68>MainRadius+13,"detached airstrip retains open-water gap");
        var filters=UnityEngine.Object.FindObjectsOfType<MeshFilter>();
        check(filters.Count(f=>f.name.StartsWith("club-slab-"))==5,"exactly five full-width club slabs");
        check(filters.Count(f=>f.name.StartsWith("core-landing-"))==36,"36 smaller core landings beyond the five club levels");
        check(filters.Where(f=>f.name.StartsWith("club-slab-")).All(f=>Mathf.Abs(f.sharedMesh.bounds.size.x-104)<.1f),"all main club slabs keep the same 104 m diameter");
        check(!Physics.Raycast(P(38,20,Arrival+RoofLevel*Pitch+1),Vector3.up,150),"main rooftop remains open to the sky");
        long triangles=0;foreach(var f in filters){check(f.sharedMesh!=null,"mesh assigned: "+f.name);if(f.sharedMesh!=null)triangles+=f.sharedMesh.triangles.Length/3;}
        foreach(var r in UnityEngine.Object.FindObjectsOfType<Renderer>())foreach(var m in r.sharedMaterials)check(m!=null&&m.shader!=null&&m.shader.isSupported,"supported material: "+r.name);
        report.Insert(3,"Errors="+errors+"; support samples="+samples+"; rendered triangles="+triangles+"; renderers="+UnityEngine.Object.FindObjectsOfType<Renderer>().Length);
        report.Add("Not yet tested: headset comfort, client performance, synchronized live tide, vehicle boarding/gameplay; exact game dimensions not verified.");
        Directory.CreateDirectory(Review);File.WriteAllLines(Review+"/mami-structure-validation-20261030.txt",report);
        if(errors>0)Debug.LogError("Mami structural seed validation errors: "+errors);else Debug.Log("Mami structural seed validation passed.");
    }
    [MenuItem("MommysWorld/Structure/Photograph Seed")]
    public static void Photos()
    {
        var cam=UnityEngine.Object.FindObjectOfType<Camera>();if(cam==null)throw new Exception("No study camera.");
        Shot(cam,"mami-structure-overview",new Vector3(-240,175,-360),new Vector3(-60,38,-65),55);
        Shot(cam,"mami-coliseum-exterior",new Vector3(-85,43,-112),new Vector3(0,20,0),52);
        Shot(cam,"mami-open-rooftop",new Vector3(44,57,-42),new Vector3(0,31,0),62);
        Shot(cam,"mami-arrival-airstrip",dockOrigin+new Vector3(87,78,-110),dockOrigin+new Vector3(17,6,0),52);
        Shot(cam,"mami-marble-atrium",new Vector3(-8,Arrival+2.6f,-5),new Vector3(0,Arrival+11,0),74);
        Shot(cam,"mami-nearby-caves",new Vector3(74,7,-26),new Vector3(79,6,20),65);
        Shot(cam,"mami-upper-galleries",new Vector3(63,62,-74),new Vector3(0,34,0),57);
        Shot(cam,"mami-lower-galleries",new Vector3(-8,Arrival-Floors*Pitch+3,-5),new Vector3(0,Arrival-Floors*Pitch+12,0),74);
        SetTide(LowTide);Shot(cam,"mami-low-tide-dock",dockOrigin+new Vector3(52,12,-67),dockOrigin+new Vector3(20,3,-29),61);
        SetTide(HighTide);Shot(cam,"mami-high-tide-dock",dockOrigin+new Vector3(52,12,-67),dockOrigin+new Vector3(20,3,-29),61);
        SetTide(0);cam.transform.position=new Vector3(-240,175,-360);cam.transform.LookAt(new Vector3(-60,38,-65));cam.fieldOfView=55;
    }
    [MenuItem("MommysWorld/Structure/Export Seed Package")]
    public static void ExportSeed()
    {
        AssetDatabase.ExportPackage(new[]{Root,"Assets/MommysWorld/StructureSeed/shaders","Assets/MommysWorld/Editor/mami-structure-seed-20261030.cs"},Review+"/mami-structure-seed-20261030.unitypackage",ExportPackageOptions.Recurse);
    }
    [MenuItem("MommysWorld/Structure/Open Structural Review")]
    public static void OpenReview()
    {
        var view=ScriptableObject.CreateInstance<SceneView>();view.titleContent=new GUIContent("Mami's World | structure review");
        view.position=new Rect(80,80,1280,850);view.ShowUtility();
        view.LookAt(new Vector3(0,28,0),Quaternion.Euler(18,32,0),145);view.Focus();
    }
    [MenuItem("MommysWorld/Structure/Play Mode Smoke Check")]
    public static void PlaySmoke(){SessionState.SetBool("MamiSmoke20261030",true);EditorApplication.isPlaying=true;}
    static double smokeStart;
    static int smokePhase;
    static List<string> smokeLines;
    [InitializeOnLoadMethod]
    static void RegisterSmoke()
    {EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("MamiSmoke20261030",false)){smokeStart=EditorApplication.timeSinceStartup;smokePhase=0;smokeLines=new List<string>();EditorApplication.update+=SmokeTick;}};}
    static void SmokeTick()
    {
        if(EditorApplication.timeSinceStartup-smokeStart<(smokePhase==0?12:6))return;
        try{
            var networking=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("VRC.SDKBase.Networking")).FirstOrDefault(t=>t!=null);
            var player=networking==null?null:networking.GetProperty("LocalPlayer")?.GetValue(null);
            if(smokePhase==0){smokeLines.Add("UTC "+DateTime.UtcNow.ToString("o"));smokeLines.Add("Unity Play mode entered for 12 seconds.");smokeLines.Add("ClientSim local player present: "+(player!=null));}
            if(player!=null){var pos=(Vector3)player.GetType().GetMethod("GetPosition").Invoke(player,null);
                if(smokePhase==0){smokeLines.Add("Player position: "+pos.ToString("F3"));smokeLines.Add("Spawn region: "+(Vector3.Distance(pos,dockOrigin+new Vector3(0,DockDeck,15))<5));
                    var teleport=player.GetType().GetMethod("TeleportTo",new[]{typeof(Vector3),typeof(Quaternion)});
                    if(teleport!=null){
                        var dt=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("VRC.SDK3.Components.VRCSceneDescriptor")).First(t=>t!=null);
                        var descriptor=GameObject.Find("07 | world spawn and SDK descriptor").GetComponent(dt);
                        smokeLines.Add("Runtime respawn height: "+new SerializedObject(descriptor).FindProperty("RespawnHeightY").floatValue);
                        var target=new Vector3(4,Arrival-Floors*Pitch+.15f,0);Physics.SyncTransforms();RaycastHit support;
                        smokeLines.Add("Hell target has floor support: "+Physics.Raycast(target+Vector3.up,Vector3.down,out support,2));
                        teleport.Invoke(player,new object[]{target,Quaternion.identity});
                        smokeLines.Add("Immediately after teleport: "+((Vector3)player.GetType().GetMethod("GetPosition").Invoke(player,null)).ToString("F3"));
                        smokePhase=1;smokeStart=EditorApplication.timeSinceStartup;return;}
                    smokeLines.Add("Lower-level teleport method unavailable; not tested.");
                }else{smokeLines.Add("Lower terminus position after 6 seconds: "+pos.ToString("F3"));smokeLines.Add("Lower terminus remained walkable without premature respawn: "+(Vector3.Distance(pos,new Vector3(4,Arrival-Floors*Pitch,0))<3));}
            }
            File.WriteAllLines(Review+"/mami-playmode-smoke-20261030.txt",smokeLines);
        }catch(Exception e){File.WriteAllText(Review+"/mami-playmode-smoke-20261030.txt","FAILED "+e.Message);}
        EditorApplication.update-=SmokeTick;SessionState.SetBool("MamiSmoke20261030",false);EditorApplication.isPlaying=false;
    }
    static void Shot(Camera cam,string name,Vector3 pos,Vector3 target,float fov)
    {
        cam.transform.position=pos;cam.transform.LookAt(target);cam.fieldOfView=fov;
        var rt=new RenderTexture(1800,1200,24,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;
        var tex=new Texture2D(1800,1200,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1800,1200),0,0);tex.Apply();File.WriteAllBytes(Review+"/"+name+"-20261030.png",tex.EncodeToPNG());
        cam.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);
    }
}
