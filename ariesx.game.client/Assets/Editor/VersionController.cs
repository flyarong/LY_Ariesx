using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Poukoute;
using Protocol;
using Resources = UnityEngine.Resources;

public struct VersionParam {
    public string language;
    public string channel;
    public NetGate netGate;
    public string account;
    public string url;
}

public enum VersionType {
    googleplay,
    taptap,
    qacn,
    qaen,
    qaenproduct,
    qacnproduct,
    develop
}

public class VersionController : MonoBehaviour {
    private static readonly string scriptPath =
        Application.dataPath + "/Scripts/CSharp/Const/VersionConst.cs";
    private static readonly string gradleVersionPath =
        "/Users/poukouteci/Documents/AndroidProjects/";

    public static readonly Dictionary<VersionType, VersionParam> versionDict =
        new Dictionary<VersionType, VersionParam>() {
            {VersionType.googleplay, new VersionParam {
                    language = "en",
                    channel = "GP",
                    account = "googleplay",
                    netGate = NetGate.googleplay,
                    url = "http://api.gateway.ariesx.dragonest.com/"
                }
            },
            {VersionType.taptap, new VersionParam {
                    language = "cn",
                    channel = "Taptap",
                    account = "longyuan",
                    netGate = NetGate.taptap,
                    url = "http://api.gateway.ariesx.dragonest.com/"
                }
            },
            {VersionType.qacn, new VersionParam {
                    language = "cn",
                    channel = "QA",
                    account = "longyuan",
                    netGate = NetGate.LYtest,
                    url = "http://api.gatewaytest.ariesx.dragonest.com/"
                }
            },
            {VersionType.qaen, new VersionParam {
                    language = "en",
                    channel = "QA",
                    netGate = NetGate.LYtest,
                    url = "http://api.gatewaytest.ariesx.dragonest.com/"
                }
            },
            {VersionType.develop, new VersionParam {
                    language = "cn",
                    channel = "DEV",
                    account = "longyuan",
                    netGate = NetGate.developer,
                    url = "http://192.168.20.212:9901/"
                }
            },
            {VersionType.qaenproduct, new VersionParam {
                    language = "en",
                    channel = "QA",
                    account = "googleplay",
                    netGate = NetGate.LYtest,
                    url = "http://api.gatewaytest.ariesx.dragonest.com/"
                }
            },
            {VersionType.qacnproduct, new VersionParam {
                    language = "cn",
                    channel = "QA",
                    account = "longyuan",
                    netGate = NetGate.LYtest,
                    url = "http://api.gatewaytest.ariesx.dragonest.com/"
                }
            },
        };


    [MenuItem("Poukoute/Generator/Generate Version")]
    private static void GenerateVersion() {
        EditorWindow window = EditorWindow.GetWindow<VersionGenerator>(
            false,"Version Generater", false
        ) as EditorWindow;
        window.ShowPopup();
        window.Focus();
        EditorWindow.FocusWindowIfItsOpen<VersionGenerator>();
    }

    public static void GenerateVersionConst() {
        TextAsset versionFile = Resources.Load("Configures/version") as TextAsset;
        string[] versionArray = versionFile.text.Split('-');
        string versionStr = versionArray[3].Split('(')[0];
        VersionType version = (VersionType)System.Enum.Parse(typeof(VersionType), versionStr);
        GenerateVersionConst(version);
    }

    public static void GenerateVersionConst(VersionType version) {
        // Generate VersionConst.cs.
        VersionParam versionParam = versionDict[version];
        using (StreamWriter writer = new StreamWriter(scriptPath, false, Encoding.UTF8)) {
            StringBuilder builder = new StringBuilder();
            string header = "namespace Poukoute{";
            string headerSub = "\tpublic partial class VersionConst {";
            string bodyLanguage = string.Concat("\t\tpublic static string language = \"", versionParam.language, "\";");
            string bodyChannel = string.Concat("\t\tpublic const string channel = \"", versionParam.channel, "\";");
            string bodyVersion = string.Concat("\t\tpublic const string version = \"", version, "\";");
            string bodyAccount = string.Concat("\t\tpublic const string account = \"", versionParam.account, "\";");
            string bodyUrl = string.Concat("\t\tpublic const string url = \"", versionParam.url, "\";");
            string bodyNetgate = string.Concat(
                "\t\tpublic const NetGate netGate = NetGate.",
                versionParam.netGate.ToString(), ";");
            string footerSub = "\t}";
            string footer = "}";
            builder.AppendLine(header);
            builder.AppendLine(headerSub);
            builder.AppendLine(bodyLanguage);
            builder.AppendLine(bodyChannel);
            builder.AppendLine(bodyVersion);
            builder.AppendLine(bodyAccount);
            builder.AppendLine(bodyUrl);
            builder.AppendLine(bodyNetgate);
            builder.AppendLine(footerSub);
            builder.AppendLine(footer);
            Debug.LogError("Builder String Length:" + builder.ToString().Length);
            writer.Write(builder.ToString());
            writer.Flush();
            writer.Close();
        }

        // This may cause AssetData.Refresh, need to call after writer.Flush.
        string defineSymbols = string.Empty;
        if (versionParam.account == "longyuan") {
            defineSymbols = "LONGYUAN";
        } else if (versionParam.account == "googleplay") {
            defineSymbols = "GOOGLEPLAY";
        }

        if (versionParam.netGate == NetGate.developer ||
            versionParam.netGate == NetGate.LYtest) {
            defineSymbols = string.Concat(defineSymbols, ";DEVELOPER");
            if (versionParam.netGate == NetGate.developer) {
                defineSymbols = string.Concat(defineSymbols, ";NOLYLOGIN");
            }
        }

        if (!defineSymbols.CustomIsEmpty()) {
            Debug.LogError("Set Developer");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android | BuildTargetGroup.iOS, defineSymbols);
        }

        // Generate version.txt for AndroidStudio project.
        GenerateGradleVersionText(versionParam.language, versionParam.account,
            PlayerSettings.Android.bundleVersionCode, Application.version, version.ToString());
    }

    private static void GenerateGradleVersionText(string language,
        string account, int versionCode, string versionName, string versionType) {
        if (Directory.Exists(gradleVersionPath)) {
            using (StreamWriter writer = new StreamWriter(
                gradleVersionPath + "version.txt",
                false, Encoding.ASCII)) {
                string version = string.Format("{0} {1} {2} {3} {4}",
                    language, account, versionCode, versionName, versionType);
                writer.Write(version);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
