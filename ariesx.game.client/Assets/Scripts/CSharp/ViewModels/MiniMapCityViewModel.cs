using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class MiniMapCityViewModel : BaseViewModel {
        private MiniMapViewModel parent;
        private MiniMapCityView view;
        /* Model data get set */
        public int State {
            get {
                return this.parent.State;
            }
        }

        public List<NPCCityConf> CityList {
            get {
                return NPCCityConf.GetCityIn(this.State);
            }
        }

        public Vector2 CurrentCoord {
            get; set;
        }

        public bool IsSelectItem {
            get;set;
        }

        private bool NeedRefresh { get; set; }
        
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<MiniMapCityView>();
            this.parent = this.transform.parent.GetComponent<MiniMapViewModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            this.parent.resetChoseAction = this.view.ResetItemIsChosen; 
            if (this.NeedRefresh) {
                this.view.ResetItems(this.CityList.Count);
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
                this.view.ResetItemIsChosen();
                this.view.ResetItems(this.CityList.Count);
                this.CityStatusReq();
            } else {
                this.NeedRefresh = true;
            }
        }

        private void CityStatusReq() {
            GetFallenInfoReq cityStatusReq = new GetFallenInfoReq();
            foreach (NPCCityConf city in this.CityList) {
                MiniMapCityConf cityConf = MiniMapCityConf.GetConf(city.id);
                Coord coord = new Coord(Mathf.RoundToInt(cityConf.coordinate.x),
                                        Mathf.RoundToInt(cityConf.coordinate.y));
                cityStatusReq.Coords.Add(coord);
            }
            NetManager.SendMessage(cityStatusReq, typeof(GetFallenInfoAck).Name, this.CityStatusAck);
        }

        /* Add 'NetMessageAck' function here*/
        private void CityStatusAck(IExtensible message) {
            GetFallenInfoAck cityStatusAck = message as GetFallenInfoAck;
            int count = 0;
            foreach (FallenInfo info in cityStatusAck.Infos) {
                if (info.Index != this.CityList[count].id) {
                    continue;
                }
                this.CityList[count].allianceName = info.AllianceName;
                if (!info.AllianceName.CustomIsEmpty()) {
                    this.view.SetCityStatus(count);
                }
                count++;
            }
        }
        /***********************************/
    }
}
