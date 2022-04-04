using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.SceneManagement;

namespace Poukoute {
    public class BattleSimulationReportViewModel : MonoBehaviour {
        private BattleSimulationReportView view;
        private BattleSimulationModel model;
        private BattleSimulationReportDetailViewModel detailModel;

        /* Model data get set */
        public List<BattleReport> BattleReportList {
            get {
                return this.model.battleReportList;
            }
        }

        public Dictionary<string ,BattleReport> BattleReportDic {
            get {
                return this.model.battleReportDict;
            }
        }

        /* Other members */
        public bool NeedRefresh { get; set; }

        /*****************/

        void Awake() {
            this.model = BattleSimulationModel.Instance;
            this.view = this.gameObject.AddComponent<BattleSimulationReportView>();
            this.detailModel = this.gameObject.GetComponent<BattleSimulationReportDetailViewModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            if (this.NeedRefresh) {
                this.view.Show();
            }
        }

        public void Hide() {

        }

        public void ShowBattleReportDetail(string id) {
            this.detailModel.Show(id);
        }

        public void PlayBattle(string id, UnityAction endCallback = null) {
            ModelManager.GetModelData<BattleSimulation3DModel>().LoadBattleTroop(
                new GetBattleReportAck() {
                    ReportId = id,
                    Rounds = this.model.battleReportRounds[int.Parse(id)]
                },
               this.BattleReportDic[id]
           );
            this.gameObject.AddComponent<BattleSimulation3DViewModel>();
            this.view.Hide();
        }
    }
}
