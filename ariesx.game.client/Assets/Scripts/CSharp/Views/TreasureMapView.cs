using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
	public class TreasureMapView : BaseView {
		private TreasureMapViewModel viewModel;
        private TreasureMapViewPreference viewPref;
        /*************/

        private Dictionary<Resource, Transform> resourceDict =
                    new Dictionary<Resource, Transform>();

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<TreasureMapViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITreasure");
            this.viewPref = this.ui.transform.GetComponent<TreasureMapViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
        }

        public override void PlayShow(UnityAction callback) {
            base.PlayShow(callback);
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(callback);
        }

        private void AddResource(Resource resource, int amount) {
            GameObject resourceObj =
                PoolManager.GetObject(PrefabPath.pnlPlayerResourcesItem, this.viewPref.pnlResources);
            PlayerResourcesItemView resourceView = resourceObj.GetComponent<PlayerResourcesItemView>();
            resourceView.SetContent(resource, amount);
            resourceDict.Add(resource, resourceView.imgResource.transform);
        }

        public void CollectReward(Protocol.Resources resources,
        Protocol.Currency currency, CommonReward commonReward) {
            GameHelper.CommonRewardCollect(resources, currency,
                commonReward,(Vector2)this.viewPref.transform.position,true);
        }

        /* Propert change function */

        public void OnTreasureLevelChange() {
            this.resourceDict.Clear();
            TreasureConf treasureConf = this.viewModel.TreasureConf;
            if (treasureConf.gem != 0) {
                this.viewPref.pnlGem.gameObject.SetActiveSafe(true);
                this.viewPref.txtGemAmount.text = "x" + treasureConf.gem;
                resourceDict.Add(Resource.Gem, this.viewPref.pnlGem.Find("Image"));
            } else {
                this.viewPref.pnlGem.gameObject.SetActiveSafe(false);
            }

            GameHelper.ClearChildren(this.viewPref.pnlResources);
            foreach (var pair in treasureConf.resourceDict) {
                if (pair.Value != 0) {
                    this.AddResource(pair.Key, pair.Value);
                }
            }
        }
        /***************************/

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
