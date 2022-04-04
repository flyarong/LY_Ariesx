using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class CampaignRewardDominationViewPreference : BaseViewPreference
    {
        [Tooltip("UICampaignRewardDomination.BtnBackground")]
        public Button  btnBackground;
        [Tooltip("UICampaignRewardDomination.PnlTitle.BtnClose")]
        public CustomButton btnClose;
        public Transform pnlList;
        public RectTransform rectTransform;
    }
}