using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;

namespace Poukoute {
    public class BattleSimulatorPosView : BattlePositionView {
        private static Rect uiRect = new Rect(375, 667, 750, 1334);

        //private bool isDead = false;
        private string currentSkill;
        BattleSimulatorPosView targetView;
        private Vector2 lastPosition;

        private static readonly Rect rect = new Rect(375, -667, 750, 1334);

        public int Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (this.position != value)
                {
                    this.position = value;
                    this.OnPositionChange();
                }
            }
        }

        new void Awake() {
            base.Awake();
            this.positionRoot = this.transform.Find("Position");

        }

        public void ResetPosition(AnimationParam param) {
            Vector3 position =
                this.transform.parent.right * positionList[this.position - 1].x +
                this.transform.parent.up * positionList[this.position - 1].y +
                this.transform.parent.forward * positionList[this.position - 1].z +
                this.transform.parent.position;
            UnityAction reset = () => {
                this.SetUIPosition();
                this.role.localEulerAngles = Vector3.zero;
            };
            Vector3 direction = position - this.transform.position;
            if (direction != Vector3.zero && param != null) {
                this.role.forward = direction;
                this.Move(position, param, reset);
            } else {
                reset();
            }
        }

        public Transform SetPosition(int position, string camp,
            string heroId, BattleSimulatorPosView target) {
            this.battleCamera = GameObject.FindWithTag("BattleCamera").GetComponent<Camera>();
            this.targetView = target;
            this.Camp = camp;
            this.audioHeroConf = AudioHeroConf.GetConf(heroId);
            if (this.pnlHealth == null) {
                this.pnlHealth = PoolManager.GetObject(
                     camp == BattleConst.attack ?
                         PrefabPath.battleAttackerHealth : PrefabPath.battleDefenderHealth,
                     this.pnlHealthRoot,
                     poolType: PoolType.Battle
                 ).transform;
                BattleHealthSimulator healthView = this.pnlHealth.gameObject.AddComponent<BattleHealthSimulator>();
                healthView.root = this.healthRoot;
                this.rectHealth = this.pnlHealth.GetComponent<RectTransform>();
                this.healthView = this.pnlHealth.GetComponent<BattleHealthView>();
                this.healthView.SetHealth(200);
            }
            this.Position = position;
            return this.pnlHealth;
        }

        private new void SetUIPosition() {
            return;
            if (pnlHealth != null) {
                this.rectHealth.anchoredPosition =
                    this.ScreenToUIPoint(
                    this.battleCamera.WorldToScreenPoint(this.healthRoot.position));
            }
        }

        public new void Move(Vector3 target, AnimationParam param, UnityAction callback) {
            this.SetStatus(GameConst.animIsMove, true);
            this.Animate(GameConst.animMove);
            AnimationManager.Animate(this.gameObject, param, start: this.transform.position,
                target: target, finishCallback: () => {
                    callback.InvokeSafe();
                    this.SetStatus(GameConst.animIsMove, false);
                }, frameCallback: this.SetUIPosition, space: PositionSpace.World
            );
        }

        public Vector2 ScreenToUIPoint(Vector2 point) {
            Vector2 offsetScreen = point - new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 offsetUi = offsetScreen * (uiRect.height / Screen.height);
            return offsetUi + new Vector2(uiRect.width / 2, -uiRect.height / 2);
        }

        private Vector2 WorldToUI(Vector2 point) {
            Vector2 offsetUi = point * (rect.height / 13.3f);
            return offsetUi + new Vector2(rect.width / 2, -rect.height / 2);
        }

        private new void OnPositionChange() {
            if (position > 0 && position < 7) {
                this.transform.localPosition = positionList[position - 1];
                if (this.camp == BattleConst.defense) {
                    this.transform.localPosition = new Vector3(
                        -this.transform.localPosition.x,
                        this.transform.localPosition.y,
                        this.transform.localPosition.z
                    );
                }
                this.SetUIPosition();
            }
        }

        public void Attack() {
            string attack = string.Format(
                "{0}_attack_1_1",
                this.heroId
            );
            BattleEffectView attackView = this.effectDict[attack];
            attackView.Begin(
                this, " ", this.heroId,
                new List<BattlePositionView>() { this.targetView },
                HeroBattleConf.IsCampaignHero(this.heroId),
                this.ResetPosition, null, null
            );
        }

        public void ReleaseSkill(int index) {
            foreach (var pair in this.effectDict) {
                if (pair.Key.Contains("skill_" + index)) {
                    BattleEffectView skillView = pair.Value;
                    skillView.Begin(
                        this, pair.Key, this.heroId,
                        new List<BattlePositionView>() { this.targetView },
                        HeroBattleConf.IsCampaignHero(this.heroId),
                        this.ResetPosition, null, null
                    );
                }
            }
        }

        protected override void OnActionPlay(RoleAction action) {
            switch (action) {
                case RoleAction.Victor:
                    this.OnVictorPlay();
                    break;
                case RoleAction.Failed:
                    this.OnFailedPlay();
                    break;
                default:
                    Debug.LogErrorf("No such action {0}!", action.ToString());
                    break;
            }
        }

        //private void OnAudioPlay(RoleAction action) {
        //    switch (action) {
        //        case RoleAction.Attack:
        //            AudioManager.Play(
        //                this.audioHeroConf.attack,
        //                AudioType.Action,
        //                AudioVolumn.High,
        //                isAdditive: true
        //            );
        //            break;
        //        case RoleAction.Skill:
        //            AudioManager.Play(
        //                this.audioSkillConf.cast,
        //                AudioType.Action,
        //                AudioVolumn.High,
        //                isAdditive: true
        //            );
        //            break;
        //        default:
        //            Debug.LogWarningf("No such role action {0}", action);
        //            break;
        //    }
        //}

        protected override void OnActionEnd(RoleAction action) {
            //this.ResetPosition(null);
        }

        public override IEnumerator GetHitDelay(float second, UnityAction callback, Battle.Action action) {
            if (second == 0) {
                yield return null;
            } else {
                yield return new WaitForSeconds(1);
            }
            this.GetHit(callback, action);
        }

        public override void GetHit(UnityAction callback, Battle.Action action, bool showDamage = false) {
            this.animator.SetTrigger(GameConst.animHit);
        }

        public override void SetSkillLast(GameObject obj,
            BattleEffectTargetType target, string effectName) {
            Transform lastRoot;
            switch (target) {
                case BattleEffectTargetType.Head:
                    lastRoot = this.skillLastHeadRoot;
                    break;
                case BattleEffectTargetType.Body:
                    lastRoot = this.skillLastBodyRoot;
                    break;
                case BattleEffectTargetType.Foot:
                    lastRoot = this.skillLastFootRoot;
                    break;
                default:
                    lastRoot = null;
                    break;
            }
            GameHelper.ClearChildren(lastRoot);
            obj.transform.SetParent(lastRoot);
            obj.transform.localPosition = Vector3.zero;
        }

        private IEnumerator Delay(float seconds, UnityAction callback) {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }

        void OnDestroy() {
            if (this.pnlHealth != null) {
                PoolManager.RemoveObject(this.pnlHealth.gameObject);
            }
            this.pnlHealth = null;
        }
    }
}
