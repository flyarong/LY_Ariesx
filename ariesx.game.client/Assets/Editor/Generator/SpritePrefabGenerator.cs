using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Poukoute {

    public class SpritePrefabGenerator {
        private static List<string> ORIGIN_DIR = new List<string>{
            "\\Arts\\Sprites\\Heroes",
            "\\Arts\\Sprites\\Terrain",
            "\\Arts\\Sprites\\v4ui"
        };

        private static List<string> TARGET_DIR = new List<string>{
            "\\Resources\\Sprites\\Heroes",
            "\\Resources\\Sprites\\Terrain",
            "\\Resources\\Sprites\\v4ui"
        };


        [MenuItem("Poukoute/Generator/Generate Sprite Prefabs")]
        private static void GenerateSpritePrefabs() {
            EditorUtility.DisplayProgressBar("Make Sprite Prefabs", "Please wait...", 1);

            //string targetDir = Application.dataPath + TARGET_DIR;
            //if (Directory.Exists(targetDir))
            //    Directory.Delete(targetDir, true);
            //if (File.Exists(targetDir + ".meta"))
            //    File.Delete(targetDir + ".meta");
            //if (!Directory.Exists(targetDir))
            //    Directory.CreateDirectory(targetDir);

            //string originDir = Application.dataPath + ORIGIN_DIR;
            //DirectoryInfo originDirInfo = new DirectoryInfo(originDir);
            //MakeSpritePrefabsProcess(originDirInfo.GetFiles("*.jpg", SearchOption.AllDirectories), targetDir);
            //MakeSpritePrefabsProcess(originDirInfo.GetFiles("*.png", SearchOption.AllDirectories), targetDir);
            for (int i = 0; i < ORIGIN_DIR.Count; i++) {
                EditorUtility.ClearProgressBar();
                string targetRootDir = Application.dataPath + TARGET_DIR[i];
                string originRootDir = Application.dataPath + ORIGIN_DIR[i];
                GeneratePrefabsIn(targetRootDir, 
                    originRootDir, TARGET_DIR[i], ORIGIN_DIR[i]);
            }

        }

        static private void GeneratePrefabsIn(string targetRootDir, string originRootDir, 
            string targetPrefix, string originPrefix) {
            if (!Directory.Exists(targetRootDir)) {
                Directory.CreateDirectory(targetRootDir);
            }
            List<string> targetDirList = new List<string>(Directory.GetDirectories(targetRootDir));
            List<string> originDirList = new List<string>(Directory.GetDirectories(originRootDir));

            DirectoryInfo targetDirInfo = new DirectoryInfo(targetRootDir);
            DirectoryInfo originDirInfo = new DirectoryInfo(originRootDir);
            List<FileInfo> targetFileList = new List<FileInfo>(targetDirInfo.GetFiles("*.prefab"));
            List<FileInfo> originFileList = new List<FileInfo>(originDirInfo.GetFiles("*.png"));

            foreach (string originDir in originDirList) {
                if (originDir.Contains("Sprites\\Effects")) {
                    continue;
                }
                string targetDir =
                    originDir.Replace(originPrefix, targetPrefix);
                if (!targetDirList.Contains(targetDir)) {
                    Directory.CreateDirectory(targetDir);
                }
                GeneratePrefabsIn(targetDir, originDir, targetPrefix, originPrefix);
            }
            foreach (FileInfo originFile in originFileList) {
                string targetFile =
                    originFile.FullName.Replace(originPrefix, targetPrefix).Replace(".png", ".prefab");
                if (!File.Exists(targetFile)) {
                    MakeSpritePrefabsProcess(originFile, targetFile.Replace(".prefab", ""));
                }
            }

            foreach (string targetDir in targetDirList) {
                string originDir =
                    targetDir.Replace(targetPrefix, originPrefix);
                if (!originDirList.Contains(originDir)) {
                    Directory.Delete(targetDir, true);
                }
            }
            foreach (FileInfo targetFile in originFileList) {
                string originFile =
                    targetFile.FullName.Replace(targetPrefix, originPrefix).Replace(".prefab", ".png");
                if (!File.Exists(originFile)) {
                    File.Delete(originFile);
                }
            }
        }

        static private void MakeSpritePrefabsProcess(FileInfo file, string targetDir) {
            string allPath = file.FullName;
            string assetPath = allPath.Substring(allPath.IndexOf("Assets"));

            TextureImporter textureImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter.spriteImportMode == SpriteImportMode.Multiple) {
                Debug.LogError(AssetDatabase.LoadAllAssetsAtPath(assetPath).Length);
                Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                for(int i = 1; i < sprites.Length; i++) {
                    Sprite sprite = sprites[i] as Sprite;
                    GameObject go = new GameObject(sprite.name);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;
                    string targetPath = targetDir.Replace(file.Name.Replace(".png", ""), sprite.name);
                    string prefabPath = targetPath.Substring(targetPath.IndexOf("Assets"));
                    PrefabUtility.CreatePrefab(prefabPath.Replace("\\", "/") + ".prefab", go);
                    GameObject.DestroyImmediate(go);
                }
            } else {
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                if (sprite == null) {
                    Debug.LogError(assetPath);
                    return;
                }
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                string prefabPath = targetDir.Substring(targetDir.IndexOf("Assets"));
                PrefabUtility.CreatePrefab(prefabPath.Replace("\\", "/") + ".prefab", go);
                GameObject.DestroyImmediate(go);
            }
        }
    }
}