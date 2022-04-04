using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class PlayerData {
        public string name;
        public int level;
    }

    public class BattleSummary {
        public int sent;
        public int kills;
        public int loss;
        public int wounded;
    }

    public class BattleReportModel : BaseModel {
        //private string WIN = "win";
        //private string LOST = "lost";

        //public string result;
        public Dictionary<string, int> resourceDict = new Dictionary<string, int>();
        public Dictionary<string, int> expDict = new Dictionary<string, int>();
        public Dictionary<string, int> itemDict = new Dictionary<string, int>();

        public List<PlayerData> playerList = new List<PlayerData>();

        public string battleSummary = "BattleSummary";

        public List<BattleSummary> summaryList = new List<BattleSummary>();

        public void Refresh(object message) {
            this.FakeRefresh();

           // BattleReport.Report report = (BattleReport.Report)message;

            //this.summaryList.Add(new BattleSummary {
            //    sent = (int)report.SideASummary.Total,
            //    kills = (int)report.SideASummary.Kill,
            //    loss = (int)report.SideASummary.Losses,
            //    wounded = (int)report.SideASummary.Wounded,
            //});

            //this.summaryList.Add(new BattleSummary {
            //    sent = (int)report.SideBSummary.Total,
            //    kills = (int)report.SideBSummary.Kill,
            //    loss = (int)report.SideBSummary.Losses,
            //    wounded = (int)report.SideBSummary.Wounded,
            //});
        }

        public void FakeRefresh() {
            //this.result = this.WIN;

            this.resourceDict.Add("Food", -1123);
            this.resourceDict.Add("Water", 2000);
            this.resourceDict.Add("Wood", 400);
            this.resourceDict.Add("Energy", 10120);

            this.expDict.Add("Exp", 100);
            this.expDict.Add("Honor", 100);

            this.itemDict.Add("Item1", 1);
            this.itemDict.Add("Item2", 1);
            this.itemDict.Add("Item3", 1);
            this.itemDict.Add("Item4", 1);
            this.itemDict.Add("Item5", 1);
            this.itemDict.Add("Item6", 1);
            this.itemDict.Add("Item7", 1);
            this.itemDict.Add("Item8", 1);
            this.itemDict.Add("Item9", 1);
            this.itemDict.Add("Item10", 1);


            this.playerList.Add(new PlayerData {
                name = "Roy",
                level = 1
            });

            this.playerList.Add(new PlayerData {
                name = "AlckLee",
                level = 1
            });

            //this.summaryList.Add(new BattleSummary {
            //    sent = 2000,
            //    kills = 700,
            //    loss = 1300,
            //    wounded = 700
            //});

            //this.summaryList.Add(new BattleSummary {
            //    sent = 1000,
            //    kills = 1700,
            //    loss = 300,
            //    wounded = 700
            //});
        }
    }
}
