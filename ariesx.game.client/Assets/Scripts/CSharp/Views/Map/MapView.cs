using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Protocol;
using TMPro;
using System.Collections;
using System.Text;

/// <summary>
/// Another way: use two axis mapping.
/// </summary>

namespace Poukoute {
    public enum CollectChestType {
        collectWithShow,
        normalCollect,
        collectWithDropOut
    }
    public class MapView : BaseView, IDragHandler,
        IPointerDownHandler, IPinchHandler, IBeginPinchHandler,
        IEndPinchHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler {

        private MapViewModel viewModel;
        private MapViewPreference viewPref;
        private BoxCollider2D boxCollider2D;
        private BoxCollider2D clickMaskCollider2D;
        private GameObject mapDarkMask;
        private GameObject tileRoot;
        private GameObject marchRoot;
        private TerrainBackgroundView terrainBackgroundView;
        //  private Transform 
        
        public static Vector2 tileCache = new Vector2(5, 9);
        [HideInInspector]
        public Vector2 offset = Vector2.zero;
        private Vector2 offsetCell = Vector2.zero;
        private Vector2 endDragOffset = Vector2.zero;
        private static Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        public int oddEven = 0;
        private GameObject aboveUICamera;
        private Camera renderCamera;
        private Camera battlePrerenderCamera;
        //private Camera fteCamera;

        private Vector2 viewSize = Vector2.zero;
        private Vector2 velocity = Vector2.zero;
        private Vector2 dragDelta = Vector2.zero;
        private Vector2 chestVector2 = new Vector2(-0.9f, 1.3f);
        private Vector3 target = Vector3.zero;
        private bool isDraggable = true;
        private bool isDragging = false;
        private bool isJumping = false;
        public bool IsJumping {
            get {
                return this.isJumping;
            }
            set {
                this.isJumping = value;
                if (!value) {
                    this.jumpFrame = 0;
                    this.followObj = null;
                    this.viewModel.moveEvent = null;
                    this.viewModel.followMarchEvent.RemoveAllListeners();
                }
            }
        }

        public GameObject followObj = null;
        private const float damping = 3f;
        private float targetLerp = 0f;
        private const float jumpLerp = 4;
        private float jumpFrame = 0;
        private float curMaxSpeed = 100;
        private const float maxSpeed = 100;
        private const float minSpeed = 1;

        //private bool isHouseKeeperShake = false;
        public bool isHouseKeeper = false;

        private Vector2 previouse = Vector2.zero;
        private Vector2 dragDeltaTmp = Vector2.zero;
        //private Vector2 current = Vector2.zero;
        private Vector2 oldDistance = Vector2.zero;
        private Vector2 originDistance = Vector2.zero;
        private bool isClickable = true;
        private float originSize = 0;
        private float targetSize = 0f;
        private float resizeLerp = 3;
        private bool isPinching = false;
        private bool isPinchable = false;
        private bool isResizing = false;
        private bool isShowMarchRole = true;
        private bool IsShowMarchRole {
            get {
                return isShowMarchRole;
            }
            set {
                if (this.isShowMarchRole != value) {
                    this.isShowMarchRole = value;
                    this.OnShowMarchRole();
                }
            }
        }
        private bool isFocus = false;
        private bool isNewMarchFocus = false;
        private bool isCameraInit = false;
        private bool isHighlight = false;
        private Vector2 elasticDistance = Vector2.zero;
        private const float elasticity = 4f;
        private bool isOverstep = false;
        private bool isShowBtnsPnl = false;
        public bool isInStep61 = false;
        private Protocol.CommonReward fteReward = new Protocol.CommonReward();
        private Protocol.Resources fteResources = new Protocol.Resources();
        private Protocol.Currency fteCurrency = new Currency();
        Dictionary<Resource, Transform> fteresourceDict
            = new Dictionary<Resource, Transform>();
        //private Camera mainCamera;

        private bool isEdit = false;
        private Vector2 homeCoord = Vector2.zero;
        public bool canCreatRelation = true;
        private readonly Vector3 imgShowBtnsOpt = new Vector3(-1, 1, 1);
        
        private BattleResultPreview battleResultPreview;
        private bool isShowTileReward = false;

        public bool IsEdit {
            get {
                return isEdit;
            }
            set {
                this.isEdit = value;
                this.OnEditStateChange();
            }
        }

        public Dictionary<Vector2, GameObject> tileDict = new Dictionary<Vector2, GameObject>();
        public const int MARCH_LINE_MAX_3D = 8;
        public const int MARCH_LINE_MAX_2D = 30;
        public const int MARCH_LINE_MAX = 60;
        public Dictionary<string, MarchLineView> selfMarchDict = new Dictionary<string, MarchLineView>();
        public Dictionary<string, MarchLineView> otherMarchDict = new Dictionary<string, MarchLineView>();
        public Dictionary<Vector2, GameObject> abandonDict = new Dictionary<Vector2, GameObject>();
        public Dictionary<Vector2, GameObject> giveUpDict = new Dictionary<Vector2, GameObject>();
        // To do: make march dict's value as gameobject.
        public Dictionary<string, GameObject> newMarchDict = new Dictionary<string, GameObject>();
        public Dictionary<Vector2, GameObject> buildDict = new Dictionary<Vector2, GameObject>();
        // Fte
        private readonly Dictionary<string, GameObject> fteUIDict = new Dictionary<string, GameObject>();

        private Vector2 preClick = Vector2.zero;
        private TaskType taskType = TaskType.drama;
        private bool isReceiveable = false;
        private int taskId = -1;
        private UnityAction taskJumpAction;
        public UnityAction afterChestCollect;
        public UnityAction afterBattleGetResouse;
        private string oldSN = string.Empty;

        public int velocityCount = 0;
        public Vector2 Velocity {
            get {
                return this.velocity;
            }
        }

        public GameObject March {
            get {
                return this.marchRoot;
            }
        }

        public bool IsClickable {
            get {
                return this.isClickable;
            }
            set {
                this.isClickable = value;
            }
        }

        public float pinchdamp = 1.5f;


        private GameObject uiDynamic;
        private GameObject uiSlider;
        // Slider
        private Transform pnlSlider;
        private CanvasGroup cgSlider;

        // Tribute
        //  private GameObject tributeObj;
        private Animator tributeAnim;

        // Home
        //private Transform pnlHome;
        private RectTransform homeRectTransform;
        //private CanvasGroup homeCanvasGroup;
        private GameObject objHome;
        private Button btnHome;
        private Transform pnlHomeText;

        public GameObject Tile {
            get {
                return this.tileRoot;
            }
        }

        public float CameraSize {
            get {
                return GameManager.MainCamera.orthographicSize;
            }
            set {
                this.renderCamera.orthographicSize =
                //  this.highlightCamera.orthographicSize =
                this.battlePrerenderCamera.orthographicSize =
                    GameManager.MainCamera.orthographicSize = value;
                float height = GameManager.MainCamera.orthographicSize * 2f;
                this.clickMaskCollider2D.size =
                this.mapDarkMask.transform.localScale =
                this.boxCollider2D.size =
                this.viewSize = new Vector2(GameManager.MainCamera.aspect * height, height);
                this.curMaxSpeed = value / this.viewModel.CameraInfo.maxSize * maxSpeed;
                if ((this.IsShowMarchRole &&
                        value <= this.viewModel.CameraInfo.hideMarchThreshold) ||
                    (!this.IsShowMarchRole &&
                        value >= this.viewModel.CameraInfo.showMarchThreshold)
                ) {
                    this.IsShowMarchRole = !this.IsShowMarchRole;
                }
            }
        }

        private bool hasFreeLottery = false;
        private bool HasFreeLottery {
            get {
                return this.hasFreeLottery;
            }
            set {
                this.hasFreeLottery = value;
                this.viewPref.btnHero.gameObject.SetActiveSafe(!value);
                this.viewPref.btnFreeChest.gameObject.SetActiveSafe(value);

                this.viewPref.pnlChestIcon.gameObject.SetActiveSafe(!value);
                if (value) {
                    if (!this.viewPref.pnlFreeChestContent.gameObject.activeSelf) {
                        AnimationManager.Animate(this.viewPref.pnlFreeChestWhole.gameObject,
                            "Show", start: this.viewPref.pnlChestIcon.GetComponent<RectTransform>().anchoredPosition,
                            target: this.viewPref.pnlFreeChestWhole.GetComponent<RectTransform>().anchoredPosition,
                            finishCallback: () => {
                                this.viewPref.pnlFreeChestContent.gameObject.SetActiveSafe(true);
                                AnimationManager.Animate(this.viewPref.pnlFreeChestContent.gameObject,
                                    "Show", loop: true, needRestart: true, isOffset: false);
                            }, space: PositionSpace.UI);
                    }
                } else {
                    this.viewPref.pnlFreeChestContent.gameObject.SetActiveSafe(false);
                    AnimationManager.Stop(this.viewPref.pnlFreeChestContent.gameObject);
                }
            }
        }

        // Callback
        public UnityAction hideHudCallback = null;

        void Awake() {
            //this.count = 0;
            //this.mainCamera = GameManager.MainCamera;
            TileView.isShowLevel = false;
            TileView.isShowSelfBuilding = false;
            TileView.tileWaterReference = 0;
            this.viewModel = this.GetComponent<MapViewModel>();
            this.tileRoot = GameObject.Find("Tile");
            this.marchRoot = GameObject.Find("MarchLine");
            this.terrainBackgroundView = GameObject.Find("Terrain").
                transform.Find("TerrainBackground").GetComponent<TerrainBackgroundView>();
            this.boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
            this.clickMaskCollider2D = PoolManager.GetObject(PrefabPath.mapClickMask, this.transform).
                GetComponent<BoxCollider2D>();
            this.clickMaskCollider2D.enabled = false;
            this.mapDarkMask = PoolManager.GetObject(PrefabPath.mapDarkMask, this.transform);
            this.mapDarkMask.transform.localPosition = -2 * Vector3.forward;
            this.DisableDarkMask();
            this.InitCamera();

            this.aboveUICamera = GameManager.MainCamera.transform.Find("AboveUICamera").gameObject;
            this.ui = UIManager.GetUI("UIMap");
            this.group = UIGroup.Normal;
            GameHelper.SetCanvasCamera(this.ui.GetComponent<Canvas>());
            this.InitBtnCallbackInfo();
            //this.viewPref.pnlCampaign.gameObject.SetActiveSafe(false);

            this.uiSlider = UIManager.GetUI("UIMapSlider");
            UIManager.ShowUI(this.uiSlider);
            this.pnlSlider = this.uiSlider.transform.Find("PnlSlider");
            this.cgSlider = this.pnlSlider.GetComponent<CanvasGroup>();
            EventManager.AddEventAction(Event.DurabilityRecover, this.UpdateDurabilityState);
            EventManager.AddEventAction(Event.DefenderRecover, this.UpdateDefenderState);
        }

        private void InitBtnCallbackInfo() {
            this.viewPref = this.ui.gameObject.GetComponent<MapViewPreference>();
            this.viewPref.btnHero.onClick.AddListener(this.OnBtnHeroClick);
            this.viewPref.btnFreeChest.onClick.AddListener(this.OnBtnHeroClick);
            //this.viewPref.btnTribute.onClick.AddListener(this.OnBtnTributeClick);
            this.viewPref.btnMiniMap.onClick.AddListener(this.OnBtnMiniMapClick);
            this.viewPref.btnLyVoice.onClick.AddListener(this.OnBtnLyVoiceClick);
            this.viewPref.btnFallen.onClick.AddListener(this.OnBtnFallenClick);
            this.viewPref.btnTask.onClick.AddListener(this.OnBtnMissionClick);
            this.viewPref.btnJump.onClick.AddListener(this.OnBtnJumpClick);
            this.viewPref.btnAlliance.onClick.AddListener(this.OnBtnAllianceClick);
            this.viewPref.btnRank.onClick.AddListener(this.OnBtnRankClick);
            this.viewPref.btnMail.onClick.AddListener(this.OnBtnMailClick);
            this.viewPref.btnChat.onClick.AddListener(this.OnBtnChatClick);
            this.viewPref.btnCampaign.onClick.AddListener(this.OnCampaignClick);
            this.viewPref.btnPay.onClick.AddListener(this.OnBtnPayClick);
            this.viewPref.btnStore.onClick.AddListener(this.OnBtnStoreClick);
            this.viewPref.btnPayReward.onClick.AddListener(this.OnBtnPayRewardClick);
            this.viewPref.btnHouseKeeper.onClick.AddListener(this.OnBtnHouseKeeperClick);
            this.viewPref.btnBuild.onClick.AddListener(this.OnBtnBuildClick);
            this.viewPref.btnShowBtnsPnl.onClick.AddListener(this.OnBtnShowBtnsPnlClick);
            this.viewPref.btnBottomHouseKeeper.onClick.AddListener(this.OnBtnHouseKeeperClick);
            this.viewPref.btnAutoBattle.onClick.AddListener(this.OnBtnAutoBattleClick);
            this.viewPref.btnSelectSever.onClick.AddListener(this.OnBtnSelectSeverClick);
        }

        private void Start() {
            this.OnCenterChanged();
            this.fteUIDict.Add(FteConst.Chapter, this.viewPref.pnlTask.gameObject);
            this.fteUIDict.Add(FteConst.Alliance, this.viewPref.btnAlliance.gameObject);
            this.fteUIDict.Add(FteConst.Rank, this.viewPref.btnRank.gameObject);
            this.fteUIDict.Add(FteConst.Mail, this.viewPref.btnMail.gameObject);
            this.fteUIDict.Add(FteConst.Chat, this.viewPref.btnChat.gameObject);
            //this.fteUIDict.Add(FteConst.Tribute, this.viewPref.btnTribute.gameObject);
            this.fteUIDict.Add(FteConst.Build, this.viewPref.btnBuild.gameObject);
            this.fteUIDict.Add(FteConst.Lottey, this.viewPref.pnlLottery.gameObject);
            //this.fteUIDict.Add(FteConst.Treasure, this.viewPref.btnTreasure.gameObject);
            this.fteUIDict.Add(FteConst.Live, this.viewPref.btnLyVoice.gameObject);
            this.fteUIDict.Add(FteConst.Housekeeper, this.viewPref.btnHouseKeeper.gameObject);
            this.fteUIDict.Add(FteConst.ShowBtnsPanel, this.viewPref.btnShowBtnsPnl.gameObject);
            this.fteUIDict.Add(FteConst.Queue, this.viewPref.pnlQueue.gameObject);
            this.fteUIDict.Add(FteConst.Pay, this.viewPref.btnPay.gameObject);
            this.fteUIDict.Add(FteConst.MiniMap, this.viewPref.btnMiniMap.gameObject);
            this.fteUIDict.Add(FteConst.Campaign, this.viewPref.pnlCampaign.gameObject);
            this.fteUIDict.Add(FteConst.Store, this.viewPref.pnlStore.gameObject);
            this.fteUIDict.Add(FteConst.FirstPay, this.viewPref.pnlPayReward.gameObject);
            this.fteresourceDict.Add(Resource.Food, this.viewPref.transform);
            this.fteresourceDict.Add(Resource.Lumber, this.viewPref.transform);
            this.fteresourceDict.Add(Resource.Steel, this.viewPref.transform);
            this.fteresourceDict.Add(Resource.Marble, this.viewPref.transform);
            this.fteresourceDict.Add(Resource.Gem, this.viewPref.transform);
            this.fteresourceDict.Add(Resource.Gold, this.viewPref.transform);
            this.SetBtnAutoBattle();
            this.homeCoord = RoleManager.GetRoleCoordinate();
            UpdateManager.Regist(UpdateInfo.MapView, this.UpdateAction);
        }

        protected override void OnUIInit() {
            GameHelper.SetCanvasCamera(this.uiSlider.GetComponent<Canvas>());
            this.uiSlider.GetComponent<CanvasScaler>().matchWidthOrHeight
                = UIManager.AdaptiveParam;
        }

        public void Init() {
            base.InitUI();
            this.InitMap();
            this.viewModel.ShowTopHUD(true);
            MapElementManager.Instance.RemoveElement(0);
            AudioManager.Stop(AudioType.Background);
            UIManager.UpdateProgress(1.1f, 0.5f, this.AfterInit);
            this.viewModel.SetInitArrow();
        }

        private void AfterInit() {
            ModelManager.UnLoadScene("SceneLogin");
            ModelManager.UnLoadScene("SceneCountry");
            FteView.SetCloudsUIVisible(false, () => {
                this.isResizing = true;
                UIManager.HideLoading();
            });
            this.PlayInitAnimation();
        }

        public void InitCamera() {
            this.renderCamera = GameManager.MainCamera.transform.Find("RenderCamera").GetComponent<Camera>();
            this.battlePrerenderCamera = GameManager.MainCamera.transform.
                Find("BattlePrerenderCamera").GetComponent<Camera>();
            //  this.highlightCamera = GameManager.MainCamera.transform.Find("HighlightCamera").GetComponent<Camera>();
            //this.fteCamera = GameObject.FindGameObjectWithTag("UICamera").transform.Find("FteCamera").GetComponent<Camera>();
            this.CameraSize = this.viewModel.CameraInfo.orginSize;
            this.targetSize =
                (this.viewModel.CameraInfo.maxSize + this.viewModel.CameraInfo.minSize) / 2;
            //this.isResizing = true;
            this.resizeLerp = 2;
            this.isDraggable = false;
            this.isPinchable = false;
            Vector3 position = MapUtils.CoordinateToPosition(this.viewModel.CenterCoordinate);
            GameManager.MainCamera.transform.position = position + new Vector3(0, 0, -12);
            this.transform.position =
            this.terrainBackgroundView.Center = position;
        }

        public void CameraAnimate() {
            this.CameraSize = this.viewModel.CameraInfo.orginSize;
            this.targetSize =
             (this.viewModel.CameraInfo.maxSize + this.viewModel.CameraInfo.minSize) / 2;
            this.isResizing = true;
            this.resizeLerp = 2;
        }

        private void InitMap() {
            float horizon = Mathf.Floor(tileCache.x / 2);
            float vertical = Mathf.Floor(tileCache.y / 2);
            Vector2 coordinate = Vector2.zero;
            for (float i = -horizon; i <= horizon; i++) {
                for (float j = -vertical; j <= vertical; j++) {
                    coordinate = this.viewModel.CenterCoordinate +
                        i * Vector2.one + j * GameConst.LeftUp;
                    this.CreateTile(coordinate);
                }
            }

            horizon = tileCache.x / 2;
            vertical = tileCache.y / 2;
            for (float i = -horizon; i <= horizon; i++) {
                for (float j = -vertical; j <= vertical; j++) {
                    coordinate = this.viewModel.CenterCoordinate +
                        i * Vector2.one + j * GameConst.LeftUp;
                    this.CreateTile(coordinate);
                }
            }
            MapElementManager.ShowElement();
        }

        public void SetChestLimit() {
            DailyLimit dailyLimit = this.viewModel.GetDailyLimit();
            this.viewPref.imgFlash.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(this.viewPref.imgFlash.gameObject, "Flash", () => {
                this.viewPref.sliderChestNum.value
                                = (float)dailyLimit.ChestCurrent / (float)dailyLimit.ChestLimit;
                this.viewPref.txtChestNum.text =
                    string.Concat(dailyLimit.ChestCurrent.ToString(), "/",
                    dailyLimit.ChestLimit.ToString());
                this.viewPref.imgFlash.gameObject.SetActiveSafe(false);
            });
        }

        public void Move(Vector2 position) {
            if (!this.isFocus) {
                this.StartJumping(jumpLerp, position);
            }
        }

        public void ShowBattleResult(TroopArrivedNtf message) {
            if (this.isShowTileReward) {
                this.viewModel.ChangeResource(message.Reward);
                return;
            }

            if (this.battleResultPreview == null) {
                GameObject pnlBattleResult = PoolManager.GetObject(PrefabPath.pnlBattleResult, this.ui.transform);
                this.battleResultPreview =
                    pnlBattleResult.GetComponent<BattleResultPreview>();
            }

            // Can't judeg is the first win battle.
            UnityAction afterBattleAction = () => {
                if (isInStep61) {
                    Debug.LogError("isInsetp61");
                    //this.viewModel.ChangeResource(message.Reward);
                    this.fteReward = message.Reward;
                    this.fteResources = message.Resources;
                    this.fteCurrency = message.Currency;
                    return;
                }
                UnityAction collectAction = () => {
                    this.viewModel.ShowBattleResultTip(message);
                    this.battleResultPreview.CollectLottery(
                       message, tileDict,
                       () => {
                           this.afterBattleGetResouse.InvokeSafe();
                           this.afterBattleGetResouse = null;
                           this.isShowTileReward = false;
                           this.viewModel.ShowFirstDownReward();
                       }
                   );
                };
                UnityAction forceAniShowAction =
                    () => this.viewModel.ForceUpgradeAnimation(collectAction);
                if (!PlayerPrefs.HasKey(GameConst.AUTO_BATTLE_KEY) &&
                    message.BattleTimes >= GameConst.BATTLE_MUST_REPLAY_TIMES) {
                    UIManager.ShowConfirm(
                          LocalManager.GetValue(LocalHashConst.notice_title_warning),
                          LocalManager.GetValue(LocalHashConst.fte_battle_animation_auto_desc),
                          () => {
                              PlayerPrefs.SetInt(GameConst.AUTO_BATTLE_KEY, 1);
                              this.SetBtnAutoBattle();
                              forceAniShowAction();
                          },
                          () => {
                              PlayerPrefs.SetInt(GameConst.AUTO_BATTLE_KEY, 0);
                              this.SetBtnAutoBattle();
                              forceAniShowAction();
                          },
                          txtYes: LocalManager.GetValue(LocalHashConst.on),
                          txtNo: LocalManager.GetValue(LocalHashConst.off),
                          canHide: false
                    );
                } else {
                    forceAniShowAction();
                }
            };

            this.isShowTileReward = true;
            if (message.BattleTimes <= GameConst.BATTLE_MUST_REPLAY_TIMES ||
                !PlayerPrefs.HasKey(GameConst.AUTO_BATTLE_KEY) ||
                (PlayerPrefs.HasKey(GameConst.AUTO_BATTLE_KEY) &&
                PlayerPrefs.GetInt(GameConst.AUTO_BATTLE_KEY) == 1)) {
                this.viewModel.PlayBattleReport(message.ReportId, afterBattleAction);

            } else {
                afterBattleAction.InvokeSafe();
            }
        }

        public void FteFocus(Vector2 position) {
            if (position != GameConst.LeftDown) {
                this.viewModel.Move(position);
            }
            this.isFocus = true;
            this.originSize = GameManager.MainCamera.orthographicSize;
            this.StartResizing(4, 7);
            this.EnableHighlight();
            this.HideCloud();
        }

        public void Focus(Vector2 position) {
            this.StartJumping(jumpLerp, position);
            this.isFocus = true;
            this.originSize = GameManager.MainCamera.orthographicSize;
            this.StartResizing(4, 7.8f);
            this.EnableDarkMask();
            this.EnableHighlight();
            this.HideButtons();
            this.HideCloud();
            this.viewModel.HideTopHUD();
            this.viewModel.HideQueueView();
        }

        public void LoseFocus() {
            if (this.isFocus) {
                this.isFocus = false;
                this.StartResizing(12, Mathf.Min(this.originSize, this.viewModel.CameraInfo.maxSize));
                this.DisableDarkMask();
                this.DisableHighlight();
                this.ShowButtons();
                this.ShowCloud();
                this.viewModel.ShowTopHUD();
                this.viewModel.ShowQueueView();
            }
        }

        public void FollowMarch(string id) {
            if (!this.selfMarchDict.ContainsKey(id) &&
                !this.otherMarchDict.ContainsKey(id)) {
                this.CreateMarch(EventManager.GetMarchById(id));
            }
            MarchLineView marchLine =
                this.selfMarchDict.ContainsKey(id) ?
                this.selfMarchDict[id] :
                this.otherMarchDict[id];
            GameObject marchObj = marchLine.transform.Find("March").gameObject;
            StartJumping(jumpLerp, marchObj.transform.position, marchObj);

        }

        public void FollowNewMarch(string id) {
            this.isNewMarchFocus = true;
            this.originSize = GameManager.MainCamera.orthographicSize;
            this.StartResizing(4, 12);
            this.FollowMarch(id);
        }

        public void DisFollowNewMarch() {
            if (this.isNewMarchFocus) {
                this.isNewMarchFocus = false;
                this.StartResizing(9, this.originSize);
            }
        }

        public bool IsMarchVisible(string id) {
            return this.selfMarchDict.ContainsKey(id) || this.otherMarchDict.ContainsKey(id);
        }

        private bool allianceStatusChange = false;
        public void NoticeAllianceStatusChange() {
            if (!RoleManager.GetAllianceId().CustomIsEmpty()) {
                allianceStatusChange = true;
                this.viewPref.pnlAllianceNotice.gameObject.SetActiveSafe(true);
            }
        }

        public void SetMapChatInfoView(string channel, string name, string content) {
            this.viewPref.txtChatPlayer.text =
                string.Format(
                    "<color=#60DBFFFF>[{0}]</color> <color=#FFF77CFF>[{1}]</color>",
                    channel, name
                );
            this.viewPref.txtChatInfo.StripLengthWithSuffix(content);
        }

        public void SetFallenInfoVisible(bool isPlayerFalled) {
            this.viewPref.pnlFallen.gameObject.SetActiveSafe(isPlayerFalled);
        }

        private void UpdateAction() {
            this.UpdatePosition();
            this.UpdateHome();
            if (this.isResizing) {
                this.UpdateCameraSize();
            }
            //  this.UpdateCheckList();
        }

        public void MoveToBattlePoint(Vector2 coordinate) {
            this.HideButtonPanel(false);
            this.HideQueuePanel(false);
            this.viewModel.HideTileInfo();
            UnityAction afterFocus = () => {
                this.StartResizing(3, 7);
                UIManager.ShowAboveUIBattle(() => {
                    ModelManager.LoadScene("Scene3DBattle", true);
                }, () => this.RemoveBattleFocus(coordinate));
            };
            this.viewModel.MoveWithEvent(
                coordinate,
                () => {
                    this.AddBattleFocus(coordinate, afterFocus);
                }
            );
        }

        private void AddBattleFocus(Vector2 coordinate, UnityAction callback) {
            this.viewModel.CloseAboveUI();
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                tileView.AddBattleFocus(callback);
            }
        }

        private void RemoveBattleFocus(Vector2 coordinate) {
            GameObject tile;
            this.isResizing = false;
            this.CameraSize =
                (this.viewModel.CameraInfo.maxSize + this.viewModel.CameraInfo.minSize) / 2;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                tileView.RemoveBattleFocus();
            }
        }
        // end Test
        public void SetTaskDetail(int taskId, TaskType type, string content, bool receiveable, UnityAction jumpAction) {
            AnimationManager.Animate(this.viewPref.pnlTask.gameObject, "Rotate");
            this.taskId = taskId;
            this.taskType = type;
            this.isReceiveable = receiveable;
            bool isDailyTask = (type == TaskType.daily);
            bool hasGuid = !content.CustomIsEmpty();
            if (hasGuid) {
                this.taskJumpAction = (!isDailyTask || receiveable) ?
                    this.OnBtnMissionClick : jumpAction;
                this.viewPref.imgJumpBG.material = receiveable ?
                    PoolManager.GetMaterial(MaterialPath.matScan) :
                    PoolManager.GetMaterial(MaterialPath.matImageFast);
                if (receiveable) {
                    content = string.Concat(content, " ", isDailyTask ?
                                LocalManager.GetValue(LocalHashConst.receive_daily_award) :
                                LocalManager.GetValue(LocalHashConst.receive_chapter_award));
                }
                this.viewPref.txtTaskContent.text = content;
            } else {
                this.viewPref.txtTaskContent.text = LocalManager.GetValue(LocalHashConst.chapter_continue);
                this.viewPref.imgJumpBG.material = null;
            }
            bool taskDone = (hasGuid && receiveable);
            this.viewPref.imgJumpBG.sprite = ArtPrefabConf.GetSprite(SpritePath.chapterTaskPrefix,
                taskDone ? "done" : "undone");
            Color imgJumpBGColor = taskDone ? Color.white : Color.black;
            imgJumpBGColor.a = taskDone ? 1.0f : 0.5f;
            this.viewPref.imgJumpBG.color = imgJumpBGColor;
            this.viewPref.txtTaskStatus.text = isDailyTask ?
                    LocalManager.GetValue(LocalHashConst.HUD_daily_task) :
                    LocalManager.GetValue(LocalHashConst.HUD_chapter_task);
            this.viewPref.imgTaskIcon.sprite = ArtPrefabConf.GetSprite(
                SpritePath.chapterTaskPrefix, Enum.GetName(typeof(TaskType), type));
        }

        public void UpdateTaskCollectableCount(int count) {
            this.viewPref.pnlTaskNewPoint.gameObject.SetActiveSafe(count > 0);
            this.viewPref.txtTaskRewardCount.text = count.ToString();
        }

        public void CollectTaskReward(Protocol.Resources resources,
        Protocol.Currency currency, CommonReward commonReward) {
            GameHelper.CommonRewardCollect(resources, currency, commonReward,
                (Vector2)this.viewPref.btnJump.transform.position, false);
        }

        private void OnBtnHomeClick() {
            this.viewModel.HideTileInfo();
            this.HideButtonPanel(true);
            this.HideQueuePanel(true);
            this.StartJumping(jumpLerp,
                MapUtils.CoordinateToPosition(RoleManager.GetRoleCoordinate()));
        }

        private void OnBtnHeroClick() {
            this.viewModel.ShowHero();
        }

        private void OnBtnRankClick() {
            this.viewModel.ShowRank();
        }

        private void OnBtnPayClick() {
            LYGameData.OnOtherOpenStore();
            this.viewModel.ShowPay();
        }

        private void OnBtnStoreClick() {
            LYGameData.OnOtherClickIgButton();
            this.viewPref.storeBtnView.SetStoreBtn(false);
            this.viewModel.ShowPay();
        }

        private void OnBtnPayRewardClick() {
            this.viewModel.ShowPayReward();
        }

        private void OnBtnMailClick() {
            this.viewModel.ShowMail();
        }

        private void OnBtnHouseKeeperClick() {
            Debug.Log(EventManager.IsBuildEventMaxFull());
            if (!EventManager.IsBuildEventFull()) {
                Debug.Log("EventManager.IsBuildEventMaxFull()");
                //this.viewModel.RefreshHouseKeeperEvent();
                this.ShowBuildBtn();
            }
            this.viewModel.ShowHouseKeeper();
        }

        private void OnBtnAutoBattleClick() {
            int value = PlayerPrefs.GetInt(GameConst.AUTO_BATTLE_KEY);
            if (value == 1) {
                PlayerPrefs.SetInt(GameConst.AUTO_BATTLE_KEY, 0);
                this.viewPref.txtAutoBattle.text =
                    LocalManager.GetValue(LocalHashConst.off);
            } else {
                PlayerPrefs.SetInt(GameConst.AUTO_BATTLE_KEY, 1);
                this.viewPref.txtAutoBattle.text =
                    LocalManager.GetValue(LocalHashConst.on);
            }
        }

        private void OnBtnSelectSeverClick() {
            this.viewModel.ShowSelectServer();
        }

        private void OnBtnBuildClick() {
            this.viewModel.ShowBuildList();
        }

        private void OnBtnShowBtnsPnlClick() {
            this.viewModel.OnAnyOperate();
            if (isShowBtnsPnl) {
                this.HideButtonPanel(true);
            } else {
                this.ShowButtonsPanel();
            }
        }

        private void ShowButtonsPanel(UnityAction afterShow = null) {
            AnimationManager.Animate(this.viewPref.pnlButtonPanel.gameObject, "Show",
                () => {
                    this.viewPref.imgShowBtnsPnl.localScale = imgShowBtnsOpt;
                    afterShow.InvokeSafe();
                }, isOffset: false);
            this.isShowBtnsPnl = true;
        }

        private void OnBtnAllianceClick() {
            this.viewModel.ShowAllianceInfo();
            this.allianceStatusChange = false;
            this.SetButtonPanelNotice();
        }

        private void OnBtnFallenClick() {
            this.viewModel.ShowFallen();
        }

        private void OnBtnLyVoiceClick() {
            this.viewModel.ShowLyVoice();
        }

        //private bool tributeTipsVisible = false;
        private void OnBtnTributeClick() {
            this.viewModel.HideTileInfo();
            this.HideButtonPanel(true);
            this.HideQueuePanel(true);
            this.viewModel.ShowTribute();
        }

        private void OnBtnMiniMapClick() {
            this.viewModel.ShowMiniMap();
        }

        private void OnBtnChatClick() {
            this.viewModel.ShowChat();
        }

        private void OnBtnMissionClick() {
            bool isDrama = taskType == TaskType.drama;
            if (isReceiveable && isDrama && this.taskId != -1) {
                this.viewModel.GetDramaRewards(this.taskId);
            } else {
                this.viewModel.ShowMission(isDrama ? 0 : 1);
            }
        }

        private void OnBtnJumpClick() {
            this.taskJumpAction.InvokeSafe();
        }

        public void SetPayBtn(bool canReward) {
            if (canReward) {
                this.viewPref.btnPay.pnlContent.Find("ImgIcon").gameObject.SetActiveSafe(false);
                this.viewPref.btnPay.pnlContent.Find("ImgIconHighlight").gameObject.SetActiveSafe(true);
                AnimationManager.Animate(
                            this.viewPref.btnPay.pnlContent.Find("PnlIcon").gameObject, "Highlight", null
                        );
            } else {
                this.viewPref.btnPay.pnlContent.Find("ImgIcon").gameObject.SetActiveSafe(true);
                this.viewPref.btnPay.pnlContent.Find("ImgIconHighlight").gameObject.SetActiveSafe(false);
                AnimationManager.Stop(this.viewPref.btnPay.pnlContent.Find("PnlIcon").gameObject);
            }
        }

        #region Campaign logic

        private void OnCampaignClick() {
            this.viewModel.ShowCampaignsPanel();
        }
        
        public Activity GetThisActivity() {
            return this.viewPref.CampaignBtnView.GetCurrentDisplayActivity();
        }
        #endregion

        //public void ChangeStoreEntrance() {
        //    //this.viewPref.storeBtnView.SetContent(10000);
        //}

        public void OnCenterChanged() {
            //Vector2 center = MapUtils.PositionToCoordinate(GameManager.MainCamera.transform.position);
            string snContent = this.viewModel.GetTileZone(this.viewModel.CenterCoordinate);
            if (snContent.CustomIsEmpty()) {
                snContent = this.oldSN;
            }
            this.oldSN = snContent;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ({1}, {2})",
                snContent,
                Mathf.CeilToInt(this.viewModel.CenterCoordinate.x),
                Mathf.CeilToInt(this.viewModel.CenterCoordinate.y));
            this.viewPref.txtSN.text = stringBuilder.ToString();
            this.terrainBackgroundView.Center = MapUtils.CoordinateToPosition(this.viewModel.CenterCoordinate);
        }

        /// <summary>
        /// When target changed.
        /// 1.Find the a particular point around the target.
        /// 2.Move camera, map collider and current map tiles to new point.
        /// 3.Set Jumping flag as true, then set target point.
        /// </summary>
        public void StartJumping(float lerp, Vector2 targetVect, GameObject followObj = null) {

            this.targetLerp = lerp;
            this.target = targetVect;
            Vector2 distance = this.target - GameManager.MainCamera.transform.position;

            const float maxDistance = 70f;
            this.followObj = followObj;
            this.velocity = Vector2.zero;
            this.isOverstep = false;
            this.elasticDistance = Vector2.zero;
            this.IsJumping = true;
            this.isDragging = false;
            this.isDraggable = false;
            if (distance.sqrMagnitude > maxDistance * maxDistance) { //cache has this point;
                this.jumpFrame = 0;
                Vector2 move = maxDistance * distance.normalized;
                Vector2 newCenter = MapUtils.PositionToCoordinate((Vector2)this.target - move, this.oddEven);
                newCenter.x = Mathf.Min(Mathf.Max(newCenter.x, this.viewModel.MinCoordinate.x + (1 - this.oddEven * 0.5f)), this.viewModel.MaxCoordinate.x - (1 - this.oddEven * 0.5f));
                newCenter.y = Mathf.Min(Mathf.Max(newCenter.y, this.viewModel.MinCoordinate.y + (1 - this.oddEven * 0.5f)), this.viewModel.MaxCoordinate.y - (1 - this.oddEven * 0.5f));
                this.ResetBindUIElement();
                this.RefreshTileDict(newCenter - this.viewModel.CenterCoordinate);
                Vector2 jumpOffset = (newCenter - this.viewModel.CenterCoordinate).x * MapUtils.TileHorizon +
                    (newCenter - this.viewModel.CenterCoordinate).y * MapUtils.TileVertical;
                GameManager.MainCamera.transform.position += (Vector3)jumpOffset;
                foreach (Transform cell in this.Tile.transform) {
                    cell.Translate(jumpOffset);
                }
                this.viewModel.CenterCoordinate = newCenter;
                this.transform.Translate(jumpOffset);
            }
        }

        private void StartResizing(float lerp, float size) {
            this.isResizing = true;
            this.targetSize = size;
            this.resizeLerp = lerp;
        }

        /// <summary>
        /// Update view, Camera first.
        /// Problem: Need split border elastic and gototion.
        /// </summary>
        private void UpdatePosition() {
            float deltaTime = Time.unscaledDeltaTime;
            if (isDragging) {
                this.velocity = this.dragDelta / deltaTime;
            } else if (this.IsJumping) {
                if (!this.isDraggable && this.jumpFrame++ > 60) {
                    this.isDraggable = true;
                    this.jumpFrame = 0;
                }
                if (this.followObj != null) {
                    this.target = this.followObj.transform.position;
                    //float distance = Vector2.Distance(this.target, GameManager.MainCamera.transform.position);

                    float distance = ((Vector2)this.target - (Vector2)GameManager.MainCamera.transform.position).magnitude;
                    if (distance < 2f) {
                        this.targetLerp = (1 / deltaTime) * (0.1f / distance);
                        if (distance < 0.04f) {
                            this.viewModel.ShowMarchInfo(this.followObj);
                        }
                    }
                }
                // To do: Fte temp, need change.
                float sqrDistance = ((Vector2)this.target - (Vector2)GameManager.MainCamera.transform.position).sqrMagnitude;
                if (sqrDistance < 0.04f) {
                    this.viewModel.InvokeMoveEvent();
                    if (this.followObj == null) {
                        this.IsJumping = false;
                        this.isDraggable = true;
                    }
                }
                Vector3 tmp = Vector3.Lerp(GameManager.MainCamera.transform.position, this.target, this.targetLerp * deltaTime);
                this.velocity = -(tmp - GameManager.MainCamera.transform.position) / deltaTime;
            } else if (this.isOverstep) {
                Vector3 tmp = Vector3.Lerp(GameManager.MainCamera.transform.position, this.target, this.targetLerp * deltaTime);
                this.velocity = -(tmp - GameManager.MainCamera.transform.position) / deltaTime;
            } else {
                if (this.velocity.sqrMagnitude < minSpeed * minSpeed) {
                    this.velocity = Vector2.zero;
                }
                this.velocity = Vector2.Lerp(this.velocity, Vector2.zero, damping * deltaTime);
            }
            Vector2 delta = this.velocity * deltaTime;
            if (delta == Vector2.zero) {
                return;
            }
            offsetCell = Vector2.zero;
            // Check Camera border first.
            delta = this.CheckBorder(-delta);
            Vector2 newOffset = this.offset + delta;
            this.dragDelta = Vector2.zero;
            GameManager.MainCamera.transform.position -= (Vector3)delta;
            if (newOffset.x.Abs() > MapUtils.TileSize.x ||
                newOffset.y.Abs() > MapUtils.TileSize.y) {
                offsetCell.x = Mathf.Floor(newOffset.x.Abs() / MapUtils.TileSize.x) *
                        Mathf.Sign(newOffset.x);
                offsetCell.y = Mathf.Floor(newOffset.y.Abs() / MapUtils.TileSize.y) *
                        Mathf.Sign(newOffset.y);
                this.HandleOffset(offsetCell, Vector2.right);
                this.HandleOffset(offsetCell, Vector2.up);
            }
            this.transform.Translate(-delta);
            this.offset.x = newOffset.x - MapUtils.TileSize.x * offsetCell.x;
            this.offset.y = newOffset.y - MapUtils.TileSize.y * offsetCell.y;
        }

        public void ShowHome(bool visible) {
            if (this.objHome == null && !visible) {
                return;
            }

            if (this.objHome == null && visible) {
                this.uiDynamic = UIManager.GetUI("UIMapDynamic");
                GameHelper.SetCanvasCamera(this.uiDynamic.GetComponent<Canvas>());
                this.uiDynamic.GetComponent<CanvasScaler>().matchWidthOrHeight
                    = UIManager.AdaptiveParam;
                UIManager.ShowUI(this.uiDynamic);
                Transform pnlHome = this.uiDynamic.transform.Find("PnlHome");
                this.homeRectTransform = pnlHome.GetComponent<RectTransform>();
                this.objHome = pnlHome.gameObject;
                Button btnHome = pnlHome.GetComponent<Button>();
                btnHome.onClick.AddListener(this.OnBtnHomeClick);
                this.pnlHomeText = pnlHome.Find("Text");
            }

            GameHelper.SetLayer(this.objHome,
                visible ? UIManager.LayerUIIndex : UIManager.LayerUIInvisibleIndex);
        }

        private void UpdateHome() {
            bool isShowHome = !this.tileDict.ContainsKey(homeCoord);
            this.ShowHome(isShowHome);
            if (isShowHome) {
                Vector2 homePos = MapUtils.CoordinateToPosition(homeCoord);
                Vector2 direction = (homePos - (Vector2)GameManager.MainCamera.transform.position).normalized;
                Vector2 rect = new Vector2(
                    UIManager.UIRect.width - 240,
                    UIManager.UIRect.height - 520
                );
                Vector2 position = direction * (1 / direction.x.Abs()) * rect.x / 2;
                if (position.y.Abs() > rect.y / 2) {
                    position = direction * (1 / direction.y.Abs()) * rect.y / 2;
                }
                this.homeRectTransform.anchoredPosition = position;
                float angle = Vector2.Angle(Vector2.up, direction);
                if (direction.x < 0) {
                    angle = -angle;
                    this.pnlHomeText.localEulerAngles =
                    this.homeRectTransform.localEulerAngles = Vector3.zero;
                } else {
                    this.pnlHomeText.localEulerAngles = Vector3.forward * 180;
                }
                this.homeRectTransform.eulerAngles = new Vector3(0, 0, -angle - 90);
            }
        }

        private void UpdateCameraSize() {
            if (!this.isPinching) {
                this.CameraSize += (this.targetSize -
                    this.CameraSize) * this.resizeLerp * 0.0167f;
            }

            // Only execute once.
            if (this.CameraSize < this.viewModel.CameraInfo.maxSize &&
                !this.isPinchable) {
                this.isPinchable = true;
                this.isDraggable = true;
                if (!this.isCameraInit) {
                    //PoolManager.GeneratePreloadGameObjs();
                    //this.viewModel.PlayInitAnimation();
                    this.isCameraInit = true;
                }
            }

            if ((this.CameraSize - this.targetSize).Abs() < 0.01f) {
                this.isResizing = false;
            }
        }

        public void PlayInitAnimation() {
            this.viewPref.cgBottom.interactable = false;
            this.viewPref.cgBottom.alpha = 1;
            AnimationManager.Animate(this.viewPref.pnlLeftBottom.gameObject, "Show", () => {
                this.viewModel.SetMapChatWorldInfo();
                this.viewModel.SetMapChatAllianceInfo();
                this.viewPref.btnJump.gameObject.SetActiveSafe(true);
                FteManager.CanStart = true;
            }, isOffset: false);
            AnimationManager.Animate(this.viewPref.pnlLeftUp.gameObject, "Show", () => {
            }, isOffset: false);
            AnimationManager.Animate(this.viewPref.pnlRightBottom.gameObject, "Show", () => {
                this.viewPref.cgBottom.interactable = true;
            }, isOffset: false);
            AnimationManager.Animate(this.viewPref.pnlRightUp.gameObject, "Show", isOffset: false);
        }

        /// <summary>
        /// Use scroll algorithm to refresh map.
        /// </summary>
        /// <param name="offsetCell"></param>
        /// <param name="axis"></param>
        private Vector2 offsetAbs = Vector2.zero;
        private Vector2 unitOpposition = Vector2.one;
        public void HandleOffset(Vector2 offsetCell, Vector2 axis) {
            this.offsetAbs.x = offsetCell.x.Abs();
            this.offsetAbs.y = offsetCell.y.Abs();
            Vector2 axisOpposition = Vector2.one - axis;
            Vector2 oldCenter = this.viewModel.CenterCoordinate;
            Vector2 unit = axis == Vector2.right ? Vector2.one : GameConst.LeftUp;
            this.unitOpposition.x = -unit.x;
            this.viewModel.CenterCoordinate =
                this.viewModel.CenterCoordinate + (-Vector2.Dot(offsetCell, axis) / 2) * unit;
            
            int offsetAbsDotAxis = (int)Vector2.Dot(offsetAbs, axis);
            for (int i = 0; i < offsetAbsDotAxis; i++) {
                Vector2 offsetX = (
                    Mathf.Ceil(Vector2.Dot(tileCache, axis) / 2f) + i / 2f) *
                    unit * Mathf.Sign(-Vector2.Dot(offsetCell, axis)
                );
                int tileCacheDotAxisOpposition = (int)Vector2.Dot(tileCache, axisOpposition);
                int tileCacheDotAxisOppositionPlusOddEven = tileCacheDotAxisOpposition + this.oddEven;
                for (int j = 0; j < tileCacheDotAxisOppositionPlusOddEven; j++) {
                    Vector2 offsetY = (Mathf.Floor(tileCacheDotAxisOpposition / 2f) - j + this.oddEven / 2f) * unitOpposition;
                    Vector2 coordinate = oldCenter + offsetX + offsetY;
                    this.CreateTile(coordinate);
                }

                this.oddEven = (++this.oddEven) % 2;
                tileCacheDotAxisOppositionPlusOddEven = tileCacheDotAxisOpposition + this.oddEven;
                for (int j = 0; j < tileCacheDotAxisOppositionPlusOddEven; j++) {
                    Vector2 coordinate = (
                        oldCenter +
                        (Mathf.Ceil(Vector2.Dot(tileCache, axis) / 2f) - i / 2f - 0.5f) * unit * Mathf.Sign(Vector2.Dot(offsetCell, axis)) +
                        (Mathf.Floor(tileCacheDotAxisOpposition / 2f) - j + this.oddEven / 2f) * unitOpposition
                    );

                    if (this.tileDict.ContainsKey(coordinate)) {
                        this.RemoveTile(coordinate);
                        this.tileDict.Remove(coordinate);
                    }
                }
            }
        }

        /// <summary>
        /// Called by OnTargetChanged, to move current map object to particular ponit.
        /// </summary>
        /// <param name="offset"></param>

        private Dictionary<Vector2, GameObject> newTileDict =
            new Dictionary<Vector2, GameObject>(24);
        private void RefreshTileDict(Vector2 offset) {
            this.newTileDict.Clear();
            foreach (var pair in this.tileDict) {
                this.newTileDict.Add(pair.Key + offset, pair.Value);
            }
            this.tileDict.Clear();
            foreach (var pair in this.newTileDict) {
                this.tileDict.Add(pair.Key, pair.Value);
            }
        }

        public void HidePopUp() {
            this.viewModel.HideTileInfo();
            this.HideButtonPanel(true);
            this.HideQueuePanel(true);
        }

        public void HideButtonPanel(bool needAnimation) {
            if (this.isShowBtnsPnl) {
                this.isShowBtnsPnl = false;
                AnimationManager.Animate(this.viewPref.pnlButtonPanel.gameObject, "Hide",
                    isOffset: false, finishCallback: () => {
                        this.viewPref.imgShowBtnsPnl.localScale = Vector3.one;
                    }
                );
                if (!needAnimation) {
                    AnimationManager.Finish(this.viewPref.pnlButtonPanel.gameObject);
                }
            }
            if (!needAnimation) {
                this.viewPref.pnlButtonPanel.gameObject.SetActive(false);
            }
        }

        public void HideQueuePanel(bool needAnimation) {
            if (this.viewModel.IsQueueShow) {
                this.viewModel.IsQueueShow = false;
                this.viewModel.SetQueueIsFold(true);
                AnimationManager.Animate(this.viewPref.pnlQueue.gameObject, "Hide",
                    isOffset: false, finishCallback: () => {
                        this.viewPref.imgShowQueue.localScale = Vector3.one;
                    }
                );
                if (!needAnimation) {
                    AnimationManager.Finish(this.viewPref.pnlQueue.gameObject);
                }
            }
            if (!needAnimation) {
                this.viewPref.pnlQueue.gameObject.SetActive(false);
            }
        }

        public void HideHUD() {
            this.hideHudCallback.InvokeSafe();
            this.hideHudCallback = null;
            this.HideButtons();
            this.viewModel.HideTopHUD();
            this.viewModel.HideQueueView();
        }

        public void HideHUDWithoutTop() {
            this.HideButtons();
            this.viewModel.HideQueueView();
        }

        public void HideTileBindUI(Vector2 coordinate) {
            this.HideBuidProgressBar(coordinate);
        }

        public void ShowTileBindUI(Vector2 coordinate) {
            this.ShowBuildProgressBar(coordinate);
        }

        private void ShowButtons() {
            this.viewPref.pnlButtonPanel.gameObject.SetActive(true);
            this.viewPref.pnlQueue.gameObject.SetActive(true);
            UIManager.ShowUI(this.viewPref.pnlLeftBottom.gameObject);
            UIManager.ShowUI(this.viewPref.pnlLeftUp.gameObject);
            UIManager.ShowUI(this.viewPref.pnlRightBottom.gameObject);
            UIManager.ShowUI(this.viewPref.pnlRightUp.gameObject);
            AnimationManager.Animate(this.viewPref.pnlStore.gameObject, "Full", null);
        }

        public void HideButtons() {
            this.HideButtonPanel(false);
            this.HideQueuePanel(false);
            UIManager.HideUI(this.viewPref.pnlLeftBottom.gameObject);
            UIManager.HideUI(this.viewPref.pnlLeftUp.gameObject);
            UIManager.HideUI(this.viewPref.pnlRightBottom.gameObject);
            UIManager.HideUI(this.viewPref.pnlRightUp.gameObject);
            AnimationManager.Stop(this.viewPref.pnlStore.gameObject);
            this.viewModel.OnAnyOperate();
        }

        //private void Update() {
        //    if (Input.GetKeyDown(KeyCode.A)) {
        //        Debug.Log(!FteManager.HasArrow());
        //    }
        //}

        public void ShowHUD(bool isDrag = false) {
            if (!this.IsEdit) {
                StartCoroutine(this.DelayStarRefreshHouseKeeperEvent());
                this.ShowButtons();
                this.viewModel.ShowQueueView();
                this.viewModel.ShowTopHUD();
            }
        }

        private IEnumerator DelayStarRefreshHouseKeeperEvent() {
            yield return YieldManager.EndOfFrame;
            if (EventManager.IsBuildEventFull()
                && this.viewModel.HasAvaliableTroop()
                && !FteManager.HasArrow()) {
                this.viewModel.RefreshHouseKeeperEvent();
            }
        }

        public void ShowBuildEditViewUI() {
            this.ShowButtons();
        }

        public void ShowBuildViewUI() {
            this.HideButtons();
        }

        public void SetFteUI() {
            foreach (var ui in this.fteUIDict) {
                ui.Value.SetActiveSafe(FteManager.CheckUI(ui.Key));
            }
        }
        
        public void ShowBottomHousekeeperBtn(bool needTips, bool enforce = false) {
            if (FteManager.HasArrow() && !enforce) {
                return;
            }
            AnimationManager.Animate(this.viewPref.pnlBuildContent.gameObject, "Hide",
                finishCallback: () => {
                    this.viewPref.btnBuild.gameObject.SetActiveSafe(false);
                    this.viewPref.btnBottomHouseKeeper.gameObject.SetActiveSafe(true);
                    this.isHouseKeeper = true;
                    AnimationManager.Animate(this.viewPref.pnlHouseKeeperContent.gameObject, "Show"
                        , () => {
                            if (enforce) {
                                StartCoroutine(this.SetHousekeeperNotice(true));
                            }
                        });
                    if (needTips) {
                        //this.isHouseKeeperShake = false;
                        this.viewPref.pnlHouseKeeperTips.gameObject.SetActiveSafe(true);
                        (this.viewPref.pnlHouseKeeperTips as RectTransform).localScale = Vector3.one;
                        this.viewModel.operateAction += this.ShowBuildBtn;
                        AnimationManager.Animate(this.viewPref.pnlHouseKeeperTips.gameObject, "Show",
                            finishCallback: () => {
                                StartCoroutine(ShowHouseKeeperBtnShake());
                            }
                        );
                    }
                });
        }


        private IEnumerator ShowHouseKeeperBtnShake() {
            yield return YieldManager.GetWaitForSeconds(1.5f);
            AnimationManager.Animate(this.viewPref.pnlHouseKeeperContent.gameObject, "Shake");
            yield return YieldManager.GetWaitForSeconds(5f);
            AnimationManager.Animate(this.viewPref.pnlHouseKeeperTips.gameObject, "Hide",
                                 finishCallback: () => {
                                     //this.isHouseKeeperShake = true;
                                     this.viewPref.pnlHouseKeeperTips.gameObject.SetActiveSafe(false);
                                 });
        }

        public void ShowBuildBtn() {
            if (this.viewPref.btnBuild.gameObject.activeSelf &&
                !this.viewPref.btnBottomHouseKeeper.gameObject.activeSelf) {
                return;
            }
            this.viewPref.pnlHouseKeeperTips.gameObject.SetActiveSafe(false);
            AnimationManager.Animate(this.viewPref.pnlHouseKeeperContent.gameObject, "Hide",
                finishCallback: () => {
                    this.viewPref.btnBottomHouseKeeper.gameObject.SetActiveSafe(false);
                    this.viewPref.btnBuild.gameObject.SetActiveSafe(true);
                    this.isHouseKeeper = false;
                    AnimationManager.Animate(this.viewPref.pnlBuildContent.gameObject, "Show");
                });
        }

        public void SetHouseKeeperBtnHighlight() {
            this.ShowBottomHousekeeperBtn(false, true);
            FteManager.SetMask(
                this.viewPref.btnBottomHouseKeeper.transform,
                autoNext: false,
                hasArrow: false,
                isHighlight: true,
                afterCallBack: () => {
                    FteManager.HideRightChat();
                    this.viewModel.ShowHouseKeeper(isSetHighlightFrame: true);
                    this.ShowBuildBtn();
                    StartCoroutine(this.SetHousekeeperNotice(false));
                }
                );
        }
        public IEnumerator SetHousekeeperNotice(bool isShow) {
            yield return YieldManager.EndOfFrame;
            if (isShow) {
                this.viewPref.pnlHousekeeperNotice.gameObject.SetActiveSafe(true);
                yield return YieldManager.EndOfFrame;
                AnimationManager.Animate(this.viewPref.pnlHousekeeperNotice.gameObject,
                    "Show");
            } else {
                this.viewPref.pnlHousekeeperNotice.gameObject.SetActiveSafe(false);
                yield return YieldManager.EndOfFrame;
                AnimationManager.Stop(this.viewPref.pnlHousekeeperNotice.gameObject);
            }
        }

        public Transform GetViewPrefPoint() {
            return this.viewPref.transform;
        }

        public void SetTileLevelVisible(bool visible) {
            TileView.isShowLevel = visible;
            foreach (GameObject tile in this.tileDict.Values) {
                tile.GetComponent<TileView>().SetLevelVisible(visible);
            }
        }

        public void SetSelfBuildingVisible(bool visible) {
            TileView.isShowSelfBuilding = visible;
            foreach (GameObject tile in this.tileDict.Values) {
                tile.GetComponent<TileView>().SetSelfBuildingVisible(visible);
            }
        }

        public void ResetTileLevel() {
            foreach (GameObject tile in this.tileDict.Values) {
                tile.GetComponent<TileView>().ResetTileLevel();
            }
        }

        public void ResetSelfBuilding() {
            foreach (GameObject tile in this.tileDict.Values) {
                tile.GetComponent<TileView>().ResetSelfBuilding();
            }
        }

        #region FTE

        public void SetDramaArrow() {
            FteManager.SetArrow(this.viewPref.pnlTaskInfo, offset: Vector2.up * 50,
                arrowParent: this.viewPref.pnlLeftBottom);
        }

        public bool HasDramaArrow() {
            Transform pnlArrow = this.viewPref.pnlLeftBottom.Find("PnlFteArrow");
            return !(pnlArrow == null);
        }

        public void SetFteMarchLine(string id) {
            MarchLineView lineView = null;
            if (!this.selfMarchDict.TryGetValue(id, out lineView)) {
                this.otherMarchDict.TryGetValue(id, out lineView);
            }
            if (lineView != null) {
                lineView.FteSetLine();
            }
        }

        public void FteSetTileAboveVisible(Vector2 coordinate, bool visible) {
            if (this.tileDict.ContainsKey(coordinate)) {
                this.tileDict[coordinate].GetComponent<TileView>().FteSetTileAboveVisible(visible);
            }
        }

        public void OnFteStep41Start() {
            this.ShowBtnBuild();
            this.viewPref.btnBuild.pnlContent.Find("ImgNewPoint").gameObject.SetActiveSafe(false);
            FteManager.SetMask(this.viewPref.btnBuild.pnlContent,
                isEnforce: true, isHighlight: true);
            this.isInStep61 = true;
        }

        public void OnFteStep81Start() {
            if (this.fteReward.Resources != null) {
                GameHelper.CollectResources(
                    this.fteReward,
                    this.fteResources,
                    this.fteCurrency,
                    this.fteresourceDict,
                    true
                );
            }
            GachaGroupConf lottery = GachaGroupConf.GetConf("guide_free_gift_1");
            GameHelper.ChestCollect(
                this.viewPref.transform.position,
                lottery, CollectChestType.collectWithDropOut
            );
            this.isShowTileReward = false;
            this.isInStep61 = false;
        }

        public void OnFteStep101Start() {
            FteManager.SetMask(this.viewPref.btnFreeChest.pnlContent,
                isEnforce: true, isHighlight: true, offset: new Vector2(0, 40));
        }

        public void DeleteFteTroop(Vector2 coordinate) {
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                tile.GetComponent<TileView>().DeleteFteTroop();
            }
        }

        public void AddFteTroop(Vector2 coordinate) {
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                tile.GetComponent<TileView>().CreatFteTroop();
            }
        }

        public void OnBuildStep1Start() {
            // Text : FTE need rewrite
            bool isEnforce = FteManager.Instance.curStep.CustomEquals("chapter_task_3");
            this.hideHudCallback = FteManager.StopFte;
            FteManager.SetMask(this.viewPref.btnBuild.transform,
                isButton: true, isEnforce: isEnforce, arrowParent: this.viewPref.pnlLeftBottom);
        }

        public void OnBuildStep1End() {
            this.hideHudCallback = null;
        }

        public void OnAllianceStep1Start() {
            // Text : FTE need rewrite
            this.ShowButtonsPanel(() => {
                this.hideHudCallback = FteManager.StopFte;
                FteManager.SetMask(this.viewPref.btnAlliance.pnlContent, isButton: true,
                    arrowParent: this.viewPref.pnlButtonPanel);
            });
        }

        public void OnAllianceStep1End() {
            this.hideHudCallback = null;
        }

        public void OnChatStep1Start() {
            this.hideHudCallback = FteManager.StopFte;
            FteManager.SetMask(this.viewPref.btnChat.pnlContent, isButton: true,
                arrowParent: this.ui.transform, rotation: -45, offset: new Vector2(40, 20));
        }

        public void OnChatStep1End() {
            this.hideHudCallback = null;
        }

        #endregion

        public void ShowGetChest(Vector3 startPosition,
            GachaGroupConf lotteryConf, CollectChestType Type, UnityAction AfterShowCallback) {
            GameObject collectResource = PoolManager.GetObject(
                PrefabPath.collectChest,
                AnimationManager.Instance.transform
            );
            SpriteRenderer img = collectResource.GetComponent<SpriteRenderer>();
            img.sprite = ArtPrefabConf.GetChestSprite(lotteryConf.chest);
            collectResource.transform.position = startPosition;
            if (Type == CollectChestType.collectWithShow) {
                AnimationManager.Animate(collectResource, "Show", finishCallback: () => {
                    AnimationManager.Animate(collectResource, "Move",
                        start: startPosition,
                        target: this.viewPref.btnHero.transform.position + (Vector3)this.chestVector2,
                        finishCallback: () => {
                            AnimationManager.Animate(
                                collectResource, "Hide",
                                finishCallback: () => {
                                    AnimationManager.Animate(
                                        this.viewPref.pnlLottery.gameObject,
                                        "Beat", () => {
                                            if (AfterShowCallback != null) {
                                                AfterShowCallback();
                                            }
                                        }
                                    );
                                    PoolManager.RemoveObject(collectResource);
                                });
                        }, space: PositionSpace.World
                        );
                }
                );
            } else if (Type == CollectChestType.normalCollect) {
                AnimationManager.Animate(collectResource, "Move",
                    start: startPosition,
                    target: this.viewPref.btnHero.transform.position + (Vector3)this.chestVector2
                    , finishCallback: () => {
                        AnimationManager.Animate(collectResource,
                    "Hide", finishCallback: () => {
                        AnimationManager.Animate(this.viewPref.btnFreeChest.gameObject,
                            "Beat", () => {
                                if (AfterShowCallback != null) {
                                    AfterShowCallback();
                                }
                            });
                        PoolManager.RemoveObject(collectResource);
                    });
                    }, space: PositionSpace.World
                );
            } else if (Type == CollectChestType.collectWithDropOut) {
                List<int> DropOutRandomList = new List<int>();
                DropOutRandomList = new List<int>();
                for (int i = 0; i < 7; i++) {
                    DropOutRandomList.Add(i + 10);
                }
                for (int i = 0; i < 7; i++) {
                    DropOutRandomList.Add(i + 20);
                }
                int outIndex = UnityEngine.Random.Range(0, DropOutRandomList.Count);
                int outKey = DropOutRandomList[outIndex];
                AudioManager.Play(
                     string.Format("show_{0}_chest_drop", lotteryConf.material),
                     Poukoute.AudioType.Show, AudioVolumn.High, isAdditive: true
                );
                AnimationManager.Animate(
                    collectResource,
                    "MoveOut" + (outKey % 10).ToString(),
                    start: startPosition,
                    space: PositionSpace.World, isTureOver: 1 == (outKey / 10),
                    finishCallback: () => {
                        StartCoroutine(CollectActionResource(
                            collectResource, AfterShowCallback
                        ));
                    }
               );
            }
        }

        private IEnumerator CollectActionResource(GameObject playChestObj,
            UnityAction AfterShowCallback) {
            yield return new WaitForSeconds(0.5f);
            AnimationManager.Animate(playChestObj, "Move",
                start: playChestObj.transform.position,
                target: this.viewPref.pnlLottery.position
                , finishCallback: (UnityAction)(() => {
                    PoolManager.RemoveObject(playChestObj.gameObject);
                    this.SetChestLimit();
                    AnimationManager.Animate(
                        this.viewPref.btnFreeChest.gameObject,
                        "Beat", () => {
                            if (AfterShowCallback != null) {
                                AfterShowCallback();
                            }
                            this.viewModel.SetTileLimitChat(afterChestCollect);
                            this.afterChestCollect = null;
                        }
                    );
                }), space: PositionSpace.World
            );
        }

        public void SetChestBtnHighlight() {
            FteManager.SetMask(
                this.viewPref.btnFreeChest.transform,
                autoNext: false,
                hasArrow: false,
                isHighlight: true,
                afterCallBack: () => {
                    this.OnBtnHeroClick();
                }
                );
        }
        
        // Button
        public void ShowBtnMail() {
            this.viewPref.btnMail.gameObject.SetActiveSafe(true);
        }

        private bool hasNewMail = false;
        public void SetBtnMail(int number) {
            Transform imgNewPoint = this.viewPref.btnMail.pnlContent.Find("ImgNewPoint");
            this.hasNewMail = (number > 0);
            imgNewPoint.gameObject.SetActiveSafe(hasNewMail);
            this.SetButtonPanelNotice();
            if (hasNewMail) {
                Transform txtNumber = imgNewPoint.Find("TxtAmount");
                txtNumber.GetComponent<TextMeshProUGUI>().text = number.ToString();
            }
        }

        public void SetBtnBuild() {
            Transform imgNewPoint = this.viewPref.btnBuild.pnlContent.Find("ImgNewPoint");
            int number = this.viewModel.GetCanBeBuiltBuildingCount();
            bool hasBuildable = (number > 0);
            imgNewPoint.gameObject.SetActiveSafe(hasBuildable);
            if (hasBuildable) {
                Transform txtNumber = imgNewPoint.Find("TxtAmount");
                txtNumber.GetComponent<TextMeshProUGUI>().text = number.ToString();
            }
        }

        public void SetBtnAutoBattle() {
            if (PlayerPrefs.HasKey(GameConst.AUTO_BATTLE_KEY)) {
                this.viewPref.btnAutoBattle.gameObject.SetActive(true);
                int autoBattle = PlayerPrefs.GetInt(GameConst.AUTO_BATTLE_KEY);
                if (autoBattle == 1) {
                    this.viewPref.txtAutoBattle.text = LocalManager.GetValue(
                        LocalHashConst.on
                    );
                } else {
                    this.viewPref.txtAutoBattle.text = LocalManager.GetValue(
                       LocalHashConst.off
                   );
                }
            } else {
                this.viewPref.btnAutoBattle.gameObject.SetActive(false);
            }
        }

        private void SetButtonPanelNotice() {
            this.viewPref.pnlButtonPanelNotice.gameObject.SetActiveSafe(
                this.hasNewMail || allianceStatusChange);
        }

        private void ShowBtnBuild() {
            this.viewPref.btnBuild.gameObject.SetActiveSafe(true);
        }

        public void SetBtnPayReward(int index) {
            if (index == FirstPayConf.maxLevel) {
                this.viewPref.btnPayReward.gameObject.SetActive(false);
                return;
            }
            this.viewPref.btnPayReward.gameObject.SetActive(true);
            Image imgIcon = this.viewPref.btnPayReward.transform.Find("PnlContent").
                GetComponent<Image>();
            GameObject txtObj = imgIcon.transform.Find("Text").gameObject;
            txtObj.SetActive(index == 0);
            if (index > 0) {
                imgIcon.sprite = ArtPrefabConf.GetSprite(SpritePath.payRewardIconPrefix, "other");
            } else {
                imgIcon.sprite = ArtPrefabConf.GetSprite(SpritePath.payRewardIconPrefix, "first");
            }
        }
        //public void ShowBtnLottery() {
        //    this.viewPref.pnlLottery.gameObject.SetActive(true);
        //}

        public void SetBtnGacha() {
            this.viewPref.imgNewPoint.gameObject.SetActiveSafe(false);
            this.viewPref.imgNewFreePoint.gameObject.SetActiveSafe(false);
            this.viewPref.imgLevelUpPoint.gameObject.SetActiveSafe(false);

            this.HasFreeLottery = this.viewModel.FreeLotteryCount > 0;
            //Debug.LogError("HasFreeLottery " + this.HasFreeLottery);
            if (this.HasFreeLottery) {
                return;
            }
            if (this.viewModel.NewHeroCount > 0) {
                this.viewPref.imgNewPoint.gameObject.SetActiveSafe(true);
                this.viewPref.txtNewNumber.GetComponent<TextMeshProUGUI>().text = this.viewModel.NewHeroCount.ToString();
                return;
            }

            if (this.viewModel.CanLevelUpCount > 0) {
                this.viewPref.imgLevelUpPoint.gameObject.SetActiveSafe(true);
                this.viewPref.txtLevelUpNumber.text = this.viewModel.CanLevelUpCount.ToString();
            }
        }

        public void ShowBtnTribute() {
            //this.viewPref.btnTribute.gameObject.SetActiveSafe(true);
        }

        public void OnMasterAllianceChange() {
            foreach (GameObject tile in this.tileDict.Values) {
                TileView tileView = tile.GetComponent<TileView>();
                if (tileView.Relation == MapTileRelation.master) {
                    this.RefreshTile(tileView.Coordinate);
                }
            }
        }

        public void RefreshTile(Vector2 coordinate) {
            GameObject tileGameObject;
            if (this.tileDict.TryGetValue(coordinate, out tileGameObject)) {
                AnimationManager.Finish(tileGameObject);
                if (this.viewModel.CurrentTile.coordinate == coordinate) {
                    this.DisableAboveUICamera();
                }
                this.CreateTile(coordinate);
            }
        }

        public void RefreshTownHallTroop() {
            GameObject tileGameObject;
            if (this.tileDict.TryGetValue(
                RoleManager.GetRoleCoordinate(),
                out tileGameObject
            )) {
                tileGameObject.GetComponent<TileView>().UpdateTroop();
            }
        }

        public void RefreshTileMonsterInfo(Vector2 coordinate) {
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                Monster monster = this.viewModel.GetMonsterInfo(coordinate);
                if (monster != null && monster.Level > 0) {
                    tileView.ShowMonster(monster.Level);
                } else {
                    tileView.Remove3DHero();
                }
            }
        }

        public void RefreshTileDominationInfo(Vector2 coordinate) {
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                TileView tileView = tile.GetComponent<TileView>();
                Boss boss = this.viewModel.GetBossInfo(coordinate);
                if (boss != null) {
                    tileView.ShowBoss();
                    if (!this.viewModel.bossInfoChangeList.Contains(coordinate)) {
                        this.viewModel.bossInfoChangeList.Add(coordinate);
                    }
                } else {
                    tileView.RemoveBoss();
                    if (this.viewModel.bossInfoChangeList.Contains(coordinate)) {
                        this.viewModel.bossInfoChangeList.Remove(coordinate);
                    }
                }
            }
        }


        public void CreateMarch(EventMarchClient march) {
            Dictionary<string, MarchLineView> marchDict;
            LineDimensionType marchLineType = LineDimensionType.None;
            if (march.playeId == RoleManager.GetRoleId()) {
                marchDict = this.selfMarchDict;
                marchLineType = LineDimensionType.ThreeD;
            } else {
                marchDict = this.otherMarchDict;
                if (marchDict.Count < MARCH_LINE_MAX_3D) {
                    marchLineType = LineDimensionType.ThreeD;
                } else if (marchDict.Count < MARCH_LINE_MAX_2D) {
                    marchLineType = LineDimensionType.TwoD;
                } else if (marchDict.Count < MARCH_LINE_MAX) {
                    marchLineType = LineDimensionType.OneD;
                } else {
                    return;
                }
            }
            if (!marchDict.ContainsKey(march.id)) {
                GameObject marchLine = PoolManager.GetObject(
                    PrefabPath.march, this.March.transform
                );
                MarchLineView marchlineView = marchLine.GetComponent<MarchLineView>();
                Vector3 marchOffet = Vector3.forward * TileView.GetMarchLineLayer();
                //new Vector3(0, 0, TileView.GetMarchLineLayer());
                marchlineView.SetLine(march, this.OnMarchClick, marchOffet, marchLineType);
                marchDict.Add(march.id, marchlineView);
                viewModel.SetQueueIsFold(false);
            }
        }

        private ElementType tileType = ElementType.none;
        private static Vector2 cityCenterCoord = Vector2.zero;
        public void CreateTile(Vector2 coordinate) {
            tileType = this.viewModel.GetTileType(coordinate);
            cityCenterCoord = (tileType == ElementType.npc_city) ?
                this.viewModel.GetCityCenterCoord(coordinate) : Vector2.zero;
            if (cityCenterCoord != Vector2.zero &&
                !this.tileDict.ContainsKey(cityCenterCoord)) {
                this.CreateTile(cityCenterCoord);
            }

            GameObject tile;
            if (!this.tileDict.TryGetValue(coordinate, out tile)) {
                tile = PoolManager.GetObject(PrefabPath.tile,
                    this.Tile.transform, poolType: PoolType.Tile);
                this.tileDict[coordinate] = tile;
            } else {
                tile.GetComponent<TileView>().Clear();
            }

            TileView tileView = tile.GetComponent<TileView>();
            tileView.Coordinate = coordinate;
            tileView.RefreshQueueItemAnimation = this.viewModel.RefreshQueueItemAnimation;
            tileView.BuildTile();
            // Note: can't change the sequence between CreateRelation and CreateTroop.
            tileView.RefreshUpgradableMark();
            tileView.CreateTroop();
            tileView.CreateBuildCure();
            tileView.CreateBattleStatus();
            tileView.onTroopClick = this.viewModel.ShowTileTroop;
            this.RefreshTileMarkInfo(coordinate);
            tileView.SetTileProtection();
            this.SetTileAbandonInfo(tile, coordinate);
            this.SetTileGiveUpBuildingInfo(tile, coordinate);
            this.SetTileUpgradeInfo(tile, coordinate);
            Monster monster = this.viewModel.GetMonsterInfo(coordinate);
            if (monster != null) {
                tileView.ShowMonster(monster.Level);
            }
            Boss boss = this.viewModel.GetBossInfo(coordinate);
            if (boss != null) {
                tileView.ShowBoss();
            }
            if (this.IsEdit) {
                string buildingId = string.Concat(this.viewModel.GetBuildId(), "_", 1);
                tileView.CreateAvaliable(buildingId);
            } else {
                if (this.canCreatRelation) {
                    tileView.CreateRelation();
                }
            }
        }

        public void RefreshBuildCureVisible(bool visible) {
            TileView tileView;
            foreach (GameObject tile in this.tileDict.Values) {
                tileView = tile.GetComponent<TileView>();
                tileView.SetBuildCureVisible(visible);
            }
        }

        private void SetTileProtectedInfo(GameObject tile,
                                          Vector2 coordinate,
                                          TileView tileView) {
        }

        private void SetTileAbandonInfo(GameObject tile, Vector2 coordinate) {
            if (EventManager.IsTileAbandon(coordinate) &&
                !this.abandonDict.ContainsKey(coordinate)) {
                GameObject pnlAbandon = PoolManager.GetObject(PrefabPath.pnlAbandon, this.pnlSlider);
                UIManager.UIBind(pnlAbandon.transform, tile.transform, Vector2.zero,
                    BindDirection.Up, BindCameraMode.None);
                this.abandonDict.Add(coordinate, pnlAbandon);
            }
        }

        private void SetTileGiveUpBuildingInfo(GameObject tile, Vector2 coordinate) {
            if (EventManager.IsTileGiveUpBuilding(coordinate) &&
                !this.giveUpDict.ContainsKey(coordinate)) {
                EventGiveUpBuilding queueBuild = EventManager.GetGiveUpBuildingByCoordinate(coordinate);
                this.CreateBuildingBar(tile, coordinate,
                    this.giveUpDict, queueBuild.id, BuildBarType.GiveUp);
            }
        }

        private void SetTileUpgradeInfo(GameObject tile, Vector2 coordinate) {
            if (EventManager.IsTileInBuildEvent(coordinate) &&
                !this.buildDict.ContainsKey(coordinate)) {
                EventBuildClient queueBuild = EventManager.GetBuildEventByCoordinate(coordinate);
                ElementBuilding building = this.viewModel.GetBuildByName(queueBuild.buildingName);
                BuildBarType barType = BuildBarType.Building;
                if (building != null && building.Level > 0 && !building.IsBroken) {
                    barType = BuildBarType.Upgrade;
                }
                this.CreateBuildingBar(tile, coordinate,
                    this.buildDict, queueBuild.id, barType);
            }
        }

        private void CreateBuildingBar(GameObject tile, Vector2 coordinate,
                                        Dictionary<Vector2, GameObject> dictInfo,
                                        string eventId, BuildBarType barTye) {
            GameObject itemObj = PoolManager.GetObject(PrefabPath.pnlBuildingBar, this.pnlSlider);
            itemObj.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 100, 0);
            BuildingProgressBarView itemView = itemObj.GetComponent<BuildingProgressBarView>();
            itemView.BarType = barTye;
            UIManager.UIBind(
                itemObj.transform,
                tile.transform,
                MapUtils.TileSize,
                BindDirection.Down,
                BindCameraMode.None,
                Vector2.up * 7
            );
            dictInfo.Add(coordinate, itemObj);
        }

        private void ShowBuildProgressBar(Vector2 coordinate) {
            GameObject gameObject;
            if (this.buildDict.TryGetValue(coordinate, out gameObject)) {
                GameObject pnlBuildingBar = this.buildDict[coordinate];
                gameObject.SetActiveSafe(true);
                GameObject tileGameObject;
                if (this.tileDict.TryGetValue(coordinate, out tileGameObject)) {
                    UIManager.UIBind(
                        pnlBuildingBar.transform,
                        tileGameObject.transform,
                        MapUtils.TileSize,
                        BindDirection.Down,
                        BindCameraMode.None,
                        Vector2.up * 7
                    );
                }
            }
        }

        private void HideBuidProgressBar(Vector2 coordinate) {
            GameObject gameObject;
            if (this.buildDict.TryGetValue(coordinate, out gameObject)) {
                gameObject.SetActiveSafe(false);
            }
        }

        private void RemoveTile(Vector2 coordinate) {
            GameObject tile = this.tileDict[coordinate];
            tile.GetComponent<TileView>().Clear();
            this.RemoveTileBindUI(coordinate);
            PoolManager.RemoveObject(tile, PoolType.Tile);
        }

        private void ResetBindUIElement() {
            foreach (GameObject gameObject in this.giveUpDict.Values) {
                this.InnerRemoveGiveupUI(gameObject);
            }
            this.giveUpDict.Clear();

            foreach (GameObject gameObject in this.abandonDict.Values) {
                this.InnerRemoveAbandonUI(gameObject);
            }
            this.abandonDict.Clear();

            foreach (GameObject gameObject in this.buildDict.Values) {
                PoolManager.RemoveObject(gameObject);
            }
            this.buildDict.Clear();
        }

        private void RemoveTileBindUI(Vector2 coordinate) {
            this.RemoveAbandonUI(coordinate);
            this.RemoveGiveupUI(coordinate);
            this.RemoveBuildupUI(coordinate);
            this.RemoveShield(coordinate);
        }

        private void RemoveAbandonUI(Vector2 coordinate) {
            GameObject gameObject;
            if (this.abandonDict.TryGetValue(coordinate, out gameObject)) {
                this.InnerRemoveAbandonUI(gameObject);
                this.abandonDict.Remove(coordinate);
            }
        }


        private void InnerRemoveAbandonUI(GameObject gameObject) {
            UIManager.UIUnBind(gameObject.transform);
            PoolManager.RemoveObject(gameObject);
        }

        private void RemoveBuildupUI(Vector2 coordinate) {
            GameObject gameObject;
            if (this.buildDict.TryGetValue(coordinate, out gameObject)) {
                UIManager.UIUnBind(gameObject.transform);
                PoolManager.RemoveObject(gameObject);
                this.buildDict.Remove(coordinate);
            }
        }

        private void RemoveGiveupUI(Vector2 coordinate) {
            GameObject gameObject;
            if (this.giveUpDict.TryGetValue(coordinate, out gameObject)) {
                this.InnerRemoveGiveupUI(gameObject);
                this.giveUpDict.Remove(coordinate);
            }
        }

        private void InnerRemoveGiveupUI(GameObject gameObject) {
            UIManager.UIUnBind(gameObject.transform);
            PoolManager.RemoveObject(gameObject);
        }

        private void RemoveShield(Vector2 coordinate) {
            GameObject tile;
            if (this.tileDict.TryGetValue(coordinate, out tile)) {
                Transform protectShield = tile.transform.Find(PrefabName.tileProtection);
                if (protectShield) {
                    PoolManager.RemoveObject(protectShield.gameObject);
                }
            }
        }

        private void RefreshTileMarkInfo(Vector2 coordinate) {
            Vector3 markVector = new Vector3(
                coordinate.x,
                coordinate.y,
                0
            );
            for (int i = 1; i < 5; i++) {
                if (this.viewModel.MarkDict.ContainsKey(markVector + Vector3.forward * i)) {
                    this.RefreshMarkInTile(coordinate, (MapMarkType)i, true);
                    break;
                }
            }
        }

        public void RefreshMarkInTile(Vector2 coord, MapMarkType type, bool isAdd) {
            GameObject gameObject;
            if (this.tileDict.TryGetValue(coord, out gameObject)) {
                TileView tileView = gameObject.GetComponent<TileView>();
                if (isAdd) {
                    tileView.CreateMark(type);
                } else {
                    tileView.RemoveMark(type);
                }
            }
        }

        public void RefreshUpgradeableInTile(Vector2 coord) {
            GameObject gameObject;
            if (this.tileDict.TryGetValue(coord, out gameObject)) {
                gameObject.GetComponent<TileView>().RefreshUpgradableMark();
            }
        }


        // To do: update bug when demon diappear reconnect.
        public void UpdateTileTroopStatus(Troop troop) {
            Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
            GameObject gameObject;
            if (this.tileDict.TryGetValue(coordinate, out gameObject)) {
                gameObject.GetComponent<TileView>().UpdateTroop();
            }
        }

        public void UpdateDurabilityState(EventBase eventBase) {
            EventDurabilityRecover eventDurability = eventBase as EventDurabilityRecover;
            GameObject tile;
            if (eventDurability.finishAt < RoleManager.GetCurrentUtcTime() &&
                this.tileDict.TryGetValue(eventDurability.coordinate, out tile)) {
                tile.GetComponent<TileView>().RemoveDurabilityState();
            }
            EventManager.FinishedList.Add(eventBase.id);
        }

        public void UpdateDefenderState(EventBase eventBase) {
            EventDefenderRecover eventDefender = eventBase as EventDefenderRecover;
            GameObject tile;

            if (eventDefender.finishAt < RoleManager.GetCurrentUtcTime() &&
               this.tileDict.TryGetValue(eventDefender.coordinate, out tile)) {
                tile.GetComponent<TileView>().RemoveDefenderState();
            }
            EventManager.FinishedList.Add(eventBase.id);
        }

        public void ShowTributeObjDirectly() {
            if (this.tributeAnim == null) {
                this.CreateTributeObj();
            }
        }

        private void ShowTributeObj() {
            if (this.tributeAnim == null) {
                this.CreateTributeObj();
                this.tributeAnim.SetTrigger("Begin");
            }
        }

        private void CreateTributeObj() {
            GameObject tributeObj = PoolManager.GetObject(PrefabPath.tribute, null);
            this.tributeAnim = tributeObj.GetComponentInChildren<Animator>();
            CustomClick customClick = tributeObj.GetComponent<CustomClick>();
            customClick.onClick.RemoveAllListeners();
            customClick.onClick.AddListener(this.OnBtnTributeClick);
            Vector2 roleCoordinate = RoleManager.GetRoleCoordinate();
            tributeObj.transform.position =
                (Vector3)MapUtils.CoordinateToPosition(roleCoordinate) +
                new Vector3(0, 2.7f, -0.5f);
            this.tributeAnim.transform.Rotate(
                Vector3.up * (UnityEngine.Random.Range(-30, 30) +
                    UnityEngine.Random.Range(0, 2) * 180),
                Space.Self
            );
        }

        public void HideTributeObj() {
            this.tributeAnim.SetTrigger("Finish");
            this.tributeAnim = null;
        }

        private Vector2 CheckBorder(Vector2 delta) {
            Vector3 cameraPosition = GameManager.MainCamera.transform.position;
            Vector2 start = (Vector2)cameraPosition;
            Vector2 end = start + delta;
            Vector2 endCoordinate = MapUtils.PositionToCoordinate(end);

            if (endCoordinate.x >= this.viewModel.MinCoordinate.x &&
                endCoordinate.x <= this.viewModel.MaxCoordinate.x &&
                endCoordinate.y >= this.viewModel.MinCoordinate.y &&
                endCoordinate.y <= this.viewModel.MaxCoordinate.y) {
                this.isOverstep = false;
            } else {
                this.isOverstep = true;
            }

            if (this.isOverstep) {
                if (endCoordinate.x < this.viewModel.MinCoordinate.x) {
                    endCoordinate.x = this.viewModel.MinCoordinate.x;
                } else if (endCoordinate.x > this.viewModel.MaxCoordinate.x) {
                    endCoordinate.x = this.viewModel.MaxCoordinate.x;
                }
                if (endCoordinate.y < this.viewModel.MinCoordinate.y) {
                    endCoordinate.y = this.viewModel.MinCoordinate.y;
                } else if (endCoordinate.y > this.viewModel.MaxCoordinate.y) {
                    endCoordinate.y = this.viewModel.MaxCoordinate.y;
                }
                Vector2 tmpStart = MapUtils.CoordinateToPosition(endCoordinate);
                Vector2 newDistance = end - tmpStart;
                if (newDistance.sqrMagnitude > this.elasticDistance.sqrMagnitude) {
                    delta = Vector2.Lerp(Vector2.zero, delta, 1 / (1 + Mathf.Log(newDistance.sqrMagnitude, 4)));
                }
                this.elasticDistance = (start + delta) - tmpStart;

                this.target = GameManager.MainCamera.transform.position - (Vector3)this.elasticDistance;
                this.targetLerp = Mathf.Max(elasticDistance.sqrMagnitude / 4, elasticity);
            } else {
                this.elasticDistance = Vector2.zero;
            }
            return -delta;
        }

        private void OnEditStateChange() {
            if (this.IsEdit) {
                this.EnableDarkMask();
            } else {
                this.DisableDarkMask();
            }
            foreach (var pair in this.tileDict) {
                Transform tile = pair.Value.transform;
                TileView tileView = tile.GetComponent<TileView>();
                if (this.IsEdit) {
                    tileView.CreateAvaliable(this.viewModel.GetBuildId() + "_" + 1);
                } else {
                    tileView.DestroyAvaliable(this.viewModel.GetBuildId() + "_" + 1);
                }
            }
        }

        private void OnShowMarchRole() {
            int mask = 1 << LayerMask.NameToLayer("LayerMarchRole");
            GameManager.MainCamera.cullingMask ^= mask;
            GameManager.RayCaster.eventMask ^= mask;
        }

        private void OnMarchClick(string id) {
            this.viewModel.HideTileInfo();
            this.IsJumping = false;
            this.viewModel.CurrentMarch = id;
            MarchLineView lineView;
            if (!this.selfMarchDict.TryGetValue(id, out lineView)) {
                this.otherMarchDict.TryGetValue(id, out lineView);
            }
            GameObject marchObj = lineView.transform.Find("March").gameObject;
            StartJumping(jumpLerp, marchObj.transform.position, marchObj);
        }

        public void DeleteMarch(string id) {
            Dictionary<string, MarchLineView> marchLineDict;
            if (this.selfMarchDict.ContainsKey(id)) {
                marchLineDict = this.selfMarchDict;
            } else if (this.otherMarchDict.ContainsKey(id)) {
                marchLineDict = this.otherMarchDict;
            } else {
                return;
            }
            MarchLineView lineView = marchLineDict[id];
            if (this.followObj == lineView.transform.Find("March").gameObject) {
                this.followObj = null;
            }
            marchLineDict.Remove(id);
            lineView.RemoveMarchUI();
            PoolManager.RemoveObject(lineView.gameObject);

            if ((this.selfMarchDict.Count == 0 || this.selfMarchDict == null)
                && (this.otherMarchDict.Count == 0 || this.otherMarchDict == null)) {
                viewModel.SetQueueIsFold(true);
            }
        }

        public void DeleteFarAwayMarch(string id) {
            Dictionary<string, MarchLineView> marchLineDict;
            if (this.selfMarchDict.ContainsKey(id)) {
                marchLineDict = this.selfMarchDict;
            } else if (this.otherMarchDict.ContainsKey(id)) {
                marchLineDict = this.otherMarchDict;
            } else {
                return;
            }
            MarchLineView lineView = marchLineDict[id];
            if (this.followObj != lineView.transform.Find("March").gameObject) {
                Debug.LogError(id);
                if (this.followObj != null) {
                    Debug.LogError(this.followObj.GetHashCode());
                }
                marchLineDict.Remove(id);
                lineView.RemoveMarchUI();
                PoolManager.RemoveObject(lineView.gameObject);
            }
        }

        //public void OnBtnBuildCancelClick(string id) {
        //    this.viewModel.BuildCancelReq(id);
        //}

        // Need a march component.
        public void UpdateMarch(EventBase eventBase) {
            Dictionary<string, MarchLineView> marchLineDict;
            if (this.selfMarchDict.ContainsKey(eventBase.id)) {
                marchLineDict = this.selfMarchDict;
            } else if (this.otherMarchDict.ContainsKey(eventBase.id)) {
                marchLineDict = this.otherMarchDict;
            } else {
                return;
            }
            MarchLineView lineView = marchLineDict[eventBase.id];
            lineView.Refresh(eventBase);
        }

        public void UpdateAbandon(EventBase eventBase) {
            EventAbandonClient eventAbandon = eventBase as EventAbandonClient;
            GameObject abandon;
            if (this.abandonDict.TryGetValue(eventAbandon.coordinate, out abandon)) {
                QueueItemView queueItemView = abandon.GetComponent<QueueItemView>();
                queueItemView.UpdateItem(eventBase);
            }
        }

        public void DeleteAbandon(Vector2 coordinate) {
            GameObject abandon;
            if (this.abandonDict.TryGetValue(coordinate, out abandon)) {
                PoolManager.RemoveObject(abandon);
                UIManager.UIUnBind(abandon.transform);
                this.abandonDict.Remove(coordinate);
            }
        }


        public void UpdateShield(EventBase eventBase) {
            EventShieldClient eventShield = eventBase as EventShieldClient;
            long now = RoleManager.GetCurrentUtcTime();
            long left = eventBase.duration + eventBase.startTime - now;
            if (left <= 0 && !EventManager.FinishedList.Contains(eventBase.id)) {
                EventManager.FinishedList.Add(eventBase.id);
                this.RemoveShield(eventShield.coordinate);
            }
        }

        public void UpdateGiveUpBuilding(EventBase eventBase) {
            EventGiveUpBuilding eventGiveup = eventBase as EventGiveUpBuilding;
            GameObject giveup;
            if (this.giveUpDict.TryGetValue(eventGiveup.coordinate, out giveup)) {
                //GameObject giveup = this.giveUpDict[eventGiveup.coordinate];
                BuildingProgressBarView barView = giveup.GetComponent<BuildingProgressBarView>();

                long now = RoleManager.GetCurrentUtcTime();
                long left = eventBase.duration - now + eventBase.startTime;

                left = (long)Mathf.Max(0, left);
                //System.TimeSpan timeSpan = System.TimeSpan.FromMilliseconds(left);
                barView.Time = GameHelper.TimeFormat(left);
                barView.Value = barView.MaxValue *
                    (1 - (float)left / eventBase.duration);
            }
        }

        public void UpdateBuild(EventBase eventBase) {
            EventBuildClient eventBuild = eventBase as EventBuildClient;
            GameObject build;
            if (this.buildDict.TryGetValue(eventBuild.coordinate, out build)) {
                BuildingProgressBarView barView = build.GetComponent<BuildingProgressBarView>();
                long now = RoleManager.GetCurrentUtcTime();
                long left = eventBase.duration - now + eventBase.startTime;

                left = (long)Mathf.Max(0, left);
                barView.Time = GameHelper.TimeFormat(left);
                barView.Value = barView.MaxValue *
                    (1 - (float)left / eventBase.duration);
            }
        }

        public void UpdateTributeTime(EventBase eventBase) {
            long left = eventBase.startTime + eventBase.duration - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            if (left == 0 && this.viewModel.GetBuildByName("townhall").Level >= 3) {
                this.ShowTributeObj();
                EventManager.FinishedList.Add(eventBase.id);
            }
        }

        public void DeleteBuild(Vector2 coordinate) {
            GameObject gameObject;
            if (this.buildDict.TryGetValue(coordinate, out gameObject)) {
                PoolManager.RemoveObject(gameObject);
                this.buildDict.Remove(coordinate);
            }
        }

        public void EnableChoseEffect(Vector2 coordinate) {
            GameObject gameObject;
            if (this.tileDict.TryGetValue(coordinate, out gameObject)) {
                Transform tile = gameObject.transform;
                this.EnableChoseEffect(tile);
            }
        }

        public void ShowBuldingCompleteEffect(Coord coord) {
            Vector2 coordinate = new Vector2(coord.X, coord.Y);
            if (this.tileDict.ContainsKey(coordinate)) {
                this.tileDict[coordinate].GetComponent<TileView>().SetBuildingCompleteEffect();
            }
        }

        //public void ShowChoseEffect() {
        //    GameObject gameObject;
        //    if (this.tileDict.TryGetValue(
        //        this.viewModel.CurrentTile.coordinate, out gameObject)) {
        //        Transform tile = gameObject.transform;
        //        tile.GetComponent<TileView>().ShowChoseEffect();
        //    }
        //}

        public void EnableSlider() {
            this.cgSlider.alpha = 1;
        }

        public void DisableSlider() {
            this.cgSlider.alpha = 0;
        }

        private bool EnableChoseEffect(Transform tile) {
            return tile.GetComponent<TileView>().EnableChosenEffect();
        }

        public void DisableChosenEffect() {
            Vector2 coordinate = this.viewModel.CurrentTile.coordinate;
            this.ShowTileBindUI(coordinate);
            GameObject gameObject;
            if (this.tileDict.TryGetValue(coordinate, out gameObject)) {
                gameObject.GetComponent<TileView>().DisableChosenEffect();
            } 
        }

        public void EnableHighlight() {
            if (!this.isHighlight) {
                Vector2 coordinate = this.viewModel.CurrentTile.coordinate;
                GameObject gameObject;
                if (this.tileDict.TryGetValue(coordinate, out gameObject)) {
                    TileView tileView = gameObject.GetComponent<TileView>();
                    if (tileView.type == ElementType.npc_city) {
                        GameObject cityObj;
                        if (this.tileDict.TryGetValue(coordinate + tileView.cityOffset, out cityObj)) {
                            TileView cityView = cityObj.GetComponent<TileView>();
                            cityView.ChangeToHighlightLayer();
                        }
                    } else {
                        tileView.ChangeToHighlightLayer();
                    }
                }
                //   this.highlightCamera.gameObject.SetActiveSafe(true);
                this.isHighlight = true;
                this.DisableSlider();
            }
        }

        public void DisableHighlight() {
            if (this.isHighlight) {
                Vector2 coordinate = this.viewModel.CurrentTile.coordinate;
                GameObject gameObject;
                if (this.tileDict.TryGetValue(coordinate, out gameObject)) {
                    TileView tileView = gameObject.GetComponent<TileView>();
                    if (tileView.type == ElementType.npc_city) {
                        GameObject cityObj;
                        if (this.tileDict.TryGetValue(coordinate + tileView.cityOffset, out cityObj)) {
                            TileView cityView = cityObj.GetComponent<TileView>();
                            cityView.ChangeToNormalLayer();
                        }
                    } else {
                        tileView.ChangeToNormalLayer();
                    }
                }
                this.isHighlight = false;
                this.EnableSlider();
            }
        }

        public void EnableAboveUICamera() {
            Vector2 coordinate = this.viewModel.CurrentTile.coordinate;
            GameObject tileObj;
            if (this.tileDict.TryGetValue(coordinate, out tileObj)) {
                TileView tileView = tileObj.GetComponent<TileView>();
                this.aboveUICamera.transform.SetParent(tileView.transform);
                this.aboveUICamera.transform.localPosition = Vector3.forward *
                    this.aboveUICamera.transform.localPosition.z;
                if (tileView.type == ElementType.npc_city) {
                    GameObject cityObj;
                    if (this.tileDict.TryGetValue(coordinate + tileView.cityOffset, out cityObj)) {
                        TileView cityView = cityObj.GetComponent<TileView>();
                        cityView.ChangeToAboveUILayer();
                    }
                } else {
                    tileView.ChangeToAboveUILayer();
                }
            }
        }

        public void DisableAboveUICamera() {
            Vector2 coordinate = this.viewModel.CurrentTile.coordinate;
            GameObject tileObj;
            if (this.tileDict.TryGetValue(coordinate, out tileObj)) {
                TileView tileView = tileObj.GetComponent<TileView>();
                this.aboveUICamera.transform.SetParent(GameManager.MainCamera.transform);
                this.aboveUICamera.transform.localPosition = Vector3.zero;
                if (tileView.type == ElementType.npc_city) {
                    GameObject cityObj;
                    if (this.tileDict.TryGetValue(coordinate + tileView.cityOffset, out cityObj)) {
                        TileView cityView = cityObj.GetComponent<TileView>();
                        cityView.ChangeToNormalLayer();
                    }
                } else {
                    tileView.ChangeToNormalLayer();
                }
            }
        }

        public void EnableDarkMask() {
            this.mapDarkMask.SetActiveSafe(true);
        }

        public void DisableDarkMask() {
            this.mapDarkMask.SetActiveSafe(false);
        }

        private void ShowCloud() {
            MapElementManager.ShowCloud();
        }

        private void HideCloud() {
            MapElementManager.HideCloud();
        }

        public void OnPointerDown(PointerEventData eventData) {
            this.OnBeginDrag(eventData, 0);
        }

        public void OnPointerUp(PointerEventData eventData) {
            this.OnEndDrag(eventData);
        }

        public void OnBeginPinch(PinchEventData eventData) {
            if (!this.isDraggable || !this.isPinchable) {
                return;
            }
            Vector2 pointA = eventData.data[0].position;
            Vector2 pointB = eventData.data[1].position;
            this.oldDistance = pointA - pointB;
            this.originDistance = this.oldDistance;
            this.isPinching = true;
            this.isResizing = false;
        }

        public void OnPinch(PinchEventData eventData) {
            if (!this.isDraggable || !this.isPinchable) {
                return;
            }
            this.IsJumping = false;
#if UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            float size = -eventData.scrollAxis + this.CameraSize;
            this.isResizing = false;
#else
            Vector2 pointA = eventData.data[0].position;
            Vector2 pointB = eventData.data[1].position;
            Vector2 currentDistance = pointA - pointB;
            if (Vector2.Angle(currentDistance, originDistance) > 100) {
                return;
            }
            float size = this.CameraSize * Mathf.Sqrt(this.oldDistance.sqrMagnitude / currentDistance.sqrMagnitude);
            this.oldDistance = currentDistance;
#endif
            float offset = size - this.CameraSize;
            if (size > this.viewModel.CameraInfo.maxSize && offset > 0) {
                size = this.CameraSize + offset /
                    (1 + (size - this.viewModel.CameraInfo.maxSize) * this.pinchdamp);
            } else if (size < this.viewModel.CameraInfo.minSize && offset < 0) {
                size = this.CameraSize + offset /
                    (1 + (this.viewModel.CameraInfo.minSize - size) * this.pinchdamp);
            }

            this.CameraSize = size;
        }

        public void OnEndPinch(PinchEventData eventData) {
            if (this.CameraSize > this.viewModel.CameraInfo.maxSize) {
                this.targetSize = this.viewModel.CameraInfo.maxSize;
                this.isResizing = true;
                this.resizeLerp = 2;
            } else if (this.CameraSize < this.viewModel.CameraInfo.minSize) {
                this.targetSize = this.viewModel.CameraInfo.minSize;
                this.isResizing = true;
                this.resizeLerp = 2;
            }
            this.isPinching = false;
        }

        private void OnBeginDrag(PointerEventData eventData, int threshold) {
            if (!this.isDraggable) {
                return;
            }
            if (threshold > 0) {
                this.SetTileLevelVisible(true);
                this.SetSelfBuildingVisible(true);
                this.HidePopUp();
                this.ShowHUD(isDrag: true);
            } else {
                this.isDragging = true;
                this.IsJumping = false;
                this.preClick = eventData.position;
                this.previouse = eventData.position;
                this.velocity = Vector2.zero;
                this.viewModel.SetTileInfoStatus();
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            this.OnBeginDrag(eventData, 15);
        }

        public void OnDrag(PointerEventData eventData) {
            if (!this.isDraggable) {
                return;
            }
            this.dragDeltaTmp = eventData.position - this.previouse;
            this.dragDelta = (this.viewSize.x / Screen.width) * this.dragDeltaTmp;
            this.previouse = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (!this.isDraggable) {
                return;
            }
            this.SetTileLevelVisible(false);
            this.SetSelfBuildingVisible(false);
            this.isDragging = false;
            if (this.velocity.sqrMagnitude > this.curMaxSpeed * this.curMaxSpeed) {
                this.velocity = this.velocity.normalized * this.curMaxSpeed;
            }
            //// Click judge
            if ((preClick - eventData.position).sqrMagnitude > 64 || !this.isClickable) {
                return;
            }            
            float heightOffset = eventData.position.y - screenCenter.y;
            float widthOffset = eventData.position.x - screenCenter.x;
            endDragOffset.x = widthOffset / Screen.width * viewSize.x;
            endDragOffset.y = heightOffset / Screen.height * viewSize.y;
            Vector2 position = (Vector2)GameManager.MainCamera.transform.position + endDragOffset;
            Vector2 coordinate = MapUtils.PositionToCoordinate(position);
            this.IsJumping = false;
            this.viewModel.OnTileClick(coordinate);
        }

        public void OnEnable() {
            UIManager.ShowUI(this.ui);
        }
        #region FTE

        #endregion

        public void FullPnlStore() {
            AnimationManager.Animate(this.viewPref.pnlStore.gameObject, "Full", null);
            this.viewPref.storeBtnView.SetStoreBtn(true);
        }
    }
}