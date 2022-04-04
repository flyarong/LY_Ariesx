using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System;
using TMPro;

namespace Poukoute {
    public class TroopInfoViewPreference : BaseViewPreference {
        [Tooltip("UITroop.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UITroop.PnlDetail")]
        public Transform pnlDetail;
        public RectTransform detailRectTransform;
        [Tooltip("UITroop.PnlDetail.PnlTitle.BtnClose")]
        public Button btnClose;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlTroop.PnlInfo")]
        public Transform pnlTroopInfo;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlTroop.PnlTroopGrid Items")]
        public Transform[] heroTransform;
        public GameObject[] imgEmpty;
        public CustomClick[] imgEmptyClick;
        public GameObject[] imgLocked;
        public RectTransform TipsRT;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlResult.TxtResult")]
        public TextMeshProUGUI txtResult;
        public RectTransform resultRT;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlTips.PnlMarchTips")]
        public Transform pnlMarchTips;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther")]
        public Transform pnlOther;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlEnergy")]
        public Transform pnlEnergy;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlEnergy.ImgBackground.TxtNumber")]
        public TextMeshProUGUI txtEnergy;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlTimeCost")]
        public Transform pnlTimeCost;
        public Button btnTimeCost;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlTimeCost.PnlTip")]
        public Transform pnlTimeTip;
        public TextMeshProUGUI txtTimeTip;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlTimeCost.ImgBackground.TxtNumber")]
        public TextMeshProUGUI txtTimeCost;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlTimeArrive")]
        public Transform pnlTimeArrive;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlAttackInfo.PnlOther.PnlTimeArrive.ImgBackground.TxtNumber")]
        public TextMeshProUGUI txtTimeArrive;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlButton.Button")]
        public CustomButton btnSend;
        [Tooltip("UITroop.PnlDetail.PnlContent.PnlButton.BtnReturnImmediately")]
        public CustomButton btnReturnImmediately;
        [Tooltip("UITroop.PnlDetail")]
        public Button btnPnl;
    }
}
