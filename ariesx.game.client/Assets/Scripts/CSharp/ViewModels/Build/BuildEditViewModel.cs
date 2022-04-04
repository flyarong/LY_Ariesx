using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class BuildEditViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private BuildModel model;
        private MapModel mapModel;
        private BuildEditView view;

        /* Model data get set */
        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.model.buildingDict;
            }
        }

        public string Building {
            get {
                return this.model.currentBuilding;
            }
        }

        public Vector2 MapCenter {
            get {
                return this.mapModel.centerCoordinate;
            }
        }

        public Vector2 BuildCoordinate {
            get {
                return this.model.buildCoordinate;
            }
            set {
                //  if (this.model.buildCoordinate != value) {
                this.model.buildCoordinate = value;
                this.view.OnCoordinateChange();
                //  }
            }
        }

        public UnityAction moveEvent = null;

        public bool IsFixed {
            get; set;
        }
        public Vector2 FixedCoord {
            get; set;
        }
        /**********************/



        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<BuildModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<BuildEditView>();

            // FTE
            //FteManager.SetStartCallback(GameConst.NORMAL, 61, this.OnFteStep61Start);
            //FteManager.SetEndCallback(GameConst.NORMAL, 61, this.OnFteStep61End);
            //FteManager.SetStartCallback(GameConst.NORMAL, 71, this.OnFteStep71Start);
            FteManager.SetStartCallback(GameConst.BUILDING_LEVEL, 3, this.OnBuildStep3Start);
            FteManager.SetEndCallback(GameConst.BUILDING_LEVEL, 3, this.OnBuildStep3End);
            FteManager.SetStartCallback(GameConst.BUILDING_LEVEL, 4, this.OnBuildStep4Start);
        }

        public void Show() {
            this.view.afterShowCallback = this.view.SetBuildEditor;
            this.view.Show(callback: () => {
                this.parent.OnAddViewAboveMap(this, AddOnMap.Edit);
            });
        }

        public void Hide() {
            this.view.Hide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });

        }

        public void HideImmediatly() {
            this.view.HideImmediatly(
                () => this.parent.OnRemoveViewAboveMap(this)
            );
        }

        public void ShowGetTileTip() {
            this.Hide();
            this.parent.ShowGetTileTip();
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        public void JumpTo(Vector2 coordinate) {
            this.parent.MoveWithEvent(coordinate, this.moveEvent);
            this.moveEvent = null;
        }

        public bool IsAvaliable(Vector2 coord) {
            return this.mapModel.IsAvaliable(coord, string.Concat(this.Building, "_1"));
        }

        public Vector2 GetNearestTile() {
            Vector2 coordinate = RoleManager.GetRoleCoordinate();
            float distance = Mathf.Infinity;
            foreach (Point point in RoleManager.GetPointDict().Values) {
                Vector2 position = new Vector2(point.Coord.X, point.Coord.Y);
                float sqrCurDistance = (this.MapCenter - position).sqrMagnitude;
                if (this.IsAvaliable(point) &&
                    sqrCurDistance < distance) {
                    coordinate = position;
                    distance = sqrCurDistance;
                }
            }
            return coordinate;
        }

        private bool IsAvaliable(Point point) {
            Vector2 position = new Vector2(point.Coord.X, point.Coord.Y);
            return this.IsAvaliable(position);
        }

        public void SetIsFixd() {
            this.parent.SetIsFixed();
        }

        #region FTE

        //private void OnFteStep61Start(string index) {
        //    this.model.currentBuilding = ElementName.townhall;
        //    this.moveEvent = this.view.OnFteStep61Start;
        //    this.Show();
        //}

        //private void OnFteStep61End() {
        //    this.view.OnFteStep61End();
        //    this.Hide();
        //}

        //private void OnFteStep71Start(string index) {
        //    this.model.currentBuilding = ElementName.townhall;
        //    FteManager.SetCurrentBuilding(this.model.currentBuilding);
        //    SendBuildRequest();
        //}

        private void OnBuildStep3Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            this.moveEvent = this.view.OnBuildStep3Start;
            this.Show();
        }

        private void OnBuildStep3End() {
            this.view.afterHideCallback = null;
            this.Hide();
        }

        private void OnBuildStep4Start(string index) {
            bool autoNext = index.Contains("chapter_task_5") || (index.Contains("chapter_task_1"));
            this.view.OnBuildStep4Start();
            FteManager.EndFte(true, autoNext);
            this.parent.StartChapterDailyGuid();
        }

        #endregion


        public void SendBuildRequest() {
            if (EventManager.IsBuildEventFull()) {
                //Debug.LogError("ShowUnlock");
                this.parent.ShowUnlockBuild(this.BuildReq, true);
            } else {
                this.BuildReq();
            }
        }

        public void BuildReq() {
            BuildReq buildReq = new BuildReq() {
                Name = this.Building,
                Coord = new Coord()
            };
            buildReq.Coord.X = Mathf.RoundToInt(this.BuildCoordinate.x);
            buildReq.Coord.Y = Mathf.RoundToInt(this.BuildCoordinate.y);
            this.view.SetBtnOKDisable();
            NetManager.SendMessage(buildReq, typeof(BuildAck).Name, this.BuildAck,
                (message) => { this.view.SetBtnOKEnable(); }, this.view.SetBtnOKEnable);
        }

        /* Add 'NetMessageAck' function here*/
        private void BuildAck(IExtensible message) {
            this.view.SetBtnOKEnable();
            this.Hide();
        }
        /***********************************/
    }
}
