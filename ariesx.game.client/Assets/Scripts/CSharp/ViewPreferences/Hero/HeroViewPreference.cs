using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class HeroViewPreference : BaseViewPreference {
        [Tooltip("UIHero.BtnBackground")]
        public CustomButton btnBackground;
        [Tooltip("UIHero.PnlHero")]
        public Transform pnlHero;
        [Tooltip("UIHero.PnlHero.PnlTab")]
        public PanelNewTabsView tabView;
    }
}
