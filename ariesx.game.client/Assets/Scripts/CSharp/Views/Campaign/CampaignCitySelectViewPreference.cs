using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class CampaignCitySelectViewPreference : BaseViewPreference
    {
        [Tooltip("UICampaignCitySelect.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UICampaignCitySelect.PnlTitle.BtnClose")]
        public CustomButton btnClose;
        [Tooltip("UICampaignCitySelect.PnlTitle.BtnNext")]
        public CustomButton btnNext;
        public Transform pnlList;
    }
}