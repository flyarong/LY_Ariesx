using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace Poukoute {
    [CustomEditor(typeof(AnimationParam))]

    public class AnimationParamEditor : Editor {
        private AnimationParam parameter;

        void OnEnable() {
            this.parameter = (AnimationParam)target;
        }

        public override void OnInspectorGUI() {

            this.parameter.animationName = EditorGUILayout.TextField(
                new GUIContent("Name", "Animation Name."),
                this.parameter.animationName
            );

            this.parameter.delay = EditorGUILayout.FloatField(
                new GUIContent("Delay", "delay in seconds."),
                this.parameter.delay
            );

            this.parameter.loop = EditorGUILayout.Toggle(
                new GUIContent("Loop", "Is loop."),
                this.parameter.loop
            );

            this.parameter.startTime = EditorGUILayout.FloatField(
                new GUIContent("Start time", "The time begin with."),
                this.parameter.startTime
            );

            this.parameter.isOffset = EditorGUILayout.Toggle(
                new GUIContent("IsOffset", "is the value below offset."),
                this.parameter.isOffset
            );

            this.parameter.isMoving = EditorGUILayout.Toggle(
                new GUIContent("Move", "Move."),
                this.parameter.isMoving
            );
            if (this.parameter.isMoving) {
                this.ShowMoveParams();
            }
            this.parameter.isRotating = EditorGUILayout.Toggle(
                new GUIContent("Rotate", "Rotate."),
                this.parameter.isRotating
            );
            if (this.parameter.isRotating) {
                this.ShowRotateParams();
            }
            this.parameter.isFading = EditorGUILayout.Toggle(
                new GUIContent("Fade", "Fade."),
                this.parameter.isFading
            );
            if (this.parameter.isFading) {
                this.ShowFadeParams();
            }
            this.parameter.isScaling = EditorGUILayout.Toggle(
               new GUIContent("Scale", "Scale."),
               this.parameter.isScaling
            );
            if (this.parameter.isScaling) {
                this.ShowScaleParams();
            }
            this.parameter.isResizing = EditorGUILayout.Toggle(
               new GUIContent("Size", "Size."),
               this.parameter.isResizing
            );
            if (this.parameter.isResizing) {
                this.ShowResizeParams();
            }
            this.parameter.isEvent = EditorGUILayout.Toggle(
                new GUIContent("Event", "Event."),
                this.parameter.isEvent
            );
            if (this.parameter.isEvent) {
                this.ShowEventParams();
            }
            this.parameter.isColor = EditorGUILayout.Toggle(
               new GUIContent("Color", "Color."),
               this.parameter.isColor
           );
            if (this.parameter.isColor) {
                this.ShowColorParams();
            }
            if (GUILayout.Button("Start")) {
                if (this.parameter.isOffset) {
                    AnimationManager.Animate(this.parameter.gameObject, this.parameter.animationName, null);
                } else {
                    AnimationManager.Animate(this.parameter.gameObject, this.parameter.animationName,
                        Vector3.zero, Vector3.zero, null);
                }
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(this.parameter);
            }
        }

        private void ShowMoveParams() {
            this.parameter.useSpeed = EditorGUILayout.Toggle(
                new GUIContent("Use Speed", ""),
                this.parameter.useSpeed
            );
            this.parameter.useTrack = EditorGUILayout.Toggle(
               new GUIContent("Use Track", ""),
               this.parameter.useTrack
           );
            this.parameter.isInitSpeed = EditorGUILayout.Toggle(
                new GUIContent("Init Speed", ""),
                this.parameter.isInitSpeed
            );
            this.parameter.isXYZSeperate = EditorGUILayout.Toggle(
                new GUIContent("XYZ Speed", ""),
                this.parameter.isXYZSeperate
            );
            this.parameter.startOffset = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startOffset
            );
            this.parameter.targetOffset = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetOffset
            );
            if (this.parameter.useSpeed) {
                this.parameter.speed = EditorGUILayout.FloatField(
                    new GUIContent("Speed", "The speed of animation."),
                    this.parameter.speed
                );
            } else if (this.parameter.useTrack) {
                this.parameter.trackDuration = EditorGUILayout.FloatField(
                    new GUIContent("Duration", "The duration of the animation."),
                    this.parameter.trackDuration
                );
                this.parameter.trackCurve = EditorGUILayout.CurveField(
                    new GUIContent("TrackCurve", "The TrackCurve."),
                    this.parameter.trackCurve
                );
                this.parameter.interval = EditorGUILayout.IntField(
                    new GUIContent("Interval", "The interval."),
                    this.parameter.interval
                );
                this.parameter.magnificationTimes = EditorGUILayout.FloatField(
                    new GUIContent("MagnificationTimes", "The magnificationTimes."),
                    this.parameter.magnificationTimes
                );
            } else if (this.parameter.isInitSpeed) {
                this.parameter.speed = EditorGUILayout.FloatField(
                   new GUIContent("Speed", "The speed of animation."),
                   this.parameter.speed
                );
                this.parameter.moveAngle = EditorGUILayout.FloatField(
                    new GUIContent("Angle", "The move angle."),
                    this.parameter.moveAngle
                );
                this.parameter.accelerateCurve = EditorGUILayout.CurveField(
                    new GUIContent("Accelerate Curve", "The accelerate curve."),
                    this.parameter.accelerateCurve
                );
                this.parameter.crossVelocityFade = EditorGUILayout.Slider(
                    new GUIContent("Fade param", "The cross velocity fade param."),
                    this.parameter.crossVelocityFade, 0, 1
                );
            } else if (this.parameter.isXYZSeperate) {
                this.parameter.moveDuration = EditorGUILayout.FloatField(
                    new GUIContent("Duration", "The duration of the animation."),
                    this.parameter.moveDuration
                );
                this.parameter.moveCurve = EditorGUILayout.CurveField(
                    new GUIContent("Curve", "The curve of the animation."),
                    this.parameter.moveCurve
                );
                this.parameter.moveXCurve = EditorGUILayout.CurveField(
                    new GUIContent("XCurve", "The curve of the x-axis animation."),
                    this.parameter.moveXCurve
                );
                this.parameter.moveYCurve = EditorGUILayout.CurveField(
                    new GUIContent("YCurve", "The curve of the y-axis animation."),
                    this.parameter.moveYCurve
                );
                this.parameter.moveZCurve = EditorGUILayout.CurveField(
                    new GUIContent("ZCurve", "The curve of the z-axis animation."),
                    this.parameter.moveZCurve
                );
                if (GUILayout.Button("Curve Window")) {

                }
            } else {
                this.parameter.moveDuration = EditorGUILayout.FloatField(
                   new GUIContent("Duration", "The duration of the animation."),
                   this.parameter.moveDuration
                );
                this.parameter.moveCurve = EditorGUILayout.CurveField(
                    new GUIContent("Curve", "The curve of the animation."),
                    this.parameter.moveCurve
                );
            }

        }

        private void ShowRotateParams() {
            this.parameter.startAngle = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startAngle
            );
            this.parameter.targetAngle = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetAngle
            );
            this.parameter.rotateDuration = EditorGUILayout.FloatField(
               new GUIContent("Duration", "The duration of the animation."),
               this.parameter.rotateDuration
            );

            this.parameter.angleInterval = EditorGUILayout.IntField(
                new GUIContent("Interval", "The interval of rotate."),
                this.parameter.angleInterval
            );

            this.parameter.rotateCurve = EditorGUILayout.CurveField(
                new GUIContent("Curve", "The curve of the animation."),
                this.parameter.rotateCurve
            );
        }

        private void ShowFadeParams() {
            this.parameter.startAlpha = EditorGUILayout.FloatField(
                new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startAlpha
            );
            this.parameter.targetAlpha = EditorGUILayout.FloatField(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetAlpha
            );
            this.parameter.fadeDuration = EditorGUILayout.FloatField(
               new GUIContent("Duration", "The duration of the animation."),
               this.parameter.fadeDuration
           );

            this.parameter.canvasGroup = EditorGUILayout.ObjectField(
                new GUIContent("Canvas", "The canvas of the object."),
                this.parameter.canvasGroup,
                typeof(CanvasGroup), true
           ) as CanvasGroup;

            this.parameter.spriteRender = EditorGUILayout.ObjectField(
                new GUIContent("Renderer", "The renderer of the object."),
                this.parameter.spriteRender,
                typeof(SpriteRenderer), true
           ) as SpriteRenderer;

            this.parameter.fadeCurve = EditorGUILayout.CurveField(
                new GUIContent("Curve", "The curve of the animation."),
                this.parameter.fadeCurve
            );
        }

        private void ShowScaleParams() {
            this.parameter.startScale = EditorGUILayout.Vector3Field(
                    new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startScale
            );
            this.parameter.targetScale = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetScale
            );
            this.parameter.scaleDuration = EditorGUILayout.FloatField(
               new GUIContent("Duration", "The duration of the animation."),
               this.parameter.scaleDuration
           );

            this.parameter.scaleCurve = EditorGUILayout.CurveField(
                new GUIContent("Curve", "The curve of the animation."),
                this.parameter.scaleCurve
            );
        }

        private void ShowResizeParams() {
            this.parameter.startSize = EditorGUILayout.Vector3Field(
                    new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startSize
            );
            this.parameter.targetSize = EditorGUILayout.Vector3Field(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetSize
            );
            this.parameter.resizeDuration = EditorGUILayout.FloatField(
               new GUIContent("Duration", "The duration of the animation."),
               this.parameter.resizeDuration
           );

            this.parameter.sizeCurve = EditorGUILayout.CurveField(
                new GUIContent("Curve", "The curve of the animation."),
                this.parameter.sizeCurve
            );
        }

        private void ShowEventParams() {
            this.parameter.startValue = EditorGUILayout.FloatField(
                    new GUIContent("Axis(y=0)", "The value of curve y = 0"),
                this.parameter.startValue
            );
            this.parameter.targetValue = EditorGUILayout.FloatField(
                new GUIContent("Axis(y=1)", "The value of curve y = 1"),
                this.parameter.targetValue
            );
            this.parameter.eventDuration = EditorGUILayout.FloatField(
               new GUIContent("Duration", "The duration of the animation."),
               this.parameter.eventDuration
            );
            this.parameter.eventInterval = EditorGUILayout.FloatField(
                new GUIContent("Interval Value", "The interval between to event."),
                this.parameter.eventInterval
            );
            this.parameter.eventCurve = EditorGUILayout.CurveField(
                new GUIContent("Curve", "The curve of the animation."),
                this.parameter.eventCurve
            );
        }

        private void ShowColorParams() {
            this.parameter.startColor = EditorGUILayout.ColorField(
                    new GUIContent("StartColor", "StartColor"),
                this.parameter.startColor
            );
            this.parameter.targetColor = EditorGUILayout.ColorField(
                   new GUIContent("TargerColor", "TargerColor"),
               this.parameter.targetColor
           );
            this.parameter.colorCurve = EditorGUILayout.CurveField(
                     new GUIContent("ColorCurve", "ColorCurve"),
                 this.parameter.colorCurve
           );
            this.parameter.colorDuration = EditorGUILayout.FloatField(
                   new GUIContent("ColorDuration", "colorDuration"),
               this.parameter.colorDuration
         );
            this.parameter.renderer = EditorGUILayout.ObjectField(
                     new GUIContent("Renderer", "The renderer of the object."),
                     this.parameter.renderer,
                     typeof(SpriteRenderer), true
               ) as SpriteRenderer;
            this.parameter.image = EditorGUILayout.ObjectField(
                 new GUIContent("Image", "The image of the object."),
                 this.parameter.image,
                 typeof(Image), true
           ) as Image;

        }
    }
}
