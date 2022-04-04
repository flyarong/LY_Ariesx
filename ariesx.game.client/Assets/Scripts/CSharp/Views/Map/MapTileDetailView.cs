using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapTileDetailView : BaseView {
        private MapTileDetailViewModel viewModel;
        private MapTileDetailViewPreference viewPref;

        //void Awake() {
        //    this.viewModel = this.GetComponent<MapTileDetailViewModel>();
        //}

        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<MapTileDetailViewModel>();
            this.ui = UIManager.GetUI("UITileDetail");
            this.viewPref = this.ui.gameObject.GetComponent<MapTileDetailViewPreference>();
            this.viewPref.btnDetailBackground.onClick.AddListener(this.OnBtnDetailCloseClick);
            this.viewPref.btnDetailReturn.onClick.AddListener(this.OnBtnDetailCloseClick);
        }

        public override void PlayShow() {
            base.PlayShow(this.SetDetailView, true);
            UIManager.SetUICanvasGroupEnable(this.viewPref.troopGridCG, false);
        }

        public void SetTroopGrid(NpcTroop troop, int count, int totalCount) {
            this.viewPref.txtDetailTroopPower.text =
                this.GetTroopPower(troop).ToString();
            this.viewPref.txtDetailTroopAmount.text = string.Concat(count, "/", totalCount);
            UIManager.SetUICanvasGroupEnable(this.viewPref.troopGridCG, true);
            this.viewPref.troopGridView.SetTroopGrid(troop, this.OnHeroClick);
        }

        private int GetTroopPower(NpcTroop troop) {
            if (troop.Heroes.Count < 1) {
                return 0;
            }
            int totoalPower = 0;
            foreach (NpcHero hero in troop.Heroes) {
                totoalPower += HeroAttributeConf.GetPower(hero.Name, hero.Level);
            }

            return totoalPower;
        }

        public void SetMonsterGridInfo(MonsterTroop monster) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.troopGridCG, true);
            this.viewPref.troopGridView.SetMonsterTroopGrid(monster, this.OnHeroClick);
            this.viewPref.txtDetailTile.text = string.Concat(
                LocalManager.GetValue(LocalHashConst.chapter_10_title),
                string.Format(
                    LocalManager.GetValue(LocalHashConst.hero_heroinfo_level),
                    monster.Level));
        }

        private void OnHeroClick(Hero hero, UnityAction levelCallback = null) {
            this.viewModel.ShowHeroInfo(hero, HeroInfoType.Others);
        }

        private void SetDetailView() {
            MapTileInfo tileInfo = this.viewModel.TileInfo;
            this.viewPref.txtDetailTile.text =
            this.viewPref.txtTileLevel.text = MapUtils.GetTileNameAndLevelLocal(tileInfo);
            // Set Base Info
            //TileEnemyConf tileEnemyConf = TileEnemyConf.GetConf(tileInfo.level.ToString());
            //if (tileEnemyConf == null) {
            //    return;
            //}
            //this.txtDetailDefender.text = tileEnemyConf.GetLevelInfo();

            this.viewPref.pnlDetailEndurance.gameObject.SetActiveSafe(tileInfo.isVisible);
            int maxEndurance = tileInfo.maxEndurance;
            //int endurance = maxEndurance;
            if (tileInfo.type.CustomEquals(ElementCategory.camp) ||
                (tileInfo.relation == MapTileRelation.neutral &&
                 tileInfo.type.CustomEquals(ElementCategory.resource)) ||
                !tileInfo.isVisible) {
            } else {
                maxEndurance = tileInfo.maxEndurance;
                //endurance = tileInfo.endurance;
            }
            this.viewPref.txtDetailEndurance.text =
                 string.Concat(GameHelper.GetFormatNum(tileInfo.endurance, maxLength: 4), "/",
                 GameHelper.GetFormatNum(maxEndurance, maxLength: 4)
             );
            this.viewPref.sldDetailEndurance.maxValue = maxEndurance;
            this.viewPref.sldDetailEndurance.value = tileInfo.endurance;
            this.viewPref.txtDetailTroopPower.text =
            this.viewPref.txtDetailTroopAmount.text =
                LocalManager.GetValue(LocalHashConst.net_is_waiting);

            // Reward
            Dictionary<Resource, int> rewardDict = null;
            Dictionary<Resource, int> resourceBuffDict = null;
            bool hasLimitReward = false;
            bool hasChest = true;
            bool isNpcCity = tileInfo.type.CustomEquals(ElementCategory.npc_city);
            this.viewPref.pnlTributeAddition.gameObject.SetActiveSafe(isNpcCity);
            if (isNpcCity) {
                NPCCityConf npcCityConf = NPCCityConf.GetConf(tileInfo.city.id);
                rewardDict = npcCityConf.rewardDict;
                resourceBuffDict = npcCityConf.resourceBuff;
                hasChest = !npcCityConf.isCenter;

                string citySubId = tileInfo.city.id.Substring(0, tileInfo.city.id.Length - 1);
                NPCCityConf otherCityConf = NPCCityConf.GetConf(citySubId + (npcCityConf.isCenter ? "0" : "1"));
                int tributeGoldBuff = npcCityConf.tributeGoldBuff;
                float tributeGoldBuffBonus = 0.0f;
                if (npcCityConf.isCenter) {
                    tributeGoldBuff = npcCityConf.allianceTributeGoldBuff;
                } else {
                    tributeGoldBuffBonus = otherCityConf.tributeGoldBuffBonus;
                }
                this.SetTributeBonusViewInfo(tributeGoldBuff, 
                    tributeGoldBuffBonus, NPCCityConf.GetNpcCityLocalName(npcCityConf.name, true));

                long remainTime = 0;
                hasLimitReward = tileInfo.allianceId.CustomIsEmpty() &&
                    npcCityConf.IsLimitRewardValid(out remainTime);
                if (hasLimitReward) {
                    this.viewPref.txtRemainTime.text = GameHelper.TimeFormat(remainTime);
                    this.viewPref.txtGemCount.text = npcCityConf.limitGem;
                }
            } else if (tileInfo.type.CustomEquals(ElementCategory.resource)) {
                TileRewardConf tileRewardConf =
                    TileRewardConf.GetConf(string.Concat(tileInfo.name, tileInfo.level));
                rewardDict = tileRewardConf.rewardDict;
                MapResourceProductionConf productionConf =
                    MapResourceProductionConf.GetConf(
                       string.Concat(tileInfo.name, tileInfo.level));
                resourceBuffDict = productionConf.productionDict;
            } else if (tileInfo.type.CustomEquals(ElementCategory.pass)) {
                PassConf passConf = PassConf.GetConf(tileInfo.GetTilePassId());
                rewardDict = passConf.rewardDict;
                hasChest = false;
            }
            this.viewPref.pnlLimitReward.gameObject.SetActiveSafe(hasLimitReward);
            this.SetRewardViewInfo(rewardDict, hasChest);
            this.SetResourceProductionViewInfo(resourceBuffDict);

            // Description.
            this.SetTileDescription(tileInfo);
            this.FormatView();
        }

        private void SetTributeBonusViewInfo(int tributeGoldBuff, float tributeGoldBuffBonus, string cityName) {
            this.viewPref.txtTributeGoldBuff.text =
                string.Concat("+", (tributeGoldBuff > 0 ? tributeGoldBuff.ToString() : "0/ "),
                    LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtTributeGoldBuffBonus.text = (tributeGoldBuffBonus == 0.0f) ? string.Empty :
                string.Format(
                    LocalManager.GetValue(LocalHashConst.city_tribute_gold_buff_bonus_desc),
                    cityName, tributeGoldBuffBonus * 100);
        }

        private void SetRewardViewInfo(Dictionary<Resource, int> rewardDict, bool hasChest) {
            bool containReward = (rewardDict != null && rewardDict.Count > 0);
            this.viewPref.pnlReward.gameObject.SetActiveSafe(containReward);
            if (containReward) {
                int rewardCount = rewardDict.Count + (hasChest ? 1 : 0);
                GameHelper.ResizeChildreCount(this.viewPref.pnlDetailReward,
                    rewardCount, PrefabPath.pnlItemWithCount);
                ItemWithCountView itemView = null;
                int i = 0;
                foreach (var pair in rewardDict) {
                    itemView = this.viewPref.pnlDetailReward.GetChild(i++).GetComponent<ItemWithCountView>();
                    itemView.SetResourceInfo(pair.Key, pair.Value);
                }

                if (hasChest) {
                    itemView = this.viewPref.pnlDetailReward.GetChild(i).GetComponent<ItemWithCountView>();
                    string chestLocal = string.Format(LocalManager.GetValue(LocalHashConst.level),
                        this.viewModel.TileInfo.level);
                    //string chest = GameConst.freeChestPre + Mathf.Min(12, this.viewModel.TileInfo.level);
                    //chest = LocalManager.GetValue(GameConst.chestPre, chest);
                    itemView.SetResourceInfo(chestLocal, true);
                }
            }
        }

        private void SetResourceProductionViewInfo(Dictionary<Resource, int> resourceDict) {
            bool containResource = (resourceDict != null && resourceDict.Count > 0);
            this.viewPref.pnlResourceProduct.gameObject.SetActiveSafe(containResource);
            if (containResource) {
                int productCount = resourceDict.Count;
                GameHelper.ResizeChildreCount(this.viewPref.pnlProductionsList,
                    productCount, PrefabPath.pnlTributeResourceItem);
                int i = 0;
                foreach (var pair in resourceDict) {
                    this.viewPref.pnlProductionsList.GetChild(i++).GetComponent<ResourceItemView>().SetResource(
                        pair.Key, MapResourceProductionConf.GetProduction(pair.Value));
                }
            }
        }

        private void SetTileDescription(MapTileInfo tileInfo) {
            bool isShowDescription = true;
            bool isAllyReward = false;
            switch (tileInfo.type) {
                case "pass":
                    isAllyReward =
                    isShowDescription = !tileInfo.IsTilePassBridge();
                    this.SetPassDescription(tileInfo);
                    break;
                case "building":
                    this.SetBuildingDescription(tileInfo);
                    break;
                case "npc_city":
                    isAllyReward = tileInfo.city.isCenter;
                    isShowDescription = tileInfo.city.isCenter;
                    this.SetCityDescription(tileInfo);
                    break;
                case "camp":
                    this.viewPref.txtDetailDescription.text =
                        LocalManager.GetValue(LocalHashConst.map_tile_balloon_intro);
                    break;
                default:
                    isShowDescription = false;
                    break;
            }
            this.viewPref.pnlDescription.gameObject.SetActiveSafe(isShowDescription);
            this.viewPref.txtLabelRewardTitle.text = isAllyReward ?
                LocalManager.GetValue(LocalHashConst.battle_alliance_rewards) :
                LocalManager.GetValue(LocalHashConst.battle_rewards);
        }

        private void SetPassDescription(MapTileInfo tileInfo) {
            if (tileInfo.GetPassType().Equals(GameConst.PASS_PASS)) {
                this.viewPref.txtDetailDescription.text =
                    tileInfo.pass.GetPassDescription();
            } else {
                this.viewPref.txtDetailDescription.text =
                    LocalManager.GetValue(LocalHashConst.map_tile_bridge_intro);
            }
        }

        private void SetBuildingDescription(MapTileInfo tileInfo) {
            BuildingConf buildingConf = BuildingConf.GetConf(tileInfo.buildingInfo.GetId());
            this.viewPref.txtDetailDescription.text =
                LocalManager.GetValue("detail_", buildingConf.type);
        }

        private void SetCityDescription(MapTileInfo tileInfo) {
            string cityDescTail = string.Empty;
            if (!tileInfo.city.isCenter) {
                cityDescTail = "_outside";
            }
            string cityName = MapUtils.GetTileName(tileInfo);
            NPCCityConf cityConf = NPCCityConf.GetConf(tileInfo.city.id);
            switch (cityConf.type) {
                case GameConst.CAPITAL:
                    this.viewPref.txtDetailDescription.text = string.Format(
                        LocalManager.GetValue("city_capital_desc", cityDescTail),
                        cityName
                    );
                    break;
                case GameConst.REGION_CAPITAL:
                    this.viewPref.txtDetailDescription.text = string.Format(
                        LocalManager.GetValue("city_state_desc", cityDescTail),
                        cityName
                    );
                    break;
                case GameConst.ZONE_CAPITAL:
                    this.viewPref.txtDetailDescription.text = string.Format(
                        LocalManager.GetValue("city_zone_desc", cityDescTail),
                        cityName
                    );
                    break;
                default:
                    this.viewPref.txtDetailDescription.text =
                        LocalManager.GetValue("city_normal_desc", cityDescTail);
                    break;
            }
        }

        private IEnumerator DelayFormat() {
            this.viewPref.lgTileDetail.enabled = true;
            this.viewPref.csfTileDetail.enabled = true;
            yield return YieldManager.EndOfFrame;
            this.viewPref.csfTileDetail.enabled = false;
            this.viewPref.lgTileDetail.enabled = false;
        }

        private void FormatView() {
            base.StartCoroutine(this.DelayFormat());
        }

        private void OnBtnDetailCloseClick() {
            this.viewModel.Return();
        }
    }
}
