using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class BossInfoViewModel: BaseViewModel {
        //private MapViewModel parent;
        private BossInfoView view;
        /* Model data get set */

        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<BossInfoView>();
        }

        public void Show(BossTroop boss) {
            this.view.PlayShow();
            this.view.SetBossInfo(boss);
        }

        public void Hide() {
            this.view.PlayHide();
            this.view.HideSkillDetailCG();
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
