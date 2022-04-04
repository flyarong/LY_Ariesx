using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class TilePool : BasePool {
        private Transform tileRoot;

        public TilePool(PoolType type):base(type) {
            this.tileRoot = GameObject.FindGameObjectWithTag("TileRoot").transform;
        }

        public override GameObject GetObject(string path, string name, Transform root) {
            PoolMember poolMember;
            if (this.memberDict.TryGetValue(name, out poolMember)) {
                if (poolMember.poolDict.Count > 0) {
                    var poolDictEnum = poolMember.poolDict.GetEnumerator();
                    poolDictEnum.MoveNext();
                    GameObject tmpPoolObj = poolDictEnum.Current.Value.poolObject;
                    poolMember.poolDict.Remove(poolDictEnum.Current.Key);
                    if (tmpPoolObj != null) {
                        this.SetOutPoolObj(tmpPoolObj, root);
                        return tmpPoolObj;
                    }
                    else {
                        Debug.LogError("tmpPoolObj is null, check if is destroyed " + tmpPoolObj.name);
                    }
                }
            } else {
                poolMember = new PoolMember {
                    maxChild = this.GetChildMaxCount(name), // To do : Find the best number.
                    poolDict = new Dictionary<int, PoolGameobject>(20)
                };
                this.memberDict.Add(name, poolMember);
            }

            GameObject poolPrefab = UnityEngine.Resources.Load<GameObject>(path);
            if (poolPrefab == null) {
                Debug.LogWarningf("There is no such prefab with path: {0}", path);
                return null;
            }
            GameObject poolObj = GameObject.Instantiate(poolPrefab, this.tileRoot);
            if (poolObj != null) {
                poolObj.name = name;
                this.SetOutPoolObj(poolObj, root);
            }
            return poolObj;
        }

        public override void RemoveObject(GameObject gameObject) {
            PoolMember poolMember;
            if (this.memberDict.TryGetValue(gameObject.name, out poolMember)) {
                int gameObjectId = gameObject.GetInstanceID();
                if (!poolMember.poolDict.ContainsKey(gameObjectId)) {
                    if (poolMember.poolDict.Count < poolMember.maxChild) {
                        PoolGameobject poolObject = new PoolGameobject() {
                            timeStamp = RoleManager.GetCurrentUtcTime(),
                            poolObject = gameObject
                        };
                        poolMember.poolDict.Add(gameObjectId, poolObject);
                        this.SetInPoolObj(gameObject, PoolType.Tile);
                    } else {
                        GameObject.Destroy(gameObject);
                    }
                }
            } else {
                Debug.LogError("Tile pool error: " + gameObject.name);
            }
        }

        protected override void SetOutPoolObj(GameObject poolObj, Transform root) {
            poolObj.transform.localPosition = Vector3.zero;
            poolObj.transform.localScale = Vector3.one;
            poolObj.transform.localEulerAngles = Vector3.zero;
            IPoolHandler handler = poolObj.GetComponent<IPoolHandler>();
            if (handler != null) {
                handler.OnOutPool();
            }
        }

        protected override void SetInPoolObj(GameObject poolObj, PoolType poolType) {
            poolObj.transform.localPosition = Vector3.one * -10000;
            IPoolHandler handler = poolObj.GetComponent<IPoolHandler>();
            if (handler != null) {
                handler.OnInPool();
            }
        }
    }
}
