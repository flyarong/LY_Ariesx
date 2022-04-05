using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Poukoute {

    [CustomEditor(typeof(ParticleRender))]

    public class ParticleRenderEditor : Editor {
        private ParticleRender parameter;

        void OnEnable() {
            this.parameter = (ParticleRender)target;
        }

        public override void OnInspectorGUI() {
            this.parameter.framesToCapture = EditorGUILayout.IntField(
               new GUIContent("Frames To Capture", "Frames To Capture."),
               this.parameter.framesToCapture
           );

            this.parameter.interval = EditorGUILayout.IntField(
               new GUIContent("Interval", "Texture Scale."),
               this.parameter.interval
           );

            this.parameter.frameRate = EditorGUILayout.IntField(
             new GUIContent("Frames Rate", "Rate."),
             this.parameter.frameRate
         );

            //this.parameter.blackCam =
            //    EditorGUILayout.ObjectField(new GUIContent("Black Cam", "Black Cam."), 
            //    this.parameter.blackCam, typeof(Camera),true) as Camera;
            //this.parameter.whiteCam =
            //    EditorGUILayout.ObjectField(new GUIContent("White Cam", "White Cam."),
            //    this.parameter.whiteCam, typeof(Camera), true) as Camera;


            if (GUILayout.Button("Start")) {
                this.parameter.StartGenerating();
            }
        }
    }
}