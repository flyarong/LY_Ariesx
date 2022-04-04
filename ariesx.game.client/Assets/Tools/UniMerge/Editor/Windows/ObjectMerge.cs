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
//ObjectMerge Window

//With the DEV flag defined, the ObjectMege window will, by default, search for GameObjects called mine and theirs
//in the scene and put them in mine and theirs.  This is so that when using the demo scene, I don't have to reset 
//those references after each compile.  I leave it on by default so that there is one less step when users first
//see the tool.  Comment this line out if this behavior somehow interferes with your workflow.

#define DEV

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Windows {
	public class ObjectMerge : UniMergeWindow {
		public const string MineDemoName = "Mine", TheirsDemoName = "Theirs";

#if Unity3
		static string filters = "", lastFilters;
		public static List<System.Type> filterTypes;
		List<string> badTypes;
		List<string> notComponents;
		System.Reflection.Assembly[] assemblies;
#else
		public static Type[][] componentTypes { get; private set; }
		string[][] componentTypeStrings;
#endif

		public bool deepCopy { get; private set; }
		public bool log { get; private set; }
		public bool compareAttrs { get; private set; }
		public int[] typeMask { get; private set; }

		public GameObjectHelper root { get; private set; }

		public SceneMerge sceneMerge;

		string mineName = MineDemoName, theirName = TheirsDemoName;

		// ReSharper disable once UnusedMember.Local
		[MenuItem("Window/UniMerge/Object Merge %m")]
		static void Init() {
			GetWindow(typeof(ObjectMerge), false, "ObjectMerge");
		}

		protected override void OnEnable() {
			base.OnEnable();
			blockRefresh = false;
			root = new GameObjectHelper(this);

#if Unity3 // TODO: Unity 3 path stuff?

//Component filters
			assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			//NB: For some reason, after a compile, filters starts out as "", though the field retains the value.  Then when it's modified the string is set... as a result sometime you see filter text with nothing being filtered
			ParseFilters();
#else
			//Get path
			var scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
			UniMergeConfig.defaultPath = scriptPath.Substring(0, scriptPath.IndexOf("Editor") - 1);

			//Component filters
			var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in assembly.GetTypes()
				where type.IsSubclassOf(typeof(Component))
				select type;
			var compTypeStrs = new List<List<string>> { new List<string>() };
			var compTypes = new List<List<Type>> { new List<Type>() };
			var setCount = 0;
			foreach (var t in subclasses) {
				if (compTypes[setCount].Count == 31) {
					setCount++;
					compTypeStrs.Add(new List<string>());
					compTypes.Add(new List<Type>());
				}
				compTypeStrs[setCount].Add(t.Name);
				compTypes[setCount].Add(t);
			}
			componentTypes = new Type[setCount + 1][];
			componentTypeStrings = new string[setCount + 1][];
			typeMask = new int[setCount + 1];
			for (var i = 0; i < setCount + 1; i++) {
				typeMask[i] = -1;
				componentTypes[i] = compTypes[i].ToArray();
				componentTypeStrings[i] = compTypeStrs[i].ToArray();
			}
#endif

#if DEV
			root.mine = GameObject.Find(MineDemoName);
			root.theirs = GameObject.Find(TheirsDemoName);
#endif

			deepCopy = true;
			compareAttrs = true;

			if (EditorPrefs.HasKey(RowHeightKey))
				selectedRowHeight = EditorPrefs.GetInt(RowHeightKey);
		}

		public override IEnumerator Refresh() {
			//This is where we initiate the merge for the first time
			root.components.Clear();
			root.attributes.Clear();
			if (root.children != null)
				root.children.Clear();

			if (!root.mine || !root.theirs)
				yield break;

#if Unity3
			updateType = RefreshType.Updating;
			update = root.DoRefresh();
#else
			//Check if the objects are prefabs
			switch (PrefabUtility.GetPrefabType(root.mine)) {
				case PrefabType.ModelPrefab:
				case PrefabType.Prefab:
					switch (PrefabUtility.GetPrefabType(root.theirs)) {
						case PrefabType.ModelPrefab:
						case PrefabType.Prefab:
							if (EditorUtility.DisplayDialog("Instantiate prefabs?",
								"In order to merge prefabs, you must instantiate them and merge the instances. Then you must apply the changes.",
								"Instantiate", "Cancel")) {
								root.mine = PrefabUtility.InstantiatePrefab(root.mine) as GameObject;
								root.theirs = PrefabUtility.InstantiatePrefab(root.theirs) as GameObject;
							} else {
								root.mine = null;
								root.theirs = null;
							}
							break;
						default:
							Debug.LogWarning("Sorry, you must compare a prefab with a prefab");
							break;
					}
					break;
				case PrefabType.DisconnectedPrefabInstance:
				case PrefabType.PrefabInstance:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.None:
					switch (PrefabUtility.GetPrefabType(root.theirs)) {
						case PrefabType.DisconnectedPrefabInstance:
						case PrefabType.PrefabInstance:
						case PrefabType.ModelPrefabInstance:
						case PrefabType.None:
							break;
						default:
							Debug.LogWarning("Sorry, this prefab type is not supported");
							break;
					}
					break;
				default:
					Debug.LogWarning("Sorry, this prefab type is not supported");
					break;
			}
#endif

			updateType = RefreshType.Updating;
			update = root.Refresh();
		}

		protected override int GetDrawCount() { return root.GetDrawCount(); }

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			if (InitGUI())
				return;

			/*
			 * BEGIN GUI
			 */

			GUILayout.BeginHorizontal();
			{
				/*
				 * Options
				 */
				GUILayout.BeginVertical();
				{
					const string tooltip = "When enabled, copying GameObjects or Components will search for references"
						+ " to them and try to set them.  Disable if you do not want this behavior or if the window "
						+ "locks up on copy (too many objects)";
					deepCopy = EditorGUILayout.Toggle(new GUIContent("Deep Copy", tooltip), deepCopy);
				}

				{
					const string tooltip = "When enabled, non-obvious events (like deep copy reference setting) will be logged";
					log = EditorGUILayout.Toggle(new GUIContent("Log", tooltip), log);
				}

				{
					const string tooltip = "When disabled, attributes will not be included in comparison algorithm."
						+ "  To choose which components are included, use the drop-downs to the right.";
					compareAttrs = EditorGUILayout.Toggle(new GUIContent("Compare Attributes", tooltip), compareAttrs);
				}

				GUI.enabled = update == null;
				if (GUILayout.Button("Expand Differences")) {
					updateCount = 0;
					totalUpdateNum = root.Count();
					updateType = RefreshType.Expanding;
					update = root.ExpandDiffs();
				}

				//GUILayout.BeginHorizontal();
				//if (GUILayout.Button("Prev Difference")) {
				//	PrevDifference();
				//}
				//if (GUILayout.Button("Next Difference")) {
				//	NextDifference();
				//}
				//GUILayout.EndHorizontal();

				if (GUILayout.Button("Refresh"))
					root.BubbleRefresh();

				GUI.enabled = true;

				DrawRowHeight();

				GUILayout.Space(10); //Padding between controls and merge space
				GUILayout.EndVertical();

				/*
				 * Comparison Filters
				 */
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();

#if !Unity3
				GUILayout.FlexibleSpace();
#endif

#if Unity3 //TODO: Better masking for U3
				GUILayout.BeginVertical();
				GUILayout.Label("Enter a list of component types to exclude, separated by commas");
				filters = EditorGUILayout.TextField("Filters", filters);
				if(filters != lastFilters){
					ParseFilters();
				}
				lastFilters = filters;
				string filt = "Filtering: ";
				if(filterTypes.Count > 0){
					foreach(System.Type bad in filterTypes)
						filt += bad.Name + ", ";
					GUILayout.Label(filt.Substring(0, filt.Length - 2));
				}
				string err = "Sorry, the following types are invalid: ";
				if(badTypes.Count > 0){
					foreach(string bad in badTypes)
						err += bad + ", ";
					GUILayout.Label(err.Substring(0, err.Length - 2));
				}
				string cerr = "Sorry, the following types aren't components: ";
				if(notComponents.Count > 0){
					foreach(string bad in notComponents)
						cerr += bad + ", ";
					GUILayout.Label(cerr.Substring(0, cerr.Length - 2));
				}
				GUILayout.EndVertical();
#else
				GUILayout.Label(new GUIContent("Comparison Filters",
					"Select which components should be included in"
					+ " the comparison.  This is a little bugged right now so its disabled.  You can't filter more"
					+ " than 31 things :("));
				if (componentTypeStrings != null)
					for (var i = 0; i < componentTypeStrings.Length; i++) {
						typeMask[i] = EditorGUILayout.MaskField(typeMask[i], componentTypeStrings[i], GUILayout.Width(75));
						if (i % 3 == 2) {
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
						}
					}
#endif
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical(GUILayout.Width(colWidth));
				{
#if !(Unity3 || Unity4_0To4_2)
					GUILayout.BeginHorizontal();
					mineName = GUILayout.TextField(mineName, GUILayout.Width(EditorGUIUtility.labelWidth));
					root.mine = (GameObject) EditorGUILayoutExt.ObjectField(root.mine, typeof(GameObject), true);
					GUILayout.EndHorizontal();
#else
					root.mine = (GameObject) EditorGUILayoutExt.ObjectField(mineName, root.mine, typeof(GameObject), true);
#endif
				}
				GUILayout.EndVertical();
				GUILayout.Space(UniMergeConfig.midWidth * 2);
				GUILayout.BeginVertical(GUILayout.Width(colWidth));
				{
#if !(Unity3 || Unity4_0To4_2)
					GUILayout.BeginHorizontal();
					theirName = GUILayout.TextField(theirName, GUILayout.Width(EditorGUIUtility.labelWidth));
					root.theirs = (GameObject) EditorGUILayoutExt.ObjectField(root.theirs, typeof(GameObject), true);
					GUILayout.EndHorizontal();
#else
					root.theirs = (GameObject)EditorGUILayoutExt.ObjectField(theirName, root.theirs, typeof(GameObject), true);
#endif
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

#if !(Unity3 || Unity4_0To4_2)
			EditorGUIUtility.labelWidth = 75; //Make labels just a bit tighter for compactness
#endif
			if (root.mine && root.theirs)
				CustomScroll(() => { root.Draw(colWidth); });

			ProgressBar();
		}

#if Unity3
		void ParseFilters(){
			filterTypes = new List<System.Type>();
			badTypes = new List<string>();
			notComponents = new List<string>();
			string[] tmp = filters.Replace(" ", "").Split(',');
			foreach(string filter in tmp){
				if(!string.IsNullOrEmpty(filter)){
					bool found = false;
					foreach(System.Reflection.Assembly asm in assemblies){
						foreach(System.Type t in asm.GetTypes()){
							if(t.Name.ToLower() == filter.ToLower()){
								if(t.IsSubclassOf(typeof(Component))){
									filterTypes.Add(t);
								} else notComponents.Add(filter);
								found = true;
								break;
							}
						}
						if(found)
							break;
					}
					if(!found)
						badTypes.Add(filter);
				}
			}
		}
#endif
	}
}
