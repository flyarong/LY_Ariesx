using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public enum MapMarkType {
        Alliance = 1,
        TownHall,
        StrongHold,
        Others
    }

    public class MapMark {
        public Mark mark;
        public MapMarkType type;
        public bool isNew;
    }

    public class MapMarkModel: BaseModel {
        public List<MapMark> markList = new List<MapMark>(20);
        public Dictionary<Vector3, MapMark> markDict =
                new Dictionary<Vector3, MapMark>(20);

        public void Rrefresh(AllMarksNtf allMarksNtf, List<ElementBuilding> strongHoldList) {
            this.markList.Clear();
            this.markDict.Clear();
            foreach (Mark mark in allMarksNtf.Marks) {
                MapMark mapMark = new MapMark {
                    mark = mark,
                    type = MapMarkType.Others
                };
                Vector3 tmpCoord = new Vector3(mark.Coord.X, mark.Coord.Y, (int)mapMark.type);
                if (!this.markDict.ContainsKey(tmpCoord)) {
                    this.markDict.Add(tmpCoord, mapMark);
                }               
                this.markList.Add(mapMark);
            }            
            this.InsertTownHallCoord();
            this.AddStrongHold(strongHoldList);
        }

        private void InsertTownHallCoord() {
            Mark townHallMark = new Mark() {
                Coord = new Coord()
            };
            Vector2 townHallCoord = RoleManager.GetRoleCoordinate();
            townHallMark.Coord.X = Mathf.RoundToInt(townHallCoord.x);
            townHallMark.Coord.Y = Mathf.RoundToInt(townHallCoord.y);
            townHallMark.Name = LocalManager.GetValue(LocalHashConst.name_townhall);
            MapMark townHallMapMark = new MapMark {
                mark = townHallMark,
                type = MapMarkType.TownHall,
                isNew = false
            };
            Vector3 townhallCoord = new Vector3(
                                        townHallMark.Coord.X,
                                        townHallMark.Coord.Y,
                                        (int)MapMarkType.TownHall);
            this.markDict.Add(townhallCoord, townHallMapMark);
            this.markList.Add(townHallMapMark);
        }

        private void AddStrongHold(List<ElementBuilding> strongHoldList) {
            foreach (ElementBuilding building in strongHoldList) {
                if (building.IsBroken) continue;
                Mark mark = new Mark() {
                    Coord = new Coord()
                };
                mark.Coord = building.Coord;
                mark.Name = LocalManager.GetValue(LocalHashConst.name_stronghold);
                MapMark mapMark = new MapMark {
                    mark = mark,
                    type = MapMarkType.StrongHold,
                    isNew = false
                };
                Vector3 coordinate = new Vector3(mark.Coord.X, mark.Coord.Y,
                                                 (int)MapMarkType.StrongHold);
                if (!this.markDict.ContainsKey(coordinate)) {
                    this.markDict.Add(coordinate, mapMark);
                    this.markList.Add(mapMark);
                }
            }
        }

        public void RemoveStrongHold(Vector2 coordinate) {
            Vector3 tmpCoordinate =
                new Vector3(coordinate.x, coordinate.y, (int)MapMarkType.StrongHold);
            MapMark mapMark;
            if (this.markDict.TryGetValue(tmpCoordinate, out mapMark)) {
                this.markDict.Remove(tmpCoordinate);
                this.markList.Remove(mapMark);
            }
        }

        public void AddStrongHold(Vector2 coordinate, string name) {
            Vector3 tmpCoordinate =
                new Vector3(coordinate.x, coordinate.y, (int)MapMarkType.StrongHold);
            if (!this.markDict.ContainsKey(tmpCoordinate)) {
                Mark mark = new Mark() {
                    Name = LocalManager.GetValue(LocalHashConst.name_stronghold),
                    Coord = new Coord() {
                        X = Mathf.RoundToInt(tmpCoordinate.x),
                        Y = Mathf.RoundToInt(tmpCoordinate.y)
                    }
                };
                MapMark mapMark = new MapMark {
                    mark = mark,
                    type = MapMarkType.StrongHold
                };
                //this.markList.Insert(this.allianceCount + 1, mapMark);
                this.markList.Add(mapMark);
                this.markDict.Add(tmpCoordinate, mapMark);
            }           
        }

    }
}
