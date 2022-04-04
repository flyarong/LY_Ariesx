using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class AudioHeroConf : BaseConf {
        public string heroId;
        public string attack;
        public string skill;
        public string show;
        public string hit;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero_id"];
            this.show = attrDict["audio_new"];
            this.attack = attrDict["audio_attack"];
            this.skill = attrDict["audio_skill"];
            this.hit = attrDict["audio_hit"];
        }

        public override string GetId() {
            return this.heroId;
        }

        static AudioHeroConf() {
            ConfigureManager.Instance.LoadConfigure<AudioHeroConf>();
        }

        public static AudioHeroConf GetConf(string id) {
            return ConfigureManager.GetConfById<AudioHeroConf>(id);
        }

        //public void Attack() {
        //    AudioManager.Play(
        //        this.attack,
        //        AudioType.Action,
        //        AudioVolumn.High,
        //        isAdditive: true
        //    );
        //}

        //public void Hit() {
        //    AudioManager.Play(
        //        this.hit,
        //        AudioType.Action,
        //        AudioVolumn.High,
        //        isAdditive: true
        //    );
        //}

        //public void Skill() {
        //    AudioManager.Play(
        //        "rush",
        //        AudioType.Action,
        //        AudioVolumn.High,
        //        isAdditive: true
        //    );
        //}
    }
}