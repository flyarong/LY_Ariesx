using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class AnimationScale : MonoBehaviour {
        public static bool Scale(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 scale = item.parameter.targetScale - item.parameter.startScale;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.scaleDuration;
            } else {
                time = Mathf.Min(item.time, parameter.scaleDuration);
            }
            if (item.isReverse) {
                time = item.parameter.scaleDuration - time;
            }
            item.obj.transform.localScale = item.parameter.startScale +
                scale * item.parameter.scaleCurve.Evaluate(time);
            if (item.time >= item.parameter.scaleDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }
    }
}
