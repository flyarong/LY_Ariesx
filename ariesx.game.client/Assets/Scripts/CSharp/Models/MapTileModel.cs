using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public enum TilePage {
        None = 0,
        Main,
        Sub,
        March,
        Monster
    }

    public class MapTileModel : BaseModel {
        Point point;
        public TilePage page = TilePage.Main; 
        public MapTileInfo tileInfo;
        public Troop currentTroop;
        public EventMarchClient currentMarch;

        public List<Troop> currentTroopList = new List<Troop>();
        public List<GetPointPlayerTroopsAck.Troop> playerTroopList = new List<GetPointPlayerTroopsAck.Troop>();
        public Dictionary<string, GetPointPlayerTroopsAck.Troop> playerTroopDict =
            new Dictionary<string, GetPointPlayerTroopsAck.Troop>();
        //public List<string> mainKeyList = new List<string> {
        //    "name",
        //    "level",
        //    "belong"
        //};

        //public List<string> subKeyList = new List<string> {
        //    "food",
        //    "lumber",
        //    "marble",
        //    "steel",
        //    "crystal"
        //};
    }
}
