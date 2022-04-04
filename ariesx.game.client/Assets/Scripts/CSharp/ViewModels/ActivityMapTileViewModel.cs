using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;
using UnityEngine.Events;

namespace Poukoute {
    public class ActivityMapTileViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private ActivityMapTileView view;
        public CampaignModel campaignModel;
        //private MapTileModel mapTileModel;
        private MapModel mapModel;
        /* Model data get set */
        //public List<Domination> RewardBasicContent {
        //    get {
        //        return this.campaignModel.dominationRewardList;
        //    }
        //}

        public List<Domination> GetCampaignReward {
            get {
                return this.campaignModel.dominationRewardList;
            }
        }

        public long MonsterRemainTime {
            get {
                return this.campaignModel.GetDevilFightingLeftTime();
            }
        }

        public Activity RewardBasicContent {
            get {
                return this.campaignModel.chosenActivity;
            }
        }

        public List<Activity> AllActivities {
            get {
                return this.campaignModel.allActivities;
            }
        }
        public Transform Target {
            get {
                return this.parent.GetTileTargetTransform();
            }
        }

        /* Other members */
        public Vector2 coordinate;
        public BossTroop bossInfo;
        public GetMonsterByCoordAck monsterInfo;
        public RankDomination selfRank;
        public List<RankDomination> rankDomination = new List<RankDomination>();
        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            //this.mapTileModel = ModelManager.GetModelData<MapTileModel>();
            this.campaignModel = ModelManager.GetModelData<CampaignModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.view = this.gameObject.AddComponent<ActivityMapTileView>();
        }

        public void ShowMonster(Coord coord) {
            this.GetMonsterByCoorReq(coord);
            this.coordinate = coord;
            this.Show();
        }

        public void ShowDemon(Coord coord) {
            this.GetDominationByCoordReq(coord);
            this.coordinate = coord;
            this.Show();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
                //this.view.ShowAnimation();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide(this.view.HideAnimation);
            }
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void ShowMonsterDetail() {
            this.parent.Move(this.coordinate);
            this.parent.EnableAboveUICamera();
            this.parent.ShowMonsterDetail(this.monsterInfo);
        }

        public int GetMonsterLevel() {
            return this.mapModel.GetMonsterInfo(this.coordinate).Level;
        }

        public int GetRightButtonsCount() {
            return this.parent.GetRightButtonsCount();
        }

        /* Add 'NetMessageAck' function here*/
        private void GetMonsterByCoorReq(Coord coord) {
            GetMonsterByCoordReq monsterReq = new GetMonsterByCoordReq() {
                Coord = coord
            };
            NetManager.SendMessage(monsterReq, typeof(GetMonsterByCoordAck).Name,
                this.GetMonsterByCoorAck, (msg) => {
                    this.OnGetMonsterInfoError();
                }, this.OnGetMonsterInfoError);
        }

        private void GetMonsterByCoorAck(IExtensible message) {
            this.monsterInfo = message as GetMonsterByCoordAck;
            if (this.view.IsVisible && monsterInfo.Troops.Count > 0) {
                this.view.SetMonsterInfo();
            }
        }

        private void OnGetMonsterInfoError() {
            StartCoroutine(this.InnerOnGetMonsterInfoError());
        }

        private IEnumerator InnerOnGetMonsterInfoError() {
            GameManager.IsNeedShowErrorAckMsg = false;
            yield return YieldManager.EndOfFrame;
            UIManager.ShowTip(
                LocalManager.GetValue(LocalHashConst.warning_net_problem),
                TipType.Info);
            this.parent.RemoveMonsterOnTile(this.coordinate);
            GameManager.IsNeedShowErrorAckMsg = true;
        }

        private void GetDominationByCoordReq(Coord coord) {
            GetDominationByCoordReq byCoordReq = new GetDominationByCoordReq() {
                Coord = coord
            };
            NetManager.SendMessage(byCoordReq, typeof(GetDominationByCoordAck).Name,
                this.GetDominationByCoordAck);
        }

        private void GetDominationByCoordAck(IExtensible message) {
            GetDominationByCoordAck byCoordAck = message as GetDominationByCoordAck;
            this.rankDomination = byCoordAck.Other;
            this.selfRank = byCoordAck.Self;
            this.bossInfo = byCoordAck.Info;
            this.view.SetDemonInfo();
        }
        /***********************************/
    }
}
