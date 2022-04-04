using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceCitiesViewViewPreference : BaseViewPreference {
        [Tooltip("UIAlliance.PnlAlliance.PnlCitiesHoldler.PnlCities CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIAlliance.PnlAlliance.PnlCitiesHoldler.PnlCitiesPnlList")]
        public Transform pnlList;
        public VerticalLayoutGroup listVerticalLayoutGroup;
        public ContentSizeFitter listContentSizeFitter;
        public RectTransform listRectTransform;

        [Tooltip("UIAlliance.PnlAlliance.PnlCityHead")]
        public Transform pnlCityHead;
        public AllianceCityHeadViewPreference cityHeadPre;
    }
}
