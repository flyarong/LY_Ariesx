using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MarchViewPreference : BaseViewPreference {
        [Tooltip("UIMarchBind.PnlMarchBind")]
        public Transform pnlMarchBind;
        [Tooltip("UIMarchBind.PnlMarchBind.PnlMarchDetail")]
        public Transform pnlMarchDetail;
        [Preference("PnlMarchBind.PnlMarchDetail")]
        public VerticalLayoutGroup vgMarchDetail;
        [Preference("PnlMarchBind.PnlMarchDetail")]
        public ContentSizeFitter csfMarchDetail;
        [Tooltip("UIMarchBind.PnlMarchBind.PnlMarchDetail.PnlContent.PnlTroop.PnlInfo")]
        public Transform pnlInfo;
        [Tooltip("UIMarchBind.PnlMarchBind.PnlMarchDetail.PnlContent.PnlTroop.PnlTroopGrid")]
        public Transform pnlTroopGrid;
        [Tooltip("UIMarchBind.PnlMarchBind.PnlMarchDetail.PnlContent.PnlTroop.PnlTroopGrid TroopGridView")]
        public TroopGridView troopGridView;
        [Tooltip("UIMarchBind.PnlMarchBind.PnlMarchDetail.PnlContent.PnlPlayerName.Text")]
        public TextMeshProUGUI txtTroopName;
        [Preference("PnlMarchBind.PnlMarchDetail.PnlCoordinate")]
        public Transform pnlCoordinate;
        [Preference("$pnlCoordinate.PnlOrigin.TxtNumber")]
        public TextMeshProUGUI txtOrigin;
        [Preference("$pnlCoordinate.PnlOrigin.TxtNumber")]
        public TextMeshProUGUI txtTarget;
        public Transform pnlButton;
        public CustomButton btnReturnImmediately;
    }
}
