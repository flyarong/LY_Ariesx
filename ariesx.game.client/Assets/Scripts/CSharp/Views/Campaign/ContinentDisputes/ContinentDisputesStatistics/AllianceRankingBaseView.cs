using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class AllianceRankingBaseView: MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI txtRank;
        [SerializeField]
        private Image imgRank;
        [SerializeField]
        private TextMeshProUGUI txtAllianceName;
        [SerializeField]
        private TextMeshProUGUI txtAllianceSN;
        [SerializeField]
        private TextMeshProUGUI txtRankPoint;
        [SerializeField]
        private Image imgAlliance;
        [SerializeField]
        private CustomButton btnAllianceInfo;

        public UnityEvent OnItemClick {
            get {
                this.btnAllianceInfo.onClick.RemoveAllListeners();
                return this.btnAllianceInfo.onClick;
            }
        }


        public void SetContent(int rank, string allianceName, int point,
            int imgAlliance, int mapSn) {
            this.imgAlliance.sprite = ArtPrefabConf.GetAliEmblemSprite(imgAlliance);
            this.imgRank.sprite = ArtPrefabConf.GetRankIcon(rank, txtRank);
            this.txtAllianceName.text = allianceName;
            this.txtRankPoint.text = GameHelper.GetFormatNum(point);
            this.txtAllianceSN.text = NPCCityConf.GetMapSNLocalName(mapSn);
        }
    }
}
