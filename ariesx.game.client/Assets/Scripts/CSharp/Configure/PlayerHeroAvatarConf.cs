using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class PlayerHeroAvatarConf : BaseConf {
        public string avatarId;
        public string avatarName;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.avatarId = attrDict["id"];
            this.avatarName = attrDict["name"];
        }

        public override string GetId() {
            return this.avatarId;
        }

        static PlayerHeroAvatarConf() {
            ConfigureManager.Instance.LoadConfigure<PlayerHeroAvatarConf>();

        }

        public static PlayerHeroAvatarConf GetConf(string id) {
            return ConfigureManager.GetConfById<PlayerHeroAvatarConf>(id);
        }

        public static string GetAvatarName(string key) {
            PlayerHeroAvatarConf playerHeroAvatarConf = PlayerHeroAvatarConf.GetConf(key);
            return (playerHeroAvatarConf == null) ?
                GameConst.AVATAR_DEFAULT :
                playerHeroAvatarConf.avatarName;
        }

        public static string GetMiniAvatarName(string key) {
            PlayerHeroAvatarConf playerHeroAvatarConf = PlayerHeroAvatarConf.GetConf(key);
            return (playerHeroAvatarConf == null) ?
                GameConst.AVATAR_DEFAULT :
                playerHeroAvatarConf.avatarName + SpritePath.heroAvatarSmallSuffix;
        }
    }
}