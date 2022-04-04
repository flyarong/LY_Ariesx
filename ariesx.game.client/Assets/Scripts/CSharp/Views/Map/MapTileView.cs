using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;
using System.Text;

namespace Poukoute {
    public enum TileButtonType {
        None,
        Mark,
        DeleteMark,
        MarkAlliance,
        Giveup,
        StrongholdGiveUp,
        DeleteAllianceMark,
        Move,
        Upgrade,
        CancelUpgrade,
        Build,
        Attack,
        Format,
        Return,
        Recruit
    }

    public class MapTileView : BaseView {
        private MapTileViewModel viewModel;
        public MapTileViewPreference viewPref;
        private bool isTerrain;
        private GameObject target;
        [HideInInspector]
        public Transform pnlMarchBind = null;

        private List<TileButtonType> leftBtnList = new List<TileButtonType>();
        private List<TileButtonType> rightBtnList = new List<TileButtonType>();
        private Dictionary<TileButtonType, CustomButton> btnDict =
            new Dictionary<TileButtonType, CustomButton>();
        private List<TileButtonType> grayBtnList = new List<TileButtonType>();
        private Dictionary<TileButtonType, ulong> btnLabelDict = new Dictionary<TileButtonType, ulong>();


        private readonly List<TileButtonType> notTroopBtnList = new List<TileButtonType>() {
            TileButtonType.Move,
            TileButtonType.Attack,
            TileButtonType.Build,
            TileButtonType.CancelUpgrade,
            TileButtonType.Upgrade,
           // TileButtonType.Recall
        };

        private Dictionary<TileButtonType, UnityAction> btnCallbackDict =
            new Dictionary<TileButtonType, UnityAction>();
        private Dictionary<string, TileButtonType> fteUIDict = new Dictionary<string, TileButtonType>();
        private Vector2 TILESIZE_MIDDLE = MapUtils.TileSize * 2.2f;
        private Vector2 TILESIZE_BIG = MapUtils.TileSize * 2.2f;
        private Vector2 overViewTileSize = Vector2.zero;
        //private List<string> bigBuilding = new List<string>() {
        //    ElementName.townhall,
        //    "tavern"
        //};

        private bool showCompleted = false;
        private bool isShowTroop = false;
        private bool isChosenTroop = false;
        public bool IsChoseTroop {
            get {
                return this.isChosenTroop;
            }
        }
        public UnityAction showCompletedCallback = null;
        public UnityAction afterReturnClick = null;
        private int rightButtonsCount = 0;
        private int leftButtonsCount = 0;

        void Awake() {
            this.group = UIGroup.MapTile;
            this.target = new GameObject() {
                name = "Target"
            };

            this.btnCallbackDict.Add(TileButtonType.Mark, this.OnBtnMarkClick);
            this.btnCallbackDict.Add(TileButtonType.DeleteMark, this.OnBtnDeleteMarkClick);
            this.btnCallbackDict.Add(TileButtonType.MarkAlliance, this.OnBtnAllianceMarkClick);
            this.btnCallbackDict.Add(TileButtonType.Giveup, this.OnBtnGiveUpClick);
            this.btnCallbackDict.Add(TileButtonType.StrongholdGiveUp, this.OnStrongholdGiveUpClick);
            this.btnCallbackDict.Add(TileButtonType.DeleteAllianceMark, this.OnBtnGiveUpAllianceClick);
            this.btnCallbackDict.Add(TileButtonType.Attack, this.OnBtnAttackClick);
            this.btnCallbackDict.Add(TileButtonType.Move, this.OnBtnMoveClick);
            this.btnCallbackDict.Add(TileButtonType.Upgrade, this.OnBtnUpgradeClick);
            this.btnCallbackDict.Add(TileButtonType.CancelUpgrade, this.OnBtnCancelUpgradeClick);
            this.btnCallbackDict.Add(TileButtonType.Build, this.OnBtnBuildClick);
            this.btnCallbackDict.Add(TileButtonType.Format, this.OnBtnFormatClick);
            this.btnCallbackDict.Add(TileButtonType.Recruit, this.OnBtnRecruitClick);
            this.btnCallbackDict.Add(TileButtonType.Return, this.OnBtnReturnClick);

            // Fte
            this.fteUIDict.Add(FteConst.Alliance, TileButtonType.Giveup);
        }

        //IEnumerator Start() {
        //    ResourceRequest srcReq = UnityEngine.Resources.LoadAsync("UI/Root/UITile");
        //    yield return srcReq;
        //    this.ui = Instantiate(srcReq.asset as GameObject);
        //    this.ui.transform.SetParent(UIManager.Instance.transform);
        //    this.ui.name = "UITile";
        //    this.viewPref = this.ui.gameObject.GetComponent<MapTileViewPreference>();
        //    this.InitBtnOnClickInfo();
        //    this.pnlMarchBind = this.viewModel.GetMarchBind();
        //}

        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<MapTileViewModel>();
            this.ui = UIManager.GetUI("UITile");
            this.viewPref = this.ui.gameObject.GetComponent<MapTileViewPreference>();
            this.InitBtnOnClickInfo();
            this.pnlMarchBind = this.viewModel.GetMarchBind();
        }

        private void InitBtnOnClickInfo() {
            this.viewPref.btnDetail.onClick.AddListener(this.OnBtnDetailClick);
            this.viewPref.btnPlayerInfo.onClick.AddListener(this.OnBtnPlayerInfoClick);

            this.viewPref.ifMarkName.characterLimit = 15;
            this.viewPref.ifMarkName.onValueChanged.AddListener(this.OnIfNameValueChange);
            this.viewPref.btnCancel.onClick.AddListener(this.OnAllianceMarkCancelClick);
            this.viewPref.btnConfirm.onClick.AddListener(this.OnAllianceMarkConfirmClick);
        }

        public void HideSubView() {
            this.viewModel.HideTroopOverview();
            this.viewModel.HideDetail();
            this.viewModel.HideTileReward();
        }

        public Transform GetTargetTransform() {
            return this.target.transform;
        }


        private void SetTroops() {
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, true);
            this.viewPref.pnlBottomBlank.gameObject.SetActiveSafe(true);
            var troopList = this.viewModel.GetTroopsAt(this.viewModel.TileInfo.coordinate);
            bool hasTroop = (troopList.Count + this.viewModel.TileInfo.troopCount != 0);
            if (!hasTroop) {
                this.viewModel.HideTroopSelect();
            } else if (this.viewModel.TileInfo.coordinate == RoleManager.GetRoleCoordinate()) {
                this.viewModel.ShowStationTroop(TroopViewType.Format);
            } else {
                this.viewModel.ShowStationTroop(TroopViewType.Idle);
            }
            this.SetTroopButtons(hasTroop);
        }

        public void SetBottomBlank(int count) {
            this.viewModel.HideActivityTileView();
            this.viewPref.pnlBottomBlank.gameObject.SetActiveSafe(count > 0);
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, count > 0);
        }

        public string GetTileZone(Vector2 coordinate) {
            return this.viewModel.GetTileZone(coordinate);
        }

        public void SetTileViewInfo() {
            this.rightBtnList.Clear();
            this.leftBtnList.Clear();
            this.grayBtnList.Clear();
            this.btnDict.Clear();
            this.target.transform.position =
                MapUtils.CoordinateToPosition(this.viewModel.TileInfo.coordinate);
            UIManager.SetUIVisible(this.viewPref.pnlAddAllianceMark.gameObject, false);
            if (this.viewModel.hasBossOnTile) {
                this.viewModel.ShowDemonShadowTileView();
                this.SetTileCampaignView();
            } else if (this.viewModel.hasMonsterOnTile) {
                this.viewModel.ShowMonsterTileView();
                this.SetTileCampaignView();
            } else {
                this.SetOverview();
            }
            this.SetBottom();
            this.SetButtons();
            this.ResetAllianceMarkView();


        }

        public void UpdateDefendersRecoverInfo(long time) {
            this.viewPref.txtRecoverTime.gameObject.SetActiveSafe(time > 0);
            if (time < 0) {
                return;
            }
            this.viewPref.txtRecoverTime.text = GameHelper.TimeFormat(time);
        }

        public void UpdateProtectTime(long time) {
            this.viewPref.txtProtectTime.gameObject.SetActiveSafe(time > 0);
            if (time < 0) {
                this.viewPref.pnlFreshProtect.gameObject.SetActiveSafe(false);
                this.viewPref.pnlAvoidProtect.gameObject.SetActiveSafe(false);
                return;
            }

            this.viewPref.txtProtectTime.text = GameHelper.TimeFormat(time);
        }

        private void SetBottom() {
            this.SetTroops();
        }

        private void SetButtons() {
            this.SetLargeButtons();
            this.rightButtonsCount = this.rightBtnList.Count;
            this.leftButtonsCount = this.leftBtnList.Count;
            this.ResetRightButtons();
            this.ResetLeftButtons();
        }

        public void ShowBottom() {
            int offset = 0;
            if (!GameHelper.NearlyEqual(this.viewPref.bottomCG.alpha, 1)) {
                UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, true);
                offset = -(this.rightButtonsCount - 1) * 53;
            }
            UIManager.UIBind(this.viewPref.pnlBottom, this.target.transform, TILESIZE_MIDDLE,
                BindDirection.Down, BindCameraMode.None, Vector2.up * offset);
        }

        private void PlayAllAnimation(ref int delay, Transform root, ref int reference, UnityAction action) {
            float interval = 0.0333f;
            foreach (Transform transform in root) {
                if (transform.GetComponent<AnimationCombo>() != null &&
                    transform.gameObject.activeSelf && transform.gameObject.activeInHierarchy) {
                    AnimationManager.Finish(transform.gameObject);
                    string objName = transform.name;
                    reference++;
                    AnimationManager.Animate(transform.gameObject, "Show",
                        (delay++) * interval, axis: Vector2.up, isOffset: false, finishCallback: () => {
                            action.InvokeSafe();
                        });
                }
                this.PlayAllAnimation(ref delay, transform, ref reference, action);
            }
        }

        private void SetMarksBtnInfo() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            Vector2 coord = new Vector2(tileInfo.coordinate.x, tileInfo.coordinate.y);
            string buildingId = string.Empty;
            if (this.viewModel.TileInfo.buildingInfo != null) {
                buildingId = tileInfo.buildingInfo.GetId();
            }
            BuildingConf buildingConf = null;
            if (!string.IsNullOrEmpty(buildingId)) {
                buildingConf = ConfigureManager.GetConfById<BuildingConf>(buildingId);
            }
            if (buildingConf != null &&
                tileInfo.playerId.CustomEquals(RoleManager.GetRoleId()) &&
                (buildingConf.type.CustomEquals(ElementName.townhall) ||
                buildingConf.type.CustomEquals(ElementName.stronghold))) {

            } else {
                bool isShowMarkPnl = this.viewModel.IsShowMarkBtnView(coord);
                if (isShowMarkPnl) {
                    this.leftBtnList.Add(TileButtonType.Mark);
                } else {
                    this.leftBtnList.Add(TileButtonType.DeleteMark);
                }
            }

            bool canEditAllianceMark = !RoleManager.GetAllianceId().CustomIsEmpty() &&
               ((int)RoleManager.GetAllianceRole() > (int)AllianceRole.Elder);
            coord = new Vector3(tileInfo.coordinate.x, tileInfo.coordinate.y, (int)MapMarkType.Alliance);
            bool allianceMarkContainCoord = this.viewModel.IsAllianceMarkContainCoord(coord);

            if (canEditAllianceMark && !allianceMarkContainCoord) {
                this.leftBtnList.Add(TileButtonType.MarkAlliance);
            } else if (canEditAllianceMark && allianceMarkContainCoord) {
                this.leftBtnList.Add(TileButtonType.DeleteAllianceMark);
            }
        }

        private bool isNeeShowHealTips = false;
        private void SetLargeButtons() {
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, true);
            this.SetMarksBtnInfo();
            bool isTileTroopFull = this.viewModel.IsTileTroopAmountFull();
            bool isNeeShowMoveTips = this.viewModel.IsNeedShowMoveHealTips();
            bool isNeeShowReturnTips = this.viewModel.IsNeedShowReturnTips();
            this.isNeeShowHealTips = isNeeShowMoveTips || isNeeShowReturnTips;
            this.viewPref.pnlMoveTip.gameObject.SetActive(this.isNeeShowHealTips);
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            this.btnLabelDict[TileButtonType.Move] = LocalHashConst.button_tile_move;
            switch (tileInfo.relation) {
                case MapTileRelation.self:
                    this.SetButtonsSelf(tileInfo, isTileTroopFull);
                    break;
                case MapTileRelation.master:
                    break;
                case MapTileRelation.ally:
                    if ((this.viewModel.IsAllianceNpcCity() && !isTileTroopFull) ||
                        this.viewModel.CanReachable()) {
                        this.rightBtnList.Add(TileButtonType.Move);
                    }
                    break;
                case MapTileRelation.fallen:
                    if (tileInfo.allianceId.CustomIsEmpty() ||
                        tileInfo.allianceId != RoleManager.GetAllianceId() ||
                        RoleManager.GetMasterAllianceId().CustomIsEmpty()) {
                        this.rightBtnList.Add(TileButtonType.Attack);
                    }

                    break;
                case MapTileRelation.slave:
                    if (this.viewModel.CanReachable()) {
                        this.rightBtnList.Add(TileButtonType.Move);
                    }
                    break;
                default:
                    if (this.viewModel.IsTileOperateAble()) {
                        this.rightBtnList.Add(TileButtonType.Attack);
                    }
                    break;
            }

            if (this.viewModel.HasMonsterOrBossOnTile() &&
                !this.rightBtnList.Contains(TileButtonType.Attack)) {
                this.rightBtnList.Add(TileButtonType.Attack);
                this.rightBtnList.TryRemove(TileButtonType.Move);
            }
            if (this.rightBtnList.Contains(TileButtonType.Attack) &&
                this.viewModel.TileInfo.isProtected &&
                this.viewModel.TileInfo.relation != MapTileRelation.self &&
                !this.viewModel.HasMonsterOrBossOnTile()) {
                this.grayBtnList.Add(TileButtonType.Attack);
            }
        }

        private void SetTileCampaignView() {
            this.HideSubView();
            this.viewPref.pnlOverviewBind.gameObject.SetActive(false);
        }

        private void ResetRightButtons() {
            this.viewPref.pnlRight.gameObject.SetActiveSafe(this.rightButtonsCount > 0);
            float centerIndex = (this.rightButtonsCount - 1) / 2f;
            float radius = 200f;
            Vector2 centerPosition = new Vector2(180f, 0);
            Vector2 centerOffset = new Vector2(0, 15f);
            float angle = 40 * Mathf.Deg2Rad;
            UnityAction<Transform, int> reset = (child, index) => {
                float offsetIndex = centerIndex - index;
                float offsetAngle = offsetIndex * angle;
                Vector2 originPosition = new Vector2(
                    radius * Mathf.Cos(offsetAngle),
                    radius * Mathf.Sin(offsetAngle)
                );
                Vector2 childPosition = originPosition -
                    centerPosition + centerOffset;
                child.GetComponent<RectTransform>().anchoredPosition = childPosition;
            };
            GameHelper.ResizeChildreCount(this.viewPref.pnlRightButtons, this.rightBtnList.Count,
                PrefabPath.pnlTileRightButton);
            this.rightBtnList.Sort((a, b) => {
                return a.CompareTo(b);
            });
            for (int i = 0; i < this.rightBtnList.Count; i++) {
                Transform btnTrans = this.viewPref.pnlRightButtons.GetChild(i);
                TileButtonType btnType = this.rightBtnList[i];
                TileButtonView btnView = btnTrans.GetComponent<TileButtonView>();
                btnView.SetBtnType(
                    btnType,
                    this.btnCallbackDict[btnType],
                    this.grayBtnList.Contains(btnType),
                    this.btnLabelDict.ContainsKey(btnType) ?
                        LocalManager.GetValue(this.btnLabelDict[btnType]) :
                        string.Empty
                );
                this.btnDict.Add(btnType, btnView.button);
                reset.Invoke(btnTrans, i);
            }
        }

        private void ResetLeftButtons() {
            this.viewPref.pnlLeft.gameObject.SetActiveSafe(this.leftButtonsCount > 0);
            float centerIndex = (this.leftButtonsCount - 1) / 2f;
            float radius = 160f;
            Vector2 centerPosition = new Vector2(140f, 0);
            Vector2 centerOffset = new Vector2(0, 15f);
            float angle = 40 * Mathf.Deg2Rad;

            GameHelper.ResizeChildreCount(this.viewPref.pnlLeftButtons, this.leftBtnList.Count,
                PrefabPath.pnlTileLeftButton);
            for (int i = 0; i < this.leftBtnList.Count; i++) {
                Transform btnTrans = this.viewPref.pnlLeftButtons.GetChild(i);
                TileButtonType btnType = leftBtnList[i];
                TileButtonView btnView = btnTrans.GetComponent<TileButtonView>();
                btnView.SetBtnType(
                    btnType,
                    this.btnCallbackDict[btnType],
                    this.grayBtnList.Contains(btnType),
                    this.btnLabelDict.ContainsKey(btnType) ?
                        LocalManager.GetValue(this.btnLabelDict[btnType]) :
                        string.Empty
                );
                this.btnDict.Add(btnType, btnView.button);
                float offsetIndex = centerIndex - i;
                float offsetAngle = offsetIndex * angle;
                Vector2 originPosition = new Vector2(
                    radius * Mathf.Cos(offsetAngle),
                    radius * Mathf.Sin(offsetAngle)
                );
                Vector2 childPosition = centerPosition - originPosition
                     + centerOffset;
                btnTrans.GetComponent<RectTransform>().anchoredPosition = childPosition;
            }
        }

        private void SetButtonsSelf(MapTileInfo tileInfo, bool isTileTroopFull) {
            if (this.viewModel.GetTileGiveupable()) {
                if (tileInfo.type.CustomEquals(ElementCategory.resource)) {
                    this.rightBtnList.Add(TileButtonType.Build);
                }
                this.rightBtnList.Add(TileButtonType.Attack);
                this.leftBtnList.Add(TileButtonType.Giveup);
                if (this.viewModel.TileInfo.tileProtectType == TileProtectType.AvoidExpireAt) {
                    this.grayBtnList.Add(TileButtonType.Giveup);
                }
            } else if (tileInfo.type.CustomEquals(ElementCategory.building)) {
                bool tileIsBuilding = EventManager.IsTileInBuildEvent(tileInfo.coordinate);
                bool canCancelUpgrading = (tileInfo.buildingInfo.Level > 0);
                if (tileIsBuilding && canCancelUpgrading) {
                    this.rightBtnList.Add(TileButtonType.CancelUpgrade);
                }
                if (!tileIsBuilding) {
                    bool reachMaxLevel;
                    bool giveUping;
                    bool canUpgrade = this.viewModel.IsBuildingCanUpgrade(out reachMaxLevel, out giveUping);
                    this.rightBtnList.Add(TileButtonType.Upgrade);
                    if (canUpgrade) {
                        this.btnLabelDict[TileButtonType.Upgrade] = LocalHashConst.button_tile_upgrade;
                    } else {
                        this.grayBtnList.Add(TileButtonType.Upgrade);
                        if (reachMaxLevel) {
                            this.btnLabelDict[TileButtonType.Upgrade] = LocalHashConst.button_full_level;
                        } else if (giveUping) {
                            this.btnLabelDict[TileButtonType.Upgrade] = LocalHashConst.building_giveuping;
                        }
                    }
                }

                this.viewPref.pnlMoveTip.gameObject.SetActive(this.isNeeShowHealTips);

                if (this.viewModel.IsShowGiveUpStrongholdPnl()) {
                    this.leftBtnList.Add(TileButtonType.StrongholdGiveUp);
                }
            }
            if (!isTileTroopFull) {
                bool isTownhall = tileInfo.buildingInfo == null ? false : tileInfo.buildingInfo.Type == (int)ElementType.townhall;
                if (isTownhall) {
                    this.btnLabelDict[TileButtonType.Move] = LocalHashConst.button_tile_recall;
                }
                this.rightBtnList.Add(TileButtonType.Move);
                if (this.viewModel.IsAllTroopStayInCoord()) {
                    this.grayBtnList.Add(TileButtonType.Move);
                }
            }
        }

        private void ShowButtons() {
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, true);
            this.showCompleted = false;
            if (this.viewModel.ShowAnimation) {
                int delay = 0;
                int reference = 0;
                UnityAction callback = () => {
                    if (--reference == 0 && this.showCompleted) {
                        this.showCompletedCallback.InvokeSafe();
                        this.showCompletedCallback = null;
                    }
                };
                this.PlayAllAnimation(ref delay, this.viewPref.pnlLeft, ref reference, callback);
                delay += 3;
                this.PlayAllAnimation(ref delay, this.viewPref.pnlRight, ref reference, callback);
                this.showCompleted = true;
            }
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            if (this.rightBtnList.Contains(TileButtonType.Attack) &&
                 this.viewModel.TileInfo.playerId.CustomIsEmpty() &&
                !(this.viewModel.HasMonsterOrBossOnTile())) {
                if (this.showCompleted) {
                    this.viewModel.ShowTileRewardViewWithDelay(0.3f);
                } else {
                    this.viewModel.ShowTileRewardViewWithDelay(0);
                }
            }
            UIManager.UIBind(this.viewPref.pnlLeft, this.target.transform, TILESIZE_MIDDLE,
                BindDirection.Left, BindCameraMode.None);
            UIManager.UIBind(this.viewPref.pnlRight, this.target.transform, TILESIZE_MIDDLE,
                    BindDirection.Right, BindCameraMode.None);
            this.viewPref.pnlBelow.gameObject.SetActiveSafe(false);
        }

        public void ShowOverview() {
            this.isShowTroop = false;
            this.isChosenTroop = false;
            this.viewModel.DisableHighlight();
            UIManager.SetUICanvasGroupVisible(this.viewPref.terrainBindCG, this.isTerrain);
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(
                !this.isTerrain && !(this.viewModel.HasMonsterOrBossOnTile()));
            if (this.isTerrain) {
                UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
                UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, false);
                UIManager.UIBind(this.viewPref.pnlTerrainBind, this.target.transform, TILESIZE_MIDDLE,
                    BindDirection.Up, BindCameraMode.None);
                this.showCompletedCallback.InvokeSafe();
                this.showCompletedCallback = null;
            } else {
                this.ShowButtons();
                if (!(this.viewModel.HasMonsterOrBossOnTile())) {
                    this.ShowBottom();
                }
                int offset = Mathf.Max(55, this.rightButtonsCount * 40);
                UIManager.UIBind(this.viewPref.pnlOverviewBind,
                    this.target.transform, this.overViewTileSize,
                    BindDirection.Up, BindCameraMode.None, Vector2.up * offset);
                this.SetCampaignStatus();
            }

            if (this.viewModel.ShowAnimation && this.viewPref.overViewRT.localScale.x != 1) {
                AnimationManager.Animate(this.viewPref.pnlOverview.gameObject,
                    "Show", isOffset: false);
            }
        }

        private TileType GetTileType() {
            if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.npc_city)) {
                return TileType.npc_city;
            } else if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.resource)) {
                return TileType.resource;
            } else if (this.viewModel.TileInfo.IsTilePassBridge()) {
                return TileType.bridge;
            } else {
                return TileType.None;
            }
        }

        private TileType GetCapturePointsTileType() {
            if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.pass)) {
                return TileType.pass;
            } else {
                return TileType.resource;
            }
        }

        private void SetCampaignStatus() {
            bool needOccupyShowStatus = false;
            bool needCaptureShowStatus = false;
            foreach (Activity activity in this.viewModel.AllActivties) {
                switch (activity.CampaignType) {
                    case CampaignType.occupy://占地为王
                        if (activity.Status == Activity.ActivityStatus.Started) {
                            needOccupyShowStatus = this.CampaignOccupy(activity);
                        }
                        break;
                    case CampaignType.capture://州际纷争
                        if (activity.Status == Activity.ActivityStatus.Started) {
                            needCaptureShowStatus = this.CampaignCapture(activity);
                        }
                        break;
                    default:
                        break;
                }
            }
            UIManager.SetUICanvasGroupVisible(
                this.viewPref.pnlCampaignPromptingCG,
                needOccupyShowStatus || needCaptureShowStatus
            );
        }

        //占地为王
        private bool CampaignOccupy(Activity activity) {
            GameHelper.ClearChildren(this.viewPref.pnlCampaignPromptingList);
            if (!this.viewModel.TileInfo.playerId.CustomIsEmpty() &&
                this.viewModel.TileInfo.relation == MapTileRelation.enemy &&
                this.IsCampaignBuilding()) {
                GameObject itemObj = PoolManager.GetObject(
                    PrefabPath.pnlCampaignPromptingItemView, this.viewPref.pnlCampaignPromptingList);
                CampaignPromptingItemView itemView = itemObj.GetComponent<CampaignPromptingItemView>();
                itemView.SetContent(activity, this.GetOccupyTilePoint());
                return true;
            }
            return false;
        }

        private bool IsCampaignBuilding() {
            if (this.viewModel.TileInfo.buildingInfo != null) {
                BuildingConf buildingConf = BuildingConf.GetConf(
                    this.viewModel.TileInfo.buildingInfo.GetId());
                if (buildingConf.type == ElementType.townhall.ToString()) {
                    return false;
                }
            } else {
                return true;
            }
            return true;
        }

        private string GetOccupyTilePoint() {
            int tileLevel = this.viewModel.TileInfo.level;
            TileType tileType = this.GetTileType();
            if (tileType != TileType.None) {
                return OccupyPointsConf.GetConf(tileLevel + tileType.ToString()).point;
            } else {
                tileLevel = int.Parse(this.viewModel.GetLayerAboveLevel());
                return OccupyPointsConf.GetConf(tileLevel + "resource").point;
            }
        }

        //州际纷争
        private bool CampaignCapture(Activity activity) {
            if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.resource) ||
                this.viewModel.TileInfo.type.CustomEquals(ElementCategory.pass) ||
                this.viewModel.TileInfo.type.CustomEquals(ElementCategory.building)
            ) {
                bool isCaptureableTile = !(
                    (this.viewModel.TileInfo.relation == MapTileRelation.ally) ||
                    (this.viewModel.TileInfo.relation == MapTileRelation.self) ||
                    (this.viewModel.TileInfo.relation == MapTileRelation.master) ||
                    (this.viewModel.TileInfo.relation == MapTileRelation.slave)
                );
                bool isOtherZone = this.GetTileZone(this.viewModel.TileInfo.coordinate) !=
                    this.GetTileZone(RoleManager.GetRoleCoordinate());
                if (isOtherZone && isCaptureableTile) {
                    GameHelper.ResizeChildreCount(this.viewPref.pnlCapturePointsList, 1,
                        PrefabPath.pnlCampaignPromptingItemView);
                    UIManager.SetUICanvasGroupVisible(this.viewPref.pnlCampaignPromptingCG, true);
                    string captureConfKey = string.Concat(
                        this.viewModel.GetResourcesInfoLevel(),
                        this.GetCapturePointsTileType()
                    );
                    CapturePointsConf CaptureConf = CapturePointsConf.GetConf(captureConfKey);

                    if (CaptureConf == null) {
                        Debug.LogError("找不到洲际纷争ID");
                        return false;
                    }
                    CampaignPromptingItemView itemView =
                        this.viewPref.pnlCapturePointsList.GetChild(0).GetComponent<CampaignPromptingItemView>();
                    itemView.SetContent(activity, CaptureConf.point);
                    return true;
                }
            }
            return false;
        }



        public void ShowAllianceMarkEditView() {
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.viewModel.HideActivityTileView();
            this.viewPref.ifMarkName.text = string.Empty;

            this.viewPref.ifMarkName.characterLimit =
                LocalConst.GetLimit(LocalConst.ALLIANCE_MARK);
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            this.viewPref.txtMarkCoordinate.text = string.Concat("(", tileInfo.coordinate.x, ",",
                tileInfo.coordinate.y, ")");
            string tileName = MapUtils.GetTileName(tileInfo);
            if (tileInfo.level != 0) {
                this.viewPref.txtMarkInfo.StripLengthWithSuffix("<color=#B87507>" + string.Format(
                    LocalManager.GetValue(LocalHashConst.level), tileInfo.level)
                    + "</color> " + tileName);
            } else {
                this.viewPref.txtMarkInfo.StripLengthWithSuffix(tileName);
            }
            if (tileInfo.level != 0) {
                tileName = string.Concat(tileName, "    ", GameHelper.GetLevelLocal(tileInfo.level));
            }
            this.viewPref.txtAllianceMarkInfo.text = string.Concat(tileName, "  (", tileInfo.coordinate.x,
                ",", tileInfo.coordinate.y, ")");
            AnimationManager.Animate(this.viewPref.pnlAddAllianceMark.gameObject, "Show", () => {
                this.viewPref.pnlAddAllianceMarkCG.interactable = true;
                this.viewPref.pnlAddAllianceMarkCG.blocksRaycasts = true;
            });
        }

        public void HideAllianceMarkEditView() {
            AnimationManager.Animate(this.viewPref.pnlAddAllianceMark.gameObject, "Hide",
                this.ResetAllianceMarkView);
        }

        private void ResetAllianceMarkView() {
            this.viewPref.ifMarkName.text = string.Empty;
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
        }

        private void SetOverview() {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(true);
            this.viewPref.troopOverviewCG.alpha = 0;
            this.viewModel.HideSelfMarch();
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            if (tileInfo.type.CustomEquals(ElementCategory.river) ||
                tileInfo.type.CustomEquals(ElementCategory.mountain) ||
                tileInfo.type.CustomEquals(ElementCategory.forest)) {
                this.SetCouldNotOccupyTileInfo();
            } else {
                this.viewPref.txtDefenderPower.text =
                    LocalManager.GetValue(LocalHashConst.net_is_waiting);
                this.viewModel.TroopInfoReq();
                this.isTerrain = false;
                string tileName = MapUtils.GetTileName(tileInfo);
                if (tileInfo.level != 0) {
                    this.viewPref.txtInfo.StripLengthWithSuffix(string.Concat(tileName, "[",
                        GameHelper.GetLevelLocal(tileInfo.level), "]"));
                } else {
                    this.viewPref.txtInfo.StripLengthWithSuffix(tileName);
                }
                this.viewPref.imgAllianceIconBG.gameObject.SetActive(
                    tileInfo.allianceIcon != 0
                );
                this.viewPref.imgAllianceIcon.sprite =
                    ArtPrefabConf.GetAliEmblemSprite(tileInfo.allianceIcon);
                this.overViewTileSize = this.GetTileSize(tileInfo);
                this.SetPlayerInfo();
                this.SetTileTributeAddtionInfo();
                this.SetProductionInfo();
                this.SetTileEnduranceInfo();
            }
            if (!this.viewModel.isTileCoordChange) {
                long leftDuration = this.viewModel.defenderRecoverTimeAt * 1000 -
                    RoleManager.GetCurrentUtcTime();
                this.viewPref.pnlDefendersRecover.gameObject.SetActiveSafe(leftDuration > 0);
            }
            this.FormateView();
        }

        public void SetTileDefendersInfo(int power, int count, int totalCount) {
            this.viewPref.txtDefenderPower.text = string.Format(
                "{0} <color=#B97608>{1}</color>",
                LocalManager.GetValue(LocalHashConst.defender_power), power
            );
            this.viewPref.txtDefenderCount.text = string.Format(
                "{0} <color=#B97608>{1}/{2}</color>",
                LocalManager.GetValue(LocalHashConst.map_tile_troop_amount),
                count, totalCount
            );
        }

        private void FormateView() {
            this.viewPref.mainInfoVerticalLG.CalculateLayoutInputHorizontal();
            this.viewPref.mainInfoVerticalLG.CalculateLayoutInputVertical();
            this.viewPref.mainInfoContentSF.SetLayoutVertical();
        }

        private void SetTileEnduranceInfo() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            if (tileInfo.type.CustomEquals(ElementCategory.camp)) {
                this.SetCampCount(tileInfo.camp.RemainTimes);
            } else {
                this.SetEndurance(tileInfo);
            }
        }

        private void SetCouldNotOccupyTileInfo() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            this.isTerrain = true;
            this.viewPref.txtTerrainCoordinate.text = string.Format("({0},{1})",
                tileInfo.coordinate.x, tileInfo.coordinate.y);
            this.viewPref.txtTerrainDescription.text =
                LocalManager.GetValue("map_tile_", tileInfo.name, "_intro");

            this.overViewTileSize = this.TILESIZE_MIDDLE;
        }

        private void SetProductionInfo() {
            Dictionary<Resource, int> productDict = null;
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            if (tileInfo.type.CustomEquals(ElementType.npc_city.ToString())) {
                productDict = tileInfo.city.resourceBuff;
            } else if (!tileInfo.type.CustomEquals(ElementType.pass.ToString())) {
                string tileProductionKey = string.Concat(tileInfo.name, tileInfo.level);
                MapResourceProductionConf mp =
                    MapResourceProductionConf.GetConf(tileProductionKey);
                if (mp != null) {
                    productDict = mp.productionDict;
                }
            }
            bool hasProductionInfo = ((productDict != null) && productDict.Count > 0);
            this.viewPref.pnlProductions.gameObject.SetActiveSafe(hasProductionInfo);
            if (hasProductionInfo) {
                int productCount = productDict.Count;
                GameHelper.ResizeChildreCount(this.viewPref.pnlProductionList,
                    productCount, PrefabPath.pnlHourlyOutput);
                int i = this.viewPref.pnlProductionList.childCount;
                foreach (var pair in productDict) {
                    this.viewPref.pnlProductionList.GetChild(--i).GetComponent<ResourceItemView>().SetResource(
                        pair.Key, MapResourceProductionConf.GetProduction(pair.Value));
                }
            }
        }

        private void SetTileTributeAddtionInfo() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            bool isNpcCityEdge = tileInfo.type.CustomEquals(ElementType.npc_city.ToString()) &&
                                 !tileInfo.city.isCenter;
            this.viewPref.pnlTributeAddition.gameObject.SetActiveSafe(isNpcCityEdge);
            if (isNpcCityEdge) {
                NPCCityConf npcCityConf = NPCCityConf.GetConf(tileInfo.city.id);
                string tributeInfo = string.Concat(
                    "+", GameHelper.GetFormatNum(npcCityConf.tributeGoldBuff),
                    "/", LocalManager.GetValue(LocalHashConst.tribute));
                this.viewPref.txtTributeAddition.text = tributeInfo;
            }
        }

        private void SetPlayerInfoVisible(bool visible) {
            this.viewPref.txtPlayer.gameObject.SetActiveSafe(visible);
            this.viewPref.txtAlliance.gameObject.SetActiveSafe(visible);
            this.viewPref.txtFallenInfo.transform.gameObject.SetActiveSafe(visible);
            this.viewPref.txtFreeMan.transform.gameObject.SetActiveSafe(visible);
            this.viewPref.txtFieldAllianceInfo.gameObject.SetActiveSafe(!visible);
        }

        private void SetPlayerInfo() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            bool tilePlayerInfoAvaliable = !tileInfo.playerId.CustomIsEmpty();
            bool playerInfoVisible = false;

            this.viewPref.pnlAvoidProtect.gameObject.SetActiveSafe(false);
            this.viewPref.pnlFreshProtect.gameObject.SetActiveSafe(false);
            this.viewPref.txtProtectTime.gameObject.SetActiveSafe(false);
            TileProtectType protectType;
            bool hasProtection = this.viewModel.GetTileProtectTime(out protectType) > 0;
            bool isFreshProtect = (protectType == TileProtectType.FreshProtect);
            if (tilePlayerInfoAvaliable) {
                playerInfoVisible = true;
                this.SetPlayerInfoVisible(true);
                this.viewPref.txtPlayer.text = tileInfo.playerName;
                this.viewPref.txtAlliance.StripLengthWithSuffix(tileInfo.allianceName);
                if (hasProtection) {
                    this.viewPref.txtFallenInfo.gameObject.SetActiveSafe(false);
                    this.viewPref.txtFreeMan.gameObject.SetActiveSafe(false);
                    this.viewPref.txtProtectTime.gameObject.SetActiveSafe(true);
                    this.viewPref.pnlAvoidProtect.gameObject.SetActiveSafe(!isFreshProtect);
                    this.viewPref.pnlFreshProtect.gameObject.SetActiveSafe(isFreshProtect);
                } else {
                    this.viewPref.txtFallenInfo.gameObject.SetActiveSafe(tileInfo.isFallen);
                    this.viewPref.txtFreeMan.gameObject.SetActiveSafe(!tileInfo.isFallen);
                }
            } else if (tileInfo.type.CustomEquals(ElementCategory.npc_city) ||
                       tileInfo.type.CustomEquals(ElementCategory.pass)) {
                this.SetPlayerInfoVisible(false);
                bool isAllianceTile = !string.IsNullOrEmpty(tileInfo.allianceId);
                playerInfoVisible = isAllianceTile;
                if (hasProtection) {
                    this.viewPref.pnlAvoidProtect.gameObject.SetActiveSafe(!isFreshProtect);
                    this.viewPref.pnlFreshProtect.gameObject.SetActiveSafe(isFreshProtect);
                }
                this.viewPref.txtFieldAllianceInfo.gameObject.SetActiveSafe(isAllianceTile);
                this.viewPref.txtFieldAllianceInfo.text = tileInfo.allianceName;
            }
            this.SetPlayerInfoBG();
            this.viewPref.pnlPlayerInfo.gameObject.SetActiveSafe(playerInfoVisible);
        }

        private void SetPlayerInfoBG() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            string playerInfoBGImgPath = string.Empty;
            if (tileInfo.isFallen) {
                playerInfoBGImgPath = "fallen";
            } else if (tileInfo.relation == MapTileRelation.ally) {
                playerInfoBGImgPath = "ally";
            } else if (tileInfo.relation == MapTileRelation.self) {
                playerInfoBGImgPath = "self";
            } else {
                playerInfoBGImgPath = "enemy";
            }
            this.viewPref.imgPlayerInfo.sprite =
                ArtPrefabConf.GetSprite(
                    string.Concat(SpritePath.tilePlayerInfoPrefix, playerInfoBGImgPath));
        }

        private void SetCampCount(int times) {
            this.viewPref.pnlEndurance.gameObject.SetActiveSafe(true);
            this.viewPref.sldEndurance.maxValue = GameConst.CAMP_MAX_TIMES;
            this.viewPref.sldEndurance.value = times;
            this.viewPref.txtEndurance.text = string.Concat(
                GameHelper.GetFormatNum(Mathf.RoundToInt(this.viewPref.sldEndurance.value), maxLength: 4), "/",
                GameHelper.GetFormatNum(Mathf.RoundToInt(this.viewPref.sldEndurance.maxValue), maxLength: 4));
        }

        private void SetEndurance(MapTileInfo tileInfo) {
            this.viewPref.txtCoordinate.text = string.Concat("(", tileInfo.coordinate.x, ",",
                tileInfo.coordinate.y, ")");

            bool enduranceInfoVisible = this.viewModel.IsShowTileEndurance();
            this.viewPref.pnlEndurance.gameObject.SetActiveSafe(enduranceInfoVisible);
            if (enduranceInfoVisible) {
                this.viewPref.sldEndurance.value = this.viewPref.sldEndurance.maxValue *
                    (tileInfo.endurance / (float)tileInfo.maxEndurance);
                this.viewPref.txtEndurance.text = string.Concat(
                    GameHelper.GetFormatNum(tileInfo.endurance, maxLength: 4), "/",
                    GameHelper.GetFormatNum(tileInfo.maxEndurance, maxLength: 4));
                float percent = this.viewPref.sldEndurance.value / this.viewPref.sldEndurance.maxValue;
                string sliderTail = "green";
                if (percent <= 0.2f) {
                    sliderTail = "red";
                } else if (percent < 0.5f) {
                    sliderTail = "orange";
                }
                this.viewPref.imgEndurance.sprite =
                    ArtPrefabConf.GetSprite(string.Concat(SpritePath.resouceSliderPrefix, sliderTail));
            }

            this.SetAllianceMarkInfo(tileInfo);
        }

        private void SetAllianceMarkInfo(MapTileInfo tileInfo) {
            Vector3 coord = new Vector3(
                tileInfo.coordinate.x,
                tileInfo.coordinate.y,
                (int)MapMarkType.Alliance
            );
            MapMark mapMark;
            bool showAllianceMarkInfo = this.viewModel.MarkDict.TryGetValue(coord, out mapMark);
            this.viewPref.pnlAllianceMarkInfo.gameObject.SetActiveSafe(showAllianceMarkInfo);
            if (showAllianceMarkInfo) {
                this.viewPref.pnlAllianceMarkInfo.Find("TxtAllianceMark").GetComponent<TextMeshProUGUI>().text =
                    LocalManager.GetValue(LocalHashConst.button_tile_markalliance);
                this.viewPref.pnlAllianceMarkInfo.Find("TxtAllianceMarkName").GetComponent<TextMeshProUGUI>().text =
                    mapMark.mark.Name;
            }
        }

        private Vector2 GetTileSize(MapTileInfo tileInfo) {
            if (tileInfo.type.CustomEquals(ElementCategory.building) &&
                tileInfo.buildingInfo.Name.CustomEquals(ElementName.townhall)) {
                return this.TILESIZE_BIG;
            } else {
                return this.TILESIZE_MIDDLE;
            }
        }

        public void SetTroopOverviewVisible(bool isVisible) {
            this.viewPref.troopOverviewCG.blocksRaycasts = isVisible;
            this.viewPref.troopOverviewCG.interactable = isVisible;
            this.viewPref.troopOverviewCG.alpha = isVisible ? 1 : 0;
        }

        private void ShowDetail() {
            this.viewModel.MoveToTile();
            this.viewModel.Page = TilePage.Sub;
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
            this.viewModel.HideTileReward();
            this.viewModel.ShowTileDetail();
        }

        public void ShowOtherMarch(EventMarchClient march) {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.viewPref.terrainBindCG.alpha = 0;
            this.SetTroopOverviewVisible(true);
            UIManager.UIBind(
                this.viewPref.pnlTroopBind,
                GameManager.MainCamera.gameObject.transform,
                new Vector2(0, 4),
                BindDirection.Up,
                BindCameraMode.None
            );
            this.viewPref.btnTroopDetail.onClick.RemoveAllListeners();
            this.viewPref.btnTroopDetail.onClick.AddListener(() => {
                this.OnBtnTroopDetailClick(march.playeId);
            });
            this.viewPref.txtTroopName.text = march.playerName;
            this.viewPref.txtOrigin.text = string.Concat(march.origin.x, ", ", march.origin.y);
            this.viewPref.txtTarget.text = string.Concat(march.target.x, ", ", march.target.y);
            this.viewPref.pnlBelow.gameObject.SetActiveSafe(false);
        }

        public int GetRightButtonsCount() {
            return this.rightButtonsCount;
        }

        public void ShowSelfTroop() {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.SetTroopOverviewVisible(false);
            this.viewPref.pnlLeft.gameObject.SetActiveSafe(false);
            this.viewPref.pnlRight.gameObject.SetActiveSafe(true && !this.isChosenTroop);
            UIManager.UIBind(
                this.pnlMarchBind,
                this.target.transform,
                this.TILESIZE_MIDDLE,
                BindDirection.Up,
                BindCameraMode.None,
                Vector2.up * 100
            );
            UIManager.UIBind(this.viewPref.pnlRight, this.target.transform, TILESIZE_MIDDLE,
                  BindDirection.Right, BindCameraMode.None);
        }

        public void ShowOtherTroop() {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.SetTroopOverviewVisible(true);
            this.viewPref.pnlLeft.gameObject.SetActiveSafe(false);
            this.viewPref.pnlRight.gameObject.SetActiveSafe(true);
            UIManager.UIBind(this.viewPref.pnlTroopBind, this.target.transform, this.overViewTileSize,
                BindDirection.Up, BindCameraMode.None, Vector2.up * 50);
            UIManager.UIBind(this.viewPref.pnlRight, this.target.transform, TILESIZE_MIDDLE,
                    BindDirection.Right, BindCameraMode.None);
            AnimationManager.Animate(this.viewPref.pnlTroopInfo.gameObject, "Show", null);
        }

        public void RemoveNotTroopButton() {
            foreach (TileButtonType btnType in this.notTroopBtnList) {
                this.rightBtnList.TryRemove(btnType);
            }
            this.btnDict.Clear();
            this.rightButtonsCount = this.rightBtnList.Count;
            this.ResetRightButtons();
        }

        public void SetTroop(int troopCount) {
            this.isShowTroop = true;
            bool showReturn = true;
            if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.building) &&
                this.viewModel.TileInfo.relation == MapTileRelation.self) {
                BuildingConf buildingConf = ConfigureManager.GetConfById<BuildingConf>(
                    this.viewModel.TileInfo.buildingInfo.GetId()
                );
                if (buildingConf.type.CustomEquals(ElementName.townhall)) {
                    showReturn = false;
                }
            }
            if (showReturn) {
                this.viewPref.pnlMoveTip.gameObject.SetActive(this.isNeeShowHealTips);
            }
        }

        private void ShowTroopChose(TroopChoseAction type) {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.viewModel.ShowTroopChose(type);
            this.ShowBottom();
        }

        private void SetTroopButtons(bool hasTroop) {
            bool showFormat = false;
            bool showRecruit = false;
            bool showReturn = true;
            if (hasTroop) {
                if (this.viewModel.TileInfo.type.CustomEquals(ElementCategory.building) &&
                    this.viewModel.TileInfo.relation == MapTileRelation.self) {
                    //Debug.LogError("地块部队+"+ this.viewModel.TileInfo.relation);
                    BuildingConf buildingConf = ConfigureManager.GetConfById<BuildingConf>(
                      this.viewModel.TileInfo.buildingInfo.GetId()
                    );
                    bool isBuildLevelAvaliable = this.viewModel.TileInfo.buildingInfo.Level > 0;
                    var troopList = this.viewModel.GetTroopsAt(this.viewModel.TileInfo.coordinate);
                    if (buildingConf.type.CustomEquals(ElementName.townhall)) {
                        if (troopList.Count != 0) {
                            showFormat = true;
                            showRecruit = isBuildLevelAvaliable;
                        }
                        showReturn = false;
                    } else if (buildingConf.type.CustomEquals(ElementName.stronghold)) {
                        showRecruit = isBuildLevelAvaliable;
                    }

                } else if (this.viewModel.TileInfo.type.CustomEquals(ElementName.npc_city) &&
                    this.viewModel.TileInfo.city.isCenter) {
                    showRecruit = true;
                }
            }
            if (showFormat) {
                this.rightBtnList.Add(TileButtonType.Format);
            }
            if (showRecruit) {
                this.rightBtnList.Add(TileButtonType.Recruit);
            }
            if (showReturn && hasTroop) {
                this.rightBtnList.Add(TileButtonType.Return);
                this.viewPref.pnlMoveTip.gameObject.SetActive(this.isNeeShowHealTips);
            }
            this.viewPref.pnlRetreat.gameObject.SetActiveSafe(false);
        }

        public void SetMarchView() {
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, true);
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            this.viewPref.troopOverviewCG.alpha = 0;
            this.viewModel.HideTileDetail();
            this.viewPref.pnlLeft.gameObject.SetActiveSafe(false);
            this.viewPref.pnlRight.gameObject.SetActiveSafe(false);
            this.viewPref.pnlBelow.gameObject.SetActiveSafe(true);
            this.viewModel.HideTileReward();

            this.viewPref.pnlRetreat.gameObject.SetActiveSafe(true);
            this.viewPref.pnlRight.GetComponent<RectTransform>().anchoredPosition =
                MapUtils.WorldToUIPoint(GameManager.MainCamera.transform.position) +
                new Vector2(250, -50);
            UIManager.UIBind(
                this.pnlMarchBind,
                GameManager.MainCamera.gameObject.transform,
                new Vector2(this.TILESIZE_MIDDLE.x, 3),
                BindDirection.Up,
                BindCameraMode.None
            );
            float timeInMiniute = GameHelper.MillisecondToMinute(
                RoleManager.GetCurrentUtcTime() - this.viewModel.CurrentMarch.startTime
            );
            this.viewPref.btnRetreat.onClick.RemoveAllListeners();
            if (this.viewModel.CurrentMarch.type == MarchType.FallBack ||
                this.viewModel.CurrentMarch.type == MarchType.Return) {
                this.viewPref.btnRetreat.Grayable = true;
                this.viewPref.btnRetreat.onClick.AddListener(
                    () => UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.troop_forbidden_retreat_pullback_defeated), TipType.Info)
                );
            } else if (timeInMiniute > 5) {
                this.viewPref.btnRetreat.Grayable = true;
                this.viewPref.btnRetreat.onClick.AddListener(
                    () => UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.troop_forbidden_retreat_exceed_time), TipType.Info)
                );
            } else {
                this.viewPref.btnRetreat.Grayable = false;
                this.viewPref.btnRetreat.onClick.AddListener(this.OnBtnRetreatClick);
            }
        }

        private void SetItem(Transform item, string attribute, string value, Color color) {
            item.Find("ImgIcon").GetComponent<Image>().sprite =
                     ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix + attribute);
            item.Find("ImgState").gameObject.SetActiveSafe(false);
            item.Find("Text").GetComponent<TextMeshProUGUI>().text = value;
            item.Find("Text").GetComponent<TextMeshProUGUI>().color = color;
        }

        public void SetTroopLabel(string label) {
            this.viewPref.txtTroopLabel.text = label;
        }

        private void SetAttackStatus() {
            this.viewPref.pnlOverviewBind.gameObject.SetActiveSafe(false);
            UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, false);
        }

        public void SetArrowMask(TileArrowTrans tileArrowTrans) {
            Debug.Log("SetArrowMask");
            if (tileArrowTrans == TileArrowTrans.upgrade) {
                this.showCompletedCallback += () => {
                    if (this.btnDict.ContainsKey(TileButtonType.Upgrade)) {
                        FteManager.SetArrow(
                            this.btnDict[TileButtonType.Upgrade].transform
                        );
                    }
                };
            } else if (tileArrowTrans == TileArrowTrans.Attack) {
                this.showCompletedCallback += () => {
                    if (this.btnDict.ContainsKey(TileButtonType.Attack)) {
                        FteManager.SetArrow(
                            this.btnDict[TileButtonType.Attack].transform
                        );
                    }

                };
            }
            this.afterHideCallback += () => {
                FteManager.HideArrow();
                this.showCompletedCallback = null;
            };
        }

        private void FormatButtons(Transform root) {
            int count = 0;
            foreach (Transform child in root) {
                if (child.gameObject.activeSelf) {
                    count++;
                }
            }
            int index = 0;
            foreach (Transform child in root) {
                if (child.gameObject.activeSelf) {
                    child.GetChild(0).GetComponent<RectTransform>().anchoredPosition =
                        Vector2.right * ((count - 1) / 2f - index).Abs() * (-5);
                    index++;
                }
            }
        }

        public void OnBtnTroopDetailClick(string playerId) {
            this.viewModel.ShowPlayerInfo(playerId);
        }

        public void OnBtnReturnClick() {
            this.afterReturnClick.InvokeSafe();
            this.afterReturnClick = null;
            if (isShowTroop) {
                this.viewModel.TroopReturnReq();
            } else {
                this.isChosenTroop = true;
                UIManager.SetUICanvasGroupVisible(this.viewPref.tileButtonsCG, false);
                this.viewModel.HideActivityTileView();
                this.viewModel.Return();
                this.ShowTroopChose(TroopChoseAction.Return);
            }
        }

        public void OnBtnRetreatClick() {
            this.SetBtnRetreatInteractable(false);
            this.viewModel.MarchRetreatReq();
        }

        public void SetBtnRetreatInteractable(bool interactable) {
            this.viewPref.btnRetreat.Grayable = !interactable;
            this.viewPref.btnRetreat.onClick.RemoveAllListeners();
            if (interactable) {
                this.viewPref.btnRetreat.onClick.AddListener(this.OnBtnRetreatClick);
            }
        }

        public void OnBtnRecruitClick() {
            if (isShowTroop) {
                this.viewModel.HideTileInfo();
                this.viewModel.ShowRecruit();
            } else {
                this.isChosenTroop = true;
                this.viewPref.pnlRight.gameObject.SetActive(false);
                this.viewModel.HideActivityTileView();
                this.ShowTroopChose(TroopChoseAction.Recruit);
            }
        }

        public void OnBtnFormatClick() {
            if (isShowTroop) {
                this.viewModel.HideTileInfo();
                this.viewModel.ShowTroopFormation(this.viewModel.CurrentTroop.Id);
            } else {
                this.isChosenTroop = true;
                this.viewPref.pnlRight.gameObject.SetActive(false);
                this.ShowTroopChose(TroopChoseAction.Format);
            }
        }

        public void OnBtnUpgradeClick() {
            if (this.grayBtnList.Contains(TileButtonType.Upgrade)) {
                UIManager.ShowTip(
                    "无法升级"
                 /*LocalManager.GetValue(LocalHashConst.land_under_fresh_protection)*/, TipType.Notice);
                return;
            }
            UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
            this.viewModel.ShowBuilding(BuildViewType.Upgrade,
                this.viewModel.TileInfo.buildingInfo.GetNextLevelId());
        }

        public void OnBtnBuildClick() {
            this.viewModel.HideTileInfo();
            this.viewModel.ShowBuildList();
        }

        private void OnBtnCancelUpgradeClick() {
            this.viewModel.CancelUpgradeBuilding();
        }

        private void OnBtnAttackClick() {
            List<Troop> avaliableTroopList = this.viewModel.troopModel.GetAvaliableTroop(this.viewModel.TileInfo.coordinate);
            if (avaliableTroopList.Count <= 0) {
                UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.troop_not_available),
                    TipType.Info);
                this.viewModel.parent.SetQueueIsFold(false);
                this.viewModel.Hide();
                this.viewModel.HideActivityTileView();
                return;
            }
            if (this.rightBtnList.Contains(TileButtonType.Attack) &&
                this.grayBtnList.Contains(TileButtonType.Attack)) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.land_under_fresh_protection), TipType.Notice);
                return;
            }
            this.viewModel.HideTileReward();
            if (this.viewModel.IsTileCoordReachable()) {
                this.SetAttackStatus();
                Vector2 coordinate = this.viewModel.TileInfo.coordinate;
                this.viewModel.Attack(MapUtils.CoordinateToPosition(coordinate));
            } else {
                this.viewModel.Hide();
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.warning_tile_can_not_reach),
                    TipType.Warn);
            }
        }

        private void OnBtnMoveClick() {
            if (this.grayBtnList.Contains(TileButtonType.Move)) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.troop_at_tile_all),
                    TipType.Warn
                );
            } else if (this.viewModel.IsTileCoordReachable()) {
                this.SetAttackStatus();
                Vector2 coordinate = this.viewModel.TileInfo.coordinate;
                this.viewModel.Move(MapUtils.CoordinateToPosition(coordinate));
            } else {
                this.viewModel.Hide();
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.warning_tile_can_not_reach), TipType.Warn);
            }
        }

        public void OnPageChange() {
            if (this.viewModel.Page == TilePage.Main) {
                this.HideSubView();
                this.ShowOverview();
            } else if (this.viewModel.Page == TilePage.None ||
                this.viewModel.Page == TilePage.March) {
                this.viewModel.ShowTileBindUI();
            } else if (this.viewModel.Page == TilePage.Monster) {
                UIManager.SetUICanvasGroupVisible(this.viewPref.terrainBindCG, false);
                this.ShowButtons();
            }
        }

        private void OnBtnGiveUpClick() {
            if (this.grayBtnList.Contains(TileButtonType.Giveup)) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_abandon_point_is_avoid), TipType.Info);
            } else {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.button_tile_giveup),
                    LocalManager.GetValue(LocalHashConst.land_give_confirm),
                    this.viewModel.AbandonPointReq, () => { });
            }
        }

        private void OnBtnMarkClick() {
            this.viewModel.Mark();
        }

        private void OnBtnDeleteMarkClick() {
            this.viewModel.DeleteMark();
        }


        #region alliance mark
        private void OnAllianceMarkCancelClick() {
            this.HideAllianceMarkEditView();
            this.viewModel.Show();
            this.viewModel.SetTileInfo(this.viewModel.TileInfo);
        }

        private void OnAllianceMarkConfirmClick() {
            this.viewModel.AddAllianceMark();
        }

        private void OnBtnAllianceMarkClick() {
            this.viewModel.ShowAllianceMarkEditView();
        }

        private void OnIfNameValueChange(string value) {
            this.viewModel.AllianceMarkName = value;
        }
        #endregion

        private void OnBtnGiveUpAllianceClick() {
            this.viewModel.GiveUpAllianceMark();
        }

        private void OnStrongholdGiveUpClick() {
            UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.button_tile_strongholdgiveup),
                LocalManager.GetValue(LocalHashConst.stronghold_give_up_confirm),
               this.viewModel.GiveUpStronghold, () => { });
        }

        private void OnBtnDetailClick() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            if (tileInfo.type.CustomEquals(ElementCategory.building) &&
                tileInfo.playerName.CustomEquals(RoleManager.GetRoleName())) {
                UIManager.SetUICanvasGroupEnable(this.viewPref.bottomCG, false);
                this.viewModel.ShowBuilding(BuildViewType.Info,
                    tileInfo.buildingInfo.GetId());
            } else {
                this.ShowDetail();
            }
        }

        private void OnBtnDetailCloseClick() {
            this.viewModel.UIReturn();
        }

        private void OnBtnPlayerInfoClick() {
            this.viewModel.ShowPlayerDetailInfo();
        }

        #region FTE

        public void OnTroopStep2Start() {
            this.showCompletedCallback += () => {
                this.viewModel.OnTroopStep2Process();
                if (this.viewModel.TileInfo.coordinate == RoleManager.GetRoleCoordinate()) {
                    if (this.btnDict.ContainsKey(TileButtonType.Format)) {
                        FteManager.SetMask(this.btnDict[TileButtonType.Format].pnlContent, isButton: true);
                    }
                } else {
                    if (FteManager.Instance.curStep.CustomEquals("chapter_task_8")) {
                        this.afterReturnClick += () => {
                            TriggerManager.Invoke(Trigger.DramaArrow);
                        };
                    }
                    if (this.btnDict.ContainsKey(TileButtonType.Return)) {
                        FteManager.SetArrow(this.btnDict[TileButtonType.Return].pnlContent);
                    }
                }
            };

        }

        public void OnRecruitStep2Start() {
            this.showCompletedCallback += () => {
                if (this.viewModel.TileInfo.coordinate == RoleManager.GetRoleCoordinate() &&
                    this.btnDict.ContainsKey(TileButtonType.Recruit)) {
                    FteManager.SetMask(this.btnDict[TileButtonType.Recruit].pnlContent, isButton: true);
                } else if (this.btnDict.ContainsKey(TileButtonType.Return)) {
                    FteManager.SetMask(this.btnDict[TileButtonType.Return].pnlContent, isButton: true, autoNext: false);
                }
            };
        }

        public void OnBuildUpStep1Start() {
            this.showCompletedCallback += () => {
                if (!FteManager.Instance.curStep.CustomIsEmpty()) {
                    if (this.rightBtnList.Contains(TileButtonType.Upgrade)) {
                        FteManager.SetMask(btnDict[TileButtonType.Upgrade].pnlContent, isButton: true);
                    }
                }
            };
        }

        public void OnBuildUpStep1End() {
            this.afterHideCallback = null;
            this.OnBtnUpgradeClick();
        }

        public void OnResourceStep2Start(bool isEnforce) {
            if (!this.IsVisible) {
                this.showCompletedCallback += () => {
                    if (this.rightBtnList.Contains(TileButtonType.Attack)) {
                        FteManager.SetMask(
                            this.btnDict[TileButtonType.Attack].pnlContent,
                            isButton: true,
                            isEnforce: isEnforce
                        );
                    }
                };
            } else {
                FteManager.StopFte();
                this.viewModel.StartChapterDailyGuid();
            }
        }

        public void OnResourceStep3Start() {
            this.OnBtnAttackClick();
        }

        public void SetFteUI() {
            CustomButton btn;
            foreach (var ui in this.fteUIDict) {
                if (this.btnDict.TryGetValue(ui.Value, out btn)) {
                    btn.gameObject.SetActiveSafe(FteManager.CheckUI(ui.Key));
                }
            }
        }

        public void OnResourceStep1SetActive(bool canShow) {
            if (this.leftBtnList.Contains(TileButtonType.Mark)) {
                this.btnDict[TileButtonType.Mark].gameObject.SetActiveSafe(canShow);
            }
        }
        #endregion

        protected override void OnVisible() {
            this.target.gameObject.SetActiveSafe(true);
            this.ResetAllianceMarkView();
        }

        protected override void OnInvisible() {
            this.target.gameObject.SetActiveSafe(false);
            this.viewModel.HideTroopSelect();
            this.viewModel.HideSelfMarch();
        }
    }
}