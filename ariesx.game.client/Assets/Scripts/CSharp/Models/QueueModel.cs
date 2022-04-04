using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public enum TroopStatus {
        Unconfiged,     // 未配置
        HeroNotFull,    // 可上阵新英雄
        Fatigue,        // 疲劳
        NeedCure,       // 需要治疗
        Recruiting,     // 征兵
        Idle,           // 空闲
        Marching,       // 行军
        NoBlood,
        None,
    }

    public class QueueModel : BaseModel {
        public Dictionary<string, EventMarch> eventMarchDict = 
                new Dictionary<string, EventMarch>(8);

        public void Refresh(LoginAck loginAck) {
            foreach(EventMarch eventMarch in loginAck.EventMarches) {
                this.eventMarchDict.Add(eventMarch.Id, eventMarch);                
            }
        }
    }
}
