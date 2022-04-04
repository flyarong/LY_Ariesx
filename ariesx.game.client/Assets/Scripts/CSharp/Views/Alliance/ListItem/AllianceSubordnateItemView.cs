using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceSubordnateItemView : BaseItemViewsHolder {
        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.btnItemInfo.onClick;
            }
        }

        public FallenPlayer SubordinateData {
            get {
                return this.subordinateData;
            }
            set {
                if (subordinateData != value) {
                    subordinateData = value;
                    this.OnSubordinateDataChange();
                }
            }
        }
        private FallenPlayer subordinateData;


        #region UI component cache
        [SerializeField]
        private Button btnItemInfo;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private TextMeshProUGUI txtOfficial;
        [SerializeField]
        private TextMeshProUGUI txtRegion;
        [SerializeField]
        private TextMeshProUGUI txtCoord;
        [SerializeField]
        private TextMeshProUGUI txtCrown;
        #endregion

        private void OnSubordinateDataChange() {
            FallenPlayer fallenPlayer = subordinateData;
            this.txtName.text = fallenPlayer.Name;
            this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(fallenPlayer.Avatar);
            this.txtOfficial.text = fallenPlayer.AllianceName;
            this.txtRegion.text = NPCCityConf.GetMapSNLocalName(fallenPlayer.MapSN);
            this.txtCoord.text = string.Concat("(", fallenPlayer.Coord.X, ",",
                                fallenPlayer.Coord.Y, ")");
            this.txtCrown.text = fallenPlayer.Force.ToString();
        }
    }
}
