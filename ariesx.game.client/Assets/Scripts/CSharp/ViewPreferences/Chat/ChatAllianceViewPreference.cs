using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Poukoute {
    public class ChatAllianceViewPreference : BaseViewPreference {
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlAlliance CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlAlliance.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlAlliance.BtnFilter")]
        public Button btnFilter;
    }
}
