using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Poukoute {
    [Serializable]
    public class LoginWorldList {
        public LoginWorld[] worlds;
    }

    [Serializable]
    public class LoginWorld {
        public string name;
        public string world_id;
        public string[] agent_ips;
    }

    public class LoginModel : BaseModel {
        public string name = string.Empty;
        public string state = "1";


        public List<LoginWorld> worldList = new List<LoginWorld>(5);
        public List<object> roleList = new List<object>();
        public List<object> allWorldList = new List<object>();
        public List<Protocol.GetBornPointsAck.Status> bornPoint = 
            new List<Protocol.GetBornPointsAck.Status>(12);

        public void Refresh(object message) {
            ;
        }
    }
}
