using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MailSystemViewPreference : BaseViewPreference {
        [Tooltip("UIMail.PnlMail.PnlContent.PnlSystem CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMail.PnlMail.PnlContent.PnlSystem.pnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        [Tooltip("UIMail.PnlMail.PnlContent.PnlNoMailBG")]
        public GameObject pnlNoMailBG;

        [Tooltip("UIMail.PnlMail.PnlSystemDetail")]
        public CanvasGroup systemDetail;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.BtnReturn")]
        public CustomButton btnBack;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlContent.PnlTop.TxtTime")]
        public TextMeshProUGUI txtTime;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlContent.PnlTop.PnlSubject.TxtSubject")]
        public TextMeshProUGUI txtSubject;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlContent.TxtContent")]
        public TextMeshProUGUI txtContent;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlRewards")]
        public GameObject mailRewards;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlRewards.PnlContent")]
        public Transform pnlRewardsContent;
        [Tooltip("UIMail.PnlMail.PnlSystemDetail.PnlRewards.BtnReceive")]
        public CustomButton btnReceive;
        public TextMeshProUGUI isReceive;
        [Tooltip("UIMail.PnlMail.PnlSystem.btnRead")]
        public CustomButton btnRead;
    }
}
