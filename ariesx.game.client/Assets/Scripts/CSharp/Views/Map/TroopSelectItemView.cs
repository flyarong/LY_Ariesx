using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using ProtoBuf;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public enum TroopRelation {
        None,
        Self,
        Enemy,
        Ally
    }

    public class TroopSelectItemView : BaseItemViewsHolder {
        private Troop troop;
        public Troop Troop {
            get {
                return this.troop;
            }
            set {
                this.troop = value;
                this.troopAttributes =
                    ModelManager.GetModelData<TroopModel>().GetTroopAttributes(this.troop.Id);
                this.OnTroopChange();
                this.UpdateTroopTreatmentStatus();
            }
        }

        public Vector2 target { get; set; }
        private bool IsAttack { get; set; }

        private TroopAttributes troopAttributes;

        private bool isShining = false;
        public bool IsShining {
            get {
                return this.isShining;
            }
        }

        //private GameObject ui;
        #region ui cache
        [SerializeField]
        private CustomButton btnItemView;
        [SerializeField]
        private Transform pnlContent;
        [SerializeField]
        private RectTransform rtContentRT;
        [SerializeField]
        private Image imgSelected;
        [SerializeField]
        private Image imgShining;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private GameObject objHeroAvatar;
        [SerializeField]
        private Image imgHeroBackground;
        [SerializeField]
        private Image imgHeroAvatar;
        [SerializeField]
        private Transform pnlHeroDefault;
        [SerializeField]
        private Slider sldArmyAmount;
        [SerializeField]
        private TextMeshProUGUI txtArmyAmount;
        [SerializeField]
        private Transform pnlDistance;
        [SerializeField]
        private TextMeshProUGUI txtDistance;
        #endregion
        //private Vector2 previousePosition = Vector2.zero;
        //private float threshold = 5f;
        //private UnityAction onClick = null;

        /*************************************************/

        public void DisableHighlight() {
            this.imgSelected.gameObject.SetActiveSafe(false);
            this.pnlContent.localScale = Vector3.one;
            this.rtContentRT.offsetMax = Vector2.zero;
            this.rtContentRT.offsetMin = Vector2.zero;
        }

        public void EnableHighlight() {
            this.imgSelected.gameObject.SetActiveSafe(true);
            this.pnlContent.localScale = 1.1f * Vector3.one;
            this.rtContentRT.offsetMax = Vector2.up * -7;
            this.rtContentRT.offsetMin = Vector2.up * -7;
        }

        public void EnableShining() {
            this.imgShining.gameObject.SetActiveSafe(true);
            this.isShining = true;
            AnimationManager.Animate(this.gameObject, "Shake", loop: true);
        }

        public void DisableShining() {
            this.imgShining.gameObject.SetActiveSafe(false);
            this.isShining = false;
            AnimationManager.Stop(this.gameObject);
        }

        public void SetSelfTroopInfo(bool isAttack, Vector2 target, Troop troop, UnityAction onClick) {
            this.IsAttack = isAttack;
            this.target = target;
            this.Troop = troop;
            this.DisableHighlight();
            this.btnItemView.onClick.RemoveAllListeners();
            this.btnItemView.onClick.AddListener(onClick);
            this.imgHeroBackground.sprite =
                ArtPrefabConf.GetSprite(SpritePath.tileTroopSelectBgPrefix + "self");
        }

        public void SetOtherTroopInfo(string playerName, int avatarId,
            string allianceId, string belong, UnityAction onClick) {
            this.DisableHighlight();
            if ((!belong.CustomIsEmpty() && belong.CustomEquals(RoleManager.GetAllianceId())|| 
                (!allianceId.CustomIsEmpty() && allianceId == RoleManager.GetAllianceId()))) {
                this.imgHeroBackground.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.tileTroopSelectBgPrefix + "ally");
            } else {
                this.imgHeroBackground.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.tileTroopSelectBgPrefix + "enemy");
            }
            string avatarName = PlayerHeroAvatarConf.GetAvatarName(avatarId.ToString());
            this.imgHeroAvatar.sprite = ArtPrefabConf.GetRoleAvatarSprite(avatarName);
            this.txtName.text = playerName;
            this.txtArmyAmount.text = "? ? ?";
            this.sldArmyAmount.value = 0;
            this.btnItemView.onClick.RemoveAllListeners();
            this.btnItemView.onClick.AddListener(onClick);
            this.pnlDistance.gameObject.SetActive(false);
        }

        private void OnTroopChange() {
            this.sldArmyAmount.maxValue = this.troopAttributes.maxArmy;
            this.sldArmyAmount.value = this.troopAttributes.army;
            this.txtArmyAmount.text = string.Concat(
                GameHelper.GetFormatNum(this.troopAttributes.army),
                "/", GameHelper.GetFormatNum(this.troopAttributes.maxArmy));
            bool hasHero = (this.troop.Positions.Count > 0);
            this.pnlHeroDefault.gameObject.SetActiveSafe(!hasHero);
            this.objHeroAvatar.gameObject.SetActive(hasHero);
            if (hasHero) {
                TroopModel.TroopPositionReSort(this.troop);
                string spriteName = this.troop.Positions[0].Name.Replace(" ", string.Empty);
                this.imgHeroAvatar.sprite = ArtPrefabConf.GetSprite(
                    spriteName, SpritePath.heroAvatarMiddleSuffix);
            }

            this.txtName.text = TroopModel.GetTroopName(this.Troop.ArmyCamp);
            this.pnlDistance.gameObject.SetActiveSafe(true);
            this.ShowDistanceLabel();
            this.ShowOtherLabel();
        }

        private void UpdateTroopTreatmentStatus() {
            if (this.Troop == null) {
                return;
            }

            bool hasHero = (this.troop.Positions.Count > 0);
            bool isTroopUnterTreatment = EventManager.IsTroopUnderTreatment(this.Troop.Id);
            UIManager.SetHeroRecoverEffectS(hasHero && isTroopUnterTreatment,
                this.imgHeroAvatar.transform.parent);
        }

        private void ShowDistanceLabel() {
            Vector2 position = new Vector2(this.troop.Coord.X, this.troop.Coord.Y);
            float sqrDistance = (position - this.target).sqrMagnitude;
            if (sqrDistance <= GameConst.TROOP_NEAR * GameConst.TROOP_NEAR) {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_near);
                this.txtDistance.color = ArtConst.TroopYellow;
            } else if (sqrDistance <= GameConst.TROOP_MEDIUM * GameConst.TROOP_MEDIUM) {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_medium);
                this.txtDistance.color = ArtConst.TroopGreen;
            } else if (sqrDistance <= GameConst.TROOP_FAR * GameConst.TROOP_FAR) {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_far);
                this.txtDistance.color = ArtConst.TroopBlue;
            } else {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_too_far);
                this.txtDistance.color = ArtConst.TroopPurple;
            }
        }

        private void ShowOtherLabel() {
            if (TroopModel.CheckTroopIsRecruiting(this.troop)) {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_recruit);
                this.txtDistance.color = Color.yellow;
                return;
            }

            // Check energy
            if (this.troopAttributes.energy < GameConst.HERO_ENERGY_COST) {
                this.txtDistance.text = LocalManager.GetValue(LocalHashConst.troop_fatigued);
                this.txtDistance.color = Color.red;
                return;
            }
        }
    }
}
