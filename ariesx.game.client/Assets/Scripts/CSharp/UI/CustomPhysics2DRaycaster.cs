using Protocol;
using System.Collections.Generic;

namespace UnityEngine.EventSystems {
    /// <summary>
    /// Simple event system using physics raycasts.
    /// </summary>
    [AddComponentMenu("Event/Physics 2D Raycaster Custom")]
    [RequireComponent(typeof(Camera))]
    public class CustomPhysics2DRaycaster : PhysicsRaycaster {
        protected CustomPhysics2DRaycaster() { }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
            if (eventCamera == null)
                return;

            var ray = eventCamera.ScreenPointToRay(eventData.position);
            float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            var hits = Physics2D.GetRayIntersectionAll(ray, dist, finalEventMask);


            if (hits.Length != 0) {
                for (int b = 0, bmax = hits.Length; b < bmax; ++b) {
                    var sr = hits[b].collider.gameObject.GetComponent<SpriteRenderer>();
                    var result = new RaycastResult {
                        gameObject = hits[b].collider.gameObject,
                        module = this,
                        distance = (eventCamera.transform.position.z - hits[b].transform.position.z).Abs(), 
                        worldPosition = hits[b].point,
                        worldNormal = hits[b].normal,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        sortingLayer = sr != null ? sr.sortingLayerID : 0,
                        sortingOrder = sr != null ? sr.sortingOrder : 0
                    };
            //        Debug.LogError(b + ":" + result.gameObject.name + ":" + result.distance);
                    resultAppendList.Add(result);
                }
            }
        }
    }
}