using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatStateViewPeference : BaseViewPreference {
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlState CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlState.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
    }
}
