using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI {
    public class CustomGridLayoutGroup : GridLayoutGroup {
        public class CustomScrollEvent : UnityEvent<UnityAction<float>> { }
        public class CustomScrollEventEx : UnityEvent<float> { }

        public CustomScrollRect scrollRect;
        public float size;
        private float original = Mathf.Infinity;

        public CustomScrollEvent onValueChanged = new CustomScrollEvent();
        public CustomScrollEventEx onValueChangedEx = new CustomScrollEventEx();

        protected override void Awake() {
            base.Awake();
            if (this.scrollRect == null) {
                this.scrollRect = this.transform.parent.GetComponent<CustomScrollRect>();
            }

            if (this.size != 0) {
                //this.scrollRect.unitOffset = new Vector2(0, this.size);
            } else {
                this.size = Mathf.Infinity;
            }

            this.scrollRect.onContentPosChanged.AddListener(this.OnPositionChanged);
            this.scrollRect.onContentPosChanged.AddListener(this.OnPostionChangedEx);
        }

        /* because update is execute early than ondrag each frame, so it may cause beyond the board.  */
        private void OnPositionChanged() {
            if (onValueChanged == null || this.original == Mathf.Infinity)
                return;
            this.onValueChanged.Invoke(this.SetRectOffset);
        }

        private void OnPostionChangedEx() {
            if (onValueChangedEx == null || this.original == Mathf.Infinity) {
                return;
            }
            float offset = this.rectTransform.anchoredPosition.y - this.original;
            this.onValueChangedEx.Invoke(offset);
        }

        public void SetRectOffset(float offset) {
            this.scrollRect.SetContentAnchoredPosition(
              this.rectTransform.anchoredPosition - new Vector2(0, offset)
            );
            this.original -= offset;
            this.scrollRect.m_FrameOffset += new Vector2(0, -offset);
        }

        public void SetOriginal() {
            this.original = this.rectTransform.anchoredPosition.y;
        }
    }
}
