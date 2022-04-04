using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class PayCountersignViewPreference : BaseViewPreference {
        PayCountersignViewPreference() {
            //this.path = "E:/ariesx.game.client/Assets/Scripts/CSharp/Views/pay/PayCountersignViewPreference.cs";
        }
        
       [Tooltip("UIPayCountersign/PnlPay/PnlHead/BtnClose")]
        public Button btnClose;
        
       [Tooltip("UIPayCountersign/BtnBackground")]
        public CustomButton background;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlDetail/BtnPay")]
        public Button btnPay;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlDetail/BtnPay/Txt")]
        public TextMeshProUGUI txtPrice;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlHead/TxtTitle")]
        public TextMeshProUGUI txtTitle;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlDetail/PnlImg/Image/Image/TxtNum")]
        public TextMeshProUGUI txtNum;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlDetail/PnlImg/Image/Image/Image")]
        public Image ImgGold;
        
       [Tooltip("UIPayCountersign/PnlPay/PnlTip")]
        public GameObject pnlTip;
    }
}
