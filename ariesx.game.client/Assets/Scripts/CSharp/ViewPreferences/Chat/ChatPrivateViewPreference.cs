using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatPrivateViewPreference : BaseViewPreference {
        [Tooltip("$PnlPrivate")]
        public CustomScrollRect scrollRect;

        [Tooltip("$ScrollView.PnlList")]
        public Transform pnlList;
        [Tooltip("$ScrollView.PnlList")]
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
    }
}
