using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class DailyTaskView : SRIA<DailyTaskViewModel, DailyTaskItemView> {
        private DailyTaskViewPreference viewPref;

        public List<DailyTaskVitalityConf> taskRewardsConfList =
            new List<DailyTaskVitalityConf>(GameConst.DAILY_TASK_STAGE);

        //StringBuilder sbuilder = new StringBuilder();

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMission.PnlMission.PnlDailyTask");
            this.viewModel = this.gameObject.GetComponent<DailyTaskViewModel>();
            this.viewPref = this.ui.transform.GetComponent<DailyTaskViewPreference>();

            this.viewPref.ccBackground.onClick.AddListener(this.OnCCBackgroundClick);
            this.viewPref.scrollRect.onBeginDrag.AddListener(
                this.viewPref.stageRewardView.HideStageRewardDetailImmediatly
            );

            this.SRIAInit(this.viewPref.scrollRect,
                          this.viewPref.verticalLayoutGroup,
                          defaultItemSize: 103);
        }

        #region SRIA implementation
        protected override DailyTaskItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlDailyTaskItem, this.viewPref.pnlList);
            DailyTaskItemView itemView = itemObj.GetComponent<DailyTaskItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.TaskList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(DailyTaskItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.TaskList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(DailyTaskItemView itemView, Task itemData) {
            itemView.pnlDestination = this.viewPref.stageRewardView.GetImgPointTransform();
            itemView.pnlDeparture = this.viewPref.pnlStarPos;
            itemView.Task = itemData;
            itemView.OnGetTaskRewards = this.GetTaskRewards;
            itemView.OnReceiveVitality = this.OnReceiveVitality;
            DailyTaskConf dailyTaskConf = DailyTaskConf.GetConf(itemData.Id.ToString());
            itemView.OnGoClick.AddListener(
                this.viewModel.GetJumpAction(dailyTaskConf));
        }


        public override void Hide(UnityAction callback = null) {
            if (this.IsUIInit) {
                this.viewPref.stageRewardView.HideStageRewardDetailImmediatly();
            }
            base.Hide(callback);
        }

        public void SetViewContent(bool isUnlocked) {
            this.viewPref.pnlUnlockTips.gameObject.SetActiveSafe(!isUnlocked);
            this.viewPref.scrollRect.gameObject.SetActive(isUnlocked);
            UIManager.SetUICanvasGroupEnable(this.viewPref.topCG, isUnlocked);
            UIManager.SetUICanvasGroupEnable(this.viewPref.pnlContent, isUnlocked);
            if (isUnlocked) {
                this.SetDailyTask();
            }
        }

        private void RefreshDescription() {
            long leftTime = this.viewModel.DailyExpireAt * 1000 - RoleManager.GetCurrentUtcTime();
            this.viewPref.txtDescription.text = string.Format(
                LocalManager.GetValue(LocalHashConst.daily_task_desc),
                string.Concat("<color=#FFE650FF>", GameHelper.TimeFormat(leftTime), "</color>")
            );
        }

        public void SetDailyTaskTopInfo() {
            this.RefreshDescription();
            int vitalityIndex = 0;
            this.taskRewardsConfList.Clear();
            int startIndex = this.viewModel.CurrentTaskTownhallLevel - 3;
            for (int i = 0; i < GameConst.DAILY_TASK_STAGE; i++) {
                vitalityIndex = startIndex * GameConst.DAILY_TASK_STAGE + (i + 1);
                DailyTaskVitalityConf vitalityConf = DailyTaskVitalityConf.GetConf(
                    string.Concat(this.viewModel.CurrentTaskTownhallLevel, vitalityIndex));
                this.taskRewardsConfList.Add(vitalityConf);
            }
            this.SetStageInfo();
        }

        private void SetStageInfo() {
            this.viewPref.stageRewardView.SetStageInfo(this.taskRewardsConfList,
                                                        this.viewModel.AllVitality,
                                                        this.viewModel.CanGetRewardList,
                                                        this.OnGetSpecificPointRewards);
        }


        public void RefresTaskItem(Task task) {
            int itemViewCount = this._VisibleItems.Count;
            DailyTaskItemView dailyTaskItemView;
            for (int i = 0; i < itemViewCount; i++) {
                dailyTaskItemView = this._VisibleItems[i];
                if (dailyTaskItemView.Task.Id == task.Id) {
                    dailyTaskItemView.UpdateProgress(task.Value, task.IsDone);
                    return;
                }
            }
        }

        public void SetDailyTask() {
            this.ResetItems(this.viewModel.TaskList.Count);
        }

        private void OnCCBackgroundClick() {
            this.viewPref.stageRewardView.HideStateRewardDetail();
        }

        public void OnGetVitalityRewardAck(IExtensible message) {
            this.viewPref.stageRewardView.OnGetPointRewards(message);
            this.SetStageInfo();
        }

        private void OnReceiveVitality(int vitality, int taskId) {
            this.viewModel.SetPoint();
            this.viewPref.stageRewardView.ChangeStagePoint(this.viewModel.AllVitality + vitality);
            this.viewModel.OnDailyTaskRefresh(taskId, !this.isGettingTaskRewards);
            this.viewModel.AllVitality += vitality;
        }

        private bool isGettingTaskRewards = false;
        private void GetTaskRewards(int taskId, UnityAction<IExtensible> callback) {
            this.isGettingTaskRewards = true;
            GetTaskRewardReq getTaskRewardsReq = new GetTaskRewardReq() {
                Id = taskId
            };
            NetManager.SendMessage(getTaskRewardsReq, typeof(GetTaskRewardAck).Name,
                (msg) => {
                    this.isGettingTaskRewards = false;
                    callback.InvokeSafe(msg);
                });
        }

        private void OnGetSpecificPointRewards(int confListIndex) {
            this.viewModel.GetVitalityReward(confListIndex);
        }
    }
}
