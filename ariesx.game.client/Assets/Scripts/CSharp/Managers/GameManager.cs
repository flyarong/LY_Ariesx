using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
//using System.Threading;
using ProtoBuf;
using Protocol;
using UnityEngine.Video;
using System.Text;
using System.Diagnostics;

namespace Poukoute {
    //public delegate void SceneAction(AckHandler ackHandler);

    //public class SceneRecord {
    //    public ReqAction req;
    //}

    public class GameManager : MonoBehaviour {
        public static Camera MainCamera;
        public static CustomPhysics2DRaycaster RayCaster;
        private static GameManager self;
        //private Thread chatThread;
        private int cullingMask;
        //[HideInInspector]
        //public bool needRestart = false;

        public UnityEngine.UI.CanvasScaler splashScaler;
        public GameObject logoLY;
        public GameObject logoPK;
        public GameObject loading;

        public bool initStepC = false;

        public static GameManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("GameManager is not initialized");
                }
                return self;
            }
        }

        void Awake() {
#if UNITY_EDITOR || DEVELOPER
            Debug.isDebug = true;
#else
            Debug.isDebug = false;
#endif
            self = this;
            MainCamera = Camera.main;
            RayCaster = MainCamera.GetComponent<CustomPhysics2DRaycaster>();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
#if !UNITY_EDITOR && !DEVELOPER
            Application.logMessageReceived += OnError; //Debug.OnError;
#endif
            Debug.debugEvent.AddListener(this.OnDebugUpload);
            StartCoroutine(LoadLYlogo());
            NetHandler.AddNtfHandler(typeof(ErrorAck).Name, this.ErrorAck);
        }

        private IEnumerator LoadLYlogo() {
            string sPath = string.Concat(Application.streamingAssetsPath, "/LogoLY.mp4");
            WWW www = new WWW(sPath);
            yield return www;
            VideoPlayer vPlayer = this.logoLY.GetComponent<VideoPlayer>();
            vPlayer.url = www.url;
            vPlayer.Play();
            StartCoroutine(InitStepA());
        }

        private IEnumerator InitStepA() {
            yield return YieldManager.GetWaitForSeconds(2.5f);
            Destroy(this.logoLY);
            if (MainCamera.aspect - GameConst.CAMERA_ASPECT < -0.0001f) {
                this.splashScaler.matchWidthOrHeight = 1;
            } else {
                this.splashScaler.matchWidthOrHeight = 0;
            }
            this.logoPK.gameObject.SetActiveSafe(true);
            yield return YieldManager.GetWaitForSeconds(0.5f);
            Instance.gameObject.AddComponent<UpdateManager>();
            Instance.gameObject.AddComponent<RoleManager>();
            Instance.gameObject.AddComponent<PoolManager>();
            Instance.gameObject.AddComponent<ConfigureManager>();
            Instance.gameObject.AddComponent<LocalManager>();
            this.InitUI();
            Instance.gameObject.AddComponent<AudioManager>();
            Instance.gameObject.AddComponent<NetManager>();
            Instance.gameObject.AddComponent<ModelManager>();
            yield return YieldManager.GetWaitForSeconds(1f);

            if (Instance.logoPK != null) {
                Instance.logoPK.gameObject.SetActiveSafe(false);
            }
            AudioManager.Play("bg_loading", AudioType.Show, volumn: AudioVolumn.Low);
            if (Instance.loading != null) {
                UIManager.ShowLoading();
                Instance.loading.gameObject.SetActiveSafe(true);
            }
        }

        public static void InitStepB() {
            Instance.gameObject.AddComponent<TriggerManager>();
            Instance.gameObject.AddComponent<EventManager>();
            Instance.gameObject.AddComponent<ScreenEffectManager>();
            Instance.gameObject.AddComponent<FteManager>();
            Instance.gameObject.AddComponent<MapElementManager>();
            Instance.gameObject.AddComponent<SdkManager>();
        }

        public static void InitStepD() {
            Instance.gameObject.AddComponent<AdvanceSdkManager>();
        }

        public static void InitStepC() {
            if (Instance.initStepC) {
                return;
            }
            ConfigureManager.LoadOtherConfigures();
            Instance.initStepC = true;
        }

        public static bool IsNeedShowErrorAckMsg = true;
        private void ErrorAck(IExtensible message) {
            if (IsNeedShowErrorAckMsg) {
                ErrorAck error = (ErrorAck)message;
                UIManager.ShowTip(LocalManager.GetValue(error.Error), TipType.Info);
            }
        }

#if UNITY_EDITOR
        void Update() {
            if (UnityEditor.EditorApplication.isCompiling) {
                Debug.Log("Compiled during play; automatically quit.");
                UnityEditor.EditorApplication.Beep();
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
#endif

        private void InitUI() {
            SceneManager.LoadScene("SceneUI", LoadSceneMode.Additive);
            Debug.Log("InitUi Complete");
        }

        private static void OnError(string condition, string stackTrace, LogType type) {
            if (type == LogType.Error) {
                FteManager.StopFte();
                BaseView.IsPlaying = false;
            }
        }

        // loggly
        private void OnDebugUpload(string logString) {
#if UNITY_EDITOR || DEVELOPER
            return;
#else
            //Initialize WWWForm and store log level as a string
            var loggingForm = new WWWForm();
            //Add log message to WWWForm
            loggingForm.AddField("Message", logString);
            StringBuilder sb = new StringBuilder();
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++) {
                StackFrame sf = st.GetFrame(i);
                sb.Append(sf.ToString());
            }
            loggingForm.AddField("Trace", sb.ToString());
            //Add any User, Game, or Device MetaData that would be useful to finding issues later
            loggingForm.AddField("Device_Model", SystemInfo.deviceModel);
            loggingForm.AddField("App_Version", Application.version);
            loggingForm.AddField("OS_Version", SystemInfo.operatingSystem);

            StartCoroutine(SendData(loggingForm));
#endif
        }

#if !UNITY_EDITOR && !DEVELOPER
        private IEnumerator SendData(WWWForm form) {
            //Send WWW Form to Loggly, replace TOKEN with your unique ID from Loggly
            WWW sendLog = new WWW("http://logs-01.loggly.com/inputs/43ac04f8-eec0-4d14-9d7d-f1a921728494/tag/Unity3D", form);
            yield return sendLog;
        }
#endif

        public static void DisableMainCamera() {
            Instance.cullingMask = MainCamera.cullingMask;
            MainCamera.cullingMask = 0;
        }

        // To do: has nullexception.
        public static void EnableMainCamera() {
            MainCamera.cullingMask |= Instance.cullingMask;
        }

        public static void RestartGame() {
            TriggerManager.Invoke(Trigger.Logout);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneMain");
        }


        public static IEnumerator RestartGame(float second) {
            yield return YieldManager.GetWaitForSeconds(second);
            RestartGame();
        }

        void OnDestroy() {
            Debug.debugEvent.RemoveAllListeners();
#if !UNITY_EDITOR && !DEVELOPER
            Application.logMessageReceived -= OnError;
#endif
            NetHandler.RemoveNtfHandler(typeof(ErrorAck).Name, this.ErrorAck);
        }
    }
}
