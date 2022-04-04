using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildViewPreference : BaseViewPreference {
        [Tooltip("UIBuild.ScrollView")]
        public Transform scrollView;
        [Tooltip("UIBuild.ScrollView ScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIBuild.ScrollView.PnlBuildList")]
        public Transform pnlBuildList;
        [Tooltip("UIBuild.ScrollView.PnlBuildList RectTransform")]
        public RectTransform pnlBuildListRect;
        [Tooltip("UIBuild.ScrollView.PnlBuildList CustomContentSizeFitter")]
        public CustomContentSizeFitter contentSizeFitter;
    }
}
