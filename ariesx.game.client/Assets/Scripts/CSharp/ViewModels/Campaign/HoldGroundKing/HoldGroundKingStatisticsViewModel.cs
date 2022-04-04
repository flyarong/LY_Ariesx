using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class HoldGroundKingStatisticsViewModel : BaseViewModel {
        private HoldGroundKingViewModel parent;
        private HoldGroundKingStatisticsView view;
        private CampaignModel model;

        /* Other members */
        public List<OccupyLog> occupyLogList = new List<OccupyLog>(10);

        public int Page {
            get {
                return this.model.holdGroundKingLogPage;
            }
            set {
                this.model.holdGroundKingLogPage = value;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.holdGroundKingLogIsLoadAll;
            }
            set {
                this.model.holdGroundKingLogIsLoadAll = value;
            }
        }

        private bool isRequestPage = false;
        private bool NeedRefresh { get; set; }
        private int CountShow = 10;

        /********************************************************/

        void Awake() {
            this.model = ModelManager.GetModelData<CampaignModel>();
            this.parent = this.transform.parent.GetComponent<HoldGroundKingViewModel>();
            this.view = this.gameObject.AddComponent<HoldGroundKingStatisticsView>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.isRequestPage = false;
            this.IsLoadAll = false;
            this.GetStatisticsReq();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.NeedRefresh = true;
                this.Page = 0;
            }
        }

        /* Add 'NetMessageAck' function here*/
        //If it finishes loading and is visible on the request page
        public void GetStatisticsReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            this.isRequestPage = true;
            OccupyLogsReq occupyLogsReq = new OccupyLogsReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(occupyLogsReq,
                typeof(OccupyLogsAck).Name, this.GetStatisticsAck);

        }

        private void GetStatisticsAck(IExtensible message) {
            OccupyLogsAck ack = message as OccupyLogsAck;
            this.isRequestPage = false;
            if (ack.Logs.Count < this.CountShow) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.occupyLogList.Clear();
            }

            int originCount = this.occupyLogList.Count;
            int newDataCount = ack.Logs.Count;
            foreach (OccupyLog log in ack.Logs) {
                this.occupyLogList.Add(log);
            }

            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetStatisticsReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        /***********************************/
        public void MoveWithClick(Vector2 coordinate) {
            this.parent.HideView();
            this.parent.MoveWithClick(coordinate);
        }
    }
}

