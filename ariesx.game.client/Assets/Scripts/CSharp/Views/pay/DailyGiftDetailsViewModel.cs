using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine;

namespace Poukoute {
    public class DailyGiftDetailsViewModel : BaseViewModel {
        private PayViewModel parent;
        private DailyGiftDetailsView view;
        private PayPduItemView payPduItemView;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<PayViewModel>();
            this.view = this.gameObject.AddComponent<DailyGiftDetailsView>();
           
        }

        public void Show(GachaGroupConf lotteryConf, ProductConf productConf,PayPduItemView pduItem) {
            this.view.PlayShow();
            this.view.SetProductConf(productConf);
            this.view.SetChest(lotteryConf);
            this.payPduItemView = pduItem;
        }

        public void Buy() {
            this.payPduItemView.Buy();
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void ShowHeroPool(string groupName, string building = null) {
            this.parent.ShowHeroPool(groupName, building);
        }

        //public void GetChests(List<Chest> chests) {
        //    this.parent.GetChests(chests);
        //}

        //public void SetStoreInfo() {
        //    this.parent.GetStoreInfo();
        //}
    }
}
