using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Protocol;
using UnityEditor;

namespace Poukoute {
    public class ArtConstGenerator {
        private static ArtConstGenerator self;
        public static ArtConstGenerator Instance {
            get {
                if (self == null) {
                    self = new ArtConstGenerator();
                }
                return self;
            }
        }

        private string scriptConstPath = null;
        private List<string> folderList = new List<string> {
            "Battle",
            "Map",
            "UI",
            "Chest"
        };

        public static void GenerateConfigPath() {
            Instance.scriptConstPath = Application.dataPath + "/Scripts/CSharp/Const";

            //Instance.GenerateSpritePath();
            Instance.GeneratePrefabPath();
            Instance.GeneratePrefabName();
        }

        private void GenerateSpritePath() {
            //TextAsset conf = Resources.Load<TextAsset>(Path.configure + "art_prefab");
            TextAsset conf =
                AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Configures/art_prefab.csv");
            string confText = conf.text;
            string[] lines = confText.CustomSplit('\n');
            GameHelper.RemoveFormatChar(ref lines[0]);
            Regex r = new Regex(",");
            string[] attributes = r.Split(lines[0]);

            string filePath = this.scriptConstPath + "/SpritePathHashConst.cs";
            File.WriteAllText(filePath, "");
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                StringBuilder builder = new StringBuilder();
                this.GenerateUsingAndHeader("SpritePathHashConst", builder);

                for (int i = 1; i < lines.Length; i++) {
                    GameHelper.RemoveFormatChar(ref lines[i]);
                    string[] values = r.Split(lines[i]);
                    if (values.Length != attributes.Length) {
                        if (i != lines.Length - 1) {
                            Debug.LogError("Config " + "art_prefab" + " row " + i + " has error!");
                        }
                        continue;
                    }
                    string[] nameArray = values[0].Split("_"[0]);
                    string name = nameArray[0];
                    for (int j = 1; j < nameArray.Length; j++) {
                        name += GameHelper.UpperFirstCase(nameArray[j]);
                    }
                    builder.Append("\t\tpublic static string " + name + " = \"" + values[0] + "\";\n");
                }

                this.GenerateFoot(builder);
                writer.Write(builder.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        private void GeneratePrefabPath() {
            string filePath = this.scriptConstPath + "/PrefabPath.cs";
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                StringBuilder builder = new StringBuilder();
                this.GenerateUsingAndHeader("PrefabPath", builder);

                foreach (string folder in this.folderList) {
                    this.GeneratePrefabPathItem(Application.dataPath + "/Resources/" + folder, builder);
                }

                this.GenerateFoot(builder);
                writer.Write(builder.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        private void GeneratePrefabName() {
            string filePath = this.scriptConstPath + "/PrefabName.cs";
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                StringBuilder builder = new StringBuilder();
                this.GenerateUsingAndHeader("PrefabName", builder);

                foreach (string folder in this.folderList) {
                    this.GeneratePrefabNameItem(Application.dataPath + "/Resources/" + folder, builder);
                }

                this.GenerateFoot(builder);
                writer.Write(builder.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        private void GeneratePrefabPathItem(string directory, StringBuilder builder) {
            string[] fileArray = Directory.GetFiles(directory);
            string[] directoryArray = Directory.GetDirectories(directory);

            foreach(string value in directoryArray) {
                this.GeneratePrefabPathItem(value, builder);
            }

            foreach (string file in fileArray) {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".meta") {
                    continue;
                }
                string fileName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length,
                    fileInfo.Extension.Length);
                string path = file.Remove(0, (Application.dataPath + "/Resources/").Length).Replace("\\", "/");
                path = path.Remove(path.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
                builder.Append("\t\tpublic static string " + GameHelper.LowerFirstCase(fileName) +
                    " = \"" + path + "\";\n");
            }
        }

        private void GeneratePrefabNameItem(string directory, StringBuilder builder) {
            string[] fileArray = Directory.GetFiles(directory);
            string[] directoryArray = Directory.GetDirectories(directory);

            foreach (string value in directoryArray) {
                this.GeneratePrefabNameItem(value, builder);
            }

            foreach (string file in fileArray) {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".meta") {
                    continue;
                }
                string fileName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length,
                    fileInfo.Extension.Length);
                string path = file.Remove(0, (Application.dataPath + "/Resources/").Length).Replace("\\", "/");
                path = path.Remove(path.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
                string[] names = path.Split("/"[0]);
                string name = names[names.Length - 1];
                builder.Append("\t\tpublic static string " + GameHelper.LowerFirstCase(fileName) +
                    " = \"" + name + "\";\n");
            }
        }


        private void GenerateUsingAndHeader(string name, StringBuilder builder) {
            builder.AppendLine("namespace Poukoute {");
            builder.AppendFormat("\tpublic class {0} {{\n", name);
        }
        private void GenerateContent() {

        }
        private void GenerateFoot(StringBuilder builder) {
            builder.AppendLine("\t}");
            builder.AppendLine("}");
        }
    }
}
