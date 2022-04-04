using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class FirstDownRewardViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private FirstDownRewardView view;

        /* Model data get set */
        public Dictionary<int, FieldFirstDown.Record> FieldFirstDownDict {
            get {
                return RoleManager.GetFieldRewardDict();
            }
        }
        /**********************/
        public List<int> AllFieldReward = new List<int>() {
            2,3,4,5,6,7,8,9,10,11,12,13
        };

        public bool NeedRefresh { get; set; }
        public int NewLevel {
            get; set;
        }

        private FieldFirstDownRewardNtf rewardNtf;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<FirstDownRewardView>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            NetHandler.AddNtfHandler(typeof(FieldFirstDownNtf).Name, this.FieldFirstDownNtf);
            NetHandler.AddNtfHandler(typeof(FieldFirstDownRewardNtf).Name, this.FieldFirstDownRewardNtf);
        }

        // To do: need rebuild, split two ui.
        public void Show() {
            if (!this.view.IsVisible) {
                this.view.SetRewardListView();
                this.parent.OnAddViewAboveMap(this);
            }
        }

        public void ShowFirstDownReward() {
            if (this.NeedRefresh && RoleManager.GetFDRecordMaxLevel() != 1) {
                if (!this.view.IsVisible) {
                    this.view.SetGetRewardView();
                    this.parent.OnAddViewAboveMap(this);
                }
                this.NeedRefresh = false;
            }
            this.parent.RefreshLotteryView();
        }


        // To do: show wrong when list view is visible.
        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => this.parent.OnRemoveViewAboveMap(this));
        }

        public void GetReward() {
            this.view.GetReward(this.rewardNtf);
            this.parent.ShowTopHUD();
        }

        /* Add 'NetMessageAck' function here*/
        private void FieldFirstDownNtf(IExtensible message) {
            this.parent.NeedShowFirstDownReward = true;
        }

        private void FieldFirstDownRewardNtf(IExtensible message) {
            this.rewardNtf = message as FieldFirstDownRewardNtf;
            RoleManager.Instance.NeedResourceAnimation = true;
            RoleManager.Instance.NeedCurrencyAnimation = true;

            //Debug.LogError("FieldFirstDownRewardNtf " + rewardNtf.Level + " " +
            //    this.rewardNtf.Resources.Steel + " " +
            //    this.rewardNtf.Resources.Lumber + " " +
            //    this.rewardNtf.Resources.Food + " " +
            //    this.rewardNtf.Resources.Marble);

            RoleManager.SetResource(this.rewardNtf.Resources);
            RoleManager.SetCurrency(this.rewardNtf.Currency);

            this.NewLevel = rewardNtf.Level;
            this.NeedRefresh = true;
        }
        /***********************************/
    }
}
