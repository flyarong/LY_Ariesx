using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;

namespace Poukoute {
    public class EditorBattleCurveConf : BaseConf {
        public string id;
        public List<Keyframe> frameList = new List<Keyframe>();
        public Vector3 origin;
        public Vector3 target;
        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.SetKeyFrameList(attrDict["frames"]);

            string[] originArray = attrDict["origin"].CustomSplit(',');
            this.origin = new Vector3(
                float.Parse(originArray[0]),
                float.Parse(originArray[1]),
                float.Parse(originArray[2])
            );

            string[] targetArray = attrDict["target"].CustomSplit(',');
            this.target = new Vector3(
                float.Parse(targetArray[0]),
                float.Parse(targetArray[1]),
                float.Parse(targetArray[2])
            );
        }

        public override string GetId() {
            return id;
        }

        static EditorBattleCurveConf() {
            ConfigureManager.Instance.LoadConfigure<EditorBattleCurveConf>();
        }

        private void SetKeyFrameList(string frames) {
            string[] frameParams = frames.CustomSplit('|');
            foreach (string frameParam in frameParams) {
                string[] values = frameParam.CustomSplit(',');
                Keyframe frame = new Keyframe();
                frame.value = float.Parse(values[0]);
                frame.time = float.Parse(values[1]);
                frame.inTangent = float.Parse(values[2]);
                frame.outTangent = float.Parse(values[3]);
                this.frameList.Add(frame);
            }
        }
    }
}
