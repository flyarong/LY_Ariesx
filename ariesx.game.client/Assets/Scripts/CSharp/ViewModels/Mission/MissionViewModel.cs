using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public enum TaskType {
        drama,
        daily,
        none
    }
    public class MissionViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private BuildModel buildModel;
        //private TroopModel troopModel;
        private MissionView view;
        /**********************/

        /* Other members */

        private DramaViewModel dramaViewModel;
        private DailyTaskViewModel dailyTaskViewModel;
        //private static int dramaLoadIndex = -1;
        //private static string dramaLoadIndexLabel = "dramaLoadIndex";

        public bool NeedRefresh {
            get; set;
        }
        public TaskType ViewType {
            get {
                return this.viewType;
            }
            set {
                this.viewType = value;
                this.OnViewTypeChange();
            }
        }

        public bool HaveDramaListArrow {
            get {
                return this.dramaViewModel.HasArrow();
            }
        }
        private TaskType viewType = TaskType.none;

        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            //this.troopModel = ModelManager.GetModelData<TroopModel>();

            this.dramaViewModel = PoolManager.GetObject<DramaViewModel>(this.transform);
            this.dailyTaskViewModel = PoolManager.GetObject<DailyTaskViewModel>(this.transform);
            this.view = this.gameObject.AddComponent<MissionView>();
        }

        public void Show(int tabIndex = 0) {
            this.view.PlayShow(tabIndex, () => {
                this.view.SetTabpointInfo(
                    this.dramaViewModel.GetCollectableTasksCount(),
                    this.dailyTaskViewModel.GetCollectableTasksCount()
                );
                this.parent.OnAddViewAboveMap(this);
            });
        }

        public void GetDramaRewards(int taskId) {
            this.dramaViewModel.GetDramaRewards(taskId);
        }

        public void Hide() {
            this.view.PlayHide(this.OnHideCallback);
        }

        public void ShowDramaView() {
            if (!this.view.IsVisible) {
                this.Show();
            }
        }

        private void HideAllSubPanels() {
            this.dailyTaskViewModel.Hide();
            this.dramaViewModel.Hide();
        }

        private void OnHideCallback() {
            this.HideAllSubPanels();
            this.parent.OnRemoveViewAboveMap(this);
        }

        public void PlayInitAnimation() {
            this.parent.PlayInitAnimation();
        }

        public void HideImmediatly() {
            //Debug.LogError("HideImmediatly");
            this.view.HideImmediatly(this.OnHideCallback);
        }

        public void HideHud() {
            //Debug.LogError("HideHud");
            this.parent.HideHUD();
        }

        public void SetAfterHideCallBack(UnityAction action) {
            this.view.afterHideCallback += action;
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void ShowNewHero(LotteryResult lotteryResult,
                string groupName, UnityAction callback, bool isForceFte) {
            this.parent.ShowNewHero(lotteryResult, groupName, callback, isForceFte);
        }

        public void ShowOpenChest(List<LotteryResult> result, UnityAction callback) {
            this.parent.ShowOpenChestView(result, callback);
        }

        public bool CheckDramNeedShow() {
            return this.dramaViewModel.CheckDramNeedShow();
        }

        public void CollectTaskReward(Protocol.Resources resources,
        Protocol.Currency currency, CommonReward commonReward) {
            this.parent.CollectTaskReward(resources, currency, commonReward);
        }
        #region drama public function
        public void AddAfterShowCallback(UnityAction action) {
            this.view.afterShowCallback += action;
        }
        public void ShowBanner(Protocol.CommonReward reward,Protocol.Resources resources
            ,Protocol.Currency currency,bool needNext) {
            this.parent.ShowBanner(reward,resources,currency,needNext);
        }

        //public void ShowBannerEx() {
        //    this.parent.ShowBannerEx();
        //}

        public void OnRemoveViewAboveMap(IViewModel baseViewModel) {
            this.parent.OnRemoveViewAboveMap(baseViewModel);
        }

        public void ShowTopHUD() {
            this.parent.ShowTopHUD();
        }

        public void CloseAboveUI() {
            this.parent.CloseAboveUI();
        }

        public void SetDramaArrow() {
            this.parent.SetDramaArrow();
        }

        public void SetDramaPoint() {
            if (this.view.IsVisible) {
                this.view.SetDramaPointInfo(
                    this.dramaViewModel.GetCollectableTasksCount());
            }
        }

        public void SetDailyTaskPoint() {
            if (this.view.IsVisible) {
                this.view.SetDailyTaskPointInfo(
                    this.dailyTaskViewModel.GetCollectableTasksCount());
            }
        }
        #endregion

        public bool HasDramaArrow() {
            return this.parent.HasDramaArrow();
        }

        #region Map task guid logic
        public void SetTaskDetail(int taskId, string taskDetail, bool receiveable, UnityAction jumpAction) {
            TaskType type = TaskType.drama;
            if (taskDetail.CustomIsEmpty() &&
                this.dailyTaskViewModel.IsDailyTaskUnlock()) {
                type = TaskType.daily;
                taskDetail = this.dailyTaskViewModel.GetDailyTips(out taskId, out receiveable, out jumpAction);
                //Debug.LogError("SetTaskDetail " + taskDetail + " " + receiveable + " " + jumpAction);
            }

            this.SetTaskDetail(taskId, type, taskDetail, receiveable, jumpAction);
        }

        public void SetTaskDetail(int taskId, TaskType type, string taskDetail, bool receiveable, UnityAction jumpAction) {
            this.parent.SetTaskDetail(taskId, type, taskDetail, receiveable, jumpAction);
        }

        public void StartDailyTaskGuid() {
            this.dailyTaskViewModel.StartSpecificDailyTaskGuid();
        }

        public void ShowDramaInMap() {
            this.dramaViewModel.ShowInMap(true);
        }

        public void StartChapterDailyGuid() {
            this.dramaViewModel.StartChapterDailyGuid();
        }

        public bool IsDailyTaskUnlock() {
            return this.dailyTaskViewModel.IsDailyTaskUnlock();
        }

        public void OnDailyTaskRefresh(int count) {
            this.parent.OnDailyTaskRefresh(count);
        }

        public void OnDramaTaskRefresh(int count) {
            this.parent.OnDramaTaskRefresh(count);
        }
        #endregion

        private void OnViewTypeChange() {
            switch (this.viewType) {
                case TaskType.drama:
                    this.dailyTaskViewModel.Hide();
                    this.dramaViewModel.Show();
                    break;
                case TaskType.daily:
                    this.dramaViewModel.Hide();
                    this.dailyTaskViewModel.Show();
                    break;
                default:
                    break;
            }
        }

        #region task logic
        // 1. Find the idel troop who's army amount not full, 
        // or not you don't have avaliable troop
        public void JumpToHeroRecuit() {
            // To do: Need Call FteManager startCallbackDict directly but has no effect on FTE.
            UnityAction action = () => {
                FteManager.StartFte("chapter_task_27", false);
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToHeroLevelUp() {
            // To do: Need Call FteManager startCallbackDict directly but has no effect on FTE.
            UnityAction action = () => {
                FteManager.StartFte("chapter_task_196", false);
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToTile(int tileLevel = 0, string tileType = "") {
            UnityAction action = () => {
                this.parent.GetTileCoordByLevel(tileLevel, tileType);
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToOtherPersonTile() {
            UnityAction action = () => {
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToBuilding() {
            UnityAction action = () => {
                Vector2 coordinate = this.buildModel.GetUpgradeableBuildingCoord();
                if (coordinate == Vector2.zero) {
                    this.parent.ShowBuildList();
                } else {
                    this.parent.MoveWithClick(coordinate);
                }
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToLottery() {
            UnityAction action = () => {
                this.parent.ShowLottery();
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToForceView() {
            UnityAction action = () => {
                this.parent.ShowForceReward();
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumToTribute() {
            UnityAction action = () => {
                this.parent.ShowTribute();
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToNearnestNpcCity() {
            UnityAction action = () => {
                Vector2 townHallCoord =
                    this.buildModel.GetBuildCoordinateByName(ElementName.townhall);
                Vector2 npcCityCoord =
                    NPCCityConf.GetNearestNPCCityCoord(townHallCoord, RoleManager.GetMapSN());
                this.parent.MoveWithClick(npcCityCoord);
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        public void JumpToNearnestPass() {
            UnityAction action = () => {
                Vector2 townHallCoord =
                    this.buildModel.GetBuildCoordinateByName(ElementName.townhall);
                Vector2 passCoord =
                    MiniMapPassConf.GetNearestPassCoord(townHallCoord, RoleManager.GetMapSN());
                this.parent.MoveWithClick(passCoord);
            };
            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }

        // Jump to own smallest level tile
        public void JumpToOwnTile() {
            UnityAction action = () => {
                Vector2 destination = RoleManager.GetOwnMinLevelPoint();
                this.parent.MoveWithClick(destination);
            };
            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.Hide();
            } else {
                action.Invoke();
            }
        }
        #endregion
        /***********************************/
    }
}
