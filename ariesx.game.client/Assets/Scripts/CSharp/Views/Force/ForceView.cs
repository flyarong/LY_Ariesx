using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class ForceView: BaseView {
        private ForceViewModel viewModel;
        private ForceViewPreference viewPref;
        /*************/
        public Dictionary<Resource, Transform> resourceDict =
            new Dictionary<Resource, Transform>();

        #region PositionList
        private float btnHorizontalNormalized = 0f;
        private List<float> PositionList = new List<float>();
        private List<float> MoveList = new List<float>();
        private float move = 0f;
        private int allForceCount = ForceRewardConf.AllForceConfDict.Count;
        #endregion

        private bool isReceive = false;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ForceViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPlayerInfo.PnlPlayerInfo.PnlForce");
            this.viewPref = this.ui.transform.GetComponent<ForceViewPreference>();
            this.viewPref.btnCloseRight.onClick.AddListener(OnBtnClickRight);
            this.viewPref.btnCloseLeft.onClick.AddListener(OnBtnClickLeft);
            this.viewPref.scrollRect.onEndDrag.AddListener(OnScrollRectEndDrag);
        }

        #region callback
        public void UpdateForceInfo(int level) {
            this.viewModel.CollectedForceList.Add(level);
            if (this.viewModel.CanReceiveList.Contains(level)) {
                this.viewModel.CanReceiveList.Remove(level);
            }
            this.SetForceInfo();
        }
        #endregion

        public void SetForceInfo() {
            int count = this.allForceCount;
            ForceRewardConf currentForceConf = new ForceRewardConf();
            GameHelper.ResizeChildreCount(this.viewPref.pnlForceList,
                count, PrefabPath.pnlForceItem);
            ForceItemView forceItemView = null;
            this.SetStartPointerValue();
            for (int i = 1; i < (count + 1); i++) {
                int index = i;
                currentForceConf = ForceRewardConf.AllForceConfDict[index];
                forceItemView = this.viewPref.pnlForceList.GetChild(index - 1).GetComponent<ForceItemView>();
                bool isAchieve = (this.viewModel.CurrentForce >= currentForceConf.force);
                bool notReceiveRewards = this.viewModel.CollectedForceList.Contains(index);
                bool isReceive = !this.viewModel.CanReceiveList.Contains(index);
                if (!notReceiveRewards && isReceive && isAchieve) {
                    this.viewModel.CanReceiveList.Add(index);
                }
                forceItemView.SetForceStageContent(ForceRewardConf.AllForceConfDict[index],
                    index, isAchieve, notReceiveRewards, () => { UpdateForceInfo(index); },
                    () => { ShowDisplayBoardViewModel(); });
            }
            for (int i = 1; i <= count; i++) {
                bool isStar = this.viewModel.CollectedForceList.Contains(i);
                if (!isStar) {
                    currentForceConf = ForceRewardConf.AllForceConfDict[i];
                    bool isAchieve = (this.viewModel.CurrentForce >= currentForceConf.force);
                    if (isAchieve) {
                        this.viewModel.CloseIconAnimation(!isStar);
                        break;
                    }
                } else {
                    this.viewModel.CloseIconAnimation(!isStar);
                }
            }
            this.SetForceViewState();
        }

        private void SetForceViewState() {
            int canReceibeLevel = 1;
            this.isReceive = this.viewModel.CanReceiveList.Count != 0;
            if (this.isReceive) {
                canReceibeLevel = this.viewModel.CanReceiveList[0];
                ShowCanReceiveForce(canReceibeLevel);
            } else {
                ShowCanReceiveForce(this.viewModel.CurrentLevel);
            }
        }

        private void ShowCanReceiveForce(int level) {
            if (level == 0) {
                level = 1;
            }
            this.viewPref.scrollRect.horizontalNormalizedPosition = MoveList[level - 1];
            //Test To do!!
            if (level != 13 && this.viewPref.scrollRect.horizontalNormalizedPosition == 1) {
                level = 1;
            }
            if (level == 1) {
                this.SetBtnClickLeftActive(false);
                this.SetBtnClickRightActive(true);
            } else if (level == this.allForceCount) {
                this.SetBtnClickRightActive(false);
                this.SetBtnClickLeftActive(true);
            } else {
                this.SetBtnClickActive(true);
            }
        }

        private void ShowDisplayBoardViewModel() {
            this.viewModel.ShowDisplayBoardViewModel(
                LocalManager.GetValue(LocalHashConst.force_grow_info_title),
                LocalManager.GetValue(LocalHashConst.force_grow_info_content));
        }

        #region HorizontalNormalizedPositionLogic

        public void SetBtnClickLeftActive(bool show) {
            this.viewPref.btnCloseLeft.gameObject.SetActiveSafe(show);
        }

        private void SetBtnClickActive(bool show) {
            this.SetBtnClickLeftActive(show);
            this.SetBtnClickRightActive(show);
        }

        public void SetBtnClickRightActive(bool show) {
            this.viewPref.btnCloseRight.gameObject.SetActiveSafe(show);
        }

        private void SetStartPointerValue() {
            int count = this.allForceCount - 1;
            this.btnHorizontalNormalized = ((float)1 / count) / 10;
            for (int i = 0; i <= count; i++) {
                float moveNumber = ((float)i / count);
                float position = (moveNumber + 0.0400f);
                if (i != count) {
                    this.PositionList.Add(position);
                }
                this.MoveList.Add(moveNumber);
            }
            this.move = (float)1 / count;
        }


        private void OnBtnClickLeft() {
            StartCoroutine(BtnClickLeft());
        }

        private void OnBtnClickRight() {
            StartCoroutine(BtnClickRight());
        }

        private void OnScrollRectEndDrag() {
            StartCoroutine(ScrollRectDrag());
        }

        private IEnumerator ScrollRectDrag() {
            yield return YieldManager.GetWaitForSeconds(0.2f);
            StartCoroutine(ScrollRectEndDrag());
        }

        public void SetScrollRectStatePosition() {
            this.viewPref.scrollRect.horizontalNormalizedPosition = 0;
        }

        private IEnumerator ScrollRectEndDrag() {
            this.viewPref.scrollRect.horizontalNormalizedPosition =
            (OnThisPointer() - this.viewPref.scrollRect.horizontalNormalizedPosition)
            * 0.1f + this.viewPref.scrollRect.horizontalNormalizedPosition;
            if (Mathf.Abs(this.viewPref.scrollRect.horizontalNormalizedPosition - OnThisPointer()) < 0.001) {
                this.viewPref.scrollRect.horizontalNormalizedPosition = OnThisPointer();
                yield return YieldManager.GetWaitForSeconds(0.01f);
            } else {
                yield return YieldManager.GetWaitForSeconds(0.01f);
                StartCoroutine(ScrollRectEndDrag());
            }

            if (this.viewPref.scrollRect.horizontalNormalizedPosition >= 0.99) {
                this.SetBtnClickRightActive(false);
            } else {
                this.SetBtnClickRightActive(true);
            }

            if (this.viewPref.scrollRect.horizontalNormalizedPosition <= 0.01) {
                this.SetBtnClickLeftActive(false);
            } else {
                this.SetBtnClickLeftActive(true);
            }
        }

        private float OnThisPointer() {
            float Position = 0;
            for (int i = 0; i < PositionList.Count; i++) {
                if (this.viewPref.scrollRect.horizontalNormalizedPosition <= PositionList[i]) {
                    return Position = MoveList[i];
                } else {
                    Position = 1;
                }
            }
            return Position;
        }

        private IEnumerator BtnClickRight() {
            this.SetBtnClickLeftActive(true);
            for (int i = 0; i < 10; i++) {
                if (this.viewPref.scrollRect.horizontalNormalizedPosition < 1 &&
                    this.viewPref.scrollRect.horizontalNormalizedPosition <
                    (this.viewPref.scrollRect.horizontalNormalizedPosition + this.move)) {
                    this.viewPref.scrollRect.horizontalNormalizedPosition += this.btnHorizontalNormalized;
                    if (this.viewPref.scrollRect.horizontalNormalizedPosition >= 0.99) {
                        this.viewPref.scrollRect.horizontalNormalizedPosition = 1;
                        this.SetBtnClickRightActive(false);
                        break;
                    }
                    yield return YieldManager.GetWaitForSeconds(0.01f);
                }
            }
        }

        private IEnumerator BtnClickLeft() {
            this.SetBtnClickRightActive(true);
            for (int i = 0; i < 10; i++) {
                if (this.viewPref.scrollRect.horizontalNormalizedPosition > 0 &&
                    this.viewPref.scrollRect.horizontalNormalizedPosition >
                    (this.viewPref.scrollRect.horizontalNormalizedPosition - this.move)) {
                    this.viewPref.scrollRect.horizontalNormalizedPosition -= this.btnHorizontalNormalized;
                    if (this.viewPref.scrollRect.horizontalNormalizedPosition <= 0.01) {
                        this.viewPref.scrollRect.horizontalNormalizedPosition = 0;
                        this.SetBtnClickLeftActive(false);
                        break;
                    }
                    yield return YieldManager.GetWaitForSeconds(0.01f);
                }
            }
        }
        #endregion

    }
}
