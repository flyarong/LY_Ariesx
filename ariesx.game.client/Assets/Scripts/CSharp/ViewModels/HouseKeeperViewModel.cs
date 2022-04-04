using Protocol;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public enum HouseKeeperInfoType {
        None,
        Daily,
        Build
    }
    public class HouseKeeperViewModel : BaseViewModel, IViewModel {
        private HouseKeeperView view;
        private HouseKeeperModel model;
        private TroopModel troopModel;
        private MapViewModel parent;

        //private HouseKeeperStateViewModel stateViewModle;
        private HouseKeeperDailyViewModel dailyViewModle;
        private BuildViewModel buildViewModel;
        public HeroAttributeConf heroConf;

        public HouseKeeperInfoType Channel {
            get {
                return this.model.channel;
            }
            set {
                if (this.model.channel != value) {
                    this.model.channel = value;
                    this.OnChannelChange();
                }
            }
        }

        void Awake() {
            //this.stateViewModle =
            //   PoolManager.GetObject<HouseKeeperStateViewModel>(this.transform);
            this.dailyViewModle =
                PoolManager.GetObject<HouseKeeperDailyViewModel>(this.transform);
            this.buildViewModel =
                PoolManager.GetObject<BuildViewModel>(this.transform);
            this.model = ModelManager.GetModelData<HouseKeeperModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<HouseKeeperView>();

            //FteManager.SetStartCallback(GameConst.NORMAL, 51, this.OnFteStep51Start);
            //FteManager.SetEndCallback(GameConst.NORMAL, 51, this.OnFteStep51End);
            FteManager.SetStartCallback(GameConst.SPECIAL_BUILDING_LEVEL, 1, this.OnFteStepBuildLevel1Start);
            FteManager.SetEndCallback(GameConst.SPECIAL_BUILDING_LEVEL, 1, this.OnFteStepBuildLevel1End);
            FteManager.SetStartCallback(GameConst.BUILDING_LEVEL, 2, this.OnBuildStep2Start);
            FteManager.SetEndCallback(GameConst.BUILDING_LEVEL, 2, this.OnBuildStep2End);
            if (EventManager.IsBuildEventMaxFull()
                     && this.troopModel.HasAvaliableTroop()
                     && !FteManager.HasArrow()) {
                this.RefreshHouseKeeperEvent();
            }
        }

        public void Show(int index = 0, bool isSetHighlightFrame = false) {
            if (isSetHighlightFrame) {
                this.view.afterShowCallback += () => {
                    this.dailyViewModle.SetHighlightFrame();
                };
            }
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
                this.view.SetTab(index);
                this.dailyViewModle.RefeshInShow();
            }, true);
        }

        public void Hide() {
            this.view.PlayHide(this.OnHideCallback);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        private void OnHideCallback() {
            this.parent.OnRemoveViewAboveMap(this);
            this.view.SetAllOff();
            this.Channel = HouseKeeperInfoType.None;
        }

        public void ShowBuildEditViewUI(bool isFixed, Vector2 fixedCoord) {
            this.parent.ShowBuildEditViewUI(isFixed, fixedCoord);
        }

        public void SetIsFixd() {
            this.buildViewModel.IsFixed = false;
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void ShowBuildList(bool isFixed, Vector2 fixedCoord) {
            this.buildViewModel.IsFixed = isFixed;
            this.buildViewModel.FixedCoord = fixedCoord;
            this.Show(1);
        }

        public void ShowUnlockConfirm() {
            this.parent.ShowUnlockBuild(this.dailyViewModle.ResetList);
        }

        public void BuildingLevelUpHandler(string buildingName) {
            this.parent.BuildingLevelUpHandler(buildingName);
        }

        public void RefreshArmyCampTroopStatus(string armyCampName) {
            this.parent.RefreshArmyCampTroopStatus(armyCampName);
        }

        public void SetBuildingArrow(string buildingName) {
            this.buildViewModel.SetArrow(buildingName);
            this.Show(1);
        }

        public void ChangeChannel(HouseKeeperInfoType channel) {
            this.view.SetTab((int)channel - 1);
        }

        public void MoveWithClickAttack(Vector2 coordinate, TileArrowTrans tileArrowTrans) {
            this.parent.MoveWithClickAttack(coordinate, tileArrowTrans);
        }

        public void ClickTile(Vector2 condition, TileArrowTrans tileArrowTrans) {
            this.view.afterHideCallback += () => {
                this.MoveWithClickAttack(condition, tileArrowTrans);
            };
            this.Hide();
        }

        private void UpdateHouseKeeper(EventBase eventBase) {
            long left = RoleManager.GetCurrentUtcTime() - eventBase.startTime;
            if (left > 1000 * 5) {
                EventManager.RemoveEventAction(Event.HouseKeeper, this.UpdateHouseKeeper);
                this.parent.ShowHouseKeeperBtn();
            }
        }

        public void RefreshHouseKeeperEvent() {
            EventManager.RemoveEventAction(Event.HouseKeeper, this.UpdateHouseKeeper);
            EventManager.AddHouseKeeperEvent();
            EventManager.AddEventAction(Event.HouseKeeper, this.UpdateHouseKeeper);
        }

        private void OnChannelChange() {
            switch (this.Channel) {
                case HouseKeeperInfoType.Daily:
                    this.dailyViewModle.Show();
                    this.buildViewModel.Hide();
                    break;
                case HouseKeeperInfoType.Build:
                    this.dailyViewModle.Hide();
                    this.buildViewModel.Show();
                    break;
                default:
                    this.buildViewModel.Hide();
                    this.dailyViewModle.Hide();
                    break;
            }
        }

        #region FTE
        private void OnFteStep51Start(string index) {
            this.view.afterShowCallback = this.buildViewModel.OnFteStep51Start;
            this.Show(1);
        }

        // To do: need a callback when have animation or net request.
        private void OnFteStep51End() {
            this.view.afterHideCallback = this.buildViewModel.OnFteStep51End;
            this.Hide();
        }

        private void OnFteStepBuildLevel1Start(string index) {
            this.view.afterShowCallback = this.buildViewModel.OnFteStepBuildLevel1Start;
            this.Show(1);
        }

        private void OnFteStepBuildLevel1End() {
            this.view.afterHideCallback = this.buildViewModel.OnFteStepBuildLevel1End;
            this.Hide();
        }

        private void OnBuildStep2Start(string index) {
            this.view.afterShowCallback = () => {
                this.buildViewModel.OnBuildStep2Start(index);
            };
            this.Show(1);
        }

        private void OnBuildStep2End() {
            this.buildViewModel.OnBuildStep2End();
            this.Hide();
        }

        #endregion
    }
}
