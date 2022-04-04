using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class BattleSimulatorModel : BaseModel {
        /* Add data member in this */
        public List<string> heroList = new List<string>();
        public List<string> actionList = new List<string>();
        public List<string> skillList = new List<string>();
        /***************************/


        public void Refresh(object message) {
#if UNITY_EDITOR
            /* Refresh your data in this function */
            foreach (string model in HeroBattleConf.heroDict.Keys) {
                this.heroList.Add(model);
            }
            this.heroList.Sort((a1, a2) => {
                return a1.CompareTo(a2);
            });
            for (int i = 1; i <= 48; i++) {
                this.skillList.Add(string.Format("effect_skill_hero_{0}_1", i));
            }

            actionList.Add("Attack");
            actionList.Add("Skill");
#endif
        }
    }
}
