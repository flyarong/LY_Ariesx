using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class PayCountersignView : BaseView {
        private PayCountersignViewModel viewModel;
        private PayCountersignViewPreference viewPref;

        private string ChestName;
        private GoldProductConf product;
        private Dictionary<Resource, Transform> resourceDict = new Dictionary<Resource, Transform>();

        private const int PRODUCTION_TIMES = 10;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<PayCountersignViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPayCountersign");
            this.viewPref = this.ui.transform.GetComponent<PayCountersignViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.background.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.btnPay.onClick.AddListener(this.OnClickPay);
            this.resourceDict.Add(Resource.Gold, this.viewPref.ImgGold.transform);
        }

        public override void PlayShow() {
            base.PlayShow(() => {
            });
        }

        public override void PlayHide() {
            base.PlayHide();
        }

        public void SetProductConf(GoldProductConf productConf) {
            this.product = productConf;
            this.viewPref.txtNum.text = product.goldAmount;
            this.viewPref.txtPrice.text = RoleManager.GetResource(Resource.Gem) < int.Parse(product.gemPrice) ?
                                string.Format("<color=#FF0000FF>{0}</color>", product.gemPrice) : product.gemPrice;
            this.viewPref.txtTitle.text = "购买"+LocalManager.GetValue(product.id);
            this.viewPref.ImgGold.sprite = ArtPrefabConf.GetSprite(this.product.id + "_icon");
        }

         private IEnumerator ShowTip() {
            this.viewPref.pnlTip.SetActive(true);
            AnimationManager.Animate(this.viewPref.pnlTip,"Show",finishCallback:()=>{
            });
            yield return YieldManager.GetWaitForSeconds(4);
            this.viewPref.pnlTip.SetActive(false);
        }

        private void OnClickPay() {
            if (RoleManager.GetResource(Resource.Gem) < int.Parse(product.gemPrice)) {
                StartCoroutine(ShowTip());
                return;
            }
            GemExchangeGoldReq gemExchangeGoldReq = new GemExchangeGoldReq {
                ProductId = product.id
            };
            NetManager.SendMessage(gemExchangeGoldReq, "GemExchangeGoldAck", this.GemExchangeGoldAck,this.ErrorGemExchangeGoldAck);
        }

        private void ErrorGemExchangeGoldAck(IExtensible message) {
            UIManager.ShowTip(LocalManager.GetValue("gold_by_gem_short"), TipType.Error);
        }

        private void GemExchangeGoldAck(IExtensible message) {
            GemExchangeGoldAck gemExchangeGoldAck = message as GemExchangeGoldAck;
            UIManager.ShowTip(string.Format(LocalManager.GetValue(LocalHashConst.shop_buy_tips),
                this.product.goldAmount)+LocalManager.GetValue(LocalHashConst.resource_gold), TipType.Notice);
            GameHelper.CollectResources(gemExchangeGoldAck.Reward.Resources, gemExchangeGoldAck.Reward.Currency,
                gemExchangeGoldAck.Resources, gemExchangeGoldAck.Currency, this.resourceDict);
            this.viewModel.SetChangeGem(-int.Parse(this.product.gemPrice));
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide();
        }
    }
}
