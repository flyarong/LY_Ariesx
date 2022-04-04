using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class ForceViewModel: BaseViewModel, IViewModel {
        private PlayerInfoViewModel parent;
        private ForceView view;

        /* Model data get set */
        //public bool NeedRefresh {
        //    get; set;
        //}
        /**********************/

        /* Other members */

        public List<int> CollectedForceList = new List<int>(12);
        public List<int> CanReceiveList = new List<int>(12);
        public int CurrentLevel = -1;
        public int CurrentForce = 0;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<PlayerInfoViewModel>();
            this.view = this.gameObject.AddComponent<ForceView>();
            NetHandler.AddNtfHandler(typeof(ForceNtf).Name, this.ForceNtf);
            this.GetForceRewardStatusReq();
            // this.NeedRefresh = true;
        }

        public void Show() {
            if (PlayerPrefs.GetString("FirstOpenForce").CustomIsEmpty()) {
                FteManager.SetLeftChat(LocalManager.GetValue("fte_force_1_1_left"));
                PlayerPrefs.SetString("FirstOpenForce", "FirstOpenForce");
            }
            this.view.Show();
            this.CurrentForce = RoleManager.GetForce();
            this.CurrentLevel =
                ForceRewardConf.GetForceLevel(this.CurrentForce);
            //this.view.SetScrollRectStatePosition();
            this.view.SetForceInfo();
        }

        public void ShowDisplayBoardViewModel(string title, string content) {
            this.parent.ShowDisplayBoardViewModel(title, content);
        }

        public void Hide() {
            this.view.Hide(() => {
                this.view.SetScrollRectStatePosition();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            //this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void GetForceRewardStatusReq() {
            GetForceRewardStatusReq getForceRewardStatus =
                new GetForceRewardStatusReq();
            NetManager.SendMessage(getForceRewardStatus,
                typeof(GetForceRewardStatusAck).Name, this.GetForceRewardStatusAck);
        }

        public void CloseIconAnimation(bool isStar) {
            this.parent.CloseIconAnimation(isStar);
        }

        private void GetForceRewardStatusAck(IExtensible message) {

            GetForceRewardStatusAck forceRewardStatus = message as GetForceRewardStatusAck;
            this.CollectedForceList = forceRewardStatus.ForceRewards.Levels;
            int forceLevel = ForceRewardConf.GetForceLevel(RoleManager.GetForce());
            int levelSum = 0;
            foreach (var level in forceRewardStatus.ForceRewards.Levels) {
                levelSum += level;
            }

            this.CloseIconAnimation(
                (int)((float)(1 + forceLevel) / 2f * forceLevel) > levelSum);
            //if ((int)((float)(1 + forceLevel) / 2f * forceLevel) > levelSum) {
            //    this.CloseIconAnimation(true);
            //} else {
            //    this.CloseIconAnimation(false);
            //}
        }

        /* Add 'NetMessageAck' function here*/
        private void ForceNtf(IExtensible message) {
            //ForceNtf newTaskCountNtf = message as ForceNtf;
            //Debug.LogError("ForceNtf " + newTaskCountNtf.Force);
            this.GetForceRewardStatusReq();
            if (this.view.IsVisible) {
                this.Show();
            }
        }
        /***********************************/
    }
}
