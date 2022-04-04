using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class PointsStatisticsItemView : BaseItemViewsHolder {
        #region UI elements
        [SerializeField]
        private TextMeshProUGUI txtgetGroundLevel;
        [SerializeField]
        private CustomButton btnCoordinate;
        [SerializeField]
        private TextMeshProUGUI txtGetTime;
        [SerializeField]
        private TextMeshProUGUI txtGetTimes;
        [SerializeField]
        private TextMeshProUGUI txtCoordinate;
        [SerializeField]
        private TextMeshProUGUI txtIntegralcount;
        [SerializeField]
        private TextMeshProUGUI txtpointscount;
        #endregion

        private static string timeFormat = "yyyy.MM.dd";
        //Capture
        private CaptureLog captureLog;
        public CaptureLog CaptureLog {
            get {
                return this.captureLog;
            }
            set {
                if (this.captureLog != value) {
                    this.captureLog = value;
                    this.txtCoordinate.text = "[" + captureLog.PointInfo.Coord.X.ToString() +
                        "," + captureLog.PointInfo.Coord.Y.ToString() + "]";
                }
            }
        }

        public void SetCaptureCount(string groundLevel, int integral,
            int pointscount, UnityAction clickCallback) {
            this.txtGetTime.text =
                GameHelper.DateFormat(this.captureLog.CreatedAt, timeFormat);
            this.txtGetTimes.text =
                GameHelper.HistoryTimeFormat(this.captureLog.CreatedAt);
            this.txtgetGroundLevel.text = groundLevel;
            this.txtIntegralcount.text = integral.ToString();
            this.txtpointscount.text = pointscount.ToString();
            this.btnCoordinate.onClick.RemoveAllListeners();
            this.btnCoordinate.onClick.AddListener(clickCallback);
        }

        //occupy
        public int Index = -1;
        private OccupyLog occupyLog;
        public OccupyLog OccupyLog {
            get {
                return this.occupyLog;
            }
            set {
                if (this.occupyLog != value) {
                    this.occupyLog = value;
                    this.txtCoordinate.text =
                        "[" + occupyLog.PointInfo.Coord.X.ToString() +
                        "," + occupyLog.PointInfo.Coord.Y.ToString() + "]";

                }
            }
        }

        public void SetCount(string groundLevel, int integral, int pointscount, UnityAction clickCallback) {
            this.txtGetTime.text =
                GameHelper.DateFormat(this.occupyLog.CreatedAt, timeFormat);
            this.txtGetTimes.text =
                GameHelper.HistoryTimeFormat(this.occupyLog.CreatedAt);
            this.txtgetGroundLevel.text = groundLevel;
            this.txtIntegralcount.text = integral.ToString();
            this.txtpointscount.text = pointscount.ToString();
            this.btnCoordinate.onClick.RemoveAllListeners();
            this.btnCoordinate.onClick.AddListener(clickCallback);
        }

    }
}