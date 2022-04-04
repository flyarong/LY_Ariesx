using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class DramaView : BaseView {
        private DramaViewModel viewModel;
        private DramaViewPreference viewPref;

        // Format
        public bool needRefresh;
        /*************/

        private Dictionary<int, DramaItemView> dramaItemViewDict =
                                new Dictionary<int, DramaItemView>();
        private Dictionary<Resource, Transform> resourceDict =
                                new Dictionary<Resource, Transform>();
        public bool hasShowHighlight = false;
        private int currentShowDetailItem = -1;
        private int arrowStep = 0;


        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMission.PnlMission.PnlDrama");
            this.viewModel = this.gameObject.GetComponent<DramaViewModel>();
            this.viewPref = this.ui.transform.GetComponent<DramaViewPreference>();
            this.viewPref.ccDrama.onClick.AddListener(this.HideRewardDetail);
            this.viewPref.targetScroll.onValueChanged.AddListener((value) => this.HideRewardDetail());
        }

        public override void Hide(UnityAction callback = null) {
            this.HideRewardDetail();
            base.Hide();
        }

        public override void HideImmediatly(UnityAction callback = null) {
            this.HideRewardDetail();
            base.HideImmediatly(callback);
        }

        public void SetTarget() {
            this.InitUI();
            bool showPreview = this.viewModel.DramaList.Count == 0;
            this.viewPref.pnlDescription.gameObject.SetActiveSafe(!showPreview);
            this.viewPref.pnlTarget.gameObject.SetActiveSafe(!showPreview);
            this.viewPref.pnlReward.gameObject.SetActiveSafe(!showPreview);
            this.viewPref.pnlPreview.gameObject.SetActiveSafe(showPreview);
            if (showPreview) {
                this.viewPref.txtTitle.text = string.Empty;
                return;
            }

            int dramaListCount = this.viewModel.DramaList.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlTargetList,
                dramaListCount, PrefabPath.pnlDramaItem);
            this.dramaItemViewDict.Clear();
            DramaConf dramaConf = DramaConf.GetConf(this.viewModel.DramaList[0].Id.ToString());
            this.viewPref.txtTitle.text = LocalManager.GetValue("chapter_title_", dramaConf.chapter.ToString());
            this.viewPref.txtDescName.text = dramaConf.GetTitle();
            this.viewPref.txtDescContent.text = dramaConf.GetDescription();
            int count = 0;
            int collectCount = 0;
            this.viewModel.MiniIndex = 0;
            int index = 1;
            Transform targetObj = null;
            for (int i = 0; i < dramaListCount; i++) {
                targetObj = this.viewPref.pnlTargetList.GetChild(i);
                DramaItemView dramaItemView = targetObj.GetComponent<DramaItemView>();
                ChapterTask task = this.viewModel.DramaList[i];
                dramaItemView.Task = task;
                dramaItemView.OnGoClick.AddListener(() => this.OnBtnGoClick(task.Id));
                dramaItemView.OnReceiveClick.AddListener(() => this.OnBtnReceiveClick(task.Id));
                dramaItemView.ccRewardDetail.onClick.RemoveAllListeners();
                dramaItemView.ccRewardDetail.onClick.AddListener(() => this.OnRewardDetailClick(task.Id));
                if (this.viewModel.MiniIndex == 0 && (!task.IsDone || !task.IsCollect)) {
                    this.viewModel.MiniIndex = index;
                }
                if (task.IsDone && task.unlocked) {
                    count++;
                }
                if (task.IsCollect) {
                    collectCount++;
                }
                this.dramaItemViewDict.Add(task.Id, dramaItemView);
                index++;
            }
            this.SetChapterRewardEnable(collectCount == this.viewModel.DramaList.Count);

            this.viewPref.txtProgress.text = LocalManager.GetValue(LocalHashConst.chapter_progress) + "   " +
                count + "/" + this.viewModel.DramaList.Count;
            ChapterConf chapterConf = ChapterConf.GetConf(dramaConf.chapter.ToString());
            int resourceCount = chapterConf.resourcesDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlResources,
                resourceCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView resourceItemView = null;
            this.resourceDict.Clear();
            index = 0;
            foreach (var pair in chapterConf.resourcesDict) {
                resourceItemView = this.viewPref.pnlResources.GetChild(index++).GetComponent<ItemWithCountView>();
                resourceItemView.SetResourceInfo(pair.Key, pair.Value);
                this.resourceDict.Add(pair.Key, resourceItemView.imgItem.transform);
            }

            this.Format(this.viewModel.MiniIndex, this.viewModel.DramaList.Count);
        }

        public void SetChapterRewardEnable(bool enable) {
            this.viewPref.imgRewardLeft.material =
            this.viewPref.imgRewardRight.material =
            enable ? null : PoolManager.GetMaterial(MaterialPath.matGray);
        }

        public void SetBtnReceive(int id, bool interactable) {
            this.dramaItemViewDict[id].SetBtnReceive(interactable);
        }

        public void ShowHighlight() {
            if (this.IsVisible && !this.hasShowHighlight) {
                this.hasShowHighlight = true;
                foreach (DramaItemView itemView in this.dramaItemViewDict.Values) {
                    itemView.ShowHighlight();
                }
            }
        }

        public void RefreshProgress() {
            int count = 0;
            int collectCount = 0;
            int index = 1;
            this.viewModel.MiniIndex = 0;
            foreach (ChapterTask task in this.viewModel.DramaList) {
                if (this.viewModel.MiniIndex == 0 && (!task.IsDone || !task.IsCollect)) {
                    this.viewModel.MiniIndex = index;
                }
                if (task.IsDone && task.IsDone) {
                    count++;
                }
                if (task.IsCollect) {
                    collectCount++;
                }
                index++;
            }
            if (this.IsUIInit) {
                this.viewPref.txtProgress.text = count + "/" + this.viewModel.DramaList.Count;
                if (collectCount == this.viewModel.DramaList.Count) {
                    this.SetChapterRewardEnable(true);
                }
            }
        }

        public void PlayCollectAnimation(int taskId, CommonReward commonReward,
            Protocol.Resources resources, Protocol.Currency currency, UnityAction action) {
            DramaItemView itemView;
            if (this.dramaItemViewDict.TryGetValue(taskId, out itemView)) {
                itemView.PlayCollectAnimation(commonReward, resources, currency, action);
            }
        }

        public bool RefreshTask(ChapterTask task, int taskId, bool isDone, bool isCollect) {
            DramaItemView itemView;
            if (this.dramaItemViewDict.TryGetValue(taskId, out itemView)) {
                if (task != null && itemView.Task != task) {
                    itemView.Task = task;
                }
                itemView.SetStatus(isDone, isCollect, this.IsVisible);
                return true;
            }
            Debug.LogWarningf("No such task id {0}", taskId);
            return false;
        }

        public void CollectResources(Protocol.Resources addResources, Protocol.Currency addCurrency,
            Protocol.Resources resources, Protocol.Currency currency) {
            GameHelper.CollectResources(addResources, addCurrency, resources, currency, this.resourceDict);
        }

        public void SetScroll(bool enable) {
            this.InitUI();
            this.viewPref.targetScroll.vertical = enable;
        }

        public void Format(int index, int count) {
            float position = Mathf.Min(1, Mathf.Max(0, 1 -
                ((index - 2 * Mathf.Sign(count / 2 - index)) / (float)count)));
            this.OnContentSizeFitter(position);
        }

        private void OnContentSizeFitter(float position) {
            this.viewPref.targetScroll.verticalNormalizedPosition = position;
        }

        private void OnBtnGoClick(int id) {
            this.viewModel.StartDrama(id);
        }

        private void OnBtnReceiveClick(int id) {
            this.HideRewardDetail();
            this.viewModel.TaskRewardReceiveReq(id);
        }

        private readonly Vector2 rewardsStarPos = new Vector2(140, 20);
        private void OnRewardDetailClick(int id) {
            if (this.viewPref.pnlRewardDetail.gameObject.activeSelf && currentShowDetailItem == id) {
                AnimationManager.Animate(this.viewPref.pnlRewardDetail.gameObject, "Hide",
                    finishCallback: this.HideRewardDetail);
                return;
            }
            DramaItemView itemView = this.dramaItemViewDict[id];
            if ((!itemView.Task.IsDone) || (itemView.Task.IsCollect && itemView.Task.IsDone) ||
                !itemView.Task.unlocked) {
                this.viewPref.pnlRewardDetail.gameObject.SetActive(true);
                this.viewPref.pnlRewardDetail.position = itemView.transform.position;
                this.currentShowDetailItem = id;
                DramaConf conf = this.dramaItemViewDict[id].taskConf;
                this.viewPref.glgRewardDetail.constraintCount = Mathf.Min(6, conf.resourcesDict.Count);
                Transform pnlContent = this.viewPref.glgRewardDetail.transform;
                int resourceCount = conf.resourcesDict.Count;
                int index = 0;
                ItemWithCountView resourceItemView;
                GameHelper.ResizeChildreCount(pnlContent, resourceCount, PrefabPath.pnlItemWithCount);
                foreach (var pair in conf.resourcesDict) {
                    resourceItemView = pnlContent.GetChild(index++).GetComponent<ItemWithCountView>();
                    resourceItemView.SetResourceInfo(pair.Key, pair.Value);
                }

                AnimationManager.Animate(this.viewPref.pnlRewardDetail.gameObject, "Show",
                    this.rewardsStarPos, this.rewardsStarPos, isOffset: true);
            }
        }

        private void HideRewardDetail() {
            this.InitUI();
            this.viewPref.pnlRewardDetail.gameObject.SetActiveSafe(false);
        }

        public bool HasArrow() {
            Transform arrow = this.viewPref.pnlDrama.Find("PnlFteArrow");
            return (this.arrowStep > 3) && (arrow != null);
        }

        #region FTE
        public void OnFteStepGo(int step) {
            DramaItemView itemView;
            bool isHighlight = step < 4;
            this.arrowStep = step;
            if (this.dramaItemViewDict.TryGetValue(step, out itemView)) {
                itemView.SetBtnGo(this.viewPref.pnlDrama, isHighlight);
            } else {
                FteManager.StopFte();
            }
        }

        public void OnFteStepReceive(int step) {
            DramaItemView itemView;
            if (this.dramaItemViewDict.TryGetValue(step, out itemView)) {
                itemView.SetBtnReceive(this.viewPref.pnlDrama);
            } else {
                FteManager.StopFte();
            }
        }
        #endregion

        protected override void OnVisible() {
            this.ShowHighlight();
        }

        protected override void OnInvisible() {
            foreach (DramaItemView itemView in this.dramaItemViewDict.Values) {
                itemView.OnDisable();
            }
            this.currentShowDetailItem = -1;
            this.viewPref.pnlRewardDetail.gameObject.SetActive(true);
            this.hasShowHighlight = false;
        }
    }
}
