using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Protocol;

namespace Poukoute {
    public class BattleCamera : MonoBehaviour, IBeginDragHandler, IDragHandler {
        private Vector2 previouse;
        public Transform parent;
        public new Camera camera;
        public UnityAction onDrag;

        public void OnBeginDrag(PointerEventData eventData) {
            this.previouse = eventData.position;
        }

        public void OnDrag(PointerEventData eventData) {
            Vector2 currentPos = eventData.position;
            Vector2 delta = currentPos - this.previouse;
            // Debug.LogError(delta.x);
            float tan = Mathf.Abs(delta.x / camera.transform.position.z);
            float radians = Mathf.Atan(tan);
            float degree = Mathf.Sign(delta.x) * radians * 180 / Mathf.PI / 50;
            float finalDegree = this.parent.transform.eulerAngles.y + degree;
            if (finalDegree > 180) {
                finalDegree = finalDegree - 360;
            }
            if (Mathf.Abs(finalDegree) >= 30) {
                finalDegree = Mathf.Sign(finalDegree) * 30;
                degree = finalDegree - this.parent.transform.eulerAngles.y;
            }

            camera.transform.localPosition =
                Vector3.forward * Mathf.Abs(finalDegree) / 30 * (-5) +
                Vector3.right * finalDegree / 30 +
                Vector3.up * Mathf.Abs(finalDegree) / 30 * 0.5f;
            parent.Rotate(Vector3.up, degree);
            this.previouse = currentPos;
            onDrag.InvokeSafe();
        }
    }
}
