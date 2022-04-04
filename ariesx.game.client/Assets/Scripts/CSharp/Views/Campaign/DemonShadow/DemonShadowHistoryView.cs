using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class DemonShadowHistoryView: SRIA<DemonShadowHistoryViewModel,
        DemonShadowHistoryDetailItemView> {
        /* UI Members*/
        private DemonShadowHistoryViewPeference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<DemonShadowHistoryViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDemonShadowHoldler.PnlDemonShadow.PnlDemonShadowHistory");
            this.viewPref = this.ui.transform.GetComponent<DemonShadowHistoryViewPeference>();
            this.SRIAInit(this.viewPref.scrollRect,
               this.viewPref.verticalLayoutGroup,
               defaultItemSize: 344f);
        }

        #region SRIA implementation
        protected override DemonShadowHistoryDetailItemView CreateViewsHolder(int itemIndex) {
            //Debug.LogError(itemIndex);
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlDemonShadowHistoryDetail,
                this.viewPref.pnlPointsStatisticsList);
            DemonShadowHistoryDetailItemView itemView = itemObj.GetComponent<DemonShadowHistoryDetailItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnIyemContentChangr(itemView,
                this.viewModel.dominationHistoryList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(DemonShadowHistoryDetailItemView itemView) {
            this.OnIyemContentChangr(itemView,
                this.viewModel.dominationHistoryList[itemView.ItemIndex]);
        }
        #endregion

        private void OnIyemContentChangr(DemonShadowHistoryDetailItemView itemView,
            DominationHistory record) {
            bool results = false;
            if (RoleManager.GetAllianceId().CustomIsEmpty()) {
                results = true;//抢夺击杀
            } else {
                results = (RoleManager.GetAllianceId()
                    != record.AllianceId);//抢夺击杀
            }
            if (!results) {
                results = record.Status == 1;//自己联盟击杀
            }
            itemView.SetdominationHistoryCount(record, results, () => {
                this.ShowDemonShadowHistoryRank(record);
            }, this.GetCoodZone(record.Coord));
        }

        /* Propert change function */
        public void ShowDemonShadowHistoryRank(DominationHistory record) {
            this.viewModel.ShowDemonShadowHistoryRank(record);
        }

        private string GetCoodZone(Vector2 coodinate) {
            return this.viewModel.resourceInfo.GetTileZone(coodinate);
        }

        public void ShowHistory() {
            this.viewModel.GetDominationHistoryReq();
        }
        /***************************/
    }
}
