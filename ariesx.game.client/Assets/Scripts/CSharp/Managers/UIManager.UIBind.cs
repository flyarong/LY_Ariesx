using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public partial class UIManager : MonoBehaviour {

        public static void UIBind(Transform ui, Transform target, Vector2 size,
            BindDirection direction, BindCameraMode cameraMode, Vector2 offset) {
            Instance.bindDict[ui] = new Bind() {
                target = target,
                size = size,
                direction = direction,
                cameraMode = cameraMode,
                offset = offset
            };
        }

        public static void UIBind(Transform ui, Transform target, Vector2 size,
            BindDirection direction, BindCameraMode cameraMode) {
            Instance.bindDict[ui] = new Bind() {
                target = target,
                size = size,
                direction = direction,
                cameraMode = cameraMode
            };
        }

        public static void UIUnBind(Transform ui) {
            if (Instance.bindDict.ContainsKey(ui)) {
                Instance.bindDict.Remove(ui);
            }
        }

        void LateUpdate() {
            if (this.bindDict.Count > 0) {
                this.UpdateBindUI();
            }
        }

        private List<Transform> removeList = new List<Transform>();
        private Vector2 cameraOffset = Vector2.zero;
        private Vector2 offsetNormal = Vector2.zero;
        private Vector2 uiBoundary = Vector2.zero;
        private Vector2 tmpCameraOffset = Vector2.zero;
        private Vector2 sizeInUi = Vector2.zero;
        private RectTransform bindUIRect = null;
        private void UpdateBindUI() {
            bool needFocus = false;
            this.removeList.Clear();
            foreach (var pair in this.bindDict) {
                if (pair.Key == null ||
                   !pair.Key.gameObject.activeInHierarchy ||
                   !pair.Key.gameObject.activeSelf ||
                   !pair.Value.target.gameObject.activeSelf ||
                   !pair.Value.target.gameObject.activeInHierarchy) {
                    this.removeList.Add(pair.Key);
                    continue;
                }
                switch (pair.Value.direction) {
                    case BindDirection.Up:
                        offsetNormal = Vector2.up;
                        break;
                    case BindDirection.Down:
                        offsetNormal = Vector2.down;
                        break;
                    case BindDirection.Left:
                        offsetNormal = Vector2.left;
                        break;
                    case BindDirection.Right:
                        offsetNormal = Vector2.right;
                        break;
                    case BindDirection.Center:
                        offsetNormal = Vector2.zero;
                        break;
                }
                float cameraHeght = GameManager.MainCamera.orthographicSize * 2;
                float cameraWidth = GameManager.MainCamera.aspect * cameraHeght;
                this.sizeInUi.x = pair.Value.size.x / cameraWidth * UIRect.width;
                this.sizeInUi.y = pair.Value.size.y / cameraHeght * UIRect.height;
                this.bindUIRect = pair.Key.GetComponent<RectTransform>();
                Vector2 uiSize = this.bindUIRect.rect.size;
                Vector2 offsetInUi =
                    Vector2.Scale((sizeInUi + uiSize) / 2, offsetNormal) + pair.Value.offset;
                Vector2 uiPosition = MapUtils.WorldToUIPoint(pair.Value.target.position);
                this.bindUIRect.anchoredPosition = uiPosition + offsetInUi;
                uiPosition = this.bindUIRect.anchoredPosition;
                if (pair.Value.cameraMode == BindCameraMode.Focus) {
                    this.tmpCameraOffset = Vector2.zero;
                    if (uiPosition.x + uiSize.x / 2 > UIRect.width) {
                        this.tmpCameraOffset.x += (uiPosition.x + uiSize.x / 2 - UIRect.width);
                        needFocus = true;
                    } else if (uiPosition.x - uiSize.x / 2 < 0) {
                        this.tmpCameraOffset.x -= (uiSize.x / 2 - uiPosition.x);
                        needFocus = true;
                    }

                    this.uiBoundary.x = uiPosition.y + uiSize.y / 2 + 120;
                    this.uiBoundary.x = uiPosition.y - uiSize.y / 2;
                    if (uiBoundary.x > 0) {
                        this.tmpCameraOffset.y += uiBoundary.x;
                        needFocus = true;
                    } else if (uiBoundary.y < -UIRect.height) {
                        this.tmpCameraOffset.y += uiBoundary.y + UIRect.height;
                        needFocus = true;
                    }

                    if (cameraOffset.sqrMagnitude < this.tmpCameraOffset.sqrMagnitude) {
                        cameraOffset = this.tmpCameraOffset;
                    }
                }
            }

            if (needFocus) {
                Vector2 uiTarget = MapUtils.WorldToUIPoint(GameManager.MainCamera.transform.position) + cameraOffset;
                Vector2 target = MapUtils.UIToWorldPoint(uiTarget);
                Vector2 coordinate = MapUtils.PositionToCoordinate(target);
                TriggerManager.Invoke(Trigger.CameraMove, coordinate);
            }

            foreach (Transform transform in removeList) {
                this.bindDict.Remove(transform);
            }
        }
    }
}
