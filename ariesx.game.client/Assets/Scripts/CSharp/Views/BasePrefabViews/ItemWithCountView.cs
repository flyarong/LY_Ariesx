using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class ItemWithCountView : BaseView {
        public TextMeshProUGUI amount;
        public Image imgBG;
        public Image imgItem;
        [SerializeField]
        private Button btnItem;

        /**********************************/
        public void SetResourceInfo(ItemType type) {
            this.amount.text = this.GetRewardItemTypeLocal(type);
            this.imgItem.sprite = ArtPrefabConf.GetSprite(
                    SpritePath.itemType, type.ToString());

            string imgBgPathSuffix = "others";
            switch (type) {
                case ItemType.gem:
                case ItemType.fragment:
                    imgBgPathSuffix = type.ToString().ToLower();
                    break;
                default:
                    break;
            }
            if (imgBG != null) {
                this.imgBG.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.itemTypeBg, imgBgPathSuffix);
            }
        }

        public void SetHeroFragmentsClickInfo(HeroFragment fragment,
            UnityAction<string> OnItemClick) {
            this.SetResourceInfo(Resource.Fragment, fragment.Count);
            this.btnItem.onClick.RemoveAllListeners();
            this.btnItem.onClick.AddListener(() => {
                OnItemClick.InvokeSafe(fragment.Name);
            });
        }

        private string GetRewardItemTypeLocal(ItemType type) {
            switch (type) {
                case ItemType.chest:
                    return LocalManager.GetValue(LocalHashConst.title_treasure_chest);
                case ItemType.fragment:
                    return LocalManager.GetValue(LocalHashConst.hero_fragment);
                case ItemType.gem:
                    return LocalManager.GetValue(LocalHashConst.resource_gem);
                case ItemType.gold:
                    return LocalManager.GetValue(LocalHashConst.resource_gold);
                case ItemType.resource:
                    return LocalManager.GetValue(LocalHashConst.resouce);
                default:
                    return string.Empty;
            }
        }

        public void SetResourceInfo(Resource type, int amount, bool isShowBG = true) {
            //Debug.LogError(type.ToString());
            this.amount.text = (amount == 0) ?
                "--" : GameHelper.GetFormatNum(amount);
            this.imgItem.sprite = ArtPrefabConf.GetSprite(
                    SpritePath.resourceIconPrefix, type.ToString().ToLower());
            if (this.imgBG != null) {
                this.imgBG.enabled = isShowBG;
            }
            if (isShowBG) {
                this.SetResourceBg(type);
            }

        }

        private void SetResourceBg(Resource type) {
            string imgBgPathSuffix = "others";
            switch (type) {
                case Resource.Fragment:
                case Resource.Gem:
                    imgBgPathSuffix = type.ToString().ToLower();
                    break;
                default:
                    break;
            }
            if (imgBG != null) {
                this.imgBG.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.itemTypeBg, imgBgPathSuffix);
            }
        }

        public void SetResourceInfo() {
            this.amount.text = "1";
            this.imgItem.sprite = ArtPrefabConf.GetSprite("battle_report_lottery");
        }

        public void SetResourceInfo(string content, bool isRandomChest = false) {
            this.amount.text = content;
            if (isRandomChest)
                this.imgItem.sprite = ArtPrefabConf.GetSprite("battle_report_chest");
            else
                this.imgItem.sprite = ArtPrefabConf.GetSprite("battle_report_lottery");
        }
    }
}
