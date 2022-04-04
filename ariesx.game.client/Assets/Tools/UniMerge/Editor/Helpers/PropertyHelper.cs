//Matt Schoen
//9-27-2015
//
// This software is the copyrighted material of its author, Matt Schoen, and his company Defective Studios.
// It is available for sale on the Unity Asset store and is subject to their restrictions and limitations, as well as
// the following: You shall not reproduce or re-distribute this software without the express written (e-mail is fine)
// permission of the author. If permission is granted, the code (this file and related files) must bear this license 
// in its entirety. Anyone who purchases the script is welcome to modify and re-use the code at their personal risk 
// and under the condition that it not be included in any distribution builds. The software is provided as-is without 
// warranty and the author bears no responsibility for damages or losses caused by the software.  
// This Agreement becomes effective from the day you have installed, copied, accessed, downloaded and/or otherwise used
// the software.

//UniMerge 1.8.5
//PropertyHelper class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace UniMerge.Editor.Helpers {
	public class PropertyHelper : Helper {
		public SerializedProperty mine { get; private set; }
		public SerializedProperty theirs { get; private set; }

		public List<PropertyHelper> children { get; private set; }

		public SerializedProperty property { get { return mine ?? theirs; } }

		readonly ObjectMerge objectMerge;
		readonly SceneMerge sceneMerge;

		readonly GameObjectHelper gameObjectParent;
		readonly ComponentHelper componentParent;
		readonly PropertyHelper propertyParent;

		readonly string propertyPath;

		GameObjectHelper root { get { return objectMerge ? objectMerge.root : null; } }

		//Local use only for GC
		readonly List<PropertyHelper> tmpList = new List<PropertyHelper>();

		public PropertyHelper(SerializedProperty mine, SerializedProperty theirs, string propertyPath,
			Helper parent = null, UniMergeWindow window = null) : base(parent, window) {
			this.mine = mine;
			this.theirs = theirs;
			this.propertyPath = propertyPath;

			objectMerge = window as ObjectMerge;
			sceneMerge = window as SceneMerge;

			gameObjectParent = parent as GameObjectHelper;
			componentParent = parent as ComponentHelper;
			propertyParent = parent as PropertyHelper;
		}

		public void CheckSame() {
			if (mine == null || theirs == null) {
				Same = false;
				return;
			}

			if (sceneMerge && !sceneMerge.compareLightingData) {
				if (propertyPath.Contains("lightingData") || propertyPath.Contains("lightmaps")) {
					Same = true;
					return;
				}
			}

			GameObject myRoot = null;
			GameObject theirRoot = null;
			if (objectMerge) {
				var root = objectMerge.root;
				myRoot = root.mine;
				theirRoot = root.theirs;
			}

#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
			if (property.propertyType == SerializedPropertyType.ArraySize) {
				var propParent = (PropertyHelper) parent;
				Same = propParent.mine.arraySize == propParent.theirs.arraySize;
				return;
			}
#endif
			Same = Util.PropEqual(mine, theirs, myRoot, theirRoot);
		}

		public IEnumerator CheckSameRecursively() {
			CheckSame();

			if (children != null) {
				foreach (var child in children) {
					var e = child.CheckSameRecursively();
					while (e.MoveNext())
						yield return null;

					if (!child.Same)
						Same = child.Same;
				}
			}
		}

		public SerializedProperty GetProperty(bool isMine) { return isMine ? mine : theirs; }

		public void Draw(float indent, float colWidth) {
			if (window.drawAbort)
				return;

			if (!window || window.ScrollCheck()) {
				window.StartRow(Same);

				DrawProperty(true, indent, colWidth);

				//Swap buttons
				var hasBothParents = gameObjectParent != null && gameObjectParent.mine && gameObjectParent.theirs
					|| componentParent != null && componentParent.mine && componentParent.theirs
					|| propertyParent != null && propertyParent.mine != null && propertyParent.theirs != null
					|| mine != null && theirs != null && mine.depth == 0;
				if (hasBothParents)
					DrawMidButtons(mine != null, theirs != null,
						//Copy theirs to mine
						() => {
							Copy(true);
							BubbleRefresh();
							//Copy mine to theirs
						}, () => {
							Copy(false);
							BubbleRefresh();
						},
						//Delete should only occur for arrays
						//Delete mine
						() => {
							if (propertyParent.children != null) {
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
								var index = propertyParent.children.IndexOf(this) - 1; // Index - 1 because of array size
								propertyParent.mine.DeleteArrayElementAtIndex(index);
#endif
								propertyParent.children.Clear(); //Clear PropertyHelpers whenever we remove array elements
							}

							BubbleRefresh();
						},

						//Delete theirs
						() => {
							if (propertyParent.children != null) {
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
								var index = propertyParent.children.IndexOf(this) - 1; // Index - 1 because of array size
								propertyParent.theirs.DeleteArrayElementAtIndex(index);
#endif
								if (propertyParent.children != null)
									propertyParent.children.Clear(); //Clear PropertyHelpers whenever we remove array elements
							}

							BubbleRefresh();
						});
				else
					GUILayout.Space(UniMergeConfig.midWidth * 2);

				DrawProperty(false, indent, colWidth);

				window.EndRow();
			}

			if (showChildren && children != null) {
				tmpList.Clear();
				tmpList.AddRange(children);
				foreach (var property in tmpList)
					property.Draw(indent + Util.tabSize, colWidth);
			}
		}

		public void Copy(bool toMine) {
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
			if (property.propertyType == SerializedPropertyType.ArraySize) {
				var propParent = (PropertyHelper) parent;
				var leftArray = propParent.GetProperty(toMine);
				var rightArray = propParent.GetProperty(!toMine);
				leftArray.arraySize = rightArray.arraySize;

				if (propParent.children != null)
					propParent.children.Clear(); //Clear PropertyHelpers whenever we remove array elements
				return;
			}
#endif

			var left = GetProperty(toMine);
			var right = GetProperty(!toMine);

#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
			if (left == null) {
				//Insert should only occur for arrays
				var propParent = (PropertyHelper) parent;
				var array = propParent.GetProperty(toMine);
				var count = array.arraySize;
				array.InsertArrayElementAtIndex(count);
				Util.SetProperty(right, array.GetArrayElementAtIndex(count));
				return;
			}
#endif

			if (left.propertyType == SerializedPropertyType.ObjectReference)
				if (right.objectReferenceValue != null) {
					var t = right.objectReferenceValue.GetType();
					if (window)
						if (t == typeof(GameObject)) {
							var g = root.GetObjectSpouse((GameObject) right.objectReferenceValue, true);
							left.objectReferenceValue = g ?? right.objectReferenceValue;
						} else if (t.IsSubclassOf(typeof(Component))) {
							var g = root.GetObjectSpouse(((Component) right.objectReferenceValue).gameObject, true);
							if (g) {
								var c = g.GetComponent(t);
								left.objectReferenceValue = c ? c : g.AddComponent(t);
							} else { left.objectReferenceValue = right.objectReferenceValue; }
						} else { left.objectReferenceValue = right.objectReferenceValue; }
					else
						left.objectReferenceValue = right.objectReferenceValue;

					if (left.serializedObject.targetObject != null)
						left.serializedObject.ApplyModifiedProperties();

					return;
				}

#if Unity3
			Util.SetProperty(right, left);
#else
			left.serializedObject.CopyFromSerializedProperty(right);
#endif

			if (left.serializedObject.targetObject != null)
				left.serializedObject.ApplyModifiedProperties();
		}

		void DrawProperty(bool isMine, float indent, float colWidth) {
			if (window.drawAbort)
				return;

			GUILayout.BeginVertical(GUILayout.Width(colWidth + UniMergeWindow.columnPadding));
			var property = GetProperty(isMine);
			if (property != null) {
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
				EditorGUI.BeginChangeCheck();
#endif

				var propertyType = property.propertyType;
				switch (propertyType) {
#if UNITY_4_5 || UNITY_4_5_0 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
					case SerializedPropertyType.Quaternion:
					case SerializedPropertyType.Vector4:
#elif !Unity3
					case (SerializedPropertyType) 16:
#endif
					case SerializedPropertyType.Vector2:
					case SerializedPropertyType.Vector3:
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3)
					case SerializedPropertyType.Bounds:
#endif
					case SerializedPropertyType.Generic:
					case SerializedPropertyType.Rect:
						Util.Indent(indent, () => {
#if UNITY_5_5_OR_NEWER
							showChildren = EditorGUILayout.Foldout(showChildren, ObjectNames.NicifyVariableName(property.name), true);
#else
							showChildren = EditorGUILayout.Foldout(showChildren, ObjectNames.NicifyVariableName(property.name));
#endif
						});
						break;
					default:
						if (children != null && children.Count > 0)
							Util.Indent(indent + Util.tabSize, () => {
								GUILayout.BeginHorizontal();
#if Unity3
								EditorGUILayout.PropertyField(property);
#else
								EditorGUILayout.PropertyField(property, false);
#endif
								var lastRect = GUILayoutUtility.GetLastRect();
								lastRect.xMin -= Util.tabSize;
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
								showChildren = EditorGUI.Foldout(lastRect, showChildren, " ", true);
#else
								showChildren = EditorGUI.Foldout(lastRect, showChildren, " ");
#endif
								GUILayout.EndHorizontal();
							});
						else
							Util.Indent(indent, () => {
#if Unity3
								EditorGUILayout.PropertyField(property);
#else
								EditorGUILayout.PropertyField(property, false);
#endif
							});
						break;
				}

#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
				if (EditorGUI.EndChangeCheck()) {
#endif
					if (property.serializedObject != null && property.serializedObject.targetObject != null) {
						if (property.serializedObject.ApplyModifiedProperties()) {
							if (propertyType == SerializedPropertyType.ArraySize) {
								var phParent = (PropertyHelper) parent;
								if (phParent.children != null)
									phParent.children.Clear();
							}

							BubbleRefresh();
						}
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
					}
#endif
				}
			} else {
				GUILayout.Label("");
				GUILayout.Space(EmptyRowSpace);
			}
			GUILayout.EndVertical();
		}

		public static void GetProperties(List<SerializedProperty> properties, SerializedObject obj) {
			var iterator = obj.GetIterator();
			while (iterator.Next(true))
				properties.Add(iterator.Copy());
		}

		public static IEnumerator<bool> UpdatePropertyList(List<PropertyHelper> properties, SerializedObject myObject,
			SerializedObject theirObject, Helper parent = null, UniMergeWindow window = null, bool showHidden = false) {
			if (myObject == null && theirObject == null)
				yield break;

			SerializedProperty myIterator = null;
			if (myObject != null)
				myIterator = myObject.GetIterator();

			SerializedProperty theirIterator = null;
			if (theirObject != null)
				theirIterator = theirObject.GetIterator();

			var isGameObject = (myObject == null ? theirObject.targetObject : myObject.targetObject) is GameObject;
			var isTransform = (myObject == null ? theirObject.targetObject : myObject.targetObject) is Transform;
			var tempShowHiddenDepth = -1;
			var tempShowHidden = false;
			var same = true;
			var mineHasNext = myIterator != null;
			var theirsHasNext = theirIterator != null;
			var lastDepth = 0;
			PropertyHelper lastHelper = null;
			var root = parent;
			var ignored = false;
			while (mineHasNext || theirsHasNext) {
				var _myIterator = myIterator;
				var _theirIterator = theirIterator;
				var iterator = _myIterator != null && mineHasNext ? _myIterator : _theirIterator;

#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
				if (iterator.propertyType == SerializedPropertyType.Gradient) {
					tempShowHiddenDepth = iterator.depth;
					tempShowHidden = true;
				} else
#endif
				if (iterator.depth == tempShowHiddenDepth) {
					tempShowHidden = false;
					tempShowHiddenDepth = -1;
				}

				if (mineHasNext && theirsHasNext) {
					if (myIterator.depth > theirIterator.depth) {
						//Catch up myIterator
						if (showHidden || tempShowHidden)
							mineHasNext &= myIterator.Next(!ignored);
						else
							mineHasNext &= myIterator.NextVisible(!ignored);
					} else if (theirIterator.depth > myIterator.depth && theirsHasNext) {
						// Catch up theirIterator
						if (showHidden || tempShowHidden)
							theirsHasNext &= theirIterator.Next(!ignored);
						else
							theirsHasNext &= theirIterator.NextVisible(!ignored);
					} else {
						if (showHidden || tempShowHidden) {
							mineHasNext &= myIterator.Next(!ignored);
							theirsHasNext &= theirIterator.Next(!ignored);
						} else {
							mineHasNext &= myIterator.NextVisible(!ignored);
							theirsHasNext &= theirIterator.NextVisible(!ignored);
						}
					}

					if (mineHasNext && theirsHasNext) {
						if (myIterator.depth > theirIterator.depth) // Missing elements in mine
							_theirIterator = null;

						if (theirIterator.depth > myIterator.depth) // Missing elements in theirs
							_myIterator = null;
					}
				} else {
					if (mineHasNext)
						if (showHidden || tempShowHidden)
							mineHasNext &= myIterator.Next(!ignored);
						else
							mineHasNext &= myIterator.NextVisible(!ignored);

					if (theirsHasNext)
						if (showHidden || tempShowHidden)
							theirsHasNext &= theirIterator.Next(!ignored);
						else
							theirsHasNext &= theirIterator.NextVisible(!ignored);
				}

				if (!mineHasNext && !theirsHasNext)
					break;

				if (!mineHasNext)
					_myIterator = null;

				if (!theirsHasNext)
					_theirIterator = null;

				// Get new iterator if one has become null
				iterator = _myIterator ?? _theirIterator;

				var propertyPath = iterator.propertyPath;
				ignored = propertyPath == "m_Script";

				if (isGameObject) {
					var type = iterator.propertyType;
					ignored = type == SerializedPropertyType.ObjectReference || type == SerializedPropertyType.Generic;
				} else if (isTransform) {
					var type = iterator.propertyType;

#if UNITY_4_5 || UNITY_4_5_0 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
					ignored = type != SerializedPropertyType.Vector3 && type != SerializedPropertyType.Quaternion && type != SerializedPropertyType.Float;
#elif !Unity3
					ignored = type != SerializedPropertyType.Vector3 && type != (SerializedPropertyType)16 && type != SerializedPropertyType.Float;
#else
					ignored = type != SerializedPropertyType.Vector3 && type != SerializedPropertyType.Float;
#endif
				}

				if (ignored)
					continue;

				var ph = properties.Find(helper => helper.propertyPath == propertyPath);

				if (iterator.depth > lastDepth)
					parent = lastHelper;

				if (iterator.depth < lastDepth && parent != null)
					parent = parent.parent;

				if (iterator.depth > 0) {
					var phParent = (PropertyHelper) parent;
					if (phParent.children != null)
						ph = phParent.children.Find(helper => helper.propertyPath == propertyPath);
				}

				SerializedProperty myIteratorCopy = null;
				if (_myIterator != null)
					myIteratorCopy = _myIterator.Copy();

				SerializedProperty theirIteratorCopy = null;
				if (_theirIterator != null)
					theirIteratorCopy = _theirIterator.Copy();

				if (ph == null) {
					ph = new PropertyHelper(myIteratorCopy, theirIteratorCopy, propertyPath,
						iterator.depth == 0 ? root : parent, window);

					if (iterator.depth == 0) {
						properties.Add(ph);
					} else {
						var phParent = (PropertyHelper) parent;
						if (phParent.children == null)
							phParent.children = new List<PropertyHelper>(1);

						phParent.children.Add(ph); 
						
					}
				} else {
					ph.mine = myIteratorCopy;
					ph.theirs = theirIteratorCopy;
				}

				lastHelper = ph;
				lastDepth = iterator.depth;
			}

			foreach (var property in properties) {
				if (property.children == null) {
					property.CheckSame();
				} else {
					var enumerator = property.CheckSameRecursively();
					while (enumerator.MoveNext())
						yield return false;
				}

				if (!property.Same)
					same = false;
			}

			yield return same;
		}

		public override IEnumerator Refresh() {
			if (children == null) {
				CheckSame();
			} else {
				var enumerator = CheckSameRecursively();
				while (enumerator.MoveNext())
					yield return null;
			}
		}

		public override int GetDrawCount() {
			var count = 1;
			if (showChildren && children != null)
				count += children.Sum(child => child.GetDrawCount());

			return count;
		}

		public void ToList(List<PropertyHelper> list) {
			list.Add(this);
			if (children != null)
				for (var i = 0; i < children.Count; i++)
					children[i].ToList(list);
		}
	}
}
