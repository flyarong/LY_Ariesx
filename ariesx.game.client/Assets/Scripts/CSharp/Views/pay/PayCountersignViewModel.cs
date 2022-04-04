using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine;

namespace Poukoute {
    public class PayCountersignViewModel : BaseViewModel {
        private PayViewModel parent;
        private PayCountersignView view;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<PayViewModel>();
            this.view = this.gameObject.AddComponent<PayCountersignView>();

        }

        public void Show( GoldProductConf productConf) {
            this.view.PlayShow();
            this.view.SetProductConf(productConf);
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void SetChangeGem(int gem) {
            this.parent.SetChangeGem(gem);
        }

        public void RefreshHouseKeeperEvent() {
            this.parent.RefreshHouseKeeperEvent();
        }
       //

    }
}
