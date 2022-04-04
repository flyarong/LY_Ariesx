using Poukoute;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardsInfo : MonoBehaviour {

    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtNum;
    public TextMeshProUGUI txtLabel;

    public void SetHeroCardsInfo(string cardInfo, int index, int diffQualityNumCount) {
        string[] tmpCard = cardInfo.CustomSplit(',');
        HeroQuality quality = (HeroQuality)Enum.Parse(
            typeof(HeroQuality), tmpCard[0]);
        string qualityString = this.GetHeroCardQuality(quality);
        txtName.text = LocalManager.GetValue(qualityString.CustomGetHashCode());
        txtNum.text = tmpCard[1];
        txtLabel.transform.parent.gameObject.SetActiveSafe(
            (index + 1) != diffQualityNumCount);
    }

    private string GetHeroCardQuality(HeroQuality quality) {
        switch (quality) {
            case HeroQuality.T1:
                return "hero_card_ordinary";
            case HeroQuality.T2:
                return "hero_card_excellent";
            case HeroQuality.T3:
                return "hero_card_rare";
            case HeroQuality.T4:
                return "hero_card_epic";
            case HeroQuality.T5:
                return "hero_card_legend";
            default:
                return string.Empty;
        }
    }
}
