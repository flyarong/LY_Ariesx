using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class BuildInfoViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private BuildModel model;
        private BuildInfoView view;
        /* Other members */

        /*****************/

        /* Model data get set */
        public string BuildingAndLevel {
            get {
                return this.model.currentBuilding;
            }
            set {
                if (!this.model.currentBuilding.CustomEquals(value)) {
                    this.model.currentBuilding = value;
                }
            }
        }

        public BuildViewType BuildViewType {
            get {
                return this.model.buildViewType;
            }
            set {
                this.model.buildViewType = value;
            }
        }
        /**********************/

        public bool NeedFresh {
            get; set;
        }

        private UnityAction hideCallback = null;

        void Awake() {
            //ConfigureManager.Instance.LoadConfigure<DominantUpConf>("dominant_up_building");
            //ConfigureManager.Instance.LoadConfigure<WarehouseAttributeConf>("warehouse_attribute");
            //ConfigureManager.Instance.LoadConfigure<SiegeUpConf>("siege_up_building");
            //ConfigureManager.Instance.LoadConfigure<MarchSpeedUpConf>("march_speed_up_building");
            //ConfigureManager.Instance.LoadConfigure<TributeBuildingConf>("tribute_gold_building");
            //ConfigureManager.Instance.LoadConfigure<StrongholdRecruitConf>("stronghold_recruit");
            //ConfigureManager.Instance.LoadConfigure<HeroAttackUpConf>("hero_attack_up_building");
            //ConfigureManager.Instance.LoadConfigure<HeroDefenceUpConf>("hero_defence_up_building");
            //ConfigureManager.Instance.LoadConfigure<DurabilityUpConf>("durability_up_building");
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<BuildInfoView>();

            FteManager.SetStartCallback(GameConst.BUILDING_UPGRADE, 2, OnBuildUpStep2Start);
            FteManager.SetEndCallback(GameConst.BUILDING_UPGRADE, 2, OnBuildUpStep2End);
            TriggerManager.Regist(Trigger.ResourceChange, this.ResourcesChange);

            this.NeedFresh = true;
            this.Hide();
        }

        public void Show(UnityAction callback) {
            this.hideCallback = callback;
            this.Show(this.BuildViewType);
        }

        public void Show(BuildViewType viewType) {
            this.BuildViewType = viewType;
            this.view.PlayShow(null, true);
            if (this.NeedFresh) {
                this.view.SetInfo();
            }
        }

        //public void Show(BuildingConf buildingConf) {
        //    this.view.PlayShow(null, false);
        //    this.view.SetInfo(buildingConf);
        //}

        public void Hide() {
            this.view.PlayHide(this.hideCallback);
            this.view.HidePnlUnlockBuildingDesc();
            this.hideCallback = null;
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.hideCallback);
            this.hideCallback = null;
        }

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.view.SetInfo();
            } else {
                this.NeedFresh = true;
            }
        }

        public void UpgradeReq() {
            UpgradeReq upgradeReq = new UpgradeReq();
            BuildingConf buildingConf =
                ConfigureManager.GetConfById<BuildingConf>(this.BuildingAndLevel);
            upgradeReq.Name = buildingConf.buildingName;
            this.view.SetBtnUpgrade(false);
            NetManager.SendMessage(upgradeReq, typeof(UpgradeAck).Name, this.UpgradeAck,
                (message) => this.view.SetBtnUpgrade(true), () => this.view.SetBtnUpgrade(true));
        }

        public bool IsBuildingRechMaxLevel(string buildingName) {
            return this.model.IsBuildingRechMaxLevel(buildingName);
        }

        public void ShowUnlockBuild() {
            this.parent.ShowUnlockBuild(null, true);
            this.Hide();
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public float GetDurabilityBonus() {
            return this.model.GetDurabilityBonus();
        }

        public Coord FindBuildUnlockCondition(string condition) {
            foreach (var build in this.model.buildingDict) {
                if (condition.CustomEquals(build.Value.Name)) {
                    return build.Value.Coord;
                }
            }
            return null;
        }

        public void ClickTile(Vector2 condition, TileArrowTrans tileArrowTrans) {
            this.view.afterHideCallback += () => {
                this.parent.MoveWithClickAttack(condition, tileArrowTrans);
            };
            this.Hide();
        }

        public bool IsEqualsTownHallLevel() {
            string[] name = this.BuildingAndLevel.CustomSplit('_');
            string level = name[name.Length - 1];
            if (name[0].CustomEquals(ElementName.townhall)) {
                return false;
            }
            if (int.Parse(level) > this.model.GetTownhallLevel()) {
                Coord coord = FindBuildUnlockCondition(ElementName.townhall);
                ClickTile(new Vector2(coord.X, coord.Y), TileArrowTrans.upgrade);
                return true;
            } else {
                return false;
            }
        }

        public void FindBtnGoText(string condition, string conditionLevel) {
            foreach (var build in this.model.canBeBuildBuildingDict) {
                if (condition.CustomEquals(build.Value.buildingName)) {
                    this.view.SetbtnBuildGo(LocalManager.GetValue(LocalHashConst.button_go_build));
                    return;
                }
            }
            Coord coord = this.FindBuildUnlockCondition(condition);
            if (coord != null) {
                this.view.SetbtnBuildGo(LocalManager.GetValue(LocalHashConst.button_go_upgrade));
                return;
            }
            //Debug.Log(condition + "_" + conditionLevel);
            BuildingConf buildingConf = BuildingConf.GetConf(condition + "_" + conditionLevel);
            //Debug.Log(buildingConf.unlockCondition);
            string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
            //string relyBuildingName = unlockValue[0];
            //string relyBuildingLevel = unlockValue[1];
            this.FindBtnGoText(unlockValue[0], unlockValue[1]);
        }

        public void FindBuildingCanBeBuild(string condition, string conditionLevel) {
            foreach (var build in this.model.canBeBuildBuildingDict) {
                if (condition.CustomEquals(build.Value.buildingName)) {
                    this.view.afterHideCallback += () => {
                        this.parent.HideTileInfo();
                        this.parent.SetBuildingArrow(condition);
                    };
                    this.Hide();
                    return;
                }
            }
            Coord coord = this.FindBuildUnlockCondition(condition);
            if (coord != null) {
                this.ClickTile(coord, TileArrowTrans.upgrade);
                return;
            }
            BuildingConf buildingConf = BuildingConf.GetConf(condition + "_" + conditionLevel);
            string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
            //string relyBuildingName = unlockValue[0];
            //string relyBuildingLevel = unlockValue[1];
            this.FindBuildingCanBeBuild(unlockValue[0], unlockValue[1]);
        }

        public void OnUpgradeResourcesShort(string tileType) {
            this.parent.HideTileInfo();
            this.parent.GetTileCoordByLevel(tileType: tileType);
        }

        #region data
        public List<UnlockBuildingInfo> GetBuildingUnlockList(string buildingName, int level) {
            Dictionary<string, UnlockBuildingInfo> unlockDict =
                            this.model.GetBuildingUnlockDict(buildingName, level);

            return new List<UnlockBuildingInfo>(unlockDict.Values);
        }

        public string GetUnlockBuildingTips(BuildingConf conf) {
            return this.model.GetUnlockBuildingTips(conf);
        }

        public bool GetUpgradeConditionConf(out string upgrade, BuildingConf buildingConf,
            out bool showImg, out string unlockBuildName, out string unlockBuildType, out string level) {
            return this.model.GetUpgradeConditionConf(
                out upgrade, buildingConf, out showImg, out unlockBuildName, out unlockBuildType
                , out level);
        }

        public bool GetUpgradeForceConf(out string upgradeDiscrible, BuildingConf buildingConf) {
            return this.model.GetUpgradeForceConf(out upgradeDiscrible, buildingConf);
        }

        public int GetCanBeBuiltBuildingCount() {
            int unlockBuildingCount = 0;
            foreach (UnlockBuildingInfo unlockBuilding in this.model.canBeBuildBuildingDict.Values) {
                if (!unlockBuilding.buildingName.Contains("stronghold")) {
                    unlockBuildingCount += unlockBuilding.buildingCount;
                }
            }
            return unlockBuildingCount + this.model.brokenBuildingList.Count;
        }
        #endregion

        /* Add 'NetMessageAck' function here*/
        private void UpgradeAck(IExtensible message) {
            this.view.SetBtnUpgrade(true);
            this.Hide();
        }

        private void ResourcesChange() {
            if (this.view.IsVisible) {
                this.Show(() => { });
            } else {
                this.NeedFresh = true;
            }
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
            this.Hide();
            this.ClickTile(new Vector2(coord.X, coord.Y), TileArrowTrans.Attack);
        }
        /***********************************/

        #region FTE

        private void OnBuildUpStep2Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            this.view.OnBuildUpstep2Start();
            Debug.LogError("BuildUpStep " + index);
        }

        private void OnBuildUpStep2End() {
            this.view.afterHideCallback = FteManager.StartFte;
            this.UpgradeReq();
            this.StartChapterDailyGuid();
        }

        #endregion
    }
}
