using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapHeroHeadView : MonoBehaviour {
        //HeroModel heroModel;

        [Tooltip("Troop.Amount")]
        public Transform pnlAmount;
        [Tooltip("Troop.Amount")]
        public SpriteRenderer spriteAmount;
        [Tooltip("Troop.Amount.Text")]
        public TextMeshPro txtAmount;
        [Tooltip("Troop")]
        public SpriteRenderer spriteBackground;
        [Tooltip("Troop.Avatar")]
        public SpriteRenderer spriteAvatar;
        [Tooltip("Troop.Empty")]
        public GameObject spriteEmpty;

        //private void Awake() {
        //    this.heroModel = ModelManager.GetModelData<HeroModel>();
        //}

        public void SetHeroHead(TroopRelation relation, string avatarId, int amount) {
            string relationStr = GameHelper.LowerFirstCase(relation.ToString());
            spriteBackground.sprite = ArtPrefabConf.GetSprite(
                SpritePath.tileTroopRelationPrefix + relationStr
            );
            spriteAmount.sprite = ArtPrefabConf.GetSprite(
                SpritePath.tileTroopRelationAmountPrefix + relationStr
            );
            if (relation == TroopRelation.Self) {
                bool isEmpty = string.IsNullOrEmpty(avatarId);
                this.spriteEmpty.gameObject.SetActive(isEmpty);
                if (!isEmpty) {
                    spriteAvatar.sprite = ArtPrefabConf.GetSprite(avatarId, SpritePath.heroAvatarSmallSuffix);
                } else {
                    spriteAvatar.sprite = ArtPrefabConf.GetSprite(SpritePath.heroAvatarPrefix + "default");
                }
            } else {
                string avatarName = PlayerHeroAvatarConf.GetMiniAvatarName(avatarId.ToString());
                spriteAvatar.sprite = ArtPrefabConf.GetRoleMiniAvatarSprite(avatarName);
            }
            this.pnlAmount.gameObject.SetActive(amount > 1);
            txtAmount.text = amount > 99 ? "99+" : amount.ToString();
        }
    }
}
