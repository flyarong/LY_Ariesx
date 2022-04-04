using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class ContinentDisputesStatisticsView: SRIA<ContinentDisputesStatisticsViewModel,
        PointsStatisticsItemView> {
        private ContinentDisputesStatisticsViewPeference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ContinentDisputesStatisticsViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlContinentDisputes.PnlContinentDisputesStatistics");
            this.viewPref = this.ui.transform.GetComponent<ContinentDisputesStatisticsViewPeference>();
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
                this.viewModel.captureLogList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(PointsStatisticsItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.captureLogList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(PointsStatisticsItemView itemView, CaptureLog itemData) {
            itemView.CaptureLog = itemData;
            int level = 0;
            string contendType = string.Empty;
            switch (itemData.PointInfo.ElementType) {
                case (int)ElementType.pass:
                    level = itemData.PointInfo.Pass.Level;
                    if (itemData.Delta < 0) {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.capture_points_detail_alliance_beoccupied), level);
                    } else {
                        contendType = string.Format(LocalManager.GetValue(
                            LocalHashConst.capture_points_detail_alliance_occupy), level);
                    }
                    break;
                default:
                    if (itemData.PointInfo.Resource != null) {
                        level = itemData.PointInfo.Resource.Level;
                        if (itemData.Delta < 0) {
                            contendType = string.Format(LocalManager.GetValue(
                                LocalHashConst.capture_points_detail_beoccupied), level);
                        } else {
                            contendType = string.Format(
                                LocalManager.GetValue(LocalHashConst.capture_points_detail_occupy), level);
                        }
                    }
                    break;
            }

            itemView.SetCaptureCount(contendType, itemData.Delta, itemData.Total, () => {
                this.viewModel.MoveWithClick(itemData.PointInfo.Coord);
            });
        }       
    }
}
