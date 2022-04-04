using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {

    public class ListItemView : BaseView {
        private float height;
        [HideInInspector]
        public virtual float Height {
            get {
                return height;
            }

            set {
                height = value;
            }
        }
    }
}
