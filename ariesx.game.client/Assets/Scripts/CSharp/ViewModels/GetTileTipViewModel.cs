using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class GetTileTipViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        //private BuildModel buildModel;
        private GetTileTipView view;

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            //this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<GetTileTipView>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.PlayShow(() => {
                    this.parent.OnAddViewAboveMap(this, AddOnMap.HideAll);
                }, true);
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide(() => {
                    this.parent.OnRemoveViewAboveMap(this);
                });
            }
        }

        public void HideImmediatly() {
            if (this.view.IsVisible) {
                this.view.HideImmediatly(() => this.parent.OnRemoveViewAboveMap(this));
            }
        }

        public void GoTileReq() {
            this.Hide();
            GetCanAddForceCoordReq req = new Protocol.GetCanAddForceCoordReq() { };
            NetManager.SendMessage(req, typeof(GetCanAddForceCoordAck).Name, this.GetTileAck);
        }

        public void GetTileAck(IExtensible message) {
            GetCanAddForceCoordAck ack = message as GetCanAddForceCoordAck;
            this.parent.MoveWithClick(new Vector2(ack.Coord.X, ack.Coord.Y));
        }
        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
