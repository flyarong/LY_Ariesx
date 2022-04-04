using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;

namespace Poukoute {
    public class PayPduItemView : MonoBehaviour {
        // ui参数和自定义参数 分开.
        private PayPduItemViewPriference viewPref;
        private PayMonthCardItemViewPreference monthCardViewPref;
        private HeroModel heroModel;
        private PayModel payModel;
        public PayView parent;
        [HideInInspector]
        public bool isPay = false;

        private ProductConf pduSelfName;
        private GoldProductConf goldSelfName;

        private bool isFree = false;
        private long expiredTime;
        private Dictionary<Resource, Transform> resourceDict =
                    new Dictionary<Resource, Transform>();
        private Currency addCurrency = new Currency();

        public List<GachaGroupConf> LotteryDict {
            get {
                return this.heroModel.lotteryList;
            }
        }

        /******************************/
        private void Awake() {
            //this.model = ModelManager.GetModelData<PayModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.payModel = ModelManager.GetModelData<PayModel>();
            this.resourceDict.Add(Resource.Gem, this.transform);
        }

        public void ChooseVision<T>(T whitch) {
            switch (whitch.GetType().ToString()) {
                case "Poukoute.ProductConf":
                    pduSelfName = whitch as ProductConf;
                    switch (pduSelfName.type) {
                        case ProductType.Gem:
                            this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
                            this.viewPref.btnSellOut.onClick.AddListener(OnBtnSellOutClick);
                            this.viewPref.txtSellOut.text =
                                LocalManager.GetValue(LocalHashConst.shop_sold_out);
                            this.SetGemItem();
                            break;
                        case ProductType.DailyGift:
                            this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
                            this.viewPref.btnSellOut.onClick.AddListener(OnBtnSellOutClick);
                            this.viewPref.txtSellOut.text =
                                LocalManager.GetValue(LocalHashConst.shop_sold_out);
                            this.SetDailyGiftItem();
                            break;
                        case ProductType.MonthCard:
                            this.monthCardViewPref =
                                transform.GetComponent<PayMonthCardItemViewPreference>();
                            this.SetmonthCardItem();
                            break;
                        default:
                            break;
                    }
                    break;
                case "Poukoute.GoldProductConf":
                    this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
                    goldSelfName = whitch as GoldProductConf;
                    SetGemExchangeGoldItem();
                    break;
            }
        }
        #region SmartGiftBag
        public string FragmentSliderText {
            get {
                return this.viewPref.txtFragment.text;
            }
            private set {
                this.viewPref.txtFragment.text = value;
            }
        }

        public float Percent {
            get {
                return this.viewPref.sliFragment.value / this.viewPref.sliFragment.maxValue;
            }
            private set {
                this.viewPref.sliFragment.value = value * this.viewPref.sliFragment.maxValue;
            }
        }

        public void ChooseSmartGigtBagByGold(int gold, bool freeGoldCollected) {
            this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
            this.isFree = true;
            this.viewPref.btnSellOut.onClick.AddListener(OnBtnSellOutClick);
            this.viewPref.txtSellOut.text = LocalManager.GetValue(LocalHashConst.shop_have_collected);
            this.viewPref.pnlSellOut.transform.Find("ImgSellOut").GetComponent<Image>().sprite
                = ArtPrefabConf.GetSprite("Sell_Out_Free");
            this.viewPref.txtDis.text = LocalManager.GetValue(LocalHashConst.title_product_gold);
            this.viewPref.imgDimPdu.sprite =
               ArtPrefabConf.GetSprite("gold_by_gem_1_icon");
            this.viewPref.txtAmount.text =
                LocalManager.GetValue(LocalHashConst.button_chest_free);
            this.viewPref.txtDimPduNum.text = string.Format("<color=#FFF934>{0}</color>", gold);
            this.viewPref.btnPay.onClick.RemoveAllListeners();
            this.viewPref.btnPay.onClick.AddListener(() => {
                OnClickSmartGirfBag(SmartGiftBagType.Gold, freeGoldCollected);
            });
            addCurrency.Gem = 0;
            addCurrency.Gold = gold;
            if (freeGoldCollected) {
                this.viewPref.pnlSellOut.SetActiveSafe(true);
                this.viewPref.pnlAmount.SetActiveSafe(false);
                this.viewPref.pnlGemAmount.SetActiveSafe(false);
            } else {
                this.viewPref.pnlSellOut.SetActiveSafe(false);
            }
        }

        public void ChooseSmartGiftBagByGem(int freeGem, bool freeGemCollected) {
            this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
            this.isFree = true;
            this.viewPref.btnSellOut.onClick.AddListener(OnBtnSellOutClick);
            this.viewPref.txtSellOut.text = LocalManager.GetValue(LocalHashConst.shop_have_collected);
            this.viewPref.pnlSellOut.transform.Find("ImgSellOut").GetComponent<Image>().sprite
                = ArtPrefabConf.GetSprite("Sell_Out_Free");
            this.viewPref.txtDis.text = LocalManager.GetValue(LocalHashConst.title_product_gem);
            this.viewPref.imgDimPdu.sprite =
                ArtPrefabConf.GetSprite("normal_gem_80_icon");
            this.viewPref.txtAmount.text =
                 LocalManager.GetValue(LocalHashConst.button_chest_free);
            this.viewPref.txtDimPduNum.text = string.Format("<color=#67d632><size=25>{0}</size></color>", freeGem);
            this.viewPref.btnPay.onClick.RemoveAllListeners();
            this.viewPref.btnPay.onClick.AddListener(() => { OnClickSmartGirfBag(SmartGiftBagType.Gem, freeGemCollected); });
            addCurrency.Gem = freeGem;
            addCurrency.Gold = 0;
            if (freeGemCollected) {
                this.viewPref.pnlSellOut.SetActiveSafe(true);
                this.viewPref.pnlAmount.SetActiveSafe(false);
                this.viewPref.pnlGoldAmount.SetActiveSafe(false);
            } else {
                this.viewPref.pnlSellOut.SetActiveSafe(false);
            }
        }

        public void ChooseSmartGiftBagByHero(HeroItem heroItem) {
            this.viewPref = transform.GetComponent<PayPduItemViewPriference>();
            this.isFree = false;
            this.viewPref.btnSellOut.onClick.AddListener(OnBtnSellOutClick);
            this.viewPref.txtSellOut.text = LocalManager.GetValue(LocalHashConst.shop_have_bought);
            this.viewPref.pnlSellOut.transform.Find("ImgSellOut").GetComponent<Image>().sprite
                = ArtPrefabConf.GetSprite("Sell_Out_Charge");
            this.viewPref.pnlDiamond.gameObject.SetActiveSafe(false);
            this.viewPref.pnlFirstTopup.gameObject.SetActiveSafe(false);
            this.viewPref.pnlHero.gameObject.SetActiveSafe(true);
            if (heroItem.IsCollect) {
                //this.viewPref.pnlAmount.gameObject.SetActiveSafe(true);
                //this.viewPref.txtAmount.text = LocalManager.GetValue(LocalHashConst.shop_have_bought);
                //this.viewPref.pnlGoldAmount.gameObject.SetActive(false);
                this.viewPref.pnlSellOut.SetActiveSafe(true);
                this.viewPref.pnlAmount.SetActiveSafe(false);
                this.viewPref.pnlGoldAmount.SetActiveSafe(false);
            } else {
                this.viewPref.pnlAmount.gameObject.SetActiveSafe(false);
                this.viewPref.pnlSellOut.SetActiveSafe(false);
                this.viewPref.pnlGoldAmount.gameObject.SetActive(true);
                this.viewPref.txtGoldAmount.text = heroItem.GoldPrice.ToString();
                StartCoroutine(RestartHLG(this.viewPref.pnlGoldAmount.GetComponent<HorizontalLayoutGroup>()));
            }
            this.viewPref.txtHeroNum.text = "X" + (heroItem.FragmentCount).ToString();
            this.viewPref.txtDis.text = HeroAttributeConf.GetLocalName(heroItem.HeroName);
            this.viewPref.imgHero.sprite = ArtPrefabConf.GetSprite(heroItem.HeroName + "_l");
            int heroLevel = 1;
            string heroName = null;
            int fragment = 0;
            if (HeroModel.Instance.heroDict.ContainsKey(heroItem.HeroName)) {
                heroLevel = HeroModel.Instance.heroDict[heroItem.HeroName].Level;
                heroName = HeroModel.Instance.heroDict[heroItem.HeroName].Name;
                fragment = HeroModel.Instance.heroDict[heroItem.HeroName].FragmentCount;
                SetFragment(heroName, heroLevel, fragment);
            } else {
                this.viewPref.pnlTierUp.gameObject.SetActiveSafe(false);
                this.FragmentSliderText = "0/0";
                this.Percent = 0;
            }
            SetHeroRarityInfo(heroItem.HeroName);
            this.viewPref.btnPay.onClick.RemoveAllListeners();
            this.viewPref.btnPay.onClick.AddListener(() => { OnClickSmartGirfBag(SmartGiftBagType.Hero, heroItem.IsCollect, heroItem); });
        }

        private void OnClickSmartGirfBag(SmartGiftBagType smartGiftBagType, bool isCollect = false, HeroItem heroItem = null) {
            if (isCollect) {
                this.parent.ShowPnlTip(true);
                return;
            }
            if (smartGiftBagType == SmartGiftBagType.Hero) {
                this.parent.ShowSmartGiftBag(heroItem);
            } else if (smartGiftBagType == SmartGiftBagType.Gold) {
                this.resourceDict.Clear();
                this.resourceDict[Resource.Gold] = this.transform;
                LYGameData.OnOtherClickBuyButton(
                   "Gold",
                   addCurrency.Gold.ToString(),
                   "0"
               );
                GetDailyShopReq(1);
            } else if (smartGiftBagType == SmartGiftBagType.Gem) {
                this.resourceDict.Clear();
                this.resourceDict[Resource.Gem] = this.transform;
                LYGameData.OnOtherClickBuyButton(
                    "Gem",
                    addCurrency.Gem.ToString(),
                    "0"
                );
                GetDailyShopReq(2);
            }
        }

        public void GetDailyShopReq(int catagory) {
            GetDailyShopReq getDailyShopReq = new GetDailyShopReq() {
                Fetch = 0,
                Catagory = catagory
            };
            NetManager.SendMessage(getDailyShopReq, "GetDailyShopAck", this.GetDailyShopAck, this.ErrorGetDailyShopAck);
        }

        private void GetDailyShopAck(IExtensible message) {
            GetDailyShopAck getDailyShopAck = message as GetDailyShopAck;
            GameHelper.CollectCurrency(addCurrency, getDailyShopAck.Currency, this.transform, true);
            if (addCurrency.Gem != 0) {
                this.RefreshSmartGiftBagGemItem(addCurrency.Gem, true);
                LYGameData.OnOtherBuyItemSuccess(
                    "Gem",
                    addCurrency.Gem.ToString(),
                    "0"
                );
            }
            if (addCurrency.Gold != 0) {
                this.RefreshSmartGiftBagGoldItem(addCurrency.Gold, true);
                LYGameData.OnOtherBuyItemSuccess(
                    "Gold",
                    addCurrency.Gold.ToString(),
                    "0"
                );
            }
        }

        private void ErrorGetDailyShopAck(IExtensible message) {
            UIManager.ShowTip(LocalManager.GetValue("ErrorGetDailyShopAck"), TipType.Error);
        }

        private void SetFragment(string heroName, int heroLevel, int fragment) {
            int heroFragments = HeroLevelConf.GetHeroUpgradFragments(heroName, heroLevel);
            bool reachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(heroName, heroLevel);
            this.viewPref.imgMaxLevel.gameObject.SetActiveSafe(reachMaxLevel);
            int fragmentCount = fragment;
            if (reachMaxLevel) {
                this.FragmentSliderText = fragmentCount.ToString();
                this.Percent = 1.0f;
            } else {
                if (heroLevel == 1) {
                    this.FragmentSliderText = string.Concat(fragmentCount + 1, "/", heroFragments + 1);
                } else {
                    this.FragmentSliderText = string.Concat(fragmentCount, "/", heroFragments);
                }
                this.Percent = fragmentCount / (heroFragments * 1.0f);
            }
            bool canLevelUp = fragmentCount >= heroFragments && !reachMaxLevel;
            this.viewPref.pnlTierUp.gameObject.SetActiveSafe(canLevelUp);
        }

        private void SetHeroRarityInfo(string heroName) {
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(heroName);
            if (heroAttribute != null) {
                int i = 0;
                for (; i < heroAttribute.rarity; i++) {
                    this.viewPref.heroRarity[i].gameObject.SetActiveSafe(true);
                }
                int rarityCount = this.viewPref.heroRarity.Length;
                for (; i < rarityCount; i++) {
                    this.viewPref.heroRarity[i].gameObject.SetActiveSafe(false);
                }
            }
        }

        public void RefreshSmartGiftBagGoldItem(int freeGold, bool freeGoldCollected) {
            this.ChooseSmartGigtBagByGold(freeGold, freeGoldCollected);
        }

        public void RefreshSmartGiftBagGemItem(int freeGem, bool freeGemCollected) {
            this.ChooseSmartGiftBagByGem(freeGem, freeGemCollected);
        }

        public void RefreshSmartGiftBagHeroItem(HeroItem heroItem) {
            ChooseSmartGiftBagByHero(heroItem);
        }

        #endregion

        private void SetGemItem() {
            this.viewPref.txtDis.text = LocalManager.GetValue(pduSelfName.id);
            this.viewPref.imgDimPdu.sprite =
                ArtPrefabConf.GetSprite(this.pduSelfName.id + "_icon");
            this.viewPref.btnPay.onClick.AddListener(OnClickGemBuy);
            this.viewPref.txtAmount.text = "￥" + this.pduSelfName.price;
            if (GemProductConf.GetConf(this.pduSelfName.id).extraGem != "0") {
                this.viewPref.txtDimPduNum.text =
                    string.Concat(GemProductConf.GetConf(this.pduSelfName.id).basicGem
                     , " <color=#67d632><size=25>+",
                        GemProductConf.GetConf(this.pduSelfName.id).extraGem);
            } else {
                this.viewPref.txtDimPduNum.text =
                      GemProductConf.GetConf(this.pduSelfName.id).basicGem;
            }
        }

        public void RefreshDailyGiftItem() {
            this.SetDailyGiftItem();
        }

        public void SetDailyGiftCanBuy() {
            this.viewPref.pnlFirstTopup.SetActiveSafe(true);
            this.viewPref.pnlAmount.SetActiveSafe(true);
            this.viewPref.pnlSellOut.SetActiveSafe(false);
            // this.viewPref.pnlBtnPay.SetActiveSafe(true);
        }


        public void SetDailyGiftCantBuy() {
            this.viewPref.pnlFirstTopup.SetActiveSafe(true);
            this.viewPref.pnlAmount.SetActiveSafe(false);
            this.viewPref.pnlSellOut.SetActiveSafe(true);
            // this.viewPref.pnlBtnPay.SetActiveSafe(false);
        }

        private void SetDailyGiftItem() {
            this.viewPref.txtDis.text = LocalManager.GetValue(this.pduSelfName.id);
            this.viewPref.imgNum.SetActiveSafe(false);
            this.viewPref.txtAmount.text = "￥" + this.pduSelfName.price;
            this.viewPref.txtChestNum.text
                = string.Concat("X ", DailyGiftConf.GetConf(pduSelfName.id).ChestAmount);
            this.viewPref.txtFBPduNum.text =
                DailyGiftConf.GetConf(pduSelfName.id).GenAmount;
            this.viewPref.pnlDiamond.SetActiveSafe(false);
            this.viewPref.pnlFirstTopup.SetActiveSafe(true);
            //this.viewPref.imgFTPdu.sprite =
            //   ArtPrefabConf.GetSprite("normal_gem_1200" + "_icon");
            int count = this.LotteryDict.Count;
            if (this.LotteryDict != null && count > 0) {
                this.viewPref.imgChest.gameObject.SetActiveSafe(true);
                GachaGroupConf lotteryConf = this.LotteryDict[count - 1];
                this.viewPref.imgChest.sprite =
                     ArtPrefabConf.GetChestSprite(lotteryConf.chest);
            } else {
                this.viewPref.imgChest.gameObject.SetActiveSafe(false);
            }
            this.viewPref.btnPay.onClick.AddListener(OnClickDailyGift);
        }

        private void SetGemExchangeGoldItem() {
            this.viewPref.txtDis.text = LocalManager.GetValue(goldSelfName.id);
            this.viewPref.imgDimPdu.sprite =
               ArtPrefabConf.GetSprite(this.goldSelfName.id + "_icon");
            this.viewPref.pnlGemAmount.SetActiveSafe(true);
            this.viewPref.pnlAmount.SetActiveSafe(false);
            this.viewPref.txtGemAmount.text = this.goldSelfName.gemPrice;
            this.viewPref.txtDimPduNum.text = "<color=#FFF934>" + this.goldSelfName.goldAmount;
            this.viewPref.btnPay.onClick.AddListener(this.OnClickGemExchangeGold);
            StartCoroutine(RestartHLG(this.viewPref.pnlGemAmount.GetComponent<HorizontalLayoutGroup>()));
        }
        private IEnumerator RestartHLG(HorizontalLayoutGroup HLG) {
            yield return new WaitForFixedUpdate();
            HLG.enabled = false;
            HLG.enabled = true;
        }

        private void SetmonthCardItem() {
            this.monthCardViewPref.PnlGet.SetActiveSafe(false);
            MonthCardConf monthCardConf = MonthCardConf.GetConf(this.pduSelfName.id);

            this.monthCardViewPref.txtbuy.text = string.Concat("￥ ", pduSelfName.price);
            this.monthCardViewPref.txtRenew.text = string.Concat("￥ ", pduSelfName.price);
            this.monthCardViewPref.txtDirectGemAmount.text = monthCardConf.directGemAmount;
            this.monthCardViewPref.txtDailyGemAmount.text = monthCardConf.dailyGemAmount;
            this.monthCardViewPref.txtChestFreeAmount.text = string.Concat("X " + monthCardConf.chestFreeAmount);
            this.monthCardViewPref.txtGetChestFreeAmount.text = "X " + monthCardConf.chestFreeAmount;
            this.monthCardViewPref.txtGetDailyGemAmount.text = monthCardConf.dailyGemAmount;
            this.monthCardViewPref.btnBuy.onClick.AddListener(this.Buy);
            this.monthCardViewPref.btnGet.onClick.AddListener(this.OnClickGetmonthCard);
            this.monthCardViewPref.btnRenew.onClick.AddListener(this.Buy);
            EventManager.AddEventAction(Event.MonthCardDaily, this.UpdateRefreshMouthCardItem);
        }

        public void RefreshMouthCardItem
            (string productId, long ExpiredTime, bool isReward, bool noMonthCard = false) {
            if (noMonthCard) {
                this.monthCardViewPref.PnlGet.SetActiveSafe(false);
            } else {
                this.monthCardViewPref.PnlGet.SetActiveSafe(true);
                EventManager.AddEventAction(Event.MonthCard, this.UpdateMonthCard);

                expiredTime = ExpiredTime;
                if (isReward) {
                    this.monthCardViewPref.btnGet.gameObject.SetActiveSafe(true);
                    this.monthCardViewPref.btnRenew.gameObject.SetActiveSafe(false);
                } else {
                    this.monthCardViewPref.btnGet.gameObject.SetActiveSafe(false);
                    this.monthCardViewPref.btnRenew.gameObject.SetActiveSafe(true);
                }
            }
        }

        private void UpdateMonthCard(EventBase eventbase) {
            this.monthCardViewPref.txtTime.text
                = string.Format(LocalManager.GetValue(LocalHashConst.month_card_remaining),
                        GameHelper.TimeFormat
                            (eventbase.duration * 1000 - RoleManager.GetCurrentUtcTime()));
            if (eventbase.duration * 1000 < RoleManager.GetCurrentUtcTime()) {
                EventManager.RemoveEventAction(Event.MonthCard, this.UpdateMonthCard);
            }
        }

        public void SetMonthCardUpdate() {
            EventManager.AddEventAction(Event.MonthCardDaily, this.UpdateRefreshMouthCardItem);
        }

        private void UpdateRefreshMouthCardItem(EventBase eventbase) {
            long left = eventbase.duration * 1000 - RoleManager.GetCurrentUtcTime();
            if (left < 0) {
                EventManager.RemoveEventAction(Event.MonthCardDaily, this.UpdateRefreshMouthCardItem);
                this.RefreshMouthCardItem(this.pduSelfName.id, this.expiredTime, true);
                this.parent.SortVision(ProductType.MonthCard, 0);
            }
        }

        private void OnClickGemExchangeGold() {
            this.parent.PayCountersingShow(this.goldSelfName);
        }

        private void OnClickDailyGift() {
            int count = this.LotteryDict.Count;
            GachaGroupConf lotteryConf;
            if (this.LotteryDict != null && count > 0) {
                lotteryConf = this.LotteryDict[count - 1];
            } else {
                lotteryConf = null;
            }
            this.parent.DailyGiftShow(lotteryConf, this.pduSelfName, this);
        }

        private void OnClickGemBuy() {
            this.Buy();
        }

        private void OnClickGetmonthCard() {
            GetMonthCardRewardReq getmonthCardRewardReq = new GetMonthCardRewardReq() {
                MonthCardId = pduSelfName.id
            };
            NetManager.SendMessage(getmonthCardRewardReq,
               typeof(GetMonthCardRewardAck).Name, this.GetMonthCardRewardAck);
        }

        private void OnBtnSellOutClick() {
            this.parent.ShowPnlTip(this.isFree);
        }

        private void GetMonthCardRewardAck(IExtensible message) {
            GetMonthCardRewardAck getmonthCardRewardAck = message as GetMonthCardRewardAck;
            CommonReward reward = getmonthCardRewardAck.Reward;
            Debug.Log(reward.Currency.Gem);
            Protocol.Resources resource = getmonthCardRewardAck.Resources;
            Protocol.Currency currency = getmonthCardRewardAck.Currency;
            GameHelper.CollectResources(reward, resource, currency, this.resourceDict);
            if (reward.Chests.Count > 0) {
                // To do: change chest collect;
                //this.parent.GetChests(reward.Chests);
                GachaGroupConf lotterConf = GachaGroupConf.GetConf(reward.Chests[0].Name);
                HeroModel.AddlotteryChances(reward.Chests);
                GameHelper.ChestCollect(
                        this.transform.position,
                        lotterConf, CollectChestType.collectWithShow, null);
            }

            this.parent.SetPayBtn(false);
            this.monthCardViewPref.btnGet.gameObject.SetActiveSafe(false);
            this.monthCardViewPref.btnRenew.gameObject.SetActiveSafe(true);
            this.parent.SortVision(ProductType.MonthCard, 0);
            if (getmonthCardRewardAck.DailyExpiredAt * 1000 > RoleManager.GetCurrentUtcTime()) {
                EventManager.AddMonthCardDailyEvent(getmonthCardRewardAck.DailyExpiredAt);
                this.SetMonthCardUpdate();
            }
        }

        public void Buy() {
            BuyProductReq buyProductReq = new BuyProductReq() {
                ProductId = this.pduSelfName.id,
#if LONGYUAN
                Channel = 1 //龙渊
#elif GOOGLEPLAY
                Channel = 2 //googleplay
#endif
            };
            NetManager.SendMessage
                (buyProductReq, typeof(BuyProductAck).Name, this.BuyProductAck);
        }

        private void BuyProductAck(IExtensible message) {
            BuyProductAck buyProductAck = message as BuyProductAck;
            this.parent.Pay(
                this.pduSelfName.price, 
                RoleManager.GetRoleId(), 
                pduSelfName.id, 
                "1", buyProductAck.OrderId, 
                LocalManager.GetValue(this.pduSelfName.id)
            );
        }

        public void DoOrderConfirmReq() {
            OrderConfirmReq orderConfirmReq = new OrderConfirmReq();
            NetManager.SendMessage(orderConfirmReq,
                typeof(OrderConfirmAck).Name, this.OrderConfirmAck);
        }

        void OnApplicationFocus(bool hasFocus) {
            if (hasFocus) {
                if (this.isPay == true) {
                    OrderConfirmReq orderConfirmReq = new OrderConfirmReq();
                    NetManager.SendMessage(orderConfirmReq,
                        typeof(OrderConfirmAck).Name, this.OrderConfirmAck);
                    this.isPay = false;
                }
            }
        }

        private void OrderConfirmAck(IExtensible message) {
            OrderConfirmAck orderConfirmAck = message as OrderConfirmAck;
            List<OrderInfo> orderInfo = orderConfirmAck.OrderInfo;
            StartCoroutine(this.ShowAnimation(orderInfo));
            Debug.Log(orderConfirmAck.AllDone);
            foreach (OrderInfo orderInfoChild in orderConfirmAck.OrderInfo) {
                float price;
                if (float.TryParse(ProductConf.GetConf(orderInfoChild.ProductId).price, out price)) {
                    this.payModel.payAmount += price;
                }
            }
            if (!orderConfirmAck.AllDone) {
                StartCoroutine(this.SendMessageAgain());
            }
        }

        private IEnumerator SendMessageAgain() {
            yield return YieldManager.GetWaitForSeconds(PayViewModel.orderConfirmTime);
            OrderConfirmReq orderConfirmReq = new OrderConfirmReq();
            NetManager.SendMessage(orderConfirmReq,
                typeof(OrderConfirmAck).Name, this.OrderConfirmAck);
        }

        private IEnumerator ShowAnimation(List<OrderInfo> orderInfo) {
            OrderInfo order = null;
            int orderInfoCount = orderInfo.Count;
            for (int i = 0; i < orderInfoCount; i++) {
                order = orderInfo[i];
                if (order.ProductId.CustomEquals(GameConst.MONTH_CARD_1)) {
                    UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.unlock_build_queue_tips), TipType.Notice);
                    EventBuildClient.maxQueueCount = 2;
                    this.parent.SetPayBtn(true);
                    this.parent.GetStoreInfo();
                }

                GameHelper.CollectResources(order.Reward, order.Resources, order.Currency,
                    this.parent.IsVisible ? this.resourceDict : this.parent.ResourceDict);

                if (order.Reward.Chests.Count > 0) {
                    // To do: change chest collect;
                    GachaGroupConf lotterConf =
                        GachaGroupConf.GetConf(order.Reward.Chests[0].Name);
                    HeroModel.AddlotteryChances(orderInfo[i].Reward.Chests);
                    GameHelper.ChestCollect(
                        this.transform.position,
                        lotterConf, CollectChestType.collectWithShow, null);
                    this.parent.GetStoreInfo();
                }
                yield return YieldManager.GetWaitForSeconds(1f);
            }
        }
    }
}
