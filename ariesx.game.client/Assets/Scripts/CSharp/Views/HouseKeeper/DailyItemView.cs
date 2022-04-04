using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;

namespace Poukoute {
    public class DailyItemView : MonoBehaviour {
        // ui参数和自定义参数 分开.
        #region ui element
        [SerializeField]
        private Transform pnl2lDailyList;
        [SerializeField]
        private TextMeshProUGUI itemName;
        [SerializeField]
        private Button btnDetail;
        [SerializeField]
        private Transform pnlItemDetail;
        [SerializeField]
        private TextMeshProUGUI txtDescription;
        //public LayoutGroup lgBanner;
        //public ContentSizeFitter csfBanner;
        [SerializeField]
        private Transform PnlGo;
        [SerializeField]
        private CustomButton btnItem;
        #endregion



        //public Dictionary<Hero, string> heroList = new Dictionary<Hero, string>();
        [HideInInspector]
        public HouseKeeperDailyView parent;
        [HideInInspector]
        public int treasureMapAmount;
        [HideInInspector]
        public DailyAdvise selfName;
        [HideInInspector]
        public int tileCount;
        public UnityAction onIdle;
        public UnityAction onUnlock;


        private float detailOffset = 0;
        private long treasureMapRefresh;
        private bool isDetailVision = false;
        private DailyItemPreference temporaryViewfab;
        private HeroModel heroModel;
        private BuildModel buildModel;
        private TroopModel troopModel;
        private HouseKeeperDailyModel model;
        private DailyTaskModel taskModel;

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.buildModel.buildingDict;
            }
        }

        public Dictionary<string, DailyItemPreference> buildEventDict =
            new Dictionary<string, DailyItemPreference>();

        void Awake() {
            NetHandler.AddNtfHandler(typeof(TreasureMapNtf).Name, this.TreasureMapNtf);
            this.btnDetail.onClick.AddListener(this.OnBtnDetailClick);
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.taskModel = ModelManager.GetModelData<DailyTaskModel>();
            this.model = ModelManager.GetModelData<HouseKeeperDailyModel>();
        }

        private void TreasureMapNtf(IExtensible message) {
            TreasureMapNtf treasureMapNtf = message as TreasureMapNtf;
            this.treasureMapRefresh = treasureMapNtf.TreasureMap.CDExpireAt;
            this.treasureMapAmount = treasureMapNtf.TreasureMap.Amount;
        }

        private void Start() {
            this.pnlItemDetail.gameObject.SetActiveSafe(false);
        }

        private void ShowDetail() {
            //Debug.Log(SDKtest.ADD(100, 200));

            parent.CleanDetail(name: this.selfName);

            this.pnlItemDetail.gameObject.SetActiveSafe(true);
            Vector2 position = this.pnlItemDetail.
                GetComponent<RectTransform>().anchoredPosition;
            AnimationManager.Animate(this.pnlItemDetail.gameObject, "Show",
                position + Vector2.up * this.detailOffset, position, null);
            isDetailVision = true;
        }

        public void HideDetail(DailyAdvise name = DailyAdvise.Null) {
            if (name != selfName) {
                if (isDetailVision) {
                    //this.viewPref.pnlSkillDetail.gameObject.SetActiveSafe(false);
                    Vector2 position = this.pnlItemDetail.GetComponent<RectTransform>()
                        .anchoredPosition;
                    AnimationManager.Animate(this.pnlItemDetail.gameObject,
                        "Hide", position,
                        position + Vector2.up * this.detailOffset, () => {
                            this.pnlItemDetail.gameObject.SetActiveSafe(false);
                        });
                    isDetailVision = false;
                }
            }
        }

        private void OnBtnDetailClick() {
            if (!this.pnlItemDetail.gameObject.activeSelf) {
                this.ShowDetail();
            } else {
                this.HideDetail();
            }
        }

        public void ChooseVision(DailyAdvise whitch) {
            GameHelper.ClearChildren(this.pnl2lDailyList);
            selfName = whitch;
            txtDescription.text = LocalManager.GetValue(
                string.Concat("house_keeper_detail_", Enum.GetName(typeof(DailyAdvise), selfName)).CustomGetHashCode());
            switch (whitch) {
                case DailyAdvise.TileCount:
                    this.TileCount();
                    break;
                case DailyAdvise.BuildArray:
                    this.BuildArray();
                    break;
                case DailyAdvise.GetTribute:
                    this.GetTribute();
                    break;
                case DailyAdvise.HeroLevelUp:
                    this.HeroLevelUp();
                    break;
                case DailyAdvise.Force:
                    this.Force();
                    break;
                case DailyAdvise.DailyReward:
                    this.DailyReward();
                    break;
                case DailyAdvise.DailyTask:
                    this.DailyTask();
                    break;
                default:
                    break;
            }
            //if (this.gameObject.activeSelf) {
            //    this.lgBanner.enabled = true;
            //    this.csfBanner.enabled = true;
            //    base.StartCoroutine(this.OnInitEnd());
            //}
        }

        public void CleanEvent() {
            EventManager.RemoveEventAction(Event.Build, this.UpdateBuildProgressItem);
            //EventManager.RemoveEventAction(Event.Treasure, this.UpdateTreasureTimeItem);
            EventManager.RemoveEventAction(Event.Tribute, this.UpdateTributeTime);
            EventManager.RemoveEventAction(Event.DailyReward, this.UpdateDailyReward);
        }

        public void TileCount() {
            GameObject itemObj =
               PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
            DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
            viewfab.txtName.text = LocalManager.GetValue(LocalHashConst.player_fields);
            itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_tilelimit);
            viewfab.headImage.sprite
                = ArtPrefabConf.GetSprite("tile_limit");
            viewfab.slider.value
                = (float)RoleManager.GetPointDict().Count /
                (float)RoleManager.GetPointsLimit();
            viewfab.txtAmount.text
                = String.Concat(RoleManager.GetPointDict().Count,
                "/", RoleManager.GetPointsLimit());
            tileCount = RoleManager.GetPointDict().Count;
        }

        private void BuildArray() {
            buildEventDict.Clear();
            //EventManager.RemoveEventAction(Event.Build, this.UpdateBuildProgressItem);
            Dictionary<string, EventBase> eventDict = EventManager.EventDict[Event.Build];
            List<string> eventIdList = new List<string>(eventDict.Keys);
            int length = eventDict.Count;
            for (int i = 0; i < 2; i++) {
                GameObject itemObj =
                    PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
                DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
                viewfab.pnlUpicon.gameObject.SetActiveSafe(false);
                temporaryViewfab = viewfab;
                viewfab.txtName.gameObject.SetActiveSafe(false);
                itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_buildarray);
                viewfab.headImage.sprite
                   = ArtPrefabConf.GetSprite("HouseKeeper_Build");
                if (i >= length && i < EventBuildClient.maxQueueCount) {
                    this.BuildArrayIdle();
                } else if (i >= EventBuildClient.maxQueueCount) {
                    this.BuildArrayUnlock();
                } else {
                    viewfab.slider.gameObject.SetActive(true);
                    viewfab.btnItem.gameObject.SetActive(false);
                    this.buildEventDict.Add(eventIdList[i], viewfab);
                }
            }
            EventManager.AddEventAction(Event.Build, this.UpdateBuildProgressItem);
        }

        private void BuildArrayIdle() {
            temporaryViewfab.slider.gameObject.SetActive(false);
            temporaryViewfab.btnItem.gameObject.SetActive(true);
            temporaryViewfab.btnItem.onClick.RemoveAllListeners();
            temporaryViewfab.btnItem.onClick.AddListener(this.onIdle);
            temporaryViewfab.btnItem.Text = LocalManager.GetValue(LocalHashConst.button_go);
        }

        private void BuildArrayUnlock() {
            temporaryViewfab.slider.gameObject.SetActive(false);
            temporaryViewfab.btnItem.gameObject.SetActive(true);
            temporaryViewfab.btnItem.onClick.RemoveAllListeners();
            temporaryViewfab.btnItem.onClick.AddListener(this.onUnlock);
            temporaryViewfab.btnItem.Text =
                LocalManager.GetValue(LocalHashConst.button_open_second_build_queue);
        }

        private void UpdateBuildProgressItem(EventBase eventBase) {
            DailyItemPreference viewPref;
            if (!this.buildEventDict.TryGetValue(eventBase.id, out viewPref)) {
                return;
            }
            EventBuildClient eventBuild = eventBase as EventBuildClient;
            ElementBuilding thisBuild = null;
            foreach (var build in BuildingDict) {
                if (build.Key == eventBuild.buildingName) {
                    thisBuild = build.Value;
                    break;
                }
            }
            BuildingConf buildingConf = BuildingConf.GetConf(thisBuild.Name + "_" + (thisBuild.Level + 1));
            long left = eventBuild.duration -
                (RoleManager.GetCurrentUtcTime() - eventBuild.startTime);
            left = (long)Mathf.Max(0, left);
            viewPref.slider.value =
                (eventBuild.duration - left) / (float)eventBuild.duration;
            viewPref.txtAmount.gameObject.SetActiveSafe(true);
            viewPref.txtAmount.text = string.Concat("<color=#F3F98B>",
                string.Format(LocalManager.GetValue("name_", buildingConf.type),
                GameHelper.GetBuildIndex(buildingConf.buildingName)), "[",
                        GameHelper.GetLevelLocal(thisBuild.Level + 1),
                        "]</color>  ", GameHelper.TimeFormat(left));
            if (left == 0) {
                BuildArrayIdle();
            }
        }

        private void Treasure() {
            GameObject itemObj =
             PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
            DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
            temporaryViewfab = viewfab;
            itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_treasure);
            viewfab.headImage.sprite
              = ArtPrefabConf.GetSprite("HouseKeeper_Treasure");
            if (treasureMapRefresh < RoleManager.GetCurrentUtcTime() / 1000) {
                ViewTreasure();
            }

            //EventManager.AddEventAction(Event.Treasure, this.UpdateTreasureTimeItem);
        }

        private void ViewTreasure() {
            if (this.model.treasureMapAmount < 6) {
                temporaryViewfab.slider.value
                    = (float)this.model.treasureMapAmount / 6f;
                temporaryViewfab.txtAmount.text
                                = String.Concat(this.model.treasureMapAmount, "/", 6);
            } else {
                temporaryViewfab.slider.value
                  = 1;
                temporaryViewfab.txtAmount.text
                    = String.Concat(LocalManager.GetValue(LocalHashConst.house_keeper_treasurecanget));
            }
        }

        //public void UpdateTreasureTimeItem(EventBase eventBase) {
        //    long left = eventBase.startTime - RoleManager.GetCurrentUTCTime();
        //    left = (long)Mathf.Max(0, left);
        //    if (left == 0) {
        //        ViewTreasure();
        //    } else {
        //        temporaryViewfab.slider.value = (eventBase.duration - left)
        //            / (float)eventBase.duration;
        //    }
        //}

        public void GetTribute() {
            GameObject itemObj =
              PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
            DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
            viewfab.headImage.transform.parent
                .GetComponent<RectTransform>().sizeDelta = new Vector2(76.8f, 67.2f);
            temporaryViewfab = viewfab;
            viewfab.txtName.gameObject.SetActiveSafe(false);
            itemName.text = LocalManager.GetValue(LocalHashConst.tribute);
            viewfab.headImage.sprite
              = ArtPrefabConf.GetSprite("HouseKeeper_Tribute");
            TributeOK();
            EventManager.AddEventAction(Event.Tribute, this.UpdateTributeTime);
        }

        private void TributeOK() {
            temporaryViewfab.slider.value = 1;
            temporaryViewfab.txtAmount.text =
                this.buildModel.GetTownhallLevel() >= 3 ?
                LocalManager.GetValue(LocalHashConst.house_keeper_tributecanget) :
                LocalManager.GetValue(LocalHashConst.unlock_tribute);
        }

        public void UpdateTributeTime(EventBase eventBase) {
            long left = eventBase.startTime +
                eventBase.duration - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            if (left == 0) {
                TributeOK();
            } else {
                temporaryViewfab.slider.value =
                    (eventBase.duration - left) / (float)eventBase.duration;
                temporaryViewfab.txtAmount.text = string.Format(LocalManager.GetValue("house_keeper_canget_time"), GameHelper.TimeFormat(left));
            }
        }

        public void HeroLevelUp() {
            itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_canherolevelup);
            this.FindUpGradeHero();
        }

        private void FindUpGradeHero() {
            //heroList.Clear();
            GameObject itemObj;
            DailyItemPreference viewfab;
            foreach (var item in HeroDict) {
                if (item.Value.IsUpgradeable) {
                    int heroFragments = HeroLevelConf.GetHeroUpgradFragments(item.Value);
                    int fragmentCount = item.Value.FragmentCount;
                    int level = item.Value.Level;
                    if (item.Value.Level == 1) {
                        fragmentCount += 1;
                        heroFragments += 1;
                    }
                    //if (heroFragments > 0) {
                    itemObj = PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
                    viewfab = itemObj.GetComponent<DailyItemPreference>();
                    viewfab.txtName.text = LocalManager.GetValue(item.Value.Name);
                    viewfab.headImage.sprite = ArtPrefabConf.GetSprite(item.Value.Name, "_s");
                    viewfab.slider.value = 1.0f * fragmentCount / heroFragments;
                    viewfab.txtAmount.text = string.Concat(fragmentCount, "/", heroFragments);
                    //if (fragmentCount >= heroFragments) {
                    viewfab.txtOldLevel.text = string.Concat(LocalManager.GetValue(LocalHashConst.troop_format_order_level), level);
                    viewfab.txtNewLevel.text = string.Concat(LocalManager.GetValue(LocalHashConst.troop_format_order_level), (level + 1));
                    viewfab.pnlUpicon.gameObject.SetActiveSafe(true);
                    viewfab.txtAmount.gameObject.SetActiveSafe(false);
                    //} else {
                    //    viewfab.pnlUpicon.gameObject.SetActiveSafe(false);
                    //    viewfab.txtAmount.gameObject.SetActiveSafe(true);
                    //    viewfab.txtAmount.text = string.Concat(fragmentCount, "/", heroFragments);
                    //}
                    //}
                }
            }
        }

        void Force() {
            itemName.text = LocalManager.GetValue(LocalHashConst.player_influence);
            int force = RoleManager.GetForce();
            int forceBackground = ForceRewardConf.GetCurrentForceLevelForce();
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
            DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
            viewfab.iconRectTransform.sizeDelta = new Vector2(64.5f, 53.5f);
            int forceLevel =
                ForceRewardConf.GetForceLevel(RoleManager.GetForce());
            bool isShowForceLevel = (forceLevel != 0);
            viewfab.txtName.gameObject.SetActiveSafe(isShowForceLevel);
            if (isShowForceLevel) {
                ForceRewardConf forceConf = ForceRewardConf.AllForceConfDict[forceLevel];
                viewfab.txtName.text = forceConf.forceLocal;
            }
            viewfab.headImage.sprite = ArtPrefabConf.GetSprite("HouseKeeper_Force");
            viewfab.slider.value = (float)force / (float)forceBackground;
            viewfab.txtAmount.text = string.Concat(
                                GameHelper.GetFormatNum(force), "/",
                                GameHelper.GetFormatNum(forceBackground));
        }

        private void DailyEventSet() {
            EventManager.AddEventAction(Event.DailyReward, this.UpdateDailyReward);
        }

        List<DailyItemPreference> RewardList = new List<DailyItemPreference>();


        private void UpdateDailyReward(EventBase eventBase) {
            //Debug.Log( DateTime.Now.Ticks);
            //Debug.Log(RoleManager.GetCurrentUTCTime());
            long left =
                eventBase.startTime + eventBase.duration - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(0, left);
            if (left == 0) {
                troopModel.dailyLimit =
                    DailyRewardConf.GetNewDailyLimit(buildModel.GetTownhallLevel().ToString());
                this.parent.SortVision(DailyAdvise.DailyReward, 1);
                EventManager.RemoveEventAction(Event.DailyReward, UpdateDailyReward);
            } else {
                foreach (var item in RewardList) {
                    item.txtAmount.text =
                        string.Format(LocalManager.GetValue("house_keeper_laterrefresh"), GameHelper.TimeFormat(left));

                }
            }

        }

        private bool DailyRewardComplete() {
            if (this.troopModel.dailyLimit.ResourceCurrent.Lumber
                != this.troopModel.dailyLimit.ResourceLimit.Lumber)
                return true;
            if (this.troopModel.dailyLimit.ResourceCurrent.Food
                != this.troopModel.dailyLimit.ResourceLimit.Food)
                return true;
            if (this.troopModel.dailyLimit.ResourceCurrent.Steel
                != this.troopModel.dailyLimit.ResourceLimit.Steel)
                return true;
            if (this.troopModel.dailyLimit.ResourceCurrent.Marble
                != this.troopModel.dailyLimit.ResourceLimit.Marble)
                return true;
            if (this.troopModel.dailyLimit.GoldCurrent
                != this.troopModel.dailyLimit.GoldLimit)
                return true;
            if (this.troopModel.dailyLimit.ChestCurrent
                != this.troopModel.dailyLimit.ChestLimit)
                return true;
            return false;
        }

        private void OnBtnGoClick() {
            ScreenEffectManager.EndImmediately();
            this.parent.GetCanAddForceCoordReq();
            this.btnItem.onClick.RemoveAllListeners();
        }

        public void AddBtnGoListrners() {
            this.btnItem.onClick.AddListener(OnBtnGoClick);
        }

        private void RefreshDailyLimit() {
            RoleManager.GetCurrentChestLevel();
            this.troopModel.dailyLimit.ResourceCurrent.Lumber = 0;
            //this.troopModel.dailyLimit.ResourceLimit.Lumber
        }

        private void DailyReward() {
            DailyEventSet();
            RewardList.Clear();
            itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_dailyreward);
            this.PnlGo.gameObject.SetActiveSafe(DailyRewardComplete());
            this.btnItem.onClick.RemoveAllListeners();
            this.btnItem.onClick.AddListener(OnBtnGoClick);
            this.ADDDailyRewardItem(SpritePath.resourceIconPrefix,
                    Enum.GetName(typeof(Resource), Resource.Lumber).ToLower(),
                    this.troopModel.dailyLimit.ResourceCurrent.Lumber,
                    this.troopModel.dailyLimit.ResourceLimit.Lumber);
            this.ADDDailyRewardItem(SpritePath.resourceIconPrefix,
                    Enum.GetName(typeof(Resource), Resource.Food).ToLower(),
                    this.troopModel.dailyLimit.ResourceCurrent.Food,
                    this.troopModel.dailyLimit.ResourceLimit.Food);
            this.ADDDailyRewardItem(SpritePath.resourceIconPrefix,
                     Enum.GetName(typeof(Resource), Resource.Steel).ToLower(),
                     this.troopModel.dailyLimit.ResourceCurrent.Steel,
                     this.troopModel.dailyLimit.ResourceLimit.Steel);
            this.ADDDailyRewardItem(SpritePath.resourceIconPrefix,
                     Enum.GetName(typeof(Resource), Resource.Marble).ToLower(),
                     this.troopModel.dailyLimit.ResourceCurrent.Marble,
                     this.troopModel.dailyLimit.ResourceLimit.Marble);
            this.ADDDailyRewardItem(SpritePath.resourceIconPrefix,
                     Enum.GetName(typeof(Resource), Resource.Gold).ToLower(),
                     this.troopModel.dailyLimit.GoldCurrent,
                     this.troopModel.dailyLimit.GoldLimit);
            this.ADDDailyRewardItem("battle_report_lottery",
                     this.troopModel.dailyLimit.ChestCurrent,
                     this.troopModel.dailyLimit.ChestLimit);
        }

        private void ADDDailyRewardItem(string spriteName, int current, int limit) {
            GameObject itemObjChest =
               PoolManager.GetObject(PrefabPath.pnlRe2DailyItem, pnl2lDailyList);
            DailyItemPreference viewfabChest = itemObjChest.GetComponent<DailyItemPreference>();
            viewfabChest.txtName.text = LocalManager.GetValue(LocalHashConst.hero_title_lottery);
            viewfabChest.headImage.sprite
             = ArtPrefabConf.GetSprite(spriteName);
            viewfabChest.slider.value = (float)current / (float)limit;
            viewfabChest.txtAmount.text = string.Concat(current, "/", limit);
            if (current == limit)
                RewardList.Add(viewfabChest);
        }

        private void ADDDailyRewardItem(string prefixStr, string tailStr, int current, int limit) {
            GameObject itemObjChest =
               PoolManager.GetObject(PrefabPath.pnlRe2DailyItem, pnl2lDailyList);
            DailyItemPreference viewfabChest = itemObjChest.GetComponent<DailyItemPreference>();
            viewfabChest.txtName.text = LocalManager.GetValue("resource_" + tailStr);
            viewfabChest.headImage.sprite
             = ArtPrefabConf.GetSprite(prefixStr, tailStr);
            viewfabChest.slider.value = (float)current / (float)limit;
            viewfabChest.txtAmount.text = string.Concat(current, "/", limit);
            if (current == limit)
                RewardList.Add(viewfabChest);
        }

        public void DailyTask() {
            int max, doneTask;
            this.taskModel.HowManyIsDone(out doneTask, out max);
            itemName.text = LocalManager.GetValue(LocalHashConst.house_keeper_dailytask);
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnl2lDailyItem, pnl2lDailyList);
            DailyItemPreference viewfab = itemObj.GetComponent<DailyItemPreference>();
            viewfab.txtName.gameObject.SetActiveSafe(false);
            viewfab.headImage.sprite =
                ArtPrefabConf.GetSprite("HouseKeeper_DailyTask");
            viewfab.slider.value = (float)((float)doneTask / (float)max);
            //Debug.Log((float)((float)doneTask / (float)max));
            viewfab.txtAmount.text = string.Concat(doneTask, "/", max);
        }

        //private IEnumerator OnInitEnd() {
        //    yield return YieldManager.EndOfFrame;
        //    this.lgBanner.enabled = false;
        //    this.csfBanner.enabled = false;
        //}
    }
}
