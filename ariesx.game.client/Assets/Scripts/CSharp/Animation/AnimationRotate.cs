using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class AnimationRotate : AnimationBase {
        public static bool Rotate(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 angle = item.parameter.targetAngle - item.parameter.startAngle;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.rotateDuration;
            } else {
                time = Mathf.Min(item.time, parameter.rotateDuration);
            }
            if (item.isReverse) {
                time = item.parameter.rotateDuration - time;
            }
            Vector3 changeAngle = angle * item.parameter.rotateCurve.Evaluate(time);
            int interval = item.parameter.angleInterval;
            if (interval != 0) {
                changeAngle = new Vector3(
                    ((int)changeAngle.x / interval) * interval,
                    ((int)changeAngle.y / interval) * interval,
                    ((int)changeAngle.z / interval) * interval
                );
            }
            item.obj.transform.rotation = Quaternion.Euler(
                item.parameter.startAngle + changeAngle
            );
            if (item.time >= item.parameter.rotateDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }
    }
}
