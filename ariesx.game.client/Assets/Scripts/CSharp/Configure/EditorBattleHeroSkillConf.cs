using System.Collections.Generic;

namespace Poukoute {
    public class EditorBattleHeroSkillConf : BaseConf {
        public string index;
        public string stepStr;
        public float endDelay;
        public BattleEffectStepType stepType;
        public string heroId;
        public BattleEffectTargetType target;

        private static Dictionary<string, Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>>> heroSkillDict =
           new Dictionary<string, Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>>>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero"];
            this.index = attrDict["index"];
            this.stepStr = attrDict["step"];
            this.stepType = (BattleEffectStepType)System.Enum.Parse(
                typeof(BattleEffectStepType),
                GameHelper.UpperFirstCase(this.stepStr.ToLower())
            );
            this.endDelay = float.Parse(attrDict["end_delay"]);
            this.target = (BattleEffectTargetType)System.Enum.Parse(
                typeof(BattleEffectTargetType),
                GameHelper.UpperFirstCase(attrDict["target"].ToLower())
            );

            Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>> effectDict;
            if (!(heroSkillDict.TryGetValue(this.heroId, out effectDict))) {
                effectDict = new Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>>();
                heroSkillDict.Add(this.heroId, effectDict);
            }
            Dictionary<string, EditorBattleHeroSkillConf> stepDict;
            if (!effectDict.TryGetValue(this.index, out stepDict)) {
                stepDict = new Dictionary<string, EditorBattleHeroSkillConf>();
                effectDict.Add(this.index, stepDict);
            }
            stepDict[this.stepStr] = this;

        }

        public override string GetId() {
            return string.Concat(this.heroId, "_", this.index + "_" + this.stepStr);
        }

        static EditorBattleHeroSkillConf() {
            ConfigureManager.Instance.LoadConfigure<EditorBattleHeroSkillConf>();

        }

        public static DemonTroopConf GetConf(string id) {
            return ConfigureManager.GetConfById<DemonTroopConf>(id);
        }

        public static void ClearHeroSkillDict() {
            heroSkillDict.Clear();
        }

        public static Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>>
        GetEffectDict(string heroId) {
            if (heroSkillDict.ContainsKey(heroId)) {
                return heroSkillDict[heroId];
            } else {
                return null;
            }
        }
    }
}
