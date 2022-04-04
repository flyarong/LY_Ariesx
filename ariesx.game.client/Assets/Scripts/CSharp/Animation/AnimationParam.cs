using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Poukoute {
    [RequireComponent(typeof(AnimationCombo))]
    public class AnimationParam : AnimationBase {
        public string animationName;
        public float delay;
        [HideInInspector]
        public bool isMoving;
        [HideInInspector]
        public bool isInitSpeed;
        [HideInInspector]
        public bool isXYZSeperate;
        [HideInInspector]
        public bool isOffset;
        [HideInInspector]
        public bool loop;
        [HideInInspector]
        public float startTime;
        [HideInInspector]
        public UnityAction frameCallback;
        // Move parameters.
        [HideInInspector]
        public bool useSpeed;
        [HideInInspector]
        public bool useTrack;
        [HideInInspector]
        public AnimationCurve trackCurve = new AnimationCurve();
        [HideInInspector]
        public float trackDuration {
            get {
                return (trackCurve.keys.Length + (trackCurve.keys.Length-1)* interval) * (1f/GameConst.frame);
            }
            set {
                 return;
            }
        }
        [HideInInspector]
        public float magnificationTimes;
        [HideInInspector]
        public int interval;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public Vector3 startPosition = Vector3.zero;
        [HideInInspector]
        public Vector3 targetPosition = Vector3.zero;
        [HideInInspector]
        public Vector3 startOffset;
        [HideInInspector]
        public Vector3 targetOffset;
        [HideInInspector]
        public float moveDuration {
            get {
                if (isInitSpeed) {
                    return Mathf.Max(
                        moveXCurve.length == 0 ? 0 : moveXCurve.keys[moveXCurve.length - 1].time,
                        moveYCurve.length == 0 ? 0 : moveYCurve.keys[moveYCurve.length - 1].time
                    );
                } else {
                    return moveCurve.length == 0 ? 0 : moveCurve.keys[moveCurve.length - 1].time;
                }
            }
            set {
                if (moveCurve.length == 0) {
                    return;
                }
                moveCurve.keys[moveCurve.length - 1].time = value;
            }
        }

        [HideInInspector]
        public AnimationCurve moveCurve = new AnimationCurve();
        [HideInInspector]
        public float moveAngle;
        [HideInInspector]
        public Vector2 velocity;
        [HideInInspector]
        public float crossVelocityFade;
        [HideInInspector]
        public AnimationCurve accelerateCurve = new AnimationCurve();
        [HideInInspector]
        public AnimationCurve moveXCurve = new AnimationCurve();
        [HideInInspector]
        public AnimationCurve moveYCurve = new AnimationCurve();
        [HideInInspector]
        public AnimationCurve moveZCurve = new AnimationCurve();

        public bool isRotating;
        // Rotate parameters.
        [HideInInspector]
        public Vector3 startAngle;
        [HideInInspector]
        public Vector3 targetAngle;
        [HideInInspector]
        public int angleInterval = 0;
        [HideInInspector]
        public float rotateDuration {
            get {
                if (rotateCurve.length == 0) {
                    return 0;
                }
                return rotateCurve.keys[rotateCurve.length - 1].time;
            }
            set {
                if (rotateCurve.length == 0) {
                    return;
                }
                rotateCurve.keys[rotateCurve.length - 1].time = value;
            }
        }
        [HideInInspector]
        public AnimationCurve rotateCurve = new AnimationCurve();

        public bool isFading;
        // Fade parameters.
        [HideInInspector]
        public float startAlpha;
        [HideInInspector]
        public float targetAlpha;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        [HideInInspector]
        public SpriteRenderer spriteRender;
        [HideInInspector]
        public float fadeDuration {
            get {
                if (fadeCurve.length == 0) {
                    return 0;
                }
                return fadeCurve.keys[fadeCurve.length - 1].time;
            }

            set {
                if (fadeCurve.length == 0) {
                    return;
                }
                fadeCurve.keys[fadeCurve.length - 1].time = value;
            }
        }
        [HideInInspector]
        public AnimationCurve fadeCurve = new AnimationCurve();

        public bool isScaling;
        // Fade parameters.
        [HideInInspector]
        public Vector3 startScale;
        [HideInInspector]
        public Vector3 targetScale;
        [HideInInspector]
        public float scaleDuration {
            get {
                if (scaleCurve.length == 0) {
                    return 0;
                }
                return scaleCurve.keys[scaleCurve.length - 1].time;
            }

            set {
                if (scaleCurve.length == 0) {
                    return;
                }
                scaleCurve.keys[scaleCurve.length - 1].time = value;
            }
        }
        [HideInInspector]
        public AnimationCurve scaleCurve = new AnimationCurve();

        public bool isResizing;
        // Fade parameters.
        [HideInInspector]
        public Vector3 startSize;
        [HideInInspector]
        public Vector3 targetSize;
        [HideInInspector]
        public float resizeDuration {
            get {
                if (sizeCurve.length == 0) {
                    return 0;
                }
                return sizeCurve.keys[sizeCurve.length - 1].time;
            }

            set {
                if (sizeCurve.length == 0) {
                    return;
                }
                sizeCurve.keys[sizeCurve.length - 1].time = value;
            }
        }
        [HideInInspector]
        public AnimationCurve sizeCurve = new AnimationCurve();

        public bool isEvent;
        // Event paramter.
        [HideInInspector]
        public float startValue;
        [HideInInspector]
        public float targetValue;
        [HideInInspector]
        public int maxAmount;
        [HideInInspector]
        public int currentAmount;
        [HideInInspector]
        public float eventInterval;
        [HideInInspector]
        public UnityEvent animationEvent;
        [HideInInspector]
        public float lastValue;
        [HideInInspector]
        public float eventDuration {
            get {
                if (eventCurve.length == 0) {
                    return 0;
                }
                return eventCurve.keys[eventCurve.length - 1].time;
            }
            set {
                if (eventCurve.length == 0) {
                    return;
                }
                eventCurve.keys[eventCurve.length - 1].time = value;
            }
        }
        [HideInInspector]
        public AnimationCurve eventCurve = new AnimationCurve();
        [HideInInspector]
        public bool isColor;
        [HideInInspector]
        public Color startColor;
        [HideInInspector]
        public Color targetColor;
        [HideInInspector]
        public SpriteRenderer renderer;
        [HideInInspector]
        public Image image;
        [HideInInspector]
        public AnimationCurve colorCurve = new AnimationCurve();
        [HideInInspector]
        public float colorDuration {
            get {
                if (colorCurve.length == 0) {
                    return 0;
                }
                return colorCurve.keys[colorCurve.length - 1].time;
            }
            set {
                if (colorCurve.length == 0) {
                    return;
                }
                colorCurve.keys[colorCurve.length - 1].time = value;
            }
        }
    }
}
