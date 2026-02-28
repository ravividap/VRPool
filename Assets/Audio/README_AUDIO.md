# Audio Files for VRPool

Place the following audio files in this directory (`Assets/Audio/`). Unity's `AudioManager` component references them via Inspector-assigned `AudioClip` fields.

## Required Audio Files

| File | Description | Duration |
|------|-------------|----------|
| `BallHitBall.wav` | Short sharp click/clack sound for ball-to-ball collisions | ~0.1 s |
| `BallHitCushion.wav` | Softer thud sound for when a ball bounces off a cushion | ~0.2 s |
| `BallPocketed.wav` | Satisfying "drop" sound played when a ball falls into a pocket | ~0.5 s |
| `CueHitBall.wav` | Clean "crack" sound for the cue stick striking the cue ball | ~0.15 s |
| `UIClick.wav` | Short UI button click sound | ~0.05 s |
| `GameWon.wav` | Victory/celebration jingle played at game end | ~3 s |

## How to Wire Up in the Inspector

1. Open the **GameSystems** GameObject in the **MainScene** hierarchy.
2. Select the **AudioManager** component.
3. Drag each `.wav` file from the Project window (`Assets/Audio/`) into the corresponding field:
   - **Ball Hit Ball Clip** → `BallHitBall.wav`
   - **Ball Hit Cushion Clip** → `BallHitCushion.wav`
   - **Ball Pocketed Clip** → `BallPocketed.wav`
   - **Cue Hit Ball Clip** → `CueHitBall.wav`
   - **UI Click Clip** → `UIClick.wav`
   - **Game Won Clip** → `GameWon.wav`

## Recommended Specifications

- Format: **WAV** (PCM 16-bit)
- Sample rate: **44100 Hz**
- Channels: **Mono** (for 3D spatial audio) or **Stereo** (for UI/music clips)
- Import settings: Set **Load Type** to *Decompress On Load* for short clips; *Streaming* for `GameWon.wav`
