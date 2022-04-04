using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class DemonShadowDetailsView : BaseView {
        private DemonShadowDetailsViewModel viewModel;
        private DemonShadowDetailsViewPeference viewPref;
        private Vector2 domnationCoord;

        /*************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<DemonShadowDetailsViewModel>();
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDemonShadowHoldler.PnlDemonShadow.PnlDemonShadowDetails");
            this.viewPref = this.ui.transform.GetComponent<DemonShadowDetailsViewPeference>();
            this.viewPref.btnAgainst.onClick.AddListener(this.OnClickAgainst);
            this.viewPref.btnShowRewardDetails.onClick.AddListener(this.OnClickShowRewardDetails);
            this.viewPref.btnShowRule.onClick.AddListener(this.OnClickShowRule);
            this.viewPref.btnStartAgainst.onClick.AddListener(() => {
                this.OnClickStartAganst();
            });
        }

        /// <summary>
        /// 迎敌
        /// </summary>
        private void OnClickStartAganst() {
            this.viewModel.MoveWithClick(domnationCoord);
        }

        /// <summary>
        /// 显示召唤的怪物详情
        /// </summary>
        private void OnClickShowDemonInfo(BossTroop dominaInfo) {
            this.viewModel.OnClickShowBossInfo(dominaInfo);
        }

        private void OnClickShowRule() {
            this.viewModel.OnClickShowRule();
        }

        private void OnClickShowRewardDetails() {
            this.viewModel.OnClickShowRewardDetails();
        }

        private string GetCoodZone(Vector2 coodinate) {
            return this.viewModel.resourceInfo.GetTileZone(coodinate);
        }

        private void OnClickAgainst() {
            Alliance alliance = RoleManager.GetAlliance();
            bool inAlliance = (alliance != null) && !alliance.Id.CustomIsEmpty();
            if (!inAlliance) {
                //没有加入联盟 --》弹框提示
                UIManager.ShowAlert(null,
                    LocalManager.GetValue(LocalHashConst.domination_allianceonly),
                    btnInfoLabel: LocalManager.GetValue(LocalHashConst.button_confirm));
                return;
            }
            this.viewModel.OnClickAgainst();
        }

        public void SetCampaigRewards(CommonReward reward) {
            List<ItemType> rewardItemType = reward.GetRewardItemTypes();
            int rewardsCount = rewardItemType.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlRewardsList,
                rewardsCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView itemView = null;
            int i = 0;
            for (; i < rewardsCount; i++) {
                itemView = this.viewPref.pnlRewardsList.GetChild(i).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(rewardItemType[i]);
            }
        }

        /* Propert change function */

        /***************************/

        /// <summary>
        /// 设置召唤前活动详情UI
        /// </summary>
        public void SetPnlAgainstNotOpen(bool isBoss) {
            UIManager.SetUICanvasGroupVisible(this.viewPref.pnlAgainstOpened, isBoss);
            this.viewPref.pnlAgainstOpened.interactable = isBoss;
            this.viewPref.pnlAgainstOpened.blocksRaycasts = isBoss;
            UIManager.SetUICanvasGroupVisible(this.viewPref.pnlAgainstNotOpen, !isBoss);
            this.viewPref.pnlAgainstNotOpen.interactable = !isBoss;
            this.viewPref.pnlAgainstNotOpen.blocksRaycasts = !isBoss;
        }

        /// <summary>
        /// 设置召唤后的恶魔之影活动详情UI
        /// </summary>
        /// <param name="dominationInfo">恶魔信息，如果为空则表示还没有召唤</param>
        public void SetPnlAgainstOpen(BossTroop dominaInfo) {
            if (dominaInfo == null) {//没有召唤直接退出
                Debug.LogError("No Boss Information!!!!");
                return;
            }
            this.viewPref.btnDemonIcon.onClick.RemoveAllListeners();
            this.viewPref.btnDemonIcon.onClick.AddListener(() => {
                this.OnClickShowDemonInfo(dominaInfo);
            });
            domnationCoord = dominaInfo.Coord;
            UIManager.SetUICanvasGroupEnable(this.viewPref.pnlAgainstNotOpen, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.pnlAgainstOpened, true);
            this.viewPref.txtDemonHPNum.text = string.Format("{0}/{1}", 
                HeroAttributeConf.GetBossTroopThisHP(dominaInfo),
                HeroAttributeConf.GetBossMaxHP(dominaInfo));//血量            
            this.viewPref.sliderDemonHp.maxValue = HeroAttributeConf.GetBossMaxHP(dominaInfo);
            this.viewPref.sliderDemonHp.value = HeroAttributeConf.GetBossTroopThisHP(dominaInfo);
            this.viewPref.txtDemonLevel.text = string.Format(LocalManager.GetValue(
                LocalHashConst.domination_detail_name), dominaInfo.Level.ToString());
            this.viewPref.txtFighting.text = string.Format(LocalManager.GetValue(
                LocalHashConst.hero_power) + " {0}", HeroAttributeConf.GetBossPower(dominaInfo));//战斗力
            long endTime = dominaInfo.ExistTime * 1000 - RoleManager.GetCurrentUtcTime();
            this.viewPref.txtEndTime.text = GameHelper.TimeFormat(endTime);
            this.viewPref.txtDemonPos.text = string.Concat(this.GetCoodZone(domnationCoord),
                string.Format("({0},{1})", dominaInfo.Coord.X.ToString(), dominaInfo.Coord.Y.ToString()));
        }
    }
}
