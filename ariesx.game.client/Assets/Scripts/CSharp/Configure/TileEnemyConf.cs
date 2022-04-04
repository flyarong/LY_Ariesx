using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class TileEnemyConf : BaseConf {
        public string id;

        public string minLevel;
        public string maxLevel;
        public int minArmyAmount;
        public int maxArmyAmount;

        private string minLevelLocal;
        private string maxLevelLocal;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["land"];
            this.minLevel = attrDict["monster_level_1"];
            this.maxLevel = attrDict["monster_level_2"];
            this.minArmyAmount = int.Parse(attrDict["monster_amount_1"]);
            this.maxArmyAmount = int.Parse(attrDict["monster_amount_2"]);

            string levelLocal = LocalManager.GetValue(LocalHashConst.level);
            this.minLevelLocal = string.Format(levelLocal, minLevel);
            this.maxLevelLocal = string.Format(levelLocal, maxLevel);
        }

        public override string GetId() {
            return this.id;
        }

        static TileEnemyConf() {
            ConfigureManager.Instance.LoadConfigure<TileEnemyConf>();
        }

        public string GetLevelInfo() {
            string level = string.Empty;
            //string levelLocal = LocalManager.GetValue(LocalHashConst.level);
            if (minLevel == maxLevel) {
                level = this.minLevelLocal;
            } else {
                level = string.Format("{0} - {1}", this.minLevelLocal, this.maxLevelLocal);
            }
            //return string.Format(LocalManager.GetValue("hero_level"), level);
            return level;
        }

        public static TileEnemyConf GetConf(string id) {
            return ConfigureManager.GetConfById<TileEnemyConf>(id);
        }
    }
}
