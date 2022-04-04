using Poukoute;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionResourcesItemView : MonoBehaviour {
    public TextMeshProUGUI amount;
    public Image img;

    public void SetResourceInfo(Resource type, int amount) {
        this.amount.text = GameHelper.GetFormatNum(amount);
        this.img.sprite = ArtPrefabConf.GetSprite(
                SpritePath.resourceIconPrefix, type.ToString().ToLower());
    }

    public void SetResourceInfo() {
        this.amount.text = "1";
        this.img.sprite = ArtPrefabConf.GetSprite("battle_report_lottery");
    }

}
