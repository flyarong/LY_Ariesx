using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class ProjectSettings : MonoBehaviour {
        [MenuItem("Poukoute/Others/Default Settings")]
        public static void DefaultSettint() {
            EditorSettings.serializationMode = SerializationMode.ForceText;
        }

        //[MenuItem("Poukoute/Textures/FullRect")]
        //public static void TextureFullRect() {
        //    string path = Application.dataPath + "/Arts/Sprites";
        //    SetTextures(path, SpriteMeshType.FullRect);
        //}

        //[MenuItem("Poukoute/Textures/Tight")]
        //public static void TextureTight() {
        //    string path = Application.dataPath + "/Arts/Sprites";
        //    SetTextures(path, SpriteMeshType.Tight);
        //}
        [MenuItem("Poukoute/Textures/Animation Sprite Pivot")]
        public static void SpritePivotBottom() {
            string path = Application.dataPath + "/Arts/Animations/Battle/SkillTest/skill01";
            SetTexturesPivot(path);
        }

        [MenuItem("Poukoute/Textures/Set Select Pivot Center")]
        public static void SpritePivotCenter() {

        }

        static void SetTexturesPivot(string directory) {
            string[] fileArray = Directory.GetFiles(directory, "*.png");
            string[] directoryArray = Directory.GetDirectories(directory);

            foreach (string value in directoryArray) {
                SetTexturesPivot(value);
            }

            foreach (string fileName in fileArray) {
                string path = "Assets" + fileName.Substring(Application.dataPath.Length);
                SetTexturePivot(path);
            }
        }

        static void SetTextures(string directory, SpriteMeshType type) {
            string[] fileArray = Directory.GetFiles(directory, "*.png");
            string[] directoryArray = Directory.GetDirectories(directory);

            foreach (string value in directoryArray) {
                SetTextures(value, type);
            }

            foreach(string fileName in fileArray) {
                string path = "Assets" + fileName.Substring(Application.dataPath.Length);
                SetTexture(path, type);     
            }
        }

        static void SetTexturePivot(string path, 
            SpriteAlignment alignment = SpriteAlignment.Center) {
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)alignment;
            importer.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path);
        }

        static void SetTexture(string path, SpriteMeshType type) {
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = type;
            importer.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path);
        }
    }

    
}
