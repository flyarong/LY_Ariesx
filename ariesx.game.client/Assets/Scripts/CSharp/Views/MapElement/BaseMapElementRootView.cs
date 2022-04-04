using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class BaseMapElementRootView : MonoBehaviour {
        public int maxCount = 0;
        protected List<GameObject> elementList = new List<GameObject>();
        protected static MapModel mapModel;
        protected const float tileLayerInterval = 0.01f;

        protected virtual List<string> DirectionList {
            get;
        }

        protected virtual List<string> ElementTypeList {
            get;
        }

        protected virtual Dictionary<string, Vector2> DirectionDict {
            get;
        }

        protected virtual void Generate() {
        
        }

        protected void OnElementDestroy(GameObject element) {
            PoolManager.RemoveObject(element);
            elementList.Remove(element);
        }
    }
}
