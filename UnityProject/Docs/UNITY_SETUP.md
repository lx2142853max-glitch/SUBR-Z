# Unity Setup (tumhara kaam)

## 1. Version
- **Unity 2022.3 LTS** (SUBR reference build `2022.3.62f2` ke paas)
- Modules: **Android Build Support**, OpenJDK, Android SDK & NDK Tools

## 2. Project
Option A — naya project:
1. Hub → New Project → 3D (URP recommended)
2. Is repo se `UnityProject/Assets/Scripts` copy → `Assets/Scripts`
3. `Docs` optional copy

Option B — pehle se project hai:
- Sirf Scripts folder merge karo, namespace conflicts check karo (`SUBR.*`)

## 3. Player settings (Android baad mein)
```
Company Name:  (tumhara)
Product Name:  SUBR  (ya naya naam)
Package Name:  com.yourstudio.subrgame
Minimum API:   23+
Target API:    35
Scripting Backend: IL2CPP
Target Architectures: ARM64 only (release)
```

## 4. Minimum scene graph

### Boot
- empty GO `GameBootstrap` + component `GameBootstrap`
- loads Lobby scene

### Lobby
- UI Canvas + `LobbyUI`
- button Start → load Match

### Match
```
MatchDirector          (MatchDirector.cs)
├── SafeZone           (SafeZone.cs)
├── SpawnPoints        (empty children as spawns)
├── Players/
│    └── Player        (PlayerController, Health, WeaponController, …)
├── Bots/              (BotController prefabs)
└── World/             (tumhara map)
```

## 5. Layers & tags (suggested)
Tags: `Player`, `Bot`, `Pickup`, `Zone`
Layers: `Default`, `Player`, `Hitbox`, `IgnoreRaycast`

## 6. Compile check
Console clean hona chahiye. Agar Input System package only project hai to
`PlayerSettings → Active Input Handling = Both`.

## 7. Next code drops (agent se maango)
- Photon/Mirror network layer implement
- Inventory UI full
- Recoil curves / attachment system
- Addressables loadout
- Anti-cheat hooks (basic)
