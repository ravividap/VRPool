# VRPool — Complete Build & Deploy Guide (Beginner-Friendly)

> **Audience:** You've never built a VR app before. This guide walks you through every step — from installing software on your laptop to playing VRPool on your Meta Quest 3.

---

## Table of Contents

1. [What You'll Need (Hardware)](#1-what-youll-need-hardware)
2. [Install Software on Your Laptop](#2-install-software-on-your-laptop)
   - 2.1 [Install Unity Hub & Unity Editor](#21-install-unity-hub--unity-editor)
   - 2.2 [Install Android Build Support](#22-install-android-build-support)
   - 2.3 [Install Git (to clone the project)](#23-install-git-to-clone-the-project)
3. [Set Up Your Meta Quest 3](#3-set-up-your-meta-quest-3)
   - 3.1 [Create a Meta Developer Account](#31-create-a-meta-developer-account)
   - 3.2 [Enable Developer Mode on the Headset](#32-enable-developer-mode-on-the-headset)
   - 3.3 [Install ADB Drivers (Windows Only)](#33-install-adb-drivers-windows-only)
4. [Clone & Open the VRPool Project](#4-clone--open-the-vrpool-project)
5. [Let Unity Install Packages](#5-let-unity-install-packages)
6. [Configure XR Settings in Unity](#6-configure-xr-settings-in-unity)
   - 6.1 [Enable OpenXR for Android](#61-enable-openxr-for-android)
   - 6.2 [Set OpenXR Feature Flags](#62-set-openxr-feature-flags)
   - 6.3 [Fix the Orange Warning Triangle (Interaction Profile)](#63-fix-the-orange-warning-triangle-interaction-profile)
7. [Configure Player Settings for Quest 3](#7-configure-player-settings-for-quest-3)
8. [Scene Setup — Build the Game Scene](#8-scene-setup--build-the-game-scene)
9. [Build the APK](#9-build-the-apk)
10. [Deploy (Sideload) to Your Quest 3](#10-deploy-sideload-to-your-quest-3)
    - 10.1 [USB Deployment (Recommended for First-Timers)](#101-usb-deployment-recommended-for-first-timers)
    - 10.2 [Wireless Deployment (Optional)](#102-wireless-deployment-optional)
11. [Launch & Play the Game](#11-launch--play-the-game)
12. [Troubleshooting](#12-troubleshooting)
13. [Glossary](#13-glossary)

---

## 1. What You'll Need (Hardware)

| Item | Notes |
|------|-------|
| **Laptop / Desktop** | Windows 10/11 (64-bit) or macOS 12+. At least 8 GB RAM, 20 GB free disk space. |
| **Meta Quest 3 headset** | Fully charged, with a Meta account signed in. |
| **USB-C cable** | A USB-C to USB-C (or USB-C to USB-A) data cable — the charging cable that came with your Quest works. |
| **Wi-Fi** | Needed to download Unity, packages, and SDK files. |

---

## 2. Install Software on Your Laptop

### 2.1 Install Unity Hub & Unity Editor

Unity is the game engine used to build VRPool. Unity Hub is the launcher that manages Unity versions.

1. Go to <https://unity.com/download>.
2. Download and install **Unity Hub** for your OS.
3. Open Unity Hub → click **Installs** (left sidebar) → **Install Editor**.
4. Find **Unity 2022.3.22f1 LTS** in the list.
   - If you can't find it, go to [Unity Archive](https://unity.com/releases/editor/archive) → select **2022.3.22f1** → click **Install**.
   - 🔑 **It must be exactly `2022.3.22f1`** — other versions may cause package conflicts.
5. On the **"Add modules"** screen, check the boxes described in the next section (2.2) before clicking Install.

> **What is LTS?** "Long Term Support" — it means this version gets bug fixes for 2+ years. It's the safest choice.

### 2.2 Install Android Build Support

Quest 3 runs Android internally, so you need Android tools inside Unity.

When installing Unity (step 2.1 above), check these modules:

- ☑ **Android Build Support**
- ☑ **Android SDK & NDK Tools** (sub-checkbox)
- ☑ **OpenJDK** (sub-checkbox)

> If Unity is already installed without these, go to **Unity Hub → Installs → gear icon ⚙️ on 2022.3.22f1 → Add Modules** and add them.

### 2.3 Install Git (to clone the project)

1. Go to <https://git-scm.com/downloads>.
2. Download and install for your OS (accept defaults).
3. Verify by opening a terminal / command prompt:
   ```bash
   git --version
   ```
   You should see something like `git version 2.43.0`.

---

## 3. Set Up Your Meta Quest 3

### 3.1 Create a Meta Developer Account

1. On your **phone**, open the **Meta Horizon** app (formerly Oculus app).
2. Make sure your Quest 3 is paired to the app.
3. Go to <https://developer.oculus.com/> in a browser.
4. Log in with the **same Meta account** you use on the headset.
5. Create a new "Organization" (any name, e.g., your name). This is required to enable Developer Mode.

### 3.2 Enable Developer Mode on the Headset

1. Open the **Meta Horizon app** on your phone.
2. Tap **Menu** → **Devices** → select your **Quest 3**.
3. Tap **Headset Settings** → **Developer Mode**.
4. Toggle **Developer Mode ON**.
5. **Restart your Quest 3** (hold the power button → Restart).

> **Why?** Developer Mode lets your laptop talk to the headset via USB and install apps you build yourself (called "sideloading").

### 3.3 Install ADB Drivers (Windows Only)

ADB (Android Debug Bridge) lets your laptop communicate with the Quest over USB.

- **Windows:** Download the [Oculus ADB Drivers](https://developer.oculus.com/downloads/package/oculus-adb-drivers/) → unzip → right-click `android_winusb.inf` → **Install**.
- **macOS / Linux:** ADB drivers are not needed; they work out of the box.

**Test the connection:**

1. Plug your Quest 3 into your laptop via USB-C.
2. **Put on the headset** — you'll see a popup: **"Allow USB Debugging?"** → check **"Always allow"** → tap **Allow**.
3. On your laptop, open a terminal and run:
   ```bash
   adb devices
   ```
   > If `adb` is not found, Unity bundled it here:
   > - **Windows:** `C:\Program Files\Unity\Hub\Editor\2022.3.22f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe`
   > - **macOS:** `/Applications/Unity/Hub/Editor/2022.3.22f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`

   You should see something like:
   ```
   List of devices attached
   1WMHH815T10034    device
   ```
   If it says `unauthorized`, put the headset on and accept the USB debugging prompt.

---

## 4. Clone & Open the VRPool Project

1. Open a terminal / command prompt.
2. Navigate to a folder where you want the project (e.g., `cd ~/Projects`).
3. Clone the repo:
   ```bash
   git clone https://github.com/ravividap/VRPool.git
   ```
4. Open **Unity Hub** → click **Open** → navigate to the `VRPool` folder you just cloned → click **Open**.
5. Unity Hub may ask which editor version to use — pick **2022.3.22f1**.
6. Unity will open the project. **The first load takes 5–15 minutes** as it imports assets and compiles scripts. Let it finish — don't click around.

---

## 5. Let Unity Install Packages

The project's `Packages/manifest.json` tells Unity which packages to download:

| Package | Version | What It Does |
|---------|---------|-------------|
| Meta XR All-in-One SDK | 60.0.0 | Quest 3 headset & controller support |
| Input System | 1.7.0 | Modern controller input handling |
| XR Interaction Toolkit | 2.5.2 | Grab, point, and interact in VR |
| XR Management | 4.4.0 | Manages XR loaders (OpenXR) |
| OpenXR Plugin | 1.10.0 | The standard VR runtime |
| TextMeshPro | 3.0.6 | Crisp text rendering for the HUD |
| Universal Render Pipeline | 14.0.8 | Optimized graphics for mobile VR |
| Physics, Audio, UI, XR modules | 1.0.0 | Core Unity systems |

Unity should download these automatically. If prompted to import **TextMeshPro Essentials**, click **Import**.

> **If packages fail to resolve:** Go to **Window → Package Manager** → click the refresh icon 🔄. Ensure you have internet.

---

## 6. Configure XR Settings in Unity

### 6.1 Enable OpenXR for Android

1. Go to **Edit → Project Settings** (menu bar at the top).
2. In the left panel, click **XR Plug-in Management**.
3. Click the **Android tab** (little Android robot icon 🤖).
4. Check the box next to **OpenXR**.
5. Wait for Unity to process — it may take a moment.

> ⚠️ Make sure you're on the **Android tab**, not the Desktop/PC tab. The PC tab controls PC VR (not Quest).

### 6.2 Set OpenXR Feature Flags

1. Still in Project Settings, expand **XR Plug-in Management → OpenXR** (under the Android tab).
2. Under **Enabled Interaction Profiles**, click the **+** button and add:
   - **Oculus Touch Controller Profile**
3. Under **OpenXR Feature Groups**, enable:
   - ☑ **Meta Quest Support** (or "Meta Quest Feature Group")
   - ☑ **Hand Tracking Subsystem** (optional — enable if you want hand tracking later)

### 6.3 Fix the Orange Warning Triangle (Interaction Profile)

If you see an orange ⚠️ triangle next to OpenXR:

1. Click the triangle — Unity will tell you what's wrong.
2. Usually it says "No interaction profile added." You fixed this in 6.2 above.
3. Click **Fix All** if the button appears.

---

## 7. Configure Player Settings for Quest 3

These settings are already in the repo's `ProjectSettings/ProjectSettings.asset`, but verify them:

1. Go to **Edit → Project Settings → Player** (left panel).
2. Click the **Android tab** 🤖.
3. Verify / set these values:

| Setting | Where to Find It | Value |
|---------|------------------|-------|
| **Company Name** | Player → Android | Anything (e.g., `DefaultCompany`) |
| **Product Name** | Player → Android | `VRPool` |
| **Package Name** | Other Settings → Identification | `com.DefaultCompany.VRPool` |
| **Minimum API Level** | Other Settings → Identification | **Android 12L (API 32)** |
| **Target API Level** | Other Settings → Identification | **Android 12L (API 32)** |
| **Scripting Backend** | Other Settings → Configuration | **IL2CPP** (not Mono) |
| **Target Architectures** | Other Settings → Configuration | ☑ **ARM64** |
| **Color Space** | Other Settings → Rendering | **Linear** |
| **Graphics API** | Other Settings → Rendering → Auto Graphics API OFF | **Vulkan** only (remove OpenGLES if present) |

> **Why IL2CPP?** Quest 3 requires ARM64 builds, and Mono only supports x86. IL2CPP compiles your C# code to native ARM, which is faster too.

> **Why Linear color space?** VR looks correct only in Linear mode. Gamma will make everything look washed out.

---

## 8. Scene Setup — Build the Game Scene

The repo contains all the **scripts** but not a pre-built Unity scene. You need to assemble it once.

### 8.1 Create the Scene

1. In Unity's **Project** window (bottom panel), navigate to `Assets/Scenes/`.
2. If `MainScene.unity` doesn't exist: right-click → **Create → Scene** → name it `MainScene`.
3. **Double-click** `MainScene` to open it.
4. Delete the default **Main Camera** (the VR rig will replace it).

### 8.2 Add the XR Rig (Your VR Camera + Controllers)

1. In the **Hierarchy** window (left panel), right-click → **XR → XR Origin (XR Rig)**.
   - If you don't see the XR menu: go to **Window → Package Manager → XR Interaction Toolkit → Samples** → import **Starter Assets**.
2. This creates a GameObject with a camera and two hand controllers. Position it at `(0, 0, 0)`.

### 8.3 Build the Pool Table

1. **Right-click Hierarchy → Create Empty** → name it `PoolTable`.
2. Add the component: **Inspector → Add Component → search `PoolTableManager`** → add it.
3. Inside `PoolTable`, create child GameObjects for:

   **Table Surface:**
   - Right-click `PoolTable` → **3D Object → Cube**.
   - Name it `TableSurface`. Scale it to look like a pool table top (e.g., Scale `2.74, 0.05, 1.37` — standard 9-foot table in meters).
   - In the Inspector, set its **Tag** to `TableSurface` (create the tag first via **Add Tag** if it doesn't exist).
   - Add a **Box Collider** (already on Cubes by default).

   **Cushions (6 sides):**
   - Create 6 child Cubes inside `PoolTable`, name them `Cushion_Top`, `Cushion_Bottom`, `Cushion_Left`, etc.
   - Position and scale them around the table edges.
   - **Tag** each as `Cushion`.

   **Pockets (6 triggers):**
   - Create 6 child **Empty GameObjects** inside `PoolTable`, name them `Pocket_TopLeft`, `Pocket_TopMid`, etc.
   - Position them at the 6 pocket locations (4 corners + 2 mid-sides).
   - Add a **Sphere Collider** to each → check **Is Trigger** ☑.
   - Add the **`PocketDetector`** component to each.

### 8.4 Create the Balls

1. **Right-click Hierarchy → Create Empty** → name it `Balls`.
2. For each of the 16 balls (cue ball + 15 numbered):
   - Right-click `Balls` → **3D Object → Sphere**.
   - **Scale** each to `(0.056, 0.056, 0.056)` — diameter 0.056m = radius 0.028m.
   - Add a **Rigidbody** component (for physics).
   - Add **`BallController`** component.
   - Add **`BallCollisionAudio`** component.
   - **Tag** each as `Ball` (create the tag if needed).
   - In the `BallController` Inspector:
     - Set **Ball Number**: 0 for cue ball, 1–15 for the others.
     - For the cue ball (number 0): check **Is Cue Ball** ☑.

> **Tip:** Create one ball, configure it fully, then **duplicate** it 15 times (Ctrl+D / Cmd+D) and just change the ball number and material.

### 8.5 Create the Cue Stick

1. **Right-click Hierarchy → Create Empty** → name it `CueStick`.
2. Add a child **3D Object → Cylinder** — scale it to look like a long thin cue (e.g., Scale `0.015, 0.7, 0.015`).
3. Add the **`CueController`** component to the `CueStick` parent.
4. Create an empty child called `CueTip` and position it at the thin end of the cue.
5. Create an empty child called `PowerIndicator` and add the **`ShotPowerIndicator`** component.
6. In the `CueController` Inspector:
   - Drag `CueTip` into the **Cue Tip** field.
   - Drag `PowerIndicator` into the **Shot Power Indicator** field.

### 8.6 Create Game Systems

1. **Right-click Hierarchy → Create Empty** → name it `GameSystems`.
2. Add these components to it (Inspector → Add Component):
   - `GameManager`
   - `UIManager`
   - `AudioManager`
   - `BallRack`
   - `GameInitializer`
   - `AimGuide`
   - `CueBallPlacement`
   - `VRInputHandler`
   - `WorldSpaceHUD`

### 8.7 Wire Up Serialized Fields

This is the most tedious but critical step. Each script has **public or `[SerializeField]`** fields in the Inspector that need references.

1. Click `GameSystems` in the Hierarchy.
2. For each component, drag the appropriate GameObjects / components into the empty fields:
   - `GameManager` → likely needs references to `UIManager`, `BallRack`, `AudioManager`, all ball GameObjects, the cue ball, etc.
   - `UIManager` → needs references to TextMeshPro text objects (create a **World Space Canvas** with text elements for Score, Shots, Balls Remaining).
   - `BallRack` → needs references to the 15 numbered ball GameObjects.
   - `AimGuide` → needs a **Line Renderer** component (add one to the same or child object).
   - `CueBallPlacement` → needs a reference to the cue ball.

> **How to know what to wire?** Click each component and look at the empty fields in the Inspector. Each field name tells you what to drag in. If a field says `cueBall`, drag the cue ball object there.

### 8.8 Save the Scene

- Press **Ctrl+S** (Cmd+S on Mac).

---

## 9. Build the APK

An **APK** is an Android app file — it's what gets installed on the Quest.

1. Go to **File → Build Settings**.
2. In the **Platform** list on the left, click **Android**.
3. Click **Switch Platform** (bottom-right). Wait for Unity to re-import (first time takes a few minutes).
4. Settings to verify:
   - **Run Device:** Select your Quest 3 (if plugged in and recognized).
   - **Texture Compression:** ASTC (default, best for Quest).
   - ☑ **Development Build** — enables debugging. Uncheck this for final builds.
   - ☑ **Script Debugging** (optional — helps if something crashes).
5. Click **Add Open Scenes** to add `MainScene` to the build list. Make sure it shows:
   ```
   ☑ Scenes/MainScene    0
   ```
6. Click **Build** (to create an APK file) or **Build and Run** (to build AND immediately install on the connected Quest).

   - If you clicked **Build**: choose a folder (e.g., create a `Builds/` folder inside the project). Name the file `VRPool.apk`.
   - If you clicked **Build and Run**: Unity will build the APK, push it via USB, and launch it on the headset automatically.

> **⏱ First build takes 10–30 minutes** (IL2CPP compilation is slow the first time). Subsequent builds are faster.

### Common Build Errors & Fixes

| Error | Fix |
|-------|-----|
| `Android SDK not found` | Unity Hub → Installs → ⚙️ → Add Modules → Android SDK & NDK Tools |
| `Gradle build failed` | Delete the `Library/` folder in the project and re-open Unity |
| `No Android device found` | Make sure USB Debugging is allowed on the headset, and the cable supports data (not charge-only) |
| `IL2CPP error` | Ensure you installed Android NDK via Unity Hub modules |
| `Min API Level` error | Project Settings → Player → Android → Other Settings → Min API Level = 32 |

---

## 10. Deploy (Sideload) to Your Quest 3

### 10.1 USB Deployment (Recommended for First-Timers)

**Option A: Build and Run (easiest)**
- Just click **Build and Run** in Build Settings (Step 9). Done! It installs and launches automatically.

**Option B: Manual install with ADB**
If you clicked **Build** and have a `VRPool.apk` file:

1. Open a terminal / command prompt.
2. Navigate to where `adb` lives (or add it to your system PATH):
   ```bash
   cd "C:\Program Files\Unity\Hub\Editor\2022.3.22f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools"
   ```
3. Install the APK:
   ```bash
   adb install -r "C:\path\to\your\VRPool.apk"
   ```
   The `-r` flag means "replace" if already installed.
4. You should see:
   ```
   Performing Streamed Install
   Success
   ```

### 10.2 Wireless Deployment (Optional)

Once you've done USB at least once, you can go wireless:

1. Connect via USB and ensure `adb devices` shows your Quest.
2. Run:
   ```bash
   adb tcpip 5555
   ```
3. Find the Quest's IP address: on the headset, go to **Settings → Wi-Fi → click your network → scroll down to IP address** (e.g., `192.168.1.42`).
4. Unplug the USB cable.
5. Run:
   ```bash
   adb connect 192.168.1.42:5555
   ```
6. Now you can do `adb install` wirelessly!

---

## 11. Launch & Play the Game

1. On your Quest 3, go to **App Library** (the grid icon in your home menu).
2. In the top-right, change the filter from **All** to **Unknown Sources**.
3. You'll see **VRPool** — tap it to launch.

### Controls

| Action | How |
|--------|-----|
| **Grab the cue stick** | Squeeze both controller grips at the same time |
| **Aim** | While gripping, move controllers backward (pulling the cue back) |
| **Shoot** | Release both grip triggers (like a real pool shot!) |
| **Place cue ball (after scratch)** | Grip + move the cue ball to your desired position |
| **Pause** | Press the **Menu button** (≡) on the left controller |

---

## 12. Troubleshooting

### The app doesn't appear on the Quest
- Make sure you selected **Unknown Sources** in the App Library filter.
- Verify the APK installed successfully (`adb install` showed `Success`).

### Black screen when launching
- You didn't add the XR Origin to the scene, or the scene wasn't added to Build Settings.
- Verify: **File → Build Settings** → your scene must be checked with index `0`.

### Controllers don't work
- In Project Settings → XR Plug-in Management → OpenXR → verify **Oculus Touch Controller Profile** is added.

### Balls fall through the table / no physics
- Ensure every ball has a **Rigidbody** and **Sphere Collider**.
- Ensure the table surface has a **Box Collider** (not set as trigger).

### Unity says "Unsupported platform" or "No XR loader"
- Project Settings → XR Plug-in Management → Android tab → check ☑ **OpenXR**.
- Make sure you're on the **Android tab**, not the Desktop tab.

### Build takes forever
- First IL2CPP build is slow (10–30 min). Subsequent builds reuse cached files and are much faster.
- Enable **incremental builds**: Player → Other Settings → ☑ `Incremental IL2CPP Build`.

### ADB says "unauthorized"
- Put on the headset → accept the "Allow USB Debugging?" popup → check "Always allow."

### Streaming logs from the Quest (for debugging)
Run this in a terminal while the app is running:
```bash
adb logcat -s Unity:* ActivityManager:* -v time
```
This shows Unity debug logs in real-time. Press `Ctrl+C` to stop.

---

## 13. Glossary

| Term | Meaning |
|------|---------|
| **APK** | Android Package — the app file that gets installed on the Quest |
| **ADB** | Android Debug Bridge — a command-line tool to communicate with Android devices (Quest is Android) |
| **Sideload** | Installing an app on the Quest without going through the official Meta Store |
| **OpenXR** | An open standard for VR/AR. Unity uses this to talk to the Quest hardware |
| **IL2CPP** | A Unity compilation method that converts C# to C++ → native code. Required for ARM64 (Quest) |
| **XR Origin / XR Rig** | The VR camera + controller tracking setup in Unity |
| **Developer Mode** | A Quest setting that allows USB debugging and sideloading |
| **Serialized Fields** | Variables in Unity scripts that show up in the Inspector and need to be connected to GameObjects |
| **URP** | Universal Render Pipeline — Unity's optimized rendering for mobile and VR |
| **Linear Color Space** | A color rendering mode required for correct VR visuals |

---

## Quick Reference — Full Command Cheat Sheet

```bash
# Clone the project
git clone https://github.com/ravividap/VRPool.git

# Check if Quest is connected
adb devices

# Install APK manually
adb install -r ./Builds/VRPool.apk

# Uninstall old version
adb uninstall com.DefaultCompany.VRPool

# Stream logs
adb logcat -s Unity:* -v time

# Go wireless
adb tcpip 5555
adb connect <QUEST_IP>:5555

# Take a screenshot from the Quest
adb exec-out screencap -p > screenshot.png
```

---

**You're all set! 🎱 Put on your Quest 3 and enjoy VRPool.**
