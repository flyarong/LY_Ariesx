using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceSubordinateView : SRIA<AllianceSubordinateViewModel, AllianceSubordnateItemView> {
        private AllianceSubordinateViewPreference viewPref;

        /*************/
        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UIAlliance.PnlAlliance.PnlSubordnateHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UIAlliance.PnlAlliance.PnlSubordnate");
            this.viewModel = this.gameObject.GetComponent<AllianceSubordinateViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceSubordinateViewPreference>();
            this.ResetSortBtnInfo();
            this.SRIAInit(this.viewPref.subordnateScrollRect,
                this.viewPref.listVerticalLayoutGroup);
        }


        #region SRIA implementation
        protected override AllianceSubordnateItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlSubordnateItem, this.viewPref.pnlList);
            AllianceSubordnateItemView itemView = itemObj.GetComponent<AllianceSubordnateItemView>();
            this.OnItemContentChange(itemView,
                this.viewModel.AllianceSubordinates[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(AllianceSubordnateItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.AllianceSubordinates[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(AllianceSubordnateItemView itemView, FallenPlayer itemData) {
            itemView.SubordinateData = itemData;
            itemView.OnInfoClick.AddListener(() => {
                this.OnItemInfoClick(itemData);
            });
        }

        public void InitView() {            
            this.viewPref.subordinateStatusPre.btnSortUIBtn.interactable = true;
            this.viewPref.subordinateStatusPre.imgBtnBG.sprite =
                    PoolManager.GetSprite("Sprites/v4ui/ui_normal/btn04");
            Debug.LogError(LocalManager.GetValue(LocalHashConst.alliance_subordinate_status));
            this.viewPref.subordinateStatusPre.txtBtnTitle.text = 
                    LocalManager.GetValue(LocalHashConst.alliance_subordinate_status);
            this.viewPref.subordinateStatusPre.txtMemberTitle.text = 
                    LocalManager.GetValue(LocalHashConst.alliance_subordinate_list);
        }

        public void ResetSortBtnInfo() {
            this.viewPref.subordinateStatusPre.btnSortUIBtn.onClick.RemoveAllListeners();
            this.viewPref.subordinateStatusPre.btnSortUIBtn.onClick.AddListener(this.OnBtnSubordinateStatus);
        }


        /*************** btn callback ***************************/
        private void OnItemInfoClick(FallenPlayer fallenPlayer) {
            this.viewModel.SetCurrentFallenPlayerInfo(fallenPlayer);
        }

        private void OnBtnSubordinateStatus() {
            this.viewModel.ShowSubordinateStatus();
        }
    }
}
