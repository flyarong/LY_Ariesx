using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Poukoute {
    public class CampaignDemonItemView: MonoBehaviour {
        #region UI element
        [SerializeField]
        private Image imgActivityBG;
        [SerializeField]
        private Image imgActivityIcon;
        [SerializeField]
        private TextMeshProUGUI txtActivityName;
        [SerializeField]
        private Button btnAcvity;
        [SerializeField]
        private CanvasGroup noticeNewCG;
        [SerializeField]
        private CanvasGroup endMark;
        [SerializeField]
        private Image isChosenIcon;
        #endregion

        public Activity Campaign {
            get; set;
        }

        private bool isClicked = false;

        public void SetContent(Activity activity, UnityAction OnActivityClick) {
            if (activity!=null) {
                this.Campaign = activity;
                // To do Set acvitiyContent
                string activityType = string.Concat("campaign_", activity.CampaignType.ToString());
                if (activity.Melee != null && CampaignModel.monsterType == CampaignModel.nian) {
                    activityType = string.Concat(activityType, "_nian");
                }
                this.txtActivityName.text = LocalManager.GetValue(activityType);
                this.imgActivityIcon.sprite = ArtPrefabConf.GetSprite(activityType);
                this.SetStatusInfo(activity.Status);
            } else {
                this.txtActivityName.text = "开服活动";
                this.imgActivityIcon.sprite = ArtPrefabConf.GetSprite("campaign_melee_nian");
            }
            this.btnAcvity.onClick.RemoveAllListeners();
            this.btnAcvity.onClick.AddListener(OnActivityClick);

        }

        public void SetItemChosen(bool isChosen) {
            this.imgActivityBG.gameObject.SetActiveSafe(isChosen);
            this.isChosenIcon.sprite = ArtPrefabConf.GetSprite("campaign_Chosen_" + isChosen);
            this.txtActivityName.color = isChosen ? Color.yellow : Color.white;
            this.isClicked = (this.isClicked || isChosen);
            UIManager.SetUICanvasGroupVisible(this.noticeNewCG, false);
        }

        private void SetStatusInfo(Activity.ActivityStatus status) {
            bool isActivityEnd = (status == Activity.ActivityStatus.Finish);
            UIManager.SetUICanvasGroupVisible(this.endMark, isActivityEnd);
            bool isActivityNew = (status == Activity.ActivityStatus.Preheat) &&
                !this.isClicked;
            UIManager.SetUICanvasGroupVisible(this.noticeNewCG, isActivityNew);
        }
    }
}