using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;

namespace Poukoute {

    public class MapAnimatorGenerator : MonoBehaviour {
        private static GameObject configureManager;
        private const string heroAnimatorPath =
            "Assets/Arts/Animators/Map/Heroes";
        private const string heroPath =
            "/Arts/Models/Battles/Heroes/";
        private const string heroPrefabPath =
            "Assets/Resources/Map/Heroes/";

        private const string MOVE_DOWN = "MoveDown";
        private const string MOVE_UP = "MoveUp";
        private const string MOVE_RIGHT = "MoveRight";
        private const string IDLE = "idle";

        private const float heroRate = 20f;

        private static Dictionary<string, AnimationClip> defaultDict;

        [MenuItem("Poukoute/Generator/Generate Animator/Map")]
        private static void GeneratorRoleAnimators() {
            try {
                InitPrefabConf();
                GenerateRoleAnimator();
            } finally {
                UninitPrefabConf();
            }
        }

        #region role

        private static void GenerateRoleAnimator() {
            foreach (string key in HeroBattleConf.modelDict.Keys) {
                //AnimatorController acR = GenerateRoleAnimator(pair.Value, pair.Key, "r");
                GenerateRolePrefab(key, "r");
            }
        }

        private static void InitPrefabConf() {
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

        //private static AnimatorController GenerateRoleAnimator(string heroName,
        //    string heroPath, string direction) {
        //    string fullPath = string.Format("{0}/{1}_{2}.controller",
        //        heroAnimatorPath, heroName, direction);
        //    string absPath = Application.dataPath + "/" + fullPath;
        //    if (File.Exists(absPath)) {
        //        AssetDatabase.LoadAssetAtPath<AnimatorController>(string.Format("Assets/{0}", fullPath));
        //    }
        //    AnimationClip idleClip = GenerateRoleAnimation(heroPath, IDLE, direction);

        //    AnimationClip moveDownClip = GenerateRoleAnimation(heroPath, "move", "f");
        //    AnimationClip moveUpClip = GenerateRoleAnimation(heroPath, "move", "b");
        //    AnimationClip moveRightClip = GenerateRoleAnimation(heroPath, "move", "r");
        //    AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(fullPath);
        //    AnimatorStateMachine rootStateMachine = ac.layers[0].stateMachine;

        //    AnimatorState idle = rootStateMachine.AddState("Idle");
        //    ac.AddParameter("Reset", AnimatorControllerParameterType.Trigger);
        //    idle.motion = idleClip;
        //    CreateRoleState(ac, MOVE_DOWN, rootStateMachine,
        //        GameHelper.UpperFirstCase(MOVE_DOWN), idle, moveDownClip);
        //    CreateRoleState(ac, MOVE_UP, rootStateMachine,
        //        GameHelper.UpperFirstCase(MOVE_UP), idle, moveUpClip);
        //    CreateRoleState(ac, MOVE_RIGHT, rootStateMachine,
        //        GameHelper.UpperFirstCase(MOVE_RIGHT), idle, moveRightClip);
        //    return ac;
        //}

        private static void CreateRoleState(AnimatorController ac, string animation,
            AnimatorStateMachine root, string triggerName, AnimatorState idle, AnimationClip clip) {
            AnimatorState state = root.AddState(triggerName);
            state.motion = clip;

            AnimatorStateTransition enterTrans = root.AddAnyStateTransition(state);
            AnimatorStateTransition exitTrans = state.AddExitTransition();
            ac.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);
            enterTrans.AddCondition(AnimatorConditionMode.If, 0, triggerName);

            exitTrans.AddCondition(AnimatorConditionMode.If, 0, "Reset");
            exitTrans.destinationState = idle;
        }

        private static void GenerateRolePrefab(string heroName, string direction) {
            string fullPath = string.Format("{0}/map_{1}.controller", heroAnimatorPath, heroName);
            string absPath = Application.dataPath + "/" + fullPath;
            AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(fullPath);
            AnimatorStateMachine sm = ac.layers[0].stateMachine;
            AnimatorState state = sm.AddState("Move");

            string animPath = string.Format("Assets{0}{1}/{1}_{2}.anim",
              heroPath, heroName, "move");
            AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            state.motion = animClip;

            string folder = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            string assetPath = string.Format("{0}move_{1}.prefab",
                heroPrefabPath, heroName);
            GameObject newPrefab = new GameObject();
            string rolePath = string.Format(
                "Assets{0}{1}/{1}.prefab",
                heroPath,
                heroName
            );
            string moveRolePath = string.Format(
                "Assets{0}{1}/move_{1}.prefab",
                heroPath,
                heroName
            );
            GameObject hero = AssetDatabase.LoadAssetAtPath<GameObject>(rolePath);
            GameObject moveHero = null;
            try {
                moveHero = Instantiate(
                   PrefabUtility.CreatePrefab(moveRolePath, hero)
               );
            } catch {
                Debug.LogError(rolePath);
                return;
            }
            GameHelper.SetLayer(moveHero, LayerMask.NameToLayer("LayerMarchRole"));
            moveHero.GetComponent<Animator>().
                runtimeAnimatorController = ac;
            moveHero.transform.SetParent(newPrefab.transform);
            moveHero.transform.localPosition = Vector3.zero;
            moveHero.name = "Hero";
            GameObject heroPrefab = PrefabUtility.CreatePrefab(assetPath, newPrefab) as GameObject;
            DestroyImmediate(newPrefab);

        }

        #endregion

    }
}
