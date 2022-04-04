using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    
    public class DailyTaskViewPreference : MonoBehaviour {
        [Tooltip("UIMission.PnlMission.PnlDailyTask")]
        public CustomClick ccBackground;
        [Tooltip("UIMission.PnlMission.PnlDailyTask.TxtUnlockTips")]
        public GameObject pnlUnlockTips;
        [Tooltip("UIMission.PnlMission.PnlDailyTask.PnlTop CanvasGroup")]
        public CanvasGroup topCG;
        [Tooltip("UIMission.PnlMission.PnlDailyTask.PnlTop.TxtDescription")]
        public TextMeshProUGUI txtDescription;
        [Tooltip("UIMission.PnlMission.PnlDailyTask.PnlStarPos")]
        public Transform pnlStarPos;

        [Tooltip("UIMission.PnlMission.PnlDailyTask.PnlContent ScrollRect")]
        public CustomScrollRect scrollRect;
        public CanvasGroup pnlContent;
        [Tooltip("UIMission.PnlMission.PnlDailyTask.PnlContent.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;

        public PanelStageRewardsView stageRewardView;
    }
}
