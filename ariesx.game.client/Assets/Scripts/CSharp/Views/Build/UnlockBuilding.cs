using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnlockBuilding : MonoBehaviour {
    public TextMeshProUGUI txtTips;
    public Image building;
    public Button btnBuildTribute;

    public void SetUnlockBuildingInfo(
        string tips, Sprite tmpSprit, UnityAction action) {
        this.txtTips.text = tips;
        this.building.sprite = tmpSprit;
        btnBuildTribute.onClick.RemoveAllListeners();
        btnBuildTribute.onClick.AddListener(action);
    }
}
