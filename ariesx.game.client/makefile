UnityPath = /Applications/Unity5.6/Unity.app/Contents/MacOS/Unity
UnityProjectPath = /Users/poukouteci/.jenkins/jobs/aries-x-android/workspace

Build:
	@$(UnityPath) -batchMode -logFile -projectPath $(UnityProjectPath) -executeMethod BuildSettings.BuildAndroid -quit

VersionPath = $(UnityProjectPath)/Assets/Resources/Configures/version.txt
Version = `cat $(VersionPath)`
WorkspacePath = $(shell pwd)
ApkPath = /Users/poukouteci/Documents/AndroidProjects
Date = $(shell date +%Y-%m-%d-%H:%M:%S)
PackageName := ariesx_$(Date)_$(Version)
OriginName := KeenKeep
