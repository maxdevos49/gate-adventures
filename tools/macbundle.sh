#!/bin/bash
set -euo pipefail

# Documentation for how this script builds a .app bundle can be found at the following link:
# https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFBundles/Introduction/Introduction.html#//apple_ref/doc/uid/10000123i-CH1-SW1

TITLE="Gate Adventures"
IDENTIFIER="com.gate-adventures.www"
VERSION="0.0.1"
PROJECT_PATH="../GateAdventures"
EXECUTABLE="GateAdventures"
ICON_PATH="../GateAdventures/Icon.ico"

# Always start with clean slate
rm -rf "$TITLE.app"

# Place the executable inside here
mkdir -p "$TITLE.app/Contents/MacOS"
dotnet publish $PROJECT_PATH --os osx --arch arm64 -o "$TITLE.app/Contents/MacOS"

# Place applications resources here. Ex: App icon, textures, audio,...
mkdir -p "$TITLE.app/Contents/Resources"
cp $ICON_PATH "$TITLE.app/Contents/Resources/AppIcon.ico"

cat >"$TITLE.app/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>CFBundleName</key>
	<string>$TITLE</string>
	<key>CFBundleDisplayName</key>
	<string>$TITLE</string>
	<key>CFBundleIdentifier</key>
	<string>$IDENTIFIER</string>
	<key>CFBundleIconFile</key>
	<string>AppIcon.ico</string>
	<key>CFBundleVersion</key>
	<string>$VERSION</string>
	<key>CFBundlePackageType</key>
	<string>APPL</string>
	<key>CFBundleExecutable</key>
	<string>$EXECUTABLE</string>
</dict>
</plist>
EOF
