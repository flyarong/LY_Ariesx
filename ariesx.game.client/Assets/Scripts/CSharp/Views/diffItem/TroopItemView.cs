using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TroopItemView : MonoBehaviour {
    public TextMeshProUGUI label;
    public TextMeshProUGUI content;

    public void SetItemInfos(string label, string content) {
        this.label.text = label;
        this.content.text = content;
    }
}
