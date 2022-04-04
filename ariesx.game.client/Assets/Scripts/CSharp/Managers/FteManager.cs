using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public class FteManager : MonoBehaviour {
        private static FteManager self;
        public static FteManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("FTEManager is not initialized.");
                }
                return self;
            }
        }

        public Vector2 DemonOrigin {
            get {
                return this.model.coorDemonOrigin;
            }
        }

        public Vector2 DemonTarget {
            get {
                return this.model.coorDemonTarget;
            }
        }

        public Vector2 ElfOrigin {
            get {
                return this.model.coorElfOrigin;
            }
        }

        public Vector2 ElfTarget {
            get {
                return this.model.coorElfTarget;
            }
        }

        public bool DemonVsDragon {
            get {
                return this.model.demonVsDragon;
            }
            set {
                this.model.demonVsDragon = value;
            }
        }

        private bool skipFte;
        public static bool SkipFte {
            get {
                //return false;
                return Instance.skipFte;
            }
        }

        public static bool FteOver = false;

        private Dictionary<string, Dictionary<int, UnityAction<string>>> startCallbackDict =
            new Dictionary<string, Dictionary<int, UnityAction<string>>>();
        private Dictionary<string, Dictionary<int, UnityAction>> endCallbackDict =
           new Dictionary<string, Dictionary<int, UnityAction>>();

        private UnityAction stopCallback = null;
        public static UnityAction StopCallback {
            get {
                return Instance.stopCallback;
            }
            set {
                Instance.stopCallback = value;
            }
        }
        [HideInInspector]
        public string curStep = string.Empty;
        public static bool CanStart = false;
        private FteModel model;
        private FteViewModel viewModel;
        private DramaModel dramaModel;
        private bool isDrama = false;
        private string dramaType = string.Empty;
        private int dramaStep = 0;
        private int curDirection = 1;
        public static Vector2 step57 = new Vector2(-1, -1);

        void Awake() {
            self = this;
            this.model = ModelManager.GetModelData<FteModel>();
            this.viewModel = PoolManager.GetObject<FteViewModel>(this.transform);
            this.dramaModel = ModelManager.GetModelData<DramaModel>();

            FteOver =
            this.skipFte = PlayerPrefs.HasKey(RoleManager.Udid + "skip_fte") &&
               PlayerPrefs.GetInt(RoleManager.Udid + "skip_fte") == 1;

        }

        private void StartFteWhenOK(string index) {
            this.viewModel.ShowFteMask();
            StartCoroutine(this.DelayFte(index));
        }

        private IEnumerator DelayFte(string index) {
            yield return new WaitUntil(() => CanStart);
            Debug.Log("B:" + index);
            StartFte(index);
        }

        public static void SetStartCallback(string type, int step, UnityAction<string> callback) {
            Dictionary<int, UnityAction<string>> callbackDict;
            if (!Instance.startCallbackDict.TryGetValue(type, out callbackDict)) {
                callbackDict = new Dictionary<int, UnityAction<string>>();
                Instance.startCallbackDict.Add(type, callbackDict);
            }
            if (callbackDict.ContainsKey(step)) {
                callbackDict[step] += callback;
            } else {
                callbackDict[step] = callback;
            }
        }

        public static void SetEndCallback(string type, int step, UnityAction callback) {
            Dictionary<int, UnityAction> callbackDict;
            if (!Instance.endCallbackDict.TryGetValue(type, out callbackDict)) {
                callbackDict = new Dictionary<int, UnityAction>();
                Instance.endCallbackDict.Add(type, callbackDict);
            }
            if (callbackDict.ContainsKey(step) && !(type == GameConst.NORMAL &&
                (step == 241 || step == 481))) {
                callbackDict[step] += callback;
            } else {
                callbackDict[step] = callback;
            }
        }

        public static void RemoveEndCallback(string type, int step, UnityAction callback) {
            Dictionary<int, UnityAction> callbackDict;
            if (!Instance.endCallbackDict.TryGetValue(type, out callbackDict)) {
                callbackDict = new Dictionary<int, UnityAction>();
                Instance.endCallbackDict.Add(type, callbackDict);
            }

        }

        public static bool CheckDrama(int step) {
            if (step == 0) {
                return true;
            }
            foreach (ChapterTask task in Instance.dramaModel.dramaList) {
                if (task.Id == step) {
                    return task.IsDone && task.unlocked;
                }
            }
            return true;
        }

        public static void DelayStartFte(string index) {
            Instance.curStep = index;
            Instance.StartFteWhenOK(index);
        }

        public static void DelayEndFte() {
            Instance.StartCoroutine(Instance.InnerDelayFte());
        }

        private IEnumerator InnerDelayFte() {
            yield return YieldManager.GetWaitForSeconds(2f);
            FteManager.EndFte(true, false);
        }


        public static bool CheckFte(string type, int step) {
            string index = type + "_" + step;
            FteStepConf stepConf = FteStepConf.GetConf(Instance.curStep);
            if (stepConf != null && Instance.curStep == index) {
                return true;
            }
            return false;
        }

        public static bool CheckUI(string ui) {
            FteStepConf stepConf = FteStepConf.GetConf(Instance.curStep);
            if (stepConf == null || !stepConf.lockedUI.Contains(ui)) {
                return true;
            } else {
                return false;
            }
        }

        private void GetChapterId(string index, out string type, out int step) {
            int pos = index.LastIndexOf('_');
            if (pos == -1) {
                type = string.Empty;
                step = 0;
                return;
            }
            type = index.Substring(0, pos);
            step = int.Parse(index.Substring(pos + 1));
            if (type != GameConst.NORMAL) {
                type = DramaConf.GetConf(step.ToString()).type;
                if (!this.isDrama) {
                    this.isDrama = true;
                    step =
                    this.dramaStep = 1;
                    dramaType = type;
                } else {
                    step = 1;
                }
            } else {
                this.isDrama = false;
            }
        }

        public static void StartFte(string type, int step, bool isDrama = false) {
            if (isDrama) {
                Instance.isDrama = true;
                Instance.dramaStep = step - 1;
                Instance.dramaType = type;
                StartDrama();
            } else {
                string index = type + "_" + step;
                StartFte(index);
            }
        }

        public static void StartFte(string index, bool isNeedCheckUI = true) {
            string type;
            int step;
            Instance.curStep = index;
            if (isNeedCheckUI && 
                !(PlayerPrefs.HasKey(RoleManager.Udid + "skip_fte") &&
                (PlayerPrefs.GetInt(RoleManager.Udid + "skip_fte") == 1))) {
                TriggerManager.Invoke(Trigger.Fte);
            }
            Instance.GetChapterId(index, out type, out step);
            if (Instance.isDrama) {
                Instance.dramaStep = 1;
                Instance.dramaType = type;
            }
            bool isEnforce = type == GameConst.NORMAL;
            Dictionary<int, UnityAction<string>> callbackDict;
            if (Instance.startCallbackDict.TryGetValue(type, out callbackDict)) {
                UnityAction<string> callback;
                if (callbackDict.TryGetValue(step, out callback)) {
                    FteStepConf stepConf = FteStepConf.GetConf(index.ToString());
                    if (stepConf != null && isEnforce) {
                        Debug.Log("ShowFteMask");
                        Instance.viewModel.ShowFteMask();
                    }
                    try {
                        callback.Invoke(index);
                    } catch (Exception e) {
#if UNITY_EDITOR || DEVELOPER
                        throw e;
                        //if (VersionConst.IsDeveloper()) {
                        //    throw e;
                        //}
#endif
                        //  StopFte();
                    }
                    return;
                }
            }
            Debug.Log("endfte");
            Instance.viewModel.EndFte();
        }

        private static void StartDrama() {
            string type = Instance.dramaType;
            int step = ++Instance.dramaStep;
            Dictionary<int, UnityAction<string>> callbackDict;
            Debug.Log("Start Fte:" + type + ", " + step);
            if (Instance.startCallbackDict.TryGetValue(type, out callbackDict)) {
                UnityAction<string> callback;
                if (callbackDict.TryGetValue(step, out callback)) {
                    Instance.viewModel.ShowFteMask();
                    try {
                        callback.Invoke(Instance.curStep);
                    } catch (Exception e) {
#if UNITY_EDITOR || DEVELOPER
                        throw e;
#endif
                        // StopFte();
                    }
                    Debug.Log("return");
                    return;
                }
            }
            Instance.viewModel.HideFteMask();
            Instance.isDrama = false;
            StartFte();
        }

        public static void StartFte() {
            if (Instance.isDrama) {
                StartDrama();
            } else {
                FteStepConf stepConf = FteStepConf.GetConf(Instance.curStep);
                if (stepConf != null) {
                    StartFte(stepConf.next);
                }
            }
        }

        public static void EndFte(bool next, bool autoNext = true) {
            string type;
            int step;
            if (Instance.isDrama) {
                type = Instance.dramaType;
                step = Instance.dramaStep;
            } else {
                Debug.Log("Instance.curStep:" + Instance.curStep);
                Instance.GetChapterId(Instance.curStep, out type, out step);
            }
            AddFteStepsReq req = new AddFteStepsReq() {
                Key = string.Concat("step_new_",
                Instance.curStep.CustomIsEmpty() ? "special" : Instance.curStep)
            };
            if (Instance.isDrama) {
                req.Key = string.Concat(req.Key, "_" + step);
            } else {
                SetMaxFteStepReq maxStepReq = new SetMaxFteStepReq() {
                    Step = step
                };
                NetManager.SendMessage(maxStepReq, string.Empty, null);
            }
            NetManager.SendMessage(req, string.Empty, null, null, null);
            Dictionary<int, UnityAction> callbackDict;
            Instance.viewModel.EndFte();
            if (Instance.endCallbackDict.TryGetValue(type, out callbackDict)) {
                UnityAction callback;
                if (callbackDict.TryGetValue(step, out callback)) {
                    try {
                        callback.Invoke();
                    } catch (Exception e) {
#if UNITY_EDITOR || DEVELOPER
                        Debug.LogError("Exception");
                        throw e;
#endif
                    }
                }
            }
#if UNITY_EDITOR || DEVELOPER
            else {
                Debug.LogError("No callbackDict");
            }
#endif
            if (autoNext) {
                if (next) {
                    StartFte();
                } else {
                    FteStepConf stepConf = FteStepConf.GetConf(Instance.curStep);
                    if (stepConf != null) {
                        StartFte(stepConf.previouse);
                    }
                }
            }
        }

        public static void StopFte() {
            if (FteManager.Instance.curStep.CustomEquals(string.Empty)) {
                return;
            }
            if (Instance == null) {
                return;
            }
            Instance.viewModel.EndFte();
            Instance.viewModel.HideBanner(null, true);
            Instance.isDrama = false;
            Instance.dramaStep = 0;
            if (Instance.curStep.CustomIsEmpty()) {
                return;
            }
            string[] fteArray = Instance.curStep.CustomSplit('_');
            string type = fteArray[0];
            int step = int.Parse(fteArray[fteArray.Length - 1]);

            //string[] dramaStep =  FteManager.Instance.curStep.CustomSplit('_');
            //if (dramaStep.Length >= 3) {
            //    if (step > 3 && step < 9) {
            //        Debug.Log("Invoke(Trigger.DramaArrow);");
            //        TriggerManager.Invoke(Trigger.DramaArrow);
            //    }
            //}
            Instance.curStep = string.Empty;
            if ((type != GameConst.NORMAL && step > 3 && step < 10)) {//|| type == GameConst.NORMAL) {
                Instance.stopCallback.Invoke();
            }
        }

        public static void SetDragMask(Transform from,
            Transform to, CustomDrag fromDrag, CustomDrop toDrop) {
            Instance.viewModel.SetDragMask(from, to, fromDrag, toDrop);
        }

        public static void RemoveMask() {
            Instance.viewModel.RemoveMask();
        }

        public static void SetMask(Transform nextTrans, Transform prevTrans = null,
            bool hasArrow = true, bool isButton = false, bool autoNext = true,
            bool isEnforce = false, bool isHighlight = false, Transform arrowParent = null,
            Vector2 offset = default(Vector2), float rotation = 0,UnityAction afterCallBack = null) {
            TriggerManager.Invoke(Trigger.ShowBtnBuild);
            Instance.viewModel.SetMask(nextTrans, prevTrans, hasArrow, isButton,
                autoNext, isEnforce, isHighlight, arrowParent, offset, rotation,afterCallBack);
        }

        public static void SetArrow(Transform rectTrans, Transform arrowParent = null,
            bool isEnforce = false, Vector2 offset = default(Vector2), float rotation = 0) {
            TriggerManager.Invoke(Trigger.ShowBtnBuild);
            Instance.viewModel.SetArrow(rectTrans, arrowParent, isEnforce, offset, rotation);
        }

        public static void HideArrow() {
            Debug.Log("HideArrow");
            Instance.viewModel.HideArrow();
        }

        public static bool HasArrow() {
            return Instance.viewModel.HasArrow();
        }

        public static void SetLeftChat(string text,bool isHigh = false,
            UnityAction aftetCallBack = null) {
            Instance.viewModel.SetLeftChat(text,isHigh, aftetCallBack);
        }

        public static void SetRightChat(string text, UnityAction afterCallBack) {
            Instance.viewModel.SetRightChat(text, afterCallBack);
        }

        public static void HideRightChat() {
            Instance.viewModel.HideRightChat();
        }

        public static void SetChat(float offset, int subIndex = 1,
            bool needBackground = true, bool delay = false, bool transparent = false) {
            bool needButton = (Instance.curStep.CustomEquals("normal_31"));
            FteStepConf fteStepConf = FteStepConf.GetConf(Instance.curStep);
#if UNITY_EDITOR
            if (fteStepConf == null) {
                Debug.LogError(Instance.curStep + " is null, check it");
                return;
            }
#endif
            string nextStep = fteStepConf.next;
            string subIndexStr = string.Empty;
            string sufixStr = "_left";
            subIndexStr = "_" + subIndex;
            string text = LocalManager.GetValue("fte_", Instance.curStep, subIndexStr, sufixStr);
            string nextText = LocalManager.GetValue("fte_", nextStep, "_1", sufixStr);
            int direction = 1;
            if (string.IsNullOrEmpty(text)) {
                sufixStr = "_right";
                if (string.IsNullOrEmpty(text)) {
                    Instance.viewModel.HideChat(() => {
                        FteManager.EndFte(true, !delay);
                    }, string.IsNullOrEmpty(nextText));
                    return;
                }
            } else {
                direction = -1;
            }

            UnityAction action = () => {
                float second = delay ? 2f : 0;
                if (second == 0) {
                    Instance.viewModel.SetChat(
                        offset,
                        text,
                        subIndex,
                        needButton,
                        needBackground,
                        Instance.curDirection,
                        transparent
                    );
                } else {
                    Instance.StartCoroutine(Instance.SetChat(offset, text,
                        subIndex, needButton, needBackground, Instance.curDirection, second, transparent));
                }
            };

            if (direction != Instance.curDirection && subIndex > 1) {
                Instance.curDirection = direction;
                Instance.viewModel.HideChat(() => {
                    action.Invoke();
                }, true);
            } else {
                Instance.curDirection = direction;
                action.Invoke();
            }

        }


        private IEnumerator SetChat(float offset, string text, int subIndex,
            bool needButton, bool needBackground, int direction,
            float second, bool transparent) {
            yield return YieldManager.GetWaitForSeconds(second);
            this.viewModel.SetChat(
                offset,
                text,
                subIndex,
                needButton,
                needBackground,
                direction,
                transparent
            );
        }

        public static void ShowBanner(UnityAction callback) {
            Instance.viewModel.ShowBanner(callback);
        }

        public static void HideBanner(UnityAction callback) {
            Instance.viewModel.HideBanner(callback);
        }

        public static void SetElfCoordinate(Vector2 origin, Vector2 target) {
            Instance.model.coorElfOrigin = origin;
            Instance.model.coorElfTarget = target;
        }

        public static void SetCurrentTroop(string troop, Vector2 target, string playerName) {
            Instance.viewModel.CurrentTroop = troop;
            Instance.viewModel.CurrentTarget = target;
            Instance.viewModel.CurrentName = playerName;
        }

        public static string GetCurrentTroop() {
            return Instance.viewModel.CurrentTroop;
        }

        public static void SetCurrentTroopName(string troopName) {
            Instance.viewModel.CurrentTroopName = troopName;
        }

        public static string GetCurrentTroopName() {
            return Instance.viewModel.CurrentTroopName;
        }

        public static void SetCurrentLotteryGroup(string lotteryGroup) {
            Instance.viewModel.CurrentLotteryGroup = lotteryGroup;
        }

        public static string GetCurrentLotteryGroup() {
            return Instance.viewModel.CurrentLotteryGroup;
        }

        public static bool CheckBattleReport(Vector2 target, string playerName) {
            // To do : check the paly report logic
            return Instance.viewModel.CurrentTarget == target &&
                Instance.viewModel.CurrentName == playerName;
        }

        public static void SetCurrentBuilding(string building) {
            Instance.viewModel.CurrentBuild = building;
        }

        public static string GetCurBuilding() {
            return Instance.viewModel.CurrentBuild;
        }

        public static void SetCurHero(string heroName) {
            Instance.viewModel.CurrentHero = heroName;
        }

        public static string GetCurHero() {
            return Instance.viewModel.CurrentHero;
        }

        public static void SetCurCoordinate(Vector2 coordinate) {
            Instance.viewModel.CurrentTarget = coordinate;
        }

        public static Vector2 GetCurCoordinate() {
            return Instance.viewModel.CurrentTarget;
        }

        public static void SetCurChapterTaskId(int id) {
            Instance.viewModel.CurrentTaskId = id;
        }

        public static int GetCurChapterTaskId() {
            return Instance.viewModel.CurrentTaskId;
        }

        public static void ShowFteMask() {
            Instance.viewModel.ShowFteMask();
        }

        public static void HideFteMask() {
            Instance.viewModel.HideFteMask();
        }

        void OnDestroy() {
            CanStart = false;
        }
    }
}
