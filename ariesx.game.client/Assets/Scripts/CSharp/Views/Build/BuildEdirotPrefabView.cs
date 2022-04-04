using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class BuildEdirotPrefabView : MonoBehaviour {
        [Tooltip("BuildEditor.Building")]
        public GameObject building;
        [Tooltip("BuildEditor.Building SpriteRenderer")]
        public SpriteRenderer buildSprite;
        [Tooltip("BuildEditor.Base SpriteRenderer")]
        public SpriteRenderer buildingBase;
        [Tooltip("BuildEditor.Arrow")]
        public GameObject buildArrow;
        [Tooltip("BuildEditor CustomDrag")]
        public CustomDrag drag;
    }
}
