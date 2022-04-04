using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MarchView : BaseView {
        private MarchViewModel viewModel;
        private MarchViewPreference viewPref;

        private Transform currentHeroPositon;
        private Vector3 currentHeroOrigin;
        private HeroHeadView currentHero;

        /****************************************/

        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<MarchViewModel>();
            this.ui = UIManager.GetUI("UIMarchBind");
            this.viewPref = this.ui.transform.GetComponent<MarchViewPreference>();
            this.viewPref.btnReturnImmediately.
               onClick.AddListener(this.OnBtnReturnImmediatelyClick);
        }

        public Transform GetMarchBind() {
            if (this.viewPref == null) {
                this.InitUI();
            }
            return this.viewPref.pnlMarchBind;
        }

        private void ShowMarch() {
            Troop troop = this.viewModel.CurrentMarch.troop;
            if (troop == null) {
                Debug.LogError("ShowMarch troop null");
                return;
            }
            this.viewPref.troopGridView.SetTroopGrid(troop, this.OnHeroClick);
            this.viewPref.txtTroopName.text = TroopModel.GetTroopName(troop.ArmyCamp);
            if (troop.Positions.Count == 0) {
                this.SetAttribute("0", this.viewPref.pnlInfo.GetChild(0));
                this.SetAttribute("--", this.viewPref.pnlInfo.GetChild(1));
                this.SetAttribute("--", this.viewPref.pnlInfo.GetChild(2));
                return;
            }
            Hero mainHero = this.viewModel.HeroDict[troop.Positions[0].Name];
            int armyAmount = 0;
            foreach (HeroPosition heroPosition in troop.Positions) {
                armyAmount += this.viewModel.HeroDict[heroPosition.Name].ArmyAmount;
            }

            // Set attributes.
            MarchAttributes attributes = this.viewModel.MarchAttributes;
            Vector2 origin = this.viewModel.CurrentMarch.origin;
            Vector2 target = this.viewModel.CurrentMarch.target;
            this.viewPref.pnlCoordinate.gameObject.SetActive(true);
            this.SetAttribute(attributes.army.ToString(), this.viewPref.pnlInfo.GetChild(0));
            this.SetAttribute(HeroAttributeConf.GetSpeed(attributes.speed), this.viewPref.pnlInfo.GetChild(1));
            this.SetAttribute(attributes.siege.ToString(), this.viewPref.pnlInfo.GetChild(2));
            this.SetAttribute(
                string.Concat("(", origin.x + ", " + origin.y, ")"),
                this.viewPref.pnlCoordinate.Find("PnlOrigin")
            );
            this.SetAttribute(
                string.Concat("(", target.x + ", " + target.y, ")"),
                this.viewPref.pnlCoordinate.Find("PnlTarget")
            );
            this.SetButton();
            this.Format();
            //Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
        }

        public void ShowTroop() {
            this.viewPref.pnlCoordinate.gameObject.SetActive(false);
            this.viewPref.troopGridView.TroopFormationCallback =
                () => this.viewModel.ShowTroopFormation(this.viewModel.CurrentTroop.Id);
            this.viewPref.troopGridView.SetTroopGrid(this.viewModel.CurrentTroop, this.OnHeroClick);
            Troop troop = this.viewModel.CurrentTroop;
            this.viewPref.txtTroopName.text = TroopModel.GetTroopName(troop.ArmyCamp);
            if (troop.Positions.Count == 0) {
                this.SetAttribute("0", this.viewPref.pnlInfo.GetChild(0));
                this.SetAttribute("--", this.viewPref.pnlInfo.GetChild(1));
                this.SetAttribute("--", this.viewPref.pnlInfo.GetChild(2));
                return;
            }
            Hero mainHero = this.viewModel.HeroDict[troop.Positions[0].Name];
            int armyAmount = 0;
            foreach (HeroPosition heroPosition in troop.Positions) {
                armyAmount += this.viewModel.HeroDict[heroPosition.Name].ArmyAmount;
            }

            // Set attributes.
            MarchAttributes attributes = this.viewModel.TroopAttributes;
            this.SetAttribute(attributes.army.ToString(), this.viewPref.pnlInfo.GetChild(0));
            this.SetAttribute(HeroAttributeConf.GetSpeed(attributes.speed), this.viewPref.pnlInfo.GetChild(1));
            this.SetAttribute(attributes.siege.ToString(), this.viewPref.pnlInfo.GetChild(2));
            //Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
            this.viewPref.pnlButton.
               gameObject.SetActiveSafe(false);
        }

        private void Format() {
            this.viewPref.vgMarchDetail.enabled = true;
            this.viewPref.csfMarchDetail.enabled = true;
            base.StartCoroutine(this.DelayFormat());
        }

        private IEnumerator DelayFormat() {
            yield return YieldManager.EndOfFrame;
            this.viewPref.csfMarchDetail.enabled = false;
            this.viewPref.vgMarchDetail.enabled = false;
        }

        private void SetButton() {
            this.viewPref.pnlButton.
                gameObject.SetActiveSafe(this.viewModel.IsTownhall());
        }

        private void SetAttribute(string value, Transform attributeObj) {
            attributeObj.Find("TxtNumber").GetComponent<TextMeshProUGUI>().text = value;
        }

        private void OnHeroClick(string name, UnityAction levelUpCallback) {
            this.viewModel.ShowHeroInfo(name, levelUpCallback);
        }

        public void OnMarchChange() {
            this.ShowMarch();
        }

        public void OnTroopChange() {
            this.ShowTroop();
        }

        private void OnBtnReturnImmediatelyClick() {
            if (RoleManager.GetResource(Resource.Gem) < 20) {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.gem_short),
                    LocalManager.GetValue(LocalHashConst.server_gem_not_enough),
                    () => { this.viewModel.ShowPay(); },
                    txtYes: LocalManager.GetValue(LocalHashConst.button_enter_shop));
                return;
            }
            this.viewModel.CompleteTroopMarchReq();
        }
    }
}
