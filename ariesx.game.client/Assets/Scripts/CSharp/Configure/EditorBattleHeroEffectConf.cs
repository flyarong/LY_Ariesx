using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class EditorBattleHeroEffectConf : BaseConf {
        public BattleEffectStepType stepType;
        public string heroId;

        public override void SetProperty(Dictionary<string, string> attrDict) { }
        public override string GetId() { return null; }
    }
}