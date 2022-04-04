using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public class CampaignRuleViewModel : BaseViewModel, IViewModel {
        //private MapViewModel parent;
        private CampaignRuleView view;
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
            //this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<CampaignRuleView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show(string content) {
            if (!this.view.IsVisible) {
                this.view.PlayShow();
            }

            this.view.SetContent(content);
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }        
    }
}
