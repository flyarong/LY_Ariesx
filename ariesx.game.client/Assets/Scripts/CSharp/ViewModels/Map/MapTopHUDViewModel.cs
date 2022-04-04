using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class MapTopHUDViewModel: BaseViewModel, IViewModel {
        private static MapTopHUDViewModel self;
        private static MapTopHUDViewModel Instance {
            get {
                if (self == null) {
                    Debug.LogError("MapTopHUDViewModel is not initialized.");
                }
                return self;
            }
        }

        private MapViewModel parent;
        private MapTopHUDView view;
        private UpgradeViewModel upgradeViewModel;
        private bool NeedRefresh {
            get; set;
        }

        //public bool IsResourceFull {
        //    get; set;
        //}

        public bool IsVisible {
            get {
                return this.view.IsVisible;
            }
        }

        public bool CanShowForceAni {
            get {
                return this.view.canShowForceAni;
            }
            set {
                this.view.canShowForceAni = value;
            }
        }

        public bool ShowForceView {
            get {
                return this.view.showForceView;
            }
            set {
                this.view.showForceView = value;
            }
        }

        public UnityAction ForceAniEndAction {
            get {
                return this.view.ForceAniEndAction;
            }
            set {
                this.view.ForceAniEndAction = value;
            }
        }
        /*****************/

        void Awake() {
            self = this;
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<MapTopHUDView>();
            this.upgradeViewModel = PoolManager.GetObject<UpgradeViewModel>(this.transform);
            this.NeedRefresh = true;
            FteManager.SetStartCallback(GameConst.NORMAL, 2, this.view.OnFteStep2Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 61, this.view.OnFteStep61Start);
            NetHandler.AddNtfHandler(typeof(CurrencyNtf).Name, this.CurrencyNtf);
            NetHandler.AddNtfHandler(typeof(ResourcesNtf).Name, this.ResourcesNtf);
            NetHandler.AddNtfHandler(typeof(ResourcesLimitNtf).Name, this.ResourcesLimitNtf);
            NetHandler.AddNtfHandler(typeof(PlayerPointNtf).Name, this.PlayerPointNtf);
            NetHandler.AddNtfHandler(typeof(PointLimitNtf).Name, this.PointLimitNtf);
            NetHandler.AddNtfHandler(typeof(ForceNtf).Name, this.ForceNtf);

            TriggerManager.Regist(Trigger.ResourceCollect, this.view.CollectResource);
            TriggerManager.Regist(Trigger.SimpleResourceCollect, this.view.CollectResourceSimple);
            TriggerManager.Regist(Trigger.Fte, this.SetFteUI);
            if (RoleManager.IsUnderProtection()) {
                EventManager.AddNoviceState(RoleManager.GetFreshProtectionFinishAt() * 1000);
                EventManager.AddEventAction(Event.NoviceState, this.UpdateNoviceState);
            } else {
                this.view.SetBtnNoviceState(false);
            }
        }

        public void Show(bool needAnimation = false) {
            if (needAnimation) {
                this.view.PlayShow();
            } else {
                this.view.Show();
            }
            if (this.NeedRefresh) {
                this.Refresh();
                this.NeedRefresh = false;
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void Refresh() {
            this.view.SetResources();
            this.view.SetCurrency();
        }

        public void RefreshPlayerName() {
            this.view.SetPlayerInfo();
        }

        public void SetChangeGem(int gem) {
            this.view.SetChangeGem(gem);
        }

        public void CollectResource(Resource type, int addAmount, Vector2 resourcePos,
                                    bool isPlayDroupOutAnimation, bool isCollect) {
            this.view.CollectResource(type, addAmount, resourcePos,
                isPlayDroupOutAnimation, isCollect);
        }

        public void ChangeResource(CommonReward reward) {
            this.view.ChangeResourcesWithoutAnimation(reward);
        }

        public void ShowPlayerInfo() {
            this.parent.ShowSelfInfo();
        }

        public void ShowPay() {
            LYGameData.OnOtherOpenStore();
            this.parent.ShowPay();
        }

        public void ShowResource() {
            this.parent.ShowResource();
        }

        public void ShowForceReward() {
            this.parent.ShowForceReward();
        }

        public void ShowFieldReward() {
            this.parent.ShowFieldReward();
        }

        public void ShowMiniMap() {
            this.parent.ShowMiniMap();
        }

        public void ShowFallen() {
            this.parent.ShowFallen();
        }

        public void ShowNoviceState() {
            this.parent.ShowNoviceState();
        }

        public void SetFallen(bool isFallen) {
            this.view.SetBtnFallen(isFallen);
        }

        public void ShowCampaignPanel() {
            this.parent.ShowCampaignsPanel();
        }

        public void SetOpenServiceActivityHUD(int count,bool isShow,bool showNotice) {
            this.view.SetOpenServiceActivityHUD(count, isShow, showNotice);
        }

        public void SetMonthCard(bool canShow) {
            this.view.SetBtnMonthCard(canShow);
        }

        public void SetResourceScreenEffect() {
            this.view.SetResourceScreenEffect();
        }

        public void SetPlayerInfoScreenEffect(UnityAction afterCallback) {
            this.view.SetPlayerInfoScreenEffect(afterCallback);
        }

        public void AddMarkOnTile(Vector2 coordinate) {
            this.parent.AddMarkOnTile(coordinate);
        }

        public void MoveTo(Vector2 coordinate) {
            this.parent.HideTileInfo();
            this.parent.Move(coordinate);
        }

        public void ForceUpgradeAnimation(UnityAction action) {
            this.upgradeViewModel.Show(action);
        }

        public void CloseIconAnimation(bool isStart) {
            this.view.CloseIconAnimation(isStart);
        }

        private void UpdateNoviceState(EventBase eventBase) {
            long left = eventBase.startTime - RoleManager.GetCurrentUtcTime();
            if (left < 0) {
                this.view.SetBtnNoviceState(false);
                EventManager.RemoveEventAction(Event.NoviceState, UpdateNoviceState);
            }
        }

        public void SetChangeResources(Protocol.Resources resources) {
            this.view.SetChangeResources(resources);
        }

        /* Add 'NetMessageAck' function here*/
        // To do: Need set the target resource amount.
        private void CurrencyNtf(IExtensible message) {
            CurrencyNtf currency = message as CurrencyNtf;
            //Debug.LogError("CurrencyNtf " + currency.Currency.Gold +
            //    " " + currency.ChangedCurrency.Gold + " " + RoleManager.Instance.NeedCurrencyAnimation);

            if (!RoleManager.Instance.NeedCurrencyAnimation) {
                this.view.SetCurrency();
            } else {
                this.view.SetChangeCurrency((message as CurrencyNtf).ChangedCurrency);
            }
        }

        private void ResourcesNtf(IExtensible message) {
            if (!RoleManager.Instance.NeedResourceAnimation) {
                this.view.SetResources();
            } else {
                this.view.SetChangeResources((message as ResourcesNtf).ChangedResources);
            }
        }

        private void ResourcesLimitNtf(IExtensible message) {
            this.Refresh();
        }

        private void PlayerPointNtf(IExtensible message) {
            this.view.SetPlayerInfo();
        }

        private void PointLimitNtf(IExtensible message) {
            this.view.SetPlayerInfo();
        }

        private void ForceNtf(IExtensible message) {
            this.view.SetPlayerInfo();
        }
        /***********************************/

        #region FTE
        private void SetFteUI() {
            this.view.SetFteUI();
        }
        #endregion

        public void ChangeAvatar() {
            this.view.ChangeAvatar();
        }
    }
}
