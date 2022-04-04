using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class AnimationFade : AnimationBase {
        public static bool Fade(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            float alpha = item.parameter.targetAlpha - item.parameter.startAlpha;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.fadeDuration;
            } else {
                time = Mathf.Min(item.time, parameter.fadeDuration);
            }
            if (item.isReverse) {
                time = item.parameter.fadeDuration - time;
            }
            if (item.parameter.spriteRender != null) {
                Color color = item.parameter.spriteRender.color;
                item.parameter.spriteRender.color = new Color(color.r, color.g, color.b, 
                    item.parameter.startAlpha +
                    alpha * item.parameter.fadeCurve.Evaluate(time));
            } else if(item.parameter.canvasGroup != null) {
                item.parameter.canvasGroup.alpha = item.parameter.startAlpha +
                    alpha * item.parameter.fadeCurve.Evaluate(time);
            } else {
                Debug.LogError("This is no renderer on this gameobject: " + item.obj.name);
                return true;
            }

            if (item.time >= item.parameter.fadeDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }
    }
}
