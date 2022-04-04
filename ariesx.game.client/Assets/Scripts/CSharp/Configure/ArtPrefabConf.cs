using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {

    public class ArtPrefabConf {
        public Dictionary<string, string> dict = new Dictionary<string, string>();
        private static ArtPrefabConf instance;
        public static ArtPrefabConf Instance {
            get {
                if (instance == null) {
                    instance = new ArtPrefabConf();
                }
                return instance;
            }
        }

        private ArtPrefabConf() {
        }

        public void Clear() {
            this.dict.Clear();
        }

        public void SetProperty(Dictionary<string, string> attriDict) {
            dict.Add(attriDict["name"], attriDict["path"]);
        }


        private string AllianceEmblemPathPre =
            "Sprites/v4ui/ui_alliance_elbm/alliance";
        public static Sprite GetAliEmblemSprite(int allianceEmblem) {
            return PoolManager.GetSprite(
                string.Concat(Instance.AllianceEmblemPathPre, allianceEmblem), true);
        }

        public static int GetAlliEmblemCount() {
            return 21;
        }

        public static Sprite GetChestSprite(string chestName) {
            return GetSprite(chestName);
        }

        public static Sprite GetRoleMiniAvatarSprite(string roleAvatarName) {
            return GetSprite(roleAvatarName);
        }

        public static Sprite GetRoleAvatarSprite(string roleAvatarName) {
            return GetSprite(string.Concat(roleAvatarName, SpritePath.heroAvatarMiddleSuffix));
        }

        public static Sprite GetRoleHDAvatarSprite(string roleAvatarName) {
            return GetSprite(string.Concat(roleAvatarName, "_xl"));
        }

        public static Sprite GetRankIcon(int rank, TextMeshProUGUI txtRank) {
            string rankBG = "rank";
            if (rank < 4) {
                rankBG = string.Concat(rankBG, rank);
                txtRank.text = string.Empty;
            } else {
                txtRank.text = rank.ToString();
            }
            return GetSprite(rankBG);
        }

        public static Sprite GetSprite(string prefixStr, string tailStr) {
            return GetSprite(string.Concat(prefixStr, tailStr));
        }

        public static Sprite GetSprite(string name, bool needPool = true) {
            string path;
            if (Instance.dict.TryGetValue(name, out path)) {
                return PoolManager.GetSprite(path, needPool);
            } else {
                Debug.LogWarningf("No such sprite {0}", name);
                return null;
            }
        }

        public static AudioClip GetAudio(string name) {
            string path;
            if (Instance.dict.TryGetValue(name, out path)) {
                return PoolManager.GetAudio(path);
            } else {
                Debug.LogWarningf("No such audio clip with name {0}", name);
                return null;
            }
        }

        public static string GetValue(string name) {
            string path;
            if (Instance.dict.TryGetValue(name, out path)) {
                return path;
            } else {
                Debug.LogWarningf("No such art value with name {0}", name);
                return null;
            }
        }
    }
}
