using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class TributeViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private TributeModel model;
        private TributeView view;
        /* Model data get set */

        public GetTributeAck TributeResult {
            get {
                return this.model.tributeResult;
            }
            set {
                this.model.tributeResult = value;
            }
        }

        /* Other members */

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<TributeModel>();
            this.view = this.gameObject.AddComponent<TributeView>();
        }

        public void ShowWithReq() {
            this.GetTributeReq();
        }

        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
            }, true);
            this.view.SetBtnReceiveInteractable(true);
            this.view.OnForceChange();
            this.view.OnTributeGroupChange();
        }

        public void Hide() {
            this.view.PlayHide(()=> {
                this.parent.OnRemoveViewAboveMap(this);
                this.parent.HideTributeObj();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void GetTributeReq() {
            GetTributeReq getTributeReq = new GetTributeReq();

            NetManager.SendMessage(getTributeReq, typeof(GetTributeAck).Name,
                this.GetTributeAck);
            //RoleManager.Instance.NeedResourceAnimation = true;
            //RoleManager.Instance.NeedCurrencyAnimation = true;
        }
        /* Add 'NetMessageAck' function here*/
        public void GetTributeAck(IExtensible message) {
            GetTributeAck tributeAck = message as GetTributeAck;
            //Debug.Log("***********************************");
            //Debug.LogError(GameHelper.DateFormat(tributeAck.Tribute.Timestamp));
            long startTime = tributeAck.Tribute.Timestamp;
            EventManager.AddTributeEvent(startTime);
            this.TributeResult = tributeAck;
            this.Show();
        }

        public void CollectResources() {
            this.view.SetBtnReceiveInteractable(false);
            this.view.CollectResource(this.TributeResult);
            this.Hide();
        }
        /***********************************/
    }
}
