using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using Protocol;
using TMPro;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;

namespace Poukoute {
    public class QueueItemView : MonoBehaviour {

        #region ui element
        [SerializeField]
        private Button btnTroopInfo;
        [SerializeField]
        private CanvasGroup statusCG;
        [SerializeField]
        private Transform fillArea;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Image imgSliderFill;
        [SerializeField]
        private Transform pnlWarning;
        [SerializeField]
        private Transform pnlIdleAnim;
        [SerializeField]
        private Transform pnlAvatar;
        [SerializeField]
        private Image imgAvatar;
        #endregion

        private TroopStatus status = TroopStatus.None;
        public string Time {
            get {
                return this.txtTime.text;
            }
            set {
                this.txtTime.text = value;
            }
        }

        public string Name {
            get {
                return this.txtName.text;
            }
            set {
                this.txtName.text = value;
            }
        }

        public float Value {
            get {
                return this.slider.value;
            }
            set {
                this.slider.value = value;
            }
        }

        public float MaxValue {
            get {
                return this.slider.maxValue;
            }
        }
        [HideInInspector]
        public string TroopId;
        [HideInInspector]
        public Hero HeroInfo;
        public UnityAction OnTroopInfoClick;

        private const float SliderValueMin = 0.001f;

        public TroopStatus Status {
            get {
                return this.status;
            }
            set {
                if (this.status != value) {
                    this.status = value;
                    this.OnStatusChange();
                }
                else if (this.status == TroopStatus.HeroNotFull) {
                    this.OnStatusChange();
                }
            }
        }

        private void Start() {
            if (this.btnTroopInfo != null) {
                this.btnTroopInfo.onClick.AddListener(this.OnTroopInfoClick);
            }
        }

        public void SetItemStatusVisible(bool isVisible) {
            UIManager.SetUICanvasGroupEnable(this.statusCG, isVisible);
            if (!isVisible) {
                this.RefreshAnimate();
            }
        }

        public void RefreshAnimate() {
            if (this.Status == TroopStatus.HeroNotFull ||
                this.Status == TroopStatus.Unconfiged) {
                AnimationManager.Animate(this.gameObject, "Flash", null);
            } else {
                AnimationManager.Stop(this.gameObject);
            }
        }

        private void OnStatusChange() {
            if (!this.gameObject.activeSelf) {
                return;
            }
            string heroAvatarPath = "hero_avatar_small_default";
            if (HeroInfo != null) {
                heroAvatarPath = string.Format("{0}_s", HeroInfo.Name);
            }
            this.imgAvatar.sprite = ArtPrefabConf.GetSprite(heroAvatarPath);
            this.pnlWarning.gameObject.SetActiveSafe(false);
            this.StopIdelAnimation();
            string sliderBGPath = (this.status == TroopStatus.Marching) ?
                 "troop_progressbar_recruiting" : "troop_progressbar_marching";
            this.imgSliderFill.sprite = ArtPrefabConf.GetSprite(sliderBGPath);
            bool isRecruiting = false;
            switch (this.status) {
                case TroopStatus.Unconfiged:
                    this.SetQueueItemHeroUnconfigedStatus();
                    break;
                case TroopStatus.HeroNotFull:
                    this.SetQueueItemHeroNotFullStatus();
                    break;
                case TroopStatus.Fatigue:
                    this.SetQueueItemFatigueStatus();
                    break;
                case TroopStatus.NeedCure:
                    this.SetQueueItemNeedCureStatus();
                    break;
                case TroopStatus.Recruiting:
                    isRecruiting = true;
                    this.SetQueueItemRecruitingStatus();
                    break;
                case TroopStatus.Idle:
                    this.SetQueueItemIdleStatus();
                    break;
                case TroopStatus.Marching:
                    this.SetQueueItemMarchingStatus();
                    break;
                default:
                    Debug.LogError("Shoud not come here");
                    break;
            }
            UIManager.SetHeroRecoverEffectS(isRecruiting, this.pnlAvatar);
        }

        private void SetQueueItemMarchingStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(true);
            this.txtTime.color = Color.white;
        }

        private void SetQueueItemRecruitingStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(true);
            this.txtTime.color = Color.white;
        }

        private void SetQueueItemExploreStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(true);
            this.txtTime.color = Color.white;
        }

        private void SetQueueItemHeroUnconfigedStatus() {
            this.fillArea.gameObject.SetActiveSafe(false);
            this.pnlWarning.gameObject.SetActiveSafe(true);

            this.txtTime.text = LocalManager.GetValue(LocalHashConst.HUD_troop_not_configured);
            this.txtTime.color = Color.white;
        }

        private void SetQueueItemHeroNotFullStatus() {
            this.fillArea.gameObject.SetActiveSafe(false);
            this.pnlWarning.gameObject.SetActiveSafe(true);

            this.txtTime.text = LocalManager.GetValue(LocalHashConst.HUD_troop_not_full);
            this.txtTime.color = Color.white;
        }

        private void SetQueueItemIdleStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(false);
            this.pnlWarning.gameObject.SetActiveSafe(false);

            this.txtTime.text = LocalManager.GetValue(LocalHashConst.HUD_troop_idle);
            this.txtTime.color = Color.green;
            this.PlayIdleAnimation();
        }

        private void SetQueueItemFatigueStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(false);
            this.pnlWarning.gameObject.SetActiveSafe(false);

            this.txtTime.text = string.Concat(
                this.GetFatigueHeroNameInTroop(),
                LocalManager.GetValue(LocalHashConst.HUD_troop_fatigue));
            this.txtTime.color = Color.red;
            this.PlayIdleAnimation();
        }

        private string GetFatigueHeroNameInTroop() {
            Dictionary<string, Hero> heroDict = ModelManager.GetModelData<HeroModel>().heroDict;
            Dictionary<string, Troop> troopDict = ModelManager.GetModelData<TroopModel>().troopDict;
            Troop currentTroop = troopDict[TroopId];
            HeroPosition heroPosition;
            Hero hero;
            int troopHeroCount = currentTroop.Positions.Count;
            for (int i = 0; i < troopHeroCount; i++) {
                heroPosition = currentTroop.Positions[i];
                hero = heroDict[heroPosition.Name];
                if (hero.GetNewEnergy() < GameConst.HERO_ENERGY_COST) {
                    return HeroAttributeConf.GetLocalName(heroPosition.Name);
                }
            }

            return string.Empty;
        }


        private void SetQueueItemNeedCureStatus() {
            AnimationManager.Finish(this.gameObject);
            this.fillArea.gameObject.SetActiveSafe(false);
            this.pnlWarning.gameObject.SetActiveSafe(false);
            //StringBuilder s = new StringBuilder();
            this.txtTime.text = LocalManager.GetValue(LocalHashConst.HUD_troop_needcure);
            this.txtTime.color = Color.green;
            this.PlayIdleAnimation();
        }

        private string GetNeedCureHeroNameInTroop() {
            Dictionary<string, Hero> heroDict = ModelManager.GetModelData<HeroModel>().heroDict;
            Dictionary<string, Troop> troopDict = ModelManager.GetModelData<TroopModel>().troopDict;
            Troop currentTroop = troopDict[TroopId];
            HeroPosition heroPosition;
            Hero hero;
            int troopHeroCount = currentTroop.Positions.Count;
            int maxArmy;
            for (int i = 0; i < troopHeroCount; i++) {
                heroPosition = currentTroop.Positions[i];
                hero = heroDict[heroPosition.Name];
                maxArmy = HeroAttributeConf.GetHeroArmyAmount(hero.Name, hero.Level, hero.armyCoeff);
                if (hero.ArmyAmount < maxArmy * GameConst.TROOP_SHOW_CURE) {
                    return HeroAttributeConf.GetLocalName(heroPosition.Name);
                }
            }

            return string.Empty;
        }

        private void PlayIdleAnimation() {
            this.pnlIdleAnim.gameObject.SetActiveSafe(true);
            for (var i = 0; i < 3; i++) {
                AnimationManager.Animate(this.pnlIdleAnim.GetChild(i).gameObject,
                    "Play", Vector3.zero, Vector3.zero, null);
            }
        }

        private void StopIdelAnimation() {
            for (var i = 0; i < 3; i++) {
                AnimationManager.Stop(this.pnlIdleAnim.GetChild(i).gameObject);
            }
            this.pnlIdleAnim.gameObject.SetActiveSafe(false);
        }

        public void UpdateItem(EventBase eventBase) {
            long now = RoleManager.GetCurrentUtcTime();
            long left = eventBase.duration - now + eventBase.startTime;
            left = (long)Mathf.Max(0, left);
            this.InnerUpdateItem(left, eventBase.duration);
        }

        private void InnerUpdateItem(long left, long duration) {
            if (duration > SliderValueMin) {
                this.Value = this.MaxValue *
                    (1 - (float)left / duration);
            } else {
                this.Value = 0;
            }
            this.Time = GameHelper.TimeFormat(left);
        }
    }
}
