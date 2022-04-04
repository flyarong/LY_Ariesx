using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System;


#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.

namespace Poukoute {

    public class TextMeshProUGUIHref : MonoBehaviour, IPointerClickHandler {
        public TextMeshProUGUI m_TextMeshPro;

        [Serializable]
        public class HrefClickEvent : UnityEvent<string, string> { }

        [SerializeField]
        private HrefClickEvent m_OnHrefClick = new HrefClickEvent();
        public HrefClickEvent OnHrefClick {
            get {
                m_OnHrefClick.RemoveAllListeners();
                return m_OnHrefClick;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            // Check if mouse intersects with any links.
            int linkIndex = 
                TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, GameManager.MainCamera);
            
            if (linkIndex != -1) {
                TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
                this.m_OnHrefClick.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText());
            }
        }
    }
}
