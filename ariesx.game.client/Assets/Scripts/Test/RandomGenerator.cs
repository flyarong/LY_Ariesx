using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestPoint {
    public int position;
    public List<int> pointList = new List<int>();
    public List<int> avaliableList = new List<int>();
}

public class RandomGenerator {
    Vector2 cellSize = Vector2.zero;
    Vector2 totalSize = Vector2.zero;
    Vector2 effectSize = Vector2.zero;
    int amount = 0;

    List<int> axisXList = new List<int>();

    Dictionary<int, TestPoint> usedXDict = new Dictionary<int, TestPoint>();
    Dictionary<int, TestPoint> usedYDict = new Dictionary<int, TestPoint>();
    List<Rect> rectList = new List<Rect>();
    List<Vector2> cellList = new List<Vector2>();

    private void Init() {
        this.axisXList.Clear();
        for (int i = 0; i <= this.totalSize.x - this.cellSize.x; i++) {
            this.axisXList.Add(i);
        }
        this.usedXDict.Clear();
        this.usedYDict.Clear();
        this.cellList.Clear();
        this.rectList.Clear();
    }

    public List<Vector2> RandomSquare(Vector2 cellSize, Vector2 totalSize, int amount) {
        this.cellSize = cellSize;
        this.totalSize = totalSize;
        this.amount = amount;
        this.effectSize = new Vector2(
            this.totalSize.x - this.cellSize.x,
            this.totalSize.y - this.cellSize.y
        );
        this.Init();
        this.GenerateX();
        return this.cellList;
    }

    void GenerateX() {
        while (true) {
            if (this.axisXList.Count == 0) {
                Debug.LogError("Exit");
                Debug.LogError(this.rectList.Count);
                return;
            }
            int index = Random.Range(0, this.axisXList.Count);
            int position = this.axisXList[index];
            TestPoint point;
            if (this.usedXDict.ContainsKey(position)) {
                point = this.usedXDict[position];
            } else {
                point = new TestPoint {
                    position = position,
                };
                for (int i = 0; i <= effectSize.x; i++) {
                    point.avaliableList.Add(i);
                }
                this.usedXDict.Add(position, point);
            }

            if (!this.GenerateY(point)) {
                this.axisXList.Remove(position);
            }
            if (this.rectList.Count == this.amount) {
                return;
            }
        }
    }

    bool GenerateY(TestPoint pointX) {
        TestPoint point;
        while (true) {
            if (pointX.avaliableList.Count == 0) {
                return false;
            }
            int index = Random.Range(0, pointX.avaliableList.Count);
            int position = pointX.avaliableList[index];
            int cellSizeX = Mathf.RoundToInt(this.cellSize.x);
            int cellSizeY = Mathf.RoundToInt(this.cellSize.y);
            if (this.usedYDict.ContainsKey(position)) {
                point = this.usedYDict[position];
                if (point.avaliableList.Contains(pointX.position) && !this.IsInRect(pointX.position, point.position)) {
                    pointX.pointList.Add(position);
                    for (int i = point.position - (cellSizeY - 1); i < point.position + cellSizeY; i++) {
                        if (pointX.avaliableList.Contains(i))
                            pointX.avaliableList.Remove(i);
                    }
                    for (int i = pointX.position - (cellSizeX - 1); i < pointX.position + cellSizeX; i++) {
                        if (point.avaliableList.Contains(i))
                            point.avaliableList.Remove(i);
                    }
                    this.cellList.Add(new Vector2(
                        pointX.position + this.cellSize.x / 2,
                        point.position + this.cellSize.y / 2
                    ));
                    this.rectList.Add(new Rect(pointX.position, point.position, cellSizeX, cellSizeY));
                    return true;
                } else {
                    if (pointX.avaliableList.Contains(point.position))
                        pointX.avaliableList.Remove(point.position);
                    if (point.avaliableList.Contains(pointX.position))
                        point.avaliableList.Remove(pointX.position);
                    continue;
                }
            } else {
                point = new TestPoint {
                    position = position
                };
                for (int i = 0; i <= effectSize.y; i++) {
                    point.avaliableList.Add(i);
                }
                if (pointX.avaliableList.Contains(point.position))
                    pointX.avaliableList.Remove(point.position);
                if (point.avaliableList.Contains(pointX.position))
                    point.avaliableList.Remove(pointX.position);
                if (!this.IsInRect(pointX.position, point.position)) {
                    for (int i = point.position - (cellSizeY - 1); i < point.position + cellSizeY; i++) {
                        if (pointX.avaliableList.Contains(i))
                            pointX.avaliableList.Remove(i);
                    }
                    for (int i = pointX.position - (cellSizeX - 1); i < pointX.position + cellSizeX; i++) {
                        if (point.avaliableList.Contains(i))
                            point.avaliableList.Remove(i);
                    }
                    this.usedYDict.Add(position, point);
                    pointX.pointList.Add(position);
                    this.cellList.Add(new Vector2(
                        pointX.position + this.cellSize.x / 2,
                        point.position + this.cellSize.y / 2
                    ));
                    this.rectList.Add(new Rect(pointX.position, point.position, cellSizeX, cellSizeY));
                    return true;
                } else {
                    continue;
                }
            }
        }
    }

    bool IsInRect(float x, float y) {
        Rect newRect = new Rect(x, y, this.cellSize.x, this.cellSize.y);
        foreach (Rect rect in this.rectList) {
            if (rect.Overlaps(newRect)) {
                return true;
            }
        }
        return false;
    }
}
