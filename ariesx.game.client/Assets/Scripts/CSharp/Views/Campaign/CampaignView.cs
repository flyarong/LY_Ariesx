using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class CampaignView: BaseView {
        private CampaignViewModel viewModel;
        private CampaignViewPreference viewPref;
        private MapTileViewModel mapTileViewModel;
        private Vector3 ArrowStartPosition = new Vector3(86, -172, 0);
        private Vector3 childAnchor3D = Vector3.zero;
        private RectTransform selectedActivityRT = null;
        private static string timeFormat = "yyyy.MM.dd";
        private bool bannerValiable = false;
        //private Dictionary<string, Texture2D> bannerDict = new Dictionary<string, Texture2D>();

        /********************************************************************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<CampaignViewModel>();
            this.ui = UIManager.GetUI("UICampaign");
            this.viewPref = this.ui.transform.GetComponent<CampaignViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.scrollRect.onDrag.AddListener(this.OnScrollRectBeginDrag);
            this.viewPref.scrollRect.onEndDrag.AddListener(this.OnScrollRectEndDrag);
            this.viewPref.btnShowCampaignRule.onClick.AddListener(ShowCampaignRuleClick);
        }

        private void ShowCampaignRuleClick() {
            if (this.viewModel.ChosenActivity!=null) {
                this.viewModel.ShowCampaignRules(
                this.viewModel.ChosenActivity.GetActivityBody());
            } else {
                this.viewModel.ShowCampaignRules("空空空");
            }            
        }

        public void SetBtnRuleActive(bool show) {
            this.viewPref.btnShowCampaignRule.gameObject.SetActiveSafe(show);
        }

        private List<CampaignDemonItemView> campaignDemoList = new List<CampaignDemonItemView>(4);
        public void SetActivitiesInfo(Activity activity = null) {
            int count = this.viewModel.OpenServiceActivityCout;
            bool isActivities = activity != null;
            int activitiesCount = isActivities ?
                count + this.viewModel.UsableActivityCount : count;

            GameHelper.ResizeChildreCount(
                this.viewPref.pnlList, activitiesCount, PrefabPath.pnlCampaignDemon);
            //this.SetArrowStartPosition();
            this.campaignDemoList.Clear();
            int initialIndex = 0;
            for (int i = 0; i < activitiesCount; i++) {
                int childIndex = i;
                if (count > 0) {
                    childIndex = i;
                    count--;
                    CampaignDemonItemView itemView =
                    this.viewPref.pnlList.GetChild(i).GetComponent<CampaignDemonItemView>();
                    itemView.SetContent(null, () => {
                        this.OnActivityClick(null, childIndex);
                    });
                    this.campaignDemoList.Add(itemView);
                } else {
                    Activity tmpActivity = this.viewModel.AllActivities[i - this.viewModel.OpenServiceActivityCout];
                    if (tmpActivity.Status == Activity.ActivityStatus.None) {
                        continue;
                    }
                    if (activity.Base.Id.CustomEquals(tmpActivity.Base.Id)) {
                        initialIndex = i - count;
                    }
                    CampaignDemonItemView itemView =
                    this.viewPref.pnlList.GetChild(i).GetComponent<CampaignDemonItemView>();

                    itemView.SetItemChosen(false);
                    itemView.SetContent(this.viewModel.AllActivities[i - this.viewModel.OpenServiceActivityCout], () => {
                        this.OnActivityClick(tmpActivity, childIndex);
                    });
                    this.campaignDemoList.Add(itemView);
                }
            }
            //Debug.LogError("AllActivities " + this.viewModel.AllActivities.Count);
            
            this.OnChosenActivityChange(isActivities ?
                this.viewModel.AllActivities[initialIndex- this.viewModel.OpenServiceActivityCout] : null);
            this.SetArrowStartPosition(initialIndex);
            this.campaignDemoList[initialIndex].SetItemChosen(true);
        }

        // private callbacks
        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
        
        private void OnChosenActivityChange(Activity activity = null) {
            bool isActivity = activity != null;
            this.viewPref.imgBanners.gameObject.SetActiveSafe(!isActivity);
            if (isActivity) {
                string startTimeStr = GameHelper.DateFormat(activity.Base.PrepareTime, timeFormat);
                string endTimeStr = GameHelper.DateFormat(activity.Base.EndTime, timeFormat);
                this.viewPref.txtDuration.text = string.Concat(startTimeStr, "-", endTimeStr);
                string url = activity.Base.Banner;
                this.bannerValiable = this.viewModel.BannerDict.ContainsKey(url);
                this.viewPref.pnlImgLoading.gameObject.SetActiveSafe(!this.bannerValiable);
                UIManager.SetUICanvasGroupVisible(this.viewPref.bannerCanvasGroup, this.bannerValiable);
                if (this.bannerValiable) {
                    this.viewPref.imgBanner.texture = this.viewModel.BannerDict[url];
                }
                this.viewModel.ChosenActivity = activity;
                this.SetBtnRuleActive(activity.CampaignType != CampaignType.domination);
            } else {
                Debug.LogError("没有Banner");
                this.viewModel.ChosenActivity = null;
                UIManager.SetUICanvasGroupVisible(this.viewPref.bannerCanvasGroup, false);
                this.viewPref.pnlImgLoading.gameObject.SetActiveSafe(false);
                //this.viewPref.imgBanners.sprite = ArtPrefabConf.GetSprite("Banners");
                this.viewPref.txtDuration.text = string.Empty;
            }
        }

        public void SetBanner(WWW www, string url) {
            if (this.IsVisible &&
                this.viewModel.ChosenActivity != null &&
                this.viewModel.ChosenActivity.Base.Banner.CustomEquals(url) &&
                !this.bannerValiable) {
                this.viewPref.pnlImgLoading.gameObject.SetActiveSafe(false);
                UIManager.SetUICanvasGroupVisible(this.viewPref.bannerCanvasGroup, true);
                this.viewPref.imgBanner.texture = www.texture;
                this.bannerValiable = true;
            }
        }        

        private void OnActivityClick(Activity activity = null, int childIndex = 0) {
            if (activity != null) {
                this.ResetCampaignDemonIsChosen(this.viewModel.ChosenActivity);
            }
            this.OnChosenActivityChange(activity);
            this.campaignDemoList[childIndex].SetItemChosen(true);
            AnimationManager.Animate(this.viewPref.pnlArrow.gameObject, "Move",
                start: this.viewPref.arrowRT.anchoredPosition3D,
                target: this.SetArrowPosition(childIndex));
        }

        private Vector3 SetArrowPosition(int index) {
            selectedActivityRT = this.viewPref.pnlList.GetChild(index).GetComponent<RectTransform>();
            childAnchor3D = selectedActivityRT.anchoredPosition3D;
            childAnchor3D.x += this.viewPref.listRectTransform.anchoredPosition3D.x;
            childAnchor3D.x += this.viewPref.CampaignsRT.anchoredPosition3D.x;
            childAnchor3D.y = this.viewPref.arrowRT.anchoredPosition3D.y;
            return childAnchor3D;
        }

        public void SetArrowStartPosition(int index) {
            Vector3 vector = index == 0 ? this.ArrowStartPosition : this.SetArrowPosition(index);
            Debug.LogError(index + " " + vector + " " + this.SetArrowPosition(index));
            AnimationManager.Animate(this.viewPref.pnlArrow.gameObject, "Move",
              start: this.viewPref.arrowRT.anchoredPosition3D,
              target: vector);
            
        }

        // To do: rewrite, because some dont have Campaing.Base.
        private void ResetCampaignDemonIsChosen(Activity activity) {
            if (activity == null) {
                return;
            }
            int campaignDemoCount = this.campaignDemoList.Count;
            for (int i = 0; i < campaignDemoCount; i++) {
                i = i + this.viewModel.OpenServiceActivityCout;
                if (this.campaignDemoList[i].Campaign.Base.Id == activity.Base.Id) {
                    this.campaignDemoList[i].SetItemChosen(false);
                    return;
                }
            }
        }
        private void OnScrollRectBeginDrag() {
            this.viewPref.pnlArrow.gameObject.SetActiveSafe(false);
        }

        private void OnScrollRectEndDrag() {
            this.viewPref.pnlArrow.gameObject.SetActiveSafe(true);
        }
    }
}
