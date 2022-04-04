using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
	public class CampaignRewardDominationViewModel : BaseViewModel ,IViewModel{
        private CampaignRewardDominationView view;
        private CampaignModel model;
        private MapViewModel parent;
        /* Model data get set */

        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<CampaignRewardDominationView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

		public void Show() {
            if (!this.view.IsVisible)
            {
                this.view.PlayShow();
            }
            //this.GetCampaignRewardDaminationReq();
            this.view.SetContent(model.dominationRewardList);
        }

        //private void GetCampaignRewardDaminationReq()
        //{
        //    DominationRewardInfoReq dominationRewardInfoReq = new DominationRewardInfoReq();
        //    NetManager.SendMessage(dominationRewardInfoReq,
        //        typeof(DominationRewardInfoAck).Name, this.GetCampaignRewardDaminationAck);
        //}

        //private void GetCampaignRewardDaminationAck(IExtensible message)
        //{
        //    DominationRewardInfoAck dominationRewardInfoAck = message as DominationRewardInfoAck;
        //    //设置魔影入侵奖励详情
        //    this.view.SetContent(dominationRewardInfoAck.DominationReward);
        //}

        public void Hide() {
            if (this.view.IsVisible)
            {
                this.view.PlayHide();
            }
        }

        public void HideImmediatly()
        {
            this.Hide();
        }

        protected override void OnReLogin()
        {
            this.Hide();
        }

        public void ShowHeroInfo(string heroName) {
            this.parent.ShowHeroInfo(heroName,infoType:HeroInfoType.Unlock,isSubWindow:true);
        }
        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
