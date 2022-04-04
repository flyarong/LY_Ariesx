using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class BuildUnlockViewModel : BaseViewModel {
        private BuildUnlockView view;
        private MapViewModel parent;
        /* Model data get set */

        /**********************/

        /* Other members */
        UnityAction unlockCallback = null;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<BuildUnlockView>();
        }

        public void Show(UnityAction callback, bool needTip) {
            this.unlockCallback = callback;
            this.view.PlayShow();
            this.view.SetContent(needTip);
        }
                
        public void Hide() {
            this.view.PlayHide();
        }

        public void ShowPayOnClick() {
            LYGameData.OnOtherOpenStore();
            this.parent.ShowPay();
            this.Hide();
        }

        /* Add 'NetMessageAck' function here*/
        public void UnlockReq() {
            if (RoleManager.GetResource(Resource.Gem) < GameConst.BUILD_QUEUE_COST) {
                UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.gem_short), TipType.Warn);
                this.view.EnableBtnUnlockClick();
                return;
            }
            PayExtraBuildQueueReq req = new PayExtraBuildQueueReq();
            NetManager.SendMessage(req, typeof(PayExtraBuildQueueAck).Name,
                UnlockAck, (message) => this.view.EnableBtnUnlockClick(), 
                () => { this.view.EnableBtnUnlockClick(); });
        }

        public void UnlockAck(IExtensible message) {
            UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.unlock_build_queue_tips), TipType.Notice);
            EventBuildClient.maxQueueCount = 2;
            this.view.EnableBtnUnlockClick();
            this.Hide();
            this.unlockCallback.InvokeSafe();
        }
        /***********************************/
    }
}
