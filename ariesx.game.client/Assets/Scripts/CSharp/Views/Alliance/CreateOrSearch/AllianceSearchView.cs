using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceSearchView : SRIA<AllianceSearchViewModel, AllianceListItemView> {
        private AllianceSearchViewPreference viewPref;

        /*************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlContent.PnlSearch");
            this.viewModel = this.gameObject.GetComponent<AllianceSearchViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceSearchViewPreference>();
            /* Cache the ui components here */
            this.viewPref.btnSearch.onClick.AddListener(this.OnBtnSearchClick);
            this.SRIAInit(this.viewPref.searchScrollRect,
                this.viewPref.listVerticalLayoutGroup,
                defaultItemSize: 85f);
        }

        #region SRIA implementation
        protected override AllianceListItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlAllianceListItem, this.viewPref.pnlList);
            AllianceListItemView itemView = itemObj.GetComponent<AllianceListItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.AlliancesList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(AllianceListItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.AlliancesList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(AllianceListItemView itemView, AllianceCache itemData) {
            itemView.index = itemView.ItemIndex;
            itemView.AllianceData = itemData;
            string allianceId = itemData.Id;
            itemView.OnInfoClick.AddListener(() => {
                this.OnBtnAllianceItemInfoClick(allianceId);
            });
        }

        private void OnBtnAllianceItemJoinClick(string allianceId, string message) {
            this.viewModel.JoinAllianceReq(allianceId, message);
        }

        private void OnBtnAllianceItemInfoClick(string allianceId) {
            this.viewModel.ShowAllianceInfo(allianceId);
        }

        private void OnBtnSearchClick() {
            this.viewModel.SearchAllianceName = this.viewPref.ifName.text;
            if (this.viewModel.SearchAllianceName.CustomIsEmpty()) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.search_alliance_name_default), TipType.Info);
                return;
            }
            //GameHelper.ClearChildren(this.viewPref.pnlList);
            this.viewModel.SearchAllianceReq();
            this.viewPref.ifName.text = string.Empty;
        }
    }
}
