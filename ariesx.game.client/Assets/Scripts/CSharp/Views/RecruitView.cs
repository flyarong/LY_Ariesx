using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class RecruitView : BaseView {
        private RecruitViewModel viewModel;
        private RecruitViewPreference viewPref;

        private bool isStrongHold = false;
        public bool needPlayResourceAnim = false;
        private Vector2 scrollRectOffSetMin = Vector2.zero;
        /*************/

        private Dictionary<string, RecruitItemView> recruitItemDict =
            new Dictionary<string, RecruitItemView>(6);
        private Dictionary<Resource, int> recruitResourceDict =
            new Dictionary<Resource, int>(5);

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRecruit");
            this.viewModel = this.gameObject.GetComponent<RecruitViewModel>();
            this.viewPref = this.ui.transform.GetComponent<RecruitViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnTreatmentAll.onClick.AddListener(this.OnBtnTreatmentAllClick);
        }

        public void SetContent(bool showTips) {
            this.isStrongHold = showTips;
            this.CreateListItem();
            this.viewPref.pnlTips.gameObject.SetActiveSafe(showTips);
            scrollRectOffSetMin = this.viewPref.srollRectTransform.offsetMin;
            scrollRectOffSetMin.y = showTips ? 345f : 287f;
            this.viewPref.srollRectTransform.offsetMin = scrollRectOffSetMin;
            int positionCount = this.viewModel.Troop.Positions.Count;
            this.recruitItemDict.Clear();
            GameObject recruitItem;
            for (int i = 0; i < positionCount; i++) {
                recruitItem = this.viewPref.pnlList.GetChild(i).gameObject;
                RecruitItemView recruitItemView = recruitItem.GetComponent<RecruitItemView>();
                string heroName = this.viewModel.Troop.Positions[i].Name;
                Hero hero = this.viewModel.HeroDict[heroName];
                TroopModel troopModel = ModelManager.GetModelData<TroopModel>();
                this.viewPref.txtTitle.text = string.Concat(
                    "<color=#FBFA9Bff>", LocalManager.GetValue(LocalHashConst.troop),
                   GameHelper.GetBuildIndex(troopModel.GetHeroTroopName(hero.Name)),
                    "</color> ", LocalManager.GetValue(LocalHashConst.button_tile_recruit));
                string heroKey = hero.GetId();
                this.recruitItemDict.Add(heroKey, recruitItemView);
                float armyAmount = hero.ArmyAmount;
                recruitItemView.SetHero(
                    hero,
                    this.viewModel.RecruiteSpeed,
                    this.isStrongHold,
                    (value) => { this.OnArmyAmountChange(heroKey, value, armyAmount); }
                );
            }
            this.UpdateTreatmentResourceInfo();
            this.FormatView();
        }

        public void FormatView() {
            this.viewPref.contentSizeFitter.onSetLayoutVertical.AddListener(() => {
                GameHelper.anchoredInitPos.x =
                    this.viewPref.rectTransform.rect.width / 2;
                GameHelper.anchoredInitPos.y =
                    -this.viewPref.rectTransform.rect.height / 2;
                this.viewPref.rectTransform.anchoredPosition = GameHelper.anchoredInitPos;
                this.viewPref.verticalLayoutGroup.SetOriginal();
                this.viewPref.scrollRect.velocity = Vector2.zero;
            });
        }

        public void UpdateRecruitStatus(EventBase eventBase) {
            EventRecruitClient recruitEvent = eventBase as EventRecruitClient;
            HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
            Hero hero = heroModel.heroDict[recruitEvent.heroName];
            string heroKey = hero.GetId();
            RecruitItemView recruitItemView;
            if (this.recruitItemDict.TryGetValue(heroKey, out recruitItemView)) {
                long timeCost = (RoleManager.GetCurrentUtcTime() - eventBase.startTime);
                long timeLeft = eventBase.duration - timeCost;
                timeCost = timeCost > 0 ? timeCost : 0;
                if (eventBase.duration == 0) {
                    recruitItemView.Percent = 1;
                } else {
                    recruitItemView.Percent = timeCost / (float)eventBase.duration;
                }
                timeLeft = Mathf.RoundToInt(Mathf.Max(timeLeft, 0));
                this.recruitItemDict[heroKey].TimeLeft = GameHelper.TimeFormat(timeLeft);
            }
        }

        public void SetItemStatus(string heroName, bool isRecruting) {
            this.recruitItemDict[heroName].IsRecruiting = isRecruting;
        }

        public void RefreshRecruitItem(Hero hero) {
            string heroKey = hero.Name;
            RecruitItemView itemView;
            if (this.recruitItemDict.TryGetValue(heroKey, out itemView)) {
                float armyAmount = hero.ArmyAmount;
                itemView.SetHero(
                    hero,
                    this.viewModel.RecruiteSpeed,
                    this.isStrongHold,
                    (value) => { this.OnArmyAmountChange(heroKey, value, armyAmount); }
                );
            }
            this.UpdateTreatmentResourceInfo();
        }

        public void OnTreatmentResourceChange(Protocol.Resources resources,
            Currency currency, CommonReward resourceChange, bool isCollect) {
            RoleManager.Instance.NeedCurrencyAnimation = true;
            RoleManager.Instance.NeedResourceAnimation = true;
            if (this.IsVisible && this.needPlayResourceAnim) {
                Dictionary<Resource, int> changedResourcesDict =
                    resourceChange.GetRewardsDict();
                foreach (var pair in changedResourcesDict) {
                    if (needPlayResourceAnim) {
                        this.viewModel.CollectResource(pair.Key, pair.Value.Abs(),
                            this.recruitResourcesDict[pair.Key].position, isCollect);
                    }
                }
                this.needPlayResourceAnim = false;
            }

            RoleManager.SetResource(resources);
            RoleManager.SetCurrency(currency);
        }

        private void UpdateTreatmentResourceInfo() {
            bool hasRecruitEvent = false;
            this.recruitResourceDict.Clear();
            foreach (var army in this.viewModel.ArmyDict) {
                EventRecruitClient eventRecruit = EventManager.GetRecruitEventByHeroName(army.Key);
                if (eventRecruit != null) {
                    this.SetRecruitResourceDict(eventRecruit);
                    hasRecruitEvent = true;
                }
            }
            if (hasRecruitEvent) {
                this.SetTreatmentResource(recruitResourceDict);
            } else {
                this.ResetTreatmentResource();
            }
            this.UpdateBtnTreatmentAllGrayableInfo();
        }

        private void UpdateBtnTreatmentAllGrayableInfo() {
            bool isHeroCanRecruit = false;
            GameObject recruitItem;
            int positionCount = this.viewModel.Troop.Positions.Count;
            for (int i = 0; i < positionCount; i++) {
                recruitItem = this.viewPref.pnlList.GetChild(i).gameObject;
                RecruitItemView recruitItemView = recruitItem.GetComponent<RecruitItemView>();
                isHeroCanRecruit = recruitItemView.HeroRecruitable || isHeroCanRecruit;
            }
            this.viewPref.btnTreatmentAll.Grayable = !isHeroCanRecruit;
        }

        private void ResetTreatmentResource() {
            List<Resource> resourceList = this.viewModel.GetProduceResourceList();
            Dictionary<Resource, int> playerResource =
                new Dictionary<Resource, int>(resourceList.Count);
            foreach (var resource in resourceList) {
                playerResource[resource] = 0;
            }
            this.SetTreatmentResource(playerResource);
        }

        private void SetRecruitResourceDict(EventRecruitClient eventRecruit) {
            Dictionary<Resource, int> tmpResourceDict = eventRecruit.costedResources.GetResourceDict();
            foreach (var resource in tmpResourceDict) {
                if (this.recruitResourceDict.ContainsKey(resource.Key)) {
                    this.recruitResourceDict[resource.Key] += resource.Value;
                } else {
                    this.recruitResourceDict.Add(resource.Key, resource.Value);
                }
            }

            if (eventRecruit.costedCurrency.Gold != 0) {
                if (this.recruitResourceDict.ContainsKey(Resource.Gold)) {
                    this.recruitResourceDict[Resource.Gold] += eventRecruit.costedCurrency.Gold;
                } else {
                    this.recruitResourceDict.Add(Resource.Gold,
                    eventRecruit.costedCurrency.Gold);
                }
            }
        }


        private Dictionary<Resource, Transform> recruitResourcesDict =
            new Dictionary<Resource, Transform>(4);
        private void SetTreatmentResource(Dictionary<Resource, int> resourceDict) {
            int resourcesCount = resourceDict.Count;
            this.recruitResourcesDict.Clear();
            GameHelper.ResizeChildreCount(this.viewPref.pnlResource,
                resourcesCount, PrefabPath.pnlItemWithCountSmall);
            ItemWithCountView itemView = null;
            int index = 0;
            foreach (var pair in resourceDict) {
                itemView = this.viewPref.pnlResource.GetChild(index++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(pair.Key, pair.Value, false);
                this.recruitResourcesDict.Add(pair.Key, itemView.imgItem.transform);
            }
        }

        private void CreateListItem() {
            int existCount = this.viewPref.pnlList.childCount;
            int left = this.viewModel.Troop.Positions.Count - existCount;
            if (left > 0) {
                for (int i = 0; i < left; i++) {
                    GameObject recruitItem = PoolManager.GetObject(
                                    PrefabPath.pnlRecruitItem, this.viewPref.pnlList);
                    RecruitItemView recruitItemView = recruitItem.GetComponent<RecruitItemView>();
                    recruitItemView.OnRecruit = this.viewModel.RecruitMaxReq;
                    recruitItemView.OnCancel = this.OnCancelRecruit;
                }
            } else {
                for (int i = 0; i < -left; i++) {
                    PoolManager.RemoveObject(this.viewPref.pnlList.GetChild(0).gameObject);
                }
            }
        }

        private void OnCancelRecruit(string heroName) {
            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.cancel_recruit),
                LocalManager.GetValue(LocalHashConst.cancel_recruit_confirm),
                () => this.viewModel.RecruitCancelReq(heroName),
                () => { }
            );
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnBtnTreatmentAllClick() {
            if (this.viewPref.btnTreatmentAll.Grayable) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_hero_army_amount_maximum),
                    TipType.Info);
            } else {
                this.viewModel.TreatmentTroopAllHeroes();
            }
        }

        /* Propert change function */
        private void OnArmyAmountChange(string heroKey, float value, float originValue) {
            this.viewModel.ChangeResource(heroKey, value - originValue);
        }
        /***************************/

        #region FTE

        public void OnRecruitStep3Start() {
            RecruitItemView item;
            if (this.recruitItemDict.TryGetValue(FteManager.GetCurHero(), out item)) {
                item.OnRecruitStep3Start();
            } else {
                FteManager.StopFte();
            }
        }

        #endregion

        protected override void OnVisible() {
            this.viewModel.OnVisible();
            needPlayResourceAnim = false;
            this.recruitResourceDict.Clear();
        }

        protected override void OnInvisible() {
            this.viewModel.OnInvisible();
            needPlayResourceAnim = false;
        }
    }
}
