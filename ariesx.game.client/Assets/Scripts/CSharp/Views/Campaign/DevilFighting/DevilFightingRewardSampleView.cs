using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class DevilFightingRewardSampleView: MonoBehaviour {
        [SerializeField]
        private RewardsItemView[] itemArray;
        //private bool IsSpecialResource = true;

        public UnityAction<string> OnHeroClick {
            get; set;
        }


        /*************************************************/
        public void SetContent(Protocol.CommonReward reward) {
            Dictionary<Resource, int> resourceDict = reward.GetRewardsDict();
            List<HeroFragment> fragments = reward.Fragments;
            int Count = 0;
            bool maxCount = Count > 2;
            if (fragments != null) {
                foreach (HeroFragment value in reward.Fragments) {
                    itemArray[Count++].OnHeroClick = this.OnHeroClick;
                    itemArray[Count++].SetContent(value);
                    if (maxCount) break;
                }
            }

            if (!maxCount) {
                foreach (var item in resourceDict) {
                    if (item.Key == Resource.Chest) {
                        itemArray[Count++].SetContent(type: item.Key, amount: item.Value);
                    } else if (item.Key == Resource.Gem) {
                        itemArray[Count++].SetContent(type: item.Key, amount: item.Value);
                    } else if (item.Key == Resource.Gold) {
                        itemArray[Count++].SetContent(type: item.Key, amount: item.Value);
                    }
                    if (maxCount) return;
                }

                if (!maxCount) {
                    foreach (var item in resourceDict) {
                        itemArray[Count++].SetContent(type: item.Key, amount: item.Value);
                        if (maxCount) return;
                    }
                }

            }

        }
    }
}