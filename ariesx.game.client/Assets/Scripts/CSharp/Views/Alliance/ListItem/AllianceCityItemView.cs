using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceTileItem {
        public ElementType type = ElementType.none;
        public string key = string.Empty;
        public string belong = string.Empty;
    }

    public class AllianceCityItemView : BaseItemViewsHolder {
        [HideInInspector]
        public float Height {
            get {
                return 92;
            }
        }
        //public int index = 0;

        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();

                return this.btnItemInfo.onClick;
            }
        }
        public UnityAction jump = null;
        public UnityAction OnInfo {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.jump;
            }
        }

        //private FallenTarget cityData;
        //private NPCCityConf cityConf;

        // ui
        #region ui element

        [SerializeField]
        private GameObject pnlChosen;

        [SerializeField]
        private GameObject pnlCity;
        [SerializeField]
        private TextMeshProUGUI cityName;
        [SerializeField]
        private Image imgCity;
        [SerializeField]
        private TextMeshProUGUI txtCityAllianceNam;
        [SerializeField]
        private TextMeshProUGUI cityAddr;
        [SerializeField]
        private TextMeshProUGUI txtZoneName;
        [SerializeField]
        private Transform pnlResource;
        [SerializeField]
        private GameObject[] resources;
        [SerializeField]
        private Image[] resourceImages;
        [SerializeField]
        private TextMeshProUGUI[] resourceValues;
        [SerializeField]
        private Button btnItemInfo;
        [SerializeField]
        private Image imgCityType;

        [SerializeField]
        private GameObject pnlPass;
        [SerializeField]
        private TextMeshProUGUI passName;
        [SerializeField]
        private TextMeshProUGUI txtPassAllianceName;
        [SerializeField]
        private TextMeshProUGUI passAddr;

        public Coord coord = new Coord();
        #endregion

        public Vector2 Coordinate {
            get {
                if (this.tileItem.type == ElementType.npc_city) {
                    return this.miniCity.coordinate;
                } else {
                    return this.miniPass.coordinate;
                }
            }
        }

        public string AllianceName {
            get {
                return this.tileItem.belong;
            }
            set {
                if (this.tileItem.belong != value) {
                    this.tileItem.belong = value;
                    this.OnAllianceNameChanged();
                }
            }
        }

        public TextMeshProUGUI TxtAllianceName {
            get {
                if (this.tileItem.type == ElementType.npc_city) {
                    return this.txtCityAllianceNam;
                } else {
                    return this.txtPassAllianceName;
                }
            }
        }

        private AllianceTileItem tileItem;

        //private bool isCity = false;
        //private bool isPass = false;
        private MiniMapCityConf miniCity = null;
        private MiniMapPassConf miniPass = null;

        public void SetItem(string key, ElementType type, string belong,
            UnityAction callback, bool isChosen, bool needChosenEffect = true) {
            if (this.tileItem == null) {
                this.tileItem = new AllianceTileItem();
            }
            if (this.tileItem.key != key) {
                this.tileItem.key = key;
                this.tileItem.type = type;
                this.OnTileItemChange();
            }
            this.AllianceName = belong;
            this.btnItemInfo.onClick.RemoveAllListeners();
            this.btnItemInfo.onClick.AddListener(callback);
            if (needChosenEffect) {
                this.btnItemInfo.onClick.AddListener(() => this.SetItemIsChosen(true));
            }
            this.SetItemIsChosen(isChosen);
        }

        private void OnTileItemChange() {
            this.pnlCity.gameObject.SetActiveSafe(this.tileItem.type == ElementType.npc_city);
            this.pnlPass.gameObject.SetActiveSafe(this.tileItem.type != ElementType.npc_city);
            string spritePath = string.Empty;
            if (this.tileItem.type == ElementType.npc_city) {
                this.imgCityType.sprite = ArtPrefabConf.GetSprite(SpritePath.cityIconPrefix, "city");
                NPCCityConf city = NPCCityConf.GetConf(this.tileItem.key);
                this.miniCity = MiniMapCityConf.GetConf(this.tileItem.key);
                this.cityName.text = string.Concat(
                    NPCCityConf.GetNpcCityLocalName(city.name, city.isCenter),
                    " ", GameHelper.GetLevelLocal(city.level)
                );
                this.cityAddr.text = string.Concat(
                    city.GetNpcCityMapZoneLocal(),
                    "(", miniCity.coordinate.x, ", ",
                    miniCity.coordinate.y, ")"
                );
                this.ResetResourceVisible();
                bool isCapital = city.type.CustomEquals(GameConst.CAPITAL);
                ulong localHash = LocalHashConst.minimap_normal;
                if (isCapital) {
                    this.txtZoneName.gameObject.SetActive(false);
                    spritePath = MiniMap.MINIMAP_DRAGON;
                    this.resources[0].SetActiveSafe(true);
                    this.resourceImages[0].sprite = ArtPrefabConf.GetSprite("resource_icon_all");
                    this.resourceValues[0].text = string.Concat(
                        "+", city.resourceBuff[Resource.Lumber], "/",
                        LocalManager.GetValue(LocalHashConst.time_hour)
                    );
                    localHash = LocalHashConst.minimap_capital;
                } else {
                    this.txtZoneName.gameObject.SetActive(true);
                    int resourceIndex = 0;
                    foreach (Resource resource in city.resourceBuff.Keys) {
                        if (city.resourceBuff[resource] > 0) {
                            this.SetResourceInfo(resource,
                                city.resourceBuff[resource], resourceIndex);
                            resourceIndex++;
                        }
                    }
                    spritePath = MiniMap.MINIMAP_CITY_S;
                    if (city.type.CustomEquals(GameConst.REGION_CAPITAL)) {
                        localHash = LocalHashConst.minimap_state;
                        spritePath = MiniMap.MINIMAP_CITY_L;
                    } else if (city.type.CustomEquals(GameConst.ZONE_CAPITAL)) {
                        localHash = LocalHashConst.minimap_zone;
                        spritePath = MiniMap.MINIMAP_CITY_M;
                    }
                }
                this.txtZoneName.text = LocalManager.GetValue(localHash);
                this.imgCity.sprite = PoolManager.GetSprite(string.Concat(spritePath, "y"));
                this.imgCity.SetNativeSize();
                this.EnableClick();
            } else if (this.tileItem.type == ElementType.pass) {
                this.miniPass = MiniMapPassConf.GetConf(this.tileItem.key);
                this.passName.text = string.Concat(
                    this.miniPass.LocalName, " ",
                    string.Format(
                        LocalManager.GetValue(LocalHashConst.level),
                        this.miniPass.level
                    )
                );
                this.txtPassAllianceName.text = this.AllianceName;
                this.passAddr.text = string.Concat(
                    " (", miniPass.coordinate.x, ", ",
                    miniPass.coordinate.y, ")"
                );
            }
        }


        public void SetItemIsChosen(bool isChosen) {
            this.pnlChosen.SetActiveSafe(isChosen);
        }

        private void SetResourceInfo(Resource resource, int value, int index) {
            // Temp: need configures change.
            if (index >= 2) {
                return;
            }
            this.resources[index].SetActiveSafe(true);
            this.resourceImages[index].sprite =
                ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix,
                resource.ToString().ToLower());
            this.resourceValues[index].text =
                string.Concat("+", value, "/",
                LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void ResetResourceVisible() {
            int resourceImageCount = this.resources.Length;
            for (int i = 0; i < resourceImageCount; i++) {
                this.resources[i].SetActiveSafe(false);
            }
        }

        private void OnAllianceNameChanged() {
            TextMeshProUGUI txtAllianceName = this.TxtAllianceName;
            if (!this.tileItem.belong.CustomIsEmpty()) {
                txtAllianceName.StripLengthWithSuffix(this.tileItem.belong);
                if (this.tileItem.belong == RoleManager.GetAllianceName()) {
                    txtAllianceName.color = Color.blue;
                } else {
                    txtAllianceName.color = Color.red;
                }
            } else {
                txtAllianceName.text = string.Empty;
            }
        }

        private void EnableClick() {
            if (!this.btnItemInfo.IsInteractable()) {
                this.btnItemInfo.interactable = true;
            }
        }
    }
}
