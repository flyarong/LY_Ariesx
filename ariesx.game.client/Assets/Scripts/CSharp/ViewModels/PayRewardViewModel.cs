using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class PayRewardViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private PayModel model;
        private HeroModel heroModel;
        private PayRewardView view;
        /* Model data get set */
        public int CurrentLevel {
            get {
                return this.model.payRewardLevel;
            }
            set {
                this.model.payRewardLevel = value;
            }
        }

        public FirstPayConf CurrentConf {
            get; set;
        }

        public float CurrentValue {
            get {
                return this.model.payAmount;
            }
        }

        public List<Chest> LotteryChanceList {
            get {
                return this.heroModel.lotteryChanceList;
            }
        }
        /**********************/

        /* Other members */
        private bool NeedRefresh {
            get; set;
        }
        private GetRechargeRewardAck reward;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<PayModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<PayRewardView>();

            NetHandler.AddNtfHandler(typeof(RechargeRewardNtf).Name, this.RechargeRewardNtf);

            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
                if (this.NeedRefresh) {
                    this.view.SetInfo();
                    // this.NeedRefresh = false;
                }
            }, needHideBack: true);
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void ShowHeroInfo() {
            this.parent.ShowHeroInfo(new Hero {
                Name = this.CurrentConf.hero
            }, infoType: HeroInfoType.Unlock, isSubWindow: true);
        }

        public void ShowPay() {
            this.HideImmediatly();
            this.parent.ShowPay();
        }

        /* Add 'NetMessageAck' function here*/
        private void RechargeRewardNtf(IExtensible message) {
            if (this.view.IsVisible) {

            } else {
                this.NeedRefresh = true;
            }
        }

        public void GetRewardReq() {
            GetRechargeRewardReq req = new GetRechargeRewardReq();
            req.RewardId = this.CurrentLevel + 1;
            Debug.LogError(req.RewardId);
            this.view.SetBtnEnable(false);
            NetManager.SendMessage(req, typeof(GetRechargeRewardAck).Name, this.GetRewardAck,
                (message) => { this.view.SetBtnEnable(true); }, () => { this.view.SetBtnEnable(true); });
        }

        public void GetRewardAck(IExtensible message) {
            this.view.SetBtnEnable(true);
            this.reward = message as GetRechargeRewardAck;
            this.CurrentLevel++;
            if (this.reward.LotteryResults.Count > 0) {
                this.parent.ShowOpenChestView(this.reward.LotteryResults, this.ShowGetResources);
            } else {
                this.ShowGetResources();
            }
        }

        private void ShowGetResources() {
            if (this.CurrentConf.resourceDict.Count > 0) {
                this.view.CollectResource(this.reward);
            }
            this.parent.SetBtnPayReward(this.CurrentLevel);
            if (this.CurrentLevel >= FirstPayConf.maxLevel) {
                this.Hide();
                return;
            }
            if (this.view.IsVisible) {
                this.view.SetInfo();
            } else {
                this.NeedRefresh = true;
            }
        }
        /***********************************/

    }
}
