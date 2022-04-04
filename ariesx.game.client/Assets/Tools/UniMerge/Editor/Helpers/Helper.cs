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
//Helper class

using System;
using System.Collections;
using UniMerge.Editor.Windows;
using UnityEngine;

namespace UniMerge.Editor.Helpers {
	public abstract class Helper {
		protected const float EmptyRowSpace = 8;

		public readonly Helper parent;
		public bool showChildren { protected get; set; }
		protected readonly UniMergeWindow window;

		protected Helper(Helper parent = null, UniMergeWindow window = null) {
			this.parent = parent;
			this.window = window;
		}

		public bool Same { get; protected set; }

		public virtual void BubbleRefresh() {
			window.drawAbort = true;
			if (parent != null) {
				parent.BubbleRefresh();
			} else {
				window.updateType = RefreshType.Updating;
				window.update = Refresh();
			}
		}

		public abstract IEnumerator Refresh();
		public abstract int GetDrawCount();

		public static void DrawMidButtons(Action toMine, Action toTheirs) {
			DrawMidButtons(true, true, toMine, toTheirs, null, null);
		}

		public static void DrawMidButtons(bool hasMine, bool hasTheirs, Action toMine, Action toTheirs,
			Action delMine, Action delTheirs) {
			DrawMidButtons(hasMine, hasTheirs, hasMine, hasTheirs, toMine, toTheirs, delMine, delTheirs);
		}

		public static void DrawMidButtons(bool hasMine, bool hasTheirs, bool hasMyParent, bool hasTheirParent,
			Action toMine, Action toTheirs, Action delMine, Action delTheirs) {
			GUILayout.BeginVertical(GUILayout.Width(UniMergeConfig.midWidth * 2));
#if Unity3
			GUILayout.Space(2);
#endif
			GUILayout.BeginHorizontal();
			if (hasTheirs) {
				if (hasMyParent) {
					if (GUILayout.Button(new GUIContent("<", "Copy theirs (properties and children) to mine"),
						GUILayout.Width(UniMergeConfig.midWidth)))
						toMine.Invoke();
				} else {
					GUILayout.Space(UniMergeConfig.midWidth + 4);
				}
			} else {
				if (GUILayout.Button(new GUIContent("X", "Delete mine"), GUILayout.Width(UniMergeConfig.midWidth)))
					delMine.Invoke();
			}
			if (hasMine) {
				if (hasTheirParent) {
					if (GUILayout.Button(new GUIContent(">", "Copy mine (properties and children) to theirs"),
						GUILayout.Width(UniMergeConfig.midWidth)))
						toTheirs.Invoke();
				} else {
					GUILayout.Space(UniMergeConfig.midWidth + 4);
				}
			} else {
				if (GUILayout.Button(new GUIContent("X", "Delete theirs"), GUILayout.Width(UniMergeConfig.midWidth)))
					delTheirs.Invoke();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
}
