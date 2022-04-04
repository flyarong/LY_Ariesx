using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class BattleSimulation3DViewModel : BaseViewModel {
        private BattleSimulation3DView view;
        private BattleSimulation3DModel model;
        private BattleSimulationViewModel parentModel;
        //private MapModel mapModel;
        //private AllianceCreateOrJoinModel allianceCreateOrJoinModel;

        private float playSpeed = 1;
        public float PlaySpeed {
            get {
                return playSpeed;
            }
            set {
                if (value < 8 && value > 0.12f) {
                    this.playSpeed = value;
                    if (!this.isPause) {
                        Time.timeScale = this.playSpeed;
                    }
                    this.view.OnSpeedChange();
                }
            }
        }

        private bool isPause = false;
        public bool IsPause {
            get {
                return this.isPause;
            }
            set {
                if (this.isPause != value) {
                    this.isPause = value;
                    this.view.OnPauseStatusChange();
                }
            }
        }

        public Battle.ReportRounds BattleRounds {
            get {
                return this.model.battleDetail.Rounds;
            }
        }

        public Battle.Report BattleInfo {
            get {
                return this.model.battleResult.Report;
            }
        }

        public BattleReport BattleResult {
            get {
                return this.model.battleResult;
            }
        }

        public Battle.PlayerInfo Attacker {
            get {
                return this.model.attacker;
            }
            private set {
                this.model.attacker = value;
            }
        }

        public Battle.PlayerInfo Defender {
            get {
                return this.model.defender;
            }
            private set {
                this.model.defender = value;
            }
        }

        public int TotalLost {
            get {
                return this.model.totalLost;
            }
            set {
                this.model.totalLost = value;
            }
        }

        public bool Win {
            get; set;
        }

        public bool PointLosed {
            get {
                return this.model.battleResult.Report.Result.FinalInfo.Occupy &&
                    (this.model.battleResult.Report.Defender.BasicInfo.Id == RoleManager.GetRoleId());
            }
        }

        public bool PointOccupied {
            get {
                return this.model.battleResult.Report.Result.FinalInfo.Occupy &&
                    (this.model.battleResult.Report.Defender.BasicInfo.Id != RoleManager.GetRoleId());
            }
        }

        void Awake() {
            this.model = ModelManager.GetModelData<BattleSimulation3DModel>();
            this.parentModel = GameObject.FindObjectOfType<BattleSimulationViewModel>();
            if (this.model.isFte) {
                byte[] detail = UnityEngine.Resources.Load<TextAsset>("Battle/GetBattleReportAck").bytes;
                byte[] troop = UnityEngine.Resources.Load<TextAsset>("Battle/GetBattleReportsAck").bytes;
                this.model.LoadBattle(detail, troop);
            }
            this.InitData();
            this.view = this.gameObject.AddComponent<BattleSimulation3DView>();
            //UpdateManager.Regist(UpdateInfo.BattleViewModel, this.UpdateAction);
        }

        void Start() {
            this.view.Play();
        }

        private void InitData() {
            if (this.BattleInfo.Defender.BasicInfo.Name == "RoleManager.GetRoleName()") {
                this.Attacker = this.BattleInfo.Defender;
                this.Defender = this.BattleInfo.Attacker;
                this.Win = this.BattleInfo.Winner.CustomEquals("defender");
            }
            else {
                this.Attacker = this.BattleInfo.Attacker;
                this.Defender = this.BattleInfo.Defender;
                this.Win = this.BattleInfo.Winner.CustomEquals("attacker");
            }
            this.TotalLost = 0;
            foreach (Battle.Deaths deaths in this.Attacker.Deaths) {
                this.TotalLost += deaths.Amount;
            }
        }
        public void Close() {
            this.parentModel.ShowSimulationUI(true);
            StopAllCoroutines();
            GameObject.Destroy(this.view);
            GameObject.Destroy(this);
        }

        private void OnPauseStatusChange() {
            if (!this.IsPause) {
                this.view.Play();
            }
        }

        protected override void OnReLogin() {
            this.Close();
        }

        void OnEnable() {
            this.parentModel.ShowSimulationUI(false);
            RoleManager.AddLoginAction(this.OnReLogin);
            //GameManager.DisableMainCamera();
            AudioManager.PlayBg(AudioScene.Battle);
            AudioManager.Stop(AudioType.Enviroment);
        }

        void OnDisable() {
            RoleManager.RemoveLoginAction(this.OnReLogin);
            Time.timeScale = 1;
            this.model.isFte = false;
            //GameManager.EnableMainCamera();
            PoolManager.ClearPool(PoolType.Battle);
            AudioManager.Stop(AudioType.Show);
            AudioManager.StopBg();
        }
    }
}