using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class HeroView : BaseView {
        private HeroViewModel viewModel;
        private HeroViewPreference viewPref;

        //void Awake() {
        //    this.viewModel = this.GetComponent<HeroViewModel>();
        //}


        /*****************************************************************/
        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<HeroViewModel>();
            this.ui = UIManager.GetUI("UIHero");
            this.viewPref = this.ui.transform.GetComponent<HeroViewPreference>();

            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(0,
                new TabInfo(LocalManager.GetValue(LocalHashConst.hero_title_hero), null,
                (state) => OnToggleHeroClick(state)));
            this.viewPref.tabView.SetTab(1,
                new TabInfo(LocalManager.GetValue(LocalHashConst.title_treasure_chest), null,
                (state) => OnToggleLotteryClick(state)));
            this.viewPref.tabView.SetAllOff();
            this.viewPref.tabView.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        public void Init() {
            float heroCount = this.viewModel.HeroDict.Count;
            UIManager.ShowUI(this.viewPref.pnlHero.gameObject);
            this.viewPref.tabView.SetAllOff();
            if (this.viewModel.ViewType == HeroSubViewType.Info) {
                this.viewModel.ShowInfo();
                UIManager.HideUI(this.viewPref.pnlHero.gameObject);
            } else if (this.viewModel.ViewType == HeroSubViewType.All) {
                this.viewPref.tabView.SetActiveTab(0);
            } else if (this.viewModel.ViewType == HeroSubViewType.Lottery ||
                this.viewModel.Lottery) {
                this.viewPref.tabView.SetActiveTab(1);
            } else {
                this.viewPref.tabView.SetActiveTab(0);
            }
        }

        public override void PlayShow(UnityAction action) {
            base.PlayShow(action, true);
            SetHeroTabPointVisible();
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(() => {
                this.viewPref.tabView.SetAllOff();
                callback.InvokeSafe();
            });
        }

        public void SetHeroInfoPoint() {
            this.SetLotteryInfoPointVisible();
            this.SetHeroListPointView();
        }

        private void OnToggleHeroClick(bool state) {
            this.viewModel.ShowHeroList(state);
            this.SetLotteryInfoPointVisible();
        }

        private void OnToggleLotteryClick(bool state) {
            this.viewModel.ShowLotteryList(state);
            this.SetHeroListPointView();
        }

        private void SetHeroListPointView() {
            this.viewPref.tabView.SetColorPointVisible(
                0, false, TabNoticePointType.Red);
            this.viewPref.tabView.SetColorPointVisible(
                0, false, TabNoticePointType.Green);
            if (this.viewModel.NewHeroCount > 0) {
                this.viewPref.tabView.SetColorPointVisible(
                    0, true, TabNoticePointType.Red);
                return;
            }

            if (this.viewModel.CanLevelUpCount > 0) {
                this.viewPref.tabView.SetColorPointVisible(
                    0, true, TabNoticePointType.Green);
            }
        }

        private void SetLotteryInfoPointVisible() {
            this.viewPref.tabView.SetColorPointVisible(
                1, this.viewModel.FreeLotteryCount > 0,
                TabNoticePointType.Yellow);
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        public void OnBackGroundClick(Transform pnl) {
            //pnl.gameObject.SetActiveSafe(false);
            if (this.viewModel.ViewType == HeroSubViewType.Info) {
                this.viewModel.Hide();
            }
        }

        public void SetAsLastSibling() {
            this.ui.transform.SetAsLastSibling();
        }

        #region FTE

        public void OnFteStep111Start() {
            this.afterShowCallback = () => {
                this.viewModel.OnFteStep111Process();
            };
        }

        public void OnHeroStep1Start() {
            if (!this.IsVisible) {
                //Debug.LogError("Hero Step 1 Start");
                this.afterShowCallback = () => {
                    this.viewModel.OnHeroStep1Process();
                };
            } else {
                this.viewModel.OnHeroStep1Process();
            }
        }

        #endregion        

        public void SetHeroTabPointVisible() {
            this.InitUI();
            if (this.viewModel.NewHeroCount > 0) {
                this.viewPref.tabView.SetPointVisible(0, true, this.viewModel.NewHeroCount);
                this.viewPref.tabView.SetColorPoint(0, TabNoticePointType.Red);
                return;
            }
            if (this.viewModel.CanLevelUpCount > 0) {
                this.viewPref.tabView.SetPointVisible(0, true, this.viewModel.CanLevelUpCount);
                this.viewPref.tabView.SetColorPoint(0, TabNoticePointType.Green);
                return;
            }
            this.viewPref.tabView.SetPointVisible(0, false);
        }
    }
}
