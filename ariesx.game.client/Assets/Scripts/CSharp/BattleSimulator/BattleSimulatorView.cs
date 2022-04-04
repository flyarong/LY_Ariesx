using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Poukoute {
    public class BattleSimulatorView : BaseView {
        private BattleSimulatorViewModel viewModel;
        //private GameObject ui;
        /* UI Members*/
        private Transform pnlDefender;
        private Dropdown ddlDefHero;
        private Dropdown ddlDefAction;
        private Dropdown ddlDefSkill;

        private Transform pnlAttacker;
        private Dropdown ddlAtkHero;
        private Dropdown ddlAtkAction;
        private Dropdown ddlAtkSkill;

        private Transform format;
        private Transform defender;
        private BattleSimulatorPosView defenderPos;
        private GameObject defenderObj;

        private Transform attaker;
        private BattleSimulatorPosView attackerPos;
        private GameObject attackerObj;

        private Button btnDefPlay;
        private Button btnDefSave;
        private Button btnAtkPlay;
        private Button btnAtkSave;

        private Button btnDefVictor;
        private Button btnDefFailed;
        private Button btnAtkVictor;
        private Button btnAtkFailed;

        private Button btnSaveAssets;

        //private string defSkill = string.Empty;
        //private string atkSkill = string.Empty;

        private Button btnDefenderAdd;
        private Button btnDefenderRemove;
        private Button btnAttackerAdd;
        private Button btnAttackerRemove;
        /*************/

        private const string assetPath = "Assets/Resources/Battle/Heroes/{0}.prefab";

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BattleSimulatorViewModel>();
            this.InitUi();
        }

        private void InitUi() {
            this.ui = GameObject.Find("UI").transform.Find("UIBattleSimulator").gameObject;
            /* Cache the ui components here */
            this.btnSaveAssets = this.ui.transform.Find("BtnSave").GetComponent<Button>();
            this.btnSaveAssets.onClick.RemoveAllListeners();
            this.btnSaveAssets.onClick.AddListener(this.OnBtnSaveAsset);

            this.pnlDefender = this.ui.transform.Find("PnlDefender");
            this.ddlDefHero = this.pnlDefender.Find("DdlHero").GetComponent<Dropdown>();
            this.ddlDefHero.onValueChanged.RemoveAllListeners();
            this.ddlDefHero.onValueChanged.AddListener(this.OnDefHeroChange);
            this.ddlDefAction = this.pnlDefender.Find("DdlAction").GetComponent<Dropdown>();
            this.ddlDefAction.onValueChanged.RemoveAllListeners();
            this.ddlDefAction.onValueChanged.AddListener(this.OnDefActionChange);
            this.ddlDefSkill = this.pnlDefender.Find("DdlSkill").GetComponent<Dropdown>();
            this.ddlDefSkill.onValueChanged.RemoveAllListeners();
            this.ddlDefSkill.onValueChanged.AddListener(this.OnDefSkillChange);

            this.pnlAttacker = this.ui.transform.Find("PnlAttacker");
            this.ddlAtkHero = this.pnlAttacker.Find("DdlHero").GetComponent<Dropdown>();
            this.ddlAtkHero.onValueChanged.RemoveAllListeners();
            this.ddlAtkHero.onValueChanged.AddListener(this.OnAtkHeroChange);
            this.ddlAtkAction = this.pnlAttacker.Find("DdlAction").GetComponent<Dropdown>();
            this.ddlAtkAction.onValueChanged.RemoveAllListeners();
            this.ddlAtkAction.onValueChanged.AddListener(this.OnAtkActionChange);
            this.ddlAtkSkill = this.pnlAttacker.Find("DdlSkill").GetComponent<Dropdown>();
            this.ddlAtkSkill.onValueChanged.RemoveAllListeners();
            this.ddlAtkSkill.onValueChanged.AddListener(this.OnAtkSkillChange);

            Transform battle = GameObject.Find("Battle").transform;
            this.format = battle.Find("Format");
            this.defender = this.format.Find("Defender");
            this.attaker = this.format.Find("Attacker");

            this.btnDefPlay = this.pnlDefender.Find("BtnPlay").GetComponent<Button>();
            this.btnDefPlay.onClick.RemoveAllListeners();
            this.btnDefPlay.onClick.AddListener(this.OnBtnDefPlay);
            this.btnDefSave = this.pnlDefender.Find("BtnSave").GetComponent<Button>();
            this.btnDefSave.onClick.RemoveAllListeners();
            this.btnDefSave.onClick.AddListener(this.OnBtnDefSave);
            this.btnAtkPlay = this.pnlAttacker.Find("BtnPlay").GetComponent<Button>();
            this.btnAtkPlay.onClick.RemoveAllListeners();
            this.btnAtkPlay.onClick.AddListener(this.OnBtnAtkPlay);
            this.btnAtkSave = this.pnlAttacker.Find("BtnSave").GetComponent<Button>();
            this.btnAtkSave.onClick.RemoveAllListeners();
            this.btnAtkSave.onClick.AddListener(this.OnBtnAtkSave);

            this.btnDefVictor = this.pnlDefender.Find("BtnVictor").GetComponent<Button>();
            this.btnDefVictor.onClick.RemoveAllListeners();
            this.btnDefVictor.onClick.AddListener(this.OnBtnDefVictor);
            this.btnDefFailed = this.pnlDefender.Find("BtnFailed").GetComponent<Button>();
            this.btnDefFailed.onClick.RemoveAllListeners();
            this.btnDefFailed.onClick.AddListener(this.OnBtnDefFailed);
            this.btnAtkVictor = this.pnlAttacker.Find("BtnVictor").GetComponent<Button>();
            this.btnAtkVictor.onClick.RemoveAllListeners();
            this.btnAtkVictor.onClick.AddListener(this.OnBtnAtkVictor);
            this.btnAtkFailed = this.pnlAttacker.Find("BtnFailed").GetComponent<Button>();
            this.btnAtkFailed.onClick.RemoveAllListeners();
            this.btnAtkFailed.onClick.AddListener(this.OnBtnAtkFailed);

            this.btnDefenderAdd = this.pnlDefender.Find("BtnAdd").GetComponent<Button>();
            this.btnDefenderAdd.onClick.RemoveAllListeners();
            this.btnDefenderAdd.onClick.AddListener(this.OnBtnDefenderAddClick);
            this.btnDefenderRemove = this.pnlDefender.Find("BtnRemove").GetComponent<Button>();
            this.btnDefenderRemove.onClick.RemoveAllListeners();
            this.btnDefenderRemove.onClick.AddListener(this.OnBtnDefenderRemoveClick);
            this.btnAttackerAdd = this.pnlAttacker.Find("BtnAdd").GetComponent<Button>();
            this.btnAttackerAdd.onClick.RemoveAllListeners();
            this.btnAttackerAdd.onClick.AddListener(this.OnBtnAttackerAddClick);
            this.btnAttackerRemove = this.pnlAttacker.Find("BtnRemove").GetComponent<Button>();
            this.btnAttackerRemove.onClick.RemoveAllListeners();
            this.btnAttackerRemove.onClick.AddListener(this.OnBtnAttackerRemoveClick);
        }

        void Start() {
            this.ddlDefHero.value = 1;
            this.ddlDefAction.value = 1;
            this.ddlDefSkill.value = 1;
            this.ddlAtkHero.value = 1;
            this.ddlAtkAction.value = 1;
            this.ddlAtkSkill.value = 1;
        }

        public void SetHeroList() {
            this.ddlDefHero.ClearOptions();
            this.ddlDefHero.AddOptions(this.viewModel.HeroList);

            this.ddlAtkHero.ClearOptions();
            this.ddlAtkHero.AddOptions(this.viewModel.HeroList);

            this.ddlDefSkill.ClearOptions();
            this.ddlDefSkill.AddOptions(this.viewModel.SkillList);

            this.ddlAtkSkill.ClearOptions();
            this.ddlAtkSkill.AddOptions(this.viewModel.SkillList);
        }

        private void OnDefHeroChange(int index) {
#if UNITY_EDITOR
            Battle.Hero hero = new Battle.Hero {
                Name = HeroBattleConf.modelDict[this.viewModel.HeroList[index]]
            };
            if (this.defenderPos != null) {
                PoolManager.RemoveObject(this.defenderObj);
                DestroyImmediate(this.defenderPos);
            }
            this.defenderObj = PoolManager.GetObject(BattleConst.positionPrefix +
                    HeroBattleConf.GetModelPath(hero.Name), defender);
            Debug.LogError(BattleConst.positionPrefix +
                    HeroBattleConf.GetModelPath(hero.Name));
            DestroyImmediate(this.defenderObj.GetComponent<BattlePositionView>());
            this.defenderPos = this.defenderObj.AddComponent<BattleSimulatorPosView>();
            this.defenderPos.SetPosition(1, BattleConst.defense, hero.Name, this.attackerPos);

            this.ddlDefAction.ClearOptions();
            this.viewModel.ActionList.Clear();

            this.viewModel.ActionList.Add("Attack");
            for (int i = 1; i < defenderPos.effectDict.Count; i++) {
                this.viewModel.ActionList.Add("Skill_" + i);
            }

            this.ddlDefAction.AddOptions(this.viewModel.ActionList);
            if (this.attackerPos != null) {
                this.attackerPos.SetPosition(2, BattleConst.attack, attackerPos.heroId, this.defenderPos);
            }
#endif
        }

        private void OnAtkHeroChange(int index) {
            Battle.Hero hero = new Battle.Hero {
                Name = this.viewModel.HeroList[index]
            };
            if (this.attackerPos != null) {
                PoolManager.RemoveObject(this.attackerObj);
                DestroyImmediate(this.attackerPos);
            }

            this.attackerObj = PoolManager.GetObject(
                BattleConst.positionPrefix + hero.Name, 
                attaker
            );
            DestroyImmediate(this.attackerObj.GetComponent<BattlePositionView>());
            this.attackerPos = this.attackerObj.AddComponent<BattleSimulatorPosView>();
            this.attackerPos.SetPosition(2, BattleConst.attack, hero.Name, this.defenderPos);

            this.ddlAtkAction.ClearOptions();
            this.viewModel.ActionList.Clear();

            this.viewModel.ActionList.Add("Attack");
            for (int i = 1; i < attackerPos.effectDict.Count; i++) {
                this.viewModel.ActionList.Add("Skill_" + i);
            }
            
            this.ddlAtkAction.AddOptions(this.viewModel.ActionList);

            if (this.defenderPos != null) {
                this.defenderPos.SetPosition(1, BattleConst.defense, defenderPos.heroId, this.attackerPos);
            }
        }

        private void OnDefActionChange(int index) {
            //this.ddlDefSkill.gameObject.SetActiveSafe(index == 1);
        }

        private void OnAtkActionChange(int index) {
            //this.ddlAtkSkill.gameObject.SetActiveSafe(index == 1);
        }

        private void OnDefSkillChange(int index) {
            //this.defSkill = this.viewModel.SkillList[index];
        }

        private void OnAtkSkillChange(int index) {
            //this.atkSkill = this.viewModel.SkillList[index];
        }

        private void OnBtnDefSave() {
#if UNITY_EDITOR
            string prefabPath = string.Format(assetPath, this.defenderPos.gameObject.name);
            foreach (Transform child in this.defenderPos.transform.Find("Position")) {
                if (child.name == "SkillLast" || child.name == "SkillHit") {
                    foreach (Transform grandChild in child) {
                        grandChild.GetComponent<BattleEffectPositionView>().Clear();
                        GameHelper.ClearChildren(grandChild);
                    }
                } else {
                    child.GetComponent<BattleEffectPositionView>().Clear();
                    GameHelper.ClearChildren(child);
                }
            }
            string heroName = this.defenderPos.heroId;
            DestroyImmediate(defenderPos);
            if (defenderObj.GetComponent<BattlePositionView>() == null) {
                defenderObj.AddComponent<BattlePositionView>();
            }
            PrefabUtility.CreatePrefab(prefabPath, defenderObj);
            DestroyImmediate(this.defenderObj.GetComponent<BattlePositionView>());
            this.defenderPos = this.defenderObj.AddComponent<BattleSimulatorPosView>();
            this.defenderPos.SetPosition(1, BattleConst.defense, heroName, this.attackerPos);
            this.attackerPos.SetPosition(2, BattleConst.attack, attackerPos.heroId, this.defenderPos);
#endif
        }

        private void OnBtnAtkSave() {
#if UNITY_EDITOR
            string prefabPath = string.Format(assetPath, this.attackerPos.gameObject.name);
            foreach (Transform child in this.attackerPos.transform.Find("Position")) {
                if (child.name == "SkillLast" || child.name == "SkillHit") {
                    foreach(Transform grandChild in child) {
                        grandChild.GetComponent<BattleEffectPositionView>().Clear();
                        GameHelper.ClearChildren(grandChild);
                    }
                } else {
                    child.GetComponent<BattleEffectPositionView>().Clear();
                    GameHelper.ClearChildren(child);
                }

            }
            string heroName = this.attackerPos.heroId;
            attackerPos.OnInPool();
            DestroyImmediate(attackerPos);
            if (attackerObj.GetComponent<BattlePositionView>() == null) {
                attackerObj.AddComponent<BattlePositionView>();
            }
            PrefabUtility.CreatePrefab(prefabPath, attackerObj);
            DestroyImmediate(this.attackerObj.GetComponent<BattlePositionView>());
            this.attackerPos = this.attackerObj.AddComponent<BattleSimulatorPosView>();
            this.attackerPos.SetPosition(2, BattleConst.attack, heroName, this.defenderPos);
            this.defenderPos.SetPosition(1, BattleConst.defense, defenderPos.heroId, this.attackerPos);
#endif
        }

        private void OnBtnDefPlay() {
            switch (this.viewModel.ActionList[this.ddlDefAction.value]) {
                case "Attack":
                    this.defenderPos.Attack();
                    break;
                default:
                    this.defenderPos.ReleaseSkill(this.ddlDefAction.value);
                    break;
            }
        }

        private void OnBtnAtkPlay() {
            switch (this.viewModel.ActionList[this.ddlAtkAction.value]) {
                case "Attack":
                    this.attackerPos.Attack();
                    break;
                default:
                    this.attackerPos.ReleaseSkill(this.ddlAtkAction.value);
                    break;
            }
        }

        private void OnBtnDefVictor() {
            this.defenderPos.Victor();
        }

        private void OnBtnDefFailed() {
            this.defenderPos.Failed();
        }

        private void OnBtnAtkVictor() {
            this.attackerPos.Victor();
        }

        private void OnBtnAtkFailed() {
            this.attackerPos.Failed();
        }

        private void OnBtnDefenderAddClick() {
            
        }

        private void OnBtnDefenderRemoveClick() {
            
        }

        private void OnBtnAttackerAddClick() {

        }

        private void OnBtnAttackerRemoveClick() {
            
        }

        private void OnBtnSaveAsset() {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }
    }
}



