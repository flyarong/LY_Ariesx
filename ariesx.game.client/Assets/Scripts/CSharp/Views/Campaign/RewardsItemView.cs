using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class RewardsItemView: MonoBehaviour {

        #region UI element
        [SerializeField]
        private Image imgHeroAvatar;
        [SerializeField]
        private TextMeshProUGUI txtHeroFragmentCount;
        [SerializeField]
        private GameObject[] heroRarity;
        [SerializeField]
        private Button btnHero;
        [SerializeField]
        private Image imgBG;
        [SerializeField]
        private Image imgResourceItem;
        [SerializeField]
        private TextMeshProUGUI txtResourcAmount;
        [SerializeField]
        private Transform pnlResourceInfo;
        [SerializeField]
        private Transform pnlFragment;
        #endregion

        public UnityAction<string> OnHeroClick {
            get; set;
        }

        Color imgBGisFragment = Color.clear;
        /*************************************************/
        public void SetContent(HeroFragment heroFragment = null,
            Resource type = Resource.Food, int amount = 0) {
            bool isFragment = (heroFragment != null);
            if (this.pnlFragment != null) {
                this.pnlFragment.gameObject.SetActiveSafe(isFragment);
                this.pnlResourceInfo.gameObject.SetActiveSafe(!isFragment);
                this.imgBG.color = imgBGisFragment;
            }
            if (heroFragment != null) {
                this.imgHeroAvatar.sprite = ArtPrefabConf.GetSprite(heroFragment.Name + "_l");
                this.SetHeroRarityInfo(HeroAttributeConf.GetConf(heroFragment.Name).rarity);
                this.txtHeroFragmentCount.text = string.Concat("x", heroFragment.Count);
                this.btnHero.onClick.RemoveAllListeners();
                this.btnHero.onClick.AddListener(() => {
                    this.OnHeroClick.InvokeSafe(heroFragment.Name);                    
                });
            } else {
                this.txtResourcAmount.text = (amount == 0) ?
                    "--" : GameHelper.GetFormatNum(amount);
                this.imgResourceItem.sprite = ArtPrefabConf.GetSprite(
                     SpritePath.resourceIconPrefix, type.ToString().ToLower());
            }
        }
        
        private void SetHeroRarityInfo(int rarity) {
            int i = 0;
            for (; i < rarity; i++) {
                this.heroRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.heroRarity.Length;
            for (; i < rarityCount; i++) {
                this.heroRarity[i].SetActiveSafe(false);
            }
        }
    }
}
