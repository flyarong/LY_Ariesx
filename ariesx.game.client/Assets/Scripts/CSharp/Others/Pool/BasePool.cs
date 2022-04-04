using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class BasePool {
        public PoolType type;
        public static Vector3 poolPosition = Vector3.one * -10000;
        public Dictionary<string, PoolMember> memberDict =
            new Dictionary<string, PoolMember>();
        public Transform poolRoot;

        public BasePool(PoolType type) {
            this.poolRoot = (new GameObject()).transform;
            this.poolRoot.name = type.ToString();
            this.poolRoot.SetParent(PoolManager.Instance.transform);
        }

        protected int GetChildMaxCount(string name) {
            if (name.CustomStartsWith("tileLevel")) {
                return 65;
            }

            if (name.CustomStartsWith("tile_layer") ||
                name.CustomStartsWith("Tile") ||
                name.CustomStartsWith("Relation")) {
                return 20;
            }

            return 10;
        }

        public virtual GameObject GetObject(string path, string name, Transform root) {
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
                    } else {
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
            GameObject poolObj = GameObject.Instantiate(poolPrefab);
            if (poolObj != null) {
                poolObj.name = name;
                this.SetOutPoolObj(poolObj, root);
            }
            return poolObj;
        }

        public virtual GameObject GetObject(string path, int name, Transform root) {
            return this.GetObject(path, name.ToString(), root);
        }

        public virtual void RemoveObject(GameObject gameObject) {
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
                        this.SetInPoolObj(gameObject);
                    } else {
                        // Debug.LogError("Destroy gameobject " + gameObject.name + " " + poolMember.maxChild);
                        GameObject.Destroy(gameObject);
                    }
                }
            } else {
                GameObject.Destroy(gameObject);
            }
        }

        public void SortOut(long currentTime, long timeOffset) {
            Dictionary<int, PoolGameobject> tmpPoolDict = new Dictionary<int, PoolGameobject>(20);
            foreach (PoolMember poolMember in this.memberDict.Values) {
                foreach (var child in poolMember.poolDict) {
                    if ((currentTime - child.Value.timeStamp) > timeOffset) {
                        tmpPoolDict.Add(child.Key, child.Value);
                    }
                }

                foreach (var child in tmpPoolDict) {
                    //Debug.LogError("SortOut Destroy gameobject " + child.Value.poolObject.name);
                    poolMember.poolDict.Remove(child.Key);
                    GameObject.Destroy(child.Value.poolObject);
                }
            }
        }

        public virtual void Clear() {
            foreach (PoolMember poolMember in this.memberDict.Values) {
                foreach (var child in poolMember.poolDict) {
                    GameObject.Destroy(child.Value.poolObject);
                }
            }
            memberDict.Clear();
        }

        public virtual bool CheckMember(string key) {
            return this.memberDict.ContainsKey(key) && this.memberDict[key].poolDict.Count > 0;
        }

        private IPoolHandler handler = null;
        protected virtual void SetOutPoolObj(GameObject poolObj, Transform root) {
            if (root != poolObj.transform.parent) {
                poolObj.transform.SetParent(root);
            }
            poolObj.transform.localPosition = Vector3.zero;
            poolObj.transform.localScale = Vector3.one;
            poolObj.transform.localEulerAngles = Vector3.zero;
            poolObj.SetActiveSafe(true);
            this.handler = poolObj.GetComponent<IPoolHandler>();
            if (this.handler != null) {
                this.handler.OnOutPool();
            }
        }

        protected virtual void SetInPoolObj(GameObject poolObj,
            PoolType poolType = PoolType.Normal) {
            poolObj.transform.SetParent(this.poolRoot);
            poolObj.transform.localPosition = poolPosition;
            poolObj.transform.localScale = Vector3.zero;
            this.handler = poolObj.GetComponent<IPoolHandler>();
            if (this.handler != null) {
                this.handler.OnInPool();
            }
        }
    }
}
