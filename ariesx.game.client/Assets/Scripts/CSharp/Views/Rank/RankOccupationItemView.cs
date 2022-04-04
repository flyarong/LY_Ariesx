using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class RankOccupationItemView: BaseItemViewsHolder {
        [HideInInspector]
        public RankItemViewType viewType;
        [HideInInspector]
        public float Height {
            get {
                return 90;
            }
        }
        public RankAlliance OccupationRankData {
            get {
                return this.occupationRankData;
            }
            set {
                if (this.occupationRankData != value) {
                    this.occupationRankData = value;
                    this.OnOccupationRankDataChange();
                }
            }
        }
        private RankAlliance occupationRankData;

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
        private Image imgAllianceEmblem;
        [SerializeField]
        private TextMeshProUGUI txtAllianceName;
        [SerializeField]
        private TextMeshProUGUI txtRegion;
        [SerializeField]
        private TextMeshProUGUI txtPoint;
        [SerializeField]
        private TextMeshProUGUI txtCities;
        [SerializeField]
        private Button btnRankInfo;
        [SerializeField]
        private Image imgBackground;
        #endregion



        private void OnOccupationRankDataChange() {
            if (this.occupationRankData == null) {
                Debug.LogError("occupationRankData is null");
                return;
            }
            //occupationRankData.Rank
            this.imgRank.sprite = ArtPrefabConf.GetRankIcon(occupationRankData.OccupationRank, this.rank);
            this.txtPoint.text = occupationRankData.OccupationPoints.ToString();
            this.txtCities.text = occupationRankData.OccupationCount.ToString();
            this.txtRegion.text = NPCCityConf.GetMapSNLocalName(occupationRankData.MapSN);
            this.imgAllianceEmblem.sprite =
                ArtPrefabConf.GetAliEmblemSprite(occupationRankData.Emblem);
            if (this.viewType == RankItemViewType.OwnRankView) {
                this.txtAllianceName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
            } else {
                this.txtAllianceName.text = string.Format("{0} <color=#fffc9dff>{1}</color>",
                occupationRankData.Name, GameHelper.GetLevelLocal(occupationRankData.Level));
                this.ResetRangeInfo(occupationRankData.OccupationRank,
                                    occupationRankData.OccupationRankBefore);
            }

            // To do: need add alliance flag index.
            bool isForOwn = occupationRankData.Id.CustomEquals(RoleManager.GetAllianceId());
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
