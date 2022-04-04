using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class BattleReportView : SRIA<BattleReportViewModel, BattleReportItemView> {
        private BattleReportViewPreference viewPref;

        /*************/

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMail.PnlMail.PnlContent.PnlBattle");
            this.viewModel = this.gameObject.GetComponent<BattleReportViewModel>();
            this.viewPref = this.ui.transform.GetComponent<BattleReportViewPreference>();
            this.viewPref.btnRead.onClick.AddListener(OnBtnMarkAllReadClick);
            this.SRIAInit(this.viewPref.scrollRect,
                this.viewPref.verticalLayoutGroup, defaultItemSize: 371f);
        }

        #region SRIA implementation
        //protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc) {
        //    base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
        //    Debug.LogError("CollectItemsSizes callded");
        //}

        protected override BattleReportItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlBattleReportItem, this.viewPref.pnlList);
            BattleReportItemView itemView = itemObj.GetComponent<BattleReportItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.BattleReportList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(BattleReportItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.BattleReportList[itemView.ItemIndex]);
            itemView.MarkForRebuild();
            this.ScheduleComputeVisibilityTwinPass(true);
        }
        #endregion

        private readonly string titleFormat = "<color=#C4FEE4>{0}</color> VS <color=#FFCFD0>{1}</color>";
        private void OnItemContentChange(BattleReportItemView itemView, BattleReport itemData) {
            itemView.BattleReport = itemData;
            string id = itemData.Id;
            itemView.OnBtnDetailClick.AddListener(() => {
                if (!itemView.IsRead) {
                    itemView.IsRead = true;
                }
                string title = string.Format(titleFormat, itemView.AttackerName, itemView.DefenderName);
                this.OnBtnBattleReportClick(id, title);
            });
            itemView.OnBtnPlayClick.AddListener(() => {
                if (!itemView.IsRead) {
                    itemView.IsRead = true;
                }
                string title = string.Format(titleFormat, itemView.AttackerName, itemView.DefenderName);
                this.OnBtnBattleReportPlay(id, title);
            });

            itemView.OnBtnItemClick.AddListener(() => {
                this.OnBtnBattleReportItemClick(id, itemView);
            });
        }

        /***************************/
        private void OnBtnBattleReportClick(string id, string title) {
            this.viewModel.ShowBattleReportDetail(id, title);
        }

        private void OnBtnBattleReportPlay(string id, string title) {
            Debug.LogError(id);
            this.viewModel.PlayBattle(id);
        }

        private void OnBtnBattleReportItemClick(string id, BattleReportItemView itemView) {
            if (!itemView.IsRead) {
                itemView.IsRead = true;
                this.viewModel.MarkReadReq(id);
            }
        }

        private void OnBtnMarkAllReadClick() {
            int listCount = this.viewModel.BattleReportList.Count;
            int childCount = this.viewPref.pnlList.childCount;
            BattleReportItemView itemView = null;
            for (int i = 0; i < childCount; i++) {
                itemView = this.viewPref.pnlList.
                    GetChild(i).GetComponent<BattleReportItemView>();
                itemView.IsRead = true;
            }
            if (this.viewModel.BattleReportList != null) {
                for (int i = 0; i < listCount; i++) {
                    BattleReport battle = this.viewModel.BattleReportList[i];
                    battle.IsRead = true;
                }
            }
            this.viewModel.MarkAllReadReq();
        }
    }
}
