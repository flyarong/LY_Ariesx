using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class DevilFightingRankItemView : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI txtRank;
        [SerializeField]
        private Image imgRank;
        [SerializeField]
        private TextMeshProUGUI txtRankPlayer;
        [SerializeField]
        private TextMeshProUGUI txtRankPoint;
        [SerializeField]
        private CustomButton btnPlayerInfo;

        public UnityEvent OnPlayerInfoClick {
            get {
                this.btnPlayerInfo.onClick.RemoveAllListeners();
                return this.btnPlayerInfo.onClick;
            }
        }

        public void SetContent(int rank, string player, int point) {
            this.imgRank.sprite = ArtPrefabConf.GetSprite(this.GetRankBG(rank));
            bool showRank = (rank > 3);
            this.txtRank.gameObject.SetActiveSafe(showRank);
            if (showRank) {
                this.txtRank.text = rank.ToString();
            }
            this.txtRankPlayer.text = player;
            this.txtRankPoint.text = point.ToString();
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
