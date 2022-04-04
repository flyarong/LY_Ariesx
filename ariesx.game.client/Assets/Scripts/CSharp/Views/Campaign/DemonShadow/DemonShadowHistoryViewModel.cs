using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class DemonShadowHistoryViewModel: BaseViewModel {
        private DemonShadowHistoryView view;
        private DemonShadowViewModel parent;
        private CampaignModel model;
        public MapResouceInfo resourceInfo = new MapResouceInfo();
        /* Model data get set */
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }

        public int CountShow {
            get {
                return 10;
            }
        }

        public int Page {
            get {
                return this.model.dominationHistoryPage;
            }
            set {
                this.model.dominationHistoryPage = value;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.dominationHistoryIsLoadAll;
            }
            set {
                this.model.dominationHistoryIsLoadAll = value;
            }
        }

        private bool isRequestPage = false;
        public bool NeedRefresh { get; set; }
        /**********************/

        /* Other members */

        public List<DominationHistory> dominationHistoryList =
            new List<DominationHistory>();
        public Dictionary<string, DominationHistory> dominationHistoryDict =
            new Dictionary<string, DominationHistory>();

        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<DemonShadowHistoryView>();
            this.parent = this.transform.parent.GetComponent<DemonShadowViewModel>();
            this.model = ModelManager.GetModelData<CampaignModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Page = 0;
            }
            //this.view.ShowHistory();
            this.GetDominationHistoryReq();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide(this.OnInvisible);
            }
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void ShowDemonShadowHistoryRank(DominationHistory record) {
            this.parent.ShowDemonShadowHistoryRank(record);
        }

        /* Add 'NetMessageAck' function here*/
        public void GetDominationHistoryReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            this.isRequestPage = true;
            DominationHistoryReq dominationHistoryReq = new DominationHistoryReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(dominationHistoryReq, typeof(
                DominationHistoryAck).Name, this.GetDominationHistoryAck);
        }

        private void GetDominationHistoryAck(IExtensible message) {
            DominationHistoryAck dominationHistoryAck = message as DominationHistoryAck;
            Debug.LogError("Domination History Count =" + dominationHistoryAck.Info.Count);
            this.isRequestPage = false;
            if (dominationHistoryAck.Info.Count < this.CountShow) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.dominationHistoryList.Clear();
                this.dominationHistoryDict.Clear();
            }

            int originCount = this.dominationHistoryList.Count;
            int newDataCount = dominationHistoryAck.Info.Count;
            foreach (DominationHistory log in dominationHistoryAck.Info) {
                if (!this.dominationHistoryDict.ContainsKey(log.DominationId)) {
                    this.dominationHistoryList.Add(log);
                    this.dominationHistoryDict[log.DominationId] = log;
                }
            }

            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += GetDominationHistoryReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        private void OnInvisible() {
            this.isRequestPage = false;
            this.NeedRefresh = true;
            this.IsLoadAll = false;
            this.Page = 0;
        }

        private void GetDataFromServer(float pos) {
            if (pos > 1.1f) {
                this.GetDominationHistoryReq();
            }
        }

    }
    /***********************************/
}

