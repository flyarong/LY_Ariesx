using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class DailyGiftDetailsView : BaseView {
        private DailyGiftDetailsViewModel viewModel;
        private DailyGiftDetailsPreference viewPref;

        private string ChestName;
        private ProductConf product;

        private const int PRODUCTION_TIMES = 10;

        //private bool isPay = false;
        private Dictionary<Resource, Transform> resourceDict =
                   new Dictionary<Resource, Transform>();

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<DailyGiftDetailsViewModel>();

            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIDailyGiftDetails");
            this.viewPref = this.ui.transform.GetComponent<DailyGiftDetailsPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.background.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.btnDetail.onClick.AddListener(this.OnClickDetail);
            this.viewPref.btnPay.onClick.AddListener(this.OnClickPay);
            this.resourceDict.Add(Resource.Gem, this.viewPref.btnPay.transform);
        }

        private void GetChest() {
            this.viewPref.imgMoveChest.sprite =
                ArtPrefabConf.GetChestSprite(this.ChestName);
            this.viewPref.pnlChest.SetActiveSafe(true);
            AnimationManager.Animate(this.viewPref.pnlChest, "Show", finishCallback: () => {
                AnimationManager.Animate(this.viewPref.pnlChest, "Move",
                    start: this.viewPref.pnlChest.
                    GetComponent<RectTransform>().anchoredPosition,
                    target: this.viewPref.pnlHide.GetComponent<RectTransform>()
                    .anchoredPosition, finishCallback: () => {
                         AnimationManager.Animate(this.viewPref.pnlChest,
                             "Hide", finishCallback: () => {
                                 this.viewPref.pnlChest.SetActiveSafe(false);
                             });
                     }, space: PositionSpace.World
                    );
            });
        }

        public override void PlayShow() {
            this.InitUI();
            base.PlayShow(() => {
            });
        }

        public override void PlayHide() {
            base.PlayHide();
        }

        public void SetChest(GachaGroupConf lotteryConf) {
            if (lotteryConf != null) {
                this.viewPref.imgChest.gameObject.SetActiveSafe(true);
                this.viewPref.btnDetail.gameObject.SetActiveSafe(true);
                this.viewPref.imgChest.sprite =
                     ArtPrefabConf.GetChestSprite(lotteryConf.chest);
                //Debug.Log(this.product.id);
                this.viewPref.txtChestAmount.text = string.Concat("X ", DailyGiftConf.GetConf(this.product.id).ChestAmount);
                this.viewPref.txtGemAmount.text = DailyGiftConf.GetConf(this.product.id).GenAmount;
                string chestName = lotteryConf.chest;
                this.viewPref.txtChest.text = LocalManager.GetValue(chestName);
                this.ChestName = lotteryConf.chest;
            } else {
                this.viewPref.imgChest.gameObject.SetActiveSafe(false);
                this.viewPref.btnDetail.gameObject.SetActiveSafe(false);
            }
        }

        public void SetProductConf(ProductConf productConf) {
            this.product = productConf;
            this.viewPref.txtTitle.text = LocalManager.GetValue(this.product.id);
            this.viewPref.txtPrice.text = string.Concat("￥ ", product.price);
        }

        private void OnClickDetail() {
            this.viewModel.ShowHeroPool(ChestName);
        }

        private void OnClickPay() {
            this.viewModel.Hide();
            this.viewModel.Buy();
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide();
        }
    }
}
