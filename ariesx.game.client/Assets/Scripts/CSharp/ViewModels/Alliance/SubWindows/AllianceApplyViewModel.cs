using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceApplyViewModel : BaseViewModel, IViewModel {
        private AllianceSubWindowsViewModel parent;
        private AllianceModel model;
        private AllianceApplyView view;


        public string Description {
            get {
                return this.model.ApplyContent;
            }
            set {
                if (this.model.ApplyContent != value) {
                    this.model.ApplyContent = value;
                }
            }
        }

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceSubWindowsViewModel>();
            this.model = ModelManager.GetModelData<AllianceModel>();
            this.view = this.gameObject.AddComponent<AllianceApplyView>();
        }

        public void Show() {
            this.view.Show();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void HideWindow() {
            this.parent.Hide();
        }

        public void HideSelf() {
            this.view.Hide();
        }

        public void ApplyWithMessage() {
            string tmp = this.Description.Replace(" ", string.Empty);
            if (tmp.CustomIsEmpty()) {
                this.Description = LocalManager.GetValue(LocalHashConst.apply_alliance_default);
            }
            //Debug.LogError(this.Description);
            this.parent.ApplyJoinAlliance(this.Description);
            this.HideWindow();
        }
    }
}
