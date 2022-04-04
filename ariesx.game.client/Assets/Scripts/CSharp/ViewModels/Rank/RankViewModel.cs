using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public enum RankInfoType {
        None,
        Personal,
        Alliance,
        Occupation,
        MapSN
    }

    public class RankViewModel: BaseViewModel, IViewModel {
        private RankView view;
        private MapViewModel parent;
        
        

        public RankInfoType Channel {
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
        private RankInfoType channel = RankInfoType.None;

        /***** Other *****/

        private MapSNRankViewModel MapSNRank {
            get {
                if (this.mapSN == null) {
                    this.mapSN =
                        PoolManager.GetObject<MapSNRankViewModel>(this.transform);
                }
                return this.mapSN;
            }
        }
        private MapSNRankViewModel mapSN;

        private OccupationRankViewModel OccupationRank {
            get {
                if (this.occupation == null) {
                    this.occupation =
                        PoolManager.GetObject<OccupationRankViewModel>(this.transform);
                }
                return this.occupation;
            }
        }
        private OccupationRankViewModel occupation;

        private AllianceRankViewModel AllianceRank {
            get {
                if (this.allianceRank == null) {
                    this.allianceRank =
                        PoolManager.GetObject<AllianceRankViewModel>(this.transform);
                }
                return this.allianceRank;
            }
        }
        private AllianceRankViewModel allianceRank;

        private PersonalRankViewModel PersonalRank {
            get {
                if (this.personalRank == null) {
                    this.personalRank =
                        PoolManager.GetObject<PersonalRankViewModel>(this.transform);
                }
                return this.personalRank;
            }
        }
        private PersonalRankViewModel personalRank;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<RankView>();
            
        }

        public void Show() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
            }, true);
            this.view.SetInfo();           
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
                this.HideAllSubView();
                this.Channel = RankInfoType.None;
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => this.parent.OnRemoveViewAboveMap(this));
        }

        public void SetOwnRankInfo() {
            this.view.SetOwnRankInfo();
        }

        public int GetCurrentInfoListCount() {
            switch (this.Channel) {
                case RankInfoType.Alliance:
                    return this.AllianceRank.RankInfoList.Count;
                case RankInfoType.Personal:
                    return this.PersonalRank.RankInfoList.Count;
                case RankInfoType.Occupation:
                    return this.OccupationRank.RankInfoList.Count;
                case RankInfoType.MapSN:
                    return this.MapSNRank.MapSNRankInfoList.Count;
                default:
                    return -1;
            }
        }        

        public RankPlayer GetMapSNSelfRankInfo() {
            return this.MapSNRank.SelfRankInfo;
        }

        public RankPlayer GetPersonalOwnRankInfo() {
            return this.PersonalRank.OwnRankInfo;
        }

        public RankAlliance GetAllianceOwnRankInfo() {
            return this.AllianceRank.OwnRankInfo;
        }

        public RankAlliance GetOccupationOwnRankInfo() {
            return this.OccupationRank.OwnRankInfo;
        }

        public bool IsShowOwnRankInfo() {
            switch (this.Channel) {
                case RankInfoType.Alliance:
                    return this.AllianceRank.IsShowOwnRankInfo();
                case RankInfoType.Personal:
                    return this.PersonalRank.IsShowOwnRankInfo();
                case RankInfoType.Occupation:
                    return this.OccupationRank.IsShowOwnRankInfo();
                case RankInfoType.MapSN:
                    return this.MapSNRank.IsShowSelfRankInfo();
                default:
                    return false;
            }
        }

        public void JumpToOwnRank() {
            switch (this.Channel) {
                case RankInfoType.Alliance:
                    this.AllianceRank.JumpToOwnRank();
                    break;
                case RankInfoType.Personal:
                    this.PersonalRank.JumpToOwnRank();
                    break;
                case RankInfoType.Occupation:
                    this.OccupationRank.JumpToOwnRank();
                    break;
                case RankInfoType.MapSN:
                    this.MapSNRank.SetSelfRankInfo();
                    break;
                default:
                    break;
            }
        }

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void ShowAllianceInfo(string allianceId) {
            this.parent.ShowAllianceInfo(allianceId);
        }

        public void ShowOccupationIntro() {
            this.view.ShowOccupationIntro();
        }

        private void HideAllSubView() {
            this.PersonalRank.Hide();
            this.AllianceRank.Hide();
            this.OccupationRank.Hide();
            this.MapSNRank.Hide();
            this.view.SetOwnRankInfoVisible(false);
        }

        private void OnChannelChange() {
            this.HideAllSubView();
            switch (this.Channel) {
                case RankInfoType.Alliance:
                    this.AllianceRank.Show();
                    break;
                case RankInfoType.Personal:
                    this.PersonalRank.Show();
                    break;
                case RankInfoType.Occupation:
                    this.OccupationRank.Show();
                    break;
                case RankInfoType.MapSN:
                    this.MapSNRank.Show();
                    break;
                default:
                    break;
            }
        }
    }
}
