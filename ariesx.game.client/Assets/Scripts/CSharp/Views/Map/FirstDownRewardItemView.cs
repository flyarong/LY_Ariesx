using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class FirstDownRewardItemView : BaseItemViewsHolder {
        #region ui element
        [SerializeField]
        private GameObject pnlStatus;
        [SerializeField]
        private Image imgFieldLevel;
        [SerializeField]
        private TextMeshProUGUI txtTips;
        [SerializeField]
        private Image imgChestUnlock;
        [SerializeField]
        private TextMeshProUGUI txtChestUnlock;
        [SerializeField]
        private Transform pnlResources;
        [SerializeField]
        private CustomButton btnReceive;
        [SerializeField]
        private Transform pnLandUnlock;
        [SerializeField]
        private TextMeshProUGUI txtLandCount;

        #endregion

        [HideInInspector]
        public UnityEvent OnReceiveClick;

        private int level;
        public int Level {
            get {
                return this.level;
            }
            set {
                this.level = value;
                this.OnLevelChange();
            }
        }

        private Dictionary<Resource, Transform> resourceDict =
                    new Dictionary<Resource, Transform>();

        public bool CanCollect {
            get; set;
        }
        public bool IsCollected {
            get; set;
        }
        public FieldFirstDownViewType Type {
            get; set;
        }

        private void OnLevelChange() {

            if (this.btnReceive != null) {
                this.btnReceive.onClick.RemoveAllListeners();
                this.btnReceive.onClick.AddListener(this.OnBtnReceiveClick);
            }
            string conquerTips =
                string.Format(LocalManager.GetValue(LocalHashConst.reward_fdfield_congrats), this.level);
            string conqueredTip =
                string.Format(LocalManager.GetValue(LocalHashConst.reward_fdfield_desc), this.level);
            string conquestTip =
                string.Format(LocalManager.GetValue(LocalHashConst.firstdown_reward_received), this.level);

            bool isDefeat = (this.CanCollect || this.IsCollected);
            if (this.pnlStatus != null) {
                this.pnlStatus.SetActive(isDefeat);
            }
            bool isRewardList = (this.Type == FieldFirstDownViewType.RewardList);
            this.txtTips.text = isRewardList ?
                (isDefeat ? conquestTip : conquerTips) : conqueredTip;
            if (this.level != 1) {
                this.imgFieldLevel.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.fieldRewardPrefix, this.level.ToString());
            }
            this.SetFirstDownRewards();
        }

        private void OnBtnReceiveClick() {
            this.OnReceiveClick.InvokeSafe();
        }

        private void SetFirstDownRewards() {
            //bool landShow = (this.level != 1);
            //point_limit
            FirstDownRewardConf fieldRewardConf =
                        FirstDownRewardConf.GetConf(this.level.ToString());
            this.txtChestUnlock.text =
                LocalManager.GetValue(fieldRewardConf.unlockChest);
            this.imgChestUnlock.sprite =
                    ArtPrefabConf.GetChestSprite(fieldRewardConf.unlockChest);
            PointsLimitConf landCountConf = PointsLimitConf.GetConf("land", this.level);
            PointsLimitConf lastLandConf = PointsLimitConf.GetConf("land",
                (this.Type == FieldFirstDownViewType.GetReward) ?
                RoleManager.GetFDRecordMinorLevle() : RoleManager.GetFDRecordMaxLevel());

            int tileAdd = landCountConf.amountReward - lastLandConf.amountReward;
            bool isAddTile = (tileAdd > 0);
            this.pnLandUnlock.gameObject.SetActive(isAddTile);
            if (isAddTile) {
                this.txtLandCount.text = string.Concat(
                    LocalManager.GetValue(LocalHashConst.house_keeper_tilelimit),
                    " <color=#93EE6BFF>+", tileAdd.ToString(), "</color>"
                );
            }

            this.resourceDict.Clear();
            int resourceDictCount = fieldRewardConf.resourceDict.Count;
            GameHelper.ResizeChildreCount(this.pnlResources,
                resourceDictCount, PrefabPath.pnlItemWithCountSmall);
            ItemWithCountView itemView = null;
            int index = 0;
            foreach (var pair in fieldRewardConf.resourceDict) {
                itemView = this.pnlResources.GetChild(index++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(pair.Key, pair.Value);
                this.resourceDict.Add(pair.Key, itemView.imgItem.transform);
            }
        }

        public void GetFirstFieldDownReward(FieldFirstDownRewardNtf rewardNtf) {
            Protocol.Resources addResources = new Protocol.Resources();
            Protocol.Currency addCurrency = new Currency();
            if (rewardNtf.Reward != null) {
                addResources = rewardNtf.Reward.Resources;
                addCurrency = rewardNtf.Reward.Currency;
            } else {
                addResources.Food = 0;
                addResources.Crystal = 0;
                addResources.Lumber = 0;
                addResources.Marble = 0;
                addResources.Steel = 0;
                addCurrency.Gem = 0;
                addCurrency.Gold = 0;
            }

            Protocol.Resources resources = rewardNtf.Resources;
            Protocol.Currency currency = rewardNtf.Currency;
            this.IsCollected = true;
            GameHelper.CollectResources(addResources, addCurrency,
                resources, currency, resourceDict);
        }
    }
}
