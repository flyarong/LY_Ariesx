using Protocol;
using UnityEngine;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;

namespace Poukoute {
    public class AllianceMembersView : SRIA<AllianceMembersViewModel, AllianceMemberItemView> {
        private AllianceMembersViewPreference viewPref;
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceMembers");
            this.viewModel = this.gameObject.GetComponent<AllianceMembersViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceMembersViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnHint.onClick.AddListener(this.OnBtnInfoClick);
            this.SRIAInit(this.viewPref.allianceMembersScrollRect,
                this.viewPref.listVerticalLayoutGroup,
                defaultItemSize: 85f);
        }

        private void OnBtnInfoClick() {
            this.viewModel.ShowAllianceDisplayBoard(DisplayType.AllianceMembers);
        }

        #region SRIA implementation
        protected override AllianceMemberItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlAllianceMemItem, this.viewPref.pnlList);
            AllianceMemberItemView itemView = itemObj.GetComponent<AllianceMemberItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.SelfAllianceMembersInfo[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(AllianceMemberItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.SelfAllianceMembersInfo[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(AllianceMemberItemView itemView, AllianceMemberWithIndex itemData) {
            itemView.MemberData = itemData;
            itemView.OnInfoClick.AddListener(() => {
                this.OnBtnMemberItemInfoClick(itemData);
            });
        }

        public void SetAllianceSortContent() {
            this.viewPref.allianceSortPre.btnSortUIBtn.onClick.RemoveAllListeners();
            this.viewPref.allianceSortPre.btnSortUIBtn.onClick.AddListener(this.OnBtnSortClick);
            this.viewPref.allianceSortPre.txtMemberTitle.text = LocalManager.GetValue(LocalHashConst.alliance_member);
            this.SetSortTitlelabel();
        }

        public void RefreshCurrentPlayer() {
            int itemViewListsCount = this._VisibleItems.Count;
            AllianceMemberItemView memberItem;
            for (int index = 0; index < itemViewListsCount; index++) {
                memberItem = this._VisibleItems[index];
                if (memberItem.MemberData.member.Id.CustomEquals(
                        this.viewModel.CurrentPlayer.member.Id)) {
                    memberItem.MemberData = this.viewModel.CurrentPlayer;
                    return;
                }
            }
        }

        public void SetMembersInfo() {
            this.viewPref.allianceSortPre.btnSortUIBtn.interactable = true;
        }

        private void OnBtnSortClick() {
            this.viewPref.allianceSortPre.btnSortUIBtn.interactable = false;
            this.viewModel.NeedFresh = true;
            switch (this.viewModel.SortType) {
                case AllianceMemberSortType.alliance_exp:
                    this.viewModel.SortType = AllianceMemberSortType.force;
                    break;
                case AllianceMemberSortType.force:
                    this.viewModel.SortType = AllianceMemberSortType.role;
                    break;
                case AllianceMemberSortType.role:
                    this.viewModel.SortType = AllianceMemberSortType.alliance_exp;
                    break;
                default:
                    break;
            }
            this.SetSortTitlelabel();
        }

        private void SetSortTitlelabel() {
            if (this.IsVisible) {
                switch (this.viewModel.SortType) {
                    case AllianceMemberSortType.alliance_exp:
                        this.viewPref.allianceSortPre.txtBtnTitle.text =
                            LocalManager.GetValue(LocalHashConst.alliance_member_activity);
                        break;
                    case AllianceMemberSortType.force:
                        this.viewPref.allianceSortPre.txtBtnTitle.text =
                            LocalManager.GetValue(LocalHashConst.player_influence);
                        break;
                    case AllianceMemberSortType.role:
                        this.viewPref.allianceSortPre.txtBtnTitle.text =
                            LocalManager.GetValue(LocalHashConst.alliance_role );
                        break;
                    default:
                        break;
                }
            }
        }

        public void ShowSortBtn(bool isShow) {
            this.viewPref.butSortBtn.gameObject.SetActiveSafe(isShow);
        }

        private void OnBtnMemberItemInfoClick(AllianceMemberWithIndex memeber) {
            this.viewModel.SetCurrentPlayerInfo(memeber);
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
