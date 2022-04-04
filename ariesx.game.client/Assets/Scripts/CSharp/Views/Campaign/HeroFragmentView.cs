using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class HeroFragmentView: MonoBehaviour {

        #region UI element
        [SerializeField]
        private Image imgHeroAvatar;
        [SerializeField]
        private TextMeshProUGUI txtHeroFragmentCount;
        [SerializeField]
        private GameObject[] heroRarity;
        [SerializeField]
        private Button btnHero;       
        #endregion

        public UnityAction<string> OnHeroClick {
            get; set;
        }
        
        /*************************************************/
        public void SetContent(HeroFragment heroFragment = null) {
            this.imgHeroAvatar.sprite = ArtPrefabConf.GetSprite(heroFragment.Name + "_l");
            this.SetHeroRarityInfo(HeroAttributeConf.GetConf(heroFragment.Name).rarity);
            this.txtHeroFragmentCount.text = string.Concat("x", heroFragment.Count);
            this.btnHero.onClick.RemoveAllListeners();
            this.btnHero.onClick.AddListener(() => {
                this.OnHeroClick.InvokeSafe(heroFragment.Name);
            });
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
