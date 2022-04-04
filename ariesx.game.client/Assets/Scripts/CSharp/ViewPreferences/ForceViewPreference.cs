using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class ForceViewPreference : BaseViewPreference {
       [Tooltip("PnlForce/PnlForceScroll")]
        public CustomScrollRect scrollRect;        
       [Tooltip("PnlForce/BtnCloseLeft")]
        public CustomButton btnCloseLeft;        
       [Tooltip("PnlForce/BtnCloseRight")]
        public CustomButton btnCloseRight;        
       [Tooltip("PnlForce/PnlForceScroll/PnlForceList")]
        public Transform pnlForceList;
    }
}
