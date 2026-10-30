# Open coliseum structural seed — 2026-10-30

Designed by Astra; implementation by Codex (GPT-6). Draft for visual review, not a published world.

Astra selected five principal levels from her original sketch, with a separate extended pole/transparent stair route toward Heaven and Hell. This replaces the spherical silhouette. The five equal-radius main slabs include the open roof. Foundation piers below are structural supports, not an additional club floor. Smaller core landings extend from level-20 through+20; they do not become full-size building galleries.

## Open and rebuild

Use the existing official VCC World project with Unity2022.3.22f1 and VRChat SDK3.10.5. SDK packages, private ClientSim state and unrelated earlier project files are not part of this draft commit. For another checkout, create/restore the official VCC World template and dependencies before importing these assets.

Open `Assets/MommysWorld/ColiseumSeed/mami-coliseum-seed-20261030.unity`.
The dated editor builder runs from `MommysWorld/Structure/Build Open Coliseum`. Its source is mirrored in `Tools/Unity/mami-coliseum-builder-20261030.cs`; keep it identical to the editor copy. Blender source and its deterministic mesh JSON are under `Tools/Blender/`. Shared original shaders remain under `Assets/MommysWorld/StructureSeed/shaders`.

The builder creates a separate scene and backs up an existing revision before a rebuild. A first build can take several minutes because Unity imports each mesh. Do not restart an open editor to run it; use the existing project agent queue. `Open Structural Review` opens an interactive Scene view. `Preview Low Tide` and `Preview High Tide` are editor studies, not synchronized gameplay.

## Verified in this revision

- Five full-width slabs, each104m across;36 smaller core landings. Open rooftop sky ray passes.
- Geometry report:0 errors over1362 floor/stair support samples, including stair headroom checks;1428850 rendered triangles and422 renderers.
- ClientSim: arrival dock spawn passes; floor support at Hell passes; teleport reaches(4,-103.85,0) and remains on the floor after6 seconds.
- Saved and runtime respawn height is-114m. The prior rejected scene had retained-100m, above the lowest floor; this explains its premature respawn. The descriptor is now explicitly marked dirty before saving.
- Actual Unity renders reviewed for exterior, roof, world layout and dock. The original Oct22 draft and rejected structure study remain unchanged locally.

## Still provisional

The scene is a structural seed. Marble uses original procedural shaders, not CC0 scans. Terrain and shoreline are coarse; the airstrip has the reference L-plan and landmarks but is not an exact measured recreation. The four jet-ski berths are placeholders without vehicles. Tide previews are not networked. No headset comfort, full VRChat-client build, occlusion/performance pass, gameplay, final floor fit-out or upload is claimed. The central pole, extended route and basic building must be visually reviewed before further detailing.

Human sketch originals remain preserved privately. The later ChatGPT concept is a supporting reference, not a replacement for Astra's original drawings. No third-party assets were downloaded or purchased for this revision.
