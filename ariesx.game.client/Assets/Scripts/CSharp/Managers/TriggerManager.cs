using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Protocol;


namespace Poukoute {
    public enum Trigger {
        CameraMove,
        CameraFocus,
        HeroStatusChange,
        HeroArmyAmountChange,
        ResourceCollect,
        ChestCollect,
        ForceCollect,
        SimpleResourceCollect,
        //TileLimitCollect,
        ResourceChange,
        CurrencyChange,
        OnChestsGet,
        Fte,
        BeenKickedOutAlliance,
        PlayBattleReportStart,
        PlayBattleReportDone,
        RefreshPlayerPoint,
        // Tile Water
        ShowWater,
        HideWater,
        // Data point
        Login,
        Logout,
        FirstLogin,
        FinishFte,
        // Voice Live
        VoiceLiveStatusChange,
        VoiceLiveUserDataChange,
        DramaArrow,
        ShowBtnBuild,
        None
    }

    public class TriggerManager : MonoBehaviour {
        private static TriggerManager self;
        public static TriggerManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("TriggerManager is not initialized.");
                }
                return self;
            }
        }

        private Dictionary<Trigger, UnityEvent> eventWithVoidDict =
            new Dictionary<Trigger, UnityEvent>();
        private class TriggerEventVector2 : UnityEvent<Vector2> { }
        private Dictionary<Trigger, TriggerEventVector2> eventWithVector2Dict =
            new Dictionary<Trigger, TriggerEventVector2>();
        private class TriggerEventInt : UnityEvent<int> { }
        private Dictionary<Trigger, TriggerEventInt> eventWithIntDict =
            new Dictionary<Trigger, TriggerEventInt>();
        private class TriggerEventString : UnityEvent<string> { }
        private Dictionary<Trigger, TriggerEventString> eventWithStringDict =
            new Dictionary<Trigger, TriggerEventString>();
        private class TriggerEventTroop : UnityEvent<Troop> { }
        private Dictionary<Trigger, TriggerEventTroop> eventWithTroopDict =
            new Dictionary<Trigger, TriggerEventTroop>();

        private class TriggerEventResourceIntVector2Bool : UnityEvent<Resource, int, Vector2, bool> { }
        private Dictionary<Trigger, TriggerEventResourceIntVector2Bool> TriggerEventResourceIntVector2BoolDict =
                new Dictionary<Trigger, TriggerEventResourceIntVector2Bool>();
        private class TriggerEventResourceInt : UnityEvent<Resource, int> { }
        private Dictionary<Trigger, TriggerEventResourceInt> eventWithResourceInt =
            new Dictionary<Trigger, TriggerEventResourceInt>();
        private class TriggerEventStringInt : UnityEvent<string, int> { }
        private Dictionary<Trigger, TriggerEventStringInt> eventWithStringInt =
            new Dictionary<Trigger, TriggerEventStringInt>();
        private class TriggerEventLotteryResultString :
            UnityEvent<LotteryResult, string, UnityAction, bool> { }
        private Dictionary<Trigger, TriggerEventLotteryResultString> eventWithLotteryString =
            new Dictionary<Trigger, TriggerEventLotteryResultString>();

        private class TriggerEventVector2String : UnityEvent<Vector2, string> { }
        private Dictionary<Trigger, TriggerEventVector2String> eventVector2String =
            new Dictionary<Trigger, TriggerEventVector2String>();
        private class TriggerEventVector3GachaGroupConfCollectChestType : UnityEvent<Vector3, GachaGroupConf, CollectChestType, UnityAction> { }
        private Dictionary<Trigger, TriggerEventVector3GachaGroupConfCollectChestType> eventVector3GachaGroupConfCollectChestType =
            new Dictionary<Trigger, TriggerEventVector3GachaGroupConfCollectChestType>();
        void Awake() {
            self = this;
        }

        public static void Regist(Trigger trigger, UnityAction action) {
            UnityEvent unityEvent;
            if (!Instance.eventWithVoidDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new UnityEvent();
                Instance.eventWithVoidDict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction action) {
            UnityEvent unityEvent;
            if (Instance.eventWithVoidDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger) {
            UnityEvent unityEvent;
            if (Instance.eventWithVoidDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke();
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Vector2> action) {
            TriggerEventVector2 unityEvent;
            if (!Instance.eventWithVector2Dict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventVector2();
                Instance.eventWithVector2Dict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<Vector2> action) {
            TriggerEventVector2 unityEvent;
            if (Instance.eventWithVector2Dict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, Vector2 param) {
            TriggerEventVector2 unityEvent;
            if (Instance.eventWithVector2Dict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Troop> action) {
            TriggerEventTroop unityEvent;
            if (!Instance.eventWithTroopDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventTroop();
                Instance.eventWithTroopDict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<Troop> action) {
            TriggerEventTroop unityEvent;
            if (Instance.eventWithTroopDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, Troop troop) {
            TriggerEventTroop unityEvent;
            if (Instance.eventWithTroopDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(troop);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<string> action) {
            TriggerEventString unityEvent;
            if (!Instance.eventWithStringDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventString();
                Instance.eventWithStringDict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<string> action) {
            TriggerEventString unityEvent;
            if (Instance.eventWithStringDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, string param) {
            TriggerEventString unityEvent;
            if (Instance.eventWithStringDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<int> action) {
            TriggerEventInt unityEvent;
            if (!Instance.eventWithIntDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventInt();
                Instance.eventWithIntDict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<int> action) {
            TriggerEventInt unityEvent;
            if (Instance.eventWithIntDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, int param) {
            TriggerEventInt unityEvent;
            if (Instance.eventWithIntDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Resource, int, Vector2, bool> action) {
            TriggerEventResourceIntVector2Bool unityEvent;
            if (!Instance.TriggerEventResourceIntVector2BoolDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventResourceIntVector2Bool();
                Instance.TriggerEventResourceIntVector2BoolDict.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<Resource, int, Vector2, bool> action) {
            TriggerEventResourceIntVector2Bool unityEvent;
            if (Instance.TriggerEventResourceIntVector2BoolDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, Resource param1, int param2, Vector2 param3, bool param4) {
            TriggerEventResourceIntVector2Bool unityEvent;
            if (Instance.TriggerEventResourceIntVector2BoolDict.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param1, param2, param3, param4);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Resource, int> action) {
            TriggerEventResourceInt unityEvent;
            if (!Instance.eventWithResourceInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventResourceInt();
                Instance.eventWithResourceInt.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<Resource, int> action) {
            TriggerEventResourceInt unityEvent;
            if (Instance.eventWithResourceInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, Resource param1, int param2) {
            TriggerEventResourceInt unityEvent;
            if (Instance.eventWithResourceInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param1, param2);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<string, int> action) {
            TriggerEventStringInt unityEvent;
            if (!Instance.eventWithStringInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventStringInt();
                Instance.eventWithStringInt.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<string, int> action) {
            TriggerEventStringInt unityEvent;
            if (Instance.eventWithStringInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, string param1, int param2) {
            TriggerEventStringInt unityEvent;
            if (Instance.eventWithStringInt.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param1, param2);
            }
        }

        public static void Regist(Trigger trigger,
            UnityAction<LotteryResult, string, UnityAction, bool> action) {
            TriggerEventLotteryResultString unityEvent;
            if (!Instance.eventWithLotteryString.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventLotteryResultString();
                Instance.eventWithLotteryString.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger,
            UnityAction<LotteryResult, string, UnityAction, bool> action) {
            TriggerEventLotteryResultString unityEvent;
            if (Instance.eventWithLotteryString.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, LotteryResult param1,
            string param2, UnityAction param3, bool param4) {
            TriggerEventLotteryResultString unityEvent;
            if (Instance.eventWithLotteryString.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param1, param2, param3, param4);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Vector2, string> action) {
            TriggerEventVector2String unityEvent;
            if (!Instance.eventVector2String.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventVector2String();
                Instance.eventVector2String.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Unregist(Trigger trigger, UnityAction<Vector2, string> action) {
            TriggerEventVector2String unityEvent;
            if (Instance.eventVector2String.TryGetValue(trigger, out unityEvent)) {
                unityEvent.RemoveListener(action);
            }
        }

        public static void Invoke(Trigger trigger, Vector2 param1, string param2) {
            TriggerEventVector2String unityEvent;
            if (Instance.eventVector2String.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(param1, param2);
            }
        }

        public static void Regist(Trigger trigger, UnityAction<Vector3, GachaGroupConf, CollectChestType, UnityAction> action) {
            TriggerEventVector3GachaGroupConfCollectChestType unityEvent;
            if (!Instance.eventVector3GachaGroupConfCollectChestType.TryGetValue(trigger, out unityEvent)) {
                unityEvent = new TriggerEventVector3GachaGroupConfCollectChestType();
                Instance.eventVector3GachaGroupConfCollectChestType.Add(trigger, unityEvent);
            }
            unityEvent.AddListener(action);
        }

        public static void Invoke(Trigger trigger, Vector3 startAndTargetPos, GachaGroupConf gachaGroupConf, CollectChestType type, UnityAction customAni) {
            TriggerEventVector3GachaGroupConfCollectChestType unityEvent;
            if (Instance.eventVector3GachaGroupConfCollectChestType.TryGetValue(trigger, out unityEvent)) {
                unityEvent.Invoke(startAndTargetPos, gachaGroupConf, type, customAni);
            }
        }
    }
}
