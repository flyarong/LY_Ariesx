using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class TreasureMapViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private TreasureMapModel model;
        private TreasureMapView view;
        /* Model data get set */
        public TreasureConf TreasureConf {
            get {
                return this.model.treasureConf;
            }
            private set {
                if (value != null) {
                    this.model.treasureConf = value;
                }
            }
        }

        public int Level {
            get {
                return this.model.level;
            }
            set {
                if (this.model.level != value) {
                    this.model.level = value;
                    this.TreasureConf = TreasureConf.GetConf(value.ToString());
                }
            }
        }
        /**********************/

        /* Other members */
        private bool getTreasureMapRewarding = false;
        /*****************/

        void Awake() {
            //ConfigureManager.Instance.LoadConfigure<TreasureConf>("treasure_map_reward");
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<TreasureMapModel>();
            this.view = this.gameObject.AddComponent<TreasureMapView>();
        }

        public void Show() {
            this.view.PlayShow(
                () => this.parent.OnAddViewAboveMap(this)
            );
            this.view.OnTreasureLevelChange();
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void GetTreasureMapReward() {
            if (this.getTreasureMapRewarding) {
                return;
            }
            this.getTreasureMapRewarding = true;
            GetTreasureMapRewardReq getTreasureRewardReq =
                new GetTreasureMapRewardReq();
            NetManager.SendMessage(getTreasureRewardReq,
                typeof(GetTreasureMapRewardAck).Name, this.GetTreasureMapRewardAck);
        }


        /* Add 'NetMessageAck' function here*/
        private void GetTreasureMapRewardAck(IExtensible message) {
            GetTreasureMapRewardAck ack = message as GetTreasureMapRewardAck;
            this.view.CollectReward(ack.Resources, ack.Currency, ack.Reward);
            // Ask
            this.Level = 1;
            this.Show();
            this.getTreasureMapRewarding = false;
        }

        /***********************************/
    }
}
