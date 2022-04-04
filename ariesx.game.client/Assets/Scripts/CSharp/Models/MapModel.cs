using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public delegate void RrefreshDelegate(Point point, bool needCheckSight, int sightRadius);

    public enum TileLayer {
        Below,
        Base,
        Level,
        Above,
        Camp,
        Border,
        BuildingRelation
    }

    public class MapCameraInfo {
        public float height = -12;
        public Vector3 euler = Vector3.zero;
        public float orginSize = 16.2f;
        public float maxSize = 11.7f;
        public float minSize = 8.775f;
        public float hideMarchThreshold = 9f;
        public float showMarchThreshold = 9.5f;
    }

    public class MapTileInfo {
        public string name;
        public int level;
        public Vector2 coordinate;
        public MapTileRelation relation;
        public string type;
        public int defender;
        public ElementBuilding buildingInfo = null;
        public ElementCamp camp = null;
        public NPCCityConf city = null;
        public MiniMapPassConf pass = null;
        public int endurance;
        public int maxEndurance;
        public bool isProtected;
        public TileProtectType tileProtectType;
        public long defenderRecoverTimeAt;
        public string playerName = string.Empty;
        public string playerId = string.Empty;
        public string allianceName = string.Empty;
        public string allianceId = string.Empty;
        public int allianceIcon = 0;
        public string roleAvatar = string.Empty;
        public bool isFallen = false;
        public bool isVisible = true;
        public int troopCount = 0;
    }

    // resourceBlock playerBlock terrianBlock
    public class MapModel : BaseModel {
        public MapCameraInfo cameraInfo = new MapCameraInfo();
        public MapResouceInfo resourceInfo = new MapResouceInfo();
        public MapPlayerInfo playerInfo = new MapPlayerInfo();
        public MapTileInfo currentTile = new MapTileInfo();
        // Map marks
        public List<Protocol.Mark> mapMarkList = new List<Mark>();
        // Special coordinate
        public Vector2 originCoordinate = Vector2.zero;
        public Vector2 centerCoordinate = Vector2.zero;
        public Vector2 targetCoordiante = Vector2.zero;

        public Vector2 minCoordinate = Vector2.one;
        public Vector2 maxCoordinate;

        // TreasureMap
        public int treasureMapAmount = -1;
        public long treasureMapRefresh = 0;
        public Dictionary<string, TextAsset> blockBytesDict = new Dictionary<string, TextAsset>();


        public MapModel() {
            this.InitMap();
        }

        // Need optimize;
        public void RrefreshPlayerInfo(FeedBlocksNtf feedBlockNtf,
                                       Dictionary<string, UnityEvent> feedBlocksEventDict,
                                       RrefreshDelegate callback = null) {
            UnityEvent blockEvent;
            bool needCheckSight;
            int sightRadius;
            foreach (Block block in feedBlockNtf.Blocks) {
                this.playerInfo.AddBlock(block.Offset);
                if (feedBlocksEventDict != null &&
                    feedBlocksEventDict.TryGetValue(block.Offset, out blockEvent)) {
                    blockEvent.Invoke();
                    blockEvent.RemoveAllListeners();
                    feedBlocksEventDict.Remove(block.Offset);
                }
                foreach (Point point in block.Points) {
                    needCheckSight = false;
                    sightRadius = 1;
                    this.playerInfo.AddPoint(point,
                        ref needCheckSight, ref sightRadius);
                    if (callback != null) {
                        callback.Invoke(point, needCheckSight, sightRadius);
                    }
                }
            }
        }

        public void RefreshAllianceInfo(AllianceInfosNtf ntf) {
            this.playerInfo.RefreshAllianceInfo(ntf);
        }


        public delegate void MonsterRefreshDelegate(Vector2 coordinate);
        public void RefreshMonsterInfo(FeedBlockMonsterNtf monsterBlockNtf,
                                       MonsterRefreshDelegate callback = null) {
            //Debug.LogError(monsterBlockNtf.MonsterBlocks.Count);
            foreach (MonsterBlock block in monsterBlockNtf.MonsterBlocks) {
                //Debug.LogError(block.Monsters.Count);
                foreach (Monster monster in block.Monsters) {
                    this.playerInfo.RefreshMonsterInfo(monster);
                    if (callback != null) {
                        callback.Invoke(monster.Coord);
                    }
                }
            }

        }

        public Dictionary<Vector2, Dictionary<Vector2, Monster>> GetMonsterPoint() {
            return this.playerInfo.GetMonsterPoint();
        }

        public Monster GetMonsterInfo(Vector2 coordinate) {
            return this.playerInfo.GetMonsterInfo(coordinate);
        }

        public void RemoveMonsterInfo(Vector2 coordinate) {
            this.playerInfo.RemoveMonsterInfo(coordinate);
        }

        public Boss GetBossInfo(Vector2 coordinate) {
            return this.playerInfo.GetBossInfo(coordinate);
        }

        public void InitMap() {
            this.maxCoordinate = this.resourceInfo.maxCoordinate;
            this.centerCoordinate = RoleManager.GetRoleCoordinate();
            this.originCoordinate = 0.5f * this.resourceInfo.maxCoordinate;
            this.resourceInfo.Init(this.centerCoordinate);
            this.resourceInfo.Refresh();
            this.playerInfo.Init(this.centerCoordinate);
        }

        public Vector2 GetPlayerBlock(Vector2 coordinate) {
            return this.playerInfo.GetBlock(coordinate);
        }

        public FeedBlocksReq FeedBlocks() {
            List<Vector2> blockList = this.playerInfo.GetBlockList(this.playerInfo.block);
            FeedBlocksReq feedBlocks = new FeedBlocksReq();

            foreach (Vector2 block in blockList) {
                feedBlocks.Offsets.Add(block.x + "," + block.y);
            }
            return feedBlocks;
        }

        public int GetOtherTroopCount(Vector2 coordinate, ref int avatarId, ref TroopRelation relation) {
            return this.playerInfo.GetOtherTroopCount(coordinate, ref avatarId, ref relation);
        }

        public string GetTileZone(Vector2 coordinate) {
            return this.resourceInfo.GetTileZone(coordinate);
        }

        public string GetLayerCamp(Vector2 coordinate) {
            return this.playerInfo.GetLayerCamp(coordinate);
        }

        public string GetLayerAbove(Vector2 coordinate,
            ref MapTileRelation relation, ref bool isVisible, ref bool isInSight) {
            string above = this.playerInfo.GetLayerAbove(
                coordinate, ref relation, ref isVisible, ref isInSight
            );
            if (above.CustomIsEmpty()) {
                above = this.resourceInfo.GetLayerAbove(coordinate);
            }
            return above;
        }

        //public string GetLayerLevel(Vector2 coordinate) {
        //    string level = this.playerInfo.GetLayerLevel(coordinate);
        //    if (level == null) {
        //        level = this.GetResourcesInfoLevel(coordinate);
        //    }
        //    return level;
        //}

        public string GetResourcesInfoLevel(Vector2 coordinate) {
            return this.resourceInfo.GetLayerLevel(coordinate);
        }

        public string GetLayerAboveLevel(Vector2 coordinate) {
            return this.resourceInfo.GetLayerAbove(coordinate,true);
        }

        public string GetLayerBase(Vector2 coordinate) {
            return this.resourceInfo.GetLayerBase(coordinate);
        }

        public string GetLayerBelow(Vector2 coordinate) {
            return this.resourceInfo.GetLayerBelow(coordinate);
        }

        public string GetLayerCityBase(Vector2 coordinate, ref Vector2 offset) {
            return this.resourceInfo.GetLayerCityBase(coordinate, ref offset);
        }

        public Vector2 GetCityCenterCoord(Vector2 coordinate) {
            return this.resourceInfo.GetCityCenterCoord(coordinate);
        }

        public ElementType GetTileType(Vector2 coordinate) {
            ElementType type = this.playerInfo.GetTileType(coordinate);
            if (type == ElementType.none) {
                type = this.resourceInfo.GetTileType(coordinate);
            }
            return type;
        }

        public bool GetTileUpgradeable(Vector2 coordinate) {
            return this.playerInfo.GetBuildingUpgradeable(coordinate);
        }

        public TileProtectType GetTileProtectType(Vector2 coordinate) {
            return this.playerInfo.GetTileProtectType(coordinate);
        }

        public long GetTileProtectTime(Vector2 coordinate, out TileProtectType type) {
            return this.playerInfo.GetTileProtectTime(coordinate, out type);
        }

        public string GetBorderType(Vector2 coordinate) {
            return this.resourceInfo.GetBorderType(coordinate);
        }

        public MapTileRelation GetRelation(Vector2 coordinate) {
            return this.playerInfo.GetRelation(coordinate);
        }

        public delegate void BossRefreshDelegate(Vector2 coordinate);
        public void DominationInfo(FeedBlockDominationNtf feedBlockNtf,
                                      BossRefreshDelegate callback = null) {
            foreach (BossBlock block in feedBlockNtf.BossBlocks) {
                foreach (Boss boss in block.Bosses) {
                    this.playerInfo.DominationBossInfo(boss);
                    if (callback != null) {
                        callback.Invoke(boss.Coord);
                    }
                }

            }
        }

        public List<Vector2> GetAlliancePoint(string allianceId) {
            return this.playerInfo.GetAlliancePoint(allianceId);
        }

        public bool IsAvaliable(Vector2 coordinate, string buildingId) {
            return this.playerInfo.IsAvaliable(coordinate, buildingId);
        }

        public void CaculatePlayerBlock() {
            this.playerInfo.CaculateBlockList();
        }

        public MapTileInfo GetTileInfoSeparate(Vector2 coordinate) {
            MapTileInfo tileInfo = new MapTileInfo();
            tileInfo.coordinate = coordinate;
            if (!this.CheckCoordinate(coordinate)) {
                return null;
            }
            if (this.playerInfo.GetTileInfo(tileInfo)) {
                return tileInfo;
            } else if (this.resourceInfo.GetTileInfo(tileInfo)) {
                return tileInfo;
            } else {
                return null;
            }
        }

        public MapTileInfo GetTileInfo(Vector2 coordinate) {
            //Debug.LogError("GetTileInfo " + coordinate);
            if (!this.CheckCoordinate(coordinate)) {
                return null;
            }
            this.ResetCurrentTileInfo(coordinate);
            // to do : when get npc city tile info, both playerinfo and resourceInfo contain it.
            if (this.playerInfo.GetTileInfo(this.currentTile)) {
                return this.currentTile;
            } else if (this.resourceInfo.GetTileInfo(this.currentTile)) {
                return this.currentTile;
            } else {
                return null;
            }
        }

        private void ResetCurrentTileInfo(Vector2 coordinate) {
            this.currentTile.playerName = string.Empty;
            this.currentTile.playerId = string.Empty;
            this.currentTile.allianceName = string.Empty;
            this.currentTile.allianceId = string.Empty;
            this.currentTile.roleAvatar = string.Empty;
            this.currentTile.name = string.Empty;
            this.currentTile.level = 0;
            this.currentTile.coordinate = coordinate;
            this.currentTile.relation = MapTileRelation.none;
            this.currentTile.type = string.Empty;
            this.currentTile.defender = 0;
            this.currentTile.allianceIcon = 0;
            this.currentTile.buildingInfo = null;
            this.currentTile.camp = null;
            this.currentTile.city = null;
            this.currentTile.pass = null;
            this.currentTile.endurance = int.MaxValue;
            this.currentTile.isProtected = false;
            this.currentTile.tileProtectType = TileProtectType.None;
            this.currentTile.defenderRecoverTimeAt = 0;
            this.currentTile.isFallen = false;
            this.currentTile.maxEndurance = 0;
            this.currentTile.troopCount = 0;
        }

        public int GetLevel(Vector2 coordinate) {
            int level = this.playerInfo.GetLevel(coordinate);
            if (level == -1) {
                return this.resourceInfo.GetLevel(coordinate);
            } else {
                return level;
            }
        }

        public int GetTileLevel(Vector2 coordinate) {
            return this.resourceInfo.GetLevel(coordinate);
        }

        public List<Vector2> GetResourceBlockList(Vector2 center) {
            return this.resourceInfo.GetBlockList(center);
        }

        public List<Vector2> GetPlayerBlockList(Vector2 center) {
            return this.playerInfo.GetBlockList(center);
        }

        public void RefreshPoint(Point point) {
            this.playerInfo.RefreshPoint((Point)point);
        }

        public bool CheckCoordinate(Vector2 coordinate) {
            if (coordinate.x < this.minCoordinate.x || coordinate.x > this.maxCoordinate.x
                || coordinate.y < this.minCoordinate.y || coordinate.y > this.maxCoordinate.y) {
                return false;
            }
            return true;
        }

        public bool CheckPlayerBlock(Vector2 block) {
            Vector2 minBlock = Vector2.zero;
            Vector2 maxBlock = new Vector2(
                Mathf.Floor(this.maxCoordinate.x / this.playerInfo.blockSize.x),
                Mathf.Floor(this.maxCoordinate.y / this.playerInfo.blockSize.y)
            );

            if (block.x < minBlock.x || block.x > maxBlock.x ||
                block.y < minBlock.y || block.y > maxBlock.y) {
                return false;
            }
            return true;
        }

        public List<Vector2> GetCoordinateList(Vector2 center, int radius, bool needCenter = true) {
            return this.resourceInfo.GetCoordinateList(center, radius, needCenter);
        }

        public Point GetPlayerPoint(Vector2 coordinate) {
            return this.playerInfo.GetPoint(coordinate);
        }

        public Vector2 GetReachableTile(int level) {
            foreach (Vector2 center in RoleManager.GetPointDict().Keys) {
                foreach (Vector2 coord in this.GetCoordinateList(center, 1)) {
                    int type = (int)this.resourceInfo.GetTileType(coord);
                    if (this.GetRelation(coord) == MapTileRelation.none &&
                        (type <= (int)ElementType.crystal && type >= (int)ElementType.road) &&
                        (this.resourceInfo.GetLevel(coord) == level)) {
                        return coord;
                    }
                }
            }
            return Vector2.zero;
        }

        public bool IsInPlayerBlock(Vector2 coordinate) {
            return this.playerInfo.IsInBlock(coordinate);
        }

        public bool IsMarchInBlocks(Vector2 origin, Vector2 target) {
            return this.playerInfo.IsMarchInBlocks(origin, target);
        }

        public bool IsDurabilityMax(Vector2 coordinate, ref long duration) {
            return this.playerInfo.IsDurabilityMax(coordinate, ref duration);
        }

        public bool IsDefenderRecover(Vector2 coordinate, ref long duration) {
            return this.playerInfo.IsDefenderRecover(coordinate, ref duration);
        }

        public bool IsCoordinateInBlocks(Vector2 coordinate) {
            return this.resourceInfo.IsCoordinateInBlocks(coordinate);
        }

        public Vector2 GetTileArmy(Vector2 coordinate) {
            int level = this.resourceInfo.GetLevel(coordinate);
            TileEnemyConf enemyConf = TileEnemyConf.GetConf(level.ToString());
            return new Vector2(enemyConf.minArmyAmount, enemyConf.maxArmyAmount);
        }

        public ElementType GetPointOriginalElementType(Vector2 coordinate) {
            return this.resourceInfo.GetTileType(coordinate);
        }

        public void RemoveBlockDict(Dictionary<uint, uint> dict) {
            this.resourceInfo.RemoveBlockDict(dict);
        }
    }
}
