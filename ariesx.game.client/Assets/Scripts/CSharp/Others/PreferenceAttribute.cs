using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class PreferenceAttribute : PropertyAttribute {
        public readonly string preferencePath;

        public PreferenceAttribute(string preferencePath) {
            this.preferencePath = preferencePath;
#if UNITY_EDITOR
#endif
        }

    }
}
