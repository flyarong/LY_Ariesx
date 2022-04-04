using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Protocol;

namespace Poukoute {
    public class LocalManager : MonoBehaviour {
        private static LocalManager self;
        public static LocalManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("LocalManager is not initialized.");
                }
                return self;
            }
        }

        private Dictionary<SystemLanguage, string> langDict = new Dictionary<SystemLanguage, string> {
            { SystemLanguage.Chinese,   "cn" },
            { SystemLanguage.ChineseSimplified, "cn" },
            { SystemLanguage.ChineseTraditional, "cn" },
            { SystemLanguage.English,   "en" },
            { SystemLanguage.French,    "fr" },
            { SystemLanguage.German,    "de" },
            { SystemLanguage.Czech,     "cz" },
            { SystemLanguage.Polish,    "pl" }
        };

        public static string SdkLanguage {
            get {
                switch (Instance.local) {
                    case "cn":
                        return "zh-CN";
                    default:
                        return Instance.langDict[SystemLanguage.English];
                }
            }
        }

        // To do: default language should be EN.
        private string local = VersionConst.language;
        public static string Language {
            get {
                return Instance.local;
            }
            set {
                VersionConst.language = value;
                GameManager.RestartGame();
            }
        }

        public static string ServerLanguage {
            get {
                SystemLanguage sysLang = Application.systemLanguage;
                if (Instance.langDict.ContainsKey(sysLang)) {
                    return Instance.langDict[sysLang];
                } else {
                    return "en";
                }
            }
        }
        private Dictionary<ulong, string> localDict = new Dictionary<ulong, string>(3700);
        void Awake() {
            self = this;
            this.InnerLoadAllLocal();
        }

        private void InnerLoadAllLocal() {
            this.LoadLocal("local_fte");
            this.LoadLocal("local_system");
            this.LoadLocal("local_task");
            this.LoadLocal("local_hero");
            this.LoadLocal("local_skill");
            this.LoadLocal("local_battle");
            this.LoadLocal("local_building");
            this.LoadLocal("local_normal");
            this.LoadLocal("local_city");
            this.LoadLocal("local_warning");
            this.LoadLocal("local_server");
            this.LoadLocal("local_shop");
        }

        private void LoadLocal(string fileName) {
            int amount = ListConf.GetFileAmount(fileName);
            for (int i = 1; i <= amount; i++) {
                string fullName =
                    string.Format("{0}{1}/{2}_{3}", Path.local, this.local, fileName, i);
                CSVReader.ReadCSV(
                    UnityEngine.Resources.Load<TextAsset>(fullName),
                    this.localDict);
            }
        }

        public static void SetLocalLanguage(SystemLanguage lang) {
            Language = Instance.langDict[lang];
        }

        public static Dictionary<SystemLanguage, string> GetGameLanguageDict() {
            return Instance.langDict;
        }

        public static string GetValue(string str0, string str1) {
            return GetValue(string.Concat(str0, str1).CustomGetHashCode());
        }

        public static string GetValue(string str0, string str1, string str2) {
            return GetValue(string.Concat(str0, str1, str2).CustomGetHashCode());
        }

        public static string GetValue(string str0, string str1, string str2, string str3) {
            return GetValue(string.Concat(str0, str1, str2, str3).CustomGetHashCode());
        }

        public static string GetValue(string str0, int int1, string str2, int int3) {
            return GetValue(string.Concat(str0, int1, str2, int3).CustomGetHashCode());
        }

        public static string GetValue(string key) {
            return GetValue(key.CustomGetHashCode());
        }

        public static string GetValue(ulong key, params object[] args) {
            return string.Format(GetValue(key), args);
        }

        public static string GetValue(ulong key) {
            string localValue;
#if UNITY_EDITOR
            if (self == null) {
                return string.Empty;
            }
#endif
            if (!Instance.localDict.TryGetValue(key, out localValue)) {
                Debug.LogWarning(key + " not valid!");
                return string.Empty;
            }
            if (localValue.Contains("\"\"")) {
                localValue = localValue.Replace("\"\"", "\"");
            }
            return localValue.Replace("\\n", "\n");
        }
    }
}
