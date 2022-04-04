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
//ComponentHelper class

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

namespace UniMerge.Editor.Helpers {
	public class ComponentHelper : Helper {
		ComponentContainer componentContainer;

		public Component mine {
			get { return componentContainer.mine; }
			set {
				if (componentContainer == null)
					componentContainer = ScriptableObject.CreateInstance<ComponentContainer>();

				var objContainer = new SerializedObject(componentContainer);
				var mineProp = objContainer.FindProperty("_mine");
				mineProp.objectReferenceValue = value;
				mineProp.serializedObject.ApplyModifiedProperties();
			}
		}
		public Component theirs {
			get { return componentContainer.theirs; }
			set {
				if (componentContainer == null)
					componentContainer = ScriptableObject.CreateInstance<ComponentContainer>();

				var objContainer = new SerializedObject(componentContainer);
				var theirsProp = objContainer.FindProperty("_theirs");
				theirsProp.objectReferenceValue = value;
				theirsProp.serializedObject.ApplyModifiedProperties();
			}
		}

		public readonly Type type;
		public readonly List<PropertyHelper> properties = new List<PropertyHelper>(8); // 8 seems like a good average # of components

		SerializedObject mySO, theirSO;
		new readonly GameObjectHelper parent;

		public ComponentHelper(Component mine, Component theirs, GameObjectHelper parent = null,
			UniMergeWindow window = null) : base(parent, window) {
			this.parent = parent;
			this.mine = mine;
			this.theirs = theirs;

			type = mine ? mine.GetType() : theirs.GetType();
		}

		public Component GetComponent(bool isMine) { return isMine ? mine : theirs; }

		public override IEnumerator Refresh() {
			if (mine)
				mySO = new SerializedObject(mine);
			if (theirs)
				theirSO = new SerializedObject(theirs);

			var enumerator = PropertyHelper.UpdatePropertyList(properties, mySO, theirSO, this, window);
			while (enumerator.MoveNext())
				yield return null;

			Same = enumerator.Current;
		}

		public void Draw(float indent, float colWidth) {
			if (window.drawAbort)
				return;

			if (window.ScrollCheck()) {
				window.StartRow(Same);
				DrawComponent(true, indent, colWidth);
				//Swap buttons
				if (parent.mine && parent.theirs)
					DrawMidButtons(mine, theirs, parent.mine, parent.theirs, delegate {
						//Copy theirs to mine
						EditorUtility.CopySerialized(theirs, mine ?? parent.mine.AddComponent(theirs.GetType()));
						BubbleRefresh();
					}, delegate {
						//Copy mine to theirs
						EditorUtility.CopySerialized(mine, theirs ?? parent.theirs.AddComponent(mine.GetType()));
						BubbleRefresh();
					}, delegate {
						//Delete mine
						window.updateType = RefreshType.Deleting;
						window.update = Delete(true);
					}, delegate {
						//Delete theirs
						window.updateType = RefreshType.Deleting;
						window.update = Delete(false);
					});
				else
					GUILayout.Space(UniMergeConfig.midWidth * 2);
				//Display theirs
				DrawComponent(false, indent, colWidth);
				window.EndRow();
			}

			if (showChildren) {
				var tmp = new List<PropertyHelper>(properties);
				foreach (var property in tmp)
					property.Draw(indent + Util.tabSize, colWidth);
			}

			if (mySO != null && mySO.targetObject != null)
				if (mySO.ApplyModifiedProperties())
					BubbleRefresh();

			if (theirSO != null && theirSO.targetObject != null)
				if (theirSO.ApplyModifiedProperties())
					BubbleRefresh();
		}

		IEnumerator Delete(bool isMine) {
			var component = GetComponent(isMine);
			if (component is Camera)
			{
				var enumerator = parent.DestroyAndClearRefs(component.GetComponent<AudioListener>(), true);
				while (enumerator.MoveNext())
					yield return null;

				enumerator = parent.DestroyAndClearRefs(component.GetComponent<GUILayer>(), true);
				while (enumerator.MoveNext())
					yield return null;

				enumerator = parent.DestroyAndClearRefs(component.GetComponent("FlareLayer"), true);
				while (enumerator.MoveNext())
					yield return null;
			}
			var e = parent.DestroyAndClearRefs(component, true);
			while (e.MoveNext())
				yield return null;

			BubbleRefresh();
		}

		void DrawComponent(bool isMine, float indent, float colWidth) {
			GUILayout.BeginVertical(GUILayout.Width(colWidth + UniMergeWindow.columnPadding));

#if Unity3
			GUILayout.Space(3);
#endif

			Util.Indent(indent, delegate {
				if (GetComponent(isMine)) {
#if UNITY_5_5_OR_NEWER
					showChildren = EditorGUILayout.Foldout(showChildren, GetComponent(isMine).GetType().Name, true);
#else
					showChildren = EditorGUILayout.Foldout(showChildren, GetComponent(isMine).GetType().Name);
#endif
				} else {
					GUILayout.Label("");
					GUILayout.Space(EmptyRowSpace);
				}
			});

#if Unity3
			GUILayout.Space(-4);
#endif

			GUILayout.EndVertical();
		}

		public void GetFullPropertyList(List<PropertyHelper> list) {
			for (var i = 0; i < properties.Count; i++)
				properties[i].ToList(list);
		}

		public override int GetDrawCount() {
			var count = 1;
			if (showChildren)
				count += properties.Sum(property => property.GetDrawCount());

			return count;
		}
	}
}
