using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public enum OccupyPointType {
        Resoure,
        Bridge,
        NpcCity,
        None
    }
    public class HoldGroundKingDetailsViewModel: BaseViewModel {
        //private HoldGroundKingViewModel parent;
        private HoldGroundKingDetailsView view;
        private CampaignModel model;
        /* Model data get set */

        /**********************/

        /* Other members */
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }
        /*****************/
        void Awake() {
            //this.parent = this.transform.parent.GetComponent<HoldGroundKingViewModel>();
            this.view = this.gameObject.AddComponent<HoldGroundKingDetailsView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.view.SetContent();
            this.GetHoldGroundOwnPoint();
            this.CustomContentSizeFitterSettle();

        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.view.HideStateRewardDetail();
            }
        }
        public void CustomContentSizeFitterSettle() {
            this.view.CustomContentSizeFitterSettle();
        }

        private void GetHoldGroundOwnPoint() {
            GetOccupySelfRankReq OccupyRankReq = new GetOccupySelfRankReq();
            NetManager.SendMessage(OccupyRankReq, typeof(
                GetOccupySelfRankAck).Name, this.OnGetHoldGroundKingAck);
        }

        private void OnGetHoldGroundKingAck(IExtensible extensible) {
            GetOccupySelfRankAck getOccupy = extensible as GetOccupySelfRankAck;
            this.view.SetHoldGroundKingOwnInfo(getOccupy.Self.Points);
        }

        /* Add 'NetMessageAck' function here*/
        
        /***********************************/

    }
}
