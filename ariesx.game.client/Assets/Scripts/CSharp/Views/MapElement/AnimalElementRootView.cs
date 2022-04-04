using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class AnimalElementRootView : BaseMapElementRootView {
        private List<string> directionList = new List<string>() {
            "WE",
            "NS",
            "EW",
            "SN"
        };
        protected override List<string> DirectionList {
            get {
                return directionList;
            }
        }

        private List<string> elementTypeList = new List<string>() {
            PrefabPath.squirrel,
            PrefabPath.redpanda,
            PrefabPath.deer
        };
        protected override List<string> ElementTypeList {
            get {
                return this.elementTypeList;
            }
        }

        private Dictionary<string, Vector2> directionDict = new Dictionary<string, Vector2>() {
            {"WE", Vector2.right },
            {"NS", -Vector2.up },
            {"EW", -Vector2.right },
            {"SN", Vector2.up }
        };
        protected override Dictionary<string, Vector2> DirectionDict {
            get {
                return directionDict;
            }
        }

        void Awake() {
            this.maxCount = 3;
            base.InvokeRepeating("Generate", 4, 10);
            if (mapModel == null) {
                mapModel = ModelManager.GetModelData<MapModel>();
            }
        }

        protected override void Generate() {
            if (this.elementList.Count < maxCount) {
                int elementIndex = Random.Range(0, this.ElementTypeList.Count);
                GameObject obj = PoolManager.GetObject(
                    this.ElementTypeList[elementIndex],
                    this.transform
                );

                MapElementView elementView = obj.GetComponent<MapElementView>();
                elementView.endAction = this.OnElementDestroy;
                int directionIndex = Random.Range(0, this.DirectionList.Count);
                string directionStr = this.DirectionList[directionIndex];
                Vector2 direction = this.DirectionDict[directionStr].normalized;


                Vector2 orthogonal = new Vector2(-direction.y, direction.x);
                Vector2 startCoordinate = -5 * direction + orthogonal * Random.Range(-5, 6) +
                     mapModel.centerCoordinate;
                startCoordinate = new Vector2(
                    Mathf.RoundToInt(startCoordinate.x),
                    Mathf.RoundToInt(startCoordinate.y)
                ) + Vector2.one * 0.5f;
                float startDepth = 3 + startCoordinate.y * tileLayerInterval +
                    (mapModel.maxCoordinate.x - startCoordinate.x) * tileLayerInterval;
                Vector3 startPos = (Vector3)MapUtils.CoordinateToPosition(startCoordinate) +
                    Vector3.forward * (startDepth - 2.004f);

                Vector2 endCoordinate = startCoordinate + direction * 10;
                float endDepth = 3 + endCoordinate.y * tileLayerInterval +
                    (mapModel.maxCoordinate.x - endCoordinate.x) * tileLayerInterval;
                Vector3 endPos = (Vector3)MapUtils.CoordinateToPosition(endCoordinate) +
                    Vector3.forward * (endDepth - 2.004f);

                elementView.Begin(directionStr, startPos, endPos);
                elementList.Add(obj);
            }
        }
    }
}
