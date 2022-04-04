using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceLogoViewModel : BaseViewModel, IViewModel {
        private AllianceSubWindowsViewModel parent;
        private AllianceLogoView view;

        public bool NeedRefresh {
            get; set;
        }

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceSubWindowsViewModel>();
            this.view = this.gameObject.AddComponent<AllianceLogoView>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.view.SetLogo();
                this.NeedRefresh = false;
            } else {
                this.view.FormatPnlLogoGrid();
            }
        }
        public void HideWindow() {
            this.parent.Hide();
        }

        public void HideSelf() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void SetLogo(int emblem) {
            this.parent.AllianceEmblem = emblem;
            this.HideWindow();
        }
    }
}
