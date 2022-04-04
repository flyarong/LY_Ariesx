using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapBuildingNoticeItemView : MapNoticeItemView {
        #region ui element
        [SerializeField]
        private Image imgBuilding;
        [SerializeField]
        private TextMeshProUGUI txtBuildingName;
        //[SerializeField]
        //private TextMeshProUGUI txtBuildingLevel;
        [SerializeField]
        private TextMeshProUGUI txtBuildingCoord;
        [SerializeField]
        private TextMeshProUGUI txtBuildingStatus;
        [SerializeField]
        private Button btnItem;
        #endregion

        public UnityEvent OnClick {
            get {
                this.btnItem.onClick.RemoveAllListeners();
                return this.btnItem.onClick;
            }
        }

        public void SetItemContent(ElementBuilding building) {
            this.txtBuildingName.text = MapUtils.GetBuildingLocalName(building);
            this.txtBuildingName.text =
                string.Format("{0} <color=#92FF62FF>({1})</color>",
                MapUtils.GetBuildingLocalName(building),
                GameHelper.GetLevelLocal(building.Level + 1));
            //this.txtBuildingLevel.text = GameHelper.GetLevelLocal(building.Level + 1);
            this.imgBuilding.sprite = ArtPrefabConf.GetSprite(
                string.Format(
                    SpritePath.buildingIconFormat,
                    ((ElementType)building.Type).ToString(),
                    Mathf.CeilToInt((building.Level + 1) / ArtConst.buildingInterval))
                );
            this.txtBuildingCoord.text = building.Coord.ToString();
            bool isUpgrading = building.Level > 0;
            this.txtBuildingStatus.text = isUpgrading ?
                LocalManager.GetValue(LocalHashConst.building_upgrade_done) :
                LocalManager.GetValue(LocalHashConst.building_build_done);

        }
    }
}