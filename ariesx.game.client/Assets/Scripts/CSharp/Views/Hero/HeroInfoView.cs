using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class HeroInfoView: BaseView {
        private HeroInfoViewModel viewModel;
        private HeroInfoViewPreference viewPref;

        // Other param
        private HeroSkillView currentSkillView = null;
        private bool isReachMaxLevel = false;

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<HeroInfoViewModel>();
            this.ui = UIManager.GetUI("UIHeroInfo");
            this.viewPref = this.ui.transform.GetComponent<HeroInfoViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(() => this.OnBtnCloseClick());
            this.viewPref.btnDescription.onClick.AddListener(this.OnBtnDescriptionClick);
            this.viewPref.btnBack.onClick.AddListener(this.OnBtnBackClick);
            this.viewPref.btnUpgrade.onClick.AddListener(this.OnBtnUpgradeClick);
        }

        public override void PlayShow() {
            base.PlayShow(() => {
                this.viewPref.imgBackground.color =
                    this.viewModel.IsSubWindow ?
                    new Color(0, 0, 0, 0.4f) :
                    new Color(0, 0, 0, 0f);

                this.viewModel.isLevelUping = false;
            }, true);
        }

        public void SetHeroInfo(HeroInfoType infoType = HeroInfoType.Self, bool isFresh = false) {
            this.SetHeroImage();
            this.SetHeroAttribute(infoType);
            this.SetSlider();
            this.SetSkill();
            this.SetDescription();
            this.SetHeroInfoView(isFresh);
            this.SetButton();
            if (infoType == HeroInfoType.Self) {
                this.viewPref.fragSlider.gameObject.SetActive(true);
                this.SetFragment();
            } else {
                this.viewPref.fragSlider.gameObject.SetActive(false);
            }
        }

        private void SetFragment() {
            bool reachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(this.viewModel.CurrentHero.Name,
                this.viewModel.CurrentHero.Level);
            int heroFragments = HeroLevelConf.GetHeroUpgradFragments(
                this.viewModel.CurrentHero.Name, this.viewModel.CurrentHero.Level);
            bool canLeveUp = (this.viewModel.CurrentHero.FragmentCount >= heroFragments)
                && !reachMaxLevel;
            bool isMaxFill = heroFragments > 0;
            this.viewPref.fragSlider.gameObject.SetActiveSafe(isMaxFill);
            this.viewPref.imgFragSliderFill.gameObject.SetActiveSafe(!isMaxFill);
            if (isMaxFill) {
                this.viewPref.imgFragSliderFill.gameObject.SetActiveSafe(false);
                if (this.viewModel.CurrentHero.Level == 1) {
                    this.viewPref.txtSlider.text = string.Concat(this.viewModel.CurrentHero.FragmentCount + 1, "/",
                         heroFragments + 1);
                    this.viewPref.fragSlider.value = 1.0f * (this.viewModel.CurrentHero.FragmentCount + 1) / (heroFragments + 1)
                        * this.viewPref.fragSlider.maxValue;
                } else {
                    this.viewPref.txtSlider.text = string.Concat(this.viewModel.CurrentHero.FragmentCount, "/",
                         heroFragments);
                    this.viewPref.fragSlider.value = 1.0f * this.viewModel.CurrentHero.FragmentCount / heroFragments
                        * this.viewPref.fragSlider.maxValue;
                }

                this.viewPref.imgUpgrade.gameObject.SetActiveSafe(!canLeveUp);
                this.viewPref.animUpgrade.gameObject.SetActiveSafe(canLeveUp);
                this.viewPref.animSliderFill.gameObject.SetActiveSafe(canLeveUp);
                this.viewPref.animSliderFill.SetBool("IsShow", canLeveUp);
                this.viewPref.animUpgrade.SetBool("IsShow", canLeveUp);
                this.viewPref.imgFragmentSliFill.sprite = ArtPrefabConf.GetSprite(
                    SpritePath.resouceSliderPrefix, canLeveUp ? "hightlightgreen" : "hightlightblue");
            }
        }

        private void SetHeroImage() {
            Hero hero = this.viewModel.CurrentHero;
            hero.IsNew = false;
            int level = hero.Level;
            this.viewPref.heroHeadView.SetHero(hero, showLevel: false, showStar: false, showHeroStatus: false);

            this.viewPref.txtTitle.text =
                HeroAttributeConf.GetLocalName(this.viewModel.CurrentHero.GetId());
            this.viewPref.txtHeroLevel.text =
                string.Concat(LocalManager.GetValue(LocalHashConst.troop_format_order_level),
                ".", level);
        }

        private void SetHeroAttribute(HeroInfoType infoType) {
            HeroAttributeConf heroConf = this.viewModel.CurrentHeroConf;
            int heroLevel = 1;
            Hero hero = this.viewModel.CurrentHero;
            heroLevel = hero.Level;
            bool isOwnHero = (infoType == HeroInfoType.Self);
            int attackBonus = isOwnHero ? this.viewModel.GetAttackBonus() : 0;
            this.SetAttribute(
                heroConf.GetAttribute(heroLevel, HeroAttribute.Attack, afterBonus: false),
                this.viewPref.pnlAttrAttack,
                attackBonus,
                attackBonus != 0,
                 string.Format(
                        LocalManager.GetValue(LocalHashConst.attribute_bonus_attack),
                        attackBonus
                    )
            );
            int defense = isOwnHero ? this.viewModel.GetDefenseBonus() : 0;
            this.SetAttribute(
                heroConf.GetAttribute(heroLevel, HeroAttribute.Defense, afterBonus: false),
                this.viewPref.pnlAttrDefence,
                defense,
                defense != 0,
                string.Format(
                        LocalManager.GetValue(LocalHashConst.attribute_bonus_defence),
                        defense));
            this.SetAttribute(
                heroConf.GetAttribute(heroLevel, HeroAttribute.Speed),
                this.viewPref.pnlAttrSpeed
            );

            float siegeBonusPercent = this.viewModel.GetSiegeBonus();
            if (this.viewModel.infoType == HeroInfoType.Self) {
                int siege = heroConf.GetAttribute(heroLevel, HeroAttribute.Siege, afterBonus: false);
                int siegeBonus = Mathf.RoundToInt(siege * siegeBonusPercent);
                this.SetAttribute(
                    siege,
                    this.viewPref.pnlAttrSiege,
                    siegeBonus,
                    siegeBonus != 0,
                    string.Format(
                        LocalManager.GetValue(LocalHashConst.attribute_bonus_siege),
                        siegeBonus)
                    );
            } else {
                this.SetAttribute(
                    heroConf.GetAttribute(heroLevel, HeroAttribute.Siege, afterBonus: false),
                    this.viewPref.pnlAttrSiege, 0);
            }
            this.SetHeroRarityInfo(heroConf.rarity);
        }

        private void SetHeroRarityInfo(int rarity) {
            int i = 0;
            for (; i < rarity; i++) {
                this.viewPref.heroRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.viewPref.heroRarity.Length;
            for (; i < rarityCount; i++) {
                this.viewPref.heroRarity[i].SetActiveSafe(false);
            }
        }

        private List<AttributeView> attributeViewList = new List<AttributeView>(4);
        private void SetAttribute(float value, Transform pnl, float bonus = 0,
            bool enableTip = false, string tip = "") {
            AttributeView attributeView = pnl.GetComponent<AttributeView>();
            attributeView.Value = value.ToString();
            if (bonus != 0) {
                attributeView.Value = string.Format(
                    "{0}     <color=#73EE67FF>+ {1}</color>",
                    value, bonus
                );
            }
            attributeView.NeedShowTip = enableTip;
            attributeView.OnTipShow = this.OnShowAttributeTip;
            attributeView.Tip = tip;
            this.attributeViewList.TryAdd(attributeView);
        }

        private void OnShowAttributeTip(AttributeView view) {
            foreach (AttributeView attribute in this.attributeViewList) {
                if (view.name.CustomEquals(attribute.name) && view.IsShowTip) {
                    continue;
                }
                attribute.IsShowTip = false;
            }
        }

        private void HideAllAttributeTips() {
            foreach (AttributeView attribute in this.attributeViewList) {
                attribute.IsShowTip = false;
            }
            this.SetHeroBasicSkillInfoVisible(true);
        }

        private void SetSlider() {
            Hero hero = this.viewModel.CurrentHero;
            this.isReachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(hero);
            int heroUpgradCost = HeroLevelConf.GetHeroUpgradCost(hero);
            this.viewPref.txtHeroLevelUpCost.text = heroUpgradCost.ToString();
            if (heroUpgradCost > RoleManager.GetResource(Resource.Gold)) {
                this.viewPref.txtHeroLevelUpCost.color = Color.red;
            } else {
                this.viewPref.txtHeroLevelUpCost.color = Color.white;
            }
            bool isShowEnergy = this.viewModel.infoType != HeroInfoType.Others;
            this.viewPref.pnlEnergySlider.gameObject.SetActiveSafe(isShowEnergy);
            if (isShowEnergy) {
                this.viewPref.energySlider.maxValue = GameConst.HERO_ENERGY_MAX;
                this.viewPref.energySlider.value = hero.NewEnergy;
                this.viewPref.energyAmount.text =
                    string.Concat(hero.NewEnergy, "/", GameConst.HERO_ENERGY_MAX);
            }
            HeroAttributeConf heroConf = this.viewModel.CurrentHeroConf;
            int armyAmount = heroConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount, hero.armyCoeff);
            this.viewPref.troopSlider.maxValue = armyAmount;
            this.viewPref.troopSlider.value = hero.ArmyAmount;
            this.viewPref.troopAmount.text = string.Concat(hero.ArmyAmount, "/", armyAmount);
        }

        private void SetButton() {
            Hero hero = this.viewModel.CurrentHero;
            int fragmentCount = hero.FragmentCount;
            int heroFragments = HeroLevelConf.GetHeroUpgradFragments(hero);
            int heroUpgradCost = HeroLevelConf.GetHeroUpgradCost(hero);
            this.isReachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(hero);
            float heroThisGold = RoleManager.GetResource(Resource.Gold);
            bool canLevelUp = (fragmentCount >= heroFragments) && !this.isReachMaxLevel;
            bool canUpgrade = canLevelUp && (heroThisGold >= heroUpgradCost);
            this.viewPref.btnUpgrade.Grayable = !canUpgrade;
        }

        private void OnBtnUpgradeClick() {
            if (this.viewPref.btnUpgrade.Grayable) {
                Hero hero = this.viewModel.CurrentHero;
                int fragmentCount = hero.FragmentCount;
                int heroFragments = HeroLevelConf.GetHeroUpgradFragments(hero);
                int heroUpgradCost = HeroLevelConf.GetHeroUpgradCost(hero);
                float heroThisGold = RoleManager.GetResource(Resource.Gold);
                this.SetToUpgrade(fragmentCount, heroFragments, heroUpgradCost, heroThisGold);
            } else {
                this.OnBtnCanUpgradeClick();
            }
        }

        private void SetToUpgrade(int fragmentCount, int heroFragments,
            int heroUpgradCost, float heroThisGold) {
            int lackfragmentCount = heroFragments - fragmentCount;
            float lackGoldCount = heroUpgradCost - heroThisGold;
            bool isLackfragmen = lackfragmentCount > 0;
            bool isLackGoldCount = lackGoldCount > 0;
            if (isLackfragmen) {
                UIManager.ShowTip(string.Format(LocalManager.GetValue(
                LocalHashConst.hero_upgrade_fragment_short), lackfragmentCount), TipType.Info);
            } else if (isLackGoldCount) {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.gold_short),
                LocalManager.GetValue(LocalHashConst.hero_upgrade_gold_short), ShowPay, CancelPay);
            }
        }

        private void CancelPay() {
            this.viewModel.Hide();
        }

        private void ShowPay() {
            this.viewModel.ShowPay();
        }

        public void SetDescription() {
            this.viewPref.txtHeroDescription.text =
                LocalManager.GetValue(this.viewModel.CurrentHero.Name, "_description");
            this.viewPref.txtHeroDescription.GetComponent<CustomContentSizeFitter>().
                onSetLayoutVertical.AddListener(
                () => {
                    RectTransform rect =
                    this.viewPref.txtHeroDescription.transform.GetComponent<RectTransform>();
                    rect.anchoredPosition = Vector2.zero;
                }
            );
        }

        private void SetSkill() {
            this.viewPref.txtSkillName.text =
                SkillConf.GetName(this.viewModel.CurrentHeroConf.skills);
            if (this.viewModel.infoType == HeroInfoType.Unlock) {
                HeroSkillDefaultConf skillDefaultConf =
                    HeroSkillDefaultConf.GetConf(this.viewModel.CurrentHero.Name);
                this.viewPref.txtSkillDescription.text =
                string.Format(
                    SkillConf.GetDescription(this.viewModel.CurrentHeroConf.skills),
                    skillDefaultConf.attrList.ToArray()
                );
            } else {
                this.viewPref.txtSkillDescription.text =
                    string.Format(
                        SkillConf.GetDescription(this.viewModel.CurrentHeroConf.skills),
                        this.viewModel.CurrentHero.Skills[0].Args.ToArray()
                    );
            }
        }

        //private void ShowHeroStatus() {
        //    this.viewPref.pnlBasicInfo.gameObject.SetActiveSafe(true);
        //    this.viewPref.btnDescription.gameObject.SetActiveSafe(true);
        //    this.viewPref.pnlDescription.gameObject.SetActiveSafe(false);
        //    this.viewPref.btnBack.gameObject.SetActiveSafe(false);
        //}

        //private void ShowHeroDescription() {
        //    this.viewPref.pnlBasicInfo.gameObject.SetActiveSafe(false);
        //    this.viewPref.btnDescription.gameObject.SetActiveSafe(false);
        //    this.viewPref.pnlDescription.gameObject.SetActiveSafe(true);
        //    this.viewPref.btnBack.gameObject.SetActiveSafe(true);
        //}

        private void SetHeroInfoView(bool isFresh) {
            bool isSelf = this.viewModel.infoType == HeroInfoType.Self;
            this.viewPref.pnlButton.gameObject.SetActiveSafe(isSelf && !isReachMaxLevel);
        }

        private void OnBtnDescriptionClick() {
            this.SetHeroBasicSkillInfoVisible(false);
        }

        private void OnBtnBackClick() {
            this.SetHeroBasicSkillInfoVisible(true);
        }

        private void SetHeroBasicSkillInfoVisible(bool isShowHeroSkill) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.basicInfoCanvasGroup, isShowHeroSkill);
            UIManager.SetUICanvasGroupEnable(this.viewPref.heroDescriptionCanvasGroup, !isShowHeroSkill);
        }

        public void ContinueShowSkillDetail() {
            this.currentSkillView.onBtnDetailClick.Invoke(
                this.currentSkillView.GetComponent<RectTransform>());
        }

        private void OnBtnCanUpgradeClick() {
            this.viewModel.LevelUpReq();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        #region FTE

        public void OnHeroStep2Start() {
            this.afterShowCallback = () => {
                if (!this.viewPref.btnUpgrade.Grayable) {
                    FteManager.SetMask(this.viewPref.btnUpgrade.pnlContent, isButton: true, isEnforce: !FteManager.FteOver);
                } else {
                    FteManager.StopFte();
                    this.viewModel.StartChapterDailyGuid();
                }
            };
        }

        #endregion

        protected override void OnInvisible() {
            this.HideAllAttributeTips();
        }
    }
}
