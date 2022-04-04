using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ParticleRender : MonoBehaviour {
    public string animationName = string.Empty;
    public string folder = "/Arts/Temp/";
    public int frameRate = 25;
    public int interval = 1;

    public int framesToCapture = 25;

    public Camera whiteCam;

    public Camera blackCam;

    private int videoframe = 0; 
    private float originaltimescaleTime;
    private string realFolder = string.Empty; 

    private bool done = true;

    private bool readyToCapture = false;  

    private Texture2D texb;
    private Texture2D texw;

    private Texture2D outputtex; 

    private RenderTexture blackCamRenderTexture;

    private RenderTexture whiteCamRenderTexture;
    private ParticleSystem particle;

    public void Start() {
        Time.captureFramerate = frameRate;
        this.particle = this.GetComponent<ParticleSystem>();
        originaltimescaleTime = Time.timeScale;
        readyToCapture = true;
        this.blackCam = GameObject.Find("Black Camera").GetComponent<Camera>();
        this.whiteCam = GameObject.Find("White Camera").GetComponent<Camera>();
    }

    void Update() {
        if (!done && readyToCapture && Time.frameCount % interval == 0) {
            StartCoroutine(Capture());
        }
    }

    void LateUpdate() {
    }

    public void StartGenerating() {
        this.done = false;
        this.particle.Play();
    }

    IEnumerator Capture() {
        if (videoframe < framesToCapture) {
            string filename = Application.dataPath + folder + this.name + "_" + this.videoframe + ".png";

            Time.timeScale = 0;
            yield return new WaitForEndOfFrame();

            blackCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            whiteCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

            blackCam.targetTexture = blackCamRenderTexture;
            blackCam.Render();
            RenderTexture.active = blackCamRenderTexture;
            texb = GetTex2D(true);

            whiteCam.targetTexture = whiteCamRenderTexture;
            whiteCam.Render();
            RenderTexture.active = whiteCamRenderTexture;
            texw = GetTex2D(true);


            if (texw && texb) {

                int width = Screen.width;
                int height = Screen.height;

                outputtex = new Texture2D(width, height, TextureFormat.ARGB32, false);

                for (int y = 0; y < outputtex.height; ++y) { // each row
                    for (int x = 0; x < outputtex.width; ++x) { // each column
                        float alpha;
                        Color wColor = texw.GetPixel(x, y);
                        Color bColor = texb.GetPixel(x, y);
                        alpha = Mathf.Min(
                            wColor.r - bColor.r,
                            wColor.g - bColor.g,
                            wColor.b - bColor.b
                        );
                        alpha = 1.0f - alpha;
                        Color color;
                        if (alpha == 0) {
                            color = Color.clear;
                        } else {
                            color = texb.GetPixel(x, y)/ alpha;
                        }
                        color.a = alpha;
                        outputtex.SetPixel(x, y, color);
                    }
                }

                byte[] pngShot = outputtex.EncodeToPNG();
                Debug.LogError("Save: " + filename);
                File.WriteAllBytes(filename, pngShot);

                Time.timeScale = originaltimescaleTime;
                videoframe++;
            }

        } else {
            Debug.Log("Complete! " + videoframe + " videoframes rendered (0 indexed)");
            done = true;
        }
    }

    private Texture2D GetTex2D(bool renderAll) {
        int width = Screen.width;
        int height = Screen.height;
        if (!renderAll) {
            width = width / 2;
        }

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        return tex;
    }
}