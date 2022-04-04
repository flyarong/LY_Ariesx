using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
	public class TreasureMapModel : BaseModel {
        /* Add data member in this */
        public TreasureConf treasureConf;
        public int level;
		/***************************/
		public void Refresh(object message) {
			/* Refresh your data in this function */
		}
	}
}
