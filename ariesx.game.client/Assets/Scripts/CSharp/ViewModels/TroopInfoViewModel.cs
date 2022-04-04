using ProtoBuf;
using Protocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class TroopInfoViewModel: BaseViewModel, IViewModel {
        MapViewModel parent;
        TroopModel model;
        MapModel mapModel;
        BuildModel buildModel;
        HeroModel heroModel;
        TroopInfoView view;
        MarchConfirmViewModel marchConfirmViewModel;

        public Dictionary<string, Troop> TroopDict {
            get {
                return this.model.troopDict;
            }
        }

        public Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.buildModel.buildingDict;
            }
        }

        public MarchAttributes CurrentMarchAttributes {
            get {
                return this.model.currentMarchAttributes;
            }
        }

        public string CurrentTroop {
            get {
                return this.model.currentTroop;
            }
            set {
                this.model.currentTroop = value;
                this.NeedRefresh = true;
                //   this.formationViewModel.NeedRefresh = true;
            }
        }

        public Vector2 Target {
            get {
                return this.model.target;
            }
            set {
                if (this.model.target != value) {
                    this.model.target = value;
                    this.NeedRefresh = true;
                }
            }
        }

        public TroopViewType ViewType {
            get {
                return this.model.viewType;
            }
            set {
                if (this.model.viewType != value) {
                    this.model.viewType = value;
                    this.NeedRefresh = true;
                }
            }
        }

        public DailyLimit DailyLimit {
            get {
                return this.model.dailyLimit;
            }
            set {
                this.model.dailyLimit = value;
            }
        }

        public Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public ElementBuilding CurExplorerCamp {
            get; set;
        }

        public bool NeedRefresh {
            get; set;
        }

        private UnityAction afterHide = null;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<TroopModel>();
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();

            this.view = this.gameObject.AddComponent<TroopInfoView>();
            this.marchConfirmViewModel = PoolManager.GetObject<MarchConfirmViewModel>(this.transform);

            NetHandler.AddDataHandler(typeof(NewTroopNtf).Name, this.NewTroopNtf);
            NetHandler.AddDataHandler(typeof(DailyLimitNtf).Name, this.DailyLimitNtf);

            FteManager.SetStartCallback(GameConst.RESOURCE_LEVEL, 4, this.OnResourceStep4Start);
            FteManager.SetEndCallback(GameConst.RESOURCE_LEVEL, 4, this.OnResourceStep4End);
            this.NeedRefresh = true;
        }

        public void Show(UnityAction afterHide = null) {
            this.afterHide = afterHide;
            if (this.NeedRefresh) {
                this.view.PlayShow(() => {
                    if (afterHide == null) {
                        this.parent.OnAddViewAboveMap(this);
                    }
                });
                this.view.EnableBtnSend();
                this.NeedRefresh = false;
            }
        }

        public void Hide(bool needHideTileInfo = true) {
            this.view.PlayHide(() => {
                this.marchConfirmViewModel.Hide();
                this.NeedRefresh = true;
                if (needHideTileInfo) {
                    this.parent.HideTileInfo();
                    this.parent.OnRemoveViewAboveMap(this);
                } else {
                    this.afterHide.InvokeSafe();
                    this.afterHide = null;
                }
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.view.HideImmediatly();
                this.parent.OnRemoveViewAboveMap(this);
            });
            this.marchConfirmViewModel.HideImmediatly();
        }

        public void ShowPay() {
            this.parent.CloseAboveUI();
            this.parent.ShowPay();
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void ShowMarhcConfirm(BattleSimulationResult result) {
            this.marchConfirmViewModel.Show(result);
        }

        public void OnBtnTroopClick(string index) {
            this.CurrentTroop = index;
        }

        public void UpdateTroopStatus(Troop troop) {
            this.parent.UpdateTroopStatus(troop);
        }

        public void MarchReq() {
            switch (this.model.viewType) {
                case TroopViewType.Move:
                    this.MarchReq(MarchType.Move);
                    break;
                case TroopViewType.Attack:
                    if (this.parent.GetMonsterInfo(this.Target) != null) {
                        this.MarchReq(MarchType.MonsterAttack);
                    } else if (this.parent.GetBossInfo(this.Target) != null) {
                        this.MarchReq(MarchType.BossAttack);
                    } else {
                        this.MarchReq(MarchType.Attack);
                    }
                    break;
                case TroopViewType.Raid:
                    if (this.parent.GetMonsterInfo(this.Target) != null) {
                        this.MarchReq(MarchType.MonsterAttack);
                    } else if (this.parent.GetBossInfo(this.Target) != null) {
                        this.MarchReq(MarchType.BossAttack);
                    } else {
                        this.MarchReq(MarchType.Raid);
                    }
                    break;
                case TroopViewType.Return:
                    this.ReturnReq();
                    break;
                default:
                    break;
            }
        }

        public float GetSpeedBonus() {
            return this.buildModel.GetSpeedBonus();
        }

        public Monster GetMonsterInfo() {
            return this.mapModel.GetMonsterInfo(this.Target);
        }

        public bool IsChestsReachUpperLimit() {
            return this.DailyLimit.ChestCurrent >= this.DailyLimit.ChestLimit;
        }

        public bool IsResourcsReachUpperLimit() {
            Protocol.Resources resCur = this.DailyLimit.ResourceCurrent;
            Protocol.Resources resLimit = this.DailyLimit.ResourceLimit;
            return resCur.Lumber >= resLimit.Lumber &&
                resCur.Food >= resLimit.Food &&
                resCur.Marble >= resLimit.Marble &&
                resCur.Steel >= resLimit.Steel;
            //return this.DailyLimit.ResourceCurrent >= this.DailyLimit.ResourceLimit;
        }

        public bool IsMarchBelongsToPVE() {
            return string.IsNullOrEmpty(this.mapModel.currentTile.playerName);
        }

        public bool LowChanceGetLottery() {
            if (this.mapModel.currentTile.playerId.CustomIsEmpty()) {
                int fieldMaxLevel = RoleManager.GetFDRecordMaxLevel();
                return (fieldMaxLevel - this.mapModel.currentTile.level > GameConst.LOTTERY_LOW_LEVEL);
            }
            return false;
        }

        private void MarchReq(MarchType marchType) {
            MarchReq troop = new MarchReq() {
                Target = new Coord()
            };
            troop.Target.X = (int)this.Target.x;
            troop.Target.Y = (int)this.Target.y;
            troop.TroopId = this.CurrentTroop;
            troop.MarchType = (int)marchType;
            if (this.view.IsVisible) {
                this.view.DisableBtnSend();
            }
            NetManager.SendMessage(troop, typeof(MarchAck).Name, this.MarchAck, null,
                () => {
                    if (this.view.IsVisible) {
                        this.view.EnableBtnSend();
                    }
                });
        }

        private void MarchAck(IExtensible message) {
            AudioManager.Play("act_troop_send", AudioType.Action, AudioVolumn.Medium);
            this.Hide();
        }

        private void ReturnReq() {
            CallbackTroopReq callbackTroopReq = new CallbackTroopReq() {
                Id = this.CurrentTroop
            };
            NetManager.SendMessage(callbackTroopReq, typeof(CallbackTroopAck).Name, this.ReturnAck);
        }

        private void ReturnAck(IExtensible message) {
            this.Hide();
        }

        public void CompleteTroopMarchReq() {
            //CompleteTroopMarchReq callbackTroopReq = new Protocol.CompleteTroopMarchReq() {
            //    TroopId = this.CurrentTroop
            //};
            //NetManager.SendMessage(callbackTroopReq,
            //    typeof(CompleteTroopMarchReq).Name, this.CompleteTroopMarchAck);
        }

        private void CompleteTroopMarchAck(IExtensible message) {
            this.Hide();
        }

        private void NewTroopNtf(IExtensible message) {
            NewTroopNtf newTroopNtf = message as NewTroopNtf;
            this.TroopDict.Add(newTroopNtf.Troop.Id, newTroopNtf.Troop);
        }

        private void DailyLimitNtf(IExtensible message) {
            DailyLimitNtf ntf = message as DailyLimitNtf;
            this.DailyLimit = ntf.Limit;
        }

        public void ShowHero(HeroSubViewType viewType) {
            this.parent.ShowHero(viewType);
        }

        public void ShowHeroInfo(string heroName, UnityAction levelUpCallback) {
            this.parent.ShowHeroInfo(heroName, levelUpCallback);
        }

        public void OnAddViewAboveMap(IViewModel viewModel) {
            this.parent.OnAddViewAboveMap(viewModel);
        }

        public void ShowRecruit(string id) {
            this.parent.ShowRecruit(id);
        }

        public MarchAttributes GetMarchAttributes(string troopId) {
            return this.model.GetMarchAttributes(troopId);
        }

        public BattleSimulationResult BattleSimulation() {
            //if (this.CurrentMarchAttributes.army <= 0) {
            //    return BattleSimulationResult.NoTroop;
            //}
            long army = this.CurrentMarchAttributes.army;
            long maxArmy = this.CurrentMarchAttributes.maxArmy;
            Vector2 result = mapModel.GetTileArmy(this.Target);

            bool isMonster = this.mapModel.GetMonsterInfo(this.Target) != null;
            bool isBoss = this.mapModel.GetBossInfo(this.Target) != null;
            //待沟通
            Monster monster = this.mapModel.GetMonsterInfo(this.Target);
            Boss boss = this.mapModel.GetBossInfo(this.Target);
            
            if (!isMonster|| !isBoss) {
                if (army < result.x) {
                    if (maxArmy < result.x) {
                        return BattleSimulationResult.HardWeak;
                    }
                    return BattleSimulationResult.Hard;
                } else if (army < result.y && army > result.x) {
                    return BattleSimulationResult.Normal;
                } else {
                    return BattleSimulationResult.Easy;
                }
            } else {
                return BattleSimulationResult.None;
            }
           
        }

        public bool IsAlliacneTarget() {
            MapTileInfo tileInfo = this.mapModel.GetTileInfoSeparate(this.Target);
            if ((tileInfo.city != null) || tileInfo.GetPassType().Equals(GameConst.PASS_PASS)) {
                return true;
            }
            return false;
        }

        public bool IsTownhall() {
            Coord coord = new Coord {
                X = (int)this.Target.x,
                Y = (int)this.Target.y
            };
            if (this.buildModel.GetBuildingByCoord(coord) == null) {
                return false;
            }
            return this.buildModel.GetBuildingByCoord(coord).Name == "townhall";
        }

        #region FTE

        private void OnResourceStep4Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            //DramaConf dramaConf = DramaConf.GetConfByFull(index);
            // FTE To do: need check troop is avaliable.
            this.view.OnResourceStep4Start(!FteManager.FteOver);
            this.parent.ShowTroopInfo(FteManager.GetCurrentTroop(), TroopViewType.Attack);
            if (!FteManager.FteOver) {
                FteManager.SetCurrentTroop(string.Empty, Vector2.zero, string.Empty);
            }
        }

        private void OnResourceStep4End() {
            this.view.afterHideCallback = null;
            this.view.InvokeBtnSend();
            this.parent.StartChapterDailyGuid();
        }
        #endregion
    }
}
