using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class DailyRewardReceivedViewModel: BaseViewModel {
        private MapViewModel parent;
        private CampaignModel model;
        private DailyRewardReceivedView view;
        /* Model data get set */
        public LoginRewardAck LoginRewardAck {
            get {
                return this.model.loginRewardAck;
            }
        }
        /**********************/

        /* Other members */
        public Dictionary<Resource, int> resourceDict =
            new Dictionary<Resource, int>();
        public Dictionary<Resource, Transform> resourceTransformDict =
            new Dictionary<Resource, Transform>();
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<DailyRewardReceivedView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show(LoginRewardConf rewardConf) {
            this.view.PlayShow();
            this.view.SetRewardInfo(rewardConf);
        }

        public void Hide() {
            this.view.PlayHide();
            this.view.StopReceiveAmin();
        }

        /* Add 'NetMessageAck' function here*/
        public void GetLoginRewardReq() {
            GetLoginRewardReq loginRewardReq = new GetLoginRewardReq();
            NetManager.SendMessage(loginRewardReq,
                typeof(GetLoginRewardAck).Name, this.GetLoginRewardAck);
        }

        private void GetLoginRewardAck(IExtensible message) {
            GetLoginRewardAck loginRewardAck = message as GetLoginRewardAck;
            if (loginRewardAck.GetStatus == true) {
                if (loginRewardAck.LotteryResults.Count > 0) {
                    this.parent.ShowOpenChestView(loginRewardAck.LotteryResults, () => {
                        this.CollectResources();
                    });
                } else {
                    this.CollectResources();
                }
                this.UpdateLoginRewardReq();
            }
        }

        public void UpdateLoginRewardReq() {
            this.parent.UpdateLoginRewardReq();
        }

        private void CollectResources() {
            Protocol.Resources addResources = new Protocol.Resources();
            Protocol.Currency addcurrency = new Currency();
            if (resourceDict.ContainsKey(Resource.Lumber)) {
                addResources.Lumber = resourceDict[Resource.Lumber];
            }

            if (resourceDict.ContainsKey(Resource.Steel)) {
                addResources.Steel = resourceDict[Resource.Steel];
            }

            if (resourceDict.ContainsKey(Resource.Marble)) {
                addResources.Marble = resourceDict[Resource.Marble];
            }

            if (resourceDict.ContainsKey(Resource.Food)) {
                addResources.Food = resourceDict[Resource.Food];
            }

            if (resourceDict.ContainsKey(Resource.Gem)) {
                addcurrency.Gem = resourceDict[Resource.Gem];
            }

            if (resourceDict.ContainsKey(Resource.Gold)) {
                addcurrency.Gold = resourceDict[Resource.Gold];
            }

            Protocol.Resources resources = addResources;
            Protocol.Currency currency = addcurrency;
            GameHelper.CollectResources(addResources, addcurrency,
                resources, currency, resourceTransformDict);
            this.Hide();
        }

        public void ShowHeroInfo(string heroName) {
            this.parent.ShowHeroInfo(heroName, infoType: HeroInfoType.Unlock, isSubWindow: true);
        }

        /***********************************/
    }
}
