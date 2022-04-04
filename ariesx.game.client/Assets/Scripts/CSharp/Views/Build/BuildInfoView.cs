using UnityEngine;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Poukoute {
    public class BuildInfoView : BaseView {
        private BuildInfoViewModel viewModel;
        private BuildInfoViewPreference viewPref;

        private bool isShowUnlockBuildInfo = false;
        private bool canUpgrade = true;

        private string unlockCondition = string.Empty;
        private string level = string.Empty;
        private BuildingConf buildingConf = null;


        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BuildInfoViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBuildInfo");
            this.viewPref = this.ui.transform.GetComponent<BuildInfoViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnUpgrade.onClick.AddListener(this.OnBtnUpgradeClick);
            this.viewPref.btnBuildUpgrade.onClick.AddListener(this.OnBtnBuildUnlockConditionClick);
            this.viewPref.btnFeceUpgrade.onClick.AddListener(this.OnBtnForceNotEnoughClick);
            this.viewPref.btnResourceUpgrade.onClick.AddListener(this.OnBtnResourceUpgrade);
            this.afterShowCallback += () => { FteManager.HideArrow(); };
        }

        public override void PlayShow(UnityAction callback,
            bool needShowFakeBackground, float delay = 0) {
            base.PlayShow(callback, needShowFakeBackground);
        }

        public void SetInfo() {
            buildingConf = BuildingConf.GetConf(this.viewModel.BuildingAndLevel);
            this.viewPref.imgAvatar.sprite = ArtPrefabConf.GetSprite(
                string.Concat(SpritePath.tileLayerAbovePrefix,
                buildingConf.type, buildingConf.buildingLevel));
            this.viewPref.imgAvatar.SetNativeSize();
            this.viewPref.txtHealth.text = buildingConf.durability.ToString();
            if (this.viewModel.GetDurabilityBonus() != 0) {
                this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(true);
                this.viewPref.txtHealthAddition.text = "+" + Mathf.Ceil(buildingConf.durability * this.viewModel.GetDurabilityBonus()).ToString();
            } else {
                this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(false);
            }
            this.viewPref.pnlUnlockBuilds.gameObject.SetActiveSafe(false);
            bool isBuildMaxLevelRech =
                this.viewModel.IsBuildingRechMaxLevel(buildingConf.buildingName);
            if ((this.viewModel.BuildViewType == BuildViewType.Upgrade) &&
                !isBuildMaxLevelRech) {
                this.SetUpgradeInfo(buildingConf);
            } else {
                this.ShowBuildInfoView(buildingConf);
            }
        }

        //public void SetInfo(BuildingConf buildingConf) {
        //    this.viewPref.imgAvatar.sprite = ArtPrefabConf.GetSprite(
        //        string.Concat(SpritePath.tileLayerAbovePrefix,
        //        buildingConf.type, buildingConf.buildingLevel));
        //    this.viewPref.imgAvatar.SetNativeSize();
        //    this.viewPref.txtHealth.text = buildingConf.durability.ToString();
        //    if (this.viewModel.GetDurabilityBonus() != 0) {
        //        this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(true);
        //        this.viewPref.txtHealthAddition.text = "+" + Mathf.Ceil(buildingConf.durability * this.viewModel.GetDurabilityBonus()).ToString();
        //    } else {
        //        this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(false);
        //    }
        //    this.ShowBuildInfoView(buildingConf);
        //    this.SetUpgradeInfo(buildingConf);
        //    this.viewPref.pnDescription.gameObject.SetActiveSafe(true);
        //    this.SetShowUpgradeInfo(false);
        //}

        private void SetShowUpgradeInfo(bool show) {
            this.viewPref.pnlUpgradeFailInfo.gameObject.SetActiveSafe(show);
            this.viewPref.pnlUpgradeRequire.gameObject.SetActiveSafe(show);
        }

        // NOTICE!!!
        // buildingConf refer to the next level buildingConf
        private void SetUpgradeInfo(BuildingConf buildingConf) {
            this.viewPref.pnlResourcesBonus.gameObject.SetActiveSafe(true);
            this.viewPref.pnDescription.gameObject.SetActiveSafe(false);
            this.SetBuildInfoHeadInfo(buildingConf);
            this.SetUpgradeBonusInfo(buildingConf);
            this.SetUnlockBuildingInfo(buildingConf);
            string upgradeDesc = string.Empty;
            string upgradeForceDesc = string.Empty;
            string unlockBuildName = string.Empty;
            string unlockBuildType = string.Empty;
            bool showImg = false;
            bool canUpgradeAboutBuilding =
                this.viewModel.GetUpgradeConditionConf(
                    out upgradeDesc, buildingConf, out showImg, out unlockBuildName, out unlockBuildType, out this.level);
            bool canUpgradeAboutForce = this.viewModel.GetUpgradeForceConf(out upgradeForceDesc, buildingConf);
            bool canUpgrade = canUpgradeAboutBuilding && canUpgradeAboutForce;
            this.viewPref.pnlUpgradeFailInfo.gameObject.SetActiveSafe(!canUpgrade);
            this.viewPref.pnlUpgradeRequire.gameObject.SetActiveSafe(canUpgrade);
            string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
            this.unlockCondition = unlockValue[0];
            this.level = unlockValue[1];
            if (canUpgrade) {
                this.SetUpgradeRequireResourcesInfo(buildingConf);
            } else {
                if (showImg) {
                    this.viewPref.txtUpgradeInfoTitle.text =
                    LocalManager.GetValue(LocalHashConst.building_upgrade_previouse);
                } else {
                    this.viewPref.txtUpgradeInfoTitle.text =
                    LocalManager.GetValue(LocalHashConst.tip_error);
                }
                this.viewPref.txtBuildUpgrade.gameObject.SetActiveSafe(!canUpgradeAboutBuilding);
                this.viewPref.imgBuildUpgrade.gameObject.SetActiveSafe(showImg);
                if (!canUpgradeAboutBuilding) {
                    this.viewModel.FindBtnGoText(this.unlockCondition, this.level);
                    this.viewPref.txtBuildUpgrade.text = upgradeDesc;
                    this.viewPref.btnBuildUpgrade.Grayable = EventManager.IsBuildEventMaxFull();
                    if (showImg) {
                        this.viewPref.imgBuildUpgrade.sprite = ArtPrefabConf.GetSprite(
                            string.Format(
                                SpritePath.buildingIconFormat,
                                unlockBuildType,
                                Mathf.CeilToInt(buildingConf.buildingLevel / ArtConst.buildingInterval)
                            )
                        );
                    }
                } else {
                    this.viewPref.txtBuildUpgrade.text = string.Empty;
                }
                this.viewPref.pnlForceUpgrade.SetActiveSafe(!canUpgradeAboutForce);
                this.viewPref.pnlBuildUpgrade.SetActiveSafe(!canUpgradeAboutBuilding);
                if (!canUpgradeAboutForce) {
                    this.viewPref.txtForceUpgrade.text = upgradeForceDesc;
                }
            }
        }

        private void SetBuildInfoHeadInfo(BuildingConf buildingConf) {
            BuildingConf buildingPrelevelConf = BuildingConf.GetConf(
                buildingConf.buildingName + "_" + Mathf.Max(1, (buildingConf.buildingLevel - 1))
            );
            this.viewPref.txtHealth.text = buildingPrelevelConf.durability.ToString();
            this.viewPref.txtTile.text = string.Format(
                                LocalManager.GetValue(LocalHashConst.upgrade_build),
                                MapUtils.GetBuildingLocalName(buildingConf),
                                buildingConf.buildingLevel);
            int helthAddition = buildingConf.durability - buildingPrelevelConf.durability;
            if (helthAddition > 0) {
                this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(true);
                this.viewPref.txtHealthAddition.text = "+" + helthAddition;
            }
        }

        public void SetBtnUpgrade(bool enable) {
            this.viewPref.btnUpgrade.interactable = enable;
        }

        // To do: get conf by name
        #region Set building upgrade bonus info
        private void SetUpgradeBonusInfo(BuildingConf buildingConf) {
            bool upgradeBonusVisible = false;
            upgradeBonusVisible = this.SetWarehouseUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetCampUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetStrongHoldUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetProduceUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetDominantUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetSiegeUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetAttackUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetDefenseUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetDurabilityUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetMarchSpeedUpgradeBonus(buildingConf) || upgradeBonusVisible;
            upgradeBonusVisible = this.SetTributeUpgradeBonus(buildingConf) || upgradeBonusVisible;
            this.viewPref.pnlResourcesBonus.gameObject.SetActiveSafe(upgradeBonusVisible);
        }

        private bool SetStrongHoldUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals(ElementName.stronghold)) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
            string currentLevel = currentBuildingLevel.ToString();
            string nextLevel = buildingConf.buildingLevel.ToString();
            StrongholdRecruitConf nextLevelConf = StrongholdRecruitConf.GetConf(nextLevel);
            StrongholdRecruitConf currentLevelConf = StrongholdRecruitConf.GetConf(currentLevel);
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            }
            int recruitQueueBonus = nextLevelConf.recruitQueue - currentLevelConf.recruitQueue;
            string bonusPath = string.Concat(SpritePath.buildUpgradeIconPrefix, "stronghold_recruit_queue");
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.recruit_queue),
                currentLevelConf.recruitQueue.ToString(),
                string.Concat("+", recruitQueueBonus)
            );
            return true;
        }

        private bool SetWarehouseUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.Contains("storage")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            WarehouseAttributeConf nextLevelConf = WarehouseAttributeConf.GetConf(
                  buildingConf.buildingLevel.ToString()
              );
            WarehouseAttributeConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = WarehouseAttributeConf.GetConf(
                    currentBuildingLevel.ToString()
                );
            }
            long currentStorage = currentLevelConf.resourceBonus[buildingConf.buildingName];
            long nextStorage = nextLevelConf.resourceBonus[buildingConf.buildingName];
            string resourceStr = GameHelper.LowerFirstCase(buildingConf.buildingName.CustomSplit('_')[1]);
            string bonusPath = SpritePath.buildStorageIconPrefix + resourceStr;
            this.CreateUpgradeBonusInfo(
                bonusPath,
                string.Concat(
                    LocalManager.GetValue("resource_", resourceStr),
                    " ", LocalManager.GetValue(LocalHashConst.capacity)
                ),
                GameHelper.GetFormatNum(currentStorage),
                string.Format("+{0}", GameHelper.GetFormatNum(nextStorage - currentStorage))
            );
            return true;
        }

        private bool SetDominantUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.Contains("dominant")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            DominantUpConf nextLevelConf = DominantUpConf.GetConf(
                buildingConf.type,
                buildingConf.buildingLevel
            );
            DominantUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                //this.viewPref.pnlDominant.gameObject.SetActive(true);
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = DominantUpConf.GetConf(
                    buildingConf.type,
                    currentBuildingLevel
                );
            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + currentLevelConf.type.ToString().ToLower();
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.building_restraint_description),
                string.Format("{0:0}%", currentLevelConf.bonus * 100 + GameConst.DOMINANT_BASIC),
                string.Format("+{0:0}%", (nextLevelConf.bonus - currentLevelConf.bonus) * 100)
            );
            return true;
        }

        private bool SetSiegeUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("siege_up")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            SiegeUpConf nextLevelConf = SiegeUpConf.GetConf(
                buildingConf.buildingLevel.ToString()
            );
            SiegeUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = SiegeUpConf.GetConf(
                    currentBuildingLevel.ToString()
                );

            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + "hero_siege";
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue("info_siege_up"),
                currentLevelConf.GetBonus(),
                string.Format("+{0}%", (nextLevelConf.bonus - currentLevelConf.bonus) * 100)
            );
            return true;
        }

        private bool SetAttackUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("hero_attack_up")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            HeroAttackUpConf nextLevelConf = HeroAttackUpConf.GetConf(
                buildingConf.buildingLevel.ToString()
            );
            HeroAttackUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = HeroAttackUpConf.GetConf(
                    currentBuildingLevel.ToString()
                );

            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + "hero_attack";
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue("hero_attack_description"),
                currentLevelConf.attack_up.ToString(),
                string.Format("+{0}", nextLevelConf.attack_up - currentLevelConf.attack_up)
            );
            return true;
        }

        private bool SetDefenseUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("hero_defence_up")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            HeroDefenceUpConf nextLevelConf = HeroDefenceUpConf.GetConf(
                buildingConf.buildingLevel.ToString()
            );
            HeroDefenceUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = HeroDefenceUpConf.GetConf(
                    currentBuildingLevel.ToString()
                );

            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + "hero_defence";
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue("hero_defence_description"),
                currentLevelConf.defence_up.ToString(),
                string.Format("+{0}", Mathf.Ceil(nextLevelConf.defence_up - currentLevelConf.defence_up))
            );
            return true;
        }

        private bool SetDurabilityUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("durability_up")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            DurabilityUpConf nextLevelConf = DurabilityUpConf.GetConf(
                buildingConf.buildingLevel.ToString()
            );
            DurabilityUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = DurabilityUpConf.GetConf(
                    currentBuildingLevel.ToString()
                );

            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + "durability";
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.durability_description),
                (currentLevelConf.defence_up * 100).ToString() + "%",
                string.Format("+{0}", Mathf.Ceil((nextLevelConf.defence_up - currentLevelConf.defence_up) * 100)) + "%"
            );
            return true;
        }

        private bool SetMarchSpeedUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("march_speed_up")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            MarchSpeedUpConf nextLevelConf = MarchSpeedUpConf.GetConf(
                buildingConf.buildingLevel.ToString()
            );
            MarchSpeedUpConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = MarchSpeedUpConf.GetConf(
                    currentBuildingLevel.ToString()
                );
            }
            string bonusPath = SpritePath.buildUpgradeIconPrefix + "speed";
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.lose_speedup),
                string.Format("{0:0}%", currentLevelConf.percent * 100),
                string.Format("+{0:0}%", (nextLevelConf.percent - currentLevelConf.percent) * 100)
            );
            return true;
        }

        private bool SetTributeUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("tribute_gold")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            TributeBuildingConf nextLevelConf = TributeBuildingConf.GetConf(
                buildingConf.buildingName, buildingConf.buildingLevel
            );
            TributeBuildingConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = TributeBuildingConf.GetConf(
                    buildingConf.buildingName, currentBuildingLevel
                );
            }
            string bonusPath = SpritePath.buildingInfoTributeIcon;
            string localTribute = LocalManager.GetValue(LocalHashConst.tribute);
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.house_keeper_state_tribute),
                string.Format("+{0}/{1}", currentLevelConf.gold, localTribute),
                string.Format("+{0}", nextLevelConf.gold - currentLevelConf.gold)
            );
            return true;
        }

        private bool SetProduceUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.Contains("produce")) {
                return false;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            ResourceProduceConf nextLevelConf = ResourceProduceConf.GetConf(
                buildingConf.buildingName,
                buildingConf.buildingLevel
            );
            ResourceProduceConf currentLevelConf;
            if (this.viewModel.BuildViewType == BuildViewType.Info) {
                currentLevelConf = nextLevelConf;
            } else {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelConf = ResourceProduceConf.GetConf(
                    buildingConf.buildingName,
                    currentBuildingLevel
                );
            }
            string resourceStr = GameHelper.LowerFirstCase(currentLevelConf.type.ToString());
            string bonusPath = SpritePath.resourceIconPrefix + resourceStr;
            this.CreateUpgradeBonusInfo(
                bonusPath,
                string.Concat(
                    LocalManager.GetValue("resource_", resourceStr),
                    "", LocalManager.GetValue(LocalHashConst.produce)
                ),
                string.Concat(
                    GameHelper.GetFormatNum(currentLevelConf.bonus),
                    LocalManager.GetValue(LocalHashConst.fieldinfo_production)
                ),
                string.Format(
                    "+{0}",
                    GameHelper.GetFormatNum(nextLevelConf.bonus - currentLevelConf.bonus)
                 )
            );
            return true;
        }

        //public void FormatList() {
        //    this.viewPref.listVerticalLayoutGroup.CalculateLayoutInputHorizontal();
        //    this.viewPref.listVerticalLayoutGroup.CalculateLayoutInputVertical();
        //    this.viewPref.listContentSizeFitter.SetLayoutVertical();
        //    this.viewPref.listRectTransform.anchoredPosition = new Vector2(
        //        this.viewPref.listRectTransform.rect.width / 2,
        //        -this.viewPref.listRectTransform.rect.height / 2
        //    );
        //}

        private bool SetCampUpgradeBonus(BuildingConf buildingConf) {
            if (!buildingConf.type.CustomEquals("armycamp")) {
                return false;
            }
            ArmyCampConf nextLevelArmyConf = ConfigureManager.GetConfById<ArmyCampConf>(
                        buildingConf.buildingLevel.ToString());
            ArmyCampConf currentLevelArmyConf = null;
            if (this.viewModel.BuildViewType == BuildViewType.Upgrade) {
                int currentBuildingLevel = buildingConf.buildingLevel > 1 ?
                    buildingConf.buildingLevel - 1 : buildingConf.buildingLevel;
                currentLevelArmyConf = ConfigureManager.GetConfById<ArmyCampConf>(
                            currentBuildingLevel.ToString());
            } else {
                currentLevelArmyConf = nextLevelArmyConf;
            }
            GameHelper.ClearChildren(this.viewPref.pnlResourcesBonus);
            string bonusPath = SpritePath.buildUpgradeIconPrefix + buildingConf.type + "_amount";
            int levelHeroBonus = nextLevelArmyConf.heroAmount - currentLevelArmyConf.heroAmount;
            string bonusStr = levelHeroBonus == 0 ?
                string.Empty : string.Concat("+", levelHeroBonus);
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.troop_amount),
                currentLevelArmyConf.heroAmount.ToString(),
                bonusStr
            );
            bonusPath = string.Concat(
                SpritePath.buildUpgradeIconPrefix,
                buildingConf.type, "_conscription"
            );
            float levelSpeedBonus = nextLevelArmyConf.recruitmentSpeed * 100 - currentLevelArmyConf.recruitmentSpeed * 100;
            this.CreateUpgradeBonusInfo(
                bonusPath,
                LocalManager.GetValue(LocalHashConst.recruit_speed),
                string.Format("{0:0}%", currentLevelArmyConf.recruitmentSpeed * 100),
                string.Format("+{0:0}%", levelSpeedBonus));

            return true;
        }



        private void CreateUpgradeBonusInfo(string spritePath, string nameText,
            string currentValue, string bonus) {
            GameObject resourceBonusObj =
                PoolManager.GetObject(PrefabPath.pnlResourcesBonus, this.viewPref.pnlResourcesBonus);
            if (this.viewModel.BuildViewType != BuildViewType.Upgrade || bonus.StartsWith("+0")) {
                bonus = string.Empty;
            }
            ResourcesBonusView resourceBonusView = resourceBonusObj.GetComponent<ResourcesBonusView>();
            resourceBonusView.SetResouceInfo(spritePath, nameText, currentValue, bonus);
        }
        #endregion

        private void SetUnlockBuildingInfo(BuildingConf buildingConf) {
            List<UnlockBuildingInfo> unlockList =
                this.viewModel.GetBuildingUnlockList(
                    buildingConf.buildingName, buildingConf.buildingLevel);
            unlockList.Sort((a, b) => {
                return b.isBuildingNew.CompareTo(a.isBuildingNew);
            });
            int unlockListCount = unlockList.Count;
            bool hasUnlockBuilds = unlockListCount > 0;
            this.viewPref.pnlUnlockBuilds.gameObject.SetActiveSafe(hasUnlockBuilds);
            if (hasUnlockBuilds) {
                GameHelper.ClearChildren(this.viewPref.pnlUnlockBuildList);
                BuildingConf unlockBuildingConf;
                Sprite tmpSprit;
                for (int index = 0; index < unlockListCount; index++) {
                    GameObject unlockObj =
                        PoolManager.GetObject(PrefabPath.pnlUnlockItem, this.viewPref.pnlUnlockBuildList);
                    unlockBuildingConf =
                        ConfigureManager.GetConfById<BuildingConf>(unlockList[index].buildingKey);
                    string tributeStr = MapUtils.GetBuildingLocalName(unlockBuildingConf) +
                                            "\n\n" + LocalManager.GetValue("brief_", unlockBuildingConf.type);
                    UnityAction action = (() => {
                        if (!this.isShowUnlockBuildInfo) {
                            this.viewPref.txtTribute.text = tributeStr;
                            Vector3 unlockObjTransform = unlockObj.transform.position;
                            Vector2 unlockObjUICood = MapUtils.WorldToUIPoint(unlockObjTransform);
                            this.viewPref.pnlUnlockBuildingDesc.position = unlockObjTransform;
                            this.ShowPnlUnlockBuildingDesc(unlockObjUICood);
                        } else {
                            this.HidePnlUnlockBuildingDesc();
                        }
                    });

                    string unlockBuildTips =
                        this.viewModel.GetUnlockBuildingTips(unlockBuildingConf);
                    if (unlockList[index].buildingCount > 1) {
                        unlockBuildTips =
                              string.Concat(unlockBuildTips, "x",
                                            unlockList[index].buildingCount);
                    }
                    UnlockBuilding unlockBuilding = unlockObj.GetComponent<UnlockBuilding>();
                    tmpSprit = ArtPrefabConf.GetSprite(
                        string.Format(
                            SpritePath.buildingIconFormat,
                            unlockBuildingConf.type,
                            Mathf.CeilToInt(unlockBuildingConf.buildingLevel /
                                ArtConst.buildingInterval)
                        )
                     );
                    unlockBuilding.SetUnlockBuildingInfo(unlockBuildTips, tmpSprit, action);
                }
            }
        }

        private readonly int UPGRADE_RESOURCE_TYPE = 3;
        private List<Resource> upgradingResourceNotEnough = new List<Resource>(4);
        private void SetUpgradeRequireResourcesInfo(BuildingConf buildingConf) {
            this.viewPref.txtTime.text = GameHelper.TimeFormat(buildingConf.duration * 1000);
            this.canUpgrade = true;
            int index = 0;
            this.upgradingResourceNotEnough.Clear();
            foreach (var resource in buildingConf.resourceDict) {
                this.viewPref.upgradeResources[index].SetActiveSafe(true);
                if (!this.viewPref.resourceItemView[index++].SetResouceInfo(
                resource.Key, resource.Value,
                (int)RoleManager.GetResource(resource.Key))) {
                    this.upgradingResourceNotEnough.Add(resource.Key);
                    this.canUpgrade = false;
                }
                if (index > UPGRADE_RESOURCE_TYPE) {
                    break;
                }
            }
            for (; index < UPGRADE_RESOURCE_TYPE; index++) {
                this.viewPref.upgradeResources[index].SetActiveSafe(false);
            }
            this.viewPref.pnlResouceBtn.SetActiveSafe(!this.canUpgrade);
            this.viewPref.btnUpgrade.Grayable = !this.canUpgrade;
        }

        public void SetbtnBuildGo(string txt) {
            this.viewPref.txtBtnGo.text = txt;
        }

        private void ShowBuildInfoView(BuildingConf buildingConf) {
            if (this.viewModel.GetDurabilityBonus() == 0) {
                this.viewPref.txtHealthAddition.gameObject.SetActiveSafe(false);
            }
            UIManager.SetUIVisible(this.viewPref.pnlUnlockBuildingDesc.gameObject, false);
            this.viewPref.pnDescription.gameObject.SetActiveSafe(true);
            this.SetShowUpgradeInfo(false);
            if (buildingConf.type.CustomEndsWith("dominant_up")) {
                DominantUpConf dominatUpConf = DominantUpConf.GetConf(
                    buildingConf.type,
                    buildingConf.buildingLevel);
                this.viewPref.txtDescription.text = string.Format(
                    LocalManager.GetValue("detail_", buildingConf.type),
                    GameConst.DOMINANT_BASIC + dominatUpConf.bonus * 100);
            } else if (buildingConf.type.CustomEquals("march_speed_up")) {
                MarchSpeedUpConf speedConf =
                    MarchSpeedUpConf.GetConf(buildingConf.buildingLevel.ToString());
                this.viewPref.txtDescription.text = string.Format(
                   LocalManager.GetValue("detail_", buildingConf.type),
                   speedConf.percent * 100);
            } else if (buildingConf.type.CustomEquals("siege_up")) {
                SiegeUpConf siegeConf =
                    SiegeUpConf.GetConf(buildingConf.buildingLevel.ToString());
                this.viewPref.txtDescription.text = string.Format(
                   LocalManager.GetValue("detail_", buildingConf.type),
                   siegeConf.bonus);
            } else {
                this.viewPref.txtDescription.text =
                    LocalManager.GetValue("detail_", buildingConf.type);
            }
            this.viewPref.txtTile.text = GameHelper.GetNameAndLevel(
                MapUtils.GetBuildingLocalName(buildingConf),
                buildingConf.buildingLevel, true);

            this.SetUpgradeBonusInfo(buildingConf);
        }

        private void ShowPnlUnlockBuildingDesc(Vector3 start) {
            //if (!this.isShowUnlockBuildInfo) {
            UIManager.SetUIVisible(this.viewPref.pnlUnlockBuildingDesc.gameObject, true);
            AnimationManager.Animate(
                this.viewPref.pnlUnlockBuildingDesc.gameObject, "Show", start, start,
                () => this.isShowUnlockBuildInfo = true);
            //} else {
            //    this.HidePnlUnlockBuildingDesc();
            //}
        }

        public void HidePnlUnlockBuildingDesc() {
            if (this.isShowUnlockBuildInfo) {
                AnimationManager.Animate(this.viewPref.pnlUnlockBuildingDesc.gameObject, "Hide",
                        () => {
                            UIManager.SetUIVisible(this.viewPref.pnlUnlockBuildingDesc.gameObject, false);
                            this.isShowUnlockBuildInfo = false;
                        });
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnBtnUpgradeClick() {
            //Debug.LogError("OnBtnUpgradeClick " + this.canUpgrade);
            if (!this.canUpgrade) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.tips_get_resource), TipType.Info);
            } else if (EventManager.IsBuildEventFull()) {
                if (EventManager.IsBuildEventMaxFull()) {
                    UIManager.ShowTip(LocalManager.GetValue(
                            LocalHashConst.server_build_event_queue_maximum), TipType.Info);
                } else {
                    this.viewModel.ShowUnlockBuild();
                }
            } else {
                this.viewModel.UpgradeReq();
            }
        }

        private void OnBtnBuildUnlockConditionClick() {
            if (this.viewPref.btnBuildUpgrade.Grayable) {
                UIManager.ShowTip(LocalManager.GetValue(
                        LocalHashConst.server_build_event_queue_maximum), TipType.Info);
                return;
            }
            if (this.viewModel.IsEqualsTownHallLevel()) {
                return;
            }
            Coord conditionCoord =
                this.viewModel.FindBuildUnlockCondition(this.unlockCondition);
            if (conditionCoord == null) {
                this.viewModel.FindBuildingCanBeBuild(this.unlockCondition, "1");
            } else {
                this.viewModel.ClickTile(conditionCoord, TileArrowTrans.upgrade);
            }
        }

        private void OnBtnForceNotEnoughClick() {
            this.viewModel.GetCanAddForceCoordReq();
        }

        private void OnBtnResourceUpgrade() {
            if (this.upgradingResourceNotEnough.Count > 0) {
                this.afterHideCallback = () => {
                    this.viewModel.OnUpgradeResourcesShort(
                        this.upgradingResourceNotEnough[0].ToString());
                };
                this.viewModel.Hide();
            }
        }
        #region FTE

        public void OnBuildUpstep2Start() {
            this.InitUI();
            if (this.viewPref.btnUpgrade.interactable) {
                FteManager.SetMask(this.viewPref.btnUpgrade.pnlContent, isButton: true, autoNext: false, offset: new Vector2(0, -80), rotation: 180);
            } else {
                FteManager.StopFte();
                this.viewModel.StartChapterDailyGuid();
            }
        }

        #endregion
        protected override void OnInvisible() {
            base.OnInvisible();
            this.HidePnlUnlockBuildingDesc();
        }
    }
}
