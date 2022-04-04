using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class HeroPoolViewModel : BaseViewModel, IViewModel {
        private HeroPoolView view;
        private HeroModel model;
        private MapViewModel parent;

        public BuildModel buildModel;
        public Dictionary<string, Hero> HeroDict {
            get {
                return this.model.heroDict;
            }
        }

        public HeroSortType HeroSortBy {
            get {
                return this.model.heroSortType;
            }
            set {
                this.model.heroSortType = value;
            }
        }

        public string CurrentGachaName {
            get {
                return this.currentGachaName;
            }
            set {
                this.currentGachaName = value;
                this.CurGachaConf = GachaGroupConf.GetConf(value);
            }
        }
        private string currentGachaName;

        public GachaGroupConf CurGachaConf {
            get; set;
        }

        public bool NeedRefresh {
            get; set;
        }

        void Awake() {
            this.model = ModelManager.GetModelData<HeroModel>();
            this.view = this.gameObject.AddComponent<HeroPoolView>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.NeedRefresh = true;
        }

        public void Show(string groupName, string buildingId = null) {
            this.view.PlayShow();
            this.CurrentGachaName = groupName;
            if (this.NeedRefresh) {
                this.view.SetHeroPoolDetail();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void RefreshHeroView(string hero) {
            this.view.RefreshHeroView(hero);
        }

        public void OnHeroClick(string name) {
            this.parent.ShowHeroInfo(name, infoType: HeroInfoType.Unlock);
        }
    }
}
