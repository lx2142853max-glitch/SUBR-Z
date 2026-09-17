# APK Build Types — Complete Context File

> Reference: har tarike se Android APK kaise banti hai, andar kya hota hai,
> kaunsi programming language / toolchain use hoti hai, aur **SUBR** kis type ki hai.

**SUBR is dump ka match:** → **Type U2 + P1 + M2**  
(Unity IL2CPP → Play AAB/PAD fused standalone APK → mod/re-sign)

---

## 0. APK kya hoti hai? (common base)

```
APK = ZIP archive (extension .apk)
├── AndroidManifest.xml     (binary AXML)
├── classes.dex [+ 2,3…]    (Dalvik/ART bytecode)
├── resources.arsc          (compiled resources index)
├── res/                    (layouts, drawables, xml)
├── lib/<abi>/*.so          (optional native C/C++)
├── assets/                 (raw files, game data, etc.)
├── META-INF/               (signature: MANIFEST.MF, *.SF, *.RSA)
└── kotlin/, okhttp3/, …    (sometimes embedded lib resources)
```

**Phone pe chalane wala runtime:** Android **ART** (pehle Dalvik)  
ART samajhta hai: **DEX bytecode** + optional **.so (JNI)**.

Har build type aakhir mein yahi structure produce karta hai — farq ye hai ki
*source language* aur *compiler chain* alag hoti hai.

---

## 1. Map — saari major build types

```
A. NATIVE ANDROID (Google official stack)
   A1  Java source          →  javac → d8/r8 → DEX
   A2  Kotlin source        →  kotlinc → d8/r8 → DEX
   A3  Java/Kotlin + NDK    →  + C/C++ → .so (JNI)

B. CROSS-PLATFORM UI FRAMEWORKS
   B1  Flutter              →  Dart → AOT/JIT + Engine .so + DEX shell
   B2  React Native         →  JS/TS + Hermes/JSC + native bridge + DEX
   B3  .NET MAUI / Xamarin  →  C# → mono/.NET Android + DEX/JNI
   B4  Cordova / Capacitor  →  HTML/CSS/JS → WebView shell APK
   B5  Ionic                →  same as B4 (+ Angular/React/Vue)
   B6  NativeScript         →  JS/TS → native UI bindings

C. GAME ENGINES
   C1  Unity Mono           →  C# → .NET DLLs in assets + libmono + DEX
   C2  Unity IL2CPP         →  C# → C++ → .so (libil2cpp) + DEX   ★ SUBR
   C3  Unreal Engine        →  C++ → libUE4.so + pak assets + DEX
   C4  Godot                →  GDScript/C#/C++ → libgodot + pck
   C5  Cocos2d-x / Cocos    →  C++/JS/Lua → .so + assets
   C6  LibGDX               →  Java/Kotlin → DEX (+ optional native)
   C7  Buildbox / other     →  mostly C++/.so templates

D. OTHER / SPECIAL
   D1  Xamarin / MAUI       →  (see B3)
   D2  Kotlin Multiplatform →  shared KMP → Android DEX target
   D3  Compose Multiplatform→  Kotlin → Android DEX
   D4  Python (BeeWare/Chaquopy/Kivy) → Python + native + DEX wrapper
   D5  Rust (cargo-apk / mozilla) → Rust → .so + thin DEX/Java
   D6  Go (gomobile)        →  Go → .so + DEX bindings

E. DISTRIBUTION FORMAT (build ka “packaging mode”)
   E1  Fat / Universal APK       — saari ABIs ek APK
   E2  ABI / density splits      — alag APK per abi/screen
   E3  Android App Bundle (AAB)  — Play pe upload, device-specific APKs
   E4  Play Asset Delivery (PAD) — game assets alag packs
   E5  Instant App / Dynamic feature modules
   E6  Standalone fused APK from Play (STAMP_TYPE_STANDALONE_APK)  ★ SUBR

F. POST-BUILD / MOD LAYERS
   F1  Official signed release
   F2  Debug / sideload signed
   F3  Repack + re-sign (apktool, MT Manager)
   F4  Modded (ads remove, cheat, wrapper)  ★ SUBR has speedremoveads
```

---

## 2. Type-by-type detail

---

### A1 / A2 — Native Java & Kotlin (Android Studio)

| | |
|--|--|
| **Kaise banti** | Android Studio / Gradle (`com.android.application`) |
| **Source language** | **Java** and/or **Kotlin** |
| **UI language** | XML layouts **or** Jetpack Compose (Kotlin) |
| **Compile chain** | `javac` / `kotlinc` → `.class` → **D8** → `.dex` → optional **R8** shrink/obfuscate → `aapt2` resources → AGP pack → sign |
| **Native code** | Optional (A3) |
| **APK pehchan** | Mostly DEX + `res/` + `resources.arsc`; little/no huge `lib/*.so` game engines; package often `com.company.app` |
| **Kab use** | Normal apps (banking, social, tools, settings…) |

```
.java / .kt  →  javac/kotlinc  →  .class  →  D8/R8  →  classes.dex
layout.xml   →  aapt2         →  res/ + resources.arsc
AndroidManifest.xml → binary AXML
                  ↓
            APK (zip) + apksigner
```

**Languages inside final APK runtime:**
- Primary logic: **DEX** (from Java/Kotlin)
- System calls: Android Framework (Java APIs)

---

### A3 — Native + NDK (C/C++)

| | |
|--|--|
| **Source** | Java/Kotlin **+ C / C++ / sometimes Assembly** |
| **Tool** | Android NDK, CMake/ndk-build |
| **Output extra** | `lib/arm64-v8a/libfoo.so`, `lib/armeabi-v7a/…` |
| **Bridge** | **JNI** (`native` methods in Java/Kotlin) |
| **Kab** | Performance, codecs, crypto, game parts, ML |

```
C/C++  →  clang (NDK)  →  .so
Java   →  DEX
DEX calls System.loadLibrary("foo") → JNI → .so
```

---

### B1 — Flutter

| | |
|--|--|
| **Source language** | **Dart** |
| **UI** | Flutter widgets (Dart), not XML |
| **Engine** | `libflutter.so` (C++) + Dart AOT snapshot |
| **Shell** | Thin Java/Kotlin Android embedding (DEX) |
| **APK pehchan** | `lib/arm64-v8a/libflutter.so`, `libapp.so`, assets `flutter_assets/` |
| **Chain** | `flutter build apk` → Gradle + Dart compiler (AOT) |

```
Dart code  →  dart2aot  →  libapp.so (or isolate snapshot)
Flutter engine (C++)   →  libflutter.so
Android embedding (Java/Kotlin) → classes.dex
```

**Languages:** Dart (app) + C++ (engine) + Java/Kotlin (Android glue)

---

### B2 — React Native

| | |
|--|--|
| **Source** | **JavaScript / TypeScript** |
| **UI** | RN components → native views |
| **JS engine** | **Hermes** (`libhermes.so`) or JavaScriptCore |
| **Bridge** | Java/Kotlin + C++ turbo modules / fabric |
| **APK pehchan** | `index.android.bundle` (or Hermes bytecode) in assets, `libhermes.so`, `libreactnativejni.so` |
| **Chain** | Metro bundler → Gradle |

**Languages:** JS/TS + Java/Kotlin + C++

---

### B3 — .NET MAUI / Xamarin

| | |
|--|--|
| **Source** | **C#** (+ XAML UI) |
| **Runtime** | Mono / .NET for Android |
| **APK pehchan** | `assemblies/` or compressed DLL blobs, `libmonosgen-2.0.so` / native .NET runtime |
| **Chain** | `dotnet build` / MSBuild → Android packaging |

**Languages:** C# + Java interop + native mono

---

### B4 / B5 — Cordova, Capacitor, Ionic

| | |
|--|--|
| **Source** | **HTML + CSS + JavaScript/TypeScript** |
| **UI** | Browser **WebView** fullscreen |
| **Native** | Thin Java plugin bridge |
| **APK pehchan** | `assets/www/index.html`, `assets/public/`, little game-engine .so |
| **Chain** | `cordova build android` / `cap sync` + Gradle |

**Languages:** JS/TS/HTML/CSS + Java plugins

---

### C1 — Unity **Mono** backend

| | |
|--|--|
| **Game source** | **C#** (Unity scripts) |
| **Backend** | Mono JIT/AOT — DLLs mostly under `assets/bin/Data/Managed/*.dll` |
| **Native** | `libmain.so`, `libunity.so`, `libmono*.so` |
| **Android shell** | Java `UnityPlayerActivity` (DEX) |
| **APK pehchan** | Managed **.dll** files present; smaller native than IL2CPP sometimes |
| **Kab** | Older Unity, faster player builds, easier modding (dnSpy on DLLs) |

```
C# scripts → .NET DLLs → assets/bin/Data/Managed/
Unity engine C/C++ → libunity.so
Java activity → DEX
```

---

### C2 — Unity **IL2CPP** backend  ★ **SUBR YAHI HAI**

| | |
|--|--|
| **Game source** | **C#** (Unity) |
| **Backend** | **IL2CPP** = Intermediate Language → C++ |
| **Native** | `libil2cpp.so` (saari game logic yahan), `libunity.so`, `libmain.so` |
| **Metadata** | `assets/bin/Data/Managed/Metadata/global-metadata.dat` |
| **Game data** | `assets/bin/Data/*.unity3d`, `sharedassets*.resource` |
| **Android shell** | Java/Kotlin DEX + `UnityPlayerActivity` |
| **APK pehchan** | Bada `libil2cpp.so` (SUBR ~47MB), **no** readable C# DLLs, metadata.dat |
| **Build** | Unity Editor 2022.3.x → Build Settings → Android → IL2CPP → Gradle |

```
C# (Unity scripts)
   ↓  IL (CIL)
IL2CPP converter
   ↓  generated C++
NDK clang
   ↓
libil2cpp.so   +   global-metadata.dat
libunity.so (engine)
assets/bin/Data/  (scenes, meshes, audio…)
classes.dex (Java: Unity player, ads SDKs, Play services)
   ↓
Gradle → AAB/APK → (optional) Play fuse
```

**Languages in SUBR-like APK:**

| Layer | Language / format |
|-------|-------------------|
| Gameplay scripts (original) | **C#** (source; final mein C++ ban chuka) |
| Engine | **C/C++** (`libunity`) |
| IL2CPP output | **C++** → machine code in `.so` |
| Android host / ads / FCM | **Java + Kotlin** → DEX |
| Build scripts | **Gradle (Groovy/Kotlin DSL)** |
| Resources | Binary XML, ARSC |
| Config sometimes | JSON (Unity Services, boot.config) |

**SUBR concrete:**
- Unity **2022.3.62f2**
- IL2CPP + **arm64-v8a only**
- Package `com.pro.game.FreeSurvivalUnknownBattle`
- Label **SUBR**
- Mediation: AdMob, AppLovin, IronSource, Unity Ads (Java SDKs in DEX)
- Play Games, Firebase, OneSignal

---

### C3 — Unreal Engine

| | |
|--|--|
| **Source** | **C++** + Blueprints (visual; compile to C++/bytecode) |
| **Native** | Huge `libUE4.so` / `libUnreal.so` |
| **Assets** | `.pak` files in assets or OBB |
| **Shell** | Java GameActivity DEX |
| **Languages** | C++ primary + Java glue |

---

### C4 — Godot

| | |
|--|--|
| **Source** | **GDScript** and/or C# and/or C++ |
| **Native** | `libgodot_android.so` |
| **Data** | `assets/main.pck` or similar |
| **Languages** | GDScript / C# / C++ + Java shell |

---

### C5 — Cocos2d-x

| | |
|--|--|
| **Source** | **C++**, sometimes **JS** or **Lua** scripting |
| **Native** | `libcocos2djs.so` / `libcocos2dcpp.so` |
| **Languages** | C++ / JS / Lua + Java |

---

### C6 — LibGDX

| | |
|--|--|
| **Source** | **Java** (or Kotlin) |
| **Mostly DEX** | Game logic in Dalvik bytecode |
| **Optional** | native box2d etc. `.so` |
| **Languages** | Java/Kotlin |

---

## 3. Distribution / packaging types (E) — kaise “deliver” hoti hai

Ye alag “engine” nahi — **same app** ko Play/device tak bhejne ka mode hai.

### E1 — Single “fat” APK
- Ek file, saari ABIs (`arm64`, `armeabi-v7a`, `x86_64`…) andar  
- Seedha sideload / website download  
- Size badi

### E2 — Split APKs
- `split_config.arm64_v8a.apk`, `split_config.xxhdpi.apk`…  
- `bundletool` ya Gradle splits  
- Install time multiple APKs merge

### E3 — Android App Bundle (`.aab`)
- Developer **AAB** Play Console pe upload karta hai  
- Play har device ke liye optimized APK set banata hai  
- Local test: `bundletool build-apks`

```
Source build → app.aab  →  Play  →  device-specific APK(s)
```

### E4 — Play Asset Delivery (PAD)
- Game assets alag **asset packs** (install-time / fast-follow / on-demand)  
- Unity option: **Split Application Binary** / Addressables + PAD  
- Manifest modules jaise `UnityDataAssetPack`

### E5 — Dynamic feature modules
- On-demand features (language packs, extra modules)

### E6 — Play “Standalone fused APK”  ★ **SUBR stamp**

Play kabhi-kabhi modules ko **ek single APK** mein fuse karke deta hai:

```
meta-data:
  com.android.stamp.type   = STAMP_TYPE_STANDALONE_APK
  com.android.stamp.source = https://play.google.com/store
  com.android.dynamic.apk.fused.modules = UnityDataAssetPack,base
  com.android.vending.derived.apk.id = 3
```

**Matlab:** originally **AAB + asset pack** thi; jo file tum dekh rahe ho wo Play-derived **merged standalone APK** jaisi hai (baad mein mod/re-sign ho sakti hai).

---

## 4. Signature / post-build types (F)

| Type | Kaise | Pehchan |
|------|--------|---------|
| **F1 Official release** | Play App Signing / developer upload key | Real org cert, long history, Play integrity |
| **F2 Debug** | Android debug.keystore | `CN=Android Debug` |
| **F3 Repack re-sign** | apktool decode → edit → build → apksigner | Naya cert, same package name |
| **F4 Modded wrapper** | smali/DEX inject, new Application class | Jaise SUBR: `com.hm.speedremoveads.RMApplication` |

**SUBR signing evidence:**
```
Signer: SIGN23_K.RSA
Cert:  C=usa, ST=str, L=msk, O=O, OU=u, CN=usa
       (self-signed style, Aug 2025 → 2052)
```
→ **F3/F4**, pure Play store cert nahi.

---

## 5. Language cheat-sheet (final APK ke andar kya “chalta” hai)

| Build type | Human source languages | Final executable form |
|------------|------------------------|------------------------|
| Java app | Java | DEX |
| Kotlin app | Kotlin | DEX |
| Java/Kotlin + NDK | Java/Kotlin + C/C++ | DEX + .so |
| Flutter | Dart (+ Java glue) | libapp/libflutter .so + DEX |
| React Native | JS/TS (+ Java/C++) | JS bundle/Hermes + .so + DEX |
| Cordova/Ionic | HTML/CSS/JS | WebView assets + DEX |
| MAUI/Xamarin | C# | Mono/.NET + DEX |
| Unity Mono | C# | DLLs + libmono + DEX |
| **Unity IL2CPP** | **C# → C++** | **libil2cpp.so + metadata + DEX** |
| Unreal | C++ / Blueprint | libUE4.so + pak + DEX |
| Godot | GDScript/C#/C++ | libgodot + pck + DEX |
| Cocos | C++/JS/Lua | .so + assets + DEX |
| LibGDX | Java/Kotlin | DEX (+ optional .so) |
| KMP | Kotlin | DEX (Android target) |
| Rust/Go mobile | Rust or Go | .so + thin DEX |

**Zaroori baat:**  
Phone **C# ya Dart source seedha nahi chalata**. Hamesha convert hota hai:
- managed bytecode (DEX, or mono DLL), **ya**
- native machine code (`.so`), **ya**
- JS engine bytecode / WebView.

---

## 6. Identify karne ka tarika (unknown APK aaye to)

```
1. lib/ folder dekho
   - libil2cpp.so + libunity.so     → Unity IL2CPP
   - libmono + libunity             → Unity Mono
   - libflutter.so + libapp.so      → Flutter
   - libhermes.so + libreact*.so    → React Native
   - libUE4.so / libUnreal          → Unreal
   - libgodot*                      → Godot
   - almost empty lib/              → pure Java/Kotlin (or WebView)

2. assets/ dekho
   - bin/Data/*.unity3d             → Unity
   - flutter_assets/                → Flutter
   - index.android.bundle           → React Native
   - www/index.html                 → Cordova/WebView
   - main.pck                       → Godot
   - *.pak                          → Unreal

3. classes.dex strings
   - UnityPlayerActivity            → Unity shell
   - io.flutter                     → Flutter
   - com.facebook.react             → RN
   - only androidx + app package    → native app

4. META-INF + manifest meta-data
   - STAMP_TYPE_STANDALONE_APK      → Play-derived
   - fused.modules                  → asset packs merged
   - CN=Android Debug               → debug build
```

---

## 7. SUBR — exact type classification

| Axis | SUBR value |
|------|------------|
| **Engine type** | **C2 — Unity IL2CPP** |
| **Game language (source)** | **C#** |
| **Game language (shipped)** | **Native C++ in `libil2cpp.so`** |
| **Engine language** | C/C++ (`libunity.so`) |
| **Android shell language** | Java + Kotlin → **DEX** (5 dex files) |
| **Ads/SDKs language** | Java/Kotlin (AppLovin, AdMob, IronSource, Firebase, OneSignal…) |
| **Build system** | Unity → **Gradle (AGP)** + D8/R8 |
| **Unity version** | 2022.3.62f2 |
| **ABI** | arm64-v8a only |
| **Distribution type** | **E3/E4 → E6** (AAB + `UnityDataAssetPack` → Play standalone fused APK stamps) |
| **Post-build** | **F3+F4** re-sign + `speedremoveads` Application wrapper |
| **Package** | `com.pro.game.FreeSurvivalUnknownBattle` |
| **App label** | SUBR |
| **Version** | 5.3.43 (versionCode 82) |

```
SUBR stack (top → bottom):

  [You see]  SUBR icon / UnityPlayerActivity
       ↓
  [DEX]      Java/Kotlin: Unity player, ads, Play Games, FCM, OneSignal,
             + mod: com.hm.speedremoveads.RMApplication
       ↓ JNI
  [Native]   libmain.so → libunity.so → libil2cpp.so
       ↓
  [Assets]   bin/Data/data.unity3d, sharedassets*, global-metadata.dat
       ↓
  [Origin]   Unity C# project → IL2CPP → Gradle → Play AAB/PAD → fused APK
             → unpack/mod/re-sign → zip pieces on GitHub
```

---

## 8. Build command examples (har family)

```bash
# A1/A2 Native
./gradlew assembleRelease

# B1 Flutter
flutter build apk --release

# B2 React Native
cd android && ./gradlew assembleRelease

# B4 Cordova
cordova build android --release

# C2 Unity IL2CPP (Editor / CLI)
# Build Settings: Android, IL2CPP, ARM64, Create App Bundle
Unity -quit -batchmode -projectPath . -executeMethod BuildScript.BuildAndroid

# E3 bundletool (AAB → APKs)
bundletool build-apks --bundle=app.aab --output=app.apks --mode=universal
```

---

## 9. Size & performance rough compare

| Type | Typical size | CPU speed | Reverse ease |
|------|--------------|----------|--------------|
| Pure Kotlin app | 5–40 MB | High | smali/jadx easy |
| Flutter | 20–80 MB | High | Dart AOT harder |
| RN | 20–60 MB | Medium | JS bundle often readable |
| Cordova | 5–30 MB | Lower (WebView) | HTML/JS easy |
| Unity Mono | 80–300 MB | Medium | DLL dump easy |
| **Unity IL2CPP** | **150–500 MB+** | **High** | **hard (Il2CppDumper)** |
| Unreal | 200 MB–2 GB | High | very hard |

SUBR pieces: ~94 MB without assets + ~268 MB assets ≈ **~350 MB class** game.

---

## 10. Quick “kaunsi language seekhun” guide

| Banani hai | Seekho |
|------------|--------|
| Normal Android app | **Kotlin** (+ XML or Compose) |
| Google-heavy native | Kotlin + thoda **Java** legacy |
| Fast UI cross-platform | **Dart** (Flutter) |
| Web se app | **JS/TS** (RN or Capacitor) |
| 3D/multiplayer game jaisa SUBR | **C#** + **Unity**; samajhne ke liye thoda **C++**/IL2CPP concepts |
| AAA console-class | **C++** + Unreal |
| Max performance library | **C/C++** NDK or **Rust** |

---

## 11. Glossary (short)

| Term | Meaning |
|------|---------|
| **DEX** | Dalvik Executable — Android app bytecode |
| **ART** | Android Runtime — DEX chalata hai |
| **JNI** | Java Native Interface — DEX ↔ .so bridge |
| **IL2CPP** | Unity: C# IL ko C++ mein translate |
| **AAB** | Android App Bundle — Play upload format |
| **PAD** | Play Asset Delivery — alag asset packs |
| **R8/D8** | Google dexer + shrinker |
| **AGP** | Android Gradle Plugin |
| **ABI** | CPU type: arm64-v8a, armeabi-v7a, x86_64 |
| **smali** | DEX ka readable assembly form |
| **global-metadata.dat** | IL2CPP type/method names table |

---

## 12. Related files in this repo

| File | Role |
|------|------|
| `docs/HOW_TO_BUILD_APK.md` | Is SUBR dump se APK **dobara pack** kaise karein |
| `build_apk.sh` | Automatic pack script |
| `README.md` | Short project summary |
| Ye file | **Saari APK build types + languages context** |

---

*Last aligned with SUBR-Z analysis: Unity 2022.3.62f2 IL2CPP, Play fused standalone stamps, speedremoveads re-sign layer.*
