using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MiniMapTileItemView : BaseItemViewsHolder {
        [HideInInspector]
        public float Height {
            get {
                return 92;
            }
        }

        // ui
        #region ui element
        [SerializeField]
        private Image imgHighlight;
        [SerializeField]
        private Image imgResource;
        [SerializeField]
        private TextMeshProUGUI tileInfo;
        [SerializeField]
        private TextMeshProUGUI tileAddr;
        [SerializeField]
        private CanvasGroup resourceCG;
        [SerializeField]
        private Image[] imgResourceProducts;
        [SerializeField]
        private TextMeshProUGUI[] txtResourceProducts;
        [SerializeField]
        private Button btnItemInfo;
        #endregion


        private Point tile;
        public Point Tile {
            get {
                return tile;
            }
            set {
                if (this.tile != value) {
                    this.tile = value;
                    this.OnTileChange();
                }
            }
        }

        public void SetItemViewInfo(Point tile, UnityAction callback, bool isHighlight) {
            this.Tile = tile;
            this.btnItemInfo.onClick.RemoveAllListeners();
            this.btnItemInfo.onClick.AddListener(callback);
            this.btnItemInfo.onClick.AddListener(() => this.SetHighlight(true));
            this.SetHighlight(isHighlight);
        }

        private void SetResourceInfo(Resource resource, int value, int index) {
            this.txtResourceProducts[index].transform.parent.gameObject.SetActiveSafe(true);
            this.imgResourceProducts[index].sprite =
                ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix, 
                resource.ToString().ToLower());
            this.txtResourceProducts[index].text = 
                string.Concat(value, "/", 
                LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void OnTileChange() {
            UIManager.SetUICanvasGroupVisible(this.resourceCG, true);
            MapBasicTypeConf typeConf =
                MapBasicTypeConf.GetConf(this.tile.ElementType.ToString());
            MapResourceProductionConf resConf = MapResourceProductionConf.GetConf(
                typeConf.type + this.tile.Resource.Level
            );

            this.tileInfo.text = string.Concat(
                LocalManager.GetValue("resource_", resConf.type),
                "  ",
                GameHelper.GetLevelLocal(tile.Resource.Level)
            );
            this.tileAddr.text = string.Concat(
                NPCCityConf.GetMapSNLocalName(tile.MapSN), ",",
                NPCCityConf.GetZoneSnLocalName(tile.MapSN,tile.ZoneSN),
                "  (", tile.Coord.X, ", ", tile.Coord.Y, ")"
            );

            this.imgResource.sprite = ArtPrefabConf.GetSprite(
                SpritePath.resourceIconPrefix + resConf.type
            );

            int index = 0;
            foreach(var value in resConf.productionDict) {
                this.SetResourceInfo(value.Key, value.Value, index++);
            }
            for(int i = resConf.productionDict.Count; i < imgResourceProducts.Length; i++) {
                this.txtResourceProducts[i].transform.parent.gameObject.SetActive(false);
            }
            this.EnableClick();
        }

        private void EnableClick() {
            if (!this.btnItemInfo.IsInteractable()) {
                this.btnItemInfo.interactable = true;
            }
        }

        public void SetHighlight(bool isHighlight) {
            this.imgHighlight.gameObject.SetActive(isHighlight);
        }

    }
}
