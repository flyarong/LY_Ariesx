using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MailSystemView: SRIA<MailSystemViewModel, MailSystemItemView> {
        private MailSystemViewPreference viewPref;
        private bool isShowSystemMailDetail = false;
        private bool hasAttachment = false;
        /*************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIMail.PnlMail.PnlContent.PnlSystem");
            this.viewModel = this.gameObject.GetComponent<MailSystemViewModel>();
            this.viewPref = this.ui.transform.GetComponent<MailSystemViewPreference>();
            this.viewPref.btnBack.onClick.AddListener(HideSystemMailDetail);
            this.viewPref.btnRead.onClick.AddListener(OnBtnMarkAllReadClick);
            this.SRIAInit(this.viewPref.scrollRect,
                         this.viewPref.verticalLayoutGroup,
                         defaultItemSize: 108f);
        }

        public void SetMail() {
            bool hasSystemMail = (this.viewModel.SystemMailList.Count > 0);
            this.viewPref.pnlNoMailBG.SetActiveSafe(!hasSystemMail);
        }

        #region SRIA implementation
        protected override MailSystemItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlSystemMailItem, this.viewPref.pnlList);
            MailSystemItemView itemView = itemObj.GetComponent<MailSystemItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView, this.viewModel.SystemMailList[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(MailSystemItemView itemView) {
            this.OnItemContentChange(itemView,
            this.viewModel.SystemMailList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(MailSystemItemView itemView, SystemMessage itemData) {
            itemView.SetItemContent(itemData, itemData.IsTemplate ?
                this.viewModel.GetSystemContent(itemData) : itemData.Body);
            SystemMessage system = itemData;
            itemView.OnInfoClick.AddListener(
                () => this.viewModel.SystemMail = system);
        }

        public void HidePnlNoMalilBG() {
            this.viewPref.pnlNoMailBG.SetActiveSafe(false);
        }

        public void HideSystemMailDetail() {
            if (this.isShowSystemMailDetail) {
                UIManager.SetUICanvasGroupEnable(this.viewPref.systemDetail, false);
                this.viewModel.NewSystemMail();
                this.isShowSystemMailDetail = false;
            }
        }

        public void ShowSystemMailDetail() {
            UIManager.SetUICanvasGroupEnable(this.viewPref.systemDetail, true);
            this.isShowSystemMailDetail = true;
            this.viewPref.txtSubject.text = this.viewModel.SystemMail.IsTemplate ?
                LocalManager.GetValue(this.viewModel.SystemMail.Subject) :
                this.viewModel.SystemMail.Subject;
            this.viewPref.txtContent.text = this.viewModel.SystemMail.IsTemplate ?
                this.viewModel.GetSystemContent(this.viewModel.SystemMail) :
                this.viewModel.SystemMail.Body;
            this.hasAttachment = (this.viewModel.SystemMail.Attachment != null);
            this.viewPref.mailRewards.SetActiveSafe(this.hasAttachment);
            this.SetBtnReceiveReward(this.viewModel.SystemMail.IsCollect);
            if (this.hasAttachment) {
                this.SetSystemMailRewardInfo();
            }
        }

        private void SetSystemMailRewardInfo() {
            Dictionary<Resource, int> rewardDict = this.viewModel.SystemMail.Attachment.GetRewardsDict(false);
            int rewardCount = rewardDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlRewardsContent,
                rewardCount, PrefabPath.pnlItemWithCount);
            int index = 0;
            ItemWithCountView itemView = null;
            foreach (var resource in rewardDict) {
                //Debug.LogError(resource.Key + " " + resource.Value);
                itemView = this.viewPref.pnlRewardsContent
                    .GetChild(index++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(resource.Key, resource.Value);
                this.viewModel.resourceTransDict[resource.Key] = itemView.imgItem.transform;
            }

            this.SetSystemMailFragmentsInfo(
                this.viewModel.SystemMail.Attachment.Fragments);

            this.viewPref.btnReceive.onClick.RemoveAllListeners();
            this.viewPref.btnReceive.onClick.AddListener(
                this.viewModel.GetMailAttachMent);
        }

        private void SetSystemMailFragmentsInfo(List<Protocol.HeroFragment> fragments) {
            if (fragments != null && fragments.Count > 0) {
                ItemWithCountView itemView = null;
                foreach (HeroFragment fragment in fragments) {
                    itemView = PoolManager.GetObject(PrefabPath.pnlItemWithCount,
                        this.viewPref.pnlRewardsContent).GetComponent<ItemWithCountView>();
                    itemView.SetHeroFragmentsClickInfo(fragment, this.OnHeroClick);
                }
            }
        }

        private void OnHeroClick(string heroName) {
            this.viewModel.OnHeroClick(heroName);
        }

        public void SetBtnReceiveReward(bool isCollected) {
            if (this.isShowSystemMailDetail && this.hasAttachment) {
                this.viewPref.isReceive.text = isCollected ?
                LocalManager.GetValue(LocalHashConst.button_received) :
                LocalManager.GetValue(LocalHashConst.button_receive);
                this.viewPref.btnReceive.Grayable = isCollected;
            }
        }

        private void OnBtnMarkAllReadClick() {
            this.viewModel.MarkMailIsRead();
            this.viewModel.Show();
        }
    }
}
