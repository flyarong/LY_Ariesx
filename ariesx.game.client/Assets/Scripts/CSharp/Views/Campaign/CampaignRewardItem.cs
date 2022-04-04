using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Poukoute {
    public class CampaignRewardItem : MonoBehaviour {
        #region UI element
        [SerializeField]
        private RectTransform itemRectTransform;
        [SerializeField]
        private LayoutElement itemLayoutElement;

        [SerializeField]
        private TextMeshProUGUI txtRank;
        [SerializeField]
        private Image imgRankBG;
        [SerializeField]
        private Transform pnlFragments;
        [SerializeField]
        private Transform pnlResources;
        [SerializeField]
        private TextMeshProUGUI TxtRewardType;
        #endregion

        public UnityAction<string> OnHeroClick {
            get; set;
        }

        private int REWARD_COL_COUNT = 6;

        public void SetContent(Protocol.RankReward rankReward) {
            bool showRank = false;
            string rankLabel = string.Empty;
            int rank = rankReward.Min;
            if (rankReward.Max == 0 ||
                rankReward.Min == rankReward.Max) {
                showRank = rank > 3;
                rankLabel = rank.ToString();
            } else {
                rank = 4;
                showRank = true;
                rankLabel = string.Concat(rankReward.Min, "-", rankReward.Max);
            }

            this.txtRank.gameObject.SetActiveSafe(showRank);
            if (showRank) {
                this.txtRank.text = rankLabel;
            }
            this.imgRankBG.sprite = ArtPrefabConf.GetSprite(this.GetRankBG(rank));
            this.imgRankBG.SetNativeSize();

            SetResourcesItemInfo(rankReward.Reward);
        }

        public void SetContentLastBloodReward(LastBloodReward reward) {
            if (reward == null) return;
            this.txtRank.gameObject.SetActiveSafe(false);
            this.imgRankBG.sprite = ArtPrefabConf.GetSprite("campaign_domination_last_hit");
            this.imgRankBG.SetNativeSize();
            this.TxtRewardType.text = LocalManager.GetValue(LocalHashConst.domination_last_hit);
            SetResourcesItemInfo(reward.Reward);
        }

        public void SetContentAllianceReward(AllianceReward reward) {
            if (reward == null) return;
            this.txtRank.gameObject.SetActiveSafe(false);
            this.imgRankBG.sprite = ArtPrefabConf.GetSprite("campaign_domination_all_member");
            this.imgRankBG.SetNativeSize();
            this.TxtRewardType.text = LocalManager.GetValue(LocalHashConst.domination_all_member);
            this.SetResourcesItemInfo(reward.Reward);
        }

        private void SetResourcesItemInfo(CommonReward reward) {
            bool hasFragments = this.SetRewardFragmentsInfo(reward);
            this.pnlFragments.gameObject.SetActive(hasFragments);
            Dictionary<Resource, int> resourceDict = reward.GetRewardsDict(false);
            int rewardCount = resourceDict.Count;
            GameHelper.ResizeChildreCount(this.pnlResources,
                rewardCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView itemView = null;
            int i = 0;
            foreach (var resourceValue in resourceDict) {
                itemView = this.pnlResources.GetChild(i++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(resourceValue.Key, resourceValue.Value);
            }
            this.itemLayoutElement.preferredHeight = ((i > REWARD_COL_COUNT) ? 350 : 240) - (hasFragments ? 0 : 64);
        }

        private bool SetRewardFragmentsInfo(CommonReward reward) {
            List<HeroFragment> fragments = reward.Fragments;
            if (fragments != null) {
                int fragmentsCount = fragments.Count;
                this.pnlFragments.gameObject.SetActive(fragmentsCount > 0);
                GameHelper.ResizeChildreCount(this.pnlFragments,
                    fragmentsCount, PrefabPath.pnlHeroFragment);
                HeroFragmentView itemView;
                for (int i = 0; i < fragmentsCount; i++) {
                    itemView = this.pnlFragments.GetChild(i).GetComponent<HeroFragmentView>();
                    itemView.OnHeroClick = this.OnHeroClick;
                    itemView.SetContent(fragments[i]);
                }
                return (fragmentsCount > 0);
            }
            return false;
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
