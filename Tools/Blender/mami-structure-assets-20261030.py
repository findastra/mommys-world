"""Original sculpted architectural mesh seed for Astra's Mami's World.
Run with Blender 4.5+ --background --factory-startup --python-exit-code 1 --python this.py.
No downloaded assets. Exported axes: Unity (x,y,z) = Blender (x,z,y), winding reversed.
"""
import bpy, bmesh, math, json, os, random, sys, argparse
from mathutils import Vector
from mathutils.noise import noise
from mathutils.bvhtree import BVHTree
from datetime import datetime, timezone

parser=argparse.ArgumentParser()
parser.add_argument('--out',default=os.path.dirname(os.path.abspath(__file__)))
args=parser.parse_args(sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else [])
OUT=os.path.abspath(args.out); os.makedirs(OUT,exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.context.preferences.filepaths.save_version=0
random.seed(261030)

def mesh_obj(name, verts, faces):
    mesh=bpy.data.meshes.new(name)
    mesh.from_pydata(verts,[],faces); mesh.update()
    ob=bpy.data.objects.new(name,mesh); bpy.context.collection.objects.link(ob)
    bm=bmesh.new(); bm.from_mesh(mesh)
    bmesh.ops.recalc_face_normals(bm, faces=list(bm.faces)); bm.to_mesh(mesh); bm.free()
    for p in mesh.polygons:p.use_smooth=True
    return ob

def column(name, broken=False):
    # Closely paired lathe rings round each carved molding without blurring its silhouette.
    profile=[(0,.50),(.035,.50),(.065,.495),(.085,.47),(.18,.47),(.205,.44),
             (.235,.44),(.27,.41),(.30,.39),(.35,.39),(.385,.365),(.43,.35),
             (.49,.35),(.53,.32),(.60,.32)]
    height=2.8 if broken else 5.4
    top=2.45 if broken else 4.72
    for k in range(1,25):
        z=.60+(top-.60)*k/24
        profile.append((z,.325-.034*(z-.6)/4.2))
    if broken:
        profile +=[(2.53,.32),(2.63,.30),(2.8,.28)]
    else:
        profile +=[(4.76,.31),(4.80,.33),(4.83,.36),(4.88,.36),(4.93,.41),
                    (4.99,.43),(5.02,.43),(5.08,.40),(5.12,.40),(5.18,.47),
                    (5.22,.49),(5.27,.49),(5.30,.50),(5.37,.50),(5.40,.485)]
    N=96; vs=[]; faces=[]
    for row,(z,r) in enumerate(profile):
        for i in range(N):
            a=i*math.tau/N
            zz=z
            flute=(.018*(.5+.5*math.cos(a*20))) if .53<=z<=4.76 else .003*math.cos(a*20)
            # Subtle chipped lower edges and stone grain; no rounded sphere assembly.
            n=noise(Vector((math.cos(a)*5, math.sin(a)*5,z*2)))
            wear=.008*n + .008*max(0,math.sin(3*a+z*4))**9
            rr=r-flute-wear
            if broken and row>=len(profile)-3:
                strength=(row-(len(profile)-4))/3
                zz-=strength*(.24*(.5+.5*math.sin(a*3+.7))+.11*(.5+.5*math.sin(a*11)))
                rr-=strength*.022*(.5+.5*math.cos(a*7))
            vs.append((rr*math.cos(a),rr*math.sin(a),zz))
    for j in range(len(profile)-1):
        for i in range(N):
            q=j*N+i; k=j*N+(i+1)%N
            faces.append((q,k,k+N,q+N))
    faces.append(tuple(range(N-1,-1,-1)))
    if broken:
        # Faceted fractured crown with a shallow dished centre, sealed and not a flat cutoff.
        center=len(vs); vs.append((.015,-.01,2.52))
        off=(len(profile)-1)*N
        for i in range(N):faces.append((off+i,off+(i+1)%N,center))
    else:faces.append(tuple((len(profile)-1)*N+i for i in range(N)))
    ob=mesh_obj(name,vs,faces)
    return ob

def tunnel(name,width,depth,height,inner_width,inner_height,seed):
    # A single watertight annular half-arch volume. Its void is open all the way through.
    # The inner arc starts on the ground, so the mouth is not a sealed hole or painted dark patch.
    N=100; J=34; vs=[]; faces=[]
    phase=seed*.137
    for j in range(J+1):
        v=j/J; y=(v-.5)*depth
        taper=.78+.22*math.sin(math.pi*v)**.65
        for inner in (False,True):
            for i in range(N+1):
                a=i*math.pi/N
                if inner:
                    # Clearance never narrows below requested inner envelope.
                    w=inner_width*.5+.25+.20*math.sin(v*8+a*3+phase)**2
                    h=inner_height+.35+.25*math.sin(v*11+a*2)**2
                    rough=.18*noise(Vector((a*3,v*10,seed)))
                    x=math.cos(a)*(w+rough)
                    z=math.sin(a)*(h+rough)
                    yy=y
                else:
                    # Erosion combines broad rock folds with strata and restrained small chips.
                    broad=noise(Vector((math.cos(a)*2.2,v*4,math.sin(a)*2.5+phase)))
                    medium=noise(Vector((a*6,v*13,phase)))
                    strata=.7*math.sin(a*19+v*3)+.3*math.sin(a*47-v*8)
                    radius=taper*(1+.09*broad)+.018*medium
                    x=math.cos(a)*(width*.5*radius)+math.sin(a)*1.8*math.sin(v*7+phase)
                    z=math.sin(a)*(height*radius+strata)+max(0,math.sin(a))*.8*medium
                    yy=y+(math.sin(math.pi*v)*.7)*medium
                vs.append((x,yy,max(0,z)))
    stride=2*(N+1)
    radial=22; bottom={}
    for j in range(J+1):
        for side in (0,N):
            outside=Vector(vs[j*stride+side]); inside=Vector(vs[j*stride+N+1+side])
            for k in range(radial+1):
                if k==0: idx=j*stride+N+1+side
                elif k==radial: idx=j*stride+side
                else: idx=len(vs);vs.append(tuple(inside.lerp(outside,k/radial)))
                bottom[j,side,k]=idx
    for j in range(J):
        for inner in (0,1):
            for i in range(N):
                q=j*stride+inner*(N+1)+i
                faces.append((q,q+1,q+stride+1,q+stride))
        # Ground-contact legs use the same radial samples as the sculpted front faces.
        for i in (0,N):
            for k in range(radial):
                faces.append((bottom[j,i,k],bottom[j+1,i,k],bottom[j+1,i,k+1],bottom[j,i,k+1]))
    for j in (0,J):
        cap={}
        for k in range(radial+1):
            t=k/radial
            for i in range(N+1):
                if k==0:idx=j*stride+N+1+i
                elif k==radial:idx=j*stride+i
                elif i in (0,N):idx=bottom[j,i,k]
                else:
                    outside=Vector(vs[j*stride+i]);inside=Vector(vs[j*stride+N+1+i]);p=inside.lerp(outside,t)
                    fade=math.sin(math.pi*t)*math.sin(math.pi*i/N)**.45
                    sample=Vector((p.x*.29,p.z*.23,seed))
                    # Fixed-offset scalar noise is reproducible across Blender processes;
                    # noise_vector internally uses process-dependent random offsets.
                    nv=Vector((noise(sample),noise(sample+Vector((13.7,8.3,5.1))),noise(sample+Vector((29.1,3.5,17.7)))))
                    ledges=.38*math.sin(p.z*1.3+p.x*.20)+.18*math.sin(p.z*3.7+p.x*.4)
                    p.y+=(1 if j==J else -1)*fade*(1.9+2.2*nv.y+ledges)
                    p.x+=fade*nv.x*.7;p.z+=fade*nv.z*.4
                    idx=len(vs);vs.append(tuple(p))
                cap[k,i]=idx
        for k in range(radial):
            for i in range(N):faces.append((cap[k,i],cap[k+1,i],cap[k+1,i+1],cap[k,i+1]))
    ob=mesh_obj(name,vs,faces)
    # Keep rock folds readable with normals that are smooth on continuous faces.
    return ob

objects=[column('eroded-column'),column('broken-column',True),
         tunnel('mountain-cave',45,30,45,10,8,11),
         tunnel('rock-arch',30,14,18,10,9,19)]
hero_count=len(objects)
for source in objects[:2]:
    lod=source.copy();lod.data=source.data.copy();bpy.context.collection.objects.link(lod);lod.name=source.name+'-lod1'
    bpy.context.view_layer.objects.active=lod
    mod=lod.modifiers.new('distance-simplification','DECIMATE');mod.ratio=.15
    bpy.ops.object.modifier_apply(modifier=mod.name)
    objects.append(lod)
lod=objects[0].copy();lod.data=objects[0].data.copy();bpy.context.collection.objects.link(lod);lod.name='eroded-column-lod2'
bpy.context.view_layer.objects.active=lod
mod=lod.modifiers.new('far-distance-simplification','DECIMATE');mod.ratio=.04
bpy.ops.object.modifier_apply(modifier=mod.name)
objects.append(lod)

def material(name,dark=False,rock=False):
    ma=bpy.data.materials.new(name); ma.use_nodes=True
    n=ma.node_tree.nodes; l=ma.node_tree.links; bs=n.get('Principled BSDF')
    bs.inputs['Roughness'].default_value=.76 if rock else .3
    tex=n.new('ShaderNodeTexNoise'); tex.inputs['Scale'].default_value=2.8 if rock else 6
    tex.inputs['Detail'].default_value=5; tex.inputs['Roughness'].default_value=.65
    coords=n.new('ShaderNodeTexCoord')
    mapping=n.new('ShaderNodeVectorMath'); mapping.operation='MULTIPLY'
    mapping.inputs[1].default_value=(1,1,.32) if rock else (1,1,.12)
    l.new(coords.outputs['Generated'],mapping.inputs[0]); l.new(mapping.outputs[0],tex.inputs['Vector'])
    ramp=n.new('ShaderNodeValToRGB')
    ramp.color_ramp.elements[0].position=.29
    ramp.color_ramp.elements[0].color=(.075,.08,.10,1) if not rock else (.13,.15,.19,1)
    ramp.color_ramp.elements[1].position=.68
    ramp.color_ramp.elements[1].color=(.82,.83,.86,1) if not rock else (.5,.52,.56,1)
    l.new(tex.outputs['Fac'],ramp.inputs[0]);l.new(ramp.outputs[0],bs.inputs['Base Color'])
    bump=n.new('ShaderNodeBump'); bump.inputs['Strength'].default_value=.23 if rock else .09
    bump.inputs['Distance'].default_value=.22 if rock else .018
    l.new(tex.outputs['Fac'],bump.inputs['Height']);l.new(bump.outputs[0],bs.inputs['Normal'])
    return ma
stone=material('preview-carved-marble');rock=material('preview-weathered-limestone',rock=True)
for i,o in enumerate(objects):o.data.materials.append(stone if ('column' in o.name) else rock)

def export(ob):
    me=ob.data; me.calc_loop_triangles()
    # Split loop vertices for correct smooth normals and planar/triplanar-friendly UVs.
    vs=[];ns=[];uv=[];tri=[]
    for t in me.loop_triangles:
        base=len(vs)//3
        for vi in t.vertices:
            v=me.vertices[vi];p=v.co;n=v.normal
            vs.extend((round(p.x,6),round(p.z,6),round(p.y,6)))
            ns.extend((round(n.x,6),round(n.z,6),round(n.y,6)))
            uv.extend((round(p.x*.25,6),round(p.z*.25,6)))
        tri.extend((base,base+2,base+1))
    return dict(name=ob.name,vertices=vs,normals=ns,uv=uv,triangles=tri)

bundle={'units':'metres','axes':'Unity x,y,z; original Blender x,z,y','meshes':[export(o) for o in objects]}
jsonpath=os.path.join(OUT,'mami-structure-meshes-20261030.json')
with open(jsonpath,'w',encoding='utf-8') as f:json.dump(bundle,f,separators=(',',':'))

report={'generated_utc':datetime.now(timezone.utc).isoformat(),'mesh_file':os.path.basename(jsonpath),'meshes':[]}
for ob in objects:
    me=ob.data;me.calc_loop_triangles();bm=bmesh.new();bm.from_mesh(me)
    deg=sum(1 for p in me.polygons if p.area<1e-10)
    bounds={'min':[round(min(v.co[k] for v in me.vertices),4) for k in (0,2,1)],
            'max':[round(max(v.co[k] for v in me.vertices),4) for k in (0,2,1)]}
    report['meshes'].append(dict(name=ob.name,triangles=len(me.loop_triangles),bounds_unity_metres=bounds,
                                degenerate_faces=deg,nonmanifold_edges=sum(1 for e in bm.edges if not e.is_manifold)))
    bm.free()
    if 'cave' in ob.name or 'arch' in ob.name:
        tree=BVHTree.FromPolygons([v.co for v in me.vertices],[p.vertices[:] for p in me.polygons],all_triangles=False)
        rays=[]
        for x in (-3,0,3):
            for z in (1.8,4,6):
                hit=tree.ray_cast(Vector((x,-50,z)),Vector((0,1,0)),100)[0]
                rays.append({'x':x,'height':z,'clear':hit is None})
        report['meshes'][-1]['tunnel_clearance_rays']=rays
assert all(m['degenerate_faces']==0 and m['nonmanifold_edges']==0 for m in report['meshes'])
assert all(r['clear'] for m in report['meshes'] for r in m.get('tunnel_clearance_rays',[]))
with open(os.path.join(OUT,'mami-structure-mesh-report-20261030.json'),'w',encoding='utf-8') as f:json.dump(report,f,indent=2)

# Contact display uses untouched original meshes, at well separated positions.
objects[0].location=(-20,-13,0);objects[0].scale=(3,3,3)
objects[1].location=(-14,-13,0);objects[1].scale=(3,3,3)
objects[2].location=(12,13,0)
objects[3].location=(-29,18,0)
for o in objects[hero_count:]:o.hide_render=True;o.hide_viewport=True
bpy.ops.mesh.primitive_plane_add(size=250,location=(0,0,-.03))
floor=bpy.context.object;floor.name='preview-ground'
floor_mat=bpy.data.materials.new('preview-ground');floor_mat.diffuse_color=(.055,.065,.09,1);floor.data.materials.append(floor_mat)
world=bpy.data.worlds.new('preview-world');bpy.context.scene.world=world;world.use_nodes=True
world.node_tree.nodes['Background'].inputs[0].default_value=(.22,.26,.36,1)
world.node_tree.nodes['Background'].inputs[1].default_value=.6
def area(name,location,power,size,color):
    data=bpy.data.lights.new(name,'AREA');ob=bpy.data.objects.new(name,data);bpy.context.collection.objects.link(ob)
    ob.location=location;data.energy=power;data.shape='DISK';data.size=size;data.color=color
    ob.rotation_euler=(Vector((0,0,15))-ob.location).to_track_quat('-Z','Y').to_euler()
area('large-softbox',(20,-35,65),18000,45,(1,.83,.72));area('cool-rim',(-40,40,50),14000,30,(.5,.65,1))
sun=bpy.data.lights.new('sun','SUN');so=bpy.data.objects.new('sun',sun);bpy.context.collection.objects.link(so)
sun.energy=2.3;sun.angle=.15;so.rotation_euler=(math.radians(30),math.radians(-20),math.radians(-30))
camd=bpy.data.cameras.new('review-camera');cam=bpy.data.objects.new('review-camera',camd);bpy.context.collection.objects.link(cam)
cam.location=(75,-105,65);cam.rotation_euler=(Vector((-4,5,17))-cam.location).to_track_quat('-Z','Y').to_euler()
camd.type='ORTHO';camd.ortho_scale=103
sc=bpy.context.scene;sc.camera=cam;sc.render.engine='BLENDER_EEVEE_NEXT';sc.render.resolution_x=1800;sc.render.resolution_y=1200;sc.render.resolution_percentage=100
sc.render.image_settings.file_format='PNG';sc.render.filepath=os.path.join(OUT,'mami-structure-contact-20261030.png')
sc.view_settings.view_transform='AgX'
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(OUT,'mami-structure-assets-20261030.blend'))
bpy.ops.render.render(write_still=True)
print(json.dumps(report,indent=2))
