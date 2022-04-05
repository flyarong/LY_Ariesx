using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;

namespace Poukoute {

    public class BattleAnimatorGenerator : MonoBehaviour {
        private static GameObject configureManager;
        private static StreamWriter fileWriter;
        private static StreamWriter curveWriter;

        // Skill path
        private const string skillSrcEffectPath =
            "Arts/Effects/Prefab/Battles/";
        private const string heroDstPath =
            "Resources/Battle/Heroes/";
        private const string skillDstEffectPath =
            "Resources/Battle/Skills/";
        private const string attackDstPath =
            "Resources/Battle/Attacks/";
        // Role path
        private const string heroAnimatorPath =
            "Assets/Arts/Animators/Battle/Heroes";
        private const string heroPath =
            "/Arts/Models/Battles/Heroes/";
        private const string heroEffectPath =
            "Assets/Resources/Battle/Skills/Effects";
        private const string heroAttackPath =
            "Assets/Resources/Battle/Skills/Attacks";
        private const string heroPrefabPath =
            "Assets/Resources/Battle/Heroes/";

        private const string ATTACK = "attack";
        private const string HIT = "hit";
        private const string IDLE = "idle";
        private const string MOVE = "move";
        private const string SKILL = "skill";
        private const string DEAD = "dead";
        private const string VICTOR = "victor";
        private const string FAILED = "failed";

        public const string CAST = "Cast";
        public const string SKILL_HIT = "Hit";
        public const string EFFECT = "Effect";

        //[MenuItem("Poukoute/Generator/Generate Animator/Skill/Conf2Prefab")]
        private static void GenerateBattlePrefabs() {
            Caching.CleanCache();
            try {
                InitPrefabConf();
                GenerateSkills();
                GenerateHeroes();
                // GenerateRoleAnimator();
                //GenerateStep();
                AssetDatabase.SaveAssets();
            } finally {
                UninitPrefabConf();
            }
        }

        [MenuItem("Poukoute/Generator/Generate Animator/Skill/Refresh Animator")]
        private static void RefreshAnimator() {
            Caching.CleanCache();
            try {
                InitPrefabConf();
                GenerateRoleAnimator();
                AssetDatabase.SaveAssets();
            } finally {
                UninitPrefabConf();
            }
        }

        public static void RefreshHeroRole(string heroId) {
            string path = string.Format(
                "{0}battle_{1}.prefab",
                heroPrefabPath,
                heroId
            );
            GameObject battlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject battleObj = PrefabUtility.InstantiatePrefab(
                battlePrefab
            ) as GameObject;
            Transform role = battleObj.transform.Find("Role");
            foreach (Transform child in role) {
                DestroyImmediate(child.gameObject);
            }
            string modelPath = string.Format(
                "Assets{0}{1}/{1}.prefab",
                heroPath,
                HeroBattleConf.heroDict[heroId]
            );
            Debug.LogError(modelPath);
            GameObject modelObj = Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(modelPath)
            );
            modelObj.transform.name = "Hero";
            modelObj.transform.SetParent(role);
            modelObj.transform.localPosition = Vector3.zero;
            PrefabUtility.ReplacePrefab(battleObj, battlePrefab, ReplacePrefabOptions.ConnectToPrefab);
            DestroyImmediate(battleObj);
        }

        //[MenuItem("Poukoute/Generator/Generate Animator/Skill/NewConf")]
        private static void NewConf() {
            string path = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_animation.csv"
            );
            string dstPath = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_animation_new.csv"
            );
            StreamReader fileReader = new StreamReader(path, Encoding.UTF8);
            StreamWriter fileWriter = new StreamWriter(dstPath, false, Encoding.UTF8);

            fileWriter.WriteLine(fileReader.ReadLine());
            string line = fileReader.ReadLine();
            fileWriter.WriteLine(line);
            string hero = line.CustomSplit(',')[0];
            while (!(line = fileReader.ReadLine()).CustomIsEmpty()) {
                string newHero = line.CustomSplit(',')[0];
                if (newHero != hero) {
                    fileWriter.WriteLine(string.Format(
                        "{0},{1},{2},{3}", hero,
                        "victor", "ActionPlay", 0.2
                    ));
                    fileWriter.WriteLine(string.Format(
                        "{0},{1},{2},{3}", hero,
                        "failed", "ActionPlay", 0.2
                    ));
                    fileWriter.WriteLine(string.Format(
                        "{0},{1},{2},{3}", hero,
                        "failed", "ActionEnd", 0.2
                    ));
                    hero = newHero;
                }
                fileWriter.WriteLine(line);
            }
            fileReader.Close();
            fileWriter.Flush();
            fileWriter.Close();
        }


        //[MenuItem("Poukoute/Generator/Generate Animator/Skill/Prefab2Conf")]
        private static void GenerateBattleConfs() {
            Caching.CleanCache();
            try {
                //InitPrefabConf();
                string path = string.Concat(
                    Application.dataPath,
                    "/Configures/battle_curve.csv"
                );
                File.WriteAllText(path, "");
                curveWriter = new StreamWriter(path, true, Encoding.UTF8);
                curveWriter.WriteLine("id,frames,origin,target");

                GenerateSkillConf();
                GenerateAttackConf();
                GenerateHeroConf();
                GenerateAnimationConf();

                curveWriter.Flush();
                curveWriter.Close();
                AssetDatabase.Refresh();
                ConfigureGenerator.GeneratorConfigure();
            } finally {
                //UninitPrefabConf();
            }
        }

        [MenuItem("Poukoute/Generator/Generate Battle/Refresh Effects")]
        private static void RefreshEffects() {
            GenerateStep();
        }


        [MenuItem("Poukoute/Generator/Generate Battle/New Hero")]
        private static void GenerateNewHeroes() {
            EditorWindow window = EditorWindow.GetWindow<BattleNewHeroGenerator>(true, "New Hero", true) as EditorWindow;
            window.ShowPopup();
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<BattleNewHeroGenerator>();
        }

        #region single
        public static void GenerateNewHero(string heroes) {
            try {
                InitPrefabConf();
                string[] heroList = heroes.CustomSplit(',');
                foreach (string id in heroList) {
                    string path = string.Format(
                        "{0}{1}battle_{2}.prefab",
                        Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")),
                        heroPrefabPath,
                        id
                    );
                    GenerateNewStep(id);
                    if (File.Exists(path)) {
                        RefreshHeroRole(id);
                        GenerateNewRoleAnimator(id);
                        return;
                    }
                    Debug.LogError("Not exist");
                    GenerateNewSkill(id);
                    GenerateNewHeroPostion(id);
                    GenerateNewRoleAnimator(id);
                }
            } finally {
                UninitPrefabConf();
            }
        }

        private static void GenerateNewSkill(string id) {
            GenerateOneSkill(id);
        }

        private static void GenerateNewRoleAnimator(string id) {
            GenerateOneRoleAnimator(id);
        }

        public static void GenerateNewStep(string id) {
            string assetSrcDirectory = string.Format(
                "Assets/{0}{1}",
                skillSrcEffectPath,
                GameHelper.UpperFirstCase(id)
            );
            if (Directory.Exists(assetSrcDirectory)) {
                string assetDstFolder = string.Format(
                   "Assets/{0}{1}",
                   skillDstEffectPath,
                   GameHelper.UpperFirstCase(id)
                );
                if (!Directory.Exists(assetDstFolder)) {
                    Directory.CreateDirectory(assetDstFolder);
                } else {
                    foreach (FileInfo fileInfo in (new DirectoryInfo(assetDstFolder)).GetFiles()) {
                        fileInfo.Delete();
                    }
                }
                GenerateOneHeroStep(id, assetSrcDirectory);
            }
        }

        //private static void GenerateNewHeroRole(string specularId) {
        //    foreach(string heroId in HeroBattleConf.modelDict.Keys) {
        //        if (heroId == specularId) {
        //            GameObject battleObj = new GameObject("battle_" + heroId);
        //            BattlePositionView positionView = battleObj.AddComponent<BattlePositionView>();
        //            positionView.heroId = heroId;
        //            GameObject role = GenerateRoleObj(heroId);
        //            role.transform.SetParent(battleObj.transform);
        //            role.transform.localPosition = Vector3.zero;
        //        }
        //    }
        //}

        private static void GenerateNewHeroPostion(string specularId) {
            foreach (string heroId in HeroBattleConf.modelDict.Keys) {
                if (heroId == specularId) {
                    GameObject battleObj = new GameObject("battle_" + heroId);
                    BattlePositionView positionView = battleObj.AddComponent<BattlePositionView>();
                    positionView.heroId = heroId;
                    GameObject role = GenerateRoleObj(heroId);
                    role.transform.SetParent(battleObj.transform);
                    role.transform.localPosition = Vector3.zero;

                    GameObject position = GeneratePositionObj(heroId, role.transform);
                    position.transform.SetParent(battleObj.transform);
                    position.transform.localPosition = Vector3.zero;

                    GameObject skill = GenerateSkillObj(heroId);
                    skill.transform.SetParent(battleObj.transform);
                    skill.transform.localPosition = Vector3.zero;

                    string assetPath = string.Format(
                        "Assets/{0}battle_{1}.prefab",
                        heroDstPath,
                        heroId
                    );
                    PrefabUtility.CreatePrefab(assetPath, battleObj);
                    DestroyImmediate(battleObj);
                }
            }
        }
        #endregion

        #region battle_hero


        private static void GenerateHeroes() {
            foreach (string heroId in HeroBattleConf.modelDict.Keys) {
                GameObject battleObj = new GameObject("battle_" + heroId);
                BattlePositionView positionView = battleObj.AddComponent<BattlePositionView>();
                positionView.heroId = heroId;
                GameObject role = GenerateRoleObj(heroId);
                role.transform.SetParent(battleObj.transform);
                role.transform.localPosition = Vector3.zero;

                GameObject position = GeneratePositionObj(heroId, role.transform);
                position.transform.SetParent(battleObj.transform);
                position.transform.localPosition = Vector3.zero;

                GameObject skill = GenerateSkillObj(heroId);
                skill.transform.SetParent(battleObj.transform);
                skill.transform.localPosition = Vector3.zero;

                string assetPath = string.Format(
                    "Assets/{0}battle_{1}.prefab",
                    heroDstPath,
                    heroId
                );
                PrefabUtility.CreatePrefab(assetPath, battleObj);
                DestroyImmediate(battleObj);
            }
        }

        private static GameObject GenerateRoleObj(string heroId) {
            GameObject roleObj = new GameObject("Role");
            heroId = HeroBattleConf.heroDict[heroId];
            string rolePath = string.Format(
                "Assets{0}{1}/{1}.prefab",
                heroPath,
                heroId
            );
            GameObject hero = PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(rolePath)
            ) as GameObject;
            hero.transform.SetParent(roleObj.transform);
            hero.transform.localPosition = Vector3.zero;
            hero.name = "Hero";
            return roleObj;
        }

        private static GameObject GeneratePositionObj(string heroId, Transform role) {
            GameObject positionObj = new GameObject("Position");
            Dictionary<string, EditorBattleHeroPositionConf> positionDict =
             EditorBattleHeroPositionConf.GetPositionDict(heroId);
            if (positionDict == null) {
                Debug.LogErrorf("There is no sucn conf of hero: {0}", heroId);
                return null;
            }
            foreach (var pair in positionDict) {
                string key = pair.Key;
                EditorBattleHeroPositionConf conf = pair.Value;
                string[] points = key.CustomSplit('.');
                Transform root = positionObj.transform;
                foreach (string point in points) {
                    Transform pointTrans = root.transform.Find(point);
                    GameObject pointObj = null;
                    if (pointTrans == null) {
                        pointObj = new GameObject(point);
                    } else {
                        pointObj = pointTrans.gameObject;
                    }
                    pointObj.transform.SetParent(root);
                    root = pointObj.transform;
                }
                BattleEffectPositionView positionView =
                    root.gameObject.AddComponent<BattleEffectPositionView>();
                positionView.offset = conf.offset;
                positionView.Root = GameHelper.GetTransformByName(role, conf.root);
            }
            return positionObj;
        }

        private static GameObject GenerateSkillObj(string heroId) {
            GameObject skillObj = new GameObject("Skill");

            foreach (string key in EditorBattleHeroSkillConf.GetEffectDict(heroId).Keys) {
                string skillPath = string.Format(
                    "{0}/{1}_{2}.prefab",
                    heroEffectPath,
                    heroId,
                    key
                );
                GameObject effect = PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(skillPath)
                ) as GameObject;
                effect.transform.SetParent(skillObj.transform);
                effect.transform.localPosition = Vector3.zero;
                effect.name = string.Concat(heroId, '_', key);
            }
            return skillObj;
        }


        private static void GenerateStep() {
            DirectoryInfo effectInfo = new DirectoryInfo(
                string.Format("{0}/{1}", Application.dataPath, skillSrcEffectPath)
            );
            foreach (DirectoryInfo direcotryInfo in effectInfo.GetDirectories()) {
                string assetDstFolder = string.Format(
                    "Assets/{0}{1}",
                    skillDstEffectPath,
                    GameHelper.LowerFirstCase(direcotryInfo.Name)
                );

                if (!Directory.Exists(assetDstFolder)) {
                    Directory.CreateDirectory(assetDstFolder);
                }

                DirectoryInfo dstDirectoryInfo = new DirectoryInfo(assetDstFolder);
                foreach (FileInfo fileInfo in dstDirectoryInfo.GetFiles()) {
                    fileInfo.Delete();
                }
                GenerateOneHeroStep(
                    GameHelper.LowerFirstCase(direcotryInfo.Name),
                    direcotryInfo.FullName
                );
            }
        }

        private static void GenerateOneHeroStep(string heroId, string assetPath) {
            DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
                if (fileInfo.Extension != ".prefab") {
                    continue;
                }
                string assetSrcPath = string.Format(
                    "Assets/{0}{1}/{2}",
                    skillSrcEffectPath,
                    GameHelper.UpperFirstCase(heroId),
                    fileInfo.Name
                );
                string assetDstPath = string.Format(
                   "Assets/{0}{1}/{2}",
                    skillDstEffectPath,
                    GameHelper.LowerFirstCase(heroId),
                    fileInfo.Name
                );
                GameObject effectPrfab = Instantiate(
                    AssetDatabase.LoadAssetAtPath<GameObject>(assetSrcPath)
                );
                BattleEffectAction effectAction = effectPrfab.AddComponent<BattleEffectAction>();

                float particleDuration = 0;
                float animatorDuration = 0;
                //animator
                effectAction.animator = GameHelper.GetFirstComponent<Animator>(effectAction.transform);
                if (effectAction.animator != null) {
                    AnimatorController ac = effectAction.animator.runtimeAnimatorController as AnimatorController;
                    AnimatorStateMachine sm = ac.layers[0].stateMachine;
                    foreach (ChildAnimatorState state in sm.states) {
                        AnimationClip clip = state.state.motion as AnimationClip;
                        effectAction.loop |= clip.isLooping;
                        animatorDuration = clip.length;
                        break;
                    }
                }

                //particle
                effectAction.particle = GameHelper.GetFirstComponent<ParticleSystem>(effectAction.transform);
                if (effectAction.particle != null) {
                    ParticleSystem.MainModule main = effectAction.particle.main;
                    main.playOnAwake = false;
                    effectAction.loop |= effectAction.particle.main.loop;
                    particleDuration = effectAction.particle.main.duration;
                }

                effectAction.duration = Mathf.Max(particleDuration, animatorDuration);

                GameObject newPrefab = PrefabUtility.CreatePrefab(
                    assetDstPath, effectPrfab
                ) as GameObject;
                DestroyImmediate(effectPrfab);
            }
        }

        private static void Tmp(List<FileInfo> fileInfoList, string heroId) {

        }

        private static void InitPrefabConf() {
            ConfigureGenerator.GeneratorConfigure();
            AssetDatabase.Refresh();
            configureManager = new GameObject();
            configureManager.name = "ConfigureManager";
            configureManager.transform.position = UnityEngine.Vector3.zero;
            configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadHeroEditorConfigures();
        }

        private static void UninitPrefabConf() {
            GameObject.DestroyImmediate(configureManager);
            EditorBattleHeroSkillConf.ClearHeroSkillDict();
            EditorBattleHeroPositionConf.ClearPositionDict();
            EditorBattleHeroAttackConf.ClearHeroAttackDict();
            EditorBattleHeroAnimationConf.ClearActionEventDict();
        }
        #endregion

        #region effect_hero 

        private static void GenerateSkills() {
            foreach (string heroId in HeroBattleConf.modelDict.Keys) {
                GenerateOneSkill(heroId);
            }
        }

        private static void GenerateOneSkill(string heroId) {
            Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>> effectDict =
                   EditorBattleHeroSkillConf.GetEffectDict(heroId);
            foreach (var pair in effectDict) {
                GameObject effectObj = new GameObject(string.Concat(heroId, "_", pair.Key));
                BattleEffectView view = effectObj.AddComponent<BattleEffectView>();
                view.heroName = heroId;
                view.index = pair.Key;
                view.stepList = new BattleEffectStep[pair.Value.Values.Count];
                int index = 0;
                foreach (EditorBattleHeroSkillConf conf in pair.Value.Values) {
                    view.effectType = conf.index.Contains("attack") ? BattleEffectType.Attack : BattleEffectType.Skill;
                    view.stepList[index++] = new BattleEffectStep() {
                        endDelay = conf.endDelay,
                        type = conf.stepType,
                        target = conf.target,
                    };
                    GenerateEffectCurve(index, conf, effectObj);
                }
                string effectPath = string.Format(
                    "{0}/{1}_{2}.prefab",
                    heroEffectPath,
                    heroId,
                    pair.Key
                );
                PrefabUtility.CreatePrefab(effectPath, effectObj);

                DestroyImmediate(effectObj);
            }
        }

        private static void GenerateAttacks() {
            foreach (string heroId in HeroBattleConf.modelDict.Keys) {
                Dictionary<string, Dictionary<string, EditorBattleHeroSkillConf>> effectDict =
                    EditorBattleHeroSkillConf.GetEffectDict(heroId);
                Debug.LogError(heroId);
                foreach (var pair in effectDict) {
                    GameObject effectObj = new GameObject(string.Concat(heroId, "_", pair.Key));
                    BattleEffectView view = effectObj.AddComponent<BattleEffectView>();
                    view.effectType = BattleEffectType.Skill;
                    view.heroName = heroId;
                    view.index = pair.Key;
                    view.stepList = new BattleEffectStep[pair.Value.Values.Count];
                    int index = 0;
                    foreach (EditorBattleHeroSkillConf conf in pair.Value.Values) {
                        view.stepList[index++] = new BattleEffectStep() {
                            endDelay = conf.endDelay,
                            type = conf.stepType,
                            target = conf.target,
                        };
                        GenerateEffectCurve(index, conf, effectObj);
                    }
                    string effectPath = string.Format(
                        "{0}/{1}_{2}.prefab",
                        heroAttackPath,
                        heroId,
                        pair.Key
                    );
                    PrefabUtility.CreatePrefab(effectPath, effectObj);

                    DestroyImmediate(effectObj);
                }
            }
        }

        private static void
        GenerateEffectCurve(int step, EditorBattleHeroSkillConf conf, GameObject obj) {
            if (conf.stepType == BattleEffectStepType.Fly ||
                conf.stepType == BattleEffectStepType.Move) {
                AnimationParam animParam = obj.AddComponent<AnimationParam>();
                animParam.animationName = string.Concat(conf.stepType.ToString(), step);
                animParam.isMoving = true;
                animParam.isXYZSeperate = true;

                string curvePrefix = string.Format(
                    "{0}_{1}_{2}{3}",
                    conf.heroId,
                    conf.index,
                    conf.stepType.ToString().ToLower(),
                    step
                );

                string curveMoveName = string.Format(
                    "{0}_{1}",
                    curvePrefix,
                    "move"
                );
                animParam.moveCurve = GetAnimationCurve(curveMoveName);
                EditorBattleCurveConf tmpConf = ConfigureManager.GetConfById<EditorBattleCurveConf>(curveMoveName);
                if (tmpConf != null) {
                    animParam.startOffset = tmpConf.origin;
                    animParam.targetOffset = tmpConf.target;
                }

                string curveMoveXName = string.Format(
                    "{0}_{1}",
                    curvePrefix,
                    "move_x"
                );
                animParam.moveXCurve = GetAnimationCurve(curveMoveXName);

                string curveMoveYName = string.Format(
                    "{0}_{1}",
                    curvePrefix,
                    "move_y"
                );
                animParam.moveYCurve = GetAnimationCurve(curveMoveYName);

                string curveMoveZName = string.Format(
                    "{0}_{1}",
                    curvePrefix,
                    "move_z"
                );
                animParam.moveZCurve = GetAnimationCurve(curveMoveZName);
            }
        }

        private static AnimationCurve GetAnimationCurve(string curveName) {
            EditorBattleCurveConf conf = ConfigureManager.GetConfById<EditorBattleCurveConf>(curveName);
            if (conf == null) {
                return null;
            }
            return new AnimationCurve(conf.frameList.ToArray());
        }



        #endregion

        #region role animation
        private static void GenerateRoleAnimator() {
            AssetDatabase.Refresh();
            foreach (string heroName in HeroBattleConf.modelDict.Keys) {
                GenerateOneRoleAnimator(heroName);
            }
        }

        private static void GenerateOneRoleAnimator(string heroId) {
            AnimatorController acB = GenerateRoleAnimator(heroId, HeroBattleConf.heroDict[heroId]);
            GenerateRolePrefab(acB, heroId);
        }

        private static void InitRole() {
            configureManager = new GameObject();
            configureManager.name = "ConfigureManager";
            configureManager.transform.position = UnityEngine.Vector3.zero;
            configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadHeroEditorConfigures();
        }

        private static AnimatorController GenerateRoleAnimator(string heroName, string heroPath) {
            string fullPath = string.Format("{0}/{1}.controller", heroAnimatorPath, heroName);
            string absPath = Application.dataPath + "/" + fullPath;
            if (File.Exists(absPath)) {
                AssetDatabase.LoadAssetAtPath<AnimatorController>(string.Format("Assets/{0}", fullPath));
            }
            AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(fullPath);
            AnimationClip attackClip = GenerateRoleAnimation(heroPath, ATTACK);
            AnimationClip hitClip = GenerateRoleAnimation(heroPath, HIT);
            AnimationClip idleClip = GenerateRoleAnimation(heroPath, IDLE);
            AnimationClip moveClip = GenerateRoleAnimation(heroPath, MOVE);
            AnimationClip skillClip = GenerateRoleAnimation(heroPath, SKILL);
            AnimationClip victorClip = GenerateRoleAnimation(heroPath, VICTOR);
            AnimationClip failedClip = GenerateRoleAnimation(heroPath, FAILED);
            AnimatorStateMachine rootStateMachine = ac.layers[0].stateMachine;
            AnimatorState idle = rootStateMachine.AddState("Idle");
            //BattleStateMachineBehaviour behaviour =
            //    idle.AddStateMachineBehaviour<BattleStateMachineBehaviour>();

            idle.motion = idleClip;
            CreateRoleState(ac, ATTACK, rootStateMachine, GameHelper.UpperFirstCase(ATTACK), idle, attackClip);
            CreateRoleState(ac, HIT, rootStateMachine, GameHelper.UpperFirstCase(HIT), idle, hitClip);
            CreateRoleState(ac, MOVE, rootStateMachine, GameHelper.UpperFirstCase(MOVE), idle, moveClip);
            CreateRoleState(ac, SKILL, rootStateMachine, GameHelper.UpperFirstCase(SKILL), idle, skillClip);
            CreateRoleState(ac, DEAD, rootStateMachine, GameHelper.UpperFirstCase(DEAD), idle, null);
            CreateRoleState(ac, FAILED, rootStateMachine, GameHelper.UpperFirstCase(FAILED), idle, failedClip);
            CreateRoleState(ac, VICTOR, rootStateMachine, GameHelper.UpperFirstCase(VICTOR), idle, victorClip);
            return ac;
        }

        private static AnimationClip GenerateRoleAnimation(string path, string action) {
            string directory = string.Format("{0}{1}{2}/{3}/",
                Application.dataPath, heroPath, path, action);
            string animPath = string.Format("Assets{0}{1}/{1}_{2}.anim",
                heroPath, path, action);
            AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            if (animClip == null) {
                return null;
            }
            float animLength = animClip.length;
            List<UnityEngine.AnimationEvent> animationList = new List<UnityEngine.AnimationEvent>();
            switch (action) {
                case VICTOR:
                case FAILED:
                    EditorBattleHeroAnimationConf heroAnimationConf =
                        ConfigureManager.GetConfById<EditorBattleHeroAnimationConf>(path + action);

                    foreach (EditorBattleHeroAnimationConf conf in
                        EditorBattleHeroAnimationConf.GetActionEventDict(path + action).Values) {
                        UnityEngine.AnimationEvent actonEvent = new UnityEngine.AnimationEvent();
                        actonEvent.intParameter = (int)System.Enum.Parse(typeof(RoleAction),
                            GameHelper.UpperFirstCase(conf.animation));
                        actonEvent.functionName = conf.functionName;
                        actonEvent.time = conf.time;
                        animationList.Insert(0, actonEvent);
                    }
                    break;
                case SKILL:
                    string failedPath = string.Format(
                        "Assets{0} {1}/{1}_{2}.anim",
                        heroPath, path, FAILED
                    );
                    AnimationClip failedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
                    if (failedClip == null) {
                        UnityEngine.AnimationEvent actonEvent = new UnityEngine.AnimationEvent();
                        actonEvent.intParameter = (int)System.Enum.Parse(typeof(RoleAction),
                            GameHelper.UpperFirstCase(FAILED));
                        actonEvent.functionName = "ActionEnd";
                        actonEvent.time = animClip.length - 0.1f;
                        animationList.Insert(0, actonEvent);
                    }
                    break;
                case ATTACK:
                case HIT:
                    break;
                case MOVE:
                case IDLE:
                    AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClip);
                    settings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(animClip, settings);
                    break;
                default:
                    break;
            }
            animationList.Sort((a1, a2) => {
                return a1.time.CompareTo(a2.time);
            });

            AnimationUtility.SetAnimationEvents(animClip, animationList.ToArray());
            return animClip;
        }

        private static void CreateRoleState(AnimatorController ac, string animation,
            AnimatorStateMachine root, string triggerName, AnimatorState idle, AnimationClip clip) {
            AnimatorState state = root.AddState(triggerName);
            state.motion = clip;

            AnimatorStateTransition enterTrans = root.AddAnyStateTransition(state);
            AnimatorStateTransition exitTrans = state.AddExitTransition();
            ac.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);
            enterTrans.AddCondition(AnimatorConditionMode.If, 0, triggerName);
            if (triggerName == "Move") {
                ac.AddParameter("IsMove", AnimatorControllerParameterType.Bool);
                enterTrans.AddCondition(AnimatorConditionMode.If, 0, "IsMove");
            }
            switch (triggerName) {
                case "Move":
                    exitTrans.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMove");
                    break;
                case "Dead":
                case "Failed":
                case "Victor":
                    bool hasReset = false;
                    foreach (var param in ac.parameters) {
                        if (param.name == "Reset") {
                            hasReset = true;
                            break;
                        }
                    }
                    
                    if (!hasReset) {
                        ac.AddParameter("Reset", AnimatorControllerParameterType.Trigger);
                    }
                    exitTrans.AddCondition(AnimatorConditionMode.If, 0, "Reset");
                    break;
                default:
                    exitTrans.hasExitTime = true;
                    exitTrans.exitTime = 0.99f;
                    exitTrans.duration = 0;
                    break;
            }
            exitTrans.destinationState = idle;
        }

        private static void GenerateRolePrefab(AnimatorController ac, string heroName) {
            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string assetPath = string.Format("{0}battle_{1}.prefab",
                heroPrefabPath, heroName);
            string fullPath = folder + assetPath;
            if (File.Exists(fullPath)) {
                GameObject heroPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                GameObject heroObj = Instantiate(heroPrefab);
                Transform role = heroObj.transform.Find("Role");
                role.localScale = Vector3.one;
                GameObject model = role.childCount > 0 ? role.GetChild(0).gameObject : null;
                if (model == null) {
                    string modelPath = string.Format(
                        "Assets{0}{1}/{1}.prefab",
                        heroPath,
                        HeroBattleConf.heroDict[heroName]
                    );
                    model = Instantiate(
                        AssetDatabase.LoadAssetAtPath<GameObject>(modelPath)
                    );
                    model.transform.SetParent(role);
                    model.transform.localScale = Vector3.one;
                    model.transform.localPosition = Vector3.zero;
                }
                if (model.GetComponent<BattleAction>() == null) {
                    model.gameObject.AddComponent<BattleAction>();
                }
                model.name = "Hero";
                GameHelper.SetLayer(model, LayerMask.NameToLayer("Battle"));
                role.GetComponentInChildren<Animator>().runtimeAnimatorController = ac;
                GameObject newPrefab =
                    PrefabUtility.CreatePrefab(assetPath, heroObj) as GameObject;
                DestroyImmediate(heroObj);
            } else {
                Debug.LogErrorf("No such battle position with hero {0}", heroName);
            }
        }

        #endregion

        #region skill_conf
        private static void GenerateSkillConf() {
            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string path = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_skill.csv"
            );
            string dirPath = string.Concat(folder, heroEffectPath);

            File.WriteAllText(path, "");
            fileWriter = new StreamWriter(path, true, Encoding.UTF8);
            fileWriter.WriteLine("hero,index,step,end_delay,target");

            DirectoryInfo effectInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fileInfo in effectInfo.GetFiles()) {
                if (fileInfo.Extension != ".prefab" || fileInfo.Name.Contains("attack")) {
                    continue;
                }
                GameObject effectObj = AssetDatabase.LoadAssetAtPath<GameObject>(
                    string.Concat(heroEffectPath, "/", fileInfo.Name)
                );
                BattleEffectView effectView = effectObj.GetComponent<BattleEffectView>();
                foreach (BattleEffectStep step in effectView.stepList) {
                    fileWriter.WriteLine(string.Format(
                        "{0},{1},{2},{3},{4}",
                        effectView.heroName,
                        effectView.index,
                        step.type.ToString().ToLower(),
                        step.endDelay,
                        step.target.ToString().ToLower()
                    ));
                }
                GenerateCurveConf(effectView.heroName + "_skill_", effectObj.GetComponents<AnimationParam>());
            }

            fileWriter.Flush();
            fileWriter.Close();

        }
        #endregion

        #region attack_conf
        private static void GenerateAttackConf() {
            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string path = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_attack.csv"
            );
            string dirPath = string.Concat(folder, heroAttackPath);

            File.WriteAllText(path, "");
            fileWriter = new StreamWriter(path, true, Encoding.UTF8);
            fileWriter.WriteLine("hero,step,end_delay,target");

            DirectoryInfo effectInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fileInfo in effectInfo.GetFiles()) {
                if (fileInfo.Extension != ".prefab" || fileInfo.Name.Contains("skill")) {
                    continue;
                }
                GameObject effectObj = AssetDatabase.LoadAssetAtPath<GameObject>(
                    string.Concat(heroAttackPath, "/", fileInfo.Name)
                );
                BattleEffectView effectView = effectObj.GetComponent<BattleEffectView>();
                foreach (BattleEffectStep step in effectView.stepList) {
                    fileWriter.WriteLine(string.Format(
                        "{0},{1},{2},{3},{4}",
                        effectView.heroName,
                        effectView.index,
                        step.type.ToString().ToLower(),
                        step.endDelay,
                        step.target.ToString().ToLower()
                    ));
                }
                GenerateCurveConf(effectView.heroName + "_" + effectView.index +
                    "_" + "_attack_", effectObj.GetComponents<AnimationParam>());
            }

            fileWriter.Flush();
            fileWriter.Close();
        }
        #endregion


        #region hero_conf

        private static void GenerateHeroConf() {
            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string path = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_position.csv"
            );
            string dirPath = string.Concat(folder, heroPrefabPath);

            File.WriteAllText(path, "");
            fileWriter = new StreamWriter(path, true, Encoding.UTF8);
            fileWriter.WriteLine("hero,point,root,x,y,z");

            DirectoryInfo heroInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fileInfo in heroInfo.GetFiles()) {
                if (fileInfo.Extension != ".prefab") {
                    continue;
                }
                GameObject heroObj = AssetDatabase.LoadAssetAtPath<GameObject>(
                    string.Concat(heroPrefabPath, fileInfo.Name)
                );
                string heroId = fileInfo.Name.Substring(fileInfo.Name.IndexOf('_') + 1, 8);
                foreach (Transform child in heroObj.transform.Find("Position")) {
                    string point = GameHelper.ToLowercaseNamingConvention(child.name);
                    BattleEffectPositionView posView = null;
                    Vector3 offset = Vector2.zero;
                    if (child.childCount > 0) {
                        foreach (Transform grandChild in child) {
                            posView = grandChild.GetComponent<BattleEffectPositionView>();
                            offset = grandChild.transform.localPosition;
                            string childPoint = string.Concat(point, '.',
                                 GameHelper.ToLowercaseNamingConvention(
                                    grandChild.name
                                )
                            );
                            fileWriter.WriteLine(string.Format(
                                "{0},{1},{2},{3},{4},{5}",
                                heroId,
                                childPoint,
                                posView.effectRoot == null ? "" : posView.effectRoot.name,
                                offset.x,
                                offset.y,
                                offset.z
                            ));
                        }
                    } else {
                        posView = child.GetComponent<BattleEffectPositionView>();
                        fileWriter.WriteLine(string.Format(
                            "{0},{1},{2},{3},{4},{5}",
                            heroId,
                            point,
                            posView.effectRoot == null ? "" : posView.effectRoot.name,
                            offset.x,
                            offset.y,
                            offset.z
                        ));
                    }
                }

            }
            fileWriter.Flush();
            fileWriter.Close();
        }
        #endregion

        #region animation_conf
        private static void GenerateAnimationConf() {
            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string path = string.Concat(
                Application.dataPath,
                "/Configures/battle_hero_animation.csv"
            );
            string dirPath = string.Concat(folder, heroPrefabPath);

            File.WriteAllText(path, "");
            fileWriter = new StreamWriter(path, true, Encoding.UTF8);
            fileWriter.WriteLine("hero,animation,function,time");

            DirectoryInfo heroInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fileInfo in heroInfo.GetFiles()) {
                if (fileInfo.Extension != ".prefab") {
                    continue;
                }
                GameObject heroObj = AssetDatabase.LoadAssetAtPath<GameObject>(
                    string.Concat(heroPrefabPath, fileInfo.Name)
                );
                string heroId = fileInfo.Name.Substring(fileInfo.Name.IndexOf('_') + 1, 8);
                Animator animator = heroObj.transform.Find("Role").GetComponentInChildren<Animator>();
                AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
                AnimatorStateMachine sm = ac.layers[0].stateMachine;
                for (int i = 0; i < sm.states.Length; i++) {
                    AnimationClip clip = sm.states[i].state.motion as AnimationClip;
                    if (clip == null) {
                        continue;
                    }
                    string stateName = sm.states[i].state.name;
                    foreach (UnityEngine.AnimationEvent animEvent in clip.events) {
                        fileWriter.WriteLine(
                            "{0},{1},{2},{3}",
                            heroId,
                            stateName.ToLower(),
                            animEvent.functionName,
                            animEvent.time
                        );
                    }
                }
            }
            fileWriter.Flush();
            fileWriter.Close();
        }
        #endregion

        #region curve conf

        private static void GenerateCurveConf(string prefix, AnimationParam[] paramList) {
            UnityAction<Keyframe[], string, AnimationParam> action = (keyFrames, suffix, param) => {
                string frames = string.Empty;
                for (int i = 0; i < keyFrames.Length; i++) {
                    frames = string.Concat(frames, keyFrames[i].value, ",", keyFrames[i].time, ",",
                        keyFrames[i].inTangent, ",", keyFrames[i].outTangent);
                    if (i < keyFrames.Length - 1) {
                        frames = string.Concat(frames, "|");
                    }
                }
                curveWriter.WriteLine(string.Format(
                    "{0},\"{1}\",\"{2}\",\"{3}\"",
                    string.Concat(prefix, param.animationName.ToLower(), suffix),
                    frames,
                    string.Concat(param.startOffset.x, ",", param.startOffset.y, ",", param.startOffset.z),
                    string.Concat(param.targetOffset.x, ",", param.targetOffset.y, ",", param.targetOffset.z)
                ));
            };


            foreach (AnimationParam param in paramList) {
                string frames = string.Empty;
                Keyframe[] keyFrames = param.moveCurve.keys;
                if (keyFrames.Length > 0) {
                    action.Invoke(keyFrames, "_move", param);
                }

                keyFrames = param.moveXCurve.keys;
                if (keyFrames.Length > 0) {
                    action.Invoke(keyFrames, "_move_x", param);
                }

                keyFrames = param.moveYCurve.keys;
                if (keyFrames.Length > 0) {
                    action.Invoke(keyFrames, "_move_x", param);
                }

                keyFrames = param.moveZCurve.keys;
                if (keyFrames.Length > 0) {
                    action.Invoke(keyFrames, "_move_x", param);
                }

            }
        }



        #endregion
    }
}