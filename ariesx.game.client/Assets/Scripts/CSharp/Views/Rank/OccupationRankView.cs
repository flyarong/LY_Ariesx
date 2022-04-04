using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class OccupationRankView : SRIA<OccupationRankViewModel, RankOccupationItemView> {
        private OccupationRankViewPreference viewPref;

        private bool listContainOwnRankInfo = false;
        private string ownAllianceName = string.Empty;
        private string ownAllianceId = string.Empty;


        //private int tmpTargetIndex = -1;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRank.PnlRank.PnlChannel.PnlOccupationRank");
            this.viewModel = this.GetComponent<OccupationRankViewModel>();
            this.viewPref = this.ui.transform.GetComponent<OccupationRankViewPreference>();
            this.viewPref.btnIntro.onClick.AddListener(this.OnBtnIntroClick);
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.customVerticalLayoutGroup);
        }

        #region SRIA implementation
        protected override RankOccupationItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlRankOccupationItem, this.viewPref.pnlList);
            RankOccupationItemView itemView = itemObj.GetComponent<RankOccupationItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.RankInfoList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(RankOccupationItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.RankInfoList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(RankOccupationItemView itemView, RankAlliance itemData) {
            itemView.OccupationRankData = itemData;
            string allianceId = itemData.Id;
            itemView.OnItemClick.AddListener(() => {
                this.OnRankInfoClick(allianceId);
            });

            if (itemData.Name.CustomEquals(this.ownAllianceName)) {
                listContainOwnRankInfo = true;
            }
        }

        //public bool GetOwnRankInfoVisible() {
        //    if (this.viewModel.OwnRankInfo == null ||
        //        this._VisibleItems.Count < 1) {
        //        this.listContainOwnRankInfo = false;
        //        return false;
        //    }
        //    if (this._VisibleItems[0].OccupationRankData.OccupationRank >= 
        //        this.viewModel.OwnRankInfo.OccupationRank) {
        //        this.listContainOwnRankInfo = true;
        //    } else {
        //        this.listContainOwnRankInfo = false;
        //        foreach (RankOccupationItemView item in this._VisibleItems) {
        //            if (item.OccupationRankData.Id.CustomEquals(this.ownAllianceId)) {
        //                this.listContainOwnRankInfo = true;
        //                break;
        //            }
        //        }
        //    }
        //    return this.listContainOwnRankInfo;
        //}
        /********************* private methods ******************************/

        private void OnRankInfoClick(string allianceId) {
            this.viewModel.ShowAllianceInfo(allianceId);
        }
        
        protected void OnBtnIntroClick() {
            this.viewModel.ShowOccupationIntro();
        }

        protected override void OnVisible() {
            this.viewPref.customScrollRect.enabled = true;
            UpdateManager.Regist(UpdateInfo.OccupationRankView, this.MyUpdate);
        }

        protected override void OnInvisible() {
            this.viewPref.customScrollRect.velocity = Vector2.zero;
            this.viewPref.customScrollRect.enabled = false;
            UpdateManager.Unregist(UpdateInfo.OccupationRankView);
        }
    }
}
