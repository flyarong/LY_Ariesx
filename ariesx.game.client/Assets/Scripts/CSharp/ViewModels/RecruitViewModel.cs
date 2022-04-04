using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class RecruitViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private RecruitModel model;
        private HeroModel heroModel;
        private BuildModel buildModel;
        private TroopModel troopModel;
        private RecruitView view;
        /* Other members */
        /*****************/

        /* Model data get set */
        public Troop Troop {
            get {
                return this.model.troop;
            }
            set {
                // To do: only refresh changed hero.
                //if (this.model.troop != value) {
                this.model.troop = value;
                this.NeedFresh = true;
                //}
                this.ArmyDict.Clear();
                foreach (HeroPosition heroPosition in value.Positions) {
                    string heroName = heroPosition.Name;
                    this.ArmyDict.Add(this.heroModel.heroDict[heroName].GetId(), 0);
                }
                int campLevel = this.buildModel.buildingDict[this.Troop.ArmyCamp].Level;
                ArmyCampConf armyCampConf =
                    ConfigureManager.GetConfById<ArmyCampConf>(campLevel.ToString());
                this.model.recruitSpeed = 1 - armyCampConf.recruitmentSpeed;
            }
        }

        public Dictionary<string, float> ArmyDict {
            get {
                return this.model.armyDict;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public float RecruiteSpeed {
            get {
                return this.model.recruitSpeed;
            }
        }
        /**********************/
        public bool NeedFresh {
            get; set;
        }

        //public List<string> RefreshHeroList = new List<string>();

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<RecruitModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.view = this.gameObject.AddComponent<RecruitView>();

            NetHandler.AddNtfHandler(typeof(EventRecruitNtf).Name, this.EventRecruitNtf);
            FteManager.SetStartCallback(GameConst.RECRUIT, 3, this.OnRecruitStep3Start);
            FteManager.SetEndCallback(GameConst.RECRUIT, 3, this.OnRecruitStep3End);

            this.NeedFresh = true;
        }

        public void Show(bool showTips) {
            this.view.PlayShow(() => {
                this.model.showTips = showTips;
                this.parent.OnAddViewAboveMap(this);
            }, true);
            if (this.NeedFresh) {
                this.view.SetContent(this.model.showTips);
                this.NeedFresh = false;
            }
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        // To do: set needrefresh.
        //protected override void OnReLogin() {
        //    this.Refresh();
        //}

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.view.SetContent(this.model.showTips);
            } else {
                this.NeedFresh = true;
            }
        }

        public void ChangeResource(string heroKey, float value) {
            float army = value - this.ArmyDict[heroKey];
            float old = this.ArmyDict[heroKey];

            this.ArmyDict[heroKey] = old + army;
        }

        public void CollectResource(Resource type, int addAmount,
            Vector2 resourcePos, bool isCollect) {
            this.parent.CollectResource(type, addAmount, resourcePos,
                false, isCollect);
        }

        //public void RefreshResourceAndCurrency() {
        //    this.parent.RefreshResourceAndCurrency();
        //}

        public List<Resource> GetProduceResourceList() {
            return this.buildModel.GetProduceResourceList();
        }

        public void RecruitMaxReq(string heroName) {
            RecruitMaxReq recruitReq = new RecruitMaxReq() {
                TroopId = this.Troop.Id,
                Name = heroName
            };
            UnityAction errorAction = () => {
                this.view.SetItemStatus(heroName, false);
                this.view.needPlayResourceAnim = false;
            };
            this.view.needPlayResourceAnim = true;
            NetManager.SendMessage(recruitReq, typeof(RecruitMaxAck).Name,
                this.OnRecruitHeroAck, (message) => errorAction(), errorAction);
        }

        public void RecruitCancelReq(string heroName) {
            CancelRecruitReq cancelRecruitReq = new CancelRecruitReq();
            EventRecruitClient eventRecruit =
                           EventManager.GetRecruitEventByHeroName(heroName);
            if (eventRecruit != null) {
                cancelRecruitReq.Id = eventRecruit.id;
                this.view.needPlayResourceAnim = true;
                NetManager.SendMessage(cancelRecruitReq,
                    typeof(CancelRecruitAck).Name, this.OnCancelRecruitAck);
            }
        }

        public void TreatmentTroopAllHeroes() {
            RecruitAllReq recruitAll = new RecruitAllReq() {
                TroopId = this.Troop.Id
            };

            this.view.needPlayResourceAnim = true;
            NetManager.SendMessage(recruitAll,
                typeof(RecruitAllAck).Name, this.OnRecruitAllAck, (message) => {
                    this.view.needPlayResourceAnim = false;
                });
        }


        /*********************************************************************/
        private void EventRecruitNtf(IExtensible message) {
            EventRecruitNtf eventRecruitNtf = message as EventRecruitNtf;
            if (eventRecruitNtf.Method.CustomEquals("del") &&
                eventRecruitNtf.EventRecruit.FinishAt * 1000 < RoleManager.GetCurrentUtcTime()) {
                this.parent.HeroRecruitDoneHandler(eventRecruitNtf.EventRecruit.HeroName);
            }

            if (this.view.IsVisible) {
                this.view.RefreshRecruitItem(
                    this.HeroDict[eventRecruitNtf.EventRecruit.HeroName]);
            } else {
                this.NeedFresh = true;
            }
            this.parent.SetQueueIsFold(false);
            this.parent.UpdateTroopStatus(eventRecruitNtf.EventRecruit.TroopId);
        }

        private bool IsTroopWithHeroRecruitDone(string heroName, out string troopName) {
            troopName = string.Empty;
            Troop heroTroop = this.troopModel.GetTroopWithHeroName(heroName);
            if (heroTroop == null) {
                return true;
            }

            foreach (HeroPosition position in heroTroop.Positions) {
                if (EventManager.GetRecruitEventByHeroName(position.Name) != null) {
                    troopName = TroopModel.GetTroopName(heroTroop.ArmyCamp);
                    return false;
                }
            }
            return true;
        }

        private void OnRecruitAllAck(IExtensible message) {
            RecruitAllAck recruitAll = message as RecruitAllAck;
            if (recruitAll.NotEnoughResource) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.resources_not_enough),
                    TipType.Info);
            }
            if (this.view.IsVisible) {
                this.view.OnTreatmentResourceChange(recruitAll.Resources,
                    recruitAll.Currency, recruitAll.Cost, false);
            }
        }

        private void OnRecruitHeroAck(IExtensible message) {
            RecruitMaxAck recruitMax = message as RecruitMaxAck;
            this.view.OnTreatmentResourceChange(
                recruitMax.Resources, recruitMax.Currency, recruitMax.Cost, false);
        }

        private void OnCancelRecruitAck(IExtensible message) {
            CancelRecruitAck cancelRecruit = message as CancelRecruitAck;
            this.view.OnTreatmentResourceChange(cancelRecruit.Resources,
                cancelRecruit.Currency, cancelRecruit.Cost, true);
        }


        //private void HeroNtf(IExtensible message) {
        //    this.Refresh();
        //}
        /***********************************/

        private void UpdateRecruitStatus(EventBase eventBase) {
            EventRecruitClient eventRecruit = eventBase as EventRecruitClient;
            if (eventRecruit.troopId.CustomEquals(this.Troop.Id)) {
                this.view.UpdateRecruitStatus(eventBase);
            }
        }

        public void OnVisible() {
            EventManager.AddEventAction(Event.Recruit, this.UpdateRecruitStatus);
        }

        public void OnInvisible() {
            EventManager.RemoveEventAction(Event.Recruit, this.UpdateRecruitStatus);
        }

        #region FTE

        private void OnRecruitStep3Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            this.view.OnRecruitStep3Start();
            // this.Troop =  FteManager.GetCurrentTroop();
        }

        private void OnRecruitStep3End() {
            this.view.afterHideCallback = null;
            this.RecruitMaxReq(FteManager.GetCurHero());
            FteManager.SetCurHero(string.Empty);
            this.parent.StartChapterDailyGuid();
        }

        #endregion
    }
}
