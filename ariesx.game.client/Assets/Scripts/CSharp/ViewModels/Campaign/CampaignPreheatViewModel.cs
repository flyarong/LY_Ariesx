using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class CampaignPreheatViewModel : BaseViewModel, IViewModel {
        private CampaignViewModel parent;
        private CampaignPreheatView view;
        private CampaignModel model;
        /**********************/
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }

        public List<Domination> DominationRewardList {
            get {
                return this.model.dominationRewardList;
            }
            set {
                if (value != null && value.Count > 0) {
                    this.model.dominationRewardList.Clear();
                    this.model.dominationRewardList.AddRange(value);
                }
            }
        }

        public long CampaigRemainTime {
            get; set;
        }
        public Dictionary<Resource, int> CampaignRewards = new Dictionary<Resource, int>(5);
        /* Other members */

        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<CampaignPreheatView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show(CampaignType type) {
            this.CampaigRemainTime =
                this.ChosenActivity.Base.StartTime - RoleManager.GetCurrentUtcTime() / 1000;
            this.view.Show();
            this.view.SetContent(type);
            UpdateManager.Unregist(UpdateInfo.CampaignPreheat);
            if (this.CampaigRemainTime > 0) {
                UpdateManager.Regist(UpdateInfo.CampaignPreheat,
                    this.view.SetCampaignRemainTimeInfo);
            }
        }

        public void Hide() {
            //Debug.LogError("Preheat Hide");
            if (this.view.IsVisible) {
                this.view.Hide();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }


        public void ShowCampaignRewards() {
            this.parent.ShowCampaignRewards();
        }
        public void ShowCampaignRules() {
            this.parent.ShowCampaignRules(
                this.ChosenActivity.GetActivityBody());
        }

        public void ShowDomination() {
            this.parent.ShowCampaignRewardDomination();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        // Campaign logic
        public void OnCampaignPreheatDone() {
            this.Hide();
            this.parent.OnCampaignPreheatDone();
        }
    }
}
