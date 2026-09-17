# SUBR APK kaise banaye (Complete Guide)

Game: **SUBR**  
Package: `com.pro.game.FreeSurvivalUnknownBattle`  
Version: `5.3.43` (versionCode 82)

Repo mein APK ke folders **zip** karke rakhe gaye hain, aur **assets alag release** pe hain.  
Neeche 3 tarike hain — sabse aasan pehle.

---

## Diagram

```
┌─────────────────────┐     ┌──────────────────────────┐
│  GitHub repo        │     │  GitHub Release (tag Zip)│
│  lib.zip, res.zip   │     │  assets.zip  (~268 MB)   │
│  classes*.dex       │     │  Unity data + metadata   │
│  AndroidManifest…   │     └───────────┬──────────────┘
└──────────┬──────────┘                 │
           │         merge              │
           ▼                            ▼
        build_work/apk/   <──── assets/ bin/Data/ …
           │
           ▼  zip
        SUBR-unsigned.apk
           │
           ▼  zipalign + sign
        SUBR-signed.apk   →  phone pe install
```

---

## Tarika 1 — Automatic script (recommended)

### PC requirements
- Linux / macOS / Windows(WSL / Termux)
- `zip`, `unzip`, `curl`
- **Java JDK 11+** (sign ke liye) → `keytool` + `jarsigner`
- Optional best: Android SDK build-tools → `zipalign` + `apksigner`

### Steps

```bash
# 1) Repo clone / open
cd SUBR-Z

# 2) assets.zip download (Release se)
# Browser: https://github.com/lx2142853max-glitch/SUBR-Z/releases/tag/Zip
# Ya:
curl -L -o assets.zip \
  "https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip"

# expected size ~280 MB, sha256:
# f161ef98f5469d5672010b2ef9223e96f81c532959b1110dd5d92cefc0100287

# 3) Build
chmod +x build_apk.sh
./build_apk.sh ./assets.zip

# 4) Output
ls -lh dist/
#   SUBR-unsigned.apk
#   SUBR-signed.apk      ← ye install karo
```

### Phone pe install

```bash
adb install -r dist/SUBR-signed.apk
```

Ya APK file phone pe copy karke open karo  
(Settings → Unknown sources / Install unknown apps allow).

---

## Tarika 2 — Manual (har step samajhne ke liye)

### Step A — Folders nikaalo

```bash
mkdir -p apk && cd apk

# ye zips apna folder khud banati hain (lib/, res/, …)
unzip -o ../lib.zip
unzip -o ../res.zip
unzip -o ../META-INF.zip
unzip -o ../kotlin.zip
unzip -o ../okhttp3.zip
unzip -o ../google.zip
unzip -o ../src.zip
```

### Step B — Root files copy

```bash
cp ../AndroidManifest.xml .
cp ../resources.arsc .
cp ../classes*.dex .
cp ../DebugProbesKt.bin .
cp ../*.properties .
cp ../*.proto . 2>/dev/null || true
```

### Step C — Assets merge (SABSE ZAROORI)

```bash
# assets.zip Release se download karke:
unzip -o ../assets.zip -d assets_tmp

# Case 1: zip ke andar pehle se "assets/" folder hai
cp -a assets_tmp/assets .

# Case 2: zip ke andar seedha "bin/" hai
# mkdir -p assets && cp -a assets_tmp/bin assets/

# Check:
ls assets/bin/Data/data.unity3d
ls assets/bin/Data/Managed/Metadata/global-metadata.dat
```

**Galat path = black screen.**  
Final path hona chahiye:

```
apk/assets/bin/Data/data.unity3d
apk/assets/bin/Data/boot.config
apk/lib/arm64-v8a/libil2cpp.so
apk/lib/arm64-v8a/libunity.so
apk/classes.dex
apk/AndroidManifest.xml
```

### Step D — Purana signature hatao

```bash
rm -f META-INF/*.RSA META-INF/*.DSA META-INF/*.SF META-INF/MANIFEST.MF
# META-INF/services/ mat hatana
```

### Step E — ZIP se APK banao

```bash
# Native / pehle se compressed → store (0)
zip -0 -X -r ../SUBR-unsigned.apk \
  lib resources.arsc classes*.dex \
  assets/bin/Data/*.unity3d \
  assets/bin/Data/*.resource \
  assets/bin/Data/Managed 2>/dev/null || true

# Baaki files → normal deflate
zip -u -X -r ../SUBR-unsigned.apk .
```

### Step F — zipalign (recommended)

```bash
zipalign -p -f 4 SUBR-unsigned.apk SUBR-aligned.apk
```

### Step G — Sign

#### Debug key banao (ek baar)

```bash
keytool -genkeypair -v \
  -keystore subr-debug.keystore \
  -alias subr \
  -keyalg RSA -keysize 2048 -validity 10000 \
  -storepass android -keypass android \
  -dname "CN=SUBR Debug, O=SUBR, C=IN"
```

#### Best: apksigner (v1+v2+v3)

```bash
apksigner sign \
  --ks subr-debug.keystore \
  --ks-key-alias subr \
  --ks-pass pass:android \
  --key-pass pass:android \
  --out SUBR-signed.apk \
  SUBR-aligned.apk

apksigner verify -v SUBR-signed.apk
```

#### Fallback: jarsigner (sirf v1 — purane Android)

```bash
cp SUBR-aligned.apk SUBR-signed.apk
jarsigner -sigalg SHA256withRSA -digestalg SHA-256 \
  -keystore subr-debug.keystore -storepass android \
  SUBR-signed.apk subr
```

---

## Tarika 3 — Android phone pe (Termux)

```bash
pkg update
pkg install unzip zip openjdk-17 wget
# optional: android-tools (adb)

cd ~/storage/shared/SUBR-Z   # jahan repo + assets.zip ho
chmod +x build_apk.sh
./build_apk.sh ./assets.zip

# Install:
# File manager se dist/SUBR-signed.apk open karo
```

---

## Tarika 4 — MT Manager / APK Tool (GUI, no PC)

1. Phone pe **MT Manager** install karo  
2. Repo files extract karo ek folder mein (`lib/`, `res/`, dex, manifest…)  
3. `assets.zip` extract karke `assets/` usi folder mein rakho  
4. MT Manager → folder long-press → **Archive** → format **APK**  
5. Bani APK pe → **APK Signer / V2 sign**  
6. Install

Ye beginners ke liye sabse visual tarika hai.

---

## Final folder layout (check list)

```
apk/
├── AndroidManifest.xml          ✅
├── classes.dex … classes5.dex   ✅
├── resources.arsc               ✅
├── lib/
│   └── arm64-v8a/
│       ├── libil2cpp.so         ✅ ~47MB
│       ├── libunity.so          ✅ ~21MB
│       ├── libmain.so           ✅
│       └── libapplovin-native-crash-reporter.so
├── res/                         ✅
├── assets/
│   ├── bin/Data/
│   │   ├── data.unity3d         ✅ CRITICAL
│   │   ├── datapack.unity3d
│   │   ├── sharedassets1..5.resource
│   │   ├── boot.config
│   │   ├── global-metadata.dat  ✅ (path: Managed/Metadata/)
│   │   └── …
│   ├── UnityServicesProjectConfiguration.json
│   ├── acf
│   └── ad-viewer/
├── META-INF/services/           (optional keep)
├── kotlin/  okhttp3/  google/  src/
└── *.properties
```

---

## Common errors

| Problem | Cause | Fix |
|---------|--------|-----|
| App install nahi hoti | unsigned / corrupt zip | `apksigner` se sign karo |
| Install hoti hai, turant close | assets path galat / missing | `assets/bin/Data/data.unity3d` check |
| "Parse error" | zip extra fields / bad compress | `zip -X` use karo, zipalign karo |
| Only 64-bit devices | APK mein sirf arm64-v8a | normal — 32-bit phone pe nahi chalega |
| "App not installed" (downgrade) | purani alag-sign wali app | pehle uninstall karo |
| Black screen + logcat `global-metadata` | assets merge adhoora | assets.zip dubara extract |
| Play Protect warning | debug keystore | expected for sideload |

---

## assets.zip verify

```bash
# size
ls -lh assets.zip
# ~280465659 bytes

# hash
sha256sum assets.zip
# f161ef98f5469d5672010b2ef9223e96f81c532959b1110dd5d92cefc0100287

# list
unzip -l assets.zip | head
```

---

## Quick one-liner (jab sab tools + assets ready hon)

```bash
./build_apk.sh ./assets.zip && adb install -r dist/SUBR-signed.apk
```

---

## Is sandbox / Arena pe limit

Yahan abhi:
- `assets.zip` CDN se download **TLS fail** (tumhe locally dena hoga)
- `java` / `apksigner` packages network ke bina install nahi ho rahe

Isliye yahan unsigned pack tak ban sakta hai; **sign + install tumhare phone/PC pe** best hai.

Script ready hai: `build_apk.sh`  
Guide: ye file (`docs/HOW_TO_BUILD_APK.md`)
