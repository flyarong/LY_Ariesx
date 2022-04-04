using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public class BattleResultItemView : MonoBehaviour {

        #region UI component cache
        [SerializeField]
        private HeroAvatar heroAvatar;
        [SerializeField]
        private TextMeshProUGUI txtDamage;
        [SerializeField]
        private TextMeshProUGUI txtSkillCount;
        [SerializeField]
        private TextMeshProUGUI txtHealth;
        [SerializeField]
        private TextMeshProUGUI txtLose;
        #endregion

        private Battle.Hero hero;
        public Battle.Hero Hero {
            set {
                this.hero = value;
                this.OnHeroChange();
            }
        }

        private void OnHeroChange() {
            this.heroAvatar.SetHeroAvatar(this.hero);
            this.txtDamage.text = this.hero.TotalDamage.ToString();
            this.txtSkillCount.text = this.hero.SkillTriggeredTimes.ToString();
            this.txtHealth.text = this.hero.TotalHeal.ToString();
            this.txtLose.text = this.hero.DeadAmount.ToString();
        }
    }
}
