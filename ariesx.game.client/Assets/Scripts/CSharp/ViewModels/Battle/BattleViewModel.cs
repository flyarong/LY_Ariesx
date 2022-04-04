using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class BattleViewModel : BaseViewModel {
        private BattleView view;
        private BattleModel model;
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
            //ConfigureManager.Instance.LoadConfigure<AudioHeroConf>("hero_audio");
            //ConfigureManager.Instance.LoadConfigure<AudioSKillConf>("skill_audio");
            this.model = ModelManager.GetModelData<BattleModel>();
            if (this.model.isFte) {
                byte[] detail = UnityEngine.Resources.Load<TextAsset>("Battle/GetBattleReportAck").bytes;
                byte[] troop = UnityEngine.Resources.Load<TextAsset>("Battle/GetBattleReportsAck").bytes;
                this.model.LoadBattle(detail, troop);
            }
            this.InitData();
            this.view = this.gameObject.AddComponent<BattleView>();
            FteManager.SetEndCallback(GameConst.NORMAL, 59, this.OnFteStep241End);
            FteManager.SetEndCallback(GameConst.NORMAL, 481, this.OnFteStep481End);
            FteManager.SetEndCallback(GameConst.NORMAL, 731, this.OnFteStep731End);
            //UpdateManager.Regist(UpdateInfo.BattleViewModel, this.UpdateAction);
        }

        void Start() {
            this.view.Play();
        }

        private void InitData() {
            if (this.BattleInfo.Defender.BasicInfo.Name == RoleManager.GetRoleName()) {
                this.Attacker = this.BattleInfo.Defender;
                this.Defender = this.BattleInfo.Attacker;
                this.Win = this.BattleInfo.Winner.CustomEquals("defender");
            } else {
                this.Attacker = this.BattleInfo.Attacker;
                this.Defender = this.BattleInfo.Defender;
                this.Win = this.BattleInfo.Winner.CustomEquals("attacker");
            }
            this.TotalLost = 0;
            foreach (Battle.Deaths deaths in this.Attacker.Deaths) {
                this.TotalLost += deaths.Amount;
            }
        }

        //private void InitPoolManager() {
        //    GameObject poolManager = new GameObject();
        //    poolManager.name = "PoolManager";
        //    poolManager.transform.position = UnityEngine.Vector3.zero;
        //    poolManager.transform.SetParent(this.transform);
        //    poolManager.AddComponent<PoolManager>();
        //    Debug.Log("InitPool Complete");
        //}

        //private void InitAnimationManager() {
        //    GameObject animationManager = new GameObject();
        //    animationManager.name = "AnimationManager";
        //    animationManager.transform.position = UnityEngine.Vector3.zero;
        //    animationManager.transform.SetParent(this.transform);
        //    animationManager.AddComponent<AnimationManager>();
        //    Debug.Log("InitAnimation Complete");
        //}

        //private void InitNetManager() {
        //    GameObject netManager = new GameObject();
        //    netManager.name = "NetManager";
        //    netManager.transform.position = UnityEngine.Vector3.zero;
        //    netManager.transform.SetParent(this.transform);
        //    netManager.AddComponent<NetManager>();
        //    Debug.Log("InitNet Complete");
        //}

        //private void UpdateAction() {
        //    if (this.IsPause) {
        //        return;
        //    }
        //}

        public void Close() {
            TriggerManager.Invoke(Trigger.PlayBattleReportDone);
            ModelManager.UnLoadScene("Scene3DBattle");
        }

        private void OnPauseStatusChange() {
            if (!this.IsPause) {
                this.view.Play();
            }
        }

        protected override void OnReLogin() {
            this.Close();
        }

        #region FTE

        private void OnFteStep241End() {
            this.view.OnFteStep241End();
        }

        private void OnFteStep481End() {
            this.view.OnFteStep481End();
        }

        private void OnFteStep731End() {
            this.view.OnFteStep731End();
        }

        #endregion

        void OnEnable() {
            RoleManager.AddLoginAction(this.OnReLogin);
            GameManager.DisableMainCamera();
            AudioManager.PlayBg(AudioScene.Battle);
            AudioManager.Stop(AudioType.Enviroment);
        }

        void OnDisable() {
            RoleManager.RemoveLoginAction(this.OnReLogin);
            Time.timeScale = 1;
            this.model.isFte = false;
            GameManager.EnableMainCamera();
            PoolManager.ClearPool(PoolType.Battle);
            AudioManager.Stop(AudioType.Show);
            AudioManager.StopBg();

            if (FteManager.CheckFte(GameConst.NORMAL, 721)) {
                if (this.Win) {
                    FteManager.StartFte("chapter_task_13");
                } else {
                    FteManager.EndFte(true);
                }
            }
        }
    }
}