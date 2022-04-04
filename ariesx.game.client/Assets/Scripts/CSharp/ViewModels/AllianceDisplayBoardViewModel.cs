using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceDisplayBoardViewModel: BaseViewModel, IViewModel {
        //private MapViewModel parent;
        private AllianceDisplayBoardView view;
        
        void Awake() {
            //this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<AllianceDisplayBoardView>();
        }

        public void Show(DisplayType tape) {
            this.view.PlayShow();
            this.view.SetDisplayBoradContent(tape);
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

    }
}
