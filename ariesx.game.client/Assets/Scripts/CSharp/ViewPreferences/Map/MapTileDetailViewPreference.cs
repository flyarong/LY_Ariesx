using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapTileDetailViewPreference : BaseViewPreference {
        [Tooltip("UITileDetail.BtnBackground")]
        public Button btnDetailBackground;
        [Tooltip("UITileDetail.PnlTileDetail")]
        public Transform pnlTileDetail;
        public LayoutGroup lgTileDetail;
        public CustomContentSizeFitter csfTileDetail;

        [Tooltip("UITileDetail.PnlTileDetail.PnlBase.TxtTileLevel")]
        public TextMeshProUGUI txtTileLevel;
        //[Tooltip("UITileDetail.PnlTileDetail.PnlBase.PnlInfo")]
        //public Transform pnlDetailTroop;
        [Tooltip("UITileDetail.PnlTileDetail.PnlBase.PnlOther.PnlEnduranceInfo")]
        public Transform pnlDetailEndurance;
        [Tooltip("UITileDetail.PnlTileDetail.PnlBase.PnlOther.PnlEnduranceInfo.PnlSlider")]
        public Slider sldDetailEndurance;
        [Tooltip("UITileDetail.PnlTileDetail.PnlBase.PnlOther.PnlEnduranceInfo.PnlSlider.TxtAmount")]
        public TextMeshProUGUI txtDetailEndurance;
        [Tooltip("PnlTroopInfo.PmlBackground.TxtTroopAmount")]
        public TextMeshProUGUI txtDetailTroopAmount;
        [Tooltip("PnlTroopPower.PmlBackground.TxtTroopPower")]
        public TextMeshProUGUI txtDetailTroopPower;

        [Tooltip("UITileDetail.PnlTileDetail.PnlBase.PnlTroopInfo.PnlTroopGrid")]
        public TroopGridView troopGridView;
        public CanvasGroup troopGridCG;
        
        [Tooltip("UITileDetail.PnlTileDetail.PnlReward")]
        public Transform pnlReward;
        [Tooltip("UITileDetail.PnlTileDetail.PnlReward.PnlLabelReward.Text")]
        public TextMeshProUGUI txtLabelRewardTitle;
        [Tooltip("UITileDetail.PnlTileDetail.PnlReward.PnlReward")]
        public Transform pnlDetailReward;

        [Tooltip("UITileDetail.PnlTileDetail.PnlLimitReward")]
        public Transform pnlLimitReward;
        [Tooltip("UITileDetail.PnlTileDetail.PnlLimitReward.PnlContent.PnlGem.PnlBackground.Text")]
        public TextMeshProUGUI txtGemCount;
        [Tooltip("UITileDetail.PnlTileDetail.PnlLimitReward.PnlContent.TxtRemainTime")]
        public TextMeshProUGUI txtRemainTime;

        [Tooltip("UITileDetail.PnlTileDetail.PnlTributeAddition")]
        public Transform pnlTributeAddition;
        [Tooltip("UITileDetail.PnlTileDetail.PnlTributeAddition.PnlTributeList")]
        public TextMeshProUGUI txtTributeGoldBuff;
        public TextMeshProUGUI txtTributeGoldBuffBonus;

        [Tooltip("UITileDetail.PnlTileDetail.PnlResourcesProduct")]
        public Transform pnlResourceProduct;
        [Tooltip("UITileDetail.PnlTileDetail.PnlResourcesProduct.PnlResources")]
        public Transform pnlProductionsList;
        
        [Tooltip("UITileDetail.PnlTileDetail.PnlDescription")]
        public Transform pnlDescription;
        [Tooltip("UITileDetail.PnlTileDetail.PnlDescription.Text")]
        public TextMeshProUGUI txtDetailDescription;

        [Tooltip("UITileDetail.PnlTileDetail.PnlContent.PnlTitle.Text")]
        public TextMeshProUGUI txtDetailTile;
        [Tooltip("UITileDetail.PnlTileDetail.PnlContent.PnlTitle.BtnBack")]
        public Button btnDetailReturn;
    }
}
