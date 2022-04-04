using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    

	public class MissionModel : BaseModel {
        /* Add data member in this */
        public int complishedMissionCount = -1;
        public List<Task> taskList;
		/***************************/
		public void Refresh(object message) {
            
			/* Refresh your data in this function */
		}
	}
}
