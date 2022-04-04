using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class PayRewardView : BaseView {
        private PayRewardViewModel viewModel;
        private PayRewardViewPreference viewPref;
        /* UI Members*/
        private Dictionary<Resource, Transform> resourceDict =
            new Dictionary<Resource, Transform>();
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<PayRewardViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIpayreward");
            this.viewPref = this.ui.GetComponent<PayRewardViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.RemoveAllListeners();
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnJump.onClick.RemoveAllListeners();
            this.viewPref.btnJump.onClick.AddListener(this.OnBtnJumpClick);
            this.viewPref.btnInfo.onClick.RemoveAllListeners();
            this.viewPref.btnInfo.onClick.AddListener(this.OnBtnInfoClick);
            this.viewPref.btnCollect.onClick.RemoveAllListeners();
            this.viewPref.btnCollect.onClick.AddListener(this.OnBtnCollectClick);
            /* Cache the ui components here */
        }

        public void SetInfo() {
            this.viewModel.CurrentConf = FirstPayConf.GetCurrentLevel(this.viewModel.CurrentLevel + 1);
            bool isDone = this.viewModel.CurrentConf.threshold <= this.viewModel.CurrentValue;

            this.viewPref.btnJump.gameObject.SetActive(!isDone);
            this.viewPref.btnCollect.gameObject.SetActive(isDone);

            TextMeshProUGUI textPrice;
            Image imgTitle;
            bool isShowHero = this.viewModel.CurrentConf.bgType == 1;
            this.viewPref.pnlShowHero.gameObject.SetActive(isShowHero);
            this.viewPref.pnlShowResource.gameObject.SetActive(!isShowHero);
            this.viewPref.btnInfo.gameObject.SetActive(isShowHero);
            if (isShowHero) {
                textPrice = this.viewPref.txtHeroPrice;
                imgTitle = this.viewPref.imgShowHero;
                this.viewPref.pnlResources.gameObject.SetActive(false);
            } else {
                textPrice = this.viewPref.txtResourcePrice;
                imgTitle = this.viewPref.imgShowResource;
                this.viewPref.pnlResources.gameObject.SetActive(true);
                this.SetResources();
            }


            this.viewPref.imgBackground.sprite = ArtPrefabConf.GetSprite(
                SpritePath.payRewardBgPrefix, isShowHero ? "hero" : "resource"
            );
            bool isFirst = this.viewModel.CurrentConf.level == 1;
            textPrice.gameObject.SetActive(!isDone);
            string imgSuffix = isDone ? "done" : (isFirst ? "first" : "other");
            imgTitle.sprite = ArtPrefabConf.GetSprite(
                SpritePath.payRewardPriceBgPrefix,
                imgSuffix
            );

            if (!isDone) {
                textPrice.text = (isFirst ? this.viewModel.CurrentConf.threshold :
                    this.viewModel.CurrentConf.threshold - this.viewModel.CurrentValue).ToString();
            }
        }

        private void SetResources() {
            GameHelper.ResizeChildreCount(this.viewPref.pnlResources,
                this.viewModel.CurrentConf.resourceDict.Count, PrefabPath.pnlItemWithCount);
            int index = 0;
            this.resourceDict.Clear();
            foreach (var pair in this.viewModel.CurrentConf.resourceDict) {
                Transform child = this.viewPref.pnlResources.GetChild(index++);
                child.GetComponent<ItemWithCountView>().
                    SetResourceInfo(pair.Key, pair.Value);
                this.resourceDict.Add(pair.Key, child);
            }
        }

        public void SetBtnEnable(bool enable) {
            this.viewPref.btnCollect.Grayable = !enable;
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnBtnInfoClick() {
            this.viewModel.ShowHeroInfo();
        }

        private void OnBtnJumpClick() {
            this.viewModel.ShowPay();
        }

        private void OnBtnCollectClick() {
            if (!this.viewPref.btnCollect.Grayable) {
                this.viewModel.GetRewardReq();
            }
        }

        public void CollectResource(GetRechargeRewardAck reward) {
            GameHelper.CollectResources(
                reward.Reward,
                reward.Resources,
                reward.Currency,
                this.resourceDict
            );
            GachaGroupConf groupConf = GachaGroupConf.GetConf(reward.Reward.Chests[0].Name);
            if (groupConf != null && this.resourceDict.ContainsKey(Resource.Chest)) {
                this.viewModel.LotteryChanceList.AddRange(reward.Reward.Chests);
                GameHelper.ChestCollect(
                    this.resourceDict[Resource.Chest].position,
                    groupConf,
                    CollectChestType.collectWithShow
                );
            }
        }

        /* Propert change function */

        /***************************/

        protected override void OnVisible() {
        }

        protected override void OnInvisible() {
        }
    }
}
