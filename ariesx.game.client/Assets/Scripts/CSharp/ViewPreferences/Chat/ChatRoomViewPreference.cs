using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public class ChatRoomViewPreference : BaseViewPreference {
        [Tooltip("UIChatRoom.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIChatRoom.PnlChat")]
        public Transform pnlChat;
        [Tooltip("UIChatRoom.PnlChat.PnlTab")]
        public PanelNewTabsView tabView;
        [Tooltip("UIChatRoom.PnlChat.PnlBottom")]
        public Transform pnlBottom;
        [Tooltip("UIChatRoom.PnlChat.PnlBottom.IfInput")]
        public TMP_InputField ifInput;
        [Tooltip("UIChatRoom.PnlChat.PnlBottom.IfInput")]
        public RectTransform ifInputRT;
        [Tooltip("UIChatRoom.PnlChat.PnlBottom.PnlNew")]
        public Transform pnlNewPrivate;
        [Tooltip("UIChatRoom.PnlChat.PnlBottom.PnlNew.BtnSend")]
        public Button btnNewPrivate;
        [Tooltip("UIChatRoom.PnlChat.PnlTabMask")]
        public Transform pnlTabMask;
    }
}
