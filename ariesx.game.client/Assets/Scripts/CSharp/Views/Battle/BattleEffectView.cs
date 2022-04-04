using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public enum BattleEffectType {
        Attack,
        Skill
    }

    public enum BattleEffectStepType {
        Cast,
        Move,
        Fly,
        End,
        Hit,
        Last,
        Wait,
        Back,
        Attack,
        Skill,
        Trans,
        Reset,
        HeroAttackAudio,
        HeroHitAudio,
        HeroSkillAudio,
        AttackCastAudio,
        AttackHitAudio,
        SkillCastAudio,
        SkillHitAudio,
        AttackMoveAudio,
        SkillMoveAudio
    }

    public enum BattleEffectTargetType {
        Head,
        Body,
        Foot
    }

    public enum BattleEffectMat {
        Replace,
        Add
    }

    [System.Serializable]
    public struct BattleEffectStep {
        public float endDelay;
        public BattleEffectStepType type;
        public BattleEffectTargetType target;
        public bool notShowDamage;
        public bool useCustomDelay;
        public List<Transform> rootList;
        public BattleEffectMat matType;
        public Material mat;
    }

    public class BattleEffectView : MonoBehaviour {
        public string heroName;
        public string index;
        public UnityAction<AnimationParam> onBack;
        public UnityAction onEnd;

        private AudioHeroConf audioHeroConf;
        private AudioSKillConf audioSKillConf;

        [HideInInspector]
        public BattleEffectType effectType;
        [HideInInspector]
        public SkinnedMeshRenderer roleRenderer;
        [SerializeField, HideInInspector]
        public BattleEffectStep[] stepList;
        [HideInInspector]
        public bool targetSplit = true;
        [HideInInspector]
        public int step = 0;

        private bool isCampaign = false;

        private BattlePositionView positionView;
        List<BattlePositionView> targetList;
        private string effectName;

        [SerializeField]
        public List<string> effectList = new List<string>();

        private UnityAction moveAction = null;

        private Vector3 oldPosition;

        public Material[] oldMaterials;

        void Awake() {
            if (this.roleRenderer != null) {
                this.oldMaterials = new Material[this.roleRenderer.sharedMaterials.Length];
                this.roleRenderer.sharedMaterials.CopyTo(this.oldMaterials, 0);
            }
        }

        public void Begin(BattlePositionView caster, string effectName, string heroName,
            List<BattlePositionView> targetList, bool isCampaign, UnityAction<AnimationParam> backCallback,
            UnityAction endCallback, UnityAction moveAction) {
            if (step != 0) {
                return;
            }
            this.moveAction = moveAction;
            this.onBack = backCallback;
            this.onEnd = endCallback;
            this.positionView = caster;
            this.oldPosition = this.positionView.transform.position;
            this.effectName = effectName;
            this.targetList = targetList;
            this.isCampaign = isCampaign;
            this.heroName = heroName;
            this.audioHeroConf = AudioHeroConf.GetConf(this.heroName);
            this.audioSKillConf = AudioSKillConf.GetConf(
                this.name.Substring(0, this.name.LastIndexOf('_'))
            );
            this.Act();
        }

        public void Act() {
            if (this.step > stepList.Length - 1) {
                this.onEnd.InvokeSafe();
                this.step = 0;
            } else {
                UnityAction action;
                BattleEffectStep effectStep = stepList[this.step++];
                switch (effectStep.type) {
                    case BattleEffectStepType.Cast:
                        action = this.Cast;
                        break;
                    case BattleEffectStepType.End:
                        action = this.End;
                        break;
                    case BattleEffectStepType.Last:
                        action = this.Last;
                        break;
                    case BattleEffectStepType.Move:
                        action = this.Move;
                        break;
                    case BattleEffectStepType.Fly:
                        action = this.Fly;
                        break;
                    case BattleEffectStepType.Hit:
                        action = this.Hit;
                        break;
                    case BattleEffectStepType.Back:
                        action = this.Back;
                        break;
                    case BattleEffectStepType.Wait:
                        action = this.Wait;
                        break;
                    case BattleEffectStepType.Attack:
                        action = this.Attack;
                        break;
                    case BattleEffectStepType.Skill:
                        action = this.Skill;
                        break;
                    case BattleEffectStepType.Trans:
                        action = this.Trans;
                        break;
                    case BattleEffectStepType.Reset:
                        action = this.Reset;
                        break;
                    default:
                        action = () => this.PlayAudio();
                        break;
                }
                action.InvokeSafe();
            }
        }

        private void Trans() {
            BattleEffectStep effectStep = this.stepList[this.step - 1];
            switch (effectStep.matType) {
                case BattleEffectMat.Add:
                    List<Material> matList = new List<Material>(this.roleRenderer.materials);
                    matList.Add(effectStep.mat);
                    this.roleRenderer.materials = matList.ToArray();
                    Debug.LogError(this.oldMaterials.Length);
                    break;
                case BattleEffectMat.Replace:
                    this.roleRenderer.material = effectStep.mat;
                    break;
            }
            base.StartCoroutine(this.ActEnd());
        }

        private void Reset() {
            this.roleRenderer.materials = this.oldMaterials;
            base.StartCoroutine(this.ActEnd());
        }

        private IEnumerator ActEnd(UnityAction callback = null) {
            float endDlay = this.stepList[this.step - 1].endDelay;
            if (endDlay != 0) {
                yield return YieldManager.GetWaitForSeconds(endDlay);
            }
            callback.InvokeSafe();
            this.Act();
        }

        private string GetEffectPath(string action) {
            string originPath = string.Format("{0}_{1}", this.name, action);
            if (this.isCampaign) {
                string path = ArtPrefabConf.GetValue(string.Concat(originPath));
                if (string.IsNullOrEmpty(path)) {
                    return string.Empty;
                } else {
                    return string.Format(
                        "Battle/Skills/{0}/{1}",
                        path.Substring(0, 8),
                        path
                    );
                }
            } else {
                return string.Format(
                    "Battle/Skills/{0}/{1}_{2}",
                    this.heroName,
                    this.name,
                    action
                );
            }
        }

        public void Cast() {
            string path = this.GetEffectPath("cast");
            if (this.stepList[this.step - 1].rootList.Count != 0) {
                foreach (Transform root in this.stepList[this.step - 1].rootList) {
                    this.positionView.GetSkillCastPosView().Create(path, root);
                }
            } else {
                this.positionView.GetSkillCastPosView().Create(path);
            }
            base.StartCoroutine(ActEnd());
        }

        public void Move() {
            BattlePositionView target = this.targetList[0];
            string animationName = string.Concat("Move", this.step);
            this.positionView.SetStatus(GameConst.animIsMove, true);
            this.positionView.Animate(GameConst.animMove);
            string path = this.GetEffectPath("move");

            GameObject moveObj = this.positionView.GetMovePosView().Create(path);
            AnimationManager.Animate(
                this.positionView.gameObject,
                this.GetComponent<AnimationCombo>().GetAnimation(animationName),
                positionView.transform.position,
                target.GetEnemyPosition(this.positionView.transform.position),
                finishCallback: () => {
                    PoolManager.RemoveObject(moveObj, PoolType.Battle);
                    this.positionView.SetStatus(GameConst.animIsMove, false);
                    this.positionView.ResetForward();
                },
                frameCallback: () => {
                    moveAction.InvokeSafe();
                    if (this.positionView == null) {
                        return;
                    }
                    Vector3 direction = this.positionView.transform.position - this.oldPosition;
                    if (direction.sqrMagnitude > 0.04f) {
                        this.positionView.SetForward(direction);
                        this.oldPosition = this.positionView.transform.position;
                    }
                },
                space: PositionSpace.World
            );
            base.StartCoroutine(this.ActEnd());
        }

        public void Fly() {
            if (this.targetSplit) {
                this.SplitFly();
            } else {
                this.UnionFly();
            }
        }

        private void UnionFly() {
            Vector3 targetPosition = Vector3.zero;
            foreach (BattlePositionView target in this.targetList) {
                targetPosition += target.GetSkillHitRoot(BattleEffectTargetType.Foot).transform.position;
            }
            targetPosition /= this.targetList.Count;
            string animationName = string.Concat("Fly", this.step);
            string path = this.GetEffectPath("fly");

            GameObject flyObj = this.positionView.GetFlyPosView().Create(path);
            if (flyObj != null) {
                bool useCustomDelay = this.stepList[this.step - 1].useCustomDelay;
                AnimationManager.Animate(
                    flyObj,
                    this.GetComponent<AnimationCombo>().GetAnimation(animationName),
                    flyObj.transform.position,
                    targetPosition,
                    () => {
                        PoolManager.RemoveObject(flyObj, PoolType.Battle);
                        if (!useCustomDelay) {
                            this.Act();
                        }
                    },
                    space: PositionSpace.World
                );
                if (useCustomDelay) {
                    base.StartCoroutine(this.ActEnd());
                }
            } else {
                this.Act();
            }
        }

        private void SplitFly() {
            int count = 0;
            bool useCustomDelay = this.stepList[this.step - 1].useCustomDelay;
            foreach (BattlePositionView target in this.targetList) {
                string animationName = string.Concat("Fly", this.step);
                string path = this.GetEffectPath("fly");
                GameObject flyObj = this.positionView.GetFlyPosView().Create(path);
                if (flyObj != null) {
                    AnimationManager.Animate(
                        flyObj,
                        this.GetComponent<AnimationCombo>().GetAnimation(animationName),
                        flyObj.transform.position,
                        target.GetSkillHitRoot(this.stepList[this.step - 1].target).transform.position,
                        () => {
                            PoolManager.RemoveObject(flyObj, PoolType.Battle);
                            if (!useCustomDelay && ++count == this.targetList.Count) {
                                this.Act();
                            }
                        }, space: PositionSpace.World
                    );
                    if (useCustomDelay && ++count == this.targetList.Count) {
                        base.StartCoroutine(this.ActEnd());
                    }
                } else {
                    if (++count == this.targetList.Count) {
                        this.Act();
                    }
                }
            }
        }

        private void End() {
            Vector3 targetPosition = Vector3.zero;
            foreach (BattlePositionView target in this.targetList) {
                targetPosition += target.
                    GetSkillHitRoot(BattleEffectTargetType.Foot).transform.position;
            }
            if (this.targetList.Count == 0) {
                targetPosition = this.transform.position;
            } else {
                targetPosition /= this.targetList.Count;
            }

            GameObject endObj = PoolManager.GetObject(
                this.GetEffectPath("end"),
                this.transform
            );
            if (endObj != null) {
                endObj.transform.position = targetPosition;
                endObj.GetComponent<BattleEffectAction>().particle.Play();
            }
            base.StartCoroutine(ActEnd());
        }

        private void Hit() {
            int count = 0;
            foreach (BattlePositionView target in this.targetList) {
                string path = this.GetEffectPath("hit");
                UnityAction judgeEnd = () => {
                    if (++count == this.targetList.Count) {
                        base.StartCoroutine(ActEnd());
                    };
                };
                target.GetSkillHitRoot(
                    this.stepList[this.step - 1].target
                ).Create(path);
                Battle.Action hitAction = null;
                target.hitActionDict.TryGetValue(effectName, out hitAction);
                if (target.heroId.CustomIsEmpty()) {
                    judgeEnd();
                    continue;
                }
                if (hitAction != null) {
                    target.GetHit(null, hitAction, !this.stepList[this.step - 1].notShowDamage);
                } else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneSimulator") {
                    ((BattleSimulatorPosView)target).GetHit(null, null);
                }
                judgeEnd();
            }
            // Temp
            //if ()
        }

        public void Last() {
            foreach (BattlePositionView target in this.targetList) {
                //Battle.Action action = null;
                GameObject lastObj = null;
                string path = this.GetEffectPath("last");
                lastObj = PoolManager.GetObject(path, null, poolType: PoolType.Battle);
                if (lastObj == null) {
                    continue;
                }
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneSimulator") {
                    ((BattleSimulatorPosView)target).
                        SetSkillLast(lastObj, this.stepList[this.step - 1].target, effectName);
                } else if (target.hitActionDict.ContainsKey(effectName) &&
                    target.hitActionDict[effectName].Type == SkillConst.TypeGetBuff) {
                    target.SetSkillLast(lastObj, this.stepList[this.step - 1].target, effectName);
                } else {
                    PoolManager.RemoveObject(lastObj, PoolType.Battle);
                    Debug.LogWarning(target.heroId + ": No last skill :" + effectName);
                }
                lastObj.GetComponent<BattleEffectAction>().particle.Play();
            }
            StartCoroutine(this.ActEnd());
        }

        private void Back() {
            AnimationParam animParam = this.GetComponent<AnimationCombo>().GetAnimation("Back" + step);
            this.onBack(animParam);
            base.StartCoroutine(this.ActEnd());
        }

        private void Wait() {
            StartCoroutine(this.ActEnd());
        }


        private void Attack() {
            this.positionView.Animate(GameConst.animAttack);
            base.StartCoroutine(this.ActEnd());
        }

        private void Skill() {
            this.positionView.Animate(GameConst.animSkill);
            base.StartCoroutine(this.ActEnd());
        }

        private void PlayAudio() {
            BattleEffectStepType audioType = this.stepList[this.step - 1].type;
            string audioPath = string.Empty;
            switch (audioType) {
                case BattleEffectStepType.HeroAttackAudio:
                    audioPath = this.audioHeroConf.attack;
                    break;
                case BattleEffectStepType.HeroSkillAudio:
                    audioPath = this.audioHeroConf.skill;
                    break;
                case BattleEffectStepType.HeroHitAudio:
                    foreach (BattlePositionView target in this.targetList) {
                        target.GetHitAudio();
                    }
                    break;
                case BattleEffectStepType.AttackCastAudio:
                case BattleEffectStepType.SkillCastAudio:
                    audioPath = this.audioSKillConf.cast;
                    break;
                case BattleEffectStepType.AttackHitAudio:
                case BattleEffectStepType.SkillHitAudio:
                    audioPath = this.audioSKillConf.hit;
                    break;
                case BattleEffectStepType.SkillMoveAudio:
                case BattleEffectStepType.AttackMoveAudio:
                    audioPath = this.audioSKillConf.move;
                    break;
                default:
                    break;
            }
            if (!audioPath.CustomIsEmpty()) {
                audioPath = string.Concat("Audio/Battle/", audioPath);
                AudioManager.PlayWithPath(audioPath, AudioType.Action, AudioVolumn.High, isAdditive: true);
            }
            base.StartCoroutine(this.ActEnd());
        }

        public void OnInPool() {

        }

        public void OnOutPool() {
            this.step = 0;
        }

    }
}
