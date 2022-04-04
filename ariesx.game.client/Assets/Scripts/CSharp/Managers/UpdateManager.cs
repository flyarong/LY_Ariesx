using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public enum UpdateInfo {
        AnimationManager,
        AudioManager,
        EventManager,
        NetManager,
        TileOutline,
        BattleViewModel,
        FteViewModel,
        MapTileViewModel,
        MapViewModel,
        MapView,
        AlliancePanelView,
        AllianceInfoView,
        AllianceCreateView,
        AllianceMemberItemView,
        OpenChestView,
        CloudView,
        TileHighlightView,
        PersonalRankView,
        AllianceRankView,
        OccupationRankView,
        MapSNRankView,
        TroopFormationView,
        KeyboardTest,
        HeroTierUpView,
        StagetRewards,
        CampaignPreheat,
        CampaignProcessing,
        CustomSlider,
        CustomButton,
        LYGameData,
        FteView,
        CountryChoose,
        Water,
        GetSmartGiftBagHero,
        DailyTask,
        LoginReward
    }
    public class UpdateManager: MonoBehaviour {
        public static UpdateManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("UpdateManager is not initialized!");
                }
                return self;
            }
        }
        private static UpdateManager self;
        //  private bool isModify = false;
        private Dictionary<UpdateInfo, UnityEvent> UpdateActionDict =
            new Dictionary<UpdateInfo, UnityEvent>(20);
        private Dictionary<UpdateInfo, List<UnityAction>> registUpdateAction =
            new Dictionary<UpdateInfo, List<UnityAction>>(20);

        private List<UpdateInfo> unregistUpdateInfos = new List<UpdateInfo>(20);
        private Dictionary<UpdateInfo, List<UnityAction>> unregistUpdateActions =
            new Dictionary<UpdateInfo, List<UnityAction>>(20);

        private void Awake() {
            self = this;
        }

        public static void Regist(UpdateInfo updateName, UnityAction action) {
            List<UnityAction> unityActionList;
            if (!Instance.registUpdateAction.TryGetValue(updateName, out unityActionList)) {
                unityActionList = new List<UnityAction>();
                Instance.registUpdateAction.Add(updateName, unityActionList);
            }
            unityActionList.Add(action);
        }

        public static void Unregist(UpdateInfo updateName) {
            if (!Instance.unregistUpdateInfos.Contains(updateName)) {
                Instance.unregistUpdateInfos.Add(updateName);
            }
        }

        public static void Unregist(UpdateInfo updateName, UnityAction action) {
            List<UnityAction> unityActionList;
            if (!Instance.unregistUpdateActions.TryGetValue(updateName, out unityActionList)) {
                unityActionList = new List<UnityAction>();
                Instance.unregistUpdateActions.Add(updateName, unityActionList);
            }
            unityActionList.Add(action);
        }

        // Update is called once per frame
        private UnityEvent registedEvent = new UnityEvent();
        void Update() {
            foreach (var updateInfo in this.unregistUpdateActions) {
                if (this.UpdateActionDict.TryGetValue(updateInfo.Key, out registedEvent)) {
                    //Debug.LogError("unregistUpdateActions " + updateInfo.Key);
                    foreach (UnityAction action in updateInfo.Value) {
                        registedEvent.RemoveListener(action);
                    }
                }
            }

            foreach (UpdateInfo updateInfo in this.unregistUpdateInfos) {
                if (this.UpdateActionDict.TryGetValue(updateInfo, out registedEvent)) {
                    //Debug.LogError("unregistUpdateInfos " + updateInfo);
                    registedEvent.RemoveAllListeners();
                    this.UpdateActionDict.Remove(updateInfo);
                }
            }
            this.unregistUpdateActions.Clear();
            this.unregistUpdateInfos.Clear();

            foreach (var updateInfo in this.registUpdateAction) {
                UnityEvent unityEvent;
                if (!this.UpdateActionDict.TryGetValue(updateInfo.Key, out unityEvent)) {
                    unityEvent = new UnityEvent();
                    this.UpdateActionDict.Add(updateInfo.Key, unityEvent);
                }
                //Debug.LogError("unregistUpdateActions " + updateInfo.Key);
                foreach (UnityAction action in updateInfo.Value) {
                    unityEvent.AddListener(action);
                }
            }
            this.registUpdateAction.Clear();

            foreach (var updateInfos in this.UpdateActionDict) {
                updateInfos.Value.InvokeSafe();
            }
        }
    }
}
