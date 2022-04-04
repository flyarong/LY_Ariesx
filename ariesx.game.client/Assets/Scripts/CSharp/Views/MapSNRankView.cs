using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MapSNRankView: SRIA<MapSNRankViewModel, MapSNRankItemView> {
        private MapSNRankViewPeference viewPref;

        #region SRIA implementation
        protected override MapSNRankItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj = PoolManager.GetObject(PrefabPath.pnlMapSNRankItem,
                this.viewPref.pnlList);
            MapSNRankItemView itemView = itemObj.GetComponent<MapSNRankItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView, this.viewModel.MapSNRankInfoList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(MapSNRankItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.MapSNRankInfoList[itemView.ItemIndex]);
        }
        #endregion
        /* UI Members*/

        /************/

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRank.PnlRank.PnlChannel.PnlMapSNRank");
            this.viewModel = this.GetComponent<MapSNRankViewModel>();
            this.viewPref = this.ui.GetComponent<MapSNRankViewPeference>();
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.layoutGroup);
            /* Cache the ui components here*/
        }

        /* Propert change function */

        private void OnItemContentChange(MapSNRankItemView itemView, RankPlayer itemData) {
            itemView.PersonalRankData = itemData;
            string playerId = itemData.Id;
            itemView.OnItemClick.AddListener(() => {
                this.OnRankInfoClick(playerId);
            });
        }        

        private void OnRankInfoClick(string playerId) {
            this.viewModel.ShowPlayerInfo(playerId);
        }

        protected override void OnVisible() {
            this.viewPref.customScrollRect.enabled = true;
            UpdateManager.Regist(UpdateInfo.MapSNRankView, this.MyUpdate);
        }

        protected override void OnInvisible() {
            this.viewPref.customScrollRect.velocity = Vector2.zero;
            this.viewPref.customScrollRect.enabled = false;
            UpdateManager.Unregist(UpdateInfo.MapSNRankView);
        }

        /***************************/
    }
}

