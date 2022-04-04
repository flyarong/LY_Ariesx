using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public enum PlayerInfoType {
        None,
        Force,
        State
    }
    public class PlayerInfoViewModel : BaseViewModel, IViewModel {
        private PlayerInfoView view;
        private MapViewModel parent;


        private PlayerInfoType channel = PlayerInfoType.None;
        public PlayerInfoType Channel {
            get {
                return this.channel;
            }
            set {
                if (this.channel != value) {
                    this.channel = value;
                    this.OnChannelChange();
                }
            }
        }


        private PlayerInfoStateViewModel stateViewModle;
        private ForceViewModel forceViewModel;

        private void Awake() {
            this.view = this.gameObject.AddComponent<PlayerInfoView>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();

            this.stateViewModle =
               PoolManager.GetObject<PlayerInfoStateViewModel>(this.transform);
            this.forceViewModel =
                PoolManager.GetObject<ForceViewModel>(this.transform);
        }

        public void Show(int index = 0) {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
                this.view.SetTab(index);
            }, true);
        }

        public void Hide() {
            this.view.PlayHide(this.OnHideCallback);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        public void ShowDisplayBoardViewModel(string title, string content) {
            this.parent.ShowDisplayBoardViewModel(title, content);
        }

        public void ShowResources() {
            this.stateViewModle.ResourcesShow();
        }

        private void OnHideCallback() {
            this.parent.OnRemoveViewAboveMap(this);
            this.view.SetAllOff();
            this.Channel = PlayerInfoType.None;
        }

        public void RefreshPlayerName() {
            this.parent.RefreshPlayerName();
        }

        public void CloseIconAnimation(bool isStart) {
            this.parent.CloseIconAnimation(isStart);
        }

        private void OnChannelChange() {
            switch (this.channel) {
                case PlayerInfoType.State:
                    this.stateViewModle.Show();
                    this.forceViewModel.Hide();
                    break;
                case PlayerInfoType.Force:
                    this.forceViewModel.Show();
                    this.stateViewModle.Hide();
                    break;
                default:
                    this.stateViewModle.Hide();
                    this.forceViewModel.Hide();
                    break;
            }
        }

        public void ChangeAvatar() {
            this.parent.ChangeAvatar();
        }
    }
}