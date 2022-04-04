using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class RoleAvatarViewModel : BaseViewModel {
        private HeroModel model;
        private RoleAvatarView view;
        /* Model data get set */
        public Dictionary<string, BaseConf> AllAvatarConfs { get; private set; }
        public Dictionary<string, BaseConf> unlockAvatarConfs = new Dictionary<string, BaseConf>();
        public Dictionary<string, BaseConf> lockAvatarConfs = new Dictionary<string, BaseConf>();
        public Dictionary<string, Hero> AllExistingHero {
            get {
                return this.model.heroDict;
            }
        }

        public int currAvatar = 0;
        public bool IsChangeAvatarSuccess { get; set; }
        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<RoleAvatarView>();
            this.AllAvatarConfs = ConfigureManager.GetConfDict<PlayerHeroAvatarConf>();
            IsChangeAvatarSuccess = false;
        }

        public void RefershAvatarConfs() {
            unlockAvatarConfs.Clear();
            lockAvatarConfs.Clear();
            foreach (var pair in this.AllAvatarConfs) {
                PlayerHeroAvatarConf avatar = pair.Value as PlayerHeroAvatarConf;
                bool isUnlockAvatar = this.AllExistingHero.ContainsKey(avatar.avatarName);
                if (isUnlockAvatar) {
                    unlockAvatarConfs.Add(pair.Key, pair.Value);
                } else {
                    lockAvatarConfs.Add(pair.Key, pair.Value);
                }
            }
        }
        public void Show() {
            this.view.PlayShow();
        }

        public void Hide(UnityEngine.Events.UnityAction callback = null) {
            this.view.PlayHide(callback);
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
