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
//ObjectHelper class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Helpers {
	public class GameObjectHelper : Helper {
		const int ComponentsMargin = 10;
		const int FoldoutPadding = 4;
#if Unity3
		const int ObjectPadding = -1;
#else
		const int ObjectPadding = -3;
#endif

		public readonly List<ComponentHelper> components = new List<ComponentHelper>(1);
		public readonly List<PropertyHelper> attributes = new List<PropertyHelper>(7);
		public List<GameObjectHelper> children { get; private set; }

		ObjectContainer objectContainer;

		public GameObject mine {
			get { return objectContainer.mine; }
			set {
				if (objectContainer == null)
					objectContainer = ObjectContainer.Create();

				if (objectContainer.mine == value)
					return;

				if (!UniMergeWindow.blockRefresh && window.root == this)
					window.update = window.Refresh();

				var mineProp = new SerializedObject(objectContainer).FindProperty("_mine");
				mineProp.objectReferenceValue = value;
				mineProp.serializedObject.ApplyModifiedProperties();
			}
		}
		public GameObject theirs {
			get { return objectContainer.theirs; }
			set {
				if (objectContainer == null)
					objectContainer = ScriptableObject.CreateInstance<ObjectContainer>();

				if (objectContainer.theirs == value)
					return;

				if (!UniMergeWindow.blockRefresh && window.root == this)
					window.update = window.Refresh();

				var theirsProp = new SerializedObject(objectContainer).FindProperty("_theirs");
				theirsProp.objectReferenceValue = value;
				theirsProp.serializedObject.ApplyModifiedProperties();
			}
		}

		bool sameAttrs, showComponents, showAttrs;

		new readonly GameObjectHelper parent; // GameObjectHelpers can only be children of each other
		new readonly ObjectMerge window; // GameObjectHelpers can only be in ObjectMerge windows

		SerializedObject mySO, theirSO;

		public GameObjectHelper(ObjectMerge window, GameObjectHelper parent = null) : base(parent, window) {
			this.parent = parent;
			this.window = window;
		}

		//Local use only for GC. Might break during multiple simultaneous refresh because of shared static instance
		//static readonly List<PropertyHelper> Properties = new List<PropertyHelper>();
		static readonly List<GameObject> MyChildren = new List<GameObject>();
		static readonly List<GameObject> TheirChildren = new List<GameObject>();
		static readonly List<Component> MyComponents = new List<Component>();
		static readonly List<Component> TheirComponents = new List<Component>();
		readonly List<GameObjectHelper> tmpList = new List<GameObjectHelper>();

		public override IEnumerator Refresh() {
			if (mine) {
				var mineList = new List<GameObject>();
				Util.GameObjectToList(mine, mineList);
				window.totalUpdateNum = mineList.Count;
			}

			if (theirs) {
				var theirsList = new List<GameObject>();
				Util.GameObjectToList(theirs, theirsList);
				window.totalUpdateNum += theirsList.Count;
			}

			window.updateCount = 0;
			var enumerator = DoRefresh();
			while (enumerator.MoveNext())
				yield return null;
		}

		/// <summary>
		///     Refresh is a very crucial function.  Not only does it refresh the abstracted lists of ObjectHelpers and
		///     ComponentHelpers to reflect the actual scene,
		///     it is responsible for actually comparing objects
		/// </summary>
		/// <returns>IEnumerator for coroutine progress</returns>
		public IEnumerator DoRefresh() {
			Same = true;
			MyChildren.Clear();
			TheirChildren.Clear();
			MyComponents.Clear();
			TheirComponents.Clear();
			//Get lists of components and children
			if (mine) {
				window.updateCount++;
				MyChildren.AddRange(from Transform t in mine.transform select t.gameObject);
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
				mine.GetComponents(MyComponents);
#else
				MyComponents.AddRange(mine.GetComponents<Component>());
#endif
			}
			if (theirs) {
				window.updateCount++;
				TheirChildren.AddRange(from Transform t in theirs.transform select t.gameObject);
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
				theirs.GetComponents(TheirComponents);
#else
				TheirComponents.AddRange(theirs.GetComponents<Component>());
#endif
			}

			// Clear empty components
			components.RemoveAll(helper => helper.mine == null && helper.theirs == null);

			//TODO: turn these two chunks into one function... somehow
			//Merge Components
			ComponentHelper ch;
			for (var i = 0; i < MyComponents.Count; i++) {
				var component = MyComponents[i];
				// Missing scripts show up as null
				if (component == null)
					continue;

				var match = TheirComponents.Where(g => g != null).FirstOrDefault(g => component.GetType() == g.GetType());

				ch = components.Find(helper => helper.mine == component || match != null && helper.theirs == match);

				if (ch == null) {
					ch = new ComponentHelper(component, match, this, window);
					components.Add(ch);
				} else {
					ch.mine = component;
					ch.theirs = match;
				}

				var enumerator = ch.Refresh();
				while (enumerator.MoveNext())
					yield return null;

				if (!ComponentIsFiltered(ch.type) && !ch.Same)
					Same = false;

				TheirComponents.Remove(match);
			}

			if (TheirComponents.Count > 0) {
				foreach (var g in TheirComponents) {
					// Missing scripts show up as null
					if (g == null)
						continue;

					ch = components.Find(helper => helper.theirs == g);

					if (ch == null) {
						ch = new ComponentHelper(null, g, this, window);
						var enumerator = ch.Refresh();
						while (enumerator.MoveNext())
							yield return null;

						components.Add(ch);
					}

					if (!ComponentIsFiltered(ch.type) && !ch.Same)
						Same = false;
				}
			}

			// Clear empty components
			if (children != null)
				children.RemoveAll(helper => helper.mine == null && helper.theirs == null);

			//Merge Children
			GameObjectHelper oh = null;
			foreach (var child in MyChildren) {
				var match = TheirChildren.FirstOrDefault(g => SameObject(child, g));

				if (children != null)
					oh = children.Find(helper => helper.mine == child || match != null && helper.theirs == match);

				if (oh == null) {
					oh = new GameObjectHelper(window, this) { mine = child, theirs = match };

					if (children == null)
						children = new List<GameObjectHelper>();

					children.Add(oh);
				} else {
					oh.mine = child;
					oh.theirs = match;
				}
				TheirChildren.Remove(match);
			}

			if (TheirChildren.Count > 0) {
				Same = false;
				foreach (var g in TheirChildren) {
					if (children != null)
						oh = children.Find(helper => helper.theirs == g);

					if (oh == null) {
						if (children == null)
							children = new List<GameObjectHelper>();

						children.Add(new GameObjectHelper(window, this) { theirs = g });
					}
				}
			}

			tmpList.Clear();
			if (children != null) {
				tmpList.AddRange(children);
				foreach (var obj in tmpList)
					if (obj.mine == null && obj.theirs == null)
						children.Remove(obj);

				children.Sort(delegate(GameObjectHelper a, GameObjectHelper b) {
					if (a.mine && b.mine)
						return a.mine.name.CompareTo(b.mine.name);
					if (a.mine && b.theirs)
						return a.mine.name.CompareTo(b.theirs.name);
					if (a.theirs && b.mine)
						return a.theirs.name.CompareTo(b.mine.name);
					if (a.theirs && b.theirs)
						return a.theirs.name.CompareTo(b.theirs.name);
					return 0;
				});

				tmpList.Clear();
				tmpList.AddRange(children);
				foreach (var child in tmpList) {
					var enumerator = child.DoRefresh();
					while (enumerator.MoveNext())
						yield return null;
					if (!child.Same)
						Same = false;
				}
			}

			if (mine)
				mySO = new SerializedObject(mine);

			if (theirs)
				theirSO = new SerializedObject(theirs);

			var e = PropertyHelper.UpdatePropertyList(attributes, mySO, theirSO, this, window, true);
			while (e.MoveNext())
				yield return null;

			sameAttrs = true;
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < attributes.Count; i++) {
				var attribute = attributes[i];
				if (!attribute.Same)
					sameAttrs = false;
			}

			if (!sameAttrs && window.compareAttrs)
				Same = false;
		}

		bool ComponentIsFiltered(Type type) {
#if Unity3 //TODO: Better U3 filtering
			for(int i = 0; i < ObjectMerge.filterTypes.Count; i++) {
				if(type == ObjectMerge.filterTypes[i])
					return true;
			}
#else
			for (var i = 0; i < ObjectMerge.componentTypes.Length; i++) {
				if (window.typeMask[i] == -1) //This has everything, continue
					continue;
				var idx = ArrayUtility.IndexOf(ObjectMerge.componentTypes[i], type);
				if (idx != -1)
					return ((window.typeMask[i] >> idx) & 1) == 0;
			}
#endif
			return false; //Assume not filtered
		}

		/// <summary>
		/// Draw the row for this GameObject
		/// </summary>
		/// <param name="colWidth"></param>
		/// <param name="indent"></param>
		public void Draw(float colWidth, float indent = 0) {
			if (window.drawAbort)
				return;

			var draw = window.ScrollCheck();
			if (draw) {
				//This object
				window.StartRow(Same);
				//Display mine
				GUILayout.BeginVertical();
				GUILayout.Space(ObjectPadding);

				DrawObject(true, indent, colWidth);

				GUILayout.EndVertical();

				var isRoot = window.root == this;
				//Swap buttons
				DrawMidButtons(mine, theirs, isRoot || parent != null && parent.mine, isRoot || parent != null && parent.theirs,
					// < button
					delegate {
						//NB: This still throws a SerializedProperty error (at least in Unity 3) gonna have to do a bit more poking.
						window.update = Copy(true);
						// > button
					}, delegate {
						window.update = Copy(false);
						// Left X button
					}, delegate {
						window.updateType = RefreshType.Deleting;
						window.update = Delete(true);
						// Right X button
					}, delegate {
						window.updateType = RefreshType.Deleting;
						window.update = Delete(false);
					});
				//Display theirs
				GUILayout.BeginVertical();
				GUILayout.Space(ObjectPadding);
				GUILayout.BeginHorizontal();

				DrawObject(false, indent, colWidth);

				if (GUILayout.Button(showComponents ? "-" : "+", GUILayout.Width(19))) {
					//Positioning on this button is super screwy
					showComponents = !showComponents;
					if (Event.current.alt) {
						showAttrs = showComponents;
						foreach (var component in components)
							component.showChildren = showComponents;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				window.EndRow();
			}

			if (showComponents) {
				DrawAttributes(indent + Util.tabSize * 2, colWidth);

				var tmp = new List<ComponentHelper>(components);
				foreach (var component in tmp)
					component.Draw(indent + Util.tabSize * 2, colWidth);

				if (draw)
					GUILayout.Space(ComponentsMargin);
			}

			//Children
			if (showChildren && children != null) {
				var tmp = new List<GameObjectHelper>(children);
				foreach (var helper in tmp) {
					helper.Draw(colWidth, indent + Util.tabSize);
				}
			}
		}

		void DrawAttributes(float indent, float colWidth) {
			if (window.ScrollCheck()) {
				//TODO: Draw GO fields as serializedProperty or something
				window.StartRow(sameAttrs);

				DrawAttribute(true, indent, colWidth);

				if (GetObject(true) && GetObject(false))
					DrawMidButtons(delegate {
						SetAttrs(true);
						BubbleRefresh();
					}, delegate {
						SetAttrs(false);
						BubbleRefresh();
					});
				else
					GUILayout.Space(UniMergeConfig.midWidth * 2);

				DrawAttribute(false, indent, colWidth);

				window.EndRow();
			}

			if (showAttrs)
				foreach (var attribute in attributes)
					attribute.Draw(indent + Util.tabSize, colWidth);
		}

		void DrawAttribute(bool isMine, float indent, float colWidth) {
			GUILayout.BeginVertical(GUILayout.Width(colWidth + UniMergeWindow.columnPadding));
			{
#if Unity3
				GUILayout.Space(3);
#endif
				Util.Indent(indent, delegate {
					if (GetObject(isMine)) {
#if UNITY_5_5_OR_NEWER
						showAttrs = EditorGUILayout.Foldout(showAttrs, "Attributes", true);
#else
						showAttrs = EditorGUILayout.Foldout(showAttrs, "Attributes");
#endif
					} else {
						GUILayout.Label("");
						GUILayout.Space(EmptyRowSpace);
					}
				});
#if Unity3
				GUILayout.Space(-4);
#endif
			}
			GUILayout.EndVertical();
		}

		void SetAttrs(bool toMine) {
			foreach (var attribute in attributes)
				attribute.Copy(toMine);

			sameAttrs = true;
		}

		IEnumerator Delete(bool isMine) {
			var enumerator = DestroyAndClearRefs(GetObject(isMine), isMine);
			while (enumerator.MoveNext())
				yield return null;

			if (parent.children != null)
				parent.children.Remove(this);

			BubbleRefresh();
		}

		public override int GetDrawCount() {
			var count = 1; //Start with 1 because we're drawing this row
			if (showComponents) {
				count++; //For attributes row

				if (showAttrs)
					count += attributes.Sum(attribute => attribute.GetDrawCount());

				count += components.Sum(component => component.GetDrawCount());
			}

			if (showChildren && children != null)
				count += children.Sum(helper => helper.GetDrawCount());

			return count;
		}

		public int Count() {
			var count = 1; //Start with 1 because we're counting this row

			if (children != null)
				count += children.Sum(helper => helper.Count());

			return count;
		}

		public IEnumerator DestroyAndClearRefs(UnityObject obj, bool isMine) {
			var searchList = new List<GameObjectHelper>();
			window.root.ToList(searchList);

			for (var i = 0; i < searchList.Count; i++) {
				var searchComponents = searchList[i].components;
				for (var j = 0; j < searchComponents.Count; j++) {
					var properties = searchComponents[j].properties;
					for (var k = 0; k < properties.Count; k++) {
						var property = properties[k];
						var prop = property.GetProperty(isMine);
						if (prop == null || prop.propertyType != SerializedPropertyType.ObjectReference)
							continue;

						if (prop.objectReferenceValue == obj) {
							prop.objectReferenceValue = null;
							if (window.log) {
								Debug.Log("Set reference to null in " + prop.serializedObject.targetObject
									+ "." + prop.name, prop.serializedObject.targetObject);
							}

							if (prop.serializedObject.targetObject != null)
								prop.serializedObject.ApplyModifiedProperties();
						}
					}
				}
				yield return null;
			}

			if (isMine)
				mine = null;
			else
				theirs = null;

#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
			UnityObject.DestroyImmediate(obj);
#else
			Undo.DestroyObjectImmediate(obj);
#endif
		}

		/// <summary>
		/// Copy an entire object from one side to the other
		/// </summary>
		/// <param name="toMine">Whether we are copying theirs to mine (true) or mie to theirs (false)</param>
		/// <returns>Iterator, for  coroutine update</returns>
		internal IEnumerator Copy(bool toMine) {
			if (Same)
				yield break;

			//Clear out old object
			if (toMine && mine)
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
				UnityObject.DestroyImmediate(mine);
#else
				Undo.DestroyObjectImmediate(mine);
#endif

			if (!toMine && theirs)
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
				UnityObject.DestroyImmediate(theirs);
#else
				Undo.DestroyObjectImmediate(theirs);
#endif

			//Clear out old helpers
			components.Clear();
			if (children != null)
				children.Clear();

			var original = toMine ? theirs : mine;

			// ReSharper disable once RedundantCast
			var copy = (GameObject) UnityObject.Instantiate(original); // Cast required for Unity < 5

			var copyTransform = copy.transform;
			if (parent != null)
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
				copyTransform.SetParent(parent.GetObject(toMine).transform);
#else
				copyTransform.parent = parent.GetObject(toMine).transform;
#endif

			// Set transform for legacy versions
			var originalTransform = original.transform;
			copyTransform.localPosition = originalTransform.localPosition;
			copyTransform.localRotation = originalTransform.localRotation;
			copyTransform.localScale = originalTransform.localScale;

			copy.name = original.name;

			UniMergeWindow.blockRefresh = true;
			//Set any references on their side to this object
			if (window.deepCopy) {
				// Deep copy will have issues if this helper references destroyed object
				if (toMine)
					mine = copy;
				else
					theirs = copy;

				window.updateType = RefreshType.Updating;
				var enumerator = Refresh();
				while (enumerator.MoveNext())
					yield return null;

				window.updateType = RefreshType.Copying;
				enumerator = FindAndSetRefs(window, original, copy, !toMine);
				while (enumerator.MoveNext())
					yield return null;
			}

			// Connect prefabs, if applicable. This must happen after all modifications
#if UNITY_5_4_OR_NEWER
			ConnectPrefabsAfterCopy(toMine);
#endif

			// Set SceneMerge window references
			if (this == window.root) {
				var sceneMerge = window.sceneMerge;
				if (sceneMerge) {
					if (toMine)
						sceneMerge.myContainer = mine;
					else
						sceneMerge.theirContainer = theirs;
				}
			}
			UniMergeWindow.blockRefresh = false;

			Undo.RegisterCreatedObjectUndo(toMine ? mine : theirs, "UniMerge");
			BubbleRefresh();
		}

#if UNITY_5_4_OR_NEWER
		/// <summary>
		/// Connect copied objects to prefabs if original object is connected, and sets helper reference back to new prefab reference
		/// Also recursively calls itself in all children
		/// </summary>
		/// <param name="toMine">Whether we are copying theirs to mine (true) or mie to theirs (false)</param>
		void ConnectPrefabsAfterCopy(bool toMine) {
			var original = toMine ? theirs : mine;
			var copy = toMine ? mine : theirs;
			if (Util.IsPrefabParent(original) && PrefabUtility.GetPrefabType(original) == PrefabType.PrefabInstance) {
				copy = PrefabUtility.ConnectGameObjectToPrefab(copy, (GameObject) PrefabUtility.GetPrefabParent(original));
				copy.name = original.name; // ConnectPrefab will rename to prefab name

				// ConnectPrefab will override transform
				var copyTransform = copy.transform;
				var originalTransform = original.transform;
				copyTransform.localPosition = originalTransform.localPosition;
				copyTransform.localRotation = originalTransform.localRotation;
				copyTransform.localScale = originalTransform.localScale;
			}

			if (toMine)
				mine = copy;
			else
				theirs = copy;

			if (children != null)
				foreach (var child in children)
					child.ConnectPrefabsAfterCopy(toMine);
		}
#endif

		/// <summary>
		/// Find references of source in mine, and set their counterparts in Theirs to copy. This "start" function calls
		/// FindRefs which searches the whole object's hierarchy, and then calls UnsetFlagRecursive to reset the flag
		/// used to avoid searching the same object twice
		/// </summary>
		/// <param name="window">The ObjectMergeWindow doing the merge</param>
		/// <param name="source">The source object to find references within</param>
		/// <param name="copy">The copy we just made of source</param>
		/// <param name="isMine">Whether the source object is on the mine (left) side</param>
		/// <returns>Iterator, for  coroutine update</returns>
		static IEnumerator FindAndSetRefs(ObjectMerge window, GameObject source, GameObject copy, bool isMine) {
			var root = window.root;
			var sourceObjs = new List<GameObject>();
			var copyObjs = new List<GameObject>();
			var srcProps = new List<SerializedProperty>();
			var copyProps = new List<SerializedProperty>();
			var sourceComps = new List<Component>();
			var copyComps = new List<Component>();
			Util.GameObjectToList(source, sourceObjs);
			yield return null;
			Util.GameObjectToList(copy, copyObjs);
			yield return null;
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
			source.GetComponentsInChildren(sourceComps);
			yield return null;
			copy.GetComponentsInChildren(copyComps);
#else
			sourceComps.AddRange(source.GetComponents<Component>());
			yield return null;
			copyComps.AddRange(copy.GetComponents<Component>());
#endif
			yield return null;
			var searchList = new List<GameObjectHelper>();
			root.ToList(searchList);

			var properties = new List<PropertyHelper>();
			var props = new List<SerializedProperty>();
			var otherProps = new List<SerializedProperty>();
			var objs = new List<object>(searchList.Count);
			var otherObjs = new List<GameObject>(searchList.Count);
			var comps = new List<object>(searchList.Count);
			var otherComps = new List<Component>(searchList.Count);

			window.updateType = RefreshType.Preparing;
			window.totalUpdateNum = searchList.Count;
			window.updateCount = 0;
			for (var i = 0; i < searchList.Count; i++) {
				window.updateCount++;
				var searchObj = searchList[i];
				objs.Add(searchObj.GetObject(isMine));
				otherObjs.Add(searchObj.GetObject(!isMine));
				var searchComponents = searchObj.components;
				for (var j = 0; j < searchComponents.Count; j++) {
					var comp = searchComponents[j];
					comps.Add(comp.GetComponent(isMine));
					otherComps.Add(comp.GetComponent(!isMine));
					properties.Clear();
					comp.GetFullPropertyList(properties);
					for (var k = 0; k < properties.Count; k++) {
						var property = properties[k];
						var prop = property.GetProperty(isMine);
						var otherProp = property.GetProperty(!isMine);
						if (prop != null && otherProp != null 
							&& prop.propertyType == SerializedPropertyType.ObjectReference
							&& prop.objectReferenceValue != null) {
							props.Add(prop);
							otherProps.Add(otherProp);
						}
					}
				}

				yield return null;
			}

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = sourceObjs.Count;
			window.updateCount = 0;
			for (var i = 0; i < sourceObjs.Count; i++) {
				window.updateCount++;
				var sourceObject = sourceObjs[i];
				var copyObject = copyObjs[i];

				// Find and set refs to the GameObject
				for (var j = 0; j < props.Count; j++) {
					var prop = props[j];
					var otherProp = otherProps[j];
					if (prop.objectReferenceValue == sourceObject) {
						//Sometimes you get an error here in older versions of Unity about using a
						//SerializedProperty after the object has been deleted.  Don't know how else to
						//detect this
						otherProp.objectReferenceValue = copyObject;
						if (window.log) {
							Debug.Log("Set reference to " + copyObject + " in "
								+ prop.serializedObject.targetObject + "." + prop.name,
								prop.serializedObject.targetObject);
						}

						if (prop.serializedObject.targetObject != null)
							prop.serializedObject.ApplyModifiedProperties();
					}
				}

				yield return null;
			}

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = sourceComps.Count;
			window.updateCount = 0;
			for (var i = 0; i < sourceComps.Count; i++) {
				window.updateCount++;
				var sourceComponent = sourceComps[i];
				// Missing scripts show up as null
				if (sourceComponent == null)
					continue;

				if (sourceComponent is Transform)
					continue;

				var copyComponent = copyComps[i];

				// Find and set refs to the Component
				for (var l = 0; l < props.Count; l++) {
					var prop = props[l];
					var otherProp = otherProps[l];
					if (prop.objectReferenceValue == sourceComponent) {
						//Sometimes you get an error here in older versions of Unity about using a
						//SerializedProperty after the object has been deleted.  Don't know how else to
						//detect this
						otherProp.objectReferenceValue = copyComponent;
						if (window.log) {
							Debug.Log("Set reference to " + copyComponent + " in "
								+ prop.serializedObject.targetObject + "." + prop.name,
								prop.serializedObject.targetObject);
						}

						if (prop.serializedObject.targetObject != null)
							prop.serializedObject.ApplyModifiedProperties();
					}
				}

				yield return null;

				//Find references outside the copied hierarchy
				srcProps.Clear();
				copyProps.Clear();
				PropertyHelper.GetProperties(srcProps, new SerializedObject(sourceComponent));
				PropertyHelper.GetProperties(copyProps, new SerializedObject(copyComponent));
				for (var j = 0; j < srcProps.Count; j++) {
					var srcProp = srcProps[j];
					if (srcProp.name == "m_Script") //Ignore the script
						continue;

					if (srcProp.propertyType == SerializedPropertyType.ObjectReference
						&& srcProp.objectReferenceValue != null) {
						if (srcProp.objectReferenceValue == null)
							continue;

						var copyProp = copyProps[j];
						if (srcProp.objectReferenceValue is GameObject) {
							var index = objs.IndexOf(srcProp.objectReferenceValue);
							if (index >= 0) {
								var otherobj = otherObjs[index];
								if (window.log) {
									Debug.Log(
										"Set reference to " + otherobj + " in "
										+ copyProp.serializedObject.targetObject + "." + copyProp.name,
										copyProp.serializedObject.targetObject);
								}
								copyProp.objectReferenceValue = otherobj;
							}
						} else {
							var index = comps.IndexOf(srcProp.objectReferenceValue);
							if (index >= 0) {
								var otherComp = otherComps[index];
								if (window.log) {
									Debug.Log(
										"Set reference to " + otherComp + " in "
										+ copyProp.serializedObject.targetObject + "." + copyProp.name,
										copyProp.serializedObject.targetObject);
								}
								copyProp.objectReferenceValue = otherComp;
							}
						}

						if (copyProp.serializedObject.targetObject != null)
							copyProp.serializedObject.ApplyModifiedProperties();
					}

					yield return null;
				}
			}
		}

		void ToList(List<GameObjectHelper> list) {
			list.Add(this);
			if (children != null)
				for (var i = 0; i < children.Count; i++)
					children[i].ToList(list);
		}

		/// <summary>
		/// Get the spouse (counterpart) of an object within this tree.
		/// </summary>
		/// <param name="obj">The object we're looking for</param>
		/// <param name="isMine">Whether the object came from mine (left)</param>
		/// <returns></returns>
		public GameObject GetObjectSpouse(GameObject obj, bool isMine) {
			if (obj == GetObject(isMine)) {
				var spouse = GetObject(!isMine);
				return spouse ? spouse : null;
			}

			if (children != null) {
				foreach (var child in children) {
					var spouse = child.GetObjectSpouse(obj, isMine);
					if (spouse)
						return spouse;
				}
			}

			return null;
		}

		void DrawObject(bool isMine, float indent, float colWidth) {
			//Store foldout state before doing GUI to check if it changed
			var foldoutState = showChildren;
			//Create space with width = colWidth
			GUILayout.BeginVertical(GUILayout.Width(colWidth));
			Util.Indent(indent, delegate {
				GUILayout.BeginHorizontal();
				if (GetObject(isMine)) {
					GUILayout.BeginVertical();
					{
						GUILayout.Space(FoldoutPadding); //Foldouts are too high by 4px... ok
						if (GetObject(isMine).transform.childCount > 0)
#if UNITY_5_5_OR_NEWER
							showChildren = EditorGUILayout.Foldout(showChildren, GetObject(isMine).name, true);
#else
							showChildren = EditorGUILayout.Foldout(showChildren, GetObject(isMine).name);
#endif
						else
							GUILayout.Label(GetObject(isMine).name);
					}
					GUILayout.EndVertical();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(new GUIContent("F", "Focus and select the object"))) {
						Selection.activeObject = GetObject(isMine);
						if (SceneView.lastActiveSceneView)
							SceneView.lastActiveSceneView.FrameSelected();
						EditorGUIUtility.PingObject(GetObject(isMine));
					}

					if (GUILayout.Button(new GUIContent("P", "Ping the object in the hierarchy")))
						EditorGUIUtility.PingObject(GetObject(isMine));
				} else { GUILayout.Label(""); }

				GUILayout.EndHorizontal();
			});
			GUILayout.EndVertical();
			//If foldout state changed and user was holding alt, set all child foldout states to this state
			if (Event.current.alt && showChildren != foldoutState)
				SetFoldoutRecur(showChildren);
		}

		void SetFoldoutRecur(bool state) {
			showChildren = state;
			if (children != null)
				foreach (var obj in children)
					obj.SetFoldoutRecur(state);
		}

		public IEnumerator<bool> ExpandDiffs() {
			window.updateCount++;
			if (children != null) {
				foreach (var child in children) {
					var enumerator = child.ExpandDiffs();
					while (enumerator.MoveNext())
						yield return !Same;

					if (enumerator.Current)
						showChildren = true;
				}
			}

			yield return !Same;
		}

		//Big ??? here.  What do we count as the same needing merge and what do we count as totally different?
		static bool SameObject(UnityObject mine, UnityObject theirs) { return mine.name == theirs.name; }

		GameObject GetObject(bool isMine) { return isMine ? mine : theirs; }
	}
}
