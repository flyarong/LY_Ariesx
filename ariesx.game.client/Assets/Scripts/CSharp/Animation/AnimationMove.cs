using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Poukoute {
    public class AnimationMove : AnimationBase {
        private static float maxSpeed = 45;
        public static bool Move(AnimationItem item) {
            if (item.parameter.useSpeed) {
                return MoveWithSpeed(item);
            } else if (item.parameter.useTrack) {
                return MoveWithTrack(item);
            } else if (item.parameter.isInitSpeed) {
                return MoveWithInitSpeed(item);
            } else if (item.parameter.isXYZSeperate) {
                return MoveSeperate(item);
            } else {
                return MoveWithTime(item);
            }
        }

        private static bool MoveSeperate(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 startPosition = item.singleCurveStartPosition;
            Vector3 targetPosition = item.singleCurveTargetPosition;
            //Vector3 startPosition = parameter.startPosition;
            //Vector3 targetPosition = parameter.targetPosition;

            Vector3 distance = targetPosition - startPosition;
            Vector3 distanceOffset = parameter.targetOffset - parameter.startOffset;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.moveDuration;
            } else {
                time = Mathf.Min(item.time, parameter.moveDuration);
            }
            if (item.isReverse) {
                time = item.parameter.moveDuration - time;
            }
            Vector3 currentPosition = startPosition +
                distance * parameter.moveCurve.Evaluate(time);
            Vector3 currentOffset =
                new Vector3(
                    distanceOffset.x * parameter.moveXCurve.Evaluate(time),
                    distanceOffset.y * parameter.moveYCurve.Evaluate(time),
                    distanceOffset.z * parameter.moveZCurve.Evaluate(time)
                );
            RectTransform rectTransform = item.obj.GetComponent<RectTransform>();
            Vector2 axisReverse = Vector3.one - item.axis;
            if (item.space == PositionSpace.SelfWorld) {
                item.obj.transform.localPosition = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.localPosition, axisReverse);
                item.obj.transform.Translate(currentOffset, Space.Self);
            } else if (item.space == PositionSpace.World) {
                item.obj.transform.position = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.position, axisReverse);
                item.obj.transform.Translate(currentOffset, Space.Self);
            } else {
                rectTransform.anchoredPosition = Vector2.Scale(currentPosition, item.axis) +
                    Vector2.Scale(rectTransform.anchoredPosition, axisReverse);
                // Need test.
                rectTransform.Translate(currentOffset, Space.Self);
            }
            if (item.time >= item.parameter.moveDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }

        private static bool MoveWithTime(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 startPosition = parameter.startPosition + parameter.startOffset;
            Vector3 targetPosition = parameter.targetPosition + parameter.targetOffset;
            Vector3 distance = targetPosition - startPosition;
            float time = 0;
            if (parameter.loop) {
                time = item.time % parameter.moveDuration;
            } else {
                time = Mathf.Min(item.time, parameter.moveDuration);
            }
            if (item.isReverse) {
                time = item.parameter.moveDuration - time;
            }
            Vector3 currentPosition = startPosition +
                distance * parameter.moveCurve.Evaluate(time);
            RectTransform rectTransform = item.obj.GetComponent<RectTransform>();
            Vector2 axisReverse = Vector3.one - item.axis;
            if (item.space == PositionSpace.SelfWorld) {
                item.obj.transform.localPosition = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.localPosition, axisReverse);
            } else if (item.space == PositionSpace.World) {
                item.obj.transform.position = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.position, axisReverse);
            } else {
                rectTransform.anchoredPosition = Vector2.Scale(currentPosition, item.axis) +
                    Vector2.Scale(rectTransform.anchoredPosition, axisReverse);
            }
            if (item.time >= item.parameter.moveDuration) {
                if (!item.parameter.loop) {
                    return true;
                }
            }
            return false;
        }

        private static bool MoveWithTrack(AnimationItem item) {
            AnimationParam parameter = item.parameter;
            Vector3 startPosition = parameter.startPosition + parameter.startOffset;
            if (item.positionList.Count < 1) {
                return true;
            }
            Vector2 currentVertor2 = item.positionList[0];
            Vector3 currentPosition;
            if (item.isTureOver) {
                currentPosition = startPosition +
               new Vector3(-currentVertor2.x* parameter.magnificationTimes
               , currentVertor2.y*parameter.magnificationTimes, 0);
            } else {
                currentPosition = startPosition +
                    new Vector3(currentVertor2.x* parameter.magnificationTimes
                    , currentVertor2.y*parameter.magnificationTimes, 0);
            }
            item.positionList.Remove(currentVertor2);
            RectTransform rectTransform = item.obj.GetComponent<RectTransform>();
            Vector2 axisReverse = Vector3.one - item.axis;
            if (item.space == PositionSpace.SelfWorld) {
                item.obj.transform.localPosition = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.localPosition, axisReverse);
            } else if (item.space == PositionSpace.World) {
                item.obj.transform.position = Vector3.Scale(currentPosition, item.axis) +
                    Vector3.Scale(item.obj.transform.position, axisReverse);
            } else {
                rectTransform.anchoredPosition = Vector2.Scale(currentPosition, item.axis) +
                    Vector2.Scale(rectTransform.anchoredPosition, axisReverse);
            }
            if (item.positionList.Count>0) {
                    return false;
            }
            return true;
        }

        private static bool MoveWithSpeed(AnimationItem item) {
            Vector2 distance = Vector2.zero;
            Vector3 targetPosition = item.parameter.targetPosition + item.parameter.targetOffset;
            RectTransform rectTransform = item.obj.GetComponent<RectTransform>();
            if (item.space == PositionSpace.UI) {
                distance = targetPosition - (Vector3)rectTransform.anchoredPosition;
            } else {
                distance = targetPosition - item.obj.transform.position;
            }
            Vector2 delta = item.parameter.speed * Time.unscaledDeltaTime * item.direction;
            Vector2 offset = Vector2.zero;
            bool isReached = false;
            if (distance.sqrMagnitude > delta.sqrMagnitude) {
                offset = delta;
            } else {
                offset = distance;
                isReached = true;
            }
            if (item.space == PositionSpace.SelfWorld) {
                item.obj.transform.localPosition += (Vector3)offset;
            } else if (item.space == PositionSpace.World) {
                item.obj.transform.position += (Vector3)offset;
            } else {
                rectTransform.anchoredPosition += offset;
            }
            return isReached;
        }

        private static bool MoveWithInitSpeed(AnimationItem item) {
            Vector2 startPosition = item.parameter.startPosition;
            Vector2 targetPosition = item.parameter.targetPosition;
            Vector2 originDirection = targetPosition - startPosition;
            Vector2 direction = (targetPosition - (Vector2)item.obj.transform.position).normalized;
            Vector2 crossDirection = new Vector2(direction.y, -direction.x);
            if (Vector2.Dot(item.parameter.velocity, crossDirection) < 0) {
                crossDirection = new Vector2(-direction.y, direction.x);
            }
            Vector2 velocityDotPositive = Vector2.Dot(item.parameter.velocity, direction) *
                direction;
            Vector2 velocityDotNegative = Vector2.Dot(item.parameter.velocity, crossDirection) *
                crossDirection * item.parameter.crossVelocityFade;
            item.parameter.velocity = velocityDotPositive + velocityDotNegative;
            item.parameter.velocity = item.parameter.velocity.normalized *
                Mathf.Min(item.parameter.velocity.magnitude, maxSpeed);
            float accelerate = item.parameter.accelerateCurve.Evaluate(item.time);
            item.parameter.velocity = direction * accelerate * Time.unscaledDeltaTime +
                item.parameter.velocity;
            Vector2 currentPosition = (Vector2)item.obj.transform.position +
                item.parameter.velocity * Time.unscaledDeltaTime;
            if ((targetPosition - currentPosition).sqrMagnitude < 0.25f ||
                Vector2.Dot(originDirection, direction) < 0) {
                item.obj.transform.position = targetPosition;
                return true;
            } else {
                item.obj.transform.position = currentPosition;
            }
            return false;
        }
    }
}


