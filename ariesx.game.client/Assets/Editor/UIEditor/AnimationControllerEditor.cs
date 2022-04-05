using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Poukoute {
    public class AnimationItemEditor {
        public GameObject gameObject = null;
        public string animation = "";
        public int length;
        public List<AnimationItemEditor> next;
    }

    public class AnimationControllerEditor : EditorWindow {
        private List<AnimationItemEditor> editorList = new List<AnimationItemEditor>();
        private int length;
        private Animator animator;
        private string trigger;

        void OnGUI() {
            this.Format(this.editorList, ref this.length);
            EditorGUILayout.BeginHorizontal();
            this.animator = EditorGUILayout.ObjectField(this.animator, typeof(Animator), true) as Animator;
            this.trigger = EditorGUILayout.TextField(this.trigger);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Go")) {
                for (int i = 0; i < this.length; i++) {
                    this.Animate(this.editorList[i]);
                }
                if (this.animator != null) {
                    this.animator.SetTrigger(this.trigger);
                }
            }

            if (GUILayout.Button("Recover")) {
                for (int i = 0; i < this.length; i++) {
                    this.Recover(this.editorList[i]);
                }
            }
        }

        private void Format(List<AnimationItemEditor> itemList, ref int length) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Animation Length.");
            length = EditorGUILayout.IntField(length);
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < length; i++) {
                if (itemList.Count < i + 1) {
                    itemList.Add(new AnimationItemEditor());
                }
                EditorGUILayout.BeginHorizontal();
                itemList[i].gameObject =
                    EditorGUILayout.ObjectField(itemList[i].gameObject, typeof(GameObject), true) as GameObject;
                itemList[i].animation =
                EditorGUILayout.TextField(itemList[i].animation);
                EditorGUILayout.EndHorizontal();
                if (itemList[i].next != null) {
                    EditorGUI.indentLevel++;
                    this.Format(itemList[i].next, ref itemList[i].length);
                    EditorGUI.indentLevel--;
                } else {
                    if (GUILayout.Button("+")) {
                        itemList[i].next = new List<AnimationItemEditor>();
                        //this.Format(itemList[i].next, itemList[i].length);
                    }
                }
            }
        }

        private void Animate(AnimationItemEditor item) {
            if (item != null) {
                AnimationParam[] paramArray = item.gameObject.GetComponents<AnimationParam>();
                foreach (AnimationParam param in paramArray) {
                    if (param.animationName == item.animation) {
                        if (param.isOffset) {
                            AnimationManager.Animate(item.gameObject, item.animation, () => {
                                if (item.next != null) {
                                    for (int i = 0; i < item.length; i++)
                                        this.Animate(item.next[i]);
                                }
                            });
                        } else {
                            AnimationManager.Animate(item.gameObject, item.animation, 
                                Vector3.zero, Vector3.zero, () => {
                                if (item.next != null) {
                                    for (int i = 0; i < item.length; i++)
                                        this.Animate(item.next[i]);
                                }
                            });
                        }
                    }
                }
            }
        }

        private void Recover(AnimationItemEditor item) {
            AnimationParam[] paramArray = item.gameObject.GetComponents<AnimationParam>();
            foreach (AnimationParam param in paramArray) {
                if (param.animationName != item.animation) {
                    continue;
                }
                if (param.isMoving && param.animationName == "MoveOld1") {
                    item.gameObject.GetComponent<RectTransform>().anchoredPosition = param.startOffset;
                }
                if (param.isRotating && param.animationName != "Rotate2") {
                    item.gameObject.GetComponent<RectTransform>().eulerAngles = param.startAngle;
                }
                if (param.isScaling) {
                    item.gameObject.GetComponent<RectTransform>().localScale = param.startScale;
                }
            }
            if (item.next != null) {
                for (int i = 0; i < item.length; i++) {
                    this.Recover(item.next[i]);
                }
            }
        }
    }
}
