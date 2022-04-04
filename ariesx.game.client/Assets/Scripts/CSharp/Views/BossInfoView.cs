using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BossInfoView: BaseView {
        private BossInfoViewModel viewModel;
        private BossInfoViewPeference viewPref;
        /* UI Members*/
        private string bossName;
        private HeroAttributeConf heroConf;
        private bool isSkillDescVisible = false;
        private RectTransform selectedActivityRT = null;
        private int skillChildindex = 0;
        /*************/

        //void Awake() {
            
        //}

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBossInfo");
            this.viewModel = this.gameObject.GetComponent<BossInfoViewModel>();
            this.viewPref = this.ui.transform.GetComponent<BossInfoViewPeference>();
            this.viewPref.btnClose.onClick.AddListener(this.viewModel.Hide);
            this.viewPref.BtnBackground.onClick.AddListener(this.viewModel.Hide);
            /* Cache the ui components here */
        }       

        private List<Skill> GetBossSkills(BossTroop boss) {
            int bossPositionCount = boss.Positions.Count;
            int bossHeroesCount = boss.Heroes.Count;
            for (int i = 0; i < bossHeroesCount; i++) {
                if (boss.Heroes[i].Name == this.bossName) {
                    return boss.Heroes[i].Skills;
                }
            }
            return boss.Heroes[0].Skills;
        }

              

        /* Propert change function */
        public void SetBossInfo(BossTroop boss) {
            if (this.bossName != HeroAttributeConf.GetBossName(boss)) {
                this.bossName = HeroAttributeConf.GetBossName(boss);
            }
            int level = HeroAttributeConf.GetBossLevelInfo(boss);
            this.SetSkillInfo(boss);
            heroConf = HeroAttributeConf.GetConf(this.bossName);
            Debug.LogError(this.bossName);
            this.viewPref.imgRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
            this.viewPref.txtAttack.text = HeroAttributeConf.GetAttack(
                    this.bossName, level).ToString();
            this.viewPref.txtTitle.text = LocalManager.GetValue(
                LocalHashConst.domination_name);
            this.viewPref.imgBossTypeIcon.sprite = ArtPrefabConf.GetSprite(
                this.bossName, 
                SpritePath.heroAvatarLargeSuffix
            );
            this.viewPref.txtPower.text = HeroAttributeConf.GetPower(
                this.bossName, level).ToString();
            this.viewPref.txtLevel.text = string.Format(LocalManager.GetValue(
               LocalHashConst.hero_level), level);
            int maxHp = HeroAttributeConf.GetBossArmyAmount(this.bossName, level);
            int thisHp = HeroAttributeConf.GetBossThisHP(boss);
            this.viewPref.sliHp.maxValue = maxHp;
            this.viewPref.sliHp.value = thisHp;
            this.viewPref.txtHp.text = string.Concat(thisHp, "/", maxHp);
            this.viewPref.txtDefence.text = HeroAttributeConf.GetDefense(
                this.bossName, level).ToString();
            this.viewPref.txtDescription.text =
                LocalManager.GetValue(this.bossName, "_description");
            this.SetBossRarityInfo(this.bossName);
        }

        private void SetSkillInfo(BossTroop boss) {
            GameHelper.ClearChildren(this.viewPref.pnlSkilList);
            int skillCount = GetBossSkills(boss).Count;
            List<Skill> skillList = GetBossSkills(boss);
            GameHelper.ResizeChildreCount(this.viewPref.pnlSkilList,
                skillCount, PrefabPath.pnlBossSkill);
            HeroSkillView bossSkillItem = null;
            SkillConf skillConf;
            for (int i = 0; i < skillCount; i++) {
                int index = i;
                skillConf = SkillConf.GetConf(skillList[i].Name);
                bossSkillItem = this.viewPref.pnlSkilList.
                    GetChild(i).GetComponent<HeroSkillView>();
                bossSkillItem.OnBtnDetailsClick.AddListener(() => {
                    ShowSkillDescription(string.Format(
                    LocalManager.GetValue(skillConf.description), skillList[index].Args.ToArray()), index);
                });
                bossSkillItem.SkillConf = skillConf;
            }

        }        
        
        private void ShowSkillDescription(string description, int index) {
            this.viewPref.txtSklillDescription.text = description;
            selectedActivityRT = this.viewPref.pnlSkilList.GetChild(index).GetComponent<RectTransform>();
            this.viewPref.pnlSkillDetail.parent = selectedActivityRT;
            this.viewPref.rectSkillDetail.anchoredPosition3D = Vector3.zero;
            this.isSkillDescVisible = this.viewPref.pnlSkillDetailCG.alpha == 1;
            if (skillChildindex != index) {
                this.HideSkillDetailCG();
                this.skillChildindex = index;                
                UIManager.SetUICanvasGroupVisible(this.viewPref.pnlSkillDetailCG, true);
                AnimationManager.Animate(this.viewPref.pnlSkillDetail.gameObject, "Show");
            } else {
                if (this.isSkillDescVisible) {
                    AnimationManager.Animate(this.viewPref.pnlSkillDetail.gameObject, "Hide", finishCallback: HideSkillDetailCG);
                } else {
                    UIManager.SetUICanvasGroupVisible(this.viewPref.pnlSkillDetailCG, true);
                    AnimationManager.Animate(this.viewPref.pnlSkillDetail.gameObject, "Show");
                }
            }

        }

        public void HideSkillDetailCG() {
            this.skillChildindex = 0;
            UIManager.SetUICanvasGroupVisible(this.viewPref.pnlSkillDetailCG, false);
        }

        private void SetBossRarityInfo(string heroName) {
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(heroName);
            int i = 0;
            for (; i < heroAttribute.rarity; i++) {
                this.viewPref.imgRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.viewPref.imgRarity.Length;
            for (; i < rarityCount; i++) {
                this.viewPref.imgRarity[i].SetActiveSafe(false);
            }
        }

        /***************************/
    }
}
