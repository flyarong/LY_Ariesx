using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Poukoute {
    public class DisputesIntegralItemView: MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI txtTxtPasslevel;
        [SerializeField]
        private TextMeshProUGUI txtHoldIntegral;

        public void SetContent(string txtTxtPasslevel ,string txtHoldIntegral) {
            this.txtTxtPasslevel.text = txtTxtPasslevel + LocalManager.GetValue(LocalHashConst.occupy_points_detail_level);
            this.txtHoldIntegral.text = txtHoldIntegral.ToString();
        }
    }
}
