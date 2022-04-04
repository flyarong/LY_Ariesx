using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
	public class DailyRewardNoRewardView : BaseView {
		private DailyRewardNoRewardViewModel viewModel;
		private DailyRewardNoRewardViewPeference viewPref;
		/* UI Members*/

		/*************/
        
		protected override void OnUIInit() {
			this.ui = UIManager.GetUI("UIDailyRewardNoReward");
            this.viewPref = this.ui.transform.GetComponent<DailyRewardNoRewardViewPeference>();            
            this.viewModel = this.gameObject.GetComponent<DailyRewardNoRewardViewModel>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnReceiveds.onClick.AddListener(this.OnBtnCloseClick);
            /* Cache the ui components here */
        }



        /* Propert change function */

        public void SetRewardViewInfo(LoginRewardConf rewardConf) {
            bool isHero = rewardConf.viewName.Contains("hero_fragments");
            this.viewPref.pnlHero.gameObject.SetActiveSafe(isHero);
            this.viewPref.txtTitle.text = string.Format(
                LocalManager.GetValue(LocalHashConst.title_login_reward), rewardConf.day);
            if (isHero) {
                this.viewPref.imgHero.sprite =
                                    ArtPrefabConf.GetSprite(rewardConf.heroName[0] + "_l");
                this.SetHeroRarity(rewardConf);
                this.viewPref.txtHeroNumber.text = "X" + rewardConf.heroName[1];
                this.viewPref.txtHeroName.text = LocalManager.GetValue(rewardConf.heroName[0]);
                this.viewPref.btnHeroClick.onClick.RemoveAllListeners();
                this.viewPref.btnHeroClick.onClick.AddListener(()=> {
                    this.ShowHeroInfo(rewardConf.heroName[0]);
                });
            }
            bool isResouce = SetResourcesItemInfo(rewardConf);
            this.viewPref.pnlResouceList.gameObject.SetActiveSafe(isResouce);
        }

        private void ShowHeroInfo(string heroName) {
            this.viewModel.ShowHeroInfo(heroName);
        }

        private void SetHeroRarity(LoginRewardConf rewardConf) {
            HeroAttributeConf hero = HeroAttributeConf.GetConf(rewardConf.heroName[0]);
            int i = 0;
            for (; i < hero.rarity; i++) {
                this.viewPref.objRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.viewPref.objRarity.Length;
            for (; i < rarityCount; i++) {
                this.viewPref.objRarity[i].SetActiveSafe(false);
            }
        }

        private bool SetResourcesItemInfo(LoginRewardConf reward) {
            Dictionary<Resource, int> resourceDict = reward.resourceDict;
            int rewardCount = resourceDict.Count;
            Transform resouceList = this.viewPref.pnlResouceList;
            GameHelper.ResizeChildreCount(resouceList,
                rewardCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView item = null;
            int i = 0;
            foreach (var resourceValue in resourceDict) {
                item = resouceList.GetChild(i++).GetComponent<ItemWithCountView>();
                item.SetResourceInfo(resourceValue.Key, resourceValue.Value);
            }
            return rewardCount > 0;
        }

        /***************************/

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
