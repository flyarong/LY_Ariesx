using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public enum CampaignType {
        melee,//恶魔大乱斗
        occupy,//占地为王
        capture,//州际纷争
        domination,//魔影召唤
        LoginReward,//登陆奖励
        none
    }
    public class CampaignViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private CampaignView view;
        private CampaignModel model;
        /**********************/

        public List<Activity> AllActivities {
            get {
                return this.model.allActivities;
            }
        }

        public int OpenServiceActivityCout {
            get {
                return this.model.OpenServiceActivityCout;
            }
        }

        public Dictionary<string, Texture2D> BannerDict =
            new Dictionary<string, Texture2D>(4);


        public int UsableActivityCount {
            get {
                return this.model.GetUsableActivityCount();
            }
        }

        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
            set {
                if (this.model.chosenActivity != value) {
                    this.model.chosenActivity = value;
                }
                this.OnCurrentActivityChange();
            }
        }
        /* Other members */
        private DevilFightingViewModel devilFightingViewModel;
        private CampaignPreheatViewModel campaignPreheatViewModel;
        private HoldGroundKingViewModel holdGroundKingViewModel;
        private ContinentDisputesViewModel continentDisputesViewModel;
        private DemonShadowViewModel demonShadowViewModel;
        private DailyRewardViewModel dailyRewardViewModel;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<CampaignView>();
            this.model = ModelManager.GetModelData<CampaignModel>();

            this.devilFightingViewModel =
                PoolManager.GetObject<DevilFightingViewModel>(this.transform);
            this.campaignPreheatViewModel =
                PoolManager.GetObject<CampaignPreheatViewModel>(this.transform);
            this.holdGroundKingViewModel =
                PoolManager.GetObject<HoldGroundKingViewModel>(this.transform);
            this.continentDisputesViewModel =
                PoolManager.GetObject<ContinentDisputesViewModel>(this.transform);
            this.demonShadowViewModel =
                 PoolManager.GetObject<DemonShadowViewModel>(this.transform);
            this.dailyRewardViewModel =
                PoolManager.GetObject<DailyRewardViewModel>(this.transform);

            NetHandler.AddDataHandler(typeof(ActivityNtf).Name, this.ActivityNtf);
            this.GetLoginRewardReq();
        }

        public void GetLoginRewardReq() {
            this.dailyRewardViewModel.GetLoginRewardReq();
        }

        public void SetOpenServiceActivityHUD(int count, bool isShow, bool showNotice) {
            this.parent.SetOpenServiceActivityHUD(count, isShow, showNotice);
        }

        public void ShowCampaignAllianceInfoClick(string allianceId) {
            this.parent.ShowAllianceInfo(allianceId);
        }

        public void ShowCapaignPlayerInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void Show(Activity activity = null) {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this, AddOnMap.HideAll);
            }, true);
            this.view.SetActivitiesInfo(activity);

        }

        public void Hide() {
            this.view.PlayHide(this.OnHideCallback);
        }

        private void OnHideCallback() {
            this.HideAllSubViews();
            this.parent.OnRemoveViewAboveMap(this);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        private void HideAllSubViews() {
            this.campaignPreheatViewModel.Hide();
            this.devilFightingViewModel.Hide();
            this.holdGroundKingViewModel.Hide();
            this.continentDisputesViewModel.Hide();
            this.demonShadowViewModel.Hide();
            UpdateManager.Unregist(UpdateInfo.CampaignProcessing);
        }

        #region Campaign logic
        private UnityAction getMonsterAction = null;
        public void GetRecentMonsterByLevel(int level) {
            getMonsterAction = () => {
                this.parent.GetRecentMonsterByLevel(level);
            };

            if (this.view.IsVisible) {
                this.view.afterHideCallback = getMonsterAction;
                this.Hide();
            } else {
                getMonsterAction.Invoke();
            }

        }

        public bool IsDevilFightingFinish(long timeStamp) {
            if (this.ChosenActivity.Base == null) {
                foreach (var Activity in this.AllActivities) {
                    if (Activity.CampaignType == CampaignType.melee) {
                        return (Activity.Base.EndTime < timeStamp);
                    }
                }
            }
            return (this.ChosenActivity.Base.EndTime < timeStamp);
        }

        public void ShowCampaignRewards() {
            this.parent.ShowCampaignRewards();
        }

        public void ShowCampaignRules(string content) {
            this.parent.ShowCampaignRules(content);
        }

        public void ShowLoginRewardReceivedView(
            LoginRewardConf rewardConf) {
            this.parent.ShowDailyRewardReceivedView(rewardConf);
        }

        public void ShowdailyRewardNoRewardView(
            LoginRewardConf rewardConf) {
            this.parent.ShowDailyRewardNoRewardView(rewardConf);
        }

        public void ShowCampaignRewardDomination() {
            this.parent.ShowCampaignRewardDomination();
        }

        public void ShowCampaignBossInfo(BossTroop dominaInfo) {
            this.parent.ShowCampaignBossInfo(dominaInfo);
        }

        public void ShowCampaignCitySelect() {
            this.parent.ShowCampaignCitySelect();
        }

        public void ShowDemonShadowHistoryRank(DominationHistory record) {
            this.parent.ShowDemonShadowHistoryRank(record);
        }

        public void OnCampaignPreheatDone() {
            this.ShowCurrentCampaign(this.ChosenActivity.CampaignType);
        }

        public void ShowHeroInfo(string heroName) {
            this.parent.ShowHeroInfo(heroName, infoType: HeroInfoType.Unlock, isSubWindow: true);
        }

        private void OnCurrentActivityChange() {
            CampaignType campaignsType;
            if (this.ChosenActivity != null) {
                campaignsType = this.ChosenActivity.CampaignType;
                bool isActivityEnd = (this.ChosenActivity.Status == Activity.ActivityStatus.Finish);
                if (!isActivityEnd) {
                    if (ChosenActivity.Status == Activity.ActivityStatus.Preheat) {
                        this.campaignPreheatViewModel.Show(campaignsType);
                        this.ShowCurrentCampaign(CampaignType.none);
                    } else {
                        this.ShowCurrentCampaign(campaignsType);
                        this.campaignPreheatViewModel.Hide();
                    }
                } else {
                    this.ShowEndRank(campaignsType);
                }
            } else {
                campaignsType = CampaignType.LoginReward;
                this.campaignPreheatViewModel.Hide();
                this.ShowCurrentCampaign(campaignsType);
            }
        }


        private void ShowEndRank(CampaignType campaignType) {
            this.ShowCurrentCampaign(campaignType, true);
        }


        private void ShowCurrentCampaign(CampaignType campaignsType, bool EndCampaign = false) {
            this.HideAllCampaign();
            switch (campaignsType) {
                case CampaignType.melee:
                    this.devilFightingViewModel.Show(EndCampaign);
                    break;
                case CampaignType.occupy:
                    this.holdGroundKingViewModel.Show(EndCampaign);
                    break;
                case CampaignType.capture:
                    this.continentDisputesViewModel.Show(EndCampaign);
                    break;
                case CampaignType.domination:
                    this.demonShadowViewModel.Show(EndCampaign);
                    break;
                case CampaignType.LoginReward:
                    this.dailyRewardViewModel.Show(EndCampaign);
                    break;
                default:
                    break;
            }

            if ((campaignsType != CampaignType.LoginReward) &&
                (this.ChosenActivity.Status != Activity.ActivityStatus.Finish)) {
                UpdateManager.Regist(UpdateInfo.CampaignProcessing, this.UpdateAction);
            }
        }

        private void HideAllCampaign() {
            this.continentDisputesViewModel.Hide();
            this.holdGroundKingViewModel.Hide();
            this.demonShadowViewModel.Hide();
            this.dailyRewardViewModel.Hide();
            this.devilFightingViewModel.Hide();
        }

        private void HideCurrentCampaign() {
            switch (this.ChosenActivity.CampaignType) {
                case CampaignType.melee:
                    this.devilFightingViewModel.Hide();
                    break;
                case CampaignType.occupy:
                    this.holdGroundKingViewModel.Hide();
                    break;
                case CampaignType.capture:
                    this.continentDisputesViewModel.Hide();
                    break;
                case CampaignType.domination:
                    this.demonShadowViewModel.Hide();
                    break;
                default:
                    break;
            }
        }

        private long currentTime = 0;
        private void UpdateAction() {
            if (this.ChosenActivity != null) {
                this.currentTime = RoleManager.GetCurrentUtcTime() / 1000;
                if (this.view.IsVisible &&
                    this.currentTime > this.ChosenActivity.Base.EndTime) {
                    UpdateManager.Unregist(UpdateInfo.CampaignProcessing);
                    this.HideCurrentCampaign();
                    this.ShowEndRank(this.ChosenActivity.CampaignType);
                }
            }
        }

        #endregion

        // Net callback
        private void ActivityNtf(IExtensible message) {
            ActivityNtf allActivities = message as ActivityNtf;
            bool hasAnyCampaign = (allActivities.Activity.Count > 0);

            if (hasAnyCampaign) {
                this.AllActivities.Clear();
                this.AllActivities.AddRange(allActivities.Activity);
                this.AllActivities.Sort((a, b) => {
                    if (a.Status == b.Status) {
                        return a.Base.StartTime.CompareTo(b.Base.StartTime);
                    } else {
                        return a.Status.CompareTo(b.Status);
                    }
                });
                if (this.model.GetUsableActivityCount() < 1) {
                    this.DelayOfPrepareActivity();
                } else {
                    this.ResetActiviesInfo();
                }
            }
        }

        private void DelayOfPrepareActivity() {
            CancelInvoke("ResetActiviesInfo");
            foreach (Activity activity in this.AllActivities) {
                if (activity.Melee != null) {
                    CampaignModel.monsterType = activity.Base.SpecialType;
                }
                if (activity.Status == Activity.ActivityStatus.None ||
                    activity.Status == Activity.ActivityStatus.Finish) {
                    Debug.LogError(activity.Status);
                    Invoke("ResetActiviesInfo", activity.ActivityRemainTime);
                }
            }
        }

        private void ResetActiviesInfo() {
            if (this.view.IsVisible) {
                this.view.SetActivitiesInfo(this.AllActivities[0]);
            } else {
                foreach (Activity activity in this.AllActivities) {
                    if (activity.Melee != null) {
                        CampaignModel.monsterType = activity.Base.SpecialType;
                    }
                    string bannerUrl = activity.Base.Banner;
                    this.StartCoroutine(NetManager.SendHttpMessage(
                        bannerUrl, (www) => this.SetBanner(www, bannerUrl)));
                }
            }
        }

        private void SetBanner(WWW www, string url) {
            if (!www.error.CustomIsEmpty()) {
                Debug.LogError("load sprit from " + url + " error:" + www.error);
            } else {
                this.BannerDict[url] = www.texture;
                this.view.SetBanner(www, url);
            }
        }

        public void Move(Vector2 coordinate) {
            this.view.afterHideCallback = () => {
                this.parent.Move(coordinate);
            };
            this.Hide();
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.view.afterHideCallback = () => {
                this.parent.MoveWithClick(coordinate);
            };
            this.Hide();
        }
    }
}
