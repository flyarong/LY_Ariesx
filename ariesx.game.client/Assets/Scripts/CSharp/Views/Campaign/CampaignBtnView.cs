using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using ProtoBuf;
using UnityEngine.Events;

namespace Poukoute {
    public class CampaignBtnView: MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI txtRemainTime;
        [SerializeField]
        private Image imgTextBG;
        [SerializeField]
        private Image imgCampaignIcon;
        [SerializeField]
        private CustomButton button;
        [SerializeField]
        private Transform pnlContent;

        private GameObject pnlCampaign;
        private CampaignModel model;
        private CampaignType type = CampaignType.none;
        private int index = 0;
        private Activity activity = null;
        private Activity.ActivityStatus status = Activity.ActivityStatus.None;

        void Awake() {
            this.pnlCampaign = this.transform.parent.gameObject;
            this.model = ModelManager.GetModelData<CampaignModel>();
            EventManager.AddEventAction(Event.Campaign, this.UpdateRemainTimeInfo);
            NetHandler.AddNtfHandler(typeof(ActivityNtf).Name, (message) => {
                this.StartChangeCampaign();
            });
        }

        public Activity GetCurrentDisplayActivity() {
            if (this.activity == null) {
                return null;
            }
            return this.activity;
        }

        private void StartChangeCampaign() {
            CancelInvoke("ChangeCampaign");
            int activityCount = this.model.GetUsableActivityCount();
            if (activityCount > 1) {
                InvokeRepeating("ChangeCampaign", 0, 120);
            } else if (activityCount == 1) {
                this.ChangeCampaign();
            } else {
                this.gameObject.SetActive(false);
                this.DelayOfPrepareActivity();
            }
        }

        private void DelayOfPrepareActivity() {
            foreach (Activity activity in this.model.allActivities) {
                if (activity.Status == Activity.ActivityStatus.None) {
                    Invoke("StartChangeCampaign", activity.ActivityRemainTime);
                }
            }
        }

        private void ChangeCampaign() {
            if (this.index >= this.model.allActivities.Count) {
                this.index = 0;
            }
            this.activity = this.model.allActivities[index++];
            Activity.ActivityStatus newStatus = this.activity.Status;
            if (newStatus == Activity.ActivityStatus.None) {
                this.model.allActivities.TryRemove(this.activity);
                this.ChangeCampaign();
                return;
            }
            this.gameObject.SetActive(true);
            AnimationManager.Animate(this.pnlCampaign, "Hide", () => {
                EventManager.EventDict[Event.Campaign].Clear();
                this.UpdateCampaignStatus(this.activity.Status);
                this.SetContent(activity.CampaignType, this.activity.Status);
                AnimationManager.Animate(this.pnlCampaign, "Show");
            });
        }

        private void UpdateCampaignStatus(Activity.ActivityStatus status) {
            //long remainTime = activity.ActivityRemainTime;
            //EventManager.AddCampaignEvent(remainTime * 1000);
            // To do: reset AddCampaignEvent eventDict in foreach??
            if (this.gameObject.activeSelf && this.gameObject.activeInHierarchy) {
                StartCoroutine(this.InnerAddCapaignEvent());
                this.SetCampaignInfo();
                this.status = status;
            }
        }

        private IEnumerator InnerAddCapaignEvent() {
            yield return YieldManager.EndOfFrame;
            EventManager.AddCampaignEvent(activity.ActivityRemainTime * 1000);
        }

        private void SetContent(CampaignType campaignType, Activity.ActivityStatus activityStatus) {
            if (this.type != campaignType) {
                this.type = campaignType;
                string campaignSuffix = campaignType.ToString();
                if (campaignType == CampaignType.melee && CampaignModel.monsterType == CampaignModel.nian) {
                    campaignSuffix = string.Concat(campaignSuffix, "_nian");
                }
                this.imgCampaignIcon.sprite =
                    ArtPrefabConf.GetSprite("campaign_", campaignSuffix);
            }
            if (this.status != activityStatus) {
                this.status = activityStatus;
            }
        }

        private void UpdateRemainTimeInfo(EventBase eventBase) {
            long left = eventBase.startTime + eventBase.duration - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            this.txtRemainTime.text = GameHelper.TimeFormat(left);
            if (left == 0) {
                this.UpdateCampaignStatus(this.activity.Status);
                this.SetCampaignInfo();
            }
        }

        private void SetCampaignInfo() {
            bool campaignStatusFinish = (this.activity.Status == Activity.ActivityStatus.Finish ||
                this.activity.Status == Activity.ActivityStatus.None);
            this.txtRemainTime.gameObject.SetActiveSafe(!campaignStatusFinish);
            this.imgTextBG.gameObject.SetActiveSafe(!campaignStatusFinish);
            this.button.Grayable = campaignStatusFinish;
        }
    }
}