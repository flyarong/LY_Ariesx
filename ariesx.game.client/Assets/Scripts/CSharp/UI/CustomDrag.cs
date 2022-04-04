using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class CustomDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        public class DragEvent : UnityEvent<Vector2> { }

        public DragEvent onBeginDrag = new DragEvent();
        public DragEvent onDrag = new DragEvent();
        public DragEvent onEndDrag = new DragEvent();

        public void OnBeginDrag(PointerEventData eventData) {
            this.onBeginDrag.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData) {
            this.onDrag.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData) {
            this.onEndDrag.Invoke(eventData.position);
        }
    }
}

