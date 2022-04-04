using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class CustomDrop : MonoBehaviour, IDropHandler {
        public UnityEvent onDrop = new UnityEvent();

        public void OnDrop(PointerEventData eventData) {
            this.onDrop.Invoke();
        }
    }
}
