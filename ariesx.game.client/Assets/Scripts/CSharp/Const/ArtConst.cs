using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Poukoute {

    public class ArtConst {
        //Troop Select Color
        public static Color TroopYellow = new Color(225 / 255f, 225 / 255f, 0 / 255f);
        public static Color TroopGreen = new Color(0 / 255f, 225 / 255f, 0 / 255f);
        public static Color TroopBlue = new Color(82 / 255f, 237 / 255f, 255 / 255f);
        public static Color TroopPurple = new Color(248 / 255f, 168 / 255f, 255 / 255f);

        public static Color MinimapAlly = new Color(0, 172/255f, 243/255f);
        public static Color MinimapSelf = new Color(130 / 255f, 1, 109/ 255f);

        public static Color Orange = new Color(248 / 255f, 171 / 255f, 80 / 255f);
        public static Color Red = new Color(220 / 255f, 79 / 255f, 56 / 255f);

        public static Color ChatMsgOwnColor = new Color(73 / 255f, 97 / 255f, 16 / 255f);
        public static Color ChatMsgOtherColor = new Color(176 / 255f, 90 / 255f, 15 / 255f);


        public static Dictionary<string, Color> battleReportColor = new Dictionary<string, Color> {
            {"self", new Color(205 / 255f, 240 / 255f, 236 / 255f) },
            {"enemy", new Color(232 / 255f, 223 / 255f, 228 / 255f) }
        };

        public static float buildingInterval = 4.1f;

        private static Color victoryTopColor = new Color(0.996f, 0.4f, 0f);
        private static Color victoryBottomColor = new Color(0.996f, 0.678f, 0f);
        private static Color failureTopColor = new Color(0.275f, 0.427f, 0.529f);
        private static Color failureBottomColor = new Color(0.549f, 0.675f, 0.761f);
        public static VertexGradient victoryVertexGradient = new VertexGradient(victoryTopColor, victoryTopColor,
                victoryBottomColor, victoryBottomColor);
        public static VertexGradient failureVertexGradient = new VertexGradient(failureTopColor, failureTopColor,
            failureBottomColor, failureBottomColor);
    }
}
