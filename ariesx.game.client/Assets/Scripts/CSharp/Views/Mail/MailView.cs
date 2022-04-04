using Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class MailView : BaseView {
        private MailViewModel viewModel;
        private MailViewPreference viewPref;
        private Vector2 tabBtnClose = new Vector2(317, -59);
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<MailViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMail");
            this.viewPref = this.ui.transform.GetComponent<MailViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(0, new TabInfo(
                LocalManager.GetValue(LocalHashConst.mail_battle_report), this.viewPref.pnlBattle, (state) => {
                    OnToggleStateChange(this.viewModel.ShowBattleReport, state);
                }));
            
            this.viewPref.tabView.SetTab(1, new TabInfo(
                LocalManager.GetValue(LocalHashConst.system_mail), this.viewPref.pnlSystem, (state) => {
                    OnToggleStateChange(this.viewModel.ShowSystemPost, state);
                }));
            this.viewPref.tabView.btnClose.transform.localPosition = tabBtnClose;
            this.viewPref.tabView.SetAllOff();
        }


        public void SetMail() {
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
            this.viewPref.tabView.SetActiveTab(0);
            this.SetNewPoint();
        }

        public void SetNewPoint() {
            this.viewPref.tabView.SetPointVisible(0, this.viewModel.NewBattleCount);
            this.viewPref.tabView.SetPointVisible(1, this.viewModel.NewSystemCount);
        }

        public override void PlayShow(UnityAction action) {
            base.PlayShow(action, true);
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(() => {
                this.viewPref.tabView.SetAllOff();
                callback.InvokeSafe();
            });
        }

        private void OnToggleStateChange(UnityAction<bool> action, bool state) {
            if (state) {
                this.viewModel.HideAllSubView();
            }
            action.Invoke(state);
        }

        private void OnBtnStartClick() {
            this.viewModel.StartBattle();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
