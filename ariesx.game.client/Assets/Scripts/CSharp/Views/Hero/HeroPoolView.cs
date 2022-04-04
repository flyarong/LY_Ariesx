using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Poukoute {
    public class HeroPoolView : BaseView {
        private HeroPoolViewModel viewModel;
        private HeroPoolViewPreference viewPref;

        private Dictionary<string, HeroHeadView> heroHeadViewDict =
            new Dictionary<string, HeroHeadView>();
        private List<string> diffQualityNum = new List<string>();

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HeroPoolViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHeroPool");

            this.viewPref = this.ui.transform.GetComponent<HeroPoolViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnInfo.onClick.AddListener(this.OnBtnInfoClick);
        }

        //public override void PlayShow() {
        //    base.PlayShow();
        //}

        //public override void PlayHide() {
        //    base.PlayHide(null);
        //}

        public void SetHeroPoolDetail() {
            int sepratorIndex = this.viewModel.CurrentGachaName.LastIndexOf("_") + 1;
            //string level = this.viewModel.CurrentGachaName.Substring(sepratorIndex);

            this.viewPref.txtChestName.text = string.Format(
                LocalManager.GetValue(LocalHashConst.hero_pool_content_top),
                LocalManager.GetValue(this.viewModel.CurGachaConf.chest)
            );
            this.SetListInfo();
            this.SetHeroCardsPoolInfo();
        }

        private string ChangeGachaGroupBy(int amount) {
            int sepratorIndex = this.viewModel.CurrentGachaName.LastIndexOf("_") + 1;
            string groupPreName = this.viewModel.CurrentGachaName.Substring(0, sepratorIndex);
            int groupIndex = int.Parse(this.viewModel.CurrentGachaName.Substring(sepratorIndex));
            return string.Concat(groupPreName, groupIndex + amount);
        }

        private void SetListInfo() {
            this.heroHeadViewDict.Clear();
            HeroAttributeConf gachaHero;
            int listCount = this.viewModel.CurGachaConf.gachaAllHeroes.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlList, listCount, PrefabPath.pnlHeroBig);
            for (int i = 0; i < listCount; i++) {
                gachaHero = HeroAttributeConf.GetConf(
                    this.viewModel.CurGachaConf.gachaAllHeroes[i]
                );
                Transform hero = this.viewPref.pnlList.GetChild(i);
                this.SetCellInfo(hero, gachaHero);
            }
            foreach(string hero in this.viewModel.CurGachaConf.joinAllHeroes) {
                if (this.viewModel.HeroDict.ContainsKey(hero)) {
                    gachaHero = HeroAttributeConf.GetConf(hero);
                    GameObject heroObj = PoolManager.GetObject(
                        PrefabPath.pnlHeroBig, 
                        this.viewPref.pnlList
                    );
                    this.SetCellInfo(heroObj.transform, gachaHero);
                }
            }
        }

        private void SetCellInfo(Transform cell, HeroAttributeConf heroAttribute) {
            HeroHeadView heroHead = cell.GetComponent<HeroHeadView>();
            Hero hero = new Hero() {
                Name = heroAttribute.name,
                Level = 1,
                ArmyAmount = heroAttribute.troopAmount
            };
            heroHead.SetHero(hero, showLevel: false, showStar: true, showPower: false, showHeroStatus: false);
            heroHead.AddListener(
                () => { this.OnHeroClick(hero.Name); }
            );

            this.heroHeadViewDict.Add(hero.Name, heroHead);
        }

        private void OnHeroClick(string heroName) {
            this.viewModel.OnHeroClick(heroName);
            if (this.isShowInfoPnl) {
                this.HideHeroPoolInfo();
            }
        }

        public void RefreshHeroView(string heroName) {
            this.heroHeadViewDict[heroName].SetHero(this.viewModel.HeroDict[heroName], true);
        }

        private void FormatGrid() {
            this.viewPref.pnlList.GetComponent<GridLayoutGroup>().CalculateLayoutInputHorizontal();
            this.viewPref.pnlList.GetComponent<GridLayoutGroup>().CalculateLayoutInputVertical();
            this.viewPref.pnlList.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            RectTransform rectTransform = this.viewPref.pnlList.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.rect.width / 2,
                -rectTransform.rect.height / 2
            );
        }

        private void SetHeroCardsPoolInfo() {
            GameHelper.ClearChildren(this.viewPref.pnlHeroCardList);
            GachaGroupConf lotteryConf = GachaGroupConf.GetConf(this.viewModel.CurGachaConf.chest);
            this.diffQualityNum = lotteryConf.diffQualityNum;
            int listCount = this.diffQualityNum.Count;
            GameObject cell;
            for (int i = 0; i < listCount; i++) {
                cell = PoolManager.GetObject(PrefabPath.pnlHeroCard, this.viewPref.pnlHeroCardList);
                cell.GetComponent<HeroCardsInfo>().SetHeroCardsInfo(this.diffQualityNum[i], i, listCount);
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
            if (this.isShowInfoPnl) {
                this.HideHeroPoolInfo();
            }
        }

        private bool isShowInfoPnl = false;
        private bool isPlayingAnimation = false;
        private void OnBtnInfoClick() {
            if (!this.isPlayingAnimation) {
                this.isPlayingAnimation = true;
                if (!this.isShowInfoPnl) {
                    AnimationManager.Animate(this.viewPref.heroPoolInfo, "Show",
                    () => {
                        this.isShowInfoPnl = true;
                        this.isPlayingAnimation = false;
                    });
                } else {
                    this.HideHeroPoolInfo();
                }
            }
        }

        private void HideHeroPoolInfo() {
            AnimationManager.Animate(this.viewPref.heroPoolInfo, "Hide",
                () => {
                    this.isShowInfoPnl = false;
                    this.isPlayingAnimation = false;
                });
        }
    }
}
