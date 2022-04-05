using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Poukoute { 
    public class BattleNewHeroGenerator : EditorWindow {
        private string heroes;

        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("New hero, split with \",\"");
            this.heroes = EditorGUILayout.TextField(heroes);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate")) {
                BattleAnimatorGenerator.GenerateNewHero(this.heroes);
            }
        }
    }
}

