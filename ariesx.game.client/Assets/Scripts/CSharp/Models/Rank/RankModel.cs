using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public enum RankItemViewType {
        None,
        OwnRankView,
        Others
    }

    public class RankChannel {
        public int countShow = 15;
        public int tailIndex = 0;
        public int page = 0;
        public int pageCount = 0;
        public bool isLoadAll = false;

        public RankChannel(int pageCount, int countShow) {
            this.pageCount = pageCount;
            this.countShow = countShow;
        }
    }

    public class PersonalRank : RankChannel {
        public List<RankPlayer> rankDataList = new List<RankPlayer>(20);
        public RankPlayer ownRankInfo = null;

        public PersonalRank(int pageCount, int countShow) : base(pageCount, countShow) {
        }
    }   

    public class AllianceRank : RankChannel {
        public List<RankAlliance> rankDataList = new List<RankAlliance>();
        public RankAlliance ownRankInfo = null;

        public AllianceRank(int pageCount, int countShow) : base(pageCount, countShow) {
        }
    }

    public class OccupationRank : RankChannel {
        public List<RankAlliance> rankDataList = new List<RankAlliance>();
        public RankAlliance ownRankInfo = null;

        public OccupationRank(int pageCount, int countShow) : base(pageCount, countShow) {
        }
    }

    public class MapSNRank: RankChannel {
        public List<RankPlayer> rankDataList = new List<RankPlayer>(20);
        public RankPlayer ownRankInfo = null;

        public MapSNRank(int pageCount, int countShow) : base(pageCount, countShow) {
        }
    }

    public class RankModel : BaseModel {
        public OccupationRank OccupationRankInfo;
        public AllianceRank AllianceRankInfo;
        public PersonalRank PersonalRankInfo;
        public MapSNRank MapSNRankInfo;

        //public string input;
        public RankModel() {
            this.PersonalRankInfo = new PersonalRank(15, 15);
            this.AllianceRankInfo = new AllianceRank(15, 15);
            this.OccupationRankInfo = new OccupationRank(15, 15);
            this.MapSNRankInfo = new MapSNRank(15,15);
        }
    }
}
