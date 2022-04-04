using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;
using System;

namespace Poukoute {

    public class TileRewardViewModel : BaseViewModel, IViewModel {
        private MapTileViewModel parent;
        private TileRewardView view;
        private MapModel mapModel;
        private MapTileModel model;

        private bool canShow = true;

        public MapTileInfo TileInfo {
            get {
                return this.model.tileInfo;
            }
        }

        public bool ShowAnimation {
            get {
                return this.parent.ShowAnimation;
            }
        }

        public Transform Target {
            get {
                return this.parent.GetTargetTransform();
            }
        }

        /***********************************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapTileViewModel>();
            this.model = ModelManager.GetModelData<MapTileModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<TileRewardView>();
        }

        public void ShowTileRewardViewWithDelay(float seconds) {
            if (this.canShow) {
                this.view.ShowTileRewardViewWithDelay(seconds);
            }
        }

        public void HideImmediatly() {
            this.view.HideTileRewardInfo();
        }

        public void SetShowTileBool(bool canShow) {
            this.canShow = canShow;
        }

        public Point GetPlayerPoint(Vector2 coordinate) {
            return this.mapModel.GetPlayerPoint(coordinate);
        }
    }
}
