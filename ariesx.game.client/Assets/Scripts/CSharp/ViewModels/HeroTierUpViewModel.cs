using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
//using UnityEngine.Events;

namespace Poukoute {
    public class HeroTierUpViewModel : BaseViewModel, IViewModel {
        private HeroInfoViewModel parent;
        private HeroModel model;
        private HeroTierUpView view;
        /* Model data get set */
        private bool isHideViewForBattle = false;

        public Hero CurrentHero {
            get {
                return this.model.heroDict[this.model.currentHeroName];
            }
        }
        /**********************/

        /* Other members */
        public UnityAction onAnimationEnd = null;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<HeroInfoViewModel>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<HeroTierUpView>();

            TriggerManager.Regist(Trigger.PlayBattleReportStart, this.PlayBattleReportStart);
            TriggerManager.Regist(Trigger.PlayBattleReportDone, this.PlayBattleReportDone);

            //FteManager.SetStartCallback(GameConst.NORMAL, 130, this.OnFteStep341Start);
            FteManager.SetStartCallback(GameConst.NORMAL, 130, this.OnFteStep130Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 130, this.OnFteStep130End);
        }

        public void Show() {
            this.view.Show(callback: this.view.SetLevelUp);
            if (this.isHideViewForBattle) {
                this.view.SetUIVisibleForBattle(false);
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.view.SetUieffectStop();
                this.view.SetFragmentActive(false);
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void PlayBattleReportStart() {
            if (this.view.IsVisible) {
                this.isHideViewForBattle = true;
                this.view.SetUIVisibleForBattle(false);
            }
        }

        public void PlayBattleReportDone() {
            if (this.isHideViewForBattle) {
                if (this.view.IsVisible) {
                    this.view.SetUIVisibleForBattle(true);
                }
                this.isHideViewForBattle = false;
            }
        }

        public void OnHeroLevelUpAnimDone() {
            this.parent.OnHeroLevelUpAnimDone();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        #region FTE

        public void OnFteStep341Start(string index) {
            this.onAnimationEnd = () => FteManager.EndFte(true);
        }

        public void OnFteStep130Start(string index) {
            this.view.OnFteStep130Start();
        }

        public void OnFteStep130End() {
            this.Hide();
        }

        #endregion
    }
}
