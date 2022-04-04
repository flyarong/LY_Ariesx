using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public enum CapturePointType {
        Resoure,
        Pass,
        None
    }
    public class ContinentDisputesDetailsViewModel : BaseViewModel, IViewModel {
        //private ContinentDisputesViewModel parent;
        private ContinentDisputesDetailsView view;
        private CampaignModel model;

        /* Other members */
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }
        public Activity currentActivity;
        /*****************/

        void Awake() {
            //this.parent = this.transform.parent.GetComponent<ContinentDisputesViewModel>();
            this.view = this.gameObject.AddComponent<ContinentDisputesDetailsView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.currentActivity = this.ChosenActivity;
            this.view.SetContent();
            this.GetContinentDisputesOwnPoint();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.view.HideStateRewardDetail();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        /* Add 'NetMessageAck' function here*/
        private void GetContinentDisputesOwnPoint() {
            GetCapturePersonalPointsReq capturePointsReq = new GetCapturePersonalPointsReq();
            NetManager.SendMessage(capturePointsReq, typeof(
                GetCapturePersonalPointsAck).Name, this.SetHoldGroundKing);
        }

        private void SetHoldGroundKing(IExtensible extensible) {
            GetCapturePersonalPointsAck getMonster = extensible as GetCapturePersonalPointsAck;
            if (this.ChosenActivity == this.currentActivity) {
                this.view.SetContinentDisputesOwnInfo(getMonster.CapturePersonalPoints.Points);
            }
        }
        /***********************************/
    }
}
