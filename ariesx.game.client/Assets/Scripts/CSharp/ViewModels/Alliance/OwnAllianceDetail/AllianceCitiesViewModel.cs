using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
namespace Poukoute {
    public class AllianceCitiesViewModel: BaseViewModel {
        private AllianceDetailModel model;
        private AllianceCitiesView view;
        private AllianceDetailViewModel parent;
        /* Model data get set */

        public List<NPCCityConf> AllianceCities {
            get {
                return this.model.allianceCities.citiesList;
            }
        }

        public List<MiniMapPassConf> AlliancePasses {
            get {
                return this.model.allianceCities.passesList;
            }
        }

        public int CitiesCount {
            get {
                return this.model.allianceCities.citiesCount;
            }
            set {
                this.model.allianceCities.citiesCount = value;
            }
        }

        public int PassesCount {
            get {
                return this.model.allianceCities.passedCount;
            }
            set {
                this.model.allianceCities.passedCount = value;
            }
        }

        private Dictionary<Resource, int> ResourceBuff {
            get {
                return this.model.allianceCities.resourceBuff;
            }
        }
        /**********************/

        /* Other members */
        public bool NeedFresh {
            get; set;
        }

        /*****************/

        void Awake() {
            this.model = ModelManager.GetModelData<AllianceDetailModel>();
            this.view = this.gameObject.AddComponent<AllianceCitiesView>();
            this.NeedFresh = true;
            this.parent = this.transform.parent.GetComponent<AllianceDetailViewModel>();
        }

        public void Show() {
            this.view.Show();
            if (this.NeedFresh) {
                this.GetAllianceFallenTargets();
            }
        }

        protected override void OnReLogin() {
            this.NeedFresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void Hide() {
            this.view.Hide(this.ResetStatus);
        }

        private void ResetStatus() {
            this.NeedFresh = true;
        }

        public Dictionary<Resource, int> GetCityResourceBuffAddition() {
            this.ResetResourceBuffDict();

            foreach (NPCCityConf cityConf in this.AllianceCities) {
                foreach (Resource resource in cityConf.resourceBuff.Keys) {
                    this.ResourceBuff[resource] += cityConf.resourceBuff[resource];
                }
            }

            return this.ResourceBuff;
        }

        private void ResetResourceBuffDict() {
            this.ResourceBuff.Clear();
            this.ResourceBuff.Add(Resource.Lumber, 0);
            this.ResourceBuff.Add(Resource.Steel, 0);
            this.ResourceBuff.Add(Resource.Marble, 0);
            this.ResourceBuff.Add(Resource.Food, 0);
        }
        
        /* Add 'NetMessageReq' function here*/
        public void GetAllianceFallenTargets() {
            GetAllianceFallenTargetsReq fallenTargetsReq = new GetAllianceFallenTargetsReq();
            NetManager.SendMessage(fallenTargetsReq,
                                    typeof(GetAllianceFallenTargetsAck).Name,
                                    this.GetAllianceFallenTargetsAck);

        }

        private NPCCityConf GetCityConf(FallenTarget fallenTarget) {
            string cityKey = NPCCityConf.GetCityKey(fallenTarget);
            NPCCityConf cityConf = NPCCityConf.GetConf(cityKey);
            return cityConf;
        }

        private MiniMapPassConf GetPassConf(FallenTarget fallenTarget) {
            string passKey = string.Concat(fallenTarget.MapSN, ",",
                                            fallenTarget.ZoneSN, ",",
                                            fallenTarget.Coord.X, ",",
                                            fallenTarget.Coord.Y);
            MiniMapPassConf passConf = MiniMapPassConf.GetConf(passKey);
            return passConf;
        }

        /* Add 'NetMessageAck' function here*/
        private void GetAllianceFallenTargetsAck(IExtensible message) {
            GetAllianceFallenTargetsAck fallenTargetsAck =
                            message as GetAllianceFallenTargetsAck;
            this.CitiesCount = fallenTargetsAck.Cities.Count;
            this.PassesCount = fallenTargetsAck.Passes.Count;
            this.AllianceCities.Clear();
            int index;
            int citiesCount = fallenTargetsAck.Cities.Count;
            for (index = 0; index < citiesCount; index++) {
                FallenTarget fallen = fallenTargetsAck.Cities[index];
                this.AllianceCities.Add(this.GetCityConf(fallen));
            }

            this.AlliancePasses.Clear();
            citiesCount = fallenTargetsAck.Passes.Count;
            for (index = 0; index < citiesCount; index++) {
                this.AlliancePasses.Add(
                    this.GetPassConf(fallenTargetsAck.Passes[index]));
            }
            if (this.view.IsVisible) {
                this.view.RefreshCitiesList();
            }

        }

        /***********************************/
        public void JumbCityItemCood(Coord coord) {
            //Debug.LogError("зјБъ"+coord);
            this.parent.JumbCityItemCood(coord);
        }

    }
}
