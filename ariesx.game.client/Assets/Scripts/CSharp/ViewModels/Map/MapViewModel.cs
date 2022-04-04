using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class MapViewModel: BaseViewModel {
        private MapModel model;
        private TroopModel troopModel;
        private MapMarkModel markModel;
        private ChatRoomModel chatRoomModel;
        private MapView view;
        private BuildModel buildModel;
        private DramaModel dramaModel;
        public List<Vector2> bossInfoChangeList = new List<Vector2>();
        private Coord currentCoord = new Coord();
        public UnityAction operateAction;
        public Vector2 CenterCoordinate {
            get {
                return this.model.centerCoordinate;
            }
            set {
                if (this.model.centerCoordinate != value) {
                    this.model.centerCoordinate = value;
                    this.OnCenterChanged();
                }
            }
        }

        public Vector2 ResourceBlock {
            get {
                return this.model.resourceInfo.block;
            }
            private set {
                if (this.model.resourceInfo.block != value) {
                    this.model.resourceInfo.SetBlock(value);
                    this.OnResourceBlockChanged();
                }
            }
        }

        public Rect ResourceBlockRect {
            get {
                return this.model.resourceInfo.blockRect;
            }
        }

        public int FreeLotteryCount {
            get {
                return this.heroViewModel.FreeLotteryCount;
            }
            set {
                this.heroViewModel.FreeLotteryCount = value;
            }
        }

        public int NewHeroCount {
            get {
                return this.heroViewModel.NewHeroCount;
            }
            set {
                this.heroViewModel.NewHeroCount = value;
            }
        }

        public int CanLevelUpCount {
            get {
                return this.heroViewModel.CanLevelUpCount;
            }
            set {
                this.heroViewModel.CanLevelUpCount = value;
            }
        }

        public Vector2 ResourceBlockSize {
            get {
                return this.model.resourceInfo.blockSize;
            }
        }

        public Vector2 PlayerBlock {
            get {
                return this.model.playerInfo.block;
            }
            private set {
                if (this.model.playerInfo.block != value) {
                    this.model.playerInfo.SetBlock(value);
                    this.NeedFeedBlock = true;
                }
            }
        }

        public Rect PlayerBlockRect {
            get {
                return this.model.playerInfo.blockRect;
            }
        }

        public Vector2 PlayerBlockSize {
            get {
                return this.model.playerInfo.blockSize;
            }
        }

        public Dictionary<Vector2, Dictionary<uint, uint>> TileDict {
            get {
                return this.model.resourceInfo.infoDict;
            }
        }

        public MapCameraInfo CameraInfo {
            get {
                return this.model.cameraInfo;
            }
        }

        public Vector2 MaxCoordinate {
            get {
                return this.model.maxCoordinate;
            }
        }

        public Vector2 MinCoordinate {
            get {
                return this.model.minCoordinate;
            }
        }

        public MapTileInfo CurrentTile {
            get {
                return this.model.currentTile;
            }
        }
        //private Vector2 preCoordinate = Vector2.zero;

        public bool TreasureMapEnable {
            get {
                return this.model.treasureMapRefresh < RoleManager.GetCurrentUtcTime() / 1000;
            }
        }

        public long TreasureMapRefresh {
            get {
                return this.model.treasureMapRefresh;
            }
            set {
                if (this.model.treasureMapRefresh != value) {
                    this.model.treasureMapRefresh = value;
                }
            }
        }

        public Dictionary<Vector3, MapMark> MarkDict {
            get {
                return this.markModel.markDict;
            }
        }

        public string CurrentMarch {
            get; set;
        }

        public bool NeedFeedBlock {
            get; set;
        }

        public bool IsResourceShow {
            get; set;
        }

        public bool IsTileInfoShow {
            get; set;
        }

        public bool IsQueueShow {
            get; set;
        }

        public bool NeedShowFirstDownReward {
            get; set;
        }

        public Dictionary<string, UnityEvent> feedblockEventDict =
            new Dictionary<string, UnityEvent>();

        //public Dictionary<string, UnityEvent> feedblockMonsterEventDict =
        //    new Dictionary<string, UnityEvent>();

        //public Dictionary<string, UnityEvent> feedblockDominationEventDict =
        //   new Dictionary<string, UnityEvent>();

        public UnityAction moveEvent = null;
        public UnityEvent followMarchEvent = new UnityEvent();

        private IEnumerator serilizer;
        private int viewCount = 0;
        private MapTileViewModel tileViewModel;
        private ActivityMapTileViewModel activityMapTileViewModel;
        private QueueViewModel queueViewModel;
        private MapTopHUDViewModel topViewModel;
        private MapRightHUDViewModel rightViewModel;
        private BuildInfoViewModel buildInfoViewModel;
        private MailViewModel mailViewModel;
        private RankViewModel rankViewModel;
        private BuildEditViewModel buildEditViewModel;
        private HeroViewModel heroViewModel;
        private HeroInfoViewModel heroInfoViewModel;
        private TroopInfoViewModel troopInfoViewModel;
        private TroopFormationViewModel troopFormationViewModel;
        private RecruitViewModel recruitViewModel;
        private AllianceViewModel allianceViewModel;
        private FallenInfoViewModel fallenInfoViewModel;
        private ChatRoomViewModel chatRoomViewModel;
        private PlayerAllianceInfoViewModel playerAllianceInfo;
        private MissionViewModel missionViewModel;
        private SelectServerViewModel selectServerViewModel;
        private BannaerViewModel BannerViewModel {
            get {
                if (this.bannerViewModel == null) {
                    this.bannerViewModel =
                        PoolManager.GetObject<BannaerViewModel>(this.transform);
                }
                return this.bannerViewModel;
            }
        }
        private BannaerViewModel bannerViewModel;
        private MiniMapViewModel miniMapViewModel;
        private FirstDownRewardViewModel fieldFirstDownViewModel;
        private TributeViewModel tributeViewModel;
        private TreasureMapViewModel treasureViewModel;
        private HeroPoolViewModel heroPoolViewModel;
        private BattleReportTipViewModel reportTipViewModel;
        private HouseKeeperViewModel houseKeeperViewModle;
        private PayViewModel payViewModel;
        private BuildUnlockViewModel buildUnlockViewModel;
        private PlayerInfoViewModel playerInfoViewModel;
        private CampaignViewModel campaignViewModel;
        private CampaignRewardViewModel campainRewardViewModel;
        private CampaignRuleViewModel campaignRuleViewModel;
        private GameSettingViewModel gameSettingViewModel;
        private DisplayBoardViewModel displayBoardViewModel;
        private CampaignRewardDominationViewModel campaignRewardDominationViewModel;
        private CampaignCitySelectViewModel campaignCitySelectViewModel;
        private DemonShadowHistoryRankViewModel demonShadowHistoryRankViewModel;
        private BossInfoViewModel bossInfoViewModel;
        private NoviceStateViewModel noviceStateViewModel;
        private GetTileTipViewModel getTileTipViewModel;
        private PayRewardViewModel payRewardViewModel;
        private DailyRewardReceivedViewModel dailyRewardReceivedViewModel;
        private DailyRewardNoRewardViewModel dailyRewardNoRewardViewModel;
        private bool isSetDailyLimit = false;
        private int dailyTaskCollectableCount = 0;
        private int dramaTaskCollectableCount = 0;

        private Dictionary<string, IViewModel> aboveUIDict =
            new Dictionary<string, IViewModel>();

        void Awake() {
            this.model = ModelManager.GetModelData<MapModel>();
            this.chatRoomModel = ModelManager.GetModelData<ChatRoomModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.markModel = ModelManager.GetModelData<MapMarkModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.dramaModel = ModelManager.GetModelData<DramaModel>();
            this.view = this.gameObject.AddComponent<MapView>();
            //#if UNITY_EDITOR || DEVELOPER
            //            long start = System.DateTime.UtcNow.Ticks;
            //#endif
            this.AddNtfHanders();
            this.RegistTriggers();
            this.SetFteConfigures();
            this.AddEventActions();
            this.AttatchViewModels();
            this.NeedFeedBlock = false;
            //Debug.LogWarning("Map init time cost: " + (System.DateTime.UtcNow.Ticks - start));
        }

        // Net handler
        private void AddNtfHanders() {
            NetHandler.AddNtfHandler(typeof(FeedBlocksNtf).Name, this.FeedBlocksNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlockMonsterNtf).Name, this.FeedBlockMonsterNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlocksMarchesNtf).Name, this.FeedBlocksMarchesNtf);
            NetHandler.AddDataHandler(typeof(PlayerPointNtf).Name, this.PlayerPointNtf);
            NetHandler.AddNtfHandler(typeof(EventMarchNtf).Name, this.EventMarchNtf);
            NetHandler.AddNtfHandler(typeof(AbandonPointAck).Name, this.AbandonPointAck);
            NetHandler.AddNtfHandler(typeof(EventAbandonNtf).Name, this.EventAbandonNtf);
            NetHandler.AddNtfHandler(typeof(EventBuildNtf).Name, this.EventBuildNtf);
            NetHandler.AddNtfHandler(typeof(EventGiveUpNtf).Name, this.EventGiveUpNtf);
            NetHandler.AddNtfHandler(typeof(PlayerBuildingNtf).Name, this.PlayerBuildingNtf);
            NetHandler.AddNtfHandler(typeof(TroopArrivedNtf).Name, this.TroopArrivedNtf);
            NetHandler.AddNtfHandler(typeof(TroopHasBeenAttackedNtf).Name, this.TroopHasBeenAttackedNtf);
            NetHandler.AddNtfHandler(typeof(RelationNtf).Name, this.OnRelationChangeNtf);
            NetHandler.AddNtfHandler(typeof(TroopNtf).Name, this.OnTroopNtf);
            NetHandler.AddNtfHandler(typeof(NewTroopNtf).Name, this.NewTroopNtf);
            NetHandler.AddNtfHandler(typeof(FieldFirstDownNtf).Name, this.FieldFirstDownNtf);
            NetHandler.AddNtfHandler(typeof(ChapterTaskChangeNtf).Name, this.ChapterTaskChangeNtf);
            NetHandler.AddDataHandler(typeof(AllianceInfosNtf).Name, this.AllianceInfosNtf);
            NetHandler.AddDataHandler(typeof(FeedBlockDominationNtf).Name, this.FeedBlockDominationNtf);
            NetHandler.AddNtfHandler(typeof(ForceNtf).Name, this.ForceNtf);
            NetHandler.AddNtfHandler(typeof(ActivityNtf).Name, this.ActivityNtf);
            NetHandler.AddNtfHandler(typeof(DailyLimitNtf).Name, this.DailyLimitNtf);
        }

        // Trigger handler
        private void RegistTriggers() {
            TriggerManager.Regist(Trigger.CameraMove, this.Move);
            TriggerManager.Regist(Trigger.CameraFocus, this.Focus);
            TriggerManager.Regist(Trigger.ResourceChange, this.RefreshBuildUpgradeableStatus);
            TriggerManager.Regist(Trigger.HeroStatusChange, this.SetBtnGacha);
            TriggerManager.Regist(Trigger.Fte, this.SetFteUI);
            TriggerManager.Regist(Trigger.HeroArmyAmountChange, this.OnHeroArmyAmountChange);
            TriggerManager.Regist(Trigger.VoiceLiveStatusChange, this.OnVoiceLiveStatusChange);
            TriggerManager.Regist(Trigger.ChestCollect, this.view.ShowGetChest);
            TriggerManager.Regist(Trigger.DramaArrow, this.view.SetDramaArrow);
            TriggerManager.Regist(Trigger.ShowBtnBuild, this.view.SetBtnBuild);
        }

        // Fte
        // To do: Start Fte and checkout the progress, start from the step.
        private void SetFteConfigures() {
            FteManager.SetStartCallback(GameConst.NORMAL, 51, this.OnFteStep51Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 57, this.OnFteStep57Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 59, this.OnFteStep59Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 61, this.OnFteStep61Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 61, this.OnFteStep61End);
            FteManager.SetStartCallback(GameConst.NORMAL, 81, this.OnFteStep81Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 101, this.OnFteStep101Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 101, this.OnFteStep101End);
            FteManager.SetStartCallback(GameConst.NORMAL, 111, this.OnFteStep111Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 140, this.OnFteStep140Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 140, this.OnFteStep140End);
            FteManager.SetStartCallback(GameConst.BUILDING_LEVEL, 1, this.OnBuildStep1Start);
            FteManager.SetEndCallback(GameConst.BUILDING_LEVEL, 1, this.OnBuildStep1End);
            FteManager.SetStartCallback(GameConst.TROOP_ADD_HERO, 2, this.OnTroopStep2Start);
            FteManager.SetStartCallback(GameConst.RESOURCE_LEVEL, 1, this.OnResourceStep1Start);
            FteManager.SetEndCallback(GameConst.RESOURCE_LEVEL, 2, this.OnResourceStep2End);
            FteManager.SetStartCallback(GameConst.RECRUIT, 2, this.OnRecruitStep2Start);
            FteManager.SetStartCallback(GameConst.BUILDING_UPGRADE, 1, this.OnBuildUpStep1Start);
            FteManager.SetEndCallback(GameConst.BUILDING_UPGRADE, 1, this.OnBuildUpStep1End);
            FteManager.SetStartCallback(GameConst.JOIN_ALLIANCE, 1, this.OnAllianceStep1Start);
            FteManager.SetEndCallback(GameConst.JOIN_ALLIANCE, 1, this.OnAllianceStep1End);
            FteManager.SetStartCallback(GameConst.ALLIANCE_CHAT, 1, this.OnChatStep1Start);
            FteManager.SetEndCallback(GameConst.ALLIANCE_CHAT, 1, this.OnChatStep1End);

        }

        // Event handler
        private void AddEventActions() {
            EventManager.AddEventAction(Event.March, this.UpdateMarch);
            EventManager.AddEventAction(Event.Abandon, this.UpdateAbandon);
            EventManager.AddEventAction(Event.GiveUpBuild, this.UpdateGiveUpBuilding);
            EventManager.AddEventAction(Event.Build, this.UpdateBuildEvent);
            EventManager.AddEventAction(Event.Shield, this.UpdateShield);
            EventManager.AddEventAction(Event.Tribute, this.view.UpdateTributeTime);
        }
        // Child view model
        private void AttatchViewModels() {
            this.queueViewModel = PoolManager.GetObject<QueueViewModel>(this.transform);
            this.tileViewModel = PoolManager.GetObject<MapTileViewModel>(this.transform);
            this.topViewModel = PoolManager.GetObject<MapTopHUDViewModel>(this.transform);
            this.rightViewModel = PoolManager.GetObject<MapRightHUDViewModel>(this.transform);
            this.mailViewModel = PoolManager.GetObject<MailViewModel>(this.transform);
            this.houseKeeperViewModle = PoolManager.GetObject<HouseKeeperViewModel>(this.transform);
            this.payViewModel = PoolManager.GetObject<PayViewModel>(this.transform);
            this.buildInfoViewModel = PoolManager.GetObject<BuildInfoViewModel>(this.transform);
            this.buildEditViewModel = PoolManager.GetObject<BuildEditViewModel>(this.transform);
            this.heroViewModel = PoolManager.GetObject<HeroViewModel>(this.transform);
            this.heroInfoViewModel = PoolManager.GetObject<HeroInfoViewModel>(this.transform);
            this.troopInfoViewModel = PoolManager.GetObject<TroopInfoViewModel>(this.transform);
            this.troopFormationViewModel = PoolManager.GetObject<TroopFormationViewModel>(this.transform);
            this.recruitViewModel = PoolManager.GetObject<RecruitViewModel>(this.transform);
            this.allianceViewModel = PoolManager.GetObject<AllianceViewModel>(this.transform);
            this.fallenInfoViewModel = PoolManager.GetObject<FallenInfoViewModel>(this.transform);
            this.chatRoomViewModel = PoolManager.GetObject<ChatRoomViewModel>(this.transform);
            this.missionViewModel = PoolManager.GetObject<MissionViewModel>(this.transform);
            this.miniMapViewModel = PoolManager.GetObject<MiniMapViewModel>(this.transform);
            this.fieldFirstDownViewModel = PoolManager.GetObject<FirstDownRewardViewModel>(this.transform);
            this.playerInfoViewModel = PoolManager.GetObject<PlayerInfoViewModel>(this.transform);
            this.campaignViewModel = PoolManager.GetObject<CampaignViewModel>(this.transform);
            this.displayBoardViewModel = PoolManager.GetObject<DisplayBoardViewModel>(this.transform);
            this.campaignRewardDominationViewModel = PoolManager.GetObject<CampaignRewardDominationViewModel>(this.transform);
            this.campaignCitySelectViewModel = PoolManager.GetObject<CampaignCitySelectViewModel>(this.transform);
            this.demonShadowHistoryRankViewModel = PoolManager.GetObject<DemonShadowHistoryRankViewModel>(this.transform);
            this.activityMapTileViewModel = PoolManager.GetObject<ActivityMapTileViewModel>(this.transform);
            this.bossInfoViewModel = PoolManager.GetObject<BossInfoViewModel>(this.transform);
            this.noviceStateViewModel = PoolManager.GetObject<NoviceStateViewModel>(this.transform);
            this.getTileTipViewModel = PoolManager.GetObject<GetTileTipViewModel>(this.transform);
            this.payRewardViewModel = PoolManager.GetObject<PayRewardViewModel>(this.transform);
            this.selectServerViewModel = PoolManager.GetObject<SelectServerViewModel>(this.transform);
            this.dailyRewardReceivedViewModel = PoolManager.GetObject<DailyRewardReceivedViewModel>(transform);
            this.dailyRewardNoRewardViewModel = PoolManager.GetObject<DailyRewardNoRewardViewModel>(transform);
            this.reportTipViewModel =
                PoolManager.GetObject<BattleReportTipViewModel>(this.transform);
        }

        IEnumerator Start() {
            this.InitTime();
            this.view.Init();
            // Other init
            this.InitMarchEvent();
            this.InitTribute();
            this.InitPayReward();
            this.NotifiesReq();
            this.ChatRoomReq();
            this.SetBtnBuild();
            this.SetBtnGacha();
            this.SetPlayerFallenInfo();
            this.rightViewModel.Show();

            if (RoleManager.IsNewPlayer()) {
                TriggerManager.Invoke(Trigger.FirstLogin);
            }
            // Fte
            UpdateManager.Regist(UpdateInfo.MapViewModel, this.UpdateAction);
            yield return ModelManager.LoadSceneAsync("SceneChest");
            // Tmp
            OpenChestView.tmpLottey = GameObject.FindGameObjectWithTag("Lottery");
            OpenChestView.tmpLottey.SetActiveSafe(false);
            HeroTierUpView.tmpUpgrade = GameObject.FindGameObjectWithTag("Upgrade");
            HeroTierUpView.tmpUpgrade.SetActiveSafe(false);
            UIManager.ChestCamera = GameObject.FindGameObjectWithTag("ChestCamera");
            UIManager.ChestCamera.SetActiveSafe(false);
        }

        private void InitTime() {
            long startTime = (long)(new DateTime(2018, 2, 4)).Subtract(GameConst.ORIGIN_TIME).TotalMilliseconds;
            long endTime = (long)(new DateTime(2018, 2, 25)).Subtract(GameConst.ORIGIN_TIME).TotalMilliseconds;
            TileView.needDecoration = RoleManager.GetCurrentLocalTime() > startTime &&
                RoleManager.GetCurrentLocalTime() < endTime;
        }

        private void InitMarchEvent() {
            foreach (EventBase march in EventManager.EventDict[Event.March].Values) {
                this.view.CreateMarch((EventMarchClient)march);
            }
        }

        private void InitTribute() {
            if (RoleManager.GetTributeStatus() && this.buildModel.GetTownhallLevel() >= 3) {
                this.view.ShowTributeObjDirectly();
            }
        }

        private void InitPayReward() {
            RechargeRewardInfoReq req = new RechargeRewardInfoReq();
            NetManager.SendMessage(req, typeof(RechargeRewardInfoAck).Name, this.RechargeRewardInfoAck);
        }

        private void RechargeRewardInfoAck(IExtensible message) {
            RechargeRewardInfoAck ack = message as RechargeRewardInfoAck;
            PayModel model = ModelManager.GetModelData<PayModel>();
            model.payRewardLevel = ack.PlayerRewardLevel;
            model.payAmount += ack.PlayerRechargeAmount;
            this.view.SetBtnPayReward(ack.PlayerRewardLevel);
        }

        private void ActivityNtf(IExtensible message) {
            ActivityNtf ntf = message as ActivityNtf;
            foreach (Activity activity in ntf.Activity) {
                if (activity.Melee != null && activity.Status == Activity.ActivityStatus.Started) {
                    foreach (var pair in this.model.GetMonsterPoint()) {
                        foreach (Vector2 point in pair.Value.Keys) {
                            this.view.RefreshTile(point);
                        }
                    }
                }
            }
        }
        
        private void DailyLimitNtf(IExtensible message) {
            DailyLimitNtf ntf = message as DailyLimitNtf;
            if (!this.isSetDailyLimit) {
                this.isSetDailyLimit = true;
                this.view.SetChestLimit();
                return;
            } 
            switch (ntf.Limit.ChestCurrent) {
                case 1:
                case 5:
                case 10:
                case 17:
                    this.view.afterChestCollect = () => {
                        this.view.SetChestBtnHighlight();
                        FteManager.SetLeftChat(
                            string.Format(LocalManager.GetValue(LocalHashConst.introduce_chest_loot_desc),
                            ntf.Limit.ChestCurrent.ToString(),
                            (20 - ntf.Limit.ChestCurrent).ToString())
                            , true,
                            () => {
                                FteManager.RemoveMask();
                            });
                    };
                    break;
                case 20:
                    if (!this.DailyRewardComplete(ntf.Limit)) {
                        string lastTime = 
                            PlayerPrefs.GetString("ResourceChatTime" + 
                            RoleManager.GetRoleId());
                        if (lastTime == null || long.Parse(lastTime) <
                            RoleManager.GetZeroTime()) {
                            this.view.afterBattleGetResouse += () => {
                                PlayerPrefs.SetString("ResourceChatTime" + RoleManager.GetRoleId(),
                                    RoleManager.GetCurrentUtcTime().ToString());
                                FteManager.SetRightChat(
                                    LocalManager.GetValue(LocalHashConst.introduce_resouce_increase_desc)
                                    ,
                                    () => {
                                        this.topViewModel.SetResourceScreenEffect();
                                    });
                            };
                        }
                        break;
                    }
                    this.view.afterChestCollect = () => {
                        this.view.SetHouseKeeperBtnHighlight();
                        FteManager.SetRightChat(
                            LocalManager.GetValue(LocalHashConst.introduce_resouce_loot_desc)
                            ,
                            () => {
                                FteManager.RemoveMask();
                                this.view.ShowBuildBtn();
                                StartCoroutine(this.view.SetHousekeeperNotice(false));
                            });
                    };
                    break;
            }
        }

        public bool DailyRewardComplete(DailyLimit dailyLimit) {
            if (dailyLimit.ResourceCurrent.Lumber
                != dailyLimit.ResourceLimit.Lumber)
                return true;
            if (dailyLimit.ResourceCurrent.Food
                != dailyLimit.ResourceLimit.Food)
                return true;
            if (dailyLimit.ResourceCurrent.Steel
                != dailyLimit.ResourceLimit.Steel)
                return true;
            if (dailyLimit.ResourceCurrent.Marble
                != dailyLimit.ResourceLimit.Marble)
                return true;
            if (dailyLimit.GoldCurrent
                != dailyLimit.GoldLimit)
                return true;
            return false;
        }

        public void SetTileLimitChat(UnityAction afterCallback) {

            if ((RoleManager.GetPointsLimit() - RoleManager.GetPointDict().Count) <= 2 &&
                FteManager.FteOver){
                string lastTime =
                               PlayerPrefs.GetString("TileLimitChatTime" +
                               RoleManager.GetRoleId());
                if (lastTime == string.Empty || long.Parse(lastTime) <
                    RoleManager.GetZeroTime()) {
                        PlayerPrefs.SetString("TileLimitChatTime" + RoleManager.GetRoleId(),
                            RoleManager.GetCurrentUtcTime().ToString());
                        FteManager.SetRightChat(
                            LocalManager.GetValue(LocalHashConst.introduce_tile_limit_short_desc)
                            ,
                            () => {
                                this.topViewModel.SetPlayerInfoScreenEffect(afterCallback);
                            });
                } else {
                    afterCallback.InvokeSafe();
                }
            } else {
                afterCallback.InvokeSafe();
            }
        }

        private void UpdateAction() {
            if (this.NeedFeedBlock &&
                this.view.Velocity.sqrMagnitude <= 81f) {
                if (this.view.velocityCount > 3) {
                    this.FeedBlocksReq();
                    this.NeedFeedBlock = false;
                    this.view.velocityCount = 0;
                } else {
                    this.view.velocityCount++;
                }
            }
        }



        // To do : Move param is different to MoveWithClick param;
        public void Move(Vector2 coordinate) {
            Vector2 position = MapUtils.CoordinateToPosition(coordinate);
            this.view.Move(position);
        }

        public void MoveToBattlePoint(Vector2 coordinate) {
            this.view.MoveToBattlePoint(coordinate);
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.Move(coordinate);
            this.moveEvent = () => this.OnTileClick(coordinate, delayClick: true);
        }

        public void MoveWithClickAttack(Vector2 coordinate, TileArrowTrans tileArrowTrans) {
            this.tileViewModel.Hide();
            this.Move(coordinate);
            this.moveEvent = () => this.OnTileClick(coordinate, delayClick: true);
            this.tileViewModel.SetMask(tileArrowTrans);
        }

        private void AddFeedblockCallback(Vector2 coordinate, UnityAction action) {
            Vector2 block = this.model.GetPlayerBlock(coordinate);
            string offset = string.Concat(block.x, ",", block.y);
            UnityEvent callbackEvent;
            if (!this.feedblockEventDict.TryGetValue(offset, out callbackEvent)) {
                callbackEvent = new UnityEvent();
                this.feedblockEventDict.Add(offset, callbackEvent);
            }
            callbackEvent.AddListener(action);
        }

        public void MoveWithEvent(Vector2 coordinate, UnityAction action) {
            this.Move(coordinate);
            this.moveEvent = action;
        }

        public void Focus(Vector2 position) {
            this.view.Focus(position);
        }

        public void LoseFocus() {
            this.view.LoseFocus();
        }

        public void FollowMarch(string id) {
            this.CurrentMarch = id;
            this.view.FollowMarch(id);
        }

        public void FollowNewMarch(string id) {
            this.CurrentMarch = id;

            this.view.FollowNewMarch(id);
        }

        public void DisFollowNewMarch() {
            this.view.DisFollowNewMarch();
        }

        public void EnableAboveUICamera() {
            this.view.EnableAboveUICamera();
        }

        public void DisableAboveUICamera() {
            this.view.DisableAboveUICamera();
        }

        public void EnableChoseEffect() {
            this.Move(this.CurrentTile.coordinate);
            this.view.EnableChoseEffect(this.CurrentTile.coordinate);
        }

        public void DisableChoseEffect() {
            this.view.DisableChosenEffect();
        }

        //public void ShowChoseEffect() {
        //    this.view.ShowChoseEffect();
        //}

        public int GetRightButtonsCount() {
            return this.tileViewModel.GetRightButtonsCount();
        }

        public Transform GetTileTargetTransform() {
            return this.tileViewModel.GetTargetTransform();
        }

        public void ShowTileTroop(string id, Vector2 coordinate) {
            this.followMarchEvent.RemoveAllListeners();
            this.MoveWithEvent(coordinate, () => {
                if (!this.model.IsInPlayerBlock(coordinate)) {
                    FteManager.StopFte();
                } else {
                    this.OnTileClick(coordinate, true, id);
                }
            });
        }

        public void ShowMonsterDetail(GetMonsterByCoordAck monsterInfoAck) {
            this.tileViewModel.ShowMonsterDetail(monsterInfoAck);
        }

        public void RemoveMonsterOnTile(Vector2 coordinate) {
            this.RemoveMonsterInfo(coordinate);
            this.view.RefreshTileMonsterInfo(coordinate);
        }

        public void ShowTileBindUI(Vector2 coordinate) {
            this.view.ShowTileBindUI(coordinate);
        }

        public void OnTileClick(Vector2 coordinate, bool onlyShowTroop = false,
            string id = "", bool delayClick = false) {
            if (!this.model.IsCoordinateInBlocks(coordinate)) {
                Debug.LogError("coordinate is not in blocks " + coordinate);
                return;
            }
            if (this.model.IsInPlayerBlock(coordinate)) {
                this.TileClicked(coordinate, onlyShowTroop, id);
            } else {
                this.tileViewModel.Hide();
                if (delayClick) {
                    this.AddFeedblockCallback(coordinate, () => OnTileClick(coordinate));
                } else {
                    UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.tips_map_data_requesting),
                        TipType.Info
                    );
                }
            }
        }

        private void TileClicked(Vector2 coordinate, bool onlyShowTroop = false, string id = "") {
            if (this.viewCount != 0) {
                Debug.LogError("viewCount is not 0 " + this.viewCount);
                FteManager.StopFte();
                return;
            }
            this.LoseFocus();
            this.view.DisableChosenEffect();
            coordinate.x = Mathf.RoundToInt(coordinate.x);
            coordinate.y = Mathf.RoundToInt(coordinate.y);
            if (this.tileViewModel.IsVisible) {
                this.ShowHUD();
                this.HideTileInfo();
            } else {
                currentCoord = coordinate;
                MapTileInfo tileInfo = this.GetTileInfo(coordinate);
                this.SetTileClickAudio(tileInfo);
                this.view.HideTileBindUI(coordinate);
                this.tileViewModel.Show();
                this.tileViewModel.SetTileInfo(tileInfo, onlyShowTroop, id);
            }

        }

        public void ShowDemonTileView() {
            this.activityMapTileViewModel.ShowDemon(currentCoord);
        }

        public void ShowMonsterTileView() {
            this.activityMapTileViewModel.ShowMonster(currentCoord);
        }

        public void HideActivityTileView() {
            this.activityMapTileViewModel.Hide();
        }

        private void SetTileClickAudio(MapTileInfo tileInfo) {
            try {
                switch (tileInfo.type) {
                    case "building":
                        if (EventManager.GetBuildEventByCoordinate(tileInfo.coordinate) != null) {
                            AudioManager.Play(
                                AudioPath.showPrefix + "build",
                                AudioType.Show,
                                AudioVolumn.High
                            );
                        } else {
                            AudioManager.Play(
                                AudioPath.actPrefix + "click_building",
                                AudioType.Action,
                                AudioVolumn.High
                            );
                        }
                        break;
                    case "npc_city":
                        if (tileInfo.city.race.CustomEquals("dragon")) {
                            AudioManager.Play(
                                AudioPath.actPrefix + "click_dragon",
                                AudioType.Action,
                                AudioVolumn.High
                            );
                        } else {
                            AudioManager.Play(
                                AudioPath.actPrefix + "click_city",
                                AudioType.Action,
                                AudioVolumn.High
                            );
                        }
                        break;
                    default:
                        AudioManager.Play(
                            AudioPath.actPrefix + "click_tile",
                            AudioType.Action,
                            AudioVolumn.High
                        );
                        break;
                }
            } catch {
                ;
            }
        }

        #region rightHUDViewModel
        public void BuildingLevelUpHandler(string buildingName) {
            this.rightViewModel.BuildingLevelUpHandler(buildingName);
            this.view.ShowBuldingCompleteEffect(this.buildModel.buildingDict[buildingName].Coord);
            SdkManager.AchievedLevel(buildingName);
        }

        public void HeroRecruitDoneHandler(string troopName) {
            this.rightViewModel.HeroRecruitDoneHandler(troopName);
        }
        #endregion

        public void HideTileInfo() {
            if (this.tileViewModel.IsVisible) {
                this.IsTileInfoShow = true;
            }
            this.LoseFocus();
            this.tileViewModel.Hide();
            this.activityMapTileViewModel.Hide();
        }

        public void SetTileInfoStatus() {
            this.IsTileInfoShow = this.tileViewModel.IsVisible;
        }

        #region queueViewModel
        public void ShowQueueView() {
            this.queueViewModel.Show();
        }

        public void HideQueueView() {
            this.queueViewModel.Hide();
        }

        public void RefreshQueueItemAnimation(Troop troop) {
            this.queueViewModel.RefreshQueueItemAnimation(troop);
        }

        public void RefreshArmyCampTroopStatus(string armyCampName) {
            this.queueViewModel.RefreshArmyCampTroopStatus(armyCampName);
        }
        #endregion

        public void DisableClick() {
            this.view.IsClickable = false;
        }

        public void EnableClick() {
            this.view.IsClickable = true;
        }

        public void ShowHero(HeroSubViewType viewType = HeroSubViewType.None) {
            this.heroViewModel.ViewType = viewType;
            //this.heroViewModel.BuildingId = null;
            this.heroViewModel.Show();
        }

        public void ShowNewHero(LotteryResult lotteryResult,
                string groupName, UnityAction callback, bool isForceFte) {
            this.heroViewModel.ShowNewHero(lotteryResult, groupName, callback, isForceFte);
        }

        public void ReadHeroReq(string heroName) {
            this.heroViewModel.ReadHeroReq(heroName);
        }

        public void ShowHeroPool(string groupName, string building = null) {
            if (this.heroPoolViewModel == null) {
                this.heroPoolViewModel =
                    PoolManager.GetObject<HeroPoolViewModel>(this.transform);
            }
            this.heroPoolViewModel.Show(groupName, building);
        }

        #region heroInfoViewModel 
        public void ShowHeroTierUp() {
            this.heroInfoViewModel.ShowHeroTierUp();
        }

        public void ShowHeroInfo(
            string heroName,
            UnityAction levelUpCallback = null,
            HeroInfoType infoType = HeroInfoType.Self,
            bool isSubWindow = false
        ) {
            this.heroInfoViewModel.Show(heroName, infoType, levelUpCallback, isSubWindow);
        }

        public void ShowHeroInfo(Hero hero, HeroInfoType infoType = HeroInfoType.Others,
            bool isSubWindow = false) {
            this.heroInfoViewModel.Show(hero, infoType, isSubWindow: isSubWindow);
        }

        public void HideHeroInfo() {
            this.heroInfoViewModel.Hide();
        }
        #endregion

        public void RefreshHeroView(string heroName) {
            this.heroViewModel.RefreshHeroView(heroName);
        }

        public void RefreshLotteryView() {
            this.heroViewModel.RefreshLotteryView();
        }

        public void ShowLottery() {
            this.heroViewModel.ViewType = HeroSubViewType.Lottery;
            this.heroViewModel.Show();
        }

        public void ShowOpenChestView(List<LotteryResult> result, UnityAction callback) {
            this.heroViewModel.ShowOpenChestView(result, callback);
        }

        public void ShowRecruit(string id, bool showTips = false) {
            if (!this.troopModel.troopDict.ContainsKey(id)) {
                Debug.LogWarningf("No such troop with id {0}", id);
                return;
            }
            Troop troop = this.troopModel.troopDict[id];
            if (troop.Positions.Count < 1) {
                UIManager.ShowTip(string.Format(
                    LocalManager.GetValue(LocalHashConst.recruit_troop_empty),
                    TroopModel.GetTroopName(troop.ArmyCamp)), TipType.Notice);
            } else {
                this.recruitViewModel.Troop = troop;
                this.recruitViewModel.Show(showTips);
            }
        }

        public void ShowCampaignBossInfo(BossTroop dominaInfo) {
            this.bossInfoViewModel.Show(dominaInfo);
        }

        public void ShowCampaignCitySelect() {
            this.campaignCitySelectViewModel.Show();
        }

        public void ShowDemonShadowHistoryRank(DominationHistory record) {
            this.demonShadowHistoryRankViewModel.Show(record);
        }

        public void ShowMail() {
            this.mailViewModel.Show();
        }

        public void PlayBattleReport(string reportId, UnityAction endCallback) {
            this.mailViewModel.PlayBattlReport(reportId, endCallback);
        }

        public void ShowRank() {
            if (this.rankViewModel == null) {
                this.rankViewModel =
                    PoolManager.GetObject<RankViewModel>(this.transform);
            }
            this.rankViewModel.Show();
        }
        
        public void ShowHouseKeeper(int tabIndex = 0,bool isSetHighlightFrame = false ) {
            this.houseKeeperViewModle.Show(index: tabIndex, isSetHighlightFrame: isSetHighlightFrame);
        }

        public void ShowPay() {
            this.payViewModel.Show();
        }

        public Transform GetViewPrefPoint() {
            return this.view.GetViewPrefPoint();
        }

        public void ShowSelectServer() {
            this.selectServerViewModel.Show();
        }

        public void ShowUnlockBuild(UnityAction callback, bool needTip = false) {
            if (this.buildUnlockViewModel == null) {
                this.buildUnlockViewModel =
                    PoolManager.GetObject<BuildUnlockViewModel>(this.transform);
            }
            this.buildUnlockViewModel.Show(callback, needTip);
        }

        public void HideMail() {

        }

        public void ShowTribute() {
            if (this.tributeViewModel == null) {
                this.tributeViewModel =
                    PoolManager.GetObject<TributeViewModel>(this.transform);
            }
            this.tributeViewModel.ShowWithReq();
        }

        public void HideTributeObj() {
            this.view.HideTributeObj();
        }
        #region Alliance Operation
        public void NoticeAllianceStatusChange() {
            this.view.NoticeAllianceStatusChange();
        }

        public void ShowAllianceInfo(string allianceId = "") {
            this.allianceViewModel.ShowAllianceInfo(allianceId);
        }

        public void AddAllianceMark(string name, Vector2 coordinate) {
            this.allianceViewModel.AddAllianceMark(name, coordinate);
        }

        public void DeleteAllianceMark(Coord coordinate) {
            this.allianceViewModel.DeleteAllianceMarkReq(coordinate);
        }

        public void ApplyJoinAlliance(string message) {
            this.allianceViewModel.ApplyJoinAlliance(message);
        }

        public void ShowSubWindowByType(AllianceSubWindowType type) {
            this.allianceViewModel.ShowSubWindowByType(type);
        }
        #endregion

        public void ShowFallen() {
            this.fallenInfoViewModel.Show();
        }

        public void ShowLyVoice() {
            LYVoiceSdk.ShowLyvoiceLiveView();
        }

        public void ShowNoviceState() {
            this.noviceStateViewModel.Show();
        }
        #region FisrtDown Reward

        public void ShowFirstDownReward() {
            if (this.NeedShowFirstDownReward) {
                this.CloseAboveUI();
                this.fieldFirstDownViewModel.ShowFirstDownReward();
                this.NeedShowFirstDownReward = false;
            }
        }

        #endregion

        #region Game Setting logic
        public void ShowGameSettingPanel() {
            if (this.gameSettingViewModel == null) {
                this.gameSettingViewModel =
                    PoolManager.GetObject<GameSettingViewModel>(this.transform);
            }
            this.gameSettingViewModel.Show();
        }
        #endregion

        #region Campaig logic

        public void UpdateLoginRewardReq() {
            this.campaignViewModel.GetLoginRewardReq();
        }

        public void ShowDailyRewardReceivedView(
            LoginRewardConf rewardConf) {
            this.dailyRewardReceivedViewModel.Show(rewardConf);
        }

        public void ShowDailyRewardNoRewardView(
            LoginRewardConf rewardConf) {
            this.dailyRewardNoRewardViewModel.Show(rewardConf);
        }

        public void ShowCampaignsPanel() {
            this.campaignViewModel.Show(this.view.GetThisActivity());
        }
        
        public void SetOpenServiceActivityHUD(int count, bool isShow, bool showNotice) {
            this.topViewModel.SetOpenServiceActivityHUD(count, isShow, showNotice);
        }
        
        public void ShowCampaignRewards() {
            if (this.campainRewardViewModel == null) {
                this.campainRewardViewModel =
                    PoolManager.GetObject<CampaignRewardViewModel>(this.transform);
            }
            this.campainRewardViewModel.Show();
        }
        public void ShowCampaignRules(string content) {
            if (this.campaignRuleViewModel == null) {
                this.campaignRuleViewModel =
                    PoolManager.GetObject<CampaignRuleViewModel>(this.transform);
            }
            this.campaignRuleViewModel.Show(content);
        }

        #endregion

        //public void ChangeStoreEntrance() {
        //    this.view.ChangeStoreEntrance();
        //}

        #region Drama & Daily Task
        public void OnDailyTaskRefresh(int count) {
            dailyTaskCollectableCount = count;
            this.view.UpdateTaskCollectableCount(
                dramaTaskCollectableCount + dailyTaskCollectableCount);
        }

        public void OnDramaTaskRefresh(int count) {
            dramaTaskCollectableCount = count;
            this.view.UpdateTaskCollectableCount(
                dramaTaskCollectableCount + dailyTaskCollectableCount);
        }

        public void GetTileCoordByLevel(int tileLevel = 0, string tileType = "") {
            int tileFlag = 0;
            //Debug.LogError("GetTileCoordByLevel " + tileLevel + " " + tileType);
            if (!tileType.CustomIsEmpty()) {
                ElementType type = (ElementType)Enum.Parse(typeof(ElementType), tileType.ToLower());
                tileFlag = MapBasicTypeConf.GetMapBasicTypeFlag(((int)type).ToString());
            }
            GetRecentCoordByLevelReq req = new GetRecentCoordByLevelReq() {
                Level = tileLevel,
                Type = tileFlag
            };
            NetManager.SendMessage(req, typeof(GetRecentCoordByLevelAck).Name,
                this.GetTileCoordByLevelAck);
        }

        public void GetRecentMonsterByLevel(int monsterLevel) {
            this.monsterLevelToGet = monsterLevel;
            GetRecentMonsterByLevelReq getMonsterReq = new GetRecentMonsterByLevelReq() {
                Level = monsterLevel
            };

            NetManager.SendMessage(
                getMonsterReq,
                typeof(GetRecentMonsterByLevelAck).Name,
                this.OnGetRecentMonsterByLevelAck,
                this.OnGetRecentMonsterError);
        }

        private int monsterLevelToGet = -1;
        private void OnGetRecentMonsterError(IExtensible message) {
            GetMonsterRefreshAtReq monsterCreatTimeReq = new GetMonsterRefreshAtReq();
            GameManager.IsNeedShowErrorAckMsg = false;
            NetManager.SendMessage(monsterCreatTimeReq,
                typeof(GetMonsterRefreshAtAck).Name, this.OnGetMonsterCreateTime);
        }

        private void OnGetMonsterCreateTime(IExtensible message) {
            GetMonsterRefreshAtAck creatTime = message as GetMonsterRefreshAtAck;
            string tips =
               this.campaignViewModel.IsDevilFightingFinish(creatTime.RefreshAt) ?
               string.Format(
                   LocalManager.GetValue(LocalHashConst.server_monster_not_found_finish),
                   this.monsterLevelToGet,
                   CampaignModel.MonsterLocalName
                ) :
                string.Format(
                    LocalManager.GetValue(LocalHashConst.server_monster_not_found),
                    this.monsterLevelToGet,
                    GameHelper.TimeFormat(creatTime.RefreshAt * 1000 - RoleManager.GetCurrentUtcTime()),
                    CampaignModel.MonsterLocalName
                );
            UIManager.ShowTip(tips, TipType.Error, waitTime: 3f);
            GameManager.IsNeedShowErrorAckMsg = true;
        }


        private void GetTileCoordByLevelAck(IExtensible message) {
            GetRecentCoordByLevelAck ack = message as GetRecentCoordByLevelAck;
            //Debug.LogError("GetTileCoordByLevelAck " + ack.Coord);
            this.MoveWithClick(ack.Coord);
        }

        private void OnGetRecentMonsterByLevelAck(IExtensible message) {
            GetRecentMonsterByLevelAck getMonsterAck = message as GetRecentMonsterByLevelAck;
            this.MoveWithClick(getMonsterAck.Coord);
        }

        public void SetTaskDetail(int taskId, TaskType type, string taskDetail, bool receiveable, UnityAction jumpaction) {
            this.view.SetTaskDetail(taskId, type, taskDetail, receiveable, jumpaction);
        }

        public void StartChapterDailyGuid() {
            this.missionViewModel.StartChapterDailyGuid();
        }
        #endregion

        //public void GetChestRefresh() {
        //    this.heroViewModel.GetChestRefresh();
        //}

        //public void SetTaskCollectableCount(int count) {
        //    this.view.UpdateTaskCollectableCount(count);
        //}

        // BuildViewModel
        public void ShowBuildList(bool isFixed = false, Vector2 coordinate = default(Vector2)) {
            this.houseKeeperViewModle.ShowBuildList(isFixed, coordinate);
        }

        public void SetBuildingArrow(string buildingName) {
            this.houseKeeperViewModle.SetBuildingArrow(buildingName);
        }

        public void RefreshHouseKeeperEvent() {
            this.houseKeeperViewModle.RefreshHouseKeeperEvent();
        }

        public void ShowHouseKeeperBtn() {
            if (!this.view.isHouseKeeper) {
                this.view.ShowBottomHousekeeperBtn(true);
            }
        }

        public void ShowBuildInfo(BuildViewType buildViewType, string building, UnityAction callback) {
            this.buildInfoViewModel.BuildViewType = buildViewType;
            this.buildInfoViewModel.BuildingAndLevel = building;
            this.buildInfoViewModel.Show(callback);
        }

        public int GetCanBeBuiltBuildingCount() {
            return this.buildInfoViewModel.GetCanBeBuiltBuildingCount();
        }

        // End

        public void ShowChat() {
            this.chatRoomViewModel.Show();
        }

        public void ShowAllianceChatroom() {
            this.chatRoomViewModel.ShowAllianceChatroom();
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
                                           ButtonClickWithLabel greenBtnInfo,
                                           ButtonClickWithLabel redBtnInfo,
                                           bool isForMemberInfo = false) {
            if (this.playerAllianceInfo == null) {
                this.playerAllianceInfo =
                    PoolManager.GetObject<PlayerAllianceInfoViewModel>(this.transform);
            }
            this.playerAllianceInfo.ShowAllianceMemOperate(
                playerInfo, greenBtnInfo, redBtnInfo, isForMemberInfo);
        }

        public void ShowPlayerDetailInfo(string playerId) {
            string userId = RoleManager.GetRoleId();
            if (playerId.CustomEquals(userId)) {
                this.ShowRolePlayerInfo();
            } else {
                GetPlayerPublicInfoReq getPlayerInfo = new GetPlayerPublicInfoReq() {
                    Id = playerId
                };
                NetManager.SendMessage(getPlayerInfo,
                                       typeof(GetPlayerPublicInfoAck).Name,
                                       this.GetPlayerPublickInfoAck);
            }
        }

        public void SendMessageTo(string userName, string userId, bool isSubWindow = false) {
            this.chatRoomViewModel.SendMessageTo(userName, userId);
        }

        public void SetMapChatAllianceInfo() {
            if (this.chatRoomModel.allianceMessageList.Count > 0) {
                AllianceMessage allianceMessage = null;
                int allianceMessageLength = this.chatRoomModel.allianceMessageList.Count;
                while (allianceMessageLength > 1) {
                    allianceMessage = this.chatRoomModel.allianceMessageList[
                    allianceMessageLength - 1];
                    if (allianceMessage.Chat != null &&
                        allianceMessage.Chat.Content.Length > 0) {
                        this.view.SetMapChatInfoView(
                                        LocalManager.GetValue(LocalHashConst.chat_alliance),
                                        allianceMessage.Chat.PlayerName,
                                        allianceMessage.Chat.Content);
                        return;
                    }
                    allianceMessageLength--;
                }
            }
        }

        public void SetMapChatWorldInfo() {
            if (this.chatRoomModel.worldMessageList.Count > 0) {
                ChatMessage chatMessage = this.chatRoomModel.worldMessageList[
                    this.chatRoomModel.worldMessageList.Count - 1];
                if (chatMessage.Content.Length > 0) {
                    this.view.SetMapChatInfoView(
                                    LocalManager.GetValue(LocalHashConst.chat_world),
                                    chatMessage.PlayerName,
                                    chatMessage.Content);
                }
            }
        }

        public void SetMapChatStateInfo() {
            if (this.chatRoomModel.stateMessageList.Count > 0) {
                ChatMessage chatMessage = this.chatRoomModel.stateMessageList[
                    this.chatRoomModel.stateMessageList.Count - 1];
                if (chatMessage.Content.Length > 0) {
                    this.view.SetMapChatInfoView(
                                    LocalManager.GetValue(LocalHashConst.chat_state),
                                    chatMessage.PlayerName,
                                    chatMessage.Content);
                }
            }
        }

        public void ShowBanner(Protocol.CommonReward reward, Protocol.Resources resources
            , Protocol.Currency currency, bool needNext) {
            this.BannerViewModel.ShowBanner(reward, resources, currency, needNext);
        }

        public void ShowMission(int tabIndex) {
            this.missionViewModel.Show(tabIndex);
        }

        public void ShowCampaignRewardDomination() {
            this.campaignRewardDominationViewModel.Show();
        }
        public void GetDramaRewards(int taskId) {
            this.missionViewModel.GetDramaRewards(taskId);
        }

        public void CollectTaskReward(Protocol.Resources resources,
        Protocol.Currency currency, CommonReward commonReward) {
            this.view.CollectTaskReward(resources, currency, commonReward);
        }

        public void ShowDramaView() {
            this.CloseAboveUI();
            this.missionViewModel.ShowDramaView();
        }

        public void ShowMiniMap() {
            this.miniMapViewModel.Show();
        }

        public void ShowForceReward() {
            this.playerInfoViewModel.Show(1);
        }

        public void ShowSelfInfo() {
            this.playerInfoViewModel.Show(0);
        }

        public void ShowResource() {
            this.playerInfoViewModel.ShowResources();
        }

        public void ShowFieldReward() {
            this.fieldFirstDownViewModel.Show();
        }

        public void OnAddViewAboveMap(IViewModel baseViewModel,
            AddOnMap addOnMap = AddOnMap.HideAllWithoutTop) {
            this.viewCount++;
            this.view.DisableSlider();
            string type = baseViewModel.GetType().Name;
            if (this.viewCount > 0) {
                switch (addOnMap) {
                    case AddOnMap.Edit:
                        this.view.ShowBuildEditViewUI();
                        this.DisableClick();
                        this.view.IsEdit = true;
                        this.view.HideHUD();
                        break;
                    case AddOnMap.HideAll:
                        this.view.HideHUD();
                        break;
                    case AddOnMap.HideAllWithoutTop:
                        this.view.HideHUDWithoutTop();
                        break;
                }
            }
            if (!aboveUIDict.ContainsKey(type)) {
                this.aboveUIDict.Add(type, baseViewModel);
            } else {
                this.viewCount = 1;
            }
        }

        public void OnRemoveViewAboveMap(IViewModel baseViewModel) {
            string type = baseViewModel.GetType().Name;
            if (this.aboveUIDict.ContainsKey(type)) {
                int count = this.viewCount - 1;
                this.viewCount = count > 0 ? count : 0;
                if (this.viewCount == 0) {
                    if (type == "BuildEditViewModel") {
                        this.view.IsEdit = false;
                    }
                    this.ShowTopHUD();
                    this.ShowHUD();
                    this.EnableClick();
                    this.view.EnableSlider();
                }
                this.aboveUIDict.Remove(type);
            }
        }

        public void OnAnyOperate() {
            this.operateAction.InvokeSafe();
        }

        public void CloseAboveUI() {
            List<IViewModel> viewModelList = new List<IViewModel>();
            foreach (IViewModel baseViewModel in this.aboveUIDict.Values) {
                viewModelList.Add(baseViewModel);
            }
            foreach (IViewModel viewModel in viewModelList) {
                viewModel.HideImmediatly();
            }
            this.heroInfoViewModel.HideImmediatly();
            this.HideTileInfo();
            this.buildInfoViewModel.Hide();
        }

        public void ShowHUD() {
            this.view.ShowHUD();
        }

        public void HideHUD() {
            this.view.HideHUD();
        }


        public string GetBuildId() {
            return this.buildEditViewModel.Building;
        }

        public Monster GetMonsterInfo(Vector2 coordinate) {
            return this.model.GetMonsterInfo(coordinate);
        }

        private void RemoveMonsterInfo(Vector2 coordinate) {
            this.model.RemoveMonsterInfo(coordinate);
        }

        public Boss GetBossInfo(Vector2 coordinate) {
            return this.model.GetBossInfo(coordinate);
        }

        public ElementBuilding GetBuildByName(string buildName) {
            ElementBuilding building;
            return this.buildModel.buildingDict.TryGetValue(buildName, out building) ?
                building : null;
        }

        public void ShowBuildEditViewUI(bool isFixed, Vector2 fixedCoord) {
            this.buildEditViewModel.IsFixed = isFixed;
            this.buildEditViewModel.FixedCoord = fixedCoord;
            this.CloseAboveUI();
            this.buildEditViewModel.Show();
        }

        public void ShowGetTileTip() {
            this.getTileTipViewModel.Show();
        }

        public void ShowPayReward() {
            this.payRewardViewModel.Show();
        }

        public void SetIsFixed() {
            this.houseKeeperViewModle.SetIsFixd();
        }

        public void HideBuildEditViewUI() {
            this.buildEditViewModel.Hide();
        }

        public void RefreshMarkInTile(Vector2 coord, MapMarkType type, bool isAdd) {
            this.view.RefreshMarkInTile(coord, type, isAdd);
        }

        public void OnAllianceChange(string allianceId) {
            foreach (Vector2 coordinate in this.model.GetAlliancePoint(allianceId)) {
                this.view.RefreshTile(coordinate);
            }
        }

        public void SetPayBtn(bool canReward) {
            this.topViewModel.SetMonthCard(canReward);
        }

        #region private callback
        private void GetPlayerPublickInfoAck(IExtensible message) {
            GetPlayerPublicInfoAck playerInfoAck = message as GetPlayerPublicInfoAck;
            this.ShowAllianceMemOperate(playerInfoAck.Info, null, null, true);
        }

        private void OnCenterChanged() {
            this.view.OnCenterChanged();
            Vector2 cameraCoordinate = this.CenterCoordinate;
            if (!this.ResourceBlockRect.Contains(cameraCoordinate)) {
                this.ResourceBlock = new Vector2(
                    Mathf.Floor((this.CenterCoordinate.x - 1) / this.ResourceBlockSize.x),
                    Mathf.Floor((this.CenterCoordinate.y - 1) / this.ResourceBlockSize.y)
                );
            }
            if (!this.PlayerBlockRect.Contains(cameraCoordinate)) {
                this.PlayerBlock = new Vector2(
                    Mathf.Floor((this.CenterCoordinate.x - 1) / this.PlayerBlockSize.x),
                    Mathf.Floor((this.CenterCoordinate.y - 1) / this.PlayerBlockSize.y)
                );
            }

            foreach (EventMarchClient march in EventManager.EventDict[Event.March].Values) {
                if (!this.model.IsMarchInBlocks(march.origin, march.target)) {
                    this.view.DeleteFarAwayMarch(march.id);
                    if (march.playeId != RoleManager.GetRoleId() &&
                           !EventManager.FinishedList.Contains(march.id)) {
                        EventManager.FinishedList.Add(march.id);
                    }
                }
            }
        }

        // To do: feedblock req timeout
        private void FeedBlocksReq() {
            this.model.CaculatePlayerBlock();
            FeedBlocksReq feedBlocks = this.model.FeedBlocks();
            NetManager.SendMessage(feedBlocks, string.Empty, null);
        }

        public void InvokeMoveEvent() {
            this.moveEvent.InvokeSafe();
            this.moveEvent = null;
        }

        private void FeedBlocksNtf(IExtensible message) {
            FeedBlocksNtf feedBlockNtf = message as FeedBlocksNtf;
            this.model.RrefreshPlayerInfo(
                feedBlockNtf,
                this.feedblockEventDict,
                this.OnPlayerInfoChange
            );
        }

        private void FeedBlockMonsterNtf(IExtensible message) {
            FeedBlockMonsterNtf feedBlockMonsterNtf = message as FeedBlockMonsterNtf;

            this.model.RefreshMonsterInfo(feedBlockMonsterNtf,
                                          this.OnMonsterInfoChange);
        }

        private void FeedBlocksMarchesNtf(IExtensible message) {
            FeedBlocksMarchesNtf marchNtf = message as FeedBlocksMarchesNtf;
            foreach (EventMarch eventMarch in marchNtf.EventMarches) {
                this.EventMarchHandler(eventMarch, "new");
            }
        }

        private void FeedBlockDominationNtf(IExtensible message) {
            FeedBlockDominationNtf dominationNtf = message as FeedBlockDominationNtf;
            this.model.DominationInfo(dominationNtf, this.OnBossInfoChange);
        }

        private void ForceNtf(IExtensible message) {
            this.view.RefreshTile(RoleManager.GetRoleCoordinate());
        }

        public void GetTreasureMapRewardReq() {
            if (this.treasureViewModel == null) {
                this.treasureViewModel =
                    PoolManager.GetObject<TreasureMapViewModel>(this.transform);
            }
            this.treasureViewModel.GetTreasureMapReward();
        }

        private void RefreshBuildUpgradeableStatus() {
            foreach (ElementBuilding build in this.buildModel.buildingDict.Values) {
                this.view.RefreshUpgradeableInTile(build.Coord);
            }
        }

        //private void PlayerPointsReq() {
        //    PlayerPointsReq playerPointsReq = new PlayerPointsReq();
        //    NetManager.SendMessage(playerPointsReq,
        //        typeof(PlayerPointsAck).Namespace, this.PlayerPointsAck);
        //}

        //private void PlayerPointsAck(IExtensible message) {
        //    PlayerPointsAck playerPointsAck = message as PlayerPointsAck;
        //    RoleManager.SetPointDict(playerPointsAck.Points);
        //}

        private void PlayerPointNtf(IExtensible message) {
            PlayerPointNtf playerPointNtf = message as PlayerPointNtf;
            if (playerPointNtf.Method.CustomEquals("del")) {
                Vector2 coordinate =
                    new Vector2(playerPointNtf.Point.Coord.X, playerPointNtf.Point.Coord.Y);
                RoleManager.RemovePoint(coordinate);
            } else {
                RoleManager.RefreshPoint(playerPointNtf.Point);
                this.model.RefreshPoint(playerPointNtf.Point);
            }
        }

        public void SetQueueIsFold(bool isFold) {
            this.queueViewModel.SetIsFold(isFold);
        }

        private void CreateMarch(EventMarch march) {
            EventManager.AddMarchEvent(march);
            if (this.model.IsMarchInBlocks(march.Origin, march.Target)) {
                this.view.CreateMarch(EventManager.GetMarchById(march.Id));
            }
        }

        //public void Test() {
        //    int i = 0;
        //    foreach (string heroName in ConfigureManager.GetConfDict<HeroAttributeConf>().Keys) {
        //        i++;
        //        EventMarch march = new EventMarch();
        //        march.Id = i.ToString();
        //        march.Origin = new Coord();
        //        march.Origin.X = 1;
        //        march.Origin.Y = 1;
        //        march.Target = new Coord();
        //        march.Target.X = 999;
        //        march.Target.Y = 999;
        //        march.Troop = new Troop();
        //        march.Troop.Marched = true;
        //        HeroPosition position = new HeroPosition();
        //        march.StartAt = System.DateTime.Now.Ticks / 10000000;
        //        march.FinishAt = System.DateTime.Now.Ticks / 10000000 + 3600;
        //        position.Position = 1;
        //        position.Name = heroName;
        //        march.Troop.Positions.Add(position);
        //        this.CreateMarch(march);
        //        if (i > 100) {
        //            break;
        //        }
        //    }
        //}

        private void DeleteMarch(EventMarch march) {
            EventManager.FinishImmediate(march.Id);
            Vector2 origin = new Vector2(march.Origin.X, march.Origin.Y);
            Vector2 target = new Vector2(march.Target.X, march.Target.Y);

            if (this.model.IsMarchInBlocks(origin, target)) {
                this.view.DeleteMarch(march.Id);
            }
            if (this.tileViewModel.CurrentMarch != null &&
                this.tileViewModel.CurrentMarch.id == march.Id) {
                this.tileViewModel.HideMarch(march.Id);
            }
        }

        private void UpdateMarch(EventBase eventBase) {
            EventMarchClient march = eventBase as EventMarchClient;
            if (!this.view.IsMarchVisible(eventBase.id)) {
                return;
            }
            //if (!this.model.IsMarchInBlocks(march.origin, march.target)) {
            //    this.view.DeleteFarAwayMarch(march.id);
            //    if (march.playeId != RoleManager.GetRoleId() &&
            //           !EventManager.FinishedList.Contains(march.id)) {
            //        EventManager.FinishedList.Add(march.id);
            //    }
            //    return;
            //}
            this.view.UpdateMarch(eventBase);
        }
        #endregion

        public void UpdateTroopStatus(string troopId) {
            Troop troop = this.troopModel.GetTroopByTroopId(troopId);
            this.UpdateTroopStatus(troop);
        }

        public bool HasAvaliableTroop() {
            return this.troopModel.HasAvaliableTroop();
        }

        public void UpdateTroopStatus(Troop troop) {
            this.view.UpdateTileTroopStatus(troop);
            this.queueViewModel.UpdateQueueTroopStatus(troop);
            this.view.RefreshBuildCureVisible(this.troopModel.IsNeedShowBuildCure());
        }
        
        public DailyLimit GetDailyLimit() {
            return this.troopModel.dailyLimit;
        }

        #region topViewModel methods
        public void ShowTopHUD(bool needAnimation = false) {
            this.topViewModel.Show(needAnimation);
        }

        public void HideTopHUD() {
            this.topViewModel.HideImmediatly();
        }

        public void RefreshPlayerName() {
            this.topViewModel.RefreshPlayerName();
        }

        public void ForceUpgradeAnimation(UnityAction action) {
            this.topViewModel.ForceUpgradeAnimation(action);
        }

        public void SetChangeGem(int gem) {
            this.topViewModel.SetChangeGem(gem);
        }

        public void ChangeResource(CommonReward reward) {
            this.topViewModel.ChangeResource(reward);
        }

        public void RefreshResourceAndCurrency() {
            this.topViewModel.Refresh();
        }

        public void ShowRolePlayerInfo() {
            this.topViewModel.ShowPlayerInfo();
        }

        public void CollectResource(Resource type, int addAmount, Vector2 resourcePos,
                                    bool isPlayDroupOutAnimation, bool isCollect) {
            this.topViewModel.CollectResource(type, addAmount, resourcePos,
                isPlayDroupOutAnimation, isCollect);
        }

        public void CloseIconAnimation(bool isStart) {
            this.topViewModel.CloseIconAnimation(isStart);
        }
        #endregion

        public void ShowDisplayBoardViewModel(string title, string content) {
            this.displayBoardViewModel.Show(title, content);
        }


        public void ShowMarchInfo(GameObject marchObj) {
            if (this.CurrentMarch.Contains("Fte")) {
                return;
            }
            EventMarchClient queueMarch = EventManager.GetMarchById(this.CurrentMarch);
            if (this.viewCount == 0 && queueMarch != null) {
                this.tileViewModel.ShowMarch(queueMarch, marchObj);
            }
        }

        public void ShowTroopInfo(string id, TroopViewType mode, UnityAction afterHide = null) {
            this.troopInfoViewModel.ViewType = mode;
            this.troopInfoViewModel.CurrentTroop = id;
            if (mode == TroopViewType.Return) {
                this.troopInfoViewModel.Target = RoleManager.GetRoleCoordinate();
            } else {
                this.troopInfoViewModel.Target = this.CurrentTile.coordinate;
                Debug.Log(this.CurrentTile.coordinate);
            }
            this.tileViewModel.Hide(false);
            this.view.HideButtonPanel(false);
            this.view.HideQueuePanel(false);
            this.view.HideHUD();
            this.troopInfoViewModel.Show(afterHide);
        }

        public void ShowTroopFormation(string id) {
            this.troopFormationViewModel.CurrentTroop = id;
            this.troopFormationViewModel.Show();
        }

        public void HideTroop() {
            this.troopInfoViewModel.Hide();
        }

        public void ShowBattleResultTip(TroopArrivedNtf message) {
            if (this.reportTipViewModel == null) {
                this.reportTipViewModel =
                    PoolManager.GetObject<BattleReportTipViewModel>(this.transform);
            }

            this.reportTipViewModel.Show(
               message, this.mailViewModel.ShowFirstBattleReport);
        }

        public void ShowDefendBattleResultTip(TroopHasBeenAttackedNtf message) {
            if (this.reportTipViewModel == null) {
                this.reportTipViewModel =
                    PoolManager.GetObject<BattleReportTipViewModel>(this.transform);
            }

            this.reportTipViewModel.ShowDefend(
               message, this.mailViewModel.ShowFirstBattleReport);
        }

        public void Mark(string markName, Vector2 coordinate) {
            this.miniMapViewModel.Mark(markName, coordinate);
        }

        public void DeleteMark(Vector2 coordinate) {
            this.miniMapViewModel.DeleteMark(coordinate);
        }

        public void AddMarkOnTile(Vector2 coordinate) {
            this.view.DisableChosenEffect();
        }

        public void StopJumping() {
            this.view.IsJumping = false;
        }

        public void PlayInitAnimation() {
            this.view.PlayInitAnimation();
        }

        //public void BuildCancelReq(string id) {
        //    UIManager.ShowConfirm(
        //        LocalManager.GetValue(LocalHashConst.notice_title_warning),
        //        LocalManager.GetValue(LocalHashConst.warning_cancel_upgrade_content),
        //        () => {
        //            CancelBuildReq cancelBuildReq = new CancelBuildReq() {
        //                Id = id
        //            };
        //            NetManager.SendMessage(cancelBuildReq, string.Empty, null);
        //        },
        //        () => { }
        //    );
        //}

        private void UpdateQueueTroopStatus(Troop troop) {
            this.queueViewModel.UpdateQueueTroopStatus(troop);
        }

        private void EventMarchNtf(IExtensible message) {
            EventMarchNtf eventMarchNtf = message as EventMarchNtf;
            EventMarch eventMarch = eventMarchNtf.EventMarch;
            this.EventMarchHandler(eventMarch, eventMarchNtf.Method);
            //this.OnPropertyChanged("queue");
        }

        private void EventMarchHandler(EventMarch eventMarch, string method) {
            //Debug.Log(method);
            if (method.CustomEquals("new")) {
                if (eventMarch.PlayerId == RoleManager.GetRoleId()) {
                    Troop troop = this.troopModel.troopDict[eventMarch.Troop.Id];
                    this.troopModel.troopDict[eventMarch.Troop.Id] = eventMarch.Troop;
                }
                this.CreateMarch(eventMarch);
            } else if (method.CustomEquals("del")) {
                if (eventMarch.PlayerId.CustomEquals(RoleManager.GetRoleId())) {
                    this.troopModel.troopDict[eventMarch.Troop.Id] = eventMarch.Troop;
                }
                this.DeleteMarch(eventMarch);
                Vector2 target = new Vector2(eventMarch.Target.X, eventMarch.Target.Y);
                //Debug.Log("method.CustomEquals(/)");
                this.view.RefreshTile(target);
                this.UpdateTroopStatus(eventMarch.Troop);
            } else if (method.CustomEquals("update")) {
                this.EventMarchHandler(eventMarch, "del");
                this.EventMarchHandler(eventMarch, "new");
            }
        }

        private void AbandonPointAck(IExtensible message) {
            this.tileViewModel.HideTileInfo();
            AbandonPointAck abandonAck = message as AbandonPointAck;
            EventManager.AddAbandonEvent(abandonAck.EventAbandon);
            Vector2 coordinate = new Vector2(abandonAck.EventAbandon.Coord.X,
                abandonAck.EventAbandon.Coord.Y);
            this.view.RefreshTile(coordinate);
        }

        private void EventAbandonNtf(IExtensible message) {
            EventAbandonNtf ntf = message as EventAbandonNtf;
            if (ntf.Method.CustomEquals("del")) {
                EventManager.FinishedList.Add(ntf.EventAbandon.Id);
                Vector2 coordinate = new Vector2(
                    ntf.EventAbandon.Coord.X,
                    ntf.EventAbandon.Coord.Y
                );
                this.view.DeleteAbandon(coordinate);
            }
        }

        private void EventBuildNtf(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            this.tileViewModel.Hide();
            this.activityMapTileViewModel.Hide();
            EventBuild eventBuild = eventBuildNtf.EventBuild;
            Vector2 coordinate = Vector2.zero;
            if (eventBuildNtf.Method.CustomEquals("del")) {
                EventManager.FinishedList.Add(eventBuild.Id);
                if (this.view.isHouseKeeper) {
                    this.view.ShowBuildBtn();
                }
                coordinate = new Vector2(
                    eventBuild.Coord.X,
                    eventBuild.Coord.Y
                );
                this.view.DeleteBuild(coordinate);
            } else if (eventBuildNtf.Method.CustomEquals("new")) {
                coordinate = new Vector2(eventBuild.Coord.X,
                    eventBuild.Coord.Y);
                EventManager.AddBuildEvent(eventBuild);
                // if (EventManager.IsBuildEventFull()) {
                //     this.view.ShowBottomHousekeeperBtn(false);
                // }
            }
            this.view.RefreshTile(coordinate);
        }

        private void EventGiveUpNtf(IExtensible message) {
            EventGiveUpNtf eventGiveUpNtf = message as EventGiveUpNtf;
            this.tileViewModel.Hide();
            this.activityMapTileViewModel.Hide();
            EventGiveUp eventGiveUp = eventGiveUpNtf.EventGiveUp;
            Vector2 coordinate = Vector2.zero;
            if (eventGiveUpNtf.Method.CustomEquals("del")) {
                EventManager.FinishedList.Add(eventGiveUp.Id);
                coordinate = new Vector2(
                    eventGiveUp.Coord.X,
                    eventGiveUp.Coord.Y
                );
                this.view.DeleteBuild(coordinate);
            } else if (eventGiveUpNtf.Method.CustomEquals("new")) {
                coordinate = new Vector2(eventGiveUp.Coord.X,
                    eventGiveUp.Coord.Y);
                EventManager.AddGiveUpBuildingEvent(eventGiveUp);
            }
            this.view.RefreshTile(coordinate);
        }

        private void PlayerBuildingNtf(IExtensible message) {
            this.view.SetBtnBuild();
            //PlayerBuildingNtf ntf = message as PlayerBuildingNtf;
            //if (ntf.Method == "up" && ntf.Building.Type == (int)ElementType.armycamp &&
            //    ntf.Building.Level == 1) {
            //    this.view.RefreshTownHallTroop();
            //}
            TriggerManager.Invoke(Trigger.ResourceChange);
        }

        private void TroopArrivedNtf(IExtensible message) {
            TroopArrivedNtf ntf = message as TroopArrivedNtf;
            RoleManager.Instance.NeedResourceAnimation = true;
            RoleManager.Instance.NeedCurrencyAnimation = true;
            AudioManager.Play("act_troop_arrive", AudioType.Action, AudioVolumn.Medium);
            AudioManager.Play(string.Concat(AudioPath.showPrefix, "tip_normal"),
              AudioType.Show, AudioVolumn.High);
            if (ntf.HasBattle) {
                this.view.ShowBattleResult(ntf);
            } else {
                this.ShowBattleResultTip(ntf);
            }
        }

        private void TroopHasBeenAttackedNtf(IExtensible message) {
            TroopHasBeenAttackedNtf ntf = message as TroopHasBeenAttackedNtf;
            this.ShowDefendBattleResultTip(ntf);
        }

        private void FieldFirstDownNtf(IExtensible message) {
            this.view.ResetTileLevel();
            this.view.ResetSelfBuilding();
        }

        private void ChapterTaskChangeNtf(IExtensible message) {
            ChapterTaskChangeNtf ntf = message as ChapterTaskChangeNtf;
            if (ntf.IsDone && ntf.TaskId < 10) {
                if (FteManager.FteOver) {
                    this.view.SetDramaArrow();
                }
            }
            if (!ntf.IsDone && ntf.TaskId == 4) {
                this.view.SetDramaArrow();
            }
        }

        public void AllianceInfosNtf(IExtensible message) {
            AllianceInfosNtf ntf = message as AllianceInfosNtf;
            this.model.RefreshAllianceInfo(ntf);
        }

        private void OnRelationChangeNtf(IExtensible message) {
            RelationNtf relationNtf = message as RelationNtf;
            bool isPlayerFallened = !string.IsNullOrEmpty(relationNtf.MasterAllianceName);
            this.topViewModel.SetFallen(isPlayerFallened);
            AllianceCreateOrJoinModel createOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            createOrJoinModel.rejoinAllianceFinishAt = relationNtf.QuitTimestamp * 1000;
        }

        private void OnTroopNtf(IExtensible message) {
            TroopNtf troopNtf = message as TroopNtf;
            if (this.troopModel.troopDict.ContainsKey(troopNtf.Troop.Id)) {
                this.troopModel.troopDict[troopNtf.Troop.Id] = troopNtf.Troop;
            }
            Debug.LogError(this.troopModel.troopDict.Count);
            Vector2 origin = new Vector2(troopNtf.Src.X, troopNtf.Src.Y);
            Vector2 target = new Vector2(troopNtf.Dst.X, troopNtf.Dst.Y);
            this.view.RefreshTile(origin);
            this.view.RefreshTile(target);
        }

        private void NewTroopNtf(IExtensible message) {
            NewTroopNtf ntf = message as NewTroopNtf;
            Vector2 coordinate = new Vector2(ntf.Troop.Coord.X, ntf.Troop.Coord.Y);
            this.view.RefreshTile(coordinate);
        }

        private void OnHeroArmyAmountChange() {
            this.view.RefreshBuildCureVisible(this.troopModel.IsNeedShowBuildCure());
        }

        private void OnVoiceLiveStatusChange(string liveStatus) {
            if (liveStatus.CustomEquals("1") ||
                liveStatus.CustomEquals("2")) {
                AudioManager.OnVoiceLiveStatusChange(true);
            } else if (liveStatus.CustomEquals("3") ||
                       liveStatus.CustomEquals("4")) {
                AudioManager.OnVoiceLiveStatusChange(false);
            } else {
                Debug.LogError("Unknown status");
            }
        }

        private void NotifiesReq() {
            NotifiesReq notifiesReq = new NotifiesReq();
            NetManager.SendMessage(notifiesReq, string.Empty, null);
        }

        private void ChatRoomReq() {
            this.chatRoomViewModel.ChatRoomReq();
        }

        public void ChatAllianceMessagesReq() {
            this.chatRoomViewModel.ChatAllianceMessagesReq();
        }

        public void SetBtnMail(int count) {
            this.view.SetBtnMail(count);
        }

        public void SetBtnBuild() {
            this.view.SetBtnBuild();
        }

        public void SetBtnGacha() {
            this.view.SetBtnGacha();
        }

        public void SetBtnPayReward(int index) {
            this.view.SetBtnPayReward(index);
        }

        public void UpdateAbandon(EventBase eventBase) {
            this.view.UpdateAbandon(eventBase);
        }

        public void UpdateGiveUpBuilding(EventBase eventBase) {
            this.view.UpdateGiveUpBuilding(eventBase);
        }

        public void UpdateBuildEvent(EventBase eventBase) {
            this.view.UpdateBuild(eventBase);
        }

        public void UpdateShield(EventBase eventBase) {
            this.view.UpdateShield(eventBase);
        }

        private void OnPlayerInfoChange(Point point, bool needCheckSight, int sightRadius) {
            Vector2 coordinate = new Vector2(point.Coord.X, point.Coord.Y);
            this.view.RefreshTile(coordinate);
            if (this.tileViewModel.IsVisible && this.tileViewModel.TileInfo != null &&
                coordinate == this.tileViewModel.TileInfo.coordinate) {
                this.tileViewModel.SetTileInfo(this.GetTileInfo(coordinate));
            }
            if (needCheckSight) {
                foreach (Vector2 coord in this.model.GetCoordinateList(coordinate, sightRadius, false)) {
                    Point coordPoint = this.model.GetPlayerPoint(coord);
                    if (coordPoint != null &&
                        (!coordPoint.PlayerId.CustomEquals(RoleManager.GetRoleId()) ||
                        !coordPoint.AllianceId.CustomIsEmpty() ||
                        !coordPoint.AllianceId.CustomEquals(RoleManager.GetAllianceId()))) {
                        coordPoint.isCaculate = false;
                        //Debug.LogError("OnPlayerInfoChange 2");
                        this.view.RefreshTile(coord);
                    }
                }
            }
        }

        private void OnMonsterInfoChange(Vector2 coordinate) {
            this.view.RefreshTileMonsterInfo(coordinate);
        }

        private void OnBossInfoChange(Vector2 coordinate) {
            this.view.RefreshTileDominationInfo(coordinate);
        }

        protected override void OnReLogin() {
            this.InitTime();
            this.NotifiesReq();
        }


        #region FTE

        public void SetDramaArrow() {
            this.view.SetDramaArrow();
        }

        public bool HasDramaArrow() {
            return this.view.HasDramaArrow();
        }

        public void SetInitArrow() {
            if (this.dramaModel.GetChapterUnDoneTaskID() > 3 && this.dramaModel.GetChapterUnDoneTaskID() < 10) {
                Debug.Log("SetDramaArrow");
                this.SetDramaArrow();
            }
        }

        public void FteCreateMarch(EventMarch march) {
            this.CreateMarch(march);
            this.view.SetFteMarchLine(march.Id);
        }

        private void OnFteStep21Start(string index) {
            this.view.InitCamera();
            AudioManager.StopBg();
            StartCoroutine(this.OnFteStep21Start());
        }

        IEnumerator OnFteStep21Start() {
            Vector2 origin = Vector2.zero;
            foreach (Vector2 coordinate in RoleManager.GetPointDict().Keys) {
                if (coordinate != RoleManager.GetRoleCoordinate()) {
                    origin = coordinate;
                    break;
                }
            }
            yield return YieldManager.GetWaitForSeconds(0f);
            EventMarch eventMarch = new EventMarch();
            eventMarch.StartAt = RoleManager.GetCurrentUtcTime() / 1000 + 2;
            eventMarch.FinishAt = eventMarch.StartAt + 5;
            eventMarch.Origin = new Coord();
            eventMarch.Target = new Coord();
            eventMarch.Troop = new Troop {
                Marched = true,
                Id = "Fte3",
            };
            eventMarch.Troop.Positions.Add(
                new HeroPosition {
                    Name = "hero_1",
                    Position = 1
                }
            );
            eventMarch.PlayerId = RoleManager.GetRoleId();
            eventMarch.Id = "Fte3";
            eventMarch.Origin.X = Mathf.RoundToInt(origin.x);
            eventMarch.Origin.Y = Mathf.RoundToInt(origin.y);

            eventMarch.Target.X = Mathf.RoundToInt(RoleManager.GetRoleCoordinate().x);
            eventMarch.Target.Y = Mathf.RoundToInt(RoleManager.GetRoleCoordinate().y);

            FteManager.ShowBanner(() => {
                this.FteCreateMarch(eventMarch);
                StartCoroutine(this.OnFteStep21End(eventMarch));
            });
        }

        IEnumerator OnFteStep21End(EventMarch eventMarch) {
            yield return YieldManager.GetWaitForSeconds(6f);
            this.DeleteMarch(eventMarch);
            this.view.AddFteTroop(RoleManager.GetRoleCoordinate());
            FteManager.HideBanner(() => FteManager.EndFte(true));
        }

        private void OnFteStep61Start(string index) {
            this.view.canCreatRelation = true;
            Debug.Log(FteManager.step57);
            this.view.FteFocus(FteManager.step57);
            GameObject tile;
            if (this.view.tileDict.TryGetValue(FteManager.step57, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                tileView.CreateRelationFte();
            }
        }

        private void OnFteStep61End() {
            this.LoseFocus();
        }

        private void OnFteStep81Start(string index) {
            this.view.OnFteStep81Start();
        }

        private void OnFteStep81End() {
            this.ShowHUD();
        }

        private void OnFteStep51Start(string index) {
            AudioManager.StopBg();
            this.BannerViewModel.ShowNextChapter();
            this.view.isInStep61 = true;
        }

        private void OnFteStep57Start(string index) {
            string x = this.CurrentTile.coordinate.x.ToString();
            string y = this.CurrentTile.coordinate.y.ToString();
            string step57 = string.Concat(x, "_", y);
            FteManager.step57 = this.CurrentTile.coordinate;
            PlayerPrefs.SetString(RoleManager.GetRoleId() + "step57", step57);
        }

        private void OnFteStep59Start(string index) {
            this.view.canCreatRelation = false;
        }

        private void OnFteStep101Start(string index) {
            this.view.OnFteStep101Start();
        }

        private void OnFteStep101End() {
            this.heroViewModel.ViewType = HeroSubViewType.Lottery;
            this.heroViewModel.Show();
        }

        private void OnFteStep111Start(string index) {
            GameObject tile;
            if (this.view.tileDict.TryGetValue(FteManager.step57, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                tileView.DeleteRelationFte();
            }
        }

        private void OnFteStep140Start(string index) {
            if (FteManager.SkipFte) {
                FteManager.StopFte();
            }
        }

        private void OnFteStep140End() {
            FteManager.StartFte();
        }

        private void OnTroopStep2Start(string index) {
            DramaConf dramaConf = DramaConf.GetDramaByIndex(index);
            Troop troop = this.troopModel.GetTroopByArmyCampName(dramaConf.buildingName);
            FteManager.SetCurrentTroop(troop.Id, Vector2.zero, string.Empty);
            Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
            if (troop.Idle) {
                this.ShowTileTroop(troop.Id, coordinate);
            } else {
                FteManager.StopFte();
                this.StartChapterDailyGuid();
            }
        }

        private void OnFteTroopReturnAck(IExtensible message) {
            Vector2 coordinate = RoleManager.GetRoleCoordinate();
            foreach (Troop troop in this.troopModel.troopDict.Values) {
                troop.Coord.X = Mathf.RoundToInt(coordinate.x);
                troop.Coord.Y = Mathf.RoundToInt(coordinate.y);
            }
            this.view.RefreshTile(coordinate);
            this.ShowTileTroop(this.troopModel.GetTroopsAt(coordinate)[0].Id, coordinate);
        }

        private void OnResourceStep1Start(string index) {
            DramaConf dramaConf = DramaConf.GetDramaByIndex(index);
            //Vector2 coordinate = this.model.GetReachableTile(dramaConf.level);
            Debug.Log("dramaConf.level " + dramaConf.level + " dramaConf.resourceLevelType " + dramaConf.resourceLevelType);
            GetRecentCoordByLevelReq req = new GetRecentCoordByLevelReq() {
                Level = dramaConf.level,
                Type = dramaConf.resourceLevelType
            };
            NetManager.SendMessage(req, typeof(GetRecentCoordByLevelAck).Name,
                this.GetRecentCoordByLevelAck, (message) => FteManager.StopFte(), FteManager.StopFte);
        }

        private void GetRecentCoordByLevelAck(IExtensible message) {
            this.CloseAboveUI();
            GetRecentCoordByLevelAck ack = message as GetRecentCoordByLevelAck;
            Vector2 coordinate = new Vector2(ack.Coord.X, ack.Coord.Y);
            this.tileViewModel.OnResourceStep1Start();
            this.MoveWithClick(coordinate);
            FteManager.EndFte(true);
        }

        private void OnResourceStep2End() {
            this.tileViewModel.OnResourceStep1End();
        }

        private void OnBuildUpStep1Start(string index) {
            DramaConf dramaConf = DramaConf.GetDramaByIndex(index);
            ElementBuilding building;
            //Debug.LogError(dramaConf.buildingName);
            if (this.buildModel.buildingDict.TryGetValue(dramaConf.buildingName, out building)) {
                if (building.IsBroken) {
                    UIManager.ShowTip(LocalManager.GetValue(
                        LocalHashConst.battle_result_your_building_is_destroyed), TipType.Info);
                    FteManager.StopFte();
                    this.StartChapterDailyGuid();
                    return;
                } else {
                    this.MoveWithClick(building.Coord);
                    return;
                }
            }

            if (!this.OnBuildStepNotFindBuildingWithName(dramaConf.buildingName)) {
                FteManager.StopFte();
                this.StartChapterDailyGuid();
            }
        }

        private bool OnBuildStepNotFindBuildingWithName(string type) {
            ElementBuilding building = null;
            ElementType buildingType =
                (ElementType)Enum.Parse(typeof(ElementType), type);
            if (buildingType != ElementType.none) {
                List<ElementBuilding> buildingList = this.buildModel.GetBuildingByType(buildingType);
                int level = -1;
                foreach (ElementBuilding build in buildingList) {
                    if (build.Level > level) {
                        level = build.Level;
                        building = build;
                    }
                }
                if (level != -1) {
                    this.MoveWithClick(building.Coord);
                    return true;
                }
            }
            return false;
        }

        private void OnRecruitStep2Start(string index) {
            this.OnRecruitStepStart();
        }

        private void OnRecruitStepStart() {
            string troopId = FteManager.GetCurrentTroop();
            FteManager.SetCurrentTroop(string.Empty, Vector2.zero, string.Empty);
            Troop troop = this.troopModel.troopDict[troopId];
            EventMarchClient march = EventManager.GetMarchEventByTroopId(troopId);
            if (troop.Idle) {
                Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
                this.LoseFocus();
                this.ShowTileTroop(troopId, coordinate);
            } else if (march != null) {
                this.FollowMarch(march.id);
                FteManager.StopFte();
                this.StartChapterDailyGuid();
            }
        }

        private void OnBuildUpStep1End() {

        }

        private void FteSetTileAboveVisible(Vector2 coordinate, bool visible) {
            this.view.FteSetTileAboveVisible(coordinate, visible);
        }

        private void OnBuildStep1Start(string index) {
            DramaConf dramaConf = DramaConf.GetDramaByIndex(index);
            Vector2 coordinate = this.buildModel.GetBuildCoordinateByName(dramaConf.buildingName);
            if (coordinate == GameConst.LeftDown) {
                this.view.OnBuildStep1Start();
            } else {
                this.OnTileClick(coordinate);
            }
        }

        private void OnBuildStep1End() {
            this.view.OnBuildStep1End();
        }

        private void OnAllianceStep1Start(string index) {
            this.view.OnAllianceStep1Start();
        }

        private void OnAllianceStep1End() {
            this.view.OnAllianceStep1End();
            this.StartChapterDailyGuid();
        }

        private void OnChatStep1Start(string index) {
            this.view.OnChatStep1Start();
        }

        private void OnChatStep1End() {
            this.view.OnChatStep1End();
        }

        private void SetFteUI() {
            this.view.SetFteUI();
        }
        #endregion

        private void SetPlayerFallenInfo() {
            string masterAllianceName = RoleManager.GetMasterAllianceName();
            bool isPlayerFalled = !string.IsNullOrEmpty(masterAllianceName);
            this.topViewModel.SetFallen(isPlayerFalled);
        }
        // The threshold should be a range not a unique number.
        private void OnResourceBlockChanged() {
            if (this.serilizer != null) {
                StopCoroutine(this.serilizer);
            }
            this.serilizer = this.SerilizeBlock();
            // Change to lazy load.
            StartCoroutine(this.serilizer);
        }

        private IEnumerator SerilizeBlock() {
            List<Vector2> blockList = this.model.GetResourceBlockList(this.ResourceBlock);
            List<Vector2> removeList = new List<Vector2>();
            foreach (Vector2 key in this.TileDict.Keys) {
                if (!blockList.Contains(key)) {
                    removeList.Add(key);
                }
            }
            foreach (Vector2 block in blockList) {
                if (!this.TileDict.ContainsKey(block)) {
                    //yield return new WaitForEndOfFrame();
                    yield return YieldManager.EndOfFrame;
                    if (this.TileDict.ContainsKey(block)) {
                        continue;
                    }
                    Dictionary<uint, uint> blockDict = this.model.resourceInfo.CreateBlockDict(block);
                    if (blockDict != null) {
                        this.TileDict.Add(block, blockDict);
                    }
                }
            }
            yield return YieldManager.EndOfFrame;
            Dictionary<uint, uint> dict;
            foreach (Vector2 block in removeList) {
                dict = this.TileDict[block];
                this.TileDict.Remove(block);
                this.model.RemoveBlockDict(dict);
            }
        }

        public MapTileInfo GetTileInfo(Vector2 coordinate) {
            return this.model.GetTileInfo(coordinate);
        }

        public MapTileInfo GetTileInfoSeparate(Vector2 coordinate) {
            return this.model.GetTileInfoSeparate(coordinate);
        }

        public TileProtectType GetTileProtectType(Vector2 coordinate) {
            return this.model.GetTileProtectType(coordinate);
        }

        public int GetLevel(Vector2 coordinate) {
            return this.model.GetLevel(coordinate);
        }

        public string GetTileZone(Vector2 coordinate) {
            return this.model.GetTileZone(coordinate);
        }

        public bool CheckCoordinate(Vector2 coordinate) {
            return this.model.CheckCoordinate(coordinate);
        }

        public ElementType GetTileType(Vector2 coordinate) {
            return this.model.GetTileType(coordinate);
        }

        public Vector2 GetCityCenterCoord(Vector2 coordinate) {
            return this.model.GetCityCenterCoord(coordinate);
        }

        public void ChangeAvatar() {
            this.topViewModel.ChangeAvatar();
        }


        public void FullPnlStore() {
            this.view.FullPnlStore();
        }
    }
}
