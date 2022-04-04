using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatWorldViewPreference : BaseViewPreference {
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlWorldHoldler.PnlWorld CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlWorldHoldler.PnlWorld.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
    }
}
