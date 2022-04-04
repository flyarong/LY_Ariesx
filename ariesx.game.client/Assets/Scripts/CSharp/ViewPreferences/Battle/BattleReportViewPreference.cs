using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BattleReportViewPreference : BaseViewPreference {
        [Tooltip("UIMail.PnlMail.PnlContent.PnlBattle CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMail.PnlMail.PnlContent.PnlBattle.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public CustomButton btnRead;
    }
}
