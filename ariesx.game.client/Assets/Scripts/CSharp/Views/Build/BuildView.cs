using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildView : BaseView {
        private BuildViewModel viewModel;
        private BuildViewPreference viewPref;

        private Dictionary<string, GameObject> pnlBuildItemDict =
            new Dictionary<string, GameObject>();

        private bool isResourceEnough;

        public bool IsResourceEnough {
            get {
                return this.isResourceEnough;
            }
            set {
                if (this.isResourceEnough != value) {
                    this.isResourceEnough = value;
                }
            }
        }

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BuildViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlBuild");
            this.viewPref = this.ui.transform.GetComponent<BuildViewPreference>();
        }

        public void SetBuildingDict() {
            //this.SetBuildTips();
            this.pnlBuildItemDict.Clear();
            //GameHelper.ClearChildren(this.viewPref.pnlBuildList);
            int listCount = this.viewModel.BrokenBuildingList.Count;
            int childCount = listCount + this.viewModel.CanBeBuiltBuildingDict.Count + 
                this.viewModel.UnlockShowBuildingDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlBuildList, childCount, PrefabPath.pnlBuildItem);
            ElementBuilding brokenBuild;
            int childeIndex = 0;
            for (int i = 0; i < listCount; i++) {
                //pnlBuildItem = PoolManager.GetObject(
                //    PrefabPath.pnlBuildItem, this.viewPref.pnlBuildList);
                GameObject pnlBuildItem = this.viewPref.pnlBuildList.GetChild(childeIndex++).gameObject;
                brokenBuild = this.viewModel.BrokenBuildingList[i];
                this.pnlBuildItemDict.Add(brokenBuild.Name, pnlBuildItem);
                this.SetBuilding(brokenBuild.Name, 1, pnlBuildItem, true, brokenBuild.Level);
            }

            foreach (var pair in this.viewModel.CanBeBuiltBuildingDict) {
                //pnlBuildItem = PoolManager.GetObject(
                //    PrefabPath.pnlBuildItem, this.viewPref.pnlBuildList);
                GameObject pnlBuildItem = this.viewPref.pnlBuildList.GetChild(childeIndex++).gameObject;
                this.pnlBuildItemDict.Add(pair.Key, pnlBuildItem);
                this.SetBuilding(pair.Value.buildingName,
                                pair.Value.buildingCount, pnlBuildItem);
            }

            foreach (var pair in this.viewModel.UnlockShowBuildingDict) {
                //pnlBuildItem = PoolManager.GetObject(
                //    PrefabPath.pnlBuildItem, this.viewPref.pnlBuildList);
                GameObject pnlBuildItem = this.viewPref.pnlBuildList.GetChild(childeIndex++).gameObject;
                this.pnlBuildItemDict.Add(string.Concat("UnclockShow_", pair.Key), pnlBuildItem);
                this.SetBuilding(pair.Value.buildingName,
                                pair.Value.buildingCount, pnlBuildItem, isUnclokShow: true, unlockShowLevel: pair.Value.unlockShowLevel);
            }
            this.FormatPnlBuildList();
        }

        public void SetScrollViewVisible(bool isVisible) {
            if (isVisible) {
                UIManager.ShowUI(this.viewPref.scrollView.gameObject);
                this.IsResourceEnough = false;
            } else {
                foreach (GameObject item in this.pnlBuildItemDict.Values) {
                    AnimationManager.Finish(item);
                }
                UIManager.HideUI(this.viewPref.scrollView.gameObject);
            }
        }

        public void SetScrollEnable(bool enable) {
            this.viewPref.scrollView.GetComponent<CustomScrollRect>().vertical = enable;
        }

        public void RefreshBuildingList() {
            //this.SetBuildTips();
            foreach (var pair in this.pnlBuildItemDict) {
                UnlockBuildingInfo unlockBuildingInfo;
                if (this.viewModel.CanBeBuiltBuildingDict.TryGetValue(pair.Key, out unlockBuildingInfo)) {
                    this.SetBuilding(unlockBuildingInfo.buildingName,
                                     unlockBuildingInfo.buildingCount,
                                     pair.Value);
                }
                if (pair.Key.Contains("UnlockShow_")) {
                    UnlockShowBuildingInfo unlockShowBuildingInfo;
                    if (this.viewModel.UnlockShowBuildingDict.TryGetValue(pair.Key.Remove(0, 11), out unlockShowBuildingInfo)) {
                        this.SetBuilding(unlockShowBuildingInfo.buildingName,
                                         unlockShowBuildingInfo.buildingCount,
                                         pair.Value, isUnclokShow: true, unlockShowLevel: unlockShowBuildingInfo.unlockShowLevel);
                    }
                }
            }
        }

        //private void SetBuildTips() {
        //    bool haveBuildableBuilding = (this.viewModel.CanBeBuiltBuildingDict.Count +
        //        this.viewModel.BrokenBuildingList.Count) > 0;
        //}

        private void SetBuilding(string buildingName, int buildingCount,
                                 GameObject pnlBuildItem, bool isBroken = false, int level = 1, bool isUnclokShow = false, string unlockShowLevel = null) {
            BuildItemView buildItemView = pnlBuildItem.GetComponent<BuildItemView>();
            BuildingConf buildConf =
                    ConfigureManager.GetConfById<BuildingConf>(
                                string.Concat(buildingName, "_", level));
            buildItemView.SetBuilding(buildConf, buildingCount, isBroken, isUnclokShow, unlockShowLevel);
            string id = buildConf.buildingName;
            buildItemView.SetBuildItem(id, (canbuild, isLock) => { this.OnBuildItemClick(canbuild, isLock, id); });
            //buildItemView.SetBuildItemDeatil(id, (canbuild) => { this.OnBuildDetailInfoClick(buildConf); });
        }

        public void SetArrow(string buildName) {
            this.viewModel.afterShow = null;
            this.viewModel.afterShow += () => {
                foreach (var item in this.pnlBuildItemDict) {
                    if (item.Value.GetComponent<BuildItemView>().name.Equals(buildName)) {
                        item.Value.transform.SetAsFirstSibling();
                        base.StartCoroutine(this.DelaySetArrow(item.Value.transform));
                        this.viewPref.scrollRect.onDrag.AddListener(this.HideFteArrow);
                        afterHideCallback += () => {
                            //this.viewModel.afterShow = null;
                            FteManager.HideArrow();
                        };
                    }
                }
            };
        }

        private IEnumerator DelaySetArrow(Transform item) {
            yield return YieldManager.EndOfFrame;
            FteManager.SetArrow(item.transform, this.viewPref.transform.parent);
        }

        private void HideFteArrow() {
            FteManager.HideArrow();
            this.viewPref.scrollRect.onDrag.RemoveAllListeners();
        }

        public IEnumerator AfterShow() {
            yield return YieldManager.EndOfFrame;
            this.viewModel.afterShow.InvokeSafe();
            this.viewModel.afterShow = null;
        }

        private void ClickLockBuild(string id) {
            BuildingConf build = BuildingConf.GetConf(id + "_1");
            string[] unlockValue = build.unlockCondition.CustomSplit(',');
            string unlockCondition = unlockValue[0];
            Coord conditionCoord =
               this.viewModel.FindBuildUnlockCondition(unlockCondition);
            if (conditionCoord == null) {
                this.viewModel.FindBuildingCanBeBuild(unlockCondition, "1");
            } else {
                this.viewModel.ClickTile(
                new Vector2(conditionCoord.X, conditionCoord.Y), TileArrowTrans.upgrade);
            }
        }

        #region FTE
        public void OnFteStep51Start() {
            //foreach (GameObject obj in this.pnlBuildItemDict.Values) {
            //    BuildItemView view = obj.GetComponent<BuildItemView>();
            //}
            BuildItemView itemView = this.pnlBuildItemDict[ElementName.townhall].GetComponent<BuildItemView>();
            RectTransform rectTransform = itemView.GetComponent<RectTransform>();
            Transform arrowParent = UIManager.GetUI("UIHouseKeeper").transform;
            FteManager.SetMask(itemView.transform, isEnforce: true,
                offset: Vector2.up * 40, arrowParent: arrowParent);
            FteManager.SetChat(rectTransform.rect.height + 120, needBackground: false);
        }

        public void OnFteStepBuildLevel1Start() {
            //foreach (GameObject obj in this.pnlBuildItemDict.Values) {
            //    BuildItemView view = obj.GetComponent<BuildItemView>();
            //}
            BuildItemView itemView = this.pnlBuildItemDict[ElementName.produce_food].GetComponent<BuildItemView>();
            RectTransform rectTransform = itemView.GetComponent<RectTransform>();
            Transform arrowParent = UIManager.GetUI("UIHouseKeeper").transform;
            FteManager.SetMask(
                itemView.transform, 
                isEnforce: true,
                offset: Vector2.up * 40, 
                arrowParent: arrowParent
            );
            FteManager.SetChat(rectTransform.rect.height + 120, needBackground: false);
        }

        public void OnFteStep51End() {
            //foreach (GameObject obj in this.pnlBuildItemDict.Values) {
            //    BuildItemView view = obj.GetComponent<BuildItemView>();
            //}
        }

        public void OnBuildStep2Start(string buildingName) {
            GameObject buildObj;
            if (this.pnlBuildItemDict.TryGetValue(buildingName, out buildObj) &&
                buildObj.activeSelf) {
                this.SetScrollEnable(false);
                this.afterHideCallback = () => {
                    this.SetScrollEnable(true);
                    FteManager.StopFte();
                };
                BuildItemView itemView = buildObj.GetComponent<BuildItemView>();
                this.viewModel.CurrentBuilding = itemView.ID;
                itemView.transform.SetAsFirstSibling();
                this.StartCoroutine(this.OnBuildStep2StartDelay(itemView));
            } else {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_building_is_locked),
                    TipType.Notice
                );
                FteManager.StopFte();
                this.viewModel.StartChapterDailyGuid();
            }
        }

        private IEnumerator OnBuildStep2StartDelay(BuildItemView itemView) {
            yield return YieldManager.EndOfFrame;
            itemView.transform.SetAsFirstSibling();
            Transform arrowParent = UIManager.GetUI("UIHouseKeeper").transform;
            bool isEnforce = FteManager.Instance.curStep.CustomEquals("chapter_task_3");
            FteManager.SetMask(itemView.transform, isEnforce: isEnforce,
                offset: Vector3.up * 40, arrowParent: arrowParent);
        }

        #endregion

        /* Propert change function */

        /***************************/

        private void OnBuildItemClick(bool isResourceEnough, bool isLock, string buildingName) {
            this.IsResourceEnough = isResourceEnough;
            if (isLock) {
                this.ClickLockBuild(buildingName);
                return;
            }

            ElementBuilding elementBuilding;
            if (this.viewModel.BuildDict.TryGetValue(buildingName, out elementBuilding)) {
                if (elementBuilding.Level == 0) {
                    UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.building_under_construction), TipType.Info);
                    return;
                } else if (EventManager.IsBuildingUnderBuildEvent(buildingName)) {
                    UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.building_under_upgrading), TipType.Info);
                    return;
                }
            }

            if (this.IsResourceEnough) {
                this.viewModel.HideHouseKeeper();
                this.viewModel.CurrentBuilding = buildingName;
                this.viewModel.ShowBuildEditViewUI();
            } else {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.tips_get_resource), TipType.Info);
            }
        }

        private void FormatPnlBuildList() {
            this.viewPref.contentSizeFitter.onSetLayoutVertical.AddListener(() => {
                this.viewPref.pnlBuildListRect.anchoredPosition = new Vector2(
                    this.viewPref.pnlBuildListRect.sizeDelta.x / 2,
                    -this.viewPref.pnlBuildListRect.sizeDelta.y / 2
                );
            });
        }

        public void Format() {
            this.afterShowCallback += () => {
                this.viewPref.scrollRect.verticalNormalizedPosition = 1;
            };
        }
    }
}
