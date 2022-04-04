using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.Events;
using TMPro;

namespace Poukoute {
    public enum TileArrowTrans {
        Attack,
        upgrade
    }

    public class TileView: MonoBehaviour, IPoolHandler {
        private enum EffectType {
            None,
            DefenderRecover,
            DurabilityRecover,
            Building,
            BuildingCompleted
        }

        public static bool needDecoration;
        public static bool isShowLevel;
        public static bool isShowSelfBuilding;
        [HideInInspector]
        public Vector3 originPosition;
        [HideInInspector]
        public bool hasMarchFlag = false;
        [HideInInspector]
        public ElementType type;
        [HideInInspector]
        public bool isVisible = true;
        public bool isInSight = true;
        [SerializeField]
        private AnimationParam focus;
        [SerializeField]
        private AnimationParam loseFocus;

        public static float FteMarchLineOffset {
            get {
                return troopAvatarOtherLayerOffset - marchLineLayerOffset - 0.001f;
            }
        }
        public Vector2 tmpCoordinate;
        public Vector2 Coordinate { get { return this.tmpCoordinate; } set { this.tmpCoordinate = value; } }

        public Vector3 Position {
            get {
                return this.originPosition;
            }
            set {
                this.originPosition = value;
                this.transform.position = value;
            }
        }
        public MapTileRelation Relation { get; set; }
        public UnityAction<Troop> RefreshQueueItemAnimation = null;

        private MapModel model;
        private CampaignModel campaignModel;
        private MapMarkModel markModel;
        private TroopModel troopModel;
        private BuildModel buildModel;

        private GameObject layerBelow;
        private GameObject layerBase;
        private GameObject layerRelation;
        private GameObject layerAbove;
        private GameObject layerAnimator;
        private GameObject layerEffect;
        private GameObject layerProtection;
        private GameObject layerBuildCure;
        private GameObject tileLevel;
        private GameObject selfBuilding;
        private GameObject chosenEffect;
        private GameObject battleFocus;
        private GameObject tileTroopSelf;
        private GameObject tileTroopOther;
        private GameObject tileDecoration;
        private GameObject layerUpgrade;
        private GameObject boss = null;
        private GameObject layerMark;
        private GameObject tileMark;
        public Vector2 cityOffset = Vector2.zero;
        private EffectType effectType;

        private bool isShowBuildingEffect;
        private bool isBuilding;

        private bool chosen = false;
        private bool IsChosen {
            get {
                return this.chosen;
            }
            set {
                if (this.chosen != value) {
                    this.chosen = value;
                    this.SetBattleStatus(!value);
                }
            }
        }

        private bool highLight = false;

        private float depth;

        // recover event
        private string durabilityEventId;
        private string defenderEventId;
        public UnityAction<string, Vector2> onTroopClick = null;
        // Tile layer.
        private const float tileLayerInterval = 0.01f;
        private const float belowLayerOffset = 0;
        private const float baseLayerOffset = -0.001f;
        private const float aboveAnimationOffset = -0.5f;
        private const float levelLayerOffset = -1.002f;
        private const float relationLayerOffset = -1.503f;
        private const float aboveLayerOffset = -2.004f;
        private static readonly Vector3 decorationLayerOffset = new Vector3(0.2f, 0.3f, -2.005f);
        private const float tileProtectionOffset = -3.005f;
        public static readonly Vector3 tile3DHeroOffset = new Vector3(0, -0.7f, -4f);
        public static int tileWaterReference = 0;
        private static readonly Vector3 tileLevelOffset =
            new Vector3(0, -0.8f, -5.006f);
        private const float chosenExLayerOffset = -2.016f;
        private const float cityTileLayerOffset = -2.054f;
        private const float relationVerticalOffset = 0;
        private const float highlightLayer = -2.5f;
        private float highlightLayerOffset = 0;

        // Global layer, camera z-axis is -12. Hightlight mask i 1.
        private const float effectLayerOffset = -1.006f;
        private const float buildUpLayerOffset = -1.5f;
        private const float tileLabelLayerOffset = -1.5f;
        private const float tileAvatarSelfLayerOffset = -1.59f;
        private const float troopAvatarOtherLayerOffset = -1.6f;
        private const float marchLineLayerOffset = -1.507f;
        public const float marchLineHighLayerOffset = -5.007f;

        private bool needShowCompleteEffect = false;

        public static readonly List<string> animationTileList = new List<string>() {
            "food8",
            "food10",
            "food11",
            "food12",
            "lumber7",
            "lumber8",
            "lumber9",
            "lumber10",
            "lumber11",
            "lumber12",
            "marble6",
            "marble7",
            "marble8",
            "marble9",
            "marble10",
            "marble11",
            "marble12",
            "steel8",
            "steel9",
            "steel10",
            "steel12"
        };

        /*****************************************************************/

        void Awake() {
            this.model = ModelManager.GetModelData<MapModel>();
            this.campaignModel = ModelManager.GetModelData<CampaignModel>();
            this.markModel = ModelManager.GetModelData<MapMarkModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
        }

        public static float GetMarchLineLayer() {
            return marchLineLayerOffset;
        }

        public void BuildTile() {
            this.SetPosition();
            this.type = this.model.GetTileType(Coordinate);
            this.isBuilding = (int)this.type >= (int)ElementType.townhall &&
                (int)this.type <= (int)ElementType.durability_up;
            if (this.type == ElementType.none) {
                return;
            } else if (this.type == ElementType.border) {
                string layerborderStr = this.model.GetBorderType(Coordinate);
                this.layerBelow = this.SetLayerSprite(PrefabPath.layerBelow,
                    SpritePath.tileLayerBelowPrefix, layerborderStr, belowLayerOffset);
            } else {
                MapTileRelation buildingRelation = MapTileRelation.self;
                string layerAboveStr = this.model.GetLayerAbove(Coordinate,
                    ref buildingRelation, ref this.isVisible, ref this.isInSight);
                if (animationTileList.Contains(layerAboveStr)) {
                    this.layerAnimator = this.GetObject(
                        "Map/TileAnimation/" + layerAboveStr, this.transform);
                    this.layerAnimator.transform.localPosition =
                        this.Position + Vector3.forward * aboveAnimationOffset;
                }
                string layerBelowStr = this.model.GetLayerBelow(Coordinate);
                string layerBaseStr = this.model.GetLayerBase(Coordinate);
                //string layerLevelStr = this.model.GetLayerLevel(coordinate);
                bool isRiver = (this.type == ElementType.river);
                if (isRiver) {
                    if (tileWaterReference == 0) {
                        TriggerManager.Invoke(Trigger.ShowWater);
                    }
                    tileWaterReference++;
                }
                this.layerBelow = this.SetLayerSprite(
                    isRiver ? PrefabPath.tileWater : PrefabPath.layerBelow,
                    SpritePath.tileLayerBelowPrefix, layerBelowStr, belowLayerOffset);

                this.layerBase = this.SetLayerSprite(PrefabPath.layerBase,
                    SpritePath.tileLayerBasePrefix, layerBaseStr, baseLayerOffset);
                if (this.type == ElementType.npc_city) {
                    string layerCityStr = this.model.GetLayerCityBase(Coordinate, ref this.cityOffset);
                    this.layerAbove = this.SetLayerCitySprite(layerCityStr);
                } else {
                    this.layerAbove = this.SetLayerSprite(
                        PrefabPath.layerAbove,
                        SpritePath.tileLayerAbovePrefix,
                        layerAboveStr,
                        aboveLayerOffset
                    );
                }

                if (this.layerAbove != null) {
                    Transform relation = this.layerAbove.transform.Find("Relation");
                    if (buildingRelation != MapTileRelation.self) {
                        relation.gameObject.SetActiveSafe(true);
                        relation.GetComponent<SpriteRenderer>().
                            sprite = ArtPrefabConf.GetSprite(
                                string.Concat(SpritePath.tileLayerAbovePrefix, layerAboveStr, "_enemy"));
                    } else {
                        relation.gameObject.SetActiveSafe(false);
                        this.isShowBuildingEffect =
                            layerAboveStr.Contains(ElementCategory.building);
                        if (this.isShowBuildingEffect) {
                            this.ShowBuildingEffect();
                        }
                        this.ShowBuildingCompleteEffect();
                    }
                }
            }
            if (TileView.isShowLevel) {
                this.SetTileLevelWithRule();
            }
            if (TileView.isShowSelfBuilding) {
                this.SetSelfBuildingWithRule();
            }
            this.CreateDecoration();
        }

        private void ShowBuildingEffect() {
            this.layerEffect = this.GetObject(
                PrefabPath.tileBuilding,
                this.transform
            );
            this.layerEffect.transform.localPosition = new Vector3(
                this.Position.x,
                this.Position.y,
                effectLayerOffset
            );
            this.effectType = EffectType.Building;
        }

        public void SetBuildingCompleteEffect() {
            this.needShowCompleteEffect = true;
        }

        private void ShowBuildingCompleteEffect() {
            if (this.needShowCompleteEffect) {
                this.layerEffect = this.GetObject(
                    PrefabPath.tileBuildComplete,
                    this.transform
                );
                this.layerEffect.transform.localPosition = new Vector3(
                    this.Position.x,
                    this.Position.y,
                    effectLayerOffset
                );
                this.needShowCompleteEffect = false;
            }
        }

        public void SetPosition() {
            this.depth = 3 + Coordinate.y * tileLayerInterval +
                (this.model.maxCoordinate.x - Coordinate.x) * tileLayerInterval;
            this.highlightLayerOffset = highlightLayer - this.depth;
            Vector2 position = MapUtils.CoordinateToPosition(Coordinate);
            this.Position = (Vector3)position + Vector3.forward * this.depth;
        }

        // To do: need checkout chest level when permit drag.
        public void SetTileLevelWithRule() {
            int level = this.model.GetLevel(Coordinate);
            int currentChestLevel = RoleManager.GetCurrentChestLevel();
            if (level > 0 && currentChestLevel > 0 &&
                Mathf.Abs(level - currentChestLevel) <= 1) {
                this.SetTileLevel(level);
            }
        }

        public void SetTileLevel(int level) {
            if (this.isBuilding) {
                return;
            }
            string label = string.Concat("tileLevel", level);
            bool isInPool = PoolManager.CheckPool(label);
            if (this.tileLevel == null) {
                this.tileLevel = this.GetObject(
                    PrefabPath.tileLevel,
                    this.transform,
                    label
                );
            }
            if (!isInPool) {
                this.tileLevel.GetComponent<TextMeshPro>().text = level > 0 ?
               string.Format(LocalManager.GetValue(LocalHashConst.level), level) : string.Empty;
            }
            this.tileLevel.transform.localPosition = new Vector3(
                this.Position.x,
                this.Position.y + tileLevelOffset.y,
                tileLevelOffset.z
            );
            this.tileLevel.GetComponent<TextMeshPro>().enabled = TileView.isShowLevel;
        }

        public void SetSelfBuildingWithRule() {
            if (this.isBuilding && (this.Relation == MapTileRelation.self)) {
                Coord crood = new Coord() {
                    X = (int)this.Coordinate.x,
                    Y = (int)this.Coordinate.y
                };
                ElementBuilding buildingInfo = this.buildModel.GetBuildingByCoord(crood);
                if (buildingInfo != null) {
                    this.SetSelfBuilding(buildingInfo);
                }
            }
        }

        public void SetSelfBuilding(ElementBuilding buildingInfo) {
            string label = string.Concat("selfBuilding", buildingInfo.Name);
            bool isInPool = PoolManager.CheckPool(label);
            if (this.selfBuilding == null) {
                this.selfBuilding = this.GetObject(
                    PrefabPath.selfBuilding,
                    this.transform,
                    label
                );
            }
            if (!isInPool) {
                this.selfBuilding.GetComponent<TextMeshPro>().text
                    = LocalManager.GetValue(LocalHashConst.mine) +
                    MapUtils.GetBuildingLocalName(buildingInfo);
            }
            this.selfBuilding.transform.localPosition = new Vector3(
                this.Position.x,
                this.Position.y + tileLevelOffset.y,
                tileLevelOffset.z
            );
            this.selfBuilding.GetComponent<TextMeshPro>().enabled = TileView.isShowSelfBuilding;
        }

        private GameObject SetLayerCitySprite(string city) {
            if (!city.CustomIsEmpty()) {
                Transform cityObj = this.transform.Find(city);
                if (cityObj == null) {
                    cityObj = this.GetObject(
                        PrefabPath.layerAbove,
                        this.transform,
                        city
                     ).transform;
                }
                cityObj.transform.localPosition = this.Position + Vector3.forward * aboveLayerOffset;
                cityObj.GetComponent<SpriteRenderer>().sprite =
                                ArtPrefabConf.GetSprite(city, false);
                return cityObj.gameObject;
            }
            return null;
        }

        private GameObject SetLayerSprite(string path,
            string spritePrefix, string sprite, float layerOffset) {
            if (!sprite.CustomIsEmpty()) {
                string spriteLabel = string.Concat(spritePrefix, sprite);
                bool isInPool = PoolManager.CheckPool(spriteLabel);
                Transform layerObj = this.transform.Find(spriteLabel);
                if (layerObj == null) {
                    layerObj = this.GetObject(path, this.transform, spriteLabel).transform;
                }
                layerObj.localPosition = this.Position;
                if (path.CustomEquals(PrefabPath.layerCamp)) {
                    layerObj.transform.localPosition += Vector3.up * 2;
                }
                layerObj.transform.localPosition += Vector3.forward * layerOffset;
                if (!isInPool) {
                    layerObj.GetComponent<SpriteRenderer>().sprite =
                        ArtPrefabConf.GetSprite(spritePrefix, sprite);
                }
                if (sprite.Contains(ElementName.mountain)) {
                    if (!sprite.Contains("mountain8")) {
                        layerObj.transform.localPosition -= Vector3.up * MapUtils.TileSize.y;
                        layerObj.transform.localPosition -= Vector3.forward * tileLayerInterval;
                    }
                }
                return layerObj.gameObject;
            }
            return null;
        }

        public bool EnableChosenEffect() {
            if (this.type == ElementType.none || this.type == ElementType.border) {
                return false;
            }
            if (!this.IsChosen) {
                this.SetBuildCureVisible(false);
                this.IsChosen = true;
                if (!(this.type == ElementType.camp || this.type == ElementType.river ||
                    this.type == ElementType.mountain || this.type == ElementType.npc_city)) {
                    this.chosenEffect = this.GetObject(PrefabPath.tileChosen, this.transform);
                    this.chosenEffect.transform.localPosition = this.Position;
                    this.HideRelation();
                    if (this.type == ElementType.pass) {
                        this.chosenEffect.transform.localPosition +=
                            Vector3.forward * (chosenExLayerOffset - 0.499f);
                    } else {
                        this.chosenEffect.transform.localPosition +=
                            Vector3.forward * (relationLayerOffset - 0.499f);
                    }
                    if (this.type != ElementType.road) {
                        this.layerBase = this.GetObject(PrefabPath.tilePlain, this.transform);
                        this.layerBase.transform.localPosition =
                            this.Position + Vector3.forward * (relationLayerOffset - 0.498f);
                        if (this.layerAnimator != null) {
                            this.layerAnimator.transform.localPosition =
                                this.Position + new Vector3(0, 0, -2.0035f);
                        }
                    } else if (this.layerBase != null) {
                        this.layerBase.transform.localPosition =
                            this.Position + Vector3.forward * (relationLayerOffset - 0.498f);
                    }
                    this.layerBelow = this.SetLayerSprite(PrefabPath.layerBelow,
                        SpritePath.tileLayerBelowPrefix, "base", belowLayerOffset);
                    this.layerBelow.transform.localPosition =
                        this.Position + Vector3.forward * (relationLayerOffset - 0.497f);
                    this.Focus();
                } else if (this.type == ElementType.npc_city) {
                    // Disable the npc_city chosen_effect
                    this.chosenEffect = this.GetObject(PrefabPath.tileChosen, this.transform);
                    this.chosenEffect.transform.localPosition = this.Position +
                        new Vector3(0, 0, (cityTileLayerOffset - 0.5f));
                    AnimationManager.Animate(this.chosenEffect, "Breath", null,
                        loop: true, space: PositionSpace.SelfWorld);
                } else {
                    this.chosenEffect = this.GetObject(PrefabPath.tileChosenEx, this.transform);
                    this.chosenEffect.transform.localPosition = this.Position +
                        Vector3.forward * chosenExLayerOffset;
                    AnimationManager.Animate(this.chosenEffect,
                        "Breath", null, loop: true, space: PositionSpace.SelfWorld);
                }
            }
            return true;
        }

        public void DisableChosenEffect() {
            if (this.IsChosen) {
                this.SetBuildCureVisible(this.troopModel.IsNeedShowBuildCure());
                this.IsChosen = false;
                if (!(this.type == ElementType.camp || this.type == ElementType.river ||
                    this.type == ElementType.mountain || this.type == ElementType.npc_city)) {
                    this.RemoveObject(this.chosenEffect);
                    AnimationManager.Finish(this.chosenEffect);
                    this.chosenEffect = null;
                    this.ShowRelation();
                    this.LoseFocus();
                    if (this.type != ElementType.road) {
                        this.RemoveObject(this.layerBase);
                        this.layerBase = null;
                        if ((this.layerRelation != null) && (this.type != ElementType.pass)) {
                            this.layerRelation.transform.localPosition =
                                this.Position + new Vector3(0, 0, relationLayerOffset);
                        }
                        if (this.layerAnimator != null) {
                            this.layerAnimator.transform.localPosition =
                                this.Position + new Vector3(0, 0, aboveAnimationOffset);
                        }
                    } else if (this.layerBase != null) {
                        this.layerBase.transform.localPosition =
                        this.Position + Vector3.forward * baseLayerOffset;
                    }
                    this.RemoveObject(this.layerBelow);
                    this.layerBelow = null;
                } else if (this.type == ElementType.npc_city) {
                    this.RemoveObject(this.chosenEffect);
                    this.chosenEffect = null;
                } else {
                    AnimationManager.Finish(this.chosenEffect);
                    this.RemoveObject(this.chosenEffect);
                    this.chosenEffect = null;
                }
            }
        }

        public void CreateAvaliable(string buildingId) {
            if (this.layerRelation != null) {
                this.RemoveObject(this.layerRelation);
                this.layerRelation = null;
            }
            if (this.model.IsAvaliable(Coordinate, buildingId)) {
                this.EnableEdit();
            }
        }

        public void DestroyAvaliable(string buildingId) {
            if (this.model.IsAvaliable(Coordinate, buildingId)) {
                this.DisableEdit();
            }
            this.CreateRelation();
        }

        public void RefreshUpgradableMark() {
            if (this.model.GetTileUpgradeable(this.Coordinate)) {
                if (this.layerUpgrade == null) {
                    this.layerUpgrade = this.GetObject(
                        PrefabPath.tileUpgradeableMark,
                        this.transform
                    );
                }
                this.layerUpgrade.transform.localPosition = new Vector3(
                    this.Position.x,
                    this.Position.y,
                    buildUpLayerOffset
                );
            } else {
                this.RemoveObject(this.layerUpgrade);
                this.layerUpgrade = null;
            }
        }

        public void CreateMark(MapMarkType markType) {
            bool isAllianceMark = (markType == MapMarkType.Alliance);
            bool hasNormalMark = this.IsMarkExist(MapMarkType.Others);
            bool hasAllianceMark = this.IsMarkExist(MapMarkType.Alliance);
            bool hasTileMarkObj = this.tileMark != null;
            if (hasAllianceMark && hasNormalMark && hasTileMarkObj && !isAllianceMark) {
                return;
            }
            if (this.tileMark == null) {
                this.tileMark = this.GetObject(PrefabPath.tileMark, this.transform);
            }
            this.tileMark.transform.localPosition = new Vector3(
                this.Position.x,
                this.Position.y,
                13
            );
            string markTail = isAllianceMark ? "alliance" : "other";
            this.tileMark.transform.Find("Mark").GetComponent<SpriteRenderer>().sprite =
                ArtPrefabConf.GetSprite(SpritePath.markIconPrefix, markTail);
        }

        public void RemoveMark(MapMarkType markType) {
            bool isAllianceMark = (markType == MapMarkType.Alliance);
            bool hasNormalMark = this.IsMarkExist(MapMarkType.Others) ||
                                this.IsMarkExist(MapMarkType.StrongHold) ||
                                this.IsMarkExist(MapMarkType.TownHall);
            bool hasAllianceMark = this.IsMarkExist(MapMarkType.Alliance);
            if (hasAllianceMark && !isAllianceMark) {
                return;
            } else if (hasNormalMark && isAllianceMark) {
                Debug.LogError(this.tileMark);
                this.tileMark.transform.Find("Mark").GetComponent<SpriteRenderer>().sprite =
                    ArtPrefabConf.GetSprite(SpritePath.markIconPrefix + "other");
            } else {
                this.RemoveObject(this.tileMark);
                this.tileMark = null;
            }
        }

        public void ShowBoss() {
            Transform bossTransform = this.transform.Find(PrefabName.pnlCampaignBoss);
            if (bossTransform != null) {
                this.boss = bossTransform.gameObject;
            } else {
                this.boss = PoolManager.GetObject(
                    PrefabPath.pnlCampaignBoss, this.transform);
                this.boss.transform.localPosition = tile3DHeroOffset;
                this.boss.transform.localScale = Vector3.one;
                this.boss.transform.eulerAngles = new Vector3(45, 180, 0);
                //float angleOffset = 45;
                //this.boss.transform.Rotate(Vector3.up, angleOffset);
            }
        }

        public void RemoveBoss() {
            if (this.boss != null) {
                PoolManager.RemoveObject(this.boss);
                this.boss = null;
            }
        }

        private bool IsMarkExist(MapMarkType markType) {
            Vector3 markKey = new Vector3() {
                x = this.Coordinate.x,
                y = this.Coordinate.y,
                z = (int)markType
            };

            return this.markModel.markDict.ContainsKey(markKey);
        }

        public void CreateBuildCure() {
            MapTileInfo tileInfo = this.model.GetTileInfoSeparate(this.Coordinate);
            bool isBuildTypeOk = (
                tileInfo != null &&
                tileInfo.buildingInfo != null &&
                tileInfo.buildingInfo.Level > 0 &&
                (tileInfo.buildingInfo.Type == (int)ElementType.townhall ||
                tileInfo.buildingInfo.Type == (int)ElementType.stronghold ||
                (tileInfo.buildingInfo.Type == (int)ElementType.npc_city)));
            if (isBuildTypeOk && (tileInfo.relation == MapTileRelation.self) &&
                this.troopModel.IsNeedShowBuildCure()) {
                if (this.layerBuildCure == null) {
                    this.layerBuildCure = this.GetObject(PrefabPath.tileBuildCure, this.transform);
                }
                this.layerBuildCure.transform.localPosition =
                    new Vector3(
                        this.Position.x,
                        this.Position.y + 0.4f,
                        tileLabelLayerOffset
                    );
            } else {
                this.RemoveObject(this.layerBuildCure);
                this.layerBuildCure = null;
            }
        }

        public void CreateBattleStatus() {
            if ((this.type == ElementType.pass ||
                this.type == ElementType.npc_city ||
                this.isBuilding) && this.effectType == EffectType.None) {
                long duration = 0;
                if (!this.model.IsDefenderRecover(Coordinate, ref duration)) {
                    if (this.layerEffect == null) {
                        this.layerEffect = this.GetObject(PrefabPath.tileDefenderRecover, this.transform);
                    }
                    this.defenderEventId = string.Concat(
                        1, Coordinate.x * 10000 + Coordinate.y
                    );
                    EventManager.AddDefenderRecoverEvent(
                        this.defenderEventId, this.Coordinate, duration
                    );

                } else if (!this.model.IsDurabilityMax(this.Coordinate, ref duration)) {
                    Debug.Log("PrefabPath.tileDurabilityRecover");
                    Debug.Log(duration);
                    if (this.layerEffect == null) {
                        this.layerEffect = this.GetObject(PrefabPath.tileDurabilityRecover, this.transform);
                    }
                    this.durabilityEventId = string.Concat(
                        2, Coordinate.x * 10000 + Coordinate.y
                    );
                    EventManager.AddDurabilityRecoverEvent(
                        this.durabilityEventId, this.Coordinate, duration
                    );
                }
                if (this.layerEffect != null) {
                    this.layerEffect.transform.localPosition = new Vector3(
                        this.Position.x,
                        this.Position.y,
                        effectLayerOffset
                    );
                }
            }
        }

        public void SetBattleStatus(bool visible) {
            if (this.layerEffect != null &&
                (this.effectType == EffectType.DurabilityRecover ||
                 this.effectType == EffectType.DefenderRecover)) {
                this.layerEffect.SetActive(visible);
                this.layerEffect.GetComponent<Animator>().enabled = visible;
            }
        }

        public void RemoveDurabilityState() {
            if (this.layerEffect != null && this.effectType == EffectType.DurabilityRecover) {
                this.layerEffect.GetComponent<Animator>().enabled = false;
                this.RemoveObject(this.layerEffect);
                this.layerEffect = null;
            }
        }

        public void RemoveDefenderState() {
            if (this.layerEffect != null && this.effectType == EffectType.DefenderRecover) {
                this.layerEffect.GetComponent<Animator>().enabled = false;
                this.RemoveObject(this.layerEffect);
                this.layerEffect = null;
            }
        }

        public void SetBuildCureVisible(bool visible) {
            if (this.layerBuildCure != null) {
                this.layerBuildCure.SetActiveSafe(visible);
            }
        }

        public void CreateTroop() {
            int otherTroopCount = 0;
            TroopRelation troopRelation = TroopRelation.Ally;
            int avatarId = 0;
            otherTroopCount = this.model.GetOtherTroopCount(
                this.Coordinate, ref avatarId, ref troopRelation
            );
            if (otherTroopCount > 0 && this.isInSight) {
                if (this.tileTroopOther == null) {
                    this.tileTroopOther = this.GetObject(
                        PrefabPath.tileTroop,
                        this.transform
                    );
                }
                this.tileTroopOther.transform.localPosition = new Vector3(
                    this.Position.x,
                    this.Position.y,
                    troopAvatarOtherLayerOffset
                );
                MapHeroHeadView heroHeadView = this.tileTroopOther.
                    transform.Find("Troop").GetComponent<MapHeroHeadView>();
                heroHeadView.SetHeroHead(troopRelation, avatarId.ToString(), otherTroopCount);
            } else if (this.tileTroopOther != null) {
                this.RemoveObject(this.tileTroopOther);
                this.tileTroopOther = null;
            }

            this.SetSelfTroopSprite();
        }

        public void UpdateTroop() {
            if (this.tileTroopSelf != null) {
                this.SetSelfTroopSprite();
                this.UpdateTroopTreatmentStatus(this.tileTroopSelf.transform);
            } else {
                this.CreateTroop();
            }
        }

        private void UpdateTroopTreatmentStatus(Transform tileTroop) {
            List<Troop> troopList = this.troopModel.GetTroopsAt(Coordinate);
            bool hasTroopUnderTreatment = false;
            foreach (Troop troop in troopList) {
                if (EventManager.IsTroopUnderTreatment(troop.Id)) {
                    hasTroopUnderTreatment = true;
                    break;
                }
            }
            UIManager.SetHeroRecoverEffectSOnTile(hasTroopUnderTreatment, tileTroop);
        }

        private void SetSelfTroopSprite() {
            List<Troop> troopList = this.troopModel.GetTroopsAt(this.Coordinate);
            int selfTroopCount = troopList.Count;
            if (selfTroopCount > 0) {
                Troop troop = troopList[0];
                if (this.tileTroopSelf == null) {
                    this.tileTroopSelf = this.GetObject(
                        PrefabPath.tileTroop,
                        this.transform
                    );
                }
                MapHeroHeadView heroHeadView = this.tileTroopSelf.
                    transform.Find("Troop").GetComponent<MapHeroHeadView>();
                string heroAvatar = string.Empty;
                if (troop.Positions.Count > 0) {
                    TroopModel.TroopPositionReSort(troop);
                    heroAvatar = troop.Positions[0].Name;
                }
                heroHeadView.SetHeroHead(TroopRelation.Self, heroAvatar, selfTroopCount);
                Vector3 offset = this.tileTroopOther != null ? Vector3.right * -0.5f : Vector3.zero;
                this.tileTroopSelf.transform.localPosition = new Vector3(
                    this.Position.x + offset.x,
                    this.Position.y,
                    tileAvatarSelfLayerOffset
                );
                TroopStatus status = this.troopModel.GetTroopStatus(troop);
                if (status == TroopStatus.HeroNotFull ||
                    status == TroopStatus.Unconfiged) {
                    AnimationManager.Animate(this.tileTroopSelf,
                        "Flash", null, space: PositionSpace.SelfWorld);
                } else {
                    AnimationManager.Finish(this.tileTroopSelf);
                }
                if (this.RefreshQueueItemAnimation != null) {
                    this.RefreshQueueItemAnimation.Invoke(troop);
                }
            } else if (this.tileTroopSelf != null) {
                this.RemoveObject(this.tileTroopSelf);
                this.tileTroopSelf = null;
            }
        }

        private void CreateDecoration() {
            if (this.type == ElementType.townhall && needDecoration) {
                this.tileDecoration = this.GetObject(PrefabPath.tileDecoration, this.transform);
                this.tileDecoration.transform.localPosition =
                    Position + decorationLayerOffset;
            }
        }

        public void CreateRelation() {
            this.Relation = this.model.GetRelation(this.Coordinate);
            if (this.Relation != MapTileRelation.none) {
                if (this.layerRelation == null) {
                    this.layerRelation = this.GetObject(PrefabPath.relation, this.transform);
                }
                this.layerRelation.GetComponent<SpriteRenderer>().sprite =
                    ArtPrefabConf.GetSprite(SpritePath.tileRelationPrefix,
                        Enum.GetName(typeof(MapTileRelation), this.Relation));
                float offset = relationLayerOffset;
                if (this.type == ElementType.pass) {
                    offset = chosenExLayerOffset;
                } else if (this.type == ElementType.npc_city) {
                    offset = -2.035f;
                }
                this.layerRelation.transform.localPosition = this.Position + new Vector3(0, 0, offset);
            }
        }

        public void CreateRelationFte() {
            this.Relation = this.model.GetRelation(this.Coordinate);
            if (this.Relation != MapTileRelation.none) {
                //if (this.layerRelation == null) {
                    this.layerRelation = this.GetObject(PrefabPath.relationFte, this.transform);
               // }
                this.layerRelation.GetComponent<SpriteRenderer>().sprite =
                    ArtPrefabConf.GetSprite(SpritePath.tileRelationPrefix,
                        Enum.GetName(typeof(MapTileRelation), this.Relation));
                this.layerRelation.transform.position =
                    (Vector3)MapUtils.CoordinateToPosition(this.Coordinate) +
                    Vector3.up * relationVerticalOffset;
                float offset = relationLayerOffset;
                if (this.type == ElementType.pass) {
                    offset = chosenExLayerOffset;
                } else if (this.type == ElementType.npc_city) {
                    offset = -2.035f;
                }
                this.layerRelation.transform.localPosition = this.Position + new Vector3(0, 0, offset);
                GameObject pnlGreen = layerRelation.transform.Find("PnlGreen").gameObject;
                AudioManager.Play("show_fte_tile_occupy", AudioType.Show,
                    AudioVolumn.High, isAdditive: true);
                AnimationManager.Animate(pnlGreen, "Show", finishCallback: () => {
                    AnimationManager.Animate(pnlGreen, "Jump");
                });
            }
        }

        public void DeleteRelationFte() {
            if (this.layerRelation.transform.Find("PnlGreen") != null) {
                this.layerRelation.transform.Find("PnlGreen").gameObject.SetActiveSafe(false);
            }
        }

        private void ShowRelation() {
            if (this.layerRelation != null) {
                this.layerRelation.SetActiveSafe(true);
            }
        }

        private void HideRelation() {
            if (this.layerRelation != null) {
                this.layerRelation.SetActiveSafe(false);
            }
        }

        private void Focus(bool needAnimation = true) {
            if (needAnimation) {
                this.AnimationComponent(this.layerBelow, this.focus);
                this.AnimationComponent(this.layerBase, this.focus);
                this.AnimationComponent(this.layerRelation, this.focus);
                this.AnimationComponent(this.layerAbove, this.focus);
                this.AnimationComponent(this.layerAnimator, this.focus);
                this.AnimationComponent(this.layerEffect, this.focus);
                this.AnimationComponent(this.chosenEffect, this.focus,
                    () => AnimationManager.Animate(this.chosenEffect, "Breath"));
            }
            this.ChangeComponentVertical(this.layerBelow, 0.4f);
            this.ChangeComponentVertical(this.layerBase, 0.4f);
            this.ChangeComponentVertical(this.layerRelation, 0.4f);
            this.ChangeComponentVertical(this.layerAbove, 0.4f);
            this.ChangeComponentVertical(this.layerAnimator, 0.4f);
            this.ChangeComponentVertical(this.layerEffect, 0.4f);
            this.ChangeComponentVertical(this.chosenEffect, 0.4f);
            this.ChangeComponentVertical(this.monster, 0.4f);
            this.ChangeComponentVertical(this.tileTroopOther, 0.4f);
            this.ChangeComponentVertical(this.tileTroopSelf, 0.4f);
        }

        private void LoseFocus(bool needAnimation = true) {
            if (needAnimation) {
                this.AnimationComponent(this.layerBelow, this.loseFocus);
                this.AnimationComponent(this.layerBase, this.loseFocus);
                this.AnimationComponent(this.layerRelation, this.loseFocus);
                this.AnimationComponent(this.layerAbove, this.loseFocus);
                this.AnimationComponent(this.layerAnimator, this.loseFocus);
                this.AnimationComponent(this.layerEffect, this.loseFocus);
                this.AnimationComponent(this.chosenEffect, this.loseFocus);
            }
            this.ChangeComponentVerticalNormal(this.layerBelow, 0.4f);
            this.ChangeComponentVerticalNormal(this.layerBase, 0.4f);
            this.ChangeComponentVerticalNormal(this.layerRelation, 0.4f);
            this.ChangeComponentVerticalNormal(this.layerAbove, 0.4f);
            this.ChangeComponentVerticalNormal(this.layerAnimator, 0.4f);
            this.ChangeComponentVerticalNormal(this.layerEffect, 0.4f);
            this.ChangeComponentVerticalNormal(this.monster, 0.4f);
            this.ChangeComponentVerticalNormal(this.tileTroopOther, 0.4f);
            this.ChangeComponentVerticalNormal(this.tileTroopSelf, 0.4f);
        }

        private void AnimationComponent(GameObject component,
            AnimationParam param, UnityAction callback = null) {
            AnimationManager.Animate(component, param, isOffset: true,
                space: PositionSpace.SelfWorld, finishCallback: callback);
        }

        private void ChangeLayer(int layer) {
            AnimationManager.Finish(this.gameObject);
            this.ChangeComponentLayer(this.layerBelow, layer);
            this.ChangeComponentLayer(this.layerBase, layer);
            this.ChangeComponentLayer(this.layerRelation, layer);
            this.ChangeComponentLayer(this.layerAbove, layer);
            this.ChangeComponentLayer(this.layerAnimator, layer);
            this.ChangeComponentLayer(this.chosenEffect, layer);
            this.ChangeComponentLayer(this.monster, layer);
            this.ChangeComponentLayer(this.tileTroopSelf, layer);
            this.ChangeComponentLayer(this.tileTroopOther, layer);
        }

        private void ChangeComponentLayer(GameObject component, int layer) {
            if (component != null) {
                GameHelper.SetLayer(component, layer);
            }
        }

        private void ChangeComponentVertical(GameObject component, float offset) {
            if (component != null) {
                component.transform.localPosition +=
                    Vector3.up * offset;
            }
        }

        private void ChangeComponentVerticalNormal(GameObject component, float offset) {
            if (component != null) {
                component.transform.localPosition -=
                    Vector3.up * offset;
            }
        }

        private void ChangeComponentToHighlight(GameObject component, float offset) {
            if (component != null) {
                component.transform.localPosition +=
                    Vector3.forward * offset;
            }
        }

        private void ChangeComponentToNormal(GameObject component, float offset) {
            if (component != null) {
                component.transform.localPosition -=
                    Vector3.forward * offset;
            }
        }

        public void ChangeToHighlightLayer() {
            if (!this.highLight) {
                this.ChangeComponentToHighlight(this.layerBelow, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.layerBase, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.layerRelation, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.layerAbove, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.layerAnimator, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.layerEffect, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.chosenEffect, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.monster, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.tileTroopOther, this.highlightLayerOffset);
                this.ChangeComponentToHighlight(this.tileTroopSelf, this.highlightLayerOffset);
                this.highLight = true;
            }
        }

        public void ChangeToAboveUILayer() {
            this.ChangeLayer(UIManager.LayerAboveUIIndex);
            if (this.transform.Find("TileTroop") != null) {
                GameHelper.SetLayer(this.transform.Find("TileTroop").gameObject,
                    UIManager.LayerUIInvisibleIndex);
            }
        }

        public void ChangeToNormalLayer() {
            this.ChangeLayer(UIManager.LayerUIInvisibleIndex);
            if (highLight) {
                this.ChangeComponentToNormal(this.layerBelow, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.layerBase, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.layerRelation, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.layerAbove, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.layerAnimator, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.layerEffect, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.chosenEffect, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.monster, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.tileTroopOther, this.highlightLayerOffset);
                this.ChangeComponentToNormal(this.tileTroopSelf, this.highlightLayerOffset);
                this.highLight = false;
            }
        }

        public void EnableEdit() {
            AnimationManager.Finish(this.gameObject);
            if (this.chosenEffect != null) {
                this.chosenEffect.gameObject.SetActiveSafe(false);
            }
            if (this.layerBase == null) {
                this.layerBase = this.GetObject(PrefabPath.tilePlain, this.transform);
                this.layerBase.transform.localPosition =
                        this.Position + Vector3.forward * baseLayerOffset;
            }
            this.ChangeToHighlightLayer();
        }

        public void DisableEdit() {
            if (this.chosenEffect != null) {
                this.chosenEffect.gameObject.SetActiveSafe(true);
            }
            this.RemoveObject(this.layerBase);
            this.layerBase = null;
            this.ChangeToNormalLayer();
        }


        private GameObject monster = null;
        private readonly Vector3 monsterEulerAngles = new Vector3(38.634f, -143.385f, 25.149f);
        private readonly Vector3 monsterLocalScale = Vector3.one * 1.241f;
        public void ShowMonster(int monsterLevel) {
            string monsterPath = PrefabPath.pnlCampaignMonster_1;
            if (CampaignModel.monsterType == CampaignModel.nian) {
                monsterPath = PrefabPath.pnlCampaignMonster_2;
            }
            if (this.monster == null) {
                this.monster = this.GetObject(monsterPath, this.transform);
            }
            this.monster.transform.localPosition = this.Position + tile3DHeroOffset;
            this.monster.transform.localScale = this.monsterLocalScale;
            this.monster.transform.eulerAngles = this.monsterEulerAngles;
        }

        public void Remove3DHero() {
            if (this.monster != null) {
                this.RemoveObject(this.monster);
                this.monster = null;
            }
        }

        public void SetLevelVisible(bool visible) {
            if (this.tileLevel != null) {
                GameHelper.SetTextVisible(this.tileLevel.transform, visible);
            } else if (visible) {
                this.SetTileLevelWithRule();
            }
        }

        public void SetSelfBuildingVisible(bool visible) {
            if (this.selfBuilding != null) {
                GameHelper.SetTextVisible(this.selfBuilding.transform, visible);
            } else if (visible) {
                this.SetSelfBuildingWithRule();
            }
        }

        public void ResetTileLevel() {
            int level = this.model.GetLevel(this.Coordinate);
            int currentChestLevel = RoleManager.GetCurrentChestLevel();
            if (Mathf.Abs(level - currentChestLevel) > 1 && this.tileLevel != null) {
                this.RemoveObject(this.tileLevel);
                this.tileLevel = null;
            } else if (currentChestLevel > 0 && this.tileLevel == null) {
                this.SetTileLevelWithRule();
            }
        }

        public void ResetSelfBuilding() {
            if (this.isBuilding && (this.Relation == MapTileRelation.self)) {
                if (!this.isBuilding || !(this.Relation == MapTileRelation.self)) {
                    this.RemoveObject(this.selfBuilding);
                    this.selfBuilding = null;
                } else {
                    this.SetSelfBuildingWithRule();
                }
            }
        }

        public void SetTileProtection() {
            TileProtectType protectType = this.model.GetTileProtectType(this.Coordinate);
            if (protectType != TileProtectType.None) {
                Transform layerProtectionTrans = this.transform.Find(PrefabName.tileProtection);
                if (layerProtectionTrans == null) {
                    this.layerProtection = this.GetObject(PrefabPath.tileProtection, this.transform);
                } else {
                    this.layerProtection = layerProtectionTrans.gameObject;
                }
                this.layerProtection.transform.localPosition =
                    this.Position + Vector3.forward * tileProtectionOffset;
                AnimationManager.Stop(this.layerProtection.gameObject);
                AnimationManager.Animate(this.layerProtection.gameObject,
                    "Flash", null, space: PositionSpace.SelfWorld);
            }
        }

        public void AddBattleFocus(UnityAction callback) {
            if (this.battleFocus == null) {
                this.battleFocus = this.GetObject(PrefabPath.tileFocus, this.transform);
            }
            this.battleFocus.transform.localPosition = this.Position +
                Vector3.forward * chosenExLayerOffset;
            AnimationManager.Animate(this.battleFocus, "Focus", () => {
                this.RemoveObject(this.battleFocus);
                this.battleFocus = null;
            }, space: PositionSpace.World);
            callback.InvokeSafe();
        }

        public void RemoveBattleFocus() {
            this.RemoveObject(this.chosenEffect);
            this.chosenEffect = null;
        }

        public void Clear() {
            this.Remove3DHero();
            this.RemoveBoss();
            if (this.effectType == EffectType.DefenderRecover) {
                EventManager.FinishedList.Add(this.defenderEventId);
            }
            if (this.effectType == EffectType.DurabilityRecover) {
                EventManager.FinishedList.Add(this.durabilityEventId);
            }
            if (this.type == ElementType.river) {
                tileWaterReference--;
                if (tileWaterReference == 0) {
                    TriggerManager.Invoke(Trigger.HideWater);
                }
            }

            // To do: need debug log collector, DEBUG 15302
            Transform aboveUICamera = this.transform.Find("AboveUICamera");
            if (aboveUICamera != null) {
                aboveUICamera.transform.SetParent(GameManager.MainCamera.transform);
                aboveUICamera.transform.localPosition = this.Position;
            }
            this.IsChosen = false;
            this.RemoveObject(this.tileLevel);
            this.tileLevel = null;
            this.RemoveObject(this.layerBelow);
            this.layerBelow = null;
            this.RemoveObject(this.layerBase);
            this.layerBase = null;
            this.RemoveObject(this.layerRelation);
            this.layerRelation = null;
            this.RemoveObject(this.layerAbove);
            this.layerAbove = null;
            this.RemoveObject(this.layerAnimator);
            this.layerAnimator = null;
            this.RemoveObject(this.layerEffect);
            this.layerEffect = null;
            this.RemoveObject(this.layerProtection);
            this.layerProtection = null;
            this.RemoveObject(this.tileTroopSelf);
            this.tileTroopSelf = null;
            this.RemoveObject(this.tileTroopOther);
            this.tileTroopOther = null;
            this.RemoveObject(this.layerUpgrade);
            this.layerUpgrade = null;
            this.RemoveObject(this.layerBuildCure);
            this.layerBuildCure = null;
            this.RemoveObject(this.tileMark);
            this.tileMark = null;
            this.RemoveObject(this.monster);
            this.monster = null;
            this.RemoveObject(this.chosenEffect);
            this.chosenEffect = null;
            this.RemoveObject(this.tileDecoration);
            this.tileDecoration = null;
            this.RemoveObject(this.selfBuilding);
            this.selfBuilding = null;

            this.effectType = EffectType.None;
            this.highLight = false;
        }

        private GameObject GetObject(string path, Transform root,
            string label = "") {
            return PoolManager.GetObject(path, root, label, poolType: PoolType.Tile);
        }

        private void RemoveObject(GameObject obj) {
            PoolManager.RemoveObject(obj, PoolType.Tile);
        }

        public void OnInPool() {
            this.needShowCompleteEffect = false;
        }

        public void OnOutPool() {

        }

        #region FTE

        public void CreatFteTroop() {
            Transform fteTroop = this.transform.Find(PrefabName.fteTroop);
            if (fteTroop == null) {
                fteTroop = this.GetObject(PrefabPath.fteTroop, this.transform).transform;
            }
            fteTroop.localPosition = this.Position + new Vector3(
                fteTroop.localPosition.x,
                fteTroop.localPosition.y,
                cityTileLayerOffset
            );
        }

        public void DeleteFteTroop() {
            this.RemoveObject(this.transform.Find("FteTroop").gameObject);
        }

        public void FteSetTileAboveVisible(bool visible) {
            if (this.layerAbove != null) {
                this.layerAbove.SetActiveSafe(visible);
            }
        }

        #endregion
    }
}