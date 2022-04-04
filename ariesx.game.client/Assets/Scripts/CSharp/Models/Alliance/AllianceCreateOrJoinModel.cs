using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {

    public class SearchAllianceChannel : BaseListInfo {
        public string allianceName = string.Empty;
        public List<AllianceCache> alliancesList =
            new List<AllianceCache>();

        public SearchAllianceChannel(int pageCount, int countShow) :
            base(pageCount, countShow) { }
    }

    public class AllianceCreateOrJoinModel : BaseModel {
        public AllianceViewType allianceViewType = AllianceViewType.None;
        public SearchAllianceChannel searchAllianceChannel;
        public string SearchAllianceName;

        public string inputAllianceName = string.Empty;
        public string description = string.Empty;
        public int influnceCondition;
        public JoinConditionType joinCondition = JoinConditionType.Free;
        public int allianceEmblem = 1;
        public long rejoinAllianceFinishAt = 0;
        public int language = 1;

        public List<string> conditionList = new List<string>();
        public List<string> conditionLocalList = new List<string>();
        // to do need fix
        public List<string> influnceList =
                new List<string> {  "0", "25", "50", "155", "590", "1820", "2130",
                                    "4260", "10760", "23720", "39740", "42320",
                                    "64280","79480", "110600", "128560",
                                    "172280", "221200", "344560"};

        public AllianceCreateOrJoinModel() {
            this.searchAllianceChannel = new SearchAllianceChannel(15, 11);
            this.RefresAllianceModel();
        }

        private void RefresAllianceModel() {
            Array conditionArray = Enum.GetValues(typeof(JoinConditionType));
            foreach (var condition in conditionArray) {
                conditionList.Add(condition.ToString());
                conditionLocalList.Add(
                    LocalManager.GetValue("alliance_join_",
                    condition.ToString().ToLower()));
            }
            //Debug.LogError("RefresAllianceModel " + this.conditionLocalList.Count);
        }
    }
}
