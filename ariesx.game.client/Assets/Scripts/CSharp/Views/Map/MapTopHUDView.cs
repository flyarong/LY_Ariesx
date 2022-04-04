using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/*** To do ***/
// make static function to change resources slider. 
/*************/

namespace Poukoute {
    public class MapTopHUDView: BaseView {
        private MapTopHUDViewModel viewModel;
        private MapTopHUDViewPreference viewPref;

        /*************/
        private float currentGem;
        private Dictionary<Resource, GameObject> resourceDict =
            new Dictionary<Resource, GameObject>(5);
        private Dictionary<string, GameObject> fteUIDict =
            new Dictionary<string, GameObject>();
        private int reference = 0;
        private int forceLevel = -3;
        public bool canShowForceAni = false;
        public UnityAction ForceAniEndAction;
        public bool showForceView = false;

        private readonly List<Resource> resourceList = new List<Resource> {
            Resource.Lumber,
            Resource.Steel,
            Resource.Marble,
            Resource.Food,
            Resource.Gold,
            Resource.Gem
        };

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<MapTopHUDViewModel>();
            this.InitUi();

            fteUIDict.Add(FteConst.PlayerInfo, this.viewPref.pnlPlayerInfo.gameObject);
            fteUIDict.Add(FteConst.Resource, this.viewPref.pnlResource.gameObject);

            InvokeRepeating("ShowHUDGemAnim", 0, UnityEngine.Random.Range(5, 10));
        }

        private void ShowHUDGemAnim() {
            this.viewPref.pnlGemAnim.SetActiveSafe(false);
            this.viewPref.pnlGemAnim.SetActiveSafe(true);
        }

        private void InitUi() {
            this.ui = UIManager.GetUI("UIMapTopHUD");
            this.group = UIGroup.MapAbove;
            this.viewPref = this.ui.transform.GetComponent<MapTopHUDViewPreference>();
            this.viewPref.btnPlayerInfo.onClick.AddListener(this.OnBtnPlayerInfoClick);
            this.viewPref.btnTerrain.onClick.AddListener(this.ShowFieldReward);
            this.viewPref.btnBuyGem.onClick.AddListener(this.OnBtnBuyGemClick);
            this.viewPref.btnFallen.onClick.AddListener(this.OnBtnFallenClick);
            this.viewPref.btnNovice.onClick.AddListener(this.OnBtnNoviceStateClick);
            this.viewPref.btnMonthCard.onClick.AddListener(this.OnBtnMonthCardClick);
            this.viewPref.pnlResource.GetComponent<CustomButton>().
                onClick.AddListener(OnBtnResourceClick);
            //this.viewPref.lumberItemView.OnResourceFullEvent.AddListener(
            //    this.OnResourceFull);
            this.resourceDict.Add(Resource.Lumber, this.viewPref.pnlLumber);
            //this.viewPref.steelItemView.OnResourceFullEvent.AddListener(
            //    this.OnResourceFull);
            this.resourceDict.Add(Resource.Steel, this.viewPref.pnlSteel);
            //this.viewPref.marbleItemView.OnResourceFullEvent.AddListener(
            //    this.OnResourceFull);
            this.resourceDict.Add(Resource.Marble, this.viewPref.pnlMarble);
            //this.viewPref.foodItemView.OnResourceFullEvent.AddListener(
            //    this.OnResourceFull);
            this.resourceDict.Add(Resource.Food, this.viewPref.pnlFood);
            this.resourceDict.Add(Resource.Gem, this.viewPref.pnlGem);
            this.resourceDict.Add(Resource.Gold, this.viewPref.pnlGold);
            this.SetResourceVisible();
            this.SetPlayerInfo();
        }


        private void SetResourceVisible() {
            foreach (var pair in this.resourceDict) {
                pair.Value.SetActiveSafe(RoleManager.IsResourceCollected(pair.Key));
            }
        }

        public void ChangeAvatar() {
            this.viewPref.imgPlayerAvatar.sprite = RoleManager.GetRoleAvatar();
        }

        public void SetPlayerInfo() {
            this.viewPref.txtPlayerName.text = RoleManager.GetRoleName();
            this.viewPref.imgPlayerAvatar.sprite = RoleManager.GetRoleAvatar();
            int maxValue = RoleManager.GetPointsLimit();
            int value = RoleManager.GetPointDict().Count;
            this.viewPref.txtTerrainInfo.text = string.Concat(value, "/", maxValue);
            int forceLevel =
                ForceRewardConf.GetForceLevel(RoleManager.GetForce());
            if (forceLevel - this.forceLevel == 1) {
                this.canShowForceAni = true;
            }
            this.forceLevel = forceLevel;
            int force = RoleManager.GetForce();
            ForceRewardConf forceConf = ForceRewardConf.GetConf((forceLevel + 1).ToString());

            if (forceLevel != 0) {
                string forceRewardKey = forceLevel.ToString();
                int forceConfLow = ForceRewardConf.GetConf(forceRewardKey).force;
                if (this.forceLevel >= 12) {
                    this.viewPref.imgCircleSliderFill.fillAmount = 1;
                } else {
                    this.viewPref.imgCircleSliderFill.fillAmount =
                    ((force * 1.0f) - forceConfLow) / (forceConf.force - forceConfLow);
                }
                this.viewPref.txtForceLevel.text = forceRewardKey;
            } else {
                this.viewPref.imgCircleSliderFill.fillAmount = (force * 1.0f) / forceConf.force;
                this.viewPref.txtForceLevel.text = "--";
            }
        }

        public void ChangeResourcesWithoutAnimation(CommonReward reward) {
            if (reward.Resources.Marble != 0) {
                RoleManager.SetResourceCollected(Resource.Marble);
                this.resourceDict[Resource.Marble].gameObject.SetActive(true);
            }
            if (reward.Resources.Steel != 0) {
                RoleManager.SetResourceCollected(Resource.Steel);
                this.resourceDict[Resource.Steel].gameObject.SetActive(true);
            }

            this.ChangeItem(Resource.Lumber, reward.Resources.Lumber);
            this.ChangeItem(Resource.Food, reward.Resources.Food);
            this.ChangeItem(Resource.Marble, reward.Resources.Marble);
            this.ChangeItem(Resource.Steel, reward.Resources.Steel);
            this.ChangeItem(Resource.Gold, reward.Currency.Gold);
            this.ChangeItem(Resource.Gem, reward.Currency.Gem);
        }

        public bool ChangeResource(Dictionary<Resource, float> resourceDict) {
            bool isEnough = true;
            foreach (var pair in this.resourceDict) {
                float resourceValue;
                if (resourceDict.TryGetValue(pair.Key, out resourceValue)) {
                    MapResourceItemView resourceItemView =
                        pair.Value.GetComponent<MapResourceItemView>();
                    if (resourceValue > resourceItemView.LeftAmount) {
                        resourceDict[pair.Key] = resourceItemView.LeftAmount;
                        isEnough = false;
                    }
                }
            }
            if (!isEnough) {
                return false;
            }
            foreach (var pair in this.resourceDict) {
                float resourceValue;
                if (resourceDict.TryGetValue(pair.Key, out resourceValue)) {
                    MapResourceItemView resourceItemView =
                        pair.Value.GetComponent<MapResourceItemView>();
                    resourceItemView.CostAmount += resourceValue;
                }
            }
            return true;
        }

        public void SetResources() {
            foreach (Resource resource in this.resourceList) {
                this.SetItem(resource);
            }
        }

        // To do: need debug log collector, DEBUG 15306
        public void SetChangeResources(Protocol.Resources changeResources) {
            if (changeResources == null) {
                Debug.LogUpload("Resource is null.");
                return;
            }
            this.ChangeItem(Resource.Lumber, changeResources.Lumber);
            this.ChangeItem(Resource.Marble, changeResources.Marble);
            this.ChangeItem(Resource.Steel, changeResources.Steel);
            this.ChangeItem(Resource.Food, changeResources.Food);
        }

        public void SetCurrency() {
            this.SetItem(Resource.Gold);
            this.SetItem(Resource.Gem);
        }

        public void SetChangeCurrency(Protocol.Currency currency) {
            this.ChangeItem(Resource.Gold, currency.Gold);
            this.ChangeItem(Resource.Gem, currency.Gem);
        }

        public void SetChangeGem(int Gem) {
            this.ChangeItem(Resource.Gem, Gem);
        }

        private void SetItem(Resource resource) {
            GameObject pnlResourceItem;
            if (this.resourceDict.TryGetValue(resource, out pnlResourceItem)) {
                MapResourceItemView resourceItem =
                    pnlResourceItem.GetComponent<MapResourceItemView>();
                resourceItem.Resource = resource;
            }
        }

        private void ChangeItem(Resource resource, int amount) {
            if (amount == 0) {
                return;
            }

            GameObject pnlResourceItem;
            if (this.resourceDict.TryGetValue(resource, out pnlResourceItem)) {
                pnlResourceItem = this.resourceDict[resource];
                MapResourceItemView resourceItem = pnlResourceItem.GetComponent<MapResourceItemView>();
                resourceItem.Value += amount;
                //if (resource == Resource.Lumber) {
                //    Debug.LogError(resource + "+"+ amount + "  改变后的值" + resourceItem.Value);
                //}

            }
        }

        public void SetBtnFallen(bool isFallen) {
            this.viewPref.btnFallen.gameObject.SetActiveSafe(isFallen);
        }

        public void SetBtnNoviceState(bool canShow) {
            this.viewPref.btnNovice.gameObject.SetActiveSafe(canShow);
        }

        public void SetBtnMonthCard(bool canShow) {
            this.viewPref.btnMonthCard.gameObject.SetActiveSafe(canShow);
        }

        private void ShowForceReward(int RewardLevel) {
            this.viewModel.ShowForceReward();
        }

        private void ShowFieldReward() {
            this.viewModel.ShowFieldReward();
        }

        public void SetResourceScreenEffect() {
            ScreenEffectManager.SetHighlightFrame(this.viewPref.pnlResource,null,
                new Vector2(-3,-3),new Vector2(3,3));
        }

        public void SetPlayerInfoScreenEffect(UnityAction afterCallback) {
            ScreenEffectManager.SetHighlightFrame(this.viewPref.pnlIcon,
                afterCallback, new Vector2(-3, -3), new Vector2(3, 3));
        }

        private void OnBtnResourceClick() {
            ScreenEffectManager.EndImmediately();
            this.viewModel.ShowResource();
        }

        private void OnBtnPlayerInfoClick() {
            AudioManager.Play(AudioPath.actPrefix + "expand",
                AudioType.Action, AudioVolumn.High);
            if (this.showForceView) {
                this.viewModel.ShowForceReward();
            } else {
                this.viewModel.ShowPlayerInfo();
            }

        }

        private void OnBtnBuyGemClick() {
            this.viewModel.ShowPay();
        }

        //private void OnResourceFull() {
        //    bool needAnimate = false;
        //    foreach (GameObject resourceItem in this.resourceDict.Values) {
        //        if (resourceItem.GetComponent<MapResourceItemView>().IsFull) {
        //            //this.viewModel.IsResourceFull = true;
        //            needAnimate = true;
        //        }
        //    }
        //    if (!needAnimate) {
        //        //this.viewModel.IsResourceFull = false;
        //    }
        //}

        public void CollectResourceSimple(Resource type, int addAmount,
            Vector2 position, bool isDropOut) {
            // To do : Confirm Simple resource collect action
            this.CollectResource(type, addAmount,
                position, isDropOut, true);
        }

        public void CollectResource(Resource type, int addAmount, Vector2 origin,
           bool isPlayDroupOutAnimation) {
            this.CollectResource(type, addAmount, origin, isPlayDroupOutAnimation, true);
        }

        public void CollectTileLimit(Vector2 origin) {
            GameObject tileLimit = PoolManager.GetObject(PrefabPath.imgTileLimit, this.ui.transform);
            AnimationManager.Animate(tileLimit, "Move", start: origin,
                target: this.viewPref.imgTerrain.position, finishCallback: () => {
                    PoolManager.RemoveObject(tileLimit.gameObject);
                    AnimationManager.Animate(this.viewPref.imgTerrain.gameObject, "Beat");
                }, space: PositionSpace.World
            );
        }

        public void CollectResource(Resource type, int addAmount, Vector2 origin,
            bool isPlayDroupOutAnimation, bool isCollect) {
            GameObject resourceObj;
            if ((type != Resource.Force) &&
                !RoleManager.IsResourceCollected(type) &&
                this.resourceDict.TryGetValue(type, out resourceObj)) {
                RoleManager.SetResourceCollected(type);
                resourceObj.SetActive(true);
                AnimationManager.Animate(resourceObj, "Show");
            }

            this.reference++;
            GameObject controller = PoolManager.GetObject(PrefabPath.collectController, this.transform);
            AnimationManager.Stop(controller);
            int objectAmount = RoleManager.GetResourcePercent(type, addAmount);
            List<int> randomList = new List<int>(5);
            for (int i = 0; i < 5; i++) {
                randomList.Add(i + 1);
            }
            //int targetAmount;
            if (type == Resource.Force) {
                //targetAmount = 1;
                objectAmount = 1;
            }
            //else {
            //    //targetAmount = Mathf.RoundToInt(RoleManager.GetResource(type));
            //    MapResourceItemView itemView =
            //        this.resourceDict[type].GetComponent<MapResourceItemView>();
            //    targetAmount = this.GetResourcesItemValue(type) + (isCollect ? addAmount : -addAmount);
            //    itemView.TargetAmount = targetAmount;
            //}
            AnimationManager.AnimateEvent(controller, objectAmount, "Generate",
                () => {
                    this.StartCoroutine(
                        this.CollectAction(type, randomList, origin,
                        addAmount, isPlayDroupOutAnimation, isCollect)
                    );
                },
                () => {
                    if (--this.reference == 0) {
                        RoleManager.Instance.NeedCurrencyAnimation = false;
                        RoleManager.Instance.NeedResourceAnimation = false;
                    }
                    PoolManager.RemoveObject(controller);
                }
            );
        }

        private int GetResourcesItemValue(Resource resource, bool isCollect) {
            GameObject itemObject;
            if (this.resourceDict.TryGetValue(resource, out itemObject)) {
                MapResourceItemView resourceItemView =
                    itemObject.GetComponent<MapResourceItemView>();
                if (isCollect) {
                    return (int)Mathf.Max(resourceItemView.Value, resourceItemView.TargetAmount);
                } else {
                    return (int)Mathf.Min(resourceItemView.Value, resourceItemView.TargetAmount);
                }
            }
            return -1;
        }

        private List<int> DropOutRandomList = null;
        private IEnumerator CollectAction(Resource type, List<int> randomList,
            Vector2 origin, int addAmount, bool isPlayDroupOutAnimation, bool isCollect) {
            yield return YieldManager.EndOfFrame;
            if (randomList.Count > 0) {
                int index = UnityEngine.Random.Range(0, randomList.Count);
                int key = randomList[index];
                randomList.Remove(key);
                GameObject animation =
                    PoolManager.GetObject(PrefabPath.collectResource, AnimationManager.Instance.transform);
                Vector2 target = Vector2.zero;
                if (type == Resource.Gem) {
                    target = this.viewPref.imgGem.position;
                } else if (type == Resource.Force) {
                    target = this.viewPref.pnlForce.position;
                } else {
                    GameObject gameObject;
                    if (this.resourceDict.TryGetValue(type, out gameObject)) {
                        target = gameObject.transform.Find("ImgIcon").position;
                    }
                }

                if (isCollect) {
                    animation.transform.position = origin;
                } else {
                    animation.transform.position = target;
                    target = origin;
                }
                animation.transform.localScale = Vector3.one;
                SpriteRenderer img = animation.GetComponent<SpriteRenderer>();
                img.sprite = ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix, type.ToString().ToLower());

                int animationCount = 3;
                if (type == Resource.Gold) {
                    animationCount = 2;
                }
                AnimationManager.Animate(animation.GetComponent<Animator>(),
                    type.ToString() + (key % animationCount + 1));
                //   target += (Vector2)Camera.main.transform.position;
                if (isPlayDroupOutAnimation) {
                    if (DropOutRandomList == null) {
                        DropOutRandomList = new List<int>();
                        for (int i = 0; i < 7; i++) {
                            DropOutRandomList.Add(i + 10);
                        }
                        for (int i = 0; i < 7; i++) {
                            DropOutRandomList.Add(i + 20);
                        }
                    }
                    int outIndex = UnityEngine.Random.Range(0, DropOutRandomList.Count);
                    int outKey = DropOutRandomList[outIndex];
                    AnimationManager.Animate(animation, "MoveOut" + (outKey % 10).ToString(), start: origin,
                       space: PositionSpace.World, isTureOver: 1 == (outKey / 10),
                       finishCallback: () => {
                           StartCoroutine(CollectActionResource(
                               type, addAmount, key, animation, target, animation.transform.position, isCollect
                           ));
                       });
                } else {
                    StartCoroutine(CollectActionResource(
                        type, addAmount, key, animation, target, origin, isCollect
                    ));
                }
            }
        }

        private IEnumerator CollectActionResource(Resource type, int addAmount,
            int key, GameObject animation, Vector2 target, Vector2 origin, bool isCollect) {
            yield return YieldManager.GetWaitForSeconds(0.5f);
            AnimationManager.Animate(animation, "Move" + (key % 5 + 1), origin, target, () => {
                if (type == Resource.Force) {
                    AnimationManager.Animate(this.viewPref.pnlForce.gameObject, "Beat");
                } else {
                    MapResourceItemView itemView =
                        this.resourceDict[type].GetComponent<MapResourceItemView>();
                    if (key == 5) {
                        int targetAmount = this.GetResourcesItemValue(type, isCollect) + (isCollect ? addAmount : -addAmount);
                        //if (type == Resource.Gold) {
                        //    Debug.LogError(addAmount + " " + targetAmount + " " + this.GetResourcesItemValue(type));
                        //}
                        itemView.TargetAmount = targetAmount;
                    }
                    this.resourceDict[type].gameObject.SetActiveSafe(true);
                    GameObject image = itemView.GetResourceImg();
                    AnimationManager.Animate(image, "Beat", null);
                    AudioManager.Play(
                        AudioPath.showPrefix + "collect_" + type.ToString().ToLower(),
                        AudioType.Action, AudioVolumn.High, isAdditive: true
                    );
                }
                PoolManager.RemoveObject(animation);
            }, space: PositionSpace.World);
        }

        public void SetOpenServiceActivityHUD(int count, bool isShow, bool showNotice) {
            this.viewPref.btnLoginReward.gameObject.SetActiveSafe(isShow);
            if (isShow) {
                this.viewPref.imgNotice.gameObject.SetActive(showNotice);
                this.viewPref.txtNoticeNumber.text = count.ToString();
                this.viewPref.btnLoginReward.onClick.AddListener(this.OnBtnLoginRewardClick);
            }
        }

        public void CloseIconAnimation(bool isStart) {
            this.viewPref.imgIconFilledAnimation.SetActiveSafe(isStart);
            this.showForceView = isStart;
        }

        private void OnBtnFallenClick() {
            this.viewModel.ShowFallen();
        }

        private void OnBtnNoviceStateClick() {
            this.viewModel.ShowNoviceState();
        }

        private void OnBtnMonthCardClick() {
            this.viewModel.ShowPay();
        }

        private void OnBtnLoginRewardClick() {
            this.viewModel.ShowCampaignPanel();
        }

        #region FTE

        public void SetFteUI() {
            foreach (var ui in this.fteUIDict) {
                ui.Value.SetActiveSafe(FteManager.CheckUI(ui.Key));
            }
        }

        public void OnFteStep2Start(string index) {
            this.viewPref.pnlForce.gameObject.SetActiveSafe(false);
        }

        public void OnFteStep61Start(string index) {
            this.viewPref.topRectTransform.gameObject.SetActiveSafe(false);
            StartCoroutine(GetForce());
        }

        private IEnumerator GetForce() {
            yield return YieldManager.GetWaitForSeconds(1);
            this.viewPref.topRectTransform.gameObject.SetActiveSafe(true);
            this.viewPref.pnlForce.gameObject.SetActiveSafe(true);
            string forceLevel =
             (ForceRewardConf.GetForceLevel(RoleManager.GetForce()) + 1).ToString();
            int force = RoleManager.GetForce();
            ForceRewardConf forceConf = ForceRewardConf.GetConf(forceLevel);
            this.viewPref.imgCircleSliderFill.fillAmount = 0;
            AnimationManager.Animate(this.viewPref.topRectTransform.gameObject, "Show");
            AnimationManager.Animate(this.viewPref.pnlForce.gameObject, "Show", finishCallback: () => {
                StartCoroutine(FlyStar((force * 1.0f) / forceConf.force));
            });
        }

        private IEnumerator FlyStar(float force) {
            yield return YieldManager.GetWaitForSeconds(1.5f);
            int StarAmount = 8;
            for (int i = StarAmount; i > 0; i--) {
                GameObject star = PoolManager.GetObject(PrefabPath.fteStar, this.viewPref.pnlStar);
                FteStarView fteStarView = star.GetComponent<FteStarView>();
                fteStarView.eventForceFillAmount += () => {
                    this.viewPref.imgCircleSliderFill.fillAmount = force / StarAmount * (9 - i);
                };

                fteStarView.FlyStar(this.viewPref.pnlStar, this.viewPref.pnlForce);
                yield return YieldManager.GetWaitForSeconds(0.15f);
            }
            FteManager.EndFte(true);
        }


        #endregion
    }
}
