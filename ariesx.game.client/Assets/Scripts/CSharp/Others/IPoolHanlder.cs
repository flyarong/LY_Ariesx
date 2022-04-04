using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public interface IPoolHandler {
        void OnInPool();
        void OnOutPool();
    }
}
