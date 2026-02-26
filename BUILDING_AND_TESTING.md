# Building and Testing VRPool on Meta Quest 3

This guide walks you through every step needed to get VRPool running on a Meta Quest 3 headset — from first-time device setup through iterative test-and-debug cycles.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Enable Developer Mode on Quest 3](#2-enable-developer-mode-on-quest-3)
3. [Install and Configure ADB](#3-install-and-configure-adb)
4. [Configure Unity for Android / Quest 3](#4-configure-unity-for-android--quest-3)
5. [Build the APK](#5-build-the-apk)
6. [Deploy to Quest 3](#6-deploy-to-quest-3)
7. [Testing Checklist](#7-testing-checklist)
8. [Viewing Logs and Debugging](#8-viewing-logs-and-debugging)
9. [Wireless Deployment (optional)](#9-wireless-deployment-optional)
10. [Performance Profiling](#10-performance-profiling)
11. [Common Errors and Fixes](#11-common-errors-and-fixes)

---

## 1. Prerequisites

Install all of the following before continuing.

### Software

| Tool | Where to get it | Notes |
|------|----------------|-------|
| **Unity Hub** | [unity.com/download](https://unity.com/download) | Required to manage Unity versions |
| **Unity 2022.3.22f1 LTS** | Unity Hub → Installs → Add | Must include **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK** modules |
| **Meta Quest Developer Hub (MQDH)** | [developer.oculus.com/meta-quest-developer-hub](https://developer.oculus.com/meta-quest-developer-hub/) | Simplifies sideloading and log viewing |
| **Android SDK Platform-Tools** (ADB) | Bundled with Unity *or* [developer.android.com/tools/releases/platform-tools](https://developer.android.com/tools/releases/platform-tools) | Needed for command-line deployment |
| **Meta developer account** | [developer.oculus.com](https://developer.oculus.com) | Free; required to create an Organisation for Developer Mode |
| **Meta Quest mobile app** | iOS App Store / Google Play | Required to enable Developer Mode |

### Unity Modules (must be installed)

When installing Unity 2022.3.22f1 LTS via Unity Hub, tick these additional modules:

- ☑ Android Build Support
- ☑ Android SDK & NDK Tools
- ☑ OpenJDK

> **Tip:** To add modules to an existing Unity install, open Unity Hub → **Installs**, click the gear ⚙ next to 2022.3.22f1, and choose **Add Modules**.

---

## 2. Enable Developer Mode on Quest 3

Developer Mode must be enabled on the headset before Unity or ADB can deploy to it.

### Step-by-step

1. **Create or join a Meta developer organisation**
   - Go to [developer.oculus.com/manage/organizations](https://developer.oculus.com/manage/organizations/) and sign in.
   - If you don't have an organisation, click **Create New Organization**, accept the terms, and give it any name.

2. **Enable Developer Mode via the Meta Quest mobile app**
   - Open the **Meta Quest** app on your phone.
   - Ensure your Quest 3 is powered on and paired.
   - Tap the **Menu** icon (bottom right) → **Devices** → select your Quest 3.
   - Tap **Headset Settings** → **Developer Mode** → toggle **Developer Mode** on.
   - Put on the headset and confirm the prompt that appears inside.

3. **Verify inside the headset**
   - Go to **Settings → System → Developer** — you should now see developer options (USB Connection Dialog, etc.).

---

## 3. Install and Configure ADB

ADB (Android Debug Bridge) lets you deploy APKs and stream logs from the command line.

### Option A — Use the ADB bundled with Unity

Unity installs platform-tools alongside the Android SDK. The default path is:

| OS | Path |
|----|------|
| Windows | `C:\Program Files\Unity\Hub\Editor\2022.3.22f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\` |
| macOS | `/Applications/Unity/Hub/Editor/2022.3.22f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/` |
| Linux | `~/Unity/Hub/Editor/2022.3.22f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/` |

Add this directory to your system `PATH` environment variable so `adb` works in any terminal.

### Option B — Install standalone Android platform-tools

Download from [developer.android.com/tools/releases/platform-tools](https://developer.android.com/tools/releases/platform-tools), extract anywhere, and add the folder to `PATH`.

### Connect Quest 3 via USB

1. Plug a **USB-C cable** from the Quest 3 to your computer.
2. Put on the headset — a **"Allow USB Debugging?"** dialog will appear.
   - Tap **Allow** (tick **Always allow from this computer** to avoid future prompts).
3. Open a terminal and verify the connection:

   ```bash
   adb devices
   ```

   Expected output:

   ```
   List of devices attached
   1XXXXXXXXXXXXXXX    device
   ```

   If the status shows `unauthorized`, put on the headset again and accept the USB Debugging prompt.

---

## 4. Configure Unity for Android / Quest 3

### 4.1 Open the project

1. Open **Unity Hub** → **Projects** → **Open** → select the `VRPool` folder.
2. Wait for Unity to import assets and download packages (first open may take several minutes).

### 4.2 Switch to Android platform

1. **File → Build Settings** (`Ctrl+Shift+B` / `Cmd+Shift+B`).
2. Select **Android** in the Platform list.
3. Click **Switch Platform** and wait for reimport.

### 4.3 Player Settings — Android

Open **Edit → Project Settings → Player** (or click **Player Settings** in Build Settings) and set:

| Setting | Value |
|---------|-------|
| Company Name | Your name |
| Product Name | VRPool |
| Package Name | `com.YourName.VRPool` |
| Minimum API Level | **API 32 (Android 12L)** |
| Target API Level | **API 32** |
| Scripting Backend | **IL2CPP** |
| Target Architectures | ☑ **ARM64** only (Quest 3 is 64-bit) |
| Texture Compression | **ASTC** |
| Internet Access | Not required |

### 4.4 Configure XR Plug-in Management

1. **Edit → Project Settings → XR Plug-in Management** → click the **Android** tab.
2. Check **OpenXR**.
3. Expand **OpenXR** in the left panel → **Features** tab.
4. Enable:
   - ☑ **Meta Quest: Support** (sets Quest-specific display settings)
   - ☑ **Oculus Touch Controller Profile**
   - ☑ **Hand Tracking Subsystem** (optional, for hand tracking)
5. Set **Render Mode** to **Multi-pass** or **Single Pass Instanced** (Single Pass is more performant).

### 4.5 Android SDK and NDK paths (if not auto-detected)

Go to **Edit → Preferences → External Tools** and confirm Unity points to its bundled SDK/NDK/JDK, or set custom paths:

| Field | Default Unity path |
|-------|--------------------|
| JDK | `<Unity install>/Editor/Data/PlaybackEngines/AndroidPlayer/OpenJDK` |
| SDK | `<Unity install>/Editor/Data/PlaybackEngines/AndroidPlayer/SDK` |
| NDK | `<Unity install>/Editor/Data/PlaybackEngines/AndroidPlayer/NDK` |

---

## 5. Build the APK

### 5.1 Add the scene

1. Ensure `Assets/Scenes/MainScene.unity` is in the **Scenes In Build** list.
   - If not, open the scene and click **Add Open Scenes** in Build Settings.

### 5.2 Development build (recommended for testing)

1. In **File → Build Settings**:
   - ☑ **Development Build**
   - ☑ **Script Debugging** (to see C# exceptions in logcat)
   - ☑ **Wait For Managed Debugger** — leave **unchecked** unless you need to attach the debugger on launch.
2. Click **Build** and choose an output folder (e.g., `Builds/`).
   - Unity generates `VRPool.apk` (or `VRPool.aab` for app bundle).

> The first build with IL2CPP takes 5–15 minutes. Subsequent builds are faster due to incremental compilation.

### 5.3 Release build (for distribution)

1. Uncheck **Development Build**.
2. Under **Player Settings → Publishing Settings**, create or select a **Keystore**:
   - Click **Keystore Manager** → **Create New** → follow the prompts.
   - Store the `.keystore` file and passwords securely — you cannot update the app without them.
3. Click **Build**.

---

## 6. Deploy to Quest 3

### Option A — Build and Run (one-click, fastest)

With the Quest 3 connected via USB and `adb devices` showing `device`:

1. In **File → Build Settings**, click **Build and Run**.
2. Unity builds the APK and automatically installs + launches it on the headset.
3. Put on the headset — the game launches immediately.

### Option B — Manual ADB install

```bash
# Install the APK
adb install -r Builds/VRPool.apk

# Launch the app (replace package name if changed)
adb shell am start -n com.YourName.VRPool/com.unity3d.player.UnityPlayerActivity
```

`-r` flag reinstalls over an existing version without uninstalling first.

### Option C — Meta Quest Developer Hub (GUI)

1. Open **MQDH** → connect your headset (it should appear in the device list).
2. Go to **Device Manager → App Manager**.
3. Drag `VRPool.apk` onto the window, or click **Install APK** and browse to the file.
4. The app appears under **Unknown Sources** in the headset library.

### Finding the app on the headset

After install, put on the Quest 3 and open the **App Library** → filter by **Unknown Sources** → tap **VRPool**.

---

## 7. Testing Checklist

Work through this checklist each time you deploy a new build.

### Launch and orientation

- [ ] App launches without a black screen or crash
- [ ] Player spawns at the correct position facing the pool table
- [ ] Pool balls are racked correctly (triangle formation)
- [ ] World-space HUD is visible and readable

### VR cue mechanics

- [ ] Gripping both controllers shows the cue stick
- [ ] Shot power indicator scales green → red as you pull back
- [ ] Releasing the triggers fires the cue ball
- [ ] Haptic feedback is felt on shot

### Ball physics

- [ ] Cue ball rolls and slows naturally (rolling friction)
- [ ] Balls bounce off cushions at the correct angle
- [ ] Pocketed balls disappear and the pocket count increments
- [ ] A scratch (cue ball pocketed) triggers cue-ball-in-hand mode

### Cue-ball-in-hand

- [ ] Cue ball is grabbable and can be moved with a controller
- [ ] Ball is constrained to the table surface height
- [ ] Dropping the ball behind the head string completes the placement
- [ ] Dropping outside the valid zone keeps placement mode active
- [ ] HUD shows "Scratch! -5 points. Place the cue ball."

### Aim guide

- [ ] White dotted line extends from cue ball in the shot direction
- [ ] Line stops at the first object ball encountered

### Score and game flow

- [ ] Score updates correctly (+1 solid, +2 stripe, +5 eight-ball, -5 scratch)
- [ ] Pocketing all 15 balls transitions to the game-over screen
- [ ] **Play Again** button resets and reruns correctly

### Pause menu

- [ ] Pressing the **Menu** button on the left controller opens the pause panel
- [ ] Pressing again resumes gameplay
- [ ] Time pauses (balls stop moving)

### Audio

- [ ] Ball-on-ball collision sound plays
- [ ] Ball-on-cushion collision sound plays
- [ ] Pocketed ball sound plays
- [ ] Cue-strikes-ball sound plays

---

## 8. Viewing Logs and Debugging

### Stream Unity logs via ADB logcat

```bash
# All Unity/Android output
adb logcat -s Unity

# Broader filter — includes Java/OpenXR errors
adb logcat -s Unity:V AndroidRuntime:E
```

Press `Ctrl+C` to stop. Logs appear in real time as the app runs on the headset.

### Capture a crash log

```bash
adb logcat -d > crash_log.txt
```

`-d` dumps the current log buffer to a file.

### Unity Remote Debugger

To attach the C# debugger in the Unity Editor to a running build on device:

1. Build with **Development Build** + **Script Debugging** enabled.
2. In Unity, go to **Edit → Attach to Unity Process** (or use your IDE's attach feature).
3. Select the Quest 3 process from the list.

> You can set breakpoints in Visual Studio / Rider and step through C# code running on the headset.

### Meta Quest Developer Hub — Device Logs

MQDH has a built-in log viewer:

1. Open MQDH → **Device Manager** → select your headset.
2. Click the **Logs** tab for a filtered, colour-coded view.

### Viewing performance overlay on device

Inside the Quest 3 headset:
1. **Settings → Developer → Performance Overlay** → enable it.
2. A HUD appears showing GPU %, CPU %, frame time, and dropped frames.

---

## 9. Wireless Deployment (optional)

Deploying over Wi-Fi lets you test without a cable attached.

### Requirements

- Quest 3 and your PC on the **same Wi-Fi network**.
- ADB already authorised via USB at least once.

### Steps

```bash
# 1. Connect via USB and enable TCP/IP mode on port 5555
adb tcpip 5555

# 2. Find the Quest 3 IP address (visible in Settings → Wi-Fi → connected network)
# Example: 192.168.1.42

# 3. Connect wirelessly
adb connect 192.168.1.42:5555

# 4. Verify
adb devices
# Should show:  192.168.1.42:5555   device

# 5. You can now unplug the USB cable and use Build and Run or adb install normally
```

To return to USB mode:

```bash
adb usb
```

---

## 10. Performance Profiling

Quest 3 targets **72 Hz** minimum, **90 Hz** recommended. Use the Unity Profiler to catch bottlenecks.

### Unity Profiler over USB

1. Build with **Development Build** + **Autoconnect Profiler** checked.
2. In the Editor, open **Window → Analysis → Profiler**.
3. Set the connection dropdown to **Android Player** (it appears when a dev build is running on device).
4. The Profiler streams CPU, GPU, memory, and physics data in real time.

### Key metrics to watch

| Metric | Quest 3 target |
|--------|---------------|
| Frame time | ≤ 11 ms (90 Hz) |
| CPU (Main thread) | ≤ 6 ms |
| GPU | ≤ 9 ms |
| Heap allocations per frame | 0 (avoid GC spikes) |
| Draw calls | ≤ 100 |

### Quest-specific tips

- Enable **Fixed Foveated Rendering** in the OpenXR Meta Quest feature settings to reduce GPU load at screen edges.
- Use **Single Pass Instanced** rendering (set in XR Plug-in Management → OpenXR).
- Avoid `FindObjectOfType` and LINQ in hot paths — cache references in `Awake`/`Start`.
- Keep ball collider count low; use `ContinuousDynamic` only on fast-moving balls.

---

## 11. Common Errors and Fixes

| Symptom | Likely cause | Fix |
|---------|-------------|-----|
| `adb devices` shows `unauthorized` | USB Debugging prompt not accepted | Put on headset, accept the "Allow USB Debugging?" dialog |
| `adb devices` shows nothing | USB cable or driver issue | Try a different USB-C cable; install **Oculus ADB Drivers** on Windows from [developer.oculus.com/downloads](https://developer.oculus.com/downloads/package/oculus-adb-drivers/) |
| Build fails: *"Android SDK not found"* | SDK path not configured | Go to **Edit → Preferences → External Tools** and point Unity to its bundled SDK |
| Build fails: *"NDK not found"* | Same as above but NDK | Ensure **Android SDK & NDK Tools** module is installed via Unity Hub |
| App installs but immediately crashes | IL2CPP strip error | Set **Managed Stripping Level** to **Minimal** in Player Settings |
| Black screen after launch | OpenXR not initialised | Ensure **Meta Quest: Support** feature is enabled under XR Plug-in Management → OpenXR |
| Controllers not tracked | Missing controller profile | Enable **Oculus Touch Controller Profile** under OpenXR Features |
| App not visible in headset library | Installed under wrong source | Check **App Library → Unknown Sources** |
| `INSTALL_FAILED_VERSION_DOWNGRADE` | Trying to install older APK over newer | `adb uninstall com.YourName.VRPool` then reinstall |
| Extreme jitter on balls | Physics step too large | Ensure `Fixed Timestep` is `0.01111` (90 Hz) in **Edit → Project Settings → Time** |
| HUD not visible in headset | WorldSpaceHUD `headTransform` not assigned | Assign the **Camera** (Main Camera under XR Origin) to the `headTransform` field |

---

## Quick-Reference: Full Deploy Cycle

```bash
# 1. Verify device is connected
adb devices

# 2. Build from Unity (File → Build Settings → Build)
#    → outputs Builds/VRPool.apk

# 3. Install and launch
adb install -r Builds/VRPool.apk
adb shell am start -n com.YourName.VRPool/com.unity3d.player.UnityPlayerActivity

# 4. Stream logs
adb logcat -s Unity

# 5. When done, stop the app
adb shell am force-stop com.YourName.VRPool
```
