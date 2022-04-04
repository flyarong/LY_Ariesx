using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public enum Direction {
        Right,
        Up,
        UpLeft,
        Left,
        Down,
        DownRight
    }

    public class BattlePosition {
        public Vector3 position = Vector3.zero;

    }

    public class BattlePositionView : MonoBehaviour, IPoolHandler {
        protected Animator animator;
        protected Camera battleCamera;
        protected BattleAction battleAction;
        public Dictionary<string, Battle.Action> hitActionDict =
            new Dictionary<string, Battle.Action>();
        protected List<BattlePositionView> targetList = new List<BattlePositionView>();

        protected Transform role;
        protected Transform pnlHealthRoot;
        protected Transform pnlHealth;
        protected RectTransform rectHealth;
        protected BattleHealthView healthView;
        protected Text txtHealth;
        protected Transform pnlDamageRoot;
        protected Transform positionRoot;
        protected Transform enemyRoot;
        protected Transform flyRoot;
        protected Transform hitRoot;
        protected Transform UISkillRoot;
        protected Transform skillCastRoot;
        protected Transform skillHitHeadRoot;
        protected Transform skillHitBodyRoot;
        protected Transform skillHitFootRoot;
        protected Transform skillLastHeadRoot;
        protected Transform skillLastBodyRoot;
        protected Transform skillLastFootRoot;
        protected Transform friendRoot;
        protected Transform healthRoot;
        protected Transform victorRoot;
        protected Transform failedRoot;
        protected Transform moveRoot;
        protected Transform idleRoot;
        protected int position = -1;
        // protected string attackType = string.Empty;
        protected string camp = string.Empty;
        protected Dictionary<string, Dictionary<int, BattlePositionView>> positionDict;
        protected Dictionary<string, BattlePositionView> heroDict;
        protected Dictionary<BattleEffectTargetType, BattleBuff> buffTargetDict =
            new Dictionary<BattleEffectTargetType, BattleBuff>() {
                {BattleEffectTargetType.Head, null },
                {BattleEffectTargetType.Body, null },
                {BattleEffectTargetType.Foot, null }
            };
        protected Dictionary<string, BattleBuff> buffDict =
            new Dictionary<string, BattleBuff>();
        protected List<BattleBuff> buffRemoveList = new List<BattleBuff>();
        protected Battle.Action currentAction;
        protected AudioHeroConf audioHeroConf;
        // protected AudioSKillConf audioSkillConf;
        protected List<BattlePosition> battlePositionStack = new List<BattlePosition>();

        private UnityAction onStepEnd;
        public string heroId = string.Empty;
        //   public UnityAction onCastPlay;
        //    public UnityAction onAttackPlay;
        public Dictionary<string, BattleEffectView> effectDict =
            new Dictionary<string, BattleEffectView>();

        public static readonly List<Vector3> positionList = new List<Vector3> {
            new Vector3(0, 0, 2.5f),
            new Vector3(0, 0, -0.5f),
            new Vector3(-2.25f, 0, 2.5f),
            new Vector3(-2.25f,0, -0.5f),
            new Vector3(2.25f, 0, 2.5f),
            new Vector3(2.25f, 0, -0.5f),
        };

        public static readonly List<string> buffList = new List<string> {
            "hot",
            "damup",
            "defup",
            "healup",
            "speedup"
        };

        public static readonly List<string> notShowNumberBuffList = new List<string> {
            "slienceimmu"
            //immu"
        };

        public ParticleSystem idleParticle;
        public ParticleSystem failedParticle;
        public ParticleSystem victorParticle;

        public int Position {
            get {
                return this.position;
            }
            set {
                if (this.position != value) {
                    this.position = value;
                    this.OnPositionChange();
                }
            }
        }

        public string Camp {
            get {
                return this.camp;
            }
            set {
                if (this.camp != value) {
                    this.camp = value;
                }
            }
        }

        protected void Awake() {
            GameObject ui = GameObject.Find("UIBattle");

            this.role = this.transform.Find("Role");
            if (!this.name.Contains("empty")) {
                this.heroId = this.name.Substring(this.name.IndexOf('_') + 1, 8);
            } else {
                this.heroId = string.Empty;
            }
            if (ui != null) {
                this.pnlHealthRoot = ui.transform.Find("PnlHealth").transform;
                this.pnlDamageRoot = ui.transform.Find("PnlDamage").transform;
            }
            this.positionRoot = this.transform.Find("Position");
            if (role.childCount != 0) {
                this.animator = this.role.GetComponentInChildren<Animator>();
                //(Animatorcon)(this.animator.runtimeAnimatorController)
                this.battleAction = this.role.GetComponentInChildren<BattleAction>();
                this.battleAction.OnActionEnd = this.OnActionEnd;
                this.battleAction.OnActionPlay = this.OnActionPlay;
                this.battleAction.OnAudioPlay = this.OnAudioPlay;
            }
            Transform skill = this.transform.Find("Skill");
            foreach (Transform child in skill) {
                this.effectDict.Add(child.name, child.GetComponent<BattleEffectView>());
            }

            this.SetEffectRoot();
        }

        void Start() {
            this.Idle();
        }

        public void MoveBack(AnimationParam param) {
            int length = this.battlePositionStack.Count;
            if (length == 0) {
                return;
            }
            BattlePosition lastPosition = this.battlePositionStack[length - 1];
            this.battlePositionStack.Remove(lastPosition);
            UnityAction reset = () => {
                this.SetUIPosition();
                if (this.role != null) {
                    this.role.localEulerAngles = Vector3.zero;
                }
            };
            Vector3 direction = lastPosition.position - this.transform.position;
            if (direction != Vector3.zero && param != null) {
                this.role.forward = direction;
                this.Move(lastPosition.position, param, reset);
            } else {
                reset();
            }
        }

        public void ResetPosition(UnityAction callback) {
            this.SetUIPosition();
            if (this.role != null) {
                this.role.localEulerAngles = Vector3.zero;
            }
            callback.InvokeSafe();
        }

        public void Animate(int trigger) {
            if (this.animator != null) {
                this.animator.SetTrigger(trigger);

            }
        }

        public void SetStatus(int status, bool enable) {
            if (this.animator != null) {
                this.animator.SetBool(status, enable);
            }
        }

        public Transform SetPosition(int position, string camp,
            string heroId, int armyAmount, Transform UISkillRoot,
             Dictionary<string, BattlePositionView> heroDict,
             Dictionary<string, Dictionary<int, BattlePositionView>> positionDict,
             Camera battleCamera
        ) {
            this.battleCamera = battleCamera;
            this.UISkillRoot = UISkillRoot;
            this.heroDict = heroDict;
            this.positionDict = positionDict;
            if (!string.IsNullOrEmpty(heroId)) {
                this.audioHeroConf = AudioHeroConf.GetConf(heroId);
            }
            if (armyAmount != 0) {
                this.pnlHealth = PoolManager.GetObject(
                    camp == BattleConst.attack ?
                        PrefabPath.battleAttackerHealth : PrefabPath.battleDefenderHealth,
                    this.pnlHealthRoot,
                    poolType: PoolType.Battle
                ).transform;
                this.pnlHealth.gameObject.SetActiveSafe(true);
                this.rectHealth = this.pnlHealth.GetComponent<RectTransform>();
                this.healthView = this.pnlHealth.GetComponent<BattleHealthView>();
                this.healthView.SetHealth(armyAmount);
            }
            this.Camp = camp;
            this.Position = position;
            return this.pnlHealth;
        }

        public void SetUIPosition() {
            if (pnlHealth != null) {
                this.rectHealth.anchoredPosition =
                    MapUtils.ScreenToUIPoint(
                    this.battleCamera.WorldToScreenPoint(this.healthRoot.position));
            }
        }

        private Vector2 WorldToUI(Vector2 point) {
            Vector2 offsetUi = point * (MapUtils.UIRect.height / 13.3f);
            return offsetUi + new Vector2(MapUtils.UIRect.width / 2, -MapUtils.UIRect.height / 2);
        }

        protected void OnPositionChange() {
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

        protected void SetEffectRoot() {
            Transform root = null;
            root = this.positionRoot;
            this.enemyRoot = root.Find("Enemy");
            this.flyRoot = root.Find("Fly");
            this.hitRoot = root.Find("Hit");
            this.skillCastRoot = root.Find("Cast");
            this.victorRoot = root.Find("Win");
            this.failedRoot = root.Find("Lose");
            this.moveRoot = root.Find("Move");
            Transform skillHitRoot = root.Find("SkillHit");
            this.skillHitHeadRoot = skillHitRoot.Find("Head");
            this.skillHitBodyRoot = skillHitRoot.Find("Body");
            this.skillHitFootRoot = skillHitRoot.Find("Foot");
            Transform skillLastRoot = root.Find("SkillLast");
            this.skillLastHeadRoot = skillLastRoot.Find("Head");
            this.skillLastBodyRoot = skillLastRoot.Find("Body");
            this.skillLastFootRoot = skillLastRoot.Find("Foot");
            this.friendRoot = root.Find("Friend");
            this.healthRoot = root.Find("Health");
            this.idleRoot = root.Find("Idle");
        }

        // To do: Need buff remove rules.
        public void OnRoundEnd() {
            this.buffRemoveList.Clear();
            foreach (BattleBuff buff in this.buffDict.Values) {
                buff.round--;
                if (buff.round == 0) {
                    this.buffRemoveList.Add(buff);
                }
            }
            foreach (BattleBuff buff in this.buffRemoveList) {
                PoolManager.RemoveObject(buff.obj, PoolType.Battle);
                this.buffDict.Remove(buff.name);
                this.buffTargetDict[buff.target] = null;
            }
        }

        public void ActMultiple(Battle.Action action, int actionIndex,
            UnityAction<int> callback = null) {
            int count = action.Actions.Count;
            UnityAction childCallBack = null;
            if (this == null) {
                return;
            }
            this.battlePositionStack.Add(
                new BattlePosition {
                    position = this.transform.position
                }
            );
            if (count > 0) {
                Battle.Action childAction = action.Actions[0];
                BattlePositionView positionView = this.heroDict[childAction.HeroId];
                childCallBack = () => {
                    this.ActNext(0, count, action, () => {
                        callback.Invoke(++actionIndex);
                    });
                };
            }
            this.ActSingle(action, childCallBack);
            if (count == 0) {
                this.ResetPosition(() => callback.Invoke(++actionIndex));
            }
        }

        public void ActNext(int index, int count, Battle.Action action, UnityAction callback) {
            if (index < count) {
                Battle.Action nextAction = action.Actions[index];
                BattlePositionView nextView = this.heroDict[nextAction.HeroId];
                nextView.ActMultiple(nextAction, index, (nextIndex) => {
                    this.ActNext(nextIndex, count, action, callback);
                });
            } else {
                this.ResetPosition(() => callback.InvokeSafe());
            }
        }

        public void ActSingle(Battle.Action action, UnityAction callback = null) {
            this.SetStepEnd(callback);
            switch (action.Type) {
                case SkillConst.TypeAttack:
                    this.Attack(action);
                    break;
                case SkillConst.TypeCastSkill:
                    this.Skill(action);
                    break;
                case SkillConst.TypeBuffActing:
                    this.BuffEffect(action);
                    break;
                case SkillConst.TypeBuffLose:
                    this.RemoveBuff(action.Via);
                    if (action.Name.CustomEquals("shield")) {
                        this.healthView.RemoveShield(-9999999, callback);
                    }
                    break;
                case SkillConst.TypeDead:
                    this.Failed();
                    if (this.pnlHealth != null) {
                        PoolManager.RemoveObject(this.pnlHealth.gameObject, PoolType.Battle);
                        this.pnlHealth = null;
                    }
                    foreach (BattleBuff buff in this.buffDict.Values) {
                        PoolManager.RemoveObject(buff.obj, PoolType.Battle);
                    }
                    this.InvokeStepEnd();
                    break;
                default:
                    this.InvokeStepEnd();
                    break;
            }
        }

        public void ActPassive(Battle.Action action, UnityAction callback = null) {
            switch (action.Type) {
                case SkillConst.TypeInjured:
                    this.GetHit(callback, action);
                    break;
                case SkillConst.TypeHeal:
                    this.PlayDamageNumber(action.Amount, callback, false, true, true);
                    break;
                case SkillConst.TypeShieldInjured:
                    this.PlayDamageNumber(action.Amount, callback, true, false, true);
                    break;
                default:
                    callback.InvokeSafe();
                    break;
            }
        }

        public void Attack(Battle.Action action) {
            this.currentAction = action;
            this.targetList.Clear();
            bool canAttack = false;
            foreach (Battle.Action childAction in action.Actions) {
                if (!(childAction.Type == SkillConst.TypeInjured ||
                    childAction.Type == SkillConst.TypeShieldInjured ||
                    childAction.Type == SkillConst.TypeImmune)) {
                    continue;
                }
                canAttack = true;
                BattlePositionView target = this.heroDict[childAction.HeroId];
                if (!this.targetList.Contains(target)) {
                    target.hitActionDict.Clear();
                    target.hitActionDict.Add(action.Name, childAction);
                    this.targetList.Add(target);
                } else if (childAction.Type == SkillConst.TypeInjured) {
                    target.hitActionDict.Add(SkillConst.TypeInjured, childAction);
                }
            }

            if (canAttack) {
                string path = this.heroId;
                GameObject attackObj =
                    this.transform.Find("Skill").Find(path + "_attack_1_1").gameObject;
                if (attackObj == null) {
                    Debug.LogErrorf("There is no attack of hero {0}", this.heroId);
                    return;
                }
                BattleEffectView attackView = attackObj.GetComponent<BattleEffectView>();
                attackView.Begin(this, action.Name, this.heroId, this.targetList,
                    HeroBattleConf.IsCampaignHero(this.heroId), this.MoveBack,
                    this.InvokeStepEnd, this.SetUIPosition
                );
            } else {
                this.InvokeStepEnd();
            }
        }

        public void Skill(Battle.Action action) {
            this.currentAction = action;
            this.animator.SetTrigger(GameConst.animSkill);
            int reference = 0;
            foreach (Battle.Action effectAction in this.currentAction.Actions) {
                foreach (Battle.Action hitAction in effectAction.Actions) {
                    this.heroDict[hitAction.HeroId].hitActionDict.Clear();
                }
            }
            int count = 0;
            foreach (Battle.Action effectAction in this.currentAction.Actions) {
                count++;
                string effectName = effectAction.Via;
                if (!this.effectDict.ContainsKey(effectName)) {
                    if (count == this.currentAction.Actions.Count && reference == 0) {
                        this.InvokeStepEnd();
                        return;
                    } else {
                        continue;
                    }
                }
                reference++;
                BattleEffectView skillView = this.effectDict[effectName];

                List<BattlePositionView> targetList = new List<BattlePositionView>();
                string targetCamp = BattleConst.attack;
                if (effectAction.ToSelf && camp == BattleConst.defense ||
                        !effectAction.ToSelf && camp == BattleConst.attack) {
                    targetCamp = BattleConst.defense;
                }

                foreach (Battle.Action hitAction in effectAction.Actions) {
                    if (hitAction.Type == SkillConst.TypeDead) {
                        continue;
                    }
                    this.heroDict[hitAction.HeroId].hitActionDict[effectName] = hitAction;
                }
                foreach (int position in effectAction.Positions) {
                    BattlePositionView positionView = this.positionDict[targetCamp][position];
                    targetList.Add(positionView);
                }
                skillView.Begin(
                    this, effectName, this.heroId, targetList, HeroBattleConf.IsCampaignHero(heroId),
                    this.MoveBack,
                    () => {
                        if (--reference == 0 || this.effectDict.Count == 2) {
                            this.InvokeStepEnd();
                        }
                    },
                    this.SetUIPosition
                );
            }
        }

        public void BuffEffect(Battle.Action action) {
            this.GetHit(this.InvokeStepEnd, action.Actions[0]);
        }

        public void Move(Vector3 target, AnimationParam param, UnityAction callback) {
            this.SetStatus(GameConst.animIsMove, true);
            this.Animate(GameConst.animMove);
            AnimationManager.Animate(this.gameObject, param, start: this.transform.position,
                target: target, finishCallback: () => {
                    callback.InvokeSafe();
                    this.SetStatus(GameConst.animIsMove, false);
                }, frameCallback: this.SetUIPosition, space: PositionSpace.World
            );
        }

        protected virtual void OnActionEnd(RoleAction action) {
            switch (action) {
                case RoleAction.Failed:
                    this.OnFailedEnd();
                    break;
                default:
                    //Debug.LogErrorf("No such action {0}!", action.ToString());
                    break;
            }
        }

        protected virtual void OnActionPlay(RoleAction action) {
            switch (action) {
                case RoleAction.Victor:
                    this.OnVictorPlay();
                    break;
                case RoleAction.Failed:
                    this.OnFailedPlay();
                    break;
                default:
                    //Debug.LogErrorf("No such action {0}!", action.ToString());
                    break;
            }
        }

        public void Idle() {
            if (this.heroId.CustomIsEmpty()) {
                return;
            }
            string path = string.Format(
               "Battle/Skills/{0}/{0}_idle_1_1_last",
               this.heroId

           );
            this.idleRoot.GetComponent<BattleEffectPositionView>().Create(path);
        }

        public void Failed() {
            if (!this.heroId.CustomIsEmpty()) {
                this.Animate(GameConst.animFailed);
                BattleEffectPositionView idleEffectPosView = this.idleRoot.GetComponent<BattleEffectPositionView>();
                idleEffectPosView.Stop(idleEffectPosView.failedStopDelay);
            }
        }

        public void Victor() {
            if (!this.heroId.CustomIsEmpty()) {
                this.Animate(GameConst.animVictor);
                BattleEffectPositionView idleEffectPosView = this.idleRoot.GetComponent<BattleEffectPositionView>();
                idleEffectPosView.Stop(idleEffectPosView.victorStopDelay);
            }
        }

        protected void OnFailedPlay() {
            string path = string.Format(
                "Battle/Skills/{0}/{0}_failed_1_1_cast",
                this.heroId
            );
            this.failedRoot.GetComponent<BattleEffectPositionView>().Create(path);
        }

        protected void OnFailedEnd() {
            Destroy(this.role.gameObject);
            GameObject flag =
                PoolManager.GetObject("Battle/whiteflag", this.transform, poolType: PoolType.Battle);
            flag.transform.localScale = Vector3.one * 0.7f;
        }

        protected void OnVictorPlay() {
            string path = string.Format(
                "Battle/Skills/{0}/{0}_victory_1_1_cast",
               this.heroId
            );
            this.victorRoot.GetComponent<BattleEffectPositionView>().Create(path);
        }

        private void OnSkillCastPlay() {
            switch (this.currentAction.Type) {
                case SkillConst.TypeCastSkill:
                    //this.CastSkill();
                    break;
                default:
                    Debug.LogErrorf("No such action type {0}", this.currentAction.Type);
                    break;
            }
        }

        private void OnAudioPlay(RoleAction action) {
            return;
            //switch (action) {
            //    case RoleAction.Attack:
            //        AudioManager.Play(
            //            this.audioHeroConf.attack,
            //            AudioType.Action,
            //            AudioVolumn.High,
            //            isAdditive: true
            //        );
            //        break;
            //    case RoleAction.Skill:
            //        AudioManager.Play(
            //            this.audioSkillConf.cast,
            //            AudioType.Action,
            //            AudioVolumn.High,
            //            isAdditive: true
            //        );
            //        break;
            //    default:
            //        Debug.LogWarningf("No such role action {0}", action);
            //        break;
            //}
        }

        public void SetForward(Vector3 forward) {
            this.role.forward = forward;
        }

        public void ResetForward() {
            // this.role.localEulerAngles = Vector3.zero;
        }

        public Vector3 GetEnemyPosition(Vector3 origin) {
            Vector3 distance = this.transform.position - origin;
            Vector3 normal = distance.normalized;
            return this.transform.position - normal * (this.enemyRoot.localPosition).magnitude;
            //return this.enemyRoot.position;
        }

        public Vector3 GetFriendPosition() {
            return this.friendRoot.position;
        }

        public BattleEffectPositionView GetSkillHitRoot(BattleEffectTargetType target) {
            Transform root = null;
            switch (target) {
                case BattleEffectTargetType.Head:
                    root = this.skillHitHeadRoot;
                    break;
                case BattleEffectTargetType.Body:
                    root = this.skillHitBodyRoot;
                    break;
                case BattleEffectTargetType.Foot:
                    root = this.skillHitFootRoot;
                    break;
                default:
                    break;
            }
            if (root != null) {
                BattleEffectPositionView positionView =
                    root.GetComponent<BattleEffectPositionView>();
                return positionView;
            } else {
                return null;
            }
        }

        public Transform GetFlyRoot() {
            BattleEffectPositionView positionView =
                this.flyRoot.GetComponent<BattleEffectPositionView>();
            return positionView.Root;
        }

        public BattleEffectPositionView GetFlyPosView() {
            return this.flyRoot.GetComponent<BattleEffectPositionView>();
        }

        public Transform GetSkillCastRoot() {
            BattleEffectPositionView positionView =
                this.skillCastRoot.GetComponent<BattleEffectPositionView>();
            return positionView.Root;
        }

        public BattleEffectPositionView GetSkillCastPosView() {
            return this.skillCastRoot.GetComponent<BattleEffectPositionView>();
        }

        public Transform GetMoveRoot() {
            return moveRoot;
        }

        public BattleEffectPositionView GetMovePosView() {
            return this.moveRoot.GetComponent<BattleEffectPositionView>();
        }

        public virtual void SetSkillLast(GameObject obj,
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
            if (lastRoot != null) {
                lastRoot = lastRoot.GetComponent<BattleEffectPositionView>().Root;
            }
            BattleBuff battleBuff = new BattleBuff {
                name = effectName,
                obj = obj,
                round = this.hitActionDict[effectName].Rounds
            };

            this.buffTargetDict[target] = battleBuff;
            this.buffDict[effectName] = battleBuff;
            GameHelper.ClearChildren(lastRoot);
            obj.transform.SetParent(lastRoot);
            obj.transform.localPosition = Vector3.zero;
        }

        public virtual IEnumerator GetHitDelay(float second,
            UnityAction callback, Battle.Action action) {
            yield return YieldManager.GetWaitForSeconds(second);
            this.GetHit(callback, action);
        }

        public virtual void GetHit(UnityAction callback,
            Battle.Action action, bool showDamage = true) {
            if (action.Type == SkillConst.TypeInjured ||
                action.Type == SkillConst.TypeShieldInjured) {
                this.animator.SetTrigger(GameConst.animHit);
            }
            if (!showDamage) {
                return;
            }
            int damage = 0;
            bool isShield = false;
            switch (action.Type) {
                case SkillConst.TypeShieldInjured:
                    damage = -action.Amount;
                    Battle.Action injuredAciton;
                    if (this.hitActionDict.TryGetValue(SkillConst.TypeInjured, out injuredAciton)) {
                        this.PlayDamageNumber(
                            injuredAciton.Amount,
                            null,
                            false,
                            false,
                            true
                        );
                    }
                    isShield = true;
                    break;
                case SkillConst.TypeGetBuff:
                    damage = action.Amount;
                    if (action.Name.CustomEquals("shield")) {
                        this.healthView.AddShield(action.Amount);
                    }
                    break;
                case SkillConst.TypeBuffLose:
                    this.RemoveBuff(action.Via);
                    if (action.Name.CustomEquals("shield")) {
                        this.healthView.RemoveShield(-9999999, null);
                    }
                    isShield = true;
                    break;
                case SkillConst.TypeImmune:
                    isShield = true;
                    damage = 0;
                    break;
                case SkillConst.TypeDispel:
                    this.RemoveBuff(action.Via);
                    if (action.Name.CustomEquals("shield")) {
                        this.healthView.RemoveShield(-9999999, callback);
                    } else {
                        callback.InvokeSafe();
                    }
                    break;
                case SkillConst.TypeInjured:
                    damage = action.Amount;
                    break;
                case SkillConst.TypeHeal:
                    damage = action.Amount;
                    break;
                case SkillConst.TypeDead:
                    damage = action.Amount;
                    Debug.LogError("技能致死 " + action.Amount + ", " + action.Name);
                    break;
                default:
                    callback.InvokeSafe();
                    return;
            }
            this.PlayDamageNumber(
                damage, callback, isShield,
                action.Type == SkillConst.TypeHeal ||
                (action.Type == SkillConst.TypeGetBuff && buffList.Contains(action.Name)),
                !notShowNumberBuffList.Contains(action.Name)
            );
        }

        public void GetHitAudio() {
            if (this.audioHeroConf == null) {
                return;
            }
            string audioPath = string.Concat("Audio/Battle/", this.audioHeroConf.hit);
            AudioManager.PlayWithPath(audioPath, AudioType.Action, AudioVolumn.High, isAdditive: true);
        }

        private void PlayDamageNumber(
            int damage, UnityAction callback,
            bool isShield, bool isHeal, bool showTxtEffect
        ) {
            Color color = Color.red;
            int reference = 1;
            if (isShield) {
                this.healthView.RemoveShield(damage, null);
                damage = -damage;
                color = Color.grey;
            } else {
                reference = 2;
                this.healthView.RemoveHealth(damage, () => {
                    reference--;
                    if (reference == 0) {
                        callback.InvokeSafe();
                    }
                });
            }
            if (isHeal) {
                color = Color.green;
            }
            GameObject txtEffect = PoolManager.GetObject(
                PrefabPath.txtEffect,
                this.pnlDamageRoot,
                poolType: PoolType.Battle
            );
            txtEffect.GetComponent<TextMeshProUGUI>().text =
                showTxtEffect ? damage.ToString() : string.Empty;
            txtEffect.GetComponent<TextMeshProUGUI>().color = color;
            txtEffect.GetComponent<RectTransform>().anchoredPosition =
                this.rectHealth.anchoredPosition;
            AnimationManager.Animate(txtEffect, "Show", () => {
                PoolManager.RemoveObject(txtEffect, PoolType.Battle);
                reference--;
                if (reference == 0) {
                    callback.InvokeSafe();
                }
            });
        }

        private void RemoveBuff(string buffName) {
            BattleBuff buff;
            if (this.buffDict.TryGetValue(buffName, out buff)) {
                PoolManager.RemoveObject(buff.obj, PoolType.Battle);
                this.buffDict.Remove(buffName);
                this.buffTargetDict[buff.target] = null;
            }
        }

        private IEnumerator Delay(float seconds, UnityAction callback) {
            yield return new WaitForSeconds(seconds);
            callback.InvokeSafe();
        }

        private void InvokeStepEnd() {
            if (this.onStepEnd != null) {
                UnityAction tmpAction = this.onStepEnd;
                this.onStepEnd = null;
                tmpAction.InvokeSafe();
            }
        }

        private void SetStepEnd(UnityAction callback) {
            if (callback != null) {
                this.onStepEnd = callback;
            }
        }

        public void OnInPool() {

        }

        public void OnOutPool() {
            foreach (BattleEffectView view in this.effectDict.Values) {
                view.OnOutPool();
            }
        }
    }
}
