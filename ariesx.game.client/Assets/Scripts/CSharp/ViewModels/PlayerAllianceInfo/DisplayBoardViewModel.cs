using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class DisplayBoardViewModel : BaseViewModel, IViewModel {
        //private MapViewModel parent;
        private DisplayBoardView view;

        void Awake() {
            //this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<DisplayBoardView>();
        }

        public void Show(string title, string content) {
            this.view.PlayShow();
            this.view.SetDisplayBoradContent(title, content);
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

    }
}
