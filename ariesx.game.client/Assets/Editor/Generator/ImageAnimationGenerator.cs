using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Poukoute {
    public class ImageAnimationGenerator : EditorWindow {
        private string animationName;
        private string imageName;
        

        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Animation Name");
            this.animationName = EditorGUILayout.TextField(this.animationName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Image Name");
            this.imageName = EditorGUILayout.TextField(this.imageName);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Generate")) {
                this.Generate();
            }
        }

        private void Generate() {
            GameObject obj = Selection.gameObjects[0];
            if (obj == null) {
                Debug.LogError("Select some game object.");
                return;
            }
            Animator animator = obj.GetComponent<Animator>();
            if (animator == null) {
                Debug.LogError("There is no animator in selected game object.");
                return;
            }
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            AnimationClip[] animationClips = ac.animationClips;
            foreach (AnimationClip clip in animationClips) {
                Debug.LogError(clip.name);
                if (clip.name == this.animationName) {
                    this.SetAnimationClip(animator);
                    return;
                }
            }
            Debug.LogErrorf("There is no animation named {0}", this.animationName);
        }

        private void SetAnimationClip(Animator animator) {
            
          //  AnimationCurve curve = new AnimationCurve();

            Sprite[] SpriteArray = Resources.LoadAll<Sprite>(Path.image + this.imageName);
            int count = 0;
            animator.StartRecording(-1);
            foreach (Sprite sprite in SpriteArray) {
                animator.GetComponent<Image>().sprite = sprite;
                count++;
               
            }
         //   animator.StopRecording();
            // //clip.SetCurve("", typeof(Sprite), "sprite", )
            // clip.
            //  AnimationClipSettings setting = new AnimationClipSettings();
            ////      setting.
        }
    }
}
