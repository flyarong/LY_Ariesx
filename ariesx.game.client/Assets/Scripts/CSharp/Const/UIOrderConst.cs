using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {

    public class UIOrderConst {
        public static readonly Dictionary<string, int> orderDict = new Dictionary<string, int> {
        {"UIOpenChest",  50},
        {"UITierUp", 50 }
    };

        public const int UIBackground = 0;
        public const int UIAbove = 101;
        public const int UIFte = 102;
        public const int UIAlert = 103;
        public const int UIDebug = 104;

        public static int GetUIOrder(string uiName) {
            int order;
            if (orderDict.TryGetValue(uiName, out order)) {
                return order;
            } else {
                return 1;
            }
        }
    }

}