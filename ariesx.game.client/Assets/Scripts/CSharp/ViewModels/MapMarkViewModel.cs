using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class MapMarkViewModel : BaseViewModel {
        private MiniMapViewModel parent;
        private MapMarkModel model;
        private BuildModel buildModel;
        private MapMarkView view;
        /* Model data get set */
        public List<MapMark> MarkList {
            get {
                return this.model.markList;
            }
        }

        public Dictionary<Vector3, MapMark> MarkDict {
            get {
                return this.model.markDict;
            }
        }

        public Vector2 CurrentCoord {
            get; set;
        }
        /* Other members */
        private bool NeedRefresh {
            get; set;
        }
        public bool IsSelectItem { get; set; }

        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<MapMarkView>();
            this.parent = this.transform.parent.GetComponent<MiniMapViewModel>();
            this.model = ModelManager.GetModelData<MapMarkModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            NetHandler.AddDataHandler(typeof(AllMarksNtf).Name, this.AllMarksNtf);
            NetHandler.AddNtfHandler(typeof(EventBuildNtf).Name, this.EventBuildNtf);
            TriggerManager.Regist(Trigger.BeenKickedOutAlliance, this.OnBeenKickcedOutALliance);
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            this.parent.resetChoseAction = this.view.ResetItemChosen;
            if (this.NeedRefresh) {
                this.view.SetContent();
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void DeleteAllianceMark(Vector2 coord) {
            this.DeleteMapMark(coord, MapMarkType.Alliance);
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void MoveTo(Vector2 coordinate) {
            this.parent.Coordinate = coordinate;
        }

        public void AddMarkReq(string name, Vector2 coordinate) {
            AddMarkReq addMarkReq = new AddMarkReq() {
                Name = name,
                Coord = new Coord()
            };
            addMarkReq.Coord.X = Mathf.RoundToInt(coordinate.x);
            addMarkReq.Coord.Y = Mathf.RoundToInt(coordinate.y);
            NetManager.SendMessage(addMarkReq, typeof(AddMarkAck).Name, this.AddMarkAck);
        }

        private void OnBeenKickcedOutALliance() {
            List<MapMark> allianceMarkList = new List<MapMark>();
            int markCount = this.MarkList.Count;
            MapMark mark;
            for (int index = 0; index < markCount; index++) {
                mark = this.MarkList[index];
                if (mark.type == MapMarkType.Alliance) {
                    allianceMarkList.Add(mark);
                }
            }

            int allianceMarkCount = allianceMarkList.Count;
            Vector2 coordinate = Vector2.zero;
            //Debug.LogError("MapMarkViewModel BeenKickedOutAlliance " + allianceMarkCount);
            for (int index = 0; index < allianceMarkCount; index++) {
                mark = allianceMarkList[index];
                coordinate.x = mark.mark.Coord.X;
                coordinate.y = mark.mark.Coord.Y;
                this.DeleteMapMark(coordinate, MapMarkType.Alliance);
            }
        }

        private void DeleteMapMark(Vector2 coord, MapMarkType markType) {
            Vector3 coordinate = new Vector3(
                coord.x,
                coord.y,
                (int)markType
            );
            MapMark mapMark;
            if (this.MarkDict.TryGetValue(coordinate, out mapMark)) {
                Debug.LogError("DeleteMapMark " + coordinate + " " + markType);
                this.MarkDict.Remove(coordinate);
                this.MarkList.Remove(mapMark);
                this.parent.RefreshMarkInTile(coordinate, markType, false);
            }
            this.view.SetContent();
        }

        /* Add 'NetMessageAck' function here*/
        private void AllMarksNtf(IExtensible message) {
            AllMarksNtf allMarksNtf = message as AllMarksNtf;
            this.model.Rrefresh(allMarksNtf,
                buildModel.GetBuildingByType(ElementType.stronghold));
            foreach (var pair in this.MarkDict) {
                this.parent.RefreshMarkInTile(pair.Key, pair.Value.type, true);
            }
        }

        private void EventBuildNtf(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            if (eventBuildNtf.EventBuild.BuildingName.Contains(ElementName.stronghold)) {
                if (eventBuildNtf.Method.CustomEquals("del")) {
                    this.model.AddStrongHold(eventBuildNtf.EventBuild.Coord,
                        eventBuildNtf.EventBuild.BuildingName);
                }
                this.NeedRefresh = true;
            }
        }

        private void DeleteAllianceMarkReq(Coord coordinate) {
            this.parent.DeleteAllianceMark(coordinate);
        }

        private void AddMarkAck(IExtensible message) {
            AddMarkAck addMarkAck = message as AddMarkAck;
            MapMark mapMark = new MapMark {
                mark = addMarkAck.Mark,
                type = MapMarkType.Others,
                isNew = true
            };
            Vector3 coordinate = new Vector3(
                addMarkAck.Mark.Coord.X,
                addMarkAck.Mark.Coord.Y,
                (int)MapMarkType.Others
            );
            if (!this.MarkDict.ContainsKey(coordinate)) {
                this.MarkList.Insert(0, mapMark);
                this.MarkDict.Add(coordinate, mapMark);
                this.parent.RefreshMarkInTile(coordinate, MapMarkType.Others, true);
            } else {
                this.parent.AddMarkOnTile(coordinate);
            }
            this.NeedRefresh = true;
        }

        public void DeleteMarkReq(Coord coordinate, MapMarkType markType) {
            switch (markType) {
                case MapMarkType.Others:
                    this.DeleteNormalMark(coordinate);
                    break;
                case MapMarkType.Alliance:
                    this.DeleteAllianceMarkReq(coordinate);
                    break;
                default:
                    break;
            }
        }

        private void DeleteNormalMark(Coord coordinate) {
            DelMarkReq deleteMark = new DelMarkReq() {
                Coord = new Coord(coordinate)
            };
            NetManager.SendMessage(deleteMark, typeof(DelMarkAck).Name,
                (message) => {
                    DelMarkAck deleteMarkAck = message as DelMarkAck;
                    this.DeleteMapMark(deleteMarkAck.Coord, MapMarkType.Others);
                });
        }

        private void ResortMarkList(List<MapMark> markList) {
            markList.Sort((a, b) => {
                return a.isNew.CompareTo(b.isNew);
            });
        }
        /***********************************/
    }
}
