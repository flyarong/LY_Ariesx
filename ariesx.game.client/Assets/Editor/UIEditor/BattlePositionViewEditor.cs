using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Poukoute {
    [CustomEditor(typeof(BattlePositionView))]
    public class BattlePositionViewEditor : Editor {
        private BattlePositionView positionView;


        void OnEnable() {
            this.positionView = (BattlePositionView)target;
        }

        public override void OnInspectorGUI() {
     
            string name = positionView.gameObject.name;
            string heroId = name.Substring(name.IndexOf('_') + 1);
            EditorGUILayout.LabelField(
                 new GUIContent("Hero ID"),
                 new GUIContent(
                     heroId
                 )
             );
            if (GUILayout.Button("Refresh Role")) {
                this.RefreshRole(heroId);
            }

            if (GUILayout.Button("Refresh Effect")) {
                this.RefreshEffect(heroId);
            }
        }

        private void RefreshRole(string heroId) {
            BattleAnimatorGenerator.RefreshHeroRole(heroId);
        }

        private void RefreshEffect(string heroId) {
            BattleAnimatorGenerator.GenerateNewStep(heroId);
        }
    }
}
