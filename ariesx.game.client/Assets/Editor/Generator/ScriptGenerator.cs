using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

/* Can write a single class to format the scripts(offset) */

namespace Poukoute {

    public class ScriptGenerator : EditorWindow {
        private string sceneName;
        private string scriptPath_CSharp;
        private Dictionary<string, List<string>> usingDict = new Dictionary<string, List<string>>();
        void OnEnable() {
            this.scriptPath_CSharp = Application.dataPath + "/Scripts/CSharp/";
            this.usingDict.Add(
                "Base", new List<string>() {
                    "UnityEngine",
                    "System.Collections",
                    "System.Collections.Generic"
                }
            );
            this.usingDict.Add(
                "Model", new List<string>() {
                    "Protocol",
                }
            );
            this.usingDict.Add(
                "ViewModel", new List<string>() {
                    "ProtoBuf",
                    "Protocol",
                }
            );
            this.usingDict.Add(
                "View", new List<string>() {
                    "System",
                    "Protocol",
                    "UnityEngine.UI",
                }
            );
        }

        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scene Name");
            sceneName = EditorGUILayout.TextField(sceneName);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("GenerateVVM")) {
                this.GenerateScripts();
            }

            if (GUILayout.Button("GenerateMVVM")) {
                this.GenerateAllScripts();
            }
        }

        private void GenerateScripts() {
            this.GenerateScript("ViewModel");
            this.GenerateScript("View");
        }

        private void GenerateAllScripts() {
            this.GenerateScript("Model");
            this.GenerateScript("ViewModel");
            this.GenerateScript("View");
        }

        private void GenerateScene() {
            System.Type type = System.Type.GetType(string.Format("Poukoute.{0}ViewModel, Assembly-CSharp", GameHelper.UpperFirstCase(this.sceneName)));
            if (type == null) {
                Debug.LogError("Generate Script First!");
                return;
            }
            GameObject obj = new GameObject(GameHelper.UpperFirstCase(this.sceneName) + "Manager");
            Component o = obj.AddComponent(type);

            FieldInfo field = o.GetType().GetField("sceneName");
            field.SetValue(o, "scene_" + this.sceneName.ToLower());

            EditorSceneManager.SaveOpenScenes();

            Close();
        }

        private void GenerateScript(string type) {
            string filePath = this.scriptPath_CSharp + type + "s/" + GameHelper.UpperFirstCase(this.sceneName) + type + ".cs";
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                StringBuilder builder = new StringBuilder();
                this.GenerateUsing(type, builder);
                builder.AppendLine();
                this.GenerateHead(type, builder);

                switch (type) {
                    case "Model":
                        this.GenerateModelBody(builder);
                        break;
                    case "ViewModel":
                        this.GenerateViewModelBody(builder);
                        break;
                    case "View":
                        this.GenerateViewBody(builder);
                        break;
                    default:
                        Debug.LogErrorf("Wrong type: {0}", type);
                        break;
                }

                this.GenerateFoot(builder);
                writer.Write(builder.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        private void GenerateUsing(string type, StringBuilder builder) {
            foreach (string str in this.usingDict["Base"]) {
                builder.AppendFormat("using {0};\n", str);
            }
            foreach (string str in this.usingDict[type]) {
                builder.AppendFormat("using {0};\n", str);
            }
        }

        private void GenerateHead(string type, StringBuilder builder) {
            builder.AppendLine("namespace Poukoute {");
            string className = GameHelper.UpperFirstCase(this.sceneName) + type;
            builder.AppendFormat("\tpublic class {0} : Base{1} {{\n", className, type);
        }

        private void GenerateFoot(StringBuilder builder) {
            builder.AppendLine("\t}");
            builder.AppendLine("}");
        }

        private void GenerateModelBody(StringBuilder builder) {
            string offset = "\t\t";
            builder.AppendLine(offset + "/* Add data member in this */");
            builder.AppendLine();
            builder.AppendLine(offset + "/***************************/");

            builder.AppendLine(offset + "public void Refresh(object message) {");
            offset += "\t";
            builder.AppendLine(offset + "/* Refresh your data in this function */");
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
        }

        private void GenerateViewModelBody(StringBuilder builder) {
            string offset = "\t\t";
            builder.AppendFormat("{0}private {1}Model model;\n", offset, GameHelper.UpperFirstCase(this.sceneName));
            builder.AppendFormat("{0}private {1}View view;\n", offset, GameHelper.UpperFirstCase(this.sceneName));
            builder.AppendLine(offset + "/* Model data get set */");
            builder.AppendLine();
            builder.AppendLine(offset + "/**********************/");
            builder.AppendLine();
            builder.AppendLine(offset + "/* Other members */");
            builder.AppendLine();
            builder.AppendLine(offset + "/*****************/");
            builder.AppendLine();
            builder.AppendLine(offset + "void Awake() {");
            offset += "\t";
            builder.AppendFormat(offset + "this.model = ModelManager.GetModelData<{0}Model>();\n", GameHelper.UpperFirstCase(this.sceneName));
            builder.AppendFormat(offset + "this.view = this.gameObject.AddComponent<{0}View>();\n", GameHelper.UpperFirstCase(this.sceneName));
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "public void Show() {");
            offset += "\t";
            builder.AppendLine(offset + "this.gameObject.SetActiveSafe(true);");
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "public void Hide() {");
            offset += "\t";
            builder.AppendLine(offset + "this.gameObject.SetActiveSafe(false);");
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "/* Add 'NetMessageAck' function here*/");
            builder.AppendLine();
            builder.AppendLine(offset + "/***********************************/");
            offset = offset.Substring(1);
        }

        private void GenerateViewBody(StringBuilder builder) {
            string offset = "\t\t";
            builder.AppendLine(offset + "private " + sceneName + "ViewModel viewModel;");
            builder.AppendLine(offset + "private " + sceneName + "ViewPeference viewPref;");
            builder.AppendLine(offset + "private GameObject ui;");
            builder.AppendLine(offset + "/* UI Members*/");
            builder.AppendLine();
            builder.AppendLine(offset + "/*************/");
            builder.AppendLine();
            builder.AppendLine(offset + "void Awake() {");
            offset += "\t";
            builder.AppendFormat(offset + "this.viewModel = this.gameObject.GetComponent<{0}ViewModel>();\n", GameHelper.UpperFirstCase(this.sceneName));
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "protected override void OnUIInit() {");
            offset += "\t";
            builder.AppendFormat(offset + "this.ui = UIManager.GetUI(\"UI{0}\");\n", this.sceneName.ToLower());
            builder.AppendLine(offset + "/* Cache the ui components here */");
            offset = offset.Substring(1);
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "/* Propert change function */");
            builder.AppendLine();
            builder.AppendLine(offset + "/***************************/");
            builder.AppendLine();
            builder.AppendLine(offset + "protected override void OnVisible() {");
            builder.AppendLine(offset + "}");
            builder.AppendLine();
            builder.AppendLine(offset + "protected override void OnInvisible() {");
            builder.AppendLine(offset + "}");

        }
    }
}
