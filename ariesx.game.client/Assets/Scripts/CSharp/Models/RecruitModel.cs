using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class RecruitModel : BaseModel {
        public Troop troop;
        public float recruitSpeed;
        public bool showTips;
        public Dictionary<string, float> armyDict = new Dictionary<string, float>();
    }
}
