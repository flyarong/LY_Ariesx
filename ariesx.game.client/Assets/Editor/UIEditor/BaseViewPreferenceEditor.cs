using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

namespace Poukoute {
    [CustomEditor(typeof(BaseViewPreference), true)]
    public class BaseViewPreferenceEditor : Editor {
        BaseViewPreference view;

        void OnEnable() {
            this.view = target as BaseViewPreference;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh")) {
                this.RefreshAssigned();
            }
            if (GUILayout.Button("CreatPath")) {
                Debug.Log(this.view.path);
                this.view.path = "d:/ariesx.game.client/assets/scripts/csharp/viewpreferences/screeneffectviewpreference.cs";
                StreamReader reader = new StreamReader(this.view.path, Encoding.UTF8);
                StringBuilder builder = new StringBuilder();
                while (true) {
                    string line = reader.ReadLine();
                    if (line == null) {
                        break;
                    }
                    string[] sArray = System.Text.RegularExpressions.Regex.Split(line,@"[' ']+") ;
                    for (int i = 0; i < sArray.Length; i++) {
                        if (sArray[i].Equals("public")) {
                            string aim = sArray[i + 2].Replace(";", "");
                            foreach (var path in this.view.pathDict) {
                                if (aim.Equals(path.Key)) {
                                    if (path.Value != null) {
                                        if (path.Value.gameObject.name.Equals(this.view.gameObject.name)) {
                                            builder.AppendLine("        [Tooltip(\"" + path.Value.name + "\")]");
                                        } else {
                                            string objctPath = "\")]";
                                            GameObject pathObjct = path.Value;
                                            while (true) {
                                                if (pathObjct.transform.parent.gameObject.name.Equals(this.view.gameObject.name)) {
                                                    objctPath = "       [Tooltip(\"" + this.view.gameObject.name + "/" + pathObjct.name + objctPath;
                                                    break;
                                                } else {
                                                    objctPath = "/" + pathObjct.name + objctPath;
                                                    pathObjct = pathObjct.transform.parent.gameObject;
                                                }
                                            }
                                            builder.AppendLine(objctPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    builder.AppendLine(line);
                }
                builder.Replace("[PathAttribute]", "");
                reader.Close();
                StreamWriter writer = new StreamWriter(this.view.path, false, Encoding.UTF8);
                writer.Write(builder.ToString());
                writer.Flush();
                writer.Close();
                this.view.pathDict.Clear();
            }
        }

        private void RefreshAssigned() {
            SerializedProperty property = this.serializedObject.FindProperty("amount");
            Debug.LogError(((BaseViewPreference)property.serializedObject.targetObject).transform.name);
        }
    }
}