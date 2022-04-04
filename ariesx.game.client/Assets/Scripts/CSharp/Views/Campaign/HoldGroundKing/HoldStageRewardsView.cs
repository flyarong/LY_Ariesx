using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {

    public class HoldStageRewardsView: BaseView {

        [Tooltip("PnlProgressSlider")]
        public Slider sliProgress;
        [Tooltip("TxtCurrent")]
        public TextMeshProUGUI txtCurrent;
        [Tooltip("ImgStar")]
        public Transform pnlCurStar;

        [Tooltip("PnlStages.PnllRewardBG")]
        public CanvasGroup taskRewardBGCG;
        public Button btnRewardBG;
        [Tooltip("PnlReward")]
        public Transform pnlTaskReward;
        public RectTransform taskRewardRT;
        public CanvasGroup taskRewardCG;

        public RectTransform rewardBGRT;

        public TextMeshProUGUI[] txtStage = new TextMeshProUGUI[5];
        public Button[] btnState = new Button[5];
        public GameObject[] Receiveable = new GameObject[5];
        public GameObject[] Collected = new GameObject[5];

        public TextMeshProUGUI txtStageDescription;

        [Tooltip("PnlReward.TxtDesc")]
        public Transform txtRewardsDesc;
        [Tooltip("PnlReward.PnlList")]
        public Transform pnlRewardList;
        [Tooltip("PnlReward.BtnReceive")]
        //public CustomButton btnReceive;
        public TextMeshProUGUI txtReceiveTitle;



        private GameObject[] rewardResourceObj =
           new GameObject[20];
        private ItemWithCountView[] rewardResourceView =
            new ItemWithCountView[20];
        private List<IntegralReward> AllRewards = new List<IntegralReward>(10);
        private Dictionary<Resource, int> resourceDict =
            new Dictionary<Resource, int>(10);
        private Dictionary<Resource, Transform> resourceTransDict =
            new Dictionary<Resource, Transform>(10);

        public UnityAction<int> GetVitalityReward;

        void Awake() {
            this.btnRewardBG.onClick.AddListener(this.HideStateRewardDetail);
        }


        public void SetSetStageEvent(List<IntegralReward> allRewards, int currentIndex) {
            allRewards.Sort((a, b) => {
                return a.Integral.CompareTo(b.Integral);
            });
            this.AllRewards = allRewards;
            this.txtCurrent.text = currentIndex.ToString();
            int rewardsCount = allRewards.Count;
            //bool isAchieve = false;
            //bool isReceiveable = false;
            //string vitalityKey = string.Empty;
            for (int i = 0; i < rewardsCount; i++) {
                this.txtStage[i].text = allRewards[i].Integral.ToString();
                //isAchieve = currentIndex >= allRewards[i].Integral;
                //isReceiveable = true;
                int btnIndex = i;
                this.btnState[btnIndex].onClick.RemoveAllListeners();
                this.btnState[btnIndex].onClick.AddListener(() => {
                    this.OnStateRewardClick(btnIndex);                    
                });
            }
            Debug.LogError(allRewards[rewardsCount - 1].Integral + " " + currentIndex + " " +
                this.GetSliderTargetValue(currentIndex));
            //this.GetSliderTargetValue(currentIndex);
            this.sliProgress.maxValue = allRewards[rewardsCount - 1].Integral;
            this.sliProgress.value = this.GetSliderTargetValue(currentIndex);
        }

        private float GetSliderTargetValue(float curValue) {
            //Control progress bar progress
            float maxValue = this.sliProgress.maxValue;
            float perPercent = 1.0f / GameConst.DAILY_TASK_STAGE;
            int max = GameConst.DAILY_TASK_STAGE - 1;
            for (int i = 0; i < max; i++) {
                if (curValue <= this.AllRewards[i].Integral) {
                    if (i < 1) {
                        return (curValue / this.AllRewards[i].Integral) * perPercent * maxValue;
                    } else {
                        float left = curValue - this.AllRewards[i - 1].Integral;
                        float offset = this.AllRewards[i].Integral - this.AllRewards[i - 1].Integral;
                        return ((left / offset) * perPercent + perPercent * (i - 1)) * maxValue;
                    }
                } else if (curValue > this.AllRewards[i].Integral && curValue <=
                    this.AllRewards[i + 1].Integral) {
                    float left = curValue - this.AllRewards[i].Integral;
                    float offset = this.AllRewards[i + 1].Integral - this.AllRewards[i].Integral;
                    return ((left / offset) * perPercent + perPercent * (i + 1)) * maxValue;
                }
            }
            return maxValue;
        }

        private void OnStateRewardClick(int btnIndex) {
            if (this.taskRewardBGCG.alpha == 1) {
                this.HideStateRewardDetail();
            } else {
                this.SetRewardCGVisible(true);
                this.resourceDict = this.AllRewards[btnIndex].Reward.GetRewardsDict();
                GameHelper.ResizeChildreCount(this.pnlRewardList, this.resourceDict.Count,
                    PrefabPath.pnlItemWithCount);
                this.ResetResourceInfo();
                int i = 0;
                foreach (Resource item in this.resourceDict.Keys) {
                    rewardResourceObj[i].SetActiveSafe(true);
                    this.resourceTransDict.Add(item, rewardResourceView[i].imgItem.transform);
                    rewardResourceView[i++].SetResourceInfo(item, this.resourceDict[item]);
                }
                //Control reward display location
                RectTransform rectTransform = this.btnState[btnIndex].GetComponent<RectTransform>();
                Vector3 vector3 = rectTransform.anchoredPosition3D;
                Vector2 taskRewardPivot = this.taskRewardRT.pivot;
                bool isRight = btnIndex >= (this.btnState.Length - 1) / 2;
                taskRewardPivot.x = isRight ? 0.9f : 0.1f;
                this.taskRewardRT.pivot = taskRewardPivot;
                Quaternion rewardQuaternion = this.rewardBGRT.localRotation;
                rewardQuaternion.y = isRight ? -180f : 0f;
                this.rewardBGRT.localRotation = rewardQuaternion;
                this.taskRewardRT.anchoredPosition3D = rectTransform.anchoredPosition3D;
                AnimationManager.Animate(this.pnlTaskReward.gameObject, "Show", () => {
                    this.taskRewardRT.localScale = Vector3.one;
                });
            }
        }

        private void ResetResourceInfo() {
            //Control reward information
            this.resourceTransDict.Clear();
            int childCount = this.pnlRewardList.childCount;
            for (int i = 0; i < childCount; i++) {
                GameObject itemObj = this.pnlRewardList.GetChild(i).gameObject;
                this.rewardResourceObj[i] = itemObj;
                ItemWithCountView itemView = itemObj.GetComponent<ItemWithCountView>();
                this.rewardResourceView[i] = itemView;
            }
        }

        private void HideStateRewardDetail() {
            if (this.taskRewardBGCG.alpha == 1) {
                AnimationManager.Animate(this.pnlTaskReward.gameObject, "Hide", () => {
                    this.SetRewardCGVisible(false);
                });
            }
        }
        private void SetRewardCGVisible(bool isAvaliable) {
            UIManager.SetUICanvasGroupEnable(this.taskRewardBGCG, isAvaliable);
            UIManager.SetUICanvasGroupEnable(this.taskRewardCG, isAvaliable);
        }
    }

}
