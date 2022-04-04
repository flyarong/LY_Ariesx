using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Poukoute {
    public class PayView : BaseView {
        private PayViewModel viewModel;
        private PayViewPreference viewPref;
        private Dictionary<string, BaseConf> pduDict;
        private Dictionary<ProductType, GameObject> pduItemDict = new Dictionary<ProductType, GameObject>();
        private Dictionary<ProductType, int> sortPduItemDict = new Dictionary<ProductType, int>();
        public Dictionary<string, GameObject> pdu2lItemDict = new Dictionary<string, GameObject>();
        private List<PayPduItemView> dailyGiftList = new List<PayPduItemView>();
        private PayPduItemView mouthCardView;
        public List<PayPduItemView> smartGiftBagViewList = new List<PayPduItemView>();
        public Transform backGround;
        public bool isInitUI = false;
        private Dictionary<ProductType, TextMeshProUGUI> pnlDailyTimeDict = new Dictionary<ProductType, TextMeshProUGUI>();

        public Dictionary<Resource, Transform> ResourceDict {
            get {
                return this.viewModel.resourceDict;
            }
        }

        void Awake() {
            this.viewModel = this.GetComponent<PayViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPay");
            this.viewPref = this.ui.transform.GetComponent<PayViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.backGround = this.viewPref.btnBackground.transform;
            pduDict = ConfigureManager.GetConfDict<ProductConf>();
            this.GetAllProduct();
            this.InitSortPduItemDict();
            this.isInitUI = true;
        }

        public void RefreshDailyGiftItem() {
            foreach (var item in this.dailyGiftList) {
                item.RefreshDailyGiftItem();
            }
        }

        public void RefreshMouthCardItem(string productId, long RemainTime, bool isReward, bool noMonthCard = false) {
            this.mouthCardView.RefreshMouthCardItem(productId, RemainTime, isReward, noMonthCard);
        }

        public void SetMonthCardUpdate() {
            this.mouthCardView.SetMonthCardUpdate();
        }

        public void RefreshSmartGiftBagItem() {
            if (this.viewModel.DailyShop.FreeGold != 0) {
                smartGiftBagViewList[0].RefreshSmartGiftBagGoldItem(
                    this.viewModel.DailyShop.FreeGold,
                    this.viewModel.DailyShop.FreeGoldCollected);
            }
            if (this.viewModel.DailyShop.FreeGem != 0) {
                smartGiftBagViewList[0].RefreshSmartGiftBagGemItem(
                    this.viewModel.DailyShop.FreeGem,
                    this.viewModel.DailyShop.FreeGemCollected);
            }

            int dailyShopHeroItemCount = this.viewModel.DailyShop.HeroItem.Count;
            if (dailyShopHeroItemCount == 3) {
                for (int i = dailyShopHeroItemCount - 1; i >= 0; i--) {
                    smartGiftBagViewList[i].RefreshSmartGiftBagHeroItem(
                        this.viewModel.DailyShop.HeroItem[i]);
                }
            } else if (dailyShopHeroItemCount == 2) {
                for (int i = dailyShopHeroItemCount; i >= 1; i--) {
                    smartGiftBagViewList[i].RefreshSmartGiftBagHeroItem(
                        this.viewModel.DailyShop.HeroItem[i - 1]);
                }
            }
            EventManager.AddEventAction(Event.SmartGiftBag, this.UpdateSmartGiftBag);
        }

        private void UpdateSmartGiftBag(EventBase eventBase) {            
            long time = eventBase.duration + eventBase.startTime - RoleManager.GetCurrentUtcTime();
            this.pnlDailyTimeDict[ProductType.SmartGiftBag].text =
                string.Format("<color=#ff5e5b>{0}</color>",
                    string.Format(GameHelper.TimeFormat(time)
                )
            );
            time = (long)Mathf.Max(0, time);
            if (time == 0) {
                Debug.Log("UpdateSmartGiftBag");
                this.GetStoreInfo();
                this.SortVision(ProductType.SmartGiftBag, 3);
                EventManager.RemoveEventAction(Event.SmartGiftBag, this.UpdateSmartGiftBag);
            }
        }

        public void GetStoreInfo() {
            this.viewModel.GetStoreInfo();
        }

        public void SetPayBtn(bool canReward) {
            this.viewModel.SetPayBtn(canReward);
        }

        public void Pay(string price, string roleId, string pduId, string channelId, string orderId, string pduName) {
#if DEVELOPER
            this.viewModel.Pay("1", roleId, pduId, channelId, orderId, pduName);
#else
            this.viewModel.Pay(price, roleId, pduId, channelId, orderId, pduName);
#endif
        }

        public void GetAllProduct() {
            ProductType type = ProductType.None;
            foreach (var item in pduDict) {
                if (type != ((ProductConf)item.Value).type) {
                    type = ((ProductConf)item.Value).type;
                    GameObject itemObj =
                    PoolManager.GetObject(PrefabPath.pnlPduItem, this.viewPref.pnlList);
                    switch (type) {
                        case ProductType.Gem:
                            itemObj.transform.Find("PnlDailyTitle").Find("TitleText").GetComponent<TextMeshProUGUI>().text
                         = LocalManager.GetValue("title_product_gem");
                            break;
                        case ProductType.DailyGift:
                            itemObj.transform.Find("PnlDailyTitle").Find("TitleText").GetComponent<TextMeshProUGUI>().text
                        = LocalManager.GetValue("title_daily_gift");
                            break;
                        case ProductType.MonthCard:
                            itemObj.transform.Find("PnlDailyTitle").Find("TitleText").GetComponent<TextMeshProUGUI>().text
                        = LocalManager.GetValue("title_month_card");
                            break;
                    }

                    pduItemDict[type] = itemObj;
                }
            }
            foreach (var item in pduDict) {
                Transform pnlViewTF = pduItemDict[((ProductConf)item.Value).type].transform.Find("PnlView");
                if (((ProductConf)item.Value).type == ProductType.MonthCard) {
                    pnlViewTF.GetComponent<GridLayoutGroup>().padding.top = 20;
                    pnlViewTF.GetComponent<GridLayoutGroup>().cellSize = new Vector2(679, 357);
                    GameObject itemObj = PoolManager.GetObject(PrefabPath.pnl2lMonthCardItem, pnlViewTF);
                    this.pdu2lItemDict[item.Key] = itemObj;
                    PayPduItemView itemView = itemObj.AddComponent<PayPduItemView>();
                    itemView.ChooseVision<ProductConf>(((ProductConf)item.Value));
                    itemView.parent = this;
                    this.mouthCardView = itemView;
                } else {
                    GameObject itemObj =
                        PoolManager.GetObject(PrefabPath.pnl2lPduItem, pnlViewTF);
                    pdu2lItemDict[item.Key] = itemObj;
                    PayPduItemView itemView = itemObj.AddComponent<PayPduItemView>();
                    if (((ProductConf)item.Value).type == ProductType.DailyGift) {
                        dailyGiftList.Add(itemView);
                    }
                    itemView.ChooseVision<ProductConf>(((ProductConf)item.Value));
                    itemView.parent = this;
                }
            }
            this.SetGemExchangeGold();
            this.SetSmartGiftBag();
        }

        //智能礼包
        private void SetSmartGiftBag() {
            DailyShop dailyShop = RoleManager.Instance.GetDailyShop();
            if (dailyShop == null) {
                return;
            }
            GameObject itemObj =
                    PoolManager.GetObject(PrefabPath.pnlPduItem, this.viewPref.pnlList);
            itemObj.transform.Find("PnlDailyTitle").Find("TitleText").GetComponent<TextMeshProUGUI>().text
                       = LocalManager.GetValue(LocalHashConst.smart_gift);
            this.pduItemDict[ProductType.SmartGiftBag] = itemObj;
            GameObject timeItem = itemObj.transform.Find("PnlDailyTime").gameObject;
            timeItem.SetActiveSafe(true);
            this.pnlDailyTimeDict[ProductType.SmartGiftBag] = timeItem.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            //Debug.LogError(dailyShop);
            for (int i = 0; i < 3; i++) {
                GameObject item2lObj =
               PoolManager.GetObject(
                   PrefabPath.pnl2lPduItem, itemObj.transform.Find("PnlView"));
                PayPduItemView itemView
                    = item2lObj.AddComponent<PayPduItemView>();
                smartGiftBagViewList.Add(itemView);
                itemView.parent = this;
            }
            if (dailyShop.FreeGem != 0) {
                smartGiftBagViewList[0].ChooseSmartGigtBagByGold(dailyShop.FreeGold, dailyShop.FreeGoldCollected);
            }
            if (dailyShop.FreeGold != 0) {
                smartGiftBagViewList[0].ChooseSmartGiftBagByGem(dailyShop.FreeGem, dailyShop.FreeGemCollected);
            }
            if (dailyShop.HeroItem.Count == 3) {
                for (int i = dailyShop.HeroItem.Count - 1; i >= 0; i--) {

                    smartGiftBagViewList[i].ChooseSmartGiftBagByHero(dailyShop.HeroItem[i]);
                }
            } else if (dailyShop.HeroItem.Count == 2) {
                for (int i = dailyShop.HeroItem.Count; i >= 1; i--) {

                    smartGiftBagViewList[i].ChooseSmartGiftBagByHero(dailyShop.HeroItem[i - 1]);
                }
            }
            itemObj.transform.SetSiblingIndex(1);
        }

        public void SetGemExchangeGold() {
            GameObject itemObj =
                    PoolManager.GetObject(PrefabPath.pnlPduItem, this.viewPref.pnlList);
            itemObj.transform.Find("PnlDailyTitle").Find("TitleText").GetComponent<TextMeshProUGUI>().text
                       = LocalManager.GetValue("title_product_gold");
            this.pduItemDict[ProductType.GemExchangeGold] = itemObj;
            foreach (var item in ConfigureManager.GetConfDict<GoldProductConf>()) {
                GameObject item2lObj =
                PoolManager.GetObject(
                    PrefabPath.pnl2lPduItem, itemObj.transform.Find("PnlView"));
                pdu2lItemDict[item.Key] = itemObj;
                PayPduItemView itemView
                    = item2lObj.AddComponent<PayPduItemView>();
                itemView.ChooseVision<GoldProductConf>(((GoldProductConf)item.Value));
                itemView.parent = this;
            }
            itemObj.transform.SetSiblingIndex(1);
        }

        public void RefreshDailyGift() {
            foreach (var item in dailyGiftList) {
                item.SetDailyGiftCanBuy();
            }
            foreach (var item in this.viewModel.DailyGifts) {
                if (item.IsBuy) {
                    pdu2lItemDict[item.ProductId].
                        GetComponent<PayPduItemView>().SetDailyGiftCantBuy();
                }
            }
        }

        private void InitSortPduItemDict() {
            this.sortPduItemDict.Add(ProductType.MonthCard, 0);
            this.sortPduItemDict.Add(ProductType.DailyGift, 2);
            this.sortPduItemDict.Add(ProductType.SmartGiftBag, 3);
            this.sortPduItemDict.Add(ProductType.Gem, 4);
            this.sortPduItemDict.Add(ProductType.GemExchangeGold, 5);
            foreach (var pdu in sortPduItemDict) {
                pduItemDict[pdu.Key].transform.SetSiblingIndex(this.GetSiblingIndex(pdu.Key));
            }
        }

        private int GetSiblingIndex(ProductType name) {
            this.sortPduItemDict = (from temp in this.sortPduItemDict orderby temp.Value select temp).
                ToDictionary(pair => pair.Key, pair => pair.Value);
            int i = 0;
            foreach (var item in this.sortPduItemDict) {
                if (item.Key == name) {
                    break;
                }
                i++;
            }
            return i;
        }

        public void SortVision(ProductType name, int num) {
            return;
            sortPduItemDict[name] = num;
            pduItemDict[name].transform.SetSiblingIndex(this.GetSiblingIndex(name));
        }
        public override void PlayShow(UnityAction callback) {
            this.InitUI();
            base.PlayShow(callback, true);
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(callback);
        }

        public void DailyGiftShow(GachaGroupConf lotteryConf, ProductConf productConf, PayPduItemView pduItem) {
            this.viewModel.DailyGiftShow(lotteryConf, productConf, pduItem);
        }

        public void PayCountersingShow(GoldProductConf goldProductConf) {
            this.viewModel.PayCountersingShow(goldProductConf);
        }

        public void ShowSmartGiftBag(HeroItem heroItem) {
            this.viewModel.ShowSmartGiftBag(heroItem);
        }

        public void DailyGiftHide() {
            this.viewModel.DailyGiftHide();
        }

        public void Format() {
            UnityAction format = () => {
                this.viewPref.customVerticalLayoutGroup.SetOriginal();
                this.viewPref.customScrollRect.velocity = Vector2.zero;
                this.viewPref.customScrollRect.verticalNormalizedPosition = 1;
            };
            this.viewPref.contentSizeFitter.onSetLayoutVertical.AddListener(format);
        }

        public void FormatEveryTime() {
            this.viewPref.customScrollRect.verticalNormalizedPosition = 1;
        }

        public void SetPnlLoad(bool visible) {
            this.viewPref.pnlLoad.SetActiveSafe(visible);
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        public void ShowPnlTip(bool isFree) {
            this.viewPref.pnlTip.transform.Find("TxtDes")
                .GetComponent<TextMeshProUGUI>().text
                 = isFree ? LocalManager.GetValue(LocalHashConst.shop_have_collected_tips) :
                LocalManager.GetValue(LocalHashConst.shop_have_bought_tips);
            AnimationManager.Animate(this.viewPref.pnlTip, "Show");
        }
    }
}
