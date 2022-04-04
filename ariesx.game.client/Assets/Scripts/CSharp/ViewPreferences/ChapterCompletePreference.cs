using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ChapterCompletePreference : BaseViewPreference {
        
        [Tooltip("PnlChapterComplete")]
        public Transform pnlChapterComplete;
        
       [Tooltip("PnlChapterComplete/Background")]
        public Button background;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlCard")]
        public Transform pnlCard;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlCard/TxtAmount")]
        public TextMeshProUGUI txtCard;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlGold")]
        public Transform pnlGold;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlGold/TxtAmount")]
        public TextMeshProUGUI txtGold;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlFood")]
        public Transform pnlFood;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlFood/TxtAmount")]
        public TextMeshProUGUI txtFood;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlLumber")]
        public Transform pnlLumber;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlLumber/TxtAmount")]
        public TextMeshProUGUI txtLumber;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlMarble")]
        public Transform pnlMarble;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlMarble/TxtAmount")]
        public TextMeshProUGUI txtMarble;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlSteel")]
        public Transform pnlSteel;
        
       [Tooltip("PnlChapterComplete/PnlDetail/PnlSteel/TxtAmount")]
        public TextMeshProUGUI txtSteel;
        
       [Tooltip("PnlChapterComplete/BtnReceive")]
        public CustomButton btnReceive;
    }
}
