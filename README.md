# VRPool

A single-player VR pool (billiards) game for **Meta Quest 3**, built with **Unity 2022.3 LTS** and **C#**.

---

## Features

- ⚽ Full 15-ball rack with standard pool rules
- 🎱 Physically accurate ball physics (rolling friction, cushion bounce, spin drag)
- 🕹️ Intuitive VR cue interaction — grip both controllers, pull back and push forward to shoot
- 💡 Real-time shot-power indicator (green → red gradient)
- 🎯 Aim guide showing cue-ball and object-ball predicted paths
- �� World-space HUD: score, shots taken, balls remaining
- 🔊 Spatial audio for ball collisions, cushion hits, and pocketing
- 🔁 Play-again flow with full ball reset
- 🎮 Pause menu via Menu button
- 📲 Targets Android API 32 (Quest 3 minimum)

---

## Project Structure

```
VRPool/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs          – Game state, turns, scoring
│   │   ├── BallController.cs       – Ball physics & pocketing
│   │   ├── BallRack.cs             – Standard triangle rack setup
│   │   ├── BallCollisionAudio.cs   – Per-ball collision sounds
│   │   ├── CueController.cs        – VR cue input & shot mechanics
│   │   ├── CueBallPlacement.cs     – Cue-ball-in-hand (after scratch)
│   │   ├── AimGuide.cs             – Predictive shot line renderer
│   │   ├── ShotPowerIndicator.cs   – Visual power feedback
│   │   ├── PoolTableManager.cs     – Table dimensions & physics materials
│   │   ├── PocketDetector.cs       – Pocket trigger collision handler
│   │   ├── UIManager.cs            – World-space HUD & game-over panel
│   │   ├── WorldSpaceHUD.cs        – HUD follow-head behaviour
│   │   ├── AudioManager.cs         – Pooled audio source manager
│   │   ├── VRInputHandler.cs       – Quest 3 controller input & pause
│   │   ├── GameInitializer.cs      – XR rig bootstrap & app settings
│   │   └── VRPool.asmdef           – Assembly definition
│   ├── Scenes/                     – Unity scene files (add in Editor)
│   ├── Materials/                  – Table felt, ball, cushion materials
│   ├── Prefabs/                    – Ball, cue, table, pocket prefabs
│   ├── Audio/                      – Sound clips
│   └── Shaders/                    – Custom URP shaders (optional)
├── Packages/
│   └── manifest.json               – Package dependencies
└── ProjectSettings/
    ├── ProjectSettings.asset       – Build target: Android/Quest 3
    ├── ProjectVersion.txt          – Unity 2022.3.22f1
    └── XRGeneralSettings.asset     – OpenXR loader configuration
```

---

## Prerequisites

| Tool | Version |
|------|---------|
| Unity | 2022.3.22f1 LTS |
| Meta XR All-in-One SDK | 60.0.0 |
| XR Interaction Toolkit | 2.5.2 |
| OpenXR Plugin | 1.10.0 |
| Android Build Support | (included with Unity) |
| Meta Quest Developer Hub (optional) | latest |

> 📖 **Full build and test instructions** — including Developer Mode setup, ADB configuration, sideloading, log streaming, wireless deployment, and a testing checklist — are in **[BUILDING_AND_TESTING.md](BUILDING_AND_TESTING.md)**.

---

## Quick Start

### 1. Open the Project

1. Launch **Unity Hub**.
2. Click **Open** and select the `VRPool` folder.
3. Unity will install required packages from `Packages/manifest.json` automatically.

### 2. Configure XR

1. Go to **Edit → Project Settings → XR Plug-in Management**.
2. Under the **Android** tab, enable **OpenXR**.
3. Under **OpenXR → Features**, enable:
   - **Meta Quest: Support**
   - **Oculus Touch Controller Profile**
   - **Hand Tracking Subsystem** (optional)

### 3. Build and Deploy to Quest 3

1. Enable **Developer Mode** on the headset (see [BUILDING_AND_TESTING.md § 2](BUILDING_AND_TESTING.md#2-enable-developer-mode-on-quest-3)).
2. Connect Quest 3 via USB and accept the **Allow USB Debugging** prompt.
3. In Unity: **File → Build Settings** → platform **Android** → ☑ **Development Build** → **Build and Run**.
4. The APK is installed and launched automatically on the headset.

### 4. Scene Setup

Open `Assets/Scenes/MainScene.unity` (create it if not present) and:

1. Add an **XR Origin (XR Rig)** from the XR Interaction Toolkit sample scenes.
2. Create a **Pool Table** GameObject:
   - Add `PoolTableManager` component.
   - Tag the surface collider as `TableSurface`.
   - Tag cushion colliders as `Cushion`.
   - Add 6 pocket trigger GameObjects with `PocketDetector` components.
3. Create 15 ball GameObjects (spheres, radius 0.028 m):
   - Add `BallController` and `BallCollisionAudio` components.
   - Tag each as `Ball`.
   - Set ball numbers 1–15; separate cue ball (number 0, `IsCueBall = true`).
4. Add a **Cue Stick** GameObject:
   - Add `CueController` component.
   - Assign `cueTip` transform and `ShotPowerIndicator` child.
5. Add `GameManager`, `UIManager`, `AudioManager`, `BallRack`, `GameInitializer` to a persistent **GameSystems** empty GameObject.
6. Wire all serialized fields in the Inspector.

---

## Controls (Quest 3)

| Action | Input |
|--------|-------|
| Grab cue | Grip both controllers simultaneously |
| Pull back to aim | Move controllers backward while gripping |
| Release to shoot | Release both grip triggers |
| Pause / Menu | Menu button (left controller) |
| Place cue ball | Grip + move after a scratch |

---

## Scoring

| Ball | Points |
|------|--------|
| Solid (1–7) | +1 |
| Stripe (9–15) | +2 |
| 8-ball | +5 |
| Scratch | −5 |

---

## License

MIT
