using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Protocol;
using UnityEngine.Profiling;

namespace Poukoute {

    public abstract class BaseConf {
        abstract public void SetProperty(Dictionary<string, string> attrDict);
        abstract public string GetId();
        //virtual public string GetId(Dictionary<string, string> attriDict) {
        //    return null;
        //} 
    }

    public partial class ConfigureManager : MonoBehaviour {
        private static bool init = false;
        private static Dictionary<string, Dictionary<string, BaseConf>> confDict = 
            new Dictionary<string, Dictionary<string, BaseConf>>();
        private static ConfigureManager self;
        public static ConfigureManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("ConfigureManager is not initialized.");
                }
                return self;
            }
        }

        void Awake() {
            self = this;
            if (!ConfigureManager.init) {
                this.ReadConf<ListConf>("local_list");
                this.ReadConf<ListConf>("configure_list");
                ConfigureManager.init = true;
            }
            ArtPrefabConf.Instance.Clear();
            this.LoadArtPrefabConf("art_audio");
        }

#if UNITY_EDITOR
        public static void LoadHeroEditorConfigures() {
            self = GameObject.Find("ConfigureManager").GetComponent<ConfigureManager>();
            Instance.Awake();
            ArtPrefabConf.Instance.Clear();
            Instance.LoadArtPrefabConf("art_audio");
            Instance.LoadArtPrefabConf("campaign_hero_effect");
        }

        public static void LoadBattleEditorConfigures() {
            self = GameObject.Find("ConfigureManager").GetComponent<ConfigureManager>();
            Instance.Awake();
            ArtPrefabConf.Instance.Clear();
            Instance.LoadArtPrefabConf("art_audio");
            Instance.LoadArtPrefabConf("campaign_hero_effect");
            Instance.LoadArtPrefabConf("art_prefab");
            Instance.LoadArtPrefabConf("art_lottery");
            Instance.LoadArtPrefabConf("art_hero_avatar");
        }

        public static void LoadCityEditorConfigures() {
            self = GameObject.Find("ConfigureManager").GetComponent<ConfigureManager>();
            Instance.Awake();
        }

        public static void LoadAudioEditorConfigures() {
            self = GameObject.Find("ConfigureManager").GetComponent<ConfigureManager>();
            Instance.Awake();
            ArtPrefabConf.Instance.Clear();
            //    Instance.LoadConfigure<AudioImportConf>();
        }
#endif

        public static void LoadOtherConfigures() {
            Instance.InnerLoadOtherConfigures();
        }

        private void InnerLoadOtherConfigures() {
            Profiler.BeginSample("InnerLoadOtherConfigures");
            long start = System.DateTime.UtcNow.Ticks;

            this.LoadArtPrefabConf("art_prefab");
            this.LoadArtPrefabConf("art_lottery");
            this.LoadArtPrefabConf("art_hero_avatar");
            this.LoadArtPrefabConf("art_tile_layer");
            this.LoadArtPrefabConf("art_skill");
            this.LoadArtPrefabConf("art_npc_city");
            this.LoadArtPrefabConf("campaign_hero_effect");

            //Debug.LogError(System.DateTime.UtcNow.Ticks - start);
            Profiler.EndSample();
        }

        public void LoadArtPrefabConf(string name, UnityAction startCallback = null,
            UnityAction endCallback = null) {
            if (startCallback != null) {
                startCallback.Invoke();
            }
            int amount = ListConf.GetFileAmount(name);
            for (int i = 1; i <= amount; i++) {
                Profiler.BeginSample("ReadArtPrefabConf");
                this.ReadArtPrefabConf(name + "_" + i);
                Profiler.EndSample();
            }
            if (endCallback != null) {
                endCallback.Invoke();
            }
        }


        // only can call from XConf.cs
        public void LoadConfigure<T>() where T : BaseConf, new() {
            if (!ConfigureManager.confDict.ContainsKey(typeof(T).Name)) {
                this.InnerLoadConfigure<T>();
            }
        }

        public bool InnerLoadConfigure<T>() where T : BaseConf, new() {
            string path;
            if (!this.ConfigurePath.TryGetValue(typeof(T).Name, out path)) {
                Debug.LogErrorf("No such configure file with type of {0}", typeof(T).Name);
            }
            UnityAction startCallback = null;
            UnityAction endCallback = null;
            switch (path) {
                case "hero_attribute_basic":
                    startCallback = HeroBaseConf.Clear;
                    break;
                case "fte_step":
                    startCallback = FteStepConf.BeforeRead;
                    endCallback = FteStepConf.AfterRead;
                    break;
                case "mini_map_pass":
                    startCallback = MiniMapPassConf.BeforeRead;
                    break;
                case "npc_city":
                    startCallback = NPCCityConf.BeforeRead;
                    endCallback = NPCCityConf.AfterRead;
                    break;
                case "chapter_task":
                    endCallback = DramaConf.AfterRead;
                    break;
                default:
                    break;
            }
            startCallback.InvokeSafe();
            int amount = ListConf.GetFileAmount(path);
            bool isExist = false;
            for (int i = 1; i <= amount; i++) {
                isExist |= this.ReadConf<T>(path + "_" + i);
            }
            endCallback.InvokeSafe();
            return isExist;
        }

        private void ReadArtPrefabConf(string name) {
            TextAsset conf = UnityEngine.Resources.Load<TextAsset>(Path.configure + name);
            string[] lines = conf.text.CustomSplit(ref CSVReader.splitArray);
            GameHelper.RemoveFormatChar(ref lines[0]);
            string[] attributes = CSVReader.SplitCsvLine(lines[0]);
            int valuesLength = 0;
            int attributesLength = 0;
            for (int i = 1; i < lines.Length; i++) {
                GameHelper.RemoveFormatChar(ref lines[i]);
                Dictionary<string, string> attrDict = new Dictionary<string, string>();
                string[] values = CSVReader.SplitCsvLine(lines[i]);
                attributesLength = attributes.Length;
                valuesLength = values.Length;
                if (valuesLength != attributesLength) {
                    if (i == lines.Length - 1) {
                        continue;
                    }
                }

                for (int j = 0; j < attributesLength; j++) {
                    if (valuesLength < j + 1) {
                        attrDict.Add(attributes[j], string.Empty);
                    } else {
#if UNITY_EDITOR
                        if (attrDict.ContainsKey(attributes[j])) {
                            Debug.LogError(i + ":" + attributes[j]);
                        }
#endif
                        attrDict.Add(attributes[j], values[j]);
                    }
                }
                ArtPrefabConf.Instance.SetProperty(attrDict);
            }
        }

        // only can call from LoadConf()
        private bool ReadConf<T>(string name) where T : BaseConf, new() {
            Profiler.BeginSample("InnerLoadConf");

            Profiler.BeginSample("ReadConf.Load from resources");
            TextAsset conf = UnityEngine.Resources.Load<TextAsset>(Path.configure + name);
            Profiler.EndSample();
#if UNITY_EDITOR
            if (conf == null) {
                Debug.LogError("Resources.Load " + name + " not found");
                return false;
            }
#endif

            Profiler.BeginSample("CustomSplit");
            string[] lines = conf.text.CustomSplit(ref CSVReader.splitArray);
            Profiler.EndSample();
            GameHelper.RemoveFormatChar(ref lines[0]);
            string typeStr = typeof(T).Name;
            string[] attributes = CSVReader.SplitCsvLine(lines[0]);
            Dictionary<string, BaseConf> table;
            if (!ConfigureManager.confDict.TryGetValue(typeStr, out table)) {
                table = new Dictionary<string, BaseConf>();
            }
            int valuesLength = 0;
            int attributesLength = 0;
            for (int i = 1; i < lines.Length; i++) {
                GameHelper.RemoveFormatChar(ref lines[i]);
                T row = new T();
                Dictionary<string, string> attrDict = new Dictionary<string, string>();
                string[] values = CSVReader.SplitCsvLine(lines[i]);
                attributesLength = attributes.Length;
                valuesLength = values.Length;
                if (valuesLength != attributesLength) {
                    if (i == lines.Length - 1) {
                        continue;
                    }
                }

                for (int j = 0; j < attributesLength; j++) {
                    if (valuesLength < j + 1) {
                        attrDict.Add(attributes[j], string.Empty);
                    } else {
#if UNITY_EDITOR
                        if (attrDict.ContainsKey(attributes[j])) {
                            Debug.LogError(i + ":" + attributes[j]);
                        }
#endif
                        attrDict.Add(attributes[j], values[j]);
                    }
                }
                row.SetProperty(attrDict);
#if UNITY_EDITOR
                string id = row.GetId();
                if (table.Keys.Contains(id)) {
                    Debug.LogError(id + " already contain in " + name);
                }
#endif
                table.Add(row.GetId(), row);
            }
            ConfigureManager.confDict[typeStr] = table;
            return true;
            Profiler.EndSample();
        }

        private BaseConf InnerGetConfById<T>(string id) where T : BaseConf, new() {
            BaseConf baseConf = null;
            Dictionary<string, BaseConf> table = null;
            string typeName = typeof(T).Name;
            UnityAction getConfAction = () => {
                if (table.ContainsKey(id)) {
                    baseConf = table[id];
                } else {
#if UNITY_EDITOR
                    Debug.LogWarningf("{0} not conclude in configure {1}", id, typeName);
#endif
                }
            };
            if (ConfigureManager.confDict.TryGetValue(typeof(T).Name, out table)) {
                getConfAction();
            } else {
                T row = new T();
                if (ConfigureManager.confDict.TryGetValue(typeof(T).Name, out table)) {
                    getConfAction();
                } else {
                    Debug.LogError(typeof(T).Name + " not conclude in ConfigurePath");
                }
            }
            return baseConf;
        }

        public static T GetConfById<T>(string id) where T : BaseConf, new() {
            if (id.CustomIsEmpty()) {
                return null;
            }
            BaseConf baseConf = self.InnerGetConfById<T>(id);
            return (T)baseConf;
        }

        public static Dictionary<string, BaseConf> GetConfDict<T>() where T : BaseConf, new() {
            Dictionary<string, BaseConf> baseDict;
            string typeName = typeof(T).Name;
            if (ConfigureManager.confDict.TryGetValue(typeName, out baseDict)) {
                return baseDict;
            } else {
                T row = new T();
                if (ConfigureManager.confDict.TryGetValue(typeName, out baseDict)) {
                    return baseDict;
                } else {
#if UNITY_EDITOR
                    Debug.LogError(typeof(T).Name + " not conclude in ConfigurePath");
                    return null;
#endif
                }
            }
        }
    }
}
