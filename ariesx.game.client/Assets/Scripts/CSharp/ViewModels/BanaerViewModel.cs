using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class BannaerViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private DramaModel model;
        private BuildModel buildModel;
        private BannaerView view;
        /* Model data get set */
        public List<ChapterTask> DramaList {
            get {
                return this.model.dramaList;
            }
        }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<DramaModel>();
            this.view = this.gameObject.AddComponent<BannaerView>();
        }

        public void ShowBanner(Protocol.CommonReward reward, Protocol.Resources resources
            , Protocol.Currency currency, bool needNext) {
            if (!this.view.IsVisible) {
                this.view.ShowBanner(reward, resources
            ,  currency, needNext);
            }
        }

        public void ShowNextChapter() {
            if (!this.view.IsVisible) {
                this.view.ShowNextChapterImmediately();
            }
        }

        //public void ShowBannerEx() {
        //    if (!this.view.IsVisible) {
        //        this.view.ShowBannerEx();
        //    }
        //}

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void ShowDramaView() {
            this.parent.ShowDramaView();
        }
    }
}
