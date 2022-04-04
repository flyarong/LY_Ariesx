using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace Poukoute{
    public class AllianceInfoModel : BaseModel {
        public Alliance viewAlliance = null;
        public AllianceMemberSortType sortType = AllianceMemberSortType.force;
        public bool isLoadAll = false;
        public List<AllianceMemberWithIndex> membersList = 
            new List<AllianceMemberWithIndex>(10);
        public AllianceMemberWithIndex currentPlayerInfo = null;


        public int countShow = 10;
        public int tailIndex = 0;
        public int page = 0;
        public int pageCount = 0;
    }
}