using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class AnimationSize : MonoBehaviour {
        public static bool Resize(AnimationItem item) {
            RectTransform rectTransform = item.obj.GetComponent<RectTransform>();
            if (rectTransform == null) {
                return true;
            }
            AnimationParam parameter = item.parameter;
            Vector3 startSize = parameter.startSize + parameter.startOffset;
            Vector3 targetSize = parameter.targetSize + parameter.targetOffset;
            Vector3 size = targetSize - startSize;
            float time = Mathf.Min(item.time, item.parameter.resizeDuration);
            if (item.isReverse) {
                time = item.parameter.resizeDuration - time;
            }
            rectTransform.sizeDelta = item.parameter.startSize +
                size * item.parameter.sizeCurve.Evaluate(time);
            if (item.time > item.parameter.resizeDuration) {
                return true;
            }
            return false;
        }
    }
}
