using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Poukoute {
    public class CustomClick : MonoBehaviour, IPointerClickHandler {
        public bool interactable = true;
        public UnityEvent onClick = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData) {
            if (this.interactable) {
                this.onClick.Invoke();
            }
        }
    }
}
