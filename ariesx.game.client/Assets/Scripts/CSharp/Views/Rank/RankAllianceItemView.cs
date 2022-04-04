using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class RankAllianceItemView: BaseItemViewsHolder {
        [HideInInspector]
        public RankItemViewType viewType;
        [HideInInspector]
        public float Height {
            get {
                return 90;
            }
        }

        public RankAlliance AllianceRankData {
            get {
                return this.allianceRankData;
            }
            set {
                if (this.allianceRankData != value) {
                    this.allianceRankData = value;
                    this.OnAllianceDataChange();
                }
            }
        }
        private RankAlliance allianceRankData;

        public UnityEvent OnItemClick {
            get {
                this.btnRankInfo.onClick.RemoveAllListeners();
                return this.btnRankInfo.onClick;
            }
        }

        #region UI component cache
        [SerializeField]
        private Image imgRank;
        [SerializeField]
        private TextMeshProUGUI rank;
        [SerializeField]
        private GameObject pnlIncrease;
        [SerializeField]
        private TextMeshProUGUI txtIncrease;
        [SerializeField]
        private GameObject pnlDecrease;
        [SerializeField]
        private TextMeshProUGUI txtDecrease;
        [SerializeField]
        private TextMeshProUGUI txtAllianceName;
        [SerializeField]
        private Image imgAllianceEmblem;
        [SerializeField]
        private TextMeshProUGUI txtRegion;
        [SerializeField]
        private TextMeshProUGUI txtCrown;
        [SerializeField]
        private TextMeshProUGUI txtMember;
        [SerializeField]
        private Button btnRankInfo;
        [SerializeField]
        private Image imgBackground;
        #endregion        

        private void OnAllianceDataChange() {
            if (this.allianceRankData == null) {
                return;
            }
            this.imgRank.sprite = ArtPrefabConf.GetRankIcon(allianceRankData.Rank, this.rank);
            //if (this.allianceRankData.Rank > 3) {
            //    string rankStr = allianceRankData.Rank.ToString();
            //    if (!rankStr.CustomIsEmpty()) {
            //        this.rank.text = rankStr;
            //    } else {
            //        this.rank.text = LocalManager.GetValue(LocalHashConst.rank_no_rank);
            //    }
            //} else {
            //    this.rank.text = string.Empty;
            //}
            this.txtCrown.text = GameHelper.GetFormatNum(allianceRankData.Exp, maxLength: 8);
            this.txtMember.text = string.Concat(
                    allianceRankData.MemberCount, "/", allianceRankData.MaxMemberCount);
            this.txtRegion.text = NPCCityConf.GetMapSNLocalName(allianceRankData.MapSN);
            if (this.viewType == RankItemViewType.OwnRankView) {
                this.txtAllianceName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
            } else {
                this.txtAllianceName.text = string.Format("{0} <color=#fffc9dff>{1}</color>",
                    allianceRankData.Name, GameHelper.GetLevelLocal(allianceRankData.Level));
                this.ResetRangeInfo(allianceRankData.Rank, allianceRankData.RankBefore);
            }
            this.imgAllianceEmblem.sprite =
                ArtPrefabConf.GetAliEmblemSprite(allianceRankData.Emblem);

            // To do: need add alliance flag index.

            bool isForOwn = allianceRankData.Id.CustomEquals(RoleManager.GetAllianceId());
            this.imgBackground.sprite = ArtPrefabConf.GetSprite(
                SpritePath.rankInfoPrefix, isForOwn ? "own" : "normal");

            this.EnableBtnRankInfo();
        }

        private void ResetRangeInfo(int rank, int rankBefore) {
            int rankRange = rankBefore - rank;
            if ((rankRange != 0) || (rankBefore == 0)) {
                bool rankDecrease = (rankRange < 0);
                pnlIncrease.gameObject.SetActiveSafe(!rankDecrease);
                pnlDecrease.gameObject.SetActiveSafe(rankDecrease);
                if (rankDecrease) {
                    this.txtDecrease.text = (-rankRange).ToString();
                } else {
                    this.txtIncrease.text = rankRange.ToString();
                }
            }
        }

        private void EnableBtnRankInfo() {
            if (!this.btnRankInfo.IsInteractable()) {
                this.btnRankInfo.interactable = true;
            }
        }
    }
}
