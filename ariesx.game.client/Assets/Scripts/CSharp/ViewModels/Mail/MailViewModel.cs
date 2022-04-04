using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class MailViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private MailModel model;
        private MailView view;

        /* Model data get set */

        /* Other members */
        private BattleReportDetailViewModel BattleDetail {
            get {
                if (this.battleDetail == null) {
                    this.battleDetail =
                        PoolManager.GetObject<BattleReportDetailViewModel>(this.transform);
                }
                return this.battleDetail;
            }
        }
        private MailSystemViewModel systemViewModel;
        private BattleReportDetailViewModel battleDetail;
        private BattleReportViewModel battleReportViewModel;

        public int NewCount {
            get {
                return this.NewBattleCount + this.NewSystemCount;
            }
        }

        public int NewBattleCount {
            get {
                return this.model.newBattleCount;
            }
            set {
                if (value >= this.model.newBattleCount) {
                    newBattleReport = true;
                }
                this.model.newBattleCount = value;
                this.SetNewCount();
            }
        }

        public int NewSystemCount {
            get {
                return this.model.newSystemCount;
            }
            set {
                if (value >= this.model.newSystemCount) {
                    newSystemReport = true;
                }
                this.model.newSystemCount = value;
                this.SetNewCount();
            }
        }
        private bool newBattleReport = false;
        private bool newSystemReport = false;
        public bool NeedRefresh { get; set; }
        public bool isPlayingBattle = false;

        /*********************************************************************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<MailModel>();
            this.view = this.gameObject.AddComponent<MailView>();
            this.battleReportViewModel =
                PoolManager.GetObject<BattleReportViewModel>(this.transform);
            this.systemViewModel = PoolManager.GetObject<MailSystemViewModel>(this.transform);


            NetHandler.AddNtfHandler(typeof(NewBattleReportCountNtf).Name,
                this.NewBattleReportCountNtf);
            NetHandler.AddNtfHandler(typeof(NewSystemMessageCountNtf).Name,
                this.NewSystemMessageCountNtf);
            TriggerManager.Regist(Trigger.PlayBattleReportStart, this.OnBattlePlayStart);
            TriggerManager.Regist(Trigger.PlayBattleReportDone, this.OnBattlePlayDone);
        }

        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
                this.view.SetMail();
            });
        }

        public void ShowFirstBattleReport() {
            this.battleReportViewModel.ShowFirstReport = true;
            this.Show();
        }

        public void ShowOpenChest(List<LotteryResult> results, UnityAction callback) {
            this.parent.ShowOpenChestView(results, callback);
        }

        public void PlayBattlReport(string reportId, UnityAction endCallback) {
            this.battleReportViewModel.PlayBattle(reportId, endCallback);
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide(() => {
                    this.parent.OnRemoveViewAboveMap(this);
                });
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.isPlayingBattle = false;
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void OnHeroClick(string heroName) {
            this.parent.ShowHeroInfo(heroName, infoType: HeroInfoType.Unlock);
        }

        private void SetNewCount() {
            this.parent.SetBtnMail(this.NewCount);
            if (this.newBattleReport) {
                this.battleReportViewModel.NewBattleReport();
                this.newBattleReport = false;
            }
            if (this.newSystemReport) {
                this.systemViewModel.NewSystemMail();
                this.newSystemReport = false;
            }
            if (this.view.IsVisible) {
                this.view.SetNewPoint();
            }
        }

        public void ShowBattleReport(bool visible) {
            if (visible) {
                this.battleReportViewModel.Show();
            } else {
                this.battleReportViewModel.Hide();
            }
        }

        public void ShowBattleReportDetail(string id, string title) {
            this.BattleDetail.Show(id, title);
        }

        private void HideBattleReportDetail() {
            this.BattleDetail.Hide();
        }

        public void ShowSystemPost(bool visible) {
            if (visible) {
                this.systemViewModel.Show();
            } else {
                this.systemViewModel.Hide();
            }
        }

        public void MoveToBattlePoint(Vector2 coordinate) {
            this.parent.MoveToBattlePoint(coordinate);
        }

        public void StartBattle() {
            ModelManager.LoadScene("Scene3DBattle", true);
        }

        public void HideAllSubView() {
            this.HideBattleReportDetail();
        }

        public void RefreshResouce() {
            this.parent.RefreshResourceAndCurrency();
        }

        private void OnBattlePlayStart() {
            this.isPlayingBattle = true;
        }

        private void OnBattlePlayDone() {
            this.isPlayingBattle = false;
        }

        /* Add 'NetMessageAck' function here*/
        private void NewBattleReportCountNtf(IExtensible message) {
            NewBattleReportCountNtf newBattlePersonalMessageCountNtf =
                message as NewBattleReportCountNtf;
            this.NewBattleCount = newBattlePersonalMessageCountNtf.Count;
        }

        private void NewSystemMessageCountNtf(IExtensible message) {
            NewSystemMessageCountNtf newSystemMessageCountNtf =
                message as NewSystemMessageCountNtf;
            this.NewSystemCount = newSystemMessageCountNtf.Count;
            //Debug.LogError("this.NewSystemCount " + this.NewSystemCount);
        }
        /***********************************/
    }
}
