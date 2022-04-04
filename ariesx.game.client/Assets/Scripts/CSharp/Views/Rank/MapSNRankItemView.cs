using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using UnityEngine.Events;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class MapSNRankItemView: BaseItemViewsHolder {
        [HideInInspector]
        public RankItemViewType viewType;
        [HideInInspector]
        public float Height {
            get {
                return 90;
            }
        }

        private RankPlayer personalRankData;
        public RankPlayer PersonalRankData {
            get {
                return this.personalRankData;
            }
            set {
                if (personalRankData != value) {
                    personalRankData = value;
                    this.OnRankDataChange();
                }
            }
        }        

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
        private TextMeshProUGUI txtPlayerName;
        [SerializeField]
        private TextMeshProUGUI txtAllianceName;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private TextMeshProUGUI txtRegion;
        [SerializeField]
        private TextMeshProUGUI txtCrown;
        [SerializeField]
        private TextMeshProUGUI txtTerrains;
        [SerializeField]
        private Button btnRankInfo;
        [SerializeField]
        private Image imgBackground;
        #endregion

        private void OnRankDataChange() {
            if (this.personalRankData == null) {
                return;
            }
            this.imgRank.sprite = ArtPrefabConf.GetRankIcon(personalRankData.MapSNRank, this.rank);
            if (personalRankData.AllianceName.Length > 0) {
                this.txtAllianceName.StripLengthWithSuffix(personalRankData.AllianceName);
            } else {
                this.txtAllianceName.text = LocalManager.GetValue(LocalHashConst.free_man);
            }
            this.txtCrown.text = personalRankData.Force.ToString();
            this.txtTerrains.text = personalRankData.PointCount.ToString();
            if (this.viewType == RankItemViewType.OwnRankView) {
                this.txtPlayerName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
                this.imgAvatar.sprite = RoleManager.GetRoleAvatar();
            } else {
                //this.txtPlayerName.StripLengthWithSuffix(personalRankData.Name);
                this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(personalRankData.Avatar);
                this.txtPlayerName.StripLengthWithSuffix(personalRankData.Name);
                this.ResetRangeInfo(personalRankData.MapSNRank, personalRankData.RankBefore);
            }
            // this.txtRegion.StripLengthWithSuffix(NPCCityConf.GetMapSNLocalName(personalRankData.MapSN));
            bool isForOwn = personalRankData.Id.CustomEquals(RoleManager.GetRoleId());
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

