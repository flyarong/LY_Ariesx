using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceListItemView : BaseItemViewsHolder {
        [HideInInspector]
        public int index = 0;
        //[HideInInspector]
        //public string emblemPath;
        public AllianceCache AllianceData {
            get {
                return this.allianceData;
            }
            set {
                if (allianceData != value) {
                    allianceData = value;
                    this.OnAllianceDataChange();
                }
            }
        }
        private AllianceCache allianceData;
        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.btnItemInfo.onClick;
            }
        }


        // ui
        #region UI component cache
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private TextMeshProUGUI txtLevel;
        [SerializeField]
        private TextMeshProUGUI txtLanguage;
        [SerializeField]
        private Image imgIcon;
        //private Image imgIconBoard;
        [SerializeField]
        private TextMeshProUGUI txtDistance;

        [SerializeField]
        private TextMeshProUGUI txtInflunce;
        [SerializeField]
        private TextMeshProUGUI txtMembersInfo;
        [SerializeField]
        private Button btnItemInfo;
        //public Image imgBackground;
        #endregion

        private void OnAllianceDataChange() {
            this.txtName.text = allianceData.Name;
            this.txtLevel.text = GameHelper.GetLevelLocal(allianceData.Level);
            this.txtLanguage.text = LocalManager.GetValue(
                GameConst.ALLIANCE_LANGUAGE,
                AllianceLanguageConf.GetAllianceLanguage(allianceData.Language.ToString())
            );
            this.txtInflunce.text = GameHelper.GetFormatNum(allianceData.Exp);
            this.txtMembersInfo.text =
                string.Concat(allianceData.MemberCount, "/", allianceData.MaxMemberCount);
            this.imgIcon.sprite = ArtPrefabConf.GetAliEmblemSprite(allianceData.Emblem);
            this.SetDistanceInfo();

            //string bgTaile = (index % 2 == 0) ? "bg2" : "bg1";
            //this.imgBackground.sprite = ArtPrefabConf.GetSprite(SpritePath.rankInfoPrefix + bgTaile);
        }

        private void SetDistanceInfo() {
            Vector2 allianceCoord = new Vector2(allianceData.Coord.X, allianceData.Coord.Y);
            float sqrDistance = (allianceCoord - RoleManager.GetRoleCoordinate()).sqrMagnitude;
            string distance = string.Empty;
            Color color = default(Color);
            if (sqrDistance < (float)AllianceSqrDistance.Nearnest) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Nearnest];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Nearnest);
            }
            else if ((sqrDistance > (float)AllianceSqrDistance.Nearnest) &&
                      (sqrDistance < (float)AllianceSqrDistance.Nearer)) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Nearer];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Nearer);
            }
            else if ((sqrDistance > (float)AllianceSqrDistance.Nearer) &&
                      (sqrDistance < (float)AllianceSqrDistance.Near)) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Near];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Near);
            }
            else if ((sqrDistance > (float)AllianceSqrDistance.Near) &&
                      (sqrDistance < (float)AllianceSqrDistance.Medium)) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Medium];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Medium);
            }
            else if ((sqrDistance > (float)AllianceSqrDistance.Medium) &&
                      (sqrDistance < (float)AllianceSqrDistance.Far)) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Far];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Far);
            }
            else if ((sqrDistance > (float)AllianceSqrDistance.Far) &&
                      (sqrDistance < (float)AllianceSqrDistance.Farther)) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Farther];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Farther);
            }
            else if (sqrDistance > (float)AllianceSqrDistance.Farther) {
                distance = AllianceModel.DistanceLocal[AllianceSqrDistance.Farthest];
                color = GameHelper.ToColor((long)AllianceDistanceColor.Farthest);
            }
            this.txtDistance.text = string.Concat(
                LocalManager.GetValue(LocalHashConst.alliance_distance), 
                ": ", distance
            );
            this.txtDistance.color = color;
        }
    }
}
