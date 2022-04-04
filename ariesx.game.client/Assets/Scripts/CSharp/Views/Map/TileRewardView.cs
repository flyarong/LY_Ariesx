using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;
using System;

namespace Poukoute {
    public class TileRewardView : BaseView {
        private TileRewardViewModel viewModel;
        private TileRewardViewPreference viewPref;

        private bool isTileRewardVisible = false;
        private Vector2 TILESIZE_MIDDLE = MapUtils.TileSize * 2.2f;

        /**************************************/

        protected override void OnUIInit() {
            this.viewModel = this.transform.GetComponent<TileRewardViewModel>();
            this.ui = UIManager.GetUI("UITileRewardBind");
            this.viewPref = this.ui.transform.GetComponent<TileRewardViewPreference>();
        }

        public void ShowTileRewardViewWithDelay(float second) {
            base.InitUI();
            AnimationManager.Finish(this.viewPref.showObj);
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            this.isTileRewardVisible = true;
            this.SetRewardInfo(second);
        }

        public void HideTileRewardInfo() {
            if (this.isTileRewardVisible) {
                this.viewPref.pnlNormalReward.gameObject.SetActiveSafe(false);
                UIManager.SetUICanvasGroupVisible(this.viewPref.normalRewardCG, false);
                this.viewPref.pnlLimitReward.gameObject.SetActiveSafe(false);
                UIManager.SetUICanvasGroupVisible(this.viewPref.limitRewardCG, false);
                this.isTileRewardVisible = false;
                this.HideImmediatly(null);
            }
        }

        private void SetRewardInfo(float second) {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            bool hasLimitReward = false;
            if ((tileInfo.city != null && tileInfo.city.isCenter)) {
                hasLimitReward = this.SetCityRewardInfo(tileInfo);
            }

            if (!hasLimitReward) {
                this.SetTileRewardInfo(tileInfo);
            }

            UIManager.UIBind(
                this.viewPref.showObj.transform,
                this.viewModel.Target,
                TILESIZE_MIDDLE,
                BindDirection.Down,
                BindCameraMode.None,
                new Vector2(0, -10)
            );

            if (this.viewModel.ShowAnimation) {
                this.PlayShow(second);
            } else {
                this.Show();
            }
        }

        private void SetTileRewardInfo(MapTileInfo tileInfo) {
            if (tileInfo.level > RoleManager.GetCurrentChestLevel() - 2) {
                this.viewPref.pnlNormalReward.gameObject.SetActiveSafe(true);
                UIManager.SetUICanvasGroupVisible(this.viewPref.normalRewardCG, true);
            }
            this.viewPref.txtChestLevel.text = string.Format(LocalManager.GetValue(LocalHashConst.melee_map_level), tileInfo.level);

            Dictionary<Resource, int> rewardDict = null;
            if (tileInfo.type.CustomEquals(ElementCategory.npc_city)) {
                NPCCityConf npcCityConf = NPCCityConf.GetConf(tileInfo.city.id);
                rewardDict = npcCityConf.rewardDict;
            } else if (tileInfo.type.CustomEquals(ElementCategory.resource)) {
                TileRewardConf tileRewardConf = TileRewardConf.GetConf(string.Concat(tileInfo.name, tileInfo.level));
                rewardDict = tileRewardConf.rewardDict;
            } else if (tileInfo.type.CustomEquals(ElementCategory.pass)) {
                PassConf passConf = PassConf.GetConf(tileInfo.GetTilePassId());
                rewardDict = passConf.rewardDict;
            }
            bool containReward = rewardDict != null && rewardDict.Count > 0;
            if (containReward) {
                this.SetTileRewardDetail(rewardDict);
            }
        }

        private bool SetCityRewardInfo(MapTileInfo tileInfo) {
            bool hasLimitReward = false;
            if (tileInfo.type.CustomEquals(ElementCategory.npc_city)) {
                NPCCityConf npcCityConf = NPCCityConf.GetConf(tileInfo.city.id);
                long remainTime = 0;
                hasLimitReward = tileInfo.allianceId.CustomIsEmpty() &&
                    npcCityConf.IsLimitRewardValid(out remainTime);
                if (hasLimitReward) {
                    this.viewPref.txtRemainTime.text = GameHelper.TimeFormat(remainTime);

                    this.viewPref.txtGemCount.text = npcCityConf.limitGem;
                    this.SetTileRewardDetail(npcCityConf.rewardDict);
                }
            }
            this.viewPref.pnlLimitReward.SetActiveSafe(hasLimitReward);
            UIManager.SetUICanvasGroupVisible(this.viewPref.limitRewardCG, hasLimitReward);
            return hasLimitReward;
        }

        private void SetTileRewardDetail(Dictionary<Resource, int> rewardDict) {
            int rewardCount = rewardDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlTileRewardList,
                rewardCount, PrefabPath.pnlItemWithCount);
            int i = 0;
            ItemWithCountView itemView = null;
            foreach (var pair in rewardDict) {
                itemView =
                    this.viewPref.pnlTileRewardList.GetChild(i++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(pair.Key, pair.Value);
            }
        }


        //private void SetProductionInfo() {
        //    Dictionary<Resource, int> productDict = null;
        //    if (this.viewModel.TileInfo.type == ElementType.npc_city.ToString()) {
        //        productDict = this.viewModel.TileInfo.city.resourceBuff;
        //    } else if (this.viewModel.TileInfo.type != ElementType.pass.ToString()) {
        //        MapResourceProductionConf mp = MapResourceProductionConf.GetConf(
        //            this.viewModel.TileInfo.name + this.viewModel.TileInfo.level
        //        );
        //        if (mp != null) {
        //            productDict = mp.productionDict;
        //        }
        //    }

        //    if (productDict != null) {
        //        int productCount = productDict.Count;
        //        GameHelper.ResizeChildreCount(this.viewPref.pnlResourcesList, 
        //            productCount, PrefabPath.pnlHourlyOutput);
        //        ResourceItemView itemView = null;
        //        int i = 0;
        //        foreach (var pair in productDict) {
        //            itemView = this.viewPref.pnlResourcesList.GetChild(i++).GetComponent<ResourceItemView>();
        //            itemView.SetResource(pair.Key.ToString().ToLower(),
        //                MapResourceProductionConf.GetProduction(pair.Value));
        //        }
        //        UIManager.UIBind(
        //          this.viewPref.pnlResourcesList.transform,
        //          this.viewModel.Target,
        //          TILESIZE_MIDDLE,
        //          BindDirection.Down,
        //          BindCameraMode.None,
        //          new Vector2(0, productDict.Count * 15)
        //      );
        //    }
        //}
    }
}
