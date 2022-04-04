using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class HeroLotteryViewModel : BaseViewModel, IViewModel {
        private HeroViewModel parent;
        private HeroLotteryView view;
        private HeroModel model;
        private TroopModel troopModel;

        public List<GachaGroupConf> LotteryList {
            get {
                return this.model.lotteryList;
            }
        }

        public List<Chest> LotteryChanceList {
            get {
                return this.model.lotteryChanceList;
            }
        }

        public string LotteryGroup {
            get {
                return this.model.currentGroupName;
            }
            set {
                this.model.currentGroupName = value;
            }
        }

        public int FreeLotteryCount {
            get {
                return this.model.FreeLotteryCount;
            }
            set {
                this.model.FreeLotteryCount = value;
            }
        }

        public DailyLimit DailyLimit {
            get {
                return this.troopModel.dailyLimit;
            }
        }

        //public bool NeedRefresh { get; set; }
        public bool LotteryReqing { get; set; }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<HeroViewModel>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.view = this.gameObject.AddComponent<HeroLotteryView>();
            NetHandler.AddNtfHandler(typeof(DailyLimitNtf).Name, this.DailyLimitNtf);
            NetHandler.AddDataHandler(typeof(ChestsNtf).Name, this.ChestNtf);

            TriggerManager.Regist(Trigger.OnChestsGet, this.view.SetGridInfo);
            TriggerManager.Regist(Trigger.CurrencyChange, this.OnCrrencyChange);
            FteManager.SetStartCallback(GameConst.NORMAL, 50, this.OnFteStep50Start);
        }

        public void Show() {
            this.view.Show();
            this.SetLotteryList();
        }

        public void Refresh() {
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void SetLotteryList() {
            int chestUpIndex = RoleManager.GetFDRecordMaxLevel() + 1;
            this.LotteryList.Clear();
            FirstDownRewardConf firstDownConf;
            for (int i = 1; i < chestUpIndex; i++) {
                firstDownConf = FirstDownRewardConf.GetConf(i.ToString());
                GachaGroupConf gachaConf = GachaGroupConf.GetConf(firstDownConf.unlockChest);
                this.LotteryList.Add(gachaConf);
            }

            this.view.SetGridInfo();
        }

        public void HideHero() {
            this.parent.Hide();
        }

        public void ShowHeroPool() {
            this.parent.ShowHeroPool(this.LotteryGroup);
        }

        public void OpenChestReq() {
            if (!HeroModel.IsFreeLotteryChanceContain(this.LotteryGroup) ||
                this.parent.OpeningChest || this.LotteryReqing) {
                Debug.LogError("OpenChestReq failed " + this.parent.OpeningChest + " " +
                    HeroModel.IsFreeLotteryChanceContain(this.LotteryGroup));
                this.Show();
                return;
            }
            this.parent.ShowOpenChestView();
        }

        public bool IsOpeningChest() {
            return this.parent.OpeningChest;
        }

        public void LotterySingleReq() {
            //if (!this.LotteryReqing) {
            this.parent.RefreshHouseKeeperEvent(); //更新30秒刷新管家按钮事件
            LotteryReq lotteryReq = new LotteryReq() {
                GroupName = this.LotteryGroup
            };
            NetManager.SendMessage(lotteryReq,
                typeof(LotteryAck).Name,
                this.LotteryAck,
                (message) => {
                    this.LotteryReqing = false;
                }, () => {
                    this.LotteryReqing = false;
                });
            this.LotteryReqing = true;
            //}
        }

        private void LotteryAck(IExtensible message) {
            LotteryAck lotteryAck = message as LotteryAck;
            this.LotteryReqing = false;
            this.parent.OpenHeroChest(lotteryAck.Results);

#if DEVELOPER || UNITY_EDITOR
            foreach (var item in lotteryAck.Results) {
                Debug.LogError(item.FragmentCount);
            }
#endif

        }

        private void OnCrrencyChange() {
            if (this.view.IsVisible) {
                this.view.OnCurrentChange();
            }
        }

        private void DailyLimitNtf(IExtensible message) {
            if (this.view.IsVisible) {
                this.view.SetLotteryLimitTip();
            }
        }

        private void ChestNtf(IExtensible message) {
            this.SetLotteryList();
        }

        #region FTE

        public void OnFteStep111Process() {
            StartCoroutine(this.view.OnFteStep291Process());
        }

        public void OnFteStep50Start(string index) {
            int count = this.LotteryChanceList.Count;
            string chest;
            for (int i = 0; i < count; i++) {
                chest = this.LotteryChanceList[i].Name;
                if (chest.CustomEquals(GameConst.GIFT_GROUP_1)) {
                    this.LotteryGroup = chest;
                    this.OpenChestReq();
                }
            }
        }

        #endregion

    }
}
