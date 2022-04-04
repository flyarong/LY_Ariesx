using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class BattleSimulationStatViewPreference : BaseViewPreference {
        public Transform[] pnlAttTroops;
        public Transform[] pnlDefTroops;
        public Text[] txtAttTroops;
        public Text[] txtDefTroops;
        public Text txtAttStat;
        public Text txtDefStat;
        public CustomButton btnClose;
        public CustomButton BG;
    }
}