using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class CampaignPromptingItemView: MonoBehaviour {
        [SerializeField]
        private Image pnlTitle;
        [SerializeField]
        private TextMeshProUGUI txtPoint;
        public void SetContent(Activity activity, string point) {
            string activityType = string.Concat("campaign_", activity.CampaignType.ToString());
            this.pnlTitle.sprite = ArtPrefabConf.GetSprite(activityType);
            this.txtPoint.text = string.Format(
                LocalManager.GetValue(LocalHashConst.occupy_map_tile),
                GameHelper.GetFormatNum(int.Parse(point)));
        }
    }
}
