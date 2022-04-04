using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute{
    //[DisallowMultipleComponent]
    public class DemonShadowDetailsViewPeference : BaseViewPreference
    {
        [Tooltip("PnlDemonShadowDetails.PnlAgainstNotOpen")]
        public CanvasGroup pnlAgainstNotOpen;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened")]
        public CanvasGroup pnlAgainstOpened;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstNotOpen.BtnAgainst")]
        public CustomButton btnAgainst;
        [Tooltip("PnlDemonShadowDetails..PnlCampaignRewards")]
        public Button btnShowRewardDetails;
        [Tooltip("PnlDemonShadowDetails.BtnShowRule")]
        public CustomButton btnShowRule;
        [Tooltip("PnlDemonShadowDetails.PnlCampaignRewards.PnlRewardItem")]
        public Transform pnlRewardsList;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.DemonIcon")]
        public Button btnDemonIcon;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.ImgDemonHP.TxtDemonHPNum")]
        public TextMeshProUGUI txtDemonHPNum;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.ImgDemonHP.SliderDemonHP")]
        public Slider sliderDemonHp;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.TxtDemonLevelDes.TxtDemonLevelNum")]
        public TextMeshProUGUI txtDemonLevel;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.TxtDemonFightingDes.TxtDemonFightingNum")]
        public TextMeshProUGUI txtFighting;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.TxtDemonEndTimeDes.TxtDemonEndTimeNum")]
        public TextMeshProUGUI txtEndTime;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.TxtDemonPos")]
        public TextMeshProUGUI txtDemonPos;
        [Tooltip("PnlDemonShadowDetails.PnlAgainstOpened.ImgAgainstBG.BtnAgainst")]
        public CustomButton btnStartAgainst;
    }
}