using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class FteStepConf : BaseConf {
        public string index;
        public string next;
        public string previouse;
        public List<string> lockedUI;
        public List<string> unlockUI;
        // Need a is force label.
        public static Dictionary<string, FteStepConf> fteStepDict =
            new Dictionary<string, FteStepConf>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.index = attrDict["index"];
            this.next = attrDict["next"];
            this.previouse = attrDict["previouse"];

            this.lockedUI = new List<string>(attrDict["unlock_ui"].CustomSplit(','));

            if (!fteStepDict.ContainsKey(index)) {
                fteStepDict.Add(index, this);
            }
        }

        public override string GetId() {
            return this.index.ToString();
        }

        static FteStepConf() {
            ConfigureManager.Instance.LoadConfigure<FteStepConf>();
        }

        public static FteStepConf GetConf(string id) {
            return ConfigureManager.GetConfById<FteStepConf>(id);
        }

        public static void AfterRead() {
            foreach (var pair in fteStepDict) {
                if (!pair.Value.previouse.CustomEquals("0")) {
                    FteStepConf prevStep = fteStepDict[pair.Value.previouse];
                    int length = prevStep.lockedUI.Count - pair.Value.lockedUI.Count;
                    string[] unlockUI = new string[length];
                    prevStep.lockedUI.CopyTo(
                        pair.Value.lockedUI.Count,
                        unlockUI, 0, length);
                    pair.Value.unlockUI = new List<string>(unlockUI);
                } else {
                    pair.Value.unlockUI = new List<string>();
                }
            }
        }

        public static void BeforeRead() {
            fteStepDict.Clear();
        }

        public FteStepConf GetNextStep() {
            return ConfigureManager.GetConfById<FteStepConf>(this.next);
        }

        public FteStepConf GetPreviouseStep() {
            return ConfigureManager.GetConfById<FteStepConf>(this.previouse);
        }
    }
}