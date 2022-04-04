using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace Poukoute {

    public enum PoolType {
        Battle,
        Tile,
        Normal
    }

    public struct PoolGameobject {
        public long timeStamp;
        public GameObject poolObject;
    }

    public struct PoolMember {
        public int maxChild;
        public Dictionary<int, PoolGameobject> poolDict;
    }

    public class MaterialPoolObj {
        public long timeStamp;
        public Material material;
    }

    public class AudioClipPoolObj {
        public long timeStamp;
        public AudioClip audioClicp;
    }

    public class SpritePoolObj {
        public long timeStamp;
        public Sprite sprite;
    }

    public class PoolManager : MonoBehaviour {
        private static PoolManager self;
        private static bool initialized = false;
        public static PoolManager Instance {
            get {
                if (!initialized) {
                    throw new POUninitializedException("PoolManager is not initialized.");
                } else if (self == null) {
                    throw new PONullException("PoolManager is destroyed.");
                }
                return self;
            }
        }

        //private Vector2 poolPosition;
        private Dictionary<PoolType, BasePool> poolDict =
            new Dictionary<PoolType, BasePool>(3);

        //private static Dictionary<string, int> preGenerateDict = new Dictionary<string, int>(10);

        private Dictionary<string, MaterialPoolObj> matPool =
            new Dictionary<string, MaterialPoolObj>(20);
        private Dictionary<string, SpritePoolObj> spritePool =
            new Dictionary<string, SpritePoolObj>(20);
        private Dictionary<string, AudioClipPoolObj> audioPool =
            new Dictionary<string, AudioClipPoolObj>(20);

        void Awake() {
            self = this;
            initialized = true;
            //this.poolPosition = Vector2.one * -10000;
#if UNITY_EDITOR
            InvokeRepeating("SortOutPool", 10, 10);
#else
            InvokeRepeating("SortOutPool", 300, 300);
#endif
        }

        //void Start() {
        //    this.Init();
        //}

        //private void Init() {
        //    this.InitSpritePool();
        //}

        //private void InitSpritePool() {
        //    //this.InitWaterMask();
        //}

#if UNITY_EDITOR
        public static void ConfigPoolManager(PoolManager poolManager) {
            self = poolManager;
            initialized = true;
        }
#endif

        private void InitWaterMask() {
            GameObject tileWater = GetObject(PrefabPath.tileWater, this.transform);
            Sprite maskSprite = tileWater.transform.Find("Mask").GetComponent<SpriteRenderer>().sprite;
            Rect rect = maskSprite.rect;
            Vector2[] sv = new Vector2[4];
            ushort[] triangles = new ushort[6];

            sv[0] = new Vector2(rect.width / 2, 0);
            sv[1] = new Vector2(rect.width, rect.height / 2);
            sv[2] = new Vector2(0, rect.height / 2);
            sv[3] = new Vector2(rect.width / 2, rect.height);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 1;
            triangles[4] = 2;
            triangles[5] = 3;

            maskSprite.OverrideGeometry(sv, triangles);
            RemoveObject(tileWater);
        }

        public static GameObject GetObject(string path, Transform root,
            string label = "", PoolType poolType = PoolType.Normal) {
            string[] pathArray = path.CustomSplit('/');
            string name = label.CustomIsEmpty() ? pathArray[pathArray.Length - 1] : label;
            BasePool pool = Instance.GetCurrentPool(poolType);
            return pool.GetObject(path, name, root);
        }

        public static T GetObject<T>(Transform root, string name = null) where T : Component {
            T behaviour;
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(root);
            if (string.IsNullOrEmpty(name)) {
                gameObject.name = typeof(T).Name;
            } else {
                gameObject.name = name;
            }
            behaviour = gameObject.AddComponent<T>();
            return behaviour;
        }

        public static void RemoveObject(GameObject poolObj,
            PoolType poolType = PoolType.Normal) {
            if (poolObj == null) {
                return;
            }
            AnimationManager.Stop(poolObj);
            BasePool pool = Instance.GetCurrentPool(poolType);
            pool.RemoveObject(poolObj);
        }

        public static Material GetMaterial(string path) {
            MaterialPoolObj materiPoolObj;
            if (!Instance.matPool.TryGetValue(path, out materiPoolObj)) {
                materiPoolObj = new MaterialPoolObj() {
                    timeStamp = RoleManager.GetCurrentUtcTime()
                };
                materiPoolObj.material = UnityEngine.Resources.Load<Material>(path);
                if (materiPoolObj.material == null) {
                    return null;
                }
                Instance.matPool.Add(path, materiPoolObj);
            }

            materiPoolObj.timeStamp = RoleManager.GetCurrentUtcTime();
            return materiPoolObj.material;
        }

        public static Sprite GetSprite(string path, bool needPool = true) {
            SpritePoolObj spritePoolObj;
            if (!Instance.spritePool.TryGetValue(path, out spritePoolObj)) {
                GameObject spriteObj = UnityEngine.Resources.Load<GameObject>(path);
                if (spriteObj == null) {
                    Debug.LogWarningf("There is no sprite of path of {0}", path);
                    return null;
                }
                spritePoolObj = new SpritePoolObj() {
                    timeStamp = RoleManager.GetCurrentUtcTime()
                };
                spritePoolObj.sprite = spriteObj.GetComponent<SpriteRenderer>().sprite;
                if (needPool) {
                    Instance.spritePool.Add(path, spritePoolObj);
                }
            }
            spritePoolObj.timeStamp = RoleManager.GetCurrentUtcTime();
            return spritePoolObj.sprite;
        }

        public static AudioClip GetAudio(string path) {
            AudioClipPoolObj audioClipPoolObj;
            if (!Instance.audioPool.TryGetValue(path, out audioClipPoolObj)) {
                audioClipPoolObj = new AudioClipPoolObj() {
                    timeStamp = RoleManager.GetCurrentUtcTime()
                };
                audioClipPoolObj.audioClicp = UnityEngine.Resources.Load<AudioClip>(path);
                if (audioClipPoolObj.audioClicp == null) {
                    Debug.LogWarningf("There is no audio of path of {0}", path);
                    return null;
                }
                Instance.audioPool.Add(path, audioClipPoolObj);
            }

            audioClipPoolObj.timeStamp = RoleManager.GetCurrentUtcTime();
            return audioClipPoolObj.audioClicp;
        }

        public static void ClearPool(PoolType poolType) {
            BasePool pool = Instance.GetCurrentPool(poolType);
            pool.Clear();
            GameHelper.GC();
        }

        public static bool CheckPool(string label, PoolType poolType = PoolType.Normal) {
            BasePool pool = Instance.GetCurrentPool(poolType);
            return pool.CheckMember(label);
        }

        private BasePool GetCurrentPool(PoolType poolType, string group = "") {
            BasePool pool;
            if (!this.poolDict.TryGetValue(poolType, out pool)) {
                if (poolType == PoolType.Tile) {
                    pool = new TilePool(poolType);
                } else {
                    pool = new BasePool(poolType);
                }
                this.poolDict.Add(poolType, pool);
            }
            return pool;
        }

        private long TIME_OFFSET_NEED_REMOVE = 180000; // Million Seconds
        private void SortOutPool() {
            long currentTime = RoleManager.GetCurrentUtcTime();
            //Debug.LogError("SortOutPool ");
            foreach (BasePool pool in this.poolDict.Values) {
                pool.SortOut(currentTime, TIME_OFFSET_NEED_REMOVE);
            }
            GameHelper.GC();
        }
    }
}
