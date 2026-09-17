#!/usr/bin/env bash
# =============================================================================
#  SUBR-Z  →  installable APK builder
#  Game : SUBR (com.pro.game.FreeSurvivalUnknownBattle)
# =============================================================================
#  Usage:
#    ./build_apk.sh                          # auto-find assets.zip
#    ./build_apk.sh /path/to/assets.zip
#    ./build_apk.sh --skip-assets            # build WITHOUT assets (will crash in-game)
#    ./build_apk.sh --download-assets        # try GitHub release download first
#
#  Output:
#    dist/SUBR-unsigned.apk
#    dist/SUBR-signed.apk     (if keytool/jarsigner/apksigner available)
# =============================================================================
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
WORK="$ROOT/build_work"
DIST="$ROOT/dist"
ASSETS_ARG="${1:-}"
SKIP_ASSETS=0
DOWNLOAD_ASSETS=0

RED=$'\033[31m'; GRN=$'\033[32m'; YLW=$'\033[33m'; CYN=$'\033[36m'; RST=$'\033[0m'
info()  { printf "%s[i]%s %s\n" "$CYN" "$RST" "$*"; }
ok()    { printf "%s[+]%s %s\n" "$GRN" "$RST" "$*"; }
warn()  { printf "%s[!]%s %s\n" "$YLW" "$RST" "$*"; }
die()   { printf "%s[x]%s %s\n" "$RED" "$RST" "$*" >&2; exit 1; }

for a in "$@"; do
  case "$a" in
    --skip-assets) SKIP_ASSETS=1 ;;
    --download-assets) DOWNLOAD_ASSETS=1 ;;
    --help|-h)
      sed -n '2,20p' "$0"; exit 0 ;;
  esac
done

need() { command -v "$1" >/dev/null 2>&1 || die "Missing tool: $1"; }
need zip
need unzip

mkdir -p "$WORK" "$DIST" "$ROOT/build_tools"
rm -rf "$WORK"/*
mkdir -p "$WORK/apk"

# -----------------------------------------------------------------------------
# 1) Locate assets.zip
# -----------------------------------------------------------------------------
ASSETS_ZIP=""
find_assets() {
  local c
  for c in \
    "$ASSETS_ARG" \
    "$ROOT/assets.zip" \
    "$ROOT/assets_analysis/assets.zip" \
    "$ROOT/../assets.zip" \
    "$HOME/assets.zip" \
    "$HOME/Downloads/assets.zip" \
    "$HOME/storage/downloads/assets.zip"
  do
    if [[ -n "$c" && -f "$c" && -s "$c" ]]; then
      # basic zip magic check
      if unzip -t "$c" >/dev/null 2>&1 || [[ "$(od -An -tx1 -N4 "$c" | tr -d ' ')" == "504b0304" ]]; then
        ASSETS_ZIP="$c"
        return 0
      fi
    fi
  done
  return 1
}

if [[ "$DOWNLOAD_ASSETS" -eq 1 ]]; then
  info "Downloading assets.zip from GitHub release..."
  DEST="$ROOT/assets_analysis/assets.zip"
  mkdir -p "$(dirname "$DEST")"
  if command -v gh >/dev/null 2>&1; then
    gh release download Zip -R lx2142853max-glitch/SUBR-Z -p assets.zip -D "$ROOT/assets_analysis" || true
  fi
  if [[ ! -s "$DEST" ]]; then
    curl -L --retry 5 --retry-delay 2 -o "$DEST" \
      "https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip" || true
  fi
fi

if [[ "$SKIP_ASSETS" -eq 0 ]]; then
  if find_assets; then
    ok "assets.zip found: $ASSETS_ZIP ($(du -h "$ASSETS_ZIP" | cut -f1))"
  else
    die "assets.zip nahi mili.

  Download karo:
    https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip

  Phir rakho yahan pe:
    $ROOT/assets.zip

  Ya path do:
    $0 /path/to/assets.zip

  (sirf test ke liye bina assets): $0 --skip-assets"
  fi
else
  warn "Building WITHOUT assets — game black-screen / crash karega"
fi

# -----------------------------------------------------------------------------
# 2) Extract folder-zips into APK root layout
# -----------------------------------------------------------------------------
info "Extracting lib / res / META-INF / kotlin / okhttp3 / google / src ..."
APK="$WORK/apk"

extract_zip() {
  local z="$1"
  if [[ -f "$ROOT/$z" ]]; then
    unzip -qo "$ROOT/$z" -d "$APK"
    ok "extracted $z"
  else
    warn "missing $z (skip)"
  fi
}

# These zips already contain their top-level folder name (lib/, res/, …)
extract_zip lib.zip
extract_zip res.zip
extract_zip META-INF.zip
extract_zip kotlin.zip
extract_zip okhttp3.zip
extract_zip google.zip
extract_zip src.zip

# Flat files that sit at APK root
info "Copying root APK files (dex, manifest, arsc, properties)..."
ROOT_FILES=(
  AndroidManifest.xml
  resources.arsc
  classes.dex classes2.dex classes3.dex classes4.dex classes5.dex
  DebugProbesKt.bin
  messaging_event.proto messaging_event_extension.proto
  app-update.properties asset-delivery.properties core-common.properties
  firebase-annotations.properties firebase-datatransport.properties
  firebase-encoders-json.properties firebase-encoders-proto.properties
  firebase-encoders.properties firebase-iid-interop.properties
  firebase-measurement-connector.properties
  play-services-ads-api.properties play-services-ads-identifier.properties
  play-services-ads.properties play-services-appset.properties
  play-services-base.properties play-services-basement.properties
  play-services-cloud-messaging.properties play-services-cronet.properties
  play-services-drive.properties play-services-games-v2.properties
  play-services-measurement-base.properties play-services-measurement-sdk-api.properties
  play-services-nearby.properties play-services-stats.properties
  play-services-tasks.properties
  transport-backend-cct.properties transport-runtime.properties
  user-messaging-platform.properties
)

for f in "${ROOT_FILES[@]}"; do
  if [[ -f "$ROOT/$f" ]]; then
    cp -a "$ROOT/$f" "$APK/"
  fi
done
ok "root files copied"

# -----------------------------------------------------------------------------
# 3) Merge assets
# -----------------------------------------------------------------------------
if [[ "$SKIP_ASSETS" -eq 0 ]]; then
  info "Merging assets.zip ..."
  # assets.zip may contain either:
  #   a) assets/... paths
  #   b) bin/Data/... (missing assets/ prefix)
  #   c) bare Data/... 
  TMPA="$WORK/assets_tmp"
  rm -rf "$TMPA"; mkdir -p "$TMPA"
  unzip -qo "$ASSETS_ZIP" -d "$TMPA"

  if [[ -d "$TMPA/assets" ]]; then
    cp -a "$TMPA/assets" "$APK/"
    ok "assets/ tree copied as-is"
  elif [[ -d "$TMPA/bin" ]]; then
    mkdir -p "$APK/assets"
    cp -a "$TMPA/bin" "$APK/assets/"
    # also copy any sibling files (UnityServices…, acf, ad-viewer, …)
    find "$TMPA" -maxdepth 1 -type f -exec cp -a {} "$APK/assets/" \;
    find "$TMPA" -maxdepth 1 -type d ! -path "$TMPA" ! -name bin -exec cp -a {} "$APK/assets/" \;
    ok "wrapped bin/ → assets/bin/"
  elif [[ -d "$TMPA/Data" ]]; then
    mkdir -p "$APK/assets/bin"
    cp -a "$TMPA/Data" "$APK/assets/bin/"
    ok "wrapped Data/ → assets/bin/Data/"
  else
    # unknown layout — dump everything under assets/
    mkdir -p "$APK/assets"
    cp -a "$TMPA"/. "$APK/assets/"
    warn "unknown assets layout — copied all under assets/ (verify manually)"
  fi

  # Sanity: critical Unity files
  crit=(
    "assets/bin/Data/data.unity3d"
    "assets/bin/Data/Managed/Metadata/global-metadata.dat"
    "assets/bin/Data/boot.config"
  )
  for c in "${crit[@]}"; do
    if [[ -f "$APK/$c" ]]; then ok "found $c"
    else warn "MISSING $c — game may not boot"
    fi
  done
fi

# -----------------------------------------------------------------------------
# 4) Drop old signature (we will re-sign)
# -----------------------------------------------------------------------------
info "Removing old signature so we can re-sign..."
rm -f "$APK"/META-INF/*.RSA "$APK"/META-INF/*.DSA "$APK"/META-INF/*.EC \
      "$APK"/META-INF/*.SF "$APK"/META-INF/MANIFEST.MF 2>/dev/null || true
# keep META-INF/services/*

# -----------------------------------------------------------------------------
# 5) Validate required pieces
# -----------------------------------------------------------------------------
info "Validating APK layout..."
[[ -f "$APK/AndroidManifest.xml" ]] || die "AndroidManifest.xml missing"
[[ -f "$APK/classes.dex" ]] || die "classes.dex missing"
[[ -f "$APK/resources.arsc" ]] || die "resources.arsc missing"
if [[ -d "$APK/lib/arm64-v8a" ]]; then
  ok "native libs: $(ls "$APK/lib/arm64-v8a" | tr '\n' ' ')"
else
  die "lib/arm64-v8a missing — extract lib.zip failed?"
fi

# -----------------------------------------------------------------------------
# 6) Pack APK
#    - store (no compress) for .so / .arsc / already-compressed unity assets
#    - deflate for the rest
# -----------------------------------------------------------------------------
info "Packing APK (this can take a minute)..."
UNSIGNED="$DIST/SUBR-unsigned.apk"
rm -f "$UNSIGNED"

# Android APK = zip with no extra fields, preferably sorted
(
  cd "$APK"
  # First pass: store compression for heavy/native/already-compressed
  find . -type f \( \
      -name '*.so' -o -name '*.arsc' -o -name '*.dex' \
      -o -name '*.unity3d' -o -name '*.resource' -o -name 'global-metadata.dat' \
      -o -name '*.png' -o -name '*.jpg' -o -name '*.mp3' -o -name '*.ogg' \
    \) -print | sed 's|^\./||' | sort | zip -q -0 -X -@ "$UNSIGNED" || true

  # Second pass: deflate everything else (zip -u updates/adds)
  find . -type f -print | sed 's|^\./||' | sort | zip -q -u -X -@ "$UNSIGNED"
)

ok "unsigned APK: $UNSIGNED ($(du -h "$UNSIGNED" | cut -f1))"

# -----------------------------------------------------------------------------
# 7) zipalign (optional)
# -----------------------------------------------------------------------------
ALIGNED="$DIST/SUBR-aligned.apk"
if command -v zipalign >/dev/null 2>&1; then
  info "zipalign -p -f 4 ..."
  zipalign -p -f 4 "$UNSIGNED" "$ALIGNED"
  ok "aligned: $ALIGNED"
else
  warn "zipalign not found — skipping (install Android build-tools for best results)"
  cp -f "$UNSIGNED" "$ALIGNED"
fi

# -----------------------------------------------------------------------------
# 8) Sign
# -----------------------------------------------------------------------------
KEYSTORE="$ROOT/build_tools/subr-debug.keystore"
KS_PASS="android"
KEY_ALIAS="subr"
KEY_PASS="android"
SIGNED="$DIST/SUBR-signed.apk"

ensure_keystore() {
  if [[ -f "$KEYSTORE" ]]; then return 0; fi
  if ! command -v keytool >/dev/null 2>&1; then
    return 1
  fi
  info "Generating debug keystore → $KEYSTORE"
  keytool -genkeypair -v \
    -keystore "$KEYSTORE" \
    -alias "$KEY_ALIAS" \
    -keyalg RSA -keysize 2048 -validity 10000 \
    -storepass "$KS_PASS" -keypass "$KEY_PASS" \
    -dname "CN=SUBR Debug, OU=Dev, O=SUBR, L=City, ST=State, C=IN" >/dev/null
  ok "keystore created (pass: $KS_PASS, alias: $KEY_ALIAS)"
}

sign_apk() {
  local in="$1" out="$2"
  if command -v apksigner >/dev/null 2>&1; then
    info "Signing with apksigner (v2/v3)..."
    apksigner sign \
      --ks "$KEYSTORE" \
      --ks-key-alias "$KEY_ALIAS" \
      --ks-pass "pass:$KS_PASS" \
      --key-pass "pass:$KEY_PASS" \
      --out "$out" \
      "$in"
    apksigner verify --verbose "$out" || warn "verify reported issues"
    return 0
  fi
  if command -v jarsigner >/dev/null 2>&1; then
    info "Signing with jarsigner (v1 only)..."
    cp -f "$in" "$out"
    jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 \
      -keystore "$KEYSTORE" -storepass "$KS_PASS" -keypass "$KEY_PASS" \
      "$out" "$KEY_ALIAS" >/dev/null
    return 0
  fi
  return 1
}

if ensure_keystore && sign_apk "$ALIGNED" "$SIGNED"; then
  ok "SIGNED APK ready: $SIGNED ($(du -h "$SIGNED" | cut -f1))"
  echo
  echo "=============================================="
  echo "  Install on phone:"
  echo "    adb install -r \"$SIGNED\""
  echo "  Or copy APK to phone and open it."
  echo "  Package : com.pro.game.FreeSurvivalUnknownBattle"
  echo "  Name    : SUBR"
  echo "=============================================="
else
  warn "Java keytool/jarsigner/apksigner nahi mile — unsigned APK bana di."
  warn "Apne PC pe sign karo (dekh o docs/HOW_TO_BUILD_APK.md)"
  cp -f "$ALIGNED" "$DIST/SUBR-NEED-SIGN.apk"
  echo
  echo "Unsigned: $UNSIGNED"
  echo "Copy of aligned (needs sign): $DIST/SUBR-NEED-SIGN.apk"
fi

# tree summary
info "APK content summary:"
(
  cd "$APK"
  echo "  top-level: $(ls -1 | tr '\n' ' ')"
  [[ -d assets ]] && echo "  assets files: $(find assets -type f | wc -l)"
  [[ -d lib ]] && echo "  libs: $(find lib -name '*.so')"
  echo "  dex: $(ls classes*.dex 2>/dev/null | tr '\n' ' ')"
)

ok "Done."
