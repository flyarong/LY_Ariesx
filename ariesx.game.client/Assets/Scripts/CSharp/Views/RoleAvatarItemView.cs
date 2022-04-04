using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Protocol;
using UnityEngine.Events;

namespace Poukoute
{

    public class RoleAvatarItemView : MonoBehaviour
    {
        #region UI elements
        [SerializeField]
        private Button btnAvatar;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private Image ImgAvatarCurrent;
        #endregion

        public void SetRoleAvatar(Sprite avatarSprite,bool isCurrentAvatar,
        bool isUnlockAvatar,UnityAction callback){
            this.SetAvatarCurrentShow(isCurrentAvatar);
            if (avatarSprite != null)
            imgAvatar.sprite = avatarSprite;
            if (isUnlockAvatar)
                imgAvatar.material = null;
            else
                imgAvatar.material = PoolManager.GetMaterial(MaterialPath.matGray);
            btnAvatar.onClick.AddListener(callback);
        }

        public void SetAvatarCurrentShow(bool isCurrentAvatar) {
            ImgAvatarCurrent.gameObject.SetActiveSafe(isCurrentAvatar);
        }

        public Sprite GetAvatarSprite()
        {
            return imgAvatar.sprite;
        }
    }
}