using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.SceneManagement;

namespace Poukoute {
    public class BattleReportViewModel : BaseViewModel {
        private MailViewModel parent;
        private MailModel model;
        private BattleReportView view;

        /* Model data get set */
        public Dictionary<string, BattleReport> BattleReportDict {
            get {
                return this.model.battleReportDict;
            }
        }

        public List<BattleReport> BattleReportList {
            get {
                return this.model.battleReportLsit;
            }
        }

        public MailChannel Channel {
            get {
                return this.model.battle;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.battle.isLoadAll;
            }
            set {
                this.model.battle.isLoadAll = value;
            }
        }

        public string CurrentBattle {
            get; set;
        }

        public bool ShowFirstReport = false;
        /**********************/

        /* Other members */
        public bool NeedRefresh { get; set; }
        public bool IsVisible { get { return this.view.IsVisible; } }
        private bool isRequestPage = false;
        private bool isPlayingBattle = false;
        private UnityAction onPlayBattleEnd = null;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<BattleReportView>();
            this.parent = this.transform.parent.GetComponent<MailViewModel>();
            this.model = ModelManager.GetModelData<MailModel>();
            this.NeedRefresh = true;
            TriggerManager.Regist(Trigger.PlayBattleReportDone, this.OnBattlePlayDone);
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Channel.page = 0;
                this.BattleReportListReq();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {// && !this.isPlayingBattle) {
                this.view.Hide();
                this.NeedRefresh = true;
            }
        }

        protected override void OnReLogin() {
            this.NewBattleReport();
        }

        public void ShowBattleReportDetail(string id, string title) {
            if (this.isPlayingBattle) {
                return;
            }
            this.parent.ShowBattleReportDetail(id, title);
        }

        // To do: unify show after request or request after show.
        public void PlayBattle(string id, UnityAction endCallback = null) {
            if (!this.BattleReportDict.ContainsKey(id)) {
                this.RefreshFirstPageReports(() => {
                    this.InnerPlayBattle(id, endCallback);
                });
            } else {
                this.InnerPlayBattle(id, endCallback);
            }
        }

        private void RefreshFirstPageReports(UnityAction callback) {
            GetBattleReportsReq getBattleReportsReq = new GetBattleReportsReq() {
                Page = 1
            };
            NetManager.SendMessage(getBattleReportsReq,
                typeof(GetBattleReportsAck).Name, (message) => {
                    GetBattleReportsAck battleReportList = message as GetBattleReportsAck;
                    foreach (BattleReport battleReport in battleReportList.Reports) {
                        if (!this.BattleReportDict.ContainsKey(battleReport.Id)) {
                            this.BattleReportList.Add(battleReport);
                            this.BattleReportDict[battleReport.Id] = battleReport;
                        }
                    }

                    callback.InvokeSafe();
                });
        }

        private void InnerPlayBattle(string id, UnityAction endCallback) {
            if (this.isPlayingBattle) {
                return;
            }
            this.onPlayBattleEnd += endCallback;
            this.isPlayingBattle = true;
            this.BattleDetailReq(id);
        }

        private void BattleDetailReq(string id) {
            this.CurrentBattle = id;
            GetBattleReportReq getBattleReportReq = new GetBattleReportReq() {
                Id = id
            };
            NetManager.SendMessage(
                getBattleReportReq,
                typeof(GetBattleReportAck).Name,
                (message) => { this.BattleDetailAck(message, id); },
                (message) => { isPlayingBattle = false; },
                () => { isPlayingBattle = false; }
            );
        }

        // To do: Need a cache.
        private void BattleDetailAck(IExtensible message, string id) {
            GetBattleReportAck battleReportAck = message as GetBattleReportAck;
            if (battleReportAck.ReportId != this.CurrentBattle) {
                return;
            }
            ModelManager.GetModelData<BattleModel>().LoadBattleTroop(
                battleReportAck,
                this.BattleReportDict[id]
            );
            if (!this.parent.isPlayingBattle) {
                TriggerManager.Invoke(Trigger.PlayBattleReportStart);
                Coord coord = this.BattleReportDict[id].Report.PointInfo.Coord;
                this.parent.MoveToBattlePoint(new Vector2(coord.X, coord.Y));
                this.parent.HideImmediatly();
                //ModelManager.LoadScene("Scene3DBattle", true);
            }
        }

        public void MarkReadReq(string id) {
            MarkBattleReportReadReq markReadReq = new MarkBattleReportReadReq();
            if (!id.CustomIsEmpty()) {
                markReadReq.Ids.Add(id);
            }
            NetManager.SendMessage(markReadReq, string.Empty, null);
        }

        public void MarkAllReadReq() {
            MarkBattleReportReadReq markReadReq = new MarkBattleReportReadReq();
            NetManager.SendMessage(markReadReq, string.Empty, null);
        }

        public void BattleReportListReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            this.isRequestPage = true;
            GetBattleReportsReq getBattleReportsReq = new GetBattleReportsReq() {
                Page = ++this.Channel.page
            };
            NetManager.SendMessage(getBattleReportsReq,
                typeof(GetBattleReportsAck).Name, this.GetBattleReportsAck);
        }

        /* Add 'NetMessageAck' function here*/
        private void GetBattleReportsAck(IExtensible message) {
            GetBattleReportsAck battleReportList = message as GetBattleReportsAck;
            this.isRequestPage = false;
            if (battleReportList.Reports.Count < this.Channel.pageCount) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.BattleReportList.Clear();
                this.BattleReportDict.Clear();
            }
            int originCount = this.BattleReportList.Count;
            int newDataCount = battleReportList.Reports.Count;
            foreach (BattleReport battleReport in battleReportList.Reports) {
                if (!this.BattleReportDict.ContainsKey(battleReport.Id)) {
                    this.BattleReportList.Add(battleReport);
                    this.BattleReportDict[battleReport.Id] = battleReport;
                }
#if UNITY_EDITOR
                else {
                    Debug.LogError(battleReport.Id);
                }
#endif
            }

            if (this.Channel.page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.BattleReportListReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        public void NewBattleReport() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void OnBattlePlayDone() {
            this.isPlayingBattle = false;
            this.onPlayBattleEnd.InvokeSafe();
            this.onPlayBattleEnd = null;
        }
        /***********************************/
    }
}
