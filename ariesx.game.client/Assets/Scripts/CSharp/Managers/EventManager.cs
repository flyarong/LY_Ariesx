using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class UpdateEvent : UnityEvent<EventBase> {
        public UpdateEvent() { }
    }

    public enum Event {
        March,
        Abandon,
        Build,
        Shield,
        Recruit,
        Tribute,
        Treasure,
        DailyReward,
        Lottry,
        GiveUpBuild,
        DefenderRecover,
        DurabilityRecover,
        UTCTimeZero,
        MonthCard,
        MonthCardDaily,
        SmartGiftBag,
        HouseKeeper,
        NoviceState,
        Campaign,
        LoginReward
    }

    public class EventBase {
        public string name;
        public string playeId;
        public string id;
        public long startTime;
        public long duration;
    }

    public class EventMarchClient : EventBase {
        public Vector2 origin;
        public Vector2 target;
        public MarchType type;
        public string playerName;
        public string allianceId;
        public string allianceName;
        public Troop troop;
    }

    public class EventAbandonClient : EventBase {
        public Vector2 coordinate;
        public bool canceled;
    }

    public class EventDurabilityRecover : EventBase {
        public Vector2 coordinate;
        public long finishAt;
    }

    public class EventDefenderRecover : EventBase {
        public Vector2 coordinate;
        public long finishAt;
    }

    public class EventBuildClient : EventBase {
        public static int maxQueueCount;
        public Vector2 coordinate;
        public string buildingName;
        public string buildingId;
    }


    public class EventGiveUpBuilding : EventBase {
        public Vector2 coordinate;
        public string buildingId;
    }

    public class EventShieldClient : EventBase {
        public Vector2 coordinate;
    }

    public class EventTroop : EventBase {
        public string troopId;
    }

    public class EventRecruitClient : EventTroop {
        public string heroName;
        public int armyAmount;
        public Protocol.Resources costedResources;
        public Protocol.Currency costedCurrency;
    }

    public class EventTributeClient : EventBase {
        public string groupName;
    }

    public class EventTreasureClient : EventBase {
        public string groupName;
    }
    public class EventDailyRewardClient : EventBase {
        public string groupName;
    }
    public class EventLotteryClient : EventBase {
        public string groupName;
    }

    public class EventExploreClient : EventTroop {
        public string buildingName;
    }

    public class EventUTCTimeZero : EventBase {
        public string groupName;
    }

    public class EventMonthCardClient : EventBase {

    }

    public class EventMonthCardDailyClient : EventBase {

    }

    public class EventSmartGiftBagClient : EventBase {

    }

    public class EventHouseKeeperClient : EventBase {

    }

    public class EventNoviceStateClient : EventBase {

    }

    public class EventManager : MonoBehaviour {
        private static EventManager self;
        public static EventManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("EventManager is not initialized.");
                }
                return self;
            }
        }

        public static UnityAction UTCTimeZeroAction;

        //QueueModel queueModel;
        Dictionary<Event, UpdateEvent> updateDict =
            new Dictionary<Event, UpdateEvent>();
        Dictionary<Event, Dictionary<string, EventBase>> eventDict =
            new Dictionary<Event, Dictionary<string, EventBase>>();
        public static Dictionary<Event, Dictionary<string, EventBase>> EventDict {
            get {
                return Instance.eventDict;
            }
        }
        List<string> finishedList = new List<string>();
        public static List<string> FinishedList {
            get {
                return Instance.finishedList;
            }
        }
        void Awake() {
            self = this;
            //this.queueModel = ModelManager.GetModelData<QueueModel>();
            eventDict[Event.March] = new Dictionary<string, EventBase>();
            eventDict[Event.Abandon] = new Dictionary<string, EventBase>();
            eventDict[Event.GiveUpBuild] = new Dictionary<string, EventBase>();
            eventDict[Event.Build] = new Dictionary<string, EventBase>();
            eventDict[Event.Shield] = new Dictionary<string, EventBase>();
            eventDict[Event.Recruit] = new Dictionary<string, EventBase>();
            eventDict[Event.Tribute] = new Dictionary<string, EventBase>();
            eventDict[Event.Treasure] = new Dictionary<string, EventBase>();
            eventDict[Event.DailyReward] = new Dictionary<string, EventBase>();
            eventDict[Event.Lottry] = new Dictionary<string, EventBase>();
            eventDict[Event.DefenderRecover] = new Dictionary<string, EventBase>();
            eventDict[Event.DurabilityRecover] = new Dictionary<string, EventBase>();
            eventDict[Event.UTCTimeZero] = new Dictionary<string, EventBase>();
            eventDict[Event.MonthCard] = new Dictionary<string, EventBase>();
            eventDict[Event.MonthCardDaily] = new Dictionary<string, EventBase>();
            eventDict[Event.SmartGiftBag] = new Dictionary<string, EventBase>();
            eventDict[Event.HouseKeeper] = new Dictionary<string, EventBase>();
            eventDict[Event.NoviceState] = new Dictionary<string, EventBase>();
            eventDict[Event.Campaign] = new Dictionary<string, EventBase>();
            eventDict[Event.LoginReward] = new Dictionary<string, EventBase>();

            updateDict[Event.March] = new UpdateEvent();
            updateDict[Event.Abandon] = new UpdateEvent();
            updateDict[Event.GiveUpBuild] = new UpdateEvent();
            updateDict[Event.Build] = new UpdateEvent();
            updateDict[Event.Shield] = new UpdateEvent();
            updateDict[Event.Recruit] = new UpdateEvent();
            updateDict[Event.Tribute] = new UpdateEvent();
            updateDict[Event.Treasure] = new UpdateEvent();
            updateDict[Event.DailyReward] = new UpdateEvent();
            updateDict[Event.Lottry] = new UpdateEvent();
            updateDict[Event.DurabilityRecover] = new UpdateEvent();
            updateDict[Event.DefenderRecover] = new UpdateEvent();
            updateDict[Event.UTCTimeZero] = new UpdateEvent();
            updateDict[Event.MonthCard] = new UpdateEvent();
            updateDict[Event.MonthCardDaily] = new UpdateEvent();
            updateDict[Event.SmartGiftBag] = new UpdateEvent();
            updateDict[Event.HouseKeeper] = new UpdateEvent();
            updateDict[Event.NoviceState] = new UpdateEvent();
            updateDict[Event.Campaign] = new UpdateEvent();
            updateDict[Event.LoginReward] = new UpdateEvent();

            UpdateManager.Regist(UpdateInfo.EventManager, this.UpdateAction);
        }


        private void UpdateAction() {
            // Delete finished event.
            if (this.finishedList.Count >= 1) {
                foreach (string id in this.finishedList) {
                    this.FinishEvent(id);
                }
                this.finishedList.Clear();
            }

            // Update event.
            foreach (var pair in this.eventDict) {
                foreach (EventBase eventBase in pair.Value.Values) {
                    try {
                        this.updateDict[pair.Key].Invoke(eventBase);
                    } catch (Exception e) {
                        Debug.LogError(pair.Key + e.ToString());
                    }
                }
            }
        }

        private void FinishEvent(string id) {
            foreach (var pair in this.eventDict) {
                if (pair.Value.ContainsKey(id)) {
                    pair.Value.Remove(id);
                    break;
                }
            }
        }

        // Event Action
        public static void AddEventAction(Event type, UnityAction<EventBase> action) {
            Instance.updateDict[type].AddListener(action);
        }

        public static void RemoveEventAction(Event type, UnityAction<EventBase> action) {
            Instance.updateDict[type].RemoveListener(action);
        }

        // Finish
        public static void FinishImmediate(string id) {
            Instance.FinishEvent(id);
        }

        public static void FinishAllEvent(Event type) {
            Instance.eventDict[type].Clear();
        }

        // March
        public static void RefreshEventMarches(List<EventMarch> EventMarches) {
            List<EventMarchClient> listToDel = new List<EventMarchClient>();
            foreach (EventMarchClient eventMarch in Instance.eventDict[Event.March].Values) {
                if (EventMarches.Find(item => item.Id.CustomEquals(eventMarch.id)) == null) {
                    listToDel.Add(eventMarch);
                }
            }

            int listToDelCount = listToDel.Count;
            for (int index = 0; index < listToDelCount; index++) {
                EventMarchClient march = listToDel[index];
                EventMarchNtf message = new EventMarchNtf();
                Coord origin = new Coord((int)march.origin.x, (int)march.origin.y);
                Coord target = new Coord((int)march.target.x, (int)march.target.y);
                message.EventMarch = new EventMarch() {
                    Id = march.id,
                    Troop = march.troop,
                    Origin = origin,
                    Target = target
                };
                message.Method = "del";


                NetEvent handler;
                if (NetHandler.Instance.dataHandleDict.TryGetValue(typeof(EventMarchNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
                if (NetHandler.Instance.ntfHandlerDict.TryGetValue(typeof(EventMarchNtf).Name, out handler)) {
                    handler.Invoke(message);
                }

            }


            foreach (EventMarch eventAbandon in EventMarches) {
                EventManager.AddMarchEvent(eventAbandon);
            }
        }

        public static void AddMarchEvent(EventMarch eventMarch) {
            Instance.InnerAddMarchEvent(eventMarch);
        }

        private void InnerAddMarchEvent(EventMarch eventMarch) {
            EventBase marchEvent;
            if (this.eventDict[Event.March].TryGetValue(eventMarch.Id, out marchEvent)) {
                marchEvent.startTime = eventMarch.StartAt * 1000;// + GameConst.TIMEZONE_OFFSET;
                marchEvent.duration = (eventMarch.FinishAt - eventMarch.StartAt) * 1000;
                return;
            }
            EventMarchClient march = new EventMarchClient {
                id = eventMarch.Id,
                playeId = eventMarch.PlayerId,
                name = typeof(EventMarchClient).Name,
                startTime = eventMarch.StartAt * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = (eventMarch.FinishAt - eventMarch.StartAt) * 1000,
                origin = new Vector2(eventMarch.Origin.X, eventMarch.Origin.Y),
                target = new Vector2(eventMarch.Target.X, eventMarch.Target.Y),
                type = (MarchType)eventMarch.Type,
                troop = eventMarch.Troop,
                playerName = eventMarch.PlayerName,
                allianceName = eventMarch.AllianceName,
                allianceId = eventMarch.AllianceId
            };
            this.eventDict[Event.March].Add(march.id, march);
        }

        public static EventMarchClient GetMarchEventByTroopId(string troopId) {
            foreach (EventMarchClient eventMarch in Instance.eventDict[Event.March].Values) {
                if (eventMarch.troop.Id == troopId) {
                    return eventMarch;
                }
            }
            return null;
        }


        public static EventMarchClient GetMarchById(string id) {
            if (Instance.eventDict[Event.March].ContainsKey(id)) {
                return (EventMarchClient)Instance.eventDict[Event.March][id];
            } else {
                return null;
            }
        }

        public static EventMarchClient GetMarchByTarget(Vector2 coordinate) {
            foreach (EventMarchClient eventMarch in Instance.eventDict[Event.March].Values) {
                if (eventMarch.target == coordinate) {
                    return eventMarch;
                }
            }
            return null;
        }

        //Campaign
        public static void AddCampaignEvent(long left) {
            EventBase eventBase = new EventBase() {
                id = "1",
                startTime = RoleManager.GetCurrentUtcTime(),
                duration = left
            };
            Instance.eventDict[Event.Campaign]["1"] = eventBase;
        }

        public static void AddLoginRewardEvent(long left) {
            EventBase eventBase = new EventBase() {
                id = "1",
                startTime = RoleManager.GetCurrentUtcTime(),
                duration = left
            };
            Instance.eventDict[Event.LoginReward]["1"] = eventBase;
        }

        // Defender and durability recover
        public static void AddDefenderRecoverEvent(string id, Vector2 coordinate, long duration) {
            EventDefenderRecover eventDefender = new EventDefenderRecover {
                id = id,
                coordinate = coordinate,
                finishAt = RoleManager.GetCurrentUtcTime() + duration
            };
            if (!Instance.eventDict[Event.DefenderRecover].ContainsKey(id)) {
                Instance.eventDict[Event.DefenderRecover].Add(id, eventDefender);
            }
        }

        public static void AddDurabilityRecoverEvent(string id, Vector2 coordinate, long duration) {
            EventDurabilityRecover eventDurability = new EventDurabilityRecover {
                id = id,
                coordinate = coordinate,
                finishAt = RoleManager.GetCurrentUtcTime() + duration
            };
            if (!Instance.eventDict[Event.DurabilityRecover].ContainsKey(id)) {
                Instance.eventDict[Event.DurabilityRecover].Add(id, eventDurability);
            }
        }

        // Abandon
        public static void RefreshAbandonEvents(List<EventAbandon> EventAbandons) {
            List<EventAbandonClient> listToDel = new List<EventAbandonClient>();
            foreach (EventAbandonClient eventAbandon in Instance.eventDict[Event.Abandon].Values) {
                if (EventAbandons.Find(item => item.Id.CustomEquals(eventAbandon.id)) == null) {
                    listToDel.Add(eventAbandon);
                }
            }

            int listToDelCount = listToDel.Count;
            for (int index = 0; index < listToDelCount; index++) {
                EventAbandonClient abandon = listToDel[index];
                EventAbandonNtf message = new EventAbandonNtf();
                Coord coord = new Coord() {
                    X = (int)abandon.coordinate.x,
                    Y = (int)abandon.coordinate.y
                };
                message.EventAbandon = new EventAbandon() {
                    Id = abandon.id,
                    Coord = coord
                };
                message.Method = "del";
                NetEvent handler;
                if (NetHandler.Instance.dataHandleDict.TryGetValue(typeof(EventAbandonNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
                if (NetHandler.Instance.ntfHandlerDict.TryGetValue(typeof(EventAbandonNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
            }

            foreach (EventAbandon eventAbandon in EventAbandons) {
                EventManager.AddAbandonEvent(eventAbandon);
            }
        }

        public static void AddAbandonEvent(EventAbandon eventAbandon) {
            Instance.InnerAddAbandonEvent(eventAbandon);
        }

        private void InnerAddAbandonEvent(EventAbandon eventAbandon) {
            EventBase abandonEvent;
            if (this.eventDict[Event.Abandon].TryGetValue(eventAbandon.Id, out abandonEvent)) {
                abandonEvent.startTime = eventAbandon.StartAt * 1000;// + GameConst.TIMEZONE_OFFSET;
                abandonEvent.duration = eventAbandon.FinishAt * 1000 - eventAbandon.StartAt * 1000;
                return;
            }
            EventAbandonClient abandon = new EventAbandonClient() {
                id = eventAbandon.Id,
                playeId = eventAbandon.PlayerId,
                name = typeof(EventAbandonClient).Name,
                startTime = eventAbandon.StartAt * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = eventAbandon.FinishAt * 1000 - eventAbandon.StartAt * 1000,
                coordinate = new Vector2(eventAbandon.Coord.X, eventAbandon.Coord.Y)
            };
            this.eventDict[Event.Abandon].Add(abandon.id, abandon);
        }


        public static bool IsTileAbandon(Vector2 coordinate) {
            foreach (EventAbandonClient abandon in Instance.eventDict[Event.Abandon].Values) {
                if (abandon.coordinate == coordinate) {
                    return true;
                }
            }
            return false;
        }

        // give up building
        public static void RefreshGiveUpBuildingEvents(List<EventGiveUp> EventGiveUps) {
            List<EventGiveUpBuilding> listToDel = new List<EventGiveUpBuilding>();
            foreach (EventGiveUpBuilding eventGiveUp in Instance.eventDict[Event.GiveUpBuild].Values) {
                if (EventGiveUps.Find(item => item.Id.CustomEquals(eventGiveUp.id)) == null) {
                    listToDel.Add(eventGiveUp);
                }
            }

            int listToDelCount = listToDel.Count;
            for (int index = 0; index < listToDelCount; index++) {
                EventGiveUpBuilding giveupBuilding = listToDel[index];
                EventGiveUpNtf message = new EventGiveUpNtf();
                Coord coord = new Coord() {
                    X = (int)giveupBuilding.coordinate.x,
                    Y = (int)giveupBuilding.coordinate.y
                };
                message.EventGiveUp = new EventGiveUp() {
                    Id = giveupBuilding.id,
                    Coord = coord
                };
                message.Method = "del";
                NetEvent handler;
                if (NetHandler.Instance.dataHandleDict.TryGetValue(typeof(EventGiveUpNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
                if (NetHandler.Instance.ntfHandlerDict.TryGetValue(typeof(EventGiveUpNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
            }


            foreach (EventGiveUp eventBuildGiveUp in EventGiveUps) {
                EventManager.AddGiveUpBuildingEvent(eventBuildGiveUp);
            }
        }

        public static void AddGiveUpBuildingEvent(EventGiveUp eventGiveUp) {
            Instance.InnerAddGiveUpBuildingEvent(eventGiveUp);
        }

        private void InnerAddGiveUpBuildingEvent(EventGiveUp eventGiveUp) {
            EventBase giveUpEvent;
            if (this.eventDict[Event.GiveUpBuild].TryGetValue(eventGiveUp.Id, out giveUpEvent)) {
                giveUpEvent.startTime = eventGiveUp.StartAt * 1000;// + GameConst.TIMEZONE_OFFSET;
                giveUpEvent.duration = eventGiveUp.FinishAt * 1000 - eventGiveUp.StartAt * 1000;
                return;
            }
            EventGiveUpBuilding giveUpBuilding = new EventGiveUpBuilding {
                id = eventGiveUp.Id,
                playeId = eventGiveUp.PlayerId,
                name = typeof(EventGiveUpBuilding).Name,
                startTime = eventGiveUp.StartAt * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = eventGiveUp.FinishAt * 1000 - eventGiveUp.StartAt * 1000,
                coordinate = new Vector2(eventGiveUp.Coord.X, eventGiveUp.Coord.Y)
            };
            this.eventDict[Event.GiveUpBuild].Add(giveUpBuilding.id, giveUpBuilding);
        }

        public static bool IsTileInBuildEvent(Vector2 coordinate) {
            foreach (EventBuildClient build in Instance.eventDict[Event.Build].Values) {
                if (build.coordinate == coordinate &&
                    !FinishedList.Contains(build.id)) {
                    return true;
                }
            }
            return false;
        }

        public static EventGiveUpBuilding GetGiveUpBuildingByCoordinate(Vector2 coordinate) {
            foreach (EventGiveUpBuilding eventGiveUp in Instance.eventDict[Event.GiveUpBuild].Values) {
                if (eventGiveUp.coordinate == coordinate) {
                    return eventGiveUp;
                }
            }
            return null;
        }

        public static bool IsGiveUpBuild(Vector2 coordinate) {
            foreach (EventGiveUpBuilding eventGiveUp in Instance.eventDict[Event.GiveUpBuild].Values) {
                if (eventGiveUp.coordinate == coordinate &&
                    !FinishedList.Contains(eventGiveUp.id)) {
                    return true;
                }
            }
            return false;
        }

        // Build
        public static void RefreshEventBuilds(List<EventBuild> EventBuilds) {
            List<EventBuildClient> listToDel = new List<EventBuildClient>();
            foreach (EventBuildClient buildEvent in Instance.eventDict[Event.Build].Values) {
                if (EventBuilds.Find(item => item.Id.CustomEquals(buildEvent.id)) == null) {
                    listToDel.Add(buildEvent);
                }
            }

            int listToDelCount = listToDel.Count;
            for (int index = 0; index < listToDelCount; index++) {
                EventBuildClient build = listToDel[index];
                EventBuildNtf message = new EventBuildNtf();
                Coord coord = new Coord() {
                    X = (int)build.coordinate.x,
                    Y = (int)build.coordinate.y
                };
                message.EventBuild = new EventBuild() {
                    Id = build.id,
                    Coord = coord,
                    BuildingName = build.buildingName
                };
                message.Method = "del";
                NetEvent handler;
                if (NetHandler.Instance.dataHandleDict.TryGetValue(typeof(EventBuildNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
                if (NetHandler.Instance.ntfHandlerDict.TryGetValue(typeof(EventBuildNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
            }



            foreach (EventBuild eventAbandon in EventBuilds) {
                EventManager.AddBuildEvent(eventAbandon);
            }
        }

        public static void AddBuildEvent(EventBuild eventBuild) {
            Instance.InnerAddBuildEvent(eventBuild);
        }

        private void InnerAddBuildEvent(EventBuild eventBuild) {
            EventBase buildEvent;
            if (this.eventDict[Event.Build].TryGetValue(eventBuild.Id, out buildEvent)) {
                buildEvent.startTime = eventBuild.StartAt * 1000;// + GameConst.TIMEZONE_OFFSET;
                buildEvent.duration = (eventBuild.FinishAt - eventBuild.StartAt) * 1000;
                return;
            }
            EventBuildClient build = new EventBuildClient {
                id = eventBuild.Id,
                buildingName = eventBuild.BuildingName,
                playeId = eventBuild.PlayerId,
                name = typeof(EventBuildClient).Name,
                startTime = eventBuild.StartAt * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = eventBuild.FinishAt * 1000 - eventBuild.StartAt * 1000,
                coordinate = new Vector2(eventBuild.Coord.X, eventBuild.Coord.Y)
            };
            this.eventDict[Event.Build].Add(build.id, build);
        }

        public static EventBuildClient GetBuildEventByCoordinate(Vector2 coordinate) {
            foreach (EventBuildClient eventBuild in Instance.eventDict[Event.Build].Values) {
                if (eventBuild.coordinate == coordinate) {
                    return eventBuild;
                }
            }
            return null;
        }

        public static bool IsBuildingUnderBuildEvent(string buildingName) {
            foreach (EventBuildClient eventBuild in Instance.eventDict[Event.Build].Values) {
                if (eventBuild.buildingName.CustomEquals(buildingName)) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTileGiveUpBuilding(Vector2 coordinate) {
            foreach (EventGiveUpBuilding build in Instance.eventDict[Event.GiveUpBuild].Values) {
                if (build.coordinate == coordinate &&
                    !FinishedList.Contains(build.id)) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsBuildEventMaxFull() {
            //Debug.LogError("GetBuildEventNum count:" + Instance.buildDict.Values.Count);
            // To do: config the queue length in configure
            return Instance.eventDict[Event.Build].Values.Count >=
                GameConst.MAX_BUILD_QUEUE_COUNT;
        }

        public static bool IsBuildEventFull() {
            return Instance.eventDict[Event.Build].Values.Count >=
                EventBuildClient.maxQueueCount;
        }
        // Recruit

        public static void RefreshEventRecruits(List<EventRecruit> EventRecruits) {
            List<EventRecruitClient> listToDel = new List<EventRecruitClient>();
            foreach (EventRecruitClient eventRecruit in Instance.eventDict[Event.Recruit].Values) {
                if (EventRecruits.Find(item => item.Id.CustomEquals(eventRecruit.id)) == null) {
                    Debug.LogError(eventRecruit.id);
                    listToDel.Add(eventRecruit);
                }
            }

            int listToDelCount = listToDel.Count;
            NetEvent handler;
            for (int index = 0; index < listToDelCount; index++) {
                EventRecruitClient recruit = listToDel[index];
                EventRecruitNtf message = new EventRecruitNtf();
                message.EventRecruit = new EventRecruit() {
                    TroopId = recruit.troopId,
                    Id = recruit.id,
                    HeroName = recruit.heroName
                };
                message.Method = "del";
                if (NetHandler.Instance.dataHandleDict.TryGetValue(typeof(EventRecruitNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
                if (NetHandler.Instance.ntfHandlerDict.TryGetValue(typeof(EventRecruitNtf).Name, out handler)) {
                    handler.Invoke(message);
                }
            }


            foreach (EventRecruit eventRecruit in EventRecruits) {
                Debug.LogError(eventRecruit.Id);
                EventManager.AddRecruitEvent(eventRecruit);
            }
        }

        public static void AddRecruitEvent(EventRecruit eventRecruit) {
            Instance.InnerAddRecruitEvent(eventRecruit);
        }

        private void InnerAddRecruitEvent(EventRecruit eventRecruit) {
            EventBase recruitEvent;
            if (this.eventDict[Event.Recruit].TryGetValue(eventRecruit.Id, out recruitEvent)) {
                recruitEvent.startTime = eventRecruit.StartAt * 1000;// + GameConst.TIMEZONE_OFFSET;
                recruitEvent.duration = eventRecruit.FinishAt * 1000 - eventRecruit.StartAt * 1000;
                return;
            }
            EventRecruitClient queueRecruit = new EventRecruitClient {
                id = eventRecruit.Id,
                playeId = eventRecruit.PlayerId,
                name = typeof(EventRecruitClient).Name,
                startTime = eventRecruit.StartAt * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = eventRecruit.FinishAt * 1000 - eventRecruit.StartAt * 1000,
                heroName = eventRecruit.HeroName,
                armyAmount = eventRecruit.Amount,
                troopId = eventRecruit.TroopId,
                costedResources = eventRecruit.CostedResources,
                costedCurrency = eventRecruit.CostedCurrency
            };
            this.eventDict[Event.Recruit].Add(eventRecruit.Id, queueRecruit);
        }


        public static EventRecruitClient GetRecruitEventByHeroName(string heroName) {
            foreach (EventRecruitClient eventRecruit in
                Instance.eventDict[Event.Recruit].Values) {
                if (eventRecruit.heroName.CustomEquals(heroName) &&
                    !FinishedList.Contains(eventRecruit.id)) {
                    return eventRecruit;
                }
            }
            return null;
        }

        public static bool IsTroopUnderTreatment(string troopId) {
            foreach (EventRecruitClient eventRecruit in
                            Instance.eventDict[Event.Recruit].Values) {
                if (!FinishedList.Contains(eventRecruit.id) &&
                    eventRecruit.troopId.CustomEquals(troopId)) {
                    return true;
                }
            }
            return false;
        }

        // Shield
        public static void AddShieldEvent(Point point) {
            Instance.InnerAddShieldEvent(point);
        }

        private void InnerAddShieldEvent(Point point) {
            long eventFinishAt = this.AddProtectEvent(point);
            long duration = eventFinishAt - RoleManager.GetCurrentUtcTime();
            if (duration <= 0) {
                return;
            }
            EventBase sheildEvent;
            if (this.eventDict[Event.Shield].TryGetValue(point.ElementId, out sheildEvent)) {
                this.FinishEvent(sheildEvent.id);
            }
            EventShieldClient tribute = new EventShieldClient {
                id = point.ElementId,
                startTime = RoleManager.GetCurrentUtcTime(),
                duration = duration,
                coordinate = new Vector2(point.Coord.X, point.Coord.Y)
            };
            this.eventDict[Event.Shield].Add(tribute.id, tribute);
        }

        private long AddProtectEvent(Point point) {
            long eventFinishAt = 0;
            long currentUtcTIme = RoleManager.GetCurrentUtcTime() / 1000;
            if (point.FreshProtectionExpireAt > currentUtcTIme) {
                eventFinishAt = point.FreshProtectionExpireAt * 1000;
            } else if (point.AvoidExpireAt > currentUtcTIme) {
                eventFinishAt = point.AvoidExpireAt * 1000;
            }
            return eventFinishAt;
        }

        public static bool IsShieldEvent(Vector2 coordinate) {
            foreach (EventShieldClient shield in Instance.eventDict[Event.Shield].Values) {
                if (shield.coordinate == coordinate &&
                    !FinishedList.Contains(shield.id)) {
                    return true;
                }
            }
            return false;
        }

        // Tribute
        public static void AddTributeEvent(long startTime) {
            Instance.InnerAddTributeEvent(startTime);
        }

        private void InnerAddTributeEvent(long startTime) {
            this.eventDict[Event.Tribute].Clear();
            EventTributeClient tribute = new EventTributeClient {
                id = "1",
                name = typeof(EventTributeClient).Name,
                startTime = startTime * 1000,// + GameConst.TIMEZONE_OFFSET,
                duration = 3 * 1000 * 3600,
            };
            this.eventDict[Event.Tribute].Add(tribute.id, tribute);
        }

        public static EventTributeClient GetTribute() {
            if (Instance.eventDict[Event.Tribute].Count == 1) {
                return (EventTributeClient)Instance.eventDict[Event.Tribute]["Tribute"];
            } else {
                Debug.LogError("Tribute Wrong!");
                return null;
            }
        }
        public static void AddTreasureEvent(long startTime) {
            Instance.InnerAddTreasureEvent(startTime);
        }

        private void InnerAddTreasureEvent(long startTime) {
            this.eventDict[Event.Treasure].Clear();
            EventTreasureClient Treasure = new EventTreasureClient {
                id = "1",
                name = typeof(EventTreasureClient).Name,
                startTime = startTime * 1000,// + GameConst.TIMEZONE_OFFSET, 
                duration = startTime - RoleManager.GetCurrentUtcTime()
            };
            this.eventDict[Event.Treasure].Add(Treasure.name, Treasure);
        }

        public static EventTreasureClient GetTreasure() {
            if (Instance.eventDict[Event.Treasure].Count == 1) {
                return (EventTreasureClient)Instance.eventDict[Event.Treasure]["Treasure"];
            } else {
                Debug.LogError("Treasure Wrong!");
                return null;
            }
        }
        public static void AddDailyRewardEvent(long endAt) {
            Instance.InnerAddDailyRewardEvent(endAt);
        }

        private void InnerAddDailyRewardEvent(long endAt) {
            this.eventDict[Event.DailyReward].Clear();
            EventDailyRewardClient DailyReward = new EventDailyRewardClient {
                id = "1",
                name = typeof(EventDailyRewardClient).Name,
                startTime = RoleManager.GetCurrentUtcTime(),
                duration = endAt * 1000 - RoleManager.GetCurrentUtcTime(),
            };
            this.eventDict[Event.DailyReward].Add(DailyReward.name, DailyReward);
        }

        public static void AddMonthCardEvent(long duration) {
            Instance.InnerAddMonthCardEvent(duration);
        }

        public void InnerAddMonthCardEvent(long duration) {
            this.eventDict[Event.MonthCard].Clear();
            EventMonthCardClient monthCard = new EventMonthCardClient {
                id = "1",
                name = typeof(EventMonthCardClient).Name,
                duration = duration
            };
            this.eventDict[Event.MonthCard].Add(monthCard.name, monthCard);
        }

        public static void AddMonthCardDailyEvent(long endAt) {
            Instance.InnerAddMonthCardDaily(endAt);
        }

        public void InnerAddMonthCardDaily(long endAt) {
            this.eventDict[Event.MonthCardDaily].Clear();
            EventMonthCardDailyClient monthCardDaily = new EventMonthCardDailyClient {
                id = "1",
                name = typeof(EventMonthCardDailyClient).Name,
                duration = endAt
            };
            this.eventDict[Event.MonthCardDaily].Add(monthCardDaily.name, monthCardDaily);
        }

        public static void AddSmartGiftBagEvent(long endAt) {
            Instance.InnerAddSmartGiftBagEvent(endAt);
        }

        public void InnerAddSmartGiftBagEvent(long endAt) {
            this.eventDict[Event.SmartGiftBag].Clear();
            EventSmartGiftBagClient smartGiftBag = new EventSmartGiftBagClient {
                id = "1",
                name = typeof(EventSmartGiftBagClient).Name,
                startTime = RoleManager.GetCurrentUtcTime(),
                duration = endAt * 1000 - RoleManager.GetCurrentUtcTime()
            };
            this.eventDict[Event.SmartGiftBag].Add(smartGiftBag.name, smartGiftBag);
        }

        public static void AddHouseKeeperEvent() {
            Instance.InnerAddHousekeeperEvent();
        }

        private void InnerAddHousekeeperEvent() {
            this.eventDict[Event.HouseKeeper].Clear();
            EventHouseKeeperClient HouseKeeper = new EventHouseKeeperClient {
                name = typeof(EventHouseKeeperClient).Name,
                startTime = RoleManager.GetCurrentUtcTime()
            };
            this.eventDict[Event.HouseKeeper].Add(HouseKeeper.name, HouseKeeper);
        }

        public static void AddNoviceState(long startTime) {
            Instance.InnerAddNoviceStateEvent(startTime);
        }

        public void InnerAddNoviceStateEvent(long startTime) {
            this.eventDict[Event.NoviceState].Clear();
            EventNoviceStateClient NoviceState = new EventNoviceStateClient {
                name = typeof(EventNoviceStateClient).Name,
                startTime = startTime
            };
            this.eventDict[Event.NoviceState].Add(NoviceState.name, NoviceState);
        }

        public static void AddUTCTimeZeroEvent() {
            Instance.InnerAddUTCTimeZeroEvent();
        }

        public static void AddUTCTimeZeroAction() {
            EventManager.AddEventAction(Event.UTCTimeZero, Instance.UpdateUTCTimeZero);
        }

        private void InnerAddUTCTimeZeroEvent() {
            this.eventDict[Event.UTCTimeZero].Clear();
            EventUTCTimeZero UTCTimeZero = new EventUTCTimeZero {
                name = typeof(EventUTCTimeZero).Name,
                startTime = RoleManager.GetNextZeroTime()
            };
            this.eventDict[Event.UTCTimeZero].Add(UTCTimeZero.name, UTCTimeZero);
        }

        private void UpdateUTCTimeZero(EventBase eventBase) {
            long left =
                eventBase.startTime - RoleManager.GetCurrentUtcTime();
            left = (long)Mathf.Max(-3000, left);
            if (left == -3000) {
                if (UTCTimeZeroAction != null) {
                    UTCTimeZeroAction.InvokeSafe();
                }
                EventManager.AddUTCTimeZeroEvent();
            }
        }
    }
}
