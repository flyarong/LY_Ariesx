using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class DramaViewPreference : MonoBehaviour {
        [Tooltip("UIMission.PnlMission.PnlDrama")]
        public Transform pnlDrama;
        public CustomClick ccDrama;
        // Head
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlHead.TxtTitle")]
        public TextMeshProUGUI txtTitle;
        // Description
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlDescription")]
        public Transform pnlDescription;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlDescription.PnlName.TxtName")]
        public TextMeshProUGUI txtDescName;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlDescription.PnlDescription.TxtDescription")]
        public TextMeshProUGUI txtDescContent;
        // Target
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlTarget")]
        public Transform pnlTarget;
        [Tooltip("UIMission.PnlMission.PnlDrama.ScrollRect")]
        public ScrollRect targetScroll;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlList")]
        public Transform pnlTargetList;
        public CustomContentSizeFitter contentSizeFitter;
        // Reward
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlReward.PnlLabel.ImgLeft")]
        public Transform pnlReward;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlReward.PnlLabel.ImgLeft")]
        public Image imgRewardLeft;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlReward.PnlLabel.ImgRight")]
        public Image imgRewardRight;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlReward.PnlLabel.TxtProgress")]
        public TextMeshProUGUI txtProgress;
        
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlResources")]
        public Transform pnlResources;
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlPreview")]
        public Transform pnlPreview;
        // Reward detail
        [Tooltip("UIMission.PnlMission.PnlDrama.PnlRewardDetail")]
        public Transform pnlRewardDetail;
        public GridLayoutGroup glgRewardDetail;
    }
}
