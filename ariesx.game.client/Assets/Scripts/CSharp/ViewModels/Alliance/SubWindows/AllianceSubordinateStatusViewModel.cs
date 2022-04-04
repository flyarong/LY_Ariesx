using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class SubordinateStatusViewModel : BaseViewModel, IViewModel {
        private AllianceSubWindowsViewModel parent;
        private SubordinateStatusView view;
        private ChatRoomModel chatModel;

        public List<AllianceMessage> MessageList {
            get {
                return this.chatModel.allianceSlaveInfoList;
            }
        }

        public bool NeedRefresh {
            get; set;
        }

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceSubWindowsViewModel>();
            this.chatModel = ModelManager.GetModelData<ChatRoomModel>();
            this.view = this.gameObject.AddComponent<SubordinateStatusView>();

            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.view.SetList();
            } else {
                this.view.FormatList();
            }
        }

        public void HideSelf() {
            this.view.Hide(() => {
                this.NeedRefresh = true;
            });
        }

        public void HideWindow() {
            this.parent.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.view.SetList();
            } else {
                this.NeedRefresh = true;
            }
        }
    }
}
