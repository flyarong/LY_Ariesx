using UnityEngine;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MiniMapPassView : SRIA<MiniMapPassViewModel, AllianceCityItemView> {
        private MiniMapPassViewPreference viewPref;
        /*************/

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlPasses");
            this.viewPref = this.ui.transform.GetComponent<MiniMapPassViewPreference>();
            this.viewModel = this.gameObject.GetComponent<MiniMapPassViewModel>();
            this.SRIAInit(this.viewPref.scrollRect,
                          this.viewPref.verticalLayoutGroup,
                          defaultItemSize: 92);
        }

        #region SRIA implementation
        protected override AllianceCityItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlAllianceCityItem, this.viewPref.pnlList);
            AllianceCityItemView itemView = itemObj.GetComponent<AllianceCityItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.PassList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(AllianceCityItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.PassList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(AllianceCityItemView itemView, MiniMapPassConf itemData) {
            itemView.SetItem(itemData.id, ElementType.pass, itemData.allianceName, () => {
                this.OnPassItemClick(itemData.coordinate, itemData.LocalName);
            }, itemData.coordinate == this.viewModel.CurrentCoord && this.viewModel.IsSelectItem);
        }

        public void SetPassStatus(int itemIndex) {
            int headIndex = this._VisibleItems[0].ItemIndex;
            int tailIndex = this._VisibleItems[this._VisibleItemsCount - 1].ItemIndex;
            if (itemIndex >= headIndex && itemIndex <= tailIndex) {
                this._VisibleItems[itemIndex - headIndex].AllianceName =
                    this.viewModel.PassList[itemIndex].allianceName;
            }
        }

        private void OnPassItemClick(Vector2 coordinate, string local) {
            this.ResetItemIsChosen();
            this.viewModel.CurrentCoord = coordinate;
            this.viewModel.MoveTo(coordinate, local);
            this.viewModel.IsSelectItem = true;
        }

        public void ResetItemIsChosen() {
            if (this.viewModel.IsSelectItem) {
                foreach (AllianceCityItemView itemView in this._VisibleItems) {
                    if (itemView.Coordinate == this.viewModel.CurrentCoord) {
                        itemView.SetItemIsChosen(false);
                        this.viewModel.IsSelectItem = false;
                        return;
                    }
                }
            }
        }
        protected override void OnInvisible() {
            this.ResetItemIsChosen();
        }

    }
}
