using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public enum CountryStatus {
        Full,
        Crowded,
        Average,
        Smooth
    }
    public class CountryViewModel : BaseViewModel {
        private CountryView view;
        private LoginModel loginModel;
        private static CountryViewModel instance;

        public static Camera countryCamera;
        public List<GetBornPointsAck.Status> BornPoints {
            get {
                return this.loginModel.bornPoint;
            }
        }
        public bool isLoginWorld = false;

        private bool blockReq = false;
        private bool playerPointsReq = false;
        private bool heroReq = false;
        private bool dramaReq = false;
        private bool isloadScene = false;
        private float lerp = 8f;
        private static Vector3 countryCameraFinalPos = Vector3.zero;

        private void Awake() {
            instance = this;
            GameManager.MainCamera.enabled = false;
            UIManager.HideUiSplash();
            this.view = this.gameObject.AddComponent<CountryView>();
            this.loginModel = ModelManager.GetModelData<LoginModel>();
            NetHandler.AddNtfHandler(typeof(FeedBlocksNtf).Name, this.FeedBlocksNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlockMonsterNtf).Name, this.FeedBlockMonsterNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlocksMarchesNtf).Name, this.FeedBlocksMarchesNtf);
            NetHandler.AddNtfHandler(typeof(RelationNtf).Name, this.RelationNtf);
            NetHandler.AddNtfHandler(typeof(FeedBlockDominationNtf).Name, this.FeedBlockDominationNtf);
            countryCamera = this.view.GetCountryCamera();
        }

        public Vector3 GetContryCameraFinalPos(Vector3 position) {
            Vector3 countriesPos = countryCamera.transform.position;
            countriesPos.x = Mathf.Clamp(position.x, -7.6f, 7.6f);
            countriesPos.y = Mathf.Clamp(position.y, -11.45f, -4.8f);
            return countriesPos;
        }

        public static void ResetCountryCameraPos(Vector3 position) {
            countryCameraFinalPos = instance.GetContryCameraFinalPos(position);
            UpdateManager.Regist(UpdateInfo.CountryChoose, instance.UpdateAction);
        }

        private void UpdateAction() {
            if ((countryCamera.transform.position - countryCameraFinalPos).sqrMagnitude > 0.01) {
                countryCamera.transform.position = Vector3.Lerp(
                    countryCamera.transform.position,
                    countryCameraFinalPos,
                    lerp * Time.unscaledDeltaTime);
            } else {
                UpdateManager.Unregist(UpdateInfo.CountryChoose);
            }
        }

        public void OnCountryChosen(int countryIndex) {
            if (countryIndex > 8) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.map_can_not_choose), TipType.Info);
            } else if (this.GetCountryLeftCount(countryIndex) == 0) {
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.country_chosen_country_full));
            } else {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_confirm),
                    string.Format(LocalManager.GetValue(LocalHashConst.country_choose_confirm),
                    NPCCityConf.GetMapSNLocalName(countryIndex)),
                    () => this.ChooseBornContry(countryIndex),
                    () => { });
            }
        }

        private void ChooseBornContry(int countryIndex) {
            this.view.HideCountryInfo();
            FteView.SetCloudsUIVisible(true, () => {
                RoleManager.LoginReq(countryIndex, this.LoginAck, this.ErrorConnectAck);
                this.isLoginWorld = true;
            });
            AudioManager.Play("show_cloud_in", AudioType.Show, AudioVolumn.High);
        }

        private int GetCountryLeftCount(int countryIndex) {
            foreach (GetBornPointsAck.Status bornStatus in this.BornPoints) {
                if (bornStatus.MapSN == countryIndex) {
                    return bornStatus.LeftCount;
                }
            }
            return 0;
        }

        private void FeedBlocksReq() {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlocksReq feedBlocksReq = map.FeedBlocks();
            this.blockReq = false;
            NetManager.SendMessage(feedBlocksReq, string.Empty, null);
        }

        private void PlayerPointsReq() {
            PlayerPointsReq playerPoints = new PlayerPointsReq();
            this.playerPointsReq = false;
            NetManager.SendMessage(playerPoints,
                typeof(PlayerPointsAck).Name, this.PlayerPointsAck);
        }

        private void HeroReq() {
            GetHeroesReq heroReq = new GetHeroesReq();
            this.heroReq = false;
            NetManager.SendMessage(heroReq,
                typeof(GetHeroesAck).Name, this.HeroAck);
        }

        private void DramaReq() {
            GetChapterTasksReq req = new GetChapterTasksReq();
            this.dramaReq = false;
            NetManager.SendMessage(req,
                typeof(GetChapterTasksAck).Name, this.DramaAck);
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

        private void PlayerPointsAck(IExtensible message) {
            PlayerPointsAck playerPointsAck = message as PlayerPointsAck;
            RoleManager.SetPointDict(playerPointsAck.Points);
            this.playerPointsReq = true;
            this.WaitForLoadMap();
        }

        private void HeroAck(IExtensible message) {
            GetHeroesAck getHeroesAck = message as GetHeroesAck;
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            heroModel.Refresh(getHeroesAck);
            this.heroReq = true;
            this.WaitForLoadMap();
        }

        private void DramaAck(IExtensible message) {
            GetChapterTasksAck ack = message as GetChapterTasksAck;
            ModelManager.GetModelData<DramaModel>().Refresh(ack.ChapterTasks);
            this.dramaReq = true;
            this.WaitForLoadMap();
        }

        private void FeedBlocksNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlocksNtf feedBlocksNtf = message as FeedBlocksNtf;
            map.RrefreshPlayerInfo(feedBlocksNtf, null);
            this.blockReq = true;
            this.WaitForLoadMap();
        }

        private void FeedBlockMonsterNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlockMonsterNtf feedBlocksMonsterNtf = message as FeedBlockMonsterNtf;
            map.RefreshMonsterInfo(feedBlocksMonsterNtf, null);
        }

        private void FeedBlockDominationNtf(IExtensible message) {
            MapModel map = ModelManager.GetModelData<MapModel>();
            FeedBlockDominationNtf feedBlockDominationNtf = message as FeedBlockDominationNtf;
            map.DominationInfo(feedBlockDominationNtf, null);
        }

        private void RelationNtf(IExtensible message) {
            RelationNtf ntf = message as RelationNtf;
            ModelManager.GetModelData<AllianceDetailModel>().Refresh(ntf);
        }

        private void FeedBlocksMarchesNtf(IExtensible message) {
            FeedBlocksMarchesNtf marchesNtf = message as FeedBlocksMarchesNtf;
            foreach (EventMarch eventMarch in marchesNtf.EventMarches) {
                EventManager.AddMarchEvent(eventMarch);
            }
        }

        private void ErrorConnectAck(IExtensible message) {
            ErrorAck errorAck = message as ErrorAck;
#if UNITY_EDITOR
            Debug.LogError("ErrorConnectAck " + errorAck.Error);
#endif
            if (errorAck.Error.Equals("server_is_full")) {
                RoleManager.Instance.RandomChooseWorldReq();
                return;
            }
            this.isLoginWorld = false;
            UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_title_net),
                LocalManager.GetValue(LocalHashConst.warning_net_problem),
                () => GameManager.RestartGame());
        }

        private void WaitForLoadMap() {
            if (this.blockReq &&
                this.heroReq &&
                this.playerPointsReq &&
                this.dramaReq &&
                !this.isloadScene) {
                this.isloadScene = true;
                StartCoroutine(this.LoadMap());
            }
        }

        private IEnumerator LoadMap() {
            yield return ModelManager.LoadSceneAsync("SceneMap");
            GameManager.MainCamera.enabled = true;
            this.isLoginWorld = false;
        }

        private void OnDestroy() {
            NetHandler.RemoveNtfHandler(typeof(FeedBlocksNtf).Name, this.FeedBlocksNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlockMonsterNtf).Name, this.FeedBlockMonsterNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlocksMarchesNtf).Name, this.FeedBlocksMarchesNtf);
            NetHandler.RemoveNtfHandler(typeof(RelationNtf).Name, this.RelationNtf);
            NetHandler.RemoveNtfHandler(typeof(FeedBlockDominationNtf).Name, this.FeedBlockDominationNtf);
        }

    }
}
