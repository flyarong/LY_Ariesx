using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class HeroSelectView : BaseView {
        private HeroSelectViewModel viewModel;
        private HeroSelectViewPreference viewPref;
        /*************/
        private Transform troopUI;
        private Vector2 origin;
        private Vector2 previouse;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HeroSelectViewModel>();
        }

        protected override void OnUIInit() {
            this.troopUI = UIManager.GetUI("UITroopFormation.PnlFormation").transform;
            this.ui = UIManager.GetUI("UITroopFormation.PnlFormation.PnlSelect");
            this.viewPref = this.ui.transform.GetComponent<HeroSelectViewPreference>();
            this.viewPref.drag.onBeginDrag.AddListener(this.OnBeginDrag);
            this.viewPref.drag.onDrag.AddListener(this.OnDrag);
            this.viewPref.drag.onEndDrag.AddListener(this.OnEndDrag);
            this.viewPref.click.onClick.AddListener(() => {
                this.viewModel.DisableEdit();
            });
        }

        private void OnBeginDrag(Vector2 position) {
            this.viewPref.pnlButtons.gameObject.SetActiveSafe(false);
            this.ui.transform.SetParent(UIManager.GetUI("UITroopFormation").transform);
            this.origin = this.viewPref.rectTransform.anchoredPosition;
            this.viewPref.rectTransform.anchoredPosition +=
                new Vector2(0, 10 + this.viewPref.buttonsRectTransform.rect.height / 2);
            this.viewPref.imgHighlight.enabled = false;
            this.viewModel.HideCurrentHero();
            this.previouse = MapUtils.ScreenToUIPoint(position);
            this.viewPref.canvasGroup.blocksRaycasts = false;
            this.HideFteArrow();
        }

        private void OnDrag(Vector2 position) {
            Vector2 current = MapUtils.ScreenToUIPoint(position);
            Vector2 delta = current - this.previouse;
            this.previouse = current;
            this.viewPref.rectTransform.anchoredPosition += delta;
            this.viewModel.parent.ShowDragSelectHighlight(GameManager.MainCamera.WorldToScreenPoint(this.viewPref.rectTransform.position));
        }

        private void OnEndDrag(Vector2 position) {
            if ((this.viewPref.rectTransform.anchoredPosition - this.origin).sqrMagnitude > 2500f) {
                AnimationManager.Animate(this.ui, "Move",
                    this.viewPref.rectTransform.anchoredPosition, this.origin, () => {
                        if (this.viewModel.parent.IsAddHeroSuccess) {
                            this.viewModel.parent.IsAddHeroSuccess = false;
                        }
                        else {
                            this.viewModel.parent.ShowDragHint();
                        }
                        this.Reset(false);
                    });
            }
            else {
                this.viewPref.rectTransform.anchoredPosition = this.origin;
                this.Reset();
            }
            this.ShowFteArrow();
        }

        public void Reset(bool setParent = true) {
            this.InitUI();
            this.viewPref.pnlButtons.gameObject.SetActiveSafe(true);
            this.viewPref.imgHighlight.enabled = true;
            this.FormatPnlSelect();
            if (setParent)
                this.ui.transform.SetParent(UIManager.GetUI("UITroopFormation.PnlFormation.ScrollView").transform);

            this.viewModel.ShowCurrentHero();
            this.viewPref.canvasGroup.blocksRaycasts = true;
        }

        private void FormatPnlSelect() {
            if (this.ui.gameObject.activeSelf) {
                this.viewPref.verticalLayoutGroup.CalculateLayoutInputHorizontal();
                this.viewPref.verticalLayoutGroup.CalculateLayoutInputVertical();
                this.viewPref.contentSizeFitter.SetLayoutVertical();
            }
        }

        #region FTE

        private void HideFteArrow() {
            Transform pnlArrow = this.troopUI.Find("PnlArrow");
            if (pnlArrow != null) {
                pnlArrow.gameObject.SetActiveSafe(false);
            }
            this.viewModel.HideFteFormation();
        }

        private void ShowFteArrow() {
            Transform pnlArrow = this.troopUI.Find("PnlArrow");
            if (pnlArrow != null) {
                pnlArrow.gameObject.SetActiveSafe(true);
            }

            this.viewModel.ShowFteFormation();
        }

        #endregion
    }
}
