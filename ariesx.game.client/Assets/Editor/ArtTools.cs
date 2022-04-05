using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Protocol;

namespace Poukoute {
    public class ArtTools {
        public static string matPath = "Assets/Arts/Models/Battles/Scenes/Materials/";
        public static string exePath = Application.dataPath + "/../Tools/GitCompare/GitCompare.exe";

        public static string srcPath = Application.dataPath + "/../ariesx.gameclient.arts/";
        public static string dstPath = Application.dataPath + "/Arts/";
        public static string timePath = Application.dataPath + "/../Tools/GitCompare/time.txt";

        [MenuItem("Poukoute/Arts/SetBakenBattleScene")]
        public static void SetBakenBattleScene() {
            if (UnityEngine.SceneManagement.SceneManager.
                GetActiveScene().name.CustomEquals("Scene3DBattle")) {
                GameObject battleSceneObj = GameObject.Find("Battle").
                    transform.Find("Battlescene01").gameObject;
                battleSceneObj.transform.eulerAngles = Vector3.zero;
                foreach (Transform child in battleSceneObj.transform) {
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    string name = child.name;
                    if (!name.Contains("yun")) {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    } else {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    if (name.Contains("shu") || name.Contains("yun")) {
                        name = "Battle01_shuyun";
                    }
                    meshRenderer.sharedMaterial =
                        AssetDatabase.LoadAssetAtPath<Material>(matPath + name + ".mat");

                }
                GameObject light = GameObject.Find("Light");
                Transform runtimeLight = light.transform.Find("RuntimeLight");
                runtimeLight.gameObject.SetActive(false);
                Transform bakenLight = light.transform.Find("BakenLight");
                bakenLight.gameObject.SetActive(true);
            }


        }

        [MenuItem("Poukoute/Arts/SetRunBattleScene")]
        public static void SetRunBattleScene() {
            if (UnityEngine.SceneManagement.SceneManager.
                GetActiveScene().name.CustomEquals("Scene3DBattle")) {
                GameObject battleSceneObj = GameObject.Find("Battle").
                    transform.Find("Battlescene01").gameObject;
                battleSceneObj.transform.eulerAngles = Vector3.up * 180;
                foreach (Transform child in battleSceneObj.transform) {
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    string name = child.name;
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    if (name.Contains("shu") || name.Contains("yun")) {
                        name = "Battle01_shuyun";
                    }
                    meshRenderer.sharedMaterial =
                        AssetDatabase.LoadAssetAtPath<Material>(matPath + name + "_unlit.mat");

                }

                GameObject light = GameObject.Find("Light");
                Transform runtimeLight = light.transform.Find("RuntimeLight");
                runtimeLight.gameObject.SetActive(true);
                Transform bakenLight = light.transform.Find("BakenLight");
                bakenLight.gameObject.SetActive(false);
            }
        }

        [MenuItem("Poukoute/Arts/Refresh")]
        public static void Refresh() {
            string gitUpdate = string.Concat("/c cd ", srcPath, "\n\n git pull -s recursive -X theirs");
            string refresh = string.Concat(srcPath, " ", dstPath, " ", timePath);
            //Debug.LogError(gitUpdate);
            Process.Start("CMD.exe", gitUpdate);
            Process.Start(exePath, refresh);
        }

        //private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
        //    DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        //    if (!dir.Exists) {
        //        throw new DirectoryNotFoundException(
        //            "Source directory does not exist or could not be found: "
        //            + sourceDirName);
        //    }

        //    DirectoryInfo[] dirs = dir.GetDirectories();
        //    if (!Directory.Exists(destDirName)) {
        //        Directory.CreateDirectory(destDirName);
        //    }

        //    FileInfo[] files = dir.GetFiles();
        //    foreach (FileInfo file in files) {
        //        string destDirPath = string.Concat(destDirName, file.Name);
        //        if (File.Exists(destDirPath)) {
        //            FileInfo dstFile = new FileInfo(destDirName);
        //            if (file.LastWriteTimeUtc > dstFile.LastWriteTimeUtc) {
        //                file.CopyTo(destDirName, true);
        //            }
        //        }
        //    }

        //    if (copySubDirs) {
        //        foreach (DirectoryInfo subdir in dirs) {
        //            string temppath = string.Concat(destDirName, subdir.Name);
        //            DirectoryCopy(subdir.FullName, temppath, copySubDirs);
        //        }
        //    }
        //}
    }
}