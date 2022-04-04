using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class TributeView : BaseView {
        private TributeViewModel viewModel;
        private TributeViewPreference viewPref;
        /*************/

        private Dictionary<Resource, Transform> resourceTransformDict =
                    new Dictionary<Resource, Transform>();

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<TributeViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITribute");
            this.viewPref = this.ui.transform.GetComponent<TributeViewPreference>();
            this.viewPref.btnReceive.onClick.AddListener(this.OnBtnReceiveClick);
        }

        public override void PlayShow(UnityAction action, bool needHideBack, float delay = 0) {
            base.PlayShow(() =>{
                action.InvokeSafe();
                AnimationManager.Animate(this.viewPref.imgHalo.gameObject, "Show");
            }, needHideBack);
        }

        /* Propert change function */
        public void OnForceChange() {
            int currentForce = RoleManager.GetForce();
            int currentLevel = ForceRewardConf.GetForceLevel(currentForce);
            ForceRewardConf forceRewardConf = ForceRewardConf.GetConf(currentLevel.ToString());
            if (forceRewardConf != null) {
                this.viewPref.txtStage.text = forceRewardConf.forceLocal;
            } else {
                this.viewPref.txtStage.text = "-";
            }
        }

        public void OnTributeGroupChange() {
            this.resourceTransformDict.Clear();
            Dictionary<Resource, int> normalDict =
                this.viewModel.TributeResult.Reward.GetNormalDict();
            Dictionary<Resource, int> specialDict =
                this.viewModel.TributeResult.Reward.GetSpecialDict();
            GameHelper.ResizeChildreCount(
                this.viewPref.pnlReward,
                normalDict.Count + specialDict.Count,
                PrefabPath.pnlItemWithCount
            );
            this.SetTributeGroup(specialDict, this.viewPref.pnlReward, false, 0);
            this.SetTributeGroup(normalDict, this.viewPref.pnlReward, true, specialDict.Count);
            this.viewPref.lgReward.enabled = true;
            GameHelper.ForceLayout(this.viewPref.lgReward);
            this.viewPref.lgReward.enabled = false;
        }

        private void SetTributeGroup(Dictionary<Resource, int> rewardDict, Transform root,
            bool isResource, int index) {
            ItemWithCountView itemView = null;
            foreach (var reward in rewardDict) {
                itemView = root.GetChild(index++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(reward.Key, reward.Value);
                if (isResource || reward.Key == Resource.Gold) {
                    this.resourceTransformDict.Add(reward.Key, itemView.imgItem.transform);
                }
            }
        }

        /***************************/
        public void CollectResource(GetTributeAck getTributeAck) {
            Protocol.Resources addResources = getTributeAck.Reward.Resources;
            Protocol.Currency addCurrency = getTributeAck.Reward.Currency;
            Protocol.Resources resources = getTributeAck.Resources;
            Protocol.Currency currency = getTributeAck.Currency;

            GameHelper.CollectResources(addResources, addCurrency,
                resources, currency, this.resourceTransformDict);
        }

        public void SetBtnReceiveInteractable(bool isEnable) {
            this.viewPref.btnReceive.interactable = isEnable;
        }

        private void OnBtnReceiveClick() {
            this.viewModel.CollectResources();
        }
    }
}
