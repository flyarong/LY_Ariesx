using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class ScreenEffectViewPreference : BaseViewPreference {
        
       [Tooltip("UIScreenEffect/PnlShow")]
        public Transform pnlShow;
        
       [Tooltip("UIScreenEffect/PnlClickable")]
        public Transform pnlClickable;
        
       [Tooltip("UIScreenEffect/PnlShow/ImgFrame")]
        public Image imgFrame;
        
       [Tooltip("UIScreenEffect/PnlShow/ImgBlack")]
        public Image imgBlack;
        
       [Tooltip("UIScreenEffect/PnlScreenClickable")]
        public Button btnScreenClickable;
        
        [Tooltip("UIScreenEffect")]
        public Canvas canvas;
    }
}
