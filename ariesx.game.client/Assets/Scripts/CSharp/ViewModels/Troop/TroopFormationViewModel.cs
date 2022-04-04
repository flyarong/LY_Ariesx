using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class TroopFormationViewModel: BaseViewModel, IViewModel {
        private MapViewModel parent;
        private TroopModel model;
        private TroopFormationView view;
        private HeroModel heroModel;
        private BuildModel buildModel;
        //private TroopInfoViewModel troopViewMode;
        private HeroSelectViewModel heroSelectViewModel;

        public Dictionary<string, HeroPosition> Formation {
            get {
                return this.model.formation;
            }
        }

        public HeroSortType HeroSortBy {
            get {
                return this.heroModel.heroSortType;
            }
            set {
                this.heroModel.heroSortType = value;
            }
        }

        public Dictionary<string, Troop> TroopDict {
            get {
                return this.model.troopDict;
            }
        }

        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.buildModel.buildingDict;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public List<Hero> heroList = new List<Hero>();
        public List<Hero> currentTroopHeroNames = new List<Hero>();
        public Coord homeCoord = new Coord();
        public List<Hero> HeroList {
            get {
                //SetGridHeroList();
                return heroList;
            }
            set {
                heroList = value;
            }
        }

        public void RefreshGridHeroList() {
            HeroList = SetGridHeroList();
        }

        private List<Hero> SetGridHeroList() {
            currentTroopHeroNames.Clear();
            foreach (HeroPosition heroPosition in this.TroopDict[this.CurrentTroop].Positions) {
                currentTroopHeroNames.Add(HeroDict[heroPosition.Name]);
            }
            foreach (var pair in this.TroopDict) {
                if (pair.Key != this.CurrentTroop && pair.Value.Coord != this.homeCoord) {
                    foreach (HeroPosition position in pair.Value.Positions) {
                        if (!currentTroopHeroNames.Contains(HeroDict[position.Name])) {
                            currentTroopHeroNames.Add(HeroDict[position.Name]);
                        }
                    }
                }
            }
            heroList = this.heroModel.GetHeroListOrderBySortType(this.HeroSortBy);
            return heroList;
        }

        public string CurrentTroop {
            get {
                return this.model.currentTroop;
            }
            set {
                this.model.currentTroop = value;
            }
        }

        public ArmyCampConf ArmyCampConf {
            get {
                int level = this.BuildingDict[this.TroopDict[this.CurrentTroop].ArmyCamp].Level;
                return ArmyCampConf.GetConf(level.ToString());
            }
        }

        public string ChosenHeroName {
            get {
                return this.model.currentHero;
            }
            set {
                this.model.currentHero = value;
            }
        }

        public string ReplaceHeroName = string.Empty;

        private int currentPosition {
            get; set;
        }

        private bool isEdit = false;
        private bool IsEdit {
            get {
                return isEdit;
            }
            set {
                if (isEdit != value) {
                    this.isEdit = value;
                    this.OnEditStatusChange();
                }
            }
        }

        public bool IsAddHeroSuccess {
            get; set;
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<TroopModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<TroopFormationView>();
            this.heroSelectViewModel =
                                PoolManager.GetObject<HeroSelectViewModel>(this.transform);
            FteManager.SetStartCallback(GameConst.TROOP_ADD_HERO, 3,
                (index) => this.OnTroopStep3Start(index));
            FteManager.SetEndCallback(GameConst.TROOP_ADD_HERO, 3, this.OnTroopStep3End);
            FteManager.SetStartCallback(GameConst.TROOP_ADD_HERO, 4,
                (index) => this.OnTroopStep4Start(index));
            FteManager.SetEndCallback(GameConst.TROOP_ADD_HERO, 4, this.OnTroopStep4End);
            this.IsAddHeroSuccess = false;
            this.homeCoord = new Coord {
                X = (int)RoleManager.GetRoleCoordinate().x,
                Y = (int)RoleManager.GetRoleCoordinate().y
            };
        }

        // Show hide
        #region show_hide
        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this, AddOnMap.HideAll);
            }, true);
            RefreshGridHeroList();
            this.model.ParseFormation();
            this.IsEdit = false;
            this.view.SetBtnRecruit(true);
            this.view.SetTroopFormat();
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        private void OnEditStatusChange() {
            this.view.SetBtnRecruit(!this.IsEdit);
        }

        #endregion

        // Net message
        #region net_message
        public void EditTroopReq() {
            Debug.LogError("EditReq");
            EditTroopReq editTroop = new EditTroopReq();
            editTroop.TroopId = CurrentTroop;
            foreach (HeroPosition heroPosition in this.Formation.Values) {
                editTroop.HeroPositions.Add(heroPosition);
                //Debug.LogError(heroPosition.Position);
            }

            if (this.IsEdit) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.warning_troop_is_editing),
                    TipType.Warn
                );
                return;
            }
            this.view.SetBtnRecruit(false);
            this.IsEdit = true;
            NetManager.SendMessage(editTroop, typeof(EditTroopAck).Name, this.EditTroopAck,
                this.EditTroopErrorCallback, () => {
                    this.IsEdit = false;
                }
            );
        }

        private void EditTroopAck(IExtensible message) {
            EditTroopAck troopUpdateAck = message as EditTroopAck;
            this.IsEdit = false;
            this.TroopDict[troopUpdateAck.Troop.Id] = troopUpdateAck.Troop;
            //foreach (var item in troopUpdateAck.Troop.Positions) {
            //    Debug.LogError(item.Position);
            //}
            this.parent.UpdateTroopStatus(troopUpdateAck.Troop);
            this.view.UpdateTroopFormation();
            this.view.RefreshHero(this.ChosenHeroName);
            if (!this.ReplaceHeroName.CustomIsEmpty()) {
                this.view.RefreshHero(this.ReplaceHeroName);
                this.ReplaceHeroName = string.Empty;
            }
        }

        private void EditTroopErrorCallback(IExtensible message) {
            ErrorAck error = (ErrorAck)message;
            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.notice_title_warning),
                error.Error, null
            );
            this.IsEdit = false;
            this.RemoveFromFormation(this.ChosenHeroName);
            this.view.RemoveHero(this.currentPosition);

        }

        public void ExchangeTroopHeroReq(List<Formation> formationList) {
            ExchangeTroopHeroReq exchangeTroopHeroReq = new Protocol.ExchangeTroopHeroReq();
            for (int i = 0; i < formationList.Count; i++) {
                exchangeTroopHeroReq.Formation.Add(formationList[i]);
            }
            if (this.IsEdit) {
                UIManager.ShowTip(
                    LocalManager.GetValue("warning_troop_is_editing"),
                    TipType.Warn
                );
                return;
            }
            this.view.SetBtnRecruit(false);
            this.IsEdit = true;
            NetManager.SendMessage(exchangeTroopHeroReq, typeof(ExchangeTroopHeroAck).Name,
                this.ExchangeTroopHeroAck,
               this.ExchangeTroopHeroErrorAck, () => {
                   this.IsEdit = false;
                   this.view.SetBtnRecruit(true);
               }
           );
        }
        private void ExchangeTroopHeroAck(IExtensible message) {
            ExchangeTroopHeroAck exchangeTroopHeroAck = message as ExchangeTroopHeroAck;
            model.UpdateFormation(exchangeTroopHeroAck);
            this.IsEdit = false;
            foreach (Troop troop in exchangeTroopHeroAck.Troops) {
                Debug.LogError(troop.Id);
                this.TroopDict[troop.Id] = troop;
                this.parent.UpdateTroopStatus(troop);
                this.view.UpdateTroopFormation();
            }
            this.view.RefreshHero(this.ChosenHeroName);
            if (!this.ReplaceHeroName.CustomIsEmpty()) {
                this.view.RefreshHero(this.ReplaceHeroName);
                this.ReplaceHeroName = string.Empty;
            }
        }

        private void ExchangeTroopHeroErrorAck(IExtensible message) {
            //ErrorAck error = (ErrorAck)message;
            //UIManager.ShowConfirm(
            //    LocalManager.GetValue(LocalHashConst.notice_title_warning), error.Error, null);
            this.IsEdit = false;
            this.parent.UpdateTroopStatus(this.TroopDict[this.CurrentTroop]);
            this.view.UpdateTroopFormation();
        }
        #endregion

        // Data execution functions.
        #region data
        public bool IsInFormation(string name) {
            return this.model.IsInFormation(name);
        }

        public void AddToFormation(string name, int position) {
            this.currentPosition = position;
            this.model.AddToFormation(name, position);
        }

        public void RemoveFromFormation(string name) {
            this.model.RemoveFromFormation(name);
        }

        public List<Hero> GetHeroListOrderBy() {
            return this.heroModel.GetHeroListOrderBy();
        }
        #endregion

        // Communicate with other viewmodel
        #region with_other_viewmodel
        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void ShowHeroInfo(string name,
                                 UnityAction levelUpCallback = null) {
            this.parent.ShowHeroInfo(name, levelUpCallback, isSubWindow: true);
        }

        public void ShowRecruit() {
            //this.gameObject.SetActiveSafe(false);
            this.HideImmediatly();
            this.parent.ShowRecruit(this.CurrentTroop);
        }

        public void HideCurrentHero() {
            this.view.HideCurrentHero();
        }

        public void ShowCurrentHero() {
            this.view.ShowCurrentHero();
        }

        public void ResetSelect() {
            if (this.heroSelectViewModel == null) {
                this.heroSelectViewModel =
                    PoolManager.GetObject<HeroSelectViewModel>(this.transform);
            }
            this.heroSelectViewModel.Reset();
        }

        public void DisableEdit() {
            this.view.DisableEdit();
        }

        #endregion

        // Fte
        #region FTE

        private void OnTroopStep3Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            this.CurrentTroop = FteManager.GetCurrentTroop();
            FteManager.SetCurrentTroop(string.Empty, Vector2.zero, string.Empty);
            // To do: need rewrite when call troopformati
            //this.parent.ViewType = TroopViewType.Format;
            this.Show();
            this.view.OnTroopStep3Start();
        }

        private void OnTroopStep3End() {
            this.view.afterHideCallback = null;
        }

        private void OnTroopStep4Start(string index) {
            this.view.afterHideCallback = () => {
                FteManager.StopFte();
                this.parent.StartChapterDailyGuid();
            };
            this.view.OnTroopStep4Start();
        }

        private void OnTroopStep4End() {
            this.view.OnTroopStep4End();
        }

        public void HideFteFormation() {
            this.view.HideFteFormation();
        }

        public void ShowFteFormation() {
            this.view.ShowFteFormation();
        }

        #endregion

        public void ShowDragHint() {
            this.view.ShowDragHint();
        }

        public void ShowDragSelectHighlight(Vector2 position) {
            this.view.ShowDragSelectHighlight(position);
        }

        public string GetHeroIdByHeroName(string name) {
            return this.HeroDict.ContainsKey(name) ? this.HeroDict[name].Id : null;
        }

        public string GetHeroIdByPosition(int position) {
            foreach (HeroPosition hero in this.Formation.Values) {
                if (hero.Position == position) {
                    return hero.Name;
                }
            }
            return string.Empty;
        }

        public string GetHeroTroopByHeroName(string name) {
            return this.model.GetTroopWithHeroName(name).Id;
        }

        public bool IsHeroInCurrentTroop(string heroName) {
            return this.model.GetHeroTroopName(heroName) == this.model.troopDict[this.CurrentTroop].ArmyCamp;
        }

        public bool IsHeroInTroop(string heroName) {
            return this.GetHeroTroopName(heroName) != string.Empty;
        }

        public string GetHeroTroopName(string heroName) {
            return this.model.GetHeroTroopName(heroName);
        }
    }
}
