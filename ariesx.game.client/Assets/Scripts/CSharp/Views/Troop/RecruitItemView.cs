using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using System.Text;

namespace Poukoute {
    public class RecruitItemView : BaseView {
        //private GameObject ui;
        /* UI Members*/
        #region ui element
        [SerializeField]
        private Transform pnlHero;
        [SerializeField]
        private HeroHeadView heroHead;
        [SerializeField]
        private CanvasGroup idleCanvasGroup;
        [SerializeField]
        private TextMeshProUGUI txtArmyAmount;
        [SerializeField]
        private TextMeshProUGUI txtTroopName;
        [SerializeField]
        private CustomButton btnRecruit;
        [SerializeField]
        private CanvasGroup busyCanvasGroup;
        [SerializeField]
        private CustomButton btnCancel;
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Image imgSlider;
        [SerializeField]
        private TextMeshProUGUI txtTimeLeft;
        #endregion
        /*************/

        private bool isFold;
        private string time;
        private Hero hero;
        private HeroAttributeConf heroConf;
        private float amountTotal;
        private float duration;
        private float Height;

        private bool recruiting = false;
        public bool IsRecruiting {
            get {
                return this.recruiting;
            }
            set {
                this.recruiting = value;
            }
        }

        public string TimeLeft {
            get {
                return this.txtTimeLeft.text;
            }
            set {
                this.txtTimeLeft.text = value;
            }
        }

        private float percent;
        public float Percent {
            get {
                return this.percent;
            }
            set {
                if (this.percent != value) {
                    this.percent = value;
                    this.percent = Mathf.Min(percent, 1);
                    this.IsRecruiting = (this.percent == 1);
                    this.OnPercentChange();
                }
            }
        }

        public bool HeroRecruitable {
            get; set;
        }

        public UnityAction<string> OnRecruit = null;
        public UnityAction<string> OnCancel = null;

        void Awake() {
            this.btnRecruit.onClick.AddListener(this.OnBtnRecruitClick);
            this.btnCancel.onClick.AddListener(this.OnBtnCancelClick);
        }

        private float heroFullArmyAmount;
        public void SetHero(Hero hero, float speed, bool inStrongHold, UnityAction<float> action) {
            //bool isHeroCanRecruit = false;
            this.hero = hero;
            //this.speed = speed;
            this.txtTroopName.text = LocalManager.GetValue(hero.Name);
            this.heroHead.SetHero(hero, false, false, false, true, false, true, showHeroStatus: false);

            // This should be set first, becouce below function will cause it's 'onvalueChanged'.
            this.slider.onValueChanged.RemoveAllListeners();
            this.slider.onValueChanged.AddListener(action);
            this.heroConf = HeroAttributeConf.GetConf(hero.GetId());

            EventRecruitClient eventRecruit =
                EventManager.GetRecruitEventByHeroName(hero.Name);
            bool isUnderTreatment = (eventRecruit != null);
            if (isUnderTreatment) {
                this.SetRecruitBusy(inStrongHold, eventRecruit);
            } else {
                this.SetRecruitIdle();
            }
            UIManager.SetHeroRecoverEffectL(isUnderTreatment, this.pnlHero);
            this.heroFullArmyAmount = this.heroConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount);
        }

        private void SetRecruitIdle() {
            this.IsRecruiting = false;

            float maxArmyamount = this.heroConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount);
            bool isHeroFull = this.hero.ArmyAmount == (int)maxArmyamount;
            float armyPercent = isHeroFull ? 1.0f : this.hero.ArmyAmount / maxArmyamount;
            Color amountColor = Color.white;
            if (armyPercent < 0.1f) {
                amountColor = ArtConst.Red;
            } else if (armyPercent < 0.6f) {
                amountColor = ArtConst.Orange;
            }

            this.slider.value = armyPercent;
            this.txtTimeLeft.text = isHeroFull ? LocalManager.GetValue(LocalHashConst.troop_full_tip) :
                string.Concat(((int)(armyPercent * 10000)) / 100, "%");
            this.SetTroopFullView(true);
            this.btnRecruit.Grayable = isHeroFull;
            this.txtArmyAmount.text = string.Concat(this.hero.ArmyAmount, "/", maxArmyamount);
            this.txtArmyAmount.color = amountColor;
            this.HeroRecruitable = !isHeroFull;
        }

        private void SetRecruitBusy(bool inStrongHold, EventRecruitClient eventRecruit) {
            this.SetTroopFullView(false);
            this.duration = eventRecruit.duration / 1000f;
            this.amountTotal = eventRecruit.armyAmount;
            long timeCost = (RoleManager.GetCurrentUtcTime() - eventRecruit.startTime) / 1000;
            this.percent = timeCost / this.duration;
            this.OnPercentChange();
            this.HeroRecruitable = false;
        }

        private void SetTroopFullView(bool isIdle) {
            this.imgSlider.type = isIdle ? Image.Type.Sliced : Image.Type.Tiled;
            this.imgSlider.sprite = ArtPrefabConf.GetSprite(
                isIdle ? "troop_progressbar_idle" : "troop_progressbar_busy");
            UIManager.SetUICanvasGroupEnable(this.idleCanvasGroup, isIdle);
            UIManager.SetUICanvasGroupEnable(this.busyCanvasGroup, !isIdle);
        }

        /* Propert change function */
        private void OnPercentChange() {
            this.txtArmyAmount.text = string.Concat(
                    Mathf.RoundToInt(this.hero.ArmyAmount + this.percent * this.amountTotal),
                    "/", this.heroFullArmyAmount);
            this.slider.value = this.percent;
        }

        private void OnBtnRecruitClick() {
            if (!this.IsRecruiting) {
                if (!this.btnRecruit.Grayable) {
                    this.OnRecruit.Invoke(this.hero.Name);
                    this.IsRecruiting = true;
                } else {
                    UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.troop_full_tip),
                        TipType.Notice);
                }
            }
        }

        private void OnBtnCancelClick() {
            this.OnCancel.Invoke(this.hero.Name);
        }

        #region FTE

        public void OnRecruitStep3Start() {
            FteManager.SetMask(this.btnRecruit.pnlContent, isButton: true);
        }

        #endregion
    }
}
