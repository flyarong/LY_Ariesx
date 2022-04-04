using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Poukoute {
    public class ActivityMapRankView: MonoBehaviour {

        public TextMeshProUGUI playerName;
        public TextMeshProUGUI txtHarm;

        public void GetCount(string playerName, string txtHarm) {
            this.playerName.text = playerName;
            this.txtHarm.text = txtHarm;
        }

    }
}
