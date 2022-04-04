using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;

namespace Poukoute {
    public class BattleSimulationReportView : MonoBehaviour {
        private BattleSimulationReportViewModel viewModel;
        private BattleSimulationReportViewPreference viewPref;
        private GameObject ui;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BattleSimulationReportViewModel>();
            InitUi();
        }

        private void InitUi() {
            this.ui = GameObject.Find("UI").transform.Find("UIBattleReport").gameObject;
            this.viewPref = this.ui.GetComponent<BattleSimulationReportViewPreference>();
            this.viewPref.btnBG.onClick.AddListener(this.Hide);
            this.viewPref.btnClose.onClick.AddListener(this.Hide);
        }


        public void Show() {
            this.ui.gameObject.SetActive(true);
            this.CreateBattleReportItemView();
        }

        public void Hide() {
            this.ui.gameObject.SetActive(false);
        }

        public void CreateBattleReportItemView() {
            GameHelper.ClearChildren(this.viewPref.pnlList);
            for (int i = 0; i < this.viewModel.BattleReportList.Count; i++) {
                int index = i;
                GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlBattleSimulationReportItem, this.viewPref.pnlList);
                BattleSimulationReportItemView itemView = itemObj.GetComponent<BattleSimulationReportItemView>();
                this.OnItemContentChange(itemView,
                    this.viewModel.BattleReportList[index]);
            }
        }

        private void OnItemContentChange(BattleSimulationReportItemView itemView, BattleReport itemData) {
            itemView.BattleReport = itemData;
            string id = itemData.Id;
            itemView.OnBtnDetailClick.AddListener(() => {
                if (!itemView.IsRead) {
                    itemView.IsRead = true;
                }
                this.OnBtnBattleReportClick(id);
            });
            itemView.OnBtnPlayClick.AddListener(() => {
                if (!itemView.IsRead) {
                    itemView.IsRead = true;
                }
                this.OnBtnBattleReportPlay(id);
            });

            itemView.OnBtnItemClick.AddListener(() => {
                this.OnBtnBattleReportItemClick(id, itemView);
            });
        }

        private void OnBtnBattleReportClick(string id) {
            this.viewModel.ShowBattleReportDetail(id);
            this.ui.gameObject.SetActive(true);
        }

        private void OnBtnBattleReportPlay(string id) {
            this.viewModel.PlayBattle(id);
        }

        private void OnBtnBattleReportItemClick(string id, BattleSimulationReportItemView itemView) {
            if (!itemView.IsRead) {
                itemView.IsRead = true;
            }
        }
    }
}
