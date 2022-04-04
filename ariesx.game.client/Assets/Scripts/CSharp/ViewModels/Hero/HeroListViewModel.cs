using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class HeroListViewModel : BaseViewModel {
        private HeroListView view;
        private HeroModel model;
        private HeroViewModel parent;

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.model.heroDict;
            }
        }

        public List<Hero> HeroList {
            get {
                return this.model.GetHeroListOrderBy();
            }
        }

        public Dictionary<string, HeroAttributeConf> UnlockHeroDict {
            get {
                return this.model.unlockHeroDict;
            }
        }

        public List<HeroAttributeConf> SortedUnlockHeroList {
            get {
                return this.model.unlockHeroList;
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

        public bool NeedRefresh {
            get; set;
        }

        public int CountShow {
            get {
                return this.model.countShow;
            }
        }

        void Awake() {
            this.view = this.gameObject.AddComponent<HeroListView>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.parent = this.transform.parent.GetComponent<HeroViewModel>();
            TriggerManager.Regist(Trigger.HeroStatusChange, () => {
                this.NeedRefresh = true;
            });
            this.NeedRefresh = true;
            NetHandler.AddNtfHandler(typeof(FieldFirstDownNtf).Name, this.FieldFirstDownNtf);
        }

        public void Show() {
            //this.NeedRefresh = true;
            this.view.Show();
            if (this.NeedRefresh) {
                this.view.SetListInfo();
                this.NeedRefresh = false;
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.NeedRefresh = true;
            }
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void RefreshHeroView(string hero) {
            if (this.view.IsVisible) {
                this.view.RefreshHeroView(hero);
            }
        }

        public List<Hero> GetHeroListOrderBy() {
            return this.model.GetHeroListOrderBy();
        }

        public int GetHeroRowlDataLength() {
            return this.model.heroRowList.Count;
        }

        public void OnHeroClick(string name) {
            this.parent.OnHeroInListClick(name);
        }

        public void ReadHeroReq(string name) {
            Hero hero;
            if (this.HeroDict.TryGetValue(name, out hero) && hero.IsNew) {
                this.parent.ReadHeroReq(name);
            }
        }

        public void OnUnlockHeroClick(string name) {
            this.parent.OnUnlockHeroClick(name);
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        private void FieldFirstDownNtf(IExtensible message) {
            this.model.ReOrderUnlockHeroList();
            if (this.view.IsVisible) {
                this.Show();
            } else {
                this.NeedRefresh = true;
            }
        }

        #region FTE

        public void OnHeroStep1Process() {
            this.view.afterHideCallback = () => {
                this.view.SetScrollEnable(true);
                FteManager.StopFte();
            };
            this.view.SetScrollEnable(false);
            StartCoroutine(this.view.OnHeroStep1Process());
        }

        public void OnHeroStep1End() {
            this.view.afterHideCallback = null;
            this.view.SetScrollEnable(true);
        }

        #endregion
    }
}
