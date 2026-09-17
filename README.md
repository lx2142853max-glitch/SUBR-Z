# SUBR-Z

Do hisse:

1. **Original SUBR APK dump** (analyze + rebuild tools)  
2. **Naya Unity C# game scaffold** — code agent likhta hai, Unity Editor tum

| Original APK | |
|--|--|
| **App name** | SUBR |
| **Package** | `com.pro.game.FreeSurvivalUnknownBattle` |
| **Version** | 5.3.43 (code 82) |
| **Engine** | Unity 2022.3.62f2 (IL2CPP, arm64-v8a) |

---

## NEW: Unity game code (C#)

```
UnityProject/
  Assets/Scripts/   ← gameplay systems (C#)
  Docs/             ← setup + GDD + script index
```

**Kaam ka batwara**

| Agent (yahan) | Tum (Unity) |
|---------------|-------------|
| C# scripts, architecture, GitHub | Scenes, prefabs, models, anim, UI layout |
| Systems: move, gun, zone, bots, match | Art, NavMesh, lighting, Android build |
| Network interface + offline stub | Photon/Mirror wire (baad mein saath) |

- Start: [`UnityProject/README.md`](UnityProject/README.md)  
- Setup: [`UnityProject/Docs/UNITY_SETUP.md`](UnityProject/Docs/UNITY_SETUP.md)  
- Script list: [`UnityProject/Docs/SCRIPT_INDEX.md`](UnityProject/Docs/SCRIPT_INDEX.md)

Haan — **gameplay files code hain (C#)**. Scenes/art/binary tum handle karo.

---

## Original APK dump + pack tools

| Item | Meaning |
|------|---------|
| `AndroidManifest.xml`, `classes*.dex`, `resources.arsc` | APK root |
| `lib.zip` | `lib/arm64-v8a/*.so` (il2cpp + unity) |
| `res.zip` | Android resources |
| `META-INF.zip`, `kotlin.zip`, `okhttp3.zip`, … | other APK folders |
| **`assets.zip`** | GitHub Release tag `Zip` (~268 MB) |

Release: https://github.com/lx2142853max-glitch/SUBR-Z/releases/tag/Zip

```bash
curl -L -o assets.zip \
  "https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip"
chmod +x build_apk.sh && ./build_apk.sh ./assets.zip
adb install -r dist/SUBR-signed.apk
```

- Rebuild guide: [`docs/HOW_TO_BUILD_APK.md`](docs/HOW_TO_BUILD_APK.md)  
- All APK build types + languages: [`docs/APK_BUILD_TYPES_CONTEXT.md`](docs/APK_BUILD_TYPES_CONTEXT.md)

| Script | Platform |
|--------|----------|
| `build_apk.sh` | Linux / macOS / Termux / WSL |
| `build_apk.bat` | Windows (basic; WSL better) |
