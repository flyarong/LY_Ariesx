using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class BattleSimulationViewPreference : MonoBehaviour {
        //Simulation
        public Transform pnlDefenders;
        public Transform[] pnlDefender;
        public Dropdown[] ddlDefHero;
        public InputField[] hpDefenderInputField;
        public InputField[] lvDefenderInputField;
        public Transform pnlAttackers;
        public Transform[] pnlAttacker;
        public Dropdown[] ddlAtkHero;
        public InputField[] hpAttakerInputField;
        public InputField[] lvAttakerInputField;

        public InputField inpBattleCount;
        public Button btnBattleStart;
        public Button btnBattleReport;
        public Button btnStatReport;

        public Transform format;
        public Transform defenders;
        public Transform[] defender;
        public Transform attakers;
        public Transform[] attaker;

        public Transform UIBattleReport;
        public Transform UIStatReport;
        public Transform UIDetailReport;
        public Transform UIMask;
        public Text txtMask;
        public Transform pnlHealth;
    }
}