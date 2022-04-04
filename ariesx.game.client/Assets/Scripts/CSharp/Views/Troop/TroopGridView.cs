using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;

namespace Poukoute {
    public class TroopGridView : MonoBehaviour {
        #region ui elements
        [SerializeField]
        private Transform[] heroTransform;
        [SerializeField]
        private GameObject[] imgEmpty;
        [SerializeField]
        private CustomClick[] imgEmptyClick;
        [SerializeField]
        private GameObject[] imgLocked;
        #endregion
        public UnityAction TroopFormationCallback;
        private BuildModel buildModel;
        private HeroModel heroModel;


        /*************************************************/
        void Awake() {
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
        }

        private const int TROOPS_COUNT_UPPER = 6;
        public void SetTroopGrid(Troop troop,
            UnityAction<string, UnityAction> onHeroClick) {
            ArmyCampConf armyCampConf = null;
            if (!troop.ArmyCamp.CustomIsEmpty()) {
                armyCampConf = ArmyCampConf.GetConf(
                    this.buildModel.buildingDict[troop.ArmyCamp].Level.ToString()
                );
                if (armyCampConf == null) {
                    return;
                }
            }
            bool unlocked = false;
            this.SetPosition(0, true);
            for (int index = 1; index < TROOPS_COUNT_UPPER; index++) {
                unlocked = armyCampConf.unlockPositionList.Contains(index + 1);
                this.SetPosition(index, unlocked);
            }

            //this.heroHeadViewList.Clear();
            foreach (HeroPosition heroPosition in troop.Positions) {
                this.imgLocked[heroPosition.Position - 1].gameObject.SetActive(false);
                this.imgEmpty[heroPosition.Position - 1].gameObject.SetActive(false);
                this.SetHero(this.heroModel.heroDict[heroPosition.Name],
                    heroPosition.Position, onHeroClick, needInfo: true);
            }
        }

        public void SetMonsterTroopGrid(MonsterTroop monsterTroop,
            UnityAction<Hero, UnityAction> onMonsterHeroClick) {
            StartCoroutine(this.InnerSetTroopGrid(monsterTroop.Heroes,
                monsterTroop.Positions, onMonsterHeroClick));
        }

        public void SetTroopGrid(NpcTroop troop,
            UnityAction<Hero, UnityAction> onHeroClick) {
            StartCoroutine(this.InnerSetTroopGrid(troop.Heroes,
                troop.Positions, onHeroClick));
        }

        private IEnumerator InnerSetTroopGrid(List<NpcHero> heroes,
            List<HeroPosition> positionList,
            UnityAction<Hero, UnityAction> onHeroClick) {
            yield return YieldManager.EndOfFrame;

            this.ResetTroopGridInfo();
            for (int i = 0; i < 6; i++) {
                imgEmpty[i].transform.parent.gameObject.SetActive(true);
                imgEmpty[i].gameObject.SetActive(false);
                imgLocked[i].gameObject.SetActive(true);
            }

            foreach (NpcHero hero in heroes) {
                Hero newHero = new Hero {
                    Name = hero.Name,
                    Level = hero.Level,
                    ArmyAmount = hero.ArmyAmount,
                    armyCoeff = hero.ArmyAmountBonus
                };
                foreach (Skill skill in hero.Skills) {
                    newHero.Skills.Add(skill);
                }
                this.SetHero(newHero, positionList.GetHeroPosition(newHero.Name),
                    (name, callback) => onHeroClick(newHero, null), true, false);
            }
        }

        private void ResetTroopGridInfo() {
            for (int index = 0; index < TROOPS_COUNT_UPPER; index++) {
                this.SetPosition(index, false);
            }
        }

        private void SetPosition(int index, bool unlocked) {
            GameHelper.ClearChildren(this.heroTransform[index]);
            this.heroTransform[index].gameObject.SetActiveSafe(false);
            this.imgEmpty[index].gameObject.SetActiveSafe(unlocked);
            this.imgLocked[index].gameObject.SetActiveSafe(!unlocked);
            this.imgEmptyClick[index].onClick.RemoveAllListeners();
            bool canEditTroop = unlocked;
            this.imgEmptyClick[index].onClick.AddListener(() => {
                if (canEditTroop) {
                    this.TroopFormationCallback.InvokeSafe();
                } else {
                    UIManager.ShowTip(
                        LocalManager.GetValue(LocalHashConst.upgrade_army_camp_tip), TipType.Info);
                }
            });
        }

        private void SetHero(Hero hero, int position, UnityAction<string, UnityAction> onHeroClick,
            bool isNpc = false, bool needInfo = true) {
            this.heroTransform[position - 1].gameObject.SetActiveSafe(true);
            GameObject heroObj = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig, this.heroTransform[position - 1]);
            HeroHeadView heroHead = heroObj.GetComponent<HeroHeadView>();
            UnityAction<Hero, bool> action = (actionHero, actionIsNpc) => {
                heroHead.SetHero(
                    actionHero, showFragment: false, isDragable: !actionIsNpc,
                    showEnergy: !actionIsNpc, showArmyAmount: true, showHeroStatus: false,
                    showLevel: needInfo, showPower: needInfo, showStar: needInfo
                );
            };
            if (isNpc) {
                this.imgLocked[position - 1].transform.parent.gameObject.SetActive(false);
            }
            action(hero, isNpc);
            heroHead.AddListener(() => onHeroClick(hero.Name,
                () => action(this.heroModel.heroDict[hero.Name], isNpc)));
            //this.heroHeadViewList.Add(heroHead);
        }
    }
}