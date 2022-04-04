using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class AudioSKillConf : BaseConf {
        public string skillId;
        public string cast;
        public string hit;
        public string move;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.skillId = attrDict["skill_id"];
            this.cast = attrDict["audio_skill_cast"];
            this.hit = attrDict["audio_skill_hit"];
            this.move = attrDict["audio_skill_move"];
        }

        public override string GetId() {
            return this.skillId;
        }

        static AudioSKillConf() {
            ConfigureManager.Instance.LoadConfigure<AudioSKillConf>();
        }

        public static AudioSKillConf GetConf(string id) {
            return ConfigureManager.GetConfById<AudioSKillConf>(id);
        }
    }
}