using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class MiniMapCityConf : BaseConf {
        public string id;
        public Vector2 coordinate;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.coordinate = new Vector2(
                int.Parse(attrDict["x"]),
                int.Parse(attrDict["y"])
            );
        }

        public override string GetId() {
            return this.id;
        }

        static MiniMapCityConf() {
            ConfigureManager.Instance.LoadConfigure<MiniMapCityConf>();
        }

        public static MiniMapCityConf GetConf(string id) {
            return ConfigureManager.GetConfById<MiniMapCityConf>(id);
        }
    }
}
