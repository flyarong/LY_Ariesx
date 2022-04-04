using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute { 
    public class FteModel : BaseModel {
        /* Add data member in this */
        public List<string> fteList = new List<string>();

        public string curTroop = string.Empty;
        public string curTroopName = string.Empty;
        public string curTargetName = string.Empty;
        public Vector2 curTargetCoord = default(Vector2);
        public int curTaskId = 0;
        public string curLotteryGroup = string.Empty;

        public string curBuild = string.Empty;

        public string curHero = string.Empty;

        public Vector2 coorDemonOrigin = Vector2.zero;
        public Vector2 coorDemonTarget = Vector2.zero;

        public Vector2 coorElfOrigin = Vector2.zero;
        public Vector2 coorElfTarget = Vector2.zero;

        public bool demonVsDragon = false;
        /***************************/

		public void Refresh(LoginAck loginAck) {
            /* Refresh your data in this function */
          //  this.fteList = loginAck.FteSteps;
		}

        public FteModel() {
            
        }
	}
}
