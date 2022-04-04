using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class AnimationEvent : AnimationBase {
        public static bool Event(AnimationItem item) {
            float startValue = item.parameter.startValue;
            float targetValue = item.parameter.targetValue;
            float time = Mathf.Min(item.time, item.parameter.eventDuration);
            float currentValue = (targetValue - startValue) * item.parameter.eventCurve.Evaluate(time);
            float offsetFloat = currentValue - item.parameter.lastValue;
            int offset = Mathf.FloorToInt(offsetFloat / item.parameter.eventInterval);
            for (int i = 0; i < offset; i++) {
                
                item.parameter.animationEvent.Invoke();
                item.parameter.currentAmount++;
            }
            item.parameter.lastValue += offset * item.parameter.eventInterval;
            if (item.time > item.parameter.eventDuration ||
                item.parameter.currentAmount >= item.parameter.maxAmount) {
                return true;
            }
            return false;
        }


    }
}
