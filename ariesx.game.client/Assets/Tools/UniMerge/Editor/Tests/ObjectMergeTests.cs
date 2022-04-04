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
//ObjecMerge Tests

#if UNITY_5_6_OR_NEWER && UNIMERGE_TESTS
using NUnit.Framework;
using System.Collections;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.TestTools;

namespace UniMerge.Editor.Tests {
	public class ObjecMergeTests {
		[UnityTest]
		public IEnumerator TestObjectMerge() {
			SceneMergeTests.CloseAllUniMergeWindows();

			var testScenePath = UniMergeConfig.defaultPath + "/Demo/Object Merge/ObjectCompare.unity";
			EditorSceneManager.OpenScene(testScenePath);
			var objectMerge = EditorWindow.GetWindow<ObjectMerge>();
			objectMerge.Show();

			var count = 0;
			const float maxMergeFrames = 1000;
			while (objectMerge.update != null) {
				Assert.That(count++ < maxMergeFrames, "SceneMerge.Merge failed to refresh");
				yield return null;
			}

			objectMerge.update = objectMerge.root.Copy(true);

			count = 0;
			while (objectMerge.update != null) {
				Assert.That(count++ < maxMergeFrames, "ObjectMerge.root.Copy failed to refresh");
				yield return null;
			}

			Assert.That(objectMerge.root.Same);
			// Note: The SceneMerge and ObjectMerge windows won't show all green becasue of the Undo which is applied after tests

			SceneMergeTests.CloseAllUniMergeWindows();
		}

		/// <summary>
		/// Test general performance
		/// Baseline: 4.827s on i7-6900K
		/// </summary>
		[UnityTest]
		public IEnumerator PerfTest() {
			SceneMergeTests.CloseAllUniMergeWindows();

			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			UnityEngine.Random.InitState(0);
			const int objectCount = 2500;
			var mine = FillScene.GenerateObjects(objectCount);
			var theirs = FillScene.GenerateObjects(objectCount);
			var objectMerge = EditorWindow.GetWindow<ObjectMerge>();
			objectMerge.Show();

			objectMerge.root.mine = mine;
			objectMerge.root.theirs = theirs;

			while (objectMerge.update != null) { yield return null; }

			objectMerge.update = objectMerge.root.Copy(true);

			while (objectMerge.update != null) { yield return null; }

			Assert.That(objectMerge.root.Same);
			// Note: The SceneMerge and ObjectMerge windows won't show all green becasue of the Undo which is applied after tests

			SceneMergeTests.CloseAllUniMergeWindows();
		}
	}
}
#endif
