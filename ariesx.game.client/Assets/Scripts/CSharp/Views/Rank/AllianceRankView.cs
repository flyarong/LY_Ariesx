using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceRankView : SRIA<AllianceRankViewModel, RankAllianceItemView> {
        private AllianceRankViewPreference viewPref;

        private bool listContainOwnRankInfo = false;
        private string ownAllianceName = string.Empty;
        private string ownAllianceId = string.Empty;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRank.PnlRank.PnlChannel.PnlAllianceRank");
            this.viewModel = this.GetComponent<AllianceRankViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceRankViewPreference>();
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.customVerticalLayoutGroup);
        }

        #region SRIA implementation
        protected override RankAllianceItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlRankAllianceItem, this.viewPref.pnlList);
            RankAllianceItemView itemView = itemObj.GetComponent<RankAllianceItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView, 
                this.viewModel.RankInfoList[itemIndex]);

            return itemView;
        }

        protected override void UpdateViewsHolder(RankAllianceItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.RankInfoList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(RankAllianceItemView itemView, RankAlliance itemData) {
            itemView.AllianceRankData = itemData;
            string allianceId = itemData.Id;
            itemView.OnItemClick.AddListener(() => {
                this.OnRankInfoClick(allianceId);
            });

            if (itemData.Name.CustomEquals(this.ownAllianceName)) {
                listContainOwnRankInfo = true;
            }
        }

        /********************* private methods ******************************/
        private void OnRankInfoClick(string allianceId) {
            this.viewModel.ShowAllianceInfo(allianceId);
        }

        protected override void OnVisible() {
            this.viewPref.customScrollRect.enabled = true;
            UpdateManager.Regist(UpdateInfo.PersonalRankView, this.MyUpdate);
        }

        protected override void OnInvisible() {
            this.viewPref.customScrollRect.velocity = Vector2.zero;
            this.viewPref.customScrollRect.enabled = false;
            UpdateManager.Unregist(UpdateInfo.PersonalRankView);
        }
    }
}
