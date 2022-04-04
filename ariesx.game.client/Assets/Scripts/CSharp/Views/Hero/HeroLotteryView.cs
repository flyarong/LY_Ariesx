using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class HeroLotteryView : BaseView {
        private HeroLotteryViewModel viewModel;
        private HeroLotteryViewPreference viewPref;

        private Dictionary<string, LotteryItemView> lotteryItemViewDict =
            new Dictionary<string, LotteryItemView>();

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHero.PnlHero.PnlContent.PnlLotteryList");
            this.viewModel = this.gameObject.GetComponent<HeroLotteryViewModel>();
            this.viewPref = this.ui.transform.GetComponent<HeroLotteryViewPreference>();
        }

        // To do: union lottery refresh.
        public void SetGridInfo() {
            if (!this.IsVisible) {
                return;
            }
            GameHelper.ClearChildren(this.viewPref.grdLottery);
            List<Chest> freeLotteryChanceList = new List<Chest>(5);
            List<Chest> guideLotteryChanceList = new List<Chest>(5);
            List<Chest> moneyChestList = new List<Chest>(5);
            List<Chest> goldenChestList = new List<Chest>(5);
            int lotteryChanceCount = 0;
            this.SortLotteryChanceList(
                ref freeLotteryChanceList, ref guideLotteryChanceList, ref moneyChestList,
                ref goldenChestList);

            int count = this.viewModel.LotteryChanceList.Count;
            for (int i = 0; i < count; i++) {
                lotteryChanceCount += this.viewModel.LotteryChanceList[i].Count;
            }

            int tmmCount = 0;
            for (int i = 0; i < freeLotteryChanceList.Count; i++) {
                tmmCount += freeLotteryChanceList[i].Count;
            }

            int freeLotteryChanceListCount = freeLotteryChanceList.Count;
            if (freeLotteryChanceListCount > 0) {
                this.SetGridItem(
                    GachaGroupConf.GetConf(freeLotteryChanceList[freeLotteryChanceListCount - 1].Name),
                    true, tmmCount
                );
            }

            Chest chest;
            int guideLotteryChanceListCount = guideLotteryChanceList.Count;
            for (int i = 0; i < guideLotteryChanceListCount; i++) {
                chest = guideLotteryChanceList[i];
                this.SetGridItem(
                    GachaGroupConf.GetConf(chest.Name), true, chest.Count);
            }

            tmmCount = 0;
            int moneyChestListCount = moneyChestList.Count;
            for (int i = 0; i < moneyChestListCount; i++) {
                tmmCount += moneyChestList[i].Count;
            }
            if (moneyChestListCount > 0) {
                this.SetGridItem(GachaGroupConf.GetConf(
                    moneyChestList[moneyChestListCount - 1].Name),
                    true, tmmCount, canOpen: true);
            }

            int goldCount = 0;
            int goldenChestListCount = goldenChestList.Count;
            for (int i = 0; i < goldenChestListCount; i++) {
                goldCount += goldenChestList[i].Count;
            }
            if (goldenChestListCount > 0) {
                this.SetGridItem(GachaGroupConf.GetConf(
                    goldenChestList[goldenChestListCount - 1].Name),
                    true, goldCount, canOpen: true);
            }

            // Set normal lottery.
            count = this.viewModel.LotteryList.Count;
            if (this.viewModel.LotteryList != null && count > 0) {
                this.SetGridItem(this.viewModel.LotteryList[count - 1]);
            }

            this.SetLotteryLimitTip();
            this.Format();
        }

        public void OnCurrentChange() {
            float cash = RoleManager.GetResource(Resource.Gem);
            foreach (LotteryItemView lottery in this.lotteryItemViewDict.Values) {
                lottery.RefreshPrice(cash);
            }
        }

        public void SetLotteryLimitTip() {
            if (this.viewModel.DailyLimit.ChestCurrent < this.viewModel.DailyLimit.ChestLimit) {
                this.viewPref.txtTip.text = string.Concat(
                    LocalManager.GetValue(LocalHashConst.free_lottery_get),
                    this.viewModel.DailyLimit.ChestCurrent, "/",
                    this.viewModel.DailyLimit.ChestLimit
                );
            } else {
                if (EventManager.EventDict.ContainsKey(Event.DailyReward)) {
                    EventBase eventBase;
                    if (EventManager.EventDict[Event.DailyReward].TryGetValue(
                        typeof(EventDailyRewardClient).Name, out eventBase)) {
                        this.viewPref.txtTip.text = string.Format(
                            LocalManager.GetValue(LocalHashConst.free_lottery_refresh),
                            string.Concat(
                                "<color=#FF0000FF>",
                                GameHelper.TimeFormatWithOneChar(
                                    eventBase.startTime + 
                                    eventBase.duration - RoleManager.GetCurrentUtcTime()
                                ),
                                "</color>"
                            )
                        );
                    }
                }
            }
        }

        private void SortLotteryChanceList(
           ref List<Chest> freeList, ref List<Chest> guideList, ref List<Chest> moneyList,
           ref List<Chest> goldenList) {

            List<Chest> lotteryChanceList = this.viewModel.LotteryChanceList;
            List<Chest> tmpNewList = new List<Chest>();
            List<Chest> tmpFreeList = new List<Chest>();
            List<Chest> tmpMoneyList = new List<Chest>();
            List<Chest> tmpGoldenList = new List<Chest>();
            Chest chest;
            int count = lotteryChanceList.Count;
            for (int i = 0; i < count; i++) {
                chest = this.viewModel.LotteryChanceList[i];
                if (chest.Name.StartsWith("guide_")) {
                    guideList.Add(chest);
                } else {
                    tmpNewList.Add(chest);
                }
            }

            count = tmpNewList.Count;
            for (int i = 0; i < tmpNewList.Count; i++) {
                chest = tmpNewList[i];
                if (chest.Name.Contains("golden_chest_")) {
                    tmpGoldenList.Add(chest);
                } else if (chest.Name.Contains("_chest_")) {
                    tmpMoneyList.Add(chest);
                } else {
                    tmpFreeList.Add(chest);
                }
            }

            int lestChestIndex = -1;
            count = tmpFreeList.Count;
            Chest leastChest;
            for (int index = 0; index < count; index++) {
                leastChest = tmpFreeList[0];
                lestChestIndex = leastChest.Name.GetNumber();
                for (int i = 0; i < tmpFreeList.Count; i++) {
                    chest = tmpFreeList[i];
                    if (lestChestIndex > chest.Name.GetNumber()) {
                        leastChest = chest;
                    }
                }
                freeList.Add(leastChest);
                tmpFreeList.Remove(leastChest);
            }

            count = tmpMoneyList.Count;
            for (int index = 0; index < count; index++) {
                leastChest = tmpMoneyList[0];
                lestChestIndex = leastChest.Name.GetNumber();
                for (int i = 0; i < tmpMoneyList.Count; i++) {
                    chest = tmpMoneyList[i];
                    if (lestChestIndex > chest.Name.GetNumber()) {
                        leastChest = chest;
                    }
                }
                moneyList.Add(leastChest);
                tmpMoneyList.Remove(leastChest);
            }

            // int goldenChestIndex = -1;
            count = tmpGoldenList.Count;
            for (int i = 0; i < count; i++) {
                leastChest = tmpGoldenList[0];
                lestChestIndex = leastChest.Name.GetNumber();
                for (int j = 0; j < tmpGoldenList.Count; j++) {
                    chest = tmpGoldenList[j];
                    if (lestChestIndex > chest.Name.GetNumber()) {
                        leastChest = chest;
                    }
                }
                goldenList.Add(leastChest);
                tmpGoldenList.Remove(leastChest);
            }
        }

        private void SetGridItem(GachaGroupConf gachaConf,
            bool isFree = false, int count = 0, int discount = 1, bool canOpen = false) {
            GameObject lotteryItem =
                PoolManager.GetObject(PrefabPath.pnlLotteryItem, this.viewPref.grdLottery);
            LotteryItemView lotteryItemView = lotteryItem.GetComponent<LotteryItemView>();
            lotteryItemView.SetLotteryItem(gachaConf);
            lotteryItemView.IsFree = isFree;
            lotteryItemView.Count = count;
            string chest = gachaConf.chest;
            lotteryItemView.OnHeroPoolClick.AddListener(
                () => {
                    this.viewModel.LotteryGroup = chest;
                    this.OnBtnHeroPoolClick();
                });
            if (gachaConf.price > 0) {
                if (canOpen) {
                    lotteryItemView.OnHeroDetailClick.AddListener(() => {
                        this.viewModel.LotteryGroup = chest;
                        this.viewModel.OpenChestReq();
                    });
                } else {
                    lotteryItemView.OnHeroDetailClick.AddListener(
                        () => this.OnGridLotteryClick(chest));
                }
            } else {
                lotteryItemView.OnHeroDetailClick.AddListener(
                    () => {
                        this.viewModel.LotteryGroup = chest;
                        this.viewModel.OpenChestReq();
                    });
            }
            this.lotteryItemViewDict[gachaConf.chest] = lotteryItemView;
        }

        private void Format() {
            if (this.viewPref.grdLottery.childCount > 6) {
                this.viewPref.gridLayoutGroup.enabled = true;
                this.viewPref.contentSizeFitter.enabled = true;
                GameHelper.ForceLayout(this.viewPref.gridLayoutGroup);
                this.viewPref.contentSizeFitter.SetLayoutHorizontal();
                this.viewPref.contentSizeFitter.SetLayoutVertical();
                RectTransform gridLooteryRT = this.viewPref.grdLottery as RectTransform;
                gridLooteryRT.anchoredPosition = new Vector2(
                        gridLooteryRT.rect.width / 2,
                       -gridLooteryRT.rect.height / 2);
                this.viewPref.gridLayoutGroup.enabled = false;
                this.viewPref.contentSizeFitter.enabled = false;
            }
        }

        private void OnBtnHeroPoolClick() {
            this.viewModel.ShowHeroPool();
        }

        private void OnGridLotteryClick(string chest) {
            this.viewModel.LotteryGroup = chest;
            GachaGroupConf gachaConf = GachaGroupConf.GetConf(chest);
            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.button_confirm),
                string.Format(LocalManager.GetValue(LocalHashConst.confirm_buy_chest),
                gachaConf.price, LocalManager.GetValue(gachaConf.chest)),
                this.OnBtnLotteryClick, () => { });
        }

        // May cause package conflict when GetHeroesReq send.
        private void OnBtnLotteryClick() {
            if (!this.viewModel.IsOpeningChest() && !this.viewModel.LotteryReqing) {
                GachaGroupConf gachaConf = GachaGroupConf.GetConf(this.viewModel.LotteryGroup);
                float cash = RoleManager.GetResource(Resource.Gem);
                if (cash < gachaConf.price) {
                    UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.gem_short), TipType.Error);
                } else {
                    this.viewModel.LotterySingleReq();
                    UIManager.HideAlertPnl();
                }
            }
        }

        #region FTE

        public IEnumerator OnFteStep291Process() {
            yield return YieldManager.EndOfFrame;
            bool hasItem = false;
            foreach (var pair in this.lotteryItemViewDict) {
                if (pair.Value.GachaGroupConf.chest.Contains(GameConst.GIFT_GROUP_PREFIX)) {
                    FteManager.SetCurrentLotteryGroup(pair.Key);
                    FteManager.SetMask(
                        pair.Value.transform, offset: -Vector2.up * 350, rotation: 180,
                        arrowParent: UIManager.GetUI("UIHero.PnlHero").transform, isEnforce: true
                    );
                    hasItem = true;
                    break;
                }
            }
            if (!hasItem) {
                FteManager.StopFte();
            }
        }

        #endregion

    }
}
