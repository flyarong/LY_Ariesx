using UnityEngine;
using System.Collections;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public enum FieldFirstDownViewType {
        None,
        GetReward,
        RewardList
    }
    public class FirstDownRewardView : SRIA<FirstDownRewardViewModel, FirstDownRewardItemView> {
        private FirstDownRewardViewPreference viewPref;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIFirstDownReward");
            this.viewModel = this.gameObject.GetComponent<FirstDownRewardViewModel>();
            this.viewPref = this.ui.transform.GetComponent<FirstDownRewardViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.verticalLayoutGroup, defaultItemSize: 627);
        }

        #region SRIA implementation
        protected override FirstDownRewardItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlFirstDownRewardItem, this.viewPref.pnlList);
            FirstDownRewardItemView itemView = itemObj.GetComponent<FirstDownRewardItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.AllFieldReward[itemIndex]);
            return itemView;
        }

        protected override void UpdateViewsHolder(FirstDownRewardItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.AllFieldReward[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(FirstDownRewardItemView itemView, int itemData) {
            itemView.Type = FieldFirstDownViewType.RewardList;
            if (this.viewModel.FieldFirstDownDict.ContainsKey(itemData)) {
                itemView.CanCollect = true;
                itemView.IsCollected =
                    this.viewModel.FieldFirstDownDict[itemData].IsCollect;
            } else {
                itemView.CanCollect = false;
                itemView.IsCollected = false;
            }
            itemView.Level = itemData;
        }

        public void SetRewardListView() {
            this.InitUI();
            this.SetPanelVisible(true);
            this.viewPref.showObj = this.viewPref.pnlFieldReward;


            this.ResetItems(this.viewModel.AllFieldReward.Count);
            this.StartCoroutine(this.ScrollToLevel());
            this.PlayShow(true);
        }

        public void SetGetRewardView() {
            this.InitUI();
            this.SetPanelVisible(false);
            this.viewPref.showObj = this.viewPref.pnlGetFieldReward;
            this.SetFirstDownRewardDetail();
            AudioManager.Play("show_lottery_rarity_2", AudioType.Show, AudioVolumn.High);
            this.PlayShow(true);
        }

        private void SetPanelVisible(bool isRewardList) {
            this.viewPref.btnBackground.enabled = isRewardList;
            UIManager.SetUICanvasGroupEnable(this.viewPref.FieldRewardCG, isRewardList);
            UIManager.SetUICanvasGroupEnable(this.viewPref.GetFieldRewardCG, !isRewardList);
        }

        public void GetReward(FieldFirstDownRewardNtf reward) {
            this.viewPref.rewardItemView.GetFirstFieldDownReward(reward);
        }

        /***************************/
        private IEnumerator ScrollToLevel() {
            yield return YieldManager.EndOfFrame;
            int itemIndex = RoleManager.GetFDRecordMaxLevel() - 1;
            //int itemIndex = RoleManager.GetFDRecordMaxLevel();
            //itemIndex = itemIndex < 1 ? 0 : itemIndex;
            this.ScrollTo(itemIndex);
        }

        private void SetFirstDownRewardDetail() {
            this.viewPref.rewardItemView.Type = FieldFirstDownViewType.GetReward;
            this.viewPref.rewardItemView.CanCollect = true;
            this.viewPref.rewardItemView.IsCollected = false;
            this.viewPref.rewardItemView.Level = this.viewModel.NewLevel;
            this.viewPref.rewardItemView.OnReceiveClick.RemoveAllListeners();
            this.viewPref.rewardItemView.OnReceiveClick.AddListener(this.OnBtnReceiveClick);
            AnimationManager.Animate(this.viewPref.imgHalo, "Show");
        }

        private void OnBtnReceiveClick() {
            this.viewModel.GetReward();
            this.viewModel.Hide();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        protected override void OnInvisible() {
            AnimationManager.Stop(this.viewPref.imgHalo);
        }
    }
}
