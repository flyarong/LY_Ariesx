using UnityEngine;
using UnityEngine.Events;
using ProtoBuf;
using Protocol;
using System.Collections.Generic;

namespace Poukoute {
    public class FallenResourceData {
        public float alreadyPaid;
        public float allNeedPay;
        public Dictionary<Resource, float> resourceInfo;
    }

    public class FallenInfoViewModel : BaseViewModel, IViewModel {
        private FallenInfoView view;

        private float allNeeded = 0;
        private float alreadyPaid = 0;
        private float currentPaid = 0;

        public float CurrentPaid {
            get {
                return this.currentPaid;
            }
            set {
                this.currentPaid = value;
                this.OnCurrentPaidChange();
            }
        }


        public float AlreadyPaid {
            get {
                return this.alreadyPaid;
            }
            set {
                this.alreadyPaid = value;
                this.OnAlreadyPaidChange();
            }
        }

        public float AllNeeded {
            get {
                return this.allNeeded;
            }
            set {
                this.allNeeded = value;
                this.OnAllNeedChange();
            }
        }

        /* Other members */
        public bool NeedFresh {
            get; set;
        }
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<FallenInfoView>();
            TriggerManager.Regist(Trigger.ResourceChange, this.SetResourcesMaxValue);

            this.NeedFresh = true;
        }

        public void Show() {
            this.GetLiberationCost();
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.view.HideCurrentPanel();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            this.NeedFresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }


        /* Add 'NetMessageReq' function here*/
        public void PayFallenResources(Dictionary<Resource, int> resourceInfo) {
            PayLiberationReq payLiberationReq = new PayLiberationReq() {
                Lumber = resourceInfo[Resource.Lumber],
                Marble = resourceInfo[Resource.Marble],
                Steel = resourceInfo[Resource.Steel],
                Food = resourceInfo[Resource.Food]
            };
            NetManager.SendMessage(payLiberationReq,
                                    typeof(PayLiberationAck).Name,
                                    this.PayLiberationAck);
            this.Hide();
        }

        private void GetLiberationCost() {
            GetLiberationCostReq getLiberationReq = new GetLiberationCostReq();
            NetManager.SendMessage(getLiberationReq,
                                    typeof(GetLiberationCostAck).Name,
                                    this.GetLiberationCostAck);
        }
        /***********************************/


        /* Add 'NetMessageAck' function here*/
        private void GetLiberationCostAck(IExtensible message) {
            GetLiberationCostAck liberationCost = message as GetLiberationCostAck;
            this.view.PlayShow();
            this.AllNeeded = liberationCost.Needs;
            this.AlreadyPaid = liberationCost.Costs;
            this.SetResourcesMaxValue();
        }

        private void SetResourcesMaxValue() {
            if (!this.view.IsVisible) {
                this.NeedFresh = true;
                return;
            }

            Dictionary<Resource, float> resourceDict = new Dictionary<Resource, float> {
                { Resource.Lumber, RoleManager.GetResource(Resource.Lumber) },
                { Resource.Steel, RoleManager.GetResource(Resource.Steel) },
                { Resource.Marble, RoleManager.GetResource(Resource.Marble) },
                { Resource.Food, RoleManager.GetResource(Resource.Food) }
            };
            this.view.SetResourcesMaxValue(resourceDict);
        }

        private void PayLiberationAck(IExtensible message) {
            //PayLiberationAck payLiberationAck = message as PayLiberationAck;
            Debug.LogError("PayLiberationAck callded");
            this.NeedFresh = true;
            this.currentPaid = 0;
            this.view.ResetResourceItemView();
        }

        /***********************************/

        private void OnAlreadyPaidChange() {
            this.view.OnAlreadyPaidChange();
        }

        private void OnAllNeedChange() {
            this.view.OnAllNeedChange();
        }

        private void OnCurrentPaidChange() {
            if (this.CurrentPaid + this.AlreadyPaid <= this.allNeeded) {
                this.view.OnCurrentPaidChange();
            }
        }

    }
}
