using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
	public class DemonShadowHistoryRankView : BaseView {
        private DemonShadowHistoryRankViewModel viewModel;
        private DemonShadowHistoryRankViewPeference viewPref;
		/* UI Members*/

		/*************/

		void Awake() {
			this.viewModel = this.gameObject.GetComponent<DemonShadowHistoryRankViewModel>();
		}

		protected override void OnUIInit() {
			this.ui = UIManager.GetUI("UIDemonShadowRank");
            this.viewPref = this.ui.transform.GetComponent<DemonShadowHistoryRankViewPeference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            /* Cache the ui components here */
        }

        private void OnShowPlayerInfoClick(string playerId) {
            this.viewModel.ShowPlayerInfo(playerId);
        }

        /* Propert change function */

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        public void SetDominationRankCount() {
            int rankListCount = this.viewModel.rankDominationList.Count;
            //Debug.LogError("排行榜"+rankListCount);
            GameHelper.ResizeChildreCount(this.viewPref.pnlList, rankListCount,
                PrefabPath.pnlDemonShadowRankItem);
            DemonShadowRankItemView rankItemView = null;
            RankDomination rankDomination = null;
            for (int i = 0; i < rankListCount; i++) {
                int index = i;
                rankDomination = this.viewModel.rankDominationList[index];
                rankItemView = this.viewPref.pnlList.GetChild(index).GetComponent<
                   DemonShadowRankItemView>();
                rankItemView.PlayerInfoClick.AddListener(()=> {
                   this. OnShowPlayerInfoClick(rankDomination.Id);
                });
                rankItemView.SetContent( rankDomination.Name,
                    rankDomination.Points, rank: rankDomination.Rank);
            }

            if (this.viewModel.rankDominationSelf != null) {
                this.viewPref.txtOwnRank.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.campaign_rank_label), this.viewModel.rankDominationSelf.Rank);
                this.viewPref.txtPoint.text = GameHelper.GetFormatNum(this.viewModel.rankDominationSelf.Points);
            }
            this.viewPref.txtOwnName.text = RoleManager.GetRoleName();
            this.viewPref.LastStraw.gameObject.SetActiveSafe(this.viewModel.rankDominationLast != null);
            if (this.viewModel.rankDominationLast != null) {
                RankDomination last = this.viewModel.rankDominationLast;
                this.viewPref.lastitemView.PlayerInfoClick.AddListener(() => {
                    this.OnShowPlayerInfoClick(last.Id);
                });
                this.viewPref.lastitemView.SetContent(last.Name, last.Points);
            }
        }
        
        /***************************/
    }
}
