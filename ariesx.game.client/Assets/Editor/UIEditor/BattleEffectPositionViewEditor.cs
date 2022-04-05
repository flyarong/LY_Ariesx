using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Poukoute {
    [CustomEditor(typeof(BattleEffectPositionView))]
    public class BattleEffectPositionViewEditor : Editor {
        private BattleEffectPositionView positionView;
        
        void OnEnable() {
            this.positionView = (BattleEffectPositionView)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            this.positionView.Root = (Transform)EditorGUILayout.ObjectField(
                new GUIContent("Root"),
                this.positionView.effectRoot,
                typeof(Transform),
                true
            );

         //   EditorGUILayout.PropertyField(base.serializedObject.FindProperty("effectList"));

            if (this.positionView.gameObject.name == "Idle") {
                this.positionView.victorStopDelay = EditorGUILayout.FloatField(
                    new GUIContent("Win Delay"),
                    this.positionView.victorStopDelay
                );

                this.positionView.victorStopDelay = EditorGUILayout.FloatField(
                   new GUIContent("Lose Delay"),
                   this.positionView.failedStopDelay
               );
            }
        }

    }
}
