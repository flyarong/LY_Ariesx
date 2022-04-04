using Protocol;
using UnityEngine;
using frame8.ScrollRectItemsAdapter.Util.GridView;

namespace Poukoute {
    public class MapMarkView : GridAdapter<MapMarkViewModel, AllianceMarkItemView> {
        private MapMarkViewPreference viewPref;
        private Vector3 preMarkCoord = Vector3.zero;
        public int VisibleItemCount {
            get {
                return GameHelper.GetVisibleChildreCount(this.viewPref.pnlList);
            }
        }

        private void Start() {
            this.viewModel = this.gameObject.GetComponent<MapMarkViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlMarks");
            this.viewPref = this.ui.transform.GetComponent<MapMarkViewPreference>();
            this.GridAdapterInit(this.viewPref.scrollRect, this.viewPref.gridLayoutGroup,
                PrefabPath.pnlAllianceMarkItem);
        }

        #region GridAdapter implementation
        protected override void UpdateCellViewsHolder(AllianceMarkItemView itemView) {
            int dataIndex = itemView.ItemIndex;
            MapMark itemData = this.viewModel.MarkList[dataIndex];
            itemView.Index = dataIndex;
            Vector3 coord = new Vector3() {
                x = itemData.mark.Coord.X,
                y = itemData.mark.Coord.Y,
                z = (int)itemData.type
            };
            itemView.SetMark(itemData, () => {
                this.OnBtnDeleteClick(itemData);
            }, () => {
                this.OnCoordinateClick(coord, dataIndex);
            });
            itemView.FocusOnItemView(this.preMarkCoord == coord &&
                this.viewModel.IsSelectItem);
        }
        #endregion

        public void SetContent() {
            if (IsUIInit) {
                base._CellsCount = this.viewModel.MarkList.Count;
                base.Refresh(false, false);
            }
        }

        private void OnBtnDeleteClick(MapMark mark) {
            this.viewModel.DeleteMarkReq(mark.mark.Coord, mark.type);
        }

        private void OnCoordinateClick(Vector3 coord, int index) {
            this.viewModel.MoveTo(coord);
            this.viewModel.CurrentCoord = coord;
            AllianceMarkItemView itemView;
            Vector3 markVect = Vector3.zero;
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as AllianceMarkItemView;
                    markVect.x = itemView.mark.mark.Coord.X;
                    markVect.y = itemView.mark.mark.Coord.Y;
                    markVect.z = (int)itemView.mark.type;
                    itemView.FocusOnItemView(coord == markVect);
                }
            }
            this.preMarkCoord = coord;
            this.viewModel.IsSelectItem = true;
        }

        public void ResetItemChosen() {
            if (this.viewModel.IsSelectItem) {
                AllianceMarkItemView itemView;
                foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                    for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                        itemView = cellGroup.ContainingCellViewsHolders[i] as AllianceMarkItemView;
                        if (itemView.coordinate == this.viewModel.CurrentCoord) {
                            itemView.FocusOnItemView(false);
                        }
                    }
                }
            }
        }

        protected override void OnInvisible() {
            this.preMarkCoord = Vector2.zero;
        }
    }
}
