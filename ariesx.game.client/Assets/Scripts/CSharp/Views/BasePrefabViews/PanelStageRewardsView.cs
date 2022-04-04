using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Protocol;
using System.Text;
using ProtoBuf;

namespace Poukoute {
    public class StageRewardDetail {
        public float stage;
        public bool receiveable;
        public bool taskDone;
        public int Id;
        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>(5);
    }

    public class PanelStageRewardsView: BaseView {
        #region ui element
        [Tooltip("PnlProgressSlider")]
        [SerializeField]
        private CustomSlider sldProgress;
        [SerializeField]
        private RectTransform rectSldProgress;
        [Tooltip("TxtCurrent")]
        [SerializeField]
        private TextMeshProUGUI txtCurrent;
        [SerializeField]
        private RectTransform rectCurrentValue;
        [Tooltip("ImgStar")]
        [SerializeField]
        private Transform imgPoint;
        [Tooltip("PnllRewardBG Button")]
        [SerializeField]
        private Button btnRewardBG;
        [Tooltip("PnlReward")]
        [SerializeField]
        private Transform pnlTaskReward;
        [SerializeField]
        private RectTransform taskRewardRT;
        [SerializeField]
        private CanvasGroup taskRewardCG;
        [SerializeField]
        private TextMeshProUGUI[] txtStage = new TextMeshProUGUI[5];
        [SerializeField]
        private Button[] btnState = new Button[5];
        [SerializeField]
        private GameObject[] Receiveable = new GameObject[5];
        [SerializeField]
        private GameObject[] Collected = new GameObject[5];
        [SerializeField]
        private TextMeshProUGUI txtStageDescription;
        [Tooltip("PnlReward.TxtDesc")]
        [SerializeField]
        private Transform txtRewardsDesc;
        [Tooltip("PnlReward.PnlList")]
        [SerializeField]
        private Transform pnlRewardList;
        [Tooltip("PnlReward.BtnReceive")]
        [SerializeField]
        private CustomButton btnReceive;
        [Tooltip("PnlReward.BtnReceive.PnlContent.Text")]
        [SerializeField]
        private TextMeshProUGUI txtReceiveTitle;
        [Tooltip("PnlReward.ImgBg.ImgArrow")]
        public RectTransform rectImgArrow;
        [SerializeField]
        private Transform campaignTips;
        #endregion
        // Other data
        private GameObject[] rewardResourceObj =
            new GameObject[20];
        private ItemWithCountView[] rewardResourceView =
            new ItemWithCountView[20];
        private float currentValue = 0;

        public List<StageRewardDetail> AllRewards = new List<StageRewardDetail>(5);
        //private List<IntegralReward> AllIntegralRewards = new List<IntegralReward>(5);
        private Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>(10);
        private Dictionary<Resource, Transform> resourceTransDict =
            new Dictionary<Resource, Transform>(10);

        private UnityAction<int> OnBtnReceiveClicked;
        private int currentRewardIndex;
        private const int sldMaxValue = 1000;
        private const float stageCount = 5;
        private const float stageInterval = 200;

        // Methods
        private void Awake() {
            if (this.btnRewardBG != null) {
                this.btnRewardBG.onClick.AddListener(this.HideStateRewardDetail);
            }

            this.sldProgress.onValueChanged.AddListener(this.OnSliderValueChange);
        }

        public Transform GetImgPointTransform() {
            return this.imgPoint;
        }

        public void SetStageInfo(List<IntegralReward> allIntegralRewards,
            float currentValue, UnityAction<int> OnGetSpecificPointRewards = null,
            bool showCanmpaignTips = false) {
            if (this.campaignTips != null) {
                this.campaignTips.gameObject.SetActiveSafe(showCanmpaignTips);
                this.btnReceive.gameObject.SetActiveSafe(!showCanmpaignTips);
            }          
            allIntegralRewards.Sort((a, b) => {
                return a.Integral.CompareTo(b.Integral);
            });
            this.AllRewards.Clear();
            int stageRewardId = -1;
            foreach (IntegralReward value in allIntegralRewards) {
                StageRewardDetail reward = new StageRewardDetail() {
                    stage = value.Integral,
                    taskDone = (currentValue >= value.Integral),
                    Id = stageRewardId,
                    receiveable = false,
                    resourcesDict = value.Reward.GetRewardsDict()
                };
                this.AllRewards.Add(reward);
            }
            this.currentValue = currentValue;
            this.InnerSetStageInfo();
            this.GetSliderTargetValue(currentValue);
        }

        public void SetStageInfo(List<DailyTaskVitalityConf> dailyTaskConf, float currentValue,
            List<int> canGetRewardList, UnityAction<int> OnGetSpecificPointRewards = null) {
            this.AllRewards.Clear();
            int stageRewardId = -1;
            foreach (DailyTaskVitalityConf value in dailyTaskConf) {
                stageRewardId = int.Parse(value.id);
                StageRewardDetail reward = new StageRewardDetail() {
                    stage = value.vitality,
                    taskDone = (currentValue >= value.vitality),
                    Id = stageRewardId,
                    receiveable = canGetRewardList.Contains(stageRewardId),
                    resourcesDict = value.resourcesDict
                };
                this.AllRewards.Add(reward);
            }
            this.currentValue = currentValue;
            this.OnBtnReceiveClicked = OnGetSpecificPointRewards;
            this.InnerSetStageInfo();
        }

        public void OnGetPointRewards(IExtensible message) {
            GetVitalityRewardAck forceReward = message as GetVitalityRewardAck;
            Protocol.Resources addResources = forceReward.Reward.Resources;
            Protocol.Currency addCurrency = forceReward.Reward.Currency;
            Protocol.Resources resources = forceReward.Resources;
            Protocol.Currency currency = forceReward.Currency;

            GameHelper.CollectResources(addResources, addCurrency,
                resources, currency, this.resourceTransDict);
        }

        private void InnerSetStageInfo() {
            this.txtCurrent.text = this.currentValue.ToString();
            this.sldProgress.maxValue = sldMaxValue;

            for (int i = 0; i < this.AllRewards.Count; i++) {
                this.txtStage[i].text = this.AllRewards[i].stage.ToString();
                this.btnState[i].onClick.RemoveAllListeners();
                int btnIndex = i;
                this.btnState[i].onClick.AddListener(() => {
                    this.OnStateRewardClick(btnIndex);
                });
            }
            int stage = this.UpdateStage();
            this.sldProgress.value = 0;
            this.sldProgress.value = this.VitalityToSldValue(stage, this.currentValue);
        }

        private int UpdateStage() {
            int currentStage = 0;
            StageRewardDetail stageReward;
            int rewardsCount = this.AllRewards.Count;
            for (int i = 0; i < rewardsCount; i++) {
                stageReward = this.AllRewards[i];
                stageReward.taskDone = (this.currentValue >= stageReward.stage);
                if (stageReward.taskDone) {
                    currentStage = i + 1;
                }
                this.SetStateMarkInfo(i, stageReward.taskDone, stageReward.receiveable);
            }
            return currentStage;
        }

        private float VitalityToSldValue(int stage, float vitality) {
            float stageCellValue = (stage / stageCount) * sldMaxValue;
            float maxValue = this.AllRewards[this.AllRewards.Count - 1].stage;
            float previousValue = stage == 0 ? 0 : this.AllRewards[stage - 1].stage;
            if (vitality >= maxValue) {
                return sldMaxValue;
            } else {
                return stageCellValue + stageInterval *
                    (vitality - previousValue) / (this.AllRewards[stage].stage - previousValue);
            }
        }

        public void ChangeStagePoint(float targetPoint) {
            this.currentValue = targetPoint;
            int stage = this.UpdateStage();
            float sldTargetPoint = this.VitalityToSldValue(stage, targetPoint);
            //this.sldProgress.ChangeTo(sldTargetPoint, () => {
                //this.txtCurrent.text = targetPoint.ToString();
            //});
        }

        private void SetStateMarkInfo(int index, bool isAchieve, bool isReceiveable) {
            this.Receiveable[index].SetActiveSafe(isAchieve && isReceiveable);
            this.Collected[index].SetActiveSafe(isAchieve && !isReceiveable);
        }

        public void HideStateRewardDetail() {
            if (this.taskRewardCG.alpha == 1) {
                AnimationManager.Animate(this.pnlTaskReward.gameObject, "Hide", () => {
                    this.SetRewardCGVisible(false);
                });
            }
        }

        public void HideStageRewardDetailImmediatly() {
            if (this.taskRewardCG.alpha == 1) {
                this.SetRewardCGVisible(false);
            }
        }

        private void SetRewardCGVisible(bool isAvaliable) {
            UIManager.SetUICanvasGroupEnable(this.taskRewardCG, isAvaliable);
        }

        private void ResetResourceInfo() {
            this.resourceTransDict.Clear();
            int childCount = this.pnlRewardList.childCount;
            for (int i = 0; i < childCount; i++) {
                GameObject itemObj = this.pnlRewardList.GetChild(i).gameObject;
                this.rewardResourceObj[i] = itemObj;
                ItemWithCountView itemView = itemObj.GetComponent<ItemWithCountView>();
                this.rewardResourceView[i] = itemView;
            }
        }

        private void OnSliderValueChange(float value) {
            int currentStage = Mathf.FloorToInt(value / stageInterval);

            // Set value text anchoredposition.
            float anchoredPosition = Mathf.Max(
                this.rectCurrentValue.rect.width,
                (value / sldMaxValue) * this.rectSldProgress.rect.width
            );
            this.rectCurrentValue.anchoredPosition = anchoredPosition * Vector2.right;

            // Set stage value.
            string currentStageLabel = this.currentValue.ToString();
            if (currentStageLabel.CustomEquals(this.txtCurrent.text)) {
                return;
            }
            if (currentStage >= stageCount) {
                this.txtCurrent.text = currentStageLabel;
            } else {
                float extraPercent = (value - currentStage * stageInterval) / stageInterval;
                float currentStageValue = currentStage == 0 ? 0 : this.AllRewards[currentStage - 1].stage;
                this.txtCurrent.text = Mathf.RoundToInt(currentStageValue +
                    extraPercent * (this.AllRewards[currentStage].stage - currentStageValue)
                ).ToString();
            }
        }


        private Vector2 RewardsOffset = new Vector2(-20, 75);
        private void OnStateRewardClick(int index) {
            if (this.taskRewardCG.alpha == 1 && this.currentRewardIndex == index) {
                this.HideStateRewardDetail();
            } else {
                this.currentRewardIndex = index;
                this.SetRewardCGVisible(true);
                StageRewardDetail stageReward = this.AllRewards[index];
                this.resourceDict = stageReward.resourcesDict;
                this.SetRewardsInfo();
                this.SetBtnReceiveClickedCallback(stageReward);

                RectTransform stateRT = this.btnState[index].GetComponent<RectTransform>();
                Vector2 taskRewardPivot = this.taskRewardRT.pivot;
                taskRewardPivot.x = ((0.82f - 0.16f) / (this.btnState.Length - 1)) * index + 0.16f;
                this.rectImgArrow.anchorMin =
                this.rectImgArrow.anchorMax = new Vector2(taskRewardPivot.x, 0);
                this.taskRewardRT.pivot = taskRewardPivot;
                this.taskRewardRT.anchoredPosition3D = stateRT.anchoredPosition3D;
                AnimationManager.Animate(this.pnlTaskReward.gameObject, "Show",
                    start: this.RewardsOffset, target: this.RewardsOffset, finishCallback: () => {
                        this.taskRewardRT.localScale = Vector3.one;
                    }, isOffset: true
                );
            }
        }

        private void SetRewardsInfo() {
            GameHelper.ResizeChildreCount(this.pnlRewardList,
                this.resourceDict.Count, PrefabPath.pnlItemWithCount);
            this.ResetResourceInfo();
            int i = 0;
            foreach (Resource resource in this.resourceDict.Keys) {
                rewardResourceObj[i].SetActiveSafe(true);
                this.resourceTransDict.Add(resource, rewardResourceView[i].imgItem.transform);
                rewardResourceView[i++].SetResourceInfo(resource, this.resourceDict[resource]);
            }
        }

        private void SetBtnReceiveClickedCallback(StageRewardDetail stageReward) {
            this.UpdateTxtReceiveTitle((stageReward.taskDone && !stageReward.receiveable));
            this.btnReceive.Grayable = !(stageReward.taskDone && stageReward.receiveable);
            this.btnReceive.onClick.RemoveAllListeners();
            if (stageReward.taskDone && stageReward.receiveable) {
                this.btnReceive.onClick.AddListener(
                    this.OnVitalityRewardsAvaliable);
            } else if (!stageReward.taskDone) {
                this.btnReceive.onClick.AddListener(
                    this.OnVitalityRewardsShortVitality);
            } else if (!stageReward.receiveable) {
                this.btnReceive.onClick.AddListener(
                    this.OnVitalityRewardsHasBeeCollected);
            } else {
                Debug.LogError("Strange logic here!!!");
            }
        }

        private void OnVitalityRewardsAvaliable() {
            StageRewardDetail stageReward =
                this.AllRewards[this.currentRewardIndex];
            //Debug.LogError("OnBtnReceiveClicked " + stageReward.Id +
            //    " " + (stageReward.taskDone && stageReward.receiveable));

            if (this.OnBtnReceiveClicked == null) {
                Debug.LogError("OnBtnReceiveClicked is null!");
            }
            this.OnBtnReceiveClicked.InvokeSafe(stageReward.Id);
            this.btnReceive.Grayable = true;
        }

        private void OnVitalityRewardsShortVitality() {
            UIManager.ShowTip(LocalManager.GetValue(
                    LocalHashConst.task_daily_vitality_short,
                    this.AllRewards[this.currentRewardIndex].stage), TipType.Info);
        }

        private void OnVitalityRewardsHasBeeCollected() {
            UIManager.ShowTip(LocalManager.GetValue(
                LocalHashConst.server_task_has_been_collected), TipType.Info);
        }

        private void UpdateTxtReceiveTitle(bool isReceived) {
            this.txtReceiveTitle.text = isReceived ?
                LocalManager.GetValue(LocalHashConst.button_received) :
                LocalManager.GetValue(LocalHashConst.button_receive);
        }

        private float GetSliderTargetValue(float curValue) {
            float maxValue = this.sldProgress.maxValue;
            float perPercent = 1.0f / GameConst.DAILY_TASK_STAGE;
            int max = GameConst.DAILY_TASK_STAGE - 1;
            for (int i = 0; i < max; i++) {
                if (curValue <= this.AllRewards[i].stage) {
                    if (i < 1) {
                        return (curValue / this.AllRewards[i].stage) * perPercent * maxValue;
                    } else {
                        float left = curValue - this.AllRewards[i - 1].stage;
                        float offset = this.AllRewards[i].stage - this.AllRewards[i - 1].stage;
                        return ((left / offset) * perPercent + perPercent * (i - 1)) * maxValue;
                    }
                } else if (curValue > this.AllRewards[i].stage &&
                    curValue <= this.AllRewards[i + 1].stage) {
                    float left = curValue - this.AllRewards[i].stage;
                    float offset = this.AllRewards[i + 1].stage - this.AllRewards[i].stage;
                    return ((left / offset) * perPercent + perPercent * (i + 1)) * maxValue;
                }
            }
            return maxValue;
        }
    }

}