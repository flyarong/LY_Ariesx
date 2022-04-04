using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class PersonalRankView : SRIA<PersonalRankViewModel, RankPlayerItemView> {
        private PersonalRankViewPreference viewPref;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRank.PnlRank.PnlChannel.PnlPersonalRank");
            this.viewModel = this.GetComponent<PersonalRankViewModel>();
            this.viewPref = this.ui.GetComponent<PersonalRankViewPreference>();
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.customVerticalLayoutGroup);
        }

        #region SRIA implementation
        protected override RankPlayerItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlRankPlayerItem, this.viewPref.pnlList);
            RankPlayerItemView itemView = itemObj.GetComponent<RankPlayerItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.RankInfoList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(RankPlayerItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.RankInfoList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(RankPlayerItemView itemView, RankPlayer itemData) {
            itemView.PersonalRankData = itemData;
            string playerId = itemData.Id;
            itemView.OnItemClick.AddListener(() => {
                this.OnRankInfoClick(playerId);
            });
        }

        /********************* private methods ******************************/
        private void OnRankInfoClick(string playerId) {
            this.viewModel.ShowPlayerInfo(playerId);
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

        public void SetUnlockedWorldRankCG(bool show) {
            UIManager.SetUICanvasGroupVisible(this.viewPref.unlockedWorldRankCG, show);
        }
    }
}
