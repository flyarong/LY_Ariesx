using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

public class VersionControllerEditor : EditorWindow {
    private string version = string.Empty;
    private string versionTemp = string.Empty;
    private int build = 0;
    private VersionType versionType = VersionType.develop;
    //private string type = "develop";

    void OnEnable() {
        TextAsset versionText = Resources.Load<TextAsset>("Configures/version");
        string versionStr = versionText.text;

        int lastSplitIndex = versionStr.LastIndexOf('-');

        version =
        versionTemp = versionStr.Substring(0, lastSplitIndex).Replace('-', '.');
        int buildStartIndex = versionStr.IndexOf('(');
        int buildEndIndex = versionStr.IndexOf(')');
        build = int.Parse(versionStr.Substring(buildStartIndex + 1,
            buildEndIndex - buildStartIndex - 1));
        versionType = (VersionType)System.Enum.Parse(
            typeof(VersionType),
            versionStr.Substring(lastSplitIndex + 1,
                buildStartIndex - lastSplitIndex - 1)
        );
    }

    void OnGUI() {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("New Version", GUILayout.Width(50));
        this.versionTemp = EditorGUILayout.TextField(this.versionTemp, GUILayout.Width(100));
        EditorGUILayout.LabelField(string.Concat("(",this.build, ")"), GUILayout.Width(50));
        versionType = (VersionType)EditorGUILayout.EnumPopup(versionType);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Set")) {
            if (versionType == VersionType.googleplay) {
                build = ++PlayerSettings.Android.bundleVersionCode;
            } else if (versionType == VersionType.taptap) {
                build = ++PlayerSettings.Android.bundleVersionCode;
            } else if (!version.Equals(versionTemp)) {
                build = 0;
            }
            version = versionTemp;
            PlayerSettings.bundleVersion = version;
            string path = string.Format(Application.dataPath + "/Resources/Configures/version.txt");
            File.WriteAllText(path, "");
            StreamWriter streamWriter = new StreamWriter(path, true, Encoding.UTF8);

            streamWriter.Write(
                string.Concat(
                    version, ".",
                    versionType.ToString(),
                    '(', (build + 1), ')'
                ).Replace('.', '-')
            );
            streamWriter.Flush();
            streamWriter.Close();
        }

    }
}
