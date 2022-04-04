using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class HeroBigGroupView : BaseItemViewsHolder {
        [HideInInspector]
        public float Height { get; set; }
        #region  ui element
        //[SerializeField,Tooltip("$PnlHeroBigGroup")]
        //private ContentSizeFitter contentSizeFitter;
        //[SerializeField, Tooltip("$PnlHeroBigGroup.PnlHeroBigs")]
        //private GameObject heroBigsGroup;
        //[SerializeField, Tooltip("$PnlHeroBigGroup.PnlHeroBigs")]
        //private GridLayoutGroup heroBigsCGLG;
        //[SerializeField, Tooltip("$PnlHeroBigGroup.PnlHeroBigs")]
        //private LayoutElement heroBigsLE;
        public HeroHeadView[] heroHeadViewList = new HeroHeadView[3];
        [SerializeField]
        private CanvasGroup[] heroHeadCanvasGroupList = new CanvasGroup[3];
        [SerializeField, Tooltip("$PnlHeroBigGroup.PnlSeparate")]
        private GameObject separate;
        #endregion

        public bool HasPendingVisualSizeChange { get; set; }
        public UnityAction<string, Vector3> OnHeroClick;
        //public UnityAction<string, Vector3> OnUnlockHeroClick;
        private HeroModel model;
        [HideInInspector]
        public float RootPreferHeight = 0f;

        // Methods 
        //public override void MarkForRebuild() {
        //    this.root.SetInsetAndSizeFromParentEdge(
        //        RectTransform.Edge.Bottom, 0, this.RootPreferHeight);
        //    base.MarkForRebuild();
        //}

        public void SetItemContent(int dataIndex) {
            this.model = ModelManager.GetModelData<HeroModel>();
            Dictionary<string, Hero> heroDict = this.model.heroDict;
            Dictionary<string, HeroAttributeConf> unlockHeroDict = this.model.unlockHeroDict;
            List<string> HeroNamesData = this.model.heroNamesList;
            string heroName = HeroNamesData[dataIndex * GameConst.HERO_ROW];
            if (heroName.CustomIsEmpty()) {
                this.SetItemViewDetail(false, 40f);
            } else if (heroDict.ContainsKey(heroName)) {
                this.SetHeroBigGroupContent(dataIndex);
            } else if (unlockHeroDict.ContainsKey(heroName)) {
                int unlockHeroDataIndex = dataIndex - this.model.UnlockHeroIndex;
                this.SetUnlockHeroGroupContent(unlockHeroDataIndex);
            } else {
                Debug.LogError("Your HeroNamesData error!!!");
            }
        }

        private void SetHeroBigGroupContent(int dataIndex) {
            this.SetItemViewDetail(true, 290f);

            List<Hero> heroList = this.model.GetHeroListOrderBy();
            int heroesCout = heroList.Count;
            int lastIndex = (dataIndex + 1) * GameConst.HERO_ROW;
            int itemIndex = 0;
            bool valideIndex = false;
            for (int i = dataIndex * GameConst.HERO_ROW; i < lastIndex; i++) {
                valideIndex = i < heroesCout;
                //this.heroHeadCanvasGroupList[itemIndex].gameObject.SetActiveSafe(valideIndex);
                UIManager.SetUICanvasGroupEnable(this.heroHeadCanvasGroupList[itemIndex], valideIndex);
                if (valideIndex) {
                    this.heroHeadViewList[itemIndex].SetHero(heroList[i], true, isShowNewMark: true);
                    string heroName = heroList[i].Name;
                    int index = itemIndex;
                    this.heroHeadViewList[itemIndex].OnHeroClick.AddListener(
                        () => this.OnHeroClick(heroName, this.heroHeadCanvasGroupList[index].transform.position));
                }
                itemIndex++;
            }
        }

        private void SetUnlockHeroGroupContent(int dataIndex) {
            this.SetItemViewDetail(true, 256f);

            List<HeroAttributeConf> unlockHeroes = this.model.unlockHeroList;
            int unlockHeroCout = unlockHeroes.Count;
            int lastIndex = (dataIndex + 1) * GameConst.HERO_ROW;
            int itemIndex = 0;
            bool valideIndex = false;
            for (int i = dataIndex * GameConst.HERO_ROW; i < lastIndex; i++) {
                valideIndex = i < unlockHeroCout;
                //this.heroHeadCanvasGroupList[itemIndex].gameObject.SetActiveSafe(valideIndex);
                UIManager.SetUICanvasGroupEnable(this.heroHeadCanvasGroupList[itemIndex], valideIndex);
                if (valideIndex) {
                    this.heroHeadViewList[itemIndex].SetUnlockHero(unlockHeroes[i]);
                    string heroName = unlockHeroes[i].name;
                    int index = itemIndex;
                    this.heroHeadViewList[itemIndex].OnHeroClick.AddListener(
                        () => this.OnHeroClick(heroName,
                        this.heroHeadCanvasGroupList[index].transform.position));
                }
                itemIndex++;
            }
        }

        private void SetItemViewDetail(bool isShowHero, float preferHeigh) {
            this.separate.SetActiveSafe(!isShowHero);
            if (!isShowHero) {
                for (int i = 0; i < 3; i++) {
                    UIManager.SetUICanvasGroupEnable(this.heroHeadCanvasGroupList[i], false);
                }
            }
            if (this.RootPreferHeight != preferHeigh) {
                this.HasPendingVisualSizeChange = true;
                this.RootPreferHeight = preferHeigh;
            }
        }
    }
}
