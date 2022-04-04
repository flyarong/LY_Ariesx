using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class TerrainBackgroundView : MonoBehaviour {
        public List<GameObject> bgList = new List<GameObject>();
        public List<MeshRenderer> waterRenderList = new List<MeshRenderer>();

        private Dictionary<Vector2, GameObject> bgDict = new Dictionary<Vector2, GameObject>();
        private List<Vector2> blockList = new List<Vector2>();
        private List<Vector2> changeBlockList = new List<Vector2>();
        private List<Vector2> oldBlockList = new List<Vector2>();
        private Vector2 size;
      //  private Vector2 center = Vector2.zero;
        private Vector2 origin = Vector2.zero;
        private Rect rect;
        //private Vector2 block = Vector2.zero;

        void Awake() {
            size = new Vector2(6 * MapUtils.TileSize.x, 10 * MapUtils.TileSize.y);
            this.transform.position = 
            origin = MapUtils.CoordinateToPosition(RoleManager.GetRoleCoordinate());
            this.rect = new Rect(-size * 0.55f + origin, size * 1.1f);
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    Vector2 key = new Vector2(i, -j);
                    this.bgDict.Add(key, bgList[4 + j * 3 + i]);
                    this.blockList.Add(key);
                }
            }
            TriggerManager.Regist(Trigger.ShowWater, this.ShowWater);
            TriggerManager.Regist(Trigger.HideWater, this.HideWater);
        }

        public Vector2 Center {
            set {
                if (!this.rect.Contains(value)) {
                    Vector2 newBlock = new Vector2(Mathf.RoundToInt((value.x - this.origin.x) / size.x),
                        Mathf.RoundToInt((value.y - this.origin.y) / size.y));
                    this.rect.center = new Vector2(
                        newBlock.x * size.x, 
                        newBlock.y * size.y
                    ) + this.origin;
                    this.OnBlockChange(newBlock);
                    //this.block = newBlock;
                }
            }
        }

        private void OnBlockChange(Vector2 newBlock) {
            List<Vector2> newBlockList = this.GetNewBlockList(newBlock);
            this.changeBlockList.Clear();
            this.oldBlockList.Clear();
            foreach (Vector2 block in this.blockList) {
                if (!newBlockList.Contains(block)) {
                    this.oldBlockList.Add(block);
                }
            }

            foreach (Vector2 block in newBlockList) {
                if (!blockList.Contains(block)) {
                    this.changeBlockList.Add(block);
                }
            }

            int count = 0;
            foreach (Vector2 block in this.oldBlockList) {
                GameObject blockObj = this.bgDict[block];
                this.bgDict.Remove(block);
                Vector2 key = this.changeBlockList[count++];
                this.bgDict[key] = blockObj;
                blockObj.transform.position = new Vector3(
                    key.x * size.x + origin.x,
                    key.y * size.y + origin.y,
                    blockObj.transform.position.z
                );
            }

            this.blockList = newBlockList;
        }

        private List<Vector2> GetNewBlockList(Vector2 block) {
            List<Vector2> newBlockList = new List<Vector2>();
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    Vector2 key = new Vector2(i + block.x, j + block.y);
                    newBlockList.Add(key);
                }
            }
            return newBlockList;
        }

        private void ShowWater() {
            foreach (MeshRenderer renderer in this.waterRenderList) {
                renderer.enabled = true;
            }
        }

        private void HideWater() {
            foreach (MeshRenderer renderer in this.waterRenderList) {
                renderer.enabled = false;
            }
        }
    }
}
