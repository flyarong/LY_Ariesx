using Protocol;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Poukoute {
    public class PayViewModel : BaseViewModel, IViewModel {
        private PayView view;
        private MapViewModel parent;
        private HeroModel heroModel;
        private PayModel model;
        private DailyGiftDetailsViewModel dailyGiftDetailsViewModel;
        private PayCountersignViewModel payContersignViewModel;
        private SmartGiftBagViewModel smartGiftBagViewModel;
        public const float orderConfirmTime = 5f;
        private Dictionary<string, int> stepDict = new Dictionary<string, int>();
        //private Dictionary<string, string> orderIdDict = new Dictionary<string, string>();
        public Dictionary<Resource, Transform> resourceDict = new Dictionary<Resource, Transform>();
        private Transform resourceTransform;
        string payItemName = "";
        string payMoney = "";

        public List<GachaGroupConf> LotteryList {
            get {
                return this.heroModel.lotteryList;
            }
        }

        public DailyShop DailyShop {
            get {
                return this.model.dailyShop;
            }
            set {
                this.model.dailyShop = value;
                this.SetViewDailyShow();
            }
        }

        public List<DailyGift> DailyGifts {
            get {
                return this.model.dailyGifts;
            }
            set {
                if (value != null && value.Count > 0) {
                    this.model.dailyGifts = value;
                    this.view.RefreshDailyGift();
                    this.SetViewDailyGifts();
                }
            }
        }

        public List<MonthCard> MonthCards {
            get {
                return this.model.monthCards;
            }
            set {
                if (value != null && value.Count > 0) {
                    this.model.monthCards = value;
                    this.SetViewMonthCardsInfo();
                }
            }
        }

        void Awake() {
            //ConfigureManager.Instance.LoadConfigure<ProductConf>("shop_product");
            //ConfigureManager.Instance.LoadConfigure<GemProductConf>("product_gem");
            //ConfigureManager.Instance.LoadConfigure<GoldProductConf>("product_gold");
            //ConfigureManager.Instance.LoadConfigure<DailyGiftConf>("daily_gift");
            //ConfigureManager.Instance.LoadConfigure<MonthCardConf>("month_card");
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<PayView>();
            this.model = ModelManager.GetModelData<PayModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.dailyGiftDetailsViewModel
                = PoolManager.GetObject<DailyGiftDetailsViewModel>(this.transform);
            this.payContersignViewModel
                = PoolManager.GetObject<PayCountersignViewModel>(this.transform);
            this.smartGiftBagViewModel
               = PoolManager.GetObject<SmartGiftBagViewModel>(this.transform);

            NetHandler.AddNtfHandler(typeof(FieldFirstDownNtf).Name, this.FieldFirstDownNtf);
            this.SetLotteryList();
            this.IsNeedCloseMonthCard();

            this.IsNeedCloseSmartGiftBag();
            this.GetStoreInfoForSmartGiftBag();

            StartCoroutine(this.GetOrderConfirm());
            EventManager.AddMonthCardEvent(this.model.expiredTime);
#if GOOGLEPLAY
            GooglePlay.self.PaySuccessEvent 
                += new GooglePlay.PaySuccessHandler( PaySuccess);
            GooglePlay.self.ConsumeSuccessEvent 
                += new GooglePlay.ConsumeSuccessHandler(OnConsumeSuccess);
#endif
        }

        public void RefreshSmartGiftBagHeroItem(HeroItem heroConf) {
            this.view.smartGiftBagViewList[heroConf.Fence - 1].RefreshSmartGiftBagHeroItem(heroConf);
        }

        private void FieldFirstDownNtf(IExtensible message) {
            this.SetLotteryList();
            this.view.RefreshDailyGiftItem();
        }

        public void Show(int index = 0) {
            if (!this.view.IsVisible) {
                this.view.PlayShow(() => {
                    this.parent.OnAddViewAboveMap(this);
                    this.GetStoreInfo();
                    this.view.Format();
                });
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide(
                    this.OnHideCallback
                   );
                this.view.SetPnlLoad(true);
                LYGameData.OnOtherCloseStore();
            }
            //this.view.ClearnList();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        private void OnHideCallback() {
            this.parent.OnRemoveViewAboveMap(this);
        }

        public void DailyGiftShow
            (GachaGroupConf lotteryConf,
            ProductConf productConf, PayPduItemView pduItem) {
            this.dailyGiftDetailsViewModel.Show(lotteryConf, productConf, pduItem);
        }

        public void PayCountersingShow(GoldProductConf goldProductConf) {
            this.payContersignViewModel.Show(goldProductConf);
        }

        public void ShowSmartGiftBag(HeroItem heroItem) {
            this.smartGiftBagViewModel.Show(heroItem);
        }

        public void DailyGiftHide() {
            this.dailyGiftDetailsViewModel.Hide();
        }

        public void ShowHeroPool(string groupName, string building = null) {
            this.parent.ShowHeroPool(groupName, building);
        }

        public void SetChangeGem(int gem) {
            this.parent.SetChangeGem(gem);
        }

        public void RefreshHouseKeeperEvent() {
            this.parent.RefreshHouseKeeperEvent();
        }


        public void Pay(string price, string roleId, string pduId,
                string channelId, string orderId, string pduName) {
            LYGameData.OnOtherClickBuyButton(pduName, "1", price);
            payItemName = pduName;
            payMoney = price;
#if LONGYUAN
            LySdk.Pay(price, roleId, pduId, channelId, orderId, pduName);
            this.view.pdu2lItemDict[pduId].GetComponent<PayPduItemView>().isPay = true;
#elif GOOGLEPLAY        
             GooglePlay.PAY(price, roleId, pduId, channelId, orderId, pduName);
#endif
        }

        private void IsNeedCloseSmartGiftBag() {
            if (RoleManager.GetNextZeroTime() - RoleManager.GetCurrentUtcTime() > 0) {
                EventManager.AddEventAction(Event.SmartGiftBag, this.UpdateSmartGiftBag);
            }
        }

        private void UpdateSmartGiftBag(EventBase eventBase) {
            long time = RoleManager.GetNextZeroTime() - RoleManager.GetCurrentUtcTime();
            time = (long)Mathf.Max(0, time);
            if (time == 0) {
                this.GetStoreInfoForSmartGiftBag();
                EventManager.RemoveEventAction(Event.SmartGiftBag, UpdateSmartGiftBag);
            }
        }

        public void GetStoreInfo() {
            GetStoreInfoReq getStoreInfoReq = new GetStoreInfoReq();
            NetManager.SendMessage(getStoreInfoReq,
                typeof(GetStoreInfoAck).Name, this.GetStoreInfoAck);
        }

        private void GetStoreInfoAck(IExtensible message) {
            GetStoreInfoAck getStoreInfoAck = message as GetStoreInfoAck;
            EventManager.AddSmartGiftBagEvent(getStoreInfoAck.DailyShop.ExpiredAt);

            this.DailyGifts = getStoreInfoAck.Gifts;
            this.MonthCards = getStoreInfoAck.MonthCards;
            this.DailyShop = getStoreInfoAck.DailyShop;
            this.view.FormatEveryTime();
            this.view.SetPnlLoad(false);
        }

        private void GetStoreInfoForSmartGiftBag() {
            GetStoreInfoReq getStoreInfoReq = new GetStoreInfoReq();
            NetManager.SendMessage(getStoreInfoReq,
                typeof(GetStoreInfoAck).Name, this.GetStoreInfoForSmartGiftBagAck);
        }

        private void GetStoreInfoForSmartGiftBagAck(IExtensible message) {
            GetStoreInfoAck getStoreInfoAck = message as GetStoreInfoAck;
            EventManager.AddSmartGiftBagEvent(getStoreInfoAck.DailyShop.ExpiredAt);
            DailyShop dailiShop =
            this.model.dailyShop = getStoreInfoAck.DailyShop;
            if ((dailiShop.FreeGemCollected == false && dailiShop.FreeGem != 0) ||
                (dailiShop.FreeGoldCollected == false && dailiShop.FreeGold != 0)) {
                this.parent.FullPnlStore();
            }
        }

        private void SetViewDailyGifts() {
            bool haveDailyGiftCanBuy = true;
            foreach (DailyGift dailyGift in this.DailyGifts) {
                if (dailyGift.IsBuy) {
                    haveDailyGiftCanBuy = false;
                    break;
                }
            }
            if (this.view.isInitUI) {
                this.view.SortVision(ProductType.DailyGift,
                    haveDailyGiftCanBuy ? 2 : 6);
            }
        }

        private void SetViewMonthCardsInfo() {
            int monthCardsCount = this.MonthCards.Count;
            MonthCard firstMonthCard = this.MonthCards[0];
            if (monthCardsCount > 0 &&
                firstMonthCard.ExpiredTime * 1000 > RoleManager.GetCurrentUtcTime()) {
                EventManager.AddMonthCardEvent(firstMonthCard.ExpiredTime);
                this.view.RefreshMouthCardItem(firstMonthCard.MonthCardId,
                    firstMonthCard.ExpiredTime, firstMonthCard.IsReward);
            } else {
                this.view.RefreshMouthCardItem(string.Empty, 1, false, noMonthCard: true);
            }
            this.view.SortVision(ProductType.MonthCard, 0);
            if (monthCardsCount > 0 &&
                firstMonthCard.DailyExpiredAt * 1000 > RoleManager.GetCurrentUtcTime()) {
                EventManager.AddMonthCardDailyEvent(firstMonthCard.DailyExpiredAt);
                this.view.SetMonthCardUpdate();
            }
        }

        private void SetViewDailyShow() {
            this.view.RefreshSmartGiftBagItem();
            bool freeSmartGiftBag = false;
            if (this.DailyShop.FreeGemCollected ||
                this.DailyShop.FreeGoldCollected) {
                for (int i = 0; i < this.DailyShop.HeroItem.Count; i++) {
                    Debug.Log(this.DailyShop.HeroItem[i].IsCollect);
                    if (this.DailyShop.HeroItem[i].IsCollect) {
                        freeSmartGiftBag = false;
                    } else {
                        freeSmartGiftBag = true;
                        break;
                    }
                }
            }

            this.view.SortVision(ProductType.SmartGiftBag,
                freeSmartGiftBag ? 3 : 7);
        }


        private void IsNeedCloseMonthCard() {
            if (this.model.haveMonthcard
                    && this.model.expiredTime * 1000
                        > RoleManager.GetCurrentUtcTime()) {
                EventBuildClient.maxQueueCount = 2;
                if (this.model.canmonthCardReward) {
                    this.parent.SetPayBtn(true);
                }
                if ((this.model.expiredTime * 1000
                    - RoleManager.GetCurrentUtcTime()) < GameConst.DAY_MILLION_SECONDS) {
                    EventManager.AddEventAction(Event.MonthCard, this.UpdateMonthCard);
                }
            }
        }

        private void UpdateMonthCard(EventBase eventBase) {
            Debug.Log(eventBase.duration * 1000 - RoleManager.GetCurrentUtcTime());
            if (eventBase.duration * 1000 < RoleManager.GetCurrentUtcTime()) {
                this.GetStoreInfo();
                this.GetMonthCardReq();
                this.parent.SetPayBtn(false);
                EventManager.RemoveEventAction(Event.MonthCard, UpdateMonthCard);
                if (this.view.isInitUI) {
                    this.view.SortVision(ProductType.MonthCard, 0);
                }
            }
        }

        private void GetMonthCardReq() {
            IsMonthCardExpiredReq isMonthCardExpireReq = new IsMonthCardExpiredReq();
            NetManager.SendMessage(isMonthCardExpireReq,
                typeof(IsMonthCardExpiredAck).Name, this.IsMonthCardExpiredAck);
        }

        public void IsMonthCardExpiredAck(IExtensible message) {
            IsMonthCardExpiredAck isMonthCardExpireAck = message as IsMonthCardExpiredAck;
            if (isMonthCardExpireAck.IsExpired) {
                EventBuildClient.maxQueueCount = 1;
            } else {
                EventBuildClient.maxQueueCount = 2;
            }
        }

        private void SetLotteryList() {
            int chestUpIndex = RoleManager.GetFDRecordMaxLevel() + 1;
            this.LotteryList.Clear();
            FirstDownRewardConf firstDownConf;
            for (int i = 1; i < chestUpIndex; i++) {
                firstDownConf = FirstDownRewardConf.GetConf(i.ToString());
                GachaGroupConf gachaConf = GachaGroupConf.GetConf(firstDownConf.unlockChest);
                this.LotteryList.Add(gachaConf);
            }

        }

        private IEnumerator GetOrderConfirm() {
            this.resourceDict.Add(Resource.Gem, this.parent.GetViewPrefPoint());
            yield return YieldManager.GetWaitForSeconds(4f);
#if GOOGLEPLAY
            GooglePlay.GetPurchases();
#endif
            OrderConfirmReq orderConfirmReq = new OrderConfirmReq();
            NetManager.SendMessage(orderConfirmReq,
                typeof(OrderConfirmAck).Name, this.OrderConfirmAck);
        }


        public void PaySuccess(object sender, PaySuccessArgument payArgument) {
            stepDict.Add(payArgument.OrderId, 1);
            OrderUpdateReq orderUpdateReq = new OrderUpdateReq() {
                Payload = payArgument.OrderId,
                Token = payArgument.Token
            };
            NetManager.SendMessage(orderUpdateReq,
                typeof(OrderUpdateAck).Name, this.OrderUpdateAck);
            LYGameData.OnOtherBuyItemSuccess(this.payItemName, "1", this.payMoney);
        }

        public void OnConsumeSuccess(object sender, ConsumeSuccessArgument conArgument) {
            OrderUpdateReq orderUpdateReq = new OrderUpdateReq() {
                Payload = conArgument.OrderId,
                Token = conArgument.Token
            };
            NetManager.SendMessage(orderUpdateReq,
                typeof(OrderUpdateAck).Name, this.OrderUpdateAck);
        }

        private void OrderUpdateAck(IExtensible message) {
            OrderUpdateAck orderUpdateAck = message as OrderUpdateAck;
            if (this.stepDict[orderUpdateAck.OrderId] == 1) {
                GooglePlay.ConsumePurchase(orderUpdateAck.Token, orderUpdateAck.OrderId);
                this.stepDict[orderUpdateAck.OrderId] = 2;
            } else if (this.stepDict[orderUpdateAck.OrderId] == 2) {
                this.view.pdu2lItemDict[orderUpdateAck.ProductId]
                    .GetComponent<PayPduItemView>().DoOrderConfirmReq();
            }
        }

        private IEnumerator ReLoginGetOrderConfirm() {
            yield return YieldManager.GetWaitForSeconds(PayViewModel.orderConfirmTime);
            OrderConfirmReq orderConfirmReq = new OrderConfirmReq();
            NetManager.SendMessage(orderConfirmReq,
                typeof(OrderConfirmAck).Name, this.OrderConfirmAck);
        }

        private void OrderConfirmAck(IExtensible message) {
            OrderConfirmAck orderConfirmAck = message as OrderConfirmAck;
            foreach (OrderInfo orderInfo in orderConfirmAck.OrderInfo) {
                float price;
                if (float.TryParse(ProductConf.GetConf(orderInfo.ProductId).price, out price)) {
                    this.model.payAmount += price;
                }
            }
            if (orderConfirmAck.OrderInfo.Count > 0) {
                StartCoroutine(this.ShowAnimation(orderConfirmAck.OrderInfo));
            }
            if (!orderConfirmAck.AllDone) {
                StartCoroutine(this.ReLoginGetOrderConfirm());
            }
        }

        private IEnumerator ShowAnimation(List<OrderInfo> orderInfo) {
            OrderInfo order = null;
            for (int i = 0; i < orderInfo.Count; i++) {
                order = orderInfo[i];
                if (orderInfo[i].Reward.Chests.Count > 0) {
                    // To do: change chest collect;
                    GachaGroupConf lotterConf = GachaGroupConf.GetConf(orderInfo[i].Reward.Chests[0].Name);
                    HeroModel.AddlotteryChances(orderInfo[i].Reward.Chests);
                    GameHelper.ChestCollect(
                        this.parent.GetViewPrefPoint().position,
                        lotterConf, CollectChestType.collectWithShow, null);
                }
                Debug.Log(orderInfo[i].ProductId);
                if (orderInfo[i].ProductId.CustomEquals(GameConst.MONTH_CARD_1)) {
                    UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.unlock_build_queue_tips), TipType.Notice);
                    EventBuildClient.maxQueueCount = 2;
                    this.parent.SetPayBtn(true);
                }

                GameHelper.CollectResources(order.Reward, orderInfo[i].Resources,
                    orderInfo[i].Currency, this.resourceDict);
                yield return YieldManager.GetWaitForSeconds(1f);
            }
        }

        public void SetPayBtn(bool canReward) {
            this.parent.SetPayBtn(canReward);
        }

        protected override void OnReLogin() {
            base.OnReLogin();
            StopAllCoroutines();
            StartCoroutine(this.ReLoginGetOrderConfirm());
        }
    }
}
