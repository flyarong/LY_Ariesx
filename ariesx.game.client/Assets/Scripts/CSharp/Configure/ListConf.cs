using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class ListConf : BaseConf {
        public string name;
        public int amount;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["file"];
            this.amount = int.Parse(attrDict["amount"]);
        }

        public override string GetId() {
            return this.name;
        }

        public static int GetFileAmount(string name) {
            ListConf listConf = ConfigureManager.GetConfById<ListConf>(name);
            if (listConf != null) {
                return listConf.amount;
            } else {
                Debug.LogErrorf("No such configure file {0}", name);
                return 0;
            }
        }
    }
}
