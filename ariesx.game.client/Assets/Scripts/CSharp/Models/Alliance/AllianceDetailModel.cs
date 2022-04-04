using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class BaseListInfo {
        public int countShow = 10;
        public int tailIndex = 0;
        public int page = 0;
        public int pageCount = 0;
        public bool isLoadAll = false;

        public BaseListInfo(int pageCount, int countShow) {
            this.pageCount = pageCount;
            this.countShow = countShow;
        }
    }

    public class AllianceMemberWithIndex {
        public AllianceMember member;
        public long abdicationTime;
        public int index;
    }

    public class AllianceDetailChannel : BaseListInfo {
        public List<AllianceMemberWithIndex> membersList =
            new List<AllianceMemberWithIndex>();
        public PlayerPublicInfo selfInfo = null;
        public Alliance alliance;
        public AllianceMemberSortType sortType = AllianceMemberSortType.force;

        public AllianceDetailChannel(int pageCount, int countShow) :
            base(pageCount, countShow) {
        }
    }

    public class FallenPlayerWithIndex {
        public FallenPlayer fallenPlayer;
        public int index;
    }

    public class AllianceSubordinateChannel : BaseListInfo {
        public List<FallenPlayer> subordinates =
            new List<FallenPlayer>();
        public AllianceSubordinateChannel(int pageCount, int countShow) :
            base(pageCount, countShow) {
        }
    }

    public class AllianceCitiesChannel : BaseListInfo {
        public List<NPCCityConf> citiesList = new List<NPCCityConf>();
        public List<MiniMapPassConf> passesList = new List<MiniMapPassConf>();
        public int citiesCount;
        public int passedCount;
        public Dictionary<Resource, int> resourceBuff =
            new Dictionary<Resource, int>();
        public AllianceCitiesChannel(int pageCount, int coutShow) :
            base(pageCount, coutShow) { }
    }

    public class AllianceMarkChannel : BaseListInfo {
        public List<MapMark> allianceMarkList = new List<MapMark>();
        public Dictionary<Vector3, MapMark> allianceMarkDict =
            new Dictionary<Vector3, MapMark>();
        public Vector2 coordinate = Vector2.zero;
        public int state = 0;

        public AllianceMarkChannel(int pageCount, int coutShow) :
            base(pageCount, coutShow) { }
    }

    public class AllianceDetailModel : BaseModel {
        public AllianceViewType allianceViewType = AllianceViewType.None;

        #region allaince detail panel
        public int influnceCondition;
        public JoinConditionType joinCondition = JoinConditionType.Free;
        public string allianceLogo = "1";
        public AllianceMemberWithIndex currentPlayerInfo;
        #endregion

        #region detail panels
        public AllianceDetailChannel allianceDetail;
        public AllianceCitiesChannel allianceCities;
        public AllianceSubordinateChannel subordinates;
        public AllianceMarkChannel allianceMarks;
        #endregion

        public AllianceDetailModel() {
            this.allianceDetail = new AllianceDetailChannel(10, 8);
            this.allianceCities = new AllianceCitiesChannel(15, 9);
            this.subordinates = new AllianceSubordinateChannel(15, 9);
            this.allianceMarks = new AllianceMarkChannel(15, 11);
        }

        public void Refresh(RelationNtf ntf) {
            if (this.allianceDetail.alliance != null) {
                this.allianceDetail.alliance.Name = ntf.AllianceName;
                this.allianceDetail.alliance.Id = ntf.AllianceId;
            } else {
                this.allianceDetail.alliance = new Alliance() {
                    Id = ntf.AllianceId,
                    Name = ntf.AllianceName
                };
            }
        }
    }
}
