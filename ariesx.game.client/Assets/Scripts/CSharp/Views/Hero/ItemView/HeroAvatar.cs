using UnityEngine.UI;
using UnityEngine;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class HeroAvatar : MonoBehaviour {
        public Image imgHead;
        public Button btnInfo;

        public void SetHeroAvatar(Hero hero, UnityAction onHeadClick = null) {
            string heroName = hero != null ? hero.Name : string.Empty;
            this.InnerSetHeroAvatar(heroName, onHeadClick);
        }

        public void SetHeroAvatar(Battle.Hero hero, UnityAction onHeadClick = null) {
            string heroName = hero != null ? hero.Name : string.Empty;
            this.InnerSetHeroAvatar(heroName, onHeadClick);
        }

        private void InnerSetHeroAvatar(string heroName, UnityAction onHeadClick) {
            if (!heroName.CustomIsEmpty()) {
                //HeroAttributeConf heroConf = HeroAttributeConf.GetConf(heroName);
                string spriteName = heroName.Replace(" ", string.Empty);
                this.imgHead.sprite = ArtPrefabConf.GetSprite(
                    spriteName, SpritePath.heroAvatarLargeSuffix
                );
                if (onHeadClick != null) {
                    this.btnInfo.onClick.RemoveAllListeners();
                    this.btnInfo.onClick.AddListener(onHeadClick);
                }
            } else {
                this.imgHead.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.heroAvatarPrefix, "default");
            }
        }
    }
}
