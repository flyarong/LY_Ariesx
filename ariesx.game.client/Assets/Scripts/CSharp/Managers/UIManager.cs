using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public enum BindDirection {
        Up,
        Down,
        Right,
        Left,
        Center
    };

    public enum BindCameraMode {
        None,
        Focus,
        Follow
    }

    public enum UICanvas {
        Main,
        Above,
        Alert
    }

    public enum UIGroup {
        MapTile,
        Normal,
        MapAbove,
        Hero,
        MapNotice
    }

    public class Bind {
        public Transform target;
        public Vector2 size;
        public BindDirection direction;
        public BindCameraMode cameraMode;
        public Vector2 oldPosition = new Vector2(Mathf.Infinity, Mathf.Infinity);
        public Vector2 offset = Vector2.zero;
    }

    public partial class UIManager : MonoBehaviour {
        private static UIManager self;
        public static UIManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("UIManager is not initialized");
                }
                return self;
            }
        }

        public static int LayerUIInvisibleIndex = -1;
        public static int LayerUIIndex = -1;
        public static int LayerAboveUIIndex = -1;

        #region UI Serializefield
        [SerializeField]
        private Canvas uiBattleAboveCanvas;
        [SerializeField]
        private CanvasScaler uiBattleAboveCanvasScaler;
        [SerializeField]
        private GameObject pnlBattle;
        [SerializeField]
        private BattlePrerenderView battlePrerenderView;

        [SerializeField]
        private RectTransform uiChestAbove;
        [SerializeField]
        private CanvasScaler uiChestAboveCanvasScaler;
        [SerializeField]
        private Canvas uiChestAboveCanvas;
        private Rect uiChestAboveRect;

        [SerializeField]
        public RectTransform uiAlert;
        [SerializeField]
        private CanvasScaler uiAlertCanvasScaler;
        [SerializeField]
        private GameGlobalTips gameGlobalTips;
        [SerializeField]
        private AlertContentWithTips alertMessageWithTips;
        #endregion

        // UI global mask.
        private GameObject uiMask;

        // Loading Image.
        private GameObject uiSplash;
        private Transform uiLoading;
        private CustomSlider sldProgress;
        private TextMeshProUGUI txtProgress;

        private readonly Dictionary<UIGroup, List<BaseView>> uiDict =
            new Dictionary<UIGroup, List<BaseView>>(20);
        private readonly Dictionary<UIGroup, int> uiGroupOrder =
            new Dictionary<UIGroup, int>() {
            {UIGroup.MapTile, 3},
            {UIGroup.Normal, 25 },
            {UIGroup.MapAbove, 24 },
            {UIGroup.Hero, 50 },
            {UIGroup.MapNotice, 75}
        };

        private GameObject uiBackground;
        private GameObject uiRenderCamera;
        private GameObject battleRenderCamera;
        private GameObject chestCamera;

        public static GameObject ChestCamera {
            get {
                return Instance.chestCamera;
            }
            set {
                Instance.chestCamera = value;
                if (Instance.adaptiveParam == 0) {
                    Instance.chestCamera.GetComponent<Camera>().fieldOfView =
                        13.44f * (1 / GameManager.MainCamera.aspect);
                }
            }
        }

        //public static GameObject Base {
        //    get {
        //        return Instance.transform.Find("UIBase").gameObject;
        //    }
        //}

        private int adaptiveParam = 1;
        public static int AdaptiveParam {
            get {
                return Instance.adaptiveParam;
            }
        }

        private int fakeBackgroundPref = 0;
        private readonly Dictionary<Transform, Bind> bindDict = new Dictionary<Transform, Bind>(5);

        void Awake() {
            self = this;
            this.uiSplash = GameObject.FindGameObjectWithTag("Splash");
            this.uiLoading = uiSplash.transform.Find("Loading");
            this.sldProgress = this.uiLoading.Find("PnlProgress").GetComponent<CustomSlider>();
            this.sldProgress.onValueChanged.AddListener(this.UpdateProgressText);
            this.txtProgress = this.sldProgress.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            if (GameManager.MainCamera.aspect < (9 / 16f)) {
                adaptiveParam = 0;
            }
            LayerUIInvisibleIndex = LayerMask.NameToLayer("UIInvisible");
            LayerUIIndex = LayerMask.NameToLayer("UI");
            LayerAboveUIIndex = LayerMask.NameToLayer("AboveUI");
            // Loading
            this.InnerInit();
            base.StartCoroutine(this.DelayInit());
        }

        private void InnerInit() {
            // To do : Separate the UIAlert away from UIManager.
            this.uiChestAboveCanvasScaler.matchWidthOrHeight = this.adaptiveParam;
            this.uiBattleAboveCanvasScaler.matchWidthOrHeight = this.adaptiveParam;
            //SetBattleAboveUIVisible(false);

            this.uiAlertCanvasScaler.matchWidthOrHeight = this.adaptiveParam;

            this.uiBackground = UIManager.GetUI("UIBackground");
            GameHelper.SetCanvasCamera(this.uiBackground.GetComponent<Canvas>());
            this.uiBackground.GetComponent<CanvasScaler>().matchWidthOrHeight = this.adaptiveParam;
            //this.uiBackground.SetActiveSafe(false);
            GameHelper.SetLayer(this.uiBackground, LayerUIInvisibleIndex);

            Transform uiMaskParent = GameObject.FindWithTag("Mask").transform;
            GameHelper.SetCanvasCamera(uiMaskParent.GetComponent<Canvas>());
            uiMaskParent.GetComponent<CanvasScaler>().matchWidthOrHeight = this.adaptiveParam;
            this.uiMask = uiMaskParent.Find("Mask").gameObject;
            SetUIVisible(this.uiMask, false);

            this.uiRenderCamera = GameManager.MainCamera.transform.Find("RenderCamera").gameObject;
            RenderTexture uiTexture = this.uiRenderCamera.GetComponent<Camera>().targetTexture;
            uiTexture.width = Mathf.RoundToInt(GameManager.MainCamera.aspect * uiTexture.height);
            this.uiRenderCamera.SetActiveSafe(false);

            this.battleRenderCamera = GameManager.MainCamera.transform.Find("BattlePrerenderCamera").gameObject;
            RenderTexture battleTexture = this.battleRenderCamera.GetComponent<Camera>().targetTexture;
            battleTexture.width = Mathf.RoundToInt(GameManager.MainCamera.aspect * battleTexture.height);
            this.battleRenderCamera.SetActive(false);
        }

        private IEnumerator DelayInit() {
            yield return YieldManager.EndOfFrame;
            this.uiChestAboveRect = this.uiChestAbove.rect;
        }

        public static Rect UIRect {
            get {
                if(Instance == null) return new Rect(375, -667, 750, 1334);
                return Instance.uiChestAboveRect;
            }
        }

        public static void HideUiSplash() {
            Instance.uiSplash.gameObject.SetActive(false);
        }

        public static GameObject GetUI(string name, UICanvas root = UICanvas.Main) {
            Transform parent = null;
            string[] uiNames = name.CustomSplit('.');

            switch (root) {
                case UICanvas.Main:
                    parent = self.transform;
                    break;
                case UICanvas.Above:
                    parent = Instance.uiChestAbove;
                    break;
                case UICanvas.Alert:
                    parent = Instance.uiAlert;
                    break;
            }
            GameObject ui = null;
            foreach (string uiName in uiNames) {
                Transform uiTransform = parent.transform.Find(uiName);
                if (uiTransform == null) {
                    GameObject uiObj = PoolManager.GetObject(
                        string.Concat("UI/Root/", uiName), parent);
                    if (uiObj == null) {
                        Debug.LogError("UiName:" + uiName + "in UI:" + name + " not found");
                        return null;
                    }
                    uiTransform = uiObj.transform;
                    uiTransform.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                    uiTransform.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                }
                parent = uiTransform;
            }
            ui = parent.gameObject;
            return ui;
        }

        public static void SetChestAboveUIVisible(bool visible) {
            //Debug.LogError("Set ui chestAbove canvase " + visible);
            Instance.uiChestAboveCanvas.enabled = visible;
        }

        public static void SetBattleAboveUIVisible(bool visible) {
            //Debug.LogError("Set battleAbove canvase " + visible);
            Instance.uiBattleAboveCanvas.enabled = visible;
        }

        public static void ShowAboveUIBattle(UnityAction callback, UnityAction afterShowCallback) {
            SetBattleAboveUIVisible(true);
            Instance.battlePrerenderView.Play(callback, afterShowCallback);
        }

        #region UI operate
        public static void HideUICanvasGroup(CanvasGroup canvasGroup) {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public static void HideUI(GameObject ui, BaseView baseView = null) {
            Canvas canvas = ui.GetComponent<Canvas>();
            if (canvas != null && baseView != null) {
                List<BaseView> uiList;
                if (Instance.uiDict.TryGetValue(baseView.group, out uiList)) {
                    uiList.TryRemove(baseView);
                }
                canvas.sortingOrder = 1;
            }
            CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
            if (canvasGroup != null) {
                UIManager.HideUICanvasGroup(canvasGroup);
            }
            GraphicRaycaster graphicRaycaster = ui.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null) {
                graphicRaycaster.enabled = false;
            }
        }

        public static void ShowUIAtTop(GameObject ui, BaseView baseView = null) {
            UIManager.ShowUI(ui);
            if (baseView != null && baseView.canvas != null) {
                List<BaseView> uiList;
                if (Instance.uiDict.TryGetValue(baseView.group, out uiList)) {
                    uiList.TryRemove(baseView);
                } else {
                    uiList = new List<BaseView>();
                    Instance.uiDict.Add(baseView.group, uiList);
                }
                uiList.Add(baseView);
                Instance.ReorderUI();
            }
        }

        private void ReorderUI() {
            foreach (var pair in this.uiDict) {
                for (int i = 0; i < pair.Value.Count; i++) {
                    pair.Value[i].canvas.sortingOrder = i + this.uiGroupOrder[pair.Key];
                }
            }
        }

        public static void ShowUI(GameObject ui) {
            CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
            if (canvasGroup != null) {
                UIManager.SetUICanvasGroupEnable(canvasGroup, true);
            }
            GraphicRaycaster graphicRaycaster = ui.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null) {
                graphicRaycaster.enabled = true;
            }
        }

        public static bool IsUIVisible(CanvasGroup canvasGroup) {
            return canvasGroup.alpha.Equals(1);
        }

        public static bool IsUIVisible(GameObject ui) {
            CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
            return canvasGroup.alpha.Equals(1);
        }

        public static void SetUICanvasGroupEnable(CanvasGroup canvasGroup, bool isEnable) {
            canvasGroup.alpha = isEnable ? 1 : 0;
            canvasGroup.interactable = isEnable ? true : false;
            canvasGroup.blocksRaycasts = isEnable ? true : false;
        }

        public static void SetUICanvasGroupVisible(CanvasGroup canvasGroup, bool isVisible) {
            canvasGroup.alpha = isVisible ? 1 : 0;
        }

        public static void SetUIVisible(GameObject ui, bool visible) {
            if (visible) {
                UIManager.ShowUI(ui);
            } else {
                UIManager.HideUI(ui);
            }
        }

        public static void SetMaskVisible(bool visible) {
            SetUIVisible(Instance.uiMask, visible);
        }

        public static void ShowFakeBack(bool isVisible) {
            
            int layer = 1 << LayerUIInvisibleIndex;
            if (isVisible) {
                if (Instance.uiBackground.layer == LayerUIInvisibleIndex) {
                    GameHelper.SetLayer(Instance.uiBackground, LayerUIIndex);
                    Instance.uiRenderCamera.SetActiveSafe(true);
                    Instance.HideUIRenderCamera();
                    GameManager.MainCamera.cullingMask &= ~(layer);
                }
                Instance.fakeBackgroundPref++;
            } else {
                if (--Instance.fakeBackgroundPref == 0) {
                    GameHelper.SetLayer(Instance.uiBackground, LayerUIInvisibleIndex);
                    GameManager.MainCamera.cullingMask |= layer;
                }
            }
        }
        
        private void HideUIRenderCamera() {
            StartCoroutine(this.HideUIRenderCameraDelay());
        }

        private IEnumerator HideUIRenderCameraDelay() {
            yield return YieldManager.EndOfFrame;
            this.uiRenderCamera.gameObject.SetActiveSafe(false);
        }

        public void ShowBattleRenderBackground() {
            Instance.battleRenderCamera.SetActiveSafe(true);
            Instance.HideBattleRenderCamera();
        }

        private void HideBattleRenderCamera() {
            StartCoroutine(this.HideBattleRenderCameraDelay());
        }

        private IEnumerator HideBattleRenderCameraDelay() {
            yield return YieldManager.EndOfFrame;
            this.battleRenderCamera.gameObject.SetActiveSafe(false);
        }
        #endregion



        #region Help create hero recruit effect
        public static void SetHeroRecoverEffectSOnTile(bool isEnable, Transform root) {
            Transform troopTreatment = root.Find(PrefabName.tileTroopTreatment);
            if (isEnable && troopTreatment == null) {
                troopTreatment = PoolManager.GetObject(PrefabPath.tileTroopTreatment, root).transform;
                troopTreatment.localPosition = Vector3.back;
                troopTreatment.localScale = new Vector3(0.5f, 0.5f, 1f);
            } else if (!isEnable && troopTreatment != null) {
                PoolManager.RemoveObject(troopTreatment.gameObject);
            }
        }

        public static void SetHeroRecoverEffectL(bool isEnable, Transform root) {
            Transform troopTreatment = root.Find(PrefabName.pnlTroopTreatmentL);
            if (isEnable && troopTreatment == null) {
                troopTreatment = PoolManager.GetObject(PrefabPath.pnlTroopTreatmentL, root).transform;
                RectTransform troopTreatmentRT = troopTreatment.GetComponent<RectTransform>();
                troopTreatmentRT.offsetMax = new Vector2(15, 15);
                troopTreatmentRT.offsetMin = new Vector2(-15, -15);
            } else if (!isEnable && troopTreatment != null) {
                PoolManager.RemoveObject(troopTreatment.gameObject);
            }
        }

        public static void SetHeroRecoverEffectS(bool isEnable, Transform root) {
            Transform troopTreatment = root.Find(PrefabName.pnlTroopTreatmentS);
            if (isEnable && troopTreatment == null) {
                PoolManager.GetObject(PrefabPath.pnlTroopTreatmentS, root);
            } else if (!isEnable && troopTreatment != null) {
                PoolManager.RemoveObject(troopTreatment.gameObject);
            }
        }
        #endregion


    }
}

