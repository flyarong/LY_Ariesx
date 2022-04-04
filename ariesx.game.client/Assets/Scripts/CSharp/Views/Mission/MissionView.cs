using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class MissionView : BaseView {
        private MissionViewModel viewModel;
        private MissionViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.GetComponent<MissionViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMission");
            this.viewPref = this.ui.transform.GetComponent<MissionViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(0, new TabInfo(
                LocalManager.GetValue(LocalHashConst.chapter),
                this.viewPref.pnlDrama, (state) => {
                    this.OnToggleStateChange(TaskType.drama, state);
                }));

            this.viewPref.tabView.SetTab(1, new TabInfo(
                LocalManager.GetValue(LocalHashConst.daily_task), this.viewPref.pnlDailyTask, (state) => {
                    this.OnToggleStateChange(TaskType.daily, state);
                }));
            this.viewPref.tabView.SetAllOff();
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
        }

        public void PlayShow(int tabIndex, UnityAction callback) {
            base.PlayShow(() => {
                callback.Invoke();
                this.viewPref.tabView.SetActiveTab(tabIndex);
            }, true);
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(callback);
        }

        public void SetTabpointInfo(int dramaCount, int dailyTaskCount) {
            this.SetDramaPointInfo(dramaCount);
            this.SetDailyTaskPointInfo(dailyTaskCount);
        }

        public void SetDramaPointInfo(int dramaCount) {
            this.viewPref.tabView.SetPointVisible(0, dramaCount);
        }

        public void SetDailyTaskPointInfo(int DailyTaskCount) {
            this.viewPref.tabView.SetPointVisible(1, DailyTaskCount);
        }

        private void OnToggleStateChange(TaskType type, bool state) {
            if (state) {
                this.viewModel.ViewType = type;
            }
        }


        private void OnBtnCloseClick() {
            Debug.Log(this.viewModel.HaveDramaListArrow);
            if (this.viewModel.HaveDramaListArrow) {
                TriggerManager.Invoke(Trigger.DramaArrow);
            }
            this.viewModel.Hide();
        }

        protected override void OnInvisible() {
            this.viewPref.tabView.SetAllOff();
        }
    }
}
