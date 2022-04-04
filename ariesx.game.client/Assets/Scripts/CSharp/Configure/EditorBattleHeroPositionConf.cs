using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class EditorBattleHeroPositionConf : BaseConf {
        public string heroId;
        public string point;
        public string root;
        public Vector3 offset;

        private static Dictionary<string, Dictionary<string, EditorBattleHeroPositionConf>> heroPositionDict =
            new Dictionary<string, Dictionary<string, EditorBattleHeroPositionConf>>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero"];
            this.point = this.GetPointName(attrDict["point"]);
            this.root = attrDict["root"];
            this.offset = new Vector3(
                float.Parse(attrDict["x"]),
                float.Parse(attrDict["y"]),
                float.Parse(attrDict["z"])
            );
            Dictionary<string, EditorBattleHeroPositionConf> positionDict;
            if (!(heroPositionDict.TryGetValue(this.heroId, out positionDict))) {
                positionDict = new Dictionary<string, EditorBattleHeroPositionConf>();
                heroPositionDict.Add(this.heroId, positionDict);
            }
            positionDict.Add(point, this);
        }

        public override string GetId() {
            return string.Concat(this.heroId, "_", this.point);
        }

        static EditorBattleHeroPositionConf() {

            ConfigureManager.Instance.LoadConfigure<EditorBattleHeroPositionConf>();
        }

        public static DemonTroopConf GetConf(string id) {
            return ConfigureManager.GetConfById<DemonTroopConf>(id);
        }

        public static Dictionary<string, EditorBattleHeroPositionConf>
        GetPositionDict(string heroId) {
            if (heroPositionDict.ContainsKey(heroId)) {
                return heroPositionDict[heroId];
            } else {
                return null;
            }
        }

        public static void ClearPositionDict() {
            heroPositionDict.Clear();
        }

        private string GetPointName(string origin) {
            string[] array = origin.CustomSplit('.');
            string[] rootArray = array[0].CustomSplit('_');
            string root = string.Empty;
            foreach (string slice in rootArray) {
                root = string.Concat(root, GameHelper.UpperFirstCase(slice));
            }
            if (array.Length > 1) {
                root = string.Concat(root, '.', GameHelper.UpperFirstCase(array[1]));
            }
            return root;
        }


    }
}
