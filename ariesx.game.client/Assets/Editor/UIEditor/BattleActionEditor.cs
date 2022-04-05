using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Poukoute {
    [CustomEditor(typeof(BattleAction))]
    public class BattleActionEditor : Editor {
        private BattleAction actionView;
        private SerializedProperty stepList;

        void OnEnable() {
            this.actionView = (BattleAction)target;
            this.stepList = this.serializedObject.FindProperty("stepList");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUIUtility.labelWidth = 100;

            Animator animator = actionView.GetComponent<Animator>();

            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            AnimatorStateMachine sm = ac.layers[0].stateMachine;
            for (int i = 0; i < sm.states.Length; i++) {
                AnimationClip clip = sm.states[i].state.motion as AnimationClip;
                string stateName = sm.states[i].state.name;

                if (!stateName.Equals("Failed") && !stateName.Equals("Victor")) {
                    continue;
                }

                if (clip != null && clip.events.Length > 0) {
                    EditorGUILayout.LabelField(
                        new GUIContent(stateName)
                    );
                    EditorGUI.indentLevel++;
                    List<UnityEngine.AnimationEvent> eventList = 
                        new List<UnityEngine.AnimationEvent>(clip.events);
                    for (int j = 0; j < eventList.Count; j++) {
                        eventList[j].time = Mathf.RoundToInt(EditorGUILayout.Slider(
                           new GUIContent(eventList[j].functionName),
                           eventList[j].time, 0, clip.length
                        ) / (1 / clip.frameRate)) * (1 / clip.frameRate);
                    }
                    eventList.Sort((a1, a2) => {
                        return a1.time.CompareTo(a2.time);
                    });

                    AnimationUtility.SetAnimationEvents(clip, eventList.ToArray());
                    EditorGUI.indentLevel--;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
