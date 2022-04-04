using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems {

    public class PinchEventData : BaseEventData {

        public List<PointerEventData> data;
        public float scrollAxis;

        public PinchEventData(EventSystem ES) : base(ES) {
            data = new List<PointerEventData>();
        }
    }
    
    public interface IPinchHandler : IEventSystemHandler {
        void OnPinch(PinchEventData data);
    }

    public interface IBeginPinchHandler: IEventSystemHandler {
        void OnBeginPinch(PinchEventData data);
    }

    public interface IEndPinchHandler : IEventSystemHandler {
        void OnEndPinch(PinchEventData data);
    }

    public static class PinchModuleEvents {
        private static void Execute(IPinchHandler handler, BaseEventData eventData) {
            handler.OnPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        private static void Execute(IBeginPinchHandler handler, BaseEventData eventData) {
            handler.OnBeginPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        private static void Execute(IEndPinchHandler handler, BaseEventData eventData) {
            handler.OnEndPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        public static ExecuteEvents.EventFunction<IPinchHandler> pinchHandler {
            get {
                return Execute;
            }
        }

        public static ExecuteEvents.EventFunction<IBeginPinchHandler> beginPinchHandler {
            get {
                return Execute;
            }
        }

        public static ExecuteEvents.EventFunction<IEndPinchHandler> endPinchHandler {
            get {
                return Execute;
            }
        }
    }

    [AddComponentMenu("Event/Pinch Input Module")]
    public class PinchInputModule : StandaloneInputModule {
        private PinchEventData _pinchData = null;
        private bool isPinching;
        private GameObject oldGameObject;

        new void Awake() {
            this.m_InputOverride = this.GetComponent<CustomInput>();
        }

        public override void Process() {
            if (Input.GetAxis("Mouse ScrollWheel") != 0.0f) {
                _pinchData = new PinchEventData(eventSystem);
                _pinchData.scrollAxis = Input.GetAxis("Mouse ScrollWheel");
                
                ExecuteEvents.Execute(GameObject.Find("Map"), _pinchData, PinchModuleEvents.pinchHandler);
            }
            bool isPinchingHandled = false;
            if (Input.touchCount >= 2) {
                bool pressed0, released0;
                bool pressed1, released1;

                PointerEventData touchData0 = GetTouchPointerEventData(Input.GetTouch(0), out pressed0, out released0);
                PointerEventData touchData1 = GetTouchPointerEventData(Input.GetTouch(1), out pressed1, out released1);
                eventSystem.RaycastAll(touchData0, m_RaycastResultCache);
                RaycastResult firstHit = FindFirstRaycast(m_RaycastResultCache);
                
                eventSystem.RaycastAll(touchData1, m_RaycastResultCache);

                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) {
                    if (firstHit.gameObject != null && FindFirstRaycast(m_RaycastResultCache).gameObject != null) {
                        if (FindFirstRaycast(m_RaycastResultCache).gameObject.Equals(firstHit.gameObject)) {

                            _pinchData = new PinchEventData(eventSystem);

                            _pinchData.data.Add(touchData0);
                            _pinchData.data.Add(touchData1);

                            if (this.isPinching) {
                                isPinchingHandled = ExecuteEvents.Execute(firstHit.gameObject, _pinchData, PinchModuleEvents.pinchHandler);
                            } else {
                                this.isPinching = true;
                                this.oldGameObject = firstHit.gameObject;
                                isPinchingHandled = ExecuteEvents.Execute(firstHit.gameObject, _pinchData, PinchModuleEvents.beginPinchHandler);
                            }
                        }
                    }
                }
                if (_pinchData != null) {
                    _pinchData.data.Clear();
                }
            } else {
                if (this.isPinching) {
                    isPinchingHandled = ExecuteEvents.Execute(this.oldGameObject, _pinchData, PinchModuleEvents.endPinchHandler);
                    this.isPinching = false;
                }
            }
            if (!isPinchingHandled) {
                base.Process();
            }

        }

        public override string ToString() {
            return string.Format("[PinchInputModule]");
        }
    }
}
