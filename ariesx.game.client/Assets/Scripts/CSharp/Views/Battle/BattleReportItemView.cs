using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using TMPro;
using System.Text;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class BattleReportItemView : BaseItemViewsHolder {
        private BattleReport battleReport;
        public BattleReport BattleReport {
            get {
                return this.battleReport;
            }
            set {
                if (this.battleReport != value) {
                    this.battleReport = value;
                    this.SetArmyInfo();
                    this.OnReadStatusChange();
                    this.SetResources();
                }
            }
        }

        public string AttackerName { get; set; }
        public string DefenderName { get; set; }

        public string BattleWinSpriteWin {
            get {
                return "mail_battle_win";
            }
        }

        public string BattleWinSpriteLose {
            get {
                return "mail_battle_lose";
            }
        }

        public string BattleAttacker {
            get {
                return "mail_battle_role_attacker";
            }
        }

        public string BattleDefender {
            get {
                return "mail_battle_role_defender";
            }
        }

        public UnityEvent OnBtnDetailClick {
            get {
                this.btnDetail.onClick.RemoveAllListeners();
                return this.btnDetail.onClick;
            }
        }

        public UnityEvent OnBtnShareClick {
            get {
                this.btnShare.onClick.RemoveAllListeners();
                return this.btnShare.onClick;
            }
        }
        public UnityEvent OnBtnPlayClick {
            get {
                this.btnPlay.onClick.RemoveAllListeners();
                return this.btnPlay.onClick;
            }
        }

        public UnityEvent OnBtnItemClick {
            get {
                this.btnItem.onClick.RemoveAllListeners();
                return this.btnItem.onClick;
            }
        }

        public bool IsRead {
            get {
                return this.battleReport.IsRead;
            }
            set {
                this.battleReport.IsRead = value;
                this.OnReadStatusChange();
            }
        }

        #region UI component cache
        [SerializeField]
        private Image imgBG;
        [SerializeField]
        private ContentSizeFitter contentSizeFitter;
        [SerializeField]
        private Transform imgNewMark;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private TextMeshProUGUI txtCoordinate;
        [SerializeField]
        private Transform pnlAttacker;
        [SerializeField]
        private Transform pnlDefender;
        [SerializeField]
        private Transform pnlRewards;
        [SerializeField]
        private Transform pnlResourcesList;
        [SerializeField]
        private TextMeshProUGUI txtResult;
        [SerializeField]
        private Transform pnlOccupation;
        [SerializeField]
        private TextMeshProUGUI txtTerritoryStatus;
        [SerializeField]
        private Button btnPlay;
        [SerializeField]
        private Button btnDetail;
        [SerializeField]
        private Button btnShare;
        [SerializeField]
        private Button btnItem;
        [SerializeField]
        private GameObject imgLandResult;
        [SerializeField]
        private TextMeshProUGUI TxtLandResult;

        #endregion
        private bool isAnyResource = false;
        private bool isDefender = false;
        private bool isRaid = false;
        private bool isMonster = false;
        private int monsterLevel = 0;
        private bool isBoss = false;
        private int BossLevel = 0;
        private bool isNeutralBattle = false;
        private bool isWin = false;
        private readonly int NEXT_TROOP_TYPE_DEFENDER = 1;
        private readonly int NEXT_TROOP_TYPE_ENDURANCE_DEFENDER = 2;
        // Methods
        public override void MarkForRebuild() {
            base.MarkForRebuild();
            contentSizeFitter.enabled = true;
        }

        private void SetInfoDetail() {
            this.txtTime.text = GameHelper.DateFormat(
               this.battleReport.Timestamp
            );
            PointInfo pointInfo = this.battleReport.Report.PointInfo;

            string stateCoord = string.Empty;
            if (pointInfo.MapSN == 0 || pointInfo.ZoneSN == 0) {
                stateCoord = string.Empty;
            } else {
                string mapSNLocal = NPCCityConf.GetMapSNLocalName(pointInfo.MapSN);
                string zoneLocal = NPCCityConf.GetZoneSnLocalName(pointInfo.MapSN, pointInfo.ZoneSN);
                stateCoord = string.Concat(mapSNLocal, ",", zoneLocal, ", ");
            }
            StringBuilder s = new StringBuilder();
            s.AppendFormat(" [{0}, {1}]", pointInfo.Coord.X, pointInfo.Coord.Y);
            this.txtCoordinate.text = string.Concat(
                stateCoord,
                pointInfo.GetBattleOccureTileName(),
                s.ToString()
            );
        }

        private void SetArmyInfo() {
            this.SetInfoDetail();
            Battle.PlayerInfo attackerInfo;
            Battle.PlayerInfo defenderInfo;
            this.isWin = this.SetResultInfo(out attackerInfo, out defenderInfo);
            Battle.Result result = this.battleReport.Report.Result;
            this.SetFormat(defenderInfo, defenderInfo.BeforeHeroes, this.pnlDefender,
                            defenderInfo.BasicInfo, result, !isWin, isDefender);
            this.SetFormat(attackerInfo, attackerInfo.BeforeHeroes, this.pnlAttacker,
                            attackerInfo.BasicInfo, result, isWin, !isDefender);
            this.imgBG.sprite =
                ArtPrefabConf.GetSprite(SpritePath.battleReportBGPrefix,
                isWin ? "victory" : "failure");
        }

        private bool SetResultInfo(out Battle.PlayerInfo attackerInfo,
                                   out Battle.PlayerInfo defenderInfo) {
            string roleOwnId = RoleManager.GetRoleId();

            string defenderId = this.battleReport.Report.Defender.BasicInfo.Id;
            string attackerId = this.battleReport.Report.Attacker.BasicInfo.Id;
            this.isDefender = (defenderId.CustomEquals(roleOwnId) && !attackerId.CustomEquals(roleOwnId));
            this.isRaid = (defenderId.CustomEquals(roleOwnId) && attackerId.CustomEquals(roleOwnId));
            this.isMonster = defenderId.Contains("monster");
            this.isBoss = defenderId.Contains("boss");
            if (this.isBoss) {
                this.BossLevel = int.Parse(defenderId.CustomSplit('+')[1]);
            }
            if (this.isMonster) {
                this.monsterLevel = int.Parse(defenderId.CustomSplit('+')[1]);
            }
            if (this.isDefender) {
                attackerInfo = this.battleReport.Report.Defender;
                defenderInfo = this.battleReport.Report.Attacker;
            } else {
                attackerInfo = this.battleReport.Report.Attacker;
                defenderInfo = this.battleReport.Report.Defender;
            }
            this.isNeutralBattle = string.IsNullOrEmpty(defenderInfo.BasicInfo.Name);
            string battleWinner = this.battleReport.Report.Winner;
            bool win = (battleWinner.CustomEquals("attacker") && !this.isDefender) ||
                       (battleWinner.CustomEquals("defender") && this.isDefender);

            this.txtResult.colorGradient = win ?
               ArtConst.victoryVertexGradient : ArtConst.failureVertexGradient;
            this.txtResult.text = string.Concat(
                this.isDefender ? LocalManager.GetValue(LocalHashConst.mail_battle_report_defence) :
                    LocalManager.GetValue(LocalHashConst.mail_battle_report_attack),
                " ", win ? LocalManager.GetValue(LocalHashConst.mail_battle_victory) :
                    LocalManager.GetValue(LocalHashConst.mail_battle_failure)
            );
            return win;
        }

        private void SetFormat(Battle.PlayerInfo amountInfo, List<Battle.Hero> heroList, Transform participants,
           Battle.BasicInfo playerInfo, Battle.Result result, bool win, bool isAttacker) {

            HeroHeadView heroHeadView =
                participants.Find("PnlHero").Find("PnlHeroBig").GetComponent<HeroHeadView>();
            this.SetBattleHeroInfo(heroList, heroHeadView, win);
            TextMeshProUGUI txtName = participants.Find("TxtName").GetComponent<TextMeshProUGUI>();
            Slider sliderTroop = participants.Find("PnlTroop").Find("SliTroop").GetComponent<Slider>();
            TextMeshProUGUI txtSliderAmount = participants.Find("PnlTroop").Find("TxtAmount").GetComponent<TextMeshProUGUI>();
            txtSliderAmount.text = this.GetAfterHeroesAmount(amountInfo).ToString() + "/" + this.GetBattleDeadAmount(amountInfo).ToString();
            sliderTroop.maxValue = this.GetBattleDeadAmount(amountInfo);
            sliderTroop.value = this.GetAfterHeroesAmount(amountInfo);
            Image allianceBG = participants.Find("ImgAlliance").GetComponent<Image>();
            Image imgAllianceIcon = allianceBG.transform.Find("ImgAllianceIcon").GetComponent<Image>();
            TextMeshProUGUI txtAllianceName = participants.Find("TxtAllianceName").GetComponent<TextMeshProUGUI>();
            RectTransform rtAllianceName = txtAllianceName.GetComponent<RectTransform>();
            PointInfo pointInfo = this.battleReport.Report.PointInfo;
            allianceBG.gameObject.SetActive(!(this.isBoss || this.isMonster));

            if (result != null) {
                if (isAttacker) {
                    if (playerInfo.Id == RoleManager.GetRoleId()) {
                        txtName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
                    } else {
                        txtName.text = playerInfo.Name;
                    }
                    this.SetAllianceInfo(allianceBG, imgAllianceIcon, txtAllianceName, playerInfo);
                } else {
                    if (this.isMonster) {
                        txtName.text = string.Concat(
                            HeroAttributeConf.GetLocalName(heroList[0].Name),
                            string.Format(
                                LocalManager.GetValue(LocalHashConst.melee_map_level),
                                this.monsterLevel
                            )
                        );
                        txtAllianceName.text = string.Empty;
                    } else if (this.isBoss) {
                        txtName.text = string.Format(LocalManager.GetValue(LocalHashConst.domination_detail_name), this.BossLevel);
                        txtAllianceName.text = string.Empty;
                    } else if (result.PvpInfo != null) {
                        if (playerInfo.Id == RoleManager.GetRoleId()) {
                            txtName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
                        } else {
                            txtName.text = playerInfo.Name;
                        }
                        this.SetAllianceInfo(allianceBG, imgAllianceIcon, txtAllianceName, playerInfo);
                    } else if (result.PveInfo != null) {
                        txtName.text = string.Format(
                            LocalManager.GetValue(LocalHashConst.battle_result_defender),
                            result.PveInfo.CurrentNumber
                        );
                        txtAllianceName.text = pointInfo.GetBattleOccureTileName();
                        allianceBG.gameObject.SetActiveSafe(false);
                        this.HideAllianceIcon(rtAllianceName);
                    } else if (result.SiegeInfo != null) {
                        txtName.text = LocalManager.GetValue(LocalHashConst.battle_result_city_defender);
                        txtAllianceName.text = pointInfo.GetBattleOccureTileName();
                        allianceBG.gameObject.SetActiveSafe(false);
                        this.HideAllianceIcon(rtAllianceName);
                    }
                }
            }
            //Debug.LogError((isAttacker ?"是攻击者":"不是攻击者")+ txtName.text);
            if (isAttacker) {
                this.AttackerName = txtName.text;
            } else {
                this.DefenderName = txtName.text;
            }
        }

        private void ShowAllianceIcon(RectTransform rect) {
            rect.offsetMin = new Vector2(77, rect.offsetMin.y);
        }

        private void HideAllianceIcon(RectTransform rect) {
            rect.offsetMin = new Vector2(50, rect.offsetMin.y);
        }

        private void SetAllianceInfo(Image allianceBG, Image imgAllianceEmblem,
            TextMeshProUGUI txtAllianceName, Battle.BasicInfo playerInfo) {
            allianceBG.gameObject.SetActiveSafe(true);
            this.ShowAllianceIcon(txtAllianceName.GetComponent<RectTransform>());
            if (playerInfo.AllianceName.CustomIsEmpty()) {
                txtAllianceName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_no_alliance);
                imgAllianceEmblem.sprite = ArtPrefabConf.GetAliEmblemSprite(0);
            } else {
                int allianceId = playerInfo.AllianceEmblem;
                imgAllianceEmblem.sprite = ArtPrefabConf.GetAliEmblemSprite(allianceId);
                txtAllianceName.StripLengthWithSuffix(playerInfo.AllianceName);
            }
        }

        private void SetBattleHeroInfo(List<Battle.Hero> heroList, HeroHeadView heroHead, bool result) {
            if (heroList.Count > 0) {
                heroList.Sort((a, b) => {
                    return a.Position.CompareTo(b.Position);
                });
                Battle.Hero hero = heroList[0];
                heroHead.IsEnable = result;
                heroHead.SetHero(hero, showPower: false, showLevel: false, showHeroStatus: false,
                    showStar: false, showRole: false, showInfo: false);
            }
        }

        private int GetAfterHeroesAmount(Battle.PlayerInfo AfterHeroes) {
            int count = AfterHeroes.AfterHeroes.Count;
            int deadAmount = 0;
            int maxdeadAmount = 0;
            for (int i = 0; i < count; i++) {
                deadAmount = AfterHeroes.AfterHeroes[i].ArmyAmount;
                maxdeadAmount = maxdeadAmount + deadAmount;
            }
            return maxdeadAmount;
        }

        private int GetBattleDeadAmount(Battle.PlayerInfo BeforeHeroes) {
            int count = BeforeHeroes.BeforeHeroes.Count;
            int deadAmount = 0;
            int maxdeadAmount = 0;
            for (int i = 0; i < count; i++) {
                deadAmount = BeforeHeroes.BeforeHeroes[i].ArmyAmount;
                maxdeadAmount = maxdeadAmount + deadAmount;
            }
            return maxdeadAmount;
        }

        private void SetResources() {
            if (this.battleReport.Report.Result != null) {
                this.SetResultDetail();
            } else {
                this.SetPointOccupiedInfo();
            }
            this.isAnyResource = this.SetShowResouce();
            this.pnlRewards.gameObject.SetActiveSafe(this.isAnyResource);
            if (this.isAnyResource) {
                Dictionary<Resource, int> attachmentsDict =
                    this.battleReport.GetAttachmentsDict();
                GameHelper.ResizeChildreCount(this.pnlResourcesList,
                    attachmentsDict.Count, PrefabPath.pnlBuildResourceItem);
                ResourceItemView resourceItemView = null;
                int index = 0;
                foreach (var value in attachmentsDict) {
                    resourceItemView = this.pnlResourcesList.GetChild(index++).
                        GetComponent<ResourceItemView>();
                    resourceItemView.SetResource(value.Key, value.Value);
                }
            }
        }

        private bool SetShowResouce() {
            return this.battleReport.Resources.Lumber != 0 ||
                   this.battleReport.Resources.Steel != 0 ||
                   this.battleReport.Resources.Marble != 0 ||
                   this.battleReport.Resources.Food != 0 ||
                   this.battleReport.Currency.Gold != 0;
        }

        private void SetResultDetail() {
            Battle.Result result = this.battleReport.Report.Result;
            string resultStr = string.Empty;
            this.SetLandResultSeal(false, true);
            if (this.isDefender) {
                resultStr = this.isWin ?
                    LocalManager.GetValue(LocalHashConst.battle_result_defence_win) :
                    this.GetDefendLoseResultDetail();
            } else {
                //string campaignBattle = string.Empty;
                //bool isCampaignBattle = this.isBoss || this.isMonster;
                //Debug.LogError(isCampaignBattle);
                //if (isCampaignBattle) {
                //    this.imgLandResult.gameObject.SetActiveSafe(false);
                //    campaignBattle = this.isBoss ? " boss 失败" : "monster 失败";
                //}
                if (this.isMonster || this.isBoss) {
                    this.imgLandResult.gameObject.SetActiveSafe(false);
                }
                resultStr = this.isWin ?
                    this.GetAttackWinResultDetail() : this.isBoss ? string.Concat(LocalManager.GetValue(
                        LocalHashConst.domination_battle_result_normal)) :
                    string.Concat(LocalManager.GetValue(LocalHashConst.battle_result_attack_fail), ",",
                        LocalManager.GetValue(LocalHashConst.battle_result_your_unit_get_back));
            }
            this.txtTerritoryStatus.text = resultStr;
        }

        private string GetDefendLoseResultDetail() {
            Battle.Result result = this.battleReport.Report.Result;
            string resultStr = string.Empty;
            if (result.PvpInfo != null) {
                resultStr = string.Concat(
                    LocalManager.GetValue(LocalHashConst.battle_result_defence_fail), ",",
                    LocalManager.GetValue(LocalHashConst.battle_result_go_to_townhall_or_stronghold)
                );
            } else if (result.PveInfo != null) {
                if (result.PveInfo.RemainTroops >= 0) {
                    resultStr = string.Format(
                        LocalManager.GetValue(LocalHashConst.battle_result_defender_fail),
                        result.PveInfo.RemainTroops
                    );
                }
            } else if (result.SiegeInfo != null) {
                resultStr = LocalManager.GetValue(LocalHashConst.battle_result_defence_fail);
                int pointType = this.battleReport.Report.PointInfo.ElementType;
                if (pointType == (int)ElementType.npc_city) {
                    if (result.FinalInfo.Occupy) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_city_is_occupied));
                        return resultStr;
                    }
                } else if (pointType == (int)ElementType.townhall) {
                    if (result.FinalInfo.Fallen) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_you_are_fallen));
                        return resultStr;
                    }
                } else if (this.battleReport.Report.PointInfo.Building != null &&
                    pointType != (int)ElementType.townhall) {
                    if (result.FinalInfo.Broken) {
                        resultStr = string.Concat(resultStr, ",",
                           LocalManager.GetValue(LocalHashConst.battle_result_your_building_is_destroyed));
                        return resultStr;
                    }
                }

                resultStr = LocalManager.GetValue(LocalHashConst.battle_result_city_defender_fail);
                if (result.SiegeInfo.DurabilityRemain == 0) {
                    if (result.FinalInfo.Occupy) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_tile_has_been_occupied));
                    } else {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_tile_has_not_been_occupied));
                    }
                } else {
                    resultStr = string.Concat(resultStr, ",", string.Format(
                            LocalManager.GetValue(LocalHashConst.battle_result_last_durability),
                            result.SiegeInfo.DurabilityLose
                        )
                    );
                }
            }
            return resultStr;
        }


        private string GetAttackWinResultDetail() {
            Battle.Result result = this.battleReport.Report.Result;
            string resultStr = string.Empty;
            if (result.PvpInfo != null) {
                // Debug.LogError("PvpInfo" + " " + this.battleReport.Report.Result.FinalInfo.Occupy);
                if (result.PvpInfo.RemainTroops > 0) {
                    resultStr = string.Format(
                        LocalManager.GetValue(LocalHashConst.battle_result_defeat_player_unit_0),
                        result.PvpInfo.RemainTroops);
                } else {
                    resultStr = LocalManager.GetValue(LocalHashConst.battle_result_defeat_player_unit);
                    if (result.PvpInfo.NextTroopType == NEXT_TROOP_TYPE_DEFENDER) {
                        resultStr = string.Concat(resultStr, ",",
                          LocalManager.GetValue(LocalHashConst.battle_result_about_to_defender));
                    } else if (result.PvpInfo.NextTroopType == NEXT_TROOP_TYPE_ENDURANCE_DEFENDER) {
                        resultStr = string.Concat(resultStr, ",",
                        LocalManager.GetValue(LocalHashConst.battle_result_about_to_city_defender));
                    }
                }
            } else if (result.PveInfo != null) {
                // Debug.LogError("PveInfo" + " " + this.battleReport.Report.Result.FinalInfo.Occupy);
                if (result.PveInfo.RemainTroops > 0) {
                    resultStr = string.Format(
                        LocalManager.GetValue(LocalHashConst.battle_result_defeat_defender_0),
                        result.PveInfo.RemainTroops);
                } else {
                    PointInfo point = this.battleReport.Report.PointInfo;
                    resultStr = LocalManager.GetValue(LocalHashConst.battle_result_defeat_defender);
                    if (result.FinalInfo.Occupy) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_occupy_tile));
                        this.SetLandResultSeal(true, true);
                    } else if (point.NpcCity == null && point.Pass == null &&
                        this.battleReport.Report.Defender.BasicInfo.Id.CustomIsEmpty()) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_tile_can_not_be_occupied));
                    } else {

                        if (this.isRaid) {
                            resultStr = string.Concat(resultStr, ",",
                                LocalManager.GetValue(LocalHashConst.battle_result_raid_win));
                        }

                        if (this.isMonster) {
                            resultStr = string.Format(
                                LocalManager.GetValue(LocalHashConst.melee_battle_result_monster_win),
                                this.monsterLevel, CampaignModel.MonsterLocalName);
                            this.SetLandResultSeal(false, true);
                        } else if (this.isBoss) {
                            this.SetLandResultSeal(false, true);
                            resultStr = LocalManager.GetValue(LocalHashConst.domination_battle_result_win);
                        }

                    }
                }
            } else if (result.SiegeInfo != null) {
                resultStr = LocalManager.GetValue(LocalHashConst.battle_result_defeat_city_defender);
                PointInfo pointInfo = this.battleReport.Report.PointInfo;
                int pointType = pointInfo.ElementType;
                if ((pointType == (int)ElementType.npc_city &&
                    this.battleReport.Report.PointInfo.NpcCity.IsCenter) ||
                    (pointType == (int)ElementType.pass &&
                    pointInfo.MapSN == 0 && pointInfo.ZoneSN == 0)) {
                    if (result.FinalInfo.Occupy) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_alliance_occupy_tile));
                        this.SetLandResultSeal(true, true);
                        return resultStr;
                    }
                } else if (pointType == (int)ElementType.townhall) {
                    if (result.FinalInfo.Fallen) {
                        resultStr = string.Concat(resultStr, ",",
                            LocalManager.GetValue(LocalHashConst.battle_result_player_is_fallen));
                        return resultStr;
                    }
                } else if (this.battleReport.Report.PointInfo.Building != null &&
                    pointType != (int)ElementType.townhall) {
                    if (result.FinalInfo.Broken) {
                        resultStr = string.Concat(resultStr, ",",
                           LocalManager.GetValue(LocalHashConst.battle_result_destroy_player_building));
                        return resultStr;
                    }
                }

                // citycent pass_pass.
                if (result.SiegeInfo.DurabilityRemain == 0) {
                    if (result.FinalInfo.Occupy) {
                        resultStr = string.Concat(resultStr, ",",
                               LocalManager.GetValue(LocalHashConst.battle_result_occupy_tile));
                        this.SetLandResultSeal(true, true);
                    } else if (!this.battleReport.HasAlliance) {
                        if (pointType == (int)ElementType.townhall) {
                            resultStr = string.Concat(resultStr, ",",
                                LocalManager.GetValue(LocalHashConst.battle_result_can_not_fallen));
                        } else if ((pointType == (int)ElementType.npc_city &&
                            this.battleReport.Report.PointInfo.NpcCity.IsCenter) ||
                            (pointType == (int)ElementType.pass &&
                            pointInfo.MapSN == 0 && pointInfo.ZoneSN == 0)) {
                            resultStr = string.Concat(resultStr, ",",
                               LocalManager.GetValue(LocalHashConst.battle_result_can_not_occupied_special));
                        }
                    } else {
                        resultStr = string.Concat(resultStr, ",",
                          LocalManager.GetValue(LocalHashConst.battle_result_tile_can_not_be_occupied));
                    }
                } else {
                    resultStr = string.Concat(resultStr, ",", string.Format(
                            LocalManager.GetValue(LocalHashConst.battle_result_last_durability),
                            result.SiegeInfo.DurabilityLose
                        )
                    );
                }
            }
            return resultStr;
        }

        private void SetLandResultSeal(bool isView, bool isTileOccupied) {
            this.imgLandResult.gameObject.SetActiveSafe(isView);
            if (!isView) {
                this.TxtLandResult.text = string.Empty;
                return;
            }
            this.TxtLandResult.text = isTileOccupied ?
                    LocalManager.GetValue(LocalHashConst.mail_battle_report_occupied) :
                    LocalManager.GetValue(LocalHashConst.mail_battle_report_territory);
        }

        private void SetPointOccupiedInfo() {
            string enduranceInfo = string.Empty;
            if (this.battleReport.Report.Result.FinalInfo.Occupy) {
                enduranceInfo = this.isDefender ?
                    LocalManager.GetValue(LocalHashConst.mail_battle_report_territory) :
                    LocalManager.GetValue(LocalHashConst.mail_battle_report_occupied);
            }
            this.SetLandResultSeal(this.battleReport.Report.Result.FinalInfo.Occupy, !this.isDefender);
            this.SetEnduranceInfo(enduranceInfo);
        }

        private void SetEnduranceInfo(string resultLocal) {
            bool isResultLocalValide = resultLocal.CustomIsEmpty();
            this.pnlOccupation.gameObject.SetActiveSafe(true);
            if (this.isNeutralBattle) {
                if (isResultLocalValide) {
                    this.pnlOccupation.gameObject.SetActiveSafe(false);
                } else {
                    this.txtTerritoryStatus.text = resultLocal;
                }
            } else {
                string enduranceInfo = this.GetReportEndurance();
                bool isEnduranceInfoValide = enduranceInfo.CustomIsEmpty();
                if (isResultLocalValide) {
                    if (isEnduranceInfoValide) {
                        this.pnlOccupation.gameObject.SetActiveSafe(false);
                    } else {
                        this.txtTerritoryStatus.text = enduranceInfo;
                    }
                } else {
                    if (isEnduranceInfoValide) {
                        this.txtTerritoryStatus.text = resultLocal;
                    } else {
                        this.txtTerritoryStatus.text = string.Concat(enduranceInfo, resultLocal);
                    }
                }
            }
        }

        private string GetReportEndurance() {
            if (this.battleReport.Report.Result.SiegeInfo.DurabilityLose == 0) {
                return string.Empty;
            }

            string enduranceInfo = string.Empty;
            string durabilityLost = string.Format(LocalManager.GetValue(LocalHashConst.mail_territory_durability),
                (0 - this.battleReport.Report.Result.SiegeInfo.DurabilityLose));
            if (this.battleReport.Report.Result.SiegeInfo.DurabilityLose >= 0) {
                string dorabilityRemain = string.Format(
                    LocalManager.GetValue(LocalHashConst.last_amount),
                    this.battleReport.Report.Result.SiegeInfo.DurabilityLose
                );
                enduranceInfo = string.Concat(durabilityLost, ",", dorabilityRemain);
            } else {
                enduranceInfo = durabilityLost;
            }

            return enduranceInfo;
        }

        private void OnReadStatusChange() {
            this.imgNewMark.gameObject.SetActiveSafe(!this.IsRead);
        }
    }
}
