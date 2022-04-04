using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceMembersViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceMembers.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIAllianceMembers.PnlMembers.PnlTitle.BtnClose")]
        public Button btnClose;

        [Tooltip("UIAllianceMembers.PnlMembers.PnlScroll CustomScrollRect")]
        public CustomScrollRect allianceMembersScrollRect;
        [Tooltip("UIAllianceMembers.PnlMembers.PnlScroll.PnlList")]
        public Transform pnlList;
        [Tooltip("UIAllianceMembers.PnlMembers.PnlScroll.PnlList CustomVerticalLayoutGroup")]
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;


        [Tooltip("UIAllianceMembers.PnlMembers.PnlListSort")]
        public AllianceSortViewPreference allianceSortPre;
        [Tooltip("UIAllianceMembers.PnlMembers.PnlListSort.BtnHint")]
        public Button btnHint;
        [Tooltip("UIAllianceMembers.PnlMembers.PnlListSort.BtnSort")]
        public CustomButton butSortBtn;
    }
}
