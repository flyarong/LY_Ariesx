using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class MiniMapPassViewModel : BaseViewModel {
        private MiniMapViewModel parent;
        private MiniMapPassView view;
        /* Model data get set */
        public int State {
            get {
                return this.parent.State;
            }
        }

        public List<MiniMapPassConf> PassList {
            get {
                return MiniMapPassConf.GetPassIn(this.State);
            }
        }

        public Vector2 CurrentCoord {
            get; set;
        }
        /**********************/

        /* Other members */
        public bool IsSelectItem { get; set; }

        private bool NeedRefresh { get; set; }
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<MiniMapPassView>();
            this.parent = this.transform.parent.GetComponent<MiniMapViewModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            this.parent.resetChoseAction = this.view.ResetItemIsChosen;
            if (this.NeedRefresh) {
                this.view.ResetItems(this.PassList.Count);
                this.NeedRefresh = false;
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void MoveTo(Vector2 coordinate, string local) {
            this.parent.MoveTo(coordinate, local);
        }

        public void OnStateChange() {
            if (this.view.IsVisible) {
                this.view.ResetItems(this.PassList.Count);
                this.PassStatusReq();
            } else {
                this.NeedRefresh = true;
            }
        }

        private void PassStatusReq() {
            GetFallenInfoReq cityStatusReq = new GetFallenInfoReq();
            foreach (MiniMapPassConf pass in this.PassList) {
                Coord coord = new Coord(Mathf.RoundToInt(pass.coordinate.x),
                                        Mathf.RoundToInt(pass.coordinate.y));
                cityStatusReq.Coords.Add(coord);
            }
            NetManager.SendMessage(cityStatusReq, typeof(GetFallenInfoAck).Name, this.PassStatusAck);
        }

        /* Add 'NetMessageAck' function here*/
        private void PassStatusAck(IExtensible message) {
            GetFallenInfoAck cityStatusAck = message as GetFallenInfoAck;
            int count = 0;
            foreach (FallenInfo info in cityStatusAck.Infos) {
                if (info.Index != this.PassList[count].id) {
                    break;
                }
                this.PassList[count].allianceName = info.AllianceName;
                if (!info.AllianceName.CustomIsEmpty()) {
                    this.view.SetPassStatus(count);
                }
                count++;
            }
        }
    }
}
