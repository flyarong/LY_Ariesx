using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class FallenInfoView: BaseView {
        private FallenInfoViewModel viewModel;
        private FallenInfoViewPreference viewPref;

        //private float payProgressWidth;
        private Dictionary<Resource, FallenResourceItemView> resoursViews =
            new Dictionary<Resource, FallenResourceItemView>();
        private readonly List<Resource> resources = new List<Resource> {
            Resource.Lumber,
            Resource.Marble,
            Resource.Steel,
            Resource.Food
        };
        private Transform currentPnl = null;
        private bool isPlayingAnimation = false;

        private string masterAllianceName = string.Empty;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<FallenInfoViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIFallenInfo");
            this.viewPref = this.ui.transform.GetComponent<FallenInfoViewPreference>();

            /* Cache the ui components here */
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnPayResource.onClick.AddListener(this.OnPayResourceClick);
            this.viewPref.btnReturn.onClick.AddListener(this.OnBtnReturnClick);
            this.viewPref.btnPayClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.sliProgress.onValueChanged.AddListener(this.OnSliderValueChange);
            this.viewPref.btnPay.onClick.AddListener(this.OnBtnPayClick);
            GameHelper.ClearChildren(this.viewPref.pnlResources);
            foreach (Resource resource in this.resources) {
                GameObject resourceItem =
                    PoolManager.GetObject(PrefabPath.pnlFallenResource, this.viewPref.pnlResources);
                FallenResourceItemView resourceItemView =
                    resourceItem.GetComponent<FallenResourceItemView>();
                resourceItemView.OnResourceChange.AddListener(this.OnReourceValueChange);
                resourceItemView.Resource = resource;
                this.resoursViews.Add(resource, resourceItemView);
            }
            this.currentPnl = this.viewPref.pnlFallen;
        }

        private void SetFallenInfoTitleInfo() {
            this.masterAllianceName = RoleManager.GetMasterAllianceName();
            string fallenInfo = String.Format(LocalManager.GetValue(LocalHashConst.fallen_title),
                                    this.masterAllianceName);
            this.viewPref.txtHeadTitle.text = fallenInfo;
            this.viewPref.txtTopTitle.text = fallenInfo;

            string colorMaster = string.Format(" <color=#63645CFF>{0}</color> ",
                            RoleManager.GetMasterAllianceName());
            this.viewPref.txtTipText.text =
                String.Format(LocalManager.GetValue(LocalHashConst.fallen_pay_desc), colorMaster);

        }

        public void SetResourcesMaxValue(Dictionary<Resource, float> resourcesInfo) {
            this.SetFallenInfoTitleInfo();
            FallenResourceItemView resoursItemView;

            foreach (Resource resource in resourcesInfo.Keys) {
                Debug.LogError("玩家资源:" + resource + "= " + RoleManager.GetResource(resource));
                resoursItemView = this.resoursViews[resource];
                //resoursItemView.SliderValue = 0;
                resoursItemView.MaxValue = resourcesInfo[resource];
            }
            //this.currentPnl = this.viewPref.pnlFallen;
            UIManager.ShowUI(this.currentPnl.gameObject);
        }


        public override void PlayShow() {
            base.PlayShow(null);
        }

        public override void PlayHide(UnityAction callback) {
            this.isPlayingAnimation = true;
            base.PlayHide(callback);
        }

        public void HideCurrentPanel() {
            UIManager.HideUI(this.currentPnl.gameObject);
            this.Hide();
            this.isPlayingAnimation = false;
        }

        public void ResetResourceItemView() {
            foreach (FallenResourceItemView resoursItemView in resoursViews.Values) {
                resoursItemView.SliderValue = 0;
            }
        }

        #region properties chage
        private void OnReourceValueChange(float resourceValue, Resource changeResource) {
            this.viewModel.CurrentPaid = 0;
            float otherResource = this.GetOtherResourceTotal(changeResource);
            float maxNeed = this.viewModel.AllNeeded - otherResource - this.viewModel.AlreadyPaid;

            if (resourceValue > maxNeed) {
                this.resoursViews[changeResource].SliderValue = maxNeed;
                this.viewModel.CurrentPaid = this.viewModel.AllNeeded - this.viewModel.AlreadyPaid;
            } else {
                this.viewModel.CurrentPaid = otherResource + resourceValue;
            }
        }

        private float GetOtherResourceTotal(Resource excludeResource) {
            float total = 0;
            foreach (Resource resource in this.resoursViews.Keys) {
                if (resource != excludeResource) {
                    total += this.resoursViews[resource].ResourceValue;
                }
            }
            return total;
        }

        public void OnAllNeedChange() {
            this.viewPref.sliProgress.maxValue = this.viewModel.AllNeeded;
            this.viewPref.paidProgress.maxValue = this.viewModel.AllNeeded;
        }

        public void OnAlreadyPaidChange() {
            this.viewPref.paidProgress.value = this.viewModel.AlreadyPaid;
            this.SetProgressInfo();
        }

        public void OnCurrentPaidChange() {
            this.viewPref.sliProgress.value = this.viewModel.CurrentPaid + this.viewModel.AlreadyPaid;
        }
        #endregion

        #region callbacks
        protected void OnBtnCloseClick() {
            if (!this.isPlayingAnimation) {
                this.viewModel.Hide();
            }
        }

        private void OnPayResourceClick() {
            this.isPlayingAnimation = true;
            AnimationManager.Animate(this.viewPref.pnlFallen.gameObject, "MoveToLeft",
                () => UIManager.HideUI(this.viewPref.pnlFallen.gameObject),
                 isOffset: false);

            UIManager.ShowUI(this.viewPref.pnlPayResource.gameObject);
            this.currentPnl = this.viewPref.pnlPayResource;
            AnimationManager.Animate(this.viewPref.pnlPayResource.gameObject, "MoveToLeft",
                () => {
                    this.isPlayingAnimation = false;
                }, isOffset: false);
        }

        private void OnBtnReturnClick() {
            this.isPlayingAnimation = true;
            UIManager.ShowUI(this.viewPref.pnlFallen.gameObject);
            this.currentPnl = this.viewPref.pnlFallen;
            AnimationManager.Animate(this.viewPref.pnlFallen.gameObject, "MoveToRight", null,
                 isOffset: false);

            AnimationManager.Animate(this.viewPref.pnlPayResource.gameObject, "MoveToRight",
                () => {
                    UIManager.HideUI(this.viewPref.pnlPayResource.gameObject);
                    this.isPlayingAnimation = false;
                }, isOffset: false);
        }

        private void OnSliderValueChange(float value) {
            this.SetProgressInfo();
        }

        private void OnBtnPayClick() {
            Dictionary<Resource, int> resourceInfo = new Dictionary<Resource, int>();
            foreach (Resource resource in this.resoursViews.Keys) {
                resourceInfo.Add(resource, (int)this.resoursViews[resource].ResourceValue);
            }

            this.viewModel.PayFallenResources(resourceInfo);
        }
        #endregion


        private void SetProgressInfo() {
            this.viewPref.btnPay.interactable = (this.viewModel.CurrentPaid > 0);
            float totalPaid = Mathf.Max(0, this.viewModel.CurrentPaid + this.viewModel.AlreadyPaid);
            this.viewPref.txtProgress.text = string.Concat(
                GameHelper.GetFormatNum((long)totalPaid), "/",
                GameHelper.GetFormatNum((long)this.viewModel.AllNeeded));
        }
    }
}
