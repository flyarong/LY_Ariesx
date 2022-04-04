using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class BattleAction : MonoBehaviour {
        public class ActionEvent : UnityEvent<RoleAction> { }
        private UnityAction<RoleAction> onActionEnd;
        private UnityAction<RoleAction> onActionPlay;
        private UnityAction<RoleAction> onAudioPlay;
        private UnityAction<int> onStateEnter;
        private UnityAction<int> onStateExist;
        
        public UnityAction<RoleAction> OnActionEnd {
            set {
                this.onActionEnd = value;
            }
        }

        public UnityAction<RoleAction> OnActionPlay {
            set {
                this.onActionPlay = value;
            }
        }

        public UnityAction<RoleAction> OnAudioPlay {
            set {
                this.onAudioPlay = value;
            }
        }

        public UnityAction<int> OnStateEnter {
            set {
                this.onStateEnter = value;
            }
        }

        public UnityAction<int> OnStateExist {
            set {
                this.onStateExist = value;
            }
        }

        public void ActionEnd(RoleAction action) {
            this.onActionEnd.InvokeSafe(action);
        }

        public void ActionPlay(RoleAction action) {
            this.onActionPlay.InvokeSafe(action);
        }

        public void AudioPlay(RoleAction action) {
            this.onAudioPlay.InvokeSafe(action);
        }

        public void StateEnter(int nameHash) {
            this.onStateEnter.InvokeSafe(nameHash);
        }

        public void StateExist(int nameHash) {
            this.onStateExist.InvokeSafe(nameHash);
        }
    }
}
