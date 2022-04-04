using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class DemonShadowHistoryRankViewModel: BaseViewModel {
        private DemonShadowHistoryRankView view;
        private MapViewModel parent;
        private CampaignModel model;
        /* Model data get set */

        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }

        /**********************/
        public RankDomination rankDominationSelf;
        public RankDomination rankDominationLast;
        /* Other members */
        public List<RankDomination> rankDominationList = new List<RankDomination>(3);

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<DemonShadowHistoryRankView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void Show(DominationHistory record) {
            if (!this.view.IsVisible) {
                this.view.PlayShow();
            }            
            this.GetDominationRankReq(record);
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
        }

        /* Add 'NetMessageAck' function here*/
        public void GetDominationRankReq(DominationHistory record) {
            GetDominationRankReq dominationHistoryReq = new GetDominationRankReq() {
                DominationId = record.DominationId
            };
            NetManager.SendMessage(dominationHistoryReq, typeof(
                GetDominationRankAck).Name, this.GetDominationRankAck);
        }
        private void GetDominationRankAck(IExtensible message) {
            GetDominationRankAck dominationRankAck = message as GetDominationRankAck;
            this.rankDominationSelf = dominationRankAck.Self;
            this.rankDominationLast = dominationRankAck.Last;
            this.rankDominationList = dominationRankAck.Other;
            this.view.SetDominationRankCount();
        }
    }
    /***********************************/
}

