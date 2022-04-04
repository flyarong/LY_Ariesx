using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Poukoute {
    public class ActivityMapTileView : BaseView {
        private ActivityMapTileViewModel viewModel;
        private ActivityMapTileViewPeference viewPref;

        /**************************************************************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<ActivityMapTileViewModel>();
            this.ui = UIManager.GetUI("UIActivityMapTile");
            this.viewPref = this.ui.gameObject.GetComponent<ActivityMapTileViewPeference>();
        }

        public void SetDemonInfo() {
            BossTroop info = this.viewModel.bossInfo;
            this.SetDemonRankContent();
            this.viewPref.txtBossFighting.text = HeroAttributeConf.GetBossPower(info).ToString();//战斗力
            this.viewPref.txtBossLevel.text = string.Concat(//Boss等级
                LocalManager.GetValue(LocalHashConst.field_level) + info.Level.ToString());
            this.viewPref.txtBossName.text = LocalManager.GetValue(
                LocalHashConst.domination_name);
            this.viewPref.sliRemainingBlood.maxValue = HeroAttributeConf.GetBossMaxHP(info);//最大血量
            this.viewPref.sliRemainingBlood.value = HeroAttributeConf.GetBossTroopThisHP(info);//当前剩余血量
            this.viewPref.txtSliAmount.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(HeroAttributeConf.GetBossTroopThisHP(info)),
                GameHelper.GetFormatNum(HeroAttributeConf.GetBossMaxHP(info)));

            long endTime = info.ExistTime * 1000 - RoleManager.GetCurrentUtcTime();
            this.viewPref.txtEndtime.text = GameHelper.TimeFormat(endTime);//结束时间
            if (this.viewModel.GetCampaignReward.Count != 0) {
                this.SetContentAllianceReward(
                    this.viewModel.GetCampaignReward[info.Level - 4].AllianceReward.Reward);
            }
        }

        public void SetMonsterInfo() {
            GetMonsterByCoordAck info = this.viewModel.monsterInfo;
            UIManager.SetUICanvasGroupEnable(this.viewPref.demonRankRewardBgCanvasGroup, false);
            //this.viewPref.btnDetail.gameObject.SetActive(true);
            //this.viewPref.btnDetail.onClick.RemoveAllListeners();
            //this.viewPref.btnDetail.onClick.AddListener(this.OnBtnDetailClick);
            this.viewPref.txtEndtime.text = GameHelper.TimeFormat(this.viewModel.MonsterRemainTime * 1000);
            long power = 0;
            int currentHealth = 0;
            float maxHealth = 0;
            foreach (MonsterTroop troop in info.Troops) {
                power += troop.GetPower();
                currentHealth += troop.GetCurrentHealth();
                maxHealth += troop.GetMaxHealth();
            }
            this.viewPref.txtBossFighting.text = GameHelper.GetFormatNum(power);
            this.viewPref.txtBossLevel.text = string.Format(
                LocalManager.GetValue(LocalHashConst.level), this.viewModel.GetMonsterLevel()
            );
            this.viewPref.txtBossName.text = CampaignModel.MonsterLocalName;
            this.viewPref.sliRemainingBlood.maxValue = maxHealth;
            this.viewPref.sliRemainingBlood.value = currentHealth;
            this.viewPref.txtSliAmount.text = string.Concat(
                currentHealth, "/", maxHealth);
        }

        public void ShowAnimation() {
            UIManager.UIBind(
                this.viewPref.pnlBossViewInfo,
                this.viewModel.Target,
                MapUtils.TileSize * 2.2f,
                BindDirection.Up, BindCameraMode.None,
                Vector2.up * 50
            );
            AnimationManager.Animate(this.viewPref.pnlBossViewInfo.gameObject, "Show");
        }

        public void HideAnimation() {
           // this.viewPref.btnDetail.gameObject.SetActive(false);
            AnimationManager.Animate(this.viewPref.pnlBossViewInfo.gameObject, "Hide");
            AnimationManager.Animate(this.viewPref.pnlDemonRankRewardBg.gameObject, "Hide", () => {
                UIManager.SetUICanvasGroupEnable(this.viewPref.demonRankRewardBgCanvasGroup, false);
            });
        }

        private void SetDemonRankContent() {
            int count = this.viewModel.rankDomination.Count;
            if (count > 0) {
                this.viewPref.pnlFirstRankSample.GetCount(this.viewModel.rankDomination[0].Name,
                 GameHelper.GetFormatNum(this.viewModel.rankDomination[0].Points));
            }
            if (count > 1) {
                this.viewPref.pnlSecondRankSample.GetCount(this.viewModel.rankDomination[1].Name,
                    GameHelper.GetFormatNum(this.viewModel.rankDomination[1].Points));
            }
            if (count > 2) {
                this.viewPref.pnlThirdRankSample.GetCount(this.viewModel.rankDomination[2].Name,
                   GameHelper.GetFormatNum(this.viewModel.rankDomination[2].Points));
            }
            if (this.viewModel.selfRank != null) {
                this.viewPref.txtOwnRank.text = string.Format(
                   LocalManager.GetValue(LocalHashConst.campaign_rank_label),
                   this.viewModel.selfRank.Rank);
                this.viewPref.txtOwnHarm.text = GameHelper.GetFormatNum(this.viewModel.selfRank.Points);
            }
            this.viewPref.txtOwnName.text = RoleManager.GetRoleName();
            UIManager.SetUICanvasGroupEnable(this.viewPref.demonRankRewardBgCanvasGroup, true);

            int offset = -(this.viewModel.GetRightButtonsCount() - 1) * 53;
            UIManager.UIBind(
                 this.viewPref.pnlDemonRankRewardBg,
                 this.viewModel.Target,
                 MapUtils.TileSize * 2.2f,
                 BindDirection.Down,
                 BindCameraMode.None,
                 Vector2.up * offset
             );
            AnimationManager.Animate(this.viewPref.pnlDemonRankRewardBg.gameObject, "Show");
        }

        private void SetContentAllianceReward(CommonReward reward) {
            if (reward == null)
                return;
            SetRewardItemToView(reward);
        }

        private void SetRewardItemToView(CommonReward reward) {
            Dictionary<Resource, int> resourceDict = reward.GetRewardsDict();
            int rewardCount = resourceDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.rewardList,
                rewardCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView itemView = null;
            int i = 0;
            foreach (var resourceValue in resourceDict) {
                itemView = this.viewPref.rewardList.GetChild(i++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(resourceValue.Key, resourceValue.Value);
            }
        }

        private void OnBtnDetailClick() {
            this.viewModel.ShowMonsterDetail();
        }
        /***************************/
    }
}
