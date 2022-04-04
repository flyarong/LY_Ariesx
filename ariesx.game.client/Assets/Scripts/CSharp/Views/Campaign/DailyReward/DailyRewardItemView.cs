using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyRewardItemView: MonoBehaviour {
        [SerializeField]
        private CustomButton btnShowContent;
        [SerializeField]
        private TextMeshProUGUI txtDeyOnline;
        [SerializeField]
        private Image imgIsReceive;
        [SerializeField]
        private GameObject objRewardResourceContent;
        [SerializeField]
        private GameObject objRewardHeroContent;
        [SerializeField]
        private Image imgHero;
        [SerializeField]
        private Image imgResource;
        [SerializeField]
        private TextMeshProUGUI txtResourceNumber;
        [SerializeField]
        private GameObject[] objRarity;
        [SerializeField]
        private GameObject pnlReceived;
        [SerializeField]
        private GameObject loginRewardHeroAnim;
        [SerializeField]
        private GameObject imgTodeyAnim;
        
        public UnityEvent OnShowContentClick {
            get {
                this.btnShowContent.onClick.RemoveAllListeners();
                return this.btnShowContent.onClick;
            }
        }

        public void SetToDeyRewardItemInfo(LoginRewardConf loginReward,
             bool isTodey, bool isReceived, bool todayStatus) {            
            CancelInvoke("ShowLoginRewardHeroAnim");
            this.txtDeyOnline.text = string.Format(
                LocalManager.GetValue(LocalHashConst.login_reward_date),
                loginReward.day);
            this.pnlReceived.gameObject.SetActive(isReceived);
            this.imgIsReceive.sprite = isTodey ? ArtPrefabConf.GetSprite("campaign_Login_BGIocn_2") :
                ArtPrefabConf.GetSprite("campaign_Login_BGIocn_1");
            imgTodeyAnim.SetActive(isTodey && !todayStatus);
            bool isHero = loginReward.viewName.Contains("hero_fragments");
            this.objRewardResourceContent.SetActive(!isHero);
            this.objRewardHeroContent.SetActive(isHero);            
            if (isHero) {
                string heroName = loginReward.heroName[0];
                this.imgHero.sprite = ArtPrefabConf.GetSprite(heroName + "_l");
                HeroAttributeConf heroAttribute =
                    HeroAttributeConf.GetConf(heroName);
                InvokeRepeating("ShowLoginRewardHeroAnim",0, Random.Range(4, 6));
                int i = 0;
                for (; i < heroAttribute.rarity; i++) {
                    this.objRarity[i].SetActive(true);
                }
                int rarityCount = this.objRarity.Length;
                for (; i < rarityCount; i++) {
                    this.objRarity[i].SetActive(false);
                }
            } else {
                imgResource.sprite = ArtPrefabConf.GetSprite("item_type_" + loginReward.viewName);
                if (loginReward.viewName.Contains("gem")) {
                    txtResourceNumber.text = loginReward.gem.ToString();
                } else if (loginReward.viewName.Contains("gold")) {
                    txtResourceNumber.text = loginReward.gold.ToString();
                } else {
                    txtResourceNumber.text = string.Empty;
                }
            }
            if (isHero  && isReceived) {
                CancelInvoke("ShowLoginRewardHeroAnim");
                this.loginRewardHeroAnim.SetActive(false);
            }            
        }
        
        private void ShowLoginRewardHeroAnim() {
            this.loginRewardHeroAnim.SetActive(false);
            this.loginRewardHeroAnim.SetActive(true);
        }

    }
}
