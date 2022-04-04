using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MapTileRelation {
    self,
    ally,
    enemy,
    neutral,
    slave,
    master,
    fallen,
    none
}

public class MapInfo {
    public Vector2 block = Vector2.zero;
    public Vector2 blockSize = Vector2.zero;
    public Rect blockRect = new Rect(Vector2.zero, Vector2.zero);
    public Vector2 minCoordinate = Vector2.one;
    public Vector2 maxCoordinate = new Vector2(1000, 1000);
    protected Vector2 minBlock = Vector2.zero;
    protected Vector2 maxBlock = Vector2.one;

    public Dictionary<uint, string> resourceName = new Dictionary<uint, string> {
        { 1, "Plain" },
        { 9, "Lumber" },
        { 10, "Marble" },
        { 11, "Steel" },
        { 12, "Food" },
        { 13, "Crystal" }
    };

    public Dictionary<uint, string> resourceStr = new Dictionary<uint, string> {
        { 1, "TileWood" },
        { 2, "TileRock" },
        { 3, "TileIron" },
        { 4, "TileFood" },     
        { 5, "TileMix" }
    };

    public Dictionary<string, string> elementsStr = new Dictionary<string, string> {
            { "resource_lumber", "TileWood" },
            { "resource_steel", "TileIron" },
            { "resource_marble", "TileRock" },
            { "resource_plain", string.Empty },
            { "resource_crystal", "TileMix"},
            { "camp_camp", "TileCamp" },
            { "resource_food", "TileFood" },
            { "npc_city_npc_city", "TileNpcCity" },
            { "_mountain", "TileBorderMountain" },
            { "pass_pass", "TilePass" },
            { "_river", "TilePassWater" },
            { "resource_road", "TilePassLand" },
            { "building_barrack", "TileBarrack" },
            { "building_city_hall", "TileCityHall" },
            { "building_warehouse", "TileWarehouse"}
        };

    public List<string> elementsInt = new List<string> {
            "TileResource",
            "TileNpcCity",
            "TileCamp",
            "TilePassLand",
            "TilePassLand",
            "TilePassWater",
            "TileBorderMountain",
            "TileBorderWater",
            "TileMountain",
            "TileCityHall",
            "TileWarehouse",
            "TileBarrack",
            "TileSelf",
            "TileEnemy"
        };

    public Vector2 GetBlock(Vector2 coordinate) {
        return new Vector2(
            Mathf.Floor((coordinate.x - 1) / this.blockSize.x),
            Mathf.Floor((coordinate.y - 1) / this.blockSize.y)
        );
    }

    public virtual void Init(Vector2 coordinate) {
        this.block = this.GetBlock(coordinate);
        this.SetBlockRect();
    }

    public void SetBlock(Vector2 block) {
        this.block = block;
        this.SetBlockRect();
    }

    private void SetBlockRect() {
        Vector2 blockCoordinate = new Vector2(
                              (this.block.x + 0.5f) * this.blockSize.x,
                              (this.block.y + 0.5f) * this.blockSize.y
        );
        this.blockRect.center = blockCoordinate;
    }

    public List<Vector2> GetCoordinateList(Vector2 center, int radius, bool containCenter) {
        List<Vector2> list = new List<Vector2>();
        for (int i = -radius; i <= radius; i++) {
            for (int j = -radius; j <= radius; j++) {
                list.Add(new Vector2(i + center.x, j + center.y));
            }
        }
        if (!containCenter) {
            list.Remove(center);
        }
        return list;
    }

    public virtual void Refresh() {

    }
}
