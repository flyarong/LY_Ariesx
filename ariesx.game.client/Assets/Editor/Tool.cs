using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Protocol;

namespace Poukoute {
    public class Tool {
        [MenuItem("Poukoute/Generator/Generate Script")]
        private static void GenerateScene() {
            EditorWindow window = EditorWindow.GetWindow<ScriptGenerator>(true, "Scene Generater", true) as EditorWindow;
            window.ShowPopup();
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<ScriptGenerator>();
        }

        //[MenuItem("Poukoute/Image Animation")]
        //private static void GenerateImageAnimation() {
        //    EditorWindow window = EditorWindow.GetWindow<ImageAnimationGenerator>(true, "Animation Generater", true) as EditorWindow;
        //    window.ShowPopup();
        //    window.Focus();
        //    EditorWindow.FocusWindowIfItsOpen<Poukoute.ImageAnimationGenerator>();
        //}
        [MenuItem("Poukoute/Generator/Generate Bundles")]
        private static void BuildAssetBundles() {
            AssetBundleEditor.BuildAssetBundlesMenuItem();
        }

        [MenuItem("Poukoute/Generator/Generate Prefab")]
        private static void GenerateArtPath() {
            UnityEngine.Debug.LogWarning("Generate Prefab");
            ArtConstGenerator.GenerateConfigPath();
        }

        [MenuItem("Poukoute/Generator/Generate Protocol")]
        private static void GenerateProtocol() {
            //    Debug.LogError("py " + Application.dataPath + "/../Tools/Init.py");
            string strCmdText;
            strCmdText = "/c cd " + Application.dataPath + "/../Tools/ & py Init.py";
            Process.Start("CMD.exe", strCmdText);
        }

        [MenuItem("Poukoute/Generator/Generate Fake Players")]
        private static void GeneratorFakePlayers() {
            EditorWindow window = EditorWindow.GetWindow<FakePlayerGenerator>(true, "Player Generater", true) as EditorWindow;
            window.ShowPopup();
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<FakePlayerGenerator>();
        }

        //  [MenuItem("Poukoute/Animation/")]
        private static void AnimationWindow() {
            EditorWindow window = EditorWindow.GetWindow<AnimationControllerEditor>(true, "Animation Editor", true) as EditorWindow;
            window.ShowPopup();
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<AnimationControllerEditor>();
        }

        [MenuItem("Poukoute/Build/Set Version")]
        private static void SetVersion() {
            EditorWindow window = EditorWindow.GetWindow<VersionControllerEditor>(true, "Animation Editor", true) as EditorWindow;
            window.ShowPopup();
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<VersionControllerEditor>();
        }

        [MenuItem("Poukoute/Generator/Generate Configure")]
        private static void GeneratorConfigure() {
            ConfigureGenerator.GeneratorConfigure();
            LocalFileGenerator.GeneratorLocal();
        }

        // Custom UI
        [MenuItem("GameObject/CustomUI/CustomButton", false, 49)]
        private static void GenerateCustomButton() {
            var selected = Selection.activeObject;
            if (!selected)
                return;
            GameObject customButton = Editor.Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Editor/CustomUI/CustomButton.prefab"
                )
            );
            PrefabUtility.DisconnectPrefabInstance(customButton);
            customButton.name = "CustomButton";
            customButton.transform.SetParent(((GameObject)selected).transform);
            customButton.transform.localPosition = Vector3.zero;
            customButton.transform.localScale = Vector3.one;
        }

        [MenuItem("GameObject/CustomUI/Image", false, 50)]
        private static void CreatImageWithoutRaycast() {
            if (Selection.activeTransform) {
                if (Selection.activeTransform.GetComponentInParent<Canvas>()) {
                    GameObject go = new GameObject("image", typeof(Image));
                    go.transform.SetParent(Selection.activeTransform);
                    Image image = go.GetComponent<Image>();
                    image.raycastTarget = false;
                    image.material = UnityEngine.Resources.Load<Material>("Material/MatUIFastDefault");
                }
            }
        }

        [MenuItem("Poukoute/Textures/Set Image Material to MatUIFastDefault")]
        private static void ReplaceImageDefaultMaterial() {
            Transform[] transForms = Selection.transforms;
            foreach (Transform trans in transForms) {
                    RevisitTransform(trans, "Material/MatUIFastDefault");
                    SavePefabChange(trans.gameObject);
            }
        }

        private static void RevisitTransform(Transform root, string materialPath) {
            Image image = root.GetComponent<Image>();
            if (image != null) {
                image.material = materialPath.CustomIsEmpty() ? null :
                    UnityEngine.Resources.Load<Material>(materialPath);
            }
            Empty4Raycast emptyRaycast = root.GetComponent<Empty4Raycast>();
            if (emptyRaycast!= null) {
                emptyRaycast.material = materialPath.CustomIsEmpty() ? null :
                    UnityEngine.Resources.Load<Material>(materialPath);
            }

            if (root.childCount > 0) {
                foreach (Transform trans in root) {
                    RevisitTransform(trans, materialPath);
                }
            }
        }

        [MenuItem("Poukoute/Textures/Revert Image Material to default")]
        private static void RevertImageDefaultMaterial() {
            Transform[] transForms = Selection.transforms;
            foreach (Transform trans in transForms) {
                RevisitTransform(trans, string.Empty);
                SavePefabChange(trans.gameObject);
            }
        }

        // Find None Image Component and replace it with pure color texture.
        [MenuItem("Poukoute/Textures/Replace None Image")]
        private static void ReplaceNoneImage() {
            Sprite defaultSprite =
                UnityEngine.Resources.Load<GameObject>("Sprites/v4ui/frm02").
                GetComponent<SpriteRenderer>().sprite;
            GameObject root = GameObject.Find("UI");
            List<Transform> childList = new List<Transform>();
            GameHelper.GetAllChildren(root.transform, childList);
            foreach (Transform child in childList) {
                Image image = child.GetComponent<Image>();
                if (image != null) {
                    if (image.sprite == null) {
                        image.sprite = defaultSprite;
                    }
                    // else if (image.sprite.name == "UI_HUD18") {
                    //    image.sprite = defaultSprite;
                    //    image.color = new Color(0, 0, 0, 0.5f);
                    //}
                    //if (image.material.name == "Sprites-Default")
                    //    image.material = null;
                }
            }
        }

        [MenuItem("Poukoute/Others/Save Select %g")]
        public static void Save() {
            GameObject[] selections = Selection.gameObjects;
            foreach (GameObject prefab in selections) {
                SavePefabChange(prefab);
            }
        }

        public static void SavePefabChange(GameObject prefab) {
            PrefabType prefabType;
            GameObject prefabRoot;
            GameObject curPrefab;
            GameObject nextPrefab;
            prefabType = PrefabUtility.GetPrefabType(prefab);
            //Debug.LogError("Save Prefab: " + prefabType);
            if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance) {
                //Debug.LogError("Save Prefab");
                prefabRoot = (PrefabUtility.GetPrefabParent(prefab) as GameObject).transform.root.gameObject;
                curPrefab = prefab;
                while (curPrefab.transform.parent != null) {
                    nextPrefab = curPrefab.transform.parent.gameObject;
                    if (PrefabUtility.GetPrefabType(nextPrefab) == PrefabType.None ||
                            prefabRoot != (PrefabUtility.GetPrefabParent(nextPrefab) as GameObject).transform.root.gameObject) {
                        break;
                    }
                    curPrefab = nextPrefab;
                }

                PrefabUtility.ReplacePrefab(curPrefab, PrefabUtility.GetPrefabParent(curPrefab), ReplacePrefabOptions.ConnectToPrefab);
            }
        }

        [MenuItem("Poukoute/Others/ChangeUdid")]
        public static void ChangeUdid() {
            //"9fe34cd2-df08-4d92-a846-3a1230822039";
            EditorWindow window = EditorWindow.GetWindow<UdidWindow>(true, "UDID", true) as EditorWindow;
        }

        [MenuItem("Poukoute/Textures/Image Animation")]
        public static void GenerateImageAnimation() {
            Object[] selections = Selection.objects;
            List<Object> selectionList = new List<Object>(selections);
            selectionList.Sort((a, b) => a.name.CompareTo(b.name));
            AnimationClip clip = new AnimationClip();
            ObjectReferenceKeyframe[] keyframe = new ObjectReferenceKeyframe[selectionList.Count];
            int count = 0;
            foreach(Object selection in selectionList) {
                keyframe[count].time = count / clip.frameRate;
                keyframe[count].value = AssetDatabase.LoadAssetAtPath<Sprite>(
                    AssetDatabase.GetAssetPath(selection)
                );
                count++;
            }
            EditorCurveBinding binding = new EditorCurveBinding();
            binding.type = typeof(Image);
            binding.path = "";
            binding.propertyName = "m_Sprite";

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframe);
            AssetDatabase.CreateAsset(
                clip, 
                AssetDatabase.GetAssetPath(selectionList[0]) + ".anim"
            );

        }
    }

    public class UdidWindow : EditorWindow {
        private string udid;

        void Awake() {
            if (PlayerPrefs.HasKey("udid")) {
                this.udid = PlayerPrefs.GetString("udid");
            } else {
                this.udid = "";
            }
        }

        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("UDID");
            udid = EditorGUILayout.TextField(udid);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Change")) {
                PlayerPrefs.SetString("udid", udid);
            }
        }

    }

}