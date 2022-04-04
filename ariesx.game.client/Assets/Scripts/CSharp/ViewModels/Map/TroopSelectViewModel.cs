using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class TroopSelectViewModel : BaseViewModel {
        private MapTileViewModel parent;
        private MapTileModel model;
        public TroopModel troopModel;
        private HeroModel heroModel;
        private TroopSelectView view;

        public List<Troop> StationTroopList {
            get {
                return this.troopModel.GetTroopsAt(this.model.tileInfo.coordinate);
            }
        }

        public TroopViewType TroopViewType {
            get; set;
        }

        public bool IsAttack {
            get; set;
        }

        public int Page {
            get; set;
        }

        public bool IsLoadAll {
            get; set;
        }

        public Vector2 Target {
            get {
                return this.model.tileInfo.coordinate;
            }
        }

        public bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        private const int pageCount = 20;

        public List<Troop> CurrentTroopList {
            get {
                return this.model.currentTroopList;
            }
            set {
                this.model.currentTroopList = value;
            }
        }

        public int CurrentTroopCount {
            get {
                return this.CurrentTroopList.Count;
            }
        }

        public List<GetPointPlayerTroopsAck.Troop> PlayerTroopList {
            get {
                return this.model.playerTroopList;
            }
        }

        public Dictionary<string, GetPointPlayerTroopsAck.Troop> PlayerTroopDict {
            get {
                return this.model.playerTroopDict;
            }
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapTileViewModel>();
            this.model = ModelManager.GetModelData<MapTileModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.view = this.gameObject.AddComponent<TroopSelectView>();

            this.NeedRefresh = true;
        }

        public void ShowAvaliable(List<Troop> avaliableTroopList) {
            this.Show();
            this.view.ShowTroop();
            this.view.ResetChoseCallback();
            this.IsAttack = true;
            this.NeedRefresh = true;
            this.CurrentTroopList = avaliableTroopList;
            if (this.NeedRefresh) {
                this.view.ResetItems(this.CurrentTroopList.Count);
            }
            this.view.EnableShining();
        }

        public void ShowStation(TroopViewType viewType, int troopCount) {
            this.Show();
            this.IsAttack = false;
            this.TroopViewType = viewType;
            this.Page = 0;
            this.NeedRefresh = true;
            this.IsLoadAll = false;
            this.CurrentTroopList = this.StationTroopList;
            if (troopCount == 0) {
                this.view.ShowTroop();
                this.view.ResetChoseCallback();
                if (this.NeedRefresh) {
                    this.view.ResetItems(this.CurrentTroopList.Count);
                }
            } else {
                this.view.HideTroop();
                this.GetTroopsReq();
            }
        }

        public void Show() {
            this.view.Show();
        }

        // To do: need debug log collector, DEBUG 15301
        public void ShowTroopChose(TroopChoseAction type) {
            if (this.StationTroopList.Count < 1) {
                this.parent.Hide();
                return;
            }
            this.view.ResetItems(this.CurrentTroopCount);
            this.view.EnableShining();
            this.view.SetChosenAction(type);
        }

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerInfo(playerId);
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        //public void ShowTroop(string id, TroopViewType mode) {
        //    this.parent.ShowTroopInfo(id, mode);
        //}

        public void ShowTroopFormation(string id) {
            this.parent.Hide();
            this.parent.ShowTroopFormation(id);
        }

        public void ShowRecruit(string troopId) {
            this.parent.HideTileInfo();
            this.parent.ShowRecruit(troopId);
        }

        public void ShowTroop(Troop troop) {
            if (!TroopModel.CheckTroopIsRecruiting(troop)) {
                this.parent.ShowTroopInfo(troop.Id, this.TroopViewType);
            } else {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.hero_recruiting),
                    TipType.Notice);
            }
        }

        public void ShowTroopOverview(Troop troop) {
            this.parent.ShowTroop(troop.Id);
        }

        public void SetHighlight(string id) {
            this.view.SetHighlight(id);
        }

        public void TroopReturnReq(string troopId) {
            this.parent.TroopReturnReq(troopId);
        }

        public void GetTroopsReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            GetPointPlayerTroopsReq req = new GetPointPlayerTroopsReq();
            this.isRequestPage = true;
            req.Coord = new Coord();
            req.Coord.X = Mathf.RoundToInt(this.Target.x);
            req.Coord.Y = Mathf.RoundToInt(this.Target.y);
            req.Page = ++this.Page;
            NetManager.SendMessage(req, typeof(GetPointPlayerTroopsAck).Name, this.GetTroopsAck);
        }

        private void GetTroopsAck(IExtensible message) {
            GetPointPlayerTroopsAck ack = message as GetPointPlayerTroopsAck;
            this.isRequestPage = false;
            this.view.ShowTroop();
            if (ack.Troops.Count < pageCount) {
                this.IsLoadAll = true;
            }
            if (this.NeedRefresh) {
                this.PlayerTroopList.Clear();
                this.PlayerTroopDict.Clear();
            }
            int originCount = this.PlayerTroopList.Count + this.CurrentTroopCount;
            int newDataCount = 0;

            foreach (GetPointPlayerTroopsAck.Troop troop in ack.Troops) {

                if (!this.PlayerTroopDict.ContainsKey(troop.TroopId) &&
                    troop.PlayerId != RoleManager.GetRoleId()) {
                    newDataCount++;
                    this.PlayerTroopList.Add(troop);
                    this.PlayerTroopDict.Add(troop.TroopId, troop);
                }
            }
            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                //Debug.LogError(newDataCount + this.CurrentTroopCount);
                this.view.ResetItems(newDataCount + this.CurrentTroopCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetTroopsReq;
                }
            } else {
                this.view.ResetItems(originCount + newDataCount,
                    keepVelocity: true, needRefresh: false);
            }
        }

        public void CheckTroopList(List<Troop> troopList, bool needSorted) {
            if (needSorted) {
                troopList.Sort((a, b) => {
                    Vector2 coordinateA = new Vector2(a.Coord.X, a.Coord.Y);
                    Vector2 coordinateB = new Vector2(b.Coord.X, b.Coord.Y);
                    float sqrDistanceA = (this.model.tileInfo.coordinate - coordinateA).sqrMagnitude;
                    float sqrDistanceB = (this.model.tileInfo.coordinate - coordinateB).sqrMagnitude;
                    if (sqrDistanceA > sqrDistanceB) {
                        return 1;
                    } else if (sqrDistanceA == sqrDistanceB) {
                        return a.ArmyCamp.CompareTo(b.ArmyCamp);
                    } else {
                        return -1;
                    }
                });
            }

            //// To do: when troop is marching the distance is wrong.
            //List<Troop> removeList = new List<Troop>();
            //if (this.IsAttack) {
            //    Vector2 position;
            //    foreach (Troop troop in troopList) {
            //        position = new Vector2(troop.Coord.X, troop.Coord.Y);
            //        //float distance = Vector2.Distance(this.Target, position);
            //        if ((this.Target - position).sqrMagnitude > GameConst.TROOP_FAR * GameConst.TROOP_FAR) {
            //            removeList.Add(troop);
            //        }
            //    }
            //}

            //foreach (Troop troop in removeList) {
            //    troopList.Remove(troop);
            //}
        }

        public void Hide() {
            this.view.Hide();
        }

        #region FTE

        public void OnTroopStep2Process() {
            this.view.OnTroopStep2Process();
        }

        public void OnResourceStep3Start(bool isEnforce) {
            this.view.OnResourceStep3Start(isEnforce);
        }

        public void OnResourceStep3End() {
            this.view.afterHideCallback = null;
            string troop = FteManager.GetCurrentTroop();
            this.ShowTroop(this.troopModel.troopDict[troop]);
        }

        #endregion
    }
}
