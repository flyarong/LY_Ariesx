using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;
using Facebook.Unity;

namespace Poukoute {

    public class LoginViewModel : BaseViewModel {
        LoginModel model;
        LoginView view;
        LyLoginViewModel lyLogin;

        private bool isloadScene = false;
        //FacebookLogin facebookLogin;

        public string Name {
            get {
                return this.model.name;
            }
            set {
                if (this.model.name != value) {
                    this.model.name = value;
                }
            }
        }

        public string State {
            get {
                return this.model.state;
            }
            set {
                if (this.model.state != value) {
                    this.model.state = value;
                }
            }
        }

        public List<LoginWorld> LoginWorlds {
            get {
                return this.model.worldList;
            }
        }

        public List<GetBornPointsAck.Status> BornPoints {
            get {
                return this.model.bornPoint;
            }
        }

        private bool blockReq = false;
        private bool playerPointsReq = false;
        private bool heroReq = false;
        private bool dramaReq = false;
        private string facebookId;
        private string userId;

        void Awake() {
            this.model = ModelManager.GetModelData<LoginModel>();
            this.view = this.gameObject.AddComponent<LoginView>();

            this.lyLogin = PoolManager.GetObject<LyLoginViewModel>(this.transform);
            // this.facebookLogin = PoolManager.GetObject<FacebookLogin>(this.transform);

            NetHandler.AddNtfHandler(typeof(FeedBlocksNtf).Name, this.FeedBlocksNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlockMonsterNtf).Name, this.FeedBlockMonsterNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlocksMarchesNtf).Name, this.FeedBlocksMarchesNtf);
            NetHandler.AddNtfHandler(typeof(RelationNtf).Name, this.RelationNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlockDominationNtf).Name, this.FeedBlockDominationNtf);
            GameManager.InitStepD();
            this.LoginTokenReq();
            GameManager.InitStepB();
        }

        public void LoginTokenReq() {
            string url = string.Concat(VersionConst.url, "api/client/accounts/login_with_udid");
            //string FBurl = string.Concat(VersionConst.url, "api/client/accounts/login_with_facebook");
            this.facebookId = PlayerPrefs.GetString("facebookId");

            int enableNewAccount = PlayerPrefs.GetInt("enable_new_account");
            if (enableNewAccount == 1) {
                StartCoroutine(NetManager.SendHttpMessage(url, this.LoginTokenAck,
                    new string[] { "udid", RoleManager.Udid }));
                return;
            }
#if UNITY_EDITOR || GOOGLEPLAY
            if (this.facebookId.CustomIsEmpty()) {
                //Debug.LogError("this.facebookId " + url);
                StartCoroutine(NetManager.SendHttpMessage(url, this.LoginTokenAck,
                    new string[] { "udid", RoleManager.Udid }));
            } else {
                this.WorldReq();
            }
#else
            lyLogin.Login();
#endif
        }

        private void LoginTokenAck(WWW www) {
            if (www.error == null && www.isDone) {
                Dictionary<string, object> dict =
                    (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
                RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
                if (RoleManager.LoginToken.CustomIsEmpty()) {
                    UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_title_net),
                        LocalManager.GetValue(LocalHashConst.net_timeout), () => this.LoginTokenReq(), canHide: false);
                } else {
                    this.WorldReq();
                }
            } else {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_title_net),
                    LocalManager.GetValue(LocalHashConst.login_udid_wrong), () => this.LoginTokenReq(), canHide: false);
            }
        }

        private void AuthCallBack(ILoginResult result) {
            if (FB.IsLoggedIn) {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                this.userId = aToken.UserId;
                string FBurl = string.Concat(VersionConst.url, "api/client/accounts/login_with_facebook");
                StartCoroutine(NetManager.SendHttpMessage(FBurl, this.FBLoginAck,
                   new string[] { "input_token", aToken.TokenString, "user_id", aToken.UserId }));
            } else {
                UIManager.ShowTip(LocalManager.GetValue("facebook_error_loginfaile"), TipType.Error);
                string innerUdid = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString("udid", innerUdid);
                PlayerPrefs.DeleteKey("facebookId");
                this.LoginTokenReq();
            }
        }

        private void FBLoginAck(WWW www) {
            if (www.error != null) {
                UIManager.ShowTip(LocalManager.GetValue("facebook_error_loginfaile"), TipType.Error);
                string innerUdid = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString("udid", innerUdid);
                PlayerPrefs.DeleteKey("facebookId");
                this.LoginTokenReq();
            } else {
                Dictionary<string, object> dict =
                  (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
                if (dict.ContainsKey("errors")) {
                    UIManager.ShowTip(LocalManager.GetValue("facebook_error_seververifyfaile"), TipType.Error);
                    UIManager.ShowTip(LocalManager.GetValue("facebook_error_loginfaile"), TipType.Error);
                    string innerUdid = SystemInfo.deviceUniqueIdentifier;
                    PlayerPrefs.SetString("udid", innerUdid);
                    PlayerPrefs.DeleteKey("facebookId");
                    this.LoginTokenReq();
                } else {
                    RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
                    PlayerPrefs.SetString("loginToken", RoleManager.LoginToken);
                    PlayerPrefs.SetString("facebookId", this.userId);
                    this.LoginTokenReq();
                }
            }
        }

        public void WorldReq() {
            UIManager.UpdateProgress(0.2f);
            string request =
                string.Format(
                    VersionConst.url +
                        "api/client/worlds?client_version={0}&client_agent={1}&client_locale={2}&login_token={3}",
                    Application.version,
                    VersionConst.netGate.ToString(),
                    LocalManager.ServerLanguage,
                    RoleManager.LoginToken
                );
            Debug.Log(RoleManager.LoginToken);
            StartCoroutine(NetManager.SendHttpMessage(request, this.WorldAck));
        }

        private void WorldAck(WWW message) {
            UIManager.UpdateProgress(0.4f);
            if (message.error != null) {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_title_net),
                    string.Concat(LocalManager.GetValue(LocalHashConst.warning_net_problem), message.error),
                    this.WorldReq, null);
                return;
            }
            Dictionary<string, object> dict = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(message.text);
            List<object> worldList = (List<System.Object>)dict["worlds"];
            List<object> roleList = (List<System.Object>)dict["roles"];
            this.model.roleList.Clear();
            this.model.roleList = roleList;
            this.model.allWorldList.Clear();
            this.model.allWorldList = worldList;

#if DEVELOPER
            if (VersionConst.netGate == NetGate.developer || VersionConst.netGate == NetGate.LYtest) {
                this.model.worldList.Clear();
                foreach (Dictionary<string, object> world in worldList) {
                    string name = (string)world["name"];
                    string world_id = (string)world["world_id"];
                    List<string> agent_ips = new List<string>();
                    foreach (object ip in (List<object>)world["agent_ips"]) {
                        agent_ips.Add((string)ip);
                    }
                    LoginWorld loginWorld = new LoginWorld {
                        name = name,
                        world_id = world_id,
                        agent_ips = agent_ips.ToArray()
                    };
                    this.model.worldList.Add(loginWorld);
                }
            }
#else
            if (worldList.Count > 0) {
                Dictionary<string, object> world = null;
                foreach (Dictionary<string, object> child in worldList) {
                    if (((bool)child["fit_login"])) {
                        world = child;
                    }
                }
                if (world == null) {
                    Debug.Log("world == null) ");
                    return;
                }
                string downloadAddr = (string)dict["client_download_address"];
                string version = world["version"].ToString();
                string[] versionArray = version.CustomSplit('.');
                string[] versionArrayClient = Application.version.CustomSplit('.');
                bool maintenance = (bool)world["maintenance"] &&
                    !(bool)dict["maintenance_for_account"];
                List<object> ipList = world["agent_ips"] as List<object>;
                if (maintenance || ipList.Count == 0) {
                    List<object> maintenanceList = world["maintenance_messages"] as List<object>;
                    foreach (object child in maintenanceList) {
                        Dictionary<string, object> childDict = child as Dictionary<string, object>;
                        if (childDict["language"].ToString() == LocalManager.Language) {
                            UIManager.ShowAlert(childDict["body"].ToString());
                            break;
                        }
                    }
                } else if (versionArrayClient[0] != versionArray[0] ||
                    versionArrayClient[1] != versionArray[1]) {
                    UIManager.ShowConfirm(
                        LocalManager.GetValue(LocalHashConst.need_update_title),
                        LocalManager.GetValue(LocalHashConst.need_update),
                        () => {
                            if (downloadAddr.CustomIsEmpty()) {
                                Application.OpenURL("market://details?id=com.poukoute.ariesx");
                            } else {
                                Application.OpenURL(downloadAddr);
                            }
                        }, canHide: false
                    );
                } else {
                    int ipIndex = Random.Range(0, ipList.Count);
                    Debug.Log((string)world["name"]);
                    this.OnBtnWorldClick(
                        world["world_id"].ToString(),
                        ipList[ipIndex].ToString()
                    );
                }
            }
#endif
            this.view.OnWorldChange();
        }

        public void OnBtnWorldClick(string worldId, string ip) {
            Debug.Log("world Id :" + worldId + " Ip :" + ip);
            RoleManager.ip = ip;
            RoleManager.WorldId = worldId;
            NetManager.AddConnectedAction(this.ConnectReq);
            NetManager.ConnectToServer(ip);
        }

        private void ConnectReq() {
            RoleManager.ConnectReq(this.ConnectAck, this.ErrorConnectAck);
        }

        private void ErrorConnectAck(IExtensible message) {
            ErrorAck errorAck = message as ErrorAck;
            NetManager.RemoveConnectedEvent(this.ConnectReq);
#if UNITY_EDITOR
            Debug.LogError("ErrorConnectAck " + errorAck.Error);
#endif
            switch (errorAck.Error) {
                case "server_token_out_time":
                case "server_cannot_find_role":
                    this.facebookId = PlayerPrefs.GetString("facebookId");
                    if (this.facebookId.CustomIsEmpty()) {
                        this.LoginTokenReq();
                    } else {
                        this.FBLogin();
                    }
                    break;
                case "server_is_full":
                    RoleManager.Instance.RandomChooseWorldReq();
                    break;
                default:
                    break;
            }

        }

        private void FBLogin() {
            if (FB.IsInitialized) {
                Debug.Log("alreadyinit");
                FB.LogInWithReadPermissions(
                    new List<string>() { "public_profile", "email", "user_friends" },
                    AuthCallBack
                );
            } else {
                FB.Init(() => {
                    Debug.Log("notinit");
                    UIManager.ShowTip(LocalManager.GetValue("facebook_error_loginagain"), TipType.Error);
                    FB.LogInWithReadPermissions(
                    new List<string>() { "public_profile", "email", "user_friends" },
                    AuthCallBack);
                });
            }
        }

        private void ConnectAck(IExtensible message) {
            NetManager.RemoveConnectedEvent(this.ConnectReq);
            UIManager.UpdateProgress(0.5f);
            ConnectAck connectAck = message as ConnectAck;
            RoleManager.SetIsNewUser(connectAck.IsNew);
            if (connectAck.Homeless) {
                this.GetBornPointsReq();
            } else {
                this.LoginReq(-1);
            }
        }

        private void GetBornPointsReq() {
            GetBornPointsReq getBornPoint = new Protocol.GetBornPointsReq();
            NetManager.SendMessage(getBornPoint,
                typeof(GetBornPointsAck).Name, this.OnGetBornPoint);
        }

        private void OnGetBornPoint(IExtensible message) {
            GetBornPointsAck bornPoint = message as GetBornPointsAck;
            this.BornPoints.Clear();
            this.BornPoints.AddRange(bornPoint.Statuses);
            //this.view.OnBornPointChange();
            StartCoroutine(this.LoadCountry());
        }

        public void LoginReq(int mapSN) {
            RoleManager.LoginReq(mapSN, this.LoginAck, this.ErrorConnectAck);
        }

        private void LoginAck(IExtensible message) {
            UIManager.UpdateProgress(0.7f);
            LoginAck loginAck = message as LoginAck;
            RoleManager.SetRole(loginAck);
            this.FeedBlocksReq();
            this.PlayerPointsReq();
            this.HeroReq();
            this.DramaReq();
            TriggerManager.Invoke(Trigger.Login);
            NetManager.AddConnectedAction(RoleManager.Reconnect);
        }

        private void RelationNtf(IExtensible message) {
            RelationNtf ntf = message as RelationNtf;
            ModelManager.GetModelData<AllianceDetailModel>().Refresh(ntf);
        }

        private void FeedBlocksReq() {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlocksReq feedBlocksReq = map.FeedBlocks();
            this.blockReq = false;
            NetManager.SendMessage(feedBlocksReq, string.Empty, null);
        }

        private void FeedBlocksNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlocksNtf feedBlocksNtf = message as FeedBlocksNtf;
            map.RrefreshPlayerInfo(feedBlocksNtf, null);
            this.blockReq = true;
            this.WaitForLoadMap();
        }

        private void FeedBlockDominationNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlockDominationNtf feedBlockDominationNtf = message as FeedBlockDominationNtf;
            map.DominationInfo(feedBlockDominationNtf, null);
        }

        private void FeedBlockMonsterNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlockMonsterNtf feedBlocksMonsterNtf = message as FeedBlockMonsterNtf;
            map.RefreshMonsterInfo(feedBlocksMonsterNtf, null);
        }

        private void WaitForLoadMap() {
            if (this.blockReq &&
                this.heroReq &&
                this.playerPointsReq &&
                this.dramaReq &&
                !this.isloadScene) {
                this.isloadScene = true;
                base.StartCoroutine(this.LoadMap());
            }
        }

        private void FeedBlocksMarchesNtf(IExtensible message) {
            FeedBlocksMarchesNtf marchesNtf = message as FeedBlocksMarchesNtf;
            foreach (EventMarch eventMarch in marchesNtf.EventMarches) {
                EventManager.AddMarchEvent(eventMarch);
            }
        }

        private void PlayerPointsReq() {
            PlayerPointsReq playerPoints = new PlayerPointsReq();
            this.playerPointsReq = false;
            NetManager.SendMessage(playerPoints,
                typeof(PlayerPointsAck).Name, this.PlayerPointsAck);
        }

        private void PlayerPointsAck(IExtensible message) {
            PlayerPointsAck playerPointsAck = message as PlayerPointsAck;
            RoleManager.SetPointDict(playerPointsAck.Points);
            this.playerPointsReq = true;
            this.WaitForLoadMap();
        }

        private void DramaReq() {
            GetChapterTasksReq req = new GetChapterTasksReq();
            this.dramaReq = false;
            NetManager.SendMessage(req,
                typeof(GetChapterTasksAck).Name, this.DramaAck);
        }

        private void DramaAck(IExtensible message) {
            GetChapterTasksAck ack = message as GetChapterTasksAck;
            ModelManager.GetModelData<DramaModel>().Refresh(ack.ChapterTasks);
            this.dramaReq = true;
            this.WaitForLoadMap();
        }

        private void HeroReq() {
            GetHeroesReq heroReq = new GetHeroesReq();
            this.heroReq = false;
            NetManager.SendMessage(heroReq,
                typeof(GetHeroesAck).Name, this.HeroAck);
        }

        private void HeroAck(IExtensible message) {
            GetHeroesAck getHeroesAck = message as GetHeroesAck;
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            heroModel.Refresh(getHeroesAck);
            this.heroReq = true;
            this.WaitForLoadMap();
        }

        private IEnumerator LoadMap() {
            AsyncOperation async = ModelManager.LoadSceneAsync("SceneMap");
            while (!async.isDone) {
                UIManager.UpdateProgress(0.4f * async.progress + 0.7f, 0.5f);
                yield return null;
            }
        }

        private IEnumerator LoadCountry() {
            ModelManager.UnLoadScene("SceneLogin");
            AsyncOperation async = ModelManager.LoadSceneAsync("SceneCountry");
            while (!async.isDone) {
                yield return null;
            }
        }

        public void ReturnLogin() {
            lyLogin.Login();
        }

        public void SetReturnLoginView() {
            this.view.SetReturnLoginView();
        }

        void OnDestroy() {
            NetHandler.RemoveNtfHandler(typeof(FeedBlocksNtf).Name, this.FeedBlocksNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlockMonsterNtf).Name, this.FeedBlockMonsterNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlocksMarchesNtf).Name, this.FeedBlocksMarchesNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlockDominationNtf).Name, this.FeedBlockDominationNtf);
            NetHandler.RemoveNtfHandler(typeof(RelationNtf).Name, this.RelationNtf);
        }
    }
}
