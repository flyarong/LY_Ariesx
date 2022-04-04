using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class DevilFightingDetailViewModel : BaseViewModel, IViewModel {
        private DevilFightingViewModel parent;
        private DevilFightingDetailView view;
        private CampaignModel model;
        /**********************/

        /* Other members */
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }

        public Dictionary<string, BaseConf> AllDemonTroopConfs;

        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<DevilFightingViewModel>();
            this.view = this.gameObject.AddComponent<DevilFightingDetailView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
            this.AllDemonTroopConfs = ConfigureManager.GetConfDict<DemonTroopConf>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }

            this.view.SetContent();
            this.GetDevilFightingOwnPoint();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.view.HideStateRewardDetail();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void OnCampaignGuidItemClick(int monsterLevel) {
            this.parent.GetRecentMonsterByLevel(monsterLevel);
        }

        private void GetDevilFightingOwnPoint() {
            GetMonsterPointsReq getPointsReq = new GetMonsterPointsReq();
            NetManager.SendMessage(getPointsReq,
                typeof(GetMonsterPointsAck).Name, this.OnGetMonsterPointsAck);
        }

        private void OnGetMonsterPointsAck(IExtensible message) {
            GetMonsterPointsAck monsterPointsAck = message as GetMonsterPointsAck;
            //Debug.LogError("OnGetMonsterPointsAck " + monsterPointsAck.Points);
            this.view.SetDevilFightingOwnInfo(monsterPointsAck.Points);
        }
    }
}
