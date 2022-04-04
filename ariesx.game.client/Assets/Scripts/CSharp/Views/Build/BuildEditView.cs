using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildEditView : BaseView {
        private BuildEditViewModel viewModel;
        private BuildEditViewPreference viewPref;


        /*************/
        private Transform buildEditor;
        private BuildEdirotPrefabView buildEdirotViewPref;

        private Vector2 previouseCoordinate;
        private Vector2 previouse;
        private Vector2 previouseTemp;
        private const int editorLayerOffset = -4;
        private string AvaliableSpritePath {
            get {
                return "build_editor_avaliable";
            }
        }
        private string UnavaliableSpritePath {
            get {
                return "build_editor_unavaliable";
            }
        }

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BuildEditViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBuildEditor");
            this.viewPref = this.ui.transform.GetComponent<BuildEditViewPreference>();
            this.viewPref.btnOK.onClick.AddListener(this.OnBtnOKClick);
            this.viewPref.btnCancel.onClick.AddListener(this.OnBtnCancelClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCancelClick);

            this.buildEditor = PoolManager.GetObject(PrefabPath.buildEditor, null).transform;// (UnityEngine.Resources.Load<GameObject>("BuildEditor")).transform;
            this.buildEdirotViewPref = this.buildEditor.GetComponent<BuildEdirotPrefabView>();
            this.buildEditor.position += Vector3.forward * editorLayerOffset;
            this.buildEdirotViewPref.drag.onDrag.AddListener(this.OnDrag);
            this.buildEditor.gameObject.SetActiveSafe(false);
        }

        public void SetBuildEditor() {
            Vector2 coordinate = this.viewModel.IsFixed ?
                this.viewModel.FixedCoord : this.viewModel.GetNearestTile();
            if (coordinate == RoleManager.GetRoleCoordinate()) {
                this.viewModel.ShowGetTileTip();
            } else {
                this.ShowEditorButtons(coordinate);
            }
        }

        private void ShowEditorButtons(Vector2 coordinate) {
            int level = 1;
            string building = this.viewModel.Building;
            ElementBuilding elementBuilding;
            if (this.viewModel.BuildingDict.TryGetValue(building, out elementBuilding)) {
                level = elementBuilding.Level;
            }
            this.viewModel.JumpTo(coordinate);
            this.SetBtnOKEnable();
            this.buildEditor.position = MapUtils.CoordinateToPosition(coordinate);
            AnimationManager.Animate(
                this.buildEdirotViewPref.building,
                "Wave", null, loop: true, space: PositionSpace.SelfWorld);
            this.viewModel.BuildCoordinate =
                MapUtils.PositionToCoordinate(this.buildEditor.position);
            BuildingConf buildConf = ConfigureManager.GetConfById<BuildingConf>(
                    string.Concat(building, "_", level));
            this.buildEdirotViewPref.buildSprite.sprite = ArtPrefabConf.GetSprite(
                 string.Concat(SpritePath.tileLayerAbovePrefix, buildConf.type, level));
            this.buildEditor.position = new Vector3(
                this.buildEditor.position.x,
                this.buildEditor.position.y,
                editorLayerOffset
            );
        }

        public void OnCoordinateChange() {
            if (this.viewModel.IsAvaliable(this.viewModel.BuildCoordinate)) {
                this.buildEdirotViewPref.buildingBase.sprite = ArtPrefabConf.GetSprite(this.AvaliableSpritePath);
                this.viewPref.btnOK.gameObject.SetActiveSafe(true);
            } else {
                this.buildEdirotViewPref.buildingBase.sprite = ArtPrefabConf.GetSprite(this.UnavaliableSpritePath);
                this.viewPref.btnOK.gameObject.SetActiveSafe(false);
            }
        }

        public void SetBtnOKEnable() {
            this.viewPref.btnOK.interactable = true;
        }

        public void SetBtnOKDisable() {
            this.viewPref.btnOK.interactable = false;
        }

        private void OnBtnOKClick() {
            this.viewModel.SendBuildRequest();
            this.viewModel.SetIsFixd();
            Debug.Log("Set Fause");
        }

        private void OnBtnCancelClick() {
            this.viewModel.Hide();
        }

        private void OnDrag(Vector2 position) {
            Vector2 current = MapUtils.ScreenToWorldPoint(position);
            Vector2 temp = MapUtils.PositionToCoordinate(current);
            this.viewModel.BuildCoordinate = temp;
            this.buildEditor.position = (Vector3)MapUtils.CoordinateToPosition(temp) +
                Vector3.forward * editorLayerOffset;
        }

        #region FTE

        public void OnFteStep61Start() {
            StartCoroutine(this.OnFteStep61DelayStart());
        }

        public IEnumerator OnFteStep61DelayStart() {
            yield return YieldManager.EndOfFrame;
            this.buildEdirotViewPref.buildArrow.gameObject.SetActiveSafe(false);
            if (this.viewPref.btnOK.gameObject.activeSelf) {
                FteManager.SetMask(
                    this.viewPref.btnOK.pnlContent,
                    this.viewPref.btnCancel.pnlContent,
                    isButton: true,
                    isEnforce: true
                );
            } else {
                this.viewModel.StartChapterDailyGuid();
                FteManager.StopFte();
            }
        }

        public void OnFteStep61End() {
            this.buildEdirotViewPref.buildArrow.gameObject.SetActiveSafe(true);
        }

        public void OnBuildStep3Start() {
            if (this.viewPref.btnOK.gameObject.activeSelf) {
                StartCoroutine(DelaySetMask());
            } else {
                this.viewModel.StartChapterDailyGuid();
                FteManager.StopFte();
            }
        }
        private IEnumerator DelaySetMask() {
            yield return YieldManager.EndOfFrame;
            bool isEnforce = FteManager.Instance.curStep.CustomEquals("chapter_task_3");
             FteManager.SetMask(this.viewPref.btnOK.pnlContent, isButton: true, isEnforce: isEnforce);
        }
        public void OnBuildStep4Start() {
            if (this.viewPref.btnOK.gameObject.activeSelf) {
                this.viewModel.SendBuildRequest();
            }
        }

        #endregion

        protected override void OnVisible() {
            this.buildEditor.gameObject.SetActiveSafe(true);
            UIManager.UIBind(this.viewPref.pnlButtons, this.buildEditor,
                Vector2.one * 4, BindDirection.Up, BindCameraMode.None);
        }

        protected override void OnInvisible() {
            this.buildEditor.gameObject.SetActiveSafe(false);
            AnimationManager.Finish(this.buildEdirotViewPref.building);
        }
    }
}
