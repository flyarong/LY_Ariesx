using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class MarchModel : BaseModel {
        public EventMarchClient march;
        public Troop troop;
        public Dictionary<string, HeroPosition> formation = 
            new Dictionary<string, HeroPosition>();
        public Dictionary<string, Hero> heroDict;

        public MarchModel() {
            this.heroDict = ModelManager.GetModelData<HeroModel>().heroDict;
        }

        public void ParseFormation() {
            if (troop != null) {
                formation.Clear();
                foreach (HeroPosition heroPosition in this.troop.Positions) {
                    Hero hero = this.heroDict[heroPosition.Name];
                    formation.Add(heroPosition.Name, heroPosition);
                }
            }
        }

        public string GetHeroKey(string name) {
            Hero hero;
            if (this.heroDict.TryGetValue(name, out hero)) {
                return string.Concat(name, hero.Id);
            } else {
                return null;
            }

            //if (this.heroDict.ContainsKey(name)) {
            //    //return name + this.heroDict[name].Tier;
            //    return name + this.heroDict[name].Id;
            //} else {
            //    return null;
            //}
        }
    }
}
