using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class ContinentDisputesStatisticsViewModel : BaseViewModel {
        private ContinentDisputesStatisticsView view;
        private ContinentDisputesViewModel parent;
        private CampaignModel model;

        /* Model data get set */
        public List<CaptureLog> captureLogList = new List<CaptureLog>(10);
        public int Page {
            get {
                return this.model.continentDisputesLogPage;
            }
            set {
                this.model.continentDisputesLogPage = value;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.continentDisputesLogIsLoadAll;
            }
            set {
                this.model.continentDisputesLogIsLoadAll = value;
            }
        }

        private bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        private int countShow = 10;


        /***********************************************************************/

        void Awake() {
            this.view = this.gameObject.AddComponent<ContinentDisputesStatisticsView>();
            this.parent = this.transform.parent.GetComponent<ContinentDisputesViewModel>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.isRequestPage = false;
            this.IsLoadAll = false;
            this.GetStatistcsReq();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.NeedRefresh = true;
                this.Page = 0;
            }
        }

        /* Add 'NetMessageAck' function here*/
        public void GetStatistcsReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            this.isRequestPage = true;
            CaptureLogsReq captureLogsReq = new CaptureLogsReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(captureLogsReq, typeof(
                CaptureLogsAck).Name, this.GetStatistcsAck);

        }

        private void GetStatistcsAck(IExtensible message) {
            CaptureLogsAck captureLogsAck = message as CaptureLogsAck;
            this.isRequestPage = false;
            if (captureLogsAck.Logs.Count < this.countShow) {
                this.IsLoadAll = true;
            }
            if (this.NeedRefresh) {
                this.captureLogList.Clear();
            }
            int originCount = this.captureLogList.Count;
            int newDataCount = captureLogsAck.Logs.Count;
            foreach (CaptureLog log in captureLogsAck.Logs) {
                this.captureLogList.Add(log);
            }

            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetStatistcsReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.parent.HideView();
            this.parent.MoveWithClick(coordinate);
        }
        /***********************************/
    }
}
