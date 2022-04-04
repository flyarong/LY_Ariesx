using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {

    public class MarchViewModel : BaseViewModel, IViewModel {
        private MapTileViewModel parent;
        private TroopModel troopModel;
        private HeroModel heroModel;
        private BuildModel buildModel;
        private MarchModel model;
        private MarchView view;

        public EventMarchClient CurrentMarch {
            get {
                return this.model.march;
            }
            set {
                this.model.march = value;
                this.view.OnMarchChange();
            }
        }

        public MarchAttributes TroopAttributes {
            get {
                return this.troopModel.GetMarchAttributes(this.CurrentTroop.Id);
            }
        }

        public MarchAttributes MarchAttributes {
            get {
                return this.troopModel.GetMarchAttributes(this.CurrentMarch.troop.Id);
            }
        }

        public Troop CurrentTroop {
            get {
                return this.model.troop;
            }
            set {
                this.model.troop = value;
                this.model.ParseFormation();
                this.view.OnTroopChange();
            }
        }

        public Dictionary<string, HeroPosition> Formation {
            get {
                return this.model.formation;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.buildModel.buildingDict;
            }
        }

        public bool NeedTroopFormat { get; set; }
        private bool fteCanShow = true;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapTileViewModel>();
            this.model = ModelManager.GetModelData<MarchModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<MarchView>();
        }

        public void ShowTroop(Troop troop) {
            if (fteCanShow) {
                this.Show();
                this.CurrentTroop = troop;
            }
        }

        public void ShowMarch(EventMarchClient march) {
            this.Show();
            this.CurrentMarch = march;
        }

        public void Show() {
            this.view.PlayShow();
        }

        public void Hide() {
            this.view.PlayHide();
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.view.OnMarchChange();
                this.view.OnTroopChange();
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public Transform GetMarchBind() {
            return this.view.GetMarchBind();
        }

        public void ShowHeroInfo(string name, UnityAction levelUpCallback) {
            this.parent.ShowHeroInfo(name, levelUpCallback, HeroInfoType.Self);
        }

        public void ShowTroopFormation(string id) {
            if (this.CurrentTroop.Idle) {
                ElementBuilding building = buildModel.GetBuildingByCoord(this.CurrentTroop.Coord);
                if (building != null &&
                    (building.Name.CustomEquals(ElementName.townhall) ||
                    building.Name.Contains(ElementName.stronghold))) {
                    this.Hide();
                    this.parent.Hide();
                    this.parent.ShowTroopFormation(id);
                }
            }
        }

        public void ShowPay() {
            this.parent.ShowPay();
        }

        public void EditTroopReq() {
            EditTroopReq editTroop = new EditTroopReq() {
                TroopId = this.CurrentTroop.Id
            };
            foreach (HeroPosition heroPosition in this.Formation.Values) {
                editTroop.HeroPositions.Add(heroPosition);
            }
            NetManager.SendMessage(editTroop, string.Empty, null);
            this.NeedTroopFormat = false;
        }

        public void TroopReturnReq() {
            CallbackTroopReq callbackTroopReq = new CallbackTroopReq() {
                Id = this.CurrentTroop.Id
            };
            NetManager.SendMessage(callbackTroopReq, string.Empty, null);
            this.Hide();
            this.parent.HideTileInfo();
        }

        public void RetreatReq() {
            CancelMarchReq cancelMarchReq = new CancelMarchReq() {
                Id = this.CurrentMarch.id
            };
            NetManager.SendMessage(cancelMarchReq, typeof(CancelMarchAck).Name, this.RetreatAck);
        }

        private void RetreatAck(IExtensible message) {
            this.parent.StopJumping();
            this.Hide();
        }

        public void CompleteTroopMarchReq() {
            Debug.Log(this.CurrentMarch.troop.Id);
            //CompleteTroopMarchReq callbackTroopReq = new Protocol.CompleteTroopMarchReq() {
            //    TroopId = this.CurrentMarch.troop.Id
            //};
            //NetManager.SendMessage(callbackTroopReq,
            //    typeof(CompleteTroopMarchReq).Name, this.CompleteTroopMarchAck);
        }

        private void CompleteTroopMarchAck(IExtensible message) {
            this.Hide();
        }

        public void SetFteCanShow(bool fteCanShow) {
            this.fteCanShow = fteCanShow;
        }

        public bool IsTownhall() {
            Debug.Log(this.CurrentMarch.target);
            Coord coord = new Coord {
                X = (int)this.CurrentMarch.target.x,
                Y = (int)this.CurrentMarch.target.y
            };
            if (this.buildModel.GetBuildingByCoord(coord) == null) {
                return false;
            }
            return this.buildModel.GetBuildingByCoord(coord).Name == "townhall";
        }
    }
}
