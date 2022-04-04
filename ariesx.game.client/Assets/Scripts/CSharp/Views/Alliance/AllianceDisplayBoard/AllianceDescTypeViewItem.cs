using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Poukoute { 
public class AllianceDescTypeViewItem : ListItemView {
        [SerializeField]
        private TextMeshProUGUI txtTypeTitle;
        [SerializeField]
        private TextMeshProUGUI txtContent;

        public void SetCount(string title,string content) {
            txtTypeTitle.text = title;
            txtContent.text = content;
        }


    }
}
