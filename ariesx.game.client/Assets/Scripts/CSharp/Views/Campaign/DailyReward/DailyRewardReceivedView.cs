using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyRewardReceivedView: BaseView {
        private DailyRewardReceivedViewModel viewModel;
        private DailyRewardReceivedViewPeference viewPref;
        /* UI Members*/
        private string heroName = string.Empty;
        /*************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIDailyRewardReceived");
            this.viewModel = this.gameObject.GetComponent<DailyRewardReceivedViewModel>();
            this.viewPref = this.ui.transform.GetComponent<DailyRewardReceivedViewPeference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            /* Cache the ui components here */
        }

        /* Propert change function */
        public void SetRewardInfo(LoginRewardConf rewardConf) {
            //bool isShow = isReceived && !thisDey;
            AnimationManager.Animate(this.viewPref.pnlReceiveRawImage.gameObject, "Rotate");
            bool isHero = rewardConf.viewName.Contains("hero_fragments");
            this.viewPref.pnlReceivedHero.gameObject.SetActiveSafe(isHero);
            this.viewPref.txtReceivedTitle.text = string.Format(
                LocalManager.GetValue(LocalHashConst.title_login_reward), rewardConf.day);
            if (isHero) {
                this.viewPref.imgReceivedHero.sprite =
                                    ArtPrefabConf.GetSprite(rewardConf.heroName[0] + "_l");
                this.SetHeroRarity(rewardConf);
                this.viewPref.txtReceivedHeroNumber.text = "X" + rewardConf.heroName[1];
                this.viewPref.txtReceivedHeroName.text = LocalManager.GetValue(rewardConf.heroName[0]);
                this.heroName = rewardConf.heroName[0];
                this.viewPref.btnHeroClick.onClick.RemoveAllListeners();
                this.viewPref.btnHeroClick.onClick.AddListener(() => {
                    this.OnShowHeroView(heroName);                   
                });
            }
            bool isResouce = SetResourcesItemInfo(rewardConf);
            bool rewardStatus = this.viewModel.LoginRewardAck.TodayStatus == true;
            this.viewPref.pnlReceivedResouceList.gameObject.SetActiveSafe(isResouce);
            this.viewPref.btnReceiveds.Grayable = rewardStatus;
            if (rewardStatus == false) {
                this.viewPref.btnReceiveds.onClick.RemoveAllListeners();
                this.viewPref.btnReceiveds.onClick.AddListener(this.GetResourceReward);
            }
        }

        public void StopReceiveAmin() {
            AnimationManager.Stop(this.viewPref.pnlReceiveRawImage.gameObject);
        }

        private void SetHeroRarity(LoginRewardConf rewardConf) {
            HeroAttributeConf hero = HeroAttributeConf.GetConf(rewardConf.heroName[0]);
            int i = 0;
            for (; i < hero.rarity; i++) {
                this.viewPref.objReceivedRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.viewPref.objReceivedRarity.Length;
            for (; i < rarityCount; i++) {
                this.viewPref.objReceivedRarity[i].SetActiveSafe(false);
            }
        }

        private bool SetResourcesItemInfo(LoginRewardConf reward) {
            Dictionary<Resource, int> resourceDict = reward.resourceDict;
            this.viewModel.resourceDict = resourceDict;
            int rewardCount = resourceDict.Count;
            Transform resouceList = this.viewPref.pnlReceivedResouceList;
            GameHelper.ResizeChildreCount(resouceList,
                rewardCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView item = null;
            int i = 0;
            foreach (var resourceValue in resourceDict) {
                item = resouceList.GetChild(i++).GetComponent<ItemWithCountView>();
                item.SetResourceInfo(resourceValue.Key, resourceValue.Value);
                this.viewModel.resourceTransformDict[resourceValue.Key] = item.imgItem.transform;
            }
            return rewardCount > 0;
        }

        private void OnReceiveClick() {
            this.OnBtnCloseClick();
            this.viewModel.GetLoginRewardReq();
        }

        private void GetResourceReward() {
            this.viewModel.GetLoginRewardReq();
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnShowHeroView(string heroName) {
            this.viewModel.ShowHeroInfo(heroName);
        }

        /***************************/

    }
}
