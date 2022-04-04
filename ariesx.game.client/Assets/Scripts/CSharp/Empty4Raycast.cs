using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class Empty4Raycast : MaskableGraphic {
        protected Empty4Raycast() {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill) {
            toFill.Clear();
        }
    }
}
