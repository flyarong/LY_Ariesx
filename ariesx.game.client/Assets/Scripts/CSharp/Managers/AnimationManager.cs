using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class AnimationItem {
        public GameObject obj;
        public float time;
        public AnimationParam parameter;
        public AnimationParam Parameter {
            set {

                this.parameter = value;
                if (value.useTrack) {
                    List<Vector2> positionList = new List<Vector2>();
                    for (int i = 0; i < value.trackCurve.keys.Length; i++) {
                        positionList.Add(new Vector2(value.trackCurve.keys[i].time, value.trackCurve.keys[i].value));
                        if (i < value.trackCurve.keys.Length - 1) {
                            for (int j = 0; j < value.interval; j++) {
                                positionList.Add(new Vector2(
                                    value.trackCurve.keys[i].time +
                                    (value.trackCurve.keys[i + 1].time - value.trackCurve.keys[i].time)
                                    * (j + 1) / (value.interval + 1),
                                    value.trackCurve.Evaluate(
                                        value.trackCurve.keys[i].time +
                                        (value.trackCurve.keys[i + 1].time - value.trackCurve.keys[i].time)
                                        * (j + 1) / (value.interval + 1))
                                    ));
                            }
                        }
                    }
                    this.positionList = positionList;
                }
            }
        }
        public UnityAction callback;
        public bool isReverse;
        public bool isTureOver;
        public float scale;
        public PositionSpace space;
        public Vector3 axis;
        public Vector2 direction;

        public Vector3 singleCurveStartPosition;
        public Vector3 singleCurveTargetPosition;

        public List<Vector2> positionList;
    }

    public enum PositionSpace {
        SelfWorld = 1,
        UI,
        World
    }

    public class AnimationManager : MonoBehaviour {
        private static AnimationManager self;
        public static AnimationManager Instance {
            get {
                if (self == null) {
                    Debug.LogWarning("AnimationManager is not initialized.");
                }
                return self;
            }
        }

        private Dictionary<GameObject, AnimationItem> animationList =
            new Dictionary<GameObject, AnimationItem>();
        private Dictionary<GameObject, Coroutine> coroutineList =
            new Dictionary<GameObject, Coroutine>();
        private List<GameObject> gcList = new List<GameObject>();
        private List<UnityAction> actionList = new List<UnityAction>();

        void Awake() {
            self = this;
            UpdateManager.Regist(UpdateInfo.AnimationManager, this.UpdateAction);
        }

        private void UpdateAction() {
            foreach (AnimationItem item in this.animationList.Values) {
                this.UpdateAnimation(item);
            }
            foreach (GameObject item in this.gcList) {
                this.animationList.Remove(item);
            }
            foreach (UnityAction action in this.actionList) {
                try {
                    action.Invoke();
                } catch (System.Exception e) {
                    UnityEngine.Debug.LogErrorFormat(
                        "AnimationManager action error: {0}",
                        e
                    );
                }
            }
            this.gcList.Clear();
            this.actionList.Clear();
        }

        private void UpdateAnimation(AnimationItem item, bool needCallback = true) {
            bool isDone = true;
            if (item.parameter.isMoving) {
                isDone &= (item.obj == null || AnimationMove.Move(item));
            }
            if (item.parameter.isRotating) {
                isDone &= (item.obj == null || AnimationRotate.Rotate(item));
            }
            if (item.parameter.isFading) {
                isDone &= (item.obj == null || AnimationFade.Fade(item));
            }
            if (item.parameter.isScaling) {
                isDone &= (item.obj == null || AnimationScale.Scale(item));
            }
            if (item.parameter.isResizing) {
                isDone &= (item.obj == null || AnimationSize.Resize(item));
            }
            if (item.parameter.isEvent) {
                isDone &= (item.obj == null || AnimationEvent.Event(item));
            }
            if (item.parameter.isColor) {
                isDone &= (item.obj == null || AnimationColor.Color(item));
            }
            if (item.parameter.frameCallback != null) {
                try {
                    item.parameter.frameCallback.Invoke();
                } catch (System.Exception e) {
                    UnityEngine.Debug.LogErrorFormat(
                        "AnimationManager action error: {0}",
                        e
                    );
                }
            }

            if (isDone) {
                if (item.callback != null && needCallback) {
                    this.actionList.Add(item.callback);
                }
                this.gcList.Add(item.obj);
            } else {
                item.time += Time.unscaledDeltaTime;
            }
        }

        // need a delay list.
        private IEnumerator AnimateInDelay(AnimationItem item, float delay) {
            //yield return new WaitForSeconds(delay);
            yield return YieldManager.GetWaitForSeconds(delay);
            this.coroutineList.Remove(item.obj);

            if (!this.animationList.ContainsKey(item.obj)) {
                this.animationList.Add(item.obj, item);
            } else {
                Debug.LogWarningf("There is a multiple anition in delay {0} of {1}",
                    item.parameter.name, item.obj.name);
            }
        }

        public static void AnimateWithAllParams(GameObject obj, AnimationParam parameter,
            UnityAction finishCallback = null, UnityAction framCallback = null,
            bool loop = false, bool isReverse = false, bool isTureOver = false, float scale = 1, float delay = 0,
            Vector3 start = default(Vector3), Vector3 target = default(Vector3),
            PositionSpace space = PositionSpace.UI, bool isOffset = false,
            Vector3? axis = null, bool needRestart = true, bool restartCallback = false) {
            if (obj == null || parameter == null) {
                return;
            }
            if (Instance.animationList.ContainsKey(obj)) {
                if (!needRestart) {
                    Debug.LogError("Something wrong");
                    return;
                } else if (restartCallback) {
                    Instance.actionList.TryAdd(Instance.animationList[obj].callback);
                }
            }

            // To do: This is not need.
            //  Finish(obj);
            if (axis == null) {
                axis = Vector3.one;
            }

            if (isOffset && parameter.isMoving) {
                if (space == PositionSpace.SelfWorld) {
                    parameter.startPosition = obj.transform.localPosition + start;
                    parameter.targetPosition = obj.transform.localPosition + target;
                } else if (space == PositionSpace.World) {
                    parameter.startPosition = obj.transform.position + start;
                    parameter.targetPosition = obj.transform.position + target;
                } else {
                    RectTransform rectTransform = obj.GetComponent<RectTransform>();
                    parameter.startPosition = rectTransform.anchoredPosition + (Vector2)start;
                    parameter.targetPosition = rectTransform.anchoredPosition + (Vector2)target;
                }
            } else {
                parameter.startSize =
                parameter.startPosition = start;
                parameter.targetSize =
                parameter.targetPosition = target;
            }

            parameter.loop = parameter.loop ? parameter.loop : loop;
            parameter.frameCallback = framCallback;
            parameter.delay = delay != 0 ? delay : parameter.delay;
            if (parameter.isInitSpeed) {
                Vector2 direction = target - start;
                Quaternion q = Quaternion.Euler(0, 0, parameter.moveAngle);
                Matrix4x4 change = new Matrix4x4();
                change.SetTRS(Vector3.zero, q, Vector3.one);
                parameter.velocity = (change * direction).normalized * parameter.speed;
            }
            if (parameter != null) {

                Vector2 direction = Vector2.zero;
                if (parameter.useSpeed) {
                    direction = (target - start).normalized;
                }
                AnimationItem item = new AnimationItem {
                    obj = obj,
                    time = parameter.startTime,
                    Parameter = parameter,
                    callback = finishCallback,
                    isReverse = isReverse,
                    isTureOver = isTureOver,
                    scale = scale,
                    space = space,
                    axis = (Vector3)axis,
                    direction = direction
                };
                if (parameter.isXYZSeperate) {
                    item.singleCurveStartPosition = parameter.startPosition;
                    item.singleCurveTargetPosition = parameter.targetPosition;
                }
                if (parameter.delay != 0) {
                    Instance.coroutineList[obj] =
                        Instance.StartCoroutine(Instance.AnimateInDelay(item, parameter.delay));
                } else {

                    Instance.animationList[obj] = item;
                }
                self.UpdateAnimation(item);
            }
        }

        public static void Animate(GameObject obj, string name, UnityAction finishCallback = null,
            UnityAction frameCallback = null, bool loop = false, bool isReverse = false,
            float scale = 1, PositionSpace space = PositionSpace.UI, bool isOffset = true,
            bool needRestart = true, float delay = 0) {
            AnimationParam parameter = GetAnimationParameter(obj, name, finishCallback);
            AnimateWithAllParams(obj: obj, parameter: parameter, finishCallback: finishCallback,
                framCallback: frameCallback, loop: loop, isReverse: isReverse, delay: delay,
                scale: scale, space: space, isOffset: isOffset, needRestart: needRestart);
        }


        public static void Animate(GameObject obj, string name, float delay,
            UnityAction finishCallback = null, Vector2? axis = null, bool isOffset = true) {
            AnimationParam parameter = GetAnimationParameter(obj, name, finishCallback);
            AnimateWithAllParams(obj: obj, parameter: parameter, delay: delay,
            finishCallback: finishCallback, isOffset: isOffset, axis: axis);
        }

        public static void Animate(GameObject obj, string name, Vector3 start, Vector3 target,
        UnityAction finishCallback = null, UnityAction frameCallback = null,
        bool isReverse = false, bool isOffset = false, float scale = 1,
        PositionSpace space = PositionSpace.UI, float delay = 0) {
            AnimationParam parameter = GetAnimationParameter(obj, name, finishCallback);
            AnimateWithAllParams(obj: obj, parameter: parameter, finishCallback: finishCallback,
                framCallback: frameCallback, start: start, target: target, delay: delay,
                isReverse: isReverse, scale: scale, space: space, isOffset: isOffset);
        }

        public static void Animate(GameObject obj, string name, Vector3 start,
            UnityAction finishCallback = null, UnityAction frameCallback = null,
            bool isReverse = false, bool isOffset = false, float scale = 1,
            PositionSpace space = PositionSpace.UI, bool isTureOver = false
        ) {
            AnimationParam parameter = GetAnimationParameter(obj, name, finishCallback);
            AnimateWithAllParams(obj: obj, parameter: parameter, finishCallback: finishCallback,
                framCallback: frameCallback, start: start,
                isReverse: isReverse, isTureOver: isTureOver, scale: scale, space: space, isOffset: isOffset);
        }

        public static void Animate(GameObject obj, AnimationParam parameter, Vector3 start = default(Vector3),
            Vector3 target = default(Vector3), UnityAction finishCallback = null, UnityAction frameCallback = null,
            bool isReverse = false, bool isOffset = false, float scale = 1, bool restartCallback = true,
            PositionSpace space = PositionSpace.UI) {
            AnimateWithAllParams(obj: obj, parameter: parameter, finishCallback: finishCallback,
                framCallback: frameCallback, start: start, target: target, restartCallback: true,
                isReverse: isReverse, scale: scale, space: space, isOffset: isOffset);
        }

        private static AnimationParam GetAnimationParameter(GameObject obj,
            string name, UnityAction finishCallback) {
            AnimationCombo combo = obj.GetComponent<AnimationCombo>();
            if (combo == null || combo.GetAnimation(name) == null) {
                //Debug.LogWarning("There is no animation on this gameObject: " + obj.name);
                // To do: need finish the callback?
                if (finishCallback != null) {
                    finishCallback.Invoke();
                }
                return null;
            }
            return combo.GetAnimation(name);
        }

        public static void Animate(Animator animator, string trigger) {
            //  animator.SetTrigger(trigger);
        }

        public static void Animator(GameObject obj, string name, bool state) {
            Animator animator = obj.GetComponent<Animator>();
            if (animator == null) {
                Debug.LogError("There is no Animator on this gameObject.");
                return;
            }
            animator.SetBool(name, state);
        }

        public static void AnimateEvent(GameObject obj, int maxAmount, string name,
            UnityAction eventAction, UnityAction callback = null, float delay = 0,
            bool isFix = true, float target = 1, float interval = 1f) {
            if (Instance.animationList.ContainsKey(obj)) {
                Finish(obj);
            }
            AnimationCombo combo = obj.GetComponent<AnimationCombo>();
            if (combo == null) {
                Debug.LogError("There is no AnimationCombo on this gameObject.");
                return;
            }
            AnimationParam parameter = combo.GetAnimation(name);
            parameter.animationEvent = new UnityEvent();
            parameter.animationEvent.RemoveAllListeners();
            parameter.animationEvent.AddListener(eventAction);
            parameter.maxAmount = maxAmount;
            parameter.lastValue = 0;
            parameter.currentAmount = 0;
            if (!isFix) {
                parameter.eventInterval = interval;
                Keyframe[] keys = parameter.eventCurve.keys;
                keys[1].time = target;
                keys[1].value = target;
                keys[1].inTangent =
                keys[0].outTangent = 1;
                parameter.eventCurve.keys = keys;
            }
            if (parameter != null) {
                AnimationItem item = new AnimationItem {
                    obj = obj,
                    time = 0,
                    Parameter = parameter,
                    callback = callback,
                    isReverse = false,
                    scale = 1
                };
                Instance.coroutineList[obj] =
                    Instance.StartCoroutine(Instance.AnimateInDelay(item, delay));
                self.UpdateAnimation(item);
            }
        }

        public static void Stop(GameObject obj) {
            if (Instance == null) {
                return;
            }
            if (Instance.animationList.ContainsKey(obj)) {
                AnimationItem item = Instance.animationList[obj];
                Instance.animationList.Remove(obj);
            } else if (Instance.coroutineList.ContainsKey(obj)) {
                Instance.StopCoroutine(Instance.coroutineList[obj]);
                Instance.coroutineList.Remove(obj);
            }
        }

        public static void Finish(GameObject obj, bool needCallback = true, bool recover = false) {
            if (Instance.animationList.ContainsKey(obj)) {
                AnimationItem item = Instance.animationList[obj];
                if (recover) {
                    item.isReverse = true;
                }
                item.time = item.parameter.moveDuration;
            } else if (Instance.coroutineList.ContainsKey(obj)) {
                Instance.StopCoroutine(Instance.coroutineList[obj]);
                Instance.coroutineList.Remove(obj);
            }
        }
    }
}
