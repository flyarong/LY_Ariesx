using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class DailyItemPreference : BaseViewPreference, IPoolHandler {
        public RectTransform iconRectTransform;
        public Image headImage;

        public Slider slider;

        public TextMeshProUGUI txtAmount;

        public CustomButton btnItem;

        public TextMeshProUGUI buildingName;

        public TextMeshProUGUI buildingStatus;

        public Transform pnlIdleAnimList;

        public TextMeshProUGUI txtButton;
        [Tooltip("Pnl2lDailyItem/PnlUpIcon")]
        public RectTransform pnlUpicon;
        [Tooltip("Pnl2lDailyItem/PnlUpIcon/TxtOldLevel")]
        public TextMeshProUGUI txtOldLevel;
        [Tooltip("Pnl2lDailyItem/PnlUpIcon/TxtNewLevel")]
        public TextMeshProUGUI txtNewLevel;
        [Tooltip("Pnl2lDailyItem/TxtName")]
        public TextMeshProUGUI txtName;
        public void OnInPool() {
            this.btnItem.gameObject.SetActive(false);
            this.slider.gameObject.SetActive(false);
        }

        public void OnOutPool() {
            this.slider.gameObject.SetActive(true);
        }
    }

}
