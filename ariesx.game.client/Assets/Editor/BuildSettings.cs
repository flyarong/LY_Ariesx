using UnityEngine;
using UnityEngine.Windows;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Poukoute;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildSettings : MonoBehaviour {
#if UNITY_EDITOR
    //private static BuildOptions currentVersionType = BuildOptions.Development;
    private static BuildOptions currentVersionType = BuildOptions.None;

    private static void GeneralSettings() {
        string productName = string.Empty;
        switch (VersionConst.language) {
            case "cn":
                productName = "迷雾大陆";
                break;
            default:
                productName = "KeenKeep";
                break;
        }

        PlayerSettings.productName = productName;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, GameConst.ANDROID_APPID);
        PlayerSettings.companyName = "Poukoute";
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        PlayerSettings.enableCrashReportAPI = true;
        QualitySettings.SetQualityLevel(0);
    }

    private static void IOSSettings() {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.Mono2x);
        PlayerSettings.enableInternalProfiler = true;
        PlayerSettings.iOS.appleDeveloperTeamID = "MFEZUPZYP8";
    }

    private static void AndroidSettings() {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.Android.androidTVCompatibility = false;
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
#if DEVELOPER
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Debug;
        EditorUserBuildSettings.connectProfiler = true;
#else
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
#endif
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        //PlayerSettings.graphicsJobs = false;
        //PlayerSettings.MTRendering = false;

        PlayerSettings.Android.keystoreName = Application.dataPath + "/../Document/poukoute.googleplay.keystore";
        PlayerSettings.Android.keystorePass = "Poukoute520";
        PlayerSettings.Android.keyaliasName = "poukoute.ariesx.google";
        PlayerSettings.Android.keyaliasPass = "Poukoute520";

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel18;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        PlayerSettings.enableInternalProfiler = false;
    }

    [MenuItem("Poukoute/Build/Windows")]
    public static void BuildWindows64() {
        GeneralSettings();
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            scenes.Add(scene.path);
        }
        //BuildPipeline.
        BuildPipeline.BuildPlayer(scenes.ToArray(), "C:/Users/wangy/Desktop/ariesx.exe",
            BuildTarget.StandaloneWindows64, currentVersionType);
    }

    [MenuItem("Poukoute/Build/Xcode")]
    public static void BuildXcode() {
        GeneralSettings();
        string proPath = "/Users/poukouteci/Documents/XcodeProjects/KeenKeep";
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            scenes.Add(scene.path);
        }
        BuildPipeline.BuildPlayer(scenes.ToArray(), proPath, BuildTarget.iOS, currentVersionType);
    }

    [PostProcessBuildAttribute(1)]
    public static void AddFrameWork(BuildTarget buildTaget, string path) {
        if (buildTaget == BuildTarget.iOS) {
            string pbxPath = PBXProject.GetPBXProjectPath(path);
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxPath));
            string target = pbxProject.TargetGuidByName("Unity-iPhone");
            pbxProject.AddFrameworkToProject(target, "Security.framework", false);
            pbxProject.AddFrameworkToProject(target, "AdSupport.framework", false);
            pbxProject.AddFrameworkToProject(target, "CoreTelephony.framework", false);
            pbxProject.AddFrameworkToProject(target, "libz.tbd", false);

            // Set a custom link flag
            pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            File.WriteAllText(pbxPath, pbxProject.WriteToString());
        }
    }

    [MenuItem("Poukoute/Build/Android")]
    public static void BuildAndroid() {
        VersionController.GenerateVersionConst();
        AssetDatabase.Refresh();
        GeneralSettings();
        AndroidSettings();
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            scenes.Add(scene.path);
        }
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes.ToArray();
        //buildPlayerOptions.locationPathName = "G:/AndroidProjects/KeenKeep";
        buildPlayerOptions.locationPathName = "/Users/poukouteci/Documents/AndroidProjects/KeenKeep";
        buildPlayerOptions.target = BuildTarget.Android;

        buildPlayerOptions.options = currentVersionType |
                                     BuildOptions.AcceptExternalModificationsToPlayer;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
#endif
}