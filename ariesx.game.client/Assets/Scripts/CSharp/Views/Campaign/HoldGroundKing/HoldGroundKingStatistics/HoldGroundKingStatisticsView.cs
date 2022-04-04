using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System.Text;

namespace Poukoute {
    public class HoldGroundKingStatisticsView: SRIA<HoldGroundKingStatisticsViewModel, PointsStatisticsItemView> {
        private HoldGroundKingStatisticsViewPeference viewPref;

        /*****************************************************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlHoldGroundKingHoldler.PnlHoldGroundKingStatistics");
            this.viewModel = this.gameObject.GetComponent<HoldGroundKingStatisticsViewModel>();
            this.viewPref = this.ui.transform.GetComponent<HoldGroundKingStatisticsViewPeference>();
            this.SRIAInit(this.viewPref.scrollRect,
                this.viewPref.verticalLayoutGroup,
                defaultItemSize: 83f);
        }


        #region SRIA implementation
        protected override PointsStatisticsItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlPointsStatisticsItemView,
                this.viewPref.pnlPointsStatisticsList);
            PointsStatisticsItemView itemView = itemObj.GetComponent<PointsStatisticsItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.occupyLogList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(PointsStatisticsItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.occupyLogList[itemView.ItemIndex]);
        }
        #endregion
        
        private void OnItemContentChange(PointsStatisticsItemView itemView, OccupyLog itemData) {
            itemView.OccupyLog = itemData;
            int level = 0;
            string contendType = string.Empty;
            string tileName = string.Empty;
            switch (itemData.PointInfo.ElementType) {
                case (int)ElementType.pass:
                    level = itemData.PointInfo.Pass.Level;
                    tileName = LocalManager.GetValue(LocalHashConst.map_tile_bridge);
                    if (itemData.Delta < 0) {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.occupy_points_detail_beoccupied), level, tileName);
                    } else {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.occupy_points_detail_occupy), level, tileName);
                    }
                    break;
                case (int)ElementType.npc_city:
                    NPCCityConf cityConf = NPCCityConf.GetConf(NPCCityConf.GetCityKey(itemData.PointInfo));
                    level = cityConf.level;
                    tileName = LocalManager.GetValue(LocalHashConst.occupy_cityoutside);
                    if (itemData.Delta < 0) {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.occupy_points_detail_beoccupied), level, tileName);
                    } else {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.occupy_points_detail_occupy), level, tileName);
                    }
                    break;
                default:
                    if (itemData.PointInfo.Resource != null) {
                        level = itemData.PointInfo.Resource.Level;
                        tileName = LocalManager.GetValue(LocalHashConst.occupy_tile);
                        if (itemData.Delta < 0) {
                            contendType = string.Format(LocalManager.GetValue(
                                LocalHashConst.occupy_points_detail_beoccupied), level, tileName);
                        } else {
                            contendType = string.Format(LocalManager.GetValue(
                                LocalHashConst.occupy_points_detail_occupy), level, tileName);
                        }
                    }
                    break;
            }
            itemView.SetCount(contendType, itemData.Delta, itemData.Total, () => {
                this.viewModel.MoveWithClick(itemData.PointInfo.Coord);
            });
        }
    }
}

