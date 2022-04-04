using UnityEngine;
using System.Collections.Generic;

namespace Poukoute {
    public class MaterialPath {
        public const string matGray = "Material/MatUIGray";
        public const string matAboveAll = "Material/MatUIAboveAll";
        public const string matScan = "Material/MatUIScan";
        public const string matImageFast = "Material/MatUIFastDefault";
        public const string matUIExPosureStar = "Material/MatUIExPosureStar";

        public const string matMarchOpaquePrefix = "Material/MatLineTilingOpaque";
        public const string matMarchTransparentPrefix = "Material/MatLineTilingTransparent";

        public static readonly List<string> matRarityList = new List<string>() {
            "Material/card01",
            "Material/card02",
            "Material/card03",
            "Material/card04",
            "Material/card05"
        };
    }
}