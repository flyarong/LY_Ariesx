using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class RankView: BaseView {
        private RankViewModel viewModel;
        private RankViewPreference viewPref;
        private Vector2 tabBtnClose = new Vector2(317, -71);
        //private bool IsShowOwnRankInfo = false;

        // Rank Order
        public const int mapSN = 0;
        public const int personal = 1;
        public const int alliance = 2;
        public const int occupation = 3;


        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRank");
            this.viewModel = this.GetComponent<RankViewModel>();
            this.viewPref = this.ui.transform.GetComponent<RankViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.SetTabView();
        }

        public void SetTabView() {
            this.viewPref.tabView.InitTab(4);
            this.viewPref.tabView.SetTab(mapSN,
                 new TabInfo(LocalManager.GetValue(LocalHashConst.rank_state),
                this.viewPref.pnlMapSNRank, (state) => {
                    OnToggleValueChange(RankInfoType.MapSN, state);
                }));
            this.viewPref.tabView.SetTab(personal,
                new TabInfo(LocalManager.GetValue(LocalHashConst.rank_person),
                this.viewPref.pnlPersonal, (state) => {
                    OnToggleValueChange(RankInfoType.Personal, state);
                }));
            this.viewPref.tabView.SetTab(alliance,
                 new TabInfo(LocalManager.GetValue(LocalHashConst.rank_alliance),
                this.viewPref.pnlAlliance, (state) => {
                    OnToggleValueChange(RankInfoType.Alliance, state);
                }));
            this.viewPref.tabView.SetTab(occupation,
                 new TabInfo(LocalManager.GetValue(LocalHashConst.rank_occupation),
                this.viewPref.pnlSiegeRank, (state) => {
                    OnToggleValueChange(RankInfoType.Occupation, state);
                }));

            this.viewPref.tabView.btnClose.transform.localPosition = tabBtnClose;
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
        }


        public void SetInfo() {
            this.viewPref.tabView.SetActiveTab(0);
        }

        public void SetOwnRankInfoVisible(bool visible) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.ownRankInfo, visible);
        }

        public void SetOwnRankInfo() {
            // To do : Set the Channel bottom value
            bool isShowOwnRankInfo = this.viewModel.IsShowOwnRankInfo();
            UIManager.SetUICanvasGroupEnable(this.viewPref.ownRankInfo, isShowOwnRankInfo);
            this.viewPref.channelRectTransform.offsetMin =
                new Vector2(this.viewPref.channelRectTransform.offsetMin.x,
                isShowOwnRankInfo ? 138f : 50f);
            if (isShowOwnRankInfo) {
                GameHelper.ClearChildren(this.viewPref.pnlOwnRankInfo);
                GameObject ownRank = null;
                switch (this.viewModel.Channel) {
                    case RankInfoType.Personal:
                        ownRank = PoolManager.GetObject(
                            PrefabPath.pnlRankPlayerItem, this.viewPref.pnlOwnRankInfo);
                        RankPlayerItemView playerItemView =
                            ownRank.transform.GetComponent<RankPlayerItemView>();
                        playerItemView.PersonalRankData =
                            this.viewModel.GetPersonalOwnRankInfo();
                        return;
                    case RankInfoType.Alliance:
                        ownRank = PoolManager.GetObject(
                            PrefabPath.pnlRankAllianceItem, this.viewPref.pnlOwnRankInfo);
                        RankAllianceItemView allianceItemView =
                            ownRank.transform.GetComponent<RankAllianceItemView>();
                        allianceItemView.AllianceRankData =
                            this.viewModel.GetAllianceOwnRankInfo();
                        return;
                    case RankInfoType.Occupation:
                        ownRank = PoolManager.GetObject(
                            PrefabPath.pnlRankOccupationItem, this.viewPref.pnlOwnRankInfo);
                        RankOccupationItemView occupationItemView =
                            ownRank.transform.GetComponent<RankOccupationItemView>();
                        occupationItemView.OccupationRankData =
                            this.viewModel.GetOccupationOwnRankInfo();
                        break;
                    case RankInfoType.MapSN:
                        ownRank = PoolManager.GetObject(
                            PrefabPath.pnlMapSNRankItem, this.viewPref.pnlOwnRankInfo);
                        MapSNRankItemView MapSnItemView =
                            ownRank.transform.GetComponent<MapSNRankItemView>();
                        MapSnItemView.PersonalRankData =
                            this.viewModel.GetMapSNSelfRankInfo();
                        return;
                    default:
                        break;
                }
                if (ownRank != null) {
                    RectTransform ownRankRT = ownRank.GetComponent<RectTransform>();
                    ownRankRT.anchorMin = Vector2.zero;
                    ownRankRT.anchorMax = Vector2.one;
                }
            }
        }
        /********************* private methods ******************************/
        private void OnToggleValueChange(RankInfoType rankType, bool state) {
            if (state) {
                this.viewModel.Channel = rankType;
            }
        }

        private void OnBtnOwnRankClick() {
            this.viewModel.JumpToOwnRank();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        #region Occupation intro
        public void ShowOccupationIntro() {
            UIManager.SetUICanvasGroupEnable(this.viewPref.occupationIntroCG, true);
            AnimationManager.Animate(this.viewPref.occupationIntro, "Show", () => {
                this.viewPref.btnOccupationBG.onClick.AddListener(this.HideOccupationIntro);
                this.viewPref.btnOccupationClose.onClick.AddListener(this.HideOccupationIntro);
            });
        }

        private void HideOccupationIntro() {
            AnimationManager.Animate(this.viewPref.occupationIntro, "Hide", () => {
                UIManager.SetUICanvasGroupEnable(this.viewPref.occupationIntroCG, false);
                this.viewPref.btnOccupationBG.onClick.RemoveAllListeners();
                this.viewPref.btnOccupationClose.onClick.RemoveAllListeners();
            });
        }
        #endregion


        protected override void OnInvisible() {
            base.OnInvisible();
            this.viewPref.tabView.SetAllOff();
        }
    }
}
