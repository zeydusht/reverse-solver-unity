# Corridor victory - original-art consistency V2

The active approved menu is `Assets/_Game/Presentation/Resources/menu_bg.png`, matching `Assets/Art/Menu/corridor-v04-source.png`. That source image is unchanged. The older `Menu/InfiniteCorridor` demo has a different view and continuous motion; this implementation does not use it.

## Review

Run `node Tools/corridor_victory_preview.js`, then open <http://127.0.0.1:8840/docs/art/corridor-victory/index.html>. Click **Simulate victory → menu**. It plays once for three seconds and stops. The ordinary-return button stays static; the slider inspects individual moments. The painting checkbox exercises the level-picture replacement. The browser UI is a stationary review overlay, not a replacement for the game's menu UI.

`qa/corridor-victory.webm` is a standalone recorded preview. `qa/frame-*.png` contains inspection frames; `qa/unity/` contains renders from the actual Unity shader. No live site or player build was published.

V2 review sheets: `qa/checkpoints-v2.png` (0/25/50/75/100%), `qa/before-after-v2.png` (old/new middle frame), and `qa/consecutive-middle-v2.png` (nine consecutive 60 fps frames). The complete previous working version, including art, shader, scripts, movie and reports, is preserved in `docs/art/backups/corridor-victory-v1-20261030/`.

## Game behavior

In Unity, win a level using the approved-art menu and select **Main menu** on the result card. One transition begins on that menu return. Startup, ordinary menu visits and losses do not independently trigger it. The existing **Next** action still enters the next level directly. Wins queue one pending menu transition; multiple wins before visiting the menu coalesce. Leaving the menu early cancels the visible transition rather than blocking interaction. Resizing during playback retains the original three-second clock.

`GameRoot.OnEvent` queues a win; `ShowMenu` consumes it. `Screens.Menu.PlayVictory` attaches the animation only to the background renderer. Its level-painting child is sampled into the same projection during travel, then restored. Other UI renderers and their input remain unchanged. The legacy menu stays static. There is no idle update or continuous animation.

## Rendering and assets

The implementation uses a single unlit 2.5D projection shader. Camera rays intersect a slanted repeating wall, a horizontal floor, a shallow picture layer and foreground column/arch planes. The camera moves left by 3.42 scene units and back by 2.0007 units, with an additional 0.32-unit pullback at the midpoint. Quintic easing gives zero endpoint velocity and acceleration. Adjacent bay silhouettes are sampled across boundaries.

The original mismatch came from replacing the middle scene with a cropped wall and independently generated pillar, arch and carpet assets, then blending back into the approved image. V2 removes those replacements and the time-dependent endpoint art blend.

Current processed artwork is in `Assets/Art/Menu/CorridorVictoryV2/`; runtime copies and the shader remain in `Assets/_Game/Presentation/Resources/CorridorVictory/`. Original visible pixels sample `menu_bg.png` directly, preserving the exact frame ornament, small distant paintings, arches, banners, plants, wall patterns and carpet. Extracted masks provide depth and occlusion. Hidden wall patches clone original plaster/panel pixels. The floor atlas reprojects original floor pixels and extends missing areas from the same material. Shared world coordinates keep the repeated layers aligned.

One narrow, previously unseen join required a built-in **image_gen** repair, referenced to the approved artwork. Its selected source is `CorridorVictoryV2/join-repair-source.png`; exact prompts are saved in `docs/art/corridor-victory-v2-prompts.json`. Only the repair strip is back-projected onto the fixed scene plane. It does not replace a whole animation frame. The same immutable texture is reused throughout the transition: there are no independently generated frames or changing decorations.

Run `docs/art/build-corridor-victory-v2.py` with Pillow to rebuild the assets. The existing `build-corridor-victory.py` command forwards to V2. All required art sources are saved in the project; the manifest records provenance, hashes and repair calibration. Old V1 art remains separately preserved.

The browser and Unity use the same `CorridorProjection.cginc` projection math. Textures have no mipmaps, a 2048-pixel maximum and clamp sampling. Artwork imports are compressed; the packed source-validity texture is uncompressed linear data. The existing `arches.png` resource binding now contains wall validity in R and floor validity in G. Repair source art stays outside Resources. Only the background draw changes material while active; at completion its original material is restored.

## Validation and remaining work

- Four Unity EditMode tests cover startup, win consumption, repeat wins, cancellation and camera travel.
- The Unity GPU smoke check renders the actual material through Direct3D12 and compares exact endpoint PNG bytes. Run with `-executeMethod ReverseSolver.Editor.CorridorVictoryRenderCheck.Run` in Unity batch mode with graphics enabled.
- `node Tools/corridor_victory_preview.js --verify` checks shader compilation, identical endpoint images, dynamic-picture endpoints, duplicate-trigger rejection, one completion, stopped idle rendering and browser exceptions. It also records the movie.
- Desktop Chrome GPU playback measured approximately 60 fps (see `qa/report.json`). This is not a mobile performance measurement.

- `docs/art/check-corridor-victory-v2.py` compares all protected camera/trigger/original hashes, inspects 181 consecutive rendered thumbnails for isolated temporal spikes, and writes the contact sheets and `qa/art-consistency-report.json`.
- The camera, easing, duration, trigger queue, `GameRoot` and `Screens` source files match the pre-update hashes. Both endpoints still use the exact approved composition. The near-endpoint RGB difference measured approximately 0.023 on a 0-255 scale in the phone preview; no endpoint image fade is used.

The five checkpoints and consecutive middle frames were visually inspected against the approved source. Temporal statistics support that review; they do not independently establish artistic quality. Foreground elements remain textured planes. No complete Web player/iOS build or real iPhone performance test has been performed. The current review is of the existing phone framing and the actual Unity shader, not a mobile-device certification.
