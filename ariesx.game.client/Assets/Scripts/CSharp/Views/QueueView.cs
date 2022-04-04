using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class QueueView: BaseView {
        private QueueViewModel viewModel;
        private TroopModel troopModel;

        #region UI element
        private Transform mapUI;
        private Transform pnlQueue;
        private Transform pnlbuildQueue;
        private CustomButton btnFold;
        private RectTransform imgFoldIcon;
        #endregion

        private bool isFold = true;
        public bool IsFold {
            get {
                return this.isFold;
            }
            set {
                if (this.isFold != value) {
                    this.isFold = value;
                    this.OnFoldValueChange();
                    this.viewModel.SetIsQueueShow(value);
                }
            }
        }


        public bool isInitUI = false;

        private Vector2 btnFoldScaler = Vector3.one;
        private DailyItemPreference temporaryViewfab;
        private Dictionary<string, QueueItemView> troopAllDict =
            new Dictionary<string, QueueItemView>(8);
        private List<string> troopEventList = new List<string>();

        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<QueueViewModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
            this.ui = UIManager.GetUI("UIMap.PnlQueue");
            this.mapUI = UIManager.GetUI("UIMap").transform;
            this.pnlQueue = this.ui.transform.Find("PnlTroopScroll").Find("PnlTroopContent");
            //this.queueVerticalLG = this.pnlQueue.GetComponent<VerticalLayoutGroup>();
            this.pnlbuildQueue = this.ui.transform.Find("PnlbuildContent");
            Transform pnlFold = this.ui.transform.Find("BtnFold");
            this.btnFold = pnlFold.GetComponent<CustomButton>();
            this.btnFold.onClick.AddListener(this.OnBtnFoldClick);
            this.imgFoldIcon = pnlFold.Find("PnlContent").Find("ImgIcon").GetComponent<RectTransform>();
            //this.InitView();
            this.isInitUI = true;
            this.BuildQueue();
        }



        public void BuildQueue() {
            this.viewModel.buildEventDict.Clear();
            Dictionary<string, EventBase> eventDict = EventManager.EventDict[Event.Build];
            List<string> eventIdList = new List<string>(eventDict.Keys);
            int length = eventDict.Count;
            for (int i = 0; i < 2; i++) {
                GameObject itemobj = this.pnlbuildQueue.GetChild(i).gameObject;
                DailyItemPreference dailyItem = itemobj.GetComponent<DailyItemPreference>();
                temporaryViewfab = dailyItem;
                if (i >= length && i < EventBuildClient.maxQueueCount) {
                    this.BuildArrayIdle();
                } else if (i >= EventBuildClient.maxQueueCount) {
                    this.BuildArrayUnlock();
                } else {
                    temporaryViewfab.buildingStatus.text = string.Empty;
                    dailyItem.slider.gameObject.SetActive(true);
                    this.viewModel.buildEventDict.Add(eventIdList[i], dailyItem);
                    dailyItem.pnlIdleAnimList.gameObject.SetActiveSafe(false);
                    for (var j = 0; j < 3; j++) {
                        AnimationManager.Stop(dailyItem.pnlIdleAnimList.GetChild(j).gameObject);
                    }
                    EventManager.AddEventAction(Event.Build, this.UpdateBuildProgressItem);
                }
            }

        }

        private void UpdateBuildProgressItem(EventBase eventBase) {
            DailyItemPreference viewPref;
            if (!this.viewModel.buildEventDict.TryGetValue(eventBase.id, out viewPref)) {
                return;
            }
            EventBuildClient eventBuild = eventBase as EventBuildClient;
            ElementBuilding thisBuild = null;
            foreach (var build in this.viewModel.BuildingDict) {
                if (build.Key == eventBuild.buildingName) {
                    thisBuild = build.Value;
                    Coord coord = thisBuild.Coord;
                    viewPref.btnItem.onClick.RemoveAllListeners();
                    viewPref.btnItem.onClick.AddListener(
                        () => { this.JumpBuildCoord(coord); });
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
            viewPref.buildingName.text = string.Concat(string.Format(LocalManager.GetValue("name_", buildingConf.type),
                GameHelper.GetBuildIndex(buildingConf.buildingName)), "(", GameHelper.GetLevelLocal(thisBuild.Level + 1), ")");
            viewPref.txtAmount.text = GameHelper.TimeFormat(left);
            if (left <= 0) {
                BuildArrayIdle();
            }
        }


        private void BuildArrayIdle() {
            temporaryViewfab.buildingStatus.text = LocalManager.GetValue(LocalHashConst.HUD_building_queue_idle);
            temporaryViewfab.slider.gameObject.SetActive(false);
            temporaryViewfab.btnItem.gameObject.SetActive(true);
            temporaryViewfab.btnItem.onClick.RemoveAllListeners();
            temporaryViewfab.btnItem.onClick.AddListener(this.JumpToBuildList);
            temporaryViewfab.pnlIdleAnimList.gameObject.SetActiveSafe(true);
            for (var i = 0; i < 3; i++) {
                AnimationManager.Animate(temporaryViewfab.pnlIdleAnimList.GetChild(i).gameObject, "Play",
                    Vector3.zero, Vector3.zero, null);
            }
        }

        private void BuildArrayUnlock() {
            temporaryViewfab.buildingStatus.text = LocalManager.GetValue(LocalHashConst.HUD_building_queue_unlock);
            temporaryViewfab.slider.gameObject.SetActive(false);
            temporaryViewfab.pnlIdleAnimList.gameObject.SetActiveSafe(false);
            temporaryViewfab.btnItem.gameObject.SetActive(true);
            temporaryViewfab.btnItem.onClick.RemoveAllListeners();
            temporaryViewfab.btnItem.onClick.AddListener(this.ShowUnlockConfirm);
            //temporaryViewfab.btnItem.Text = LocalManager.GetValue(LocalHashConst.button_go);
        }

        private void JumpToBuildList() {
            this.viewModel.ShowHouseKeeper();
        }

        private void JumpBuildCoord(Coord coord) {
            this.viewModel.JumpBuildCoord(coord);
        }

        private void ShowUnlockConfirm() {
            this.viewModel.ShowUnlockBuild(BuildQueue);
        }


        public void UpdateMarch(EventMarchClient queue) {
            this.troopEventList.Add(queue.troop.Id);
            QueueItemView troopQueue;
            if (!this.troopAllDict.TryGetValue(queue.troop.Id, out troopQueue)) {
                troopQueue = this.CreatItem(queue.troop);
            }
            if (!troopQueue.gameObject.activeSelf) {
                troopQueue.gameObject.SetActiveSafe(true);
                this.viewModel.DisFollowNewMarch();
            }
            troopQueue.Status = TroopStatus.Marching;
            troopQueue.UpdateItem(queue);
        }

        public void UpdateOtherEvent(EventTroop troopEvent, TroopStatus troopStatus) {
            this.troopEventList.Add(troopEvent.troopId);
            QueueItemView troopQueue;
            if (!this.troopAllDict.TryGetValue(troopEvent.troopId, out troopQueue)) {
                troopQueue = this.CreatItem(this.viewModel.TroopDict[troopEvent.troopId]);
            }
            troopQueue.Status = troopStatus;
            troopQueue.UpdateItem(troopEvent);

            //Troop troop = this.viewModel.TroopDict[troopEvent.troopId];
            //EventBase eventBase;
            //long left = 0;
            //long duration = 0;
            //long now = RoleManager.GetCurrentUtcTime();
            //foreach (HeroPosition heroPosition in troop.Positions) {
            //    eventBase =
            //        EventManager.GetRecruitEventByHeroName(heroPosition.Name);
            //    if (eventBase != null) {
            //        left += (eventBase.duration - now + eventBase.startTime);
            //        duration += eventBase.duration;
            //    }
            //}
            //left = (long)Mathf.Max(0, left);
            //troopQueue.UpdateItem(left, duration);
        }

        public void UpdateTroopStatusAt(Vector2 coordinate) {
            List<Troop> troopList = this.troopModel.GetTroopsAt(coordinate);
            foreach (Troop troop in troopList) {
                this.UpdateTroopStatus(troop);
            }
        }

        public void UpdateTroopStatus(Troop troop) {
            if (troop == null) return;
            QueueItemView troopQueue;
            if (!this.troopAllDict.TryGetValue(troop.Id, out troopQueue))
                return;
            TroopModel.TroopPositionReSort(troop);
            troopQueue.TroopId = troop.Id;
            troopQueue.HeroInfo = (troop.Positions.Count > 0) ?
                this.viewModel.HeroDict[troop.Positions[0].Name] : null;
            troopQueue.Name = TroopModel.GetTroopName(troop.ArmyCamp);
            troopQueue.Status = this.troopModel.GetTroopStatus(troop);
            this.RefreshQueueItemAnimation(troop);
        }

        public void RefreshQueueItemAnimation(Troop troop) {
            QueueItemView troopQueue;
            if (troop != null && this.troopAllDict.TryGetValue(troop.Id, out troopQueue)) {
                troopQueue.RefreshAnimate();
            }
        }

        public void InitView() {
            this.InitUI();
            List<Troop> troopList = this.viewModel.TroopList;
            int troopListCount = troopList.Count;
            if (troopListCount > 0) {
                for (int index = 0; index < troopListCount; index++) {
                    if (!this.troopAllDict.ContainsKey(troopList[index].Id)) {
                        this.CreatItem(troopList[index], this.troopModel.GetTroopStatus(troopList[index]));
                    }
                }
                this.Show();
            } else {
                this.Hide();
            }
        }

        private QueueItemView CreatItem(Troop troop, TroopStatus troopStatus = TroopStatus.Idle) {
            GameObject queueObj = PoolManager.GetObject(PrefabPath.pnlQueueItem, this.pnlQueue);
            QueueItemView queueUi = queueObj.GetComponent<QueueItemView>();
            TroopModel.TroopPositionReSort(troop);
            queueUi.TroopId = troop.Id;
            queueUi.HeroInfo = (troop.Positions.Count > 0) ?
                this.viewModel.HeroDict[troop.Positions[0].Name] : null;
            queueUi.OnTroopInfoClick = () => this.OnBtnTroopClick(troop.Id);
            queueUi.SetItemStatusVisible(true);
            this.troopAllDict.Add(troop.Id, queueUi);
            this.UpdateTroopStatus(troop);
            this.btnFold.transform.SetAsLastSibling();
            return queueUi;
        }

        private void OnBtnTroopClick(string id) {
            Vector2 position = Vector2.zero;
            this.viewModel.HideTileInfo();
            if (!this.viewModel.TroopDict[id].Idle) {
                EventMarchClient march = EventManager.GetMarchEventByTroopId(id);
                float cost = Mathf.Max((RoleManager.GetCurrentUtcTime() - march.startTime), 0);
                float percent = Mathf.Min((cost / march.duration), 1);
                Vector2 origin = MapUtils.CoordinateToPosition(march.origin);
                Vector2 target = MapUtils.CoordinateToPosition(march.target);
                position = (target - origin) * percent + origin;
                this.viewModel.FollowMarch(march.id);
            } else {
                Troop troop = this.viewModel.TroopDict[id];
                Vector2 coordinate = new Vector2(troop.Coord.X, troop.Coord.Y);
                this.viewModel.ShowTileTroop(troop.Id, coordinate);
            }
        }

        //public void SetQueueVisible(bool visible) {
        //this.pnlQueue.gameObject.SetActiveSafe(visible);
        //}

        #region FTE
        private readonly Vector2 FTE_ARROW_OFFSET = new Vector2(188, -58);
        public void OnFteStep151Start() {
            //this.SetQueueVisible(true);
            foreach (QueueItemView itemView in this.troopAllDict.Values) {
                FteManager.SetMask(itemView.transform, isHighlight: true,
                    rotation: -120, offset: this.FTE_ARROW_OFFSET);
                FteManager.SetChat(0);
                //AnimationManager.Animate(itemView.transform.parent.gameObject, "Flicker", () => {
                //    FteManager.StartFte();
                //});
            }
        }

        public void OnResourceStep1Start(string troop, bool needHighlight) {
            this.ShowPnlQueue();
            if (needHighlight) {
                FteManager.SetMask(this.troopAllDict[troop].transform, isHighlight: true,
                    rotation: -120, offset: this.FTE_ARROW_OFFSET, isEnforce: true);
            } else {
                FteManager.SetMask(this.troopAllDict[troop].transform,
                    rotation: -120, offset: this.FTE_ARROW_OFFSET, arrowParent: this.mapUI);
            }
        }

        public void OnFteStep53Start(string troop) {
            this.ShowPnlQueue();
            FteManager.SetMask(this.troopAllDict[troop].transform, isHighlight: true,
                rotation: -120, offset: this.FTE_ARROW_OFFSET, isEnforce: true);
        }

        public void OnRecruitStep1Start(string troop) {
            this.ShowPnlQueue();
            FteManager.SetMask(this.troopAllDict[troop].transform,
                rotation: -120, offset: this.FTE_ARROW_OFFSET, arrowParent: this.mapUI);
        }

        #endregion

        private void ShowPnlQueue() {
            if (this.isFold) {
                this.OnBtnFoldClick();
            }
        }

        private void OnBtnFoldClick() {
            this.viewModel.OnAnyOperate();
            this.IsFold = !this.isFold;
        }

        private void OnFoldValueChange() {
            if (this.isFold) {
                AnimationManager.Animate(this.ui.transform.gameObject, "Hide", () => {
                    this.UpdateQueueContent();
                }, isOffset: false);
            } else {
                this.UpdateQueueContent();
                AnimationManager.Animate(this.ui.transform.gameObject, "Show", isOffset: false);
            }
        }

        private void UpdateQueueContent() {
            if (!IsUIInit) {
                InitUI();
            }
            foreach (var troopInfo in this.troopAllDict) {
                troopInfo.Value.SetItemStatusVisible(!this.isFold);
            }
            btnFoldScaler.x = this.isFold ? 1 : -1;
            this.imgFoldIcon.localScale = btnFoldScaler;
        }
    }
}
