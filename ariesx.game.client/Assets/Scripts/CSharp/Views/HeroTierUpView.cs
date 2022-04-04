using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Poukoute {

    public class HeroTierUpView : BaseView {
        private HeroTierUpViewModel viewModel;
        private HeroTierUpViewPreference viewPref;

        private Transform pnlHeroLevel;
        private TextMeshProUGUI txtLevel;
        private Transform pnlPower;
        private TextMeshProUGUI txtPower;

        private GameObject upgrade;
        private GameObject card;
        private ChestCardView cardView;

        private Transform attributesIcon;
        private Transform attributesNumberIcon;
        private Transform attributesNumberTxt;
        private Transform attributesAddNumber;

        private Vector3 attrIconStart = new Vector3(-8, -38, 0);

        private GameObject[] pnlTierUpAttrItem;
        // power
        private bool isChanging = false;
        private float current = 0;

        private float smoothSpeed = 0f;

        // tmp
        public static GameObject tmpUpgrade;

        private float target = 0;
        private float add = 0;
        //private int animCount = -1;
        private bool isOneStepClick = false;
        //private Vector3 animNameVector = new Vector3(375, -353, 0);
        // private Vector3 AnimFragment = new Vector3(375, -379, 0);
        private Vector3 animLevelVector = new Vector3(-221, -180, 0);
        Color color = Color.white;
        Color levelColor = Color.green;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HeroTierUpViewModel>();
            UpdateManager.Regist(UpdateInfo.HeroTierUpView, this.UpdateAction);
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITierUp");
            this.group = UIGroup.Hero;
            this.viewPref = this.ui.transform.GetComponent<HeroTierUpViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            //this.viewPref.heroInfoVerticalLayoutGroup.enabled = true;
            this.pnlHeroLevel = UIManager.GetUI("PnlHeroLevelup", UICanvas.Above).transform;
            this.txtLevel = this.pnlHeroLevel.Find("TxtLevel").GetComponent<TextMeshProUGUI>();
            this.pnlPower = this.pnlHeroLevel.Find("PnlPower");
            this.txtPower = this.pnlPower.Find("TxtPower").GetComponent<TextMeshProUGUI>();
            this.upgrade = tmpUpgrade;
            
            this.card = this.upgrade.transform.Find("Card").gameObject;
            this.cardView = this.card.GetComponent<ChestCardView>();
        }

        void UpdateAction() {
            if (this.isChanging) {
                this.current = Mathf.Lerp(this.current, this.target, 3 * Time.unscaledDeltaTime);
                if ((this.target - this.current) < this.add / 20) {
                    this.current = this.target;
                    this.isChanging = false;
                    AudioManager.Stop(AudioType.Action);
                    this.viewPref.btnClose.interactable = true;
                    this.txtPower.color = Color.green;
                }
                this.txtPower.text = Mathf.RoundToInt(this.current).ToString();
            }
        }

        public void SetFragmentActive(bool isShow) {
            this.viewPref.pnlFragment.gameObject.SetActiveSafe(isShow);
            this.viewPref.animName.gameObject.SetActiveSafe(isShow);
        }

        public void SetLevelTxtColor(Hero hero) {
            AnimationManager.Animate(pnlHeroLevel.gameObject, "Scale", finishCallback: () => {
                this.txtLevel.text = string.Format(LocalManager.GetValue(LocalHashConst.hero_level), hero.Level);
                this.txtLevel.color = levelColor;
                AnimationManager.Animate(pnlHeroLevel.gameObject, "LoopScale");
            });
        }

        public void SetUieffectStop() {
            this.cardView.SetUieffectStop();
        }

        public void SetLevelUp() {
            this.pnlHeroLevel.gameObject.SetActive(true);
            Hero hero = this.viewModel.CurrentHero;
            HeroAttributeConf heroConf =
               ConfigureManager.GetConfById<HeroAttributeConf>(hero.GetId());
            this.viewPref.txtName.text = HeroAttributeConf.GetLocalName(hero.GetId());
            this.txtLevel.color = color;
            this.txtLevel.text = string.Format(LocalManager.GetValue(LocalHashConst.hero_level), hero.Level - 1);

            this.viewPref.imgFill.sprite = ArtPrefabConf.GetSprite(
               SpritePath.resouceSliderPrefix, "hightlightgreen");
            this.viewPref.imgUpgrade.sprite = ArtPrefabConf.GetSprite(
                "upgrade_arrow_green");
            this.viewPref.sldFragment.maxValue =
                HeroLevelConf.GetHeroFragments((hero.Level - 1), heroConf);
            this.viewPref.sldFragment.value = HeroLevelConf.GetHeroFragments((hero.Level - 1), heroConf);
            this.viewPref.txtFragment.text = string.Concat(HeroLevelConf.GetHeroFragments((hero.Level - 1), heroConf), "/",
                HeroLevelConf.GetHeroFragments((hero.Level - 1), heroConf));
            AnimationManager.Animate(this.pnlHeroLevel.gameObject, "Show", start: animLevelVector, finishCallback: () => {
                this.SetLevelTxtColor(hero);
                this.SetFragmentActive(true);
            });
            StartCoroutine(SetSlidGo(hero, heroConf));
            int heroLevelUpFragMents = HeroLevelConf.GetHeroFragments(hero.Level - 1, heroConf);
            if (hero.Level == 2) {
                heroLevelUpFragMents = 2;
            }
            this.current = heroConf.GetPower(hero.Level - 1);
            this.target = heroConf.GetPower(hero.Level);
            this.add = this.target - this.current;
            this.txtPower.text = this.current.ToString();
            this.txtPower.color = Color.white;
            this.viewPref.sldFragment.value = heroLevelUpFragMents;
            this.viewPref.sldFragment.maxValue = heroLevelUpFragMents;
            this.viewPref.txtFragment.text = heroLevelUpFragMents + "/" + heroLevelUpFragMents;
            this.cardView.Rarity = heroConf.rarity;
            this.cardView.HeroName = hero.Name;
            this.cardView.SetHeroInfo();
            // Set attributes.
            GameHelper.ClearChildren(this.viewPref.pnlAttributes);
            this.SetPnlTierUpAttrItem(heroConf, hero);
            this.StartCoroutine(this.ShowAttributes());

            #region 暂时不需要星星
            //Set Stars
            // SetStars(heroConf.rarity, false); 
            #endregion
            // Animate
            this.viewPref.sldFragment.onValueChanged.RemoveAllListeners();
            this.viewPref.sldFragment.onValueChanged.AddListener(this.OnSldFragmentChange);
            AudioManager.Play("show_hero_levelup", AudioType.Show, AudioVolumn.Medium);
            //this.StartCoroutine(Animate(true/*hero, heroConf*/));            
        }

        private void SetStars(int rarity, bool skipAnim) {
            for (int i = 0; i < this.viewPref.pnlStars.childCount; i++) {
                this.viewPref.pnlStars.GetChild(i).gameObject.SetActiveSafe(false);
            }
            //StartCoroutine(ShowStars(rarity, skipAnim));
        }

        IEnumerator SetSlidGo(Hero hero, HeroAttributeConf heroConf) {
            this.cardView.PlayShowEffect();
            yield return YieldManager.GetWaitForSeconds(0.206f);
            AnimationManager.Animate(this.viewPref.pnlFragment.gameObject, "Show", isOffset: false, finishCallback: () => {
                StartCoroutine(SetSlidState(hero, heroConf));
            });
            yield return YieldManager.GetWaitForSeconds(0.059f);
            AnimationManager.Animate(this.viewPref.animName.gameObject, "Show", 0.05f, isOffset: false);
        }

        IEnumerator SetSlidState(Hero hero, HeroAttributeConf heroConf) {
            yield return YieldManager.GetWaitForSeconds(0.059f);
            this.viewPref.imgFill.sprite = ArtPrefabConf.GetSprite(
            SpritePath.resouceSliderPrefix, "hightlightblue");
            this.viewPref.imgUpgrade.sprite = ArtPrefabConf.GetSprite(
                "upgrade_arrow_blue");
            this.viewPref.sldFragment.ChangeTo(0, () => {
                this.viewPref.sldFragment.maxValue =
                    HeroLevelConf.GetHeroFragments((hero.Level), heroConf);
                this.viewPref.txtFragment.text = string.Concat(0, "/",
                       HeroLevelConf.GetHeroFragments((hero.Level), heroConf));
                StartCoroutine(this.Flash());
            }, inertia: false, duration: 1.2f);
        }

        #region 暂时不需要星星,隐藏掉
        //IEnumerator ShowStars(int rarity, bool skipAnim) {
        //    yield return YieldManager.GetWaitForSeconds(1f);
        //    AnimationManager.Animate(this.viewPref.pnlStars.gameObject, "Show", isOffset: false,
        //           finishCallback: () => {
        //               for (int i = 1; i <= rarity; i++) {
        //                   GameObject star = this.viewPref.pnlStars.GetChild(i - 1).gameObject;
        //                   star.gameObject.SetActive(true);
        //                   if (!skipAnim) {
        //                       AnimationManager.Animate(star, "New", delay: 0.3f * i);
        //                   }
        //               }
        //           }
        //       );
        //} 
        #endregion

        private void SetPnlTierUpAttrItem(HeroAttributeConf heroConf, Hero hero) {
            List<HeroAttribute> attributeList = new List<HeroAttribute>() {
                HeroAttribute.ArmyAmount,
                HeroAttribute.Attack,
                HeroAttribute.Defense
            };
            int attributeListCount = attributeList.Count;
            pnlTierUpAttrItem = new GameObject[attributeListCount];
            HeroAttribute attribute;
            for (int i = 0; i < attributeListCount; i++) {
                attribute = attributeList[i];
                pnlTierUpAttrItem[i] = PoolManager.GetObject(PrefabPath.pnlTierUpAttrItem, this.viewPref.pnlAttributes);
                this.SetAttribute(
                attribute,
                heroConf.GetAttribute(hero.Level - 1, attribute),
                heroConf.GetAttribute(hero.Level, attribute),
                pnlTierUpAttrItem[i].transform
            );
                pnlTierUpAttrItem[i].GetComponent<CanvasGroup>().alpha = 0;
            }
            this.isChanging = true;
        }

        private IEnumerator Flash() {
            float value = 0;
            for (int i = 0; i < 5; i++) {
                value = value + 0.1f;
                Image image = this.viewPref.imgFlash.GetComponent<Image>();
                Material mat = Instantiate(image.material);
                mat.SetFloat("_Exposure", value);
                image.material = mat;
                Image imageUp = this.viewPref.imgUpgrade.GetComponent<Image>();
                Material matUP = Instantiate(imageUp.material);
                matUP.SetFloat("_Exposure", value);
                imageUp.material = matUP;
                yield return YieldManager.GetWaitForSeconds(0.06f);
                if (value == 0.5) {
                    for (int j = 5; j >= 0; j--) {
                        value = value - 0.1f;
                        mat = Instantiate(image.material);
                        mat.SetFloat("_Exposure", value);
                        image.material = mat;
                        matUP.SetFloat("_Exposure", value);
                        imageUp.material = matUP;
                        yield return YieldManager.GetWaitForSeconds(0.06f);
                    }
                }
            }

        }

        IEnumerator ShowAttributes() {
            yield return YieldManager.GetWaitForSeconds(0.25f);
            for (int i = 0; i < pnlTierUpAttrItem.Length; i++) {
                this.attributesIcon = pnlTierUpAttrItem[i].transform.Find("ImgIcon").GetComponent<Transform>();
                this.attributesNumberIcon = pnlTierUpAttrItem[i].transform.Find("PnlNumber").GetComponent<Transform>();
                this.attributesNumberTxt = attributesNumberIcon.transform.Find("TxtNumber").GetComponent<Transform>();
                this.attributesAddNumber = attributesNumberTxt.transform.Find("TxtAddNumber").GetComponent<Transform>();
                this.SetNumberShow(false);
                //animCount = i;
                AnimationManager.Animate(pnlTierUpAttrItem[i], "Show",
                    needRestart: false, finishCallback: () => {
                        AttributeView attributeView = pnlTierUpAttrItem[i].GetComponent<AttributeView>();
                        ShowAttributeValue(attributeView.addValue, attributeView.value, attributeView);
                    });
                AnimationManager.Animate(attributesIcon.gameObject, "Show", start: attrIconStart);
                AnimationManager.Animate(attributesIcon.gameObject, "scale", start: attrIconStart);
                yield return YieldManager.GetWaitForSeconds(0.15f);
                attributesNumberIcon.gameObject.SetActiveSafe(true);
                yield return YieldManager.GetWaitForSeconds(0.3f);
                attributesNumberTxt.gameObject.SetActiveSafe(true);
                yield return YieldManager.GetWaitForSeconds(0.3f);
                attributesAddNumber.gameObject.SetActiveSafe(true);
            }
            AudioManager.Play("show_hero_levelup_number", AudioType.Action, AudioVolumn.Medium);
            isOneStepClick = true;
        }


        private void ShowAttributesFast() {
            for (int i = 0; i < pnlTierUpAttrItem.Length; i++) {
                this.attributesIcon = pnlTierUpAttrItem[i].transform.Find("ImgIcon").GetComponent<Transform>();
                this.attributesNumberIcon = pnlTierUpAttrItem[i].transform.Find("PnlNumber").GetComponent<Transform>();
                this.attributesNumberTxt = attributesNumberIcon.transform.Find("TxtNumber").GetComponent<Transform>();
                this.attributesAddNumber = attributesNumberTxt.transform.Find("TxtAddNumber").GetComponent<Transform>();
                this.SetNumberShow(true);
                AttributeView attributeView = pnlTierUpAttrItem[i].GetComponent<AttributeView>();
                ShowAttributeValue(attributeView.addValue, attributeView.value, attributeView);
                AnimationManager.Animate(attributesIcon.gameObject, "Show", start: attrIconStart);
                AnimationManager.Animate(attributesIcon.gameObject, "scale", start: attrIconStart);
                attributeView.Show(true);
                AnimationManager.Finish(attributesIcon.gameObject);
                //animCount = i;
            }
            AudioManager.Play("show_hero_levelup_number", AudioType.Action, AudioVolumn.Medium);
        }

        public void SetNumberShow(bool show) {
            attributesNumberIcon.gameObject.SetActiveSafe(show);
            attributesNumberTxt.gameObject.SetActiveSafe(show);
            attributesAddNumber.gameObject.SetActiveSafe(show);
        }

        private void OnSldFragmentChange(float value) {
            this.viewPref.txtFragment.text =
                string.Concat(Mathf.RoundToInt(value), "/", this.viewPref.sldFragment.maxValue);
        }

        //Set Upgrade Attribute View
        private void SetAttribute(HeroAttribute attribute, float value, float newValue, Transform pnl) {
            AttributeView attributeView = pnl.GetComponent<AttributeView>();
            string attributeName = Enum.GetName(typeof(HeroAttribute), attribute).ToLower();
            attributeView.Name = LocalManager.GetValue("hero_attribute_", attributeName);
            attributeView.Icon = ArtPrefabConf.GetSprite(
                SpritePath.heroAttributeIconPrefix, attributeName
            );
            string upgradeValue;
            int addValue = Mathf.RoundToInt(newValue - value);


            if (addValue == 0) {
                upgradeValue = string.Empty;
            } else {
                upgradeValue = string.Concat("<color=#73EE67FF> + ", addValue, "</color>");
            }
            attributeView.addValue = addValue;
            attributeView.value = value;
            attributeView.AddValue = upgradeValue;
        }

        private void ShowAttributeValue(int addValue, float value, AttributeView attributeView) {
            attributeView.Value = (Mathf.CeilToInt(Mathf.SmoothDamp((value + addValue),
                value, ref smoothSpeed, 0, 3f))).ToString();
        }

        public void SetUIVisibleForBattle(bool isVisible) {
            this.ui.GetComponent<CanvasGroup>().alpha = isVisible ? 1 : 0;
            this.pnlHeroLevel.gameObject.SetActive(isVisible);
        }

        /* Propert change function */

        /***************************/
        protected void OnBtnCloseClick() {
            if (isOneStepClick) {
                isOneStepClick = false;
                this.viewModel.Hide();
                this.viewModel.onAnimationEnd.InvokeSafe();
                this.viewModel.onAnimationEnd = null;
                //animCount = -1;
            } else {
                isOneStepClick = true;
                this.OnBtnAttributesFastClick();
            }
        }

        private void OnBtnAttributesFastClick() {
            this.StopAllCoroutines();
            Hero hero = this.viewModel.CurrentHero;
            HeroAttributeConf heroConf =
                ConfigureManager.GetConfById<HeroAttributeConf>(hero.GetId());
            this.viewPref.imgFill.sprite = ArtPrefabConf.GetSprite(
            SpritePath.resouceSliderPrefix, "hightlightblue");
            this.viewPref.imgUpgrade.sprite = ArtPrefabConf.GetSprite(
                "upgrade_arrow_blue");
            this.viewPref.sldFragment.ChangeTo(0, () => {
                this.viewPref.sldFragment.maxValue =
                    HeroLevelConf.GetHeroFragments((hero.Level), heroConf);
                this.viewPref.txtFragment.text = string.Concat(0, "/",
                        HeroLevelConf.GetHeroFragments((hero.Level), heroConf));
                StartCoroutine(this.Flash());
            }, inertia: false, duration: 0f);
            this.ShowAttributesFast();
        }

        protected override void OnVisible() {
            UIManager.ChestCamera.SetActive(true);
            this.upgrade.gameObject.SetActive(true);
            UIManager.ShowFakeBack(true);
            UIManager.GetUI("UIBackground").GetComponent<Canvas>().sortingOrder =
                this.canvas.sortingOrder - 1;
        }

        protected override void OnInvisible() {
            //this.cardView.Reset();  外发光不需要 注释掉
            UIManager.ChestCamera.SetActive(false);
            this.upgrade.gameObject.SetActive(false);
            this.pnlHeroLevel.gameObject.SetActive(false);
            UIManager.ShowFakeBack(false);
            UIManager.GetUI("UIBackground").GetComponent<Canvas>().sortingOrder = 1;
            this.viewModel.OnHeroLevelUpAnimDone();
        }

        #region FTE

        public void OnFteStep130Start() {
            if (!IsUIInit) {
                this.InitUI();
            }
            FteManager.SetMask(this.viewPref.pnlHeroInfo, hasArrow: false);
        }

        #endregion
    }
}
