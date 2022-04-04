using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class MapTileDetailViewModel : BaseViewModel, IViewModel {
        private MapTileViewModel parent;
        private MapTileModel model;
        private MapModel mapModel;
        private MapTileDetailView view;


        public MapTileInfo TileInfo {
            get {
                return this.model.tileInfo;
            }
        }

        public TilePage Page {
            get {
                return this.model.page;
            }
        }

        public bool ViewVisible {
            get {
                return this.view.IsVisible;
            }
        }

        /**********************************************/

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<MapTileViewModel>();
            this.model = ModelManager.GetModelData<MapTileModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<MapTileDetailView>();
        }

        public void Show(GetPointNpcTroopsAck troopInfoAck) {
            this.view.PlayShow();
            if (troopInfoAck != null) {
                this.SetTroopInfo(troopInfoAck);
            } else {
                this.GetNpcTroopReq();
            }
        }

        public void Show(GetMonsterByCoordAck monsterInfoAck) {
            this.view.PlayShow();
            if (monsterInfoAck != null) {
                this.SetMonsterTroopInfo(monsterInfoAck);
            } else {
                this.GetMonsterReq();
            }
        }

        public void SetTroopInfo(GetPointNpcTroopsAck troopInfoAck) {
            if (this.view.IsVisible) {
                NpcTroop troop = null;
                if (troopInfoAck.Troops.Count > 0) {
                    troop = troopInfoAck.Troops[0];
                } else if (troopInfoAck.DefenceTroop != null) {
                    troop = troopInfoAck.DefenceTroop;
                }
                this.view.SetTroopGrid(troop,
                        troopInfoAck.Count, troopInfoAck.TotalCount);
            }
        }

        public void SetMonsterTroopInfo(GetMonsterByCoordAck ack) {
            if (this.view.IsVisible) {
                MonsterTroop troop = null;
                if (ack.Troops.Count > 0) {
                    troop = ack.Troops[0];
                }
                this.view.SetMonsterGridInfo(troop);
            }
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void Return() {
            if (this.Page == TilePage.Sub &&
                this.view.IsVisible) {
                this.parent.ShowAnimation = false;
                this.parent.Page = TilePage.Main;
            } else {
                this.Hide();
            }
        }

        public Point GetPlayerPoint(Vector2 coordinate) {
            return this.mapModel.GetPlayerPoint(coordinate);
        }

        public void ShowHeroInfo(Hero hero, HeroInfoType infoType = HeroInfoType.Others) {
            this.parent.ShowHeroInfo(hero, infoType);
        }


        #region net_message
        private bool getNpcTrooping = false;
        private void GetNpcTroopReq() {
            if (this.getNpcTrooping) {
                return;
            }
            this.getNpcTrooping = true;
            GetPointNpcTroopsReq req = new GetPointNpcTroopsReq() {
                Coord = this.TileInfo.coordinate
            };
            NetManager.SendMessage(req, typeof(GetPointNpcTroopsAck).Name, this.GetNpcTroopAck);
        }

        private void GetNpcTroopAck(IExtensible message) {
            GetPointNpcTroopsAck ack = message as GetPointNpcTroopsAck;
            this.SetTroopInfo(ack);
            this.getNpcTrooping = false;
        }


        private void GetMonsterReq() {
            GetMonsterByCoordReq req = new GetMonsterByCoordReq() {
                Coord = this.TileInfo.coordinate
            };
            NetManager.SendMessage(req, typeof(GetMonsterByCoordAck).Name, this.GetMonsterAck);
        }


        private void GetMonsterAck(IExtensible message) {
            GetMonsterByCoordAck ack = message as GetMonsterByCoordAck;
            this.SetMonsterTroopInfo(ack);
        }
        #endregion
    }
}
