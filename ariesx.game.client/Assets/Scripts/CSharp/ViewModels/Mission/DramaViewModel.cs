using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class DramaViewModel : BaseViewModel, IViewModel {
        private MissionViewModel parent;
        private DramaModel model;
        //private BuildModel buildModel;
        //private HeroModel heroModel;
        private DramaView view;
        /* Model data get set */
        public List<ChapterTask> DramaList {
            get {
                return this.model.dramaList;
            }
        }
        /**********************/

        /* Other members */
        private bool NeedFresh { get; set; }
        private bool needCheck = true;
        public int MiniIndex { get; set; }

        /***********************************************************************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MissionViewModel>();
            this.model = ModelManager.GetModelData<DramaModel>();
            this.view = this.gameObject.AddComponent<DramaView>();

            NetHandler.AddDataHandler(typeof(ChapterTaskChangeNtf).Name, this.ChapterTaskChangeNtf);
            NetHandler.AddDataHandler(typeof(FteNtf).Name, this.FteFinishedNtf);

            FteManager.SetEndCallback(GameConst.NORMAL, 51, this.OnFteStep51End);
            FteManager.SetStartCallback(GameConst.NORMAL, 55, (index) => this.OnFteStepGo(1));
            FteManager.SetEndCallback(GameConst.RESOURCE_LEVEL, 1, this.OnFteStepEnd);
            FteManager.SetStartCallback(GameConst.NORMAL, 129, (index) => this.OnFteStepGo(2));
            FteManager.SetStartCallback(GameConst.NORMAL, 132, (index) => this.OnFteStepGo(3));
            FteManager.SetEndCallback(GameConst.NORMAL, 55, this.OnFteStepEnd);
            FteManager.SetEndCallback(GameConst.NORMAL, 129, this.OnFteStepEnd);
            FteManager.SetEndCallback(GameConst.NORMAL, 132, this.OnFteStepEnd);

            FteManager.StopCallback = this.OnFteStop;
            this.NeedFresh = true;
            if (this.model.GetChapterUnDoneTaskID() > 3 && this.model.GetChapterUnDoneTaskID() < 10) {
                this.parent.SetDramaArrow();
            }
        }

        void Start() {
            this.InitDrama();
        }

        // To do: feature-optimize.
        public void Show() {
            if (!this.view.IsVisible) {
                if (this.parent.HasDramaArrow()) {
                    this.view.afterShowCallback += () => this.SetDramaArrow();
                }
                if (this.NeedFresh) {
                    this.view.needRefresh = true;
                    this.NeedFresh = false;
                    this.view.SetTarget();
                } else {
                    this.view.Format(this.MiniIndex, this.DramaList.Count);
                }
                //Debug.LogError("show");
                this.view.Show();
                this.parent.AddAfterShowCallback(this.CheckDrama);
                this.parent.AddAfterShowCallback(this.view.ShowHighlight);
            } else {
                this.CheckDrama();
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideParent() {
            this.parent.Hide();
        }

        public void HideImmediatly() {
            this.parent.HideImmediatly();
        }

        public void GetDramaRewards(int taskId) {
            this.TaskRewardReceiveReq(taskId);
        }

        public int GetCollectableTasksCount() {
            return this.model.GetCollectableTasksCount();
        }

        public void SetDramaArrow() {
            foreach (ChapterTask task in this.DramaList) {
                if (!task.IsDone && task.Id <= 9) {
                    this.OnFteStepGo(task.Id);
                    return;
                }
            }
        }

        public bool CheckDramNeedShow() {
            bool chaperDone = (this.model.GetChapterUnDoneTaskID() == 0);
            return chaperDone && this.model.IsChapterRewardReceived();
        }

        /* Add 'NetMessage' function here*/
        private bool isGettingTaskReward = false;
        public void TaskRewardReceiveReq(int id) {
            if (this.isGettingTaskReward) {
                return;
            }

            GetChapterTaskRewardReq req = new GetChapterTaskRewardReq() {
                Id = id
            };
            UnityAction errorCallback = () => {
                if (this.view.IsVisible) {
                    this.view.SetBtnReceive(id, true);
                    UIManager.SetMaskVisible(false);
                }
                this.isGettingTaskReward = false;
            };
            //if (this.view.IsVisible) {
            //    UIManager.SetMaskVisible(true);
            //}
            NetManager.SendMessage(req, typeof(GetChapterTaskRewardAck).Name,
                this.TaskRewardReceiveAck, (message) => errorCallback(), errorCallback);
            this.isGettingTaskReward = true;
        }

        private void TaskRewardReceiveAck(IExtensible message) {
            GetChapterTaskRewardAck ack = message as GetChapterTaskRewardAck;
            if (this.view.IsVisible) {
                UnityAction action = null;
                if (ack.Results.Count > 0) {
                    this.parent.ShowOpenChest(ack.Results, () => {
                        this.view.PlayCollectAnimation(ack.TaskId, ack.Reward, ack.Resources, ack.Currency, action);
                    });
                } else {
                    this.view.PlayCollectAnimation(ack.TaskId, ack.Reward, ack.Resources, ack.Currency, action);
                }
                this.view.RefreshProgress();
            } else {
                this.NeedFresh = true;
                if (ack.Results.Count > 0) {
                    this.parent.ShowOpenChest(ack.Results, () => {
                        this.parent.CollectTaskReward(ack.Resources, ack.Currency, ack.Reward);
                    });
                } else {
                    this.parent.CollectTaskReward(ack.Resources, ack.Currency, ack.Reward);
                }
            }
            this.view.RefreshTask(null, ack.TaskId, true, true);
            ChapterTask task = this.model.GetTaskById(ack.TaskId);
            if (task != null) {
                task.IsCollect = task.IsDone = true;
            }

            AddFteStepsReq req = new AddFteStepsReq() {
                Key = string.Concat("step_chapter_task_done_", ack.TaskId)
            };
            NetManager.SendMessage(req, string.Empty, null, null, null);
            // To do: not good, need split fte and normal logic.
            if (ack.TaskId == FteManager.GetCurChapterTaskId()) {
                FteManager.StartFte(GameConst.NORMAL, 401);
            }
            this.parent.SetDramaPoint();
            this.CheckChapterDone();
            this.isGettingTaskReward = false;
        }

        private void CheckChapterDone() {
            if (this.model.IsChapterRewardReceived()) {
                this.ChapterRewardReceiveReq(
                    DramaConf.GetConf(this.DramaList[0].Id.ToString()).chapter);
            } else {
                this.ShowInMap(true);
            }
        }

        private void ChapterRewardReceiveReq(int id) {
            //Debug.LogError("ChapterRewardReceiveReq " + id);
            GetChapterRewardReq req = new GetChapterRewardReq() {
                Id = id
            };
            NetManager.SendMessage(req, typeof(GetChapterRewardAck).Name, this.ChapterRewardReceiveAck);
        }

        private void ChapterRewardReceiveAck(IExtensible message) {
            GetChapterRewardAck ack = message as GetChapterRewardAck;
            //Debug.LogError("ChapterRewardReceiveAck " + ack.NewTasks.Count + " " + ack.NewTasks[0].Id);
            this.model.Refresh(ack.NewTasks);


            this.parent.ShowTopHUD();
            int step = 0;
            bool hasDrama = ack.NewTasks.Count > 0;
            this.ShowInMap(hasDrama);
            if (!FteManager.SkipFte) {
                FteManager.StartFte(GameConst.NORMAL, step);
            }

            if (this.view.IsVisible) {
                //this.view.needRefresh = true;
                //this.view.SetTarget();
                //this.view.hasShowHighlight = false;
                //this.view.ShowHighlight();
                this.HideParent();
                this.NeedFresh = true;
            } else {
                this.NeedFresh = true;
            }
            this.parent.ShowBanner(ack.Reward,
                    ack.Resources, ack.Currency, this.DramaList.Count != 0);
        }

        private void FteFinishedNtf(IExtensible message) {
            FteManager.EndFte(true, false);
        }

        private void ChapterTaskChangeNtf(IExtensible message) {
            ChapterTaskChangeNtf ntf = message as ChapterTaskChangeNtf;
            ChapterTask task = this.model.SetValue(ntf.TaskId, ntf.Value);
            if (task == null) {
                return;
            }
            if (!this.view.RefreshTask(task, ntf.TaskId, ntf.IsDone, false)) {
                Debug.Log("RefreshTask");
                task.IsDone = ntf.IsDone;
                DramaConf dramaConf = DramaConf.GetConf(ntf.TaskId.ToString());
                Debug.Log(dramaConf.unlockId);
                task.unlocked = FteManager.CheckDrama(dramaConf.unlockId);
            }

            if (task.Id == 3 && !this.model.CheckTaskDone(3) &&
                ntf.IsDone && !FteManager.SkipFte) {
                FteManager.StopFte();
                this.parent.CloseAboveUI();
                FteManager.StartFte(GameConst.NORMAL, 141);
                return;
            }
            if (ntf.IsDone && task.unlocked) {
                this.UnlockChapterTask(task);
                string step = string.Empty;
                string chapterId = string.Concat("chapter_" + ntf.TaskId);
                if (chapterId.CustomEquals(FteManager.Instance.curStep)) {
                    FteManager.EndFte(false, false);
                }
                Debug.Log(ntf.TaskId);
                switch (ntf.TaskId) {
                    case 1:
                        step = "normal_59";
                        break;
                    case 3:
                        step = "normal_141";
                        break;
                    case 32: // Attack marble Level 3.
                        step = "normal_831";
                        break;
                    case 5:
                        step = "normal_415";
                        break;
                    default:
                        string key = string.Concat("chapter_task_", ntf.TaskId);
                        FteStepConf fteStepConf;
                        if (FteStepConf.fteStepDict.TryGetValue(key, out fteStepConf)) {
                            step = fteStepConf.next;
                        }
                        break;
                }
                if (this.view.IsVisible) {
                    this.view.RefreshProgress();
                }
                this.ShowInMap(true);

                if (!FteManager.SkipFte && !step.CustomIsEmpty()) {
                    FteManager.StopFte();
                    this.parent.CloseAboveUI();
                    FteManager.StartFte(step);
                }
            }
        }

        private void UnlockChapterTask(ChapterTask rootTask) {
            List<int> unlock = DramaConf.GetConf(rootTask.Id.ToString()).GetUnlockDrama();
            if (unlock != null) {
                foreach (int id in unlock) {
                    ChapterTask task = this.model.GetTaskById(id);
                    if (task == null) {
                        continue;
                    }
                    if (!this.view.RefreshTask(task, id, task.IsDone, task.IsCollect)) {
                        task.unlocked = rootTask.IsDone && rootTask.unlocked;
                    }
                    if (task.IsDone) {
                        this.UnlockChapterTask(task);
                    }
                }
            }
        }

        /***********************************/
        public void CheckDrama() {
            if (FteManager.SkipFte) {
                return;
            }
            if (!this.needCheck) {
                this.needCheck = true;
                return;
            }
            if (FteManager.Instance.curStep.CustomEquals("normal_171")) {
                return;
            }
            int step = 0;
            int taskDoneId = this.model.GetChapterUnDoneTaskID();
            if (taskDoneId > 3) {
                FteManager.FteOver = true;
                return;
            }
            switch (taskDoneId) {
                case 1:
                    step = 55;
                    break;
                case 2:
                    step = 129;
                    break;
                case 3:
                    step = 132;
                    break;
                default:
                    this.view.SetScroll(true);
                    return;
            }
            Debug.Log("D:" + step);
            FteManager.StartFte(GameConst.NORMAL + "_" + step);
        }

        private void InitDrama() {
            bool hasDrama = this.DramaList.Count > 0;
            FteManager.StopFte();
            int step = 0;
            UnityAction action = () => {
                if (FteManager.SkipFte) {
                    FteManager.HideFteMask();
                    return;
                }
                string stepStr = GameConst.NORMAL + "_" + step;
                FteManager.Instance.curStep = stepStr;
                TriggerManager.Invoke(Trigger.Fte);
                FteManager.DelayStartFte(GameConst.NORMAL + "_" + step);
            };
            Debug.Log(RoleManager.GetFteMaxStep());
            if (RoleManager.GetFteMaxStep() >= 161) {
                step = 0;
            } else if (RoleManager.GetFteMaxStep() == 0) {
                step = 2;
            } else {
                FteStepConf fteStep = FteStepConf.GetConf("normal_" + RoleManager.GetFteMaxStep().ToString());
                int pos = fteStep.next.LastIndexOf('_');
                if (fteStep.next.Substring(0, pos) == "normal") {
                    step = int.Parse(fteStep.next.Substring(pos + 1));
                } else {
                    step = RoleManager.GetFteMaxStep();

                }
            }
            if (step == 112 || step == 121 || step == 124 || step == 128) {
                step = 129;
            }

            // Something wrong.
            Debug.Log(this.model.GetChapterUnDoneTaskID());

            switch (this.model.GetChapterUnDoneTaskID()) {
                case 1:
                    break;
                case 2:
                    if (RoleManager.GetFteMaxStep() <= 61 && RoleManager.GetFteMaxStep() > 55) {
                        string step57 = PlayerPrefs.GetString(RoleManager.GetRoleId() + "step57");
                        if (!step57.CustomIsEmpty()) {
                            int pos = step57.LastIndexOf('_');
                            int x = int.Parse(step57.Substring(0, pos));
                            int y = int.Parse(step57.Substring(pos + 1));
                            FteManager.step57 = new Vector2(x, y);
                            step = 61;
                        } else {
                            step = 91;
                        }
                    } else if (RoleManager.GetFteMaxStep() <= 55) {
                        SetMaxFteStepReq maxStepReq = new SetMaxFteStepReq() {
                            Step = 161
                        };
                        NetManager.SendMessage(maxStepReq, string.Empty, null);
                        GameManager.RestartGame();
                    }
                    break;
                case 3:
                    if (RoleManager.GetFteMaxStep() < 131 &&
                        RoleManager.GetFteMaxStep() > 129) {
                        step = 131;
                    } else if (RoleManager.GetFteMaxStep() <= 129) {
                        SetMaxFteStepReq maxStepReq = new SetMaxFteStepReq() {
                            Step = 161
                        };
                        NetManager.SendMessage(maxStepReq, string.Empty, null);
                        GameManager.RestartGame();
                    }
                    break;
                case 4:
                    if (RoleManager.GetFteMaxStep() < 141 &&
                        RoleManager.GetFteMaxStep() > 132) {
                        step = 141;
                    } else if (RoleManager.GetFteMaxStep() <= 132) {
                        SetMaxFteStepReq maxStepReq = new SetMaxFteStepReq() {
                            Step = 161
                        };
                        NetManager.SendMessage(maxStepReq, string.Empty, null);
                        GameManager.RestartGame();
                    }
                    break;
                default:
                    if (RoleManager.GetFteMaxStep() < 151) {
                        SetMaxFteStepReq maxStepReq = new SetMaxFteStepReq() {
                            Step = 161
                        };
                        NetManager.SendMessage(maxStepReq, string.Empty, null);
                        GameManager.RestartGame();
                    }
                    break;
            }
            
            AudioManager.Init();
            if (step == 1) {
                AudioManager.Play(
                    AudioPath.envPrefix + "wave",
                    AudioType.Enviroment,
                    AudioVolumn.High,
                    true
                );
            } else {
                AudioManager.PlayDefault(1);
            }

            //FteManager.StartFte("chapter_task_5");
            //return;
            // To do: check this.
            if (step != 0) {
                // To do: Not need this.
                this.parent.CloseAboveUI();
                action.Invoke();
            } else {
                FteManager.HideFteMask();
                TriggerManager.Invoke(Trigger.Fte);
            }
            this.ShowInMap(hasDrama);
            if (this.view.IsVisible) {
                this.view.hasShowHighlight = false;
                this.view.ShowHighlight();
            }
            this.CheckChapterDone();
        }

        private bool isNeedDailyTaskGuid = false;
        public void StartDrama(int id) {
            UnityAction action = () => {
                FteManager.StartFte("chapter_task_" + id);
            };
            if (this.view.IsVisible) {
                this.view.afterHideCallback = action;
                this.parent.Hide();
            } else {
                action.Invoke();
            }
        }

        public void StartChapterDailyGuid() {
            //Debug.LogError("StartChapterDailyGuid " + this.parent.IsDailyTaskUnlock());
            if (this.parent.IsDailyTaskUnlock()) {
                this.isNeedDailyTaskGuid = true;
                StartCoroutine(this.YieldStartDailyTaskGuid());
            }
        }

        private IEnumerator YieldStartDailyTaskGuid() {
            yield return YieldManager.GetWaitForSeconds(GameConst.TASK_REFRESH_PERIOD);
            if (this.isNeedDailyTaskGuid) {
                this.parent.StartDailyTaskGuid();
            }
        }

        public ChapterTask GetTaskById(int id) {
            return this.model.GetTaskById(id);
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            this.InitDrama();
            this.NeedFresh = true;
            this.parent.Hide();
        }

        public void AddAfterShowCallback(UnityAction callback) {
            this.parent.AddAfterShowCallback(callback);
        }

        public void ShowInMap(bool hasDrama) {
            int taskId = -1;
            string taskDetail = this.model.GetDramaDetailInMap(out taskId);
            if (hasDrama) {
                //taskId = this.model.GetReceiveableChapterTaskId();
                this.isNeedDailyTaskGuid = false;
                this.parent.OnDramaTaskRefresh(this.GetCollectableTasksCount());
            } else {
                taskDetail = string.Empty;
            }
            this.parent.SetTaskDetail(taskId, taskDetail, this.model.HasUnreceivedTask(), null);
        }

        #region FTE
        private void OnFteStop() {
            this.parent.SetDramaArrow();
        }

        public bool HasArrow() {
            return this.view.HasArrow();
        }

        private void OnFteStepGo(int step) {
            UnityAction action = () => {
                this.view.OnFteStepGo(step);
                this.parent.SetAfterHideCallBack(() => {
                    //FteManager.StopFte();
                    this.view.SetScroll(true);
                });

            };
            this.view.SetScroll(false);
            if (this.view.IsVisible) {
                action.Invoke();
            } else {
                this.needCheck = false;
                this.parent.AddAfterShowCallback(action);
                this.parent.Show();
            }
        }

        private void OnFteStepReceive(int step) {
            UnityAction action = () => {
                FteManager.SetChat(0, needBackground: false);
                this.view.OnFteStepReceive(step);
                this.view.afterHideCallback = () => {
                    FteManager.StopFte();
                    this.view.SetScroll(true);
                };
            };
            this.view.SetScroll(false);
            if (this.view.IsVisible) {
                action.Invoke();
            } else {
                this.needCheck = false;
                this.parent.AddAfterShowCallback(action);
                this.parent.Show();
            }
        }

        private void OnFteStepReceiveEnd() {
            this.TaskRewardReceiveReq(1);
            StartCoroutine(this.OnFteStepReceiveDelayEnd());
        }

        private IEnumerator OnFteStepReceiveDelayEnd() {
            yield return new WaitForSeconds(1.5f);
            this.view.SetScroll(true);
            this.view.afterHideCallback = FteManager.StartFte;
            this.parent.Hide();

        }

        private void OnFteStepEnd() {
            Debug.Log("OnFteStepEnd");
            this.view.SetScroll(true);
            this.view.afterHideCallback = FteManager.StartFte;
            this.parent.Hide();
        }

        private void OnFteStep51End() {
            this.ShowInMap(true);
        }
        #endregion

    }
}
