using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public class CampaignRewardViewModel: BaseViewModel, IViewModel {
        private MapViewModel parent;
        private CampaignRewardView view;
        private CampaignModel model;
        /**********************/
        public Activity ChoosedActivity {
            get {
                return this.model.chosenActivity;
            }
        }
        /* Other members */

        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<CampaignRewardView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.PlayShow();
            }
            this.view.SetContent();
        }
        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
        }

        public void ShowHeroInfo(string heroName) {
            this.parent.ShowHeroInfo(heroName, infoType: HeroInfoType.Unlock, isSubWindow: true);
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }
    }
}
