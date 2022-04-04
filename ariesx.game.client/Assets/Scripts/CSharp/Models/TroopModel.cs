using UnityEngine;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public enum TroopViewType {
        Idle,
        Move,
        Attack,
        Format,
        Raid,
        Return = 100
    }

    public enum MarchType {
        Move = 1,
        Attack,
        FallBack,
        Return,
        Raid,
        MonsterAttack,
        BossAttack
    }

    public enum BattleSimulationResult {
        Easy,
        Normal,
        Hard,
        HardWeak,
        NoTroop,
        None
    }

    public class TroopAttributes {
        public long army;
        public long maxArmy;
        public float speed;
        public float siege;
        public int energy;
    }

    public class MarchAttributes: TroopAttributes {
        public long timeCost;
        public long timeArrive;
    }

    public class TroopModel: BaseModel {
        public Dictionary<string, Troop> troopDict = new Dictionary<string, Troop>();
        public string currentTroop = string.Empty;
        public MarchAttributes currentMarchAttributes = new MarchAttributes();
        public string currentHero;
        public int currentPosition;
        public Vector2 target;
        public TroopViewType viewType;
        public DailyLimit dailyLimit;

        public Dictionary<string, HeroPosition> formation =
            new Dictionary<string, HeroPosition>();
        public Dictionary<string, Hero> heroDict;

        private BuildModel buildModel;


        public TroopModel() {
            this.heroDict = ModelManager.GetModelData<HeroModel>().heroDict;
            this.buildModel = ModelManager.GetModelData<BuildModel>();
        }

        public void Refresh(LoginAck loginAck) {
            this.troopDict.Clear();
            foreach (Troop troop in loginAck.Troops) {
                this.troopDict.Add(troop.Id, troop);
            }
        }

        public void ParseFormation() {
            if (!currentTroop.CustomIsEmpty()) {
                formation.Clear();
                foreach (HeroPosition heroPosition in this.troopDict[currentTroop].Positions) {
                    //Hero hero = this.heroDict[heroPosition.Name];
                    formation.Add(heroPosition.Name, heroPosition);
                }
            }
        }

        public bool IsInFormation(string name) {
            return this.formation.ContainsKey(name);
        }

        public void AddToFormation(string name, int position) {
            HeroPosition heroPosition = new HeroPosition();
            heroPosition.Name = name;
            heroPosition.Position = position;
            this.formation.Add(name, heroPosition);
        }

        public void RemoveFromFormation(string name) {
            this.formation.Remove(name);
        }

        public Troop GetTroopWithHeroName(string heroName) {
            Hero hero;
            if (this.heroDict.TryGetValue(heroName, out hero) &&
                hero.OnTroop) {
                int positionCount;
                foreach (Troop troop in this.troopDict.Values) {
                    positionCount = troop.Positions.Count;
                    for (int i = 0; i < positionCount; i++) {
                        if (troop.Positions[i].Name.CustomEquals(heroName)) {
                            return troop;
                        }
                    }
                }
            }
            return null;
        }

        public string GetHeroTroopName(string heroName) {
            Troop troop = this.GetTroopWithHeroName(heroName);
            if (troop != null) {
                return troop.ArmyCamp;
            }
            return string.Empty;
        }

        public bool IsAllTroopOutSide() {
            if (this.troopDict.Count > 0) {
                ElementType buildType = ElementType.none;
                foreach (Troop troop in this.troopDict.Values) {
                    buildType = this.buildModel.GetBuildTypeWithCoord(troop.Coord);
                    if (buildType == ElementType.townhall ||
                        buildType == ElementType.stronghold) {
                        //Debug.LogError(troop.Marched + " " + buildType + " " + 
                        //    troop.Coord.X + " " + troop.Coord.Y);
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsAllTroopStayInCoord(Coord troopCoord) {
            if (this.troopDict.Count > 0) {
                foreach (Troop troop in this.troopDict.Values) {
                    if (troop.Coord != troopCoord) {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsOutsideTroopNeedCure(Coord troopCoord) {
            if (this.troopDict.Count > 0) {
                ElementType buildType = ElementType.none;
                foreach (Troop troop in this.troopDict.Values) {
                    if (troop.Coord != troopCoord) {
                        buildType = this.buildModel.GetBuildTypeWithCoord(troop.Coord);
                        if (this.IsTroopNeedCure(troop) &&
                            buildType != ElementType.townhall &&
                            buildType != ElementType.stronghold && troop.Idle) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsNeedShowReturnTips(Vector2 tileCoord) {
            List<Troop> tileTroops = this.GetTroopsAt(tileCoord);
            int tileTroopsCount = tileTroops.Count;
            if (tileTroopsCount > 0) {
                //TroopAttributes troopAttributes;
                Troop troop;
                for (int i = 0; i < tileTroopsCount; i++) {
                    troop = tileTroops[i];
                    //troopAttributes = this.GetTroopAttributes(troop.Id);
                    if (this.IsTroopNeedCure(troop) && troop.Idle) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsNeedShowBuildCure() {
            //Debug.LogError("troopDict.Count " + this.troopDict.Count);
            if (this.troopDict.Count > 0) {
                foreach (Troop troop in this.troopDict.Values) {
                    if (this.IsTroopNeedCure(troop)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetNeedCureTroopId() {
            if (this.troopDict.Count > 0) {
                foreach (Troop troop in this.troopDict.Values) {
                    if (this.IsTroopNeedCure(troop)) {
                        return troop.Id;
                    }
                }
            }
            return string.Empty;

        }

        public string GetNeedCureHeroNameInTroop(string troopId) {
            Dictionary<string, Hero> heroDict = ModelManager.GetModelData<HeroModel>().heroDict;
            Troop currentTroop = this.troopDict[troopId];
            HeroPosition heroPosition;
            Hero hero;
            int troopHeroCount = currentTroop.Positions.Count;
            int maxArmy;
            for (int i = 0; i < troopHeroCount; i++) {
                heroPosition = currentTroop.Positions[i];
                hero = heroDict[heroPosition.Name];
                maxArmy = HeroAttributeConf.GetHeroArmyAmount(hero.Name, hero.Level, hero.armyCoeff);
                if (hero.ArmyAmount < maxArmy * GameConst.TROOP_SHOW_CURE) {
                    return HeroAttributeConf.GetLocalName(heroPosition.Name);
                }
            }

            return string.Empty;
        }

        private bool IsTroopNeedCure(Troop troop) {
            TroopAttributes troopAttr = this.GetTroopAttributes(troop.Id);
            return (troopAttr.army < troopAttr.maxArmy * GameConst.TROOP_SHOW_CURE);
        }

        private bool IsTroopNoBlood(Troop troop) {
            TroopAttributes troopAttr = this.GetTroopAttributes(troop.Id);
            return (troopAttr.army < 1);
        }

        public MarchAttributes GetMarchAttributes(string troopId) {
            this.GetTroopAttributes(troopId);
            Troop troop = this.troopDict[troopId];
            float distance = Vector2.Distance(
              new Vector2(troop.Coord.X, troop.Coord.Y),
              this.target
            );
            float speed = this.currentMarchAttributes.speed;
            long timeCost = (long)(HeroAttributeConf.GetMarchTime(speed, distance) * 1000);
            long timeArrive = RoleManager.GetCurrentUtcTime() + timeCost;
            this.currentMarchAttributes.timeCost = timeCost;
            this.currentMarchAttributes.timeArrive = timeArrive;
            return this.currentMarchAttributes;
        }

        public TroopAttributes GetTroopAttributes(string troopId) {
            Troop troop = this.troopDict[troopId];
            int army = 0;
            float speed = Mathf.Infinity;
            float siege = 0;
            int energy = 0;
            int lessEnergy = GameConst.HERO_ENERGY_MAX;
            float maxArmy = 0;
            foreach (HeroPosition heroPosition in troop.Positions) {
                Hero hero = this.heroDict[heroPosition.Name];
                HeroAttributeConf conf = HeroAttributeConf.GetConf(hero.GetId());
                maxArmy += conf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount);
                army += hero.ArmyAmount;
                speed = Mathf.Min(conf.GetAttribute(hero.Level, HeroAttribute.Speed), speed);
                siege += conf.GetAttribute(hero.Level, HeroAttribute.Siege);
                energy = hero.GetNewEnergy();
                energy = energy < GameConst.HERO_ENERGY_MAX ? energy : GameConst.HERO_ENERGY_MAX;
                lessEnergy = lessEnergy < energy ? lessEnergy : energy;
            }
            this.currentMarchAttributes.army = army;
            this.currentMarchAttributes.speed = speed;
            this.currentMarchAttributes.siege = siege;
            this.currentMarchAttributes.energy = lessEnergy;
            this.currentMarchAttributes.maxArmy = (long)maxArmy;
            return this.currentMarchAttributes;
        }

        public int GetTroopArmyAmount(string troopId) {
            Troop troop = this.troopDict[troopId];
            int army = 0;
            foreach (HeroPosition heroPosition in troop.Positions) {
                Hero hero = this.heroDict[heroPosition.Name];
                army += hero.ArmyAmount;
            }
            return army;
        }

        public List<Troop> GetTroopsAt(Vector2 coordinate) {
            List<Troop> troopList = new List<Troop>();

            foreach (Troop troop in this.troopDict.Values) {
                Vector2 troopPos = new Vector2(troop.Coord.X, troop.Coord.Y);
                if (troopPos == coordinate) {
                    troopList.Add(troop);
                }
            }
            troopList.Sort((a, b) => {
                return a.ArmyCamp.CompareTo(b.ArmyCamp);
            });
            return troopList;
        }

        public TroopStatus GetTroopStatus(Troop troop) {
            if (troop == null) {
                return TroopStatus.None;
            }
            BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
            var armyCampLevel = buildModel.GetBuildLevelByName(troop.ArmyCamp);
            if ((armyCampLevel <= 0) || (troop.Positions.Count == 0)) {
                return TroopStatus.Unconfiged;
            }

            ArmyCampConf armyCampConf =
                ArmyCampConf.GetConf(armyCampLevel.ToString());
            if ((troop.Positions.Count < armyCampConf.heroAmount) && (troop.Positions.Count > 0)) {
                return TroopStatus.HeroNotFull;
            }

            if (this.GetTroopEnegy(troop) < GameConst.HERO_ENERGY_COST) {
                return TroopStatus.Fatigue;
            }

            if (this.IsTroopNeedCure(troop)) {
                return TroopStatus.NeedCure;
            }

            if (this.IsTroopNoBlood(troop)) {
                return TroopStatus.NoBlood;
            }

            return TroopStatus.Idle;
        }

        public int GetTroopEnegy(Troop troop) {
            TroopAttributes troopAttributes = this.GetTroopAttributes(troop.Id);
            return troopAttributes.energy;
        }

        public Troop GetTroopByArmyCampName(string armyCamp) {
            foreach (Troop troop in this.troopDict.Values) {
                if (armyCamp.CustomEquals(troop.ArmyCamp)) {
                    return troop;
                }
            }
            return null;
        }

        public Troop GetTroopByTroopId(string troopId) {
            foreach (Troop troop in this.troopDict.Values) {
                if (troopId.CustomEquals(troop.Id)) {
                    return troop;
                }
            }
            return null;
        }

        // Need add more condition.
        public List<Troop> GetAvaliableTroop(Vector2 coordinate) {
            List<Troop> troopList = new List<Troop>();
            foreach (Troop troop in this.troopDict.Values) {
                Vector2 troopCoord = new Vector2(troop.Coord.X, troop.Coord.Y);
                if (troop.Positions.Count > 0 && troopCoord != coordinate && troop.Idle) {
                    troopList.Add(troop);
                }
            }
            troopList.Sort((a, b) => {
                return a.ArmyCamp.CompareTo(b.ArmyCamp);
            });
            return troopList;
        }

        public bool HasAvaliableTroop() {
            foreach (Troop troop in this.troopDict.Values) {
                TroopStatus troopStatus = this.GetTroopStatus(troop);
                if (troop.Positions.Count > 0 && troop.Idle &&
                    troopStatus != TroopStatus.Fatigue
                    && troopStatus != TroopStatus.NoBlood) {
                    return true;
                }
            }
            return false;
        }

        private bool IsTroopRecruiting(Troop troop) {
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            foreach (HeroPosition heroPosition in troop.Positions) {
                if (heroModel.heroDict[heroPosition.Name].IsRecruiting) {
                    return true;
                }
            }
            return false;
        }

        public static void TroopPositionReSort(Troop troop) {
            troop.Positions.Sort((a, b) => { return a.Position.CompareTo(b.Position); });
        }

        public static string GetArmyCampLocal(string key) {
            return string.Concat(LocalManager.GetValue(LocalHashConst.name_armycamp),
                GameHelper.GetBuildIndex(key));
        }

        public static string GetProduceLocal(string name, string type) {
            return string.Concat(LocalManager.GetValue("name_" + type),
                GameHelper.GetBuildIndex(name));
        }

        public static string GetTributeLocal(string name, string type) {
            return string.Concat(LocalManager.GetValue("name_" + type),
                GameHelper.GetBuildIndex(name));
        }

        public static string GetTroopName(string key) {
            return string.Format(LocalManager.GetValue(LocalHashConst.troopname_troop),
                GameHelper.GetBuildIndex(key));
        }

        public static bool CheckTroopIsRecruiting(Troop troop) {
            return EventManager.IsTroopUnderTreatment(troop.Id);
        }

        public bool IsHeroInCurrentTroop(string heroName) {
            return this.currentTroop == null ?
                this.GetHeroTroopName(heroName) == this.troopDict[this.currentTroop].ArmyCamp :
                false;
        }

        public void UpdateFormation(ExchangeTroopHeroAck exchangeTroopHeroAck) {
            foreach (var pair in exchangeTroopHeroAck.Troops) {
                if (this.troopDict.ContainsKey(pair.Id)) {
                    this.troopDict[pair.Id] = pair;
                }
            }
            ParseFormation();
        }
    }
}
