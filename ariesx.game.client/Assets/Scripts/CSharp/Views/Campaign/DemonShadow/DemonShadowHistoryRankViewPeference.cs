using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class DemonShadowHistoryRankViewPeference: BaseViewPreference {

        [Tooltip("UIDemonShadowRank.BtnBackground")]
        public CustomButton btnBackground;
        [Tooltip("UIDemonShadowRank.PnlRankBG.PnlTitle.BtnClose")]
        public CustomButton btnClose;
        [Tooltip("UIDemonShadowRank.PnlRankBG.PnlRankList.pnlCustomScrollRect.PnlPlayerRankList")]
        public Transform pnlList;
        [Tooltip("UIDemonShadowRank.PnlRankBG.PnlRankList.PnlOwnRanking")]
        public TextMeshProUGUI txtOwnRank;
        public TextMeshProUGUI txtOwnName;
        public TextMeshProUGUI txtPoint;
        public Transform LastStraw;
        public DemonShadowRankItemView lastitemView;
       // public CustomButton btnShowPlayerInfo;
    }
}
