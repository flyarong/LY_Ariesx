using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public static class UIExtension {
        public static void TryRemove<T>(this List<T> list, T member) {
            if (list.Contains(member)) {
                list.Remove(member);
            }
        }

        public static void TryAdd<T>(this List<T> list, T member) {
            if (!list.Contains(member)) {
                list.Add(member);
            }
        }

        public static void TryRemove<T>(this List<T> list, List<T> removePart) {
            if (removePart.Count < 1) {
                return;
            }
            foreach(T member in removePart) {
                list.TryRemove(member);
            }
        }

        public static void TryAdd<T>(this List<T> list, List<T> addPart) {
            if (addPart.Count < 1) {
                return;
            }
            foreach (T member in addPart) {
                list.TryAdd(member);
            }
        }

        public static void TryRemove<T,P>(this Dictionary<T, P> list, T key) {
            if (list.ContainsKey(key)) {
                list.Remove(key);
            }
        }
    }
}
