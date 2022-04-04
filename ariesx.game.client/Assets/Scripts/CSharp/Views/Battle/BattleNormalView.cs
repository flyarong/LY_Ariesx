using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class BattleNormalView : MonoBehaviour {

        public void OnActionEnd() {
            PoolManager.RemoveObject(this.gameObject);
        }
    }
}
