using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class SelectServerViewPreference : BaseViewPreference {
        public Transform PnlGroupList;
        public Transform PnlDetailList;
        public Button btnClose;
        public CustomButton background;
        public ToggleGroup toggleGroup;
        public Transform pnlMyServer;
        public TextMeshProUGUI txtID;
        public TextMeshProUGUI txtName;
        //public TextMeshProUGUI Time;
        public TextMeshProUGUI force;
        public TextMeshProUGUI level;
        public Image imgSmallLight;
    }
}
