using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class HeroRarityConf : BaseConf {
        public int rarity;
        public int attack;
        public int hp;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.rarity = int.Parse(attrDict["rarity"]);
            this.attack = int.Parse(attrDict["attack"]);
            this.hp = int.Parse(attrDict["hp"]);
        }

        public override string GetId() {
            return this.rarity.ToString();
        }

        static HeroRarityConf() {
            ConfigureManager.Instance.LoadConfigure<HeroRarityConf>();
        }
    }
}
