using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class OpenChestHeroInfoViewPreference : BaseViewPreference {
        [Tooltip("PnlHero/PnlHeroDetail")]
        public Transform pnlHeroDetail;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHead/TxtTitle")]
        public Transform pnlNewHeroTitle;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHead/TxtStrongTips")]
        public GameObject mostPotentialHero;

        [Tooltip("PnlHero/PnlHeroDetail/PnlSkill")]
        public Transform pnlSkill;

        [Tooltip("PnlHero/PnlHeroDetail/PnlSkill/TxtName")]
        public TextMeshProUGUI txtSkillName;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/PnlSlider")]
        public Slider sldTier;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/PnlSlider/FillArea/Fill")]
        public Image imgSliderFill;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/PnlSlider/TxtAmount")]
        public TextMeshProUGUI txtShowAmount;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/PnlSlider/ImgUpgrade")]
        public Image imgTierUp;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/PnlSlider/TxtUpdate")]
        public Transform txtTierUp;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/TxtHeroLevel")]
        public TextMeshProUGUI txtHeroLevel;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlTier/TxtAddAmount")]
        public TextMeshProUGUI txtAddAmount;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlDescription/PnlStars")]
        public Transform pnlStars;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlDescription/TxtName")]
        public TextMeshProUGUI txtHeroName;

        [Tooltip("PnlHero/PnlHeroDetail/PnlHeroInfo/PnlDescription/TxtName")]
        public TextMeshProUGUI txtNewPoolName;

        public TextMeshProUGUI txtHeroGrade;

        public GameObject ImgFlash;

        public GameObject PnlHeroName;

        public GameObject PnlSeperate;

        public GameObject PnlSkill;

        public RectTransform imgSkillRight;

        public RectTransform imgSkillLeft;

        public TextMeshProUGUI txtSkill;

        public AnimationCurve skillBackCurve;
    }
}
