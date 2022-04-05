using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Poukoute {
    [CustomEditor(typeof(BattleEffectView))]
    public class BattleEffectViewEditor : Editor {
        private BattleEffectView skillView;
        private SerializedProperty stepList;

        void OnEnable() {
            this.skillView = (BattleEffectView)target;
            this.stepList = this.serializedObject.FindProperty("stepList");
            AnimationCombo combo = this.skillView.GetComponent<AnimationCombo>();
            if (combo != null) {
                foreach (AnimationParam param in this.skillView.GetComponents<AnimationParam>()) {
                    combo.animationDict[param.animationName] = param;
                }
            }
        }

        public override void OnInspectorGUI() {
            bool hasAnimation = false;
            bool hasCombo = false;
            bool addStep = false;
            int addIndex = 0;
            bool removeStep = false;
            int removeIndex = 0;

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 100;
            this.skillView.effectType = (BattleEffectType)EditorGUILayout.EnumPopup(
               new GUIContent("Type"),
               this.skillView.effectType,
               GUILayout.Width(200)
            );

            
            SkinnedMeshRenderer tmpSkinnedMeshRenderer = 
                EditorGUILayout.ObjectField(
                    "Renderer",
                    this.skillView.roleRenderer, 
                    typeof(SkinnedMeshRenderer)
                ) as SkinnedMeshRenderer;
            if (tmpSkinnedMeshRenderer != null && this.skillView.roleRenderer == null) {
                this.skillView.roleRenderer = tmpSkinnedMeshRenderer;
                this.skillView.oldMaterials = new Material[this.skillView.roleRenderer.sharedMaterials.Length];
                this.skillView.roleRenderer.sharedMaterials.CopyTo(this.skillView.oldMaterials, 0);
            }
            EditorGUILayout.BeginHorizontal();
            stepList.arraySize = EditorGUILayout.IntField("List Size", stepList.arraySize);

            if (GUILayout.Button("+", GUILayout.Width(40))) {
                addStep = true;
                addIndex = 0;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            for (int i = 0; i < this.stepList.arraySize; i++) {
                if (addStep) {
                    break;
                }
                int step = i + 1;
                SerializedProperty listRef = this.stepList.GetArrayElementAtIndex(i);
                SerializedProperty stepType = listRef.FindPropertyRelative("type");
                SerializedProperty endDelay = listRef.FindPropertyRelative("endDelay");
                EditorGUIUtility.labelWidth = 120;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(
                    stepType,
                    new GUIContent("Step " + step),
                    GUILayout.Width(250)
                );

                if (GUILayout.Button("+", GUILayout.Width(40))) {
                    addStep = true;
                    addIndex = i + 1;
                    break;
                }

                if (GUILayout.Button("-", GUILayout.Width(40))) {
                    removeStep = true;
                    removeIndex = i;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                string stepTypeStr = ((BattleEffectStepType)stepType.enumValueIndex).ToString();
                if (stepType.enumValueIndex == (int)BattleEffectStepType.Move ||
                    stepType.enumValueIndex == (int)BattleEffectStepType.Fly ||
                    stepType.enumValueIndex == (int)BattleEffectStepType.Back) {
                    hasAnimation = false;
                    foreach (AnimationParam param in
                        this.skillView.GetComponents<AnimationParam>()) {
                        if (param == null) {
                            continue;
                        }
                        if (param.animationName == "Move" + step ||
                            param.animationName == "Fly" + step ||
                            param.animationName == "Back" + step) {
                            param.animationName = stepTypeStr + step;
                            hasAnimation = true;
                            break;
                        }
                    }
                    if (!hasAnimation) {
                        AnimationParam param =
                            this.skillView.gameObject.AddComponent<AnimationParam>();
                        param.isMoving = true;
                        param.isXYZSeperate = true;
                        Keyframe keyFrame1 = new Keyframe(0, 0, 0, 1);
                        Keyframe keyFrame2 = new Keyframe(1, 1, 1, 0);
                        AnimationCurve curve = new AnimationCurve(new Keyframe[] { keyFrame1, keyFrame2 });
                        param.moveCurve = curve;
                        param.animationName = stepTypeStr + step;
                        Debug.LogError(param.animationName);
                        this.skillView.GetComponent<AnimationCombo>().
                            animationDict.Add(param.animationName, param);
                    }
                } else {
                    hasCombo = false;
                    List<AnimationParam> removeParamList = new List<AnimationParam>();
                    foreach (AnimationParam param in
                        this.skillView.GetComponents<AnimationParam>()) {
                        if (param.animationName == "Move" + step ||
                            param.animationName == "Fly" + step ||
                            param.animationName == "Back" + step) {
                            removeParamList.Add(param);
                            this.skillView.GetComponent<AnimationCombo>().
                                animationDict.Remove(param.animationName);
                            hasCombo = true;
                            break;
                        }
                    }
                    foreach(AnimationParam param in removeParamList) {
                        DestroyImmediate(param, true);
                    }
                    if (hasCombo) {
                        if (this.skillView.GetComponents<AnimationParam>().Length == 0) {
                            AnimationCombo combo = this.skillView.GetComponent<AnimationCombo>();
                            if (combo != null) {
                                DestroyImmediate(combo, true);
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }

                if (stepType.enumValueIndex == (int)BattleEffectStepType.Attack ||
                    stepType.enumValueIndex == (int)BattleEffectStepType.Skill) {

                }

                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 140;


                if (stepType.enumValueIndex == (int)BattleEffectStepType.Last ||
                    (this.skillView.targetSplit && stepType.enumValueIndex == (int)BattleEffectStepType.Fly) ||
                    stepType.enumValueIndex == (int)BattleEffectStepType.Hit) {
                    EditorGUILayout.PropertyField(
                        listRef.FindPropertyRelative("target"),
                        new GUIContent("Target"),
                        GUILayout.Width(200)
                    );
                }

                if (stepType.enumValueIndex == (int)BattleEffectStepType.Hit) {
                    EditorGUILayout.PropertyField(
                        listRef.FindPropertyRelative("notShowDamage"),
                        new GUIContent("Not Show Damage"),
                        GUILayout.Width(200)
                    );
                }

                bool needEndDelay = true;
                if (stepType.enumValueIndex == (int)BattleEffectStepType.Fly) {
                    EditorGUILayout.PropertyField(
                        listRef.FindPropertyRelative("useCustomDelay"),
                        new GUIContent("Use Custom Delay"),
                        GUILayout.Width(200)
                    );
                    if (!listRef.FindPropertyRelative("useCustomDelay").boolValue) {
                        needEndDelay = false;
                    }
                }

                if (needEndDelay) {
                    EditorGUILayout.PropertyField(
                        endDelay,
                        new GUIContent("End Delay"),
                        GUILayout.Width(200)
                    );
                }

                if (stepType.enumValueIndex == (int)BattleEffectStepType.Cast) {
                    SerializedProperty rootList = listRef.FindPropertyRelative("rootList");
                    rootList.arraySize = EditorGUILayout.IntField(
                        new GUIContent("Root List"),
                        rootList.arraySize,
                        GUILayout.Width(200)
                    );
                    EditorGUI.indentLevel++;

                    for (int j = 0; j < rootList.arraySize; j++) {
                        SerializedProperty rootRef = rootList.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(rootRef);
                    }

                    EditorGUI.indentLevel--;
                }

                if (stepType.enumValueIndex == (int)BattleEffectStepType.Trans) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(listRef.FindPropertyRelative("matType"));
                    EditorGUILayout.PropertyField(listRef.FindPropertyRelative("mat"));
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(
                this.serializedObject.FindProperty("targetSplit"),
                new GUIContent("Split Target")
            );


            EditorGUI.indentLevel--;

            if (addStep) {
                this.stepList.InsertArrayElementAtIndex(addIndex);
                for (int i = this.stepList.arraySize - 1; i > addIndex; i--) {
                    SerializedProperty listRef = this.stepList.GetArrayElementAtIndex(i);
                    SerializedProperty stepType = listRef.FindPropertyRelative("type");
                    string stepTypeStr = ((BattleEffectStepType)stepType.enumValueIndex).ToString();
                    if (stepType.enumValueIndex == (int)BattleEffectStepType.Move ||
                        stepType.enumValueIndex == (int)BattleEffectStepType.Fly ||
                        stepType.enumValueIndex == (int)BattleEffectStepType.Back) {
                        AnimationCombo combo = this.skillView.GetComponent<AnimationCombo>();
                        string oldKey = stepTypeStr + i;
                        string newKey = stepTypeStr + (i + 1);
                        AnimationParam param = combo.animationDict[oldKey];
                        param.animationName = newKey;
                        combo.animationDict.Remove(oldKey);
                        Debug.LogError(newKey);
                        combo.animationDict.Add(newKey, param);
                    }
                }
            }

            if (removeStep) {
                this.stepList.DeleteArrayElementAtIndex(removeIndex);
                for (int i = removeIndex; i < this.stepList.arraySize; i++) {
                    SerializedProperty listRef = this.stepList.GetArrayElementAtIndex(i);
                    SerializedProperty stepType = listRef.FindPropertyRelative("type");
                    string stepTypeStr = ((BattleEffectStepType)stepType.enumValueIndex).ToString();
                    if (stepType.enumValueIndex == (int)BattleEffectStepType.Move ||
                        stepType.enumValueIndex == (int)BattleEffectStepType.Fly ||
                        stepType.enumValueIndex == (int)BattleEffectStepType.Back) {
                        AnimationCombo combo = this.skillView.GetComponent<AnimationCombo>();
                        string oldKey = stepTypeStr + (i + 2);
                        string newKey = stepTypeStr + (i + 1);
                        AnimationParam param = combo.animationDict[oldKey];
                        param.animationName = newKey;
                        combo.animationDict.Remove(oldKey);
                        combo.animationDict.Add(newKey, param);

                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this.skillView);
            if (hasCombo) {
                EditorGUIUtility.ExitGUI();
            }


        }
    }
}
