using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class LangChooseViewPreference : BaseViewPreference {
        public Button btnBackground;
        public Button btnClose;

        public CustomScrollRect scrollRect;
        public Transform PnlList;
        public RectTransform listRectTransform;
        public CustomVerticalLayoutGroup listVerticalGroup;
        public CustomContentSizeFitter listContentSizeFitter;
    }
}