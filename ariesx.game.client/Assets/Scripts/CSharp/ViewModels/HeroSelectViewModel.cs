using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class HeroSelectViewModel : BaseViewModel, IViewModel {
        public TroopFormationViewModel parent;
        //private HeroModel model;
        private HeroSelectView view;
        /* Other members */

        /*****************/

        /* Model data get set */

        /**********************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<TroopFormationViewModel>();
            //this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<HeroSelectView>();
        }

        public void HideCurrentHero() {
            this.parent.HideCurrentHero();
        }

        public void ShowCurrentHero() {
            this.parent.ShowCurrentHero();
        }


        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }


        public void Reset() {
            this.view.Reset();
        }

        public void DisableEdit() {
            this.parent.DisableEdit();
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/

        #region FTE

        public void HideFteFormation() {
            this.parent.HideFteFormation();
        }

        public void ShowFteFormation() {
            this.parent.ShowFteFormation();
        }

        #endregion
    }
}
