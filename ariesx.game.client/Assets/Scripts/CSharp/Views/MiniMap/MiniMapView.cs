using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MiniMapView : BaseView {
        private MiniMapViewModel viewModel;
        private MiniMapViewPreference viewPref;

        private int tabIndex = 0;
        private MiniMap miniMap;
        /**************************************************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<MiniMapViewModel>();
            this.ui = UIManager.GetUI("UIMiniMap");
            this.viewPref = this.ui.transform.GetComponent<MiniMapViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnUnfold.onClick.AddListener(this.OnBtnUnfoldClick);
            this.miniMap = this.viewPref.pnlMap.gameObject.GetComponent<MiniMap>();
            this.miniMap.btnJump.onClick.AddListener(this.OnBtnJumpClick);
            this.miniMap.onBubblePositionChange.AddListener(this.OnBubblePostionChange);
            this.miniMap.onSizeChange.AddListener(this.OnSizeChange);
            this.viewPref.tabView.SetTab(0, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.alliance_subtitle_city), null, (state) => {
                    OnToggleCityClick(state);
                }));
            this.viewPref.tabView.SetTab(1, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.map_tile_pass), null, (state) => {
                    OnTogglePassClick(state);
                }));
            this.viewPref.tabView.SetTab(2, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.button_mark), null, (state) => {
                    OnToggleMarkClick(state);
                }));
            this.viewPref.tabView.SetTab(3, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.map_tiles), null, (state) => {
                    OnToggleTileClick(state);
                }));

            foreach (Transform transform in this.miniMap.pnlColliders) {
                int index = transform.GetSiblingIndex() + 1;
                transform.GetComponent<MiniMapCollider>().onEnterState.AddListener(() => {
                    this.OnEnterCollider(index);
                });
            }
        }

        public override void PlayShow(UnityAction callback) {
            base.PlayShow(callback, true);
            this.viewPref.tabView.SetActiveTab(this.tabIndex);
        }

        public void ShowName(Vector2 coordinate, int state, string local) {
            this.miniMap.ShowName(coordinate, state, local);
        }

        public void Fold() {
            this.viewPref.btnUnfold.gameObject.SetActive(true);
            this.viewPref.miniMapRectTransform.sizeDelta = new Vector2(733, 513);
            this.viewPref.miniMapRectTransform.anchoredPosition = new Vector2(0, -410);
            Vector2 size = this.viewPref.infoRectTransform.sizeDelta;
            this.viewPref.infoRectTransform.sizeDelta = new Vector2(size.x, 533);
            //this.viewPref.pnlMap.gameObject.SetActive(false);
            this.viewPref.pnlMap.localScale = Vector3.zero;
            this.viewModel.SetTileViewStatus(true);
            this.viewModel.IsFold = true;
            UIManager.ShowFakeBack(false);
        }

        public void Unfold() {
            this.viewPref.btnUnfold.gameObject.SetActive(false);
            this.viewPref.miniMapRectTransform.sizeDelta = new Vector2(733, 1083);
            this.viewPref.miniMapRectTransform.anchoredPosition = new Vector2(0, -50);
            Vector2 size = this.viewPref.infoRectTransform.sizeDelta;
            this.viewPref.infoRectTransform.sizeDelta = new Vector2(size.x, 563);
            //this.viewPref.pnlMap.gameObject.SetActive(true);
            this.viewPref.pnlMap.localScale = Vector3.one;
            this.viewModel.SetTileViewStatus(false);
            this.SetArrowVisible(false);
            this.viewModel.IsFold = false;
            UIManager.ShowFakeBack(true);
        }

        public void SetArrowVisible(bool isVisible) {
            this.viewPref.pnlArrow.gameObject.SetActiveSafe(isVisible);
            if (isVisible) {
                AnimationManager.Animate(this.viewPref.pnlArrow.gameObject, "Show", isOffset: false);
            } else {
                AnimationManager.Stop(this.viewPref.pnlArrow.gameObject);
            }
        }

        public void OnBtnUnfoldClick() {
            this.Unfold();
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(() => {
                this.viewPref.tabView.SetAllOff();
                callback.InvokeSafe();
            });
        }

        private Vector2 CheckBorder(Vector2 coordinate) {
            coordinate.x = Mathf.Min(
                Mathf.Max(coordinate.x, this.viewModel.MiniCoordinate.x),
                this.viewModel.MaxCoordinate.x
            );
            coordinate.y = Mathf.Min(
                Mathf.Max(coordinate.y, this.viewModel.MiniCoordinate.y),
                this.viewModel.MaxCoordinate.y
            );
            return coordinate;
        }

        /* Propert change function */
        public void OnCoordinateChange() {
            this.miniMap.HideName();
            this.miniMap.SetMiniMap(this.viewModel.Coordinate);
        }

        public void OnMiniMapAllianceCoordsChange() {
            this.miniMap.OnMiniMapAllianceCoordsChange();
        }

        public void OnAllyCoordsChange() {
            this.miniMap.OnAllyCoordsChange();
        }

        /***************************/

        void OnToggleCityClick(bool status) {
            this.tabIndex = 0;
            this.viewModel.ShowCity(status);
        }

        void OnTogglePassClick(bool status) {
            this.tabIndex = 1;
            this.viewModel.ShowPass(status);
        }

        void OnToggleMarkClick(bool status) {
            this.tabIndex = 2;
            this.viewModel.ShowMark(status);
        }

        void OnToggleTileClick(bool status) {
            this.tabIndex = 3;
            this.viewModel.ShowTile(status);
        }

        void OnEnterCollider(int index) {
            if (index == 0) {
                return;
            }
            this.viewModel.State = index;
        }

        void OnBtnCityClick(Vector2 coordinate) {
            this.viewModel.Coordinate = coordinate;
        }
        
        void OnBubblePostionChange(Vector2 coordinate) {
            this.viewModel.ResetItemChosen();
            this.viewModel.Coordinate = this.CheckBorder(coordinate);
        }

        void OnSizeChange(bool isBig) {
            //if (!isBig) {
            //    this.viewModel.State = 0;
            //} else {
            //    this.viewModel.State = 1;
            //}
        }

        void OnBtnJumpClick() {
            this.viewModel.Jump();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        protected override void OnVisible() {
            this.viewModel.Coordinate = this.viewModel.Center;
            this.viewPref.btnUnfold.gameObject.SetActiveSafe(false);
            this.viewPref.pnlMap.localScale = Vector3.one;
        }

        protected override void OnInvisible() {
            this.Unfold();
            UIManager.ShowFakeBack(false);
            this.viewPref.pnlMap.localScale = Vector3.zero;
        }
    }
}
