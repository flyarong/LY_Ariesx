using UnityEngine.Events;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class OpenChestViewModel : BaseViewModel, IViewModel {
        private HeroViewModel parent;
        private HeroModel model;
        private OpenChestView view;
        /* Model data get set */
        public List<LotteryResult> LotteryResultList {
            get {
                return this.model.lotteryResultList;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.model.heroDict;
            }
        }

        public Dictionary<string, HeroAttributeConf> UnlockHeroDict {
            get {
                return this.model.unlockHeroDict;
            }
        }

        private LotteryResult heroResult;
        public LotteryResult CurrentResult {
            get {
                return this.heroResult;
            }
            set {
                if (this.heroResult != value) {
                    this.heroResult = value;
                    this.heroConf = HeroAttributeConf.GetConf(value.Name);
                }
            }
        }

        private HeroAttributeConf heroConf;
        public HeroAttributeConf HeroConf {
            get {
                return this.heroConf;
            }
        }

        public string CurrentGroupName {
            get {
                return this.model.currentGroupName;
            }
        }

        public List<Chest> LotteryChanceList {
            get {
                return this.model.lotteryChanceList;
            }
        }

        public int CanLevelUpCount {
            get {
                return this.parent.CanLevelUpCount;
            }
            set {
                this.parent.CanLevelUpCount = value;
            }
        }

        public int NewHeroCount {
            get {
                return this.parent.NewHeroCount;
            }
            set {
                this.parent.NewHeroCount = value;
            }
        }

        public int FreeLotteryCount {
            get {
                return this.parent.FreeLotteryCount;
            }
            set {
                this.parent.FreeLotteryCount = value;
            }
        }

        private int LotteryChanceCount {
            get {
                Chest chest = HeroModel.GetFreeLotteryChance(CurrentGroupName);
                if (chest != null) {
                    return chest.Count;
                } else {
                    return 0;
                }
            }
        }

        public bool IsNewInPool { get; set; }
        public bool isHideViewForBattle = false;
        /*********************************************************************************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<HeroViewModel>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<OpenChestView>();

            TriggerManager.Regist(Trigger.PlayBattleReportStart, this.PlayBattleReportStart);
            TriggerManager.Regist(Trigger.PlayBattleReportDone, this.PlayBattleReportDone);
            FteManager.SetStartCallback(GameConst.NORMAL, 41, this.OnFteStep41Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 41, this.OnFteStep41End);
            FteManager.SetStartCallback(GameConst.NORMAL, 112, this.OnFteStep112Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 124, this.OnFteStep124Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 128, this.OnFteStep128Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 128, this.OnFteStep128End);
        }

        // To do: feature-optimize.
        public void Show(bool isBuy = false) {
            this.view.Show();
            if (this.LotteryChanceCount > 0 && !isBuy) {
                this.OpenCurrentChest();
            } else {
                this.view.ShowStart();
            }

            if (this.isHideViewForBattle) {
                this.view.SetUIVisibleForChest(false);
            }
        }


        public void Show(List<LotteryResult> result, UnityAction callback) {
            this.view.Show();
            this.view.afterHideCallback = callback;
            this.model.currentGroupName = "silver_chest_2";
            this.LotteryResultList.Clear();
            this.LotteryResultList.AddRange(result);
            this.CurrentResult = result[0];
            this.view.ShowStart();
            TriggerManager.Invoke(Trigger.HeroStatusChange);
        }


        public void Hide() {
            this.view.Hide(() => {
                this.parent.OpeningChest = false;
            });
        }

        public void HideImmediatly() {
            this.parent.OpeningChest = false;
            this.view.HideImmediatly(null);
        }

        public void HideOpenChest() {
            this.Hide();
        }

        private void OpenCurrentChest() {
            OpenChestReq req = new OpenChestReq() {
                Name = this.CurrentGroupName
            };
            NetManager.SendMessage(req, typeof(OpenChestAck).Name,
                this.OpenChestAck, (message) => {
                    this.Hide();
                }, () => {
                    this.Hide();
                });
        }

        public void Next(bool isFirst) {
            if (isFirst) {
                this.CurrentResult = LotteryResultList[0];
                this.view.ShowNextHero();
                this.LotteryResultList.RemoveAt(0);
            } else {
                StartCoroutine(this.view.ShowHide());
            }
        }

        private void OpenChestAck(IExtensible message) {
            OpenChestAck ack = message as OpenChestAck;
            Chest chest = HeroModel.GetFreeLotteryChance(this.CurrentGroupName);
            if (chest != null) {
                if (chest.Count == 1) {
                    this.LotteryChanceList.Remove(chest);
                } else {
                    chest.Count--;
                }
                this.FreeLotteryCount--;
            }

            this.LotteryResultList.Clear();
            int ackCount = ack.Results.Count;
            for (int i = 0; i < ackCount; i++) {
                this.LotteryResultList.Add(ack.Results[i]);
            }
            this.CurrentResult = ack.Results[0];
            this.view.ShowStart();
        }

        public void ShowHeroPoolView() {
            this.parent.ShowHeroPool(this.CurrentGroupName);
        }

        public void AddFramgent(string name, int add) {
            Hero hero;
            if (add > 0 && this.HeroDict.TryGetValue(name, out hero)) {
                //hero = this.HeroDict[name];
                int heroFragments = HeroLevelConf.GetHeroUpgradFragments(hero);
                if (hero.FragmentCount < heroFragments &&
                    hero.FragmentCount + add >= heroFragments) {
                    this.CanLevelUpCount++;
                }
                this.HeroDict[name].FragmentCount += add;
            }
        }

        public void ShowNewHero(LotteryResult lotteryResult,
            string groupName, UnityAction callback, bool isForceFte) {
            this.LotteryResultList.Clear();
            this.LotteryResultList.Add(lotteryResult);
            this.model.currentGroupName = groupName;
            this.view.OnFteStepOpenStart(callback, isForceFte);
        }

        private void PlayBattleReportStart() {
            if (this.view.IsVisible) {
                this.isHideViewForBattle = true;
                this.view.SetUIVisibleForChest(false);
            }
        }

        private void PlayBattleReportDone() {
            if (this.isHideViewForBattle) {
                if (this.view.IsVisible) {
                    this.view.SetUIVisibleForChest(true);
                }
                this.isHideViewForBattle = false;
            }
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        #region FTE
        private void OnFteStep41Start(string index) {
            List<Protocol.Skill> skillList = this.HeroDict["hero_405"].Skills;
            this.HeroDict.Remove("hero_405");
            LotteryResult lotteryResult = new LotteryResult {
                Name = "hero_405",
                FragmentCount = 0
            };
            foreach (Protocol.Skill skill in skillList) {
                lotteryResult.Skills.Add(skill);
            }
            this.ShowNewHero(lotteryResult, "wooden_chest_1", null, true);
        }

        private void OnFteStep41End() {
            this.model.heroDict["hero_405"].IsNew = false;
            this.model.NewHeroCount--;
            this.view.SetHeroPoolAcvive();
            this.Hide();
        }

        private void OnFteStep112Start(string index) {
            this.model.currentGroupName = FteManager.GetCurrentLotteryGroup();
            this.view.OnFteStepOpenStart(null, true);
        }

        private void OnFteStep124Start(string index) {
            FteManager.EndFte(true);
        }

        private void OnFteStep128Start(string index) {
            this.view.OnFteStep128Start();
        }

        private void OnFteStep128End() {
            this.Hide();
        }

        #endregion
    }
}