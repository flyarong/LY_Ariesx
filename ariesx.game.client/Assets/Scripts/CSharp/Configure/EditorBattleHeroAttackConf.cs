using System.Collections.Generic;

namespace Poukoute {
    public class EditorBattleHeroAttackConf : EditorBattleHeroEffectConf {
        public string stepStr;
        public float endDelay;
        public BattleEffectTargetType target;

        private static Dictionary<string, Dictionary<string, EditorBattleHeroAttackConf>> heroAttackDict =
           new Dictionary<string, Dictionary<string, EditorBattleHeroAttackConf>>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero"];
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

            Dictionary<string, EditorBattleHeroAttackConf> stepDict;
            if (!heroAttackDict.TryGetValue(this.heroId, out stepDict)) {
                stepDict = new Dictionary<string, EditorBattleHeroAttackConf>();
                heroAttackDict.Add(this.heroId, stepDict);
            }
            stepDict.Add(this.stepStr, this);

        }

        public override string GetId() {
            return string.Concat(this.heroId, "_", this.stepStr);
        }

        public static DemonTroopConf GetConf(string id) {
            return ConfigureManager.GetConfById<DemonTroopConf>(id);
        }

        public static void ClearHeroAttackDict() {
            heroAttackDict.Clear();
        }

        public static Dictionary<string, EditorBattleHeroAttackConf>
        GetAttackDict(string heroId) {
            if (heroAttackDict.ContainsKey(heroId)) {
                return heroAttackDict[heroId];
            } else {
                return null;
            }
        }
    }
}
