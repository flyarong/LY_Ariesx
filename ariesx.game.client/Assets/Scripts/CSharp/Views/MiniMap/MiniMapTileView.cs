using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MiniMapTileView : SRIA<MiniMapTileViewModel, MiniMapTileItemView> {
        private MiniMapTileViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<MiniMapTileViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlTiles");
            this.viewModel = this.gameObject.GetComponent<MiniMapTileViewModel>();
            this.viewPref = this.ui.transform.GetComponent<MiniMapTileViewPreference>();
            this.SRIAInit(this.viewPref.scrollRect,
                          this.viewPref.verticalLayoutGroup,
                          defaultItemSize: 92);
        }

        #region SRIA implementation
        protected override MiniMapTileItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlMiniMapTileItem, this.viewPref.pnlList);
            MiniMapTileItemView itemView = itemObj.GetComponent<MiniMapTileItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.TileList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(MiniMapTileItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.TileList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(MiniMapTileItemView itemView, Point itemData) {
            itemView.SetItemViewInfo(itemData, () => OnTileItemClick(itemData.Coord),
                itemData.Coord == this.viewModel.CurrentCoord && this.viewModel.IsSelectItem);
        }

        private void SetItemViewHighlight(Coord coordinate) {
            if (coordinate == null) {
                return;
            }
            foreach (MiniMapTileItemView itemView in this._VisibleItems) {
                if (itemView.Tile.Coord == coordinate) {
                    itemView.SetHighlight(false);
                    return;
                }
            }
        }

        public void ResetItemChosen() {
            if (this.viewModel.IsSelectItem) {
                foreach (MiniMapTileItemView itemView in this._VisibleItems) {
                    if (itemView.Tile.Coord == this.viewModel.CurrentCoord) {
                        itemView.SetHighlight(false);
                        this.viewModel.IsSelectItem = false;
                        return;
                    }
                }
            }
        }
        /***************************/

        private void OnTileItemClick(Coord coordinate) {
            this.SetItemViewHighlight(this.viewModel.CurrentCoord);
            this.viewModel.CurrentCoord = coordinate;
            this.viewModel.BubbleMoveTo(coordinate);
            this.viewModel.IsSelectItem = true;
        }

        protected override void OnInvisible() {
            this.SetItemViewHighlight(this.viewModel.CurrentCoord);
            this.viewModel.CurrentCoord = new Coord(
                x: -1,
                y: -1
            );
        }
    }
}
