using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class RoundHeroView : MonoBehaviour {
        [SerializeField]
        private Image imgHeroAvatar;
        [SerializeField]
        private TextMeshProUGUI txtHeroName;
        [SerializeField]
        private TextMeshProUGUI txtTroopAmount;
        [SerializeField]
        private Slider sliTroopAmount;

        public void SetContent(Hero hero) {
            this.imgHeroAvatar.sprite = 
                ArtPrefabConf.GetSprite(SpritePath.heroAvatarPrefix, hero.Name);
            this.txtHeroName.text = LocalManager.GetValue(hero.Name);
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(hero.Name);
            int heroMaxArmyAmount = 
                heroAttribute.GetAttribute(hero.Level, HeroAttribute.ArmyAmount);
            this.txtTroopAmount.text = string.Concat(hero.ArmyAmount, "/", heroMaxArmyAmount);
            this.sliTroopAmount.maxValue = heroMaxArmyAmount;
            this.sliTroopAmount.value = hero.ArmyAmount;
        }
    }
}
