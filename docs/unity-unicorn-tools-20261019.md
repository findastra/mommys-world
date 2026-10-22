# Unity Unicorn tools we can build with

**None chosen yet.** We pick tools space by space, once each space is designed. This list was taken from
[Unity Unicorn](https://github.com/findastra/unity-unicorn)'s page on 2026-10-19: its "World tools" tab, in the panel
for every world. The page is the up-to-date list.

## Before and while building

- **world-spawn-tool:** spawns a new world. It made the public repo name for this one on 2026-10-19. Its Unity step
  still copies from a reference world, so it must be changed to start from the VCC World template before we use it.
- **asset-ledger:** one row for every asset before it ships, and credits for each world. It is a design so far, and
  this repo's [`asset-ledger-20261019.json`](asset-ledger-20261019.json) follows it.
- **unity-asset-creator:** walks an asset through the 12-step checklist, including the "make it look made, not
  generated" craft section, then exports each version.
- **shaders-shop:** installs shaders after showing their files, sources and licences, and keeps track of which shaders
  a world uses.
- **prop-decomposer:** takes a real reference prop apart in Blender (parts, true sizes, materials) so ours can be made
  from it.
- **realistic-prop-restyle:** restyles a CC0 reference prop by mask, keeping its wear and painting out brand marks.

## Checking the build

- **world-validator:** finds missing scripts and references, checks walkable surfaces and checks content budgets.
- **button-check:** finds buttons and controls with missing targets or wrong wiring.
- **scene-yaml-inspection:** reads saved scene data to check references and settings.
- **asset-contact-sheet:** draws every mesh and material on one sheet for comparison.
- **occlusion-bake** and **reflection-probe-bake:** the bakes before a test.
- **desktop-world-test:** builds the world for a local VRChat desktop test, without uploading.
- **run-unity-tool** and **foreground-tool-recorder:** run a tool and record its real run.

## Not ours to reuse

- **unity-assets-library** holds finished assets from other worlds (the Liminal Doors booth and sky). Mommy's World is
  built from scratch, so we use them only if Astra names one.
- The world panels for Piranesi House, Astra's Infinite Pole and Liminal Doors hold tools made for those worlds. We
  take one only if Astra names it, as with any other piece.
