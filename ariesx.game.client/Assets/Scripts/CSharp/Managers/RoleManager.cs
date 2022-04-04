using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
//using Protocol;
using Protocol;

namespace Poukoute {
    public enum Resource {
        None = 1,
        Lumber,
        Steel,
        Marble,
        Food,
        Gold,
        Crystal,
        Gem,
        Chest,
        Fragment,
        TreasureMap,
        Force
    }

    public class Role {
        public List<Point> pointList = new List<Point>(20);
        public Dictionary<Vector2, Point> pointDict = new Dictionary<Vector2, Point>(20);

        public Dictionary<int, FieldFirstDown.Record> fieldFirstDownDict =
            new Dictionary<int, FieldFirstDown.Record>(5);
        //public List<string> lockedHUDList = new List<string>();
        //public PlayerPublicInfo plyerPublickInfo;
        public string masterAllianceId = string.Empty;
        public string masterAllianceName = string.Empty;
        public Alliance OwnAlliance {
            get {
                return this.AllianceDetailModel.allianceDetail.alliance;
            }
        }

        private AllianceRole ownAllianceRole = AllianceRole.None;
        public AllianceRole OwnAllianceRole {
            get {
                return this.ownAllianceRole;
            }
            set {
                this.ownAllianceRole = value;
            }
        }

        private AllianceDetailModel allianceDetailModel;
        private AllianceDetailModel AllianceDetailModel {
            get {
                if (this.allianceDetailModel == null) {
                    this.allianceDetailModel = ModelManager.GetModelData<AllianceDetailModel>();
                }
                return allianceDetailModel;
            }
        }

        /******************************************************************************/
        public void SetPointDict(List<Point> pointList) {
            this.pointDict.Clear();
            foreach (Point point in pointList) {
                this.pointDict.Add(point.Coord, point);
                EventManager.AddShieldEvent(point);
            }
        }

        public void ResetPointList() {
            this.pointList.Clear();
            foreach (Point point in this.pointDict.Values) {
                if (point.Resource != null && point.Building == null) {
                    pointList.Add(point);
                }
            }
            this.pointList.Sort((a, b) => {
                return a.GetLevel().CompareTo(b.GetLevel());
            });
            TriggerManager.Invoke(Trigger.RefreshPlayerPoint);
        }
    }

    public class RoleManager : MonoBehaviour {
        private static RoleManager self;
        private LoginAck loginAck;
        private int loginTimeOutReference = 0;
        private bool IsNewUser;
        private Role role = new Role();
        private int fieldFirstDownRecordMinorLevel = 0;
        private int fieldFirstDownRecordMaxLevel = 1;

        public static RoleManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("RoleManager is not initialized");
                }
                return self;
            }
        }
        private UnityEvent loginAction = new UnityEvent();
        private GameAction allianceAction;
        public static GameAction AllianceAction {
            get {
                return Instance.allianceAction;
            }
            set {
                Instance.allianceAction = value;
            }
        }
        private long timeOffset;
        private string innerUdid;
        public static string Udid {
            get {
                return Instance.innerUdid;
            }
        }

        public static string ip;

        private string worldId;
        public static string WorldId {
            get {
                return Instance.worldId;
            }
            set {
                Instance.worldId = value;
            }
        }

        private string shortId;
        public static string ShortId {
            get {
                return Instance.shortId;
            }
            set {
                Instance.shortId = value;
            }
        }

        private string loginToken = string.Empty;
        public static string LoginToken {
            get {
                //return "0b52e00b-c9fb-42f0-9765-cc3dd9588566";
                return Instance.loginToken;
            }
            set {
                Instance.loginToken = value;
            }
        }

        private string sdkToken = string.Empty;
        public static string SDKToken {
            get {
                return Instance.sdkToken;
            }
            set {
                Instance.sdkToken = value;
            }
        }

        private string code = string.Empty;
        public static string Code {
            get {
                return Instance.code;
            }
            set {
                Instance.code = value;
            }
        }

        [HideInInspector]
        public bool NeedCurrencyAnimation = false;
        [HideInInspector]
        public bool NeedResourceAnimation = false;

        private bool blockReq;
        private bool playerPointsReq;
        private bool heroReq;
        private bool dramaReq;

        //private Vector2 giftTile = Vector2.zero;

        void Awake() {
            self = this;
            this.innerUdid = PlayerPrefs.GetString("udid");
            if (this.innerUdid.CustomIsEmpty()) {
                this.innerUdid = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString("udid", this.innerUdid);
            }
            this.sdkToken = PlayerPrefs.GetString("sdkToken");
            //Debug.Log("***************sdkToken:" + this.sdkToken);
            this.loginToken = PlayerPrefs.GetString("loginToken");
            NetHandler.AddDataHandler(typeof(ResourcesNtf).Name, this.ResourcesNtf);
            NetHandler.AddDataHandler(typeof(ResourcesLimitNtf).Name, this.ResourcesLimitNtf);
            NetHandler.AddDataHandler(typeof(CurrencyNtf).Name, this.CurrencyNtf);
            NetHandler.AddDataHandler(typeof(ForceNtf).Name, this.ForceNtf);
            NetHandler.AddDataHandler(typeof(PointLimitNtf).Name, this.PointLimitNtf);
            NetHandler.AddDataHandler(typeof(FieldFirstDownNtf).Name, this.FieldFirstDownNtf);
        }

        private void ReconnectReq() {
            UIManager.ShowNetCircle();
            RoleManager.ConnectReq(this.ConnectAck, (a) => { GameManager.RestartGame(); }, true);
        }

        private void ConnectAck(IExtensible message) {
            ConnectAck connectAck = message as ConnectAck;
            this.IsNewUser = connectAck.IsNew;
            UIManager.HideNetCircle();
            UIManager.HideWifiAlert();
            LoginReq(-1, this.LoginAck, (a) => GameManager.RestartGame());
        }

        private void CurrencyNtf(IExtensible message) {
            CurrencyNtf currencyNtf = message as CurrencyNtf;
            SetResource(Resource.Gold, currencyNtf.Currency.Gold);
            SetResource(Resource.Gem, currencyNtf.Currency.Gem);
            TriggerManager.Invoke(Trigger.CurrencyChange);
        }

        private void ForceNtf(IExtensible message) {
            ForceNtf forceNtf = message as ForceNtf;
            this.loginAck.Player.Force = forceNtf.Force;
        }

        private void PointLimitNtf(IExtensible message) {
            PointLimitNtf pointLimitNtf = message as PointLimitNtf;
            this.loginAck.PointLimit = pointLimitNtf.PointLimit;
        }

        private void ResourcesNtf(IExtensible message) {
            ResourcesNtf resourcesNtf = message as ResourcesNtf;
            SetResource(Resource.Lumber, resourcesNtf.Resources.Lumber);
            SetResource(Resource.Marble, resourcesNtf.Resources.Marble);
            SetResource(Resource.Steel, resourcesNtf.Resources.Steel);
            SetResource(Resource.Food, resourcesNtf.Resources.Food);
            SetResource(Resource.Crystal, resourcesNtf.Resources.Crystal);

            TriggerManager.Invoke(Trigger.ResourceChange);
        }

        private void ResourcesLimitNtf(IExtensible message) {
            ResourcesLimitNtf resourcesLimitNtf = message as ResourcesLimitNtf;
            SetResourcesLimit(Resource.Lumber, resourcesLimitNtf.ResourcesLimit.Lumber);
            SetResourcesLimit(Resource.Marble, resourcesLimitNtf.ResourcesLimit.Marble);
            SetResourcesLimit(Resource.Steel, resourcesLimitNtf.ResourcesLimit.Steel);
            SetResourcesLimit(Resource.Food, resourcesLimitNtf.ResourcesLimit.Food);
            SetResourcesLimit(Resource.Crystal, resourcesLimitNtf.ResourcesLimit.Crystal);
        }

        public static void ConnectReq(UnityAction<IExtensible> callBack,
            UnityAction<IExtensible> errorCallBack, bool isReconnect = false) {
            int platform = 0;
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    platform = 1;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = 2;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    platform = 3;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    platform = 4;
                    break;
                default:
                    platform = 0;
                    break;
            }

            ConnectReq connectReq = new ConnectReq() {
                WorldId = RoleManager.WorldId,
                LoginToken = RoleManager.LoginToken,
                Platform = platform,
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                ChannelId = VersionConst.channel,
                PackageId = "0",
                SdkVersion = "4.0.4",
                ClientVersion = Application.version,
                Language = LocalManager.SdkLanguage,
                ChannelUserId = RoleManager.Udid
            };
            NetManager.SendMessage(connectReq, typeof(ConnectAck).Name,
                (message) => {
                    Instance.loginTimeOutReference = 0;
                    callBack.InvokeSafe(message);
                }, (message) => {
                    Debug.LogError("connecterror");
                    Instance.loginTimeOutReference = 0;
                    errorCallBack.InvokeSafe(message);
                },
                () => {
                    if (isReconnect && Instance.loginTimeOutReference++ > 6) {
                        GameManager.RestartGame();
                    }
                }
            );
        }

        public void RandomChooseWorldReq() {                                                           //随机炫富，在服务器返回服务器已满时炫富
            string url = string.Concat(VersionConst.url, "api/client/worlds/random_choose_world");
            StartCoroutine(NetManager.SendHttpMessage(url, this.RandomChooseWorldAck,
               new string[] { "login_token", RoleManager.LoginToken }));
        }

        private void RandomChooseWorldAck(WWW www) {
            if (www.error != null) {
                Debug.LogError("RandomChooseWorldAck");
                return;
            }
            UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_title_net),
                    LocalManager.GetValue(LocalHashConst.server_is_full),
                     () => GameManager.RestartGame());
        }

        public static void LoginReq(int mapSN, UnityAction<IExtensible> callBack, UnityAction<IExtensible> errorCallBack) {
            LoginReq loginReq = new LoginReq() {
                MapSN = mapSN
            };

            NetManager.SendMessage(loginReq, typeof(LoginAck).Name,
                callBack, errorCallBack);
        }

        private void LoginAck(IExtensible message) {
            LoginAck loginAck = message as LoginAck;
            if (loginAck.Player.MaxFteStep < 151 && !FteManager.SkipFte) {
                GameManager.RestartGame();
                return;
            }

            RoleManager.SetRole(loginAck);
            this.FeedBlocksReq();
            TriggerManager.Invoke(Trigger.Login);
        }


        private void FeedBlocksReq() {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlocksReq feedBlocksReq = map.FeedBlocks();
            this.blockReq = false;
            NetManager.SendMessage(feedBlocksReq, typeof(FeedBlocksAck).Name, this.FeedBlocksAck);
        }

        private void FeedBlocksAck(IExtensible message) {
            this.blockReq = true;
            this.HeroReq();
        }

        private void HeroReq() {
            GetHeroesReq heroReq = new GetHeroesReq();
            this.heroReq = false;
            NetManager.SendMessage(heroReq, "GetHeroesAck", this.HeroAck);
        }

        private void HeroAck(IExtensible message) {
            GetHeroesAck getHeroesAck = message as GetHeroesAck;
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            heroModel.Refresh(getHeroesAck);
            this.heroReq = true;
            this.PlayerPointsReq();
        }

        private void PlayerPointsReq() {
            PlayerPointsReq playerPoints = new PlayerPointsReq();
            this.playerPointsReq = false;
            NetManager.SendMessage(playerPoints, "PlayerPointsAck", this.PlayerPointsAck);
        }

        private void PlayerPointsAck(IExtensible message) {
            PlayerPointsAck playerPointsAck = message as PlayerPointsAck;
            RoleManager.SetPointDict(playerPointsAck.Points);
            this.playerPointsReq = true;
            this.DramaReq();
        }

        private void DramaReq() {
            GetChapterTasksReq req = new GetChapterTasksReq();
            this.dramaReq = false;
            NetManager.SendMessage(req, typeof(GetChapterTasksAck).Name, this.DramaAck);
        }

        private void DramaAck(IExtensible message) {
            GetChapterTasksAck ack = message as GetChapterTasksAck;
            ModelManager.GetModelData<DramaModel>().Refresh(ack.ChapterTasks);
            this.dramaReq = true;
            this.HideCircle();
        }

        private void HideCircle() {
            if (this.blockReq &&
                this.heroReq &&
                this.playerPointsReq &&
                this.dramaReq) {
                UIManager.HideNetCircle();
                this.loginAction.InvokeSafe();
            }
        }

        public static void SetIsNewUser(bool IsNewUser) {
            Instance.IsNewUser = IsNewUser;
            if (IsNewUser) {
                PlayerPrefs.DeleteKey("FirstOpenForce");
            }
        }


        public static void SetRole(LoginAck loginAck) {
            Instance.loginAck = loginAck;

            //Debug.LogError(loginAck.Player.MaxFteStep);
            //SetMaxFteStepReq req = new SetMaxFteStepReq();
            //req.Step = 151;
            //NetManager.SendMessage(req, "", null);

            // Set origin time.
            Instance.timeOffset = loginAck.Timestamp -
              (long)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds;
            ModelManager.GetModelData<TroopModel>().Refresh(loginAck);
            ModelManager.GetModelData<FteModel>().Refresh(loginAck);
            ModelManager.GetModelData<BuildModel>().Refresh(loginAck);
            ModelManager.GetModelData<HeroModel>().Refresh(loginAck);
            ModelManager.GetModelData<PayModel>().Refresh(loginAck);
            EventManager.AddUTCTimeZeroEvent();
            EventManager.AddUTCTimeZeroAction();
            //Debug.LogError(loginAck.Player.ExtraBuildQueue);
            EventBuildClient.maxQueueCount = loginAck.Player.ExtraBuildQueue + 1;
            EventManager.RefreshEventMarches(loginAck.EventMarches);
            EventManager.RefreshEventBuilds(loginAck.EventBuilds);
            EventManager.RefreshEventRecruits(loginAck.EventRecruits);
            EventManager.RefreshAbandonEvents(loginAck.EventAbandons);
            EventManager.RefreshGiveUpBuildingEvents(loginAck.EventGiveUps);
            EventManager.AddTributeEvent(loginAck.Tribute.Timestamp);
        }

        public static bool IsFteFinished() {
            return Instance.loginAck.FteIsDone;
        }

        public static bool GetTributeStatus() {
            return Instance.loginAck.Tribute.Timestamp * 1000 +
                GameConst.HOUR_MILLION_SECONDS * 3 < RoleManager.GetCurrentUtcTime();
        }

        public static int GetFteMaxStep() {
            return Instance.loginAck.Player.MaxFteStep;
        }

        public static string GetRoleId() {
            if (Instance == null) {
                return string.Empty;
            }
            return Instance.loginAck.Player.Id;
        }

        public static string GetRoleDesc() {
            if (Instance == null) {
                return string.Empty;
            }
            return Instance.loginAck.Player.Desc;
        }

        public static void SetRoleDesc(string desc) {
            if (Instance == null) {
                return;
            }
            Instance.loginAck.Player.Desc = desc;
        }

        public static void ReOrderResource(List<Resource> resourceList) {
            resourceList.Sort((a, b) => ((int)a).CompareTo(((int)b)));
        }

        public static Vector2 GetRoleCoordinate() {
            if (Instance == null || Instance.loginAck == null) {
                return Vector2.one;
            }

            Vector2 coordinate = new Vector2(
                Instance.loginAck.HomeCoord.X,
                Instance.loginAck.HomeCoord.Y
            );
            return coordinate;
        }

        public static Sprite GetRoleAvatarByKey(int avatarKey) {
            string avatarName = PlayerHeroAvatarConf.GetAvatarName(avatarKey.ToString());
            return ArtPrefabConf.GetRoleAvatarSprite(avatarName);
        }

        public static Sprite GetRoleAvatar() {
            if (Instance == null) {
                return null;
            }
            int avatarKey = Instance.loginAck.Player.Avatar;
            string avatarName = PlayerHeroAvatarConf.GetAvatarName(avatarKey.ToString());
            return ArtPrefabConf.GetRoleAvatarSprite(avatarName);
        }

        public static Sprite GetHighDefinitionRoleAvatar() {
            if (Instance == null) {
                return null;
            }
            int avatarKey = Instance.loginAck.Player.Avatar;
            string avatarName = PlayerHeroAvatarConf.GetAvatarName(avatarKey.ToString());
            return ArtPrefabConf.GetRoleHDAvatarSprite(avatarName);
        }

        public static int GetRoleAvatarKey() {
            return Instance.loginAck.Player.Avatar;
        }

        public static void SetRoleAvatar(int avatar) {
            if (Instance == null) {
                return;
            }
            Instance.loginAck.Player.Avatar = avatar;
        }

        public static string GetRoleName() {

            if (Instance == null) {
                return string.Empty;
            }
            return Instance.loginAck.Player.Name;
        }

        public static void SetRoleName(string name) {
            if (Instance == null) {
                return;
            }
            Instance.loginAck.Player.Name = name;
            Instance.loginAck.Player.HadChangeName = true;
        }

        public static bool CanEditUserName() {
            if (Instance == null) {
                return false;
            }
            return !Instance.loginAck.Player.HadChangeName;
        }

        public static int GetNewHeroCount() {
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            return heroModel.NewHeroCount;
        }

        public static int GetOwnedHeroAmount() {
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            return heroModel.heroDict.Count;
        }

        public static int GetBuildedStrongholdNumber() {
            BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
            return buildModel.GetHavedBuilding(ElementName.stronghold);
        }

        public static int GetAllBuildableStrongholdNum() {
            BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
            return buildModel.GetAllBuildableAmount(ElementName.stronghold);
        }

        public static int GetTroopNum() {
            BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
            return buildModel.GetHavedBuilding(ElementName.armycamp);
        }

        public static int GetAllTroopNum() {
            return 8;
        }

        public static bool IsUnderProtection() {
            return RoleManager.GetFreshProtectionFinishAt() >
                   (RoleManager.GetCurrentUtcTime() / 1000);
        }

        public static long GetFreshProtectionFinishAt() {
            return Instance.loginAck.Player.CreatedAt + 48 * 3600;
        }

        //public static void SetLockedHUDList(List<string> lockedHUDList) {
        //    Instance.role.lockedHUDList = lockedHUDList;
        //}

        //public static List<string> GetLockedHUDList() {
        //    return Instance.role.lockedHUDList;
        //}

        public static Dictionary<int, FieldFirstDown.Record> GetFieldRewardDict() {
            return Instance.role.fieldFirstDownDict;
        }

        public static void SetFieldRewardList(List<FieldFirstDown.Record> fieldFirstDownList) {
            foreach (FieldFirstDown.Record record in fieldFirstDownList) {
                Instance.role.fieldFirstDownDict[record.Level] = record;
            }
        }

        public static int GetFDRecordMaxLevel() {
            if (Instance.fieldFirstDownRecordMaxLevel < 1) {
                Instance.SetFDRecordMaxLevel();
            }
            return Instance.fieldFirstDownRecordMaxLevel;
        }

        public static int GetFDRecordMinorLevle() {
            return Instance.fieldFirstDownRecordMinorLevel;
        }

        private void FieldFirstDownNtf(IExtensible message) {
            FieldFirstDownNtf fieldFirstDownNtf = message as FieldFirstDownNtf;
            SetFieldRewardList(fieldFirstDownNtf.FieldFirstDown.Records);
            Instance.SetFDRecordMaxLevel();
        }

        private void SetFDRecordMaxLevel() {
            this.fieldFirstDownRecordMinorLevel = this.fieldFirstDownRecordMaxLevel;
            foreach (var pair in this.role.fieldFirstDownDict) {
                if (pair.Key > this.fieldFirstDownRecordMaxLevel &&
                    pair.Value.IsCollect) {
                    this.fieldFirstDownRecordMaxLevel = pair.Key;
                }
            }
        }

        // Chest max level Base on max level land you own.
        public static int GetCurrentChestLevel() {
            return GetFDRecordMaxLevel();
        }

        public static bool IsResourceCollected(Resource type) {
            switch (type) {
                case Resource.Lumber:
                    return Instance.loginAck.ResourcesCollected.Lumber;
                case Resource.Marble:
                    return Instance.loginAck.ResourcesCollected.Marble;
                case Resource.Food:
                    return Instance.loginAck.ResourcesCollected.Food;
                case Resource.Steel:
                    return Instance.loginAck.ResourcesCollected.Steel;
                case Resource.Gold:
                    return Instance.loginAck.CurrencyCollected.Gold;
                case Resource.Gem:
                    // Test: Show directly
                    return true;
                //return Instance.loginAck.CurrencyCollected.Gem;
                default:
                    Debug.LogError("No such resource " + type);
                    return false;
            }
        }

        public static void SetResourceCollected(Resource type) {
            switch (type) {
                case Resource.Lumber:
                    Instance.loginAck.ResourcesCollected.Lumber = true;
                    break;
                case Resource.Marble:
                    Instance.loginAck.ResourcesCollected.Marble = true;
                    break;
                case Resource.Food:
                    Instance.loginAck.ResourcesCollected.Food = true;
                    break;
                case Resource.Steel:
                    Instance.loginAck.ResourcesCollected.Steel = true;
                    break;
                case Resource.Gold:
                    Instance.loginAck.CurrencyCollected.Gold = true;
                    break;
                case Resource.Gem:
                    Instance.loginAck.CurrencyCollected.Gem = true;
                    break;
                default:
                    Debug.LogError("No such resource " + type);
                    break;
            }
        }

        public static float GetResource(Resource type) {
            switch (type) {
                case Resource.Lumber:
                    return Instance.loginAck.Resources.Lumber;
                case Resource.Marble:
                    return Instance.loginAck.Resources.Marble;
                case Resource.Food:
                    return Instance.loginAck.Resources.Food;
                case Resource.Steel:
                    return Instance.loginAck.Resources.Steel;
                case Resource.Gold:
                    return Instance.loginAck.Player.Currency.Gold;
                case Resource.Crystal:
                    return Instance.loginAck.Resources.Crystal;
                case Resource.Gem:
                    return Instance.loginAck.Player.Currency.Gem;
                default:
                    Debug.LogWarning("No such resource " + type);
                    return 0;
            }
        }

        public static Dictionary<Resource, int> GetPlayerResource() {
            return Instance.loginAck.Resources.GetResourceDict();
        }

        public static void SetResource(Protocol.Resources resources) {
            Instance.loginAck.Resources.Lumber = resources.Lumber;
            Instance.loginAck.Resources.Marble = resources.Marble;
            Instance.loginAck.Resources.Food = resources.Food;
            Instance.loginAck.Resources.Steel = resources.Steel;
            Instance.loginAck.Resources.Crystal = resources.Crystal;
        }

        public static void SetCurrency(Protocol.Currency currency) {
            Instance.loginAck.Player.Currency.Gold = currency.Gold;
            Instance.loginAck.Player.Currency.Gem = currency.Gem;
        }

        //public static void AddResource(Protocol.Resources addResources) {
        //    Instance.loginAck.Resources.Lumber += addResources.Lumber;
        //    Instance.loginAck.Resources.Marble += addResources.Marble;
        //    Instance.loginAck.Resources.Food += addResources.Food;
        //    Instance.loginAck.Resources.Steel += addResources.Steel;
        //    Instance.loginAck.Resources.Crystal += addResources.Crystal;
        //}

        //public static void AddCurrency(Protocol.Currency addCurrency) {
        //    Instance.loginAck.Player.Currency.Gold += addCurrency.Gold;
        //    Instance.loginAck.Player.Currency.Gem += addCurrency.Gem;
        //}

        public static void SetResource(Resource type, int amount) {
            if (Instance.loginAck == null) {
                return;
            }
            switch (type) {
                case Resource.Lumber:
                    Instance.loginAck.Resources.Lumber = amount;
                    break;
                case Resource.Marble:
                    Instance.loginAck.Resources.Marble = amount;
                    break;
                case Resource.Food:
                    Instance.loginAck.Resources.Food = amount;
                    break;
                case Resource.Steel:
                    Instance.loginAck.Resources.Steel = amount;
                    break;
                case Resource.Gold:
                    Instance.loginAck.Player.Currency.Gold = amount;
                    break;
                case Resource.Crystal:
                    Instance.loginAck.Resources.Crystal = amount;
                    break;
                case Resource.Gem:
                    Instance.loginAck.Player.Currency.Gem = amount;
                    break;
                default:
                    Debug.LogError("No such resource " + type);
                    break;
            }
        }

        public static float GetResourceLimit(Resource type) {
            switch (type) {
                case Resource.Lumber:
                    return Instance.loginAck.ResourcesLimit.Lumber;
                case Resource.Marble:
                    return Instance.loginAck.ResourcesLimit.Marble;
                case Resource.Food:
                    return Instance.loginAck.ResourcesLimit.Food;
                case Resource.Steel:
                    return Instance.loginAck.ResourcesLimit.Steel;
                case Resource.Crystal:
                    return Mathf.Infinity;
                case Resource.Gold:
                    return Mathf.Infinity;
                case Resource.Gem:
                    return Mathf.Infinity;
                default:
                    Debug.LogError("No such resource limit " + type);
                    return Mathf.Infinity;
            }
        }

        public static void SetResourcesLimit(Resource type, int limit) {
            switch (type) {
                case Resource.Lumber:
                    Instance.loginAck.ResourcesLimit.Lumber = limit;
                    break;
                case Resource.Marble:
                    Instance.loginAck.ResourcesLimit.Marble = limit;
                    break;
                case Resource.Food:
                    Instance.loginAck.ResourcesLimit.Food = limit;
                    break;
                case Resource.Steel:
                    Instance.loginAck.ResourcesLimit.Steel = limit;
                    break;
                case Resource.Crystal:
                    Instance.loginAck.ResourcesLimit.Crystal = limit;
                    break;
                default:
                    Debug.LogError("No such resource limit " + type);
                    break;
            }
        }

        public static int GetResourcePercent(Resource type, int amount) {
            //float percent = amount / GetResourceLimit(type) * 100;
            return 5;
            //if (percent < 10) {
            //    return 5;
            //} else if (percent < 20) {
            //    return 10;
            //} else {
            //    return 20;
            //}
        }

        public static void SetGoldAmount(int gold) {
            Instance.loginAck.Player.Currency.Gold = gold;
        }

        public static void SetGemAmount(int gem) {
            Instance.loginAck.Player.Currency.Gem = gem;
        }

        #region Alliance info
        public static void SetMasterAllianceInfo(string name, string id) {
            Instance.role.masterAllianceName = name;
            Instance.role.masterAllianceId = id;
        }

        //public static void SetAllianceRole(PlayerPublicInfo playerInfo) {
        //    Instance.role.plyerPublickInfo = playerInfo;
        //    if (playerInfo != null) {
        //        Instance.role.allianceRole = (AllianceRole)playerInfo.AllianceRole;
        //    } else {
        //        Instance.role.allianceRole = AllianceRole.None;
        //    }
        //}

        //public static void SetAllianceExceptMembers(Alliance alliance) {
        //    Instance.role.alliance.Name = alliance.Name;
        //    Instance.role.alliance.Description = alliance.Description;
        //    Instance.role.alliance.MapSN = alliance.MapSN;
        //    Instance.role.alliance.Emblem = alliance.Emblem;
        //    Instance.role.alliance.Exp = alliance.Exp;
        //    Instance.role.alliance.JoinCondition = alliance.JoinCondition;
        //}

        public static Alliance GetAlliance() {
            return Instance.role.OwnAlliance;
        }

        public static string GetAllianceName() {
            if (Instance.role.OwnAlliance == null) {
                return string.Empty;
            }
            return Instance.role.OwnAlliance.Name;
        }

        public static AllianceRole GetAllianceRole() {
            return Instance.role.OwnAllianceRole;
        }

        public static void ResetOwnAllianceRole(int role) {
            Instance.role.OwnAllianceRole = (AllianceRole)role;
        }

        public static int GetAllianceLogoId() {
            if (Instance.role.OwnAlliance == null) {
                return -1;
            }
            return Instance.role.OwnAlliance.Emblem;
        }

        public static string GetAllianceId() {
            if (Instance.role.OwnAlliance != null) {
                return Instance.role.OwnAlliance.Id;
            } else {
                return string.Empty;
            }
        }
        #endregion

        public static Dictionary<Vector2, Point> GetPointDict() {
            return Instance.role.pointDict;
        }

        public static List<Point> GetPointList() {
            return Instance.role.pointList;
        }

        public static Vector2 GetOwnMinLevelPoint() {
            int pointCount = Instance.role.pointList.Count;
            if (pointCount < 1) {
                return MapUtils.PositionToCoordinate(GameManager.MainCamera.transform.position);
            }
            int index = 0;
            int minLevle = Instance.role.pointList[0].GetMinLevel();
            int pointLevel = -1;
            for (int i = 1; i < pointCount; i++) {
                pointLevel = Instance.role.pointList[i].GetMinLevel();
                if (minLevle < pointLevel) {
                    minLevle = pointLevel;
                    index = i;
                }
            }
            return Instance.role.pointList[index].Coord;
        }

        public static bool IsNewPlayer() {
            return Instance.IsNewUser;
        }

        public static Point GetRolePoint(Vector2 coord) {
            Point point = null;
            Instance.role.pointDict.TryGetValue(coord, out point);
            return point;
        }

        public static string GetMasterAllianceName() {
            if (!string.IsNullOrEmpty(Instance.role.masterAllianceName)) {
                return Instance.role.masterAllianceName;
            }
            return string.Empty;
        }

        public static string GetMasterAllianceId() {
            if (!string.IsNullOrEmpty(Instance.role.masterAllianceName)) {
                return Instance.role.masterAllianceId;
            }
            return string.Empty;
        }

        public static int GetPointsLimit() {
            // To do
            return Instance.loginAck.PointLimit;
        }

        public static long GetWorldOpenAt() {
            return Instance.loginAck.WorldAt;

        }

        public static void SetPointDict(List<Point> pointList) {
            Instance.role.SetPointDict(pointList);
            Instance.role.ResetPointList();
        }

        public static void RemovePoint(Vector2 coordinate) {
            if (Instance.role.pointDict.ContainsKey(coordinate)) {
                Instance.role.pointDict.Remove(coordinate);
                Instance.role.ResetPointList();
            };
        }

        public static void RefreshPoint(Point point) {
            //Vector2 coordinate = new Vector2(point.Coord.X, point.Coord.Y);
            Instance.role.pointDict[point.Coord] = point;
            Instance.role.ResetPointList();
        }

        public static bool IsPointLimitReached() {
            return Instance.role.pointDict.Count >= Instance.loginAck.PointLimit;
        }

        public static int GetForce() {
            return Instance.loginAck.Player.Force;
        }

        //public static void SetForce(int force) {
        //    Instance.loginAck.Player.Force = force;
        //}

        public static string GetRegionLocal() {
            // to do : get region name
            return NPCCityConf.GetMapSNLocalName(Instance.loginAck.Player.MapSN);
        }

        public static int GetMapSN() {
            return Instance.loginAck.Player.MapSN;
        }

        public static long GetCurrentUtcTime() {
            return Instance.timeOffset +
                (long)System.DateTime.UtcNow.Subtract(GameConst.ORIGIN_TIME).TotalMilliseconds;
        }

        public static long GetCurrentLocalTime() {
            return Instance.timeOffset +
                (long)System.DateTime.Now.Subtract(GameConst.ORIGIN_TIME).TotalMilliseconds;
        }

        public static long GetLocalTime(long timeStamp) {
            return timeStamp * 1000 + GameConst.ZONE_TIME;
        }

        public static long GetZeroTime(int offset = 0) {
            return (GetCurrentUtcTime() - offset) / GameConst.DAY_MILLION_SECONDS *
                GameConst.DAY_MILLION_SECONDS + offset * GameConst.HOUR_MILLION_SECONDS;
        }

        public static long GetNextZeroTime(int offset = 0) {
            return ((GetCurrentUtcTime() - offset) / GameConst.DAY_MILLION_SECONDS + 1) *
                GameConst.DAY_MILLION_SECONDS + offset * GameConst.HOUR_MILLION_SECONDS;
        }

        public static void Reconnect() {
            Instance.ReconnectReq();
        }

        public static void AddLoginAction(UnityAction action) {
            if (self != null) {
                Instance.loginAction.AddListener(action);
            }
        }

        public static void RemoveLoginAction(UnityAction action) {
            if (self != null) {
                Instance.loginAction.RemoveListener(action);
            }
        }

        private PayModel payModel = null;
        public DailyShop GetDailyShop() {
            if (payModel == null) {
                this.payModel = ModelManager.GetModelData<PayModel>();
            }
            return this.payModel.dailyShop;
        }
    }
}
