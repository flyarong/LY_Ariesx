using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using System.Linq;

namespace Poukoute {
    public class HouseKeeperDailyView : BaseView {
        private HouseKeeperDailyViewModel viewModel;
        private HouseKeeperDailyViewPreference viewPref;
        private Dictionary<DailyAdvise, int> visionList =
                    new Dictionary<DailyAdvise, int>();
        private Dictionary<DailyAdvise, int> visionListNew =
                    new Dictionary<DailyAdvise, int>();
        private bool isBuidling = false;
        private CustomScrollRect scroolRect;
        public bool isInitUI = false;

        private Dictionary<GameObject, int> itemDict =
            new Dictionary<GameObject, int>(5);
        private Dictionary<DailyAdvise, GameObject> itemNameDict =
            new Dictionary<DailyAdvise, GameObject>(5);

        /***************************************************/
        protected override void OnUIInit() {
            viewModel = this.GetComponent<HouseKeeperDailyViewModel>();
            this.ui = UIManager.GetUI("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlDaily");
            this.viewPref =
                this.ui.transform.GetComponent<HouseKeeperDailyViewPreference>();
            scroolRect = viewPref.pnlDaily.GetComponent<CustomScrollRect>();
            this.viewPref.customScrollRect.onBeginDrag.AddListener(CloseCleanDetail);
            this.viewPref.btnDaily.onClick.AddListener(CloseCleanDetail);

            this.SetList();
            EventManager.AddEventAction(Event.Build, this.UpdateBuildProgress);
            EventManager.AddEventAction(Event.DailyReward, this.UpdateDailyReward);
            this.isInitUI = true;
        }

        private void Start() {
            SetVisonList();
        }

        public void SetHighlightFrame() {
                ScreenEffectManager.SetHighlightFrame
                (this.itemNameDict[DailyAdvise.DailyReward].transform, 
                null,new Vector2(-3,-3),new Vector2(3,30));
        }

        private void SetVisonList() {
            visionList.Add(DailyAdvise.TileCount, 2);
            visionList.Add(DailyAdvise.BuildArray, 3);
            visionList.Add(DailyAdvise.GetTribute, 7);
            visionList.Add(DailyAdvise.HeroLevelUp, 11);
            visionList.Add(DailyAdvise.Force, 5);
            visionList.Add(DailyAdvise.DailyReward, 1);
            visionList.Add(DailyAdvise.DailyTask, 4);
        }

        private void SortVisionList() {
            DictonarySort(visionList, ref visionListNew);
        }

        private static void DictonarySort(
            Dictionary<DailyAdvise, int> dic, ref Dictionary<DailyAdvise, int> refDic) {
            dic = (from temp in dic orderby temp.Value select temp).
                ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var item in dic) {
                refDic.Add(item.Key, item.Value);
            }
        }

        private int GetSiblingIndex(DailyAdvise name) {
            itemDict = (from temp in itemDict orderby temp.Value select temp).
                ToDictionary(pair => pair.Key, pair => pair.Value);
            int i = 0;
            foreach (var item in itemDict) {
                if (item.Key.GetComponent<DailyItemView>().selfName == name) {
                    break;
                }
                i++;
            }
            return i;
        }

        public bool GetTileCount() {
            if (RoleManager.GetPointDict().Count ==
                                         RoleManager.GetPointsLimit()) {
                return true;
            } else {
                return false;
            }
        }

        public void UpdateBuildProgress(EventBase eventBase) {
            EventBuildClient eventBuild = eventBase as EventBuildClient;
            long left = eventBuild.duration -
                (RoleManager.GetCurrentUtcTime() - eventBuild.startTime);
            left = (long)Mathf.Max(0, left);
            isBuidling = true;
            if (left == 0) isBuidling = false;
        }

        public void UpdateDailyReward(EventBase eventBase) {
            long left = eventBase.startTime + eventBase.duration - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            if (left == 0) {
                this.viewModel.DailyLimitReq();
            }
        }

        public void UpdateTributeTime(EventBase eventBase) {
            long left = eventBase.startTime + eventBase.duration
                - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            if (left != 0) {
                Debug.Log(GameHelper.TimeFormat(left));
            }
        }

        public void SortVision(DailyAdvise name, int num) {
            if (num != 13) {
                if (itemDict[itemNameDict[name]] == 13) {
                    itemNameDict[name].SetActiveSafe(true);
                    itemDict[itemNameDict[name]] = num;
                    itemNameDict[name].transform.SetSiblingIndex(this.GetSiblingIndex(name));
                    itemNameDict[name].GetComponent<DailyItemView>().ChooseVision(name);
                } else {
                    itemDict[itemNameDict[name]] = num;
                    itemNameDict[name].transform.SetSiblingIndex(this.GetSiblingIndex(name));
                    itemNameDict[name].GetComponent<DailyItemView>().ChooseVision(name);
                }
            } else {
                itemDict[itemNameDict[name]] = num;
                itemNameDict[name].SetActiveSafe(false);
            }
        }

        public void RefeshItem(DailyAdvise name) {
            itemNameDict[name].GetComponent<DailyItemView>().ChooseVision(name);
        }

        public void RefeshInShow() {
            if (!this.IsVisible) return;
            if (RoleManager.GetPointDict().Count != itemNameDict[DailyAdvise.TileCount].GetComponent<DailyItemView>().tileCount) {
                if (RoleManager.GetPointDict().Count == RoleManager.GetPointsLimit()) {
                    this.SortVision(DailyAdvise.TileCount, 13);
                } else {
                    this.SortVision(DailyAdvise.TileCount, 2);
                }
            }
            if (this.viewModel.CanLevelUpCount > 0) {
                this.SortVision(DailyAdvise.HeroLevelUp, 11);
            } else {
                this.SortVision(DailyAdvise.HeroLevelUp, 13);
            }
        }

        private void SortVisionStart() {
            if (isBuidling) {
                visionList[DailyAdvise.BuildArray] = 10;
            } else {
                visionList[DailyAdvise.BuildArray] = 3;
            }
            this.viewModel.FindUpGradeHero();
            if (this.viewModel.heroList.Count == 0) {
                visionList[DailyAdvise.HeroLevelUp] = 13;
            } else {
                visionList[DailyAdvise.HeroLevelUp] = 11;
            }
            if (this.GetTileCount()) {
                visionList[DailyAdvise.TileCount] = 13;
            } else {
                visionList[DailyAdvise.TileCount] = 2;
            }
            if (this.viewModel.DailyRewardComplete()) {
                visionList[DailyAdvise.DailyReward] = 1;
            } else {
                visionList[DailyAdvise.DailyReward] = 8;
            }
            if (this.viewModel.IsTaskFull()) {
                visionList[DailyAdvise.DailyTask] = 9;
            } else {
                visionList[DailyAdvise.DailyTask] = 4;
            }
        }

        public void SetSrcool() {
            scroolRect.verticalNormalizedPosition = 1f;
        }

        public void Format() {
            UnityAction format = () => {
                this.viewPref.customVerticalLayoutGroup.SetOriginal();
                this.viewPref.customScrollRect.velocity = Vector2.zero;
                scroolRect.verticalNormalizedPosition = 1;
            };
            this.viewPref.contentSizeFitter.onSetLayoutVertical.AddListener(format);
        }

        public void SetList() {
            this.SortVisionStart();
            this.visionListNew.Clear();
            this.SortVisionList();
            foreach (var item in visionListNew) {
                GameObject itemObj;
                if (item.Key == DailyAdvise.DailyReward) {
                    itemObj =
                    PoolManager.GetObject(PrefabPath.pnlReDailyItem, this.viewPref.pnlDailyList);
                } else {
                    itemObj =
                        PoolManager.GetObject(PrefabPath.pnlDailyItem, this.viewPref.pnlDailyList);
                }
                DailyItemView itemView = itemObj.GetComponent<DailyItemView>();
                if (item.Value == 13) {
                    itemObj.SetActiveSafe(false);
                }
                if (item.Key == DailyAdvise.BuildArray) {
                    itemView.onIdle = this.viewModel.JumpToBuildList;
                    itemView.onUnlock = this.viewModel.ShowUnlockConfirm;
                }
                itemView.ChooseVision(item.Key);
                itemView.parent = gameObject.GetComponent<HouseKeeperDailyView>();

                itemDict.Add(itemObj, item.Value);
                itemNameDict.Add(item.Key, itemObj);
            }
        }

        private void CloseCleanDetail() {
            CleanDetail();
        }

        public void CleanDetail(DailyAdvise name = DailyAdvise.Null) {
            foreach (var item in itemDict) {
                DailyItemView itemView = item.Key.GetComponent<DailyItemView>();
                itemView.HideDetail(name);
            }
        }

        public void GetCanAddForceCoordReq() {
            this.viewModel.GetCanAddForceCoordReq();
        }

        public void AddBtnGoListrners() {
            this.itemNameDict[DailyAdvise.DailyReward]
                .GetComponent<DailyItemView>().AddBtnGoListrners();
        }

        protected override void OnInvisible() {
            this.CleanDetail();
        }
    }
}
