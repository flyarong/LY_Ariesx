using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute
{
    public enum DailyAdvise {
        TileCount,
        BuildArray,
        GetTribute,
        Treasure,
        HeroLevelUp,
        Force,
        DailyReward,
        DailyTask,
        Null
    }
    public class HouseKeeperDailyModel : BaseModel
    {
        public int treasureMapAmount;
    }
}

