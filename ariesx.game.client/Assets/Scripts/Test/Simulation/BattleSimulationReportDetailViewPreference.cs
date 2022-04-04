using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class BattleSimulationReportDetailViewPreference : BaseViewPreference {
        [Tooltip("UIMail.PnlBattleDetail.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIMail.PnlBattleDetail.PnlHead.BtnClose")]
        public Button btnClose;

        [Tooltip("UIMail.PnlBattleDetail.PnlArmy.PnlAttacker")]
        public Transform pnlAttacker;
        public CanvasGroup attackerCG;
        [Tooltip("UIMail.PnlBattleDetail.PnlArmy.PnlDefender")]
        public Transform pnlDefender;
        public CanvasGroup defenderCG;
        [Tooltip("UIMail.PnlBattleDetail.PnlTitle.TxtTitle")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UIMail.PnlBattleDetail.PnlContent.PnlRoundList")]
        public Transform pnlRoundList;
    }
}
