using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class ChatPrivateDetailViewPreference : BaseViewPreference {
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetail.PnlTitle.TxtName")]
        public TextMeshProUGUI txtName;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetail.PnlTitle.BtnBack")]
        public Button btnBack;

        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetail.PnlContent.ScrollView")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetail.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public CustomContentSizeFitter contentSizeFitter;
        public RectTransform rectTransform;
    }
}
