using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Poukoute {
    public class LocalFileGenerator : MonoBehaviour {
        private static StreamWriter localWriter;
        private static StreamWriter localConstWriter = null;
        //private static int localHashStart = 1;

        public static void GeneratorLocal() {
            Caching.CleanCache();
            GenerateLocalHashHead();

            string path = string.Format(Application.dataPath + "/Resources/Configures/local_list.csv");
            File.WriteAllText(path, "");
            localWriter = new StreamWriter(path, true, Encoding.UTF8);
            localWriter.WriteLine("file,amount");
            Debug.LogError("Generate Local");
            InnerLoadAllLoclFile();

            localWriter.Flush();
            localWriter.Close();

            GenerateLocalHashTail();
        }

        private static void GenerateLocalHashHead() {
            string localHashConstPath = Application.dataPath + "/Scripts/CSharp/Const/LocalHashConst.cs";
            File.WriteAllText(localHashConstPath, "");
            localConstWriter = new StreamWriter(localHashConstPath, true, Encoding.UTF8);
            localConstWriter.WriteLine("using UnityEngine;");
            localConstWriter.WriteLine("using System;");
            localConstWriter.WriteLine("using System.Collections;");
            localConstWriter.WriteLine("using System.Collections.Generic;");
            localConstWriter.WriteLine("namespace Poukoute {");
            localConstWriter.WriteLine("    public class LocalHashConst {");
        }

        private static void GenerateLocalHashTail() {
            localConstWriter.WriteLine("    }");
            localConstWriter.WriteLine("}");
            localConstWriter.Flush();
            localConstWriter.Close();
        }

        private static void InnerLoadAllLoclFile() {
            //localHashStart = 1;
            ReadFile("local_system");
            ReadFile("local_task");
            ReadFile("local_hero");
            ReadFile("local_skill");
            ReadFile("local_battle");
            ReadFile("local_building");
            ReadFile("local_normal");
            ReadFile("local_city");
            ReadFile("local_warning");
            ReadFile("local_fte");
            ReadFile("local_server");
            ReadFile("local_shop");
        }

        private static void ReadFile(string name) {
            //Debug.LogError(name);
            TextAsset conf =
                AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Configures/" + name + ".csv");
            if (conf == null) {
                Debug.LogError(name);
            }
            string confText = conf.text;
            string[] splitArray = { "\r\n", "\r", "\n" };
            string[] lines = confText.CustomSplit(ref splitArray);
            GameHelper.RemoveFormatChar(ref lines[0]);
            string[] attributes = CSVReader.SplitCsvLine(lines[0]);
            List<string[]> attrList = new List<string[]>();
            for (int i = 1; i < lines.Length; i++) {
                GameHelper.RemoveFormatChar(ref lines[i]);
                string[] values = CSVReader.SplitCsvLine(lines[i]);
                if (values.Length != attributes.Length) {
                    if (i != lines.Length - 1) {
                        Debug.LogWarning("Config " + name + " row " + i + " has error!");
                    } else {
                        continue;
                    }
                }
                attrList.Add(values);
            }
            // Generate Local hash Const cs file

            //int localIndex = localHashStart;
            int attrListLen = attrList.Count;
            for (int i = 0; i < attrListLen; i++) {
                string key = attrList[i][0];
                if (key.CustomIsEmpty()) {
                    Debug.LogError(name + " attrList has error on " + i);
                    return;
                } else {
                    string line = "        public const ulong " + key.Substring(3) + " = " + key.Substring(3).CustomGetHashCode() + ";";
                    localConstWriter.WriteLine(line);
                }
            }

            int fileCount = 0;
            StreamWriter streamWriter = null;
            for (int i = 1; i < attributes.Length; i++) {
                //localIndex = localHashStart;
                string directory = string.Format(Application.dataPath +
                    "/Resources/Local/{0}", attributes[i].ToLower());
                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                int count = 1;
                fileCount = 0;
                foreach (string[] attrArray in attrList) {
                    if (count % GameConst.configMaxLine == 1) {
                        string path = string.Format(
                            Application.dataPath + "/Resources/Local/{0}/{1}_{2}.csv",
                            attributes[i].ToLower(),
                            name,
                            ++fileCount
                        );
                        File.WriteAllText(path, "");
                        streamWriter = new StreamWriter(path, true, Encoding.UTF8);
                    }
                    //if (attrArray.Length < i + 1) {
                    //    Debug.LogError(i + "   " + count);
                    //}
                    string value = string.IsNullOrEmpty(attrArray[i]) ? "null" : attrArray[i];
                    //value = value.Replace("\"\"", "\"");
                    if (value.Contains(",")) {
                        value = "\"" + value + "\"";
                    }
                    //streamWriter.WriteLine(string.Format(
                    //    GameConst.TWO_PART_WITH_COMMA, (localIndex + count - 1), value));
                    streamWriter.WriteLine(string.Format(
                        GameConst.TWO_PART_WITH_COMMA, attrArray[0].Substring(3).CustomGetHashCode(), value));
                    if (count % GameConst.configMaxLine == 0 || count == attrList.Count) {
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    count++;
                }
            }
            //localHashStart += attrListLen;
            localWriter.WriteLine(name + "," + fileCount);
        }
    }


}
