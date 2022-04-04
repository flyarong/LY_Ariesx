using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;
namespace Poukoute {
    public class ServerToggleItemView : MonoBehaviour {
        public TextMeshProUGUI txtName;
        public UnityAction<int> toogleAction;
        private string toogleName;

        private void Awake() {
            this.transform.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void SetItemView(string name) {
            this.toogleName = name;
            if (!name.Equals("0")) {
                txtName.text = name;
            }
        }

        private void OnToggleValueChanged(bool Value) {
            if (Value == true) {
                if (this.toogleName.Equals(LocalManager.GetValue(LocalHashConst.change_server_suggest))) {
                    toogleAction.Invoke(-1);
                }else if (this.toogleName.Equals("0")) {
                    toogleAction.Invoke(0);
                } else {
                    toogleAction.Invoke(
                        int.Parse(this.toogleName.Split('-')[0]));
                }
            }
        }
    }
}
