using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Protocol;
using System;
using System.Text;
using TMPro;

namespace Poukoute {
    public delegate void GameAction();

    public enum TimeUnit {
        day,
        hour,
        minute,
        second
    }

    public class GameHelper {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        private static readonly Regex sFormatChar = new Regex(@"\t|\n|\r");

        public static RandomGenerator randomGenerator = new RandomGenerator();
        public static Dictionary<TimeUnit, int> timeUnitDict = new Dictionary<TimeUnit, int>();
        public static Vector2 anchoredInitPos = Vector2.zero;
        private static Transform[] removeList;

        public static string ReplaceWhitespace(string input, string replacement) {
            return sWhitespace.Replace(input, replacement);
        }

        public static void RemoveFormatChar(ref string s) {
            s = sFormatChar.Replace(s, string.Empty);
        }

        //public static string GetNumber(int num) {
        //    if (num == 0) {
        //        return string.Empty;
        //    } else if (num.Abs() / num > 0) {
        //        return "+";
        //    } else {
        //        return "-";
        //    }
        //}

        public static string GetNumSignColor(int num) {
            if (num == 0) {
                return string.Empty;
            } else if (num.Abs() / num > 0) {
                return "<color='red'>+" + GetFormatNum(num.Abs()) + "</color>";
            } else {
                return "<color='green'>-" + GetFormatNum(num.Abs()) + "</color>";
            }
        }

        public static string GetFormatNum(long num, double unit = 1000000000000,
            int maxLength = 5, int maxNumber = 3, int decLength = 2) {
            if (num < Mathf.Pow(10, maxLength - 1)) {
                return num.ToString();
            }
            string unitStr = string.Empty;
            switch ((long)unit) {
                case 1000000000000:
                    unitStr = "T";
                    break;
                case 1000000000:
                    unitStr = "B";
                    break;
                case 1000000:
                    unitStr = "M";
                    break;
                case 1000:
                    unitStr = "K";
                    break;
                default:
                    break;
            }

            long integer = (long)(num / unit);

            double dec = num / unit - integer;
            if (integer >= Mathf.Pow(10, maxNumber)) {
                return integer.ToString() + unitStr;
            } else if (integer >= 1) {
                if (decLength > 0) {
                    int interLength = (int)Mathf.Log10(integer) + 1;
                    int offset = maxLength - (interLength + unitStr.Length + decLength);
                    decLength = offset < 0 ? decLength + offset : decLength;
                    decLength = Mathf.Max(decLength, 0);
                    double decParse = double.Parse(dec.ToString("f" + decLength));
                    return (integer + decParse).ToString() + unitStr;
                } else {
                    return integer.ToString() + unitStr;
                }
            } else {
                return GetFormatNum(num, unit / 1000, maxLength, maxNumber, decLength);
            }
        }

        public static void SetTextVisible(Transform root, bool visible) {
            root.GetComponent<TextMeshPro>().enabled = visible;
            if (root.childCount > 0) {
                foreach (Transform child in root) {
                    SetTextSubVisible(child, visible);
                }
            }
        }

        public static void SetTextSubVisible(Transform root, bool visible) {
            root.GetComponent<TMP_SubMesh>().enabled = visible;
        }

        public static void ClearChildren(Transform root, bool needPool = true,
            PoolType poolType = PoolType.Normal) {
            if (root.childCount < 1) {
                return;
            }
            int childCount = root.childCount;
            Array.Resize(ref removeList, childCount);
            for (int i = 0; i < childCount; i++) {
                removeList[i] = root.GetChild(i);
            }
            for (int i = 0; i < childCount; i++) {
                if (needPool) {
                    PoolManager.RemoveObject(removeList[i].gameObject, poolType);
                } else {
#if UNITY_EDITOR
                    GameObject.DestroyImmediate(removeList[i].gameObject);
#else
                    Debug.LogError("GameHelper Destroy gameobject " + removeList[i].gameObject.name);
                    GameObject.Destroy(removeList[i].gameObject);
#endif
                }
            }
        }

        public static Transform GetTransformByName(Transform root, string name) {
            if (root.name == name) {
                return root;
            } else {
                foreach (Transform child in root) {
                    Transform childRoot = GetTransformByName(child, name);
                    if (childRoot != null) {
                        return childRoot;
                    }
                }
                return null;
            }
        }

        public static void ResizeChildreCount(
                            Transform root,
                            int newChildrenCount,
                            string childPath,
                            bool needPool = true) {
            int childCount = root.childCount;
            if (newChildrenCount == childCount) {
                return;
            }
            if (newChildrenCount < childCount) {
                int overflowCount = childCount - newChildrenCount;
                for (int i = 0; i < overflowCount; i++) {
                    PoolManager.RemoveObject(root.GetChild(0).gameObject);
                }
            } else {
                int newCreateCount = newChildrenCount - childCount;
                for (int i = 0; i < newCreateCount; i++) {
                    PoolManager.GetObject(childPath, root);
                }
            }
        }

        public static void SetResourcesListInfo(
            Dictionary<Resource, int> resourcesDict,
            Dictionary<Resource, Transform> resourceTransDict,
            Transform root, string childPath) {
            resourceTransDict.Clear();
            int resourcesCount = resourcesDict.Count;
            ResizeChildreCount(root, resourcesCount, childPath);
            ItemWithCountView itemView = null;
            int index = 0;
            foreach (var pair in resourcesDict) {
                itemView = root.GetChild(index++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(pair.Key, pair.Value);
                resourceTransDict.Add(pair.Key, itemView.imgItem.transform);
            }

        }

        public static int GetVisibleChildreCount(Transform root) {
            int count = 0;
            foreach (Transform tranform in root) {
                if (tranform.gameObject.activeSelf) {
                    count++;
                }
            }
            return count;
        }

        public static bool HasActiveChild(Transform root) {
            foreach (Transform transform in root) {
                if (transform.gameObject.activeSelf) {
                    return true;
                }
            }
            return false;
        }

        public static void GetAllChildren(Transform root, List<Transform> children) {
            foreach (Transform child in root) {
                children.Add(child);
                if (child.childCount != 0) {
                    GetAllChildren(child, children);
                }
            }
        }

        public static int GetTheMaxValue(Dictionary<string, int> dict) {
            int max = 0;
            foreach (int value in dict.Values) {
                if (value > max) {
                    max = value;
                }
            }
            return max;
        }

        public static void ResetScrollContent(Transform child, Transform parent, string direction = "up") {
            RectTransform rectChild = child.GetComponent<RectTransform>();
            RectTransform rectParent = parent.GetComponent<RectTransform>();
            Vector2 offset = Vector2.zero;
            Vector2 directionOffset = Vector2.zero;
            if (direction.CustomEquals("down")) {
                directionOffset = new Vector2(0, -rectParent.sizeDelta.y + rectChild.sizeDelta.y);
            }

            if (rectChild.sizeDelta.y <= rectParent.sizeDelta.y) {
                offset += new Vector2(rectChild.sizeDelta.x / 2, -rectParent.sizeDelta.y / 2);
                parent.GetComponent<CustomScrollRect>().enabled = false;
            } else {
                offset += new Vector2(rectChild.sizeDelta.x / 2, -rectChild.sizeDelta.y / 2);
                offset += directionOffset;
                parent.GetComponent<CustomScrollRect>().enabled = true;
            }
            rectChild.anchoredPosition = offset;
        }

        public static string DateFormat(long timeStamp, string format = "yyyy-MM-dd HH:mm:ss") {
            System.DateTime date = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timeStamp).ToLocalTime();
            return date.ToString(format, System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        public static string HistoryTimeFormat(long timeStamp) {
            long timeOffset = RoleManager.GetCurrentUtcTime() / 1000 - timeStamp;
            System.DateTime date = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timeStamp).ToLocalTime();
            System.DateTime dateOffset =
                new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timeOffset).ToLocalTime();
            if (dateOffset.Month > 1) {
                return string.Format(LocalManager.GetValue(LocalHashConst.time_month_ago), dateOffset.Month - 1);
            } else if (dateOffset.Day > 2) {
                return string.Format(LocalManager.GetValue(LocalHashConst.time_days_ago), dateOffset.Day - 1);
            } else if (dateOffset.Day == 2) {
                return LocalManager.GetValue(LocalHashConst.time_yesterday);
            } else {
                return date.ToString("t", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStampSeconds) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStampSeconds).ToLocalTime();
            return dtDateTime;
        }

        // To do: Time format, xxhxxmxxs.
        public static string TimeFormat(long milliseconds) {
            System.TimeSpan timeSpan = System.TimeSpan.FromMilliseconds(milliseconds);
            timeUnitDict[TimeUnit.day] = timeSpan.Days;
            timeUnitDict[TimeUnit.hour] = timeSpan.Hours;
            timeUnitDict[TimeUnit.minute] = timeSpan.Minutes;
            timeUnitDict[TimeUnit.second] = timeSpan.Seconds +
                Mathf.RoundToInt(timeSpan.Milliseconds / 1000f);

            int totalHours = timeUnitDict[TimeUnit.day] * 24 + timeUnitDict[TimeUnit.hour];
            if (totalHours < 72) {
                timeUnitDict[TimeUnit.hour] += timeUnitDict[TimeUnit.day] * 24;
                timeUnitDict[TimeUnit.day] = 0;
            }
            string timeStr = string.Empty;
            for (int i = 0; i < 4; i++) {
                if (timeUnitDict[(TimeUnit)i] > 0) {
                    timeStr = string.Concat(
                        timeUnitDict[(TimeUnit)i],
                        LocalManager.GetValue("time_", ((TimeUnit)i).ToString()));
                    if (i < 3 && timeUnitDict[(TimeUnit)(i + 1)] > 0) {
                        timeStr = string.Concat(timeStr,
                                timeUnitDict[(TimeUnit)(i + 1)],
                                LocalManager.GetValue("time_", ((TimeUnit)(i + 1)).ToString())
                                );
                    }
                    return timeStr;
                }
            }
            return "0" + LocalManager.GetValue(LocalHashConst.time_second);
        }

        public static string TimeFormatWithOneChar(long milliseconds) {
            System.TimeSpan timeSpan = System.TimeSpan.FromMilliseconds(milliseconds);
            timeUnitDict[TimeUnit.day] = timeSpan.Days;
            timeUnitDict[TimeUnit.hour] = timeSpan.Hours;
            timeUnitDict[TimeUnit.minute] = timeSpan.Minutes;
            timeUnitDict[TimeUnit.second] = timeSpan.Seconds;

            int totalHours = timeUnitDict[TimeUnit.day] * 24 + timeUnitDict[TimeUnit.hour];
            if (totalHours < 72) {
                timeUnitDict[TimeUnit.hour] += timeUnitDict[TimeUnit.day] * 24;
                timeUnitDict[TimeUnit.day] = 0;
            }
            string timeStr = string.Empty;
            for (int i = 0; i < 4; i++) {
                if (timeUnitDict[(TimeUnit)i] > 0) {
                    timeStr = string.Concat(
                        timeUnitDict[(TimeUnit)i],
                        LocalManager.GetValue("time_", ((TimeUnit)i).ToString()));
                    //if (i < 3 && timeUnitDict[(TimeUnit)(i + 1)] > 0) {
                    //    timeStr = string.Concat(//timeStr,
                    //            timeUnitDict[(TimeUnit)(i + 1)],
                    //            LocalManager.GetValue("time_", ((TimeUnit)(i + 1)).ToString())
                    //            );
                    //}
                    return timeStr;
                }
            }
            return "0" + LocalManager.GetValue(LocalHashConst.time_second);
        }

        public static string GetToEarlyMorningTimeInterval() {
            int h = DateTime.Now.Hour;
            int m = DateTime.Now.Minute;
            return string.Concat(
                        23 - h, LocalManager.GetValue(LocalHashConst.time_hour),
                        59 - m, LocalManager.GetValue(LocalHashConst.time_minute));
        }

        public static string GetToEarlyMorningTimePart() {
            int leftTime = 23 - DateTime.Now.Hour;
            if (leftTime > 0) {
                return string.Concat(leftTime, LocalManager.GetValue(LocalHashConst.time_hour));
            } else {
                leftTime = 59 - DateTime.Now.Minute;
                if (leftTime > 0) {
                    return string.Concat(leftTime, LocalManager.GetValue(LocalHashConst.time_minute));
                } else {
                    leftTime = 59 - DateTime.Now.Second;
                    return string.Concat(leftTime, LocalManager.GetValue(LocalHashConst.time_second));
                }
            }
        }

        public static string GetToEarlyMorningUTCTimePart() {
            return GameHelper.TimeFormatWithOneChar((RoleManager.GetNextZeroTime() - RoleManager.GetCurrentUtcTime()));
        }
        public static float MillisecondToMinute(long millisecond) {
            return millisecond / 1000f / 60f;
        }

        public static float MillisecondToSecond(long millisecond) {
            return millisecond / 1000f;
        }

        public static float GetSymbol(float value) {
            if (value == 0.0f) {
                return 0.0f;
            } else {
                return value.Abs() / value;
            }
        }

        public static string UpperFirstCase(string str) {
            return str.ToUpper()[0] + str.Substring(1);
        }

        public static string LowerFirstCase(string str) {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public class RandomPoint {
            public int position;
            public List<int> pointList = new List<int>();
            public List<int> avaliableList = new List<int>();
        }

        public static List<Vector2> RandomSquare(Vector2 cellSize, Vector2 totalSize, int amount) {
            return randomGenerator.RandomSquare(cellSize, totalSize, amount);
        }

        public static void SetLayer(GameObject root, int layer) {
            root.layer = layer;
            foreach (Transform child in root.transform) {
                SetLayer(child.gameObject, layer);
            }
        }

        public static void SetSortingLayerID(GameObject root, string layerName) {
            int layerId = SortingLayer.NameToID(layerName);
            SetSortingLayerID(root, layerId);
        }

        public static void SetSortingLayerID(GameObject root, int layerId) {
            Renderer render = root.GetComponent<Renderer>();
            if (render != null) {
                render.sortingLayerID = layerId;
            }
            foreach (Transform child in root.transform) {
                SetSortingLayerID(child.gameObject, layerId);
            }
        }

        public static void CopyRectTransform(RectTransform source, RectTransform target) {
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.offsetMin = source.offsetMin;
            target.offsetMax = source.offsetMax;
            LayoutElement sourceLayout = source.GetComponent<LayoutElement>();
            if (sourceLayout != null) {
                LayoutElement targetLayout = target.GetComponent<LayoutElement>();
                targetLayout.minWidth = sourceLayout.minWidth;
                targetLayout.minHeight = sourceLayout.minHeight;
                targetLayout.preferredWidth = sourceLayout.preferredWidth;
                targetLayout.preferredHeight = sourceLayout.preferredHeight;
                targetLayout.flexibleWidth = sourceLayout.flexibleWidth;
                targetLayout.flexibleHeight = sourceLayout.flexibleHeight;
            }
        }

        // color to hex
        public static string ToRGBHex(Color c) {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }

        // hex to color
        public static Color ToColor(long HexVal) {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);

            return new Color(R / 255f, G / 255f, B / 255f);
        }


        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void CommonRewardCollect(Protocol.Resources resources,
        Protocol.Currency currency, CommonReward commonReward,
        Vector2 position, bool isDropOut) {
            Protocol.Resources addResources = commonReward.Resources;
            Protocol.Currency addCurrency = commonReward.Currency;
            SimpleResourceCurrencyCollect(resources, currency, addResources, addCurrency,
                position, isDropOut);

            if (commonReward.Chests.Count > 0) {
                HeroModel.AddlotteryChances(commonReward.Chests);
            }
        }

        public static void SimpleResourceCurrencyCollect(Protocol.Resources resources, Protocol.Currency currency,
        Protocol.Resources addResources, Protocol.Currency addCurrency, Vector2 position, bool isPlayDroupOut = false) {
            RoleManager.Instance.NeedResourceAnimation = true;
            RoleManager.Instance.NeedCurrencyAnimation = true;
            RoleManager.SetResource(resources);
            RoleManager.SetCurrency(currency);
            AudioManager.Play(AudioPath.actPrefix + "get_reward", AudioType.Action, AudioVolumn.High);

            if (addResources.Crystal != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Crystal, addResources.Crystal, position, isPlayDroupOut);
            }
            if (addResources.Food != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Food, addResources.Food, position, isPlayDroupOut);
            }
            if (addResources.Lumber != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Lumber, addResources.Lumber, position, isPlayDroupOut);
            }
            if (addResources.Marble != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Marble, addResources.Marble, position, isPlayDroupOut);
            }
            if (addResources.Steel != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Steel, addResources.Steel, position, isPlayDroupOut);
            }
            if (addCurrency.Gold != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Gold, addCurrency.Gold, position, isPlayDroupOut);
            }
            if (addCurrency.Gem != 0) {
                TriggerManager.Invoke(Trigger.SimpleResourceCollect,
                    Resource.Gem, addCurrency.Gem, position, isPlayDroupOut);
            }

            TriggerManager.Invoke(Trigger.ResourceChange);
            TriggerManager.Invoke(Trigger.CurrencyChange);
        }

        public static void CollectCurrency(Currency addCurrency, Currency currency, Transform currencyPos, bool isPlayDroupOut = false) {
            RoleManager.Instance.NeedCurrencyAnimation = true;
            RoleManager.SetCurrency(currency);
            AudioManager.Play(AudioPath.actPrefix + "get_reward",
                AudioType.Action, AudioVolumn.High);
            if (addCurrency.Gold != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Gold, addCurrency.Gold,
                    currencyPos.position, isPlayDroupOut);
            }
            if (addCurrency.Gem != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Gem, addCurrency.Gem,
                    currencyPos.position, isPlayDroupOut);
            }
            TriggerManager.Invoke(Trigger.CurrencyChange);
        }

        public static void CollectResources(
            CommonReward reward,
            Protocol.Resources resources,
            Protocol.Currency currency,
            Dictionary<Resource, Transform> resourceDict,
            bool isPlayDroupOut = false
        ) {
            CollectResources(reward.Resources, reward.Currency, resources, currency, resourceDict, isPlayDroupOut);
            //HeroModel.AddlotteryChances(reward.Chests);
        }

        public static void CollectResources(Protocol.Resources addResources,
                                        Protocol.Currency addCurrency,
                                        Protocol.Resources resources,
                                        Protocol.Currency currency,
                                        Dictionary<Resource, Transform> resourceDict,
                                        bool isPlayDroupOut = false, bool isCollect = false) {
            RoleManager.Instance.NeedResourceAnimation = true;
            RoleManager.Instance.NeedCurrencyAnimation = true;
            RoleManager.SetResource(resources);
            RoleManager.SetCurrency(currency);
            AudioManager.Play(AudioPath.actPrefix + "get_reward",
                AudioType.Action, AudioVolumn.High);
            if (addResources.Food != 0) {
                TriggerManager.Invoke(
                    Trigger.ResourceCollect,
                    Resource.Food, addResources.Food,
                    resourceDict[Resource.Food].position, isPlayDroupOut
                );
            }
            if (addResources.Lumber != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                Resource.Lumber, addResources.Lumber,
                resourceDict[Resource.Lumber].position, isPlayDroupOut);
            }
            if (addResources.Marble != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Marble, addResources.Marble,
                    resourceDict[Resource.Marble].position, isPlayDroupOut);
            }
            if (addResources.Steel != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Steel, addResources.Steel,
                    resourceDict[Resource.Steel].position, isPlayDroupOut);
            }
            if (addCurrency.Gold != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Gold, addCurrency.Gold,
                    resourceDict[Resource.Gold].position, isPlayDroupOut);
            }
            if (addCurrency.Gem != 0) {
                TriggerManager.Invoke(Trigger.ResourceCollect,
                Resource.Gem, addCurrency.Gem,
                resourceDict[Resource.Gem].position, isPlayDroupOut);
            }
            TriggerManager.Invoke(Trigger.ResourceChange);
            TriggerManager.Invoke(Trigger.CurrencyChange);
        }

        public static void ForceCollect(Vector3 origin, bool isPlayDroupOut) {
            TriggerManager.Invoke(Trigger.ResourceCollect,
                    Resource.Force, 1,
                    origin, isPlayDroupOut);
        }

        public static void ChestCollect(Vector3 startAndTargetPos,
            GachaGroupConf gachaGroupConf,
            CollectChestType type = CollectChestType.normalCollect,
            UnityAction AfterShowCallback = null) {
            TriggerManager.Invoke(
                Trigger.ChestCollect, startAndTargetPos,
                gachaGroupConf, type, AfterShowCallback
            );
        }

        public static void SetCameraLayer(LayerMask layerMask, bool isVisible) {
            if (isVisible) {
                GameManager.MainCamera.cullingMask |= layerMask.value;
            } else {
                GameManager.MainCamera.cullingMask &= ~layerMask.value;
            }
        }

        public static bool IsAnimatorContainsParam(Animator animator, string param) {
            foreach (AnimatorControllerParameter controller in animator.parameters) {
                if (controller.name == param)
                    return true;
            }
            return false;
        }

        public static string GetLevelLocal(int level, bool levelColor = false) {
            string levelStr = string.Format(LocalManager.GetValue(LocalHashConst.level), level);
            if (levelColor) {
                levelStr = string.Format("<color=#fffc9d>{0}</color>", levelStr);
            }
            return levelStr;
        }

        public static string GetNameAndLevel(string name, int level, bool levelColor = false) {
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("{0}  {1}",
                name, GameHelper.GetLevelLocal(level, levelColor)).ToString();
        }

        public static bool NearlyEqual(double a, double b, double epsilon = 0.00000001) {
            double absA = a.Abs();
            double absB = b.Abs();
            double diff = (a - b).Abs();

            if (a == b) { // shortcut, handles infinities
                return true;
            } else if (a == 0 || b == 0 || diff < Double.Epsilon) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            } else { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static int GetBuildIndex(string key) {
            if (key.CustomIsEmpty()) {
                Debug.LogWarning("Key is empty.");
                return 0;
            }
            string[] keyArray = key.CustomSplit('_');
            return (int)((char)keyArray[keyArray.Length - 1][0]) - (int)('A') + 1;
        }

        public static void SetCanvasCamera(Canvas canvas, Camera camera = null) {
            if (camera == null) {
                camera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            }
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
        }

        public static T GetFirstComponent<T>(Transform root) {
            T component = root.GetComponent<T>();
            if (component.ToString() == "null") {
                foreach (Transform child in root) {
                    component = GetFirstComponent<T>(child);
                    if (component.ToString() != "null") {
                        break;
                    }
                }
            }
            return component;
        }


        public static void ForceLayout(LayoutGroup layoutGroup) {
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        public static void GC() {
            //Debug.LogError("GC callded");
            UnityEngine.Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static string ToLowercaseNamingConvention(string s) {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(s, "_").ToLower();
        }
    }
}