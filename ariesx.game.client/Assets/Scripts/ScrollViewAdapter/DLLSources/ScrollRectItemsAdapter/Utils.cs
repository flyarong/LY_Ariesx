using frame8.Logic.Misc.Other.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter {
    public static class Utils {
        /// <summary> This is needed because the PointerEventData received in OnDrag, OnEndDrag etc. is a copy of the original one </summary>
        public static PointerEventData GetOriginalPointerEventDataWithPointerDragGO(GameObject pointerDragGOToLookFor) {
            // Current input module not initialized yet
            if (EventSystem.current.currentInputModule == null)
                return null;

            var eventSystemAsPointerInputModule = EventSystem.current.currentInputModule as PointerInputModule;
            if (eventSystemAsPointerInputModule == null)
                throw new InvalidOperationException("currentInputModule is not a PointerInputModule");

            var asCompatInterface = eventSystemAsPointerInputModule as ISRIAPointerInputModule;
            Dictionary<int, PointerEventData> pointerEvents;
            if (asCompatInterface == null) {
#if UNITY_WSA || UNITY_WSA_10_0 // WSA uses .net core, which doesn't have reflection. in this case we expect the current input module to implement ISRIAPointerInputModule
				throw new UnityException("Your InputModule should extend ISRIAPointerInputModule. See Instructions.pdf");
#else
                // Dig into reflection and get the original pointer data
                pointerEvents = eventSystemAsPointerInputModule
                    .GetType()
                    .GetField("m_PointerData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(eventSystemAsPointerInputModule)
                    as Dictionary<int, PointerEventData>;
#endif
            } else {
                pointerEvents = asCompatInterface.GetPointerEventData();
            }

            foreach (var pointer in pointerEvents.Values) {
                if (pointer.pointerDrag == pointerDragGOToLookFor) {
                    return pointer;
                }
            }

            return null;
        }



        public static Vector2? ForceSetPointerEventDistanceToZero(PointerEventData pev) {
            var delta = pev.delta;
            pev.dragging = false;
            return delta;
        }
    }
}
