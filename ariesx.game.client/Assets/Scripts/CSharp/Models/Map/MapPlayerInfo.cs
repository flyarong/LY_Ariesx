using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;
using System.Text;

namespace Poukoute {

    public enum TileProtectType {
        FreshProtect,
        AvoidExpireAt,
        None
    }

    public class MapPlayerInfo : MapInfo {
        public Dictionary<Vector2, Dictionary<Vector2, Point>> infoDict =
            new Dictionary<Vector2, Dictionary<Vector2, Point>>();
        public Dictionary<string, AllianceBasicInfo> allianceInfoDict =
            new Dictionary<string, AllianceBasicInfo>();
        public int maxBlockAmount = 9;
        public Point firstPoint;
        public Point lastPoint;
        // Info block => points.
        public List<Vector2> blockList = new List<Vector2>();

        public Dictionary<float, Vector2> coordBorder = new Dictionary<float, Vector2>();

        public Dictionary<Vector2, Dictionary<Vector2, int>> sightDict =
            new Dictionary<Vector2, Dictionary<Vector2, int>>();
        public int sightPointCount = 0;
        private const int ENDURANCE_INCREASE_DURATION = 1800;

        public MapPlayerInfo() {
            this.block = Vector2.zero;
            this.blockSize = new Vector2(8, 8);
            this.blockRect = new Rect(Vector2.zero, new Vector2(10, 10));
            this.maxBlock = new Vector2(
                Mathf.Ceil(this.maxCoordinate.x / this.blockSize.x) - 1,
                Mathf.Ceil(this.maxCoordinate.y / this.blockSize.y) - 1
            );
        }

        public override void Init(Vector2 coordinate) {
            base.Init(coordinate);
            this.CaculateBlockList();
        }

        #region Monster logic
        private Dictionary<Vector2, Dictionary<Vector2, Monster>> monsterDict =
            new Dictionary<Vector2, Dictionary<Vector2, Monster>>();

        public Dictionary<Vector2, Dictionary<Vector2, Monster>> GetMonsterPoint() {
            return this.monsterDict;
        }

        public void RefreshMonsterInfo(Monster monster) {
            if (monster.Level > 0) {
                this.AddMonster(monster);
            } else if (this.GetMonsterInfo(monster.Coord) != null) {
                //Debug.LogError(monster.Level);
                this.RemoveMonster(monster.Coord);
            }
        }

        private void AddMonster(Monster monster) {
            Vector2 coordinate = monster.Coord;
            Vector2 block = this.GetBlock(coordinate);
            if (!this.monsterDict.ContainsKey(block)) {
                this.monsterDict[block] = new Dictionary<Vector2, Monster>(5);
            }
            this.monsterDict[block][coordinate] = monster;
        }

        private void RemoveMonster(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.monsterDict.ContainsKey(block)) {
                return;
            }
            if (this.monsterDict[block].ContainsKey(coordinate)) {
                this.monsterDict[block].Remove(coordinate);
            }
        }

        public Monster GetMonsterInfo(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (this.monsterDict.ContainsKey(block)
                && this.monsterDict[block].ContainsKey(coordinate)) {
                return this.monsterDict[block][coordinate];
            }
            return null;
        }

        public void RemoveMonsterInfo(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (this.monsterDict.ContainsKey(block)) {
                this.monsterDict[block].TryRemove(coordinate);
            }
        }
        #endregion

        #region Boss logic
        public Dictionary<Vector2, Dictionary<Vector2, Boss>> bossDict =
           new Dictionary<Vector2, Dictionary<Vector2, Boss>>();

        public void DominationBossInfo(Boss boss) {
            //Vector2 coordinate = boss.Coord;
            //Vector2 block = this.GetBlock(coordinate);
            if (boss.Level > 0) {
                this.AddBoss(boss);
            } else if (this.GetBossInfo(boss.Coord) != null) {
                this.RemoveBossDict(boss.Coord);
            }
        }

        /// <summary>
        /// Boss坐标不为空,添加坐标
        /// </summary>
        /// <param name="boss"></param>
        private void AddBoss(Boss boss) {
            Vector2 coordinate = boss.Coord;
            Vector2 block = this.GetBlock(coordinate);
            if (!this.bossDict.ContainsKey(block)) {
                this.bossDict[block] = new Dictionary<Vector2, Boss>();
            }
            this.bossDict[block][coordinate] = boss;
        }

        private void RemoveBossDict(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.bossDict.ContainsKey(block)) {
                return;
            }
            if (this.bossDict[block].ContainsKey(coordinate)) {
                // Debug.LogError("删除字典里面的坐标");
                this.bossDict[block].Remove(coordinate);
            }
        }

        public Boss GetBossInfo(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (this.bossDict.ContainsKey(block)
                && this.bossDict[block].ContainsKey(coordinate)) {
                return this.bossDict[block][coordinate];
            }
            return null;
        }

        #endregion


        public void AddPoint(Point point,
            ref bool needCheckSight, ref int sightRadius) {
            Vector2 coordinate = point.Coord;
            Vector2 block = this.GetBlock(coordinate);
            if (!this.blockList.Contains(block)) {
                return;
            }
            Dictionary<Vector2, int> sightPointDict = this.sightDict[block];
            string allianceId = RoleManager.GetAllianceId();
            if (point.PlayerId == RoleManager.GetRoleId() ||
                (!string.IsNullOrEmpty(point.AllianceId) && point.AllianceId == allianceId) ||
                (!string.IsNullOrEmpty(point.BelongsAllianceId) && point.BelongsAllianceId == allianceId)) {
                bool isTownhall = point.Building != null &&
                    ((point.Building.Name.Contains(ElementName.stronghold) &&
                    point.Building.Level > 0));
                bool isStronghold = point.Building != null &&
                    point.Building.Name.CustomEquals(ElementName.townhall);
                if (isTownhall) {
                    sightRadius = 5;
                } else if (isStronghold) {
                    sightRadius = 3;
                }
                if (!sightPointDict.ContainsKey(coordinate)) {
                    sightPointDict.Add(coordinate, sightRadius);
                    this.sightPointCount++;
                    needCheckSight = true;
                } else {
                    if (sightPointDict[coordinate] != sightRadius) {
                        needCheckSight = true;
                    }
                    sightPointDict[coordinate] = sightRadius;
                }
            } else {
                if (sightPointDict.ContainsKey(coordinate)) {
                    sightPointDict.Remove(coordinate);
                    this.sightPointCount--;
                    needCheckSight = true;
                }
            }
            this.infoDict[block][coordinate] = point;
            EventManager.AddShieldEvent(point);
        }

        public Point GetPoint(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return null;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return null;
            }
            return this.infoDict[block][coordinate];
        }

        public void AddBlock(string blockStr) {
            //  Debug.LogError(blockStr);
            string[] blockStrArray = blockStr.CustomSplit(',');
            Vector2 block = new Vector2(
                int.Parse(blockStrArray[0]),
                int.Parse(blockStrArray[1])
            );
            if (!this.infoDict.ContainsKey(block)) {
                this.infoDict[block] = new Dictionary<Vector2, Point>();
                this.sightDict[block] = new Dictionary<Vector2, int>();
            }
        }

        public void RefreshAllianceInfo(AllianceInfosNtf ntf) {
            foreach (AllianceBasicInfo info in ntf.Infos) {
                this.allianceInfoDict[info.Id] = info;
            }
        }

        public void RemovePoint(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            Dictionary<Vector2, Point> blockDict;
            if (this.infoDict.TryGetValue(block, out blockDict)) {
                if (blockDict.ContainsKey(coordinate)) {
                    blockDict.Remove(coordinate);
                }
                if (blockDict.Count == 0) {
                    this.infoDict.Remove(block);
                }
            }
        }

        public int GetLevel(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return -1;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return -1;
            }
            Point point = this.infoDict[block][coordinate];
            MapBasicTypeConf elementTypeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());
            if (elementTypeConf.category.CustomEquals(ElementCategory.building) &&
                (point.isVisible || point.isSightBuilding)) {
                return point.Building.Level;
            } else {
                return -1;
            }
        }

        public int GetOtherTroopCount(Vector2 coordinate, ref int avatar, ref TroopRelation relation) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return 0;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return 0;
            }
            Point point = this.infoDict[block][coordinate];
            int count = 0;
            int index = 0;
            foreach (Point.Troop troop in point.Troops) {
                if (troop.AllianceId == RoleManager.GetAllianceId() &&
                    troop.PlayerId == RoleManager.GetRoleId()) {
                    continue;
                } else {
                    index = point.Troops.IndexOf(troop);
                    count++;
                }
            }
            if (count > 0) {
                Point.Troop troop = point.Troops[index];
                avatar = troop.PlayerAvatar;
                if (troop.AllianceId == RoleManager.GetMasterAllianceId() ||
                    troop.AllianceId.CustomIsEmpty() ||
                    troop.AllianceId != RoleManager.GetAllianceId()) {
                    relation = TroopRelation.Enemy;
                } else {
                    relation = TroopRelation.Ally;
                }
            }
            return count;
        }

        public string GetLayerCamp(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return string.Empty;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return string.Empty;
            }
            Point point = this.infoDict[block][coordinate];
            MapBasicTypeConf elementTypeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());
            if (elementTypeConf.category.CustomEquals("camp")) {
                if (point == null || point.Camp == null) {
                    Debug.LogError("Camp");
                    return string.Empty;
                }
                if (point.Camp.Visible) {
                    Debug.LogError("Yes Camp!");
                    return "1";
                } else {
                    return string.Empty;
                }
            } else {
                return string.Empty;
            }
        }

        public string GetLayerAbove(Vector2 coordinate,
            ref MapTileRelation relation, ref bool isVisible, ref bool isInSight) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return string.Empty;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return string.Empty;
            }
            Point point = this.infoDict[block][coordinate];
            isVisible = this.CheckSight(coordinate, point);
            MapBasicTypeConf elementTypeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());
            string level = string.Empty;
            if (elementTypeConf.category.CustomEquals(ElementCategory.building) && isVisible) {
                if (!point.IsSelf && !point.IsAlly) {
                    relation = MapTileRelation.enemy;
                }
                if (EventManager.IsTileGiveUpBuilding(coordinate) ||
                    point.Building.IsUpgrade) {
                    return ElementCategory.building;
                } else {
                    level = point.Building.Level.ToString();
                    return elementTypeConf.type + level;
                }
            } else {
                return string.Empty;
            }
        }

        // To do: need delete.
        public string GetLayerLevel(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return null;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return null;
            }
            Point point = this.infoDict[block][coordinate];
            MapBasicTypeConf elementTypeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());
            if (elementTypeConf.category.CustomEquals(ElementCategory.building) &&
                (point.isVisible || point.isSightBuilding)) {
                // int level = Mathf.Max(1, Mathf.FloorToInt(point.Building.Level / 2));
                return string.Empty;//level.ToString();
            } else {
                return null;
            }
        }

        public bool GetBuildingUpgradeable(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return false;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return false;
            }

            Point point = this.infoDict[block][coordinate];
            MapBasicTypeConf elementTypeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());

            if (elementTypeConf.category.CustomEquals(ElementCategory.building)) {
                if (EventManager.IsTileInBuildEvent(coordinate) ||
                    EventManager.IsTileGiveUpBuilding(coordinate)) {
                    return false;
                } else {
                    if (this.GetRelation(coordinate) == MapTileRelation.self) {
                        BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
                        int level = point.Building.Level == 0 ? 1 : point.Building.Level;
                        bool canBuildUpgrade = buildModel.GetBuildingUpgradeable(
                            point.Building.Name,
                            level + 1
                        );
                        return canBuildUpgrade;
                    } else {
                        return false;
                    }
                }
            }
            return false;
        }

        public List<Vector2> GetAlliancePoint(string allianceId) {
            List<Vector2> pointList = new List<Vector2>();
            foreach (Dictionary<Vector2, Point> block in this.infoDict.Values) {
                foreach (var point in block) {
                    if (point.Value.AllianceId == allianceId) {
                        pointList.Add(point.Key);
                    }
                }
            }
            return pointList;
        }

        public ElementType GetTileType(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return ElementType.none;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return ElementType.none;
            }
            Point point = this.infoDict[block][coordinate];
            return (ElementType)point.ElementType;
        }

        public TileProtectType GetTileProtectType(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return TileProtectType.None;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return TileProtectType.None;
            }

            Point point = this.infoDict[block][coordinate];
            long currentUtcTime = RoleManager.GetCurrentUtcTime();
            long leftDuration = point.AvoidExpireAt * 1000 - currentUtcTime;
            if (leftDuration > 0) {
                return TileProtectType.AvoidExpireAt;
            }

            leftDuration = point.FreshProtectionExpireAt * 1000 - currentUtcTime;
            if (leftDuration > 0) {
                return TileProtectType.FreshProtect;
            }

            return TileProtectType.None;
        }

        public long GetTileProtectTime(Vector2 coordinate, out TileProtectType type) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                type = TileProtectType.None;
                return -1;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                type = TileProtectType.None;
                return -1;
            }

            Point point = this.infoDict[block][coordinate];
            long currentTime = RoleManager.GetCurrentUtcTime();
            long leftDuration = point.AvoidExpireAt * 1000 - currentTime;
            if (leftDuration > 0) {
                type = TileProtectType.AvoidExpireAt;
                return leftDuration;
            }
            leftDuration = point.FreshProtectionExpireAt * 1000 - currentTime;
            if (leftDuration > 0) {
                type = TileProtectType.FreshProtect;
                return leftDuration;
            }

            type = TileProtectType.None;
            return -1;
        }

        public MapTileRelation GetRelation(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            //if (this.bossDict.ContainsKey(block)
            //    && this.bossDict[block].ContainsKey(coordinate)) {
            //    return MapTileRelation.domination;
            //}

            if (!this.infoDict.ContainsKey(block)) {
                return MapTileRelation.none;
            }

            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return MapTileRelation.none;
            }

            Point point = this.infoDict[block][coordinate];
            if (string.IsNullOrEmpty(point.PlayerId)) {
                if (!string.IsNullOrEmpty(point.AllianceId)) {
                    if (point.IsAlly) {
                        return MapTileRelation.ally;
                    } else if (point.IsMaster) {
                        return MapTileRelation.master;
                    } else if (point.IsSlave) {
                        return MapTileRelation.slave;
                    } else {
                        return MapTileRelation.enemy;
                    }
                } else {
                    return MapTileRelation.none;
                }
            } else {
                if (point.PlayerId == RoleManager.GetRoleId()) {
                    return MapTileRelation.self;
                }
                if (point.IsMaster) {
                    return MapTileRelation.master;
                }
                bool isPointBeFalling = !string.IsNullOrEmpty(point.BelongsAllianceName);
                if (isPointBeFalling) {
                    if (point.IsSlave) {
                        return MapTileRelation.slave;
                    } else {
                        return MapTileRelation.fallen;
                    }
                } else {
                    if (point.IsAlly) {
                        return MapTileRelation.ally;
                    } else {
                        return MapTileRelation.enemy;
                    }
                }
            }

        }

        // To do : When build building, Poin building not refresh
        public bool IsAvaliable(Vector2 coordinate, string buildingId) {
            if (EventManager.IsTileAbandon(coordinate)) {
                return false;
            }
            Point point = RoleManager.GetRolePoint(coordinate);
            if (point == null || string.IsNullOrEmpty(point.PlayerId)) {
                return false;
            }
            BuildingConf buildingConf = ConfigureManager.GetConfById<BuildingConf>(buildingId);

            if (point.PlayerId == RoleManager.GetRoleId() && point.Building == null &&
                point.ElementType >= 3 && point.ElementType <= 9 &&
                point.Resource.Level >= buildingConf.fieldLevel) {
                return true;
            } else {
                return false;
            }
        }

        public bool GetTileInfo(MapTileInfo tileInfo) {
            Vector2 block = this.GetBlock(tileInfo.coordinate);
            tileInfo.relation = MapTileRelation.neutral;
            if (!this.infoDict.ContainsKey(block)) {
                return false;
            }
            if (!this.infoDict[block].ContainsKey(tileInfo.coordinate)) {
                return false;
            }
            bool result = true;
            StringBuilder s = new StringBuilder();
            Point point = this.infoDict[block][tileInfo.coordinate];
            MapBasicTypeConf typeConf =
                MapBasicTypeConf.GetConf(point.ElementType.ToString());
            if (!point.AllianceId.CustomIsEmpty()) {
                AllianceBasicInfo allianceBaseInfo;
                if (this.allianceInfoDict.TryGetValue(point.AllianceId, out allianceBaseInfo)) {
                    tileInfo.allianceIcon = allianceBaseInfo.Emblem;
                    tileInfo.allianceName = allianceBaseInfo.Name;
                }
            }
            tileInfo.isVisible = point.isVisible;
            tileInfo.troopCount = 0;
            if (point.isVisible) {
                foreach (Point.Troop troop in point.Troops) {
                    if (troop.PlayerId != RoleManager.GetRoleId()) {
                        tileInfo.troopCount++;
                    }
                }
            }
            switch (typeConf.category) {
                case "building":
                    if (point.isVisible || point.isSightBuilding) {
                        BuildingConf buildingConf =
                        ConfigureManager.GetConfById<BuildingConf>(point.Building.Name + "_1");
                        tileInfo.name = buildingConf.type;
                        tileInfo.buildingInfo = point.Building;
                        tileInfo.type = ElementCategory.building;
                        tileInfo.level = point.Building.Level;
                    } else {
                        result = false;
                    }
                    break;
                case "camp":
                    if (point.Camp.RemainTimes > 0) {
                        tileInfo.name = typeConf.type;
                        tileInfo.camp = point.Camp;
                        tileInfo.level = point.Camp.Level;
                        tileInfo.defender = point.Camp.Level;
                        tileInfo.type = ElementCategory.camp;
                    } else {
                        tileInfo.type =
                        tileInfo.name = ElementCategory.forest;
                    }
                    break;
                case "pass":
                    tileInfo.name =
                    tileInfo.type = ElementCategory.pass;
                    tileInfo.defender = point.Pass.Level;
                    tileInfo.level = point.Pass.Level;
                    s.AppendFormat("{0},{1},{2},{3}",
                        point.MapSN, point.ZoneSN, (int)tileInfo.coordinate.x, (int)tileInfo.coordinate.y);
                    tileInfo.pass = MiniMapPassConf.GetConf(s.ToString());
                    break;
                case "npc_city":
                    s.AppendFormat("{0},{1},{2},{3}",
                        point.MapSN, point.ZoneSN, point.NpcCity.SN, (point.NpcCity.IsCenter ? 1 : 0));
                    NPCCityConf cityConf = NPCCityConf.GetConf(s.ToString());
                    tileInfo.city = cityConf;
                    tileInfo.name = cityConf.name;
                    tileInfo.type = ElementCategory.npc_city;
                    tileInfo.level = cityConf.level;
                    break;
                default:
                    result = false;
                    break;
            }

            tileInfo.relation = this.GetRelation(tileInfo.coordinate);
            tileInfo.isFallen = !string.IsNullOrEmpty(point.BelongsAllianceName);
            tileInfo.playerName = point.PlayerName;
            tileInfo.playerId = point.PlayerId;
            tileInfo.allianceId = point.AllianceId;
            tileInfo.maxEndurance = point.MaxDurability;//this.GetMaxDurance(tileInfo);

            if (tileInfo.playerId.CustomIsEmpty() &&
            tileInfo.allianceId.CustomIsEmpty() &&
            typeConf.category.CustomEquals("resource")) {
                tileInfo.endurance = GameConst.MAX_ENDURANCE;
            } else {
                tileInfo.endurance = this.GetCurrentEndurance(
                    point.DurabilityRefreshAt,
                    point.Durability,
                    tileInfo.maxEndurance
                );
            }
            tileInfo.tileProtectType = this.GetTileProtectType(point.Coord);
            tileInfo.isProtected = (tileInfo.tileProtectType != TileProtectType.None);
            tileInfo.defenderRecoverTimeAt = point.NpcTroopRefreshAt;
            return result;
        }

        private int GetCurrentEndurance(long enduranceRefreshAt, int endurance, int maxEndurance) {
            long currentTime = RoleManager.GetCurrentUtcTime() / 1000;
            long increaseCount = (currentTime - enduranceRefreshAt) / ENDURANCE_INCREASE_DURATION;
            increaseCount = increaseCount > 2 ? 2 : increaseCount;
            int newEnduranceCount = endurance + (int)increaseCount * (maxEndurance / 2);
            return newEnduranceCount < maxEndurance ? newEnduranceCount : maxEndurance;
        }

        private int GetMaxDurance(MapTileInfo tileInfo) {
            int maxDurance = GameConst.MAX_ENDURANCE;
            if (tileInfo.type.CustomEquals(ElementCategory.building)) {
                int level = Mathf.Max(1, tileInfo.buildingInfo.Level);
                maxDurance = BuildingConf.GetDurability(
                    string.Concat(tileInfo.buildingInfo.Name, "_", level)
                );
            } else if (tileInfo.type.CustomEquals(ElementCategory.npc_city)) {
                return tileInfo.city.durability;
            } else if (tileInfo.type.CustomEquals(ElementCategory.pass)) {
                return PassConf.GetPassDurability(tileInfo.pass.level, tileInfo.GetPassType());
            }

            return maxDurance;
        }

        public bool IsDurabilityMax(Vector2 coordinate, ref long duration) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return true;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return true;
            }
            Point point = this.infoDict[block][coordinate];
            if (!point.isVisible && !point.isSightBuilding) {
                return true;
            }
            int maxDuration = point.GetMaxDuration();
            int curDurability = this.GetCurrentEndurance(
                point.DurabilityRefreshAt,
                point.Durability,
                maxDuration
            );
            int leftDurance = (maxDuration - point.Durability);
            duration = (long)Mathf.FloorToInt(leftDurance / (float)(maxDuration / 2))
                * 1800000;
            return curDurability == maxDuration;
        }

        public bool IsDefenderRecover(Vector2 coordinate, ref long duration) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return true;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return true;
            }
            Point point = this.infoDict[block][coordinate];
            if (!point.isVisible && !point.isSightBuilding) {
                return true;
            }
            duration = point.NpcTroopRefreshAt * 1000 - RoleManager.GetCurrentUtcTime();
            return duration < 0;
        }

        private bool CheckSight(Vector2 center, Point point) {
            bool visible = false;
            point.isCaculate = true;
            Vector2 block = this.GetBlock(center);
            if (this.sightDict.ContainsKey(block) &&
                this.sightDict[block].ContainsKey(center)) {
                point.isVisible = true;
                return true;
            }
            if (string.IsNullOrEmpty(point.PlayerId) && (string.IsNullOrEmpty(point.AllianceId))) {
                point.isVisible = true;
                return true;
            }
            if (point.Building != null &&
                (point.Building.Name.CustomEquals(ElementName.townhall) ||
                (point.Building.Name.Contains(ElementName.stronghold) &&
                point.Building.Level > 0))) {
                point.isSightBuilding = true;
            }

            if (this.sightPointCount <= 100) {
                visible = this.CheckSightPoint(center);
            } else {
                visible = this.CheckPoinInSight(center, 1) ||
                          this.CheckPoinInSight(center, 5) ||
                          this.CheckPoinInSight(center, 3);
            }
            point.isVisible = visible;
            return point.isSightBuilding || point.isVisible;
        }

        private bool CheckSightPoint(Vector2 coordinate) {
            foreach (Dictionary<Vector2, int> pointDict in this.sightDict.Values) {
                foreach (var pair in pointDict) {
                    if ((coordinate.x - pair.Key.x).Abs() <= pair.Value &&
                        (coordinate.y - pair.Key.y).Abs() <= pair.Value) {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckPoinInSight(Vector2 center, int sight) {
            bool isTownhall = false;
            bool isStronghold = false;
            bool isTile = false;
            foreach (Vector2 coord in this.GetCoordinateList(center, sight, false)) {
                Vector2 block = this.GetBlock(coord);
                Dictionary<Vector2, Point> pointDict;
                if (this.infoDict.TryGetValue(block, out pointDict)) {
                    Point point;
                    if (pointDict.TryGetValue(coord, out point)) {
                        if (sight == 5) {
                            isTownhall = point.Building != null &&
                               point.Building.Name.CustomEquals(ElementName.townhall);
                        } else if (sight == 3) {
                            isStronghold = point.Building != null &&
                                point.Building.Name.CustomEquals(ElementName.stronghold);
                        } else {
                            isTile = true;
                        }

                        if ((isTownhall || isStronghold || isTile) &&
                           (point.PlayerId == RoleManager.GetRoleId() ||
                           point.IsSlave || point.IsAlly)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckBlock(Vector2 block) {
            if (block.x > this.maxBlock.x || block.x < this.minBlock.x ||
                block.y > this.maxBlock.y || block.y < this.minBlock.y) {
                return false;
            }
            return true;
        }

        public void CaculateBlockList() {
            this.blockList = this.GetBlockList(this.block);
            List<Vector2> removeList = new List<Vector2>();
            foreach (Vector2 block in this.infoDict.Keys) {
                if (!this.blockList.Contains(block)) {
                    removeList.Add(block);
                }
            }
            foreach (Vector2 block in removeList) {
                this.infoDict.Remove(block);
                this.monsterDict.Remove(block);
                this.bossDict.Remove(block);
                this.sightPointCount -= sightDict[block].Count;
                sightDict.Remove(block);
                this.coordBorder.Clear();
            }
        }

        public List<Vector2> GetBlockList(Vector2 center) {
            List<Vector2> blockList = new List<Vector2>();
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    Vector2 block = center + new Vector2(i, j);
                    if (this.CheckBlock(block)) {
                        blockList.Add(block);
                    }
                }
            }
            return blockList;
        }

        public void RefreshPoint(Point point) {
            Vector2 coordinate = new Vector2(point.Coord.X, point.Coord.Y);
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return;
            }
            if (!this.infoDict[block].ContainsKey(coordinate)) {
                return;
            }
            this.infoDict[block][coordinate] = point;
        }

        private Vector2 CoordinateToBlock(Vector2 coordinate) {
            return new Vector2(
                Mathf.Floor((coordinate.x - 1) / this.blockSize.x),
                Mathf.Floor((coordinate.y - 1) / this.blockSize.y)
            );
        }

        public bool IsInBlock(Vector2 coordinate) {
            Vector2 block = this.CoordinateToBlock(coordinate);
            return this.infoDict.ContainsKey(block);
        }

        public bool IsMarchInBlocks(Vector2 origin, Vector2 target) {

            float borderLeft = (this.block.x - 1) * 8;
            float borderRight = (this.block.x + 2) * 8;

            if ((origin.x >= borderLeft && origin.x <= borderRight) ||
                (target.x >= borderLeft && target.x <= borderRight)) {
                return true;
            }

            if (this.coordBorder.Count == 0) {
                this.coordBorder.Add(
                    borderLeft,
                    new Vector2(this.block.y - 1, this.block.y + 2) * 8
                );
                this.coordBorder.Add(
                    borderRight,
                    new Vector2(this.block.y - 1, this.block.y + 2) * 8
                );
                this.coordBorder.Add(
                    (this.block.y - 1) * 8 + 10000,
                    new Vector2(this.block.x - 1, this.block.x + 2) * 8
                );
                this.coordBorder.Add(
                    (this.block.y + 2) * 8 + 10000,
                    new Vector2(this.block.x - 1, this.block.x + 2) * 8
                );
            }

            float param1 = (target.y - origin.y) / (float)(target.x - origin.x);
            float param2 = (origin.y * target.x - origin.x * target.y) / (float)(target.x - origin.x);
            Vector2 intervalX = new Vector2(
                Mathf.Min(origin.x, target.x),
                Mathf.Max(origin.x, target.x)
            );
            Vector2 intervalY = new Vector2(
                Mathf.Min(origin.y, target.y),
                Mathf.Max(origin.y, target.y)
            );
            foreach (var pair in this.coordBorder) {
                float key;
                Vector2 interval;
                float result;
                if (pair.Key > 5000) {
                    key = pair.Key - 10000;
                    interval = intervalX;
                    result = (key - param2) / param1;
                } else {
                    key = pair.Key;
                    interval = intervalY;
                    result = param1 * key + param2;
                }
                //Debug.LogError(result);

                if ((result > pair.Value.x - 0.01f && result < pair.Value.y + 0.01f) &&
                    (result > interval.x && result < interval.y)) {
                    return true;
                }
            }
            return false;
        }
    }
}
