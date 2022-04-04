using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class ForceItemView: MonoBehaviour {
        #region ui element
        [SerializeField]
        private Image imgLevelSlider;
        [SerializeField]
        private Image imgHead;
        [SerializeField]
        private Image pnlForceImgBG;
        [SerializeField]
        private Image imgTiteLeftHead;
        [SerializeField]
        private Image imgTiteRight;
        [SerializeField]
        private TextMeshProUGUI txtUnlockTownhallLevel;
        [SerializeField]
        private TextMeshProUGUI txtLevel;
        [SerializeField]
        private TextMeshProUGUI txtFrce;
        [SerializeField]
        private TextMeshProUGUI txtForceMarkHint;
        [SerializeField]
        private TextMeshProUGUI txtTite;
        [SerializeField]
        private Transform pnlRewardsList;
        [SerializeField]
        private CustomButton btnReceive;
        [SerializeField]
        private TextMeshProUGUI txtBtnReceive;
        [SerializeField]
        private CustomButton btnDisplayBoard;
        [SerializeField]
        private Image imgSupreme;
        #endregion

        private UnityAction callback;

        public Dictionary<Resource, Transform> resourceDict =
            new Dictionary<Resource, Transform>();

        public void SetForceStageContent(ForceRewardConf forceConf, int leve, bool
            isAchieve, bool isReceiveRewards, UnityAction callback, UnityAction displayBoard) {
            //int upForce = ForceRewardConf.GetCurrentForceLevelForce()
            //   - ForceRewardConf.GetThisLevelCurrentForce();
            //int thisLevelForce = ForceRewardConf.GetThisLevelCurrentForce();
            int force = RoleManager.GetForce();
            string forceLevel =
                    ForceRewardConf.GetForceLevel(RoleManager.GetForce()).ToString();
            this.btnDisplayBoard.onClick.RemoveAllListeners();
            this.btnDisplayBoard.onClick.AddListener(displayBoard);
            if (int.Parse(forceLevel) >= leve) {
                this.imgLevelSlider.fillAmount = 1;
            } else {
                this.imgLevelSlider.fillAmount = (force * 1.0f) / forceConf.force;
            }
            this.txtLevel.text = string.Format(
            LocalManager.GetValue(LocalHashConst.chest_level), leve - 1);
            bool isTxtFrceShow = leve - int.Parse(forceLevel) >= 2;
            if (isTxtFrceShow && !isAchieve) {
                txtFrce.text = string.Concat(forceConf.force.ToString());
                this.imgLevelSlider.fillAmount = 0;
            } else {
                txtFrce.text = string.Concat(force + "/" + forceConf.force.ToString());
            }
            this.txtTite.text = forceConf.forceLocal;
            imgHead.sprite = RoleManager.GetHighDefinitionRoleAvatar();//设置头像
            int unlockTownhallLevel = ForceRewardConf
                    .GetConf(forceConf.level.ToString()).unlockTownhallLevel;
            //this.txtUnlockTownhallLevel.text = (unlockTownhallLevel == 0) ? LocalManager.GetValue
               // (LocalHashConst.force_townhall_max_icon) : string.Empty;
            this.SetBtnReceiveGrayable(isAchieve, isReceiveRewards, force,
                forceConf.force, unlockTownhallLevel);
            UpdateForceInfo(callback);
            this.btnReceive.onClick.RemoveAllListeners();
            this.btnReceive.onClick.AddListener(() => { GetForceRewardReq(leve); });
            this.resourceDict.Clear();
            int rewardsCount = forceConf.resourcesDict.Count;
            GameHelper.ResizeChildreCount(this.pnlRewardsList,
                rewardsCount + 1, PrefabPath.pnlItemWithCount);
            ItemWithCountView itemView = null;
            int indexs = 0;
            foreach (var value in forceConf.resourcesDict) {
                itemView = this.pnlRewardsList.GetChild(indexs++).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(value.Key, value.Value);
                this.resourceDict.Add(value.Key, itemView.imgItem.transform);
            }
            itemView = this.pnlRewardsList.GetChild(indexs).GetComponent<ItemWithCountView>();
            itemView.SetResourceInfo(forceConf.chestLocal);
        }

        private void UpdateForceInfo(UnityAction callback) {
            this.callback = callback;
        }

        public void SetBtnReceiveGrayable(bool isAchieve, bool isReceiveRewards,
            int force, int forceConf, int unlockTownhallLevel) {
            this.SetTxtFrce(!isAchieve);
            if (isAchieve) {
                this.txtForceMarkHint.text = isReceiveRewards ?
                    LocalManager.GetValue(LocalHashConst.force_reward_received) :
                    LocalManager.GetValue(LocalHashConst.force_reward_receive);
                //this.imgSupreme.material = isReceiveRewards ? null:
                //    PoolManager.GetMaterial(MaterialPath.matGray);
                this.btnReceive.interactable = !isReceiveRewards;
                this.txtBtnReceive.text = isReceiveRewards ?
                    LocalManager.GetValue(LocalHashConst.button_received) :
                    LocalManager.GetValue(LocalHashConst.button_receive);
            } else {
                this.txtForceMarkHint.text = isReceiveRewards?
                    string.Format(LocalManager.GetValue(LocalHashConst.force_grow_upgrade_again), forceConf - force) :
                    (unlockTownhallLevel == 0) ? string.Format(LocalManager.GetValue(
                    LocalHashConst.force_townhall_max), forceConf - force) : string.Format(
                        LocalManager.GetValue(LocalHashConst.force_grow_earn_reward),
                        (forceConf - force), unlockTownhallLevel);
                this.txtBtnReceive.text = isReceiveRewards ?
                    LocalManager.GetValue(LocalHashConst.button_received) :
                    LocalManager.GetValue(LocalHashConst.button_receive);

                this.btnReceive.interactable = isAchieve;
            }
        }

        public void GetForceRewardReq(int level) {
            GetForceRewardReq getForceReward = new GetForceRewardReq() {
                Level = level
            };
            NetManager.SendMessage(getForceReward,
                typeof(GetForceRewardAck).Name, this.GetForceRewardAck);
            //RoleManager.Instance.NeedResourceAnimation = true;
            //RoleManager.Instance.NeedCurrencyAnimation = true;
        }

        private void GetForceRewardAck(IExtensible message) {
            GetForceRewardAck forceReward = message as GetForceRewardAck;
            Protocol.Resources addResources = forceReward.Reward.Resources;
            Protocol.Currency addCurrency = forceReward.Reward.Currency;
            Protocol.Resources resources = forceReward.Resources;
            Protocol.Currency currency = forceReward.Currency;
            GameHelper.CollectResources(addResources, addCurrency,
                resources, currency, this.resourceDict);

            if (forceReward.Reward.Chests.Count > 0) {
                GachaGroupConf chestName = GachaGroupConf.GetConf(
                    forceReward.Reward.Chests[0].Name);
                if (this.pnlRewardsList.childCount > 2) {
                    Vector3 position = this.pnlRewardsList.GetChild(2).
                        gameObject.transform.position;
                    GameHelper.ChestCollect(position, chestName);
                }
                HeroModel.AddlotteryChances(forceReward.Reward.Chests);
            }
            this.callback.InvokeSafe();
        }

        private void SetTxtFrce(bool show) {
            this.txtFrce.gameObject.SetActiveSafe(show);
        }


    }
}
