# SUBR-Z

**SUBR** game APK — unpacked pieces + rebuild tools.

| | |
|--|--|
| **App name** | SUBR |
| **Package** | `com.pro.game.FreeSurvivalUnknownBattle` |
| **Version** | 5.3.43 (code 82) |
| **Engine** | Unity 2022.3.62f2 (IL2CPP, arm64-v8a) |

## Repo contents

| Item | Meaning |
|------|---------|
| `AndroidManifest.xml`, `classes*.dex`, `resources.arsc` | APK root |
| `lib.zip` | `lib/arm64-v8a/*.so` (il2cpp + unity) |
| `res.zip` | Android resources |
| `META-INF.zip`, `kotlin.zip`, `okhttp3.zip`, … | other APK folders |
| **`assets.zip`** | **NOT in git** — GitHub Release tag `Zip` (~268 MB) |

Release assets:  
https://github.com/lx2142853max-glitch/SUBR-Z/releases/tag/Zip

## APK kaise banaye (shortest)

```bash
# 1. assets download
curl -L -o assets.zip \
  "https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip"

# 2. build + sign
chmod +x build_apk.sh
./build_apk.sh ./assets.zip

# 3. install
adb install -r dist/SUBR-signed.apk
```

**Full guide (3 tarike: script / manual / MT Manager):**  
→ [`docs/HOW_TO_BUILD_APK.md`](docs/HOW_TO_BUILD_APK.md)

**Saari APK build types + languages (Unity/Flutter/RN/native…):**  
→ [`docs/APK_BUILD_TYPES_CONTEXT.md`](docs/APK_BUILD_TYPES_CONTEXT.md)

| Script | Platform |
|--------|----------|
| `build_apk.sh` | Linux / macOS / Termux / WSL |
| `build_apk.bat` | Windows (basic; WSL better) |

## Output

```
dist/SUBR-unsigned.apk
dist/SUBR-signed.apk      ← install this
```

## Requirements to sign

- Java JDK 11+ (`keytool`, `jarsigner`)
- Optional: Android build-tools (`zipalign`, `apksigner`) for v2/v3 signatures

Without Java, script still packs an **unsigned** APK — sign it on another machine.
