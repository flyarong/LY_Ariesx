using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class EditorBattleHeroAnimationConf : BaseConf {
        public string heroId;
        public string animation;
        public string functionName;
        public float time;

        public static Dictionary<string, Dictionary<string, EditorBattleHeroAnimationConf>> actionEventDict =
            new Dictionary<string, Dictionary<string, EditorBattleHeroAnimationConf>>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero"];
            this.animation = attrDict["animation"];
            this.functionName = attrDict["function"];
            this.time = float.Parse(attrDict["time"]);

            Dictionary<string, EditorBattleHeroAnimationConf> functionDict;
            if (!(actionEventDict.TryGetValue(this.heroId + this.animation, out functionDict))) {
                functionDict = new Dictionary<string, EditorBattleHeroAnimationConf>();
                actionEventDict.Add(this.heroId + this.animation, functionDict);
            }
            functionDict.Add(this.functionName, this);
        }

        public override string GetId() {
            return heroId + this.animation + this.functionName;
        }

        static EditorBattleHeroAnimationConf() {
            ConfigureManager.Instance.LoadConfigure<EditorBattleHeroAnimationConf>();

        }

        public static Dictionary<string, EditorBattleHeroAnimationConf>
        GetActionEventDict(string id) {
            if (actionEventDict.ContainsKey(id)) {
                return actionEventDict[id];
            } else {
                return null;
            }
        }

        public static void ClearActionEventDict() {
            actionEventDict.Clear();
        }
    }
}
