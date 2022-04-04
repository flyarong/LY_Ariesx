using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class RoleAvatarViewPreference : BaseViewPreference
    {
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlTitle.BtnClose")]
        public CustomButton btnClose;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.BtnChangeAvatar")]
        public CustomButton btnChangeAvatar;
        [Tooltip("UIRoleAvatar.BtnBackGround")]
        public CustomButton btnBackClose;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlDetail.ScrollViewAvatar.Viewport.ContentAvatar")]
        public GridLayoutGroup glgContentAvatar;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlDetail.PnlAvatarSelected")]
        public Button btnSelectedAvatar;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlDetail.PnlAvatarSelected.ImgAvatar")]
        public Image imgSelectedAvatar;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlDragHint")]
        public RectTransform pnlHint;
        [Tooltip("UIRoleAvatar.PnlRoleAvatar.PnlDragHint.TxtDes")]
        public TextMeshProUGUI txtHint;
    }
}
