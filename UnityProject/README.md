# SUBR-Style Unity Project (Code Scaffold)

Ye **sirf code + design** hai.  
**Tum Unity Editor mein** scenes, prefabs, models, animations, build settings handle karoge.

| | |
|--|--|
| **Engine** | Unity 2022.3 LTS (SUBR ke close: 2022.3.x) |
| **Language** | C# |
| **Backend target** | IL2CPP + arm64 (jab Android build lo) |
| **Package** | tum set karo → example `com.yourstudio.subr` |
| **Scope** | Battle / survival **systems** — movement, weapons, damage, zone, bots, UI hooks, match flow |

## Tum kya karoge (Unity side)

1. Unity Hub → **2022.3 LTS** install  
2. Naya 3D project banao **ya** is `Assets/Scripts` folder ko apne project mein copy karo  
3. Scene: `Boot` → `Lobby` → `Match`  
4. Prefabs: Player, Bot, Weapon pickup, Zone  
5. Input: New Input System **optional** — abhi scripts **old Input** + hooks dono style support  
6. Android: IL2CPP, ARM64, package name, keystore — tumhari build

## Main (agent) kya diya

```
Assets/Scripts/
  Core/         Game bootstrap, events, services
  Player/       controller, health, inventory bind
  Combat/       damage, hit detection
  Weapons/      gun base, projectile, hitscan
  AI/           simple bot brain
  GameMode/     match states, safe zone, win condition
  UI/           HUD / lobby presenters (logic)
  Network/      interfaces + offline stub (baad mein Photon/Mirror)
  Inventory/    items, loadout
  Camera/       TPS camera
  Data/         ScriptableObject definitions
  Utility/      helpers
```

## Quick start (Editor)

1. Copy `UnityProject/Assets` → apne Unity project ke `Assets/` mein  
2. Menu (after scripts compile): use components on empty GOs as documented per script header  
3. Create ScriptableObjects: `Right click → Create → SUBR → …`  
4. Play mode mein Offline match test

## Repo layout

```
SUBR-Z/
  UnityProject/          ← ye naya game CODE
  docs/                  ← APK build types + how to pack old dump
  build_apk.sh           ← purane unpacked APK ke liye
  (dex, lib.zip, …)      ← original SUBR dump analysis files
```

## Collaboration model

```
Agent  →  C# systems, architecture, refactors, GitHub
Tum    →  Unity Editor, art, levels, polish, Android build, Play upload
```

Detail: `Docs/UNITY_SETUP.md` + `Docs/GDD_SHORT.md`
