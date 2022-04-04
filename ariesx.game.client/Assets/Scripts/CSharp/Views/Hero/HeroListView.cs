using UnityEngine;
using System.Collections;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using UnityEngine.Events;

namespace Poukoute {
    public class HeroListView : SRIA<HeroListViewModel, HeroBigGroupView> {
        private HeroListViewPreference viewPref;
        //public float height;

        /**************************************************************************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHero.PnlHero.PnlContent.PnlHeroList");
            this.viewModel = this.gameObject.GetComponent<HeroListViewModel>();
            this.viewPref = this.ui.transform.GetComponent<HeroListViewPreference>();
            this.viewPref.btnOrder.onClick.AddListener(this.OnBtnSortClick);
            this.SRIAInit(this.viewPref.scrollRect, this.viewPref.verticalLayoutGroup,
                defaultItemSize: 290);

            this.viewPref.scrollRect.onBeginDrag.AddListener(this.DisableSelect);
        }

        #region GridAdapter implementation
        protected override HeroBigGroupView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlHeroBigGroup, this.viewPref.pnlList);
            HeroBigGroupView itemView = itemObj.GetComponent<HeroBigGroupView>();
            itemView.SetItemContent(itemIndex);
            itemView.OnHeroClick = this.OnHeroClicked;
            return itemView;
        }

        protected override void OnItemHeightChangedPreTwinPass(HeroBigGroupView itemView) {
            base.OnItemHeightChangedPreTwinPass(itemView);
            itemView.HasPendingVisualSizeChange = false;
        }

        protected override void UpdateViewsHolder(HeroBigGroupView itemView) {
            itemView.SetItemContent(itemView.ItemIndex);
            if (itemView.HasPendingVisualSizeChange) {
                base.UpdateItemsDesc(itemView.ItemIndex, itemView.RootPreferHeight);
            }
        }
        #endregion

        public void Show() {
            this.beforeShowCallback = () => {
                base.SetVirtualAbstractNormalizedScrollPosition(1f, false);
            };
            base.Show();
        }

        public void SetListInfo() {
            if (this.viewPref.pnlList.childCount > 0) {
                base.Refresh();
            } else {
                this.SetBtnSortTitle();
                int heroCount = this.viewModel.HeroDict.Count;
                if (heroCount < 1) {
                    this.viewPref.txtTotal.text = string.Empty;
                    return;
                }
                this.viewPref.txtTotal.text = string.Concat(
                    heroCount, "/", heroCount + this.viewModel.UnlockHeroDict.Count);
                base.ResetItems(this.viewModel.GetHeroRowlDataLength());
            }
        }

        private void OnHeroClicked(string heroName, Vector3 heroPos) {
            this.EnableSelect();
            this.viewModel.ReadHeroReq(heroName);
            Hero hero;
            HeroAttributeConf heroAttribute;
            this.viewPref.btnHeroDetail.onClick.RemoveAllListeners();
            if (this.viewModel.HeroDict.TryGetValue(heroName, out hero)) {
                this.viewPref.selectHeroHeadView.SetHero(hero, showLevel: false);
                this.SetHeroUpgrad(hero);
                this.viewPref.btnHeroDetail.onClick.AddListener(
                    () => {
                        this.DisableSelect();
                        this.viewModel.OnHeroClick(heroName);
                    });
            } else if (this.viewModel.UnlockHeroDict.TryGetValue(heroName, out heroAttribute)) {
                this.viewPref.pnlHeroUpgrad.gameObject.SetActiveSafe(false);
                this.viewPref.pnlHeroDetail.gameObject.SetActiveSafe(true);
                this.viewPref.selectHeroHeadView.SetUnlockHero(heroAttribute);
                this.viewPref.btnHeroDetail.onClick.AddListener(
                    () => {
                        this.DisableSelect();
                        this.viewModel.OnUnlockHeroClick(heroName);
                    });
            }
            this.viewPref.selectHeroHeadView.OnHeroClick.AddListener(() => {
                this.DisableSelect();
            });
            this.viewPref.selectedHeroRT.position = heroPos;
        }

        private void SetHeroUpgrad(Hero hero) {
            int fragmentCount = hero.FragmentCount;
            int heroFragments = HeroLevelConf.GetHeroUpgradFragments(hero);
            bool isReachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(hero);
            int heroUpgradCost = HeroLevelConf.GetHeroUpgradCost(hero);
            bool canUpgraded = (fragmentCount >= heroFragments) && !isReachMaxLevel;
            this.viewPref.pnlHeroUpgrad.gameObject.SetActiveSafe(canUpgraded);
            this.viewPref.pnlHeroDetail.gameObject.SetActiveSafe(!canUpgraded);
            if (canUpgraded) {
                this.viewPref.txtUpgradCost.text = GameHelper.GetFormatNum(heroUpgradCost);
            }
            this.viewPref.txtUpgradCost.color =
                (heroUpgradCost >= RoleManager.GetResource(Resource.Gold)) ?
                Color.red : Color.white;
        }

        private void EnableSelect() {
            UIManager.SetUICanvasGroupEnable(this.viewPref.selectedHero, true);
            this.viewPref.selectedHero.transform.SetParent(this.viewPref.pnlList);
        }

        private void DisableSelect() {
            if (this.viewPref.selectedHero.alpha == 1) {
                UIManager.SetUICanvasGroupEnable(this.viewPref.selectedHero, false);
                this.viewPref.selectedHero.transform.SetParent(this.viewPref.scrollRect.transform);
            }
        }


        public void RefreshHeroView(string hero) {
            HeroHeadView headView = this.GetHeroHeadView(hero);
            if (headView != null) {
                headView.SetHero(this.viewModel.HeroDict[hero], true, isShowNewMark: false);
            }
        }

        private HeroHeadView GetHeroHeadView(string heroName) {
            foreach (HeroBigGroupView heroBigGroupView in this._VisibleItems) {
                int groupCount = heroBigGroupView.heroHeadViewList.Length;
                for (int i = 0; i < groupCount; i++) {
                    HeroHeadView heroHeadView = heroBigGroupView.heroHeadViewList[i];
                    if (heroHeadView.HeroName.CustomEquals(heroName)) {
                        return heroHeadView;
                    }
                }
            }
            return null;
        }

        private void OnBtnSortClick() {
            this.DisableSelect();
            base.SetVirtualAbstractNormalizedScrollPosition(1f, false);
            switch (this.viewModel.HeroSortBy) {
                case HeroSortType.Level:
                    this.viewModel.HeroSortBy = HeroSortType.Rarity;
                    break;
                case HeroSortType.Rarity:
                    this.viewModel.HeroSortBy = HeroSortType.Power;
                    break;
                case HeroSortType.Power:
                    this.viewModel.HeroSortBy = HeroSortType.Level;
                    break;
                case HeroSortType.None:
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
            this.SetBtnSortTitle();
            base.Refresh();
        }

        private void SetBtnSortTitle() {
            switch (this.viewModel.HeroSortBy) {
                case HeroSortType.Level:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_level);
                    break;
                case HeroSortType.Rarity:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_rarity);
                    break;
                case HeroSortType.Power:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_power);
                    break;
                case HeroSortType.None:
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
        }

        public void SetScrollEnable(bool enable) {
            this.InitUI();
            this.viewPref.scrollRect.enabled = enable;
        }

        #region FTE

        public IEnumerator OnHeroStep1Process() {
            yield return YieldManager.EndOfFrame;
            bool canLevelUp = false;

            foreach (HeroBigGroupView heroBigGroupView in this._VisibleItems) {
                int groupCount = heroBigGroupView.heroHeadViewList.Length;
                for (int i = 0; i < groupCount; i++) {
                    HeroHeadView heroHeadView = heroBigGroupView.heroHeadViewList[i];
                    if (heroHeadView.canLevelUp) {
                        canLevelUp = true;
                        FteManager.SetCurHero(heroHeadView.HeroName);
                        FteManager.SetMask(heroHeadView.transform,
                            arrowParent: UIManager.GetUI("UIHero").transform, isEnforce: !FteManager.FteOver);
                        yield return null;
                    }
                }
            }

            if (!canLevelUp) {
                FteManager.StopFte();
                this.SetScrollEnable(true);
                this.viewModel.StartChapterDailyGuid();
            }
        }

        #endregion

        protected override void OnInvisible() {
            this.DisableSelect();
        }
    }
}
