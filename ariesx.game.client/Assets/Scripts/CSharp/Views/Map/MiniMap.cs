using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using Protocol;
using TMPro;
using System.Collections.Generic;
using ProtoBuf;

namespace Poukoute {
    public enum MinimapItemType {
        Capital,
        State,
        Zone,
        City,
        Pass,
        None
    }
    public class MiniMapItem {
        public GameObject itemGameObject;
        public MinimapItemType type;
    }

    public class MiniMap : MonoBehaviour, IPointerClickHandler {
        #region UI elements
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect")]
        [SerializeField]
        private CustomScrollRect scrollRect;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain")]
        [SerializeField]
        private Transform pnlTerrain;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlMainCity")]
        public Transform pnlMainCity;
        [SerializeField]
        private CanvasGroup mainCityCG;
        [SerializeField]
        private CanvasGroup mainCityExampleCG;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlAllCity")]
        public Transform pnlAllCity;
        [SerializeField]
        private CanvasGroup allCityCG;
        [SerializeField]
        private CanvasGroup allCityExampleCG;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlAllianceMark")]
        [SerializeField]
        private Transform pnlAllianceMark;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlTileList")]
        [SerializeField]
        private Transform pnlTileList;
        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlAlly")]
        [SerializeField]
        private Transform pnlAlly;

        [Tooltip("PnlMiniMap.PnlContent.PnlBubble.Bubble")]
        [SerializeField]
        private Transform pnlBubble;

        [Tooltip("PnlMiniMap.PnlContent.ScrollRect.PnlTerrain.PnlColliders")]
        public Transform pnlColliders;
        [Tooltip("PnlMiniMap.PnlContent.PnlExampleTips.PnlCoordinate.IfCoordinateX")]
        public TMP_InputField ifCoordX;
        [Tooltip("PnlMiniMap.PnlContent.PnlExampleTips.PnlCoordinate.IfCoordinateY")]
        public TMP_InputField ifCoordY;
        [Tooltip("PnlMiniMap.PnlContent.PnlExampleTips.PnlCoordinate.BtnJump")]
        public CustomButton btnJump;

        [Tooltip("PnlMiniMap.PnlContent.PnlZoom.BtnZoomIn")]
        [SerializeField]
        private CustomButton btnZoomIn;
        [Tooltip("PnlMiniMap.PnlContent.PnlZoom.BtnZoomOut")]
        [SerializeField]
        private CustomButton btnZoomOut;
        [SerializeField]
        private Image imgAlly;
        private Texture2D allyTexture2D;
        #endregion


        private float ratio;
        private long lastClickTime = 0;
        private Vector2 lastClickPosition = Vector2.zero;
        private const float doubleInterval = 0.3f;
        private const float doubleDistance = 35;
        private const float miniSize = 3.0f;
        private const float maxSize = 7.0f;
        private const int MAP_NUM = 11;
        private readonly Vector3 scaleVect = new Vector3(1.5f, 2.1f, 1);
        private Matrix4x4 changeMatrix = new Matrix4x4();
        private Matrix4x4 originMatrixInvers = new Matrix4x4();
        private string ownAllianceId = string.Empty;

        private MiniMapModel miniMapModel;
        private MapModel mapModel;
        private Dictionary<Vector2, AllianceOwnedPoint> AllAllianceOwnedPoints {
            get {
                return this.miniMapModel.AllAllianceOwnedPoints;
            }
        }

        private TextMeshProUGUI currentText = null;
        private GameObject miniMapItemName;

        #region miniMapItems for update localScale
        private Dictionary<Vector2, GameObject> allianceMarkGameObjects =
            new Dictionary<Vector2, GameObject>();
        private Dictionary<Vector2, GameObject> tileGameObjects =
            new Dictionary<Vector2, GameObject>();
        private List<Vector2> allyCoordList =
            new List<Vector2>();
        #endregion

        public float Size {
            get {
                return this.pnlTerrain.localScale.x;
            }
            set {
                this.pnlTerrain.localScale = Vector2.one * value;
                this.OnSizeChange();
            }
        }

        public Vector2 MiniCoordinate {
            get {
                return this.mapModel.minCoordinate;
            }
        }

        public Vector2 MaxCoordinate {
            get {
                return this.mapModel.maxCoordinate;
            }
        }
        private Vector2 MiniMapCoordinate {
            get {
                return this.miniMapModel.coordinate;
            }

            set {
                this.miniMapModel.coordinate = value;
            }
        }

        public class MiniMapEvent : UnityEvent<Vector2> { }
        public MiniMapEvent onBubblePositionChange = new MiniMapEvent();
        public class SizeChangeEvent : UnityEvent<bool> { }
        public SizeChangeEvent onSizeChange = new SizeChangeEvent();

        void Awake() {
            this.scrollRect.onDrag.AddListener(this.OnMapPositionChange);
            this.miniMapModel = ModelManager.GetModelData<MiniMapModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();

            this.btnZoomIn.onClick.AddListener(() => {
                if (!this.btnZoomIn.Grayable) {
                    Vector2 coordinate =
                        this.MapToCoordinate(this.scrollRect.content.anchoredPosition);
                    this.Size = maxSize;
                    this.onBubblePositionChange.Invoke(coordinate);
                }
            });
            this.btnZoomOut.onClick.AddListener(() => {
                if (!this.btnZoomOut.Grayable) {
                    Vector2 coordinate =
                        this.MapToCoordinate(this.scrollRect.content.anchoredPosition);
                    this.Size = miniSize;
                    this.onBubblePositionChange.Invoke(coordinate);
                }
            });
            this.ownAllianceId = RoleManager.GetAllianceId();
            this.ifCoordX.characterValidation = TMP_InputField.CharacterValidation.Integer;
            this.ifCoordX.onValueChanged.AddListener((value) => { this.OnIfCoordinateChangeX(value); });
            this.ifCoordY.characterValidation = TMP_InputField.CharacterValidation.Integer;
            this.ifCoordY.onValueChanged.AddListener((value) => { this.OnIfCoordinateChangeY(value); });
#if UNITY_IOS
            this.allyTexture2D = new Texture2D(1400, 1000, TextureFormat.PVRTC_RGBA2, false);
#else
            if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf)) {
                this.allyTexture2D = new Texture2D(1400, 1000, TextureFormat.RGBAHalf, false);
            } else {    
                this.allyTexture2D = new Texture2D(1400, 1000, TextureFormat.RGBA32, false);
            }
#endif
            this.allyTexture2D.anisoLevel = 0;
            this.allyTexture2D.filterMode = FilterMode.Point;
            Debug.LogError(this.allyTexture2D.anisoLevel);
            this.imgAlly.material.mainTexture = allyTexture2D;
        }

        void Start() {
            this.ratio = 500 / mapModel.maxCoordinate.y;
            Quaternion rotation = Quaternion.Euler(0, 0, 45);
            Matrix4x4 originMatrix = new Matrix4x4();
            originMatrix.SetTRS(Vector2.zero, rotation, scaleVect / (this.ratio));
            this.originMatrixInvers = Matrix4x4.Inverse(originMatrix);
            this.Size = miniSize;
            this.OnMiniMapAllianceCoordsChange();
            this.SetAllianceMark();
            this.SetAllTileInfo();
            this.SetAllyInfo();
            this.InitMiniMapPos();
        }

        public void OnPointerClick(PointerEventData eventData) {
            long clickTime = DateTime.UtcNow.Ticks;
            Vector2 clickPosition = eventData.position;
            if ((clickTime - this.lastClickTime) / 10000000f < doubleInterval &&
                (clickPosition - lastClickPosition).sqrMagnitude < doubleDistance * doubleDistance) {
                this.OnDoubleClick(eventData);
            }
            this.lastClickTime = clickTime;
            this.lastClickPosition = clickPosition;
        }

        public void SetMiniMap(Vector2 coordinate) {
            this.ifCoordX.text = Mathf.Round(coordinate.x).ToString();
            this.ifCoordY.text = Mathf.Round(coordinate.y).ToString();
            Vector2 uiPosition = this.CoordinateToMap(coordinate);
            this.scrollRect.SetContentAnchoredPosition(uiPosition);
            AnimationManager.Animate(this.pnlBubble.gameObject, "Shake");
        }

        public void OnMiniMapAllianceCoordsChange() {
            Transform miniMapItem;
            Transform pnlRoot = (this.Size == miniSize) ? this.pnlAllCity : this.pnlMainCity;
            MinimapItemType minimapItemType;
            foreach (var allainceOwnedPoint in this.AllAllianceOwnedPoints) {
                miniMapItem = this.GetGameObjectByName(pnlRoot, allainceOwnedPoint.Key.ToString());
                if (miniMapItem != null) {
                    string[] namePart = miniMapItem.name.CustomSplit('_');
                    if (Enum.IsDefined(typeof(MinimapItemType), namePart[0])) {
                        minimapItemType = (MinimapItemType)Enum.Parse(typeof(MinimapItemType), namePart[0]);
                        this.UpdateItemImageColor(allainceOwnedPoint.Key,
                            miniMapItem.gameObject, minimapItemType);
                    }
                }
            }
        }

        private Transform GetGameObjectByName(Transform pnlRoot, string gameObjectName) {
            foreach (Transform child in pnlRoot) {
                if (child.name.CustomEndsWith(gameObjectName)) {
                    return child;
                }
            }
            return null;
        }

        public void OnAllyCoordsChange() {
            this.SetAllyInfo();
        }

        public void ShowName(Vector2 coordinate, int state, string local) {
            Transform miniMapItem;
            //this.currentText = null;
            if (this.Size == miniSize) {
                miniMapItem = this.GetGameObjectByName(this.pnlMainCity, coordinate.ToString());
                if (miniMapItem == null) {
                    miniMapItem = this.GetGameObjectByName(this.pnlAllCity, coordinate.ToString());
                    if (miniMapItem != null) {
                        this.Size = maxSize;
                        this.onBubblePositionChange.Invoke(coordinate);
                    }
                }
            } else {
                miniMapItem = this.GetGameObjectByName(this.pnlAllCity, coordinate.ToString());
            }
            if (miniMapItem != null) {
                if (this.miniMapItemName == null) {
                    this.LoadMiniMapItemName(miniMapItem);
                } else {
                    this.miniMapItemName.transform.SetParent(miniMapItem, false);
                }
                this.currentText.text = local;
            }
        }

        private void LoadMiniMapItemName(Transform miniMapItem) {
            this.miniMapItemName = UnityEngine.Resources.Load<GameObject>(MINIMAP_ITEM_NAME);
            if (this.miniMapItemName != null) {
                this.miniMapItemName = GameObject.Instantiate(this.miniMapItemName);
                this.miniMapItemName.transform.SetParent(miniMapItem);
                RectTransform miniMapItemNameRT = miniMapItemName.GetComponent<RectTransform>();
                miniMapItemNameRT.localPosition = new Vector3(0f, -17f, 0f);
                miniMapItemNameRT.localScale = Vector3.one;
                this.currentText = this.miniMapItemName.GetComponent<TextMeshProUGUI>();
            }
        }

        public void HideName() {
            if (this.currentText != null) {
                this.currentText.text = string.Empty;
            }
        }


        public static string MINIMAP_DRAGON = "Sprites/v4ui/ui_minimap/minimap_city_xl_";
        public static string MINIMAP_CITY_L = "Sprites/v4ui/ui_minimap/minimap_city_l_";
        public static string MINIMAP_CITY_M = "Sprites/v4ui/ui_minimap/minimap_city_m_";
        public static string MINIMAP_CITY_S = "Sprites/v4ui/ui_minimap/minimap_city_s_";
        public static string MINIMAP_PASS = "Sprites/v4ui/ui_minimap/minimap_pass_";
        public static string MINIMAP_ITEM_NAME = "UI/MiniMap/Items/TxtItemName";
        private void UpdateItemImageColor(Vector2 coord, GameObject itemGameObject,
            MinimapItemType type) {
            AllianceOwnedPoint alliancePoint;
            string minimapItemPath = string.Empty;
            switch (type) {
                case MinimapItemType.Capital:
                    minimapItemPath = MINIMAP_DRAGON;
                    break;
                case MinimapItemType.State:
                    minimapItemPath = MINIMAP_CITY_L;
                    break;
                case MinimapItemType.Zone:
                    minimapItemPath = MINIMAP_CITY_M;
                    break;
                case MinimapItemType.City:
                    minimapItemPath = MINIMAP_CITY_S;
                    break;
                case MinimapItemType.Pass:
                    minimapItemPath = MINIMAP_PASS;
                    break;
                default:
                    break;
            }
            string itemTaile = "y";
            if (this.AllAllianceOwnedPoints.TryGetValue(coord, out alliancePoint)) {
                itemTaile = alliancePoint.AllianceId.CustomEquals(this.ownAllianceId) ? "b" : "r";
            }
            itemGameObject.GetComponent<Image>().sprite =
                PoolManager.GetSprite(string.Concat(minimapItemPath, itemTaile));
        }

        private void InitMiniMapPos() {
            this.SetMiniMap(mapModel.centerCoordinate);
            this.UpdateCoordInfo(mapModel.centerCoordinate);
            this.MiniMapCoordinate = mapModel.centerCoordinate;
        }

        #region MiniMap view objects


        private void SetAllianceMark() {
            List<MapMark> markList = ModelManager.GetModelData<MapMarkModel>().markList;
            Vector3 tmpLocalScale = Vector3.one * (1 / this.Size);
            foreach (MapMark mark in markList) {
                if (mark.type == MapMarkType.Alliance) {
                    GameObject gameObject = PoolManager.GetObject(
                        PrefabPath.pnlMiniMapAllianceMark, this.pnlAllianceMark);
                    gameObject.transform.localScale = tmpLocalScale;
                    gameObject.GetComponent<RectTransform>().anchoredPosition =
                        this.originMatrixInvers * (mark.mark.Coord - MapUtils.Center);
                    this.allianceMarkGameObjects.Add(mark.mark.Coord, gameObject);
                }
            }
        }

        private void SetAllTileInfo() {
            Vector2 townHallCoord = ModelManager.GetModelData<BuildModel>().
                GetBuildCoordinateByName(ElementName.townhall);
            Dictionary<Vector2, Point> tileDict = RoleManager.GetPointDict();
            string tilePrefabPath = string.Empty;
            this.tileGameObjects.Clear();
            Vector3 tmpLocalScale = Vector3.one * (1 / this.Size);
            GameObject townHallObj = PoolManager.GetObject(
                PrefabPath.pnlMiniMapTownHall,
                this.pnlTileList
            );
            townHallObj.transform.localScale = tmpLocalScale;
            townHallObj.GetComponent<RectTransform>().anchoredPosition =
                this.originMatrixInvers * (townHallCoord - MapUtils.Center);
            this.tileGameObjects.Add(townHallCoord, townHallObj);
        }

        private void SetAllyInfo() {
            this.allyTexture2D.SetPixels(new Color[allyTexture2D.width * allyTexture2D.height]);

            GetAllyCoordsReq getAllyCoordReq = new GetAllyCoordsReq();
            NetManager.SendMessage(getAllyCoordReq, typeof(GetAllyCoordsAck).Name, this.OnGetAllyCoord);
        }

        private void OnGetAllyCoord(IExtensible message) {
            GetAllyCoordsAck allyCoord = message as GetAllyCoordsAck;
            GameHelper.ClearChildren(this.pnlAlly);
            //this.AllyCoord.Clear();
            this.allyCoordList.Clear();
            Vector3 tmpLocalScale = Vector3.one * (1 / this.Size);
            int offsetX = this.allyTexture2D.width / 2;
            int offsetY = this.allyTexture2D.height / 2;
            foreach (Coord coord in allyCoord.Coords) {
                Vector2 position = this.originMatrixInvers * (coord - MapUtils.Center) * 2;
                this.allyTexture2D.SetPixel((int)position.x + offsetX, (int)position.y + offsetY, ArtConst.MinimapAlly);
                this.allyCoordList.Add(coord);
            }
            foreach (Coord coord in RoleManager.GetPointDict().Keys) {
                Vector2 position = this.originMatrixInvers * (coord - MapUtils.Center) * 2;
                this.allyTexture2D.SetPixel((int)position.x + offsetX, (int)position.y + offsetY, ArtConst.MinimapSelf);
            }
            this.allyTexture2D.Apply();
        }

        private void UpdateMiniMapOtherGameObjectsScale() {
            Vector3 tmpLocalScale = Vector3.one * (1 / this.Size);
            //foreach (var miniMapItems in this.allyCoordList) {
            //    miniMapItems.Value.transform.localScale = tmpLocalScale;
            //}
            foreach (var miniMapItems in this.tileGameObjects) {
                miniMapItems.Value.transform.localScale = tmpLocalScale;
            }
            foreach (var miniMapItems in this.allianceMarkGameObjects) {
                miniMapItems.Value.transform.localScale = tmpLocalScale;
            }
        }
        #endregion

        private void OnSizeChange() {
            Quaternion rotation = Quaternion.Euler(0, 0, -135);
            bool isMax = this.Size == maxSize;
            this.btnZoomIn.Grayable = isMax;
            this.btnZoomOut.Grayable = !isMax;
            this.changeMatrix.SetTRS(
                Vector2.zero,
                rotation,
                scaleVect / (this.ratio * this.Size)
            );
            bool isMiniSize = (this.Size == miniSize);
            this.onSizeChange.Invoke(!isMiniSize);
            UIManager.SetUICanvasGroupVisible(this.mainCityCG, isMiniSize);
            UIManager.SetUICanvasGroupVisible(this.mainCityExampleCG, isMiniSize);
            UIManager.SetUICanvasGroupVisible(this.allCityCG, !isMiniSize);
            UIManager.SetUICanvasGroupVisible(this.allCityExampleCG, !isMiniSize);
            this.UpdateMiniMapOtherGameObjectsScale();
        }

        private void OnDoubleClick(PointerEventData eventData) {
            Vector2 coordinate =
                this.MapToCoordinate(this.scrollRect.content.anchoredPosition);
            if (this.Size == maxSize) {
                this.Size = miniSize;
            } else {
                this.Size = maxSize;
            }
            this.onBubblePositionChange.Invoke(coordinate);
        }

        private Vector2 tmpInputVector = Vector2.zero;
        void OnIfCoordinateChangeX(string value) {
            if (value.CustomIsEmpty()) {
                return;
            }
            float newCoordX;
            if (float.TryParse(value, out newCoordX)) {
                newCoordX = Mathf.Clamp(newCoordX, this.MiniCoordinate.x, MaxCoordinate.x);
                tmpInputVector.x = newCoordX;
                this.ifCoordX.text = newCoordX.ToString();
                tmpInputVector.y = this.MiniMapCoordinate.y;
                this.onBubblePositionChange.Invoke(tmpInputVector);
            }
        }

        void OnIfCoordinateChangeY(string value) {
            if (value.CustomIsEmpty()) {
                return;
            }
            float newCoordY;
            if (float.TryParse(value, out newCoordY)) {
                tmpInputVector.x = this.MiniMapCoordinate.x;
                newCoordY = Mathf.Clamp(newCoordY, this.MiniCoordinate.y, MaxCoordinate.y);
                this.ifCoordY.text = newCoordY.ToString();
                tmpInputVector.y = newCoordY;
                this.onBubblePositionChange.Invoke(tmpInputVector);
            }
        }

        private void OnMapPositionChange() {
            Vector2 position = this.scrollRect.content.anchoredPosition;
            Vector2 tmpCoordinate = this.MapToCoordinate(position);

            //this.UpdateCoordInfo(tmpCoordinate);
            this.onBubblePositionChange.Invoke(tmpCoordinate);
        }

        private void UpdateCoordInfo(Vector2 newCoordinate) {
            if (Mathf.Round(newCoordinate.x) != Mathf.Round(MiniMapCoordinate.x)) {
                this.ifCoordX.text = Mathf.Round(newCoordinate.x).ToString();
            }
            if (Mathf.Round(newCoordinate.y) != Mathf.Round(MiniMapCoordinate.y)) {
                this.ifCoordY.text = Mathf.Round(newCoordinate.y).ToString();
            }
        }

        private Vector2 MapToCoordinate(Vector2 position) {
            Vector2 coordinate = this.changeMatrix * position;
            return MapUtils.Center + coordinate;
        }

        private Vector2 CoordinateToMap(Vector2 coordinate) {
            coordinate -= MapUtils.Center;
            return Matrix4x4.Inverse(this.changeMatrix) * coordinate;
        }
    }
}
