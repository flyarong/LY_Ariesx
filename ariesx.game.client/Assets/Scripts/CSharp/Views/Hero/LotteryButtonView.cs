using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class LotteryButtonView : MonoBehaviour {
        private GameObject ui;
        private CustomButton btnLottery;
        private CustomButton btnHeroPoolInfo;
        private Image imgCoinType;
        private TextMeshProUGUI txtPrice;

        //private GachaGroupConf lotteryConf;

        public UnityEvent OnBtnLotteryClick {
            get {
                this.btnLottery.onClick.RemoveAllListeners();
                return this.btnLottery.onClick;
            }
        }

        void Awake() {
            this.ui = this.gameObject;
            Transform pnlBtnLottery = this.ui.transform.Find("BtnLottery");
            this.btnLottery = pnlBtnLottery.GetComponent<CustomButton>();
            this.imgCoinType = pnlBtnLottery.Find("PnlContent").Find("Image").GetComponent<Image>();
            this.txtPrice = pnlBtnLottery.Find("PnlContent").Find("TxtPrice").GetComponent<TextMeshProUGUI>();
        }
       
        public void SetLotteryConf(GachaGroupConf lotteryConf, bool isMultiple) {
            //this.lotteryConf = lotteryConf;
            int price = 0;
            price = lotteryConf.price;
            bool isFree = (price <= 0);
            this.imgCoinType.gameObject.SetActiveSafe(!isFree);
            if (!isFree) {
                this.txtPrice.text = price.ToString();
                if (price > RoleManager.GetResource(Resource.Gem)) {
                    txtPrice.color = Color.red;
                } else {
                    txtPrice.color = Color.white;
                }
            } else {
                this.txtPrice.text = LocalManager.GetValue(LocalHashConst.button_chest_free);
            }
            this.imgCoinType.sprite =
                ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix, 
                    Enum.GetName(typeof(Resource), Resource.Gem).ToLower());
        }

        public void SetBtnLotteryGrayable(bool grayable) {
            this.btnLottery.Grayable = grayable;
        }
    }
}
