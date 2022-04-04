using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class BaseViewPreference : MonoBehaviour {
        public GameObject showObj;
#if UNITY_EDITOR
        [HideInInspector]
        public string path;
        public  Dictionary<string, GameObject> pathDict = new Dictionary<string, GameObject>();
#endif

    }
}
