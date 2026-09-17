@echo off
REM =============================================================================
REM  SUBR-Z Windows APK builder (needs: zip/unzip OR 7-Zip, Java JDK)
REM  Better option on Windows: use WSL and run build_apk.sh
REM =============================================================================
setlocal EnableExtensions
cd /d "%~dp0"

set ROOT=%cd%
set WORK=%ROOT%\build_work
set DIST=%ROOT%\dist
set APK=%WORK%\apk
set ASSETS=%~1
if "%ASSETS%"=="" set ASSETS=%ROOT%\assets.zip

echo [i] SUBR APK builder (Windows)
echo [i] ROOT=%ROOT%

where tar >nul 2>&1
where powershell >nul 2>&1

if not exist "%ASSETS%" (
  echo [x] assets.zip nahi mili: %ASSETS%
  echo     Download: https://github.com/lx2142853max-glitch/SUBR-Z/releases/download/Zip/assets.zip
  echo     Usage: build_apk.bat C:\path\assets.zip
  exit /b 1
)

if exist "%WORK%" rmdir /s /q "%WORK%"
mkdir "%APK%" 2>nul
mkdir "%DIST%" 2>nul

echo [i] Extracting zips with tar...
tar -xf lib.zip     -C "%APK%"
tar -xf res.zip     -C "%APK%"
tar -xf META-INF.zip -C "%APK%"
tar -xf kotlin.zip  -C "%APK%"
tar -xf okhttp3.zip -C "%APK%"
tar -xf google.zip  -C "%APK%"
tar -xf src.zip     -C "%APK%"

echo [i] Copying root files...
copy /y AndroidManifest.xml "%APK%\" >nul
copy /y resources.arsc      "%APK%\" >nul
copy /y classes*.dex        "%APK%\" >nul
copy /y DebugProbesKt.bin   "%APK%\" >nul
copy /y *.properties        "%APK%\" >nul
copy /y *.proto             "%APK%\" >nul 2>nul

echo [i] Extracting assets...
mkdir "%WORK%\assets_tmp" 2>nul
tar -xf "%ASSETS%" -C "%WORK%\assets_tmp"
if exist "%WORK%\assets_tmp\assets" (
  xcopy /e /i /y "%WORK%\assets_tmp\assets" "%APK%\assets\" >nul
) else if exist "%WORK%\assets_tmp\bin" (
  mkdir "%APK%\assets" 2>nul
  xcopy /e /i /y "%WORK%\assets_tmp\*" "%APK%\assets\" >nul
) else (
  mkdir "%APK%\assets" 2>nul
  xcopy /e /i /y "%WORK%\assets_tmp\*" "%APK%\assets\" >nul
)

echo [i] Dropping old signature...
del /q "%APK%\META-INF\*.RSA" 2>nul
del /q "%APK%\META-INF\*.SF"  2>nul
del /q "%APK%\META-INF\MANIFEST.MF" 2>nul

echo [i] Packing APK with tar/powershell zip...
set UNSIGNED=%DIST%\SUBR-unsigned.apk
if exist "%UNSIGNED%" del /q "%UNSIGNED%"

powershell -NoProfile -Command ^
  "Compress-Archive -Path '%APK%\*' -DestinationPath '%DIST%\SUBR-temp.zip' -Force; " ^
  "Move-Item -Force '%DIST%\SUBR-temp.zip' '%UNSIGNED%'"

echo [+] Unsigned APK: %UNSIGNED%
echo.
echo Next - SIGN with Java:
echo   keytool -genkeypair -v -keystore build_tools\subr-debug.keystore -alias subr -keyalg RSA -keysize 2048 -validity 10000 -storepass android -keypass android -dname "CN=SUBR,O=SUBR,C=IN"
echo   jarsigner -sigalg SHA256withRSA -digestalg SHA-256 -keystore build_tools\subr-debug.keystore -storepass android %UNSIGNED% subr
echo.
echo Ya WSL mein better:  wsl ./build_apk.sh ./assets.zip
echo Guide: docs\HOW_TO_BUILD_APK.md
endlocal
