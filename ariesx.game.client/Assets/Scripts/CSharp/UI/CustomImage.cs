using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class CustomImage : Image {
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
            //Debug.LogError("Image");
            return true;// base.IsRaycastLocationValid(screenPoint, eventCamera);
        }
    }
}
