using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Protocol;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace Poukoute {
    public class DramaItemView : MonoBehaviour {
        #region ui element
        [SerializeField]
        private GameObject taskStatus;
        [SerializeField]
        private Transform pnlHighlight;
        [SerializeField]
        private CanvasGroup highlightCG;
        [SerializeField]
        private Image imgHighlightBG;
        [SerializeField]
        private Transform pnlResources;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private TextMeshProUGUI txtProgress;
        [SerializeField]
        private CustomButton btnReceive;
        [SerializeField]
        private CustomButton btnGo;
        [SerializeField]
        private GameObject receivedMark;
        [SerializeField]
        private GameObject txtComplete;

        public CustomClick ccRewardDetail;
        #endregion

        private GameObject tileObj;
        private Dictionary<Resource, Transform> resourceTransDict =
            new Dictionary<Resource, Transform>(7);
        public DramaConf taskConf;
        private ChapterTask task;
        public ChapterTask Task {
            get {
                return this.task;
            }
            set {
                this.task = value;
                taskConf = DramaConf.GetConf(this.task.Id.ToString());
                this.SetStatus(this.task.IsDone, this.task.IsCollect, false);
            }
        }

        private UnityEvent onBtnGoClick = new UnityEvent();
        public UnityEvent OnGoClick {
            get {
                this.onBtnGoClick.RemoveAllListeners();
                return this.onBtnGoClick;
            }
        }

        private UnityEvent onBtnReceiveClick = new UnityEvent();
        public UnityEvent OnReceiveClick {
            get {
                this.onBtnReceiveClick.RemoveAllListeners();
                return this.onBtnReceiveClick;
            }
        }

        private bool showReward;
        public bool ShowReward {
            get {
                return this.showReward;
            }
        }

        private void Start() {
            this.btnReceive.onClick.AddListener(this.OnBtnReceiveClick);
            this.btnGo.onClick.AddListener(this.OnBtnGoClick);
        }

        public void SetStatus(bool isDone, bool isCollect, bool needAnimation) {
            this.task.unlocked = FteManager.CheckDrama(this.taskConf.unlockId);
            bool isShowHighlight = (this.task.IsCollect != isCollect);
            this.EnableHighlight(isShowHighlight);

            if (isShowHighlight) {
                AnimationManager.Animate(this.pnlHighlight.gameObject, "Hide", isOffset: false, finishCallback: () => {
                    this.EnableHighlight(false);
                    this.imgHighlightBG.material = null;
                });
            } else {
                this.SetBtnReceive(false);
            }
            this.txtName.text = this.taskConf.GetName();
            this.txtProgress.text = string.Concat(
                LocalManager.GetValue(LocalHashConst.task_progress_short),
                "    ", this.task.unlocked ? this.task.Value :
                this.taskConf.GetDramaUnlockValue(), "/", this.taskConf.GetTarget());
            this.task.IsDone = isDone;
            this.task.IsCollect = isCollect;
            this.showReward = isDone && !isCollect && this.task.unlocked;
            if (this.showReward) {
                GameHelper.SetResourcesListInfo(
                    this.taskConf.resourcesDict,
                    this.resourceTransDict,
                    this.pnlResources,
                    PrefabPath.pnlItemWithCountSmall);
            }
            if (this.task.IsDone) {
                this.task.Value = this.taskConf.GetTarget();
            }
            bool go = !this.task.IsDone || !this.task.unlocked;
            this.btnGo.gameObject.SetActiveSafe(go);
            this.btnGo.interactable = this.task.unlocked;
            this.taskStatus.SetActiveSafe(task.IsDone && this.task.unlocked);
            this.receivedMark.SetActiveSafe(this.task.IsCollect);
            if (needAnimation && this.showReward) {
                this.ShowHighlight();
            }
        }

        public void EnableHighlight(bool enable) {
            UIManager.SetUICanvasGroupEnable(this.highlightCG, enable);
            this.txtComplete.gameObject.SetActive(enable);
        }

        public void ShowHighlight() {
            if (this.showReward) {
                this.EnableHighlight(true);
                AnimationManager.Animate(this.pnlHighlight.gameObject, "Show",
                    isOffset: false, finishCallback: () => {
                        this.SetBtnReceive(true);
                        this.imgHighlightBG.material = PoolManager.GetMaterial(MaterialPath.matScan);
                    });
            }
        }

        public void SetBtnReceive(bool interactable) {
            this.btnReceive.enabled = interactable;
        }

        public void PlayCollectAnimation(CommonReward commonReward,
            Protocol.Resources resources, Protocol.Currency currency,
            UnityAction action) {
            GameHelper.CollectResources(commonReward,
                resources, currency, resourceTransDict);
            StartCoroutine(this.DelayShowChest(action));
        }

        private IEnumerator DelayShowChest(UnityAction action) {
            yield return new WaitForSeconds(1.5f);
            action.InvokeSafe();
        }

        public void OnBtnReceiveClick() {
            this.SetBtnReceive(false);
            this.onBtnReceiveClick.InvokeSafe();
        }

        private void OnBtnGoClick() {
            this.onBtnGoClick.InvokeSafe();
        }

        #region FTE
        public void SetBtnGo(Transform arrowParent, bool isHighlight = false) {
            StartCoroutine(this.InnerSetBtnGo(arrowParent, isHighlight));
        }

        private IEnumerator InnerSetBtnGo(Transform arrowParent, bool isHighlight = false) {
            yield return YieldManager.EndOfFrame;
            if (isHighlight) {
                FteManager.SetMask(this.transform, autoNext: false, isHighlight: true, isEnforce: true);
                FteManager.SetArrow(this.btnGo.pnlContent, isEnforce: true);
            } else {
                Debug.Log("isHighlight");
                FteManager.SetArrow(btnGo.pnlContent, isEnforce: false, arrowParent: arrowParent);
            }
        }

        public void SetBtnReceive(Transform arrowParent) {
            FteManager.SetMask(btnReceive.pnlContent, autoNext: false,
                isButton: true, isEnforce: true, arrowParent: arrowParent);
        }

        #endregion

        public void OnDisable() {
            if (this.task.IsDone && !this.task.IsCollect) {
                this.EnableHighlight(false);
            }
        }
    }
}
