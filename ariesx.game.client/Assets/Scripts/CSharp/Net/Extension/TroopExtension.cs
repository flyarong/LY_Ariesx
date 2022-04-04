using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using Poukoute;

namespace Protocol {
    public partial class MonsterTroop {
        public long GetPower() {
            int power = 0;
            foreach (NpcHero hero in this.Heroes) {
                power += HeroAttributeConf.GetPower(hero.Name, hero.Level);
            }
            return power;
        }

        public int GetCurrentHealth() {
            int armyAmount = 0;
            foreach (NpcHero hero in this.Heroes) {
                armyAmount += hero.ArmyAmount;
            }
            return armyAmount;
        }

        public float GetMaxHealth() {
            float maxArmyAmount = 0;
            foreach (NpcHero hero in this.Heroes) {
                maxArmyAmount += HeroAttributeConf.GetHeroArmyAmount(
                    hero.Name, hero.Level, hero.ArmyAmountBonus
                );
            }
            return maxArmyAmount;
        }
    }

    public partial class Troop {
        
    }
    
}
