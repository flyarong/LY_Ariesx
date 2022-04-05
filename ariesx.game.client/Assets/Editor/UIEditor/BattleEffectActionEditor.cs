using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Poukoute {
    [CustomEditor(typeof(BattleEffectAction))]
    public class BattleEffectActionEditor : Editor {
        private BattleEffectAction skillAction;

        void OnEnable() {
            this.skillAction = (BattleEffectAction)target;
        }

        public override void OnInspectorGUI() {
            this.skillAction.particle = EditorGUILayout.ObjectField(
                this.skillAction.particle,
                typeof(ParticleSystem)
            ) as ParticleSystem;

            EditorGUILayout.LabelField(
                new GUIContent("duration"),
                new GUIContent(
                    string.Format("{0:0.00}", this.skillAction.duration)
                )
            );

            EditorGUILayout.Toggle(
                new GUIContent("Loop"),
                this.skillAction.loop
            );
        }
    }
}
