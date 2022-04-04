using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MiniMapCityView : SRIA<MiniMapCityViewModel, AllianceCityItemView> {
        private MiniMapCityViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<MiniMapCityViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlCities");
            this.viewPref = this.ui.transform.GetComponent<MiniMapCityViewPreference>();
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
                this.viewModel.CityList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(AllianceCityItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.CityList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(AllianceCityItemView itemView, NPCCityConf itemData) {
            MiniMapCityConf cityConf = MiniMapCityConf.GetConf(itemData.id);
            itemView.SetItem(itemData.id, ElementType.npc_city, itemData.allianceName, () => {
                this.OnCityItemClick(cityConf);
            }, cityConf.coordinate == this.viewModel.CurrentCoord && this.viewModel.IsSelectItem);
        }


        public void SetCityStatus(int itemIndex) {
            int headIndex = this._VisibleItems[0].ItemIndex;
            int tailIndex = this._VisibleItems[this._VisibleItemsCount - 1].ItemIndex;
            if (itemIndex >= headIndex && itemIndex <= tailIndex) {
                this._VisibleItems[itemIndex - headIndex].AllianceName =
                    this.viewModel.CityList[itemIndex].allianceName;
            }
        }

        private void OnCityItemClick(MiniMapCityConf cityConf) {
            NPCCityConf npcCityConf = NPCCityConf.GetConf(cityConf.id);
            this.ResetItemIsChosen();
            this.viewModel.CurrentCoord = cityConf.coordinate;
            this.viewModel.MoveTo(
                cityConf.coordinate,
                NPCCityConf.GetNpcCityLocalName(npcCityConf.name, npcCityConf.isCenter)
            );
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
