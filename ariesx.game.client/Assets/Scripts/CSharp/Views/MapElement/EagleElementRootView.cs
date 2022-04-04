using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class EagleElementRootView : BaseMapElementRootView {
        private List<string> directionList = new List<string>() {
            "WE",
            "NS",
            "EW",
            "SN",
            "SENW",
            "SWNE",
            "NESW",
            "NWSE"
        };
        protected override List<string> DirectionList {
            get {
                return directionList;
            }
        }

        private List<string> elementTypeList = new List<string>() {
            PrefabPath.eagle,
            PrefabPath.seagull
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
            {"SN", Vector2.up },
            {"SENW", Vector2.up - Vector2.right},
            {"SWNE", Vector2.up + Vector2.right},
            {"NESW", -Vector2.up - Vector2.right},
            {"NWSE", -Vector2.up + Vector2.right}
        };
        protected override Dictionary<string, Vector2> DirectionDict {
            get {
                return directionDict;
            }
        }

        void Awake() {
            this.maxCount = 2;
            base.InvokeRepeating("Generate", 5, 12);
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

                Vector2 startPos = -10 * direction + orthogonal * Random.Range(-5, 6) +
                    MapUtils.CoordinateToPosition(mapModel.centerCoordinate);
                Vector2 endPos = startPos + 20 * direction;

                elementView.Begin(directionStr, startPos, endPos);
                elementList.Add(obj);
            }
        }
    }
}
