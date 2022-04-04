using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
namespace Poukoute {
    public class DemonShadowHistoryDetailItemView: BaseItemViewsHolder {
        [HideInInspector]
        public float Height {
            get {
                return 344;
            }
        }

        [SerializeField]
        private TextMeshProUGUI txtDemonLevel;
        [SerializeField]
        private TextMeshProUGUI txtContentEndTime;
        [SerializeField]
        private TextMeshProUGUI txtDemonPower;
        [SerializeField]
        private TextMeshProUGUI txtDemonPos;
        [SerializeField]
        private GameObject imgSuccess;
        [SerializeField]
        private GameObject imgFailure;
        [SerializeField]
        private CustomButton btnCheckRanks;
        [SerializeField]
        private Slider sliderDemonHP;
        [SerializeField]
        private TextMeshProUGUI txtDemonHPNum;
        
        public void SetdominationHistoryCount(DominationHistory record,bool results,
            UnityAction onCheckRankClicks,string coord) {
            this.btnCheckRanks.onClick.RemoveAllListeners();
            this.txtDemonLevel.text =string.Format(
                LocalManager.GetValue(LocalHashConst.hero_level) , record.Level);
            this.txtDemonPower.text = LocalManager.GetValue(
                LocalHashConst.hero_power)+ record.Power;
            this.sliderDemonHP.maxValue = record.HP;
            this.sliderDemonHP.value = record.CurrentHP;
            this.txtDemonHPNum.text = record.CurrentHP.ToString()
                + "/" + record.HP.ToString();
            this.GetResultView(results, onCheckRankClicks);
            this.txtDemonPos.text =string.Concat(coord, record.Coord);
            this.txtContentEndTime.text = GameHelper.DateFormat(record.EndTime);
        }

        private void GetResultView(bool results, UnityAction onCheckRankClicks) {           
            this.GetCheckRank(onCheckRankClicks);
            if (results) {
                this.imgFailure.SetActiveSafe(false);
                this.imgSuccess.SetActiveSafe(true);
            } else {
                this.imgSuccess.SetActiveSafe(false);
                this.imgFailure.SetActiveSafe(true);
            }
        }

        private void GetCheckRank(UnityAction rank) {
            this.btnCheckRanks.onClick.AddListener(rank);
        }

    }
}
