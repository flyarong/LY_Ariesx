using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using System;
using UnityEngine.Events;

namespace Poukoute {
    public class DemonShadowRankItemView: MonoBehaviour {
        [SerializeField]
        private Image imgRank;
        [SerializeField]
        private TextMeshProUGUI txtRankPlayer;
        [SerializeField]
        private TextMeshProUGUI txtRankPoint;
        [SerializeField]
        private CustomButton btnShowPlayerInfo;

        public UnityEvent PlayerInfoClick {
            get {
                this.btnShowPlayerInfo.onClick.RemoveAllListeners();
                return this.btnShowPlayerInfo.onClick;
            }
        }


        public void SetContent(string player, int point,int rank = 0) {
            if (this.imgRank != null) {
                this.imgRank.sprite = ArtPrefabConf.GetSprite(this.GetRankBG(rank));
            }
            this.txtRankPlayer.text = player;
            this.txtRankPoint.text = GameHelper.GetFormatNum(point);
        }

        private string GetRankBG(int rank) {
            bool isSpecialRank = rank < 4;
            string path = "campaign_rank";
            if (isSpecialRank) {
                path = string.Concat(path, rank);
            }
            return path;
        }

    }
}
