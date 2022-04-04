using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class QueueViewModel: BaseViewModel {
        MapViewModel parent;
        TroopModel troopModel;
        HeroModel heroModel;
        QueueView view;
        private BuildModel buildModel;
        public Dictionary<string, Troop> TroopDict {
            get {
                return this.troopModel.troopDict;
            }
        }

        public List<Troop> TroopList {
            get {
                return this.GetTroopList();
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return heroModel.heroDict;
            }
        }

        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.buildModel.buildingDict;
            }
        }

        public Dictionary<string, DailyItemPreference> buildEventDict =
           new Dictionary<string, DailyItemPreference>();

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.view = this.gameObject.AddComponent<QueueView>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            NetHandler.AddNtfHandler(typeof(NewTroopNtf).Name, this.NewTroopNtf);
            NetHandler.AddDataHandler(typeof(EventRecruitNtf).Name, this.EventRecruitNtf);
            NetHandler.AddNtfHandler(typeof(HeroesNtf).Name, this.HeroesNtf);

            EventManager.AddEventAction(Event.March, this.UpdateMarch);
            EventManager.AddEventAction(Event.Recruit, this.UpdateRecruit);

            //FteManager.SetStartCallback(GameConst.NORMAL, 141, this.OnFteStep151Start);
            //FteManager.SetStartCallback(GameConst.NORMAL, 151, this.OnFteStep151Start);
            FteManager.SetStartCallback(GameConst.TROOP_ADD_HERO, 1,
                (index) => this.OnTroopStep1Start(index));
            FteManager.SetEndCallback(GameConst.TROOP_ADD_HERO, 1, this.OnTroopStep1End);
            FteManager.SetStartCallback(GameConst.NORMAL, 53, this.OnFteStep53Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 53, this.OnTroopStep1End);
            FteManager.SetStartCallback(GameConst.RECRUIT, 1, this.OnRecruitStep1Start);
            FteManager.SetEndCallback(GameConst.RECRUIT, 1, this.OnRecruitStep1End);
            NetHandler.AddNtfHandler(typeof(EventBuildNtf).Name, this.EventBuildNtf);
            this.Show();
        }

        private void EventBuildNtf(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            EventBuild eventBuild = eventBuildNtf.EventBuild;
            Dictionary<string, EventBase> eventDict = EventManager.EventDict[Event.Build];
            int length = eventDict.Count;
            if (this.view.isInitUI) {
                if (eventBuildNtf.Method == "new") {
                    this.view.BuildQueue();
                } else if (eventBuildNtf.Method == "del") {
                    EventManager.FinishImmediate(eventBuild.Id);
                    this.view.BuildQueue();
                }
            }
        }

        public void OnInvisible() {
            EventManager.RemoveEventAction(Event.Recruit, this.UpdateRecruit);
        }

        public void Show() {
            this.view.InitView();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideTileInfo() {
            this.parent.HideTileInfo();
        }

        public void UpdateQueueTroopStatusAt(Vector2 coordinate) {
            this.view.UpdateTroopStatusAt(coordinate);
        }

        public void UpdateQueueTroopStatus(Troop troop) {
            this.view.UpdateTroopStatus(troop);
        }

        public void RefreshQueueItemAnimation(Troop troop) {
            this.view.RefreshQueueItemAnimation(troop);
        }

        public void RefreshArmyCampTroopStatus(string armyCampName) {
            if (armyCampName.CustomStartsWith(ElementName.armycamp)) {
                Troop troop = this.troopModel.GetTroopByArmyCampName(armyCampName);
                this.UpdateQueueTroopStatus(troop);
            }
        }

        private void UpdateMarch(EventBase eventBase) {
            if (eventBase.playeId == RoleManager.GetRoleId() && !eventBase.id.Contains("Fte")) {
                this.view.UpdateMarch(eventBase as EventMarchClient);
            }
            //this.view.FormatPnlQueue();
        }

        private void UpdateRecruit(EventBase eventBase) {
            this.view.UpdateOtherEvent(eventBase as EventTroop, TroopStatus.Recruiting);
        }

        //private void UpdateRecruit() {
        //    Dictionary<string, EventRecruitClient> recruitEventDict =
        //        new Dictionary<string, EventRecruitClient>();
        //    foreach (EventRecruitClient eventRecruit in
        //        EventManager.EventDict[Event.Recruit].Values) {
        //        EventRecruitClient curRecruit;
        //        if (!recruitEventDict.TryGetValue(eventRecruit.troopId, out curRecruit)) {
        //            recruitEventDict.Add(eventRecruit.troopId, eventRecruit);
        //        } else {
        //            //EventRecruitClient curRecruit = recruitEventDict[eventRecruit.troopId]; 
        //            long left =
        //                eventRecruit.duration - (RoleManager.GetCurrentUtcTime() - eventRecruit.startTime);
        //            long curLeft =
        //                curRecruit.duration - (RoleManager.GetCurrentUtcTime() - curRecruit.startTime);
        //            if (curLeft < left) {
        //                recruitEventDict[eventRecruit.troopId] = eventRecruit;
        //            }
        //        }
        //    }
        //    foreach (EventRecruitClient queueRecruit in recruitEventDict.Values) {
        //        this.view.UpdateOtherEvent(queueRecruit, TroopStatus.Recruiting);
        //    }
        //}

        public void FollowMarch(string id) {
            this.parent.FollowMarch(id);
        }

        public void DisFollowNewMarch() {
            this.parent.DisFollowNewMarch();
        }

        public void ShowHouseKeeper() {
            this.parent.ShowHouseKeeper(tabIndex: 1);
        }

        public void ShowUnlockBuild(UnityAction Callback) {
            this.parent.ShowUnlockBuild(Callback);
        }

        public void ShowTileTroop(string id, Vector2 coordinate) {
            this.parent.ShowTileTroop(id, coordinate);
        }

        public void OnAnyOperate() {
            this.parent.OnAnyOperate();
        }

        public void JumpBuildCoord(Coord coord) {
            this.parent.MoveWithClick(coord);
        }

        //private void OnFteUI() {
        //    this.view.SetQueueVisible(FteManager.CheckUI(FteConst.Troop));
        //}

        private void NewTroopNtf(IExtensible message) {
            NewTroopNtf newTroopNtf = message as NewTroopNtf;
            this.view.InitView();
            this.parent.UpdateTroopStatus(newTroopNtf.Troop);
        }

        private List<Troop> GetTroopList() {
            List<Troop> troopList = new List<Troop>(this.TroopDict.Values);
            troopList.Sort((a, b) => {
                return a.ArmyCamp.CompareTo(b.ArmyCamp);
            });
            return troopList;
        }

        private void EventRecruitNtf(IExtensible message) {
            EventRecruitNtf eventRecruitNtf = message as EventRecruitNtf;
            if (eventRecruitNtf.Method.CustomEquals("del")) {
                EventManager.FinishedList.Add(eventRecruitNtf.EventRecruit.Id);
                Troop troop = this.TroopDict[eventRecruitNtf.EventRecruit.TroopId];
                this.UpdateQueueTroopStatusAt(troop.Coord);
            } else if (eventRecruitNtf.Method.CustomEquals("new")) {
                EventManager.AddRecruitEvent(eventRecruitNtf.EventRecruit);
            }
        }

        private void HeroesNtf(IExtensible message) {
            HeroesNtf heroesNtf = message as HeroesNtf;
            Troop troop;
            List<string> armyCamp = new List<string>(6);
            foreach (Hero hero in heroesNtf.Heroes) {
                troop = this.troopModel.GetTroopWithHeroName(hero.Name);
                if (troop != null && !armyCamp.Contains(troop.ArmyCamp)) {
                    armyCamp.Add(troop.ArmyCamp);
                    this.view.UpdateTroopStatus(troop);
                }
            }
        }

        public void SetIsFold(bool isFold) {
            this.view.IsFold = isFold;
        }

        public void SetIsQueueShow(bool isFold) {
            this.parent.IsQueueShow = !isFold;
        }
        #region FTE

        private void OnFteStep141Start(string index) {

        }

        private void OnFteStep151Start(string index) {
            this.parent.CloseAboveUI();
            this.view.OnFteStep151Start();
        }

        private void OnTroopStep1Start(string index) {
            this.view.afterHideCallback = () => FteManager.StopFte();
            DramaConf dramaConf = DramaConf.GetDramaByIndex(index);
            Troop troop = this.troopModel.GetTroopByArmyCampName(dramaConf.buildingName);
            bool needHighlight = dramaConf.id.CustomEquals("2");
            if (troop != null) {
                this.view.OnResourceStep1Start(troop.Id, needHighlight);
            } else {
                FteManager.StopFte();
                Debug.LogError("StartChapterDailyGuid");
                this.parent.StartChapterDailyGuid();
            }
        }

        private void OnFteStep53Start(string index) {
            foreach (Troop troop in this.troopModel.troopDict.Values) {
                this.view.OnFteStep53Start(troop.Id);
                break;
            }
        }

        private void OnTroopStep1End() {
            this.view.afterHideCallback = null;
        }

        private void OnRecruitStep1Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            float distance = Mathf.Infinity;
            Vector2 home = RoleManager.GetRoleCoordinate();
            string troopId = string.Empty;
            foreach (Troop troop in this.TroopList) {
                foreach (HeroPosition heroPosition in troop.Positions) {
                    Hero hero = this.HeroDict[heroPosition.Name];
                    HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.GetId());
                    if (hero.ArmyAmount < heroConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount)) {
                        Vector2 troopCoordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
                        float sqrCurDistance = (home - troopCoordinate).sqrMagnitude;
                        if (sqrCurDistance < distance) {
                            distance = sqrCurDistance;
                            FteManager.SetCurHero(hero.Name);
                            FteManager.SetCurrentTroop(troop.Id, troopCoordinate, string.Empty);
                            troopId = troop.Id;
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(troopId)) {
                this.view.OnRecruitStep1Start(troopId);
            } else {
                FteManager.StopFte();
                this.parent.StartChapterDailyGuid();
            }
        }

        private void OnRecruitStep1End() {
            this.view.afterHideCallback = null;
        }

        #endregion
    }
}
