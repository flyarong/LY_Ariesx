using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Poukoute {
    public class HeroPoolViewPreference : BaseViewPreference {
        [Tooltip("UIHeroPool.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UIHeroPool.PnlHeroPool")]
        public Transform pnlHeroPool;
        [Tooltip("UIHeroPool.PnlHeroPool.PnlHead.BtnClose")]
        public Button btnClose;
        [Tooltip("UIHeroPool.PnlHeroPool.PnlChestTitile.TxtChestName")]
        public TextMeshProUGUI txtChestName;
        [Tooltip("UIHeroPool.PnlHeroPool.BtnInfo")]
        public CustomButton btnInfo;
        [Tooltip("UIHeroPool.PnlHeroPool.PnlInfo")]
        public GameObject heroPoolInfo;

        [Tooltip("UIHeroPool.PnlHeroPool.PnlCardType")]
        public Transform pnlHeroCardList;
        [Tooltip("UIHeroPool.PnlHeroPool.PnlContent.ScrollRect.PnlList")]
        public Transform pnlList;

        //[Tooltip("UIHeroPool.PnlHeroPool.PnlOperate.BtnPre")]
        //public CustomButton btnPreLevelGocha;
        //[Tooltip("UIHeroPool.PnlHeroPool.PnlOperate.BtnNext")]
        //public CustomButton btnNextLevelGocha;
    }
}
