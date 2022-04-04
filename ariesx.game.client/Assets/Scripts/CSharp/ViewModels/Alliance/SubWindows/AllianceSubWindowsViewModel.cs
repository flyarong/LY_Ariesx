using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceSubWindowsViewModel: BaseViewModel, IViewModel {
        private AllianceViewModel parent;
        private AllianceSubWindowsView view;
        private AllianceCreateOrJoinModel allianceCreateOrJoinModel;

        /* Model data get set */
        public List<string> AllianceSubWindowTitleConst =
            new List<string> { LocalManager.GetValue(LocalHashConst.apply_alliance_title),
                               LocalManager.GetValue(LocalHashConst.create_emblem),
                               LocalManager.GetValue(LocalHashConst.button_alliance_setting),
                               LocalManager.GetValue(LocalHashConst.alliance_subordinate_status),
                               string.Empty };

        private string _windowTitle = string.Empty;
        public string WindowTitle {
            get {
                return this._windowTitle;
            }
            set {
                if (this._windowTitle != value) {
                    this._windowTitle = value;
                    this.view.SetTitleInfo(this._windowTitle);
                }
            }
        }

        private AllianceSubWindowType WindowType {
            get {
                return this.windowType;
            }
            set {
                if (this.windowType != value) {
                    this.windowType = value;
                }
                this.OnSubWindowChange();
            }
        }
        private AllianceSubWindowType windowType = AllianceSubWindowType.None;
        private AllianceSubWindowType preWindowType = AllianceSubWindowType.None;

        public int AllianceEmblem {
            get {
                return this.allianceCreateOrJoinModel.allianceEmblem;
            }
            set {
                if (this.allianceCreateOrJoinModel.allianceEmblem != value) {
                    this.allianceCreateOrJoinModel.allianceEmblem = value;
                    this.EditViewModel.AllianceEmblem = value;
                    this.parent.OnAllianceLogoChange(value);
                }
            }
        }
        /**********************/

        /* Other members */
        private AllianceEditViewModel editViewModel;
        private AllianceEditViewModel EditViewModel {
            get
            {
                return this.editViewModel ??
                       (this.editViewModel = PoolManager.GetObject<AllianceEditViewModel>(this.transform));
            }
        }
        private AllianceLogoViewModel logoViewModel;
        private AllianceLogoViewModel LogoViewModel {
            get
            {
                return this.logoViewModel ??
                       (this.logoViewModel = PoolManager.GetObject<AllianceLogoViewModel>(this.transform));
            }
        }
        private AllianceApplyViewModel applyViewModel;
        private AllianceApplyViewModel ApplyViewModel {
            get
            {
                return this.applyViewModel ??
                       (this.applyViewModel = PoolManager.GetObject<AllianceApplyViewModel>(this.transform));
            }
        }
        private SubordinateStatusViewModel subordinateStatusViewModel;
        private SubordinateStatusViewModel SubordinateStatusViewModel {
            get
            {
                return this.subordinateStatusViewModel ?? (this.subordinateStatusViewModel =
                           PoolManager.GetObject<SubordinateStatusViewModel>(this.transform));
            }
        }
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<AllianceSubWindowsView>();
            this.allianceCreateOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.parent = this.transform.parent.GetComponent<AllianceViewModel>();
        }

        public void ApplyJoinAlliance(string message) {
            this.parent.ApplyJoinAlliance(message);
        }

        public void ShowSubWindow(AllianceSubWindowType type) {
            this.WindowType = type;
        }

        public void ShowSubWindowBlow(AllianceSubWindowType newType, AllianceSubWindowType prewType) {
            this.ShowSubWindow(newType);
            this.preWindowType = prewType;
        }

        public void ShowAllianeDetailByType(AllianceViewType type) {
            this.parent.RefreshAllianeDetailByType(type);
            this.Hide();
        }


        public void Hide() {
            if (this.preWindowType != AllianceSubWindowType.None) {
                this.HideTopWindow();
            } else {
                this.view.afterHideCallback = this.HideAllSubView;
                this.view.PlayHide();
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.HideAllSubView);
        }

        public void HideAllSubView() {
            this.EditViewModel.Hide();
            this.LogoViewModel.HideSelf();
            this.ApplyViewModel.HideSelf();
            this.SubordinateStatusViewModel.HideSelf();
            this.preWindowType = AllianceSubWindowType.None;
        }

        private void HideTopWindow() {
            this.WindowTitle = this.AllianceSubWindowTitleConst[(int)this.preWindowType];

            switch (this.WindowType) {
                case AllianceSubWindowType.Apply:
                    this.ApplyViewModel.HideSelf();
                    break;
                case AllianceSubWindowType.Logo:
                    this.LogoViewModel.HideSelf();
                    break;
                case AllianceSubWindowType.Setting:
                    this.EditViewModel.Hide();
                    break;
                case AllianceSubWindowType.SubordinateStatus:
                    this.SubordinateStatusViewModel.HideSelf();
                    break;
                default:
                    break;
            }
            this.preWindowType = AllianceSubWindowType.None;
        }

        private void OnSubWindowChange() {
            this.WindowTitle = this.AllianceSubWindowTitleConst[(int)this.WindowType];
            bool notApply = (this.WindowType != AllianceSubWindowType.Apply);
            this.view.SetWindowsHeadVisible(notApply);
            switch (this.WindowType) {
                case AllianceSubWindowType.Apply:
                    this.ApplyViewModel.Show();
                    break;
                case AllianceSubWindowType.Logo:
                    this.LogoViewModel.Show();
                    break;
                case AllianceSubWindowType.Setting:
                    this.EditViewModel.Show();
                    break;
                case AllianceSubWindowType.SubordinateStatus:
                    this.SubordinateStatusViewModel.Show();
                    break;
                default:
                    break;
            }
            this.view.PlayShow();
        }
        /***********************************/

    }
}
