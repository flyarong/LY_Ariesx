using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class RewardStageItemView : MonoBehaviour {
        [SerializeField]
        private Image imgStage;
        [SerializeField]
        private GameObject receivableMark;
        [SerializeField]
        private TextMeshProUGUI txtPoint;

        public void SetContent(string stageIconPath, int point) {
            this.imgStage.sprite = ArtPrefabConf.GetSprite(stageIconPath);
            this.txtPoint.text = point.ToString();
        }

        public void SetStageReceviable(bool isReceivable, bool isReceived) {
            this.receivableMark.SetActiveSafe(isReceivable);
            this.imgStage.gameObject.SetActiveSafe(!isReceived);
        }
    }
}