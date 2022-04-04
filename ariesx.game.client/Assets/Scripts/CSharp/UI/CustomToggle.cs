using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Protocol;

namespace Poukoute {
    [RequireComponent(typeof(Toggle))]
    public class CustomToggle : MonoBehaviour {
        #region ui element
        public TextMeshProUGUI txtLabel;
        public RectTransform labelRectTransform;
        public Image imgMark;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Image imgRedNotice;
        [SerializeField]
        private TextMeshProUGUI txtNoticeNum;
        [SerializeField]
        private Image imgBackground;
        [SerializeField]
        private Image imgChoose;
        #endregion
        // Other data
        public UnityEvent onclick;
        public Toggle Toggle {
            get {
                return this.toggle;
            }
        }

        /*********************************/
        public void SetToggleInteractable(bool isEnable = true) {
            this.toggle.interactable = isEnable;
            this.imgChoose.material =
            this.imgBackground.material = (!isEnable && !this.toggle.isOn) ?
                PoolManager.GetMaterial(MaterialPath.matGray) :
                PoolManager.GetMaterial(MaterialPath.matImageFast);
        }

        public void OnPointerClick(PointerEventData eventData) {
            toggle.OnPointerClick(eventData);
            if (this.onclick != null) {
                this.onclick.Invoke();
            }
        }

        public void SetRedNoticeVisible(bool isVisible, int noticeNum) {
            this.imgRedNotice.gameObject.SetActiveSafe(isVisible);
            if (isVisible) {
                this.txtNoticeNum.text = noticeNum == 0 ? string.Empty : noticeNum.ToString();
            }
        }

        public void SetNoticeColor(TabNoticePointType color = TabNoticePointType.Red) {
            switch (color) {
                case TabNoticePointType.Red:
                    this.imgRedNotice.sprite = ArtPrefabConf.GetSprite("NoticePointRed");
                    break;
                case TabNoticePointType.Yellow:
                    this.imgRedNotice.sprite = ArtPrefabConf.GetSprite("NoticePointYellow");
                    break;
                case TabNoticePointType.Green:
                    this.imgRedNotice.sprite = ArtPrefabConf.GetSprite("NoticePointGreen");
                    break;
                default:
                    break;
            }
        }
    }
}

