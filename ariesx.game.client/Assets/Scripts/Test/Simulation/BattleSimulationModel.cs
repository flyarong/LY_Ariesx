using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class BattleSimulationModel {

        private static BattleSimulationModel instance;
        public static BattleSimulationModel Instance {
            get {
                if (instance == null) {
                    instance = new BattleSimulationModel();
                }
                return instance;
            }
        }
        /* Add data member in this */
        public List<string> heroList = new List<string>();
        public List<BattleReport> battleReportList = new List<BattleReport>();
        public Dictionary<string, BattleReport> battleReportDict = new Dictionary<string, BattleReport>();
        public Dictionary<int, Battle.ReportRounds> battleReportRounds = new Dictionary<int, Battle.ReportRounds>();
        /***************************/


        public void Refresh(object message) {
            /* Refresh your data in this function */
            foreach (string model in HeroBattleConf.heroDict.Keys) {
                this.heroList.Add(model);
            }
            this.heroList.Sort((a1, a2) => {
                return a1.CompareTo(a2);
            });
        }
    }
}
