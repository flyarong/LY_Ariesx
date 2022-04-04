using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapNoticeItemView : MonoBehaviour {
        #region ui element
        [SerializeField]
        private GameObject noticeContent;
        #endregion

        public void PlayShow() {
            AnimationManager.Animate(this.noticeContent, "Show", Vector3.zero, Vector3.zero, null);
        }

        public void PlayHide(UnityAction action) {
            AnimationManager.Animate(this.noticeContent, "Hide", action);
        }
    }
}