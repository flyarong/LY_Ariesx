using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public enum ChestStatus {
        above = 1,
        below,
        open = 4
    }

    public enum HeroSortType {
        Level,
        Rarity,
        Power,
        None
    }

    public enum HeroSubViewType {
        None,
        All,
        Info,
        Lottery
    }

    public class HeroMeta {
        private Hero hero;
        public Hero Hero {
            get {
                return hero;
            }
            set {
                if (this.hero != value) {
                    this.hero = value;
                    this.OnHeroChange();
                }
            }
        }

        public UnityEvent changeEvent;

        private void OnHeroChange() {
            this.changeEvent.Invoke();
        }

        public void AddChangeCallback(UnityAction action) {
            this.changeEvent.AddListener(action);
        }

        public void RemoveChangeCallback(UnityAction action) {
            this.changeEvent.RemoveListener(action);
        }

    }

    public class HeroModel : BaseModel {
        public static HeroModel Instance;
        public Dictionary<string, Hero> heroDict = new Dictionary<string, Hero>(66);
        public List<Hero> sortedHeroList = new List<Hero>(66);

        public List<string> heroNamesList = new List<string>(70);
        public List<string> heroRowList = new List<string>(24);

        public Dictionary<string, HeroAttributeConf> unlockHeroDict =
            new Dictionary<string, HeroAttributeConf>(66);
        public List<HeroAttributeConf> unlockHeroList = new List<HeroAttributeConf>(66);
        public List<Chest> lotteryChanceList = new List<Chest>(3);
        public List<Chest> moneyChestsList = new List<Chest>();
        public List<GachaGroupConf> lotteryList = new List<GachaGroupConf>(12);
        public List<LotteryResult> lotteryResultList = new List<LotteryResult>(2);

        public HeroSortType heroSortType = HeroSortType.Power;
        public HeroSortType lastSortType = HeroSortType.None;
        // Dynamic scroll param
        public int countShow = 8;
        public int tailIndex = 0;
        // End
        public string currentHeroName = string.Empty;
        public HeroAttributeConf heroConf;
        public string currentGroupName = string.Empty;
        public int UnlockHeroIndex = 0;
        public HeroSubViewType viewType = HeroSubViewType.None;
        public int NewHeroCount {
            get {
                return this.newHeroCount;
            }
            set {
                if (value != this.newHeroCount) {
                    if (value > this.newHeroCount) {
                        this.lastSortType = HeroSortType.None;
                        this.OnNewHeroGet();
                    }
                    this.newHeroCount = value > 0 ? value : 0;
                    TriggerManager.Invoke(Trigger.HeroStatusChange);
                }
            }
        }
        public int CanLevelUpCount {
            get {
                return this.canLevelUpCount;
            }
            set {
                if (value != this.canLevelUpCount) {
                    //Debug.LogError("Set LevelUpCount " + this.canLevelUpCount + " " + value);
                    this.canLevelUpCount = value > 0 ? value : 0;
                    TriggerManager.Invoke(Trigger.HeroStatusChange);
                }
            }
        }
        public int FreeLotteryCount {
            get {
                return this.freeLotteryCount;
            }
            set {
                if (value != this.freeLotteryCount) {
                    //Debug.LogError("freeLotteryCount calue Change " + value);
                    this.freeLotteryCount = value > 0 ? value : 0;
                    TriggerManager.Invoke(Trigger.HeroStatusChange);
                    TriggerManager.Invoke(Trigger.OnChestsGet);
                }
            }
        }

        public int MoneyChsetCount {
            get {
                return this.moneyChestCount;
            }
            set {
                if (value != this.moneyChestCount) {
                    this.moneyChestCount = value > 0 ? value : 0; ;
                    TriggerManager.Invoke(Trigger.HeroStatusChange);
                }
            }
        }
        private int canLevelUpCount = 0;
        private int newHeroCount = 0;
        private int freeLotteryCount = 0;
        private int moneyChestCount = 0;

        public HeroModel() {
            Instance = this;
        }

        // Need hero order.
        public void Refresh(GetHeroesAck heroAck) {
            this.heroDict.Clear();
            canLevelUpCount = 0;
            this.heroNamesList.Clear();
            this.newHeroCount = 0;
            foreach (Hero hero in heroAck.Heroes) {
                if (hero.IsNew) {
                    this.newHeroCount++;
                }
                if (HeroLevelConf.GetCanLevelUp(hero)) {
                    this.canLevelUpCount++;
                }
                heroDict.Add(hero.Name, hero);
                this.heroNamesList.Add(hero.Name);
            }

            this.RefreshSortedHeroList();
            this.SetHeroOtherDataInfo();
            TriggerManager.Invoke(Trigger.HeroStatusChange);
        }

        public Hero GetMaxLevelHero() {
            string heroName = string.Empty;
            int minLevel = int.MinValue;
            foreach (var hero in this.heroDict) {
                if (minLevel < hero.Value.Level) {
                    minLevel = hero.Value.Level;
                    heroName = hero.Key;
                }
            }
            return heroName.CustomIsEmpty() ? null : this.heroDict[heroName];
        }

        private void OnNewHeroGet() {
            this.heroNamesList.Clear();
            foreach (Hero hero in this.heroDict.Values) {
                this.heroNamesList.Add(hero.Name);
            }
            this.CompleteList(true);

            if (this.unlockHeroDict.Count > 0) {
                foreach (HeroAttributeConf heroAttr in this.unlockHeroDict.Values) {
                    this.heroNamesList.Add(heroAttr.name);
                }
                this.CompleteList();
            }
            this.SetHeroRowData();
            this.RefreshSortedHeroList();
        }

        private void RefreshSortedHeroList() {
            this.heroSortType = this.lastSortType != HeroSortType.None ?
                this.lastSortType : HeroSortType.Power;
            this.lastSortType = HeroSortType.None;
            this.GetHeroListOrderBy();
        }

        private void SetHeroOtherDataInfo() {
            this.CompleteList(true);
            Dictionary<string, BaseConf> allHeroInfo = ConfigureManager.GetConfDict<HeroAttributeConf>();
            this.unlockHeroDict.Clear();
            this.unlockHeroList.Clear();
            int count = 0;
            foreach (string heroId in HeroBaseConf.HeroList) {
                if (!this.heroDict.ContainsKey(heroId)) {
                    this.unlockHeroDict.Add(heroId, (HeroAttributeConf)allHeroInfo[heroId]);
                    this.heroNamesList.Add(heroId);
                }
                count++;
                if (count >= GameConst.HERO_COUNT) {
                    break;
                }
            }
            if (this.unlockHeroDict.Count > 0) {
                this.unlockHeroList = new List<HeroAttributeConf>(this.unlockHeroDict.Values);
                this.ReOrderUnlockHeroList();
                this.CompleteList();
            }
            this.SetHeroRowData();
        }

        public void ReOrderUnlockHeroList() {
            if (this.unlockHeroDict.Count > 0) {
                this.unlockHeroList.Sort(
                delegate (HeroAttributeConf heroConf1, HeroAttributeConf heroConf2) {
                    int hero1UnlockChest = GachaGroupConf.GetUnlockChestIndex(heroConf1.name);
                    int hero2UnlockChest = GachaGroupConf.GetUnlockChestIndex(heroConf2.name);
                    int sort = hero1UnlockChest.CompareTo(hero2UnlockChest);
                    return sort == 0 ? heroConf1.GetId().CompareTo(heroConf2.GetId()) : sort;
                });
                List<HeroAttributeConf> tmpList =
                    new List<HeroAttributeConf>(this.unlockHeroList.Count);
                int unlockMaxChestLevel = RoleManager.GetFDRecordMaxLevel();
                int level;
                //Debug.LogError("unlockMaxChestLevel " + unlockMaxChestLevel);
                foreach (HeroAttributeConf conf in this.unlockHeroList) {
                    level = GachaGroupConf.GetUnlockChestIndex(conf.name);
                    if (unlockMaxChestLevel >= level) {
                        tmpList.Add(conf);
                    }
                }
                foreach (HeroAttributeConf conf in this.unlockHeroList) {
                    level = GachaGroupConf.GetUnlockChestIndex(conf.name);
                    if (unlockMaxChestLevel < level) {
                        tmpList.Add(conf);
                    }
                }
                this.unlockHeroList = tmpList;
                //foreach (HeroAttributeConf conf in this.unlockHeroList) {
                //    Debug.LogError(conf.name + " " + GachaGroupConf.GetUnlockChestIndex(conf.name));
                //}
            }
        }

        private void SetHeroRowData() {
            this.heroRowList.Clear();
            int total = Mathf.FloorToInt(this.heroNamesList.Count / 3);
            for (int i = 0; i < total; i++) {
                this.heroRowList.Add(string.Empty);
            }
        }

        private void CompleteList(bool needSeparate = false) {
            int heroNamesCount = this.heroNamesList.Count;
            int remain = heroNamesCount % GameConst.HERO_ROW;
            if (remain != 0) {
                int needInsert = GameConst.HERO_ROW - remain;
                for (int i = 0; i < needInsert; i++) {
                    this.heroNamesList.Add(String.Empty);
                }
            }
            if (needSeparate && (this.heroDict.Count <
                ConfigureManager.GetConfDict<HeroAttributeConf>().Count)) {
                for (int i = 0; i < GameConst.HERO_ROW; i++) {
                    this.heroNamesList.Add(String.Empty);
                }
                this.UnlockHeroIndex = this.heroNamesList.Count / GameConst.HERO_ROW;
            }
        }

        #region lottery chance
        public bool HasFreeLottery(string groupName) {
            foreach (Chest chest in this.lotteryChanceList) {
                if (chest.Name == groupName) {
                    return true;
                }
            }
            return false;
        }

        public void Refresh(LoginAck loginAck) {
            this.lotteryChanceList.Clear();
            if (loginAck.Chests != null) {
                Chest chest;
                int chestsCount = loginAck.Chests.Count;
                for (int index = 0; index < chestsCount; index++) {
                    chest = loginAck.Chests[index];
                    if (this.lotteryChanceList.Contains(chest)) {
                        chest.Count++;
                    } else {
                        this.lotteryChanceList.Add(chest);
                    }
                }
                this.SetFreelotteryCount();
            }
        }

        private void SetFreelotteryCount() {
            int count = this.lotteryChanceList.Count;
            int tmpCount = 0;
            for (int i = 0; i < count; i++) {
                tmpCount += this.lotteryChanceList[i].Count;
            }
            this.FreeLotteryCount = tmpCount;
        }

        public static bool IsFreeLotteryChanceContain(string chestName) {
            int count = Instance.lotteryChanceList.Count;
            for (int i = 0; i < count; i++) {
                if (Instance.lotteryChanceList[i].Name.CustomEquals(chestName)) {
                    return true;
                }
            }
            return false;
        }

        public static Chest GetFreeLotteryChance(string chestName) {
            int count = Instance.lotteryChanceList.Count;
            for (int i = 0; i < count; i++) {
                if (Instance.lotteryChanceList[i].Name.CustomEquals(chestName)) {
                    return Instance.lotteryChanceList[i];
                }
            }
            return null;
        }

        public static Chest GetMoneyChests(string chestName) {
            int count = Instance.moneyChestsList.Count;
            for (int i = 0; i < count; i++) {
                if (Instance.moneyChestsList[i].Name.CustomEquals(chestName)) {
                    return Instance.moneyChestsList[i];
                }
            }
            return null;
        }

        public static void AddlotteryChances(List<Chest> newChests) {
            int newChestCount = newChests.Count;
            Chest oldChest;
            for (int i = 0; i < newChestCount; i++) {
                oldChest = HeroModel.GetFreeLotteryChance(newChests[i].Name);
                if (oldChest != null) {
                    oldChest.Count += newChests[i].Count;
                } else {
                    Instance.lotteryChanceList.Add(newChests[i]);
                }
            }
            Instance.FreeLotteryCount += newChestCount;
        }

        public static void AddMoneyChests(List<Chest> newChests) {
            int newChestCount = newChests.Count;
            Chest oldChest;
            for (int i = 0; i < newChestCount; i++) {
                oldChest = HeroModel.GetMoneyChests(newChests[i].Name);
                if (oldChest != null) {
                    oldChest.Count++;
                } else {
                    Instance.moneyChestsList.Add(newChests[i]);
                }
            }
            Instance.MoneyChsetCount += newChestCount;
        }
        #endregion

        private void ResetSortedHerosTroopList() {
            sortedHeroUsTroopList.Clear();
            foreach (Hero hero in this.heroDict.Values) {
                sortedHeroUsTroopList.Add(hero);
            }
        }

        private List<Hero> sortedHeroUsTroopList = new List<Hero>();
        public List<Hero> GetHeroListOrderBySortType(HeroSortType heroSortType) {
            Debug.Log("GetHeroListOrderBySortType");
            ResetSortedHerosTroopList();
            switch (heroSortType) {
                case HeroSortType.Level:
                    sortedHeroUsTroopList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            int level = heroB.Level.CompareTo(heroA.Level);
                            return level == 0 ? heroB.Id.CompareTo(heroA.Id) : level;
                        }
                    );
                    break;
                case HeroSortType.Rarity:
                    sortedHeroUsTroopList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            HeroAttributeConf heroConf1 = HeroAttributeConf.GetConf(heroA.GetId());
                            HeroAttributeConf heroConf2 = HeroAttributeConf.GetConf(heroB.GetId());
                            int rarity = heroConf2.rarity.CompareTo(heroConf1.rarity);
                            return rarity == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : rarity;
                        }
                    );
                    break;
                case HeroSortType.Power:
                    sortedHeroUsTroopList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            HeroAttributeConf heroConf1 = HeroAttributeConf.GetConf(heroA.GetId());
                            HeroAttributeConf heroConf2 = HeroAttributeConf.GetConf(heroB.GetId());
                            int power = heroConf2.GetPower(heroB.Level).CompareTo(heroConf1.GetPower(heroA.Level));
                            return power == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : power;
                        }
                    );
                    break;
                case HeroSortType.None:
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
            return sortedHeroUsTroopList;
        }

        private void ResetSortedHeroList() {
            sortedHeroList.Clear();
            foreach (Hero hero in this.heroDict.Values) {
                sortedHeroList.Add(hero);
            }
        }

        private void ResetUnlockHeroList() {
            unlockHeroList.Clear();
            foreach (HeroAttributeConf heroAttr in this.unlockHeroDict.Values) {
                unlockHeroList.Add(heroAttr);
            }
        }

        public List<Hero> GetHeroListOrderBy() {
            if (this.lastSortType == heroSortType) {
                return this.sortedHeroList;
            }
            //Debug.LogError("GetHeroListOrderBy " + this.heroSortType + " " + this.lastSortType);
            this.lastSortType = this.heroSortType;

            this.ResetSortedHeroList();
            this.ResetUnlockHeroList();
            switch (this.heroSortType) {
                case HeroSortType.Level:
                    sortedHeroList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            int level = heroB.Level.CompareTo(heroA.Level);
                            return level == 0 ? heroB.Name.CompareTo(heroA.Name) : level;
                        }
                    );
                    this.ReOrderUnlockHeroList();
                    break;
                case HeroSortType.Rarity:
                    sortedHeroList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            HeroAttributeConf heroConf1 = HeroAttributeConf.GetConf(heroA.GetId());
                            HeroAttributeConf heroConf2 = HeroAttributeConf.GetConf(heroB.GetId());
                            int rarity = heroConf2.rarity.CompareTo(heroConf1.rarity);
                            return rarity == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : rarity;
                        }
                    );
                    unlockHeroList.Sort(
                        delegate (HeroAttributeConf heroConf1, HeroAttributeConf heroConf2) {
                            int rarity = heroConf2.rarity.CompareTo(heroConf1.rarity);
                            return rarity == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : rarity;
                        }
                    );
                    break;
                case HeroSortType.Power:
                    sortedHeroList.Sort(
                        delegate (Hero heroA, Hero heroB) {
                            HeroAttributeConf heroConf1 = HeroAttributeConf.GetConf(heroA.GetId());
                            HeroAttributeConf heroConf2 = HeroAttributeConf.GetConf(heroB.GetId());
                            int power = heroConf2.GetPower(heroB.Level).CompareTo(heroConf1.GetPower(heroA.Level));
                            return power == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : power;
                        }
                    );
                    unlockHeroList.Sort(
                        delegate (HeroAttributeConf heroConf1, HeroAttributeConf heroConf2) {
                            int power = heroConf2.GetPower(1).CompareTo(heroConf1.GetPower(1));
                            return power == 0 ? heroConf2.GetId().CompareTo(heroConf1.GetId()) : power;
                        }
                    );
                    break;
                case HeroSortType.None:
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
            //  Debug.LogError(sortedHeroList.Count);
            return sortedHeroList;
        }
    }


}