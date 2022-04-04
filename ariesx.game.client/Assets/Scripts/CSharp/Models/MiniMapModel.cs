using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
	public class MiniMapModel : BaseModel {
        /* Add data member in this */
        public Vector2 coordinate = Vector2.zero;
        public int state = 0;

        public int cityCountShow = 7;
        public int cityTailIndex = -1;

        public int passCountShow = 7;
        public int passTailIndex = -1;

        public int tileCountShow = 7;
        public int tileTailIndex = -1;
        /***************************/
        public List<Coord> AllyCoord = new List<Coord>(10);
        public Dictionary<Vector2, AllianceOwnedPoint> AllAllianceOwnedPoints = 
            new Dictionary<Vector2, AllianceOwnedPoint>(200);
    }
}
