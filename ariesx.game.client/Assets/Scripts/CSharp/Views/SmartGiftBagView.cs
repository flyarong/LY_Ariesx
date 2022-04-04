using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using ProtoBuf;

namespace Poukoute {
    public class SmartGiftBagView : BaseView {
        private SmartGiftBagViewModel viewModel;
        private SmartGiftBagViewPreference viewPref;
        /* UI Members*/

        /*************/
        public string FragmentSliderText {
            get {
                return this.viewPref.txtFragment.text;
            }
            private set {
                this.viewPref.txtFragment.text = value;
            }
        }

        public float Percent {
            get {
                return this.viewPref.sliFragment.value / this.viewPref.sliFragment.maxValue;
            }
            private set {
                this.viewPref.sliFragment.value = value * this.viewPref.sliFragment.maxValue;
            }
        }
        void Awake() {
            this.viewModel = this.gameObject.GetComponent<SmartGiftBagViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UISmartGiftBag");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<SmartGiftBagViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.background.onClick.AddListener(this.OnCloseBtnClick);

        }

        /* Propert change function */

        /***************************/
        private void SetHero() {
            this.viewPref.btnPay.onClick.RemoveAllListeners();
            this.viewPref.btnPay.onClick.AddListener(this.OnClickPay);
            this.viewPref.txtTitle.text = HeroAttributeConf.GetLocalName(this.viewModel.heroConf.HeroName);
            if (RoleManager.GetResource(Resource.Gold) < this.viewModel.heroConf.GoldPrice) {
                this.viewPref.txtPrice.text = "<color=#D62C2CFF>" + this.viewModel.heroConf.GoldPrice.ToString() + "</color>";
            } else {
                this.viewPref.txtPrice.text = this.viewModel.heroConf.GoldPrice.ToString();
            }
            this.viewPref.txtNum.text = "X"+this.viewModel.heroConf.FragmentCount.ToString();
            this.viewPref.txtHerName.text = HeroAttributeConf.GetLocalName(this.viewModel.heroConf.HeroName);
            this.viewPref.ImgAniHero.sprite = this.viewPref.imgHero.sprite = ArtPrefabConf.GetSprite(this.viewModel.heroConf.HeroName + "_l");
            this.viewPref.imgHero.material = null;
            int heroLevel = 1;
            string heroName = null;
            int fragment = 0;

            if (HeroModel.Instance.heroDict.ContainsKey(this.viewModel.heroConf.HeroName)) {
                heroLevel = HeroModel.Instance.heroDict[this.viewModel.heroConf.HeroName].Level;
                heroName = HeroModel.Instance.heroDict[this.viewModel.heroConf.HeroName].Name;
                fragment = HeroModel.Instance.heroDict[this.viewModel.heroConf.HeroName].FragmentCount;
                SetFragment(heroName, heroLevel, fragment);
                this.isOneLevel = 1 == HeroModel.Instance.heroDict[this.viewModel.heroConf.HeroName].Level;
            } else {
                this.viewPref.imgTierUp.gameObject.SetActiveSafe(false);
                this.FragmentSliderText = "0/0";
                this.Percent = 0;
                this.isOneLevel = true;
            }
            Debug.Log("this.isOneLevel:" + this.isOneLevel);
            SetHeroRarityInfo(this.viewModel.heroConf.HeroName);
            this.viewPref.ImgAniHero.gameObject.SetActiveSafe(false);
            this.viewPref.btnPay.gameObject.SetActiveSafe(true);
            this.viewPref.txtPayed.gameObject.SetActiveSafe(false);
            this.viewPref.pnlTip.gameObject.SetActiveSafe(false);
            StartCoroutine(RestartHLG());
        }
        private IEnumerator RestartHLG() {
            yield return new WaitForFixedUpdate();
            this.viewPref.btnPay.GetComponent<HorizontalLayoutGroup>().enabled = false;
            this.viewPref.btnPay.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        public void PayedSmartGiftBag() {
            this.viewPref.txtPrice.text = LocalManager.GetValue(LocalHashConst.shop_have_bought);
            this.viewPref.txtTip.text = string.Format(LocalManager.GetValue(LocalHashConst.smart_gift_hero_collect),
                HeroAttributeConf.GetLocalName(this.viewModel.heroConf.HeroName));
            this.viewPref.btnPay.onClick.RemoveAllListeners();
            this.viewPref.btnPay.onClick.AddListener(this.OnClickPayed);
            this.viewPref.btnPay.gameObject.SetActiveSafe(false);
            this.viewPref.txtPayed.gameObject.SetActiveSafe(true);
            this.viewPref.ImgAniHero.gameObject.SetActiveSafe(true);
            this.viewPref.pnlTip.gameObject.SetActiveSafe(true);
            this.viewPref.imgHero.material = PoolManager.GetMaterial(MaterialPath.matGray);
            AnimationManager.Animate(this.viewPref.txtPayed.gameObject, "Show");
            AnimationManager.Animate(this.viewPref.ImgAniHero.gameObject, "Show", () => {
                this.viewPref.ImgAniHero.transform.localPosition = Vector3.zero;
            });
            AnimationManager.Animate(this.viewPref.pnlTip, "Show", () => {
                AnimationManager.Animate(this.viewPref.pnlTip, "Hide", () => {
                    this.viewPref.pnlTip.transform.localPosition = Vector3.zero;
                });
            });
            UpdateManager.Regist(UpdateInfo.GetSmartGiftBagHero, this.UpdateAction);
        }

        int curFragmentCount = 0;
        private void UpdateAction() {
            Debug.Log(this.viewModel.currentFragmentCount);
            Debug.Log(this.viewModel.targetFragmentCount);
            this.viewModel.currentFragmentCount = Mathf.Lerp(this.viewModel.currentFragmentCount, this.viewModel.targetFragmentCount,
               3 * Time.unscaledDeltaTime);
            if (this.reachMaxLevel) {
                this.Percent = 1f;
                this.FragmentSliderText = Mathf.RoundToInt(this.viewModel.currentFragmentCount).ToString();
            } else if (this.isOneLevel) {
                Debug.Log("isOneLevel");
                if (Mathf.RoundToInt(this.viewModel.currentFragmentCount) > curFragmentCount) {
                    AnimationManager.Animate(this.viewPref.sliFragment.gameObject, "Beat", needRestart: true);
                    StartCoroutine(Flash());
                    curFragmentCount = Mathf.RoundToInt(this.viewModel.currentFragmentCount);
                }
                this.Percent = this.viewModel.currentFragmentCount / this.viewModel.targetFragmentCount;
                this.FragmentSliderText = string.Concat((Mathf.RoundToInt(this.viewModel.currentFragmentCount) + 1).ToString(), "/",
                    heroFragments + 1);
                this.viewPref.imgTierUp.gameObject.SetActiveSafe(this.viewModel.currentFragmentCount > this.heroFragments);
            } else {
                if (Mathf.RoundToInt(this.viewModel.currentFragmentCount) > curFragmentCount) {
                    AnimationManager.Animate(this.viewPref.sliFragment.gameObject, "Beat", needRestart: true);
                    StartCoroutine(Flash());
                    curFragmentCount = Mathf.RoundToInt(this.viewModel.currentFragmentCount);
                }
                this.Percent = this.viewModel.currentFragmentCount / this.viewModel.targetFragmentCount;
                this.FragmentSliderText = string.Concat(Mathf.RoundToInt(this.viewModel.currentFragmentCount).ToString(), "/",
                    heroFragments);
                this.viewPref.imgTierUp.gameObject.SetActiveSafe(this.viewModel.currentFragmentCount > this.heroFragments);
            }
            if (this.viewModel.targetFragmentCount - this.viewModel.currentFragmentCount <= 0.1f) {
                UpdateManager.Unregist(UpdateInfo.GetSmartGiftBagHero);
            }
        }

        private IEnumerator Flash() {
            Image image = this.viewPref.imgFlash.GetComponent<Image>();
            image.material.SetFloat("_Exposure", 0.8f);
            yield return YieldManager.GetWaitForSeconds(0.1f);
            image.material.SetFloat("_Exposure", 0);
            yield return YieldManager.GetWaitForSeconds(0.1f);
        }

        private void OnClickPayed() {
            StartCoroutine(ShowTip());
        }


        int heroFragments = 1;
        bool reachMaxLevel = false;
        bool isOneLevel = false;
        private void SetFragment(string heroName, int heroLevel, int fragment) {
            heroFragments = HeroLevelConf.GetHeroUpgradFragments(heroName, heroLevel);
            reachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(heroName, heroLevel);
            this.viewPref.imgMaxLevel.gameObject.SetActiveSafe(reachMaxLevel);
            int fragmentCount = fragment;
            if (reachMaxLevel) {
                this.FragmentSliderText = fragmentCount.ToString();
                this.Percent = 1.0f;
            } else {
                if (isOneLevel) {
                    this.FragmentSliderText = string.Concat(fragmentCount + 1, "/", heroFragments + 1);
                } else {
                    this.FragmentSliderText = string.Concat(fragmentCount, "/", heroFragments);
                }
                this.Percent = fragmentCount / (heroFragments * 1.0f);
            }
            bool canLevelUp = fragmentCount >= heroFragments && !reachMaxLevel;
            this.viewPref.imgTierUp.gameObject.SetActiveSafe(canLevelUp);

            if (this.viewModel.targetFragmentCount >= this.heroFragments && !this.reachMaxLevel) {
                this.viewPref.imgFill.sprite =
                ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix + "hightlightgreen");
                this.viewPref.imgTierUp.sprite =
                ArtPrefabConf.GetSprite("upgrade_arrow_green");
            } else {
                this.viewPref.imgFill.sprite =
               ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix + "hightlightblue");
                this.viewPref.imgTierUp.sprite =
                ArtPrefabConf.GetSprite("upgrade_arrow_blue");
            }
        }

        private void SetHeroRarityInfo(string heroName) {
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(heroName);
            int i = 0;
            for (; i < heroAttribute.rarity; i++) {
                this.viewPref.heroRarity[i].gameObject.SetActiveSafe(true);
            }
            int rarityCount = this.viewPref.heroRarity.Length;
            for (; i < rarityCount; i++) {
                this.viewPref.heroRarity[i].gameObject.SetActiveSafe(false);
            }
        }

        public override void PlayShow() {
            base.PlayShow(() => {
                SetHero();
            });

        }

        public override void PlayHide() {
            base.PlayHide();
        }

        protected override void OnVisible() {
        }

        protected override void OnInvisible() {
            UpdateManager.Unregist(UpdateInfo.GetSmartGiftBagHero);
        }

        private void OnClickPay() {
            this.viewModel.GetDailyShopReq();
        }

        public IEnumerator ShowTip() {
            this.viewPref.pnlTip.SetActive(true);
            AnimationManager.Animate(this.viewPref.pnlTip, "Show");
            yield return YieldManager.GetWaitForSeconds(4);
            this.viewPref.pnlTip.SetActive(false);
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide();
        }
    }
}
