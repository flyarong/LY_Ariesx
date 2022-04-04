using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine;
using System;

namespace Poukoute {
    public enum HeroInfoType {
        None,
        Info,
        Self,
        Unlock,
        Others
    }

    public class HeroInfoViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private HeroInfoView view;
        private HeroModel model;
        private BuildModel buildModel;
        private HeroTierUpViewModel tierUpViewModel;

        public bool isLevelUping = false;

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.model.heroDict;
            }
        }

        private Hero hero = null;
        public Hero CurrentHero {
            get {
                return hero;
            }
            set {
                this.hero = value;
                if (value != null) {
                    this.model.currentHeroName = value.Name;
                }
            }
        }

        public HeroAttributeConf CurrentHeroConf {
            get {
                return this.model.heroConf;
            }
            set {
                this.model.heroConf = value;
            }
        }

        public bool IsSubWindow {
            get; set;
        }

        public HeroInfoType infoType = HeroInfoType.None;
        private UnityAction levelUpCallback = null;

        void Awake() {
            //ConfigureManager.Instance.LoadConfigure<HeroSkillDefaultConf>("default_hero_skill_value");
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<HeroInfoView>();
            this.tierUpViewModel =
                PoolManager.GetObject<HeroTierUpViewModel>(this.transform);
            FteManager.SetStartCallback(GameConst.HERO_LEVEL, 1, this.OnHeroStep1Start);
            FteManager.SetStartCallback(GameConst.HERO_LEVEL, 2, this.OnHeroStep2Start);
            FteManager.SetEndCallback(GameConst.HERO_LEVEL, 2, this.OnHeroStep2End);
        }

        public void Show(Hero hero, HeroInfoType infoType = HeroInfoType.Others,
            bool isSubWindow = false) {
            this.IsSubWindow = isSubWindow;
            if (infoType == HeroInfoType.Others) {
                this.CurrentHero = hero;
            }
            this.Show(hero.Name, infoType: infoType, isSubWindow: isSubWindow);
        }

        public void Hide() {
            if (this.isLevelUping) {
                return;
            }
            this.view.PlayHide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void Show(string heroName, HeroInfoType infoType = HeroInfoType.Self,
            UnityAction levelUpCallBack = null, bool isSubWindow = false) {
            this.IsSubWindow = isSubWindow;
            this.view.PlayShow();
            this.levelUpCallback = levelUpCallBack;
            this.infoType = infoType;
            this.CurrentHeroConf = HeroAttributeConf.GetConf(heroName);
            switch (this.infoType) {
                case HeroInfoType.Self:
                    this.CurrentHero = this.HeroDict[heroName];
                    this.CurrentHero.NewEnergy = this.CurrentHero.GetNewEnergy();
                    if (this.CurrentHero.IsNew) {
                        this.parent.ReadHeroReq(heroName);
                    }
                    break;
                case HeroInfoType.Info:
                case HeroInfoType.Unlock:
                    this.CurrentHero = new Hero {
                        Name = heroName,
                        Level = 1,
                    };
                    this.CurrentHero.ArmyAmount =
                        this.CurrentHeroConf.GetAttribute(1, HeroAttribute.ArmyAmount, 1);
                    this.CurrentHero.NewEnergy = GameConst.HERO_ENERGY_MAX;
                    break;
                default:
                    break;
            }
            if (this.CurrentHero != null) {
                this.view.SetHeroInfo(infoType);
            }
        }

        public void ShowHeroTierUp() {
            this.tierUpViewModel.Show();
        }

        public void StartChapterDailyGuid() {
            this.parent.StartChapterDailyGuid();
        }

        //public void OnHeroLevelUpAnimDone() {
        //    this.parent.OnHeroLevelUpAnimDone();
        //}

        public void GetNewHeroDetail() {
            GetHeroesInfoReq getHeroesInfoReq = new GetHeroesInfoReq();
            GetHeroesInfoReq.HeroInfo heroInfo = new GetHeroesInfoReq.HeroInfo() {
                Name = this.CurrentHero.Name,
                Level = 1
            };
            getHeroesInfoReq.Infos.Add(heroInfo);
            NetManager.SendMessage(
                getHeroesInfoReq,
                typeof(GetHeroesInfoAck).Name,
                this.GetNewHeroDetailAck
            );
        }

        private void GetNewHeroDetailAck(IExtensible message) {
            GetHeroesInfoAck heroesInfo = message as GetHeroesInfoAck;
            List<Protocol.Skill> heroSkills = heroesInfo.Heroes[0].Skills;
            this.CurrentHero.Skills.Clear();
            for (int i = 0; i < heroSkills.Count; i++) {
                this.CurrentHero.Skills.Add(heroSkills[i]);
            }
            this.view.ContinueShowSkillDetail();
        }

        public float GetSiegeBonus() {
            return this.buildModel.GetSiegeBonus();
        }

        public int GetAttackBonus() {
            return this.buildModel.GetAttackBonus();
        }

        public int GetDefenseBonus() {
            return this.buildModel.GetDefenceBonus();
        }

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.Show(this.hero.Name, this.infoType, this.levelUpCallback);
            }
        }


        public void LevelUpReq() {
            if (this.isLevelUping || this.CurrentHero == null) {
                return;
            }
            LevelUpReq levelUpReq = new LevelUpReq() {
                Name = this.CurrentHero.Name
            };
            UnityAction action = () => {
                this.isLevelUping = false;
            };
            NetManager.SendMessage(levelUpReq, typeof(LevelUpAck).Name,
                this.LevelUpAck, (message) => action(), action);
            this.isLevelUping = true;
        }

        public void OnHeroLevelUpAnimDone() {
            this.view.SetHeroInfo(HeroInfoType.Self, true);
            this.isLevelUping = false;
        }

        private void LevelUpAck(IExtensible message) {
            //this.needFresh = true;
            this.CurrentHero = this.HeroDict[this.CurrentHero.Name];
            this.CurrentHero.NewEnergy = this.CurrentHero.GetNewEnergy();
            this.ShowHeroTierUp();
            if (this.levelUpCallback != null) {
                this.levelUpCallback.Invoke();
            } else {
                this.parent.RefreshHeroView(this.CurrentHero.Name);
            }
        }

        #region FTE
        private void OnHeroStep1Start(string index) {
            this.view.afterShowCallback = () => {
                if (!this.CurrentHero.Name.CustomEquals(FteManager.GetCurHero())) {
                    FteManager.StopFte();
                }
            };
        }

        private void OnHeroStep2Start(string index) {
            this.view.afterHideCallback = () => {
                FteManager.StopFte();
            };
            this.view.OnHeroStep2Start();
            this.Show(FteManager.GetCurHero(), HeroInfoType.Self, isSubWindow: true);
        }

        private void OnHeroStep2End() {
            this.view.afterHideCallback = null;
            this.parent.StartChapterDailyGuid();
            FteManager.StopFte();
            this.LevelUpReq();
        }

        public void ShowPay() {
            this.parent.ShowPay();
            this.Hide();
        }
        #endregion

    }
}
