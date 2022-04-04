using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class BattleSimulationReportDetailViewModel : MonoBehaviour {
        private BattleSimulationModel model;
        private BattleSimulationReportDetailView view;

        public bool NeedRefresh { get; set; }
        private string id;
        public string Id {
            get {
                return id;
            }
            set {
                if (id != value || this.NeedRefresh) {
                    id = value;
                    this.OnIdChange();
                    this.NeedRefresh = false;
                }
            }
        }
        public BattleReport BattleReport {
            get {
                return this.model.battleReportDict[this.id];
            }
        }

        void Awake() {
            this.model = BattleSimulationModel.Instance;
            this.view = this.gameObject.AddComponent<BattleSimulationReportDetailView>();
        }

        public void Show(string reportId) {
            this.view.PlayShow(() => {
                this.Id = reportId;
            });
        }

        public void Hide() {
            this.view.PlayHide();
        }

        private void OnIdChange() {
            this.view.ClearInfo();
            this.BattleDetailChange(this.id);
        }

        private void BattleDetailChange(string reportId) {
            List<Battle.Round> battleRounds = this.model.battleReportRounds[int.Parse(reportId)].Rounds;
            this.view.SetBattleReport(battleRounds);
            if (!BattleReport.IsRead) {
                BattleReport.IsRead = true;
            }
        }
    }
}