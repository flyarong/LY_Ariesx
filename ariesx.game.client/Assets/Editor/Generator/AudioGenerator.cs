using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Poukoute {
    public class AudioGenerator : MonoBehaviour {
        private static GameObject configureManager;
        private const string audioOriginPath = "/../Document/Audio/";

        [MenuItem("Poukoute/Generator/Generate Audio")]
        public static void GenerateAudio() {
            InitAudio();
            SetAudioImport();
            DestroyImmediate(configureManager);        
        }

        private static void InitAudio() {
            try {
                configureManager = new GameObject();
                configureManager.name = "ConfigureManager";
                configureManager.transform.position = UnityEngine.Vector3.zero;
                configureManager.AddComponent<ConfigureManager>();
                ConfigureManager.LoadAudioEditorConfigures();
            } finally {
                DestroyImmediate(configureManager);
            }
        }

        private static void SetAudioImport() {
            string directory = Application.dataPath + audioOriginPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            FileInfo[] files = directoryInfo.GetFiles("*.ogg");
            foreach(FileInfo fileInfo in files) {
                AudioImportConf conf = 
                    AudioImportConf.GetConf(fileInfo.Name.Replace(fileInfo.Extension, ""));
                if (conf == null) {
                    Debug.LogErrorf("There is no conf for audio {0}", fileInfo.Name);
                    continue;
                }
                string targetPath = Application.dataPath + "/Resources/Audio/" + conf.name + ".ogg";
                FileInfo copyFileInfo = fileInfo.CopyTo(targetPath, true);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                int index = copyFileInfo.FullName.LastIndexOf("Assets");
                string assetPath = copyFileInfo.FullName.Substring(index);
                AudioImporter audio = AudioImporter.GetAtPath(assetPath) as AudioImporter;
                if (audio == null) {
                    Debug.LogError(assetPath);
                } else {
                    conf.SetAudioClip(audio);
                }
            //    break;
            }
            return;
            //foreach (BaseConf baseConf in ConfigureManager.GetConfDict<AudioImportConf>().Values) {
            //    AudioImportConf conf = baseConf as AudioImportConf;
            //    FileInfo[] files = directoryInfo.GetFiles(conf.index + ".ogg");
            //    if (files.Length == 0) {
            //        Debug.LogError("No such audio clip with name {0}", conf.index);
            //    } else {
            //        FileInfo fileInfo = files[0];
                   
            //    }

            //    //break;
            //}
        }
    }
}