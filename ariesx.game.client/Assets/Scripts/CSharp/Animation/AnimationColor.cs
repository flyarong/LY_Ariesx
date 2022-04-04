using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Poukoute {
    public class AnimationColor : AnimationBase {
        public static bool Color(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 startPosition = new Vector3
                (parameter.startColor.r, parameter.startColor.g, parameter.startColor.b);
            float startAlpha = parameter.startColor.a;
            Vector3 targetPosition = new Vector3
                (parameter.targetColor.r, parameter.targetColor.g, parameter.targetColor.b);
            float targetAlpha = parameter.targetColor.a;
            Vector3 distance = targetPosition - startPosition;
            float alphaDistance = targetAlpha - startAlpha;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.colorDuration;
            } else {
                time = Mathf.Min(item.time, parameter.colorDuration);
            }
            Vector3 currentPosition = startPosition +
                distance * parameter.colorCurve.Evaluate(time);
            float currentAlpha = startAlpha +
                alphaDistance * parameter.colorCurve.Evaluate(time);
            Color currentColor = new UnityEngine.Color();
            currentColor.r = currentPosition.x ;
            currentColor.g = currentPosition.y ;
            currentColor.b = currentPosition.z ;
            currentColor.a = currentAlpha;
                if (parameter.image != null) {
                    parameter.image.color = currentColor ;
                } else if (parameter.renderer != null) {
                    parameter.renderer.color = currentColor;
                } else {
                    Debug.LogError("no image or sprite");
                }

            if (item.time >= item.parameter.colorDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }
    }
}


