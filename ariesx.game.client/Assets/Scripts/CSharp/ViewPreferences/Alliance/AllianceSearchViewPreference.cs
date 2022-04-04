using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceSearchViewPreference : BaseViewPreference {
        [Tooltip("PnlSearchHead.PnlTop.BtnSearch CustomButton")]
        public CustomButton btnSearch;
        [Tooltip("PnlSearchHead.PnlTop.IfInput TMP_InputField")]
        public TMP_InputField ifName;

        [Tooltip("PnlSearch CustomScrollRect")]
        public CustomScrollRect searchScrollRect;
        [Tooltip("PnlSearch.PnlList")]
        public Transform pnlList;
        [Tooltip("PnlSearch.PnlList CustomVerticalLayoutGroup")]
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
        [Tooltip("PnlSearch.PnlNoResultTips")]
        public CanvasGroup noResultTipsCG;
    }
}
