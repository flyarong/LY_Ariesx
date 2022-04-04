using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class HeroViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private HeroModel model;
        private HeroView view;

        //private HeroInfoViewModel infoViewModel;
        private HeroListViewModel listViewModel;
        private HeroLotteryViewModel lotteryViewModel;
        private OpenChestViewModel openChestViewModel;

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.model.heroDict;
            }
            private set {
                this.model.heroDict = value;
            }
        }

        public Dictionary<string, HeroAttributeConf> UnlockHeroDict {
            get {
                return this.model.unlockHeroDict;
            }
        }

        public List<Hero> OrderHeroList {
            get {
                return this.model.GetHeroListOrderBy();
            }
        }

        public string CurrentHeroName {
            get {
                return this.model.currentHeroName;
            }
            set {
                if (this.model.currentHeroName != value) {
                    this.NeedRefresh = true;
                    this.model.currentHeroName = value;
                }
            }
        }

        public List<LotteryResult> LotteryResultList {
            get {
                return this.model.lotteryResultList;
            }
            set {
                this.model.lotteryResultList = value;
            }
        }

        public bool Lottery {
            get {
                return !(this.NewHeroCount > 0);
            }
        }

        public HeroSubViewType ViewType {
            get {
                return this.model.viewType;
            }
            set {
                if (this.model.viewType != value) {
                    this.model.viewType = value;
                    this.NeedRefresh = true;
                }
            }
        }

        public bool NeedRefresh {
            get; set;
        }

        public bool OpeningChest = false;

        public int Gold {
            get {
                return (int)RoleManager.GetResource(Resource.Gold);
            }
            set {
                if (this.Gold != value) {
                    RoleManager.SetGoldAmount(value);
                }
            }
        }

        public int Gem {
            get {
                return (int)RoleManager.GetResource(Resource.Gem);
            }
            set {
                if (this.Gem != value) {
                    RoleManager.SetGemAmount(value);
                }
            }
        }

        public int NewHeroCount {
            get {
                return this.model.NewHeroCount;
            }
            set {
                if (this.model.NewHeroCount != value) {
                    this.model.NewHeroCount = value;
                    this.view.SetHeroTabPointVisible();
                }
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

        public int CanLevelUpCount {
            get {
                return this.model.CanLevelUpCount;
            }
            set {
                this.model.CanLevelUpCount = value;
                this.view.SetHeroTabPointVisible();
            }
        }

        //private bool isVisible = false;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<HeroView>();
            this.listViewModel = PoolManager.GetObject<HeroListViewModel>(this.transform);
            this.lotteryViewModel = PoolManager.GetObject<HeroLotteryViewModel>(this.transform);
            this.openChestViewModel = PoolManager.GetObject<OpenChestViewModel>(this.transform);
            this.NeedRefresh = true;
            NetManager.AddDisConnectedAction(this.OnReLogin);
            NetHandler.AddNtfHandler(typeof(CurrencyNtf).Name, this.CurrencyNtf);
            NetHandler.AddDataHandler(typeof(HeroesNtf).Name, this.HeroesNtf);
            NetHandler.AddNtfHandler(typeof(ChestsNtf).Name, this.OnFreeLotteryGetNtf);

            TriggerManager.Regist(Trigger.HeroStatusChange, () => {
                if (this.view.IsVisible) {
                    this.view.SetHeroInfoPoint();
                }
            });

            FteManager.SetStartCallback(GameConst.NORMAL, 111, this.OnFteStep111Start);
            FteManager.SetStartCallback(GameConst.HERO_LEVEL, 1, this.OnHeroStep1Start);
            FteManager.SetEndCallback(GameConst.HERO_LEVEL, 1, this.OnHeroStep1End);
        }

        #region show_hide
        public void Show() {
            this.view.PlayShow(() => {
                if (this.ViewType != HeroSubViewType.Info) {
                    this.parent.OnAddViewAboveMap(this);
                }
            });
            this.SetHeroSubViewType();
            this.view.Init();
            this.view.SetHeroInfoPoint();
        }

        public void Hide(UnityAction callback = null) {
            this.view.PlayHide(() => {
                if (this.ViewType != HeroSubViewType.Info) {
                    this.parent.OnRemoveViewAboveMap(this);
                }
                this.HideAllSubView();
                callback.InvokeSafe();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.HideAllSubView();
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }
        #endregion

        #region with_other_view_model
        public void ShowNewHero(LotteryResult lotteryResult,
            string groupName, UnityAction callback, bool isForceFte) {
            this.openChestViewModel.ShowNewHero(lotteryResult, groupName, callback, isForceFte);
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void ShowHeroList(bool state) {
            if (state) {
                this.listViewModel.Show();
            } else {
                this.listViewModel.Hide();
            }
        }

        public void ShowLotteryList(bool state) {
            if (state) {
                this.lotteryViewModel.Show();
            } else {
                this.lotteryViewModel.Hide();
            }
        }

        public void ShowInfo(UnityAction levelUpCallback = null) {
            this.parent.ShowHeroInfo(this.CurrentHeroName, 
                levelUpCallback: levelUpCallback, isSubWindow: true);
        }

        public void HideInfo() {
            this.parent.HideHeroInfo();
        }

        public void ShowHeroTierUp() {
            this.parent.ShowHeroTierUp();
        }

        public void RefreshHeroView(string hero) {
            this.listViewModel.RefreshHeroView(hero);
        }

        public void RefreshLotteryView() {
            this.lotteryViewModel.Refresh();
        }
        #endregion

        public void ShowHeroPool(string groupName) {
            this.parent.ShowHeroPool(groupName);
        }

        public void OnHeroInListClick(string name) {
            this.CurrentHeroName = name;
            this.ShowInfo(() => {
                this.listViewModel.RefreshHeroView(name);
            });
        }

        public void OnUnlockHeroClick(string name) {
            this.parent.ShowHeroInfo(name, infoType: HeroInfoType.Unlock, isSubWindow: true);
        }

        public void RefreshHouseKeeperEvent() {
            this.parent.RefreshHouseKeeperEvent();
        }

        public void OnBackGroundClick(Transform pnl) {
            this.view.OnBackGroundClick(pnl);
        }

        private void OnFreeLotteryGetNtf(IExtensible message) {
            ChestsNtf Chests = message as ChestsNtf;
            if (Chests.Chests.Count > 0) {
                HeroModel.AddlotteryChances(Chests.Chests);
            }
        }

        private void HideAllSubView() {
            this.listViewModel.Hide();
            this.lotteryViewModel.Hide();
        }

        private void SetHeroSubViewType() {
            if (this.ViewType != HeroSubViewType.None) {
                return;
            }
            if (this.FreeLotteryCount > 0) {
                this.ViewType = HeroSubViewType.Lottery;
                return;
            }

            if (this.NewHeroCount > 0 ||
                this.CanLevelUpCount > 0) {
                this.ViewType = HeroSubViewType.All;
            } else {
                this.ViewType = HeroSubViewType.Lottery;
            }
        }

        public void ReadHeroReq(string name) {
            ReadHeroReq markReadReq = new ReadHeroReq() {
                Name = name
            };
            NetManager.SendMessage(markReadReq, typeof(ReadHeroAck).Name,
                (message) => this.ReadHeroAck(name));
        }

        /***** Net Ack *****/
        private void ReadHeroAck(string name) {
            this.NewHeroCount--;
            this.RefreshHeroView(name);
        }

        private void HeroesNtf(IExtensible message) {
            HeroesNtf heroesNtf = message as HeroesNtf;
            int heroFragments = 0;
            Hero originalHero;
            foreach (Hero newHero in heroesNtf.Heroes) {
                // To do: data change handler.
                heroFragments = HeroLevelConf.GetHeroUpgradFragments(newHero);
                if (this.HeroDict.TryGetValue(newHero.Name, out originalHero)) {
                    if ((newHero.Level > originalHero.Level) &&
                        (newHero.FragmentCount < heroFragments) &&
                        (this.CanLevelUpCount > 0)) {
                        this.CanLevelUpCount--;
                    }
                } else {
                    //Debug.LogError(HeroLevelConf.GetHeroReachMaxLevel(newHero)+" "+newHero.Name);
                    if (HeroLevelConf.GetCanLevelUp(newHero)&& !HeroLevelConf.GetHeroReachMaxLevel(newHero)) {
                        this.CanLevelUpCount++;
                    }
                    this.NewHeroCount++;
                    this.UnlockHeroDict.Remove(newHero.Name);
                }
                this.HeroDict[newHero.Name] = newHero;
                this.model.lastSortType = HeroSortType.None;
            }
            this.NeedRefresh = true;
            this.listViewModel.NeedRefresh = true;
            //this.RefreshHeroIsNewStatus(heroesNtf);
            if (heroesNtf.Heroes.Count > 0) {
                TriggerManager.Invoke(Trigger.HeroArmyAmountChange);
            }
        }

        //private void RefreshHeroIsNewStatus(HeroesNtf heroesNtf) {
        //    if (this.view.IsVisible) { 
        //    int heroesCount = heroesNtf.Heroes.Count;
        //    Hero hero;
        //    for (int index = 0; index < heroesCount; index++) {
        //        hero = heroesNtf.Heroes[index];
        //        if (hero.IsNew) {
        //            this.ReadHeroReq(hero.Name);
        //        }
        //    }
        //}

        //public void SetGridInfo() {
        //    this.lotteryViewModel.SetGridInfo();
        //}

        public void ShowOpenChestView() {
            this.openChestViewModel.Show();
        }

        public void ShowOpenChestView(List<LotteryResult> result, UnityAction callback) {
            this.openChestViewModel.Show(result, callback);
        }

        public void OpenHeroChest(List<LotteryResult> resultList) {
            this.LotteryResultList = resultList;
            this.NeedRefresh = true;
            this.OpeningChest = true;
            this.openChestViewModel.Show(isBuy:true);
        }

        // To do: a function to show reward hero.

        public void CurrencyNtf(IExtensible message) {
            CurrencyNtf currencyNtf = message as CurrencyNtf;
            if (this.view.IsVisible) {
                this.Gold = currencyNtf.Currency.Gold;
                this.Gem = currencyNtf.Currency.Gem;
            } else {
                this.NeedRefresh = true;
            }
        }
        // End

        #region FTE

        private void OnFteStep111Start(string index) {
            this.ViewType = HeroSubViewType.Lottery;
            this.view.OnFteStep111Start();
            this.Show();
        }

        public void OnFteStep111Process() {
            this.lotteryViewModel.OnFteStep111Process();
        }

        private void OnHeroStep1Start(string index) {
            this.ViewType = HeroSubViewType.All;
            this.view.OnHeroStep1Start();
            this.Show();
        }

        public void OnHeroStep1Process() {
            this.listViewModel.OnHeroStep1Process();
        }

        private void OnHeroStep1End() {
            this.view.afterHideCallback = null;
            this.listViewModel.OnHeroStep1End();
        }

        private void OnHeroStep2Start(string index) {
            this.OnHeroInListClick(FteManager.GetCurHero());
        }

        #endregion FTE

    }
}
