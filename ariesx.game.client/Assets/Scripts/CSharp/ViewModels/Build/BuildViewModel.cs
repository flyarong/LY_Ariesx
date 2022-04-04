using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class BuildViewModel : BaseViewModel, IViewModel {
        private HouseKeeperViewModel parent;
        private BuildModel model;
        private MapModel mapModel;
        private MapMarkModel mapMarkModel;
        private BuildView view;
        public Action afterShow;

        /* Model data get set */
        public Dictionary<string, UnlockShowBuildingInfo> UnlockShowBuildingDict {
            get {
                return this.model.unlockShowBuildingDict;
            }
        }

        public Dictionary<string, UnlockBuildingInfo> CanBeBuiltBuildingDict {
            get {
                return this.model.canBeBuildBuildingDict;
            }
        }

        public List<ElementBuilding> BrokenBuildingList {
            get {
                return this.model.brokenBuildingList;
            }
        }

        public Dictionary<string, ElementBuilding> BuildDict {
            get {
                return this.model.buildingDict;
            }
        }

        public string CurrentBuilding {
            get {
                return this.model.currentBuilding;
            }
            set {
                this.model.currentBuilding = value;
            }
        }

        public BuildViewType BuildViewType {
            get {
                return this.model.buildViewType;
            }
            set {
                if (this.model.buildViewType != value) {
                    this.model.buildViewType = value;
                    this.NeedFresh = true;
                }
            }
        }
        /**********************/

        /* Other members */
        private bool NeedFresh {
            get; set;
        }
        public bool IsFixed {
            get; set;
        }

        public Vector2 FixedCoord {
            get; set;
        }
        //   private BuildInfoViewModel buildInfoViewModel;
        /*****************/


        void Awake() {
            this.parent = this.transform.parent.GetComponent<HouseKeeperViewModel>();
            this.model = ModelManager.GetModelData<BuildModel>();
            this.mapMarkModel = ModelManager.GetModelData<MapMarkModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<BuildView>();

            TriggerManager.Regist(Trigger.ResourceChange, this.ResourcesChange);
            NetHandler.AddDataHandler(typeof(PlayerBuildingNtf).Name, this.PlayerBuildingNtf);
            NetHandler.AddNtfHandler(typeof(ResourcesNtf).Name, this.ResourcesNtf);
            this.NeedFresh = true;
        }

        public void Show() {
            this.view.Show();
            this.view.Format();
            if (this.NeedFresh) {
                this.view.SetBuildingDict();
                this.NeedFresh = false;
            }
            this.view.SetScrollViewVisible(true);
            StartCoroutine(this.view.AfterShow());
        }

        public void Return() {
            this.Hide();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideHouseKeeper() {
            this.parent.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void ShowBuildEditViewUI() {
            this.parent.ShowBuildEditViewUI(this.IsFixed, this.FixedCoord);
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void SetArrow(string buildingName) {
            this.view.SetArrow(buildingName);
        }

        public Coord FindBuildUnlockCondition(string condition) {
            foreach (var build in this.model.buildingDict) {
                if (condition.Equals(build.Value.Name)) {
                    return build.Value.Coord;
                }
            }
            return null;
        }

        public void FindBuildingCanBeBuild(string condition, string conditionLevel) {
            foreach (var build in this.model.canBeBuildBuildingDict) {
                if (string.Equals(condition, build.Value.buildingName)) {
                    this.SetArrow(condition);
                    return;
                }
            }
            Coord coord = this.FindBuildUnlockCondition(condition);
            if (coord != null) {
                this.ClickTile(new Vector2(coord.X, coord.Y), TileArrowTrans.upgrade);
                return;
            }
            BuildingConf buildingConf = BuildingConf.GetConf(condition + "_" + conditionLevel);
            string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
            string relyBuildingName = unlockValue[0];
            string relyBuildingLevel = unlockValue[1];
            this.FindBuildingCanBeBuild(relyBuildingName, relyBuildingLevel);
        }

        public void ClickTile(Vector2 condition, TileArrowTrans tileArrowTrans) {
            this.view.afterHideCallback += () => {
                this.parent.MoveWithClickAttack(condition, tileArrowTrans);
            };
            this.HideHouseKeeper();
        }
        /* Add 'NetMessageAck' function here*/
        private void ResourcesChange() {
            if (this.view.IsVisible) {
                this.view.RefreshBuildingList();
            } else {
                this.NeedFresh = true;
            }
        }
        private void DeleteStrongholdMark(Vector2 cooordinate) {
            this.mapMarkModel.RemoveStrongHold(cooordinate);
        }

        private void PlayerBuildingNtf(IExtensible message) {
            PlayerBuildingNtf playerBuildingNtf = message as PlayerBuildingNtf;
            ElementBuilding elementBuilding;
            bool containBuilding = this.model.buildingDict.TryGetValue(
                playerBuildingNtf.Building.Name,
                out elementBuilding
            );
            bool isMethodDel = playerBuildingNtf.Method.CustomEquals("del");
            if (containBuilding) {
                Vector2 coordinate = playerBuildingNtf.Building.Coord;
                Point point = RoleManager.GetRolePoint(coordinate);
                if (point != null) {
                    point.Building = isMethodDel ? null : playerBuildingNtf.Building;
                    if (isMethodDel) {
                        point.Resource = playerBuildingNtf.Resource;
                        point.ElementType = playerBuildingNtf.ElementType;
                    }
                    RoleManager.RefreshPoint(point);
                }
                Point normalPoint = this.mapModel.GetPlayerPoint(coordinate);
                if (normalPoint != null) {
                    normalPoint.Building = isMethodDel ? null : playerBuildingNtf.Building;
                    // To do : why should I change the ElementType.
                    if (isMethodDel) {
                        normalPoint.Resource = playerBuildingNtf.Resource;
                        normalPoint.ElementType = playerBuildingNtf.ElementType;
                    }
                }

                if (isMethodDel) {
                    this.model.buildingDict.Remove(playerBuildingNtf.Building.Name);
                    this.DeleteStrongholdMark(coordinate);
                    containBuilding = false;
                }
            }

            int buildCurrentLevel = containBuilding ? elementBuilding.Level : 0;
            if (playerBuildingNtf.Building.Level > buildCurrentLevel && !isMethodDel) {
                this.parent.BuildingLevelUpHandler(playerBuildingNtf.Building.Name);
                this.NeedFresh = true;

                if (playerBuildingNtf.Building.Name.CustomEquals(ElementName.townhall)) {
                    TriggerManager.Invoke(Trigger.VoiceLiveUserDataChange);
                }
            }
            this.model.Refresh(playerBuildingNtf);
            // Notice armycamp level change
            this.parent.RefreshArmyCampTroopStatus(playerBuildingNtf.Building.Name);
            this.OnBuildingDataChange();
        }

        private void ResourcesNtf(IExtensible message) {
            if (this.view.IsVisible) {
                this.view.RefreshBuildingList();
            } else {
                this.NeedFresh = true;
            }
        }
        /***********************************/

        private void OnBuildingDataChange() {
            if (this.view.IsVisible) {
                this.view.SetBuildingDict();
            } else {
                this.NeedFresh = true;
            }
        }

        #region FTE
        public void OnFteStep51Start() {
            this.view.OnFteStep51Start();
        }

        public void OnFteStepBuildLevel1Start() {
            this.view.OnFteStepBuildLevel1Start();
        }

        public void OnFteStepBuildLevel1End() {
            this.view.afterHideCallback = this.view.OnFteStep51End;
            this.Hide();
        }

        // To do: need a callback when have animation or net request.
        public void OnFteStep51End() {
            this.view.afterHideCallback = this.view.OnFteStep51End;
            this.Hide();
        }

        public void OnBuildStep2Start(string index) {
            Debug.Log("index " + index);

            this.BuildViewType = BuildViewType.List;
            string[] indexArray = index.CustomSplit('_');
            this.Show();
            string step = indexArray[indexArray.Length - 1];
            DramaConf dramaConf = DramaConf.GetConf(step);
            string buildingType = string.Empty;
            if (dramaConf.isBuildingSpecial) {
                //Debug.Log(dramaConf.buildingType);
                BuildingConf buildingConf = BuildingConf.GetConf(
                    string.Concat(dramaConf.buildingType, "_1"));
                //Debug.Log(buildingConf.type);
                buildingType = buildingConf.type;
            } else {
                buildingType = dramaConf.buildingType;
            }
            this.view.OnBuildStep2Start(buildingType);
        }

        public void OnBuildStep2End() {
            this.view.SetScrollEnable(true);
            this.view.afterHideCallback = null;
        }
        #endregion
    }
}
