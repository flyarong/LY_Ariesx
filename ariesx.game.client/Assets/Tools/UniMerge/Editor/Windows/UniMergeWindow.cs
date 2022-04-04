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
//UniMergeWindow class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniMerge.Editor.Windows {
	public enum RefreshType { Updating, Comparing, Deleting, Copying, Expanding, Preparing }

	public abstract class UniMergeWindow : EditorWindow {
		internal const int MaxFrameTime = 50;

		const int MaxDrawWindow = 400;
		const int ProgressBarHeight = 15;

#if Unity3
		const int BasePaddingTop = 0;
		const int BasePaddingBot = 0;
#else
		const int BasePaddingTop = 3;
		const int BasePaddingBot = -2;
#endif

		const float ColumnPadding = 5;

		public static int frameTimeTicks = MaxFrameTime * 10000; //In "ticks" which are 100ns
		public static float columnPadding;

		protected const string RowHeightKey = "RowHeight";
		static readonly int[] RowHeights = { 10, 5, 0 };

		static readonly HashSet<UniMergeWindow> Windows = new HashSet<UniMergeWindow>();

		public static bool blockRefresh;
		static bool displayWarning;
		static bool skinSetUp;

		public RefreshType updateType { private get; set; }
		public IEnumerator update { internal get; set; }
		public int updateCount, totalUpdateNum;

		public bool drawAbort;

		protected int selectedRowHeight;
		protected float colWidth;

		float lastWinHeight;
		bool cancelRefresh;
		bool alt;
		int rowPadding = 10;

		float objectDrawOffset;
		int objectDrawCount;
		int objectDrawCursor;
		int objectDrawOffsetHold;
		int objectDrawWindow;

		//timing variables
		readonly Stopwatch frameTimer = new Stopwatch();

		protected virtual void OnEnable() {
			if (Windows.Count == 0) {
#if UNITY_5 || UNITY_5_3_OR_NEWER
				Application.logMessageReceived += HandleLog;
#else
				Application.RegisterLogCallback(HandleLog);
#endif
			}

#if !(Unity3 || Unity4_0To4_2)
			if (Windows.Add(this))
				Undo.undoRedoPerformed += OnUndoPerformed;
#else
				Windows.Add(this);
#endif

			skinSetUp = false;
			objectDrawOffset = 0;
		}

		protected virtual void OnDisable() {
#if !(Unity3 || Unity4_0To4_2)
			if (Windows.Remove(this))
				Undo.undoRedoPerformed -= OnUndoPerformed;
#else
				Windows.Remove(this);
#endif

			if (Windows.Count == 0) {
#if UNITY_5 || UNITY_5_3_OR_NEWER
				Application.logMessageReceived -= HandleLog;
#else
				Application.RegisterLogCallback(null);
#endif
			}
		}

		void OnUndoPerformed() {
			updateType = RefreshType.Updating;
			update = Refresh();
		}

		public abstract IEnumerator Refresh();

		// ReSharper disable once UnusedMember.Local
		void OnDestroy() { EditorPrefs.SetInt(RowHeightKey, selectedRowHeight); }

		static void SetUpSkin(GUISkin builtinSkin) {
			skinSetUp = true;
			//Set up skin. We add the styles from the custom skin because there are a bunch (467!) of built in custom styles
			var guiSkinToUse = UniMergeConfig.defaultGuiSkinFilename;
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 //Alternate detection of dark skin
			if(UnityEditorInternal.InternalEditorUtility.HasPro() && EditorPrefs.GetInt("UserSkin") == 1)
				guiSkinToUse = UniMergeConfig.darkGuiSkinFilename;
#else
			if (EditorGUIUtility.isProSkin)
				guiSkinToUse = UniMergeConfig.darkGuiSkinFilename;
#endif
			var usedSkin =
				AssetDatabase.LoadAssetAtPath(UniMergeConfig.defaultPath + "/" + guiSkinToUse, typeof(GUISkin)) as GUISkin;

			if (usedSkin) {
				//GUISkin builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
				var customStyles = new List<GUIStyle>(builtinSkin.customStyles);
				//Clear styles from last enable, or for light/dark switch
				for (var i = 0; i < builtinSkin.customStyles.Length; i++)
					if (builtinSkin.customStyles[i].name == UniMergeConfig.listStyleName
						|| builtinSkin.customStyles[i].name == UniMergeConfig.listAltStyleName
						|| builtinSkin.customStyles[i].name == UniMergeConfig.listStyleName + UniMergeConfig.conflictSuffix
						|| builtinSkin.customStyles[i].name == UniMergeConfig.listAltStyleName + UniMergeConfig.conflictSuffix)
						customStyles.Remove(builtinSkin.customStyles[i]);

				customStyles.AddRange(usedSkin.customStyles);
				builtinSkin.customStyles = customStyles.ToArray();
			} else { UnityEngine.Debug.LogWarning("Can't find editor skin"); }
		}

		protected bool InitGUI() {
			drawAbort = false;

			if (!skinSetUp)
				SetUpSkin(GUI.skin);

			//Ctrl + w to close
			var current = Event.current;
			if (current.Equals(Event.KeyboardEvent("^w"))) {
				Close();
				GUIUtility.ExitGUI();
			}

			if (current.type == EventType.ScrollWheel && objectDrawWindow < objectDrawCount) {
				if (current.delta.y > 0)
					objectDrawOffset++;
				else
					objectDrawOffset--;

				Repaint();
				return true;
			}

#if Unity3
			EditorGUIUtility.LookLikeControls();
#endif
			alt = false;

			//Adjust colWidth as the window resizes
			colWidth = (position.width - UniMergeConfig.midWidth * 2 - UniMergeConfig.Margin) / 2;

			return false;
		}

		protected void ProgressBar() {
			if (update != null) {
				var pbar = new Rect(0, position.height - ProgressBarHeight * 2, position.width, ProgressBarHeight);
				EditorGUI.ProgressBar(pbar, (float) updateCount / totalUpdateNum, string.Format("{0} {1}/{2}", updateType, updateCount, totalUpdateNum));
				var cancel = new Rect(0, position.height - ProgressBarHeight, position.width, ProgressBarHeight);
				if (GUI.Button(cancel, "Cancel")) {
					cancelRefresh = true;
					GUIUtility.ExitGUI();
				}
			}
		}

		protected void CustomScroll(Action drawContent) {
			objectDrawCursor = 0;
			objectDrawCount = GetDrawCount();

			var lessCount = objectDrawCount - objectDrawWindow;
			if (lessCount < 0)
				lessCount = 0;

			if (lessCount > 0)
				lessCount++;

			if (objectDrawOffset < 0)
				objectDrawOffset = 0;

			if (objectDrawOffset >= lessCount)
				objectDrawOffset = lessCount;

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(0);
			if (lessCount > 0) {
				columnPadding = 0;
				var headerHeight = GUILayoutUtility.GetLastRect().y;
				var rect = new Rect(position.width - ProgressBarHeight, headerHeight, ProgressBarHeight,
					position.height - headerHeight);
				if (update != null)
					rect.height -= ProgressBarHeight * 2;

				objectDrawOffset = GUI.VerticalScrollbar(rect, objectDrawOffset, 1, 0, lessCount);
			} else { columnPadding = ColumnPadding; }

			drawContent();

			GUILayout.EndVertical();
			if (lessCount > 0)
				GUILayoutUtility.GetRect(ProgressBarHeight, ProgressBarHeight, 0, int.MaxValue);

			GUILayout.EndHorizontal();
		}

		protected abstract int GetDrawCount();

		// ReSharper disable once UnusedMember.Local
		void Update() {
			/*
			 * Ad-hoc editor window coroutine:  Function returns and IEnumerator, and the Update function calls MoveNext
			 * Refresh will only run when the ObjectMerge window is focused
			 */
			if (cancelRefresh) {
				objectDrawWindow = MaxDrawWindow;
				blockRefresh = false;
				update = null;
			}

			if (update != null) {
				var hasNext = true;
				var oldUpdate = update;
				while (hasNext) {
					if (YieldIfNeeded())
						break;

					hasNext = update.MoveNext();
				}

				if (!hasNext && update == oldUpdate) {
					objectDrawWindow = MaxDrawWindow;
					update = null;
				}

				Repaint();
			}

			var winHeight = position.height;
			if (lastWinHeight != winHeight)
				objectDrawWindow = MaxDrawWindow;

			objectDrawOffsetHold = (int) objectDrawOffset;
			lastWinHeight = winHeight;

			cancelRefresh = false;
			displayWarning = true;

			frameTimer.Reset();
			frameTimer.Start();
		}

		/// <summary>
		///     Check if we can draw objects yet, and increment counter
		/// </summary>
		/// <returns></returns>
		public bool ScrollCheck() {
			var check = objectDrawCursor >= objectDrawOffset;

			if (objectDrawCursor >= objectDrawOffset + objectDrawWindow) {
				drawAbort = true;
				check = false;
			}

			if (check) {
				var lastRect = GUILayoutUtility.GetLastRect();
				var height = position.height - 22;
				if (update != null)
					height -= ProgressBarHeight * 2;

				if (lastRect.y + lastRect.height >= height) {
					if (GUI.GetNameOfFocusedControl() != "UM_Slider")
						objectDrawWindow = objectDrawCursor - (int)objectDrawOffset;

					objectDrawOffset = objectDrawOffsetHold;
					drawAbort = true;
					check = false;
					Repaint();
					GUIUtility.ExitGUI();
				}
			}

			objectDrawCursor++;
			return check;
		}

		public void StartRow(bool same) {
			var style = alt ? UniMergeConfig.listAltStyleName : UniMergeConfig.listStyleName;
			if (!same)
				style += "Conflict";

			//TODO: Fix GUI error in 5.5
			EditorGUILayout.BeginVertical(style);
			//Top padding
			GUILayout.Space(rowPadding + BasePaddingTop);
			EditorGUILayout.BeginHorizontal();
		}

		public void EndRow() {
			EditorGUILayout.EndHorizontal();
			//Bottom padding
			GUILayout.Space(rowPadding + BasePaddingBot);
			EditorGUILayout.EndVertical();
			alt = !alt;
		}

		public void DrawRowHeight() {
			selectedRowHeight = EditorGUILayout.Popup("Row height: ", selectedRowHeight, new[] { "Large", "Medium", "Small" });
			rowPadding = RowHeights[selectedRowHeight];
		}

		bool YieldIfNeeded() {
			return frameTimer.ElapsedTicks > frameTimeTicks;
		}

		static void HandleLog(string logString, string stackTrace, LogType type) {
			//Totally hack solution, but it works.  This situation happens a lot in conflict resolution.  You have a prefab with git markup, this error spams the console, UniMerge crashes the editor.
			//TODO: handle "too many logs" in general.
			if (logString.Contains("seems to have merge conflicts. Please open it in a text editor and fix the merge.")) {
				if (displayWarning) {
					//I can't get this to stop displaying twice for some reason.
					EditorUtility.DisplayDialog("Merge canceled for your own good",
						"It appears that you have a prefab in your scene with merge conflicts. Unity spits out a"
						+ " warning about this at every step of the merge which makes it take years.  Resolve your"
						+ " prefab conflicts before resolving scene conflicts.", "OK, fine");
					displayWarning = false;
					foreach (var window in Windows) {
						var objectMerge = window as ObjectMerge;
						if (!objectMerge)
							continue;

						var root = objectMerge.root;
						root.mine = null;
						root.theirs = null;
					}
				}

				foreach (var window in Windows) {
					window.update = null;
					window.cancelRefresh = true;
				}

				GUIUtility.ExitGUI();
			}
		}
	}
}
