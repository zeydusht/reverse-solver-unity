# Corridor victory prototype

The active approved menu is `Assets/_Game/Presentation/Resources/menu_bg.png`, matching `Assets/Art/Menu/corridor-v04-source.png`. That source image is unchanged. The older `Menu/InfiniteCorridor` demo has a different view and continuous motion; this implementation does not use it.

## Review

Run `node Tools/corridor_victory_preview.js`, then open <http://127.0.0.1:8840/docs/art/corridor-victory/index.html>. Click **Simulate victory → menu**. It plays once for three seconds and stops. The ordinary-return button stays static; the slider inspects individual moments. The painting checkbox exercises the level-picture replacement. The browser UI is a stationary review overlay, not a replacement for the game's menu UI.

`qa/corridor-victory.webm` is a standalone recorded preview. `qa/frame-*.png` contains inspection frames; `qa/unity/` contains renders from the actual Unity shader. No live site or player build was published.

## Game behavior

In Unity, win a level using the approved-art menu and select **Main menu** on the result card. One transition begins on that menu return. Startup, ordinary menu visits and losses do not independently trigger it. The existing **Next** action still enters the next level directly. Wins queue one pending menu transition; multiple wins before visiting the menu coalesce. Leaving the menu early cancels the visible transition rather than blocking interaction. Resizing during playback retains the original three-second clock.

`GameRoot.OnEvent` queues a win; `ShowMenu` consumes it. `Screens.Menu.PlayVictory` attaches the animation only to the background renderer. Its level-painting child is sampled into the same projection during travel, then restored. Other UI renderers and their input remain unchanged. The legacy menu stays static. There is no idle update or continuous animation.

## Rendering and assets

The implementation uses a single unlit 2.5D projection shader. Camera rays intersect a slanted repeating wall, a horizontal floor, a shallow picture layer and foreground column/arch planes. The camera moves left by 3.42 scene units and back by 2.0007 units, with an additional 0.32-unit pullback at the midpoint. Quintic easing gives zero endpoint velocity and acceleration. Adjacent bay silhouettes are sampled across boundaries.

New artwork is separate in `Assets/Art/Menu/CorridorVictory/`; runtime copies and the shader are in `Assets/_Game/Presentation/Resources/CorridorVictory/`. The large picture/frame is extracted from the approved source. The clean wall plate, continuous floor material, complete pillar and arch were made with built-in **image_gen**, using the approved corridor as the reference. Exact prompts are saved in `docs/art/corridor-victory-prompts.json`; source paths are in `corridor-victory-input.json`. The build script uses project-local sources and Pillow; the manifest records the original source hash and output hashes.

The browser and Unity use the same `CorridorProjection.cginc` projection math. Textures have no mipmaps, a 2048-pixel maximum, clamp sampling and compressed runtime imports. Generated source art is kept outside Resources. Only one background draw changes material while active; at completion its original material is restored.

## Validation and remaining work

- Four Unity EditMode tests cover startup, win consumption, repeat wins, cancellation and camera travel.
- The Unity GPU smoke check renders the actual material through Direct3D12 and compares exact endpoint PNG bytes. Run with `-executeMethod ReverseSolver.Editor.CorridorVictoryRenderCheck.Run` in Unity batch mode with graphics enabled.
- `node Tools/corridor_victory_preview.js --verify` checks shader compilation, identical endpoint images, dynamic-picture endpoints, duplicate-trigger rejection, one completion, stopped idle rendering and browser exceptions. It also records the movie.
- Desktop Chrome GPU playback measured approximately 60 fps (see `qa/report.json`). This is not a mobile performance measurement.

**This is a reviewable prototype, not final art approval.** The reconstructed camera scene differs from the single painted perspective. To retain the exact approved static menu, it blends to/from that image over the first and last 0.36 seconds. That blend can reveal a change in architecture/lighting; it is not a fully seamless physical reconstruction. Further art calibration is required to meet the requested invisible handoff and exact proportions throughout travel. The foreground elements are textured planes rather than full volumetric models. No complete Web player/iOS build or real iPhone test has been performed. Do not treat identical endpoint checks as proof of seamless motion or production readiness.
