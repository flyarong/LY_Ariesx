using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class DailyTaskConf : BaseConf {
        public int id;
        public string type;
        public int townhallLevel;
        public string name;

        public int level;
        public int amount;
        public int force;
        public int produce;
        public int times;
        public int vitality;
        //public string localTitle;
        public string localDesc;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = int.Parse(attrDict["id"]);
            this.type = attrDict["type"];
            this.name = attrDict["name"];

            this.townhallLevel = int.Parse(attrDict["townhall_level"]);
            this.level = attrDict["level"].CustomIsEmpty() ? 0 : int.Parse(attrDict["level"]);
            this.amount = attrDict["amount"].CustomIsEmpty() ? 0 : int.Parse(attrDict["amount"]);
            this.force = attrDict["force"].CustomIsEmpty() ? 0 : int.Parse(attrDict["force"]);
            this.produce = attrDict["produce"].CustomIsEmpty() ? 0 : int.Parse(attrDict["produce"]);
            this.times = attrDict["times"].CustomIsEmpty() ? 0 : int.Parse(attrDict["times"]);
            this.vitality = int.Parse(attrDict["vitality"]);
            //this.localTitle = attrDict["locale_title"];
            this.localDesc = attrDict["locale_desc"];
        }

        public override string GetId() {
            return this.id.ToString();
        }

        static DailyTaskConf() {
            ConfigureManager.Instance.LoadConfigure<DailyTaskConf>();
        }

        public string GetContent() {
            return LocalManager.GetValue(this.localDesc);
        }

        public int GetTarget() {
            return (this.times != 0) ? this.times : (this.level + this.amount +
                    this.force + this.produce + this.times);
        }

        public static DailyTaskConf GetConf(string id) {
            return ConfigureManager.GetConfById<DailyTaskConf>(id);
        }
    }
}
