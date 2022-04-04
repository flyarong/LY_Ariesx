using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class HouseKeeperDailyViewModel : BaseViewModel {
        private HouseKeeperDailyView view;
        private HouseKeeperViewModel parent;
        private HouseKeeperDailyModel model;
        public List<GameObject> itemList = new List<GameObject>();
        private HeroModel heroModel;
        private TroopModel troopModel;
        private DailyTaskModel taskModel;
        public Dictionary<Hero, string> heroList = new Dictionary<Hero, string>();
        public int TreasureMapAmount {
            get {
                return this.model.treasureMapAmount;
            }
            set {
                this.model.treasureMapAmount = value;
            }
        }

        public int CanLevelUpCount {
            get {
                return this.heroModel.CanLevelUpCount;
            }
        }

        public DailyLimit DailyLimit {
            get {
                return this.troopModel.dailyLimit;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<HouseKeeperViewModel>();
            this.view = this.gameObject.AddComponent<HouseKeeperDailyView>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.taskModel = ModelManager.GetModelData<DailyTaskModel>();
            this.model = ModelManager.GetModelData<HouseKeeperDailyModel>();

            this.DailyLimitReq();

            this.AddNtfHanders();
        }

        public void Show() {
            this.view.Show(callback: this.view.Format);
        }

        public void Hide(bool needRefresh = false) {
            this.view.Hide();
        }

        public void ShowUnlockConfirm() {
            this.parent.ShowUnlockConfirm();
        }

        public void SetHighlightFrame() {
            this.view.SetHighlightFrame();
        }

        private void AddNtfHanders() {
            NetHandler.AddNtfHandler(typeof(EventBuildNtf).Name, this.EventBuildNtf);
            NetHandler.AddNtfHandler(typeof(ForceNtf).Name, this.ForceNtf);
            NetHandler.AddNtfHandler(typeof(PointLimitNtf).Name, this.PointNtf);
            NetHandler.AddNtfHandler(typeof(PlayerPointNtf).Name, this.PointNtf);
            NetHandler.AddNtfHandler(typeof(HeroesNtf).Name, this.HeroesNtf);
            NetHandler.AddDataHandler(typeof(EventBuildNtf).Name, this.EventBuildRemove);
            NetHandler.AddNtfHandler(typeof(DailyLimitNtf).Name, this.DailyLimitNtf);
            NetHandler.AddNtfHandler(typeof(TaskChangeNtf).Name, this.TaskChangeNtf);
        }


        private void EventBuildNtf(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            EventBuild eventBuild = eventBuildNtf.EventBuild;
            Dictionary<string, EventBase> eventDict = EventManager.EventDict[Event.Build];
            int length = eventDict.Count;
            if (this.view.isInitUI) {
                if (eventBuildNtf.Method == "new") {
                    this.view.SortVision(DailyAdvise.BuildArray, 10);
                } else {
                    if (length == 0) {
                        this.view.SortVision(DailyAdvise.BuildArray, 3);
                    } else {
                        this.view.RefeshItem(DailyAdvise.BuildArray);
                    }
                }
            }
        }

        private void EventBuildRemove(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            EventBuild eventBuild = eventBuildNtf.EventBuild;
            if (eventBuildNtf.Method == "del") {
                EventManager.FinishImmediate(eventBuild.Id);
            }
        }

        private void PointNtf(IExtensible message) {
            if (this.view.isInitUI) {
                this.view.RefeshItem(DailyAdvise.TileCount);
            }
        }

        private void ForceNtf(IExtensible message) {
            if (this.view.isInitUI) {
                this.view.RefeshItem(DailyAdvise.Force);
            }
        }

        private void DailyLimitNtf(IExtensible message) {
            if (this.view.isInitUI) {
                if (this.DailyRewardComplete()) {
                    this.view.SortVision(DailyAdvise.DailyReward, 1);
                } else {
                    this.view.SortVision(DailyAdvise.DailyReward, 8);
                }
            }
        }

        public void DailyLimitReq() {
            GetDailyLimitExpiredReq req = new GetDailyLimitExpiredReq();
            NetManager.SendMessage(req, typeof(GetDailyLimitExpiredAck).Name, this.DailyLimitAck);
        }

        private void DailyLimitAck(IExtensible message) {
            GetDailyLimitExpiredAck ack = message as GetDailyLimitExpiredAck;
            EventManager.FinishAllEvent(Event.DailyReward);
            EventManager.AddDailyRewardEvent(ack.ExpiredAt);
        }

        public void GetCanAddForceCoordReq() {
            GetCanAddForceCoordReq getCanAddForceCoordReq
                = new Protocol.GetCanAddForceCoordReq() { };
            NetManager.SendMessage(getCanAddForceCoordReq,
                typeof(GetCanAddForceCoordAck).Name, this.GetCanAddForceCoordAck);
        }

        private void GetCanAddForceCoordAck(IExtensible message) {
            GetCanAddForceCoordAck getCanAddForceCoordAck
                = message as GetCanAddForceCoordAck;
            Coord coord = getCanAddForceCoordAck.Coord;
            this.view.AddBtnGoListrners();
            this.parent.ClickTile(new Vector2(coord.X, coord.Y), TileArrowTrans.Attack);
        }

        private void TaskChangeNtf(IExtensible message) {
            int max, doneTask;
            this.taskModel.HowManyIsDone(out doneTask, out max);
            if (this.view.isInitUI) {
                if (doneTask >= max) {
                    this.view.SortVision(DailyAdvise.DailyTask, 9);
                } else {
                    this.view.SortVision(DailyAdvise.DailyTask, 4);
                }
            }
        }

        private void HeroesNtf(IExtensible message) {
            if (this.view.isInitUI) {
                if (this.heroModel.CanLevelUpCount > 0) {
                    this.view.SortVision(DailyAdvise.HeroLevelUp, 11);
                    } else {
                    this.view.SortVision(DailyAdvise.HeroLevelUp, 13);
                }
            }
        }

        public bool IsTaskFull() {
            int max, doneTask;
            this.taskModel.HowManyIsDone(out doneTask, out max);
            return (doneTask >= max);
        }

        public bool DailyRewardComplete() {
            if (this.DailyLimit.ResourceCurrent.Lumber
                != this.DailyLimit.ResourceLimit.Lumber)
                return true;
            if (this.DailyLimit.ResourceCurrent.Food
                != this.DailyLimit.ResourceLimit.Food)
                return true;
            if (this.DailyLimit.ResourceCurrent.Steel
                != this.DailyLimit.ResourceLimit.Steel)
                return true;
            if (this.DailyLimit.ResourceCurrent.Marble
                != this.DailyLimit.ResourceLimit.Marble)
                return true;
            if (this.DailyLimit.GoldCurrent
                != this.DailyLimit.GoldLimit)
                return true;
            if (this.DailyLimit.ChestCurrent
                != this.DailyLimit.ChestLimit)
                return true;
            return false;
        }

        public Dictionary<Hero, string> FindUpGradeHero() {
            heroList.Clear();
            foreach (var item in HeroDict) {
                int heroFragments = HeroLevelConf.GetHeroUpgradFragments(item.Value);
                int fragmentCount = item.Value.FragmentCount;
                if (item.Value.Level == 1) {
                    fragmentCount += 1;
                    heroFragments += 1;
                }
                if (heroFragments > 0) {
                    if (fragmentCount >= heroFragments) {
                        heroList.Add(item.Value
                            , string.Concat(fragmentCount, "/", heroFragments));
                    }
                }
            }
            return heroList;
        }

        public void JumpToBuildList() {
            this.parent.ChangeChannel(HouseKeeperInfoType.Build);
        }

        public void RefeshInShow() {
            this.view.RefeshInShow();
        }

        public void ResetList() {
            this.view.RefeshItem(DailyAdvise.BuildArray);
        }

        override protected void OnReLogin() {
            if (this.view.isInitUI) {
                this.view.RefeshItem(DailyAdvise.BuildArray);
            }
            this.DailyLimitReq();
        }
    }
}
