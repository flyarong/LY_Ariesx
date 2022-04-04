using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class MiniMapViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private MiniMapModel model;
        private MapModel mapModel;
        private MiniMapView view;

        /* Model data get set */
        public Vector2 Coordinate {
            get {
                return this.model.coordinate;
            }
            set {
                this.model.coordinate = value;
                this.view.OnCoordinateChange();
            }
        }

        public Vector2 MiniCoordinate {
            get {
                return this.mapModel.minCoordinate;
            }
        }

        public Vector2 MaxCoordinate {
            get {
                return this.mapModel.maxCoordinate;
            }
        }

        public int State {
            get {
                return this.model.state;
            }
            set {
                if (this.model.state != value) {
                    this.model.state = value;
                    this.OnStateChange();
                }
            }
        }

        private List<Coord> AllyCoord {
            get {
                return this.model.AllyCoord;
            }
        }

        public Vector2 Center {
            get {
                return this.mapModel.centerCoordinate;
            }
        }

        private Dictionary<Vector2, AllianceOwnedPoint> AllAllianceOwnedPoints {
            get {
                return this.model.AllAllianceOwnedPoints;
            }
        }

        public UnityAction resetChoseAction = null;
        /**********************/

        /* Other members */
        public bool NeedFresh { get; set; }
        public bool IsTileView { get; set; }
        public bool IsFold { get; set; }

        private MiniMapCityViewModel CityViewModel {
            get {
                return this.cityViewModel ??
                       (this.cityViewModel = PoolManager.GetObject<MiniMapCityViewModel>(this.transform));
            }
        }
        private MiniMapCityViewModel cityViewModel;

        private MiniMapPassViewModel PassViewModel {
            get {
                return this.passViewModel ??
                       (this.passViewModel = PoolManager.GetObject<MiniMapPassViewModel>(this.transform));
            }
        }
        private MiniMapPassViewModel passViewModel;

        private MapMarkViewModel markViewModel;
        private MiniMapTileViewModel tileViewModel;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<MiniMapModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<MiniMapView>();
            this.markViewModel =
                PoolManager.GetObject<MapMarkViewModel>(this.transform);
            this.tileViewModel =
                PoolManager.GetObject<MiniMapTileViewModel>(this.transform);
            NetHandler.AddDataHandler(typeof(MiniMapAllyCoordsNtf).Name, this.OnMiniMapAllyCoordChange);
            NetHandler.AddDataHandler(typeof(MiniMapAllianceCoordsNtf).Name,
                this.OnMiniMapAllianceCoordsChange);
            StartCoroutine(this.GetMiniMapAlliancePoints());
        }

        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this, AddOnMap.HideAll);
                this.Coordinate = this.mapModel.centerCoordinate;
            });
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(
                () => this.parent.OnRemoveViewAboveMap(this)
            );
        }

        private void DeleteAllianceMark(Vector2 coord) {
            this.markViewModel.DeleteAllianceMark(coord);
        }

        public void ShowCity(bool status) {
            if (status) {
                this.CityViewModel.Show();
            } else {
                this.CityViewModel.Hide();
            }
        }

        public void ShowPass(bool status) {
            if (status) {
                this.PassViewModel.Show();
            } else {
                this.PassViewModel.Hide();
            }
        }

        public void ShowMark(bool status) {
            if (status) {
                this.markViewModel.Show();
            } else {
                this.markViewModel.Hide();
            }
        }

        public void ShowTile(bool status) {
            this.IsTileView = status;
            if (status) {
                this.tileViewModel.Show();
            } else {
                this.tileViewModel.Hide();
                if (this.IsFold) {
                    this.view.Unfold();
                }
            }
        }

        public void MoveTo(Vector2 coordinate, string local) {
            this.Coordinate = coordinate;
            this.view.ShowName(coordinate, this.State, local);
        }

        public void SetTileViewStatus(bool visible) {
            this.tileViewModel.SetStatus(visible);
        }

        public void DeleteAllianceMark(Coord coordinate) {
            this.parent.DeleteAllianceMark(coordinate);
        }

        public void AddMarkOnTile(Vector2 coordinate) {
            this.parent.AddMarkOnTile(coordinate);
        }

        public void RefreshMarkInTile(Vector2 coord, MapMarkType type, bool isAdd) {
            this.parent.RefreshMarkInTile(coord, type, isAdd);
        }

        public void Mark(string name, Vector2 coordinate) {
            this.markViewModel.AddMarkReq(name, coordinate);
            // this.parent.Mark(name, coordinate);
        }

        public void DeleteMark(Vector2 coordinate,
                               MapMarkType type = MapMarkType.Others) {
            this.markViewModel.DeleteMarkReq(coordinate, type);
        }

        public void Jump() {
            if (this.IsTileView && this.tileViewModel.CurrentCoord != null &&
                this.tileViewModel.CurrentCoord.X == Mathf.RoundToInt(this.Coordinate.x) &&
                this.tileViewModel.CurrentCoord.Y == Mathf.RoundToInt(this.Coordinate.y)) {
                if (!this.IsFold) {
                    this.view.Fold();
                }
                this.view.SetArrowVisible(false);
                this.parent.MoveWithEvent(this.Coordinate, () => {
                    if (this.IsTileView) {
                        this.view.SetArrowVisible(true);
                    }
                });
            } else {
                this.Hide();
                this.parent.MoveWithClick(this.Coordinate);
            }
            if (this.view.IsVisible) {
                this.view.OnAllyCoordsChange();
            }
        }

        public void ResetItemChosen() {
            this.resetChoseAction.InvokeSafe();
        }

        private void OnStateChange() {
            this.CityViewModel.OnStateChange();
            this.PassViewModel.OnStateChange();
        }

        /* Add 'NetMessageAck' function here*/

        private IEnumerator GetMiniMapAlliancePoints() {
            yield return YieldManager.EndOfFrame;
            GetAllAlliancePointsReq getAllAlliancePoints = new GetAllAlliancePointsReq();
            NetManager.SendMessage(getAllAlliancePoints,
                typeof(GetAllAlliancePointsAck).Name, this.OnGetAllAlliancePoints);
        }

        private void OnGetAllAlliancePoints(IExtensible message) {
            this.AllAllianceOwnedPoints.Clear();
            List<AllianceOwnedPoint> points = (message as GetAllAlliancePointsAck).Points;
            foreach (AllianceOwnedPoint point in points) {
                this.AllAllianceOwnedPoints.Add(point.Coord, point);
            }
        }

        private void OnMiniMapAllyCoordChange(IExtensible message) {
            MiniMapAllyCoordsNtf miniMapAllyCoords = message as MiniMapAllyCoordsNtf;
            List<Coord> Coords = miniMapAllyCoords.Coords;
            if (Coords.Count < 1) {
                return;
            }

            if (miniMapAllyCoords.Method.CustomEquals("del")) {
                foreach (Coord coord in Coords) {
                    this.AllyCoord.TryRemove(coord);
                }
            } else {
                foreach (Coord coord in Coords) {
                    this.AllyCoord.TryAdd(coord);
                }
            }
        }

        private void OnMiniMapAllianceCoordsChange(IExtensible message) {
            MiniMapAllianceCoordsNtf allianceCoords = message as MiniMapAllianceCoordsNtf;
            //Debug.LogError(allianceCoords.Method + " " + allianceCoords.Points);
            if (allianceCoords.Method.CustomEquals("del")) {
                foreach (AllianceOwnedPoint point in allianceCoords.Points) {
                    Debug.LogError("OnMiniMapAllianceCoordsChange del " + point.Coord);
                    this.AllAllianceOwnedPoints.TryRemove(point.Coord);
                }
            } else {
                foreach (AllianceOwnedPoint point in allianceCoords.Points) {
                    Debug.LogError("OnMiniMapAllianceCoordsChange add " + point.Coord);
                    this.AllAllianceOwnedPoints[point.Coord] = point;
                }
            }

            if (this.view.IsVisible) {
                this.view.OnMiniMapAllianceCoordsChange();
            }
        }
        /***********************************/
    }
}
