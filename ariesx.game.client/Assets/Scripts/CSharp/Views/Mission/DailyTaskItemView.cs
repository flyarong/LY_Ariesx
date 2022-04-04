using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class DailyTaskItemView : BaseItemViewsHolder {
        #region ui element
        [SerializeField]
        private Image imgItemBG;
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private Transform pnlVitality;
        [SerializeField]
        private TextMeshProUGUI txtPoint;
        [SerializeField]
        private TextMeshProUGUI txtDesc;
        [SerializeField]
        private TextMeshProUGUI txtProgress;
        [SerializeField]
        private CustomButton btnGo;
        [SerializeField]
        private Image imgBtnBG;
        [SerializeField]
        private TextMeshProUGUI txtBtnlabel;
        [SerializeField]
        private GameObject receivedMark;
        [SerializeField]
        private GameObject taskStatus;
        #endregion

        [HideInInspector]
        public Transform pnlDestination;
        [HideInInspector]
        public Transform pnlDeparture;
        private Task task;
        public Task Task {
            get {
                return this.task;
            }
            set {
                this.task = value;
                this.OnTaskChange();
            }
        }
        private DailyTaskConf taskConf;
        private UnityEvent onBtnGo = new UnityEvent();
        public UnityEvent OnGoClick {
            get {
                this.onBtnGo.RemoveAllListeners();
                return this.onBtnGo;
            }
        }

        public UnityAction<int, UnityAction<IExtensible>> OnGetTaskRewards = null;
        public UnityAction<int, int> OnReceiveVitality = null;

        private void UpdateProgressText(float value) {
            StringBuilder s = new StringBuilder();
            s.AppendFormat(
                "{0}{1}/{2}",
                LocalManager.GetValue(LocalHashConst.task_progress_short),
                GameHelper.GetFormatNum((long)value),
                GameHelper.GetFormatNum(this.taskConf.GetTarget())
            );
            this.txtProgress.text = s.ToString();
        }

        public void UpdateProgress(int newValue, bool isDone) {
            this.task.IsDone = isDone;
            this.taskStatus.SetActiveSafe(isDone);
            string imgItemBGPath = (this.task.IsDone && !this.task.IsCollect) ?
                "daily_task_done" : "daily_task_undone";
            this.imgItemBG.sprite = ArtPrefabConf.GetSprite(imgItemBGPath);
            this.SetBtnGoDetail();
        }

        private void OnTaskChange() {
            this.taskConf = DailyTaskConf.GetConf(this.task.Id.ToString());
            //Debug.LogError(this.taskConf.type + " " + this.task.Id);
            this.txtPoint.text = this.taskConf.vitality.ToString();
            this.UpdateProgress(task.Value, task.IsDone);
            this.UpdateProgressText(task.Value);
            this.txtDesc.text = this.taskConf.GetContent();
            this.SetBtnGoDetail();
        }

        private void SetBtnGoDetail() {
            this.btnGo.onClick.RemoveAllListeners();
            this.btnGo.Grayable = false;
            this.btnGo.gameObject.SetActiveSafe(!this.task.IsCollect);
            this.receivedMark.SetActiveSafe(this.task.IsCollect);
            if (this.task.IsDone && !this.task.IsCollect) {
                this.btnGo.onClick.AddListener(this.OnBtnRewardClick);
            } else if (!this.task.IsDone) {
                this.btnGo.onClick.AddListener(this.OnBtnGoClick);
            }
            if (this.btnGo.gameObject.activeSelf) {
                string btnBGPath = this.task.IsDone ?
                    "daily_task_done_btn" : "daily_task_undone_btn";
                this.imgBtnBG.sprite = ArtPrefabConf.GetSprite(btnBGPath);
                this.txtBtnlabel.text = (this.task.IsDone && !this.task.IsCollect) ?
                    LocalManager.GetValue(LocalHashConst.button_receive) : LocalManager.GetValue(LocalHashConst.button_go);
            }
        }

        private void GetTaskRewards() {
            this.OnGetTaskRewards.Invoke(this.taskConf.id, this.OnGetTaskRewardAck);
        }

        private void OnGetTaskRewardAck(IExtensible message) {
            this.task.IsCollect = true;
            this.SetBtnGoDetail();
            this.PlayVitalityCollectAnim();
        }

        private void PlayVitalityCollectAnim() {
            List<int> randomList = new List<int>();
            for (int i = 0; i < this.taskConf.vitality; i++) {
                randomList.Add(i + 1);
            }
            this.pnlDeparture.position = this.pnlVitality.position;
            GameObject controller =
                PoolManager.GetObject(PrefabPath.collectController, AnimationManager.Instance.transform);
            controller.transform.position = this.pnlDeparture.position;
            AnimationManager.AnimateEvent(controller, this.taskConf.vitality, "Generate",
                () => StartCoroutine(this.PlaySignalVitalityCollectAnim(randomList)),
                () => {
                    PoolManager.RemoveObject(controller);
                    this.btnGo.Grayable = false;
                    if (this.OnReceiveVitality != null) {
                        this.OnReceiveVitality.Invoke(this.taskConf.vitality, this.taskConf.id);
                    }
                });
        }

        private IEnumerator PlaySignalVitalityCollectAnim(List<int> randomList) {
            yield return YieldManager.EndOfFrame;
            if (randomList.Count > 0) {
                int index = UnityEngine.Random.Range(0, randomList.Count);
                int key = randomList[index];
                randomList.Remove(key);
                GameObject collectObj =
                    PoolManager.GetObject(PrefabPath.pnlCollectVitality, this.pnlDeparture);
                AnimationManager.Animate(collectObj, "Move" + (key % 5 + 1),
                    start: this.pnlDeparture.position,
                    target: this.pnlDestination.position,
                    finishCallback: () => {
                        AnimationManager.Animate(this.pnlDestination.gameObject, "Beat");
                        PoolManager.RemoveObject(collectObj);
                    }, space: PositionSpace.World
                );
            }
        }

        private void OnBtnRewardClick() {
            if (!this.btnGo.Grayable) {
                this.btnGo.Grayable = true;
                this.GetTaskRewards();
            }
        }

        private void OnBtnGoClick() {
            this.onBtnGo.InvokeSafe();
        }
    }
}
