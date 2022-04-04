using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyGiftDetailsPreference : BaseViewPreference {
        [Tooltip("UIDailyGiftDetails.PalPay.PnlHead.BtnClose")]
        public Button btnClose;
        public CustomButton background;
        [Tooltip("UIDailyGiftDetails.PalPay.PnlDetail.BtnPay")]
        public Button btnPay;
        public TextMeshProUGUI txtPrice;
        [Tooltip("UIDailyGiftDetails.PalPay.PnlLeft")]
        public Image imgGem;
        public TextMeshProUGUI txtGem;
        public TextMeshProUGUI txtGemAmount;
        [Tooltip("UIDailyGiftDetails.PalPay.PnlRight")]
        public Image imgChest;
        public TextMeshProUGUI txtChest;
        public TextMeshProUGUI txtChestAmount;
        public Button btnDetail;
        [Tooltip("UIDailyGiftDetails.PalPay.PnlHead.TxtTitle")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UIDailyGiftDetails.PnlChest")]
        public GameObject pnlChest;
        public Image imgMoveChest;
        public TextMeshProUGUI txtChestNum;
        [Tooltip("UIDailyGiftDetails.PnlHide")]
        public Transform pnlHide;
    }
}
