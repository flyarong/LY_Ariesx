using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class DailyTaskViewModel : BaseViewModel {
        private MissionViewModel parent;
        private DailyTaskView view;
        private DailyTaskModel model;

        public List<Task> TaskList {
            get {
                if (this.IsDailyTaskUnlock() &&
                    this.NeedRefresh) {
                    this.GetDailyTaskReq();
                }
                return this.model.taskList;
            }
            set {
                if (this.model.taskList != value) {
                    this.model.taskList = value;
                }
            }
        }

        public int TownhallRequireLevel {
            get {
                return this.model.TownhallRequireLevel;
            }
        }

        public int CurrentTaskTownhallLevel {
            get {
                return this.model.CurrentTaskTownHallLevel;
            }
            set {
                this.model.CurrentTaskTownHallLevel = value;
            }
        }

        public int AllVitality {
            get {
                return this.model.AllVitality;
            }
            set {
                this.model.AllVitality = value;
            }
        }
        public int ChosenVitalityRewardId {
            get; set;
        }

        public List<int> CanGetRewardList {
            get {
                return this.model.vitalityCanGetReward;
            }
            set {
                this.model.vitalityCanGetReward = value;
            }
        }

        public long DailyExpireAt {
            get {
                return this.model.dailyExpireAt;
            }
            set {
                this.model.dailyExpireAt = value;
            }
        }

        public bool NeedRefresh = true;
        private bool isGetTaskDailyRewardInfo = false;
        private bool isNeedSetMapTaskDetail = false;
        private bool isNeedOpenView = false;
        private bool isRequestingDailyTask = false;
        private int mapShowTaskId = -1;
        private int dramaLoadIndex = 0;
        private string[] dailyTaskTypes = {
            GameConst.RESOURCE_OCCUPY,
            GameConst.HERO_LEVELUP_TIMES,
            GameConst.RECRUIT
        };

        private void Awake() {
            //ConfigureManager.Instance.LoadConfigure<DailyTaskConf>("task_daily");
            //ConfigureManager.Instance.LoadConfigure<DailyTaskVitalityConf>("task_daily_vitality");
            //ConfigureManager.Instance.LoadConfigure<DailyRewardConf>("daily_reward_limit");
            this.parent = this.transform.parent.GetComponent<MissionViewModel>();
            this.model = ModelManager.GetModelData<DailyTaskModel>();
            this.view = this.gameObject.AddComponent<DailyTaskView>();

            NetHandler.AddDataHandler(typeof(TaskChangeNtf).Name, this.TaskChangeNtf);
            NetHandler.AddNtfHandler(typeof(PlayerBuildingNtf).Name, this.PlayerBuildingNtf);
            this.GetDailyTaskReq();
        }

        public void Show() {
            if (this.IsDailyTaskUnlock()) {
                if (this.isGetTaskDailyRewardInfo) {
                    return;
                }
                this.isGetTaskDailyRewardInfo = true;
                GetTaskDailyReq getVitality = new GetTaskDailyReq();
                NetManager.SendMessage(getVitality,
                    typeof(GetTaskDailyAck).Name, this.GetTaskDailyAck);
            } else {
                this.view.Show();
                this.view.SetViewContent(false);
            }
        }

        public void Hide() {
            this.view.Hide(() => {
                this.isNeedOpenView = false;
                this.isGetTaskDailyRewardInfo = false;
            });
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            this.isNeedOpenView = this.view.IsVisible;
            if (this.IsDailyTaskUnlock()) {
                this.GetDailyTaskReq();
            }
        }

        public string GetDailyTips(out int taskId, out bool receiveable, out UnityAction jumpAction) {
            bool hasDailyTask = this.TaskList.Count > 0;
            this.isNeedSetMapTaskDetail = !hasDailyTask;
            taskId = -1;
            if (hasDailyTask) {
                jumpAction = null;
                receiveable = false;
                return string.Empty;
            }

            foreach (Task task in this.TaskList) {
                if (!task.IsCollect) {
                    receiveable = task.IsDone;
                    DailyTaskConf conf = DailyTaskConf.GetConf(task.Id.ToString());
                    jumpAction = this.GetJumpAction(conf);
                    taskId =
                    this.mapShowTaskId = task.Id;
                    return conf.GetContent();
                }
            }
            jumpAction = null;
            receiveable = false;
            return string.Empty;
        }

        public DailyTaskConf GetDailyTaskConfByType(string type, out UnityAction jumpAction) {
            DailyTaskConf conf;
            List<DailyTaskConf> dailyTask = new List<DailyTaskConf>(10);
            foreach (Task task in this.TaskList) {
                conf = DailyTaskConf.GetConf(task.Id.ToString());
                if (conf.type.CustomEquals(type)) {
                    dailyTask.Add(conf);
                }
            }
            if (dailyTask.Count > 0) {
                dailyTask.Sort(
                    delegate (DailyTaskConf a, DailyTaskConf b) {
                        return a.id.CompareTo(b.id);
                    });
                jumpAction = this.GetJumpAction(dailyTask[0]);
                return dailyTask[0];
            } else {
                jumpAction = null;
                return null;
            }
        }

        public void StartSpecificDailyTaskGuid() {
            bool hasGuidDailyTask = false;
            foreach (Task task in this.TaskList) {
                if (!task.IsDone) {
                    string dailyTaskType = this.GetDailyTaskGuidTaskType();
                    DailyTaskConf conf = DailyTaskConf.GetConf(task.Id.ToString());
                    if (dailyTaskType.CustomEquals(conf.type)) {
                        this.mapShowTaskId = task.Id;
                        this.parent.SetTaskDetail(task.Id,
                            TaskType.daily, conf.GetContent(), task.IsDone,
                            this.GetJumpAction(conf) + this.GuidOtherDailyTask);
                        ++this.dramaLoadIndex;
                        hasGuidDailyTask = true;
                        return;
                    }
                }
            }
            if (!hasGuidDailyTask) {
                this.parent.ShowDramaInMap();
            }
        }

        public void StartDailyTaskGuid() {
            bool hasGuidDailyTask = false;
            if (!this.parent.CheckDramNeedShow()) {
                foreach (Task task in this.TaskList) {
                    if (!task.IsDone) {
                        DailyTaskConf conf = DailyTaskConf.GetConf(task.Id.ToString());
                        this.mapShowTaskId = task.Id;
                        this.parent.SetTaskDetail(task.Id,
                            TaskType.daily, conf.GetContent(), task.IsDone,
                            this.GetJumpAction(conf));
                        hasGuidDailyTask = true;
                        return;
                    }
                }
            }
            if (!hasGuidDailyTask) {
                this.parent.ShowDramaInMap();
            }
        }

        private void InnerShow() {
            this.view.Show();
            this.view.SetDailyTaskTopInfo();
            if (this.NeedRefresh) {
                this.view.SetViewContent(true);
                this.NeedRefresh = false;
            } else {
                this.isNeedOpenView = true;
            }
        }

        private void GuidOtherDailyTask() {
            StartCoroutine(this.YieldGuidOtherDailyTask());
        }

        private IEnumerator YieldGuidOtherDailyTask() {
            yield return YieldManager.GetWaitForSeconds(GameConst.TASK_REFRESH_PERIOD);
            this.StartSpecificDailyTaskGuid();
        }

        private string GetDailyTaskGuidTaskType() {
            this.dramaLoadIndex =
                this.dramaLoadIndex < this.dailyTaskTypes.Length ? this.dramaLoadIndex : 0;
            return this.dailyTaskTypes[this.dramaLoadIndex];
        }

        public bool IsDailyTaskUnlock() {
            BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
            return (buildModel.GetBuildLevelByName(ElementName.townhall)
                                >= this.TownhallRequireLevel);
        }

        public UnityAction GetJumpAction(DailyTaskConf dailyTaskConf) {
            UnityAction action = null;
            switch (dailyTaskConf.type) {
                case GameConst.BUILDING_LEVELUP_TIMES:
                    action = () => {
                        this.JumpToBuilding();
                    };
                    break;
                case GameConst.RESOURCE_OCCUPY:
                case GameConst.RESOURCE_AMOUNT:
                case GameConst.RESOURCE_LEVEL:
                    action = () => {
                        this.JumpToTile(dailyTaskConf.level);
                    };
                    break;
                case GameConst.HERO_LEVELUP_TIMES:
                    action = () => {
                        this.JumpToHeroLevelUp();
                    };
                    break;
                case GameConst.RECRUIT:
                case GameConst.TASK_RECRUIT_AMOUNT:
                    action = () => {
                        this.JumpToHeroRecuit();
                    };
                    break;
                case GameConst.RESOURCE_PRODUCE:
                    action = () => {
                        this.JumpToTile(tileType: dailyTaskConf.name);
                    };
                    break;
                case GameConst.BATTLE_WIN:
                    int maxLevel = RoleManager.GetFDRecordMaxLevel();
                    action = () => {
                        this.JumpToTile(maxLevel);
                    };
                    break;
                case GameConst.PVP_TIMES:
                    action = () => {
                        this.JumpToOtherPersonTile();
                    };
                    break;
                case GameConst.FORCE:
                    action = () => {
                        this.JumpToForceView();
                    };
                    break;
                case GameConst.GOLD_AMOUNT:
                    action = () => {
                        this.JumpToTile();
                    };
                    break;
                case GameConst.GET_TRIBUTE_TIMES:
                    action = () => {
                        this.JumToTribute();
                    };
                    break;
                case GameConst.ATTACK_NPCCITY:
                    action = () => {
                        this.JumpToNearnestNpcCity();
                    };
                    break;
                case GameConst.ATTACK_PASS:
                    action = () => {
                        this.JumpToNearnestPass();
                    };
                    break;
                case GameConst.RESOURCE_REVOKE:
                    action = () => {
                        this.JumpToOwnTile();
                    };
                    break;
                default:
                    Debug.LogError("Unknown task type " + dailyTaskConf.type);
                    break;
            }

            return action;
        }


        #region daily task jump
        private void JumpToHeroLevelUp() {
            this.parent.JumpToHeroLevelUp();
        }

        private void JumpToHeroRecuit() {
            this.parent.JumpToHeroRecuit();
        }

        private void JumpToBuilding() {
            this.parent.JumpToBuilding();
        }

        private void JumpToLottery() {
            this.parent.JumpToLottery();
        }

        private void JumpToForceView() {
            this.parent.JumpToForceView();
        }

        private void JumpToTile(int tileLevel = 0, string tileType = "") {
            this.parent.JumpToTile(tileLevel, tileType);
        }

        private void JumpToOwnTile() {
            this.parent.JumpToOwnTile();
        }

        private void JumpToOtherPersonTile() {
            this.parent.JumpToOtherPersonTile();
        }

        private void JumToTribute() {
            this.parent.JumToTribute();
        }

        private void JumpToNearnestNpcCity() {
            this.parent.JumpToNearnestNpcCity();
        }

        private void JumpToNearnestPass() {
            this.parent.JumpToNearnestPass();
        }
        #endregion

        public void GetVitalityReward(int vitalityId) {
            this.ChosenVitalityRewardId = vitalityId;
            GetVitalityRewardReq getVitality = new GetVitalityRewardReq() {
                Id = vitalityId
            };
            NetManager.SendMessage(getVitality,
                typeof(GetVitalityRewardAck).Name, this.OnGetVitalityRewardAck);
        }

        public void SetPoint() {
            this.parent.SetDailyTaskPoint();
        }

        public void OnDailyTaskRefresh(int taskId, bool needRefreshView) {
            if (this.model.SetTaskCollected(taskId) && needRefreshView) {
                Debug.LogError("OnReceiveVitality " + this.model.GetCollectableTasksCount());
                this.parent.OnDailyTaskRefresh(this.model.GetCollectableTasksCount());
                this.ReorderTaskList();
                if (this.view.IsVisible) {
                    this.view.SetDailyTask();
                } else {
                    this.NeedRefresh = true;
                }
                this.StartDailyTaskGuid();
            }
        }

        public int GetCollectableTasksCount() {
            return this.model.GetCollectableTasksCount();
        }

        private void ReorderTaskList() {
            int taskCount = this.TaskList.Count;
            this.TaskList.Sort((a, b) => {
                return a.Id.CompareTo(b.Id);
            });
            List<Task> tmpList = new List<Task>(taskCount);
            foreach (Task task in this.TaskList) {
                if (task.IsDone && !task.IsCollect) {
                    tmpList.Add(task);
                }
            }
            foreach (Task task in this.TaskList) {
                if (!task.IsDone) {
                    tmpList.Add(task);
                }
            }
            foreach (Task task in this.TaskList) {
                if (task.IsDone && task.IsCollect) {
                    tmpList.Add(task);
                }
            }
            this.model.taskList = tmpList;
        }

        private void GetDailyTaskReq() {
            if (!this.isRequestingDailyTask) {
                this.isRequestingDailyTask = true;
                GetTasksReq taskReq = new GetTasksReq();
                NetManager.SendMessage(taskReq,
                    typeof(GetTasksAck).Name, this.OnGetDailyTaskAck, (message) => {
                        this.isRequestingDailyTask = false;
                    }, () => {
                        this.isRequestingDailyTask = false;
                    }
                );
            }
        }

        #region net message callback
        private void OnGetDailyTaskAck(IExtensible message) {
            GetTasksAck getTasksAck = message as GetTasksAck;
            bool isUnlock = getTasksAck.Tasks.Count > 0;
            if (isUnlock) {
                this.AllVitality = getTasksAck.AllVitality;
                this.TaskList = getTasksAck.Tasks;
                this.DailyExpireAt = getTasksAck.ExpiredAt;
                this.ReorderTaskList();

                if (this.view.IsVisible) {
                    this.view.SetDailyTask();
                }
                this.parent.OnDailyTaskRefresh(this.model.GetCollectableTasksCount());
                if (this.isNeedOpenView && this.parent.ViewType == TaskType.daily) {
                    this.view.SetDailyTask();
                    this.isNeedOpenView = false;
                }
                if (this.isNeedSetMapTaskDetail && !this.parent.CheckDramNeedShow()) {
                    this.mapShowTaskId = getTasksAck.Tasks[0].Id;
                    this.ShowInMap(getTasksAck.Tasks[0]);
                    this.isNeedSetMapTaskDetail = false;
                }
            }
            this.isRequestingDailyTask = false;
        }

        private void OnGetVitalityRewardAck(IExtensible message) {
            //Debug.LogError("OnGetVitalityRewardAck " + this.ChosenVitalityRewardId);
            //foreach (var value in this.CanGetRewardList) {
            //    Debug.LogError(value);
            //}
            this.CanGetRewardList.TryRemove(this.ChosenVitalityRewardId);
            this.view.OnGetVitalityRewardAck(message);
        }

        private void GetTaskDailyAck(IExtensible message) {
            GetTaskDailyAck getTaskDailyAck = message as GetTaskDailyAck;
            this.AllVitality = 0;//getTaskDailyAck.AllVitality;
            this.CurrentTaskTownhallLevel = getTaskDailyAck.TaskLevel;
            this.CanGetRewardList = getTaskDailyAck.CanGetReward;
            this.isGetTaskDailyRewardInfo = false;
            if (this.parent.ViewType == TaskType.daily && this.IsDailyTaskUnlock()) {
                this.InnerShow();
            }
        }

        private void TaskChangeNtf(IExtensible message) {
            TaskChangeNtf ntf = message as TaskChangeNtf;
            Task task = this.GetTask(ntf.TaskId);
            if (task != null) {
                task.Value = ntf.Value;
                task.IsDone = ntf.IsDone;
                this.ReorderTaskList();
                if (this.view.IsVisible) {
                    this.view.RefresTaskItem(task);
                } else {
                    this.NeedRefresh = true;
                }
                this.parent.OnDailyTaskRefresh(this.model.GetCollectableTasksCount());
                this.ShowInMap(task);
            }
        }

        private void ShowInMap(Task task) {
            if (task.Id == this.mapShowTaskId) {
                DailyTaskConf conf = DailyTaskConf.GetConf(task.Id.ToString());
                this.parent.SetTaskDetail(task.Id,
                    TaskType.daily, conf.GetContent(), task.IsDone, this.GetJumpAction(conf));
            }
        }

        private Task GetTask(int taskId) {
            foreach (Task task in this.TaskList) {
                if (task.Id == taskId) {
                    return task;
                }
            }
            return null;
        }

        private void PlayerBuildingNtf(IExtensible message) {
            PlayerBuildingNtf playerBuildingNtf = message as PlayerBuildingNtf;
            if (playerBuildingNtf.Building.Name.CustomEquals(ElementName.townhall) &&
                this.IsDailyTaskUnlock() && this.model.taskList.Count < 1) {
                this.NeedRefresh = true;
                this.GetDailyTaskReq();
            }
        }

        private int GetTaskListIndexByTaskId(int taskId) {
            int taskCount = this.TaskList.Count;
            for (int i = 0; i < taskCount; i++) {
                if (this.TaskList[i].Id == taskId) {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}
