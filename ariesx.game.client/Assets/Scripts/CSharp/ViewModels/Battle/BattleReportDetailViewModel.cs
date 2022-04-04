using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class BattleReportDetailViewModel : BaseViewModel, IViewModel {
        private MailViewModel parent;
        private MailModel model;
        private BattleReportDetailView view;

        private string id;
        private string reportId;
        public string Id {
            get {
                return id;
            }
            set {
                if (id != value) {
                    id = value;
                    this.OnIdChange();
                }
            }
        }
        private string battleDetailTitle = string.Empty;

        public BattleReport BattleReport {
            get {
                return this.model.battleReportDict[this.id];
            }
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MailViewModel>();
            this.model = ModelManager.GetModelData<MailModel>();
            this.view = this.gameObject.AddComponent<BattleReportDetailView>();
        }

        public void Show(string reportId, string title) {
            this.view.PlayShow(() => {
                this.Id = reportId;
                this.battleDetailTitle = title;
            });
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        private void OnIdChange() {
            this.view.ClearInfo();
            this.BattleDetailReq(this.id);
        }

        private void BattleDetailReq(string reportId) {
            if (!reportId.CustomEquals(this.reportId)) {
                this.view.ClearChildren();
                GetBattleReportReq getBattleReportReq = new GetBattleReportReq() {
                    Id = reportId
                };
                NetManager.SendMessage(getBattleReportReq,
                    typeof(GetBattleReportAck).Name, this.BattleDetailAck);
            }
        }

        private void BattleDetailAck(IExtensible message) {
            GetBattleReportAck battleReportAck = message as GetBattleReportAck;
            this.reportId = battleReportAck.ReportId;
            this.view.SetBattleReport(battleReportAck, battleDetailTitle);
            if (!BattleReport.IsRead) {
                BattleReport.IsRead = true;
                this.parent.NewBattleCount -= 1;
            }
        }

    }
}