using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    internal enum TileType {
        resource,
        npc_city,
        bridge,
        pass,
        None
    }

    public class MapTileViewModel : BaseViewModel, IViewModel {
        public MapViewModel parent;
        public TroopModel troopModel;
        public MapResouceInfo resourceInfo = new MapResouceInfo();
        public MapTileView view;

        private MapTileModel model;
        private MapModel mapModel;
        private MapMarkModel markModel;
        private BuildModel buildModel;
        //private HeroModel heroModel;
        private CampaignModel campaignModel;

        public Dictionary<Vector3, MapMark> MarkDict {
            get {
                return this.markModel.markDict;
            }
        }


        public MapTileInfo TileInfo {
            get {
                return this.model.tileInfo;
            }
            set {
                this.model.tileInfo = value;
                if (this.model.tileInfo != null) {
                    this.OnTileChange();
                }
            }
        }

        public Dictionary<string, Troop> TroopDict {
            get {
                return this.troopModel.troopDict;
            }
        }

        public List<Activity> AllActivties {
            get {
                return this.campaignModel.allActivities;
            }
        }


        public Troop CurrentTroop {
            get {
                return this.model.currentTroop;
            }
            set {
                if (this.model.currentTroop != value) {
                    this.model.currentTroop = value;
                }
            }
        }

        public EventMarchClient CurrentMarch {
            get {
                return this.model.currentMarch;
            }
            set {
                if (this.model.currentMarch != value) {
                    this.model.currentMarch = value;
                }
            }
        }

        public TilePage Page {
            get {
                return this.model.page;
            }
            set {
                this.model.page = value;
                if (this.view.IsVisible) {
                    this.view.OnPageChange();
                }
                this.NeedFocus = false;
            }
        }

        public TroopViewType TroopType {
            get {
                return this.troopSelectViewModel.TroopViewType;
            }
        }

        public string AllianceMarkName {
            get {
                return this.allianceMarkName;
            }
            set {
                this.allianceMarkName = value;
            }
        }
        private string allianceMarkName = string.Empty;

        public bool hasBossOnTile = false;
        public bool hasMonsterOnTile = false;
        public bool isTileCoordChange = false;
        private Vector2 tileCoordinate = Vector2.zero;
        public GetPointNpcTroopsAck troopInfoAck = null;
        public long defenderRecoverTimeAt = 0;
        private UnityAction onTileChange = null;
        public bool OnlyShowTroop { get; set; }
        public bool ShowAnimation { get; set; }
        public bool NeedFocus { get; set; }
        public bool IsVisible { get { return this.view.IsVisible; } }
        private TroopSelectViewModel troopSelectViewModel;
        private MarchViewModel marchViewModel;
        private MapTileDetailViewModel tileDetail;
        private MapTileDetailViewModel TileDetailViewModel {
            get {
                if (this.tileDetail == null) {
                    this.tileDetail = PoolManager.GetObject<MapTileDetailViewModel>(this.transform);
                }
                return this.tileDetail;
            }
        }

        public bool TileDetailVisible {
            get {
                return this.TileDetailViewModel.ViewVisible;
            }
        }

        private TileRewardViewModel tileRewardViewModel;
        private bool isInitTileDetailViewModel;
        private long leftDuration = 0;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<MapTileModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.markModel = ModelManager.GetModelData<MapMarkModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.campaignModel = ModelManager.GetModelData<CampaignModel>();
            this.view = this.gameObject.AddComponent<MapTileView>();

            this.troopSelectViewModel = PoolManager.GetObject<TroopSelectViewModel>(this.transform);
            this.marchViewModel = PoolManager.GetObject<MarchViewModel>(this.transform);
            this.tileRewardViewModel = PoolManager.GetObject<TileRewardViewModel>(this.transform);
            this.ShowAnimation = true;
            TriggerManager.Regist(Trigger.Fte, this.SetFteUI);

            FteManager.SetStartCallback(GameConst.TROOP_ADD_HERO, 2,
                (index) => this.OnTroopStep2Start(index));
            FteManager.SetEndCallback(GameConst.TROOP_ADD_HERO, 2, this.OnTroopStep2End);
            FteManager.SetStartCallback(GameConst.RESOURCE_LEVEL, 2, this.OnResourceStep2Start);
            FteManager.SetEndCallback(GameConst.RESOURCE_LEVEL, 2, this.OnResourceStep2End);
            FteManager.SetStartCallback(GameConst.RESOURCE_LEVEL, 3, this.OnResourceStep3Start);
            FteManager.SetEndCallback(GameConst.RESOURCE_LEVEL, 3, this.OnResourceStep3End);
            FteManager.SetStartCallback(GameConst.BUILDING_UPGRADE, 1, this.OnBuildUpStep1Start);
            FteManager.SetEndCallback(GameConst.BUILDING_UPGRADE, 1, this.OnBuildUpStep1End);
            FteManager.SetStartCallback(GameConst.RECRUIT, 2, this.OnRecruitStep2Start);
            FteManager.SetEndCallback(GameConst.RECRUIT, 2, this.OnRecruitStep2End);
            FteManager.SetEndCallback(GameConst.NORMAL, 41, this.OnFteStep41End);
        }

        private void UpdateAction() {
            if (this.view.IsVisible) {
                leftDuration = this.defenderRecoverTimeAt * 1000 -
                    RoleManager.GetCurrentUtcTime();
                if (leftDuration > 0) {
                    this.view.UpdateDefendersRecoverInfo(leftDuration);
                }
                TileProtectType type;
                leftDuration = this.GetTileProtectTime(out type);
                if (leftDuration > 0) {
                    this.view.UpdateProtectTime(leftDuration);
                }
            }
        }

        #region show_hide
        public long GetTileProtectTime(out TileProtectType type) {
            return this.mapModel.GetTileProtectTime(this.TileInfo.coordinate, out type);
        }

        public string GetResourcesInfoLevel() {
            return this.mapModel.GetResourcesInfoLevel(this.TileInfo.coordinate);
        }

        public string GetLayerAboveLevel() {
            return this.mapModel.GetLayerAboveLevel(this.TileInfo.coordinate);
        }

        public void Show(UnityAction ShowCallBack = null) {
            this.view.Show(callback: () => {
                UpdateManager.Regist(UpdateInfo.MapTileViewModel, this.UpdateAction);
                ShowCallBack.InvokeSafe();
            });
            this.parent.EnableChoseEffect();
            this.parent.HideHUD();
        }

        public void Hide(bool needDisableChoseEffect = true) {
            if (this.TileDetailVisible) {
                this.HideTileDetail();
            }

            this.HideCallback(needDisableChoseEffect);
        }

        private void HideCallback(bool needDisableChoseEffect = true) {
            this.ShowAnimation = true;
            this.Page = TilePage.None;
            this.parent.ShowHUD();
            if (needDisableChoseEffect) {
                this.parent.DisableChoseEffect();
            }
            UpdateManager.Unregist(UpdateInfo.MapTileViewModel);
            this.HideTileReward();
            this.view.Hide();
        }

        public string GetTileZone(Vector2 coordinate) {
            return this.resourceInfo.GetTileMapSnLocal(coordinate);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void HideSelfMarch() {
            this.marchViewModel.HideImmediatly();
        }

        public void ShowTroop(string id) {
            this.CurrentTroop = this.TroopDict[id];
            this.view.SetTroop(this.CurrentTroop.Positions.Count);
            if (this.troopModel.troopDict.ContainsKey(this.CurrentTroop.Id)) {
                this.view.ShowSelfTroop();
                this.troopSelectViewModel.SetHighlight(this.CurrentTroop.Id);
                this.marchViewModel.ShowTroop(this.CurrentTroop);
            } else {
                this.view.ShowOtherTroop();
            }
            this.view.RemoveNotTroopButton();
        }

        public void HideTroopOverview() {
            if (this.Page == TilePage.March) {
                this.Hide();
            } else {
                this.view.SetTroopOverviewVisible(false);
            }
        }

        public ElementBuilding GetBuildingByName(string name) {
            ElementBuilding building;
            if (this.buildModel.buildingDict.TryGetValue(name, out building)) {
                return building;
            } else {
                return null;
            }
        }

        public bool IsShowTileEndurance() {
            bool enduranceFull = (this.TileInfo.endurance >= this.TileInfo.maxEndurance);
            if (enduranceFull ||
                EventManager.IsTileInBuildEvent(this.TileInfo.coordinate) ||
                EventManager.IsTileGiveUpBuilding(this.TileInfo.coordinate)) {
                return false;
            }

            return true;
        }

        public bool IsShowGiveUpStrongholdPnl() {
            if (this.TileInfo.buildingInfo != null) {
                bool isTileStronghold = this.IsTileLocateStrongHold();
                bool isTileBroken = this.TileInfo.buildingInfo.IsBroken;
                bool isTileUpgrading = this.TileInfo.buildingInfo.IsUpgrade;
                bool isTileGiveuping = EventManager.IsTileGiveUpBuilding(this.TileInfo.coordinate);
                return isTileStronghold && !isTileBroken && !isTileUpgrading && !isTileGiveuping;
            }
            return false;
        }

        private bool IsTileLocateStrongHold() {
            return (this.TileInfo.buildingInfo.Type == (int)ElementType.stronghold &&
                    this.TileInfo.playerId == RoleManager.GetRoleId());
        }

        public bool IsShowMarkBtnView(Vector2 coordinate) {
            bool containOther =
                this.IsMarkContainCoordByType(coordinate, MapMarkType.Others);
            return !containOther;
        }

        public bool IsAllianceMarkContainCoord(Vector2 coordinate) {
            return this.IsMarkContainCoordByType(coordinate, MapMarkType.Alliance);
        }

        private bool IsMarkContainCoordByType(Vector2 coordinate, MapMarkType type) {
            Vector3 markVector = new Vector3(
                coordinate.x,
                coordinate.y,
                (int)type
            );
            return this.MarkDict.ContainsKey(markVector);
        }

        public void ShowMarch(EventMarchClient march, GameObject marchObj) {
            this.view.Show(callback: this.parent.HideHUD);
            if (this.Page != TilePage.March || march != this.CurrentMarch) {
                this.Page = TilePage.March;
                this.CurrentMarch = march;
                this.view.SetMarchView();
                if (march.playeId == RoleManager.GetRoleId()) {
                    this.marchViewModel.ShowMarch(march);
                } else {
                    this.view.ShowOtherMarch(march);
                }
            }
        }

        public Transform GetMarchBind() {
            return this.marchViewModel.GetMarchBind();
        }

        public void ShowDemonShadowTileView() {
            this.parent.ShowDemonTileView();
        }

        public void ShowMonsterTileView() {
            this.parent.ShowMonsterTileView();
        }

        public void HideActivityTileView() {
            this.parent.HideActivityTileView();
        }

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void ShowPay() {
            this.parent.CloseAboveUI();
            this.parent.ShowPay();
        }

        private void GetPlayerPublicInfoAck() {

        }

        public void HideMarch(string id) {
            if (this.CurrentMarch.id == id) {
                this.Hide();
            }
        }

        public void HideDetail() {
            this.HideTileDetail();
            this.parent.DisableAboveUICamera();
        }


        public void UIReturn() {
            if (this.Page == TilePage.Sub) {
                this.ShowAnimation = false;
                this.Page = TilePage.Main;
            }
        }
        #endregion

        #region tile detail operation
        public void ShowTileDetail() {
            this.TileDetailViewModel.Show(this.troopInfoAck);
        }

        public void ShowMonsterDetail(GetMonsterByCoordAck monsterInfoAck) {
            this.TileDetailViewModel.Show(monsterInfoAck);
        }

        public void HideTileDetail() {
            if (this.tileDetail != null) {
                this.tileDetail.Hide();
            }
        }

        public bool IsMonsterStayOnTile(Vector2 coord) {
            return (this.parent.GetMonsterInfo(coord) != null);
        }
        #endregion

        #region tileReward
        public void ShowTileRewardViewWithDelay(float seconds) {
            this.tileRewardViewModel.ShowTileRewardViewWithDelay(seconds);
        }

        public void HideTileReward() {
            this.tileRewardViewModel.HideImmediatly();
        }

        public Transform GetTargetTransform() {
            return this.view.GetTargetTransform();
        }

        public int GetRightButtonsCount() {
            return this.view.GetRightButtonsCount();
        }
        #endregion

        #region with_other_viewmodel
        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void ShowBuilding(BuildViewType buildViewType, string building) {
            this.parent.ShowBuildInfo(buildViewType, building, this.view.ShowBottom);
        }

        public bool IsNeedShowMoveHealTips() {
            ElementType buildType = this.buildModel.GetBuildTypeWithCoord(
                this.TileInfo.coordinate);
            //Debug.LogError("IsNeedShowBtnMoveHealTips " + buildType);
            if (buildType == ElementType.townhall ||
                buildType == ElementType.stronghold) {
                return this.troopModel.IsOutsideTroopNeedCure(this.TileInfo.coordinate);
            }
            return false;
        }

        public bool IsNeedShowReturnTips() {
            ElementType buildType = this.buildModel.GetBuildTypeWithCoord(
                this.TileInfo.coordinate);
            if (!(buildType == ElementType.townhall ||
                buildType == ElementType.stronghold)) {
                return this.troopModel.IsNeedShowReturnTips(this.TileInfo.coordinate);
            }
            return false;
        }

        public bool IsAllTroopStayInCoord() {
            return this.troopModel.IsAllTroopStayInCoord(this.TileInfo.coordinate);
        }

        public bool IsBuildingCanUpgrade(out bool reachMaxLevel, out bool giveUping) {
            bool tileNotBuilding = !EventManager.IsTileInBuildEvent(this.TileInfo.coordinate);
            reachMaxLevel = this.buildModel.IsBuildingRechMaxLevel(this.TileInfo.buildingInfo.Name);
            giveUping = EventManager.IsGiveUpBuild(this.TileInfo.coordinate);
            return tileNotBuilding && !reachMaxLevel && !giveUping;
        }

        public bool GetTileGiveupable() {
            bool isTileBuildingBroken =
                (this.TileInfo.buildingInfo != null && this.TileInfo.buildingInfo.IsBroken);
            if (isTileBuildingBroken) {
                return false;
            }

            bool canTileAbandon =
                this.TileInfo.type.CustomEndsWith(ElementCategory.resource) ||
                this.TileInfo.type.CustomEndsWith(ElementCategory.npc_city) ||
                this.TileInfo.IsTilePassBridge();

            bool isTileAbandoning =
                EventManager.IsTileAbandon(this.TileInfo.coordinate);
            return canTileAbandon && !isTileAbandoning;
        }

        public void ShowAvaliableTroop() {
            List<Troop> avaliableTroopList = this.troopModel.GetAvaliableTroop(this.TileInfo.coordinate);
            this.view.SetBottomBlank(avaliableTroopList.Count);
            this.view.SetTroopLabel(LocalManager.GetValue(LocalHashConst.map_tile_choose_troop));
            this.troopSelectViewModel.ShowAvaliable(avaliableTroopList);
            this.view.ShowBottom();
        }

        public void ShowTroopChose(TroopChoseAction type) {
            this.troopSelectViewModel.ShowTroopChose(type);
        }

        public bool IsTileOperateAble() {
            bool notValidTile = this.TileInfo.type.CustomEndsWith(ElementCategory.river) ||
                this.TileInfo.type.CustomEndsWith(ElementCategory.mountain) ||
                this.TileInfo.relation == MapTileRelation.master;

            return !notValidTile;
        }

        private bool IsTileMoveable() {
            if (this.IsTileOperateAble()) {
                bool isTileFallened = this.TileInfo.isFallen;
                bool selfReachable = this.IsTileRelationEqual(MapTileRelation.self);
                if (selfReachable) {
                    return true;
                } else {
                    bool isTileTypeAvaliable =
                        this.TileInfo.type.CustomEndsWith(ElementCategory.building) ||
                        this.TileInfo.type.CustomEndsWith(ElementCategory.npc_city) ||
                        this.TileInfo.type.CustomEndsWith(ElementCategory.pass);
                    bool allyReachable =
                        this.IsTileRelationEqual(MapTileRelation.ally) &&
                        !isTileFallened && isTileTypeAvaliable;

                    return allyReachable ? true : (this.IsTileRelationEqual(MapTileRelation.slave) &&
                                isTileTypeAvaliable);
                }
            }
            return false;
        }

        private bool IsTileRelationEqual(MapTileRelation relation) {
            return this.TileInfo.relation == relation;
        }

        public void ShowStationTroop(TroopViewType viewType) {
            this.view.SetTroopLabel(LocalManager.GetValue(LocalHashConst.map_tile_troop));
            this.troopSelectViewModel.ShowStation(viewType, this.TileInfo.troopCount);
        }

        public void HideTroopSelect() {
            this.troopSelectViewModel.Hide();
        }

        public void ShowTroopInfo(string id, TroopViewType mode) {
            UnityAction highlightAction = () => {
                this.Show();
                if (mode == TroopViewType.Return) {
                    this.view.ShowBottom();
                    //this.view.RebindBottom();
                    this.ShowStationTroop(TroopViewType.Return);
                    this.troopSelectViewModel.ShowTroopChose(TroopChoseAction.Return);
                } else {
                    this.ShowAvaliableTroop();
                }
            };

            UnityAction normalAction = () => {
                this.Show();
                this.view.ShowBottom();
                this.ShowTroop(id);
                this.troopSelectViewModel.Show();
            };

            this.parent.ShowTroopInfo(id, mode, () => {
                if (this.TroopType == TroopViewType.Return && !this.view.IsChoseTroop) {
                    normalAction.InvokeSafe();
                } else {
                    highlightAction.InvokeSafe();
                }
            });
        }

        public void ShowTroopFormation(string id) {
            this.parent.ShowTroopFormation(id);
        }

        public void ShowTileBindUI() {
            if (this.TileInfo != null) {
                this.parent.ShowTileBindUI(this.TileInfo.coordinate);
            }
        }

        public void MoveToTile() {
            this.parent.Move(this.TileInfo.coordinate);
            this.parent.EnableAboveUICamera();
        }

        public void EnableHighlight() {
            this.parent.Focus(MapUtils.CoordinateToPosition(this.TileInfo.coordinate));
        }

        public void DisableHighlight() {
            this.parent.LoseFocus();
        }

        public void ShowRecruit(string troopId = null) {
            if (troopId == null) {
                troopId = this.CurrentTroop.Id;
            }

            BuildingConf buildingConf = null;
            if (this.TileInfo.buildingInfo != null) {
                buildingConf = ConfigureManager.GetConfById<BuildingConf>(
                    this.TileInfo.buildingInfo.GetId()
                );
            }
            bool isShowTip = (buildingConf != null) &&
                (buildingConf.type == ElementType.stronghold.ToString());
            this.parent.ShowRecruit(
                troopId,
                isShowTip
            );
        }

        public void ShowHeroInfo(string name,
                                 UnityAction levelUpCallback = null,
                                 HeroInfoType infoType = HeroInfoType.Self) {
            this.parent.ShowHeroInfo(name, levelUpCallback, infoType, true);
        }

        public void ShowHeroInfo(Hero hero, HeroInfoType infoType = HeroInfoType.Others) {
            this.parent.ShowHeroInfo(hero, infoType, isSubWindow: true);
        }

        public void CancelUpgradeBuilding() {
            EventBuildClient queueBuild =
                EventManager.GetBuildEventByCoordinate(this.TileInfo.coordinate);

            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.notice_title_warning),
                LocalManager.GetValue(LocalHashConst.warning_cancel_upgrade_content),
                () => {
                    CancelBuildReq cancelGiveUp = new CancelBuildReq() {
                        Id = queueBuild.id
                    };
                    NetManager.SendMessage(cancelGiveUp, string.Empty, null);
                },
                () => { }
            );
        }

        public void ShowBuildList() {
            this.parent.ShowBuildList(true, this.TileInfo.coordinate);
        }

        public void HideTileInfo() {
            this.parent.HideTileInfo();
        }

        public void StopJumping() {
            this.parent.StopJumping();
        }

        //public void BuildCancelReq(string id) {
        //    this.parent.BuildCancelReq(id);
        //}

        public void SetMask(TileArrowTrans tileArrowTrans) {
            this.view.SetArrowMask(tileArrowTrans);
        }
        #endregion

        #region data_execution
        public void Attack(Vector2 position) {
            bool isOwnPoint = RoleManager.GetPointDict().ContainsKey(this.TileInfo.coordinate);
            if (RoleManager.IsPointLimitReached() && !isOwnPoint &&
                this.TileInfo.relation != MapTileRelation.ally &&
                !this.HasMonsterOrBossOnTile()) {
                UIManager.ShowConfirm(
                    LocalManager.GetValue(LocalHashConst.notice_title_warning),
                    LocalManager.GetValue(LocalHashConst.lands_occupy_maximum),
                    () => this.InnerAttack(position, TroopViewType.Attack),
                    () => this.Hide(),
                    LocalManager.GetValue(LocalHashConst.lands_occupy_maximum_tips),
                    canHide: false
                );
            } else {
                this.InnerAttack(position,
                    isOwnPoint ? TroopViewType.Raid : TroopViewType.Attack);
            }
        }

        private void InnerAttack(Vector2 position, TroopViewType type) {
            this.Page = TilePage.Sub;
            TriggerManager.Invoke(Trigger.CameraFocus, position);
            this.troopSelectViewModel.TroopViewType = type;
            this.ShowAvaliableTroop();
        }

        public void Move(Vector2 position) {
            bool isOwnPoint = RoleManager.GetPointDict().ContainsKey(this.TileInfo.coordinate);
            if (isOwnPoint || this.TileInfo.relation == MapTileRelation.ally ||
                (this.TileInfo.relation == MapTileRelation.slave &&
                this.TileInfo.buildingInfo != null)) {
                this.InnerAttack(position, TroopViewType.Move);
            }
        }

        public void Return() {
            TriggerManager.Invoke(
                Trigger.CameraFocus,
                MapUtils.CoordinateToPosition(this.TileInfo.coordinate)
            );
            this.troopSelectViewModel.TroopViewType = TroopViewType.Return;
        }

        public bool IsMasterTile() {
            string masterAllianceId = RoleManager.GetMasterAllianceId();
            if (!string.IsNullOrEmpty(masterAllianceId) &&
                 this.TileInfo.allianceId.CustomEquals(masterAllianceId)) {
                return true;
            }
            return false;
        }

        public bool IsAllyTile() {
            string allianceId = RoleManager.GetAllianceId();
            bool isAllay = !string.IsNullOrEmpty(allianceId) &&
                           this.TileInfo.allianceId.CustomEquals(allianceId);
            return isAllay;
        }

        public bool IsAllianceNpcCity() {
            bool isRelationAlly = this.IsTileRelationEqual(MapTileRelation.ally);
            bool isTileNpcCity = this.TileInfo.type.CustomEndsWith(ElementCategory.npc_city);
            string selfAllianceId = RoleManager.GetAllianceId();
            bool isOwnNpcCity = selfAllianceId.CustomEquals(this.TileInfo.allianceId);

            return isRelationAlly && isTileNpcCity && isOwnNpcCity;
        }

        public bool IsTileTroopAmountFull() {
            int troopAmount = this.GetTroopsAt(this.TileInfo.coordinate).Count;
            if (this.TileInfo.buildingInfo != null ||
                (this.TileInfo.city != null && this.TileInfo.city.isCenter)) {
                return false;
            } else if (troopAmount >= 1) {
                return true;
            } else {
                return false;
            }

        }

        private bool IsTileRelationReachable() {
            bool relationInappropriate =
                     this.IsTileRelationEqual(MapTileRelation.master) ||
                     this.IsTileRelationEqual(MapTileRelation.none);
            return !relationInappropriate;
        }

        public bool CanReachable() {
            bool relationOk = this.IsTileRelationReachable();
            bool isTileCoordReachable = this.IsTileCoordReachable();
            if (relationOk && isTileCoordReachable) {
                if (this.IsAllianceNpcCity()) {
                    return true;
                }
                return this.IsTileMoveable();
            }
            return false;
        }

        public bool IsTileCoordReachable() {
#if UNITY_EDITOR || DEVELOPER
            return true;
#else
            //if (VersionConst.IsDeveloper()) {
            //    return true;
            //}
            List<Vector2> list = this.mapModel.GetCoordinateList(this.TileInfo.coordinate, 1);
            Dictionary<Vector2, Point> pointDict = RoleManager.GetPointDict();
            foreach (Vector2 coordinate in list) {
                if (pointDict.ContainsKey(coordinate)) {
                    return true;
                }
            }
            string OwnAllianceId = RoleManager.GetAllianceId();
            if (!OwnAllianceId.CustomIsEmpty()) {
                Point point;
                foreach (Vector2 coordinate in list) {
                    point = this.mapModel.GetPlayerPoint(coordinate);
                    if (point != null && (point.IsAlly || point.IsSlave)) {
                        return true;
                    }
                }
            }
            return false;
#endif
        }

        public void ShowPlayerDetailInfo() {
            string lordOwnName = RoleManager.GetRoleName();
            if (lordOwnName.CustomEquals(this.TileInfo.playerName)) {
                this.parent.ShowRolePlayerInfo();
            } else {
                this.parent.ShowPlayerDetailInfo(this.TileInfo.playerId);
            }
        }

        public void SetTileInfo(MapTileInfo tileInfo,
            bool onlyShowTroop = false, string id = "") {
            this.OnlyShowTroop = onlyShowTroop;
            this.TileInfo = tileInfo;
            if (this.hasBossOnTile || this.hasMonsterOnTile) {
                this.Page = TilePage.Monster;
            } else {
                this.Page = TilePage.Main;
                if (OnlyShowTroop) {
                    this.ShowTroop(id);
                }
            }
        }

        public void Mark() {
            string name = MapUtils.GetTileName(this.TileInfo);
            if (this.TileInfo.type.CustomEquals(ElementCategory.resource)) {
                name = string.Concat(name, "  ", GameHelper.GetLevelLocal(this.TileInfo.level));
            }
            this.parent.Mark(name, this.TileInfo.coordinate);
            this.Hide();
        }

        public void DeleteMark() {
            this.parent.DeleteMark(this.TileInfo.coordinate);
            this.Hide();
        }

        public void AddAllianceMark() {
            if (this.allianceMarkName.CustomIsEmpty()) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.alliance_intput_mark_name),
                    TipType.Info);
            } else {
                this.parent.AddAllianceMark(this.allianceMarkName, this.TileInfo.coordinate);
                this.Hide();
            }
        }

        public void ShowAllianceMarkEditView() {
            this.view.PlayHide(() => {
                this.ShowAnimation = true;
                this.parent.ShowHUD();
                this.view.ShowAllianceMarkEditView();
            });
        }

        public void GiveUpAllianceMark() {
            this.parent.DeleteAllianceMark(this.TileInfo.coordinate);
            this.Hide();
        }

        public void GiveUpStronghold() {
            GiveUpBuildingReq giveUpStrongholdReq = new GiveUpBuildingReq() {
                Name = this.TileInfo.buildingInfo.Name
            };
            //Debug.LogError(this.TileInfo.buildingInfo.Name);
            NetManager.SendMessage(giveUpStrongholdReq, string.Empty, null);
            this.Hide();
        }

        private void OnTileChange() {
            this.hasBossOnTile = (this.parent.GetBossInfo(this.TileInfo.coordinate) != null);
            this.hasMonsterOnTile = (this.parent.GetMonsterInfo(this.TileInfo.coordinate) != null);
            this.isTileCoordChange = (this.tileCoordinate != this.TileInfo.coordinate);
            if (isTileCoordChange) {
                this.tileCoordinate = this.TileInfo.coordinate;
                this.troopInfoAck = null;
            }
            this.onTileChange.InvokeSafe();
            this.onTileChange = null;
            this.view.SetTileViewInfo();
        }

        public bool HasMonsterOrBossOnTile() {
            return (this.hasBossOnTile || this.hasMonsterOnTile);
        }

        public void TroopInfoReq() {
            //if (this.isTileCoordChange) {
            GetPointNpcTroopsReq troopInfoReq = new GetPointNpcTroopsReq() {
                Coord = this.tileCoordinate
            };
            NetManager.SendMessage(troopInfoReq, typeof(GetPointNpcTroopsAck).Name,
                    this.TroopInfoAck);
            //} else if (this.troopInfoAck != null) {
            //    this.InnerSetTileDefendersInfo();
            //}
        }

        private void TroopInfoAck(IExtensible message) {
            this.troopInfoAck = message as GetPointNpcTroopsAck;
            this.defenderRecoverTimeAt = this.troopInfoAck.RefreshAt;
            long leftDuration = this.defenderRecoverTimeAt * 1000
                - RoleManager.GetCurrentUtcTime();
            this.view.viewPref.pnlDefendersRecover.
                gameObject.SetActiveSafe(leftDuration > 0);
            this.InnerSetTileDefendersInfo();
            if (this.TileDetailVisible) {
                this.tileDetail.SetTroopInfo(this.troopInfoAck);
            }
        }

        private void InnerSetTileDefendersInfo() {
            if (this.view.IsVisible) {
                NpcTroop troop = null;
                if (this.troopInfoAck.Troops.Count > 0) {
                    troop = this.troopInfoAck.Troops[0];
                } else if (this.troopInfoAck.DefenceTroop != null) {
                    troop = this.troopInfoAck.DefenceTroop;
                }
                this.view.SetTileDefendersInfo(this.GetTroopPower(troop),
                        this.troopInfoAck.Count, this.troopInfoAck.TotalCount);

            }
        }

        private int GetTroopPower(NpcTroop troop) {
            if (troop.Heroes.Count < 1) {
                Debug.LogError("troop heros less than 1");
                return 0;
            }
            int totoalPower = 0;
            foreach (NpcHero hero in troop.Heroes) {
                if (hero.ArmyAmount > 0) {
                    totoalPower += HeroAttributeConf.GetPower(hero.Name, hero.Level);
                }
            }

            return totoalPower;
        }

        public List<Troop> GetTroopsAt(Vector2 coordinate) {
            return troopModel.GetTroopsAt(coordinate);
        }

        public int GetTroopArmyAmount() {
            return this.troopModel.GetTroopArmyAmount(this.CurrentTroop.Id);
        }

        public int GetMarchArmyAmount() {
            return this.troopModel.GetTroopArmyAmount(this.CurrentMarch.troop.Id);
        }

        //public void ShowChosenEffect() {
        //    this.view.afterHideCallback += () => {
        //        this.parent.ShowChoseEffect();
        //    };
        //}
        #endregion

        #region net_message
        public void AbandonPointReq() {
            Vector2 coordinate = this.TileInfo.coordinate;
            AbandonPointReq abandonPoint = new AbandonPointReq();
            abandonPoint.Coord = new Coord() {
                X = (int)coordinate.x,
                Y = (int)coordinate.y
            };
            NetManager.SendMessage(abandonPoint, string.Empty, null);
        }

        public void TroopReturnReq(string troopId = null) {
            if (troopId == null) {
                troopId = this.CurrentTroop.Id;
            }
            this.troopSelectViewModel.TroopViewType = TroopViewType.Return;
            this.ShowTroopInfo(troopId, TroopViewType.Return);
        }

        public void MarchRetreatReq() {
            CancelMarchReq cancelMarchReq = new CancelMarchReq() {
                Id = this.CurrentMarch.id
            };
            NetManager.SendMessage(cancelMarchReq, typeof(CancelMarchAck).Name, this.RetreatAck);
        }

        private void RetreatAck(IExtensible message) {
            this.parent.StopJumping();
            this.view.SetBtnRetreatInteractable(true);
            this.Hide();
        }
        #endregion

        #region FTE
        private void OnTroopStep2Start(string index) {
            this.view.afterHideCallback = () => {
                this.onTileChange = null;
                FteManager.StopFte();
            };
            this.view.showCompletedCallback = () => {
                this.onTileChange = FteManager.StopFte;
            };
            this.view.OnTroopStep2Start();
        }

        public void OnTroopStep2Process() {
            this.troopSelectViewModel.OnTroopStep2Process();
        }

        private void OnTroopStep2End() {
            this.view.afterHideCallback = null;
            this.onTileChange = null;
            this.Hide();
            if (this.TileInfo.coordinate != RoleManager.GetRoleCoordinate()) {
                this.TroopReturnReq();
                this.parent.StartChapterDailyGuid();
            }
        }

        private void OnBuildUpStep1Start(string index) {
            this.view.afterHideCallback = () => {
                this.onTileChange = null;
                FteManager.StopFte();
            };
            this.view.showCompletedCallback = () => {
                this.onTileChange = FteManager.StopFte;
            };
            this.view.OnBuildUpStep1Start();
        }

        private void OnBuildUpStep1End() {
            this.view.afterHideCallback = null;
            this.onTileChange = null;
            this.view.OnBuildUpStep1End();
            this.Hide();
        }

        private void OnRecruitStep2Start(string index) {
            this.view.afterHideCallback = () => {
                this.onTileChange = null;
                FteManager.StopFte();
            };
            this.view.showCompletedCallback = () => {
                this.onTileChange = FteManager.StopFte;
            };
            this.view.OnRecruitStep2Start();
        }

        private void OnRecruitStep2End() {
            this.view.afterHideCallback = null;
            this.onTileChange = null;
            this.Hide();
            if (this.TileInfo.coordinate != RoleManager.GetRoleCoordinate()) {
                this.TroopReturnReq();
                FteManager.StopFte();
                this.parent.StartChapterDailyGuid();
            } else {
                this.ShowRecruit();
            }
        }

        public void OnResourceStep1Start() {
            this.tileRewardViewModel.SetShowTileBool(false);
            this.view.OnResourceStep1SetActive(false);
        }

        public void OnResourceStep1End() {
            this.tileRewardViewModel.SetShowTileBool(true);
            this.view.OnResourceStep1SetActive(true);
        }

        private void OnResourceStep2Start(string index) {
            this.view.afterHideCallback = () => {
                this.onTileChange = null;
                FteManager.StopFte();
            };
            //DramaConf dramaConf = DramaConf.GetConfByFull(index);
            if (!FteManager.FteOver) {
                FteManager.ShowFteMask();
            }
            this.view.showCompletedCallback = () => {
                this.onTileChange = FteManager.StopFte;
            };
            this.view.OnResourceStep2Start(!FteManager.FteOver);
            this.marchViewModel.SetFteCanShow(false);
        }

        private void OnResourceStep2End() {
            this.view.afterHideCallback = null;
            this.onTileChange = null;
        }

        private void OnResourceStep3Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            this.view.OnResourceStep3Start();
            //DramaConf dramaConf = DramaConf.GetConfByFull(index);
            if (!FteManager.FteOver) {
                FteManager.ShowFteMask();
            }
            this.troopSelectViewModel.OnResourceStep3Start(!FteManager.FteOver);
        }

        private void OnResourceStep3End() {
            Debug.LogError("OnResourceStep3End");
            this.view.afterHideCallback = null;
            this.troopSelectViewModel.OnResourceStep3End();
            this.marchViewModel.SetFteCanShow(true);
        }

        //private void OnBuildUp2Start(string index) {
        //    this.view.afterHideCallback = FteManager.StopFte;
        //    this.view.OnBuildUp2Start();
        //}

        private void OnBuildUp2End() {
            this.view.afterHideCallback = null;
            this.Hide();
        }

        private void OnFteStep41End() {

        }

        private void SetFteUI() {
            this.view.SetFteUI();
        }

        #endregion
    }
}
