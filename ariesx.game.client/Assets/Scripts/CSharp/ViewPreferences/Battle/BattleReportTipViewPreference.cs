using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class BattleReportTipViewPreference : BaseViewPreference {
        [Tooltip("UIBattleReportTip.PnlBattleResult Button")]
        public Button btnPlayBattleReport;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail")]
        public Transform pnlDetail;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlLeft")]
        public Transform pnlLeft;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlLeft/ImgHead")]
        public Image imgLeftHead;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlLeft/TxtName")]
        public TextMeshProUGUI txtLeftName;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlRight")]
        public Transform pnlRight;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlRight/ImgHead")]
        public Image imgRightHead;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlRight/TxtName")]
        public TextMeshProUGUI txtRightName;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlTile")]
        public Transform pnlTile;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlTile/TxtName")]
        public TextMeshProUGUI txtTile;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlDefend")]
        public Transform pnlDefend;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlAttack")]
        public Transform pnlAttack;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlTroop/PnlArrive")]
        public Transform pnlArrive;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlResource")]
        public Transform pnlResource;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/Pnltroop/PnlResource/TxtResource")]
        public TextMeshProUGUI txtResource;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/PnlResult/TxtVictory")]
        public Transform txtVictory;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/PnlResult/TxtFailure")]
        public Transform txtFailure;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/PnlDetail/PnlResult/TxtArrive")]
        public Transform txtArrive;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/ImgLeft")]
        public Image imgLeft;
        
       [Tooltip("UIBattleReportTip/PnlBattleResult/ImgRight")]
        public Image imgRight;

        public AnimationCurve showCurve;
        public AnimationCurve hideCurve;
    }
}
