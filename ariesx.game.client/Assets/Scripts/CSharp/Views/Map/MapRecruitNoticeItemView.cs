using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapRecruitNoticeItemView : MapNoticeItemView {
        #region ui element
        [SerializeField]
        private Image imgHero;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private Button btnItem;
        #endregion        

        public UnityEvent OnClick {
            get {
                this.btnItem.onClick.RemoveAllListeners();
                return this.btnItem.onClick;
            }
        }

        public void SetItemContent(Hero hero, bool isHeroLevelUp) {
            this.imgHero.sprite = ArtPrefabConf.GetSprite(hero.Name, SpritePath.heroAvatarSmallSuffix);
            this.txtName.text = HeroAttributeConf.GetLocalName(hero.GetId());
        }
    }
}