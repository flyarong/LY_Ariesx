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
    public class BattleSimulationView : MonoBehaviour {
        private BattleSimulationViewModel viewModel;
        private BattleSimulationViewPreference viewPref;
        private const int heroCount = 6;
        /* UI Members*/
        private BattleSimulatorPosView[] defenderPos = new BattleSimulatorPosView[heroCount];
        private GameObject[] defenderObj = new GameObject[heroCount];
        private BattleSimulatorPosView[] attackerPos = new BattleSimulatorPosView[heroCount];
        private GameObject[] attackerObj = new GameObject[heroCount];

        private Transform[] attHP = new Transform[6];
        private Transform[] defHP = new Transform[6];

        /*************/

        private const string assetPath = "Assets/Resources/Battle/Heroes/{0}.prefab";


        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BattleSimulationViewModel>();
            this.viewPref = this.gameObject.GetComponent<BattleSimulationViewPreference>();

        }

        void Start() {
            this.SetHeroList();
            this.InitUi();
            for (int i = 0; i < heroCount; i++) {
                this.viewPref.ddlDefHero[i].value = 0;
                this.viewPref.ddlAtkHero[i].value = 0;
            }
        }

        public void SetHeroList() {
            List<string> heroList = new List<string>();
            heroList.Add("null");
            for (int i = 0; i < this.viewModel.HeroList.Count; i++) {
                heroList.Add(this.viewModel.HeroList[i]);
            }

            for (int i = 0; i < heroCount; i++) {
                this.viewPref.ddlDefHero[i].ClearOptions();
                this.viewPref.ddlDefHero[i].AddOptions(heroList);

                this.viewPref.ddlAtkHero[i].ClearOptions();
                this.viewPref.ddlAtkHero[i].AddOptions(heroList);
            }
        }

        private void InitUi() {
            InitInput(this.viewPref.ddlAtkHero, this.attackerObj, this.attackerPos, this.defenderPos, this.viewModel.attHeroData, this.viewPref.hpAttakerInputField, this.viewPref.lvAttakerInputField, this.viewPref.attaker, this.viewModel.attNeedHero, true,this.attHP);
            InitInput(this.viewPref.ddlDefHero, this.defenderObj, this.defenderPos, this.attackerPos, this.viewModel.defHeroData, this.viewPref.hpDefenderInputField, this.viewPref.lvDefenderInputField, this.viewPref.defender, this.viewModel.defNeedHero, false,this.defHP);
            this.viewPref.inpBattleCount.onEndEdit.AddListener(delegate { int.TryParse(this.viewPref.inpBattleCount.text, out this.viewModel.simulationCount); });
            this.viewPref.btnBattleStart.onClick.AddListener(this.OnStartBattleSimulation);
            this.viewPref.btnBattleReport.onClick.AddListener(this.OnShowBattleReport);
            this.viewPref.btnStatReport.onClick.AddListener(this.OnShowStatReport);
        }

        public void ShowSimulationUI(bool isShow) {
            for (int i = 0; i < this.attHP.Length; i++) {
                if (this.attHP[i] != null) this.attHP[i].gameObject.SetActive(isShow);
            }
            for (int i = 0; i < this.defHP.Length; i++) {
                if (this.defHP[i] != null) this.defHP[i].gameObject.SetActive(isShow);
            }
            this.viewPref.btnBattleReport.gameObject.SetActive(isShow);
            this.viewPref.btnBattleStart.gameObject.SetActive(isShow);
            this.viewPref.btnStatReport.gameObject.SetActive(isShow);
            this.viewPref.inpBattleCount.gameObject.SetActive(isShow);
            this.viewPref.attakers.gameObject.SetActive(isShow);
            this.viewPref.defenders.gameObject.SetActive(isShow);
        }

        private void InitInput(Dropdown[] dropdown, GameObject[] obj, BattleSimulatorPosView[] attPos, BattleSimulatorPosView[] defPos, BattleSimulationViewModel.HeroData[] heroData, InputField[] hpInput, InputField[] lvInput, Transform[] parent, bool[] needHero, bool isAttacker,Transform[] hpsTF) {
            for (int i = 0; i < heroCount; i++) {
                int index = i;
                dropdown[i].onValueChanged.RemoveAllListeners();
                dropdown[i].onValueChanged.AddListener(delegate {
                    this.OnHeroChange(dropdown[index], obj, attPos, defPos, heroData, hpInput, lvInput, parent, needHero, isAttacker,hpsTF);
                });
                hpInput[i].onEndEdit.AddListener(delegate {
                    //int.TryParse(hpInput[index].text, out heroData[index].army_amount_percent);});
                    float.TryParse(hpInput[index].text, out heroData[index].army_amount_percent);
                });
                lvInput[i].onEndEdit.AddListener(delegate { int.TryParse(lvInput[index].text, out heroData[index].level); });
            }
        }

        private void OnHeroChange(Dropdown change, GameObject[] obj, BattleSimulatorPosView[] attPos, BattleSimulatorPosView[] defPos, BattleSimulationViewModel.HeroData[] heroData, InputField[] hpInput, InputField[] lvInput, Transform[] parent, bool[] needHero, bool isAttacker,Transform[] hpsTF) {
            string name = change.transform.parent.name;
            int index = int.Parse(name.Substring(name.Length - 1, 1));
            if (attPos[index] != null) {
                PoolManager.RemoveObject(obj[index]);
                DestroyImmediate(attPos[index]);
            }

            needHero[index] = !(change.value == 0);
            if (change.value == 0) {
                lvInput[index].text = "Lv";
                hpInput[index].text = "HP";
                hpsTF[index] = null;
                return;
            }

            Battle.Hero hero = new Battle.Hero {
                Name = this.viewModel.HeroList[change.value - 1]
            };
            obj[index] = PoolManager.GetObject(
                BattleConst.positionPrefix + hero.Name,
                parent[index]
            );
            DestroyImmediate(obj[index].GetComponent<BattlePositionView>());
            attPos[index] = obj[index].AddComponent<BattleSimulatorPosView>();
            string playerType = isAttacker ? BattleConst.attack : BattleConst.defense;
            string player = isAttacker ? BattleConst.defense : BattleConst.attack;

            Transform hpTF = attPos[index].SetPosition(index + 1, playerType, hero.Name, defPos[index]);
            Image imgRole = hpTF.Find("HealthBg").Find("ImgRole").GetComponent<Image>();
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.Name);
            imgRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
            imgRole.gameObject.SetActive(true);
            hpsTF[index] = hpTF;

            if (defPos[index] != null) {
                defPos[index].SetPosition(index + 1, player, defenderPos[index].heroId, attPos[index]);
            }

            heroData[index].name = this.viewModel.HeroList[change.value - 1];
            heroData[index].position = index + 1;
            heroData[index].level = this.SetHeroRarityInfo(hero.Name);
            heroData[index].army_amount_percent = 1;

            lvInput[index].text = this.SetHeroRarityInfo(hero.Name).ToString();
            hpInput[index].text = "1";
        }

        private void OnStartBattleSimulation() {
            this.viewModel.StartBattleSimulation();
        }
        private void OnShowBattleReport() {
            this.viewPref.UIBattleReport.gameObject.SetActive(true);
            this.viewModel.ShowBattleReport();
        }

        private void OnShowStatReport() {
            this.viewPref.UIStatReport.gameObject.SetActive(true);
            this.viewModel.ShowStatReport();
        }

        public void ShowMask(bool isShow, string str = "") {
            this.viewPref.UIMask.gameObject.SetActive(isShow);
            this.viewPref.txtMask.text = str;
        }

        private int SetHeroRarityInfo(string heroName) {
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(heroName);
            switch (heroAttribute.rarity) {
                case 1: return 60;
                case 2: return 50;
                case 3: return 45;
                case 4: return 30;
                case 5: return 15;
                default: return 1;
            }
        }
    }
}



