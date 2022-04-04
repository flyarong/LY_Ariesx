using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {

    public class BattleViewPreference : BaseViewPreference {
        [Tooltip("CameraRoot")]
        public GameObject cameraRoot;
        [Tooltip("UIBattle Button")]
        public Button btnBackground;
        [Tooltip("UIBattle.PnlButtons")]
        public Transform pnlButtons;
        [Tooltip("UIBattle.PnlButtons.BtnStart")]
        public Button btnStart;
        [Tooltip("UIBattle.PnlButtons.BtnPause")]
        public Button btnPause;
        [Tooltip("UIBattle.PnlButtons.BtnExit")]
        public Button btnExit;
        [Tooltip("UIBattle.PnlButtons.BtnSpeed")]
        public Button btnPlaySpeed;
        [Tooltip("UIBattle.PnlButtons.BtnSpeed.PnlContent")]
        public TextMeshProUGUI txtPlaySpeed;


        //[Tooltip("UIBattle.PnlBegin")]
        //public Transform pnlBegin;
        [Tooltip("UIBattle.PnlBegin.PnlAttack")]
        public Transform pnlAttack;
        [Tooltip("UIBattle.PnlBegin.PnlAttack RectTransform")]
        public RectTransform rectAttack;
        [Tooltip("UIBattle.PnlBegin.PnlAttack.PnlPlayerInfo")]
        public Transform pnlAttackerInfo;

        [Tooltip("UIBattle.PnlBegin.PnlDefend")]
        public Transform pnlDefense;
        [Tooltip("UIBattle.PnlBegin.PnlDefend RectTransform")]
        public RectTransform rectDefense;
        [Tooltip("UIBattle.PnlBegin.PnlDefend.PnlPlayerInfo")]
        public Transform pnlDefenderInfo;

        [Tooltip("UIBattle.PnlEnd")]
        public CanvasGroup cgEnd;
        [Tooltip("UIBattle.PnlEnd.BtnClose")]
        public CustomButton btnClose;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead")]
        public CanvasGroup cgHead;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.ImgWin")]
        public CanvasGroup battleWinCG;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.ImgLose")]
        public CanvasGroup battleLostCG;
        public TextMeshProUGUI txtBattleResult;

        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlReward")]
        public CanvasGroup cgReward;
        public RectTransform rewardRT;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlReward.PnlChest")]
        public Transform pnlChest;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlReward.PnlChest.ImgIcon")]
        public Image imgChest;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlReward.PnlChest.TxtValue")]
        public TextMeshProUGUI txtChestName;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlReward.PnlOther.PnlResource")]
        public Transform pnlResources;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlHead.PnlNoRewards")]
        public CanvasGroup cgNoreward;
        public RectTransform rctNoRewards;


        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost")]
        public CanvasGroup cgLost;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent CustomContentSizeFitter")]
        public CustomContentSizeFitter contentSizeFitter;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent.PnlOverview")]
        public Transform pnlOverview;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent.PnlOverview.BtnDetail")]
        public CustomButton btnLostDetail;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent.PnlOverview.TxtValue")]
        public TextMeshProUGUI txtOverview;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent.PnlDetail")]
        public Transform pnlDetail;
        [Tooltip("UIBattle.PnlEnd.PnlContent.PnlLost.PnlContent.PnlDetail.PnlItems")]
        public Transform pnlItems;

        // FTE
        [Tooltip("UIBattle.BtnFteMask")]
        public Button btnFteMask;
    }
}
