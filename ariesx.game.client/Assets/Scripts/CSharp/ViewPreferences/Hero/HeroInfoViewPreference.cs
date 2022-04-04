using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class HeroInfoViewPreference : BaseViewPreference {
        [Tooltip("UIHeroInfo.BtnBackground")]
        public Button btnBackground;
        public Image imgBackground;
        
        [Tooltip("UIHeroInfo.PnlDetail.PnlTitle.Text")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UIHeroInfo.PnlDetail.PnlTitle.BtnClose")]
        public Button btnClose;

        

        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlAvatar.PnlHeroBig")]
        public HeroHeadView heroHeadView;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.BtnDescription")]
        public Button btnDescription;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlDescription")]
        //public Transform pnlDescription;
        public CanvasGroup heroDescriptionCanvasGroup;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlDescription.TxtDescription")]
        public TextMeshProUGUI txtHeroDescription;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.BtnReturn")]
        public Button btnBack;

        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlBasicInfo")]
        //public Transform pnlBasicInfo;
        public CanvasGroup basicInfoCanvasGroup;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlBasicInfo.PnlSkill.TxtSkillTitle")]
        public TextMeshProUGUI txtSkillName;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlBasicInfo.PnlSkill.TxtSkillDetail")]
        public TextMeshProUGUI txtSkillDescription;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlBasicInfo.PnlLevel.TxtLevel")]
        public TextMeshProUGUI txtHeroLevel;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlBasicInfo.PnlLevel.PnlStars")]
        public GameObject[] heroRarity;

        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlAttribute.PnlAttack")]
        public Transform pnlAttrAttack;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlAttribute.PnlDefence")]
        public Transform pnlAttrDefence;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlAttribute.PnlSpeed")]
        public Transform pnlAttrSpeed;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlAttribute.PnlSiege")]
        public Transform pnlAttrSiege;
        
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlTroop")]
        public Transform pnlTroopSlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlTroop.Slider")]
        public Slider troopSlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlTroop.Slider.TxtAmount")]
        public TextMeshProUGUI troopAmount;

        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlEnergy")]
        public Transform pnlEnergySlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlEnergy.Slider")]
        public Slider energySlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlHero.PnlStatus.PnlSlider.PnlEnergy.Slider.TxtAmount")]
        public TextMeshProUGUI energyAmount;

        [Tooltip("UIHeroInfo.PnlDetail.PnlInfo.PnlContent.PnlHero.PnlAvatar.PnlFragments")]
        public Transform pnlFragmentSlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlInfo.PnlContent.PnlHero.PnlAvatar.PnlFragments.Slider")]
        public Slider fragSlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlInfo.PnlContent.PnlHero.PnlAvatar.PnlFragments.Slider.TxtAmount")]
        public TextMeshProUGUI txtSlider;
        [Tooltip("UIHeroInfo.PnlDetail.PnlInfo.PnlContent.PnlHero.PnlAvatar.PnlFragments.Slider.FillArea.FillContent.MaskFill.Fill")]
        public Image imgFragSliderFill;

        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlButton")]
        public Transform pnlButton;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlButton.TxtCost")]
        public TextMeshProUGUI txtHeroLevelUpCost;
        [Tooltip("UIHeroInfo.PnlDetail.PnlContent.PnlButton.BtnUpgrade")]
        public CustomButton btnUpgrade;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlAvatar.SliFragment.FillArea.Fill")]
        public Image imgFragmentSliFill;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlAvatar.SliFragment.ImgUpgrade")]
        public Image imgUpgrade;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlAvatar.SliFragment.animUpgrade")]
        public Animator animUpgrade;
        [Tooltip("UIHeroInfo.PnlDetail.PnlHero.PnlAvatar.SliFragment.FillArea.animSliderFill")]
        public Animator animSliderFill;
    }
}
