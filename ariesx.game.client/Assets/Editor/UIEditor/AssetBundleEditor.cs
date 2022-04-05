using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class AssetBundleEditor {
        private static AssetBundleEditor self;
        public static AssetBundleEditor Instance {
            get {
                if (self == null) {
                    self = new AssetBundleEditor();
                }
                return self;
            }
        }

        private AssetBundleEditor() { }

        public static void BuildAssetBundlesMenuItem() {
            Instance.BuildAssetBundles();
        }

        private void BuildAssetBundles() {
            //read the assetbundle config
            string configPath = GameConst.ASSETBUNDLE_CONFIG_PATH;
            string targetPath = GameConst.ASSETBUNDLE_PATH;
            Dictionary<string, string[]> bundleDist = CSVReader.ReadCSV(configPath);
            foreach (var pair in bundleDist) {
                this.SetAssetBundle(pair.Value);
            }

            BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
           // this.CompressBundle(bundleDist.Keys);
        }

        private void SetAssetBundle(string[] line) {
            for (int i = 1; i < line.Length; i++) {
                string path = "Assets/AssetBundles/BundleRes/" + line[i];
                AssetImporter obj = AssetImporter.GetAtPath(path);
                obj.assetBundleName = line[0];
            }
        }

        private void CompressBundle(Dictionary<string, string[]>.KeyCollection fileNames) {
            string srcPath = GameConst.ASSETBUNDLE_PATH;
            //string dstPath = GameConst.ASSETBUNDLE_PATH;
            Debug.LogError(Application.persistentDataPath);
            foreach (var file in fileNames) {
                //GameHelper.CompressFileLZMA(srcPath + file, dstPath + file + ".zip");
                File.Delete(srcPath + file);
            }
        }
    }
}

